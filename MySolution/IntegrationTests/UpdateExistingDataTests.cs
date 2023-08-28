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

    public class GuidCanBeUpdatedTuple
    {
        /// <summary>
        /// the <see cref="MyModel.GuidPartOfKey"/>
        /// </summary>
        public Guid GuidOfModel { get; set; }

        /// <summary>
        /// true for those values that are _not_ affected by the bug, false for the that are
        /// </summary>
        public bool CanBeUpdated { get; set; }
    }

    /// <summary>
    /// The following updates will try to update values which have been created on the database using raw SQL beforehand.
    /// Their guid alone is enough to identify them.
    /// </summary>
    public static IEnumerable<object[]> ExampleDataCreatedFromRawSql()
    {
        // find the guids in upsert_example_data.sql
        yield return new object[]
        {
            new GuidCanBeUpdatedTuple
            {
                GuidOfModel = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                CanBeUpdated = false,
            }
        };
        yield return new object[]
        {
            new GuidCanBeUpdatedTuple
            {
                GuidOfModel = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                CanBeUpdated = true,
            }
        };
    }


    [Theory]
    [MemberData(nameof(ExampleDataCreatedFromRawSql))]
    public async Task Test_Records_With_0001_DateTime_Instead_of_Minus_Infinity_Are_Found_And_Indistinguishable_But_Cannot_Be_Updated(GuidCanBeUpdatedTuple guidCanBeUpdatedTuple)
    {
        await using var context = new ApplicationContext();
        var guidOfModel = guidCanBeUpdatedTuple.GuidOfModel;
        var modelToUpdate = context.MyModels.Single(m => m.GuidPartOfKey == guidOfModel);
        modelToUpdate.DatePartOfKey.Should().Be(DateTimeOffset.MinValue); // note all models have the _same_ C# DatePartOfKey. They're indistinguishable here.
        modelToUpdate.SomeValue = "something else";

        Func<Task> tryingToUpdateTheEntryWith0001Key = async () => await context.SaveChangesAsync();
        if (guidCanBeUpdatedTuple.CanBeUpdated)
        {
            // we run into this path for the '-infinity' case
            await tryingToUpdateTheEntryWith0001Key.Should().NotThrowAsync();
            // now let's verify that the value has been updated
            context.MyModels.Single(m => m.GuidPartOfKey == guidOfModel).SomeValue.Should().Be("something else");
        }
        else
        {
            // run run into this path for the '0001-01-01 ...' case
            await tryingToUpdateTheEntryWith0001Key.Should().ThrowAsync<DbUpdateConcurrencyException>()
                .WithMessage(
                    "The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. *");
            // This is the bug.
            // Actually the data have _not_ been modified or deleted. The just weren't found under "-infinity".
        }
    }
}
