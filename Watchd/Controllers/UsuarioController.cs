using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Watchd.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario usuario)
        {
            // Asignar tipo por defecto (ID 1 = Usuario) si no se especifica
            if (usuario.IdTipoUsuario == 0) usuario.IdTipoUsuario = 1;

            if (!ModelState.IsValid) return View(usuario);

            using (var connection = new SqlConnection(connectionString))
            {
                // Hashear la contraseña antes de guardar
                usuario.Contrasena = HashPassword(usuario.Contrasena ?? string.Empty);

                string query = @"INSERT INTO Usuarios (Username, Email, Bio, contrasena, IdTipoUsuario) 
                                 VALUES (@Username, @Email, @Bio, @Contrasena, @IdTipoUsuario)";
                connection.Execute(query, usuario);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Login(Models.LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Username, Email, Bio, contrasena AS Contrasena, IdTipoUsuario FROM Usuarios WHERE Username = @Identifier OR Email = @Identifier";
                var usuario = connection.QueryFirstOrDefault<Usuario>(query, new { Identifier = model.UsernameOrEmail });

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario o correo no encontrado.");
                    return View(model);
                }

                // Verificar contraseña (almacenada con salt:hash)
                bool ok = VerifyPassword(usuario.Contrasena ?? string.Empty, model.Password ?? string.Empty);

                // Compatibilidad: si la contraseña en BD está en texto plano, permitirla (se actualizará solo al crear usuarios nuevos)
                if (!ok && !string.IsNullOrEmpty(usuario.Contrasena) && usuario.Contrasena == model.Password)
                {
                    ok = true;
                }

                if (!ok)
                {
                    ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
                    return View(model);
                }

                // Autenticación: almacenar el usuario en TempData para la vista
                TempData["UsuarioConectado"] = usuario.Username;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {
            using var connection = new SqlConnection(connectionString);
            var tipos = connection.Query<TipoUsuario>("SELECT Id, Nombre FROM Tipos_Usuario").ToList();
            ViewBag.Tipos = tipos.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre }).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CrearUsuario(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                using var conn = new SqlConnection(connectionString);
                var tipos = conn.Query<TipoUsuario>("SELECT Id, Nombre FROM Tipos_Usuario").ToList();
                ViewBag.Tipos = tipos.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre }).ToList();
                return View(usuario);
            }

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

                string queryInsert = @"INSERT INTO Usuarios (Username, Email, Bio, contrasena, IdTipoUsuario) 
                               VALUES (@Username, @Email, @Bio, @Contrasena, @IdTipoUsuario)";

                // Hashear la contraseña antes de guardar
                usuario.Contrasena = HashPassword(usuario.Contrasena ?? string.Empty);

                connection.Execute(queryInsert, usuario);
            }

            return RedirectToAction("Index", "Home");
        }

        // Generar hash usando PBKDF2 (salt + hash en base64 separados por ':')
        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            using var derive = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var key = derive.GetBytes(32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(key);
        }

        private static bool VerifyPassword(string stored, string provided)
        {
            if (string.IsNullOrEmpty(stored) || !stored.Contains(":")) return false;
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;
            try
            {
                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);
                using var derive = new Rfc2898DeriveBytes(provided, salt, 10000, HashAlgorithmName.SHA256);
                var key = derive.GetBytes(hash.Length);
                return CryptographicOperations.FixedTimeEquals(key, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}