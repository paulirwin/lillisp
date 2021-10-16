using System.Collections.Generic;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class MacroTests
    {
        [InlineData("(quote (1 2 3))", new object[] { 1d, 2d, 3d })]
        [InlineData("'(1 2 3)", new object[] { 1d, 2d, 3d })]
        [Theory]
        public void QuoteTests(string input, object[] expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, (IEnumerable<object>)result);
        }

        [InlineData("(list 1 2 3)", new object[] { 1d, 2d, 3d })]
        [InlineData("(list)", new object[0])]
        [InlineData("()", new object[0])]
        [InlineData("nil", new object[0])]
        [Theory]
        public void ListTests(string input, object[] expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, (IEnumerable<object>)result);
        }

        [InlineData("(apply + (list 1 2 3))", 6)]
        [InlineData("(apply * (list 2 3 4))", 24)]
        [InlineData("(apply + [1 2 3])", 6)]
        [InlineData("(apply * [2 3 4])", 24)]
        [Theory]
        public void ApplyTests(string input, double expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }


        [InlineData("(if (> 3 2) 7 4)", 7d)]
        [InlineData("(if (> 2 3) 7 4)", 4d)]
        [InlineData("(if (> 3 2) (+ 3 4) (+ 2 2))", 7d)]
        [InlineData("(if (> 2 3) (+ 3 4) (+ 2 2))", 4d)]
        [Theory]
        public void IfTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(begin 4 2)", 2d)]
        [Theory]
        public void BeginTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(begin (define x 10) (+ x 2))", 12d)]
        [InlineData("(begin (define x (- 12 2)) (+ x 2))", 12d)]
        [InlineData("(begin (define x (- 12 2)) (set! x 20) (+ x 2))", 22d)]
        [InlineData("(begin (define x (- 12 2)) (set! x (* 10 2)) (+ x 2))", 22d)]
        [Theory]
        public void DefineSetTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("((lambda (x) (pow x 2)) 4)", 16d)]
        [InlineData("(begin (define square (lambda (x) (pow x 2))) (square 4))", 16d)]
        [InlineData("(begin (define fact (lambda (n) (if (= n 0) 1 (* n (fact (- n 1)))))) (fact 10))", 3628800d)]
        [Theory]
        public void LambdaTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(begin (defun square (x) (pow x 2)) (square 4))", 16d)]
        [Theory]
        public void DefunTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(force (delay (+ 1 2)))", 3d)]
        [InlineData("(let ((p (delay (+ 1 2)))) (list (force p) (force p)))", new[] { 3d, 3d })]
        [Theory]
        public void DelayTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
