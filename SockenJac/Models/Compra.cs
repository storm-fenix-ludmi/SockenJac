using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SockenJac.Models
{
    public class Compra
    {
      
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
		public string? foto { get; set; }
		public string ProductoNombre { get; set; } = string.Empty;
        public int CantidadAgregada { get; set; }
        public int PrecioAnterior { get; set; }
        public int PrecioNuevo { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}
