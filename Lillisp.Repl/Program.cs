using System;
using System.Reflection;
using Antlr4.Runtime;
using Lillisp.Core;

namespace Lillisp.Repl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new ReplOptions();

            var runtime = CreateRuntime(options);

            var visitor = new LillispVisitor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Lillisp> ");
                Console.ForegroundColor = ConsoleColor.White;

                string? input = Console.ReadLine();

                if (input == null)
                {
                    break; // happens with ctrl+c
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)
                    || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (input.Equals("clear", StringComparison.OrdinalIgnoreCase)
                    || input.Equals("cls", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                if (input.Equals("reset", StringComparison.OrdinalIgnoreCase))
                {
                    runtime = CreateRuntime(options);
                    Console.WriteLine("Runtime environment reset to defaults.");
                    continue;
                }

                try
                {
                    var lexer = new LillispLexer(new AntlrInputStream(input));
                    var parser = new LillispParser(new CommonTokenStream(lexer));

                    var prog = visitor.Visit(parser.prog());

                    if (options.ShowAst)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("AST: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(prog);
                    }

                    object? result = runtime.EvaluateProgram(prog);

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("-> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(OutputFormatter.FormatRepl(result) ?? "null");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("ERROR: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static LillispRuntime CreateRuntime(ReplOptions options)
        {
            var runtime = new LillispRuntime();

            // HACK.PI: use proper symbols to avoid polluting globals
            runtime.RegisterGlobal("show-ast", nameof(options.ShowAst));

            runtime.RegisterGlobalFunction("repl-config!", cargs =>
            {
                var prop = typeof(ReplOptions).GetProperty(cargs[0]?.ToString() ?? "unknown", BindingFlags.Public | BindingFlags.Instance);

                if (prop == null)
                {
                    throw new ArgumentException("Unknown repl-config! property");
                }

                prop.SetValue(options, cargs[1]);

                return Nil.Value;
            });

            return runtime;
        }
    }
}
