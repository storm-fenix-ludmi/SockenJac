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
    public class ProductosController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

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


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null || producto.Activo)
            {
                return NotFound();
            }

            return View(producto);
        }


      
        [HttpPost]
        public async Task<IActionResult> Vender(int[] productoId, int[] cantidad)
        {
            for (int i = 0; i < productoId.Length; i++)
            {
                if (cantidad[i] <= 0) continue;

                var producto = await _context.Productos.FindAsync(productoId[i]);
                if (producto == null || producto.Activo) continue;

                if (producto.Cantidad < cantidad[i])
                    return BadRequest($"No hay stock suficiente para {producto.Nombre}");

                producto.Cantidad -= cantidad[i];

                var venta = new Venta
                {
                    ProductoId = producto.Id,
                    foto=producto.foto,
                    ProductoNombre = producto.Nombre,
                    Cantidad = cantidad[i],
                    Total = producto.Precio * cantidad[i]
                };

                _context.Ventas.Add(venta);
                _context.Productos.Update(producto);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Fecha,Cantidad,Precio")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoFoto = archivos[0];
                    if (archivoFoto.Length > 0)
                    {
                        var rutaDestino = Path.Combine(_env.WebRootPath, "fotografias");
                        var extArch = Path.GetExtension(archivoFoto.FileName);

                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + extArch;

                        using (var filestream = new FileStream(Path.Combine(rutaDestino, archivoDestino), FileMode.Create))
                        {
                            archivoFoto.CopyTo(filestream);
                            producto.foto = archivoDestino;
                        }

                        _context.Add(producto);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(producto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null || producto.Activo)
            {
                return NotFound();
            }
            return View(producto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,foto,Fecha,Cantidad,Precio")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            var existingProducto = await _context.Productos.FindAsync(id);
            if (existingProducto == null || existingProducto.Activo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var archivos = HttpContext.Request.Form.Files;
                    if (archivos != null && archivos.Count > 0)
                    {
                        var archivoFoto = archivos[0];
                        if (archivoFoto.Length > 0)
                        {
                            var rutaDestino = Path.Combine(_env.WebRootPath, "fotografias");
                            var extArch = Path.GetExtension(archivoFoto.FileName);

                            var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + extArch;

                            using (var filestream = new FileStream(Path.Combine(rutaDestino, archivoDestino), FileMode.Create))
                            {
                                archivoFoto.CopyTo(filestream);

                                if (!string.IsNullOrEmpty(producto.foto))
                                {
                                    string fotoAnterior = Path.Combine(rutaDestino, producto.foto);
                                    if (System.IO.File.Exists(fotoAnterior))
                                        System.IO.File.Delete(fotoAnterior);
                                }

                                producto.foto = archivoDestino;
                            }
                        }
                    }
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null || producto.Activo)
            {
                return NotFound();
            }

            return View(producto);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null && !producto.Activo)  
            {
                producto.Activo = true;  
                _context.Update(producto);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id && !e.Activo); 
        }
    }
}
