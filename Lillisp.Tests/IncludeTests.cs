namespace Lillisp.Tests;

public class IncludeTests
{
    [InlineData("(include \"include/add7.lisp\") (add7 4)", 11)]
    [Theory]
    public void Include_BasicTest(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}