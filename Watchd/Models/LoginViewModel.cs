using System.ComponentModel.DataAnnotations;

namespace Watchd.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario o correo es obligatorio.")]
        [Display(Name = "Usuario o Correo")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
