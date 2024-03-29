﻿using Lillisp.Core;
// ReSharper disable StringLiteralTypo

namespace Lillisp.Tests;

public class QuasiquoteTests
{
    [Fact]
    public void BasicQuasiquoteSplicingTest()
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram("`(+ ,@(range 0 3))") as Pair;

        Assert.NotNull(result);

        var resultList = result!.ToList();

        Assert.Equal(4, resultList.Count);

        var sym = resultList[0] as Symbol;

        Assert.NotNull(sym);
        Assert.Equal("+", sym!.Value);

        Assert.Equal(0, resultList[1]);
        Assert.Equal(1, resultList[2]);
        Assert.Equal(2, resultList[3]);
    }

    [Fact]
    public void BasicQuasiquoteSplicingTest_ProcedureForm()
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram("(quasiquote (+ (unquote-splicing (range 0 3))))") as Pair;

        Assert.NotNull(result);

        var resultList = result!.ToList();

        Assert.Equal(4, resultList.Count);

        var sym = resultList[0] as Symbol;

        Assert.NotNull(sym);
        Assert.Equal("+", sym!.Value);

        Assert.Equal(0, resultList[1]);
        Assert.Equal(1, resultList[2]);
        Assert.Equal(2, resultList[3]);
    }

    [Fact]
    public void BasicEvalTest()
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram("(eval `(+ ,@(range 0 10)))");

        Assert.Equal(45, result);
    }

    [Fact]
    public void EvalDefineTest()
    {
        var runtime = new LillispRuntime();

        const string program = "(define x 42)\n(eval `(define ,(string->symbol (string-append \"myvar\" (str x))) x))\nmyvar42";

        var result = runtime.EvaluateProgram(program);

        Assert.Equal(42, result);
    }

    [Fact]
    public void EvalDefineTest_ProceduralForm()
    {
        var runtime = new LillispRuntime();

        const string program = "(define x 42)\n(eval (quasiquote (define (unquote (string->symbol (string-append \"myvar\" (str x)))) x)))\nmyvar42";

        var result = runtime.EvaluateProgram(program);

        Assert.Equal(42, result);
    }
}