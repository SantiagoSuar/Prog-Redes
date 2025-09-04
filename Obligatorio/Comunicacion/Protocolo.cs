using System.Net.Sockets;
using System.Text;

namespace Comunicacion;

public static class Protocolo
{
    public static void Enviar(NetworkStream stream, Mensaje msg)
    {
        string datos = msg.Datos ?? "";
        byte[] datosBytes = Encoding.UTF8.GetBytes(datos);
        string largo = datosBytes.Length.ToString().PadLeft(4, '0'); // 4 caracteres

        string trama = msg.Header + msg.CMD + largo + datos;
        byte[] buffer = Encoding.UTF8.GetBytes(trama);

        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
    }

    public static Mensaje Recibir(NetworkStream stream)
    {
        string header = LeerExacto(stream, 3);
        string cmd = LeerExacto(stream, 2);
        string largoStr = LeerExacto(stream, 4);

        int largo = int.Parse(largoStr);
        string datos = largo > 0 ? LeerExacto(stream, largo) : "";

        return new Mensaje(header, cmd, datos);
    }

    private static string LeerExacto(NetworkStream stream, int cantidad)
    {
        byte[] buffer = new byte[cantidad];
        int leidos = 0;
        while (leidos < cantidad)
        {
            int n = stream.Read(buffer, leidos, cantidad - leidos);
            if (n <= 0) throw new IOException("Conexión cerrada por el cliente.");
            leidos += n;
        }
        return Encoding.UTF8.GetString(buffer, 0, cantidad);
    }
}