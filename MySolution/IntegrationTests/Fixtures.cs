using DataModelAndMigration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IntegrationTests;

public class EnsureMigratedFixture
{
    public EnsureMigratedFixture()
    {
        using var context = new ApplicationContext();
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }
        else
        {
            // Get the migrator service from the context
            var migrator = context.GetService<IMigrator>();

            // Check if there are any pending migrations
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                // Apply any pending migrations
                migrator.Migrate();
            }
        }
    }
}

public class InsertDataBeforeTestFixture
{
    public InsertDataBeforeTestFixture()
    {
        using var context = new ApplicationContext();
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }
        else
        {
            // Get the migrator service from the context
            var migrator = context.GetService<IMigrator>();

            // Check if there are any pending migrations
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                // Apply any pending migrations
                migrator.Migrate();
            }
        }

        var upsert_sql = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "upsert_example_data.sql"));
        context.Database.ExecuteSqlRaw(upsert_sql);
        context.MyModels.Any(m => m.GuidPartOfKey == Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")).Should().BeTrue();
        context.MyModels.Any(m => m.GuidPartOfKey == Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")).Should().BeTrue();
    }
}
