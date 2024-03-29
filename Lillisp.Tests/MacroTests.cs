﻿using Lillisp.Core;
// ReSharper disable StringLiteralTypo

namespace Lillisp.Tests;

public class MacroTests
{
    [InlineData("(quote (1 2 3))", new object[] { 1, 2, 3 })]
    [InlineData("'(1 2 3)", new object[] { 1, 2, 3 })]
    [Theory]
    public void QuoteTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(list 1 2 3)", new object[] { 1, 2, 3 })]
    [InlineData("(list)", new object[0])]
    [InlineData("()", new object[0])]
    [InlineData("nil", new object[0])]
    [Theory]
    public void ListTests(string input, object[] expected)
    {
        TestHelper.DefaultTest(input, expected);
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
        const string program = @"
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
        const string program = @"
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
        const string program = @"
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
        const string program = @"
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
        const string program = @"
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
        Assert.Equal(123, atom!.Value);
    }

    [InlineData("(do () (#t 1))", 1)]
    [InlineData("(do () (#t 1) 0)", 1)]
    [InlineData("(let ((x '(1 3 5 7 9)))\r\n(do ((x x (cdr x))\r\n(sum 0 (+ sum (car x))))\r\n((null? x) sum)))", 25)]
    [Theory]
    public void DoTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [Fact]
    public void DoR7RSTest()
    {
        const string program = "(do ((vec (make-vector 5))\r\n(i 0 (+ i 1)))\r\n((= i 5) vec)\r\n(vector-set! vec i i))";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program) as Vector;

        Assert.NotNull(result);

        Assert.Equal(5, result!.Count);
        Assert.Equal(0, result[0]);
        Assert.Equal(1, result[1]);
        Assert.Equal(2, result[2]);
        Assert.Equal(3, result[3]);
        Assert.Equal(4, result[4]);
    }

    [Fact]
    public void NamedLetR7RSTest()
    {
        const string program = @"
(let loop ((numbers '(3 -2 1 6 -5))
           (nonneg '())
           (neg '()))
    (cond ((null? numbers) (list nonneg neg))
        ((>= (car numbers) 0)
         (loop (cdr numbers)
            (cons (car numbers) nonneg)
            neg))
        ((< (car numbers) 0)
         (loop (cdr numbers)
            nonneg
            (cons (car numbers) neg)))))";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program) as Pair;

        Assert.NotNull(result);

        var carPair = result!.Car as Pair;
        
        Assert.NotNull(carPair);

        var carList = carPair!.ToList();

        Assert.Equal(3, carList.Count);
        Assert.Equal(6, carList[0]);
        Assert.Equal(1, carList[1]);
        Assert.Equal(3, carList[2]);

        var cdrPair = result.Cdr as Pair;

        Assert.NotNull(cdrPair);

        var cdrCarPair = cdrPair!.Car as Pair;

        Assert.NotNull(cdrCarPair);

        var cdrList = cdrCarPair!.ToList();

        Assert.Equal(2, cdrList.Count);
        Assert.Equal(-5, cdrList[0]);
        Assert.Equal(-2, cdrList[1]);
    }

    [Fact]
    public void CaseLambdaR7RSTest()
    {
        const string myRange = @"
(define my-range
    (case-lambda
        ((e) (my-range 0 e))
        ((b e) (do ((r '() (cons e r))
                (e (- e 1) (- e 1)))
                ((< e b) r)))))";

        var runtime = new LillispRuntime();
        runtime.EvaluateProgram(myRange);

        var result = runtime.EvaluateProgram("(my-range 3)") as Pair;

        Assert.NotNull(result);

        var resultList = result!.ToList();

        Assert.Equal(3, resultList.Count);
        Assert.Equal(0, resultList[0]);
        Assert.Equal(1, resultList[1]);
        Assert.Equal(2, resultList[2]);

        result = runtime.EvaluateProgram("(my-range 3 5)") as Pair;

        Assert.NotNull(result);

        resultList = result!.ToList();

        Assert.Equal(2, resultList.Count);
        Assert.Equal(3, resultList[0]);
        Assert.Equal(4, resultList[1]);
    }

    [Fact]
    public void ForEachTest()
    {
        const string program = @"
(let ((v (make-vector 5)))
    (for-each 
        (lambda (i) (vector-set! v i (* i i)))
        '(0 1 2 3 4))
v)
";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program) as Vector;

        Assert.NotNull(result);
        Assert.Equal(5, result!.Count);
        Assert.Equal(0, result[0]);
        Assert.Equal(1, result[1]);
        Assert.Equal(4, result[2]);
        Assert.Equal(9, result[3]);
        Assert.Equal(16, result[4]);
    }
}