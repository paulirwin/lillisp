using System;
using System.Linq;
using BindingFlags = System.Reflection.BindingFlags;

namespace Lillisp.Core.Macros
{
    public static class InteropMacros
    {
        public static object? Use(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("use requires at least one namespace argument");
            }

            // TODO: validate namespaces are real?
            foreach (var arg in args)
            {
                if (arg is Node node)
                {
                    var value = runtime.Evaluate(node);

                    if (value is not string str)
                    {
                        throw new ArgumentException($"Argument {arg} did not evaluate to a string");
                    }

                    scope.InteropNamespaces.Add(str);
                }
                else
                {
                    if (arg is not string ns)
                    {
                        throw new ArgumentException($"Argument {arg} did not evaluate to a string");
                    }

                    scope.InteropNamespaces.Add(ns);
                }
            }

            return Nil.Value;
        }

        public static object? New(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0 || args[0] is not Node typeNode)
            {
                throw new ArgumentException("new requires at least one argument");
            }

            var typeValue = runtime.Evaluate(typeNode);

            if (typeValue is not Type type)
            {
                throw new ArgumentException("First parameter must evaluate to a System.Type");
            }

            var ctorParams = args.Skip(1)
                .Select(i => i is Node node ? runtime.Evaluate(node) : i)
                .ToArray();

            return Activator.CreateInstance(type, ctorParams);
        }
    }
}
