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

        [InlineData("(finite? 3)", true)]
        [InlineData("(finite? +inf.0)", false)]
        [InlineData("(finite? 3.0+inf.0i)", false)]
        [InlineData("(infinite? 3)", false)]
        [InlineData("(infinite? +inf.0)", true)]
        [InlineData("(infinite? +nan.0)", false)]
        [InlineData("(infinite? 3.0+inf.0i)", true)]
        [Theory]
        public void FiniteInfiniteTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
