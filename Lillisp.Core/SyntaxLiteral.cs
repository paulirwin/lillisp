namespace Lillisp.Core;

public class SyntaxLiteral : Node
{
    public SyntaxLiteral(Symbol symbol)
    {
        Symbol = symbol;
    }

    public Symbol Symbol { get; }

    public override string ToString() => Symbol.ToString();

    protected bool Equals(SyntaxLiteral other)
    {
        return Symbol.Equals(other.Symbol);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SyntaxLiteral)obj);
    }

    public override int GetHashCode()
    {
        return Symbol.GetHashCode();
    }
}