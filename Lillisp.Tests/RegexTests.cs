using Xunit;

namespace Lillisp.Tests;

public class RegexTests
{
    [InlineData("(match? /[a-z0-9]*/ \"abc123\")", true)]
    [Theory]
    public void RegexMatchTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}