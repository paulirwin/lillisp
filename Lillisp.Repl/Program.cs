using System;
using Antlr4.Runtime;
using Lillisp.Core;

namespace Lillisp.Repl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var runtime = new LillispRuntime();
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

                try
                {
                    var lexer = new LillispLexer(new AntlrInputStream(input));
                    var parser = new LillispParser(new CommonTokenStream(lexer));

                    var prog = visitor.Visit(parser.prog());

#if DEBUG
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("AST: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(prog);
#endif

                    object? result = runtime.Evaluate(prog);

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("-> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(ReplOutputFormatter.Format(result));
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
    }
}
