using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Linq;
/*Interfaz Base*/
public interface IEvent{
    double Timestamp {get;}
    void Execute(SimulationState state, SimulationEngine engine);
}



/*Estado de la Simulacion*/
public class SimulationState
{
    //Parametros de Estado
    public double CumulativeAdopters => GameTheoryEnabled ? Companies.Sum(c => c.CumulativeAdopters) : _singleAgentAdopters;
    public double PotentialAdopters => TotalMarketSize - CumulativeAdopters;
    public double _singleAgentAdopters = 0;

    //Parametros de la Simulación:
    public double TotalMarketSize { get; }
    public double InnovationCoefficent { get; } // p
    public double BaseImitationCoeficent { get; } // q
    public GeneracionCongruencial GeneradorRandom { get; }
    public double SimulationEndTime { get; }
    public bool IsSimulationFinished { get; set; } = false;

    //Componentes de 'Teoria de Juegos'
    public bool GameTheoryEnabled { get; set; } = false;
    public List<Company> Companies { get; set; }

    public SimulationState(double totalMarketSize, double innovationCoefficent, double imitationCoeffiecnt, GeneracionCongruencial generadorRandom, double simulationEndTime)
    {
        TotalMarketSize = totalMarketSize;
        InnovationCoefficent = innovationCoefficent;
        BaseImitationCoeficent = imitationCoeffiecnt;
        GeneradorRandom = generadorRandom;
        SimulationEndTime = simulationEndTime;
        Companies = new List<Company>();
    }

    public void UpdateStandartAdopters(int newAdopters)
    {
        _singleAgentAdopters += newAdopters;
    }
}

/*Motor de la Simulación*/
public class SimulationEngine
{
    public double CurrentTime { get; private set; }
    private readonly SortedDictionary<double, List<IEvent>> _eventQueue;
    private bool _simulationStopped = false;

    public SimulationEngine()
    {
        CurrentTime = 0;
        _eventQueue = new SortedDictionary<double, List<IEvent>>();
    }
    public void ScheduleEvent(IEvent newEvent)
    {
        if (!_eventQueue.ContainsKey(newEvent.Timestamp))
        {
            _eventQueue[newEvent.Timestamp] = new List<IEvent>();
        }
        _eventQueue[newEvent.Timestamp].Add(newEvent);
    }
    public void Run(SimulationState state)
    {
        while (_eventQueue.Any() && !_simulationStopped)
        {
            var (nextEventTime, nextEvents) = _eventQueue.First();
            _eventQueue.Remove(nextEventTime);

            //Avanza el reloj d la simulación
            CurrentTime = nextEventTime;

            //Ejecutamos todos los eventos programados
            foreach (var evt in nextEvents)
            {
                evt.Execute(state, this);
            }

        }
    }
    public void StopSimulation()
    {
        _simulationStopped = true;
        _eventQueue.Clear();
    }
}



/*-----  TEORIA DE JUEGOS  -----*/
/*Estrategias de Marketing/Precio que la empresa puede adoptar*/
public enum MarketingStrategy {
    LowPrice,   //Agresiva = Atrae mas imitadores
    MediumPrice,    //Neutral
    HighPrice   //Pasiva = atrae menos imitadores
}

/*Representación de un Jugador (Empresa)*/
public class Company {
    public string Name { get; }
    public MarketingStrategy CurrentStrategy { get; private set; }
    public double CumulativeAdopters { get; set; }

    // Matriz de Payoffs: define el multiplicador del coeficiente de imitación.
    // La clave es la estrategia propia, el valor es el multiplicador para 'q'.
    private readonly Dictionary<MarketingStrategy, double> _payoffMatrix = new Dictionary<MarketingStrategy, double>
    {
        {MarketingStrategy.LowPrice, 1.5},  // = aumenta el boca a boca
        {MarketingStrategy.MediumPrice, 1.0},
        {MarketingStrategy.HighPrice, 0.7}  // = reduce el boca a boca
    };

    public Company(string name)
    {
        Name = name;
        CurrentStrategy = MarketingStrategy.MediumPrice;
    }


