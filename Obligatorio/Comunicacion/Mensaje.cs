namespace Comunicacion;

public class Mensaje
{
    public string Header { get; set; }
    public string CMD { get; set; }
    public string? Datos { get; set; }

    public Mensaje(string header, string cmd, string? datos)
    {
        Header = header;
        CMD = cmd.PadLeft(2, '0'); // 2 caracteres
        Datos = datos ?? "";
    }
}