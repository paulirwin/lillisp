using System.Collections.Generic;
using System.Text;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class LispINQTests
    {
        [Fact]
        public void BasicSelectTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(use 'System.Collections.Generic)");
            prog.AppendLine("(define mylist (new (List String)))");
            prog.AppendLine("(.Add mylist \"foo\")");
            prog.AppendLine("(.Add mylist \"fizz\")");
            prog.AppendLine("(.ToArray (from i in mylist select i))");

            var result = runtime.EvaluateProgram(prog.ToString()) as string[];

            Assert.NotNull(result);

            Assert.Equal(2, result.Length);
            Assert.Equal("foo", result[0]);
            Assert.Equal("fizz", result[1]);
        }

        [Fact(Skip = "Where not yet supported")]
        public void BasicWhereTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(use 'System.Collections.Generic)");
            prog.AppendLine("(define mylist (new (List String)))");
            prog.AppendLine("(.Add mylist \"foo\")");
            prog.AppendLine("(.Add mylist \"bar\")");
            prog.AppendLine("(.Add mylist \"baz\")");
            prog.AppendLine("(.Add mylist \"fizz\")");
            prog.AppendLine("(.Add mylist \"buzz\")");
            prog.AppendLine("(.ToArray (from i in mylist where (.StartsWith i \"f\") select i))");

            var result = runtime.EvaluateProgram(prog.ToString()) as string[];

            Assert.NotNull(result);

            Assert.Equal(2, result.Length);
            Assert.Equal("foo", result[0]);
            Assert.Equal("fizz", result[1]);
        }
    }
}
