using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Models;

namespace PrototipoCompras.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<SolicitudCompra> Solicitudes => Set<SolicitudCompra>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<EvaluacionProveedor> Evaluaciones => Set<EvaluacionProveedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proveedor>().HasData(
            new Proveedor { Id = 1, Nombre = "TechSupply S.A." },
            new Proveedor { Id = 2, Nombre = "OfiMundo" },
            new Proveedor { Id = 3, Nombre = "Distribuidora Central" }
        );
    }
}