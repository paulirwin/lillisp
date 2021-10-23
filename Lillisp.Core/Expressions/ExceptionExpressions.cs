using System;
using System.IO;
using System.Linq;

namespace Lillisp.Core.Expressions
{
    public static class ExceptionExpressions
    {
        public static object? Raise(object?[] args)
        {
            // TODO: should this be moved to a C# macro so that it can capture the stack trace?
            if (args.Length == 0)
            {
                throw new RaisedException(null);
            }

            if (args.Length > 1)
            {
                throw new ArgumentException("raise requires zero or one arguments");
            }

            throw new RaisedException(args[0]);
        }

        public static object? Error(object?[] args)
        {
            // TODO: should this be moved to a C# macro so that it can capture the stack trace?
            if (args.Length == 0)
            {
                throw new ErrorException(null);
            }

            if (args.Length == 1)
            {
                throw new ErrorException(args[0]?.ToString());
            }

            throw new ErrorException(args[0]?.ToString(), args.Skip(1));
        }

        /// <summary>
        /// Determines if the argument is an error object (Exception).
        /// </summary>
        /// <remarks>
        /// Scheme R7RS says: "Returns #t if obj is an object created by error or one
        /// of an implementation-defined set of objects." I'm taking this to mean that
        /// this implementation can include the set of all Exceptions (of which
        /// ErrorException is one).
        /// </remarks>
        /// <param name="args">The arguments.</param>
        /// <returns>Boolean</returns>
        public static object? ErrorObject(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("error-object? requires one argument");
            }

            return args[0] is Exception;
        }

        public static object? ErrorObjectMessage(object?[] args)
        {
            if (args.Length != 1 || args[0] is not Exception ex)
            {
                throw new ArgumentException("error-object-message requires one error object argument");
            }

            return ex.Message;
        }

        public static object? ErrorObjectIrritants(object?[] args)
        {
            if (args.Length != 1 || args[0] is not Exception ex)
            {
                throw new ArgumentException("error-object-message requires one error object argument");
            }

            if (args[0] is not ErrorException errorException)
            {
                return Array.Empty<object?>();
            }

            return errorException.Irritants;
        }

        public static object? FileError(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("file-error? requires one argument");
            }

            return args[0] is Core.FileError or IOException;
        }
    }
}
