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
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Usuarios (Username, Email, Bio) 
                                 VALUES (@Username, @Email, @Bio)";

                connection.Execute(query, usuario);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}