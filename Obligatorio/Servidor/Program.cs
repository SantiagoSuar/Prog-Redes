using System.Net;
using System.Net.Sockets;
using System.Threading;
using Servidor;

class Program
{
    static void Main(string[] args)
    {
        //configuracion del socket expuesto por el servidor
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 10020;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        Socket socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socketServidor.Bind(localEndPoint);
        socketServidor.Listen(10); //cantidad de conexiones pendientes

        Console.WriteLine($"Servidor escuchando en {ipAddress}:{port}. Escribe 'q' para salir.");

        var store = new InMemoryStore();

        Console.WriteLine("Esperando conexiones...");

        // Thread para aceptar clientes
        Thread acceptThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    Socket socketCliente = socketServidor.Accept();
                    Console.WriteLine("Cliente conectado.");

                    // Adaptamos socket -> TcpClient para reusar tu ClienteConectado
                    TcpClient tcpCliente = new TcpClient { Client = socketCliente };

                    ClienteConectado cliente = new ClienteConectado(tcpCliente, store);
                    Thread t = new Thread(cliente.Atender);
                    t.IsBackground = true;
                    t.Start();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Error en Accept: " + ex.Message);
                    break;
                }
            }
        });
        acceptThread.Start();

        // Loop para cerrar servidor
        while (true)
        {
            string? cmd = Console.ReadLine();
            if (cmd?.Trim().ToLower() == "q")
            {
                socketServidor.Close();
                break;
            }
        }
    }
}