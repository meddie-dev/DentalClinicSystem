using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingToPrescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Billings_PrescriptionId",
                table: "Billings");

            migrationBuilder.CreateIndex(
                name: "IX_Billings_PrescriptionId",
                table: "Billings",
                column: "PrescriptionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Billings_PrescriptionId",
                table: "Billings");

            migrationBuilder.CreateIndex(
                name: "IX_Billings_PrescriptionId",
                table: "Billings",
                column: "PrescriptionId");
        }
    }
}
