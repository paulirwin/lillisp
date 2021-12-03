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

        [InlineData("(nan? +nan.0)", true)]
        [InlineData("(nan? 32)", false)]
        [InlineData("(nan? +nan.0+5.0i)", true)]
        [InlineData("(nan? 1+2i)", false)]
        [Theory]
        public void NanTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("#b1111", 0b1111)]
        [InlineData("#xabcd", 0xabcd)]
        [InlineData("#o1234", 668)]
        [InlineData("#d1234", 1234)]
        [Theory]
        public void RadixTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("#b1111_1111", 0b1111_1111)]
        [InlineData("#o12_34", 668)]
        [InlineData("#xab_cd", 0xabcd)]
        [InlineData("#d1_000_000", 1_000_000)]
        [InlineData("#d1_000_000.", 1_000_000.0)]
        [InlineData("1_000_000", 1_000_000)]
        [InlineData("1_000_000.250_111", 1_000_000.250_111)]
        [Theory]
        public void UnderscoreTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(= 1 1)", true)]
        [InlineData("(= 1 1 1)", true)]
        [InlineData("(= 1.0 1.0)", true)]
        [InlineData("(= #e1.0 #e1.0)", true)]
        [InlineData("(= 1 2)", false)]
        [InlineData("(= 1 1 2)", false)]
        [InlineData("(= +nan.0 +nan.0)", false)]
        [Theory]
        public void NumericallyEqualTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
