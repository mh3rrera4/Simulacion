namespace Cajero;
    class Program{
        static void Main(string[] args){
            // Parámetros de la simulación
            int totalClientes = 15;                     // Número total de clientes que llegan
            double tasaLlegada = 5 * 60;                // Tiempo promedio entre llegadas (en segundos)
            double tiempoServicioPromedio = 2 * 60;     // Tiempo promedio de servicio por cliente (en segundos)
            int numCajeros = 3;                         // Número de cajeros disponibles

            // Variables para registrar métricas
            Queue<double> cola = new Queue<double>();                           // Cola de clientes
            List<double> cajeros = new List<double>(new double[numCajeros]);    // Tiempos de finalización de cajeros
            Random aleatorio = new Random();
            double tiempoActual = 0.0;
            double tiempoEsperaTotal = 0.0;

            // MOTOR: Simulación de llegada y atención de clientes
            for (int i = 0; i < totalClientes; i++){
                // Generar el tiempo de llegada del próximo cliente
                double tiempoLlegada = tiempoActual + GenerarExponencial(tasaLlegada, aleatorio);

                // Encontrar el primer cajero disponible
                int indiceCajero = cajeros.IndexOf(Math.Min(cajeros[0], cajeros[1]));
                double tiempoInicioServicio = Math.Max(cajeros[indiceCajero], tiempoLlegada);
                double tiempoServicio = tiempoInicioServicio + GenerarExponencial(tiempoServicioPromedio, aleatorio);
                cajeros[indiceCajero] = tiempoServicio;

                // Calcular el tiempo de espera de este cliente
                double tiempoEspera = tiempoInicioServicio - tiempoLlegada;
                tiempoEsperaTotal += tiempoEspera;

                // Actualizar el tiempo actual
                tiempoActual = tiempoLlegada;
            }

            // Calcular y mostrar métricas en segundos
            double tiempoPromedioEspera = tiempoEsperaTotal / totalClientes;
            //Console.WriteLine($"Tiempo promedio de espera en segundos: {tiempoPromedioEspera}");
            
            int minutos = (int)(tiempoPromedioEspera / 60);
            int segundos = (int)(tiempoPromedioEspera % 60);
            
            Console.WriteLine($"Tiempo promedio de espera: {minutos} minutos y {segundos} segundos");
        }

        // Función para generar números aleatorios con distribución exponencial
        static double GenerarExponencial(double lambda, Random random){
	    return -Math.Log(1.0 - random.NextDouble()) / lambda;
}
    }
