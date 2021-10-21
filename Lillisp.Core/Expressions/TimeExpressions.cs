using System;

namespace Lillisp.Core.Expressions
{
    public static class TimeExpressions
    {
        public static object? CurrentJiffy(object?[] args)
        {
            return DateTime.UtcNow.Ticks;
        }

        public static object? JiffiesPerSecond(object?[] args)
        {
            return TimeSpan.TicksPerSecond;
        }
    }
}
