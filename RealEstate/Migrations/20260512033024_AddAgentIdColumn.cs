using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AgentId",
                table: "Appointments",
                column: "AgentId");

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
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Agents_AgentId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AgentId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Appointments");
        }
    }
}
