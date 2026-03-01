using AppConstants;

namespace DemonsAndDogs.Tests;

public class PropertyTypesTests
{
    [Fact]
    public void All_ContainsAllDefinedTypes()
    {
        var expected = new[] { "string", "int", "decimal", "bool", "enum", "datetime" };
        Assert.Equal(expected, PropertyTypes.All);
    }

    [Fact]
    public void All_HasNoDuplicates()
    {
        Assert.Equal(PropertyTypes.All.Length, PropertyTypes.All.Distinct().Count());
    }
}
