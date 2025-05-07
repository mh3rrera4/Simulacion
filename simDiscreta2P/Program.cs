using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        /*Parámetros Base*/
        double mercadoTotal = 10000; /*t*/
        double coficienteInnovacion = 0.01; /* p (influencia externa)*/ /*--1%--*/
        double coeficienteImitacion = 0.1; /*  q (       =    interna)*//*--10%--*/
        double duracionDeSimulacion = 52;
        double intervaloDeTiempo = 1.0; 
        
        /*Variables de estado/momento*/
        double adoptadoresAcumulados = 0; 
        double adoptadoresPotenciales = mercadoTotal;
        
        double tiempoActual = 0;
        
        /* Generador Aleatorio*/
        Random aleatorio = new Random();
        
        Console.WriteLine($"t=0.0: || Simulación Iniciada 😎. || Mercado={mercadoTotal}");
        

        /*RRUN RRUN*/
        while (tiempoActual < duracionDeSimulacion)
        {
            /*Avanzamos*/
            double tiempoHastaProximoEvento = GenerarExponencial(intervaloDeTiempo, aleatorio);
            tiempoActual += tiempoHastaProximoEvento;
            
            if (tiempoActual > duracionDeSimulacion)
                break;
            
            double fraccionAdoptantes = adoptadoresAcumulados / mercadoTotal;
                                                                                
            adoptadoresPotenciales = mercadoTotal - adoptadoresAcumulados;

            
            if (adoptadoresPotenciales > 0)
            {
                /*Tasa (combinación de 2 factores)*/
                double adopcionInnovacion = coficienteInnovacion * adoptadoresPotenciales;
                double adopcionImitacion = coeficienteImitacion * fraccionAdoptantes * adoptadoresPotenciales;
                
                double tasaAdopcionTotal = adopcionInnovacion + adopcionImitacion;
                
                int nuevosAdoptadores = 1; /*Cada evento de adopcion, al menos una persona adopta*/
                

                // Con probabilidad proporcional a la tasa, podríamos tener más adoptantes en el mismo evento
                if (tasaAdopcionTotal > 1.0){
                    /*Representamos multiples adopciones simultaneas*/

                        /*Cuando la tasa de adopcion es alta es razonable que varias personas adopten 
                        el producto casi al mismo tiempo (por ejemplo, un grupo de amigos que compra un 
                        nuevo gadget el mismo día). En lugar de modelar cada una de estas adopciones como 
                        eventos separados, los agrupamos en un solo evento con múltiples adoptantes.*/

                    nuevosAdoptadores = (int)Math.Round(tasaAdopcionTotal * tiempoHastaProximoEvento);
                        
                }                                      
                

                /*--Seguros antes de añadir--*/
                nuevosAdoptadores = Math.Min(nuevosAdoptadores, (int)adoptadoresPotenciales);
                nuevosAdoptadores = Math.Max(1, nuevosAdoptadores); // Al menos 1 adoptante por evento
                
                adoptadoresAcumulados += nuevosAdoptadores;
                
                /*Recalcular pa el IF*/
                adoptadoresPotenciales = mercadoTotal - adoptadoresAcumulados;
                
                /*Resultados*/
                Console.WriteLine($"T={tiempoActual:F2}: +{nuevosAdoptadores} adoptantes -> " +
                                 $"Total={adoptadoresAcumulados:F0} " +
                                 $"({adoptadoresAcumulados / mercadoTotal * 100:F1}%)");
            }
            else
            {
                // Si ya no quedan adoptantes potenciales, terminamos
                break;
            }
        }
        
        /*Mensaje Final*/
        Console.WriteLine($"\n--- Simulación Finalizada en T={tiempoActual:F2} ---");
        Console.WriteLine($"Total Adoptantes Finales: {adoptadoresAcumulados:F0} / {mercadoTotal} " +
                         $"({adoptadoresAcumulados / mercadoTotal * 100:F1}%)");
        
        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }
    static double GenerarExponencial(double tasaPromedio, Random random){
        double lambda = 1.0 / tasaPromedio;
        return -Math.Log(1.0 - random.NextDouble()) / lambda;
    }
}