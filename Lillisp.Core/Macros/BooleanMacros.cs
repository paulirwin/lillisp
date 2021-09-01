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
    }
}
