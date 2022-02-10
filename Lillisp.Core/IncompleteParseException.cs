namespace Lillisp.Core;

public sealed class IncompleteParseException : Exception
{
    public IncompleteParseException()
        : base("The parsed input code is incomplete, you may be forgetting a closing parenthesis or bracket.")
    {
    }
}