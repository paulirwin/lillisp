using System;
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
    }
}
