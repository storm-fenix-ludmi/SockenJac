using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpreadsheetLight;
using SockenJac.Data;
using SockenJac.Models;
using SockenJac.ViewModels;



namespace SockenJac.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }
       
        public async Task<IActionResult> Index(string busqProductoNombre, DateTime? busqFechaDesde, DateTime? busqFechaHasta, int? ProductoId, int pagina = 1)
        {
            paginador paginador = new paginador
            {
                paginaActual = pagina,
                cantRegistrosPagina = 3,
            };

            var appDBcontext = _context.Ventas.Include(a => a.Producto).Select(e => e);

            if (!string.IsNullOrEmpty(busqProductoNombre))
            {
                appDBcontext = appDBcontext.Where(e => e.ProductoNombre.Contains(busqProductoNombre));
                paginador.filtros.Add("busqProductoNombre", busqProductoNombre);
            }
            if (busqFechaDesde.HasValue)
            {
                appDBcontext = appDBcontext.Where(e => e.Fecha >= busqFechaDesde.Value);
                paginador.filtros.Add("busqFechaDesde", busqFechaDesde.Value.ToString("yyyy-MM-dd"));
            }
            if (busqFechaHasta.HasValue)
            {
                appDBcontext = appDBcontext.Where(e => e.Fecha <= busqFechaHasta.Value);
                paginador.filtros.Add("busqFechaHasta", busqFechaHasta.Value.ToString("yyyy-MM-dd"));
            }
            if (ProductoId.HasValue)
            {
                appDBcontext = appDBcontext.Where(e => e.ProductoId== ProductoId);
                paginador.filtros.Add("ProductoId", ProductoId.ToString());
            }

            paginador.cantRegistros = appDBcontext.Count();

            appDBcontext = appDBcontext
                .OrderByDescending(e => e.Fecha)
                .Skip(paginador.cantRegistrosPagina * (pagina - 1))
                .Take(paginador.cantRegistrosPagina);

            vmVentas modelo = new vmVentas
            {
                ListaVentas = await appDBcontext.ToListAsync(),
                ListaProductos = new SelectList(_context.Productos, "Id", "Nombre"),
                busqProductoNombre = busqProductoNombre,
                busqFechaDesde = busqFechaDesde,
                busqFechaHasta = busqFechaHasta,
                paginadorVM = paginador
            };
            return View(modelo);
        }
        //GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
              
                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == venta.ProductoId && !p.Activo);
                if (producto != null)
                {
                    producto.Cantidad += venta.Cantidad;  
                    _context.Productos.Update(producto);
                }
                _context.Ventas.Remove(venta);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}
    