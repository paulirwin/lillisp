using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lillisp.Core
{
    public class Pair : Node, IEnumerable<Node>
    {
        public Pair()
            : this(Nil.Value, Nil.Value)
        {
        }

        public Pair(Node car, Node cdr)
        {
            Car = car;
            Cdr = cdr;
        }

        public Node Car { get; set; }

        public Node Cdr { get; set; }

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
                Node next = Cdr;

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
        public IEnumerator<Node> GetEnumerator()
        {
            return new PairEnumerator(this);
        }

        public override string ToString()
        {
            // TODO: can this be written in Lillisp instead?
            if (Cdr is Nil)
            {
                return $"({Car})";
            }

            if (!IsList)
            {
                return $"({Car} . {Cdr})";
            }

            var sb = new StringBuilder("(");
            sb.Append(Car);

            Node next = Cdr;

            while (next is Pair p)
            {
                sb.Append(' ');
                sb.Append(p.Car);
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

        private class PairEnumerator : IEnumerator<Node>
        {
            private readonly Pair _startPair;
            private Pair? _current;
            private Node? _currentNode;
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
                    _currentNode = _current.Car;
                    return true;
                }
                
                if (_current.Cdr is not Pair p)
                {
                    if (_current.Cdr is Nil)
                    {
                        return false;
                    }

                    _currentNode = _current.Cdr;
                    _pairStop = true;
                    return true;
                }

                _current = p;
                _currentNode = p.Car;
                return true;
            }

            public void Reset()
            {
                _current = null;
                _currentNode = null;
                _pairStop = false;
            }

            public Node Current => _currentNode ?? throw new InvalidOperationException("Must call MoveNext first");

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}