using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_project.Migrations
{
    /// <inheritdoc />
    public partial class AddMinOrderQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinOrderQuantity",
                table: "MenuItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinOrderQuantity",
                table: "MenuItems");
        }
    }
}
