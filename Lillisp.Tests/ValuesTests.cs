using Lillisp.Core;

namespace Lillisp.Tests;

public class ValuesTests
{
    [Fact]
    public void BasicValuesTest()
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram("(values 1 2 3)") as IEnumerable<object>;

        Assert.NotNull(result);

        var list = result.ToList();

        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [InlineData("(let-values (((root rem) (exact-integer-sqrt 32))) (* root rem))", 35)]
    [Theory]
    public void LetValuesTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(let ((a #\\a) (b #\\b) (x #\\x) (y #\\y)) (let*-values (((a b) (values x y)) ((x y) (values a b))) (list a b x y)))", new object[] { 'x', 'y', 'x', 'y' })]
    [Theory]
    public void LetStarValuesTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(define-values (x y) (exact-integer-sqrt 17)) (list x y)", new object[] { 4, 1 })]
    [InlineData("(let () (define-values (x y) (values 1 2)) (+ x y))", 3)]
    [Theory]
    public void DefineValuesTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}