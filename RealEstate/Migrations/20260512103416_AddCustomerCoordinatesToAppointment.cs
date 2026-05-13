using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCoordinatesToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CustomerLatitude",
                table: "Appointments",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomerLongitude",
                table: "Appointments",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerLatitude",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CustomerLongitude",
                table: "Appointments");
        }
    }
}
