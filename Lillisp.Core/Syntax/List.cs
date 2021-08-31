using System.Collections.Generic;

namespace Lillisp.Core.Syntax
{
    public class List : Node
    {
        public List()
            : base(NodeType.Expression)
        {
        }

        public List(IEnumerable<Node> nodes)
            : this()
        {
            foreach (var node in nodes)
            {
                Children.Add(node);
            }
        }

        public IList<Node> Children { get; } = new List<Node>();

        public override string ToString() => $"({string.Join(' ', Children)})";
    }
}