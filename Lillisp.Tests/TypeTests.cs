using System;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class TypeTests
    {
        [InlineData("(typeof \"foo\")", typeof(string))]
        [InlineData("(typeof 1)", typeof(double))]
        [Theory]
        public void TypeofTests(string input, Type expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(typeof (cast 1 Int32))", typeof(int))]
        [InlineData("(typeof (cast \"1\" Double)", typeof(double))]
        [Theory]
        public void CastTests(string input, Type expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
