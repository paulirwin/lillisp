﻿namespace Lillisp.Core;

public class ErrorException : Exception
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