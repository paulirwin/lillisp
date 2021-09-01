using System;
using System.Collections.Generic;
using Lillisp.Core.Syntax;

namespace Lillisp.Core
{
    public class Procedure
    {
        public Procedure(string text, IList<Atom> parameters, Node body)
        {
            Text = text;
            Parameters = parameters;
            Body = body;
        }

        public string Text { get; }

        public IList<Atom> Parameters { get; }

        public Node Body { get; }

        public override string ToString() => Text;

        public object? Invoke(LillispRuntime runtime, Scope scope, object?[] arguments)
        {
            if (Parameters.Count != arguments.Length)
            {
                throw new ArgumentException("Arguments provided do not match parameter count");
            }

            var childScope = scope.CreateChildScope();

            for (int i = 0; i < Parameters.Count; i++)
            {
                var parm = Parameters[i];
                var arg = arguments[i];

                childScope[parm.Value!.ToString()!] = arg;
            }

            return runtime.Evaluate(childScope, Body);
        }
    }
}
