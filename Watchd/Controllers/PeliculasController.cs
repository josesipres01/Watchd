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
        public IActionResult Catalogo(string buscar = null, string ordenarPor = null)
        {
            using var connection = new SqlConnection(connectionString);

            string query = @"SELECT Id, 
                                    TMDB_ID AS TmdbId, 
                                    Titulo, 
                                    Sinopsis, 
                                    Director, 
                                    [Año_Lanzamiento] AS AnoLanzamiento, 
                                    Duracion, 
                                    Poster, 
                                    Calificacion_Promedio AS CalificacionPromedio, 
                                    Total_Reviews AS TotalReviews 
                             FROM Peliculas 
                             WHERE (@Buscar IS NULL OR @Buscar = '' 
                                    OR Titulo LIKE '%' + @Buscar + '%' 
                                    OR Director LIKE '%' + @Buscar + '%')";

            query += ordenarPor switch
            {
                "recientes" => " ORDER BY [Año_Lanzamiento] DESC, Id DESC",
                "antiguos" => " ORDER BY [Año_Lanzamiento] ASC, Id ASC",
                "calificacion" => " ORDER BY Calificacion_Promedio DESC",
                "titulo" => " ORDER BY Titulo ASC",
                _ => " ORDER BY Id DESC"
            };

            var peliculas = connection.Query<Pelicula>(query, new { Buscar = string.IsNullOrWhiteSpace(buscar) ? null : buscar.Trim() }).ToList();

            ViewBag.Buscar = buscar;
            ViewBag.OrdenarPor = ordenarPor;

            return View(peliculas);
        }

        [HttpGet]
        public IActionResult Detalle(int id)
        {
            using var connection = new SqlConnection(connectionString);
            string query = @"SELECT Id, 
                                    TMDB_ID AS TmdbId, 
                                    Titulo, 
                                    Sinopsis, 
                                    Director, 
                                    [Año_Lanzamiento] AS AnoLanzamiento, 
                                    Duracion, 
                                    Poster, 
                                    Calificacion_Promedio AS CalificacionPromedio, 
                                    Total_Reviews AS TotalReviews 
                             FROM Peliculas 
                             WHERE Id = @Id";

            var pelicula = connection.QueryFirstOrDefault<Pelicula>(query, new { Id = id });
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
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

            if (pelicula.TmdbId.HasValue && Existe(connection, "Peliculas", "TMDB_ID", pelicula.TmdbId.Value))
            {
                ModelState.AddModelError("TmdbId", "Ya existe una película con ese TMDB ID.");
                return View(pelicula);
            }

            // Normalizar y comprobar duplicados por título (insensible a mayúsculas/espacios)
            var tituloNormalizado = pelicula.Titulo?.Trim().ToLower();
            if (Existe(connection, "Peliculas", "LOWER(LTRIM(RTRIM(Titulo)))", tituloNormalizado))
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

            TempData["Exito"] = $"¡La película '{pelicula.Titulo}' se agregó correctamente al catálogo!";
            return RedirectToAction(nameof(Index));
        }

        private static bool Existe(SqlConnection connection, string tabla, string columna, object valor)
        {
            var query = $"SELECT COUNT(1) FROM {tabla} WHERE {columna} = @Valor";
            return connection.QueryFirstOrDefault<int>(query, new { Valor = valor }) > 0;
        }
    }
}
