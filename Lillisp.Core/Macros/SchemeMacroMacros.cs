using System;
using System.Collections.Generic;
using System.Linq;

namespace Lillisp.Core.Macros;

public static class SchemeMacroMacros
{
    public static object? DefineSyntax(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("define-syntax requires two arguments");
        }

        if (args[0] is not Symbol keyword)
        {
            throw new ArgumentException("define-syntax's first argument must be an identifier");
        }

        if (args[1] is not Pair { Car: Symbol { Value: "syntax-rules" } } syntaxRules)
        {
            throw new ArgumentException("define-syntax's second argument must be a transformer spec");
        }

        var macro = BuildMacro(syntaxRules);

        scope.Define(keyword.Value, macro);

        return keyword;
    }

    private static Syntax BuildMacro(Pair syntaxRules)
    {
        var args = syntaxRules.Skip(1).ToList();
        var literals = args[0] is Pair literalPair ? literalPair.Cast<Symbol>().ToList() : new List<Symbol>();
        var syntax = new Syntax { Literals = literals };

        foreach (var syntaxRule in args.Skip(1).Cast<Pair>())
        {
            if (syntaxRule.Car is not Pair { Car: Symbol keyword } pattern)
            {
                throw new ArgumentException("Pattern for a syntax rule must be a list with first element as an identifier");
            }

            if (syntaxRule.Cdr is not Pair { Car: Node templateNode })
            {
                throw new ArgumentException("Template for a syntax rule must be a node");
            }

            var patternNodes = pattern.Skip(1)
                .Cast<Node>()
                .Select(n => ResolvePatternNode(n, literals))
                .ToList();

            var rule = new SyntaxRule(keyword, patternNodes, templateNode);


            syntax.Rules.Add(rule);
        }

        return syntax;
    }

    private static Node ResolvePatternNode(Node node, IList<Symbol> literals)
    {
        return node switch
        {
            Symbol symbol => literals.Contains(symbol) ? new SyntaxLiteral(symbol) : symbol,
            Pair { Car: Node carNode, Cdr: Node cdrNode } pair => new Pair(ResolvePatternNode(carNode, literals), ResolvePatternNode(cdrNode, literals)), 
            _ => node,
        };
    }

    public static object? LetSyntax(LillispRuntime runtime, Scope scope, object?[] args)
    {
        throw new NotImplementedException();
    }
}