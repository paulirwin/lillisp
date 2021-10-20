using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public static class TestHelper
    {
        public static void DefaultTest(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
