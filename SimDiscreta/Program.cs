using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
/*Interfaz Base*/
public interface IEvent{
    double Timestamp {get;}
    void Execute(SimulationState state, SimulationEngine engine);
}

/*Estado de la Simulacion*/
public class SimulationState{
    public double CumulativeAdopters { get; set; }
    public double PotentialAdopters { get; set; }

    //Parametros de la Simulación:
    public double TotalMarketSize { get; private set; }
    public double InnovationCoefficent { get; private set; }
    public double ImitationCoeffiecnt { get; private set; }

    public SimulationState(double totalMarketSize, double innovationCoefficent, double imitationCoeffiecnt) {
        TotalMarketSize = totalMarketSize;
        InnovationCoefficent = innovationCoefficent;
        ImitationCoeffiecnt = imitationCoeffiecnt;
        CumulativeAdopters = 0;
        PotentialAdopters = totalMarketSize;
    }
}

/*Motor de la Simulación*/
public class SimulationEngine{
    public double CurrentTime { get; private set; }
    public SortedDictionary<double, List<IEvent>> _eventQueue;

    public SimulationEngine() {
        CurrentTime = 0;
        _eventQueue = new SortedDictionary<double, List<IEvent>>();
    }
    public void ScheduleEvent(IEvent newEvent) {
        if (!_eventQueue.ContainsKey(newEvent.Timestamp)) {
            _eventQueue[newEvent.Timestamp] = new List<IEvent>();
        }
        _eventQueue[newEvent.Timestamp].Add(newEvent);
    }
    public void Run(SimulationState state) {
        while (_eventQueue.Count > 0) {
            double nextEventTime = _eventQueue.Keys.First();
            List<IEvent> nextEvents = _eventQueue[nextEventTime];
            _eventQueue.Remove(nextEventTime);

            //Avanza el reloj d la simulación
            CurrentTime = nextEventTime;

            //Ejecutamos todos los eventos programados
            foreach (var evt in nextEvents) {
                evt.Execute(state, this);
            }
        }
    }
}

/*Evento de Inicio*/
public class SimulationStartEvent : IEvent{
    private double _dt; //Intervalo d tiempo sobre actualizaciones
    private double _simulatonDuration; //Duración total

    //Constructor
    public SimulationStartEvent(double dt, double simulationDuration)
    {
        _dt = dt;
        _simulatonDuration = simulationDuration;
    }

    public double Timestamp => 0; //El evento ocurre al inicio (tiempo 0)

    public void Execute(SimulationState state, SimulationEngine engine)
    {
        //Inicialización del estado
        state.CumulativeAdopters = 0;
        state.PotentialAdopters = state.TotalMarketSize;
        Console.WriteLine($"T=0.0: Simulacion Iniciada 😎. Mercado={state.TotalMarketSize}");

        //Programa el primer evento d actualización periódica
        engine.ScheduleEvent(new PeriodicUpdateEvent(engine.CurrentTime + _dt, _dt));
        //Programa el evento d finalización
        engine.ScheduleEvent(new SimulationEndEvent(_simulatonDuration));
    }
}

/*Evento de Actualización Periódica*/
public class PeriodicUpdateEvent : IEvent{
    private double _dt; //Intervalo d tiempo p/la prox actualización

