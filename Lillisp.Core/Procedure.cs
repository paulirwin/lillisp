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
            else if (Parameters is Pair {IsList: true} parms)
            {
                var list = parms.ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    if (list.ElementAt(i) is not Atom {AtomType: AtomType.Symbol} atom)
                    {
                        throw new ArgumentException($"Unhandled parameter node type: {list[i].GetType()}");
                    }

                    if (atom.Value is ".")
                    {
                        if (list.Count > i + 2 || list.Count == i + 1)
                        {
                            throw new ArgumentException("One variable must follow the dot in lambda parameters");
                        }

                        if (list[i + 1] is not Atom {AtomType: AtomType.Symbol} restAtom)
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