using Xunit;

namespace Lillisp.Tests;

public class RegexTests
{
    [InlineData("(match? /[a-z0-9]*/ \"abc123\")", true)]
    [InlineData("(match? /^https?:\\/\\/$/ \"http://\")", true)]
    [InlineData("(match? /^ABC$/ \"abc\")", false)]
    [InlineData("(match? /^ABC$/i \"abc\")", true)]
    [Theory]
    public void RegexMatchTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}