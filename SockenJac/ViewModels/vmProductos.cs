using Microsoft.AspNetCore.Mvc.Rendering;
using SockenJac.Models;
using SockenJac.ViewModels;


namespace SockenJac.ViewModels
{
    public class vmProductos
    {
        public List<Producto>? ListaProductos { get; set; }
        public string busqNombre { get; set; }
        public string busqDescripcion { get; set; }
        public int? busqPrecio { get; set; }
        public paginador? paginadorVM { get; set; }
    }
}

 