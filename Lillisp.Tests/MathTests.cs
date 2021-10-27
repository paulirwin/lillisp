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
        [Theory]
        public void AddTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(sqrt 25)", 5)]
        [InlineData("(sqrt (+ 2 2))", 2)]
        [Theory]
        public void SqrtTests(string input, double expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(>> 42 2)", 10)]
        [InlineData("(>> 42 2 1)", 5)]
        [InlineData("(<< 1 2)", 4)]
        [InlineData("(<< 1 2 7)", 512)]
        [Theory]
        public void ShiftTests(string input, int expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
