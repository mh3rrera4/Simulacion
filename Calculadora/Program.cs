class Program{
    public static int Casio(int a, int b, char operacion) {
        switch (operacion) {
            case '+':
                return a + b;
            case '-':
                return a - b;
            case '*':
                return a * b;
            case '/':
                if (b != 0) {
                    return a / b;
                } else {
                    Console.WriteLine("Error: No se puede dividir entre cero.");
                    return 0;
                }
            default:
                Console.WriteLine("Operación no válida."); //Por si el usuario idioto no pone un numero
                return 0;
        }
    }

    public static void Main() {
        Console.WriteLine("---Calculadora Perrona---");
        Console.WriteLine("Ingresa el primer numero");
        string? a = Console.ReadLine();
        int num1 = int.Parse(a!);
        Console.WriteLine("Ingrese el segundo numero: ");
        string? b = Console.ReadLine();
        int num2 = int.Parse(b!);

        Console.WriteLine("---Escribe el simbolo de la operacion que quieres hacer---");
        Console.WriteLine("'+' - Suma");
        Console.WriteLine("'-' - Resta");
        Console.WriteLine("'*' - Multiplicación");
        Console.WriteLine("'/' - División");
        char operacion = Console.ReadLine()![0]; // Obtenemos el primer carácter

        int resultado = Casio(num1,num2,operacion);

        Console.WriteLine($"El resultado es: {resultado}.");
        Console.WriteLine("Así nomas quedo.");
    }    
}