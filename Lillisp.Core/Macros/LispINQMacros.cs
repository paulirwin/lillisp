using System.Collections;

namespace Lillisp.Core.Macros;

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
            if (source is not IEnumerable nonGenericEnumerable)
            {
                throw new InvalidOperationException("Source must be enumerable");
            }

            enumerable = nonGenericEnumerable.Cast<object?>();
        }

        for (int i = 3; i < args.Length; i++)
        {
            var next = args[i];

            if (next is not Symbol nextSymbol)
            {
                throw new InvalidOperationException("Invalid LispINQ syntax");
            }

            if (nextSymbol.Value == "select")
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
            else if (nextSymbol.Value == "where")
            {
                var condition = args[++i];

                enumerable = Where(runtime, scope, enumerable, alias, condition);
            }
            else if (nextSymbol.Value == "orderby")
            {
                var selector = args[++i];
                bool desc = false;
                    
                if (args.Length > i + 2 && args[i + 1] is Symbol { Value: "desc" })
                {
                    desc = true;
                    i++;
                }

                enumerable = OrderBy(runtime, scope, enumerable, alias, selector, desc);
            }
            else if (nextSymbol.Value == "thenby")
            {
                var selector = args[++i];
                bool desc = false;

                if (args.Length > i + 2 && args[i + 1] is Symbol { Value: "desc" })
                {
                    desc = true;
                    i++;
                }

                if (enumerable is not IOrderedEnumerable<object?> orderedEnumerable)
                {
                    throw new InvalidOperationException("thenby must be used on an (IOrderedEnumerable Object). Did you forget to call 'orderby'?");
                }

                enumerable = ThenBy(runtime, scope, orderedEnumerable, alias, selector, desc);
            }
            else if (nextSymbol.Value == "let")
            {
                var bindings = args[++i];

                if (bindings is not Pair bindingsPair)
                {
                    throw new InvalidOperationException("Following 'let' must be a list of bindings");
                }

                scope = scope.CreateChildScope();

                enumerable = Let(runtime, scope, enumerable, alias, bindingsPair);
            }
            else
            {
                throw new NotImplementedException($"LispINQ operator {nextSymbol.Value} not implemented");
            }
        }

        return enumerable;
    }

    private static IEnumerable<object?> Let(LillispRuntime runtime, Scope scope, IEnumerable<object?> enumerable, string alias, Pair bindingsPair)
    {
        var bindings = new List<(Symbol Symbol, Node Expression)>();

        foreach (var bindingValue in bindingsPair)
        {
            if (bindingValue is not Pair bindingPair)
            {
                throw new InvalidOperationException("Invalid LispINQ syntax: 'let' bindings must be lists");
            }

            if (bindingPair.Car is not Symbol bindingSymbol)
            {
                throw new InvalidOperationException("Invalid LispINQ syntax: car of 'let' binding pair must be a symbol");
            }

            if (bindingPair.Cdr is not Pair { Car: Node bindingExpression })
            {
                throw new InvalidOperationException("Invalid LispINQ syntax: cadr of 'let' binding pair must be an expression");
            }

            bindings.Add((bindingSymbol, bindingExpression));
        }

        foreach (var item in enumerable)
        {
            scope.DefineOrSet(alias, item);

            foreach (var (symbol, expression) in bindings)
            {
                scope.DefineOrSet(symbol.Value, runtime.Evaluate(scope, expression));
            }

            yield return item;
        }
    }

    private static IEnumerable<object?> OrderBy(LillispRuntime runtime, 
        Scope scope, IEnumerable<object?> enumerable, string alias, object? selector, bool desc)
    {
        var childScope = scope.CreateChildScope();

        if (!desc)
        {
            return enumerable
                .OrderBy(i =>
                {
                    childScope.DefineOrSet(alias, i);
                    return runtime.Evaluate(childScope, selector);
                });
        }
        else
        {
            return enumerable
                .OrderByDescending(i =>
                {
                    childScope.DefineOrSet(alias, i);
                    return runtime.Evaluate(childScope, selector);
                });
        }
    }

    private static IEnumerable<object?> ThenBy(LillispRuntime runtime,
        Scope scope, IOrderedEnumerable<object?> enumerable, string alias, object? selector, bool desc)
    {
        var childScope = scope.CreateChildScope();

        if (!desc)
        {
            return enumerable
                .ThenBy(i =>
                {
                    childScope.DefineOrSet(alias, i);
                    return runtime.Evaluate(childScope, selector);
                });
        }
        else
        {
            return enumerable
                .ThenByDescending(i =>
                {
                    childScope.DefineOrSet(alias, i);
                    return runtime.Evaluate(childScope, selector);
                });
        }
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