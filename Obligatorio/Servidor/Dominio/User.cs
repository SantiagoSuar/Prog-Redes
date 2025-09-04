namespace Servidor.Dominio;

public class User
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public List<Clase> ClasesCreadas { get; set; } = new();
    public List<Clase> ClasesInscriptas { get; set; } = new();
    
    public override bool Equals(object? obj)
    {
        User user = obj as User;
        return this.Username == user.Username;
    }
}