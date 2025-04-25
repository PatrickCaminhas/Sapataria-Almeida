using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sapataria_Almeida.Migrations
{
    /// <inheritdoc />
    public partial class AddSinalToConserto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consertos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataAbertura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetodoPagamento = table.Column<string>(type: "TEXT", nullable: false),
                    Sinal = table.Column<float>(type: "REAL", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consertos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consertos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensConserto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoConserto = table.Column<string>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    ConsertoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensConserto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensConserto_Consertos_ConsertoId",
                        column: x => x.ConsertoId,
                        principalTable: "Consertos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consertos_ClienteId",
                table: "Consertos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensConserto_ConsertoId",
                table: "ItensConserto",
                column: "ConsertoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensConserto");

            migrationBuilder.DropTable(
                name: "Consertos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
