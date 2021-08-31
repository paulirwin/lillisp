namespace Lillisp.Core.Syntax
{
    public class ExpressionNode : Node
    {
        public ExpressionNode()
            : base(NodeType.Expression)
        {
        }

        public ExpressionNode(Node[] nodes)
            : this()
        {
            foreach (var node in nodes)
            {
                this.Children.Add(node);
            }
        }

        public override string ToString() => $"({string.Join(' ', Children)})";
    }
}