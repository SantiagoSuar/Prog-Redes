using System;
using System.Collections.Generic;
using System.Linq;
using Servidor.Dominio;


namespace Servidor
{
 

    public sealed class InMemoryStore
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, User> _users = new();
        private readonly Dictionary<int, Clase> _clases = new();
        private int _nextClaseId = 1;

        public bool CrearUsuario(string username, string password)
        {
            lock (_lock)
            {
                if (_users.ContainsKey(username)) return false;
                _users[username] = new User { Username = username, Password = password };
                return true;
            }
        }

        public bool ValidarLogin(string username, string password)
        {
            lock (_lock)
            {
                return _users.TryGetValue(username, out var u) && u.Password == password;
            }
        }

        public Clase CrearClase(string creador, string nombre, string desc, int cupo, int duracionMin, DateTime inicio)
        {
            lock (_lock)
            {
                int id = _nextClaseId++;
                var c = new Clase
                {
                    Id = id,
                    Nombre = nombre,
                    Descripcion = desc,
                    CupoMax = cupo,
                    DuracionMin = duracionMin,
                    InicioUtc = inicio,
                    Link = $"cls-{id:D5}"
                };
                _clases[id] = c;
                return c;
            }
        }

        public List<Clase> ListarClases()
        {
            lock (_lock)
            {
                return _clases.Values.OrderBy(c => c.InicioUtc).ToList();
            }
        }
    }

  
}
