namespace Lillisp.Core
{
    internal static class ObjectExtensions
    {
        public static bool IsTruthy(this object? value) => value is not false;
    }
}
