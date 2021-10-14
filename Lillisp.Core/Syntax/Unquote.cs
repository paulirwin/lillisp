namespace Lillisp.Core.Syntax
{
    public class Unquote : Node
    {
        public Unquote(Node value)
        {
            Value = value;
        }

        public Node Value { get; }

        public override string ToString() => $"'{Value}";
    }
}
