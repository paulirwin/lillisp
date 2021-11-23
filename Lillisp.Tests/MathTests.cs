using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class MathTests
    {
        [InlineData("(+ 2 4)", 6)]
        [InlineData("(+ 2.5 3.5)", 6.0)]
        [InlineData("(+ -1 3)", 2)]
        [InlineData("(+ 4 -1.5)", 2.5)]
        [InlineData("(+ 2 4 6)", 12)]
        [InlineData("(+ 3)", 3)]
        [InlineData("(+)", 0)]
        [Theory]
        public void AddTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(* 3 4)", 12)]
        [InlineData("(* 4)", 4)]
        [InlineData("(*)", 1)]
        [Theory]
        public void MultiplyTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(- 3 4)", -1)]
        [InlineData("(- 3 4 5)", -6)]
        [InlineData("(- 3)", -3)]
        [Theory]
        public void SubtractTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(abs -7)", 7)]
        [Theory]
        public void AbsTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(car (floor/ 5 2))", 2)]
        [InlineData("(cdr (floor/ 5 2))", 1)]
        [InlineData("(car (floor/ -5 2))", -3)]
        [InlineData("(cdr (floor/ -5 2))", 1)]
        [InlineData("(car (floor/ 5 -2))", -3)]
        [InlineData("(cdr (floor/ 5 -2))", -1)]
        [InlineData("(car (floor/ -5 -2))", 2)]
        [InlineData("(cdr (floor/ -5 -2))", -1)]
        [InlineData("(car (truncate/ 5 2))", 2)]
        [InlineData("(cdr (truncate/ 5 2))", 1)]
        [InlineData("(car (truncate/ -5 2))", -2)]
        [InlineData("(cdr (truncate/ -5 2))", -1)]
        [InlineData("(car (truncate/ 5 -2))", -2)]
        [InlineData("(cdr (truncate/ 5 -2))", 1)]
        [InlineData("(car (truncate/ -5 -2))", 2)]
        [InlineData("(cdr (truncate/ -5 -2))", -1)]
        [InlineData("(car (truncate/ -5.0 -2))", 2.0)]
        [InlineData("(cdr (truncate/ -5.0 -2))", -1.0)]
        [InlineData("(floor-quotient 5 2)", 2)]
        [InlineData("(floor-quotient -5 2)", -3)]
        [InlineData("(floor-quotient 5 -2)", -3)]
        [InlineData("(floor-quotient -5 -2)", 2)]
        [InlineData("(floor-remainder 5 2)", 1)]
        [InlineData("(floor-remainder -5 2)", 1)]
        [InlineData("(floor-remainder 5 -2)", -1)]
        [InlineData("(floor-remainder -5 -2)", -1)]
        [InlineData("(truncate-quotient 5 2)", 2)]
        [InlineData("(truncate-quotient -5 2)", -2)]
        [InlineData("(truncate-quotient 5 -2)", -2)]
        [InlineData("(truncate-quotient -5 -2)", 2)]
        [InlineData("(truncate-quotient -5.0 -2)", 2.0)]
        [InlineData("(truncate-remainder 5 2)", 1)]
        [InlineData("(truncate-remainder -5 2)", -1)]
        [InlineData("(truncate-remainder 5 -2)", 1)]
        [InlineData("(truncate-remainder -5 -2)", -1)]
        [InlineData("(truncate-remainder -5.0 -2)", -1.0)]
        [Theory]
        public void FloorTruncateTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(gcd 32 -36)", 4)]
        [InlineData("(gcd)", 0)]
        [InlineData("(lcm 32 -36)", 288)]
        [InlineData("(lcm 32.0 -36)", 288.0)]
        [InlineData("(lcm)", 1)]
        [Theory]
        public void GcdLcmTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(sqrt 25)", 5)]
        [InlineData("(sqrt (+ 2 2))", 2)]
        [Theory]
        public void SqrtTests(string input, double expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(>> 42 2)", 10)]
        [InlineData("(>> 42 2 1)", 5)]
        [InlineData("(<< 1 2)", 4)]
        [InlineData("(<< 1 2 7)", 512)]
        [Theory]
        public void ShiftTests(string input, int expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(log 1000)", 3d)]
        [InlineData("(log 144 12)", 2d)]
        [InlineData("(ln e)", 1d)]
        [Theory]
        public void LogarithmTests(string input, double expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.NotNull(result);
            Assert.Equal(expected, (double)result, 6);
        }
    }
}
