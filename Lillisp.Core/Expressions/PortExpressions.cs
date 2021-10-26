using System;
using System.IO;
using System.Text;

namespace Lillisp.Core.Expressions
{
    public static class PortExpressions
    {
        public static object? OpenOutputString(object?[] args)
        {
            return new StringWriter();
        }

        public static object? GetOutputString(object?[] args)
        {
            if (args.Length != 1 || args[0] is not StringWriter sw)
            {
                throw new ArgumentException("get-output-string requires one argument that must be created with open-output-string");
            }

            return sw.ToString();
        }

        public static object? IsInputPort(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("input-port? requires one argument");
            }

            return args[0] is Stream or TextReader;
        }

        public static object? IsOutputPort(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("output-port? requires one argument");
            }

            return args[0] is Stream or TextWriter;
        }

        public static object? IsTextualPort(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("textual-port? requires one argument");
            }

            return args[0] is TextReader or TextWriter;
        }

        public static object? IsBinaryPort(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("binary-port? requires one argument");
            }

            return args[0] is Stream;
        }

        /// <summary>
        /// Determines if the input port is open and capable of performing input.
        ///
        /// Note that for textual ports, this always returns true, as there is no
        /// public .NET API to determine if a TextReader is open or closed.
        /// </summary>
        /// <param name="args">The arguments to the function.</param>
        /// <returns>Boolean</returns>
        public static object? IsInputPortOpen(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("input-port-open? requires one argument");
            }

            return args[0] switch
            {
                Stream stream => stream.CanRead,
                TextReader => true, // HACK: we can't actually tell
                _ => false
            };
        }

        /// <summary>
        /// Determines if the output port is open and capable of performing output.
        ///
        /// Note that for textual ports, this always returns true, as there is no
        /// public .NET API to determine if a TextWriter is open or closed.
        /// </summary>
        /// <param name="args">The arguments to the function.</param>
        /// <returns>Boolean</returns>
        public static object? IsOutputPortOpen(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("output-port-open? requires one argument");
            }

            return args[0] switch
            {
                Stream stream => stream.CanWrite,
                TextReader => true, // HACK: we can't actually tell
                _ => false
            };
        }

        public static object? OpenInputFile(object?[] args)
        {
            if (args.Length != 1 || args[0] == null)
            {
                throw new ArgumentException("open-input-file requires one argument");
            }

            string filename = args[0]!.ToString()!;

            try
            {
                return new StreamReader(filename);
            }
            catch (Exception ex)
            {
                throw new FileError(ex);
            }
        }

        public static object? OpenBinaryInputFile(object?[] args)
        {
            if (args.Length != 1 || args[0] == null)
            {
                throw new ArgumentException("open-binary-input-file requires one argument");
            }

            string filename = args[0]!.ToString()!;

            try
            {
                return new FileStream(filename, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                throw new FileError(ex);
            }
        }

        public static object? OpenOutputFile(object?[] args)
        {
            if (args.Length != 1 || args[0] == null)
            {
                throw new ArgumentException("open-output-file requires one argument");
            }

            string filename = args[0]!.ToString()!;

            try
            {
                return new StreamWriter(filename);
            }
            catch (Exception ex)
            {
                throw new FileError(ex);
            }
        }

        public static object? OpenBinaryOutputFile(object?[] args)
        {
            if (args.Length != 1 || args[0] == null)
            {
                throw new ArgumentException("open-binary-output-file requires one argument");
            }

            string filename = args[0]!.ToString()!;

            try
            {
                return new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception ex)
            {
                throw new FileError(ex);
            }
        }

        public static object? ClosePort(object?[] args)
        {
            if (args.Length != 1 || args[0] is not Stream or TextReader or TextWriter)
            {
                throw new ArgumentException("close-port requires one port argument");
            }

            if (args[0] is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return Nil.Value;
        }

        public static object? CloseInputPort(object?[] args)
        {
            if (args.Length != 1 || args[0] is not Stream or TextReader)
            {
                throw new ArgumentException("close-input-port requires one input port argument");
            }

            if (args[0] is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return Nil.Value;
        }

        public static object? CloseOutputPort(object?[] args)
        {
            if (args.Length != 1 || args[0] is not Stream or TextWriter)
            {
                throw new ArgumentException("close-output-port requires one output port argument");
            }

            if (args[0] is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return Nil.Value;
        }

        public static object? OpenInputString(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("open-input-string requires one string argument");
            }

            if (args[0] is not string str)
            {
                if (args[0] is not StringBuilder sb)
                {
                    throw new ArgumentException("open-input-string requires one string argument");
                }

                str = sb.ToString();
            }

            return new StringReader(str);
        }

        public static object? OpenInputBytevector(object?[] args)
        {
            if (args.Length != 1 || args[0] is not Bytevector bv)
            {
                throw new ArgumentException("open-input-bytevector requires one bytevector argument");
            }

            return new MemoryStream(bv.ToByteArray(), false);
        }

        public static object? OpenOutputBytevector(object?[] args)
        {
            return new MemoryStream();
        }

        public static object? GetOutputBytevector(object?[] args)
        {
            if (args.Length != 1 || args[0] is not MemoryStream ms)
            {
                throw new ArgumentException("get-output-bytevector requires one argument that must be created with open-output-bytevector");
            }

            return new Bytevector(ms.ToArray());
        }

        public static object? IsEofObject(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("eof-object? requires one argument");
            }

            return args[0] is EofObject;
        }

        public static object? GetEofObject(object?[] args)
        {
            return EofObject.Instance;
        }
    }
}
