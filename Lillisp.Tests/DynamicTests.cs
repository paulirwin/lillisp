namespace Lillisp.Tests;

public class DynamicTests
{
    [InlineData("(count '())", 0)]
    [InlineData("(count nil)", 0)]
    [InlineData("(count '(1 2 3))", 3)]
    [InlineData("(count \"foo bar\")", 7)]
    [Theory]
    public void CountTests(string input, int expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(get '(1 2 3) 0)", 1)]
    [InlineData("(get (range 0 10 2) 2)", 4)]
    [InlineData("(get \"foo bar\" 5)", 'a')]
    [Theory]
    public void GetTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(promise? (make-promise 7))", true)]
    [InlineData("(force (make-promise 7))", 7)]
    [InlineData("(promise? (make-promise (delay 7)))", true)]
    [InlineData("(force (make-promise (delay 7)))", 7)]
    [Theory]
    public void MakePromiseTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(force (delay-force 123))", 123)]
    [InlineData("(define x 5) (define t (delay-force x)) (set! x 6) (force t)", 6)]
    [InlineData("(define x 5) (define t (delay (+ x 2))) (set! x 6) (force (delay-force t))", 8)]
    [Theory]
    public void DelayForceTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}