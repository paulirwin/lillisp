using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Rationals;

namespace Lillisp.Core
{
    public class LillispVisitor : LillispBaseVisitor<Node>
    {
        private static readonly Regex _complexRegex = new("(?<real>\\-?[0-9]+(\\.[0-9]+)?)(?<imaginary>[\\-\\+][0-9]+(\\.[0-9]+)?)i", RegexOptions.Compiled);

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

        public override Node VisitBytevector(LillispParser.BytevectorContext context)
        {
            var bv = new Bytevector();

            foreach (var child in context.children)
            {
                var childNode = Visit(child);

                if (childNode == null)
                {
                    continue;
                }

                if (childNode is Atom { AtomType: AtomType.Number, Value: >= 0 and <= 255 } atom)
                {
                    bv.Add((byte)(int)atom.Value);
                }
                else
                {
                    throw new InvalidOperationException("Only integer literals between 0-255 are supported for bytevector literal values.");
                }
            }

            return bv;
        }

        public override Node VisitInteger(LillispParser.IntegerContext context)
        {
            var integer = context.INTEGER();

            if (integer != null)
            {
                var i = int.Parse(integer.GetText());
                return new Atom(AtomType.Number, i);
            }

            throw new NotImplementedException("Unknown integer type");
        }

        public override Node VisitAtom(LillispParser.AtomContext context)
        {
            var number = context.number();

            if (number != null)
            {
                var complex = number.COMPLEX();

                if (complex != null)
                {
                    return ParseComplex(complex.GetText());
                }

                var ratio = number.RATIO();

                if (ratio != null)
                {
                    return ParseRational(ratio.GetText());
                }

                var posInfinity = number.POS_INFINITY();

                if (posInfinity != null)
                {
                    return new Atom(AtomType.Number, double.PositiveInfinity);
                }

                var negInfinity = number.NEG_INFINITY();

                if (negInfinity != null)
                {
                    return new Atom(AtomType.Number, double.NegativeInfinity);
                }

                var nan = number.NAN();

                if (nan != null)
                {
                    return new Atom(AtomType.Number, double.NaN);
                }

                var floatingPoint = number.FLOAT();

                if (floatingPoint != null)
                {
                    double num = Convert.ToDouble(number.GetText());
                    return new Atom(AtomType.Number, num);
                }

                var integer = number.INTEGER();

                if (integer != null)
                {
                    int num = Convert.ToInt32(integer.GetText());
                    return new Atom(AtomType.Number, num);
                }

                throw new NotImplementedException("Unknown number type");
            }

            var str = context.STRING();

            if (str != null)
            {
                var strText = str.GetText()[1..^1]; // exclude start and end quotes
                var unescapedStr = UnescapeString(strText);
                return new Atom(AtomType.String, unescapedStr);
            }

            var symbol = context.symbol();

            if (symbol != null)
            {
                var escapedSymbol = symbol.ESCAPED_IDENTIFIER();

                if (escapedSymbol != null)
                {
                    var escapedText = symbol.GetText()[1..^1]; // exclude start and end bars
                    var unescapedText = UnescapeString(escapedText);
                    return new Symbol(unescapedText, escaped: true);
                }
                else
                {
                    var symbolText = symbol.GetText();

                    return symbolText switch
                    {
                        "#t" or "#true" or "true" => new Atom(AtomType.Boolean, true),
                        "#f" or "#false" or "false" => new Atom(AtomType.Boolean, false),
                        _ => new Symbol(symbolText)
                    };
                }
            }

            var character = context.CHARACTER();

            if (character != null)
            {
                var charText = character.GetText()[2..];
                char c = charText.ToLowerInvariant() switch
                {
                    "" => ' ',
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

        private static Node ParseRational(string text)
        {
            var rational = Rational.Parse(text);

            return new Atom(AtomType.Number, rational);
        }

        private static Node ParseComplex(string text)
        {
            var match = _complexRegex.Match(text);

            if (!match.Success)
            {
                throw new InvalidOperationException($"Unable to parse complex number: {text}");
            }

            var real = double.Parse(match.Groups["real"].Value);
            var imaginary = double.Parse(match.Groups["imaginary"].Value);

            return new Atom(AtomType.Number, new Complex(real, imaginary));
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

            var commentDatum = context.comment_datum();

            if (commentDatum != null)
            {
                return null!;
            }

            throw new NotImplementedException("Unknown macro type");
        }

        private static string UnescapeString(string input)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c != '\\')
                {
                    sb.Append(c);
                }
                else if (i < input.Length - 1)
                {
                    char n = input[i + 1];
                    char? u = n switch
                    {
                        'a' => '\a',
                        'b' => '\b',
                        'f' => '\f',
                        'n' => '\n',
                        'r' => '\r',
                        't' => '\t',
                        'v' => '\v',
                        '0' => '\0',
                        '\\' => '\\',
                        '\"' => '\"',
                        '|' => '|',
                        _ => null
                    };

                    if (u != null)
                    {
                        sb.Append(u);
                        i++;
                        continue;
                    }

                    if (n == 'u')
                    {
                        if ((input.Length - 2 - i) < 4)
                        {
                            throw new InvalidOperationException("Not enough characters left in the string for a 16-bit unicode escape");
                        }

                        ushort val = ushort.Parse(input.Substring(i + 2, 4), NumberStyles.HexNumber);
                        sb.Append((char)val);
                        i += 5;
                    }
                    else if (n == 'x')
                    {
                        int scPos = input.IndexOf(';', i + 1);

                        if (scPos < 0)
                        {
                            throw new InvalidOperationException("Unicode escape sequences starting with \\x must end in a semicolon.");
                        }

                        string seq = input.Substring(i + 2, scPos - i - 2);
                        ushort val = ushort.Parse(seq, NumberStyles.HexNumber);
                        sb.Append((char)val);
                        i += 2 + seq.Length;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown string escape sequence: \\{n}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
