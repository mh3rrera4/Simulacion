//namespace = modulo ; cuando pidan un modulo, es esa madre.
//instanciar un objeto

using System;
using System.Collections;
using System.Reflection.Metadata;
                                    //Esta madre es el que separará las clases.
namespace ciclosycondiciones;       //<-- Ya que todas las clases iran en un solo archivo.
    class Program{
        public static void Main(){
            /*-CICLOS ASÍ INFINITOS ALV
            Console.WriteLine("Ciclo for:");
                for(int i=0; i<5;i++){
                    Console.WriteLine("Miguel Herrera Chngon");
                    i++;
                }

                Console.WriteLine("Ciclo while:");
                int w=0;
                while(w<5){
                    Console.WriteLine("Iteración: "+w);
                    w++;
                }
            */
            /*-CONDICIONALES-*/
            /*Programa que use todo alv*/
            int chilaquiles = 0;
            bool separador = true;
            while(separador){
                Console.WriteLine("Ingrese una opcion");
                    Console.WriteLine("1 - Aprender tu nombre");
                    Console.WriteLine("2 - Juego");
                    Console.WriteLine("3 - Salir");
                string? x = Console.ReadLine();
                int opc = int.Parse(x!);
                switch(opc){
                    case 1:
                        int opc1;
                        Console.WriteLine("Opcion 1 elegida");
                        Console.WriteLine("--Sistema para aprenderte tu nombre");
                        Console.WriteLine("Ingresa tu nombre:");
                        for(int i=0; i<5;i++){
                            string? nombre = Console.ReadLine();
                            i++;
                        }
                        Console.WriteLine("Para que te lo aprendas.");
                    break;
                    case 2:
                    Console.WriteLine("Opcion 2 elegida");
                    Console.WriteLine("--Piedra Papel o Tijera--");
                    Console.WriteLine("--Juega contra la IA, escoge una:--");
                    Console.WriteLine("1 - Piedra");
                    Console.WriteLine("2 - Papel");
                    Console.WriteLine("3 - Tijera");
                    string? nom = Console.ReadLine();
                    int num2 = int.Parse(nom!);
                    if(num2 == 1){
                        Console.WriteLine("Escogiste Piedra");
                        Console.WriteLine("La AI escogio Papel");
                        Console.WriteLine("\n Pelaste. La AI gano.");
                    }else if(num2 == 2){
                        Console.WriteLine("Escogiste Papel");
                        Console.WriteLine("La AI escogio Piedra");
                        Console.WriteLine("\n Eres una riata. Ganaste");
                    }else if(num2 == 3){
                        Console.WriteLine("Escogiste Tijeras");
                        Console.WriteLine("La AI escogio Piedra");
                        Console.WriteLine("\n Pelaste. La AI gano.");
                    }
                    else{
                        Console.WriteLine("No escogiste una opción valida tonto");
                    }
                    break;
                    case 3:
                    separador = false;
                    break;
                }
            } Console.WriteLine("Adios Perrillo");
        }
    }