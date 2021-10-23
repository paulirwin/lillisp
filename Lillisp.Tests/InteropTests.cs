using System;
using System.Text;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class InteropTests
    {
        [InlineData("(begin (use 'System.Text) (typeof (new StringBuilder)))", typeof(StringBuilder))]
        [InlineData("(typeof (new Uri \"https://www.google.com\"))", typeof(Uri))]
        [InlineData("(begin (def x (new DateTime)) (typeof x))", typeof(DateTime))]
        [Theory]
        public void NewAndUseTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("Math/PI", Math.PI)]
        [InlineData("(begin (def x String/Empty) x)", "")]
        [InlineData("(String/IsNullOrEmpty null)", true)]
        [InlineData("(String/IsNullOrEmpty \"foo\")", false)]
        [InlineData("(Int32/Parse \"123\")", 123)]
        [Theory]
        public void StaticMemberTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
        
        [InlineData("(begin (def x (new Uri \"https://www.google.com\")) (.Scheme x))", "https")]
        [InlineData("(begin (use 'System.Text) (def x (new StringBuilder)) (.Append x \"foo\") (.Append x \"bar\") (str x))", "foobar")]
        [Theory]
        public void InstanceMemberTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