    public PeriodicUpdateEvent(double timestamp, double dt)
    {
        Timestamp = timestamp;
        _dt = dt;
    }
    public double Timestamp { get; private set; }
    public void Execute(SimulationState state, SimulationEngine engine)
    {
        //Calcular la tasa de adopción basada en el modelo Bass
        double adoptersFraction = state.CumulativeAdopters / state.TotalMarketSize;
        double potentialAdopters = state.TotalMarketSize - state.CumulativeAdopters;

        //Si ya no hay potenciales adoptantes, nomas programar la sig actualizacion
        if (potentialAdopters <= 0)
        {
            if (engine.CurrentTime + _dt <= Timestamp)
            {
                engine.ScheduleEvent(new PeriodicUpdateEvent(engine.CurrentTime + _dt, _dt));
            }
            return;
        }
        //Efectos de innovación e imtiación
        double innovationEffect = state.InnovationCoefficent * potentialAdopters;
        double imitationEffect = state.ImitationCoeffiecnt * adoptersFraction * potentialAdopters;
        double expectedNewAdopters = (innovationEffect + imitationEffect) * _dt;

        //Determinar nuevos adoptantes reales
        int actualNewAdopters = (int)Math.Round(expectedNewAdopters);

        //¿Excede límites?
        actualNewAdopters = Math.Min(actualNewAdopters, (int)potentialAdopters);
        actualNewAdopters = Math.Max(0, actualNewAdopters);

        //Actualizar estado
        state.CumulativeAdopters += actualNewAdopters;
        state.PotentialAdopters = state.TotalMarketSize - state.CumulativeAdopters;

        //Mostrar resultados
        Console.WriteLine($"T={engine.CurrentTime:F2}: +{actualNewAdopters} adoptantes ->" +
                        $"Total={state.CumulativeAdopters:F0} ({state.CumulativeAdopters / state.TotalMarketSize * 100:F1}%)");

        //Programar siguiente actualización SÍ no hemos terminado
        if (engine.CurrentTime + _dt <= Timestamp)
        {
            engine.ScheduleEvent(new PeriodicUpdateEvent(engine.CurrentTime + _dt, _dt));
        }
    }
}

/*Evento de Finalización*/
public class SimulationEndEvent : IEvent{
    public SimulationEndEvent(double timestamp)
    {
        Timestamp = timestamp;
    }
    public double Timestamp { get; private set; }

    public void Execute(SimulationState state, SimulationEngine engine)
    {
        Console.WriteLine($"\n--- Simulación Finalizada 🎉 en T={engine.CurrentTime:F2} ---");
        Console.WriteLine($"Total Adoptantes Finales: {state.CumulativeAdopters:F0} / {state.TotalMarketSize}" +
                          $"{state.CumulativeAdopters / state.TotalMarketSize * 100}%");
    }
}


/*Función de Generación Exponencial (POISSON)*/
public class generadorExponencialPoisson
{
    public static int poisson(double lambda, Random random)
    {
        double L = Math.Exp(-lambda);
        double p = 1.0;
        int k = 0;

        do
        {
            k++;
            p *= random.NextDouble();
        } while (p > L);
        return k - 1;
    }
}

public class GeneracionCongruencial{
    private long _seed;
    private readonly long _a;
    private readonly long _c;
    private readonly long _m;

    public GeneracionCongruencial(long seed, long a = 1103515245, long c = 12345, long m = 2147483648){
        _seed = seed;
        _a = a;
        _c = c;
        _m = m;
    }

    //Genera el sig núm pseudo-aleatorio
    public long NextLong(){
        _seed = (_a*_seed+_c) % _m;
        return _seed;
    }

    //Genera un núm aleatorio (0-1)
    public double NextDouble(){
        return (double)NextLong() / _m;
    }

    //Genera un int aleatorio entre min y max
    public int Next(int min, int max){
        return min + (int)(NextDouble() * (max-min));
    }
}

class Program
{
    static void Main(string[] args)
    {
        //Parametros de entrada
        double totalMarketSize = 10000;    //
        double innovationCoefficent = 0.01;    //Coeficiente p (innovación)
        double imitationCoeffiecnt = 0.1;  //Coeficiente q (imitación)
        double simulationDuration = 52;        //Duración en Semanas
        double timeStep = 1.0;             //Intervalo de actualización (1 semana)

        //Creamos el estado y motor de la simulación
        SimulationState state = new SimulationState(
            totalMarketSize,
            innovationCoefficent,
            imitationCoeffiecnt
        );
        SimulationEngine engine = new SimulationEngine();

        //Programamos el evento inicial
        engine.ScheduleEvent(new SimulationStartEvent(timeStep, simulationDuration));
        engine.Run(state);

        Console.WriteLine("\n--Presiona cualquier tecla para salir--");
        Console.ReadKey();
    }
}