using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            ["++!"] = MathMacros.Increment,
            ["--!"] = MathMacros.Decrement,
            ["&&"] = BooleanMacros.And,
            ["||"] = BooleanMacros.Or,
            ["and"] = BooleanMacros.And,
            ["apply"] = CoreMacros.Apply,
            ["begin"] = CoreMacros.Begin,
            ["cond"] = CoreMacros.Cond,
            ["def"] = CoreMacros.Define,
            ["define"] = CoreMacros.Define,
            ["defun"] = CoreMacros.Defun,
            ["if"] = CoreMacros.If,
            //["include"] = CoreMacros.Include, // TODO
            //["include-ci"] = CoreMacros.Include, // TODO
            ["lambda"] = CoreMacros.Lambda,
            ["let"] = CoreMacros.Let,
            ["list"] = CoreMacros.List,
            ["map"] = CoreMacros.Map,
            ["new"] = InteropMacros.New,
            ["or"] = BooleanMacros.Or,
            ["quote"] = CoreMacros.Quote,
            ["set!"] = CoreMacros.Set,
            ["use"] = InteropMacros.Use,
            ["when"] = BooleanMacros.When,
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
            ["boolean?"] = TypeExpressions.IsBoolean,
            //["bytevector?"] = TypeExpressions.IsByteVector, // TODO: bytevector support
            ["car"] = ListExpressions.Car,
            ["cast"] = TypeExpressions.Cast,
            ["cdr"] = ListExpressions.Cdr,
            ["char?"] = TypeExpressions.IsChar,
            ["cons"] = ListExpressions.Cons,
            ["count"] = DynamicExpressions.Count,
            ["dec"] = MathExpressions.Decrement,
            //["eof-object?"] = TypeExpressions.IsEofObject, // TODO
            ["get"] = DynamicExpressions.Get,
            ["inc"] = MathExpressions.Increment,
            ["length"] = DynamicExpressions.Count,
            ["ln"] = MathExpressions.Ln,
            ["log"] = MathExpressions.Log,
            ["max"] = MathExpressions.Max,
            ["min"] = MathExpressions.Min,
            ["not"] = BooleanExpressions.Not,
            ["null?"] = TypeExpressions.IsNull,
            ["number?"] = TypeExpressions.IsNumber,
            ["pair?"] = TypeExpressions.IsPair,
            //["port?"] = TypeExpressions.IsPort, // TODO
            ["pow"] = MathExpressions.Power,
            ["procedure?"] = TypeExpressions.IsProcedure,
            ["print"] = StringExpressions.Print,
            ["println"] = StringExpressions.PrintLn,
            ["pr"] = StringExpressions.Pr,
            ["prn"] = StringExpressions.Prn,
            ["range"] = ListExpressions.Range,
            ["sqrt"] = MathExpressions.Sqrt,
            ["str"] = StringExpressions.Str,
            ["string?"] = TypeExpressions.IsString,
            ["symbol?"] = TypeExpressions.IsSymbol,
            ["typeof"] = TypeExpressions.TypeOf,
            //["vector?"] = TypeExpressions.IsVector, // TODO
        };

        private static readonly IReadOnlyDictionary<string, object?> _systemGlobals = new Dictionary<string, object?>
        {
            ["pi"] = Math.PI,
            ["e"] = Math.E,
            ["tau"] = Math.Tau,
            ["#t"] = true,
            ["#f"] = false,
            ["#true"] = true,
            ["#false"] = false,
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
                Pair pair => pair.Select(Quote).ToArray(),
                Symbol symbol => symbol.Value,
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
                Pair pair => EvaluateExpression(scope, pair),
                Symbol symbol => EvaluateSymbol(scope, symbol),
                Atom atom => atom.Value,
                Quote quote => EvaluateQuote(quote),
                Nil nil => nil,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private object? EvaluateQuote(Quote node)
        {
            return node.Value switch
            {
                Pair pair => pair.Select(Quote).ToArray(),
                Atom { Value: { } value } => value,
                Symbol symbol => symbol,
                Nil nil => nil,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static object? EvaluateSymbol(Scope scope, Symbol node)
        {
            string? symbol = node.Value;

            if (symbol is null or "null")
                return null;

            if (symbol == "nil")
                return Nil.Value;

            object? value = scope.Resolve(symbol);

            if (value != null)
                return value;

            value = Interop.ResolveSymbol(scope, symbol);

            if (value != null)
                return value;

            throw new ArgumentException($"Unable to resolve symbol {symbol}");
        }

        private object? EvaluateExpression(Scope scope, Pair pair)
        {
            if (pair.Car is Nil)
            {
                return Nil.Value;
            }

            if (pair.Car is Symbol symbol && symbol.Value.StartsWith('.'))
            {
                var memberArgs = pair.Skip(1).Select(i => Evaluate(scope, i)).ToArray();

                return Interop.InvokeMember(this, scope, symbol.Value, memberArgs);
            }

            var op = Evaluate(scope, pair.Car);

            if (op is MacroExpression macro)
            {
                return macro(this, scope, pair.Skip(1).Cast<object>().ToArray());
            }

            var args = pair.Skip(1).Select(i => Evaluate(scope, i)).ToArray();

            if (op is Procedure proc)
            {
                return proc.Invoke(this, scope, args);
            }

            if (op is MethodInfo method)
            {
                // HACK: only static methods for now
                return method.Invoke(null, args);
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