﻿using System.Collections.Generic;
using System.Linq;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests;

public class ListTests
{
    [Fact]
    public void List_FromNodes_EmptyList()
    {
        var list = List.FromNodes();

        Assert.Equal(Nil.Value, list);
    }

    [Fact]
    public void List_FromNodes_OneItem()
    {
        var list = List.FromNodes(new Atom(AtomType.Number, 1));

        var pair = list as Pair;

        Assert.NotNull(pair);
        Assert.Single(pair);
        Assert.Equal(1, pair.OfType<Atom>().First().Value);
        Assert.Equal(Nil.Value, pair.Cdr);
    }

    [Fact]
    public void List_FromNodes_TwoItems()
    {
        var list = List.FromNodes(new Atom(AtomType.Number, 1), new Atom(AtomType.Number, 2));

        var pair = list as Pair;

        Assert.NotNull(pair);
        Assert.Equal(2, pair.Count());
        Assert.Equal(1, pair.OfType<Atom>().First().Value);
        Assert.Equal(2, pair.OfType<Atom>().ElementAt(1).Value);
    }

    [Fact]
    public void List_FromNodes_ManyItems()
    {
        var atoms = Enumerable.Range(1, 5)
            .Select(i => new Atom(AtomType.Number, i))
            .Cast<Node>();

        var list = List.FromNodes(atoms);

        var pair = list as Pair;

        Assert.NotNull(pair);
        Assert.Equal(5, pair.Count());
        Assert.Equal(1, pair.OfType<Atom>().First().Value);
        Assert.Equal(2, pair.OfType<Atom>().ElementAt(1).Value);
        Assert.Equal(3, pair.OfType<Atom>().ElementAt(2).Value);
        Assert.Equal(4, pair.OfType<Atom>().ElementAt(3).Value);
        Assert.Equal(5, pair.OfType<Atom>().ElementAt(4).Value);
    }

    [InlineData("(car '(2 3))", 2)]
    [InlineData("(car '(2 3 4 5 6))", 2)]
    [Theory]
    public void CarTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(cdr '(2 3))", new object[] {3})]
    [InlineData("(cdr '(2 3 4 5 6))", new object[] {3, 4, 5, 6})]
    [Theory]
    public void CdrTests(string input, object[] expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, (IEnumerable<object>) result);
    }

    [InlineData("(cons 1 (list 2 3))", new object[] {1, 2, 3})]
    [InlineData("(cons 1 (cons 2 (cons 3 nil)))", new object[] {1, 2, 3})]
    [Theory]
    public void ConsTests(string input, object[] expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, (IEnumerable<object>) result);
    }

    [InlineData("(make-list 2)", new object[] { null, null })]
    [InlineData("(make-list 2 3)", new object[] { 3, 3 })]
    [Theory]
    public void MakeListTests(string input, object[] expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, (IEnumerable<object>)result);
    }

    [InlineData("(reverse '(1 2 3))", new object[] { 3, 2, 1 })]
    [InlineData("(list (caar (reverse '(1 (2 3) 4 (5 (6))))))", new object[] { 5 })]
    [Theory]
    public void ReverseTests(string input, object expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);

        Assert.Equal(expected, (IEnumerable<object>)result);
    }

    [InlineData("(list-tail '(1 2 3 4 5) 3)", new object[] { 4, 5 })]
    [InlineData("(list-tail '(1 2 3 4 5) 4)", new object[] { 5 })]
    [Theory]
    public void ListTailTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(list-ref '(1 2 3 4 5) 3)", 4)]
    [Theory]
    public void ListRefTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}