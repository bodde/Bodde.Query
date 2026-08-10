using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataParser_ParseValue
{
    [Theory]
    [InlineData("null", null, typeof(object))]
    [InlineData("true", true, typeof(bool))]
    [InlineData("false", false, typeof(bool))]
    [InlineData("True", true, typeof(bool))]
    [InlineData("False", false, typeof(bool))]
    [InlineData("123", 123, typeof(int))]
    [InlineData("123.45", 123.45, typeof(double))]
    [InlineData("'Hello, World!'", "Hello, World!", typeof(string))]
    public void ParseValue_ValidInputs(string input, object? expectedValue, Type expectedType)
    {
        var (result, type) = ODataParser.ParseValue(input);
        Assert.Equal(expectedValue, result);
        Assert.Equal(expectedType, type);
    }

    [Fact]
    public void ParseValue_UncknownInputType_ThrowsNotImplementedException()
    {
        var input = "unknownTypeValue";

        Assert.Throws<NotImplementedException>(() => ODataParser.ParseValue(input));
    }
}
