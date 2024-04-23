using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recall_roster.Migrations
{
    /// <inheritdoc />
    public partial class ContactModelUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactID",
                table: "Contacts",
                newName: "contactID");

            migrationBuilder.AddColumn<int>(
                name: "Active",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Rank",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recalls",
                columns: table => new
                {
                    recallId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rosterId = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timeStarted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    timeEnded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Employees = table.Column<int>(type: "int", nullable: false),
                    ElementChief = table.Column<int>(type: "int", nullable: false),
                    FlightChief = table.Column<int>(type: "int", nullable: false),
                    SquadronDirector = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    EmployeesMax = table.Column<int>(type: "int", nullable: false),
                    ElementChiefMax = table.Column<int>(type: "int", nullable: false),
                    FlightChiefMax = table.Column<int>(type: "int", nullable: false),
                    SquadronDirectorMax = table.Column<int>(type: "int", nullable: false),
                    TotalMax = table.Column<int>(type: "int", nullable: false),
                    active = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recalls", x => x.recallId);
                });

            migrationBuilder.CreateTable(
                name: "Rosters",
                columns: table => new
                {
                    rosterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rosters", x => x.rosterId);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    responseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contactId = table.Column<int>(type: "int", nullable: false),
                    recallId = table.Column<int>(type: "int", nullable: false),
                    responseTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.responseId);
                    table.ForeignKey(
                        name: "FK_Responses_Contacts_contactId",
                        column: x => x.contactId,
                        principalTable: "Contacts",
                        principalColumn: "contactID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Responses_Recalls_recallId",
                        column: x => x.recallId,
                        principalTable: "Recalls",
                        principalColumn: "recallId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RosterContact",
                columns: table => new
                {
                    rosterId = table.Column<int>(type: "int", nullable: false),
                    contactId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterContact", x => new { x.rosterId, x.contactId });
                    table.ForeignKey(
                        name: "FK_RosterContact_Contacts_contactId",
                        column: x => x.contactId,
                        principalTable: "Contacts",
                        principalColumn: "contactID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterContact_Rosters_rosterId",
                        column: x => x.rosterId,
                        principalTable: "Rosters",
                        principalColumn: "rosterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Responses_contactId",
                table: "Responses",
                column: "contactId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_recallId",
                table: "Responses",
                column: "recallId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterContact_contactId",
                table: "RosterContact",
                column: "contactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Responses");

            migrationBuilder.DropTable(
                name: "RosterContact");

            migrationBuilder.DropTable(
                name: "Recalls");

            migrationBuilder.DropTable(
                name: "Rosters");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Contacts");

            migrationBuilder.RenameColumn(
                name: "contactID",
                table: "Contacts",
                newName: "ContactID");
        }
    }
}
