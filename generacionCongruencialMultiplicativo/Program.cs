class Program
{
    static void Main() {
        int seed = 17;
        int a = 5 + 8 * 2;
        int m = (int)Math.Pow(2, 5);
        int cantidad = 1;

        Console.WriteLine("Numeros Pseudoaleatorios generados:");
        for(int i =0; i<cantidad; i++){
            seed = (a* seed) % m;
            double randomValue = (double)seed / (m - 1);
            Console.WriteLine(randomValue);
        }
	
    }
}
