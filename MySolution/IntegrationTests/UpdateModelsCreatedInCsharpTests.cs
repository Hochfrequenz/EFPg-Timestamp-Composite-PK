using FluentAssertions;

namespace IntegrationTests;

using System.Linq;
using Xunit;
using DataModelAndMigration;


/// <summary>
///
/// THIS TEST IS ONLY FOR DEMO / BASIC UNDERSTANDING. IT DOES _NOT_ REPRODUCE THE BUG!
/// 
/// These tests demonstrate the models which have been created with the same version of the application which is now running are not affected by the problem.
/// Meaning: Any new model, even those with DatePartOfKeys close to MinValue work fine.
/// </summary>
public class UpdateModelsCreatedInCsharpTests : IClassFixture<EnsureMigratedFixture>
{
    /// <summary>
    /// A class that describes a single test parameter: An original model + an update value
    /// </summary>
    public class OriginalModelAndUpdatedValue
    {
        /// <summary>
        /// the state of the model that will be saved to the database initially
        /// </summary>
        public MyModel OriginalModel { get; set; }

        /// <summary>
        /// The value of <see cref="MyModel.SomeValue"/> that we'll try to update
        /// </summary>
        public string UpdatedValue { get; set; }
    }

    public UpdateModelsCreatedInCsharpTests(EnsureMigratedFixture fixture)
    {
    }

    /// <summary>
    /// The following data will workfine, altough the DatePartOfKey is close to 0.
    /// </summary>
    public static IEnumerable<object[]> ExampleDataWhichWorkFine()
    {
        yield return new object[]
        {
            new OriginalModelAndUpdatedValue
            {
                OriginalModel = new MyModel
                {
                    GuidPartOfKey = Guid.NewGuid(),
                    DatePartOfKey = DateTimeOffset.UtcNow,
                    SomeValue = "foo"
                },
                UpdatedValue = "bar"
            },
        };
        yield return new object[]
        {
            new OriginalModelAndUpdatedValue
            {
                OriginalModel = new MyModel
                {
                    GuidPartOfKey = Guid.NewGuid(),
                    DatePartOfKey = new DateTimeOffset(1, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    SomeValue = "foo"
                },
                UpdatedValue = "bar"
            }
        };
        yield return new object[]
        {
            new OriginalModelAndUpdatedValue
            {
                OriginalModel = new MyModel
                {
                    GuidPartOfKey = Guid.NewGuid(),
                    DatePartOfKey = DateTimeOffset.MinValue,
                    SomeValue = "foo"
                },
                UpdatedValue = "bar"
            }
        };
    }


    [Theory]
    [MemberData(nameof(ExampleDataWhichWorkFine))]
    public async Task Test_Newly_Created_Records_Can_Be_Updated_Regardless_Of_Their_DatePartOfKey(OriginalModelAndUpdatedValue originalAndUpdate)
    {
        var originalModel = originalAndUpdate.OriginalModel;
        var updateValue = originalAndUpdate.UpdatedValue;
        await using var context = new ApplicationContext();
        context.MyModels.Add(originalModel);
        await context.SaveChangesAsync();

        var modelToUpdate = context.MyModels.Single(m => m.GuidPartOfKey == originalModel.GuidPartOfKey);
        modelToUpdate.SomeValue = updateValue;
        await context.SaveChangesAsync();

        context.MyModels.Single(m => m.GuidPartOfKey == originalModel.GuidPartOfKey).SomeValue.Should().Be(updateValue);
    }
}
