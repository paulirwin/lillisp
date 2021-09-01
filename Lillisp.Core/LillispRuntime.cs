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
            ["quote"] = CoreMacros.Quote,
            ["apply"] = CoreMacros.Apply,
            ["list"] = CoreMacros.List,
            ["if"] = CoreMacros.If,
            ["begin"] = CoreMacros.Begin,
            ["def"] = CoreMacros.Define,
            ["define"] = CoreMacros.Define,
            ["set!"] = CoreMacros.Set,
            ["lambda"] = CoreMacros.Lambda,
            ["defun"] = CoreMacros.Defun,
            ["let"] = CoreMacros.Let,
            ["++!"] = MathMacros.Increment,
            ["--!"] = MathMacros.Decrement,
            ["&&"] = BooleanMacros.And,
            ["and"] = BooleanMacros.And,
            ["||"] = BooleanMacros.Or,
            ["or"] = BooleanMacros.Or,
        };

        private static readonly IReadOnlyDictionary<string, Expression> _systemFunctions = new Dictionary<string, Expression>
        {
            ["+"] = MathExpressions.Plus,
            ["-"] = MathExpressions.Minus,
            ["*"] = MathExpressions.Multiply,
            ["/"] = MathExpressions.Divide,
            ["%"] = MathExpressions.Modulo,
            ["**"] = MathExpressions.Power,
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
            ["count"] = DynamicExpressions.Count,
            ["dec"] = MathExpressions.Decrement,
            ["inc"] = MathExpressions.Increment,
            ["length"] = DynamicExpressions.Count,
            ["ln"] = MathExpressions.Ln,
            ["log"] = MathExpressions.Log,
            ["max"] = MathExpressions.Max,
            ["min"] = MathExpressions.Min,
            ["not"] = BooleanExpressions.Not,
            ["pow"] = MathExpressions.Power,
            ["print"] = StringExpressions.Print,
            ["println"] = StringExpressions.PrintLn,
            ["range"] = ListExpressions.Range,
            ["sqrt"] = MathExpressions.Sqrt,
            ["str"] = StringExpressions.Str,
        };

        private static readonly IReadOnlyDictionary<string, object?> _systemGlobals = new Dictionary<string, object?>
        {
            ["pi"] = Math.PI,
            ["e"] = Math.E,
            ["tau"] = Math.Tau,
            ["true"] = true,
            ["false"] = false,
        };

        private readonly Scope _globalScope;

        public LillispRuntime()
        {
            _globalScope = new Scope();
            _globalScope.AddAllFrom(_systemMacros);
            _globalScope.AddAllFrom(_systemFunctions);
            _globalScope.AddAllFrom(_systemGlobals);
        }

        public void RegisterGlobal(string symbol, object? value)
        {
            _globalScope.Define(symbol, value);
        }

        public void RegisterFunction(string symbol, Expression func)
        {
            _globalScope.Define(symbol, func);
        }

        public object? EvaluateProgram(string program)
        {
            var lexer = new LillispLexer(new AntlrInputStream(program));
            var parser = new LillispParser(new CommonTokenStream(lexer));
            var visitor = new LillispVisitor();

            var prog = visitor.Visit(parser.prog());

            return Evaluate(_globalScope, prog);
        }

        public object? Quote(Node node)
        {
            return node switch
            {
                Program program => program.Children.Select(Quote).ToArray(),
                List list => list.Children.Select(Quote).ToArray(),
                Atom atom => atom.Value,
                Quote quote => quote.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public object? Evaluate(Node node) => Evaluate(_globalScope, node);

        public object? Evaluate(Scope scope, Node node)
        {
            return node switch
            {
                Program program => EvaluateProgram(scope, program),
                List list => EvaluateExpression(scope, list),
                Atom atom => EvaluateAtom(scope, atom),
                Quote quote => EvaluateQuote(quote),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private object? EvaluateQuote(Quote node)
        {
            return node.Value switch
            {
                List list => list.Children.Select(Quote).ToArray(),
                Atom { AtomType: AtomType.Number or AtomType.String, Value: { } value } => value,
                Atom { AtomType: AtomType.Symbol, Value: { } value } => value.ToString(),
                _ => null
            };
        }

        private object? EvaluateAtom(Scope scope, Atom node)
        {
            if (node.AtomType != AtomType.Symbol || node.Value == null)
                return node.Value;

            string? symbol = node.Value.ToString();

            if (symbol is null or "null")
                return null;

            if (symbol == "nil")
                return Nil.Value;

            object? value = scope.Resolve(symbol);

            return value;
        }

        private object? EvaluateExpression(Scope scope, List node)
        {
            if (node.Children.Count == 0)
            {
                return Nil.Value;
            }

            var op = Evaluate(scope, node.Children[0]);

            if (op is MacroExpression macro)
            {
                return macro(this, scope, node.Children.Skip(1).Cast<object>().ToArray());
            }

            var args = node.Children.Skip(1).Select(i => Evaluate(scope, i)).ToArray();

            if (op is Procedure proc)
            {
                return proc.Invoke(this, scope, args);
            }

            if (op is not Expression expr)
            {
                throw new InvalidOperationException($"Invalid operation: {op}");
            }

            return expr(args);
        }

        private object? EvaluateProgram(Scope scope, Program node)
        {
            object? result = null;

            foreach (var child in node.Children)
            {
                result = Evaluate(scope, child);
            }

            return result;
        }
    }
}