using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using recall_roster.Data;
namespace recall_roster.Migrations;

// The original migration assumed Contacts already existed outside EF.
[DbContext(typeof(AppDbContext))]
[Migration("20240401000000_LegacyContactsBaseline")]
public class LegacyContactsBaseline : Migration
{
    protected override void Up(MigrationBuilder m) => m.Sql("""
        IF OBJECT_ID(N'dbo.Contacts', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Contacts (
                ContactID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Contacts PRIMARY KEY,
                FirstName nvarchar(max) NOT NULL,
                LastName nvarchar(max) NOT NULL,
                PhoneNumber nvarchar(max) NOT NULL
            );
        END;
        """);
    protected override void Down(MigrationBuilder m) =>
        throw new NotSupportedException("Baseline rollback risks existing Contacts. Restore a verified backup instead.");
}
