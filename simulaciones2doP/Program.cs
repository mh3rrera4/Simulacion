using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace simulaciones2doP{
    public interface IEvent{
        double Timestamp { get; }
        void Execute(SimulacionState state, SimulationEngine); /*Logica del Evento*/
    }

    //Estado de la Sim q guarda todas las variables de estado del sistema
    public class SimulationState{
        public double CumulativeAdopters { get; set; } /*Total de adoptantes hasta el momento*/
        public double PotentialAdopters { get; set; }/*Personas que aún no han adoptado*/

        /*Parametros de la Simulación*/
        public double TotalMarketSize {get; private set;}
        public double InnovationCoefficent {get; private set;}
        public double ImitationCoefficent {get; private set;}

        public SimulationState(double totalMarketSize, double innovationCoefficent, double imitationCoefficent){
            TotalMarketSize = totalMarketSize;
            InnovationCoefficent = innovationCoefficent;
            ImitationCoefficent = imitationCoefficent;
            CumulativeAdopters = 0;
            PotentialAdopters = totalMarketSize;
        }
    }

    //Motor de la Sim q maneja el tiempo y la cola de eventos
    public class SimulationEngine{
        public double CurrentTime {get; private set;} /*Reloj de la sim*/
        private SortedDictionary<double, List<IEvent>> _eventQueue; /*Cola de eventos OrdBy tiempo*/
        public SimulationEngine(){
            CurrentTime = 0;
            _eventQueue = new SortedDictionary<double, List<IEvent>>();
        }
    } 

    //Inicio de la Sim
    public class SimulationStartEvent : IEvent{
        private double _dt; /*Intervalo de tiempo / actlz*/
        private double _simulationDuration; /*Duración total*/
    }

    //Evento q se ejecuta periódicamente pa actualizar el estado del sistema
    public class PeriodicUpdateEvent : IEvent{
        public double _dt; /*Intervalode tiempo pa la prox actualización*/
    }

    //Evento final de la Sim
    public class SimulationEndEvent : IEvent{

    }

    //Main
    class Program{
        static void Main(string[] args){
            
        }
    }

}

namespace Simulacion_Continua{
    
}