using Xunit;

namespace Lillisp.Tests
{
    public class NumberTests
    {
        [InlineData("(exact? 3.0)", false)]
        //[InlineData("(exact? #e3.0)", true)] // TODO: support exact literals
        [InlineData("(exact? 3)", true)]
        [InlineData("(inexact? 3.)", true)]
        [InlineData("(exact-integer? 32)", true)]
        [InlineData("(exact-integer? 32.0)", false)]
        [InlineData("(exact-integer? 32/5)", false)]
        [Theory]
        public void ExactInexactTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
