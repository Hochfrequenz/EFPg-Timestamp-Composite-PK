namespace DataModelAndMigration;
using System.ComponentModel.DataAnnotations;



/// <summary>
/// MyModel has a composite primary key where one part of the key is a DateTimeOffset.
/// </summary>
public class MyModel
{
    /// <summary>
    /// Actually this guid part alone is enough to identify any MyModel record in this example. But we need the composite key/second date part to demonstrate the problem.
    /// </summary>
    [Key]
    public Guid GuidPartOfKey { get; set; }
    [Key]
    public DateTimeOffset DatePartOfKey { get; set; }

    /// <summary>
    /// This column is _not_ part of the primary key. It will be used for demo/testing purposes
    /// </summary>
    public string SomeValue { get; set; }
}
