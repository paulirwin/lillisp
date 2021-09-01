namespace Lillisp.Core
{
    internal static class ObjectExtensions
    {
        public static bool IsTruthy(this object? value)
        {
            return value switch
            {
                null => false,
                0 => false,
                0d => false,
                false => false,
                _ => true
            };
        }
    }
}
