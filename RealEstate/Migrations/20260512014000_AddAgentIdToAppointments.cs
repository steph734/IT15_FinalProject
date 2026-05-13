using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentIdToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add AgentId column
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "Appointments",
                type: "int",
                nullable: true);

            // Drop the EmployeeId foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Employees_EmployeeId",
                table: "Appointments");

            // Add foreign key constraint for AgentId
            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Agents_AgentId",
                table: "Appointments",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove AgentId foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Agents_AgentId",
                table: "Appointments");

            // Add back EmployeeId foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Employees_EmployeeId",
                table: "Appointments",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            // Remove AgentId column
            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Appointments");
        }
    }
}
