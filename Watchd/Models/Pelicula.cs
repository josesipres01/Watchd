using System;
using System.ComponentModel.DataAnnotations;

namespace Watchd.Models
{
    public class Pelicula
    {
        public int Id { get; set; }

        [Display(Name = "TMDB Id")]
        public int? TmdbId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "El título debe tener entre 1 y 150 caracteres.")]
        [Display(Name = "Título")] 
        public string Titulo { get; set; }

        [Display(Name = "Sinopsis")]
        public string Sinopsis { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del director no puede superar los 100 caracteres.")]
        [Display(Name = "Director (opcional)")]
        public string Director { get; set; }

        [Display(Name = "Año de Lanzamiento")]
        public int? AnoLanzamiento { get; set; }

        [Display(Name = "Duración (min)")]
        public int? Duracion { get; set; }

        [StringLength(255)]
        [Display(Name = "Poster (URL)")]
        public string Poster { get; set; }

        [Display(Name = "Calificación Promedio")]
        [Range(0, 10, ErrorMessage = "La calificación debe estar entre 0 y 10.")]
        public decimal? CalificacionPromedio { get; set; }

        [Display(Name = "Total Reviews")]
        public int? TotalReviews { get; set; }
    }
}
