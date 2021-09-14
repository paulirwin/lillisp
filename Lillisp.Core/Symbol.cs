namespace Lillisp.Core
{
    public class Symbol : Node
    {
        public Symbol(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() => Value;

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
