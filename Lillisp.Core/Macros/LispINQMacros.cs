using System;
using System.Collections;

namespace Lillisp.Core.Macros
{
    public static class LispINQMacros
    {
        public static object? From(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 5 || args[0] is not Symbol aliasSymbol || args[1] is not Symbol { Value: "in" })
            {
                throw new ArgumentException("Invalid LispINQ syntax");
            }

            var childScope = scope.CreateChildScope();

            var alias = aliasSymbol.Value;
            var source = runtime.Evaluate(childScope, args[2]);

            if (source is not IEnumerable enumerable)
            {
                throw new InvalidOperationException("Source must be enumerable");
            }

            for (int i = 3; i < args.Length; i++)
            {
                var next = args[i];

                if (next is Symbol { Value: "select" })
                {
                    var projection = args[++i];

                    if (projection is Symbol projSym && projSym.Value.Equals(alias))
                    {
                        // selecting the alias is a no-op projection
                        break;
                    }
                    else
                    {
                        throw new NotImplementedException("Projections are not yet supported");
                    }
                }
                else
                {
                    throw new NotImplementedException("LispINQ operator not yet implemented");
                }
            }

            return enumerable;
        }
    }
}
