namespace Servidor.Dominio;

public class Usuario
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public List<Clase> ClasesCreadas { get; set; } = new();
    public List<Clase> ClasesInscriptas { get; set; } = new();
    
    public override bool Equals(object? obj)
    {
        Usuario user = obj as Usuario;
        return this.Username == user.Username;
    }
}