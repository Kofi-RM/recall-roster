using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace recall_roster.Migrations;
public partial class RecallIntegrity : Migration
{
    protected override void Up(MigrationBuilder m)
    {
        m.Sql("""
            IF OBJECT_ID(N'dbo.Contacts', N'U') IS NULL OR OBJECT_ID(N'dbo.Recalls', N'U') IS NULL
               OR OBJECT_ID(N'dbo.Responses', N'U') IS NULL OR OBJECT_ID(N'dbo.RosterContact', N'U') IS NULL
                THROW 51000, 'Missing legacy tables. Restore/baseline before RecallIntegrity.', 1;
            IF EXISTS (SELECT 1 FROM dbo.Recalls WHERE timeEnded>SYSUTCDATETIME())
                THROW 51000, 'Unfinished legacy recalls exist. Complete them before switching acknowledgment workflows.', 1;
            IF EXISTS (SELECT 1 FROM sys.triggers WHERE is_disabled=0 AND parent_id IN
                (OBJECT_ID(N'dbo.Recalls'), OBJECT_ID(N'dbo.Responses'), OBJECT_ID(N'dbo.RosterContact')))
                THROW 51000, 'Custom recall/response/roster triggers require review before migration.', 1;
            IF OBJECT_ID(N'dbo.[User]', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Logins', N'U') IS NOT NULL
                THROW 51000, 'Both User and Logins exist. Reconcile accounts without losing IDs or hashes.', 1;
            IF OBJECT_ID(N'dbo.[User]', N'U') IS NULL AND OBJECT_ID(N'dbo.Logins', N'U') IS NULL
                THROW 51000, 'No account table exists. Verify the legacy baseline.', 1;
            IF OBJECT_ID(N'dbo.[User]', N'U') IS NULL EXEC sp_rename N'dbo.Logins', N'User';
            """);
        m.Sql("""
            DECLARE @accountPk sysname;
            SELECT @accountPk=name FROM sys.key_constraints
                WHERE parent_object_id=OBJECT_ID(N'dbo.[User]') AND type=N'PK';
            IF @accountPk IS NULL THROW 51000, 'Account table has no primary key. Review the schema.', 1;
            IF @accountPk<>N'PK_User'
            BEGIN
                DECLARE @qualifiedPk nvarchar(260)=N'dbo.'+QUOTENAME(@accountPk);
                EXEC sp_rename @qualifiedPk, N'PK_User', N'OBJECT';
            END;
            """);
        m.Sql("""
            IF EXISTS (SELECT 1 FROM dbo.[User] WHERE Email IS NULL OR LEN(LTRIM(RTRIM(Email)))=0 OR DATALENGTH(Email)>640)
                THROW 51000, 'Invalid or oversized email. Reconcile before migration.', 1;
            IF EXISTS (SELECT UPPER(LTRIM(RTRIM(Email))) FROM dbo.[User]
                GROUP BY UPPER(LTRIM(RTRIM(Email))) HAVING COUNT(*)>1)
                THROW 51000, 'Duplicate normalized emails. Reconcile before migration.', 1;
            UPDATE dbo.[User] SET Email=UPPER(LTRIM(RTRIM(Email)));
            ALTER TABLE dbo.[User] ALTER COLUMN Email nvarchar(320) NOT NULL;
            IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.[User]')
                AND name=N'IX_User_Email' AND (is_unique=0 OR is_disabled=1 OR has_filter=1))
                THROW 51000, 'Existing email index is incompatible. Review the schema.', 1;
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.[User]') AND name=N'IX_User_Email')
                CREATE UNIQUE INDEX IX_User_Email ON dbo.[User](Email);
            """);
        m.Sql("""
            IF COL_LENGTH(N'dbo.Responses', N'response') IS NULL
                ALTER TABLE dbo.Responses ADD response nvarchar(max) NOT NULL CONSTRAINT DF_Responses_response DEFAULT N'';
            IF COL_LENGTH(N'dbo.Responses', N'IsDuplicate') IS NULL
                ALTER TABLE dbo.Responses ADD IsDuplicate bit NOT NULL CONSTRAINT DF_Responses_IsDuplicate DEFAULT 0;
            IF COL_LENGTH(N'dbo.Recalls', N'HasRecipientSnapshot') IS NULL
                ALTER TABLE dbo.Recalls ADD HasRecipientSnapshot bit NOT NULL CONSTRAINT DF_Recalls_HasRecipientSnapshot DEFAULT 0;
            """);
        // Archive duplicates rather than deleting any historical acknowledgment.
        m.Sql("""
            ;WITH ordered AS (
                SELECT IsDuplicate, ROW_NUMBER() OVER
                    (PARTITION BY recallId, contactId ORDER BY responseTime, responseId) AS ordinal
                FROM dbo.Responses
            )
            UPDATE ordered SET IsDuplicate=CASE WHEN ordinal=1 THEN 0 ELSE 1 END;
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Responses')
                AND name=N'IX_Responses_recallId_contactId')
                CREATE UNIQUE INDEX IX_Responses_recallId_contactId ON dbo.Responses(recallId,contactId) WHERE IsDuplicate=0;
            """);
        m.Sql("""
            IF COL_LENGTH(N'dbo.Recalls', N'LegacyTotal') IS NOT NULL
                THROW 51000, 'LegacyTotal already exists. Review partial/manual migration state.', 1;
            ALTER TABLE dbo.Recalls ADD LegacyTotal int NULL;
            """);
        m.Sql("""
            IF COL_LENGTH(N'dbo.Recalls', N'Total') IS NOT NULL
            BEGIN
                EXEC(N'UPDATE dbo.Recalls SET LegacyTotal=Total;');
                DECLARE @defaultName sysname;
                SELECT @defaultName=d.name FROM sys.default_constraints d
                JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id
                WHERE d.parent_object_id=OBJECT_ID(N'dbo.Recalls') AND c.name=N'Total';
                IF @defaultName IS NOT NULL
                BEGIN
                    DECLARE @dropDefault nvarchar(max)=N'ALTER TABLE dbo.Recalls DROP CONSTRAINT '+QUOTENAME(@defaultName);
                    EXEC(@dropDefault);
                END;
                ALTER TABLE dbo.Recalls DROP COLUMN Total;
            END;
            ALTER TABLE dbo.Recalls ADD Total AS ([Employees]+[ElementChief]+[FlightChief]+[SquadronDirector]);
            -- Keep old active values for audit; the app derives status from timestamps.
            IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.Recalls')
                AND name=N'active' AND is_computed=0 AND default_object_id=0)
                EXEC(N'ALTER TABLE dbo.Recalls ADD CONSTRAINT DF_Recalls_active DEFAULT 0 FOR active;');
            """);
        m.CreateTable(name: "RecallRecipients", columns: table => new
        {
            recallId=table.Column<int>(type:"int", nullable:false),
            contactId=table.Column<int>(type:"int", nullable:false),
            FirstName=table.Column<string>(type:"nvarchar(max)", nullable:false),
            LastName=table.Column<string>(type:"nvarchar(max)", nullable:false),
            PhoneNumber=table.Column<string>(type:"nvarchar(max)", nullable:false),
            Rank=table.Column<string>(type:"nvarchar(max)", nullable:false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_RecallRecipients", x => new { x.recallId, x.contactId });
            table.ForeignKey("FK_RecallRecipients_Recalls_recallId", x => x.recallId,
                "Recalls", "recallId", onDelete: ReferentialAction.Restrict);
        });
        m.Sql("""
            DECLARE @drop nvarchar(max)=N'';
            SELECT @drop=@drop+N'ALTER TABLE dbo.Responses DROP CONSTRAINT '+QUOTENAME(name)+N';'
            FROM sys.foreign_keys WHERE parent_object_id=OBJECT_ID(N'dbo.Responses')
                AND referenced_object_id IN (OBJECT_ID(N'dbo.Contacts'),OBJECT_ID(N'dbo.Recalls'));
            IF LEN(@drop)>0 EXEC sp_executesql @drop;
            ALTER TABLE dbo.Responses WITH CHECK ADD CONSTRAINT FK_Responses_Contacts_contactId
                FOREIGN KEY(contactId) REFERENCES dbo.Contacts(contactID) ON DELETE NO ACTION;
            ALTER TABLE dbo.Responses WITH CHECK ADD CONSTRAINT FK_Responses_Recalls_recallId
                FOREIGN KEY(recallId) REFERENCES dbo.Recalls(recallId) ON DELETE NO ACTION;
            """);
    }
    protected override void Down(MigrationBuilder migrationBuilder) =>
        throw new NotSupportedException("Forward-only migration: restore a verified backup to roll back without losing recipient history.");
}
