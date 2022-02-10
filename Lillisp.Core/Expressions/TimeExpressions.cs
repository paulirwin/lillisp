namespace Lillisp.Core.Expressions;

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

    /// <summary>
    /// Returns the current International Atomic Time (TAI) in seconds since midnight, January 1, 1970.
    /// </summary>
    /// <remarks>
    /// Scheme R7RS states that this should return the International Atomic Time (TAI) value,
    /// but that is not trivial to do. It also states that "neither high accuracy nor high precision
    /// is required", and that "returning Coordinated Universal Time plus a suitable
    /// constant might be the best an implementation can do." We're choosing to do that.
    ///
    /// Since 2017, as of 2021, "UTC is currently exactly 37 seconds behind TAI" [0] so this implementation
    /// adds 37 seconds to the UTC UNIX epoch value. This may need to be updated in the future.
    ///
    /// Also, it is in this author's opinion that this is a comically ridiculous requirement.
    ///
    /// [0]: https://en.wikipedia.org/wiki/International_Atomic_Time
    /// </remarks>
    /// <param name="args">The arguments to the function.</param>
    /// <returns>Returns an Int64 value.</returns>
    public static object? CurrentSecond(object?[] args)
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 37;
    }
}