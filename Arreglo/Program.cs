using System.ComponentModel;
using System.Reflection.PortableExecutable;

class Program{
    public static string[] Rifa(){
        string[] datos = new string[5];
        Console.Clear();
        Console.WriteLine("------Riifa de un Pollo Asado------");
        Console.WriteLine("--Registrate con los sig 5 datos:--");

        Console.WriteLine("Ingresa tu nombre:");
        datos[0] = Console.ReadLine()?? "ErrorDeRegistro";
        Console.WriteLine("Ingresa tu apellido:");
        datos[1] = Console.ReadLine()?? "ErrorDeRegistro";
        Console.WriteLine("Ingresa tu teléfono:");
        datos[2] = Console.ReadLine()?? "ErrorDeRegistro";
        Console.WriteLine("Ingresa tu edad:");
        datos[3] = Console.ReadLine()?? "ErrorDeRegistro"; 
        Console.WriteLine("Ingresa tu código postal:");
        datos[4] = Console.ReadLine()?? "ErrorDeRegistro";

        Console.WriteLine("\nDatos registrados con éxito!");
        Console.WriteLine($"Nombre: {datos[0]}");
        Console.WriteLine($"Apellido: {datos[1]}");
        Console.WriteLine($"Teléfono: {datos[2]}");
        Console.WriteLine($"Edad: {datos[3]}");
        Console.WriteLine($"Código Postal: {datos[4]}");
        
        return datos;
    }   

    public static void Main(){
        Rifa();
    }
}