﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lillisp.Core.Syntax;

namespace Lillisp.Core.Macros
{
    public static class CoreMacros
    {
        public static object? Quote(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0 || args[0] is not Node node)
            {
                throw new InvalidOperationException("quote requires an argument");
            }

            return runtime.Quote(node);
        }

        public static object? Apply(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 2 || args[0] is not Node source || args[1] is not Node target)
            {
                throw new InvalidOperationException("apply requires a fn and a list as arguments");
            }

            var sourceValue = runtime.Evaluate(scope, source);

            if (sourceValue is not Expression expr)
            {
                throw new InvalidOperationException("First parameter to `apply` must evaluate to a fn");
            }

            var list = runtime.Evaluate(scope, target);

            if (list is not object[] objArray)
            {
                throw new InvalidOperationException("Second parameter to `apply` must evaluate to a list");
            }

            return expr(objArray);
        }

        public static object? List(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                return Array.Empty<object>();
            }

            return args.Cast<Node>().Select(runtime.Quote).ToArray();
        }

        public static object? If(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length is < 2 or > 3 || args[0] is not Node cond || args[1] is not Node consequence)
            {
                throw new ArgumentException("if requires 2 or 3 arguments");
            }

            Node? alt = null;

            if (args.Length == 3)
            {
                if (args[2] is not Node altNode)
                {
                    throw new ArgumentException("argument 3 must be a node");
                }

                alt = altNode;
            }

            var result = runtime.Evaluate(scope, cond);

            if (result.IsTruthy())
            {
                return runtime.Evaluate(scope, consequence);
            }

            return alt != null ? runtime.Evaluate(scope, alt) : Nil.Value;
        }

        public static object? Begin(LillispRuntime runtime, Scope scope, object?[] args)
        {
            object? result = null;

            foreach (var arg in args)
            {
                if (arg is not Node node)
                {
                    throw new ArgumentException("invalid node");
                }

                result = runtime.Evaluate(scope, node);
            }

            return result;
        }

        public static object? Define(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("define requires two arguments");
            }

            if (args[0] is not Atom {AtomType: AtomType.Symbol, Value: string symbol} atom)
            {
                throw new ArgumentException("define's first argument must be a symbol");
            }

            if (args[1] is not Node node)
            {
                throw new ArgumentException("define's second argument must be a node");
            }

            object? value = runtime.Evaluate(scope, node);

            scope.Define(symbol, value);
            
            return atom;
        }

        public static object? Set(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("set! requires two arguments");
            }

            if (args[0] is not Atom {AtomType: AtomType.Symbol, Value: string symbol} atom)
            {
                throw new ArgumentException("set!'s first argument must be a symbol");
            }

            if (args[1] is not Node node)
            {
                throw new ArgumentException("set!'s second argument must be a node");
            }

            object? value = runtime.Evaluate(scope, node);

            scope.Set(symbol, value);

            return atom;
        }


        public static object? Lambda(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("lambda requires two arguments");
            }
            
            if (args[0] is not Node parameters)
            {
                throw new ArgumentException("lambda's first argument must be a list of symbols");
            }

            if (args[1] is not Node body)
            {
                throw new ArgumentException("lambda's second argument must be a node");
            }

            return CreateProcedure(parameters, body);
        }

        private static Procedure CreateProcedure(Node parameters, Node body)
        {
            string text = $"(lambda {parameters} {body})"; // TODO: get access to actual AST node here
            
            return new Procedure(text, parameters, body);
        }

        public static object? Defun(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("defun requires three arguments");
            }

            if (args[0] is not Atom { AtomType: AtomType.Symbol, Value: string symbol } atom)
            {
                throw new ArgumentException("defun's first argument must be a symbol");
            }

            if (args[1] is not List parameters || !parameters.Children.All(i => i is Atom { AtomType: AtomType.Symbol }))
            {
                throw new ArgumentException("defun's first argument must be a list of symbols");
            }

            if (args[2] is not Node body)
            {
                throw new ArgumentException("defun's second argument must be a node");
            }

            var procedure = CreateProcedure(parameters, body);

            scope.Define(symbol, procedure);
            
            return atom;
        }

        public static object? Let(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("let requires at least one argument");
            }

            if (args[0] is not List bindings)
            {
                throw new ArgumentException("let's first parameter must be a list");
            }

            var childScope = scope.CreateChildScope();
            var evaluatedSymbols = new HashSet<string>();

            foreach (var binding in bindings.Children)
            {
                if (binding is Atom {AtomType: AtomType.Symbol, Value: string symbol})
                {
                    if (evaluatedSymbols.Contains(symbol))
                    {
                        throw new ArgumentException($"Variable {symbol} has already been defined in this scope");
                    }

                    childScope[symbol] = Nil.Value;
                    evaluatedSymbols.Add(symbol);
                }
                else if (binding is List {Children: {Count: 2}} list 
                         && list.Children[0] is Atom { AtomType: AtomType.Symbol, Value: string listSymbol })
                {
                    if (evaluatedSymbols.Contains(listSymbol))
                    {
                        throw new ArgumentException($"Variable {listSymbol} has already been defined in this scope");
                    }

                    childScope[listSymbol] = runtime.Evaluate(childScope, list.Children[1]);
                    evaluatedSymbols.Add(listSymbol);
                }
                else
                {
                    throw new ArgumentException($"Unknown binding format: {binding}");
                }
            }

            object? result = Nil.Value;

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] is Node node)
                {
                    result = runtime.Evaluate(childScope, node);
                }
                else
                {
                    result = args[i]; // should not happen?
                }
            }

            return result;
        }

        public static object? Map(LillispRuntime runtime, Scope scope, object?[] args)
        {
            // TODO: support returning transducer fn if one arg, or multiple lists if > 2
            if (args.Length < 2 || args[0] is not Node source || args[1] is not Node target)
            {
                throw new InvalidOperationException("map requires a fn and a list as arguments");
            }

            var sourceValue = runtime.Evaluate(scope, source);

            if (sourceValue is not Expression expr)
            {
                throw new InvalidOperationException("First parameter to `map` must evaluate to a fn");
            }

            var list = runtime.Evaluate(scope, target);

            if (list is not object[] objArray)
            {
                throw new InvalidOperationException("Second parameter to `map` must evaluate to a list");
            }

            var result = new object?[objArray.Length];

            for (int i = 0; i < objArray.Length; i++)
            {
                result[i] = expr(new[] {objArray[i]});
            }

            return result;
        }

        public static object? Cond(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0 || !args.All(i => i is List { Children: { Count: > 1 }}))
            {
                throw new ArgumentException("cond requires at least one clause argument");
            }

            var clauses = args.Cast<List>().ToArray();
            List? elseClause = null;

            if (clauses[^1].Children[0] is Atom {AtomType: AtomType.Symbol, Value: "else"})
            {
                elseClause = clauses[^1];
                clauses = clauses[..^1];
            }

            foreach (var clause in clauses)
            {
                var test = runtime.Evaluate(clause.Children[0]);

                if (test.IsTruthy())
                {
                    object? result = Nil.Value;

                    foreach (var expr in clause.Children.Skip(1))
                    {
                        result = runtime.Evaluate(expr);
                    }

                    return result;
                }
            }

            if (elseClause != null)
            {
                object? result = Nil.Value;

                foreach (var expr in elseClause.Children.Skip(1))
                {
                    result = runtime.Evaluate(expr);
                }

                return result;
            }

            throw new InvalidOperationException("No clause matched for the cond expression");
        }
    }
}