namespace Lillisp.Core;

public class Unquote : Node
{
    public Unquote(Node value, bool splicing)
    {
        Value = value;
        Splicing = splicing;
    }

    public Node Value { get; }

    public bool Splicing { get; }

    public override string ToString() => $"{(Splicing ? ",@" : ",")}{Value}";
}