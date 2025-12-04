using System.ComponentModel.DataAnnotations;

namespace SockenJac.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? foto { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public int Precio { get; set; }
        public bool Activo { get; set; } = false;
    }
}
