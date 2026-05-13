using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class AddBrokerLocationToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BrokerLatitude",
                table: "Appointments",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BrokerLocationLastUpdated",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BrokerLongitude",
                table: "Appointments",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrokerLatitude",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "BrokerLocationLastUpdated",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "BrokerLongitude",
                table: "Appointments");
        }
    }
}
