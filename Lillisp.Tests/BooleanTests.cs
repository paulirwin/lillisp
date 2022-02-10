namespace Lillisp.Tests;

public class BooleanTests
{
    [InlineData("(not false)", true)]
    [InlineData("(not true)", false)]
    [InlineData("(not (> 3 2))", false)]
    [InlineData("(not (< 3 2))", true)]
    [Theory]
    public void NotTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    // R7RS 6.1
    [InlineData("(eqv? #t #t)", true)]
    [InlineData("(eqv? 'a 'a)", true)]
    [InlineData("(eqv? #\\a #\\a)", true)]
    [InlineData("(eqv? '() '())", true)]
    [InlineData("(eqv? #t #f)", false)]
    [InlineData("(eqv? 'a 'b)", false)]
    [InlineData("(eqv? 42 43)", false)]
    [InlineData("(eqv? #\\a #\\b)", false)]
    [InlineData("(eqv? '() '(1 2 3))", false)]
    [InlineData("(eqv? 2 2)", true)]
    // [InlineData("(eqv? 2 2.0)", false)] // TODO: inexact numbers
    [InlineData("(eqv? 100000000 100000000)", true)]
    [InlineData("(eqv? 0 +nan.0)", false)]
    [InlineData("(eqv? (cons 1 2) (cons 1 2))", false)]
    [InlineData("(eqv? (lambda () 1) (lambda () 2))", false)]
    [InlineData("(let ((p (lambda (x) x))) (eqv? p p))", true)]
    [InlineData("(eqv? #f 'nil)", false)]
    [Theory]
    public void EqvTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    // R7RS 6.1
    [InlineData("(eq? 'a 'a)", true)]
    [InlineData("(eq? (list 'a) (list 'a))", false)]
    [InlineData("(eq? '() '())", true)]
    [InlineData("(eq? car car)", true)]
    [InlineData("(let ((x '(a))) (eq? x x))", true)]
    [InlineData("(let ((x '#())) (eq? x x))", true)]
    [InlineData("(let ((p (lambda (x) x))) (eq? p p))", true)]
    [Theory]
    public void EqTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    // R7RS 6.1
    [InlineData("(equal? 'a 'a)", true)]
    [InlineData("(equal? '(a) '(a))", true)]
    [InlineData("(equal? '(a (b) c) '(a (b) c))", true)]
    [InlineData("(equal? \"abc\" \"abc\")", true)]
    [InlineData("(equal? 2 2)", true)]
    [InlineData("(equal? (make-vector 5 'a) (make-vector 5 'a))", true)]
    // [InlineData("(equal? '#1=(a b . #1#) '#2=(a b a b . #2#))", true)] // TODO: datum labels and circular lists
    [Theory]
    public void EqualTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}