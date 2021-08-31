using System.Collections.Generic;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class MacroTests
    {
        [InlineData("(quote (1 2 3))", new object[] { 1d, 2d, 3d })]
        [InlineData("'(1 2 3)", new object[] { 1d, 2d, 3d })]
        [Theory]
        public void QuoteTests(string input, object[] expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, (IEnumerable<object>)result);
        }

        [InlineData("(list 1 2 3)", new object[] { 1d, 2d, 3d })]
        [InlineData("(list)", new object[0])]
        [InlineData("()", new object[0])]
        [InlineData("nil", new object[0])]
        [Theory]
        public void ListTests(string input, object[] expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, (IEnumerable<object>)result);
        }

        [InlineData("(apply + (list 1 2 3))", 6)]
        [InlineData("(apply * (list 2 3 4))", 24)]
        [Theory]
        public void ApplyTests(string input, double expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
