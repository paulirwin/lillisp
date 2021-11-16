using System;
using System.Collections.Generic;

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

            if (source is not IEnumerable<object?> enumerable)
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
                        enumerable = Select(runtime, scope, enumerable, alias, projection);
                    }
                }
                else if (next is Symbol { Value: "where" })
                {
                    var condition = args[++i];

                    enumerable = Where(runtime, scope, enumerable, alias, condition);
                }
                else
                {
                    throw new NotImplementedException("LispINQ operator not yet implemented");
                }
            }

            return enumerable;
        }

        private static IEnumerable<object?> Select(LillispRuntime runtime, 
            Scope scope, IEnumerable<object?> enumerable, string alias, object? projection)
        {
            var childScope = scope.CreateChildScope();

            foreach (var item in enumerable)
            {
                childScope.DefineOrSet(alias, item);

                yield return runtime.Evaluate(childScope, projection);
            }
        }

        private static IEnumerable<object?> Where(LillispRuntime runtime, 
            Scope scope, IEnumerable<object?> enumerable, string alias, object? condition)
        {
            var childScope = scope.CreateChildScope();
            
            foreach (var item in enumerable)
            {
                childScope.DefineOrSet(alias, item);

                var conditionResult = runtime.Evaluate(childScope, condition);

                if (conditionResult.IsTruthy())
                {
                    yield return item;
                }
            }
        }
    }
}
