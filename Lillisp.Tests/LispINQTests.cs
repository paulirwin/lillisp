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

        [Fact]
        public void BasicWhereTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(use 'System.Collections.Generic)");
            prog.AppendLine("(use 'System.Linq)");
            prog.AppendLine("(define mylist (new (List String)))");
            prog.AppendLine("(.Add mylist \"foo\")");
            prog.AppendLine("(.Add mylist \"bar\")");
            prog.AppendLine("(.Add mylist \"baz\")");
            prog.AppendLine("(.Add mylist \"fizz\")");
            prog.AppendLine("(.Add mylist \"buzz\")");
            prog.AppendLine("(.ToArray (from i in mylist where (.StartsWith i \"f\") select i))");

            var result = runtime.EvaluateProgram(prog.ToString());

            var resultArray = result as object[];
            Assert.NotNull(resultArray);

            Assert.Equal(2, resultArray.Length);
            Assert.Equal("foo", resultArray[0]);
            Assert.Equal("fizz", resultArray[1]);
        }

        [Fact]
        public void SelectProjectionTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(use 'System.Collections.Generic)");
            prog.AppendLine("(use 'System.Linq)");
            prog.AppendLine("(define mylist (new (List String)))");
            prog.AppendLine("(.Add mylist \"foo\")");
            prog.AppendLine("(.Add mylist \"bar\")");
            prog.AppendLine("(.ToArray (from i in mylist select (.ToUpper i)))");

            var result = runtime.EvaluateProgram(prog.ToString());

            var resultArray = result as object[];
            Assert.NotNull(resultArray);

            Assert.Equal(2, resultArray.Length);
            Assert.Equal("FOO", resultArray[0]);
            Assert.Equal("BAR", resultArray[1]);
        }

        [Fact]
        public void BasicOrderByTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(use 'System.Collections.Generic)");
            prog.AppendLine("(use 'System.Linq)");
            prog.AppendLine("(define mylist (new (List String)))");
            prog.AppendLine("(.Add mylist \"foo\")");
            prog.AppendLine("(.Add mylist \"bar\")");
            prog.AppendLine("(.ToArray (from i in mylist orderby i select i))");

            var result = runtime.EvaluateProgram(prog.ToString());

            var resultArray = result as object[];
            Assert.NotNull(resultArray);

            Assert.Equal(2, resultArray.Length);
            Assert.Equal("bar", resultArray[0]);
            Assert.Equal("foo", resultArray[1]);
        }

        [Fact]
        public void BasicOrderByDescendingTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(use 'System.Collections.Generic)");
            prog.AppendLine("(use 'System.Linq)");
            prog.AppendLine("(define mylist (new (List String)))");
            prog.AppendLine("(.Add mylist \"xyz\")");
            prog.AppendLine("(.Add mylist \"yyy\")");
            prog.AppendLine("(.Add mylist \"zzz\")");
            prog.AppendLine("(.ToArray (from i in mylist orderby i desc select i))");

            var result = runtime.EvaluateProgram(prog.ToString());

            var resultArray = result as object[];
            Assert.NotNull(resultArray);

            Assert.Equal(3, resultArray.Length);
            Assert.Equal("zzz", resultArray[0]);
            Assert.Equal("yyy", resultArray[1]);
            Assert.Equal("xyz", resultArray[2]);
        }

        [Fact]
        public void BasicThenByTest()
        {
            var runtime = new LillispRuntime();

            var prog = new StringBuilder();

            prog.AppendLine("(use 'System.Collections.Generic)");
            prog.AppendLine("(use 'System.Linq)");
            prog.AppendLine("(define mylist (new (List String)))");
            prog.AppendLine("(.Add mylist \"xyz\")");
            prog.AppendLine("(.Add mylist \"yyy\")");
            prog.AppendLine("(.Add mylist \"zzz\")");
            prog.AppendLine("(.ToArray (from i in mylist orderby (get i 1) thenby (get i 2) select i))");

            var result = runtime.EvaluateProgram(prog.ToString());

            var resultArray = result as object[];
            Assert.NotNull(resultArray);

            Assert.Equal(3, resultArray.Length);
            Assert.Equal("yyy", resultArray[0]);
            Assert.Equal("xyz", resultArray[1]);            
            Assert.Equal("zzz", resultArray[2]);
        }
    }
}
