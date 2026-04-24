using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoCompras.Data;
using PrototipoCompras.Models;
using System.Diagnostics;

namespace PrototipoCompras.Controllers;

[Authorize]
public class ReportesController : Controller
{
    private readonly AppDbContext _db;

    public ReportesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> DashboardCostos(string? productoSeleccionado, List<string>? proveedoresSeleccionados)
    {
        var sw = Stopwatch.StartNew();

        var model = new DashboardCostosViewModel
        {
            ProductoSeleccionado = productoSeleccionado,
            ProveedoresSeleccionados = proveedoresSeleccionados ?? new List<string>(),
            ProductosDisponibles = await _db.CostosProveedorHistoricos
                .AsNoTracking()
                .Select(x => x.Producto)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(),

            ProveedoresDisponibles = await _db.CostosProveedorHistoricos
                .AsNoTracking()
                .Select(x => x.Proveedor)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync()
        };

        if (!string.IsNullOrWhiteSpace(productoSeleccionado) && model.ProveedoresSeleccionados.Any())
        {
            if (model.ProveedoresSeleccionados.Count < 2)
            {
                model.Error = "Debes seleccionar al menos dos proveedores.";
            }
            else
            {
                var datos = await _db.CostosProveedorHistoricos
                    .AsNoTracking()
                    .Where(x => x.Producto == productoSeleccionado && model.ProveedoresSeleccionados.Contains(x.Proveedor))
                    .OrderByDescending(x => x.FechaRegistro)
                    .ToListAsync();

                model.Comparativos = datos
                    .GroupBy(x => x.Proveedor)
                    .Select(g =>
                    {
                        var ultimo = g.OrderByDescending(x => x.FechaRegistro).First();
                        var promedio = g.Average(x => x.PrecioUnitario);

                        return new ComparativoCostoProveedorViewModel
                        {
                            Proveedor = g.Key,
                            PrecioActual = ultimo.PrecioUnitario,
                            PromedioHistorico = Math.Round(promedio, 2),
                            DiferenciaVsPromedio = Math.Round(ultimo.PrecioUnitario - promedio, 2),
                            UltimaFecha = ultimo.FechaRegistro
                        };
                    })
                    .OrderBy(x => x.PrecioActual)
                    .ToList();

                if (model.Comparativos.Any())
                {
                    var mejor = model.Comparativos.OrderBy(x => x.PrecioActual).First();
                    model.MejorProveedor = mejor.Proveedor;
                    model.MejorPrecio = mejor.PrecioActual;
                    model.PromedioGeneralHistorico = Math.Round(model.Comparativos.Average(x => x.PromedioHistorico), 2);
                    model.DiferenciaMaxMinActual = model.Comparativos.Max(x => x.PrecioActual) - model.Comparativos.Min(x => x.PrecioActual);
                }
            }
        }

        sw.Stop();
        model.TiempoMs = (int)sw.ElapsedMilliseconds;

        return View(model);
    }
}