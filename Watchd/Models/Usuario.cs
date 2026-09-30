using System.ComponentModel.DataAnnotations;

namespace Watchd.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Nombre de Usuario (Username)")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo electrónico válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "La biografía no puede superar los 500 caracteres.")]
        [Display(Name = "Biografía (Opcional)")]
        public string Bio { get; set; }
    }
}