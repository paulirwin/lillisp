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

            foreach (var arg in args[1..])
            {
                if (arg is Node node)
                {
                    runtime.Evaluate(scope, node);
                }
            }

            return Nil.Value;
        }
    }
}
