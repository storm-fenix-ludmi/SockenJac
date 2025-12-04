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
    public class ComprasController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context, IWebHostEnvironment env)
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

            var appDBcontext = _context.Compras.Include(a => a.Producto).Select(e => e);

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
                appDBcontext = appDBcontext.Where(e => e.ProductoId == ProductoId);
                paginador.filtros.Add("ProductoId", ProductoId.ToString());
            }

            paginador.cantRegistros = appDBcontext.Count();

            appDBcontext = appDBcontext
                .OrderByDescending(e => e.Fecha)
                .Skip(paginador.cantRegistrosPagina * (pagina - 1))
                .Take(paginador.cantRegistrosPagina);

            vmCompras modelo = new vmCompras
            {
                 ListaCompras= await appDBcontext.ToListAsync(),
                ListaProductos = new SelectList(_context.Productos, "Id", "Nombre"),
                busqProductoNombre = busqProductoNombre,
                busqFechaDesde = busqFechaDesde,
                busqFechaHasta = busqFechaHasta,
                paginadorVM = paginador
            };
            return View(modelo);
        }

        public async Task<IActionResult> Create()
        {
            var productos = await _context.Productos.Where(p => !p.Activo).ToListAsync();  
            return View(productos);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int[] productoId, int[] cantidadAgregada, int[] precioNuevo)
        {
            for (int i = 0; i < productoId.Length; i++)
            {
            

                if (cantidadAgregada[i] > 0 || precioNuevo[i] != 0)
                {
                    var prod = await _context.Productos.FindAsync(productoId[i]);
                    if (prod == null || prod.Activo) continue;

                    int precioActual = prod.Precio;
                    if (precioNuevo[i] != precioActual && precioNuevo[i] > 0)
                    {
                        prod.Precio = precioNuevo[i];
                    }

                    if (cantidadAgregada[i] > 0)
                    {
                        prod.Cantidad += cantidadAgregada[i];
                    }

                    var compra = new Compra
                    {
                        ProductoId = prod.Id,
                        ProductoNombre = prod.Nombre,
                        foto = prod.foto,
                        CantidadAgregada = cantidadAgregada[i],
                        PrecioAnterior = precioActual,
                        PrecioNuevo = prod.Precio
                    };

                    _context.Compras.Add(compra);
                    _context.Productos.Update(prod);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Productos");
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var compra = await _context.Compras
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compra == null)
            {
                return NotFound();
            }
            return View(compra);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra != null)
            {
                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == compra.ProductoId && !p.Activo);
                if (producto != null)
                {
                    producto.Cantidad -= compra.CantidadAgregada;
                    _context.Productos.Update(producto);
                }
                _context.Compras.Remove(compra);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }

    }

}
