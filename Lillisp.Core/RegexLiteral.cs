using System.Text.RegularExpressions;

namespace Lillisp.Core;

public class RegexLiteral : Node
{
    public RegexLiteral(string pattern)
    {
        Pattern = pattern;
    }

    public string Pattern { get; }

    public override string ToString() => $"/{Pattern}/";

    public Regex ToRegex() => new(Pattern);
}