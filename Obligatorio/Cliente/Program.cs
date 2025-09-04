using System;

namespace Client
{
    class Program
    {
       public static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("---- SISTEMA DE CLASES ONLINE ----");
                Console.WriteLine("\nElige una opción:");
                Console.WriteLine("1. Crear una cuenta");
                Console.WriteLine("2. Iniciar sesión");
                Console.WriteLine("3. Ver clases disponibles");
                Console.WriteLine("4. Salir");
                Console.Write("\nTu opción: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Has elegido 'Crear una cuenta'.");
                        // Aca iría la llamada al método para crear una cuenta
                        break;
                    case "2":
                        Console.WriteLine("Has elegido 'Iniciar sesión'.");
                        // Aca iría la llamada al método para crear una cuenta
                        break;
                    case "3":
                        Console.WriteLine("Has elegido 'Ver clases disponibles'.");
                        // Aca iría la llamada al método para crear una cuenta
                        break;
                    case "4":
                        Console.WriteLine("Saliendo de la aplicación...");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Presiona Enter para continuar.");
                        break;
                }

                if (!exit)
                {
                    Console.ReadLine();
                }
            }
        }
    }
}