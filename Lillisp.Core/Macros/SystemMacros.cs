using System;
using System.Linq;

namespace Lillisp.Core.Macros
{
    public static class SystemMacros
    {
        public static object? Quote(LillispRuntime runtime, object?[] args)
        {
            if (args.Length == 0 || args[0] is not Node node)
            {
                throw new InvalidOperationException("quote requires an argument");
            }

            return runtime.Quote(node);
        }


        public static object? Apply(LillispRuntime runtime, object?[] args)
        {
            if (args.Length < 2 || args[0] is not Node source || args[1] is not Node target)
            {
                throw new InvalidOperationException("apply requires a fn and a list as arguments");
            }

            var sourceValue = runtime.Evaluate(source);

            if (sourceValue is not Expression expr)
            {
                throw new InvalidOperationException("First parameter to `apply` must evaluate to a fn");
            }

            var list = runtime.Evaluate(target);

            if (list is not object[] objArray)
            {
                throw new InvalidOperationException("Second parameter to `apply` must evaluate to a list");
            }

            return expr(objArray);
        }

        public static object? List(LillispRuntime runtime, object?[] args)
        {
            if (args.Length == 0)
            {
                return Array.Empty<object>();
            }

            return args.Cast<Node>().Select(runtime.Quote).ToArray();
        }
    }
}
