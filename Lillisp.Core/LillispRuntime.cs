using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using Lillisp.Core.Expressions;
using Lillisp.Core.Macros;

namespace Lillisp.Core
{
    public class LillispRuntime
    {
        /// <summary>
        /// The default collection of system "macros."
        /// </summary>
        /// <remarks>
        /// Note that this use of the term "macro" is different than the Scheme/Lisp use of the term, and
        /// they are not intended to mean the same thing. Here, macros are .NET functions that have access
        /// to the runtime, the current environment, and the AST of the expression. Therefore, they have
        /// the power to conditionally choose not to evaluate code, manipulate the environment (scope), or
        /// otherwise cause mayhem. Normal expressions, on the other hand, can only operate with their
        /// already-evaluated inputs. Ideally, most functions would be implemented as expressions rather
        /// than macros, unless access to the runtime, AST, or environment is needed.
        /// 
        /// Also note that "unquote" and "unquote-splicing" are implemented in LillispVisitor, as they
        /// are special forms.
        /// </remarks>
        private static readonly IReadOnlyDictionary<string, MacroExpression> _systemMacros = new Dictionary<string, MacroExpression>
        {
            ["++!"] = MathMacros.Increment,
            ["--!"] = MathMacros.Decrement,
            ["and"] = BooleanMacros.And,
            ["apply"] = CoreMacros.Apply,
            ["begin"] = CoreMacros.Begin,
            ["call-with-port"] = PortMacros.CallWithPort,
            ["case"] = CoreMacros.Case,
            ["case-lambda"] = CoreMacros.CaseLambda,
            ["char-ready?"] = PortMacros.CharReady,
            ["cond"] = CoreMacros.Cond,
            ["def"] = CoreMacros.Define,
            ["defenum"] = EmitMacros.DefineEnum,
            ["define"] = CoreMacros.Define,
            ["define-record-type"] = CoreMacros.DefineRecordType,
            ["define-syntax"] = SchemeMacroMacros.DefineSyntax,
            ["defrecord"] = EmitMacros.DefineRecord,
            ["defun"] = CoreMacros.Defun,
            ["delay"] = CoreMacros.Delay,
            ["delay-force"] = CoreMacros.DelayForce,
            ["display"] = PortMacros.Display,
            ["do"] = CoreMacros.Do,
            ["eval"] = CoreMacros.Eval,
            ["from"] = LispINQMacros.From,
            ["guard"] = ExceptionMacros.Guard,
            ["if"] = CoreMacros.If,
            ["include"] = CoreMacros.Include,
            //["include-ci"] = CoreMacros.Include, // TODO
            ["lambda"] = CoreMacros.Lambda,
            ["let"] = CoreMacros.Let,
            ["let*"] = CoreMacros.LetStar,
            ["let-syntax"] = SchemeMacroMacros.LetSyntax,
            ["letrec"] = CoreMacros.Let,
            ["letrec*"] = CoreMacros.LetStar,
            ["letrec-syntax"] = SchemeMacroMacros.LetSyntax,
            ["make-parameter"] = ParameterMacros.MakeParameter,
            ["map"] = CoreMacros.Map,
            ["new"] = InteropMacros.New,
            ["newline"] = PortMacros.Newline,
            ["or"] = BooleanMacros.Or,
            ["parameterize"] = ParameterMacros.Parameterize,
            ["peek-char"] = PortMacros.PeekChar,
            ["peek-u8"] = PortMacros.PeekU8,
            ["quasiquote"] = CoreMacros.Quasiquote,
            ["quote"] = CoreMacros.Quote,
            ["raise-continuable"] = ExceptionMacros.RaiseContinuable,
            ["read"] = PortMacros.Read,
            ["read-bytevector"] = PortMacros.ReadBytevector,
            ["read-bytevector!"] = PortMacros.ReadBytevectorMutate,
            ["read-char"] = PortMacros.ReadChar,
            ["read-line"] = PortMacros.ReadLine,
            ["read-string"] = PortMacros.ReadString,
            ["read-u8"] = PortMacros.ReadU8,
            ["set!"] = CoreMacros.Set,
            ["string-map"] = CoreMacros.StringMap,
            ["syntax-rules"] = SchemeMacroMacros.SyntaxRules,
            ["u8-ready?"] = PortMacros.U8Ready,
            ["unless"] = BooleanMacros.Unless,
            ["use"] = InteropMacros.Use,
            ["vector-map"] = CoreMacros.VectorMap,
            ["when"] = BooleanMacros.When,
            ["with-exception-handler"] = ExceptionMacros.WithExceptionHandler,
            ["write"] = PortMacros.Write,
            ["write-simple"] = PortMacros.Write, // HACK: since datum labels not yet supported, all writes are "simple"
            ["write-char"] = PortMacros.WriteChar,
            ["write-string"] = PortMacros.WriteString,
            ["write-u8"] = PortMacros.WriteU8,
            ["write-bytevector"] = PortMacros.WriteBytevector,
        };

