using System;
using Lillisp.Core.Syntax;

namespace Lillisp.Core.Macros
{
    public static class MathMacros
    {
        public static object? Increment(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 1 || args[0] is not Atom { AtomType: AtomType.Symbol, Value: string symbol } atom)
            {
                throw new ArgumentException("++! requires one symbol argument");
            }

            object? value = scope.Resolve(symbol);

            double d = Convert.ToDouble(value) + 1;

            scope.Set(symbol, d);

            return atom;
        }

        public static object? Decrement(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 1 || args[0] is not Atom { AtomType: AtomType.Symbol, Value: string symbol } atom)
            {
                throw new ArgumentException("++! requires one symbol argument");
            }

            object? value = scope.Resolve(symbol);

            double d = Convert.ToDouble(value) - 1;

            scope.Set(symbol, d);

            return atom;
        }
    }
}
