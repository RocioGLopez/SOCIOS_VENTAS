using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PrototipoCompras.Migrations
{
    /// <inheritdoc />
    public partial class Historia7AnalisisCostos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CostosProveedorHistoricos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Producto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostosProveedorHistoricos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CostosProveedorHistoricos",
                columns: new[] { "Id", "FechaRegistro", "PrecioUnitario", "Producto", "Proveedor" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 14500m, "Laptop Dell", "TechSupply S.A." },
                    { 2, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 14800m, "Laptop Dell", "TechSupply S.A." },
                    { 3, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 14650m, "Laptop Dell", "TechSupply S.A." },
                    { 4, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 15050m, "Laptop Dell", "OfiMundo" },
                    { 5, new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 14900m, "Laptop Dell", "OfiMundo" },
                    { 6, new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 15100m, "Laptop Dell", "OfiMundo" },
                    { 7, new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 14380m, "Laptop Dell", "Distribuidora Central" },
                    { 8, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 14450m, "Laptop Dell", "Distribuidora Central" },
                    { 9, new DateTime(2026, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 14290m, "Laptop Dell", "Distribuidora Central" },
                    { 10, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1900m, "Monitor LG", "TechSupply S.A." },
                    { 11, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1940m, "Monitor LG", "TechSupply S.A." },
                    { 12, new DateTime(2026, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1925m, "Monitor LG", "TechSupply S.A." },
                    { 13, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1980m, "Monitor LG", "OfiMundo" },
                    { 14, new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 1965m, "Monitor LG", "OfiMundo" },
                    { 15, new DateTime(2026, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 1995m, "Monitor LG", "OfiMundo" },
                    { 16, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1880m, "Monitor LG", "Distribuidora Central" },
                    { 17, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1895m, "Monitor LG", "Distribuidora Central" },
                    { 18, new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 1875m, "Monitor LG", "Distribuidora Central" },
                    { 19, new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 620m, "Sillas de oficina", "TechSupply S.A." },
                    { 20, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 635m, "Sillas de oficina", "TechSupply S.A." },
                    { 21, new DateTime(2026, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 628m, "Sillas de oficina", "TechSupply S.A." },
                    { 22, new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 605m, "Sillas de oficina", "OfiMundo" },
                    { 23, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 615m, "Sillas de oficina", "OfiMundo" },
                    { 24, new DateTime(2026, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 610m, "Sillas de oficina", "OfiMundo" },
                    { 25, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 598m, "Sillas de oficina", "Distribuidora Central" },
                    { 26, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 602m, "Sillas de oficina", "Distribuidora Central" },
                    { 27, new DateTime(2026, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 600m, "Sillas de oficina", "Distribuidora Central" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostosProveedorHistoricos_Producto_Proveedor_FechaRegistro",
                table: "CostosProveedorHistoricos",
                columns: new[] { "Producto", "Proveedor", "FechaRegistro" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostosProveedorHistoricos");

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
    }
}
