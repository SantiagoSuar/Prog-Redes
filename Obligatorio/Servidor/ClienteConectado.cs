using System;
using System.Net.Sockets;
using System.Text;
using Comunicacion;
using Servidor;

public class ClienteConectado
{
    private Socket socket;
    private InMemoryStore store;
    private string? usuarioLogueado = null;

    public ClienteConectado(Socket socket, InMemoryStore store)
    {
        this.socket = socket;
        this.store = store;
    }

    public void Atender()
    {
        try
        {
            while (true)
            {
                Mensaje req = Protocolo.Recibir(socket);
                Console.WriteLine($"[{DateTime.Now}] CMD recibido: {req.CMD}");

                Mensaje res = Procesar(req);

                Protocolo.Enviar(socket, res);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cliente desconectado: {ex.Message}");
            socket.Close();
        }
    }

    private Mensaje Procesar(Mensaje req)
    {
        string[] datos = (req.Datos ?? "").Split('|');
        switch (req.CMD)
        {
            case "01": // SIGNUP
                string u = datos.Length > 0 ? datos[0] : "";
                string p = datos.Length > 1 ? datos[1] : "";
                bool creado = store.CrearUsuario(u, p);
                return new Mensaje("RES", "01", creado ? "OK|Usuario creado" : "ERR|Usuario ya existe");

            case "02": // LOGIN
                u = datos.Length > 0 ? datos[0] : "";
                p = datos.Length > 1 ? datos[1] : "";
                bool valido = store.ValidarLogin(u, p);
                if (valido) usuarioLogueado = u;
                return new Mensaje("RES", "02", valido ? "OK|Login correcto" : "ERR|Credenciales inválidas");

            case "10": // CREAR CLASE
                if (usuarioLogueado == null) return new Mensaje("RES", "10", "ERR|Debe loguearse");
                string nombre = datos.Length > 0 ? datos[0] : "SinNombre";
                string desc = datos.Length > 1 ? datos[1] : "";
                int cupo = datos.Length > 2 ? int.Parse(datos[2]) : 10;
                int duracion = datos.Length > 3 ? int.Parse(datos[3]) : 60;
                DateTime inicio = DateTime.UtcNow.AddMinutes(10);

                var c = store.CrearClase(usuarioLogueado, nombre, desc, cupo, duracion, inicio);
                return new Mensaje("RES", "10", $"OK|Clase {c.Id} creada");

            case "11": // LISTAR CLASES (con filtros)
                string palabraClave = datos.Length > 0 ? datos[0] : "";
                DateTime? fechaMin = null;
                if (datos.Length > 1 && DateTime.TryParse(datos[1], out var f)) fechaMin = f;
                int? duracionMax = null;
                if (datos.Length > 2 && int.TryParse(datos[2], out var d)) duracionMax = d;

                var clasesFiltradas = store.ListarClasesFiltradas(palabraClave, fechaMin, duracionMax);
                var sb = new StringBuilder();
                foreach (var clase in clasesFiltradas)
                {
                    sb.AppendLine($"{clase.Id}|{clase.Nombre}|{clase.InicioUtc:o}|{clase.DuracionMin}|{clase.CupoMax}|{clase.Inscritos.Count}");
                }
                return new Mensaje("RES", "11", sb.ToString());


            case "12": // MODIFICAR CLASE
                if (usuarioLogueado == null) return new Mensaje("RES", "12", "ERR|Debe loguearse");
                if (datos.Length < 5) return new Mensaje("RES", "12", "ERR|Faltan parámetros");

                int idClass;
                if (!int.TryParse(datos[0], out idClass)) return new Mensaje("RES", "12", "ERR|ID inválido");

                string nuevoNombre = datos[1];
                string nuevaDesc = datos[2];
                int nuevoCupo = int.Parse(datos[3]);
                int nuevaDuracion = int.Parse(datos[4]);

                var modificado = store.ModificarClase(usuarioLogueado, idClass, nuevoNombre, nuevaDesc, nuevoCupo, nuevaDuracion);
                return new Mensaje("RES", "12", modificado ? "OK|Clase modificada" : "ERR|No se pudo modificar");
            
            case "13": // ELIMINAR CLASE
                if (usuarioLogueado == null) return new Mensaje("RES", "13", "ERR|Debe loguearse");
                if (datos.Length < 1) return new Mensaje("RES", "13", "ERR|Falta ID clase");

                int idDel;
                if (!int.TryParse(datos[0], out idDel)) return new Mensaje("RES", "13", "ERR|ID inválido");

                var eliminado = store.EliminarClase(usuarioLogueado, idDel);
                return new Mensaje("RES", "13", eliminado ? "OK|Clase eliminada" : "ERR|No se pudo eliminar");


            
            case "20": // INSCRIBIRSE
                if (usuarioLogueado == null) return new Mensaje("RES", "20", "ERR|Debe loguearse");
                if (datos.Length < 1) return new Mensaje("RES", "20", "ERR|Falta ID clase");

                int idClase;
                if (!int.TryParse(datos[0], out idClase)) return new Mensaje("RES", "20", "ERR|ID inválido");

                var inscripto = store.InscribirUsuario(usuarioLogueado, idClase);
                return new Mensaje("RES", "20", inscripto ? "OK|Inscripción exitosa" : "ERR|No se pudo inscribir");
            
            case "21": // CANCELAR INSCRIPCIÓN
                if (usuarioLogueado == null) return new Mensaje("RES", "21", "ERR|Debe loguearse");
                if (datos.Length < 1) return new Mensaje("RES", "21", "ERR|Falta ID clase");

                if (!int.TryParse(datos[0], out idClase)) return new Mensaje("RES", "21", "ERR|ID inválido");

                var cancelada = store.CancelarInscripcion(usuarioLogueado, idClase);
                return new Mensaje("RES", "21", cancelada ? "OK|Inscripción cancelada" : "ERR|No se pudo cancelar");
            
            case "30": // HISTORIAL
                if (usuarioLogueado == null) return new Mensaje("RES", "30", "ERR|Debe loguearse");

                var actividades = store.ObtenerHistorial(usuarioLogueado);
                var sbHist = new StringBuilder();
                foreach (var act in actividades)
                {
                    sbHist.AppendLine($"{act.ClaseId}|{act.NombreClase}|{act.Estado}|{act.Fecha:o}");
                }
                return new Mensaje("RES", "30", sbHist.ToString());

            
            default:
                return new Mensaje("RES", "99", "ERR|Comando no reconocido");
        }
    }
}
