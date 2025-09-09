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

            case "11": // LISTAR CLASES
                var clases = store.ListarClases();
                var sb = new StringBuilder();
                foreach (var clase in clases)
                {
                    sb.AppendLine($"{clase.Id}|{clase.Nombre}|{clase.InicioUtc:o}|{clase.DuracionMin}|{clase.CupoMax}");
                }
                return new Mensaje("RES", "11", sb.ToString());

            default:
                return new Mensaje("RES", "99", "ERR|Comando no reconocido");
        }
    }
}
