// Servidor/Services/ClienteConectado.cs
using System;
using System.Net.Sockets;
using System.Text;
using Comunicacion;

public class ClienteConectado
{
    private TcpClient cliente;
    private NetworkStream stream;

    public ClienteConectado(TcpClient c)
    {
        cliente = c;
        stream = cliente.GetStream();
    }

    public void Atender()
    {
        try
        {
            while (true)
            {
                Mensaje req = Protocolo.Recibir(stream);
                Console.WriteLine($"[{DateTime.Now}] CMD recibido: {req.CMD}");

                // procesar la solicitud (se apoya en Business)
                Mensaje res = Procesar(req);

                Protocolo.Enviar(stream, res);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cliente desconectado: {ex.Message}");
            cliente.Close();
        }
    }

    private Mensaje Procesar(Mensaje req)
    {
        switch (req.CMD)
        {
            case "01": return new Mensaje("RES", "01", "Login OK");
            case "02": return new Mensaje("RES", "02", "Clase creada");
            default:   return new Mensaje("RES", "99", "Comando no reconocido");
        }
    }
}