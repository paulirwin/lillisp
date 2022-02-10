using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests;

public class R7RSTests
{
    [InlineData("(begin (def |H\\x65;llo| 42) Hello)", 42)]
    [InlineData("(begin (def |two words| 42) |two\\x20;words|)", 42)]
    [InlineData("(eqv? '|\\x3BB;| '|λ|)", true)]
    [InlineData("(eqv? '|\\x9;\\x9;| '|\t\t|)", true)]
    [Theory]
    public void R7RS_2_1_Identifiers(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(+ 2 #;(* 3 6) 4)", 6)]
    [InlineData("(* 2 6) ; look at me now", 12)]
    [InlineData("(begin (def x \"goodbye; hello\") x) ; this is a demo", "goodbye; hello")]
    [InlineData("#|\nThis is a block comment\n|#\n(+ 2 6)", 8)]
    [Theory]
    public void R7RS_2_2_Comments(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    /// <summary>
    /// Tests tail recursion with a 10,000 iteration factorial. This quickly would stack overflow without tail calls.
    /// </summary>
    [Fact(Skip = "Skipping by default due to execution time")]
    public void R7RS_3_5_Tail_Recursion_Factorial()
    {
        var runtime = new LillispRuntime();

        var prog = "(define (fact x) (begin (define (fact-tail x accum) (if (eqv? x 0) accum (fact-tail (- x 1) (* x accum)))) (fact-tail x 1)))";
        prog += "(fact 10000)";

        var result = runtime.EvaluateProgram(prog);

        Assert.IsType<double>(result);
    }

    [Fact]
    public void R7RS_4_1_4_Lambda_Example()
    {
        var runtime = new LillispRuntime();

        var prog = "((lambda (x) (+ x x)) 4)";

        var result = runtime.EvaluateProgram(prog);

        Assert.Equal(8, result);
    }

    [Fact]
    public void R7RS_4_1_4_Reverse_Subtract_Example()
    {
        var runtime = new LillispRuntime();

        var prog = "(define reverse-subtract (lambda (x y) (- y x)))\n";
        prog += "(reverse-subtract 7 10)";

        var result = runtime.EvaluateProgram(prog);

        Assert.Equal(3, result);
    }

    [Fact(Skip = "let does not return a binding scope that outlives its execution; or should define be lazy?")]
    public void R7RS_4_1_4_Add4_Example()
    {
        var runtime = new LillispRuntime();

        var prog = "(define add4 (let ((x 4)) (lambda (y) (+ x y))))\n";
        prog += "(add4 6)";

        var result = runtime.EvaluateProgram(prog);

        Assert.Equal(10, result);
    }

    [Fact]
    public void R7RS_4_1_4_Lambda_Rest_Example_1()
    {
        var runtime = new LillispRuntime();

        var prog = "((lambda x x) 3 4 5 6)";

        var result = runtime.EvaluateProgram(prog);

        Assert.IsAssignableFrom<IEnumerable<object>>(result);

        var list = (result as IEnumerable<object>)!.ToList();

        Assert.Equal(4, list.Count);
        Assert.Equal(3, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(5, list[2]);
        Assert.Equal(6, list[3]);
    }

    [Fact]
    public void R7RS_4_1_4_Lambda_Rest_Example_1_Define_Form()
    {
        var runtime = new LillispRuntime();

        var prog = "(define (mylambda . x) x) (mylambda 3 4 5 6)";

        var result = runtime.EvaluateProgram(prog);

        Assert.IsAssignableFrom<IEnumerable<object>>(result);

        var list = (result as IEnumerable<object>)!.ToList();

        Assert.Equal(4, list.Count);
        Assert.Equal(3, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(5, list[2]);
        Assert.Equal(6, list[3]);
    }

    [Fact]
    public void R7RS_4_1_4_Lambda_Rest_Example_2()
    {
        var runtime = new LillispRuntime();

        var prog = "((lambda (x y . z) z) 3 4 5 6)";

        var result = runtime.EvaluateProgram(prog);

        Assert.IsAssignableFrom<IEnumerable<object>>(result);

        var list = (result as IEnumerable<object>)!.ToList();

        Assert.Equal(2, list.Count);
        Assert.Equal(5, list[0]);
        Assert.Equal(6, list[1]);
    }

    [Fact]
    public void R7RS_4_1_4_Lambda_Rest_Example_2_Define_Form()
    {
        var runtime = new LillispRuntime();

        var prog = "(define (mylambda x y . z) z) (mylambda 3 4 5 6)";

        var result = runtime.EvaluateProgram(prog);

        Assert.IsAssignableFrom<IEnumerable<object>>(result);

        var list = (result as IEnumerable<object>)!.ToList();

        Assert.Equal(2, list.Count);
        Assert.Equal(5, list[0]);
        Assert.Equal(6, list[1]);
    }

    // HACK: the actual examples use quoted symbols, but those aren't 100% correct yet here.
    // The point of the example is more about the conditionals, so using strings instead for now.
    [InlineData("(if (> 3 2) \"yes\" \"no\")", "yes")]
    [InlineData("(if (> 2 3) \"yes\" \"no\")", "no")]
    [InlineData("(if (> 3 2) (- 3 2) (+ 3 2))", 1)]
    [Theory]
    public void R7RS_4_1_5_Conditional_Examples(string input, object expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void R7RS_4_1_6_Assignment_Example()
    {
        var runtime = new LillispRuntime();

        var prog = new StringBuilder();
        prog.AppendLine("(define x 2)");
        prog.AppendLine("(+ x 1)");

        var result = runtime.EvaluateProgram(prog.ToString());

        Assert.Equal(3, result);

        prog.Clear();

        prog.AppendLine("(set! x 4)");
        prog.AppendLine("(+ x 1)");

        result = runtime.EvaluateProgram(prog.ToString());

        Assert.Equal(5, result);
    }

    [InlineData("(cond ((> 3 2) \"greater\") ((< 3 2) \"less\"))", "greater")]
    [InlineData("(cond ((> 3 3) \"greater\") ((< 3 3) \"less\") (else \"equal\"))", "equal")]
    [InlineData("(cond ((assv 'b '((a 1) (b 2))) => cadr) (else #f))", 2)]
    [Theory]
    public void R7RS_4_2_1_Cond_Examples(string input, object expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, result);
    }
}