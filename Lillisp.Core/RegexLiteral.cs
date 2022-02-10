using System.Text.RegularExpressions;

namespace Lillisp.Core;

public class RegexLiteral : Node
{
    public RegexLiteral(string pattern, string flags)
    {
        Pattern = pattern.Replace("\\/", "/").Replace("\\ ", " ");

        var flagChars = flags.ToCharArray();

        var options = flagChars.Aggregate(RegexOptions.None, (current, flag) => current | flag switch
        {
            'i' => RegexOptions.IgnoreCase,
            'm' => RegexOptions.Multiline,
            's' => RegexOptions.Singleline,
            _ => throw new ArgumentException($"Invalid/unsupported regex literal flag: {flag}")
        });

        Options = options;
    }

    public RegexLiteral(Regex regex)
    {
        Pattern = regex.ToString();
        Options = regex.Options;
    }

    public string Pattern { get; }

    public RegexOptions Options { get; }

    public override string ToString() => $"/{Pattern.Replace("/", "\\/").Replace(" ", "\\ ")}/{OptionsString}";

    public Regex ToRegex() => new(Pattern, Options);

    private string OptionsString
    {
        get
        {
            if (Options == RegexOptions.None)
            {
                return string.Empty;
            }

            string flags = "";

            if (Options.HasFlag(RegexOptions.IgnoreCase))
                flags += "i";
            if (Options.HasFlag(RegexOptions.Multiline))
                flags += "m";
            if (Options.HasFlag(RegexOptions.Singleline))
                flags += "s";

            return flags;
        }
    }
}