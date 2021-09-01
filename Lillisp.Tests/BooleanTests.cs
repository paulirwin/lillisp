using System;
using Lillisp.Core;
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
        public void NotTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, (bool)result);
        }
    }
}
