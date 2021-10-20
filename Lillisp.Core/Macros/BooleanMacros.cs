using System;

namespace Lillisp.Core.Macros
{
    public static class BooleanMacros
    {
        public static object? And(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("and/&& requires at least two arguments");
            }
            
            foreach (var arg in args)
            {
                object? value = arg is Node node ? runtime.Evaluate(scope, node) : arg;

                if (!value.IsTruthy())
                    return false;
            }

            return true;
        }

        public static object? Or(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("or/|| requires at least two arguments");
            }

            foreach (var arg in args)
            {
                object? value = arg is Node node ? runtime.Evaluate(scope, node) : arg;

                if (value.IsTruthy())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// The test is evaluated, and if it evaluates to
        /// a true value, the expressions are evaluated in order.
        /// </summary>
        /// <remarks>
        /// Scheme R7RS states that the return value is unspecified. In order to support tail recursion,
        /// we are returning the result of the last expression if the test evaluates true, otherwise nil. 
        /// </remarks>
        /// <param name="runtime">The current runtime.</param>
        /// <param name="scope">The current scope (environment)</param>
        /// <param name="args">The arguments to `when`</param>
        /// <returns>Returns the result of the last expression, or nil.</returns>
        public static object? When(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 2 || args[0] is not Node test)
            {
                throw new ArgumentException("when requires at least a test and an expression argument");
            }

            var result = runtime.Evaluate(scope, test);

            if (!result.IsTruthy())
            {
                return Nil.Value;
            }

            for (int i = 1; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg is Node node)
                {
                    result = (i == args.Length - 1 && node is Pair pair) ? runtime.TailCall(scope, pair) : runtime.Evaluate(scope, node);
                }
            }

            return result;
        }
    }
}
