
using System.Linq.Expressions;

namespace Cajero;
    class Program{


        // Función para generar números aleatorios con distribución exponencial
        static double GenerarExponencial(double tasaPromedio, Random random){
            double lambda  = 1.0 / tasaPromedio;
	        return -Math.Log(1.0 - random.NextDouble()) / lambda;
        }

        static void Main(string[] args){
            // Parámetros de la simulación
            int totalClientes = 100;                     // Número total de clientes que llegan
            double tasaLlegada = 60;                // Tiempo promedio entre llegadas (sgnds convrt a min)
            double tiempoServicioPromedio = 4 * 60;     // Tiempo promedio de servicio por cliente (en segundos)
            int numCajeros = 2;                         // Número de cajeros disponibles

            // Variables para registrar métricas
            Queue<double> cola = new Queue<double>();                           // Cola de clientes
            List<double> cajeros = new List<double>(new double[numCajeros]);    // Tiempos de finalización de cajeros
            Random aleatorio = new Random();
            double tiempoActual = 0.0;
            double tiempoEsperaTotal = 0.0;

            // MOTOR: Simulación de llegada y atención de clientes
            for (int i = 0; i < totalClientes; i++){
                // Generar el tiempo de llegada del próximo cliente
                double intervaloLlegada = GenerarExponencial(tasaLlegada, aleatorio);
                tiempoActual += intervaloLlegada;

                // Encontrar el primer cajero disponible
                double tiempoMinimoLiberacion = cajeros.Min(); // Encuentra el valor mínimo (tiempo más temprano) en TODA la lista.
                int indiceCajero = cajeros.IndexOf(tiempoMinimoLiberacion); // Encuentra el índice (posición) de ese valor mínimo.

                double tiempoInicioServicio = Math.Max(cajeros[indiceCajero], tiempoActual);

                double duracionServicio = GenerarExponencial(tiempoServicioPromedio, aleatorio);

                cajeros[indiceCajero] = tiempoInicioServicio + duracionServicio;

                // Calcular el tiempo de espera de este cliente
                double tiempoEspera = tiempoInicioServicio - tiempoActual;
                tiempoEsperaTotal += tiempoEspera;
            }

            // Calcular y mostrar métricas en segundos
            double tiempoPromedioEspera = tiempoEsperaTotal / totalClientes;
            //Console.WriteLine($"Tiempo promedio de espera en segundos: {tiempoPromedioEspera}");
            
            int minutos = (int)(tiempoPromedioEspera / 60);
            int segundos = (int)(tiempoPromedioEspera % 60);
            
            Console.WriteLine($"Tiempo promedio de espera: {minutos} minutos y {segundos} segundos");
        }
    }