namespace Lillisp.Core.Syntax
{
    public class Quote : Node
    {
        public Quote(Node value) 
            : base(NodeType.Quote)
        {
            Value = value;
        }

        public Node Value { get; }

        public override string ToString() => $"'{Value}";
    }
}
