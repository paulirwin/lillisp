using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Lillisp.Core.Expressions;
using Lillisp.Core.Macros;
using Lillisp.Core.Syntax;

namespace Lillisp.Core
{
    public class LillispRuntime
    {
        private static readonly IReadOnlyDictionary<string, MacroExpression> _systemMacros = new Dictionary<string, MacroExpression>
        {
            ["quote"] = SystemMacros.Quote,
            ["apply"] = SystemMacros.Apply,
            ["list"] = SystemMacros.List,
        };

        private static readonly IReadOnlyDictionary<string, Expression> _systemFunctions = new Dictionary<string, Expression>
        {
            ["+"] = MathExpressions.Plus,
            ["-"] = MathExpressions.Minus,
            ["*"] = MathExpressions.Multiply,
            ["/"] = MathExpressions.Divide,
            ["%"] = MathExpressions.Modulo,
            ["^"] = MathExpressions.Power,
            [">"] = BooleanExpressions.GreaterThan,
            [">="] = BooleanExpressions.GreaterThanOrEqual,
            ["<"] = BooleanExpressions.LessThan,
            ["<="] = BooleanExpressions.LessThanOrEqual,
            ["="] = BooleanExpressions.Equal,
            ["=="] = BooleanExpressions.Equal,
            [">>"] = MathExpressions.ShiftRight,
            ["<<"] = MathExpressions.ShiftLeft,
            ["abs"] = MathExpressions.Abs,
            ["append"] = ListExpressions.Append,
            ["car"] = ListExpressions.Car,
            ["cdr"] = ListExpressions.Cdr,
            ["cons"] = ListExpressions.Cons,
            ["max"] = MathExpressions.Max,
            ["min"] = MathExpressions.Min,
            ["sqrt"] = MathExpressions.Sqrt,
        };

        private static readonly IReadOnlyDictionary<string, object?> _systemGlobals = new Dictionary<string, object?>
        {
            ["pi"] = Math.PI,
            ["e"] = Math.E,
            ["tau"] = Math.Tau,
            ["true"] = true,
            ["false"] = false,
        };

        public object? EvaluateProgram(string program)
        {
            var lexer = new LillispLexer(new AntlrInputStream(program));
            var parser = new LillispParser(new CommonTokenStream(lexer));
            var visitor = new LillispVisitor();

            var prog = visitor.Visit(parser.prog());

            return Evaluate(prog);
        }

        public object? Quote(Node node)
        {
            switch (node.Type)
            {
                case NodeType.Program:
                    return ((Program) node).Children.Select(Quote).ToArray();
                case NodeType.List:
                    return ((List) node).Children.Select(Quote).ToArray();
                case NodeType.Atom:
                    return ((Atom) node).Value;
                case NodeType.Quote:
                    return Quote(((Quote) node).Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object? Evaluate(Node node)
        {
            switch (node.Type)
            {
                case NodeType.Program:
                    return EvaluateProgram((Program) node);
                case NodeType.List:
                    return EvaluateExpression((List) node);
                case NodeType.Atom:
                    return EvaluateAtom((Atom) node);
                case NodeType.Quote:
                    return EvaluateQuote((Quote) node);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object? EvaluateQuote(Quote node)
        {
            return node.Value switch
            {
                List list => list.Children.Select(Quote).ToArray(),
                Atom { AtomType: AtomType.Number, Value: { } value } => value,
                Atom { AtomType: AtomType.Symbol, Value: { } value } => value.ToString(),
                _ => null
            };
        }

        private object? EvaluateAtom(Atom node)
        {
            if (node.AtomType != AtomType.Symbol || node.Value == null)
                return node.Value;

            string? symbol = node.Value.ToString();

            if (symbol == null)
                return null;
            
            if (_systemGlobals.TryGetValue(symbol, out var global))
                return global;

            if (_systemMacros.TryGetValue(symbol, out var macro))
                return macro;

            if (_systemFunctions.TryGetValue(symbol, out var fn))
                return fn;

            if (symbol == "nil")
                return Array.Empty<object>();

            return symbol;
        }

        private object? EvaluateExpression(List node)
        {
            if (node.Children.Count == 0)
            {
                return Array.Empty<object>();
            }

            var op = Evaluate(node.Children[0]);

            if (op is MacroExpression macro)
            {
                return macro(this, node.Children.Skip(1).Cast<object>().ToArray());
            }

            if (op is not Expression expr)
            {
                throw new InvalidOperationException($"Invalid operation format: {op}");
            }

            var args = node.Children.Skip(1).Select(Evaluate).ToArray();

            return expr(args);
        }

        private object? EvaluateProgram(Program node)
        {
            object? result = null;

            foreach (var child in node.Children)
            {
                result = Evaluate(child);
            }

            return result;
        }
    }
}