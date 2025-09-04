using System;
using System.Net.Sockets;
using Comunicacion;

namespace Cliente
{
    class Program
    {
        static void Main(string[] args)
        {
            ClienteApp app = new ClienteApp();
            app.Conectar();
            app.Menu();
        }
    }

    public class ClienteApp
    {
        private TcpClient cliente;
        private NetworkStream stream;

        public void Conectar()
        {
            try
            {
                cliente = new TcpClient("127.0.0.1", 5000);
                stream = cliente.GetStream();
                Console.WriteLine("Conectado al servidor.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error conectando al servidor: {ex.Message}");
                Environment.Exit(1);
            }
        }

        public void Menu()
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
                        CrearCuenta();
                        break;
                    case "2":
                        Login();
                        break;
                    case "3":
                        VerClases();
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

        private void CrearCuenta()
        {
            Console.Write("Usuario: ");
            string usuario = Console.ReadLine();
            Console.Write("Contraseña: ");
            string pass = Console.ReadLine();

            var msg = new Mensaje("REQ", "01", $"{usuario}|{pass}");
            Protocolo.Enviar(stream, msg);

            Mensaje res = Protocolo.Recibir(stream);
            Console.WriteLine($"Servidor: {res.Datos}");
        }

        private void Login()
        {
            Console.Write("Usuario: ");
            string usuario = Console.ReadLine();
            Console.Write("Contraseña: ");
            string pass = Console.ReadLine();

            var msg = new Mensaje("REQ", "02", $"{usuario}|{pass}");
            Protocolo.Enviar(stream, msg);

            Mensaje res = Protocolo.Recibir(stream);
            Console.WriteLine($"Servidor: {res.Datos}");
        }

        private void VerClases()
        {
            var msg = new Mensaje("REQ", "03", "");
            Protocolo.Enviar(stream, msg);

            Mensaje res = Protocolo.Recibir(stream);
            Console.WriteLine($"Clases disponibles:\n{res.Datos}");
        }
    }
}
