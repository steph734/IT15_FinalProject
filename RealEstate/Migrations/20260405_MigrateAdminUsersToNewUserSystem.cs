using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Migrations
{
    /// <inheritdoc />
    public partial class MigrateAdminUsersToNewUserSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create new Users table with unique ID for all user types
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPasswordChanged = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            // Create indexes for performance
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                table: "Users",
                column: "Status");

            // Create type-specific tables using Table-Per-Type pattern
            migrationBuilder.CreateTable(
                name: "SuperAdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    AdminPermissions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperAdminUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuperAdminUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagerUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TeamSize = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountingUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    AccountingDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccessLevel = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestorUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    InvestmentBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreferredLocations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PropertyPreferences = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrokerUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Agency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokerUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate existing AdminUsers to new Users table as SuperAdmin
            migrationBuilder.Sql(@"
                INSERT INTO [Users] 
                ([FullName], [Email], [Username], [PasswordHash], [Role], [Status], [CreatedAt], [EmailVerified], [Discriminator])
                SELECT 
                    [FullName],
                    [Email],
                    [Username],
                    [PasswordHash],
                    'SuperAdmin' as [Role],
                    'Active' as [Status],
                    ISNULL([CreatedAt], GETUTCDATE()) as [CreatedAt],
                    0 as [EmailVerified],
                    'SuperAdminUser' as [Discriminator]
                FROM [AdminUsers_Legacy]
            ");

            // Insert corresponding records into SuperAdminUsers table
            migrationBuilder.Sql(@"
                INSERT INTO [SuperAdminUsers] ([Id], [AdminPermissions])
                SELECT [Id], 'ALL' FROM [Users] WHERE [Discriminator] = 'SuperAdminUser'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new tables
            migrationBuilder.DropTable(name: "BrokerUsers");
            migrationBuilder.DropTable(name: "InvestorUsers");
            migrationBuilder.DropTable(name: "AccountingUsers");
            migrationBuilder.DropTable(name: "ManagerUsers");
            migrationBuilder.DropTable(name: "SuperAdminUsers");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
