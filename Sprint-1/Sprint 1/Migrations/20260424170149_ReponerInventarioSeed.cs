using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PrototipoCompras.Migrations
{
    /// <inheritdoc />
    public partial class ReponerInventarioSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
            migrationBuilder.DeleteData(
                table: "InventarioProductos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InventarioProductos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InventarioProductos",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
