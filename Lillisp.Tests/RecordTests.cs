using Lillisp.Core;
using System.Text;
using Xunit;

namespace Lillisp.Tests
{
    public class RecordTests
    {
        [Fact]
        public void BasicRecordTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(defrecord Customer (Id Int32) (Name String))");
            prog.AppendLine("(def c (new Customer 123 \"foo bar\"))");
            prog.AppendLine("(.Name c)");

            var result = runtime.EvaluateProgram(prog.ToString()) as string;

            Assert.NotNull(result);
            Assert.Equal("foo bar", result);
        }
    }
}
