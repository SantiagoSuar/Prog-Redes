namespace Servidor.Dominio;
public class Actividad
{
    public int ClaseId { get; set; }
    public string NombreClase { get; set; } = "";
    public string Estado { get; set; } = ""; // "INSCRIPTO", "CANCELADO", "FINALIZADO"
    public DateTime Fecha { get; set; }
}
