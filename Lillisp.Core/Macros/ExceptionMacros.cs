using System;

namespace Lillisp.Core.Macros
{
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
    }
}
