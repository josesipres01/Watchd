using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Watchd.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo electrónico válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "La biografía no puede superar los 500 caracteres.")]
        [Display(Name = "Biografía (Opcional)")]
        public string Bio { get; set; }


        // Relación con la tabla Tipos_Usuario
        [Display(Name = "Tipo de Usuario")]
        public int IdTipoUsuario { get; set; }

        [NotMapped]
        public TipoUsuario TipoUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}