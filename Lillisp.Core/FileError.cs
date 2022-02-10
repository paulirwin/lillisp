namespace Lillisp.Core;

public sealed class FileError : Exception
{
    public FileError(Exception innerException)
        : base(innerException.Message, innerException)
    {
    }
}