using System;
using System.Collections.Generic;

namespace Lillisp.Core
{
    public sealed class ErrorException : Exception
    {
        public ErrorException(string? message)
            : base(message)
        {
        }

        public ErrorException(string? message, IEnumerable<object?> irritants)
            : base(message)
        {
            foreach (var irritant in irritants)
            {
                Irritants.Add(irritant);
            }
        }

        public IList<object?> Irritants { get; } = new List<object?>();
    }
}
