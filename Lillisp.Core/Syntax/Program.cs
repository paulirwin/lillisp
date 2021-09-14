using System;
using System.Collections.Generic;

namespace Lillisp.Core.Syntax
{
    public class Program : Node
    {
        public IList<Node> Children { get; } = new List<Node>();

        public override string ToString() => string.Join(Environment.NewLine, Children);
    }
}
