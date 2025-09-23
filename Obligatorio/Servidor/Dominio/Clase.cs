namespace Servidor.Dominio;

public class Clase
{
    public int Id { get; set; }           
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int CupoMax { get; set; }
    public DateTime InicioUtc { get; set; }
    public int DuracionMin { get; set; }
    public required string Link { get; set; }
    public List<string> Imagenes { get; } = new List<string>();
    public HashSet<string> Inscritos { get; set; } = new();
    public string Creador { get; set; } = "";


}