        private static readonly IReadOnlyDictionary<string, Expression> _systemFunctions = new Dictionary<string, Expression>
        {
            ["="] = BooleanExpressions.NumericallyEqual,
            ["+"] = MathExpressions.Add,
            ["-"] = MathExpressions.Subtract,
            ["*"] = MathExpressions.Multiply,
            ["/"] = MathExpressions.Divide,
            ["%"] = MathExpressions.Modulo,
            ["**"] = MathExpressions.Power,
            [">"] = BooleanExpressions.GreaterThan,
            [">="] = BooleanExpressions.GreaterThanOrEqual,
            ["<"] = BooleanExpressions.LessThan,
            ["<="] = BooleanExpressions.LessThanOrEqual,
            [">>"] = MathExpressions.ShiftRight,
            ["<<"] = MathExpressions.ShiftLeft,
            ["abs"] = MathExpressions.Abs,
            ["angle"] = ComplexExpressions.Angle,
            ["append"] = ListExpressions.Append,
            ["binary-port"] = PortExpressions.IsBinaryPort,
            ["boolean?"] = TypeExpressions.IsBoolean,
            ["bytevector?"] = TypeExpressions.IsBytevector,
            ["bytevector"] = BytevectorExpressions.Bytevector,
            ["bytevector-append"] = BytevectorExpressions.BytevectorAppend,
            ["bytevector-copy"] = BytevectorExpressions.BytevectorCopy,
            ["bytevector-copy!"] = BytevectorExpressions.BytevectorCopyTo,
            ["bytevector-length"] = BytevectorExpressions.BytevectorLength,
            ["bytevector-u8-ref"] = BytevectorExpressions.BytevectorU8Ref,
            ["bytevector-u8-set!"] = BytevectorExpressions.BytevectorU8Set,
            ["car"] = ListExpressions.Car,
            ["cdr"] = ListExpressions.Cdr,
            ["ceiling"] = MathExpressions.Ceiling,
            ["char?"] = TypeExpressions.IsChar,
            ["char=?"] = CharacterExpressions.Equals,
            ["char<?"] = CharacterExpressions.LessThan,
            ["char>?"] = CharacterExpressions.GreaterThan,
            ["char<=?"] = CharacterExpressions.LessThanOrEqualTo,
            ["char>=?"] = CharacterExpressions.GreaterThanOrEqualTo,
            ["char-ci=?"] = CharacterExpressions.CaseInsensitiveEquals,
            ["char-ci<?"] = CharacterExpressions.CaseInsensitiveLessThan,
            ["char-ci>?"] = CharacterExpressions.CaseInsensitiveGreaterThan,
            ["char-ci<=?"] = CharacterExpressions.CaseInsensitiveLessThanOrEqualTo,
            ["char-ci>=?"] = CharacterExpressions.CaseInsensitiveGreaterThanOrEqualTo,
            ["char-alphabetic?"] = CharacterExpressions.IsAlphabetic,
            ["char-numeric?"] = CharacterExpressions.IsNumeric,
            ["char-whitespace?"] = CharacterExpressions.IsWhitespace,
            ["char-upper-case?"] = CharacterExpressions.IsUpperCase,
            ["char-lower-case?"] = CharacterExpressions.IsLowerCase,
            ["char-upcase"] = CharacterExpressions.Upcase,
            ["char-downcase"] = CharacterExpressions.Downcase,
            ["char-foldcase"] = CharacterExpressions.Foldcase,
            ["char->integer"] = TypeExpressions.CharacterToInteger,
            ["close-input-port"] = PortExpressions.CloseInputPort,
            ["close-output-port"] = PortExpressions.CloseOutputPort,
            ["close-port"] = PortExpressions.ClosePort,
            ["command-line"] = ProcessContextExpressions.CommandLine,
            ["complex?"] = TypeExpressions.IsComplex,
            ["cons"] = ListExpressions.Cons,
            ["count"] = DynamicExpressions.Count,
            ["current-jiffy"] = TimeExpressions.CurrentJiffy,
            ["current-second"] = TimeExpressions.CurrentSecond,
            ["dec"] = MathExpressions.Decrement,
            ["delete-file"] = FileExpressions.DeleteFile,
            ["denominator"] = RationalExpressions.Denominator,
            ["digit-value"] = CharacterExpressions.DigitValue,
            ["cast"] = InteropExpressions.Cast,
            ["convert"] = InteropExpressions.Convert,
            ["eof-object"] = PortExpressions.GetEofObject,
            ["eof-object?"] = PortExpressions.IsEofObject,
            ["emergency-exit"] = ProcessContextExpressions.EmergencyExit,
            ["eq?"] = BooleanExpressions.ReferencesEqual,
            ["equal?"] = BooleanExpressions.Equal,
            ["eqv?"] = BooleanExpressions.Equivalent,
            ["error"] = ExceptionExpressions.Error,
            ["error-object?"] = ExceptionExpressions.ErrorObject,
            ["error-object-irritants"] = ExceptionExpressions.ErrorObjectIrritants,
            ["error-object-message"] = ExceptionExpressions.ErrorObjectMessage,
            ["exact"] = TypeExpressions.ConvertToExact,
            ["exact?"] = TypeExpressions.IsExact,
            ["exact-integer?"] = TypeExpressions.IsExactInteger,
            ["exact-integer-sqrt"] = MathExpressions.ExactIntegerSqrt,
            ["expt"] = MathExpressions.Power,
            ["file-error?"] = ExceptionExpressions.FileError,
            ["file-exists?"] = FileExpressions.FileExists,
            ["finite?"] = TypeExpressions.IsFinite,
            ["floor"] = MathExpressions.Floor,
            ["floor/"] = MathExpressions.FloorDivide,
            ["floor-quotient"] = MathExpressions.FloorQuotient,
            ["floor-remainder"] = MathExpressions.FloorRemainder,
            ["flush-output-port"] = PortExpressions.FlushOutputPort,
            ["force"] = DynamicExpressions.Force,
            ["gcd"] = MathExpressions.Gcd,
            ["get"] = DynamicExpressions.Get,
            ["get-environment-variable"] = ProcessContextExpressions.GetEnvironmentVariable,
            ["get-environment-variables"] = ProcessContextExpressions.GetEnvironmentVariables,
            ["get-output-bytevector"] = PortExpressions.GetOutputBytevector,
            ["get-output-string"] = PortExpressions.GetOutputString,
            ["imag-part"] = ComplexExpressions.ImaginaryPart,
            ["inc"] = MathExpressions.Increment,
            ["inexact"] = TypeExpressions.ConvertToInexact,
            ["inexact?"] = TypeExpressions.IsInexact,
            ["infinite?"] = TypeExpressions.IsInfinite,
            ["input-port?"] = PortExpressions.IsInputPort,
            ["input-port-open?"] = PortExpressions.IsInputPortOpen,
            ["integer?"] = TypeExpressions.IsInteger,
            ["integer->char"] = TypeExpressions.IntegerToCharacter,
            ["jiffies-per-second"] = TimeExpressions.JiffiesPerSecond,
            ["lcm"] = MathExpressions.Lcm,
            ["length"] = DynamicExpressions.Count,
            ["list"] = ListExpressions.List,
            ["list?"] = TypeExpressions.IsList,
            ["list-set!"] = ListExpressions.ListSet,
            ["list->string"] = TypeExpressions.ListToString,
            ["ln"] = MathExpressions.Ln,
            ["log"] = MathExpressions.Log,
            ["magnitude"] = ComplexExpressions.Magnitude,
            ["make-bytevector"] = BytevectorExpressions.MakeBytevector,
            ["make-list"] = ListExpressions.MakeList,
            ["make-polar"] = ComplexExpressions.MakePolar,
            ["make-promise"] = DynamicExpressions.MakePromise,
            ["make-rectangular"] = ComplexExpressions.MakeRectangular,
            ["make-string"] = StringExpressions.MakeString,
            ["make-vector"] = VectorExpressions.MakeVector,
            ["max"] = MathExpressions.Max,
            ["min"] = MathExpressions.Min,
            ["modulo"] = MathExpressions.FloorRemainder,
            ["nan?"] = TypeExpressions.IsNaN,
            ["not"] = BooleanExpressions.Not,
            ["null?"] = TypeExpressions.IsNull,
            ["numerator"] = RationalExpressions.Numerator,
            ["number?"] = TypeExpressions.IsNumber,
            ["number->string"] = TypeExpressions.NumberToString,
            ["open-binary-input-file"] = PortExpressions.OpenBinaryInputFile,
            ["open-binary-output-file"] = PortExpressions.OpenBinaryOutputFile,
            ["open-input-bytevector"] = PortExpressions.OpenInputBytevector,
            ["open-input-file"] = PortExpressions.OpenInputFile,
            ["open-input-string"] = PortExpressions.OpenInputString,
            ["open-output-bytevector"] = PortExpressions.OpenOutputBytevector,
            ["open-output-file"] = PortExpressions.OpenOutputFile,
            ["open-output-string"] = PortExpressions.OpenOutputString,
            ["output-port?"] = PortExpressions.IsOutputPort,
            ["output-port-open?"] = PortExpressions.IsOutputPortOpen,
            ["pair?"] = TypeExpressions.IsPair,
            ["port?"] = TypeExpressions.IsPort,
            ["pow"] = MathExpressions.Power,
            ["procedure?"] = TypeExpressions.IsProcedure,
            ["promise?"] = TypeExpressions.IsPromise,
            ["print"] = StringExpressions.Print,
            ["println"] = StringExpressions.PrintLn,
            ["pr"] = StringExpressions.Pr,
            ["prn"] = StringExpressions.Prn,
            ["quotient"] = MathExpressions.TruncateQuotient,
            ["raise"] = ExceptionExpressions.Raise,
            ["range"] = ListExpressions.Range,
            ["rational?"] = TypeExpressions.IsRational,
            ["rationalize"] = RationalExpressions.Rationalize,
            ["read-error?"] = ExceptionExpressions.ReadError,
            ["real?"] = TypeExpressions.IsReal,
            ["real-part"] = ComplexExpressions.RealPart,
            ["remainder"] = MathExpressions.TruncateRemainder,
            ["reverse"] = ListExpressions.Reverse,
            ["round"] = MathExpressions.Round,
            ["set-car!"] = ListExpressions.SetCar,
            ["set-cdr!"] = ListExpressions.SetCdr,
            ["simplify"] = RationalExpressions.Simplify,
            ["sqrt"] = MathExpressions.Sqrt,
            ["str"] = StringExpressions.Str,
            ["string"] = StringExpressions.String,
            ["string?"] = TypeExpressions.IsString,
            ["string=?"] = StringExpressions.Equals,
            ["string-ci=?"] = StringExpressions.CaseInsensitiveEquals,
            ["string<?"] = StringExpressions.LessThan,
            ["string-ci<?"] = StringExpressions.CaseInsensitiveLessThan,
            ["string<=?"] = StringExpressions.LessThanOrEqualTo,
            ["string-ci<=?"] = StringExpressions.CaseInsensitiveLessThanOrEqualTo,
            ["string>?"] = StringExpressions.GreaterThan,
            ["string-ci>?"] = StringExpressions.CaseInsensitiveGreaterThan,
            ["string>=?"] = StringExpressions.GreaterThanOrEqualTo,
            ["string-ci>=?"] = StringExpressions.CaseInsensitiveGreaterThanOrEqualTo,
            ["string-append"] = StringExpressions.StringAppend,
            ["string-copy"] = StringExpressions.StringCopy,
            ["string-copy!"] = StringExpressions.StringCopyTo,
            ["string-downcase"] = StringExpressions.Downcase,
            ["string-fill!"] = StringExpressions.StringFill,
            ["string-foldcase"] = StringExpressions.Foldcase,
            ["string-length"] = StringExpressions.StringLength,
            ["string-ref"] = StringExpressions.StringRef,
            ["string-set!"] = StringExpressions.StringSet,
            ["string-upcase"] = StringExpressions.Upcase,
            ["string->list"] = TypeExpressions.StringToList,
            ["string->number"] = TypeExpressions.StringToNumber,
            ["string->symbol"] = TypeExpressions.StringToSymbol,
            ["string->utf8"] = TypeExpressions.StringToUtf8,
            ["substring"] = StringExpressions.Substring,
            ["symbol=?"] = BooleanExpressions.SymbolEquals,
            ["symbol?"] = TypeExpressions.IsSymbol,
            ["symbol->string"] = TypeExpressions.SymbolToString,
            ["textual-port?"] = PortExpressions.IsTextualPort,
            ["truncate"] = MathExpressions.Truncate,
            ["truncate/"] = MathExpressions.TruncateDivide,
            ["truncate-quotient"] = MathExpressions.TruncateQuotient,
            ["truncate-remainder"] = MathExpressions.TruncateRemainder,
            ["typeof"] = TypeExpressions.TypeOf,
            ["utf8->string"] = TypeExpressions.Utf8ToString,
            ["vector"] = VectorExpressions.Vector,
            ["vector-append"] = VectorExpressions.Append,
            ["vector-copy"] = VectorExpressions.VectorCopy,
            ["vector-copy!"] = VectorExpressions.VectorCopyTo,
            ["vector-fill!"] = VectorExpressions.VectorFill,
            ["vector-length"] = VectorExpressions.VectorLength,
            ["vector-ref"] = VectorExpressions.VectorRef,
            ["vector-set!"] = VectorExpressions.VectorSet,
            ["vector?"] = TypeExpressions.IsVector,
        };

