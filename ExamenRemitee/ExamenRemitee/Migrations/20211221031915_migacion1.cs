using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExamenRemitee.Migrations
{
    public partial class migacion1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    NroOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MonedaOrigen = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MonedaDestino = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MonedaOrigenMonto = table.Column<double>(type: "float", nullable: false),
                    MonedaDestinoMonto = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.NroOrden);
                });

            migrationBuilder.CreateTable(
                name: "Registros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exito = table.Column<bool>(type: "bit", nullable: false),
                    TerminosURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoliticaPrivacidad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MonedaFuente = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    registroId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Registros_registroId",
                        column: x => x.registroId,
                        principalTable: "Registros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_registroId",
                table: "Cotizaciones",
                column: "registroId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "Registros");
        }
    }
}
