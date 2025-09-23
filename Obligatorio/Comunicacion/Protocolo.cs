using System.Net.Sockets;
using System.Text;

namespace Comunicacion;

public static class Protocolo
{
    // Enviar mensaje serializado al socket
    public static void Enviar(Socket socket, Mensaje msg)
    {
        string datos = msg.Datos ?? "";
        byte[] datosBytes = Encoding.UTF8.GetBytes(datos);

        // Largo en caracteres ASCII (4 dígitos)
        string largoStr = datosBytes.Length.ToString().PadLeft(4, '0');
        byte[] headerCmdLen = Encoding.UTF8.GetBytes(msg.Header + msg.CMD + largoStr);

        // Mandar primero header+cmd+largo
        socket.Send(headerCmdLen);
        // Luego los datos reales
        if (datosBytes.Length > 0)
            socket.Send(datosBytes);
    }

    // Recibir mensaje completo desde socket
    public static Mensaje Recibir(Socket socket)
    {
        string header = LeerExactoString(socket, 3);
        string cmd = LeerExactoString(socket, 2);
        string largoStr = LeerExactoString(socket, 4);

        int largo = int.Parse(largoStr);

        byte[] datosBytes = LeerExactoBytes(socket, largo);
        string datos = Encoding.UTF8.GetString(datosBytes);

        return new Mensaje(header, cmd, datos);
    }

    // Lee exactamente N bytes y devuelve como string
    private static string LeerExactoString(Socket socket, int cantidad)
    {
        byte[] buffer = LeerExactoBytes(socket, cantidad);
        return Encoding.UTF8.GetString(buffer, 0, cantidad);
    }

    // Lee exactamente N bytes en un array
    private static byte[] LeerExactoBytes(Socket socket, int cantidad)
    {
        byte[] buffer = new byte[cantidad];
        int leidos = 0;
        while (leidos < cantidad)
        {
            int n = socket.Receive(buffer, leidos, cantidad - leidos, SocketFlags.None);
            if (n <= 0) throw new IOException("Conexión cerrada.");
            leidos += n;
        }
        return buffer;
    }
}