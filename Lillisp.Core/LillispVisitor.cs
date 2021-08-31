using System;
using Lillisp.Core.Syntax;

namespace Lillisp.Core
{
    public class LillispVisitor : LillispBaseVisitor<Node>
    {
        public override Node VisitProg(LillispParser.ProgContext context)
        {
            var node = new ProgramNode();

            foreach (var child in context.children)
            {
                var childNode = Visit(child);

                if (childNode != null)
                {
                    node.Children.Add(childNode);
                }
            }

            return node;
        }

        public override Node VisitExpr(LillispParser.ExprContext context)
        {
            var node = new ExpressionNode();

            foreach (var child in context.children)
            {
                var childNode = Visit(child);

                if (childNode != null)
                {
                    node.Children.Add(childNode);
                }
            }

            return node;
        }

        public override Node VisitAtom(LillispParser.AtomContext context)
        {
            var symbol = context.OPERATOR();

            if (symbol != null)
            {
                return new AtomNode(AtomType.Operator, symbol.GetText());
            }

            var number = context.NUMBER();

            if (number != null)
            {
                double num = Convert.ToDouble(number.GetText());
                return new AtomNode(AtomType.Number, num);
            }

            var ident = context.IDENTIFIER();

            if (ident != null)
            {
                return new AtomNode(AtomType.Identifier, ident.GetText());
            }

            throw new NotImplementedException("Unknown atom type");
        }
    }
}
