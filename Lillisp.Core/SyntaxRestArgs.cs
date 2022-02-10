namespace Lillisp.Core;

public class SyntaxRestArgs : Node
{
    public SyntaxRestArgs(IList<Node> children)
    {
        Children = children;
    }

    public IList<Node> Children { get; }

    public override string ToString() => string.Join(" ", Children);
}