        private static readonly IReadOnlyDictionary<string, object?> _systemGlobals = new Dictionary<string, object?>
        {
            ["pi"] = Math.PI,
            ["e"] = Math.E,
            ["tau"] = Math.Tau,
            ["current-input-port"] = new Parameter(Console.In),
            ["current-output-port"] = new Parameter(Console.Out),
            ["current-error-port"] = new Parameter(Console.Error),
        };

        private readonly Scope _globalScope;
        private readonly Scope _userScope;

        public LillispRuntime()
        {
            _globalScope = new Scope();
            _globalScope.AddAllFrom(_systemMacros);
            _globalScope.AddAllFrom(_systemFunctions);
            _globalScope.AddAllFrom(_systemGlobals);

            // TODO: don't load all libraries by default, support `import`
            EvaluateLibraryResource("Lillisp.Core.Library.base.lisp");
            EvaluateLibraryResource("Lillisp.Core.Library.cxr.lisp");
            EvaluateLibraryResource("Lillisp.Core.Library.file.lisp");
            EvaluateLibraryResource("Lillisp.Core.Library.inexact.lisp");

            _userScope = _globalScope.CreateChildScope();
        }

        public void RegisterGlobal(string symbol, object? value)
        {
            _globalScope.Define(symbol, value);
        }

