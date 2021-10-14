namespace Lillisp.Core.Syntax
{
    public class Quasiquote : Node
    {
        public Quasiquote(Node value)
        {
            Value = value;
        }

        public Node Value { get; }

        public override string ToString() => $"'{Value}";
    }
}
