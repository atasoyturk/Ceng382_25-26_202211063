using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_project.Migrations
{
    /// <inheritdoc />
    public partial class OrderSystemUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "OrderItemCustomizations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemID = table.Column<int>(type: "int", nullable: false),
                    CustomizationOptionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemCustomizations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderItemCustomizations_CustomizationOptions_CustomizationOptionID",
                        column: x => x.CustomizationOptionID,
                        principalTable: "CustomizationOptions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_OrderItemCustomizations_OrderItems_OrderItemID",
                        column: x => x.OrderItemID,
                        principalTable: "OrderItems",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemCustomizations_CustomizationOptionID",
                table: "OrderItemCustomizations",
                column: "CustomizationOptionID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemCustomizations_OrderItemID",
                table: "OrderItemCustomizations",
                column: "OrderItemID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemCustomizations");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "OrderItems");
        }
    }
}
