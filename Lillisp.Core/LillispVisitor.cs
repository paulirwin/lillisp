using System;
using System.Collections.Generic;
using Lillisp.Core.Syntax;

namespace Lillisp.Core
{
    public class LillispVisitor : LillispBaseVisitor<Node>
    {
        public override Node VisitProg(LillispParser.ProgContext context)
        {
            var node = new Program();

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

        public override Node VisitList(LillispParser.ListContext context)
        {
            var nodes = new List<Node>();

            foreach (var child in context.children)
            {
                var childNode = Visit(child);

                if (childNode != null)
                {
                    nodes.Add(childNode);
                }
            }

            return List.FromNodes(nodes);
        }

        public override Node VisitAtom(LillispParser.AtomContext context)
        {
            var number = context.NUMBER();

            if (number != null)
            {
                double num = Convert.ToDouble(number.GetText());
                return new Atom(AtomType.Number, num);
            }

            var str = context.STRING();

            if (str != null)
            {
                string strValue = str.GetText().Replace("\\\"", "\"")[1..^1];
                return new Atom(AtomType.String, strValue);
            }

            var symbol = context.SYMBOL();

            if (symbol != null)
            {
                var symbolText = symbol.GetText();
                return new Atom(AtomType.Symbol, symbolText == "null" ? null : symbolText);
            }

            throw new NotImplementedException("Unknown atom type");
        }

        public override Node VisitMacro(LillispParser.MacroContext context)
        {
            var quote = context.quote();

            if (quote != null)
            {
                var child = quote.children[1];

                var node = Visit(child);

                return new Quote(node);
            }

            throw new NotImplementedException("Unknown macro type");
        }
    }
}
