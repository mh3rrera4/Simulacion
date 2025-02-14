class Program {
    struct Persona{
        public string Nombre;
        public int Edad;
    }
    static void Main() {
        /*
        //Tipos numéricos enteros
            byte tipo1 = 255; //8 bits (0 a 255)
            sbyte tipo2 = -128; 
            short tipo3 = -32768;
            ushort tipo4 = 65535;
            int unInt = -2147483648;
            uint tipo6 = 4294967275;
            long tipo7 = -9223372036854775808;
            ulong tipo8 = 184467;

        // Tipos de punto flotante:
            float unFloat = 3.1416f;
            double unDouble = 3.14159265359;
            decimal unDecimal = 3.14159265m;

        //Tipo Boolean
            bool esVerdadero = true; //true o false

        // Tipo Carácter
            char unChar = 'A'; //16 bits (Unicode)

        //Tipo cadena
            string unaCadena = "Hola, C#"; //Cadena de Texto

        //Arreglos
            int[] unArreglo = { 1, 2, 3, 4, 5};

        //Mostrar Valores
            Console.WriteLine($"int; {unInt}");
        */
        
        // -Convertir un String a Integer-
            Console.WriteLine("Dame tu edad");
            string? strEdad = Console.ReadLine();

            int edad = int.Parse(strEdad); /*Ahí marca la adver pq el programa es sencillo y asumimos que el
                                            pnche usuario pndj nuunca escribira algo q no sea un número, pero
                                            el compilador no. Si le agregas un '!' (strEdad!) es para decrile al
                                            compilador que ntp todo chido, no pasará eso, calale pa q veas.
                                            Mini dato curioso */
            Console.WriteLine($"La edad que me dijiste es: {strEdad}"); 

                                            /* Otra solución más firme pa eso sería agg al final '?? "0"', el
                                            '??' se usa para agg excepciones, así que si pone otra mamada que
                                            no sea un número, marcará 0 ;) */
                                            

        /* -Convertir un String a Integer con excepción-
            Console.WriteLine("Dame tu edad");
        
            string? strEdad = Console.ReadLine(); // Puede ser null
            int edad = 0;

            if (!string.IsNullOrEmpty(strEdad) && int.TryParse(strEdad, out edad)) {
                Console.WriteLine($"La edad que me dijiste es: {edad}");
            } else {
                Console.WriteLine("Error: No ingresaste un número válido.");
            }
        */    
    }
}
