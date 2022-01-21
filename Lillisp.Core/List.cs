using System.Collections.Generic;
using System.Linq;

namespace Lillisp.Core
{
    public static class List
    {
        public static Node FromNodes(params object?[] nodes) => FromNodes((IEnumerable<object?>)nodes);

        public static Node FromNodes(IEnumerable<object?> nodes)
        {
            Pair? first = null, current = null;

            foreach (var node in nodes)
            {
                if (first == null)
                {
                    first = new Pair(node, Nil.Value);
                    current = first;
                }
                else
                {
                    var p = new Pair(node, Nil.Value);
                    current!.Cdr = p;
                    current = p;
                }
            }

            return (Node?)first ?? Nil.Value;
        }

        public static Node ImproperListFromNodes(IEnumerable<object?> nodes)
        {
            Pair? list = null, current = null;

            foreach (var node in nodes)
            {
                if (current == null)
                {
                    // first value
                    current = new Pair(node, null);
                    list = current;
                }
                else if (current.Cdr == null)
                {
                    // second value
                    current.Cdr = node;
                }
                else 
                {
                    var p = new Pair(current.Cdr, node);
                    current.Cdr = p;
                    current = p;
                }
            }

            return (Node?)list ?? Nil.Value;
        }

        public static Node SpreadCar(SyntaxRestArgs restArgs, object? cdr)
        {
            IEnumerable<object?> enumerable = restArgs.Children;

            if (cdr != null && cdr is not Nil)
            {
                enumerable = enumerable.Append(cdr);
            }

            return FromNodes(enumerable);
        }
    }
}