using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sapataria_Almeida.Migrations
{
    /// <inheritdoc />
    public partial class ColunasFinalizacaoConserto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MetodoPagamento",
                table: "Consertos",
                newName: "ValorPagamento");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRetirada",
                table: "Consertos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MetodoPagamentoFinal",
                table: "Consertos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MetodoPagamentoSinal",
                table: "Consertos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mensagem = table.Column<string>(type: "TEXT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Lida = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "DataRetirada",
                table: "Consertos");

            migrationBuilder.DropColumn(
                name: "MetodoPagamentoFinal",
                table: "Consertos");

            migrationBuilder.DropColumn(
                name: "MetodoPagamentoSinal",
                table: "Consertos");

            migrationBuilder.RenameColumn(
                name: "ValorPagamento",
                table: "Consertos",
                newName: "MetodoPagamento");
        }
    }
}
