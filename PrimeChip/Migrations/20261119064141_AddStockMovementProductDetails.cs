using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeChip.Migrations
{
    /// <inheritdoc />
    public partial class AddStockMovementProductDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE movement SET movement.ProductName = inventory.Name, movement.Category = inventory.Category " +
                "FROM StockMovements AS movement " +
                "INNER JOIN Inventories AS inventory ON inventory.Id = movement.InventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "StockMovements");
        }
    }
}
