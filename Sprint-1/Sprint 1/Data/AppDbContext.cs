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
    public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();

    public DbSet<CostoProveedorHistorico> CostosProveedorHistoricos => Set<CostoProveedorHistorico>();

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

        modelBuilder.Entity<CostoProveedorHistorico>()
            .HasIndex(c => new { c.Producto, c.Proveedor, c.FechaRegistro });

        modelBuilder.Entity<CostoProveedorHistorico>()
            .Property(c => c.PrecioUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CostoProveedorHistorico>().HasData(
            new CostoProveedorHistorico { Id = 1, Producto = "Laptop Dell", Proveedor = "TechSupply S.A.", PrecioUnitario = 14500m, FechaRegistro = new DateTime(2026, 1, 10) },
            new CostoProveedorHistorico { Id = 2, Producto = "Laptop Dell", Proveedor = "TechSupply S.A.", PrecioUnitario = 14800m, FechaRegistro = new DateTime(2026, 2, 15) },
            new CostoProveedorHistorico { Id = 3, Producto = "Laptop Dell", Proveedor = "TechSupply S.A.", PrecioUnitario = 14650m, FechaRegistro = new DateTime(2026, 3, 20) },

            new CostoProveedorHistorico { Id = 4, Producto = "Laptop Dell", Proveedor = "OfiMundo", PrecioUnitario = 15050m, FechaRegistro = new DateTime(2026, 1, 12) },
            new CostoProveedorHistorico { Id = 5, Producto = "Laptop Dell", Proveedor = "OfiMundo", PrecioUnitario = 14900m, FechaRegistro = new DateTime(2026, 2, 18) },
            new CostoProveedorHistorico { Id = 6, Producto = "Laptop Dell", Proveedor = "OfiMundo", PrecioUnitario = 15100m, FechaRegistro = new DateTime(2026, 3, 22) },

            new CostoProveedorHistorico { Id = 7, Producto = "Laptop Dell", Proveedor = "Distribuidora Central", PrecioUnitario = 14380m, FechaRegistro = new DateTime(2026, 1, 9) },
            new CostoProveedorHistorico { Id = 8, Producto = "Laptop Dell", Proveedor = "Distribuidora Central", PrecioUnitario = 14450m, FechaRegistro = new DateTime(2026, 2, 19) },
            new CostoProveedorHistorico { Id = 9, Producto = "Laptop Dell", Proveedor = "Distribuidora Central", PrecioUnitario = 14290m, FechaRegistro = new DateTime(2026, 3, 25) },

            new CostoProveedorHistorico { Id = 10, Producto = "Monitor LG", Proveedor = "TechSupply S.A.", PrecioUnitario = 1900m, FechaRegistro = new DateTime(2026, 1, 11) },
            new CostoProveedorHistorico { Id = 11, Producto = "Monitor LG", Proveedor = "TechSupply S.A.", PrecioUnitario = 1940m, FechaRegistro = new DateTime(2026, 2, 14) },
            new CostoProveedorHistorico { Id = 12, Producto = "Monitor LG", Proveedor = "TechSupply S.A.", PrecioUnitario = 1925m, FechaRegistro = new DateTime(2026, 3, 17) },

            new CostoProveedorHistorico { Id = 13, Producto = "Monitor LG", Proveedor = "OfiMundo", PrecioUnitario = 1980m, FechaRegistro = new DateTime(2026, 1, 13) },
            new CostoProveedorHistorico { Id = 14, Producto = "Monitor LG", Proveedor = "OfiMundo", PrecioUnitario = 1965m, FechaRegistro = new DateTime(2026, 2, 16) },
            new CostoProveedorHistorico { Id = 15, Producto = "Monitor LG", Proveedor = "OfiMundo", PrecioUnitario = 1995m, FechaRegistro = new DateTime(2026, 3, 19) },

            new CostoProveedorHistorico { Id = 16, Producto = "Monitor LG", Proveedor = "Distribuidora Central", PrecioUnitario = 1880m, FechaRegistro = new DateTime(2026, 1, 15) },
            new CostoProveedorHistorico { Id = 17, Producto = "Monitor LG", Proveedor = "Distribuidora Central", PrecioUnitario = 1895m, FechaRegistro = new DateTime(2026, 2, 20) },
            new CostoProveedorHistorico { Id = 18, Producto = "Monitor LG", Proveedor = "Distribuidora Central", PrecioUnitario = 1875m, FechaRegistro = new DateTime(2026, 3, 26) },

            new CostoProveedorHistorico { Id = 19, Producto = "Sillas de oficina", Proveedor = "TechSupply S.A.", PrecioUnitario = 620m, FechaRegistro = new DateTime(2026, 1, 8) },
            new CostoProveedorHistorico { Id = 20, Producto = "Sillas de oficina", Proveedor = "TechSupply S.A.", PrecioUnitario = 635m, FechaRegistro = new DateTime(2026, 2, 12) },
            new CostoProveedorHistorico { Id = 21, Producto = "Sillas de oficina", Proveedor = "TechSupply S.A.", PrecioUnitario = 628m, FechaRegistro = new DateTime(2026, 3, 18) },

            new CostoProveedorHistorico { Id = 22, Producto = "Sillas de oficina", Proveedor = "OfiMundo", PrecioUnitario = 605m, FechaRegistro = new DateTime(2026, 1, 7) },
            new CostoProveedorHistorico { Id = 23, Producto = "Sillas de oficina", Proveedor = "OfiMundo", PrecioUnitario = 615m, FechaRegistro = new DateTime(2026, 2, 13) },
            new CostoProveedorHistorico { Id = 24, Producto = "Sillas de oficina", Proveedor = "OfiMundo", PrecioUnitario = 610m, FechaRegistro = new DateTime(2026, 3, 21) },

            new CostoProveedorHistorico { Id = 25, Producto = "Sillas de oficina", Proveedor = "Distribuidora Central", PrecioUnitario = 598m, FechaRegistro = new DateTime(2026, 1, 6) },
            new CostoProveedorHistorico { Id = 26, Producto = "Sillas de oficina", Proveedor = "Distribuidora Central", PrecioUnitario = 602m, FechaRegistro = new DateTime(2026, 2, 11) },
            new CostoProveedorHistorico { Id = 27, Producto = "Sillas de oficina", Proveedor = "Distribuidora Central", PrecioUnitario = 600m, FechaRegistro = new DateTime(2026, 3, 24) }
        );
    }
}