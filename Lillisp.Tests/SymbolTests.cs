namespace Lillisp.Tests;

public class SymbolTests
{
    [InlineData("(symbol=? 'foo 'foo)", true)]
    [InlineData("(symbol=? 'foo 'foo (string->symbol \"foo\"))", true)]
    [InlineData("(symbol=? 'foo 'bar)", false)]
    [Theory]
    public void SymbolEqualsTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(symbol->string 'flying-fish)", "flying-fish")]
    [InlineData("(symbol->string 'Martin)", "Martin")]
    [InlineData("(symbol->string (string->symbol \"Malvina\"))", "Malvina")]
    [Theory]
    public void SymbolToStringTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}