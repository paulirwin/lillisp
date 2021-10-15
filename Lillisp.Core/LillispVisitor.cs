using System;
using System.Collections.Generic;
using System.Globalization;
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

        public override Node VisitVector(LillispParser.VectorContext context)
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

            return new Vector(nodes);
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
                return new Symbol(symbolText);
            }

            var character = context.CHARACTER();

            if (character != null)
            {
                var charText = character.GetText()[2..];
                char c = charText.ToLowerInvariant() switch
                {
                    "alarm" => '\u0007',
                    "backspace" => '\u0008',
                    "delete" => '\u007f',
                    "escape" => '\u001b',
                    "newline" => '\n',
                    "null" => '\0',
                    "return" => '\r',
                    "space" => ' ',
                    "tab" => '\t',
                    _ => charText[0]
                };

                if (c == 'x' && charText.Length > 1)
                {
                    var hex = charText[1..];

                    if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hexCode))
                    {
                        throw new ArgumentException($"Invalid hex character escape: #\\{charText}");
                    }

                    c = (char)hexCode;
                }

                return new Atom(AtomType.Character, c);
            }

            throw new NotImplementedException("Unknown atom type");
        }

        public override Node VisitMeta(LillispParser.MetaContext context)
        {
            var quote = context.quote();

            if (quote != null)
            {
                var child = quote.children[1];

                var node = Visit(child);

                return new Quote(node);
            }

            var quasiquote = context.quasiquote();

            if (quasiquote != null)
            {
                var child = quasiquote.children[1];

                var node = Visit(child);

                return new Quasiquote(node);
            }

            var unquote = context.unquote();

            if (unquote != null)
            {
                var child = unquote.children[1];

                var node = Visit(child);

                return new Unquote(node);
            }

            throw new NotImplementedException("Unknown macro type");
        }
    }
}
