﻿using System.Collections.Generic;

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
    }
}