    /// La empresa elige su próxima estrategia.
    /// Lógica simple: si el competidor > 55% del mercado, adopta una estrategia de precios bajos.
    /// De lo contrario, elige una estrategia de precios medios para maximizar el margen.
    public void ChooseStrategy(SimulationState state)
    {
        var opponent = state.Companies.First(c => c.Name != this.Name);
        double totalAdopters = state.CumulativeAdopters;

        if (totalAdopters > 0)
        {
            double opponentMarketShare = state.CumulativeAdopters / totalAdopters;
            if (opponentMarketShare > 0.55)
            {
                CurrentStrategy = MarketingStrategy.LowPrice;
            }
            else
            {
                CurrentStrategy = MarketingStrategy.MediumPrice;
            }
        }
        else
        {
            CurrentStrategy = MarketingStrategy.MediumPrice;
        }
    }

    //Obtener el coeficiente de imitación efectivo basado en la estrategia escogida.
    public double GetEffectiveImitationCoefficent2(double baseCoefficent) => baseCoefficent * _payoffMatrix[CurrentStrategy];
}

/*Evento para la toma de decisiones*/
public class GameEvent : IEvent {
    private readonly double _decisionInterval;
    public double Timestamp { get; private set; }

    public GameEvent(double timestamp, double decisionInterval)
    {
        Timestamp = timestamp;
        _decisionInterval = decisionInterval;
    }

    public void Execute(SimulationState state, SimulationEngine engine)
    {
        if (state.IsSimulationFinished) return;
        Console.WriteLine($"\n--- ♟️ Ronda de Decisión Estratégica en S={engine.CurrentTime:F0} ---");

        foreach (var company in state.Companies)
        {
            company.ChooseStrategy(state);
            Console.WriteLine($"   -> {company.Name} elige la estrategia: {company.CurrentStrategy}");
        }

        //Programar sig. evento
        double nextEventTime = engine.CurrentTime + _decisionInterval;
        if (nextEventTime <= state.SimulationEndTime)
        {
            engine.ScheduleEvent(new GameEvent(nextEventTime, _decisionInterval));
        }
    }
}



/*Evento de Inicio*/
public class SimulationStartEvent : IEvent {
    private readonly double _dt; //Intervalo d tiempo sobre actualizaciones
    private readonly double _simulatonDuration; //Duración total
    private readonly double _gameDecisionInterval;

    //Constructor
    public SimulationStartEvent(double dt, double simulationDuration, double gameDecisionInterval = 4.0)
    {
        _dt = dt;
        _simulatonDuration = simulationDuration;
        _gameDecisionInterval = gameDecisionInterval;
    }

    public double Timestamp => 0; //El evento ocurre al inicio (tiempo 0)

    public void Execute(SimulationState state, SimulationEngine engine)
    {
        //Inicialización del estado
        Console.WriteLine($"T=0.0: Simulacion Iniciada 😎. Mercado={state.TotalMarketSize} || Duración={state.SimulationEndTime} semanas");

        //TdJ ¿?
        if (state.GameTheoryEnabled)
        {
            Console.WriteLine("Modo 'Teoria de Juegos' ACTIVADO");
            engine.ScheduleEvent(new GameEvent(0, _gameDecisionInterval));
        }

        //Programa el primer evento d actualización periódica
        engine.ScheduleEvent(new PeriodicUpdateEvent(engine.CurrentTime + _dt, _dt));
        //Programa el evento d finalización
        engine.ScheduleEvent(new SimulationEndEvent(_simulatonDuration));
    }
}

/*Evento de Actualización Periódica*/
public class PeriodicUpdateEvent : IEvent {
    private double _dt; //Intervalo d tiempo p/la prox actualización

