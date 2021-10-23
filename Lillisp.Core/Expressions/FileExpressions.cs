using System;
using System.IO;
using System.Text;

namespace Lillisp.Core.Expressions
{
    public static class FileExpressions
    {
        public static object? FileExists(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("file-exists? requires one argument");
            }

            return File.Exists(args[0]?.ToString());
        }

        public static object? DeleteFile(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("delete-file requires one argument");
            }

            if (args[0] is not string filename)
            {
                if (args[0] is not StringBuilder sb)
                {
                    throw new ArgumentException("delete-file's first argument must be a string");
                }

                filename = sb.ToString();
            }

            if (!File.Exists(filename))
            {
                throw new FileError(new FileNotFoundException("The specified file was not found.", filename));
            }

            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                throw new FileError(ex);
            }

            return Nil.Value;
        }
    }
}
