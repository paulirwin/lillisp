using Xunit;
// ReSharper disable StringLiteralTypo

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

        [InlineData("(number->string 3)", "3")]
        [InlineData("(number->string 3.04)", "3.04")]
        [InlineData("(number->string 3e10)", "30000000000")]
        [InlineData("(number->string +inf.0)", "+inf.0")]
        [InlineData("(number->string +nan.0)", "+nan.0")]
        [InlineData("(number->string #xabcd)", "43981")]
        [InlineData("(number->string #xabcd 16)", "abcd")]
        [Theory]
        public void NumberToStringTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(string->number \"3\")", 3)]
        [InlineData("(string->number \"3.04\")", 3.04)]
        [InlineData("(string->number \"3e10\")", 3e10)]
        [InlineData("(string->number \"+inf.0\")", double.PositiveInfinity)]
        [InlineData("(string->number \"+nan.0\")", double.NaN)]
        [InlineData("(string->number \"abcd\")", false)]
        [InlineData("(string->number \"#xabcd\")", 43981)]
        [InlineData("(string->number \"abcd\" 16)", 43981)]
        [Theory]
        public void StringToNumberTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(eqv? (cast 2.04 Decimal) (exact 2.04))", true)]
        [Theory]
        public void ExactConversionTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(eqv? 2.04 (inexact (cast 2.04 Decimal)))", true)]
        [InlineData("(eqv? 2.0 (inexact 2))", true)]
        [Theory]
        public void InexactConversionTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
