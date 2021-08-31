using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class MathTests
    {
        [InlineData("(+ 2 4)", 6)]
        [InlineData("(+ 2.5 3.5)", 6)]
        [Theory]
        public void AddTests(string input, double expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
