using Microsoft.AspNetCore.Mvc;
using SockenJac.Data;
using SockenJac.Models;
using System.Diagnostics;

namespace SockenJac.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.Productos.Where(p => !p.Activo == true).ToList();

            return View(productos);
        }

      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
