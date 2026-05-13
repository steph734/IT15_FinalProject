using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AgentId",
                table: "Notifications",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Agents_AgentId",
                table: "Notifications",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Agents_AgentId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_AgentId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Notifications");
        }
    }
}
