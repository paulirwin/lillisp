using Lillisp.Core;

namespace Lillisp.Tests;

public class RecordTests
{
    private readonly LillispRuntime _runtime;

    public RecordTests()
    {
        _runtime = new LillispRuntime();

        _runtime.EvaluateProgram("(define-record-type <pare> (kons x y) pare? (x kar set-kar!) (y kdr))");
    }

    [InlineData("(pare? (kons 1 2))", true)]
    [InlineData("(pare? (cons 1 2))", false)]
    [InlineData("(kar (kons 1 2))", 1)]
    [InlineData("(kdr (kons 1 2))", 2)]
    [InlineData("(let ((k (kons 1 2))) (set-kar! k 3) (kar k))", 3)]
    [Theory]
    public void PareExampleTests(string input, object expected)
    {
        var result = _runtime.EvaluateProgram(input);

        Assert.Equal(expected, result);
    }
}