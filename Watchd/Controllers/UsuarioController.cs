using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Watchd.Models;

namespace Watchd.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly string connectionString;

        public UsuariosController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario usuario)
        {
            usuario.Rol = "Usuario"; 
            ModelState.Remove("Rol"); 

            if (!ModelState.IsValid) return View(usuario);

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Usuarios (Username, Email, Bio, Rol) 
                                 VALUES (@Username, @Email, @Bio, @Rol)";
                connection.Execute(query, usuario);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearUsuario(Usuario usuario)
        {
            if (!ModelState.IsValid) return View(usuario);

            using (var connection = new SqlConnection(connectionString))
            {
                string queryValidacion = "SELECT Username, Email FROM Usuarios WHERE Username = @Username OR Email = @Email";

                var duplicados = connection.Query(queryValidacion, usuario).ToList();

                if (duplicados.Any())
                {
                    bool hayErrores = false;

                    foreach (var dup in duplicados)
                    {
                        if (dup.Username.ToLower() == usuario.Username.ToLower())
                        {
                            ModelState.AddModelError("Username", "Este nombre de usuario ya está en uso. Elige otro.");
                            hayErrores = true;
                        }

                        if (dup.Email.ToLower() == usuario.Email.ToLower())
                        {
                            ModelState.AddModelError("Email", "Este correo electrónico ya está registrado.");
                            hayErrores = true;
                        }
                    }

                    if (hayErrores)
                    {
                        return View(usuario);
                    }
                }

                string queryInsert = @"INSERT INTO Usuarios (Username, Email, Bio, Rol) 
                               VALUES (@Username, @Email, @Bio, @Rol)";

                connection.Execute(queryInsert, usuario);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}