using System.Collections.Generic;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests;

public class MacroTests
{
    [InlineData("(quote (1 2 3))", new object[] { 1, 2, 3 })]
    [InlineData("'(1 2 3)", new object[] { 1, 2, 3 })]
    [Theory]
    public void QuoteTests(string input, object[] expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, (IEnumerable<object>)result);
    }

    [InlineData("(list 1 2 3)", new object[] { 1, 2, 3 })]
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
    public void ApplyTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }


    [InlineData("(if (> 3 2) 7 4)", 7)]
    [InlineData("(if (> 2 3) 7 4)", 4)]
    [InlineData("(if (> 3 2) (+ 3 4) (+ 2 2))", 7)]
    [InlineData("(if (> 2 3) (+ 3 4) (+ 2 2))", 4)]
    [Theory]
    public void IfTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(begin 4 2)", 2)]
    [Theory]
    public void BeginTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(begin (define x 10) (+ x 2))", 12)]
    [InlineData("(begin (define x (- 12 2)) (+ x 2))", 12)]
    [InlineData("(begin (define x (- 12 2)) (set! x 20) (+ x 2))", 22)]
    [InlineData("(begin (define x (- 12 2)) (set! x (* 10 2)) (+ x 2))", 22)]
    [Theory]
    public void DefineSetTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(begin (define (x y) (+ y y)) (x 3))", 6)]
    [InlineData("(begin (define (x y z) (+ y z)) (x 3 4))", 7)]
    [Theory]
    public void DefineLambdaFormTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("((lambda (x) (pow x 2)) 4)", 16d)]
    [InlineData("(begin (define square (lambda (x) (pow x 2))) (square 4))", 16d)]
    [InlineData("(begin (define fact (lambda (n) (if (eqv? n 0) 1 (* n (fact (- n 1)))))) (fact 10))", 3628800)]
    [Theory]
    public void LambdaTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(begin (defun square (x) (pow x 2)) (square 4))", 16d)]
    [Theory]
    public void DefunTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(force (delay (+ 1 2)))", 3)]
    [InlineData("(let ((p (delay (+ 1 2)))) (list (force p) (force p)))", new[] { 3, 3 })]
    [Theory]
    public void DelayTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(unless (= 1 2.0) 1 2 3)", 3)]
    [Theory]
    public void UnlessTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(let ((x 2) (y 3)) (let ((x 7) (z (+ x y))) (* z x)))", 35)]
    [InlineData("(let ((x 2) (y 3)) (let* ((x 7) (z (+ x y))) (* z x)))", 70)]
    [InlineData("(let ((x 2) (y 3)) (* x y))", 6)]
    [InlineData("(letrec ((even? (lambda (n) (if (zero? n) #t (odd? (- n 1))))) (odd? (lambda (n) (if (zero? n) #f (even? (- n 1)))))) (even? 88))", true)]
    [Theory]
    public void LetTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(case (* 2 3) ((2 3 5 7) \"prime\") ((1 4 6 8 9) \"composite\"))", "composite")]
    [InlineData("(case (car '(c d)) ((a) 'a) ((b) 'b))", false)]
    [InlineData("(str (case (car '(c d)) ((a e i o u) 'vowel) ((w y) 'semivowel) (else => (lambda (x) x))))", "c")]
    [Theory]
    public void CaseTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [Fact]
    public void DefineSyntaxBasicTest()
    {
        string program = @"
(define-syntax kwote
  (syntax-rules ()
    ((kwote exp)
     (quote exp))))
(kwote (""foo"" ""bar""))
";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program);

        Assert.Equal(new object[] { "foo", "bar" }, result);
    }
    
    [Fact]
    public void LetSyntaxBasicTest()
    {
        string program = @"
(let-syntax 
    ((given-that (syntax-rules ()
        ((given-that test stmt1 stmt2 ...)
        (if test (begin stmt1 stmt2 ...))))))
(let 
    ((if #t))
    (given-that if (set! if ""now""))
    if))
";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program);

        Assert.Equal("now", result);
    }

    [Fact]
    public void MacroEllipsisTest()
    {
        string program = @"
(define-syntax my-when
  (syntax-rules ()
    ((my-when c e ...)
     (if c (begin e ...)))))
(my-when #t 1 2 3)
";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program);

        Assert.Equal(3, result);
    }

    [Fact]
    public void LetRecSyntaxTest()
    {
        string program = @"
(letrec-syntax
    ((my-or 
        (syntax-rules ()
            ((my-or) #f)
            ((my-or e) e)
            ((my-or e1 e2 ...) (let ((temp e1)) (if temp temp (my-or e2 ...))))
        )
    ))
    (let ((x #f) (y 7) (temp 8) (let odd?) (if even?))
        (my-or x (let temp) (if y) y)
    )
)
";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program);

        Assert.Equal(7, result);
    }

    [Fact]
    public void SyntaxErrorTest()
    {
        string program = @"
(letrec-syntax
    ((err-or
        (syntax-rules ()
            ((err-or) #f)
            ((err-or e) (syntax-error ""Expected many args, got"" e))
            ((err-or e1 e2 ...) #t)
        )
    ))
    (err-or 123)
)
";

        var runtime = new LillispRuntime();

        var error = Assert.Throws<SyntaxError>(() => runtime.EvaluateProgram(program));

        Assert.Equal(1, error.Args.Count);

        var atom = error.Args[0] as Atom;

        Assert.NotNull(atom);
        Assert.Equal(123, atom.Value);
    }
}