    public PeriodicUpdateEvent(double timestamp, double dt)
    {
        Timestamp = timestamp;
        _dt = dt;
    }
    public double Timestamp { get; }
    public void Execute(SimulationState state, SimulationEngine engine)
    {
        if (state.IsSimulationFinished || state.PotentialAdopters <= 0)
        {
            return;
        }
        Console.WriteLine($"\nS={engine.CurrentTime}:");

        if (state.GameTheoryEnabled)
        {
            ExecuteGameTheoryUpdate(state);
        }
        else
        {
            ExecuteStandardUpdate(state);
        }

        double nextEventTime = engine.CurrentTime + _dt;
        if (!state.IsSimulationFinished && nextEventTime <= state.SimulationEndTime)
        {
            //Programar siguiente actualización SÍ no hemos terminado y hay adoptantes potenciales
            engine.ScheduleEvent(new PeriodicUpdateEvent(engine.CurrentTime + _dt, _dt));
        }
    }

    private void ExecuteStandardUpdate(SimulationState state)
    {
        //Calcular la tasa de adopción
        double adoptersFraction = state.CumulativeAdopters / state.TotalMarketSize;
        double innovationEffect = state.InnovationCoefficent * state.PotentialAdopters;
        double imitationEffect = state.BaseImitationCoeficent * adoptersFraction * state.PotentialAdopters;
        double expectedNewAdopters = (innovationEffect + imitationEffect) * _dt;

        //Determinar nuevos adoptantes reales
        int actualNewAdopters = generadorExponencialPoisson.poisson(expectedNewAdopters, state.GeneradorRandom);

        //¿Excede límites?
        actualNewAdopters = Math.Min(actualNewAdopters, (int)state.PotentialAdopters);
        actualNewAdopters = Math.Max(0, actualNewAdopters);

        state.UpdateStandartAdopters(actualNewAdopters);

        //Mostrar resultados
        Console.WriteLine($"   +{actualNewAdopters} adoptantes -> Total={state.CumulativeAdopters:F0} ({state.CumulativeAdopters / state.TotalMarketSize * 100:F1}%)");
    }

    private void ExecuteGameTheoryUpdate(SimulationState state)
    {
        int totalNewAdoptersThisStep = 0;
        double totalAdoptersAtStartOfStep = state.CumulativeAdopters;

        // El efecto de innovación se divide entre las empresas
        double innovationEffectPerCompany = (state.InnovationCoefficent * state.PotentialAdopters) / state.Companies.Count;

        foreach (var company in state.Companies)
        {
            // El efecto de imitación depende de la cuota de mercado de CADA empresa y su estrategia
            double companyMarketFraction = totalAdoptersAtStartOfStep > 0 ? company.CumulativeAdopters / totalAdoptersAtStartOfStep : 0;
            double effectiveImitationCoeff = company.GetEffectiveImitationCoefficent2(state.BaseImitationCoeficent);
            double imitationEffect = effectiveImitationCoeff * companyMarketFraction * state.PotentialAdopters;

            double expectedNewAdopters = (innovationEffectPerCompany + imitationEffect) * _dt;
            int actualNewAdopters = generadorExponencialPoisson.poisson(expectedNewAdopters, state.GeneradorRandom);

            // Se limita para no exceder los adoptantes potenciales restantes en este paso
            actualNewAdopters = Math.Min(actualNewAdopters, (int)(state.PotentialAdopters - totalNewAdoptersThisStep));
            actualNewAdopters = Math.Max(0, actualNewAdopters);

            company.CumulativeAdopters += actualNewAdopters;
            totalNewAdoptersThisStep += actualNewAdopters;
        }

        foreach (var company in state.Companies)
        {
            // Este cálculo es complejo, se simplifica la salida de texto
            Console.WriteLine($"   -> {company.Name}: Total={company.CumulativeAdopters:F0} (Estrategia: {company.CurrentStrategy})");
        }

        Console.WriteLine($"   Total Acumulado = {state.CumulativeAdopters:F0} / {state.TotalMarketSize} ({state.CumulativeAdopters / state.TotalMarketSize * 100:F1}%)");
    }
}

/*Evento de Finalización*/
public class SimulationEndEvent : IEvent {
    public SimulationEndEvent(double timestamp)
    {
        Timestamp = timestamp;
    }
    public double Timestamp { get; }

