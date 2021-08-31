using System.Collections.Generic;

namespace Lillisp.Core
{
    public abstract class Node
    {
        protected Node(NodeType type)
        {
            Type = type;
        }

        public NodeType Type { get; }

        public IList<Node> Children { get; } = new List<Node>();
    }
}
