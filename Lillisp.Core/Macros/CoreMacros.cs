using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Rationals;

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

            return runtime.Evaluate(scope, new Quote(node));
        }

        public static object? Quasiquote(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0 || args[0] is not Node node)
            {
                throw new InvalidOperationException("quasiquote requires an argument");
            }

            return runtime.Evaluate(scope, new Quasiquote(node));
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

            if (list is not IEnumerable<object> objArray)
            {
                throw new InvalidOperationException("Second parameter to `apply` must evaluate to a list");
            }

            return expr(objArray.ToArray());
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
                return consequence is Pair pair ? LillispRuntime.TailCall(scope, pair) : runtime.Evaluate(scope, consequence);
            }

            return alt != null ? (alt is Pair altPair ? global::Lillisp.Core.LillispRuntime.TailCall(scope, altPair) : runtime.Evaluate(scope, alt)) : Nil.Value;
        }

        public static object? DefineRecordType(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("define-record-type requires at least 3 arguments");
            }

            if (args[0] is not Symbol name)
            {
                throw new ArgumentException("define-record-type's first argument must be a symbol");
            }

            if (args[1] is not Pair { IsList: true, Car: Symbol ctorName } ctor)
            {
                throw new ArgumentException("define-record-type's second argument must be a constructor list");
            }

            if (args[2] is not Symbol predName)
            {
                throw new ArgumentException("define-record-type's third argument must be a predicate name symbol");
            }

            var recordType = new RecordTypeDefinition(name, ctorName, predName);

            for (int i = 3; i < args.Length; i++)
            {
                DefineRecordField(args[i], recordType);
            }

            var ctorArgs = ctor.Cast<Symbol>().Skip(1).ToList();

            foreach (var ctorArg in ctorArgs)
            {
                if (!recordType.Fields.Any(i => i.Name.Equals(ctorArg)))
                {
                    throw new ArgumentException($"Constructor definition refers to field {ctorArg} that is not defined");
                }

                recordType.ConstructorParameters.Add(ctorArg);
            }

            var ctorFunc = BuildRecordConstructor(recordType);
            scope.Define(ctorName.Value, ctorFunc);

            var predFunc = BuildRecordPredicate(recordType);
            scope.Define(predName.Value, predFunc);

            foreach (var field in recordType.Fields)
            {
                var accessorFunc = BuildRecordAccessor(field);
                scope.Define(field.Accessor.Value, accessorFunc);

                if (field.Modifier != null)
                {
                    var modifierFunc = BuildRecordModifier(field);
                    scope.Define(field.Modifier.Value, modifierFunc);
                }
            }

            scope.DefineRecordType(recordType);

            return Nil.Value;
        }

        private static Expression BuildRecordModifier(RecordFieldDefinition field)
        {
            if (field.Modifier == null)
            {
                throw new ArgumentException("Field does not have a modifier specified");
            }

            return args =>
            {
                if (args.Length != 2)
                {
                    throw new ArgumentException($"Record field modifier {field.Modifier} requires two arguments");
                }

                if (args[0] is not RecordInstance instance || instance.RecordType != field.RecordType)
                {
                    throw new ArgumentException($"Value is not an instance of record type {field.RecordType.Name}");
                }

                instance.Fields[field.Name] = args[1];

                return Nil.Value;
            };
        }

        private static Expression BuildRecordAccessor(RecordFieldDefinition field)
        {
            return args =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException($"Record field accessor {field.Accessor} requires one argument");
                }

                if (args[0] is not RecordInstance instance || instance.RecordType != field.RecordType)
                {
                    throw new ArgumentException($"Value is not an instance of record type {field.RecordType.Name}");
                }

                return instance.Fields[field.Name];
            };
        }

        private static Expression BuildRecordPredicate(RecordTypeDefinition recordType)
        {
            return args =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException($"Record type predicate {recordType.PredicateName} requires one argument");
                }

                return args[0] is RecordInstance instance && instance.RecordType == recordType;
            };
        }

        private static Expression BuildRecordConstructor(RecordTypeDefinition recordType)
        {
            return args =>
            {
                if (args.Length != recordType.ConstructorParameters.Count)
                {
                    throw new ArgumentException($"Record type constructor {recordType.ConstructorName} requires {recordType.ConstructorParameters.Count} argument{(recordType.ConstructorParameters.Count == 1 ? "" : "s")}");
                }

                var instance = new RecordInstance(recordType);

                for (int i = 0; i < recordType.ConstructorParameters.Count; i++)
                {
                    var fieldName = recordType.ConstructorParameters[i];

                    instance.Fields[fieldName] = args[i];
                }

                return instance;
            };
        }

        private static void DefineRecordField(object? arg, RecordTypeDefinition recordType)
        {
            if (arg is not Pair { IsList: true, Car: Symbol fieldName } fieldSpec)
            {
                throw new ArgumentException("Field definitions for record types must be a list");
            }

            var fieldSpecList = fieldSpec.ToList();

            if (fieldSpecList.Count is < 2 or > 3)
            {
                throw new ArgumentException("Field definitions must contain either an accessor only or an accessor and a modifier");
            }

            if (fieldSpecList[1] is not Symbol accessor)
            {
                throw new ArgumentException("Field definition accessor must be a symbol");
            }

            Symbol? modifier = null;

            if (fieldSpecList.Count == 3)
            {
                if (fieldSpecList[2] is not Symbol modifierSym)
                {
                    throw new ArgumentException("Field definition modifier must be a symbol");
                }

                modifier = modifierSym;
            }

            var fieldDefinition = new RecordFieldDefinition(recordType, fieldName, accessor, modifier);
            recordType.Fields.Add(fieldDefinition);
        }

        public static object? Begin(LillispRuntime runtime, Scope scope, object?[] args)
        {
            object? result = null;

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg is not Node node)
                {
                    throw new ArgumentException("invalid node");
                }

                result = (i == args.Length - 1 && arg is Pair pair) ? LillispRuntime.TailCall(scope, pair) : runtime.Evaluate(scope, node);
            }

            return result;
        }

        public static object? Define(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("define requires two arguments");
            }
            
            if (args[0] is Symbol symbol)
            {
                object? value = runtime.Evaluate(scope, args[1]);

                scope.Define(symbol.Value, value);

                return symbol;
            }

            if (args[0] is Pair { IsList: true } list)
            {
                if (list.Car is not Symbol ps)
                {
                    throw new ArgumentException("The first item of define's first argument must be a symbol");
                }

                var lambdaArgs = list.Cdr is Pair cdrp
                    ? cdrp
                    : List.FromNodes(new [] { list.Cdr });

                var lambda = Lambda(runtime, scope, new object?[] { lambdaArgs, args[1] });

                scope.Define(ps.Value, lambda);

                return ps;
            }
            else if (args[0] is Pair pair)
            {
                if (pair.Car is not Symbol ps)
                {
                    throw new ArgumentException("The first item of define's first argument must be a symbol");
                }

                var lambda = Lambda(runtime, scope, new object?[] { pair.Cdr, args[1] });

                scope.Define(ps.Value, lambda);

                return ps;
            }

            throw new ArgumentException("define's first argument must be a symbol, a pair, or a list");
        }

        public static object? Set(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("set! requires two arguments");
            }

            if (args[0] is not Symbol symbol)
            {
                throw new ArgumentException("set!'s first argument must be a symbol");
            }

            if (args[1] is not Node node)
            {
                throw new ArgumentException("set!'s second argument must be a node");
            }

            object? value = runtime.Evaluate(scope, node);

            scope.Set(symbol.Value, value);

            return symbol;
        }

        public static object? Lambda(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("lambda requires at least two arguments");
            }
            
            if (args[0] is not Node parameters)
            {
                throw new ArgumentException("lambda's first argument must be a list of symbols");
            }
            
            return CreateProcedure(parameters, args.Skip(1).Cast<Node>().ToArray());
        }

        private static Procedure CreateProcedure(Node parameters, Node[] body)
        {
            string text = $"(lambda {parameters} {string.Join(' ', body.Select(OutputFormatter.FormatPr))})";
            
            return new Procedure(text, parameters, body);
        }

        public static object? Defun(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("defun requires at least three arguments");
            }

            if (args[0] is not Symbol symbol)
            {
                throw new ArgumentException("defun's first argument must be a symbol");
            }

            if (args[1] is not Pair parameters || !parameters.All(i => i is Symbol))
            {
                throw new ArgumentException("defun's first argument must be a list of symbols");
            }
            
            var procedure = CreateProcedure(parameters, args.Skip(2).Cast<Node>().ToArray());

            scope.Define(symbol.Value, procedure);
            
            return symbol;
        }

        public static object? Let(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("let requires at least one argument");
            }

            if (args[0] is not Pair bindings)
            {
                throw new ArgumentException("let's first parameter must be a list");
            }

            var childScope = scope.CreateChildScope();
            var evaluatedSymbols = new HashSet<string>();

            foreach (var binding in bindings)
            {
                if (binding is Symbol symbol)
                {
                    if (evaluatedSymbols.Contains(symbol.Value))
                    {
                        throw new ArgumentException($"Variable {symbol} has already been defined in this scope");
                    }

                    childScope.Define(symbol.Value, Nil.Value);
                    evaluatedSymbols.Add(symbol.Value);
                }
                else if (binding is Pair { IsList: true, Car: Symbol listSymbol} list)
                {
                    if (evaluatedSymbols.Contains(listSymbol.Value))
                    {
                        throw new ArgumentException($"Variable {listSymbol} has already been defined in this scope");
                    }

                    var bindingValue = list.Cdr;

                    if (bindingValue is Pair { IsList: true } bindingValuePair)
                    {
                        bindingValue = bindingValuePair.Car;
                    }

                    childScope.Define(listSymbol.Value, runtime.Evaluate(childScope, bindingValue));
                    evaluatedSymbols.Add(listSymbol.Value);
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
                    result = (i == args.Length - 1 && node is Pair pair) ? LillispRuntime.TailCall(childScope, pair) : runtime.Evaluate(childScope, node);
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

            if (list is not IEnumerable<object> objEnum)
            {
                throw new InvalidOperationException("Second parameter to `map` must evaluate to a list");
            }

            var objArray = objEnum.ToArray();

            var result = new object?[objArray.Length];

            for (int i = 0; i < objArray.Length; i++)
            {
                result[i] = expr(new[] {objArray[i]});
            }

            return result;
        }

        public static object? Cond(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0 || !args.All(i => i is Pair { IsList: true }))
            {
                throw new ArgumentException("cond requires at least one clause argument");
            }

            var clauses = args.Cast<Pair>().ToArray();
            Pair? elseClause = null;

            if (clauses[^1].Car is Symbol { Value: "else" })
            {
                elseClause = clauses[^1];
                clauses = clauses[..^1];
            }

            foreach (var clause in clauses)
            {
                var test = runtime.Evaluate(scope, clause.Car);

                if (test.IsTruthy())
                {
                    object? result = Nil.Value;

                    var clauseForms = clause.ToList();

                    for (int i = 1; i < clauseForms.Count; i++)
                    {
                        var expr = clauseForms[i];

                        result = (i == clauseForms.Count - 1 && expr is Pair pair) ? LillispRuntime.TailCall(scope, pair) : runtime.Evaluate(scope, expr);
                    }

                    return result;
                }
            }

            if (elseClause != null)
            {
                object? result = Nil.Value;

                var clauseForms = elseClause.ToList();

                for (int i = 1; i < clauseForms.Count; i++)
                {
                    var expr = clauseForms[i];

                    result = (i == clauseForms.Count - 1 && expr is Pair pair) ? LillispRuntime.TailCall(scope, pair) : runtime.Evaluate(scope, expr);
                }

                return result;
            }

            throw new InvalidOperationException("No clause matched for the cond expression");
        }

        public static object? Delay(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 1 || args[0] is not Node node)
            {
                throw new ArgumentException("delay requires one expression argument");
            }

            return new Lazy<object?>(() => runtime.Evaluate(scope, node));
        }

        public static object? DelayForce(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 1 || args[0] is not Node node)
            {
                throw new ArgumentException("delay-force requires one expression argument");
            }

            return new Lazy<object?>(() =>
            {
                var result = runtime.Evaluate(scope, node);

                return result switch
                {
                    Lazy<object?> lazy => lazy.Value,
                    Task<object?> task => task.Result,
                    _ => result,
                };
            });
        }

        public static object? Include(LillispRuntime runtime, Scope scope, object?[] args)
        {
            object? result = Nil.Value;

            foreach (var arg in args)
            {
                var fileName = arg?.ToString();

                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentException("File name given to include cannot be null or empty");
                }

                var fileText = File.ReadAllText(fileName);

                result = runtime.EvaluateProgram(scope, fileText);
            }

            return result;
        }

        public static object? Eval(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("eval requires one argument");
            }

            return runtime.Evaluate(scope, runtime.Evaluate(scope, args[0]));
        }
    }
}