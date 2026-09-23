using System.ComponentModel.DataAnnotations;

namespace Watchd.Models
{
    public class Visualizacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título de la película es obligatorio.")]
        [Display(Name = "Título de la Película")] 
        public string TituloPelicula { get; set; }

        [Required(ErrorMessage = "Debes ingresar una calificación.")]
        [Range(1, 5, ErrorMessage = "La calificación debe ser entre 1 y 5 estrellas.")]
        [Display(Name = "Calificación (1 al 5)")] 
        public int Calificacion { get; set; }
    }
}