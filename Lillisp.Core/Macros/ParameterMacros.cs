using System;
using System.Linq;

namespace Lillisp.Core.Macros
{
    public static class ParameterMacros
    {
        public static object? MakeParameter(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length is 0 or > 2)
            {
                throw new ArgumentException("make-parameter requires one or two arguments");
            }

            var init = args[0] is Node node ? runtime.Evaluate(scope, node) : args[0];
            Procedure? converter = null;

            if (args.Length == 2)
            {
                if (args[1] is Node converterNode)
                {
                    converter = runtime.Evaluate(scope, converterNode) as Procedure;
                }
                else if (args[1] is Procedure procedure)
                {
                    converter = procedure;
                }
                else
                {
                    throw new ArgumentException("make-parameter's second argument must be a procedure");
                }

                if (converter != null)
                {
                    init = converter.Invoke(runtime, scope, new[] { init });
                }
            }

            return new Parameter(init, converter);
        }

        public static object? Parameterize(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("parameterize requires at least 2 arguments");
            }

            if (args[0] is not Pair parameters)
            {
                throw new ArgumentException("parameterize's first argument must be a pair");
            }

            // HACK: going to experiment with just creating a new parameter in the child scope for now,
            // copying the converter reference into the new parameter.
            // Thought about doing something like having a ParameterRef/Parameterized class that has
            // the new value and a reference to the original Parameter, and is also IInvokable. Also
            // considered storing parameters separately in the environment (Scope). But let's see how
            // this goes.

            var childScope = scope.CreateChildScope();

            foreach (var binding in parameters.Cast<Pair>())
            {
                if (binding.Car is not Symbol sym)
                {
                    throw new ArgumentException("Parameter bindings' first items must be a symbol");
                }

                Node bindingValue = binding.Cdr;

                if (bindingValue is Pair { IsList: true } bindingValuePair)
                {
                    bindingValue = bindingValuePair.Car;
                }

                object? value = runtime.Evaluate(scope, bindingValue);

                object? scopeValue = scope.Resolve(sym.Value);

                if (scopeValue is not Parameter p)
                {
                    throw new ArgumentException($"{sym.Value} is not a parameter");
                }

                if (p.Converter != null)
                {
                    value = p.Converter.Invoke(runtime, scope, new[] { value });
                }

                childScope.Define(sym.Value, new Parameter(value, p.Converter));
            }

            object? result = Nil.Value;

            foreach (var bodyArg in args.Skip(1))
            {
                result = bodyArg is Node node ? runtime.Evaluate(childScope, node) : bodyArg;
            }

            return result;
        }
    }
}
