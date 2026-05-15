using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototipoCompras.Migrations
{
    /// <inheritdoc />
    public partial class AgregarBitacora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bitacoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    Exitoso = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitacoras", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bitacoras_Fecha",
                table: "Bitacoras",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Bitacoras_Modulo_Usuario",
                table: "Bitacoras",
                columns: new[] { "Modulo", "Usuario" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bitacoras");
        }
    }
}
