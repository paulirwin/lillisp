using System;
using System.Linq;
using Lillisp.Core.Syntax;

namespace Lillisp.Core.Expressions
{
    public static class ListExpressions
    {
        public static object? Car(object?[] args)
        {
            if (args.Length == 0 || args[0] is not ExpressionNode expr)
            {
                throw new ArgumentException("List must contain an expression");
            }

            return expr.Children[0];
        }

        public static object? Cdr(object?[] args)
        {
            if (args.Length == 0 || args[0] is not ExpressionNode expr)
            {
                throw new ArgumentException("List must contain an expression");
            }

            return new ExpressionNode(expr.Children.Skip(1).ToArray());
        }
    }
}
