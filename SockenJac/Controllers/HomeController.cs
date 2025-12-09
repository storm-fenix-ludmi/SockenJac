using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SockenJac.Data;
using SockenJac.Models;
using SockenJac.ViewModels;
using System.Diagnostics;

namespace SockenJac.Controllers
{
    public class HomeController : Controller
    {

        private readonly IWebHostEnvironment _env;

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //public IActionResult Index()
        //{
        //    var productos = _context.Productos.Where(p => !p.Activo == true).ToList();

        //    return View(productos);
        //}

        public async Task<IActionResult> Index(string busqNombre, string busqDescripcion, int? busqPrecio)
        {


            var appDBcontext = _context.Productos.Where(p => !p.Activo).Select(e => e);

            if (!string.IsNullOrEmpty(busqNombre))
            {
                appDBcontext = appDBcontext.Where(e => e.Nombre.Contains(busqNombre));

            }
            if (!string.IsNullOrEmpty(busqDescripcion))
            {
                appDBcontext = appDBcontext.Where(e => e.Descripcion.Contains(busqDescripcion));
            }
            if (busqPrecio.HasValue)
            {
                appDBcontext = appDBcontext.Where(e => e.Precio == busqPrecio);
            }


            vmProductos modelo = new vmProductos
            {
                ListaProductos = await appDBcontext.ToListAsync(),
                busqNombre = busqNombre,
                busqDescripcion = busqDescripcion,
                busqPrecio = busqPrecio,
            };

            return View(modelo);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
