class Program{
    public static int Suma(int a, int b){
            int resultado = a + b;
            return resultado;
    }

    public static int Resta(int a, int b){
            int resultado = a - b;
            return resultado;
    }
    public static int Multi(int a, int b){
            int resultado = a * b;
            return resultado;
    }
    public static int Divi(int a, int b){
            int resultado = a / b;
            return resultado;
    }

    public static void Main() {
        Console.WriteLine("-Calculadora Perrona-");
        Console.WriteLine("Seleccione una opcion");
        Console.WriteLine("1.- Suma");
        Console.WriteLine("2.- Resta");
        Console.WriteLine("3.- Multiplicación");
        Console.WriteLine("4.- División");
        Console.WriteLine("5.- Salir");
        string? opcion = Console.ReadLine();
        int opcionInt = int.Parse(opcion!);

        if(opcionInt == 1){
            Console.WriteLine("Ingrese un numero: ");
            string? a = Console.ReadLine();
            int num1 = int.Parse(a!);
            Console.WriteLine("Ingrese un numero: ");
            string? b = Console.ReadLine();
            int num2 = int.Parse(b!);
            Console.WriteLine("EL resultado de la suma es: "+Suma(num1,num2));
        }else if(opcion == "5"){
            Console.WriteLine("Saliendo de la calculadora...");
        }

    }    
}