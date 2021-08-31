using System;
using Antlr4.Runtime;
using Lillisp.Core;

namespace Lillisp.Repl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Lillisp> ");
                Console.ForegroundColor = ConsoleColor.White;

                string? input = Console.ReadLine();

                if (input == null)
                {
                    break;
                }

                try
                {
                    var lexer = new LillispLexer(new AntlrInputStream(input));
                    var parser = new LillispParser(new CommonTokenStream(lexer));
                    var visitor = new LillispVisitor();

                    var prog = visitor.Visit(parser.prog());

#if DEBUG
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("AST: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(prog);
#endif

                    var runtime = new LillispRuntime();

                    object? result = runtime.Evaluate(prog);

                    Console.Write("-> ");
                    Console.WriteLine(result ?? "(null)");
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
