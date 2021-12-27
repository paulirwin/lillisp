using System;
using System.Collections.Generic;
using System.Linq;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class PortTests
    {
        [Fact]
        public void ParameterizedOpenOutputStringPortTest()
        {
            string program = "(parameterize ((current-output-port (open-output-string))) (display \"piece\") (display \" by piece \") (display \"by piece.\") (newline) (get-output-string (current-output-port)))";

            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(program) as string;

            Assert.NotNull(result);

            if (Environment.NewLine == "\r\n")
            {
                Assert.Equal("piece by piece by piece.\r\n", result);
            }
            else
            {
                Assert.Equal("piece by piece by piece.\n", result);
            }
        }

        [Fact]
        public void ParameterizedOpenInputStringPortTest()
        {
            string program = "(parameterize ((current-input-port (open-input-string \"ABC\"))) (list (read-char) (read-char) (read-char) (eof-object? (read-char))))";

            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(program) as IEnumerable<object>;

            Assert.NotNull(result);

            var list = result.ToList();

            Assert.Equal('A', list[0]);
            Assert.Equal('B', list[1]);
            Assert.Equal('C', list[2]);
            Assert.Equal(true, list[3]);
        }

        [InlineData("(parameterize ((current-input-port (open-input-string \"ABC\\nDEF\"))) (list (read-line) (read-line)))", new object[] { "ABC", "DEF" })]
        [Theory]
        public void ReadLineTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(parameterize ((current-input-port (open-input-string \"'(1 2 3)\"))) (eval (read)))", new object[] { 1, 2, 3 })]
        [Theory]
        public void ReadTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
