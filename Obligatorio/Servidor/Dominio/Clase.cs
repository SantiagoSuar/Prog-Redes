namespace Servidor.Dominio;

public class Clase
{
    public string Id { get; set; }           
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Cupos { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Duracion { get; set; }
    public required string Link { get; set; }
    public List<string> Imagenes { get; } = new List<string>();
}