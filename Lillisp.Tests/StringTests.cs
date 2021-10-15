using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class StringTests
    {
        [InlineData("\"\"", "")]
        [InlineData("\"foo\"", "foo")]
        [InlineData("\"\\\"foo\\\"\"", "\"foo\"")]
        [InlineData("\"foo\\tbar\"", "foo\tbar")]
        [InlineData("\"\\\"", "\\")]
        [Theory]
        public void StringLiteralTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(str 100)", "100")]
        [InlineData("(str null)", null)]
        [InlineData("(str nil)", "()")]
        [InlineData("(str 'car)", "car")]
        [InlineData("(str '(1 2 3))", "(1 2 3)")]
        [InlineData("(str (new Uri \"https://www.google.com\"))", "https://www.google.com/")]
        [Theory]
        public void StrTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
