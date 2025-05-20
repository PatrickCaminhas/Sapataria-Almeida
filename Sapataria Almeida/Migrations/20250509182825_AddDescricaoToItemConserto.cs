using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sapataria_Almeida.Migrations
{
    /// <inheritdoc />
    public partial class AddDescricaoToItemConserto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataEntrega",
                table: "ItensConserto",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "ItensConserto",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Sinal",
                table: "Consertos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinal",
                table: "Consertos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Consertos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataEntrega",
                table: "ItensConserto");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "ItensConserto");

            migrationBuilder.DropColumn(
                name: "DataFinal",
                table: "Consertos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Consertos");

            migrationBuilder.AlterColumn<float>(
                name: "Sinal",
                table: "Consertos",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }
    }
}
