using System;
using System.Threading;

namespace Lillisp.Core.Macros;

public static class ContinuationMacros
{
    public static object? CallWithCurrentContinuation(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("call-with-current-continuation requires one argument");
        }

        var procResult = runtime.Evaluate(scope, args[0]);
        
        MacroExpression escapeProcedure = (_, scope2, args2) =>
        {
            object? retVal = args2.Length switch
            {
                1 => runtime.Evaluate(scope2, args2[0]),
                0 => null,
                _ => throw new InvalidOperationException("Multiple return values from escape procedures not supported")
            };

            throw new EscapeProcedureException(retVal);
        };

        var context = ExecutionContext.Capture();
        bool value = false;

        try
        {
            value = true;
            ExecutionContext.Restore(context!);
            return runtime.InvokePossibleTailCallExpression(scope, procResult, new object?[] { escapeProcedure });
        }
        catch (EscapeProcedureException success)
        {
            return success.ReturnValue;
        }
    }

    public static object? DynamicWind(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length != 3)
        {
            throw new ArgumentException("dynamic-wind requires three arguments");
        }

        var before = runtime.Evaluate(scope, args[0]);
        var thunk = runtime.Evaluate(scope, args[1]);
        var after = runtime.Evaluate(scope, args[2]);

        scope.DynamicWind(before, after);

        var result = runtime.DynamicInvoke(scope, thunk, Array.Empty<object>());

        return result;
    }
}