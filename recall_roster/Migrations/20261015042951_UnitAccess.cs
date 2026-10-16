using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recall_roster.Migrations
{
    /// <inheritdoc />
    public partial class UnitAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemAdministrator",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Rosters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Recalls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Recalls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccessAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorUserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ParentUnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Units_ParentUnitId",
                        column: x => x.ParentUnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitMemberships",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    CanManageRosters = table.Column<bool>(type: "bit", nullable: false),
                    CanInitiateRecalls = table.Column<bool>(type: "bit", nullable: false),
                    IncludeDescendants = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitMemberships", x => new { x.UserId, x.UnitId });
                    table.ForeignKey(
                        name: "FK_UnitMemberships_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitMemberships_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Preserve all existing records in an explicitly unassigned legacy unit.
            // No memberships or administrator permissions are inferred from existing accounts.
            migrationBuilder.InsertData("Units", new[] { "Id", "Name", "ParentUnitId" }, new object[] { 1, "Legacy / unassigned unit", null });
            migrationBuilder.Sql("UPDATE Contacts SET UnitId = 1; UPDATE Rosters SET UnitId = 1; UPDATE Recalls SET UnitId = 1;");

            migrationBuilder.CreateIndex(
                name: "IX_Rosters_UnitId",
                table: "Rosters",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_CreatedByUserId",
                table: "Recalls",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_UnitId",
                table: "Recalls",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UnitId",
                table: "Contacts",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitMemberships_UnitId",
                table: "UnitMemberships",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_ParentUnitId",
                table: "Units",
                column: "ParentUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Units_UnitId",
                table: "Contacts",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recalls_Units_UnitId",
                table: "Recalls",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recalls_User_CreatedByUserId",
                table: "Recalls",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rosters_Units_UnitId",
                table: "Rosters",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException("Unit ownership and access history must not be discarded. Restore a verified backup to roll back.");
        }
    }
}
