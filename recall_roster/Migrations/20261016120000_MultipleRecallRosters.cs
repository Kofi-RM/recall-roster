using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using recall_roster.Data;
namespace recall_roster.Migrations;
[DbContext(typeof(AppDbContext))]
[Migration("20261016120000_MultipleRecallRosters")]
public partial class MultipleRecallRosters : Migration
{
    protected override void Up(MigrationBuilder m) => m.AddColumn<string>(name: "RosterIdsJson", table: "Recalls", type: "nvarchar(max)", nullable: true);
    protected override void Down(MigrationBuilder m) => m.DropColumn(name: "RosterIdsJson", table: "Recalls");
}
