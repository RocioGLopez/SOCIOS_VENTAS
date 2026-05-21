using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PrototipoCompras.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogoProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntregaRequerida",
                table: "Solicitudes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CatalogoProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StockDisponible = table.Column<int>(type: "int", nullable: false),
                    CantidadMinima = table.Column<int>(type: "int", nullable: false),
                    CantidadMaxima = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogoProductos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CatalogoProductos",
                columns: new[] { "Id", "Activo", "CantidadMaxima", "CantidadMinima", "Categoria", "FechaCreacion", "Nombre", "StockDisponible" },
                values: new object[,]
                {
                    { 1, true, 25, 1, "Periféricos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monitor", 50 },
                    { 2, true, 25, 1, "Computadoras", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CPU", 30 },
                    { 3, true, 25, 1, "Computadoras", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Laptop", 20 },
                    { 4, true, 25, 1, "Periféricos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Teclado", 80 },
                    { 5, true, 25, 1, "Periféricos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mouse", 80 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogoProductos");

            migrationBuilder.DropColumn(
                name: "FechaEntregaRequerida",
                table: "Solicitudes");
        }
    }
}
