using System.ComponentModel.DataAnnotations;

namespace Watchd.Models
{
    public class TipoUsuario
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
    }
}
