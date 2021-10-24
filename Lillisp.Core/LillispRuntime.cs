﻿using System;
using System.Collections.Generic;
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
        /// otherwise cause mayhem. Regular expressions, on the other hand, can only operate with their
        /// already- evaluated inputs. Ideally, most functions would be implemented as expressions rather
        /// than macros, unless access to the runtime, AST, or environment is needed.
        /// </remarks>
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
            ["delay"] = CoreMacros.Delay,
            ["display"] = PortMacros.Display,
            ["if"] = CoreMacros.If,
            //["include"] = CoreMacros.Include, // TODO
            //["include-ci"] = CoreMacros.Include, // TODO
            ["lambda"] = CoreMacros.Lambda,
            ["let"] = CoreMacros.Let,
            ["make-parameter"] = ParameterMacros.MakeParameter,
            ["map"] = CoreMacros.Map,
            ["new"] = InteropMacros.New,
            ["newline"] = PortMacros.Newline,
            ["or"] = BooleanMacros.Or,
            ["parameterize"] = ParameterMacros.Parameterize,
            ["peek-char"] = PortMacros.PeekChar,
            ["quote"] = CoreMacros.Quote,
            ["raise-continuable"] = ExceptionMacros.RaiseContinuable,
            ["read-char"] = PortMacros.ReadChar,
            ["read-string"] = PortMacros.ReadString,
            ["read-u8"] = PortMacros.ReadU8,
            ["set!"] = CoreMacros.Set,
            ["use"] = InteropMacros.Use,
            ["when"] = BooleanMacros.When,
            ["with-exception-handler"] = ExceptionMacros.WithExceptionHandler,
        };

        private static readonly IReadOnlyDictionary<string, Expression> _systemFunctions = new Dictionary<string, Expression>
        {
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
            ["="] = BooleanExpressions.Equal,
            ["=="] = BooleanExpressions.Equal,
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
            ["cast"] = TypeExpressions.Cast,
            ["cdr"] = ListExpressions.Cdr,
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
            ["eof-object"] = PortExpressions.EofObject,
            ["eof-object?"] = PortExpressions.IsEofObject,
            ["emergency-exit"] = ProcessContextExpressions.EmergencyExit,
            ["error"] = ExceptionExpressions.Error,
            ["error-object?"] = ExceptionExpressions.ErrorObject,
            ["error-object-irritants"] = ExceptionExpressions.ErrorObjectIrritants,
            ["error-object-message"] = ExceptionExpressions.ErrorObjectMessage,
            ["file-error?"] = ExceptionExpressions.FileError,
            ["file-exists?"] = FileExpressions.FileExists,
            ["force"] = DynamicExpressions.Force,
            ["get"] = DynamicExpressions.Get,
            ["get-environment-variable"] = ProcessContextExpressions.GetEnvironmentVariable,
            ["get-environment-variables"] = ProcessContextExpressions.GetEnvironmentVariables,
            ["get-output-bytevector"] = PortExpressions.GetOutputBytevector,
            ["get-output-string"] = PortExpressions.GetOutputString,
            ["imag-part"] = ComplexExpressions.ImaginaryPart,
            ["inc"] = MathExpressions.Increment,
            ["input-port?"] = PortExpressions.IsInputPort,
            ["input-port-open?"] = PortExpressions.IsInputPortOpen,
            ["integer?"] = TypeExpressions.IsInteger,
            ["integer->char"] = TypeExpressions.IntegerToCharacter,
            ["jiffies-per-second"] = TimeExpressions.JiffiesPerSecond,
            ["length"] = DynamicExpressions.Count,
            ["list"] = ListExpressions.List,
            ["list->string"] = TypeExpressions.ListToString,
            ["ln"] = MathExpressions.Ln,
            ["log"] = MathExpressions.Log,
            ["magnitude"] = ComplexExpressions.Magnitude,
            ["make-bytevector"] = BytevectorExpressions.MakeBytevector,
            ["make-polar"] = ComplexExpressions.MakePolar,
            ["make-promise"] = DynamicExpressions.MakePromise,
            ["make-rectangular"] = ComplexExpressions.MakeRectangular,
            ["make-string"] = StringExpressions.MakeString,
            ["make-vector"] = VectorExpressions.MakeVector,
            ["max"] = MathExpressions.Max,
            ["min"] = MathExpressions.Min,
            ["not"] = BooleanExpressions.Not,
            ["null?"] = TypeExpressions.IsNull,
            ["numerator"] = RationalExpressions.Numerator,
            ["number?"] = TypeExpressions.IsNumber,
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
            ["raise"] = ExceptionExpressions.Raise,
            ["range"] = ListExpressions.Range,
            ["rational?"] = TypeExpressions.IsRational,
            ["rationalize"] = RationalExpressions.Rationalize,
            ["real?"] = TypeExpressions.IsReal,
            ["real-part"] = ComplexExpressions.RealPart,
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
            ["string->utf8"] = TypeExpressions.StringToUtf8,
            ["substring"] = StringExpressions.Substring,
            ["symbol?"] = TypeExpressions.IsSymbol,
            ["textual-port?"] = PortExpressions.IsTextualPort,
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
            ["#t"] = true,
            ["#f"] = false,
            ["true"] = true,
            ["false"] = false,
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

        public object? EvaluateProgram(string program)
        {
            var prog = ParseProgramText(program);

            return EvaluateProgram(prog);
        }

        private static Node ParseProgramText(string program)
        {
            var lexer = new LillispLexer(new AntlrInputStream(program));
            var parser = new LillispParser(new CommonTokenStream(lexer));
            var visitor = new LillispVisitor();

            var prog = visitor.Visit(parser.prog());
            return prog;
        }

        public object? EvaluateProgram(object? node) => Evaluate(_userScope, node);

        public object? Quote(object? node)
        {
            return node switch
            {
                Program program => program.Children.Select(Quote).ToArray(),
                Vector vector => vector, // TODO: is this correct?
                Bytevector bv => bv, // TODO: is this correct?
                Pair pair => pair.Select(Quote).ToArray(),
                Symbol symbol => symbol.Value,
                Atom atom => atom.Value,
                Quote quote => quote.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
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
                Symbol symbol => EvaluateSymbol(scope, symbol, arity),
                Atom atom => atom.Value,
                Quote quote => EvaluateQuote(quote),
                Nil nil => nil,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private object? EvaluateVector(Scope scope, Vector vector)
        {
            var items = vector.Select(i => i is Node node ? Evaluate(scope, node) : i);

            return new Vector(items);
        }

        private object? EvaluateQuote(Quote node)
        {
            return node.Value switch
            {
                Vector vector => new Vector(vector.Select(i => i is Node n ? Quote(n) : i)),
                Bytevector bv => bv,
                Pair pair => pair.Select(Quote).ToArray(),
                Atom { Value: { } value } => value,
                Symbol symbol => symbol,
                Nil nil => nil,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static object? EvaluateSymbol(Scope scope, Symbol node, int? arity)
        {
            string? symbol = node.Value;

            if (symbol is null or "null")
                return null;

            if (symbol == "nil")
                return Nil.Value;

            object? value = scope.Resolve(symbol);

            if (value != null)
                return value;

            value = Interop.ResolveSymbol(scope, symbol, arity);

            if (value != null)
                return value;

            throw new ArgumentException($"Unable to resolve symbol {symbol}");
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
                return macro(this, scope, pair.Skip(1).Cast<object>().ToArray());
            }

            var args = pair.Skip(1).Select(i => Evaluate(scope, i)).ToArray();
            
            return op switch
            {
                IInvokable invokable => invokable.Invoke(this, scope, args),
                MethodInfo method => method.Invoke(null, args),
                InteropStaticOverloadSet overloadSet => overloadSet.Invoke(args),
                Expression expr => expr(args),
                Type genericType => genericType.MakeGenericType(args.Cast<Type>().ToArray()),
                _ => throw new InvalidOperationException($"Invalid operation: {op}")
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