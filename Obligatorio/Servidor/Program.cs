// Servidor/Program.cs

using System.Net;
using System.Net.Sockets;

class Program
{
    public static void Main()
    {
        ServidorApp servidor = new ServidorApp();
        servidor.Iniciar();
    }
}

// Servidor/ServidorApp.cs
public class ServidorApp
{
    private TcpListener listener;

    public void Iniciar()
    {
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Servidor iniciado en puerto 5000");

        while (true)
        {
            TcpClient cliente = listener.AcceptTcpClient();
            Thread t = new Thread(() => new ClienteConectado(cliente).Atender());
            t.Start();
        }
    }
}