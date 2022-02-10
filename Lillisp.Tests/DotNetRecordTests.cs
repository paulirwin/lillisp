using Lillisp.Core;
using System.Text;

namespace Lillisp.Tests;

public class DotNetRecordTests
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

    [Fact]
    public void RecordEqualityTest()
    {
        var runtime = new LillispRuntime();

        var prog = new StringBuilder();

        prog.AppendLine("(defrecord Customer (Id Int32) (Name String))");
        prog.AppendLine("(def c (new Customer 123 \"foo bar\"))");
        prog.AppendLine("(def c2 (new Customer 123 \"foo bar\"))");
        prog.AppendLine("(eqv? c c2)");

        var result = runtime.EvaluateProgram(prog.ToString());

        Assert.NotNull(result);
        Assert.Equal(true, result);
    }
}