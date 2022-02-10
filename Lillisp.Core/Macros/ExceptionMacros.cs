namespace Lillisp.Core.Macros;

public static class ExceptionMacros
{
    public static object? WithExceptionHandler(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("with-exception-handler requires two arguments");
        }

        if (args[0] is not Node handler
            || runtime.Evaluate(scope, handler) is not Procedure { Parameters: Pair { IsList: true, Cdr: Nil, Car: not Nil } } handlerProc)
        {
            throw new ArgumentException("with-exception-handler's first argument must be a function with a single parameter");
        }

        if (args[1] is not Node thunk
            || runtime.Evaluate(scope, thunk) is not Procedure { Parameters: Pair { IsList: true, Cdr: Nil, Car: Nil } or Nil } thunkProc)
        {
            throw new ArgumentException("with-exception-handler's second argument must be a function with no parameters");
        }

        scope = scope.CreateChildScope();
        scope.ExceptionHandler = handlerProc;
            
        try
        {
            return thunkProc.Invoke(runtime, scope, Array.Empty<object?>(), disableTailCalls: true);
        }
        catch (RaisedException ex)
        {
            var result = handlerProc.Invoke(runtime, scope, new[] { ex.Expression }, disableTailCalls: true);

            if (result != null && result is not Nil)
            {
                throw new InvalidOperationException("Exception handlers for raise must not return.");
            }
        }
        catch (Exception ex) // includes ErrorException 
        {
            var result = handlerProc.Invoke(runtime, scope, new object?[] { ex }, disableTailCalls: true);

            if (result != null && result is not Nil)
            {
                throw new InvalidOperationException("Exception handlers for raise must not return.");
            }
        }

        return Nil.Value;
    }

    public static object? RaiseContinuable(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length > 1)
        {
            throw new ArgumentException("raise-continuable requires zero or one argument");
        }

        Scope? lastHandlerScope = scope;

        while (lastHandlerScope is { ExceptionHandler: null })
        {
            lastHandlerScope = lastHandlerScope.Parent;
        }

        Procedure? handlerProc = lastHandlerScope?.ExceptionHandler;

        object? arg = args.Length == 1 ? args[0] : null;

        if (arg is Node node)
        {
            arg = runtime.Evaluate(scope, node);
        }

        if (handlerProc == null)
        {
            throw new RaisedException(arg);
        }

        return handlerProc.Invoke(runtime, scope, new[] { arg }, disableTailCalls: true);
    }

    public static object? Guard(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length == 0 || args[0] is not Pair { IsList: true } condList)
        {
            throw new ArgumentException("guard requires at least one cond list argument");
        }

        if (condList.Car is not Symbol variableName)
        {
            throw new ArgumentException("First item in the guard cond list must be a variable name");
        }

        var body = args.Skip(1).ToList();

        try
        {
            object? result = Nil.Value;
                
            foreach (var node in body)
            {
                result = runtime.Evaluate(scope, node);
            }

            return result;
        }
        catch (RaisedException ex)
        {
            return EvaluateGuardCondList(runtime, scope, condList, variableName.Value, ex.Expression);
        }
        catch (Exception ex)
        {
            return EvaluateGuardCondList(runtime, scope, condList, variableName.Value, ex);
        }
    }

    private static object? EvaluateGuardCondList(LillispRuntime runtime, Scope scope, Pair condList, string variableName, object? variable)
    {
        var childScope = scope.CreateChildScope();
        childScope.Define(variableName, variable);

        var clauses = condList.Skip(1).Cast<Pair>().ToArray();
        Pair? elseClause = null;

        if (clauses[^1].Car is Symbol { Value: "else" })
        {
            elseClause = clauses[^1];
            clauses = clauses[..^1];
        }

        foreach (var clause in clauses)
        {
            if (CondClauseUtility.EvaluateCondClause(runtime, childScope, clause, out var result))
            {
                return result;
            }
        }

        if (elseClause != null)
        {
            return CondClauseUtility.EvaluateCondElseClause(runtime, childScope, elseClause);
        }

        throw new InvalidOperationException("No clause matched for the cond expression");
    }
}