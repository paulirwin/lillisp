using System;

namespace Lillisp.Core;

public class SyntaxBinding : Node
{
    public SyntaxBinding(Symbol symbol, Scope scope)
    {
        Symbol = symbol;
        Scope = scope;
    }

    public Symbol Symbol { get; }

    public Scope Scope { get; }

    public override string ToString() => Symbol.ToString();

    protected bool Equals(SyntaxBinding other)
    {
        return Symbol.Equals(other.Symbol) && Scope.Equals(other.Scope);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SyntaxBinding)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Symbol, Scope);
    }
}