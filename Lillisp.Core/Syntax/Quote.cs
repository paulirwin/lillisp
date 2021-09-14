namespace Lillisp.Core.Syntax
{
    public class Quote : Node
    {
        public Quote(Node value)
        {
            Value = value;
        }

        public Node Value { get; }

        public override string ToString() => $"'{Value}";
    }
}
