using System;

namespace Lillisp.Core.Syntax
{
    public class ProgramNode : Node
    {
        public ProgramNode()
            : base(NodeType.Program)
        {
        }

        public override string ToString() => string.Join(Environment.NewLine, Children);
    }
}
