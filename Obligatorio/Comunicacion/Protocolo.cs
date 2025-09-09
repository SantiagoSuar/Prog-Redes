using System.Net.Sockets;
using System.Text;

namespace Comunicacion;

public static class Protocolo
{
    public static void Enviar(Socket socket, Mensaje msg)
    {
        string datos = msg.Datos ?? "";
        byte[] datosBytes = Encoding.UTF8.GetBytes(datos);
        string largo = datosBytes.Length.ToString().PadLeft(4, '0');

        string trama = msg.Header + msg.CMD + largo + datos;
        byte[] buffer = Encoding.UTF8.GetBytes(trama);

        int enviados = 0;
        while (enviados < buffer.Length)
        {
            int n = socket.Send(buffer, enviados, buffer.Length - enviados, SocketFlags.None);
            if (n <= 0) throw new IOException("Error al enviar datos");
            enviados += n;
        }
    }

    public static Mensaje Recibir(Socket socket)
    {
        string header = LeerExacto(socket, 3);
        string cmd = LeerExacto(socket, 2);
        string largoStr = LeerExacto(socket, 4);

        int largo = int.Parse(largoStr);
        string datos = largo > 0 ? LeerExacto(socket, largo) : "";

        return new Mensaje(header, cmd, datos);
    }

    private static string LeerExacto(Socket socket, int cantidad)
    {
        byte[] buffer = new byte[cantidad];
        int leidos = 0;
        while (leidos < cantidad)
        {
            int n = socket.Receive(buffer, leidos, cantidad - leidos, SocketFlags.None);
            if (n <= 0) throw new IOException("Conexión cerrada por el cliente.");
            leidos += n;
        }
        return Encoding.UTF8.GetString(buffer, 0, cantidad);
    }
}