    public void Execute(SimulationState state, SimulationEngine engine)
    {
        if (!state.IsSimulationFinished) return;
        Console.WriteLine($"\n--- Simulación Finalizada 🎉 en T={engine.CurrentTime:F2} ---");
        Console.WriteLine($"Total Adoptantes Finales: {state.CumulativeAdopters:F0} / {state.TotalMarketSize}");

        if (state.GameTheoryEnabled)
        {
            Console.WriteLine("\n--- Resultados por Empresa ---");
            foreach (var company in state.Companies)
            {
                double marketShare = state.CumulativeAdopters > 0 ? (company.CumulativeAdopters / state.CumulativeAdopters) * 100 : 0; 
                Console.WriteLine($"   - {company.Name}: {company.CumulativeAdopters:F0} adoptantes ({marketShare:F1}% de cuota)");  
            }
        }

        state.IsSimulationFinished = true;
        engine.StopSimulation();
    }
}



/*Función de Generación Exponencial (Poisson)*/
public class generadorExponencialPoisson{
    public static int poisson(double lambda, GeneracionCongruencial random)
    {
        if (lambda <= 0) return 0;
        double L = Math.Exp(-lambda);
        double p = 1.0;
        int k = 0;

        do
        {
            k++;
            double randVal = random.NextDouble();
            if (randVal == 0.0 && lambda > 0)
            {
                randVal = double.Epsilon;
            }
            p *= randVal;
            if (p == 0.0 && L > 0)
            {
                break;
            }
        } while (p > L);
        return k - 1;
    }
}

/*Función de Generación Congruencial (Números Aleatorios)*/
public class GeneracionCongruencial{
    private long _seed;
    private readonly long _a;
    private readonly long _c;
    private readonly long _m;

    public GeneracionCongruencial(long seed, long a = 1103515245, long c = 12345, long m = 2147483648)
    {
        _seed = seed;
        _a = a;
        _c = c;
        _m = m;
    }

    //Genera el sig núm pseudo-aleatorio
    public long NextLong()
    {
        _seed = (_a * _seed + _c) % _m;
        return _seed;
    }

    //Genera un núm aleatorio (0-1)
    public double NextDouble()
    {
        return (double)NextLong() / (_m + 1.0);
    }

    //Genera un int aleatorio entre min y max
    public int Next(int min, int max)
    {
        return min + (int)(NextDouble() * (max - min));
    }
}

class Program
{
    static void Main(string[] args)
    {
        //Parametros de entrada
        double totalMarketSize = 10000;
        double innovationCoefficent = 0.01;    //Coeficiente p (innovación)
        double imitationCoeffiecnt = 0.1;  //Coeficiente q (imitación)
        double simulationDuration = 52;        //Duración en Semanas
        double timeStep = 1.0;             //Intervalo de actualización (1 semana)
        double gameDecisionInterval = 4.0;      // C/cuanto se decide la newest estrategia 

        Console.WriteLine("'Teoría de Colas' NO disponible para esta simulación.");
        Console.Write("¿Deseas implementar 'Teoría de Juegos' en esta Simulación? (Y/N): ");
        string userInput = Console.ReadLine()?.Trim().ToUpper();

        bool useGameTheory = userInput == "Y";

        // ---Configuración---
            //Generador de números aleatorios
            GeneracionCongruencial generador = new GeneracionCongruencial(DateTime.Now.Ticks);
            
            //Estado y motor de la simulación
            SimulationState state = new SimulationState(
                totalMarketSize,
                innovationCoefficent,
                imitationCoeffiecnt,
                generador,
                simulationEndTime: simulationDuration
            );
            SimulationEngine engine = new SimulationEngine();

        if (useGameTheory)
        {
            state.GameTheoryEnabled = true;
            state.Companies.Add(new Company("Empresa-A"));
            state.Companies.Add(new Company("Empresa-B"));
        }

        //Programamos el evento inicial
        engine.ScheduleEvent(new SimulationStartEvent(timeStep, simulationDuration,gameDecisionInterval));
        engine.Run(state);

        Console.WriteLine("\n--Presiona cualquier tecla para salir--");
        Console.ReadKey();
    }
}
