using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleTransactionsAndAgentProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionRules_Users_ManagerId",
                table: "CommissionRules");

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SoldDate",
                table: "Properties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "CommissionRules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "AgentSplitPercent",
                table: "CommissionRules",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanySplitPercent",
                table: "CommissionRules",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumDealThreshold",
                table: "CommissionRules",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CommissionRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaleTransactions",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    TotalContractPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPool = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BrokerEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AgentEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BrokerSplitPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AgentSplitPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleTransactions", x => x.SaleId);
                    table.ForeignKey(
                        name: "FK_SaleTransactions_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaleTransactions_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleTransactions_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.UpdateData(
                table: "CommissionRules",
                keyColumn: "RuleId",
                keyValue: 1,
                columns: new[] { "AgentSplitPercent", "CompanySplitPercent", "MinimumDealThreshold", "UpdatedAt" },
                values: new object[] { 40m, 2.0m, 50000m, null });

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransactions_AgentId",
                table: "SaleTransactions",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransactions_ManagerId",
                table: "SaleTransactions",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransactions_PropertyId",
                table: "SaleTransactions",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionRules_Users_ManagerId",
                table: "CommissionRules",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionRules_Users_ManagerId",
                table: "CommissionRules");

            migrationBuilder.DropTable(
                name: "SaleTransactions");

            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "SoldDate",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "AgentSplitPercent",
                table: "CommissionRules");

            migrationBuilder.DropColumn(
                name: "CompanySplitPercent",
                table: "CommissionRules");

            migrationBuilder.DropColumn(
                name: "MinimumDealThreshold",
                table: "CommissionRules");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CommissionRules");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "CommissionRules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionRules_Users_ManagerId",
                table: "CommissionRules",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
