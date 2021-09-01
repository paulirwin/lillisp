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

        public static object? If(LillispRuntime runtime, object?[] args)
        {
            if (args.Length is < 2 or > 3 || args[0] is not Node cond || args[1] is not Node consequence)
            {
                throw new ArgumentException("if requires 2 or 3 arguments");
            }

            Node? alt = null;

            if (args.Length == 3)
            {
                if (args[2] is not Node altNode)
                {
                    throw new ArgumentException("argument 3 must be a node");
                }

                alt = altNode;
            }

            var result = runtime.Evaluate(cond);

            if (result.IsTruthy())
            {
                return runtime.Evaluate(consequence);
            }
            
            return alt != null ? runtime.Evaluate(alt) : Array.Empty<object>();
        }
    }
}
