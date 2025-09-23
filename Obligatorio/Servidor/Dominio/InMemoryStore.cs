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
        private readonly Dictionary<string, List<Actividad>> _historial = new();
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
                RegistrarActividad(username, clase, "INSCRIPTO");
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

                
                bool removed = clase.Inscritos.Remove(username);
                if (removed)
                {
                    // Registrar historial
                    RegistrarActividad(username, clase, "CANCELADO");
                }

                return removed;
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
        public bool EliminarClase(string username, int idClase)
        {
            lock (_lock)
            {
                if (!_clases.TryGetValue(idClase, out var clase)) return false;

                // Validaciones
                if (clase.Creador != username) return false; // solo el creador
                if (clase.InicioUtc <= DateTime.UtcNow) return false; // ya empezó
                if (clase.Inscritos.Count > 0) return false; // hay inscriptos

                // Borrar imágenes asociadas si existen
                try
                {
                    if (!string.IsNullOrEmpty(clase.ImagenPath) && File.Exists(clase.ImagenPath))
                    {
                        File.Delete(clase.ImagenPath);
                        string? carpeta = Path.GetDirectoryName(clase.ImagenPath);
                        if (!string.IsNullOrEmpty(carpeta) && Directory.Exists(carpeta))
                        {
                            Directory.Delete(carpeta, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARN] No se pudo borrar imágenes de la clase {idClase}: {ex.Message}");
                }

                // Finalmente eliminar la clase del diccionario
                _clases.Remove(idClase);
                return true;
            }
        }
        public List<Clase> ListarClasesFiltradas(string? palabraClave, DateTime? fechaMin, int? duracionMax)
        {
            lock (_lock)
            {
                IEnumerable<Clase> query = _clases.Values;

                if (!string.IsNullOrWhiteSpace(palabraClave))
                {
                    string f = palabraClave.ToLowerInvariant();
                    query = query.Where(c => c.Nombre.ToLower().Contains(f) || c.Descripcion.ToLower().Contains(f));
                }

                if (fechaMin.HasValue)
                {
                    query = query.Where(c => c.InicioUtc >= fechaMin.Value);
                }

                if (duracionMax.HasValue)
                {
                    query = query.Where(c => c.DuracionMin <= duracionMax.Value);
                }

                return query.OrderBy(c => c.InicioUtc).ToList();
            }
        }

        private void RegistrarActividad(string username, Clase clase, string estado)
        {
            if (!_historial.ContainsKey(username))
                _historial[username] = new List<Actividad>();

            _historial[username].Add(new Actividad
            {
                ClaseId = clase.Id,
                NombreClase = clase.Nombre,
                Estado = estado,
                Fecha = DateTime.UtcNow
            });
        }
        public List<Actividad> ObtenerHistorial(string username)
        {
            lock (_lock)
            {
                if (!_historial.ContainsKey(username))
                    return new List<Actividad>();

                //  Marcar como finalizado las clases vencidas
                foreach (var act in _historial[username])
                {
                    if (act.Estado == "INSCRIPTO" && _clases.TryGetValue(act.ClaseId, out var clase))
                    {
                        if (clase.InicioUtc.AddMinutes(clase.DuracionMin) <= DateTime.UtcNow)
                        {
                            act.Estado = "FINALIZADO";
                        }
                    }
                }

                return new List<Actividad>(_historial[username]);
            }
        }




    }

  
}
