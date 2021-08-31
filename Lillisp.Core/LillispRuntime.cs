using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Lillisp.Core.Expressions;
using Lillisp.Core.Syntax;

namespace Lillisp.Core
{
    public class LillispRuntime
    {
        private static readonly IDictionary<string, Expression> _expressions = new Dictionary<string, Expression>()
        {
            ["+"] = MathExpressions.Add,
            ["-"] = MathExpressions.Subtract,
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
            ["car"] = ListExpressions.Car,
            ["cdr"] = ListExpressions.Cdr,
        };

        public object? EvaluateProgram(string program)
        {
            var lexer = new LillispLexer(new AntlrInputStream(program));
            var parser = new LillispParser(new CommonTokenStream(lexer));
            var visitor = new LillispVisitor();

            var prog = visitor.Visit(parser.prog());

            return Evaluate(prog);
        }

        public object? Evaluate(Node node)
        {
            switch (node.Type)
            {
                case NodeType.Program:
                    return EvaluateProgram((ProgramNode)node);
                case NodeType.Expression:
                    return EvaluateExpression((ExpressionNode)node);
                case NodeType.Atom:
                    return EvaluateAtom((AtomNode)node);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object? EvaluateAtom(AtomNode node)
        {
            return node.Value; // TODO: support numbers, variables, etc.
        }

        private object? EvaluateExpression(ExpressionNode node)
        {
            if (node.Children.Count == 0)
            {
                return null; // TODO: empty list instead?
            }

            var op = Evaluate(node.Children[0]);

            if (op is not string sop)
            {
                throw new InvalidOperationException($"Invalid operation format: {op}");
            }

            if (sop == "quote")
            {
                // special form
                return node.Children[1];
            }

            var args = node.Children.Skip(1).Select(Evaluate).ToArray();

            // HACK
            if (!_expressions.TryGetValue(op.ToString()!, out var expr))
            {
                throw new InvalidOperationException($"Unknown operation: {op}");
            }

            return expr(args);
        }

        private object? EvaluateProgram(ProgramNode node)
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
