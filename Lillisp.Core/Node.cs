namespace Lillisp.Core
{
    public abstract class Node
    {
        protected Node(NodeType type)
        {
            Type = type;
        }

        public NodeType Type { get; }
    }
}
