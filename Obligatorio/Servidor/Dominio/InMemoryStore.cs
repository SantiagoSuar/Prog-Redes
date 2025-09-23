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
                    Creador = creador,
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
        public bool InscribirUsuario(string username, int idClase)
        {
            lock (_lock)
            {
                if (!_clases.TryGetValue(idClase, out var clase)) return false;

                if (clase.InicioUtc <= DateTime.UtcNow) return false; // ya empezó
                if (clase.Inscritos.Count >= clase.CupoMax) return false; // cupo lleno
                if (clase.Inscritos.Contains(username)) return false; // ya estaba inscripto

                
                clase.Inscritos.Add(username);
                return true;
            }
        }

        public bool CancelarInscripcion(string username, int idClase)
        {
            lock (_lock)
            {
                if (!_clases.TryGetValue(idClase, out var clase)) return false;

                var tiempoRestante = clase.InicioUtc - DateTime.UtcNow;
                if (tiempoRestante.TotalMinutes < 2) return false; // regla de la letra

                
                return clase.Inscritos.Remove(username);
            }
        }
        public bool ModificarClase(string username, int idClase, string nombre, string desc, int cupo, int duracion)
        {
            lock (_lock)
            {
                if (!_clases.TryGetValue(idClase, out var clase)) return false;

                // Validaciones
                if (clase.Creador != username) return false;
                if (clase.InicioUtc <= DateTime.UtcNow) return false; // ya empezó
                if (clase.Inscritos.Count > 0 && cupo < clase.Inscritos.Count) return false; // no reducir cupo debajo de inscriptos
                if (clase.Link == null) return false; // sanity check
                

                clase.Nombre = nombre;
                clase.Descripcion = desc;
                clase.CupoMax = cupo;
                clase.DuracionMin = duracion;
                return true;
            }
        }



    }

  
}
