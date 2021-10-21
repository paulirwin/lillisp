using System;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class ProcessContextTests
    {
        [Fact]
        public void GetEnvironmentVariable_BasicTest()
        {
            var runtime = new LillispRuntime();

            Environment.SetEnvironmentVariable("foo", "bar");

            var result = runtime.EvaluateProgram("(get-environment-variable \"foo\")");

            Assert.Equal("bar", result);
        }
    }
}
