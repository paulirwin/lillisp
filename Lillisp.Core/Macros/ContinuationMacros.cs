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

        // TODO: support delegates too
        if (procResult is not Procedure proc)
        {
            throw new ArgumentException("call-with-current-continuation's first argument must be a procedure");
        }
        
        Expression arg = args2 =>
        {
            object? retVal = args2.Length switch
            {
                1 => args2[0],
                0 => null,
                _ => throw new ArgumentException("Escape procedure requires no more than one argument")
            };

            throw new ThrowSuccessException(retVal);
        };

        try
        {
            var result = proc.Invoke(runtime, scope, new object?[] { arg });

            while (result is TailCall tailCall)
            {
                result = runtime.Evaluate(tailCall.Scope, tailCall.Node);
            }

            return result;
        }
        catch (ThrowSuccessException success)
        {
            return success.ReturnValue;
        }
    }

    private class ThrowSuccessException : Exception
    {
        public ThrowSuccessException(object? returnValue)
        {
            ReturnValue = returnValue;
        }

        public object? ReturnValue { get; }
    }
}