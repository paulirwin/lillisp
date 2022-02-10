namespace Lillisp.Core;

internal static class CondClauseUtility
{
    public static bool EvaluateCondClause(LillispRuntime runtime, Scope scope, Pair clause, out object? result)
    {
        result = null;

        var test = runtime.Evaluate(scope, clause.Car);

        if (!test.IsTruthy())
            return false;

        var clauseForms = clause.ToList();

        if (clauseForms.Count == 1)
        {
            result = test;

            return true;
        }

        if (clauseForms.Count == 3 && clauseForms[1] is Symbol { Value: "=>" })
        {
            var expr = clauseForms[2];
            var proc = runtime.Evaluate(scope, expr);
            result = LillispRuntime.TailCall(scope, new Pair(proc, new Pair(new Atom(AtomType.RuntimeReference, test), Nil.Value)));

            return true;
        }

        for (int i = 1; i < clauseForms.Count; i++)
        {
            var expr = clauseForms[i];

            result = (i == clauseForms.Count - 1 && expr is Pair pair) ? LillispRuntime.TailCall(scope, pair) : runtime.Evaluate(scope, expr);
        }

        return true;
    }

    public static object? EvaluateCondElseClause(LillispRuntime runtime, Scope scope, Pair elseClause)
    {
        var elseExprs = elseClause.Skip(1).ToList();

        object? result = Nil.Value;

        for (int i = 0; i < elseExprs.Count; i++)
        {
            var expr = elseExprs[i];

            result = (i == elseExprs.Count - 1 && expr is Pair pair) ? LillispRuntime.TailCall(scope, pair) : runtime.Evaluate(scope, expr);
        }

        return result;
    }
}