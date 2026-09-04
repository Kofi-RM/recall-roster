using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using recall_roster.Data;
using Xunit;

public class MigrationTests
{
    [Fact]
    public void Sql_server_migration_script_parses_and_preserves_legacy_data()
    {
        using var db = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
        var script = db.GetService<IMigrator>().GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent);
        new TSql160Parser(true).Parse(new StringReader(script), out var errors);
        Assert.True(errors.Count == 0, string.Join(Environment.NewLine,
            errors.Select(e => $"Line {e.Line}: {e.Message}")));
        Assert.Contains("LegacyTotal", script);
        Assert.Contains("IsDuplicate", script);
        Assert.DoesNotContain("DROP TABLE [Logins]", script);
        Assert.DoesNotContain("DELETE FROM dbo.Responses", script);
        Assert.True(script.IndexOf("LegacyContactsBaseline", StringComparison.Ordinal) <
            script.IndexOf("ContactModelUpdated", StringComparison.Ordinal));
    }

    [Fact]
    public void Sql_server_model_matches_migration_snapshot()
    {
        using var db = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
        Assert.False(db.Database.HasPendingModelChanges());
    }
}
