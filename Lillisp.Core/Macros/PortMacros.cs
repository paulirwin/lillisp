﻿using System.Text;

namespace Lillisp.Core.Macros;

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

    public static object? Read(LillispRuntime runtime, Scope scope, object?[] args)
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

        var input = tr.ReadToEnd();

        if (input.Length == 0)
        {
            return EofObject.Instance;
        }

        try
        {
            var node = LillispRuntime.ParseProgramText(input);

            if (node is not Program { Children.Count: > 0 } program)
            {
                throw new InvalidOperationException("Input did not parse as an object");
            }

            return program.Children[0];
        }
        catch (Exception ex)
        {
            throw new ReadError(ex);
        }
    }

    public static object? CharReady(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length > 1)
        {
            throw new ArgumentException("char-ready? requires zero or one arguments");
        }

        object? port = GetInputPort(runtime, scope, args.Length == 1 ? args[0] : null);

        if (port is not TextReader tr)
        {
            throw new ArgumentException("Specified port is not a textual input port");
        }

        return tr switch
        {
            StreamReader sr => sr.BaseStream.CanSeek,
            StringReader => true,
            _ => false
        };
    }

    public static object? PeekU8(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length > 1)
        {
            throw new ArgumentException("peek-u8 requires zero or one arguments");
        }

        object? port = GetInputPort(runtime, scope, args.Length == 1 ? args[0] : null);

        if (port is not Stream stream)
        {
            throw new ArgumentException("Specified port is not a binary input port");
        }

        if (!stream.CanSeek)
        {
            throw new ArgumentException("Stream does not support peeking");
        }

        int value = stream.ReadByte();
        stream.Seek(-1, SeekOrigin.Current);

        if (value == -1)
        {
            return EofObject.Instance;
        }

        return (byte)value;
    }

    public static object? U8Ready(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length > 1)
        {
            throw new ArgumentException("u8-ready? requires zero or one arguments");
        }

        object? port = GetInputPort(runtime, scope, args.Length == 1 ? args[0] : null);

        if (port is not Stream s)
        {
            throw new ArgumentException("Specified port is not a binary input port");
        }

        return s.CanSeek;
    }

    public static object? CallWithPort(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("call-with-port requires two arguments");
        }

        var port = runtime.Evaluate(scope, args[0]);

        if (port is not Stream and not TextReader and not TextWriter)
        {
            throw new InvalidOperationException("call-with-port's first argument must evaluate to a port");
        }

        var proc = runtime.Evaluate(scope, args[1]);

        var result = runtime.InvokePossibleTailCallExpression(scope, proc, new[] { port });

        if (port is IDisposable disposable)
        {
            disposable.Dispose();
        }

        return result;
    }

    public static object? Write(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new ArgumentException("write requires one or two arguments");
        }

        var obj = runtime.Evaluate(scope, args[0]);
        object? port = GetOutputPort(runtime, scope, args.Length == 2 ? args[1] : null);

        DisplayWithPort(OutputFormatter.FormatPr(obj), port);

        return Nil.Value;
    }

    public static object? WriteChar(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new ArgumentException("write-char requires one or two arguments");
        }

        var obj = runtime.Evaluate(scope, args[0]);

        if (obj is not char c)
        {
            throw new ArgumentException("write-char's first argument must be a character");
        }

        object? port = GetOutputPort(runtime, scope, args.Length == 2 ? args[1] : null);

        if (port is not TextWriter tw)
        {
            throw new ArgumentException("Specified port is not a textual output port");
        }
        
        tw.Write(c);

        return Nil.Value;
    }

    public static object? WriteString(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length is 0 or > 4)
        {
            throw new ArgumentException("write-string requires one to four arguments");
        }

        var obj = runtime.Evaluate(scope, args[0]);

        if (obj is not string s)
        {
            if (obj is StringBuilder sb)
            {
                s = sb.ToString();
            }
            else
            {
                throw new ArgumentException("write-string's first argument must be a string");
            }
        }

        object? port = GetOutputPort(runtime, scope, args.Length == 2 ? args[1] : null);

        if (port is not TextWriter tw)
        {
            throw new ArgumentException("Specified port is not a textual output port");
        }

        int start = 0, end = s.Length;

        if (args.Length > 2)
        {
            start = Convert.ToInt32(runtime.Evaluate(scope, args[2]));
        }

        if (args.Length == 4)
        {
            end = Convert.ToInt32(runtime.Evaluate(scope, args[3]));
        }

        tw.Write(s.AsSpan(start, end - start));

        return Nil.Value;
    }

    public static object? WriteU8(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new ArgumentException("write-u8 requires one or two arguments");
        }

        var obj = runtime.Evaluate(scope, args[0]);

        if (obj is not byte b)
        {
            throw new ArgumentException("write-u8's first argument must be a byte");
        }

        object? port = GetOutputPort(runtime, scope, args.Length == 2 ? args[1] : null);

        if (port is not Stream s)
        {
            throw new ArgumentException("Specified port is not a binary output port");
        }

        s.WriteByte(b);

        return Nil.Value;
    }

    public static object? WriteBytevector(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new ArgumentException("write-bytevector requires one or two arguments");
        }

        var obj = runtime.Evaluate(scope, args[0]);

        if (obj is not Bytevector bv)
        {
            throw new ArgumentException("write-bytevector's first argument must be a bytevector");
        }

        object? port = GetOutputPort(runtime, scope, args.Length == 2 ? args[1] : null);

        if (port is not Stream s)
        {
            throw new ArgumentException("Specified port is not a binary output port");
        }

        s.Write(bv.ToByteArray());

        return Nil.Value;
    }
}