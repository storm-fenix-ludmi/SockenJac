using Microsoft.AspNetCore.Mvc.Rendering;
using SockenJac.Models;
using SockenJac.ViewModels;


namespace SockenJac.ViewModels
{
    public class vmCompras
    {
        public SelectList ListaProductos { get; set; }
        public List<Compra> ListaCompras{ get; set; }
        public string busqProductoNombre { get; set; } 
        public DateTime? busqFechaDesde { get; set; }  
        public DateTime? busqFechaHasta { get; set; }
        public int? ProductoId { get; set; }
        public paginador paginadorVM { get; set; }
    }
}