using System.ComponentModel.DataAnnotations;

namespace PrototipoCompras.Models;
//dfsdfasda
public class Proveedor
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = "";
}