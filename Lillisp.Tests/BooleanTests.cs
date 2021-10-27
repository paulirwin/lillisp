using Xunit;

namespace Lillisp.Tests
{
    public class BooleanTests
    {
        [InlineData("(not false)", true)]
        [InlineData("(not true)", false)]
        [InlineData("(not (> 3 2))", false)]
        [InlineData("(not (< 3 2))", true)]
        [Theory]
        public void NotTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(eqv? #t #t)", true)]
        [InlineData("(eqv? 'a 'a)", true)]
        [InlineData("(eqv? 42 42)", true)]
        [InlineData("(eqv? #\\a #\\a)", true)]
        [InlineData("(eqv? '() '())", true)]
        [InlineData("(eqv? #t #f)", false)]
        [InlineData("(eqv? 'a 'b)", false)]
        [InlineData("(eqv? 42 43)", false)]
        [InlineData("(eqv? #\\a #\\b)", false)]
        [InlineData("(eqv? '() '(1 2 3))", false)]
        [Theory]
        public void EqvTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
