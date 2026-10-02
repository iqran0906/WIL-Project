using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMCGEnterpriseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Inventories");
        }
    }
}
