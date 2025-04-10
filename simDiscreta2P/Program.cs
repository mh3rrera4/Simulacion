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
        
        Console.WriteLine($"t=0.0: || Simulación Iniciada 😎. || Mercado={mercadoTotal}");
        

        /*RRUN RRUN*/
        while (tiempoActual < duracionDeSimulacion)
        {
            // Avanzamos
            tiempoActual += intervaloDeTiempo;
            
            
            double fraccionAdoptantes = adoptadoresAcumulados / mercadoTotal;
                                                                                
            adoptadoresPotenciales = mercadoTotal - adoptadoresAcumulados;

            
            if (adoptadoresPotenciales > 0)
            {
                /*Tasa (combinación de 2 factores)*/
                double adopcionInnovacion = coficienteInnovacion * adoptadoresPotenciales;
                double adopcionImitacion = coeficienteImitacion * fraccionAdoptantes * adoptadoresPotenciales;
                
                double nuevosAdoptadoresEsperados = (adopcionInnovacion + adopcionImitacion) * intervaloDeTiempo;
                int nuevosAdoptadores = (int)Math.Round(nuevosAdoptadoresEsperados);
                
                /*--Seguros antes de añadir--*/
                nuevosAdoptadores = Math.Min(nuevosAdoptadores, (int)adoptadoresPotenciales);
                nuevosAdoptadores = Math.Max(0, nuevosAdoptadores);
                
                adoptadoresAcumulados += nuevosAdoptadores;
                
                /*Recalcular pa el IF*/
                adoptadoresPotenciales = mercadoTotal - adoptadoresAcumulados;
                
                /*Resultados*/
                Console.WriteLine($"T={tiempoActual:F2}: +{nuevosAdoptadores} adoptantes -> " +
                                 $"Total={adoptadoresAcumulados:F0} " +
                                 $"({adoptadoresAcumulados / mercadoTotal * 100:F1}%)");
            }
        }
        
        /*Mensaje Final*/
        Console.WriteLine($"\n--- Simulación Finalizada en T={tiempoActual:F2} ---");
        Console.WriteLine($"Total Adoptantes Finales: {adoptadoresAcumulados:F0} / {mercadoTotal} " +
                         $"({adoptadoresAcumulados / mercadoTotal * 100:F1}%)");
        
        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }
}