        public void RegisterGlobalFunction(string symbol, Expression func)
        {
            _globalScope.Define(symbol, func);
        }

        public object? EvaluateProgram(string program) => EvaluateProgram(_userScope, program);

        public object? EvaluateProgram(Scope scope, string program)
        {
            var prog = ParseProgramText(program);

            return Evaluate(scope, prog);
        }

        public static Node ParseProgramText(string program)
        {
            var lexer = new LillispLexer(new AntlrInputStream(program));
            var parser = new LillispParser(new CommonTokenStream(lexer));
            var visitor = new LillispVisitor();
            
            var prog = visitor.Visit(parser.prog());
            return prog;
        }

        public object? EvaluateProgram(object? node) => Evaluate(_userScope, node);

        private object? Quote(Scope scope, object? node)
        {
            return node switch
            {
                Program program => program.Children.Select(i => Quote(scope, i)).ToArray(),
                Vector vector => new Vector(vector.Select(i => Quote(scope, i))),
                Bytevector bv => bv, // TODO: is this correct?
                Pair pair => QuotePair(Quote(scope, pair.Car), Quote(scope, pair.Cdr)),
                Symbol symbol => symbol,
                Atom atom => atom.Value,
                Quote quote => Quote(scope, quote.Value),
                Quasiquote quote => Quote(scope, quote.Value),
                Unquote { Splicing: false } unquote => Evaluate(scope, unquote.Value),
                Unquote { Splicing: true } unquote => EvaluateUnquoteSplicing(scope, unquote.Value),
                Nil nil => nil,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Node QuotePair(object? car, object? cdr)
        {
            if (car is not Splice && cdr is not Splice)
            {
                return new Pair(car, cdr);
            }

            var values = new List<object?>();

            if (car is Splice carSplice)
            {
                values.AddRange(carSplice.Values);
            }
            else
            {
                values.Add(car);
            }

            if (cdr is Splice cdrSplice)
            {
                values.AddRange(cdrSplice.Values);
            }
            else if (cdr is not Nil)
            {
                values.Add(cdr);
            }

            return List.FromNodes(values);
        }

        private Splice EvaluateUnquoteSplicing(Scope scope, Node unquoteValue)
        {
            var result = Evaluate(scope, unquoteValue);

            if (result is not IEnumerable enumerable)
            {
                throw new InvalidOperationException("Result of unquote-splicing operation is not enumerable");
            }

            return new Splice(enumerable.Cast<object?>());
        }

        public object? Evaluate(Scope scope, object? node) => Evaluate(scope, node, null);

        private object? Evaluate(Scope scope, object? node, int? arity)
        {
            return node switch
            {
                Program program => EvaluateProgram(scope, program),
                Vector vector => EvaluateVector(scope, vector),
                Bytevector bv => bv,
                Pair pair => EvaluateExpression(scope, pair),
                SyntaxBinding binding => TryEvaluateSymbol(binding.Scope, binding, arity, out var value) ? value : EvaluateSymbol(scope, binding, arity),
                Symbol symbol => EvaluateSymbol(scope, symbol, arity),
                Atom atom => atom.Value,
                Quote quote => Quote(scope, quote),
                Quasiquote quote => Quote(scope, quote),
                Nil nil => nil,
                _ => node
            };
        }

        private Vector EvaluateVector(Scope scope, Vector vector)
        {
            var items = vector.Select(i => i is Node node ? Evaluate(scope, node) : i);

            return new Vector(items);
        }
        
        private static object? EvaluateSymbol(Scope scope, Symbol node, int? arity)
        {
            if (TryEvaluateSymbol(scope, node, arity, out var value))
            {
                return value;
            }

            throw new ArgumentException($"Unable to resolve symbol {node}");
        }

        private static bool TryEvaluateSymbol(Scope scope, Symbol node, int? arity, out object? value)
        {
            value = null;

            string? symbol = node.Value;

            if (symbol is null or "null")
                return true;

            if (symbol == "nil")
            {
                value = Nil.Value;
                return true;
            }

            value = scope.Resolve(symbol);

            if (value != null)
                return true;

            value = Interop.ResolveSymbol(scope, symbol, arity);

            return value != null;
        }

        private object? EvaluateExpression(Scope scope, Pair pair)
        {
            var result = EvaluatePossibleTailCallExpression(scope, pair);

            while (result is TailCall tailCall)
            {
                result = EvaluatePossibleTailCallExpression(tailCall.Scope, tailCall.Node);
            }

            return result;
        }

        internal static TailCall TailCall(Scope scope, Pair pair)
        {
            return new TailCall(scope, pair);
        }

        private object? EvaluatePossibleTailCallExpression(Scope scope, Pair pair)
        {
            if (pair.Car is Nil)
            {
                throw new InvalidOperationException("nil is not a function");
            }

            if (pair.Car is Symbol symbol && symbol.Value.StartsWith('.'))
            {
                var memberArgs = pair.Skip(1).Select(i => Evaluate(scope, i)).ToArray();

                return Interop.InvokeMember(this, scope, symbol.Value, memberArgs);
            }

            int? arity = null;

            if (pair.Cdr is Pair { IsList: true } cdrList)
            {
                arity = cdrList.Count();
            }

            var op = Evaluate(scope, pair.Car, arity);

            if (op is MacroExpression macro)
            {
                Debug.WriteLine("Invoking macro: {0}", pair.Car);
                return macro(this, scope, pair.Skip(1).ToArray());
            }

            if (op is Syntax syntax)
            {
                Debug.WriteLine("Evaluating syntax: {0}", pair);
                var node = syntax.Transform(pair.Skip(1).Cast<Node>().Select(i => BindSyntaxArgNode(scope, i)).ToArray());
                Debug.WriteLine("Syntax expanded to: {0}", node);
                return Evaluate(scope, node);
            }

            Debug.WriteLine("Invoking expression: {0}", pair.Car);
            var args = pair.Skip(1).Select(i => Evaluate(scope, i)).ToArray();

            return InvokeExpression(scope, op, args);
        }

        private static Node BindSyntaxArgNode(Scope scope, Node node)
        {
            return node switch
            {
                SyntaxBinding binding => binding, // pass-thru
                Symbol symbol => new SyntaxBinding(symbol, scope),
                Pair { Car: Node carNode, Cdr: Node cdrNode } => new Pair(BindSyntaxArgNode(scope, carNode), BindSyntaxArgNode(scope, cdrNode)),
                _ => node,
            };
        }

        public object? InvokePossibleTailCallExpression(Scope scope, object? expression, object?[] args)
        {
            var result = InvokeExpression(scope, expression, args);

            while (result is TailCall tailCall)
            {
                result = EvaluatePossibleTailCallExpression(tailCall.Scope, tailCall.Node);
            }

            return result;
        }

        public object? InvokeExpression(Scope scope, object? expression, object?[] args)
        {
            return expression switch
            {
                IInvokable invokable => invokable.Invoke(this, scope, args),
                MethodInfo method => method.Invoke(null, args),
                InteropStaticOverloadSet overloadSet => overloadSet.Invoke(args),
                MacroExpression macro => macro(this, scope, args),
                Expression expr => expr(args),
                Type genericType => genericType.MakeGenericType(args.Cast<Type>().ToArray()),
                _ => throw new InvalidOperationException($"Invalid operation: {expression}")
            };
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

        private void EvaluateLibraryResource(string name)
        {
            using var stream = typeof(LillispRuntime).Assembly.GetManifestResourceStream(name);

            if (stream == null)
            {
                throw new InvalidOperationException($"Unable to find embedded resource with name {name}");
            }

            using var sr = new StreamReader(stream);

            string text = sr.ReadToEnd();

            var prog = ParseProgramText(text);

            Evaluate(_globalScope, prog);
        }
    }
}