using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PrototipoCompras.Migrations
{
    /// <inheritdoc />
    public partial class Historia8Inventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GeneradaAutomaticamente",
                table: "Solicitudes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InventarioProductoId",
                table: "Solicitudes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AlertasInventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventarioProductoId = table.Column<int>(type: "int", nullable: false),
                    Producto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StockActual = table.Column<int>(type: "int", nullable: false),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resuelta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasInventario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventarioProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioProductos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionesCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventarioProductoId = table.Column<int>(type: "int", nullable: true),
                    Destinatario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesCompra", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "InventarioProductos",
                columns: new[] { "Id", "Nombre", "Proveedor", "Stock", "StockMinimo", "UltimaActualizacion" },
                values: new object[,]
                {
                    { 1, "Laptop Dell", "TechSupply S.A.", 10, 5, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Sillas de oficina", "OfiMundo", 8, 4, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Monitor LG", "Distribuidora Central", 6, 3, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertasInventario");

            migrationBuilder.DropTable(
                name: "InventarioProductos");

            migrationBuilder.DropTable(
                name: "NotificacionesCompra");

            migrationBuilder.DropColumn(
                name: "GeneradaAutomaticamente",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "InventarioProductoId",
                table: "Solicitudes");
        }
    }
}
