namespace Lillisp.Core;

public sealed class ReadError : Exception
{
    public ReadError(Exception innerException)
        : base(innerException.Message, innerException)
    {
    }
}