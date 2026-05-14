using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeChip.Migrations
{
    /// <inheritdoc />
    public partial class InitialStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InitialStock",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialStock",
                table: "Inventories");
        }
    }
}
