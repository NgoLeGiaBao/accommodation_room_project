using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    /// <inheritdoc />
    public partial class V03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateInvoice",
                table: "Invoices",
                newName: "InvoiceDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoiceDate",
                table: "Invoices",
                newName: "DateInvoice");
        }
    }
}
