namespace Lillisp.Core;

public class SyntaxBinding : Symbol
{
    public SyntaxBinding(Symbol symbol, Scope scope)
        : base(symbol.Value)
    {
        Scope = scope;
    }

    public Scope Scope { get; }

    protected bool Equals(SyntaxBinding other)
    {
        return base.Equals(other) && Scope.Equals(other.Scope);
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
        return HashCode.Combine(base.GetHashCode(), Scope);
    }
}