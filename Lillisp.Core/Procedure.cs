using System;
using System.Linq;
using Lillisp.Core.Syntax;

namespace Lillisp.Core
{
    public class Procedure
    {
        public Procedure(string text, Node parameters, Node body)
        {
            Text = text;
            Parameters = parameters;
            Body = body;
        }

        public string Text { get; }

        public Node Parameters { get; }

        public Node Body { get; }

        public override string ToString() => Text;

        public object? Invoke(LillispRuntime runtime, Scope scope, object?[] arguments)
        {
            var childScope = scope.CreateChildScope();

            if (Parameters is Atom pAtom)
            {
                childScope[pAtom.Value!.ToString()!] = arguments;
            }
            else if (Parameters is List list)
            {
                for (int i = 0; i < list.Children.Count; i++)
                {
                    if (list.Children[i] is not Atom {AtomType: AtomType.Symbol} atom)
                    {
                        throw new ArgumentException($"Unhandled parameter node type: {list.Children[i].Type}");
                    }

                    if (atom.Value is ".")
                    {
                        if (list.Children.Count > i + 2 || list.Children.Count == i + 1)
                        {
                            throw new ArgumentException("One variable must follow the dot in lambda parameters");
                        }

                        if (list.Children[i + 1] is not Atom {AtomType: AtomType.Symbol} restAtom)
                        {
                            throw new ArgumentException("Variable must follow the dot in lambda parameters");
                        }

                        childScope[restAtom.Value!.ToString()!] = arguments.Skip(i).ToArray();
                        
                        break;
                    }

                    if (arguments.Length > i)
                    {
                        var arg = arguments[i];

                        childScope[atom.Value!.ToString()!] = arg;
                    }
                }
            }

            return runtime.Evaluate(childScope, Body);
        }
    }
}