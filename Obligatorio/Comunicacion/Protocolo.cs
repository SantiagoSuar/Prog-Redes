using System.Net.Sockets;

namespace Comunicacion;

public static class Protocolo
{
    public static void Enviar(NetworkStream stream, Mensaje msg)
    {
    }

    public static Mensaje Recibir(NetworkStream stream)
    {
        return new Mensaje("", "", null);
    }
}