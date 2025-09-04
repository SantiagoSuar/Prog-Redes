using System.Net;
using System.Net.Sockets;
using Comunicacion;

class Program
{
    static void Main(string[] args)
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 10000;

        Socket socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socketCliente.Connect(new IPEndPoint(ip, port));
        Console.WriteLine($"Conectado al servidor {ip}:{port}");

        using NetworkStream stream = new NetworkStream(socketCliente);

        bool seguir = true;
        while (seguir)
        {
            MostrarMenu();
            Console.Write("Opción: ");
            string? opcion = Console.ReadLine();

            Mensaje req;

            switch (opcion)
            {
                case "1": // Signup
                    Console.Write("Usuario: ");
                    string usuario = Console.ReadLine() ?? "";
                    Console.Write("Clave: ");
                    string clave = Console.ReadLine() ?? "";
                    req = new Mensaje("REQ", "01", $"{usuario}|{clave}");
                    break;

                case "2": // Login
                    Console.Write("Usuario: ");
                    usuario = Console.ReadLine() ?? "";
                    Console.Write("Clave: ");
                    clave = Console.ReadLine() ?? "";
                    req = new Mensaje("REQ", "02", $"{usuario}|{clave}");
                    break;

                case "3": // Crear clase
                    Console.Write("Nombre: ");
                    string nombre = Console.ReadLine() ?? "";
                    Console.Write("Descripción: ");
                    string desc = Console.ReadLine() ?? "";
                    Console.Write("Cupo: ");
                    string cupo = Console.ReadLine() ?? "10";
                    Console.Write("Duración (min): ");
                    string duracion = Console.ReadLine() ?? "60";
                    req = new Mensaje("REQ", "10", $"{nombre}|{desc}|{cupo}|{duracion}");
                    break;

                case "4": // Listar clases
                    req = new Mensaje("REQ", "11", "");
                    break;

                case "0": // Salir
                    seguir = false;
                    continue;

                default:
                    Console.WriteLine("Opción inválida.");
                    continue;
            }

            Protocolo.Enviar(stream, req);
            Mensaje res = Protocolo.Recibir(stream);
            Console.WriteLine($"\n>>> Respuesta [{res.CMD}]: {res.Datos}\n");
        }

        socketCliente.Close();
        Console.WriteLine("Cliente cerrado.");
    }

    static void MostrarMenu()
    {
        Console.WriteLine("\n=== MENÚ CLIENTE ===");
        Console.WriteLine("1. Signup (registrar usuario)");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Crear clase");
        Console.WriteLine("4. Listar clases");
        Console.WriteLine("0. Salir");
        Console.WriteLine("====================\n");
    }
}
