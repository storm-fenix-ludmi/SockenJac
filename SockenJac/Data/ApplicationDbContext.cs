using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SockenJac.Models;

namespace SockenJac.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){}


        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set;}
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Proveedores> Proveedores { get; set; }
      

    }
}
