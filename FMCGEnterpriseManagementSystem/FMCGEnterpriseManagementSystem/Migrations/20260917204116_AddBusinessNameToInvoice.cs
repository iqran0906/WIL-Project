using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMCGEnterpriseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessNameToInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "Invoices");
        }
    }
}
