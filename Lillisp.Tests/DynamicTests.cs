using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class DynamicTests
    {
        [InlineData("(count '())", 0)]
        [InlineData("(count nil)", 0)]
        [InlineData("(count '(1 2 3))", 3)]
        [InlineData("(count \"foo bar\")", 7)]
        [Theory]
        public void CountTests(string input, int expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(get '(1 2 3) 0)", 1d)]
        [InlineData("(get (range 0 10 2) 2)", 4d)]
        [InlineData("(get \"foo bar\" 5)", 'a')]
        [Theory]
        public void GetTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(promise? (make-promise 7))", true)]
        [InlineData("(force (make-promise 7))", 7d)]
        [InlineData("(promise? (make-promise (delay 7)))", true)]
        [InlineData("(force (make-promise (delay 7)))", 7d)]
        [Theory]
        public void MakePromiseTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
