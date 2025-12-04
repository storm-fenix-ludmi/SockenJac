using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SockenJac.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public string? foto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int Total { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

    }
}
