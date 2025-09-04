namespace Comunicacion;

public class Mensaje
{
    public string Header { get; set; }  // REQ / RES
    public string CMD { get; set; }     // Código de operación
    public string Datos { get; set; }   // Payload

    public Mensaje(string header, string cmd, string datos)
    {
        Header = header;
        CMD = cmd;
        Datos = datos;
    }
}
