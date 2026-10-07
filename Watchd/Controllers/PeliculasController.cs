using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Watchd.Models;

namespace Watchd.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly string connectionString;

        public PeliculasController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult AgregarPelicula()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AgregarPelicula(Pelicula pelicula)
        {
            if (!ModelState.IsValid) return View(pelicula);

            using var connection = new SqlConnection(connectionString);

            // Si se proporciona TMDB id, comprobar duplicado por TMDB_ID
            if (pelicula.TmdbId.HasValue)
            {
                var existeTmdb = connection.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Peliculas WHERE TMDB_ID = @TmdbId", new { TmdbId = pelicula.TmdbId.Value });
                if (existeTmdb > 0)
                {
                    ModelState.AddModelError("TmdbId", "Ya existe una película con ese TMDB ID.");
                    return View(pelicula);
                }
            }

            // Normalizar y comprobar duplicados por título (insensible a mayúsculas/espacios)
            var tituloNormalizado = pelicula.Titulo?.Trim().ToLower();
            var existe = connection.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Peliculas WHERE LOWER(LTRIM(RTRIM(Titulo))) = @Titulo", new { Titulo = tituloNormalizado });

            if (existe > 0)
            {
                ModelState.AddModelError("Titulo", "Ya existe una película con ese título.");
                return View(pelicula);
            }

            // Validar rango de calificación antes de insertar para evitar overflow en la BD
            if (pelicula.CalificacionPromedio.HasValue)
            {
                if (pelicula.CalificacionPromedio < 0m || pelicula.CalificacionPromedio > 10m)
                {
                    ModelState.AddModelError("CalificacionPromedio", "La calificación debe estar entre 0 y 10.");
                    return View(pelicula);
                }
                // Asegurar escala/precision razonable (2 decimales)
                pelicula.CalificacionPromedio = Math.Round(pelicula.CalificacionPromedio.Value, 2);
            }

            // Insertar usando los nombres de columnas reales
            string insert = @"INSERT INTO Peliculas (TMDB_ID, Titulo, Sinopsis, Director, [Año_Lanzamiento], Duracion, Poster, Calificacion_Promedio, Total_Reviews)
                              VALUES (@TmdbId, @Titulo, @Sinopsis, @Director, @AnoLanzamiento, @Duracion, @Poster, @CalificacionPromedio, @TotalReviews)";

            connection.Execute(insert, pelicula);

            return View();
        }
    }
}
