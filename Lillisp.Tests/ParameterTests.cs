namespace Lillisp.Tests;

public class ParameterTests
{
    [InlineData("(define x (make-parameter 12)) (x)", 12)]
    [InlineData("(define x (make-parameter 12 (lambda (y) (* y 2)))) (x)", 24)]
    [Theory]
    public void MakeParameter_BasicTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(define x (make-parameter 12)) (parameterize ((x 15)) (x))", 15)]
    [InlineData("(define x (make-parameter 12 (lambda (y) (* y 2)))) (parameterize ((x 15)) (x))", 30)]
    [Theory]
    public void Parameterize_BasicTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}