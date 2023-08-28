using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests;

using System.Linq;
using Xunit;
using DataModelAndMigration;

public class UpdateModelsCreatedInRawSql : IClassFixture<InsertDataBeforeTestFixture>
{
    public UpdateModelsCreatedInRawSql(InsertDataBeforeTestFixture fixture)
    {
    }

    /// <summary>
    /// The following updates will try to update values which have been created on the database using raw SQL beforehand.
    /// Their guid alone is enough to identify them.
    /// </summary>
    public static IEnumerable<object[]> ExampleDataCreatedFromRawSql()
    {
        yield return new object[]
        {
            Guid.Parse("5ed0b229-4ab6-45fc-94c1-f7e6c69f9857"), // see upsert_example_data.sql
        };
    }


    [Theory]
    [MemberData(nameof(ExampleDataCreatedFromRawSql))]
    public async Task Test_Records_With_0001_DateTime_Instead_of_Minus_Infinity_Are_Found_And_Indistinguishable_But_Cannot_Be_Updated(Guid guidOfExistingEntry)
    {
        await using var context = new ApplicationContext();
        var modelToUpdate = context.MyModels.Single(m => m.GuidPartOfKey == guidOfExistingEntry);
        modelToUpdate.DatePartOfKey.Should().Be(DateTimeOffset.MinValue,
            because: "Through 'EF eyes' this entry looks the same as the one with '-infinity', although on the DB there's actually '0001-01-01 00:00:00.000000 +00:00'");

        modelToUpdate.SomeValue = "something else";

        Func<Task> tryingToUpdateTheEntryWith0001Key = async () => await context.SaveChangesAsync();

        await tryingToUpdateTheEntryWith0001Key.Should().ThrowAsync<DbUpdateConcurrencyException>()
            .WithMessage(
                "The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. *");

        // Actually the data have _not_ been modified or deleted. The just weren't found under "-infinity".
    }
}
