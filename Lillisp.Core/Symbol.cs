using Microsoft.CodeAnalysis.CSharp;

namespace Lillisp.Core
{
    public class Symbol : Node
    {
        public Symbol(string value, bool escaped = false)
        {
            Value = value;
            Escaped = escaped;
        }

        public string Value { get; }

        public bool Escaped { get; }

        public override string ToString() => Escaped ? $"|{SymbolDisplay.FormatLiteral(Value, false)}|" : Value;

        protected bool Equals(Symbol other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Symbol) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
