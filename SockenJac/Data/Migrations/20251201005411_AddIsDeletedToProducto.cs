using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SockenJac.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Productos");
        }
    }
}
