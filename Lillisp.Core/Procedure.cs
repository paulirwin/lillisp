using System;
using System.Linq;

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

            if (Parameters is Symbol pSymbol)
            {
                childScope[pSymbol.Value] = arguments;
            }
            else if (Parameters is Pair {IsList: true} parms)
            {
                var list = parms.ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    if (list.ElementAt(i) is not Symbol symbol)
                    {
                        throw new ArgumentException($"Unhandled parameter node type: {list[i].GetType()}");
                    }

                    if (symbol.Value is ".")
                    {
                        if (list.Count > i + 2 || list.Count == i + 1)
                        {
                            throw new ArgumentException("One variable must follow the dot in lambda parameters");
                        }

                        if (list[i + 1] is not Symbol restSymbol)
                        {
                            throw new ArgumentException("Variable must follow the dot in lambda parameters");
                        }

                        childScope[restSymbol.Value] = arguments.Skip(i).ToArray();
                        
                        break;
                    }

                    if (arguments.Length > i)
                    {
                        var arg = arguments[i];

                        childScope[symbol.Value] = arg;
                    }
                }
            }

            return Body is Pair pair ? runtime.TailCall(childScope, pair) : runtime.Evaluate(scope, Body);
        }
    }
}