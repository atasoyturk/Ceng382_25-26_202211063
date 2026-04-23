using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_project.Migrations
{
    /// <inheritdoc />
    public partial class MenuCustomizationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaretakerID",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CustomizationGroups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomizationGroups_MenuItems_MenuID",
                        column: x => x.MenuID,
                        principalTable: "MenuItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomizationOptions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceModifier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CustomizationGroupID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationOptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomizationOptions_CustomizationGroups_CustomizationGroupID",
                        column: x => x.CustomizationGroupID,
                        principalTable: "CustomizationGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomizationGroups_MenuID",
                table: "CustomizationGroups",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizationOptions_CustomizationGroupID",
                table: "CustomizationOptions",
                column: "CustomizationGroupID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomizationOptions");

            migrationBuilder.DropTable(
                name: "CustomizationGroups");

            migrationBuilder.DropColumn(
                name: "CaretakerID",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "MenuItems");
        }
    }
}
