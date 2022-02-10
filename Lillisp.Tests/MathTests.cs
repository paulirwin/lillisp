using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests;

public class MathTests
{
    [InlineData("(+ 2 4)", 6)]
    [InlineData("(+ 2.5 3.5)", 6.0)]
    [InlineData("(+ -1 3)", 2)]
    [InlineData("(+ 4 -1.5)", 2.5)]
    [InlineData("(+ 2 4 6)", 12)]
    [InlineData("(+ 3)", 3)]
    [InlineData("(+)", 0)]
    [Theory]
    public void AddTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(* 3 4)", 12)]
    [InlineData("(* 3 4.0)", 12.0)]
    [InlineData("(* 4)", 4)]
    [InlineData("(*)", 1)]
    [Theory]
    public void MultiplyTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(- 3 4)", -1)]
    [InlineData("(- 3 4 5)", -6)]
    [InlineData("(- 3)", -3)]
    [Theory]
    public void SubtractTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(str (/ 3 4 5))", "3/20")]
    [InlineData("(str (/ 3))", "1/3")]
    [InlineData("(/ 3.0 2)", 1.5)]
    [InlineData("(/ 2.0)", 0.5)]
    [InlineData("(str (/ 1/2 2))", "1/4")]
    [Theory]
    public void DivideTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(abs -7)", 7)]
    [Theory]
    public void AbsTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(car (floor/ 5 2))", 2)]
    [InlineData("(cdr (floor/ 5 2))", 1)]
    [InlineData("(car (floor/ -5 2))", -3)]
    [InlineData("(cdr (floor/ -5 2))", 1)]
    [InlineData("(car (floor/ 5 -2))", -3)]
    [InlineData("(cdr (floor/ 5 -2))", -1)]
    [InlineData("(car (floor/ -5 -2))", 2)]
    [InlineData("(cdr (floor/ -5 -2))", -1)]
    [InlineData("(car (truncate/ 5 2))", 2)]
    [InlineData("(cdr (truncate/ 5 2))", 1)]
    [InlineData("(car (truncate/ -5 2))", -2)]
    [InlineData("(cdr (truncate/ -5 2))", -1)]
    [InlineData("(car (truncate/ 5 -2))", -2)]
    [InlineData("(cdr (truncate/ 5 -2))", 1)]
    [InlineData("(car (truncate/ -5 -2))", 2)]
    [InlineData("(cdr (truncate/ -5 -2))", -1)]
    [InlineData("(car (truncate/ -5.0 -2))", 2.0)]
    [InlineData("(cdr (truncate/ -5.0 -2))", -1.0)]
    [InlineData("(floor-quotient 5 2)", 2)]
    [InlineData("(floor-quotient -5 2)", -3)]
    [InlineData("(floor-quotient 5 -2)", -3)]
    [InlineData("(floor-quotient -5 -2)", 2)]
    [InlineData("(floor-remainder 5 2)", 1)]
    [InlineData("(floor-remainder -5 2)", 1)]
    [InlineData("(floor-remainder 5 -2)", -1)]
    [InlineData("(floor-remainder -5 -2)", -1)]
    [InlineData("(truncate-quotient 5 2)", 2)]
    [InlineData("(truncate-quotient -5 2)", -2)]
    [InlineData("(truncate-quotient 5 -2)", -2)]
    [InlineData("(truncate-quotient -5 -2)", 2)]
    [InlineData("(truncate-quotient -5.0 -2)", 2.0)]
    [InlineData("(truncate-remainder 5 2)", 1)]
    [InlineData("(truncate-remainder -5 2)", -1)]
    [InlineData("(truncate-remainder 5 -2)", 1)]
    [InlineData("(truncate-remainder -5 -2)", -1)]
    [InlineData("(truncate-remainder -5.0 -2)", -1.0)]
    [Theory]
    public void FloorTruncateTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(gcd 32 -36)", 4)]
    [InlineData("(gcd)", 0)]
    [InlineData("(lcm 32 -36)", 288)]
    [InlineData("(lcm 32.0 -36)", 288.0)]
    [InlineData("(lcm)", 1)]
    [Theory]
    public void GcdLcmTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(str (numerator (/ 6 4)))", "3")]
    [InlineData("(str (denominator (/ 6 4)))", "2")]
    //[InlineData("(denominator (inexact (/ 6 4)))", 2.0)] // TODO: inexact function
    [Theory]
    public void NumeratorDenominatorTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(sqrt 25)", 5)]
    [InlineData("(sqrt (+ 2 2))", 2)]
    [Theory]
    public void SqrtTests(string input, double expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(>> 42 2)", 10)]
    [InlineData("(>> 42 2 1)", 5)]
    [InlineData("(<< 1 2)", 4)]
    [InlineData("(<< 1 2 7)", 512)]
    [Theory]
    public void ShiftTests(string input, int expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(log 1000)", 3d)]
    [InlineData("(log 144 12)", 2d)]
    [InlineData("(ln e)", 1d)]
    [Theory]
    public void LogarithmTests(string input, double expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.NotNull(result);
        Assert.Equal(expected, (double)result, 6);
    }

    private const double eSquared = 7.3890560989306495;

    [InlineData("(exp 1)", System.Math.E)]
    [InlineData("(exp 2)", eSquared)]
    [InlineData("(sin 0)", 0.0)]
    [InlineData("(cos 0)", 1.0)]
    [InlineData("(tan 0)", 0.0)]
    [InlineData("(asin 0)", 0.0)]
    [InlineData("(acos 1)", 0.0)]
    [InlineData("(atan 0)", 0.0)]
    [InlineData("(atan 0.5 0.5)", 0.78539816339744828)]
    [Theory]
    public void TranscendentalTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(floor 3.5)", 3.0)]
    [InlineData("(ceiling 3.5)", 4.0)]
    [InlineData("(truncate 3.5)", 3.0)]
    [InlineData("(round 3.5)", 4.0)]
    [InlineData("(floor -4.3)", -5.0)]
    [InlineData("(ceiling -4.3)", -4.0)]
    [InlineData("(truncate -4.3)", -4.0)]
    [InlineData("(round -4.3)", -4.0)]
    [InlineData("(round 7/2)", 4)]
    [InlineData("(round 7)", 7)]
    [Theory]
    public void RoundingTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(zero? 0)", true)]
    [InlineData("(zero? 0.0)", true)]
    [InlineData("(zero? 1)", false)]
    [InlineData("(positive? 0)", false)]
    [InlineData("(positive? 101)", true)]
    [InlineData("(positive? -30)", false)]
    [InlineData("(negative? 0)", false)]
    [InlineData("(negative? 101)", false)]
    [InlineData("(negative? -30)", true)]
    [InlineData("(odd? 0)", false)]
    [InlineData("(odd? 101)", true)]
    [InlineData("(odd? -30)", false)]
    [InlineData("(even? 0)", true)]
    [InlineData("(even? 101)", false)]
    [InlineData("(even? -30)", true)]
    [Theory]
    public void NumberPredicateTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(square 42)", 1764)]
    [InlineData("(square 2.0)", 4.0)]
    [Theory]
    public void SquareTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(expt 2 3)", 8.0)] // note: integer exponentiation is not yet supported
    [InlineData("(expt 2.0 3)", 8.0)]
    [InlineData("(expt 4 0.5)", 2.0)]
    [Theory]
    public void ExptTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(let-values (((root rem) (exact-integer-sqrt 4))) root)", 2)]
    [InlineData("(let-values (((root rem) (exact-integer-sqrt 4))) rem)", 0)]
    [InlineData("(let-values (((root rem) (exact-integer-sqrt 5))) root)", 2)]
    [InlineData("(let-values (((root rem) (exact-integer-sqrt 5))) rem)", 1)]
    [Theory]
    public void ExactIntegerSqrtTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}