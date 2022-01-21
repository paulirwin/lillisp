using System;
using System.Collections.Generic;

namespace Lillisp.Core;

public class Syntax
{
    public Syntax(Scope scope)
    {
        Scope = scope;
    }

    public Scope Scope { get; }

    public IList<Symbol> Literals { get; set; } = new List<Symbol>();

    public IList<SyntaxRule> Rules { get; set; } = new List<SyntaxRule>();

    public Node? Transform(Node[] args)
    {
        foreach (var rule in Rules)
        {
            if (rule.TryTransform(Scope, args, out var node) && node != null)
            {
                return node;
            }
        }

        throw new InvalidOperationException("No rule matches form");
    }

    public override string ToString() => $"(syntax-rules ({string.Join(' ', Literals)}){(Rules.Count > 0 ? " " : "")}{string.Join(' ', Rules)})";
}