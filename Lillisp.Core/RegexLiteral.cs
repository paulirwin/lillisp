using System.Text.RegularExpressions;

namespace Lillisp.Core;

public class RegexLiteral : Node
{
    public RegexLiteral(string pattern)
    {
        Pattern = pattern.Replace("\\/", "/").Replace("\\ ", " ");
    }

    public RegexLiteral(Regex regex)
    {
        Pattern = regex.ToString();
    }

    public string Pattern { get; }

    public override string ToString() => $"/{Pattern.Replace("/", "\\/").Replace(" ", "\\ ")}/";

    public Regex ToRegex() => new(Pattern);
}