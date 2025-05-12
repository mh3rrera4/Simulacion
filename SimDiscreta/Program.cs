using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

public interface IEvent{
    double Timestamp {get;}
    void Execute(SimulationState state, SimulationEngine engine);
}

public class SimulationState{
    public double CumulativeAdopters {get; set;}
    public double PotentialAdopters {get; set;}

    //Parametros de la Simulación:
    public double TotalMarketSize {get; private set;}
    public double InnovationCoefficent {get; private set;}
    public double ImitationCoeffiecnt {get; private set;}

    public SimulationState(double totalMarketSize, double innovationCoefficent, double imitationCoeffiecnt){
        TotalMarketSize = totalMarketSize;
        InnovationCoefficent = innovationCoefficent;
        ImitationCoeffiecnt = imitationCoeffiecnt;
        CumulativeAdopters = 0;
        PotentialAdopters = totalMarketSize;
    }
}

public class SimulationEngine{
   public double CurrentTime {get; private set;}
   public SortedDictionary<double, List<IEvent>> _eventQueue;

   public SimulationEngine(){
        CurrentTime = 0;
        _eventQueue = new SortedDictionary<double, List<IEvent>>();
   }
   public void ScheduleEvent(IEvent newEvent){
        if(!_eventQueue.ContainsKey(newEvent.Timestamp)){
            _eventQueue[newEvent.Timestamp] = new List<IEvent>();
        }
        _eventQueue[newEvent.Timestamp].Add(newEvent); 
   }
   public void Run(SimulationState state){
        while(_eventQueue.Count > 0){
            double nextEventTime = _eventQueue.Keys.First();
            List<IEvent> nextEvents = _eventQueue[nextEventTime];
            _eventQueue.Remove(nextEventTime);

            //Avanza el reloj d la simulación
            CurrentTime = nextEventTime;

            //Ejecutamos todos los eventos programados
            foreach(var evt in nextEvents){
                evt.Execute(state, this);
            }
        }
   }
}

