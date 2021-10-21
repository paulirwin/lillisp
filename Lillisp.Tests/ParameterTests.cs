using Xunit;

namespace Lillisp.Tests
{
    public class ParameterTests
    {
        [InlineData("(define x (make-parameter 12)) (x)", 12d)]
        [InlineData("(define x (make-parameter 12 (lambda (y) (* y 2)))) (x)", 24d)]
        [Theory]
        public void MakeParameter_BasicTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(define x (make-parameter 12)) (parameterize ((x 15)) (x))", 15d)]
        [InlineData("(define x (make-parameter 12 (lambda (y) (* y 2)))) (parameterize ((x 15)) (x))", 30d)]
        [Theory]
        public void Parameterize_BasicTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
