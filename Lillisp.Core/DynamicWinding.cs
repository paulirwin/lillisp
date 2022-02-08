namespace Lillisp.Core
{
    public class DynamicWinding
    {
        public DynamicWinding(Scope scope, DynamicWinding? parent, object? before, object? after)
        {
            Scope = scope;
            Parent = parent;
            Before = before;
            After = after;
        }

        public Scope Scope { get; }

        public DynamicWinding? Parent { get; }

        public object? Before { get; }

        public object? After { get; }
    }
}
