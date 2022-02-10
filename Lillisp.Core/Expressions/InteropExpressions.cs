using LinqExpression = System.Linq.Expressions.Expression;

namespace Lillisp.Core.Expressions;

public class InteropExpressions
{
    private record CastPair(Type input, Type output);

    private static readonly IDictionary<CastPair, Delegate> _castMethodCache = new Dictionary<CastPair, Delegate>();

    public static object? Cast(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("cast requires two arguments");
        }

        if (args[0] == null)
        {
            return null;
        }

        if (args[1] is not Type type)
        {
            throw new ArgumentException("Second parameter must be a Type");
        }

        var arg0Type = args[0]!.GetType();

        var pair = new CastPair(arg0Type, type);

        if (_castMethodCache.TryGetValue(pair, out Delegate? cast) && cast != null)
        {
            return cast.DynamicInvoke(args[0]);
        }

        var param = LinqExpression.Parameter(arg0Type);
        var expr = LinqExpression.Lambda(LinqExpression.Convert(param, type), param);
        var del = expr.Compile();

        _castMethodCache[pair] = del;

        return del.DynamicInvoke(args[0]);
    }

    public static object? Convert(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("convert requires two arguments");
        }

        if (args[0] == null)
        {
            return null;
        }

        if (args[1] is not Type type)
        {
            throw new ArgumentException("Second parameter must be a Type");
        }

        return System.Convert.ChangeType(args[0], type);
    }
}