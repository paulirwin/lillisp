namespace Lillisp.Core.Syntax
{
    public class AtomNode : Node
    {
        public AtomNode(AtomType atomType, object value)
            : base(NodeType.Atom)
        {
            AtomType = atomType;
            Value = value;
        }

        public AtomType AtomType { get; }

        public object Value { get; }

        public override string ToString() => Value?.ToString() ?? "null";
    }
}
