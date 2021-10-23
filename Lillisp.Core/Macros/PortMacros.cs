using System;
using System.IO;
using System.Text;

namespace Lillisp.Core.Macros
{
    public static class PortMacros
    {
        public static object? Display(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length is 0 or > 2)
            {
                throw new ArgumentException("display requires one or two arguments");
            }

            var obj = runtime.Evaluate(scope, args[0]);
            object? port;

            if (args.Length == 1)
            {
                port = scope.Resolve("current-output-port");

                if (port is not Parameter portParam)
                {
                    throw new InvalidOperationException("current-output-port is not a parameter");
                }

                port = portParam.Value;
            }
            else
            {
                port = runtime.Evaluate(scope, args[1]);
            }

            DisplayWithPort(OutputFormatter.FormatPrint(obj), port);

            return Nil.Value;
        }

        private static void DisplayWithPort(string? text, object? port)
        {
            if (text == null)
            {
                return;

            }
            if (port is TextWriter tw)
            {
                tw.Write(text);
            }
            else if (port is Stream stream)
            {
                stream.Write(Encoding.UTF8.GetBytes(text));
            }
        }

        public static object? Newline(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("newline requires zero or one arguments");
            }
            
            object? port;

            if (args.Length == 0)
            {
                port = scope.Resolve("current-output-port");

                if (port is not Parameter portParam)
                {
                    throw new InvalidOperationException("current-output-port is not a parameter");
                }

                port = portParam.Value;
            }
            else
            {
                port = runtime.Evaluate(scope, args[1]);
            }

            DisplayWithPort(Environment.NewLine, port);

            return Nil.Value;
        }
    }
}
