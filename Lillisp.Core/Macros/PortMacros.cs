using System;
using System.Globalization;
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
            object? port = GetOutputPort(runtime, scope, args.Length == 2 ? args[1] : null);

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
            else
            {
                throw new ArgumentException("Provided port is not an output port");
            }
        }

        public static object? Newline(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("newline requires zero or one arguments");
            }

            object? port = GetOutputPort(runtime, scope, args.Length == 1 ? args[0] : null);

            DisplayWithPort(Environment.NewLine, port);

            return Nil.Value;
        }

        private static object? GetOutputPort(LillispRuntime runtime, Scope scope, object? portArg)
        {
            object? port;

            if (portArg == null)
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
                port = runtime.Evaluate(scope, portArg);
            }

            return port;
        }

        private static object? GetInputPort(LillispRuntime runtime, Scope scope, object? portArg)
        {
            object? port;

            if (portArg == null)
            {
                port = scope.Resolve("current-input-port");

                if (port is not Parameter portParam)
                {
                    throw new InvalidOperationException("current-input-port is not a parameter");
                }

                port = portParam.Value;
            }
            else
            {
                port = runtime.Evaluate(scope, portArg);
            }

            return port;
        }

        public static object? ReadChar(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("read-char requires zero or one arguments");
            }

            object? port = GetInputPort(runtime, scope, args.Length == 1 ? args[0] : null);

            if (port is not TextReader tr)
            {
                throw new ArgumentException("Specified port is not a textual input port");
            }

            int value = tr.Read();

            if (value == -1)
            {
                return EofObject.Instance;
            }

            return (char)value;
        }

        public static object? PeekChar(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("peek-char requires zero or one arguments");
            }

            object? port = GetInputPort(runtime, scope, args.Length == 1 ? args[0] : null);

            if (port is not TextReader tr)
            {
                throw new ArgumentException("Specified port is not a textual input port");
            }

            int value = tr.Peek();

            if (value == -1)
            {
                return EofObject.Instance;
            }

            return (char)value;
        }

        public static object? ReadString(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("read-string requires one or two arguments");
            }

            int k = Convert.ToInt32(runtime.Evaluate(scope, args[0]));

            if (k == 0)
            {
                return string.Empty;
            }

            object? port = GetInputPort(runtime, scope, args.Length == 2 ? args[1] : null);

            if (port is not TextReader tr)
            {
                throw new ArgumentException("Specified port is not a textual input port");
            }

            var buffer = new char[k];

            int count = tr.Read(buffer, 0, k);

            if (count == 0)
            {
                return EofObject.Instance;
            }

            return new string(buffer, 0, count);
        }

        public static object? ReadU8(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("read-u8 requires zero or one arguments");
            }

            object? port = GetInputPort(runtime, scope, args.Length == 1 ? args[0] : null);

            if (port is not Stream stream)
            {
                throw new ArgumentException("Specified port is not a binary input port");
            }

            int value = stream.ReadByte();

            if (value == -1)
            {
                return EofObject.Instance;
            }

            return (byte)value;
        }

        public static object? ReadBytevector(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("read-bytevector requires one or two arguments");
            }

            int k = Convert.ToInt32(runtime.Evaluate(scope, args[0]));

            if (k == 0)
            {
                return new Bytevector();
            }

            object? port = GetInputPort(runtime, scope, args.Length == 2 ? args[1] : null);

            if (port is not Stream stream)
            {
                throw new ArgumentException("Specified port is not a binary input port");
            }

            var buffer = new byte[k];

            int count = stream.Read(buffer, 0, k);

            if (count == 0)
            {
                return EofObject.Instance;
            }

            return new Bytevector(buffer);
        }

        public static object? ReadBytevectorMutate(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length is 0 or > 4)
            {
                throw new ArgumentException("read-bytevector! requires one to four arguments");
            }

            var bv = runtime.Evaluate(scope, args[0]) as Bytevector;

            if (bv == null)
            {
                throw new ArgumentException("read-bytevector!'s first argument must be a bytevector");
            }

            object? port = GetInputPort(runtime, scope, args.Length > 1 ? args[1] : null);

            if (port is not Stream stream)
            {
                throw new ArgumentException("Specified port is not a binary input port");
            }

            int start = 0, end = bv.Count;

            if (args.Length > 2)
            {
                start = Convert.ToInt32(runtime.Evaluate(scope, args[2]));
            }

            if (args.Length == 4)
            {
                end = Convert.ToInt32(runtime.Evaluate(scope, args[3]));
            }

            int count = end - start;

            if (count <= 0)
            {
                return EofObject.Instance;
            }

            var data = new byte[count];

            int countRead = stream.Read(data, 0, count);

            if (countRead <= 0)
            {
                return EofObject.Instance;
            }

            for (int i = 0, j = start; i < countRead && j < end; i++, j++)
            {
                bv[j] = data[i];
            }

            return countRead;
        }

        public static object? ReadLine(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length > 1)
            {
                throw new ArgumentException("read-line requires zero or one arguments");
            }

            object? port = GetInputPort(runtime, scope, args.Length == 1 ? args[0] : null);

            if (port is not TextReader tr)
            {
                throw new ArgumentException("Specified port is not a textual input port");
            }

            string? value = tr.ReadLine();

            if (value == null)
            {
                return EofObject.Instance;
            }

            return value;
        }
    }
}
