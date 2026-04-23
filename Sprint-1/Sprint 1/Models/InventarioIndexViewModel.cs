namespace PrototipoCompras.Models;

public class InventarioIndexViewModel
{
    public List<InventarioProducto> Productos { get; set; } = new();
    public List<AlertaInventario> Alertas { get; set; } = new();
    public List<SolicitudCompra> SolicitudesPendientes { get; set; } = new();
    public List<NotificacionCompra> Notificaciones { get; set; } = new();
}