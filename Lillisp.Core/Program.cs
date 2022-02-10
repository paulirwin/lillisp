namespace Lillisp.Core;

public class Program : Node
{
    public IList<Node> Children { get; } = new List<Node>();

    public override string ToString() => string.Join(Environment.NewLine, Children);
}