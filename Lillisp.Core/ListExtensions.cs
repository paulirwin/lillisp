using System.Collections.Generic;
using Lillisp.Core.Syntax;

namespace Lillisp.Core
{
    public static class ListExtensions
    {
        public static List ToListNode(this IEnumerable<Node> nodes) => new(nodes);
    }
}
