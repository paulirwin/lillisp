namespace Lillisp.Core;

public sealed class EofObject
{
    public static readonly EofObject Instance = new();

    private EofObject() {}

    public override string ToString() => "{EOF}";
}