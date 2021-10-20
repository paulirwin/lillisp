namespace Lillisp.Core
{
    public class TailCall
    {
        public TailCall(Scope scope, Pair node)
        {
            Scope = scope;
            Node = node;
        }

        public Scope Scope { get; }

        public Pair Node { get; }
    }
}
