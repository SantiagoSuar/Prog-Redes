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

        // Comunicación directa, solo con socket, sin NetworkStream

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
                case "5": // Inscribirse en clase
                    Console.Write("ID de clase: ");
                    string idIns = Console.ReadLine() ?? "";
                    req = new Mensaje("REQ", "20", idIns);
                    break;

                case "6": // Cancelar inscripción
                    Console.Write("ID de clase: ");
                    string idCan = Console.ReadLine() ?? "";
                    req = new Mensaje("REQ", "21", idCan);
                    break;
                case "7": // Modificar clase
                    Console.Write("ID clase: ");
                    string idMod = Console.ReadLine() ?? "";
                    Console.Write("Nuevo nombre: ");
                    string nNombre = Console.ReadLine() ?? "";
                    Console.Write("Nueva descripción: ");
                    string nDesc = Console.ReadLine() ?? "";
                    Console.Write("Nuevo cupo: ");
                    string nCupo = Console.ReadLine() ?? "10";
                    Console.Write("Nueva duración (min): ");
                    string nDur = Console.ReadLine() ?? "60";
                    req = new Mensaje("REQ", "12", $"{idMod}|{nNombre}|{nDesc}|{nCupo}|{nDur}");
                    break;
                case "8": // Eliminar clase
                    Console.Write("ID clase: ");
                    string idDel = Console.ReadLine() ?? "";
                    req = new Mensaje("REQ", "13", idDel);
                    break;



                case "0": // Salir
                    seguir = false;
                    continue;

                default:
                    Console.WriteLine("Opción inválida.");
                    continue;
            }

            Protocolo.Enviar(socketCliente, req);
            Mensaje res = Protocolo.Recibir(socketCliente);
            if (res.CMD == "11") // LISTAR CLASES → tabla
                MostrarClases(res.Datos ?? "");
            else
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
        Console.WriteLine("5. Inscribirse en clase");
        Console.WriteLine("6. Cancelar inscripción");
        Console.WriteLine("7. Modificar clase");
        Console.WriteLine("8. Eliminar clase");
        Console.WriteLine("0. Salir");
        Console.WriteLine("====================\n");
    }
    static void MostrarClases(string datos)
    {
        if (string.IsNullOrWhiteSpace(datos))
        {
            Console.WriteLine("\nNo hay clases disponibles.\n");
            return;
        }

        Console.WriteLine("\n=== LISTADO DE CLASES ===");
        Console.WriteLine($"{"ID",3} {"Nombre",10} {"Inicio",25} {"Dur(min)",8} {"Cupo",5} {"Inscriptos",10}");
        Console.WriteLine(new string('-', 65));

        string[] lineas = datos.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var linea in lineas)
        {
            string[] campos = linea.Split('|');
            if (campos.Length >= 6)
            {
                string id = campos[0];
                string nombre = campos[1];
                string inicio = campos[2];
                string dur = campos[3];
                string cupo = campos[4];
                string ins = campos[5];

                // Sin alineamiento a la izquierda, para que se vean valores cortos
                Console.WriteLine($"{id,3} {nombre,10} {inicio,25} {dur,8} {cupo,5} {ins,10}");
            }
        }

        Console.WriteLine(new string('=', 65));
    }
}
