using System.Diagnostics;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests;

public class ContinuationTests
{
    [InlineData("(call-with-current-continuation (lambda (exit) (for-each (lambda (x) (if (negative? x) (exit x))) '(54 0 37 -3 245 19)) #t))", -3)]
    [Theory]
    public void CallWithCurrentContinuationTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(list-length '(1 2 3 4))", 4)]
    [InlineData("(list-length '(a b . c))", false)]
    [Theory]
    public void R7RSCallWithContinuationExample(string input, object expected)
    {
        var runtime = new LillispRuntime();

        const string func = @"
(define list-length
    (lambda (obj)
        (call-with-current-continuation
            (lambda (return)
                (letrec ((r 
                    (lambda (obj)
                        (cond ((null? obj) 0)
                              ((pair? obj) (+ (r (cdr obj)) 1))
                              (else (return #f))))))
                    (r obj))))))";
        
        runtime.EvaluateProgram(func);

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, result);
    }
}