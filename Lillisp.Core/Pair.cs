﻿using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Lillisp.Core;

public class Pair : Node, IEnumerable<object?>
{
    public Pair()
        : this(Nil.Value, Nil.Value)
    {
    }

    public Pair(object? car, object? cdr)
    {
        Car = car;
        Cdr = cdr;
    }

    public object? Car { get; set; }

    public object? Cdr { get; set; }

    /// <summary>
    /// Determines if this pair is a proper list.
    /// </summary>
    /// <remarks>
    /// The first pass at this was to do a simple expression, i.e. => Cdr is Nil or Pair { IsList: true }.
    /// However, that would not be tail-recursive and could stack overflow.
    /// </remarks>
    public bool IsList
    {
        get
        {
            // TODO: can this be written in Lillisp instead?
            var next = Cdr;

            while (next is not Nil)
            {
                if (next is not Pair p)
                {
                    return false;
                }

                next = p.Cdr;
            }

            return true;
        }
    }

    [DebuggerStepThrough]
    public IEnumerator<object?> GetEnumerator()
    {
        return new PairEnumerator(this);
    }

    public override string ToString() => ToString(i => i?.ToString());
        
    public string ToString(Func<object?, string?> formatter)
    {
        if (Cdr is Nil)
        {
            return $"({formatter(Car)})";
        }

        if (!IsList)
        {
            return $"({formatter(Car)} . {formatter(Cdr)})";
        }

        var sb = new StringBuilder("(");
        sb.Append(formatter(Car));

        var next = Cdr;

        while (next is Pair p)
        {
            sb.Append(' ');
            sb.Append(formatter(p.Car));
            next = p.Cdr;
        }

        sb.Append(')');

        return sb.ToString();
    }

    [DebuggerStepThrough]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static Pair CastPair<T>(object car, object cdr) => new Pair(Convert.ChangeType(car, typeof(T)), Convert.ChangeType(cdr, typeof(T)));

    private class PairEnumerator : IEnumerator<object?>
    {
        private readonly Pair _startPair;
        private Pair? _current;
        private bool _pairStop;

        public PairEnumerator(Pair pair)
        {
            _startPair = pair;
        }

        [DebuggerStepThrough]
        public bool MoveNext()
        {
            if (_pairStop)
            {
                return false;
            }

            if (_current == null)
            {
                _current = _startPair;
                Current = _current.Car;
                return true;
            }
                
            if (_current.Cdr is not Pair p)
            {
                if (_current.Cdr is Nil)
                {
                    return false;
                }

                Current = _current.Cdr;
                _pairStop = true;
                return true;
            }

            _current = p;
            Current = p.Car;
            return true;
        }

        public void Reset()
        {
            _current = null;
            Current = null;
            _pairStop = false;
        }
        
        public object? Current { get; private set; }

        object? IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}