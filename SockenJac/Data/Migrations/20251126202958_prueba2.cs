using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SockenJac.Data.Migrations
{
    /// <inheritdoc />
    public partial class prueba2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "foto",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "foto",
                table: "Compras",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "foto",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "foto",
                table: "Compras");
        }
    }
}
