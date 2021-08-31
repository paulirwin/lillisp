using System;
using System.Linq;
using Lillisp.Core.Syntax;

namespace Lillisp.Core.Expressions
{
    public static class ListExpressions
    {
        public static object? Car(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("List must contain an expression");
            }

            return args[0] switch
            {
                List list => list.Children[0],
                Atom { Value: List atomList } => atomList.Children[0],
                _ => throw new ArgumentException("Argument to car must be a list")
            };
        }

        public static object? Cdr(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("List must contain an expression");
            }

            return args[0] switch
            {
                List list => new Atom(AtomType.List, list.Children.Skip(1).ToListNode()),
                Atom { Value: List atomList } => new Atom(AtomType.List, atomList.Children.Skip(1).ToListNode()),
                _ => throw new ArgumentException("Argument to cdr must be a list")
            };
        }
    }
}