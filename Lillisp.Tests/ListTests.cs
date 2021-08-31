using System.Collections.Generic;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class ListTests
    {
        [InlineData("(car (list 2 3))", 2)]
        [InlineData("(car (list 2 3 4 5 6))", 2)]
        [Theory]
        public void CarTests(string input, double expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(cdr (list 2 3))", new object[] { 3d })]
        [InlineData("(cdr (list 2 3 4 5 6))", new object[] { 3d, 4d, 5d, 6d })]
        [Theory]
        public void CdrTests(string input, object[] expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, (IEnumerable<object>)result);
        }

        [InlineData("(cons 1 (list 2 3))", new object[] { 1d, 2d, 3d })]
        [InlineData("(cons 1 (cons 2 (cons 3 nil)))", new object[] { 1d, 2d, 3d })]
        [Theory]
        public void ConsTests(string input, object[] expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, (IEnumerable<object>)result);
        }
    }
}
