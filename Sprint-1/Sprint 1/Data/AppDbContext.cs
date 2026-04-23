using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Models;

namespace PrototipoCompras.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SolicitudCompra> Solicitudes => Set<SolicitudCompra>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<EvaluacionProveedor> Evaluaciones => Set<EvaluacionProveedor>();
    public DbSet<InventarioProducto> InventarioProductos => Set<InventarioProducto>();
    public DbSet<AlertaInventario> AlertasInventario => Set<AlertaInventario>();
    public DbSet<NotificacionCompra> NotificacionesCompra => Set<NotificacionCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Proveedor>().HasData(
            new Proveedor { Id = 1, Nombre = "TechSupply S.A." },
            new Proveedor { Id = 2, Nombre = "OfiMundo" },
            new Proveedor { Id = 3, Nombre = "Distribuidora Central" }
        );

        modelBuilder.Entity<InventarioProducto>().HasData(
            new InventarioProducto
            {
                Id = 1,
                Nombre = "Laptop Dell",
                Proveedor = "TechSupply S.A.",
                Stock = 10,
                StockMinimo = 5,
                UltimaActualizacion = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc)
            },
            new InventarioProducto
            {
                Id = 2,
                Nombre = "Sillas de oficina",
                Proveedor = "OfiMundo",
                Stock = 8,
                StockMinimo = 4,
                UltimaActualizacion = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc)
            },
            new InventarioProducto
            {
                Id = 3,
                Nombre = "Monitor LG",
                Proveedor = "Distribuidora Central",
                Stock = 6,
                StockMinimo = 3,
                UltimaActualizacion = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}