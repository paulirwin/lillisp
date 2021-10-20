using System;
using System.Linq;

namespace Lillisp.Core.Expressions
{
    public static class BytevectorExpressions
    {
        public static object? MakeBytevector(object?[] args)
        {
            if (args.Length is 0 or > 2)
            {
                throw new ArgumentException("make-bytevector requires at least one but no more than two arguments");
            }

            int count = Convert.ToInt32(args[0]);
            byte defaultValue = 0;

            if (args.Length == 2)
            {
                defaultValue = Convert.ToByte(args[1]);
            }

            return new Bytevector(Enumerable.Repeat(defaultValue, count));
        }

        public static object? Bytevector(object?[] args)
        {
            return new Bytevector(args.Select(Convert.ToByte));
        }

        public static object? BytevectorLength(object?[] args)
        {
            if (args.Length != 1 || args[0] is not Bytevector bv)
            {
                throw new ArgumentException("bytevector-length requires one bytevector argument");
            }

            return bv.Count;
        }

        public static object? BytevectorU8Ref(object?[] args)
        {
            if (args.Length != 2 || args[0] is not Bytevector bv)
            {
                throw new ArgumentException("bytevector-u8-ref requires a bytevector argument and a position");
            }

            var k = Convert.ToInt32(args[1]);

            return bv[k];
        }

        public static object? BytevectorU8Set(object?[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("bytevector-u8-set! requires three arguments");
            }

            if (args[0] is not Bytevector bv)
            {
                throw new ArgumentException("bytevector-u8-set!'s first argument must be a bytevector");
            }

            var k = Convert.ToInt32(args[1]);
            var b = Convert.ToByte(args[2]);

            bv[k] = b;

            return b; // TODO: is this correct?
        }

        public static object? BytevectorCopy(object?[] args)
        {
            if (args.Length is 0 or > 3)
            {
                throw new ArgumentException("bytevector-copy requires 1 to 3 arguments");
            }

            if (args[0] is not Bytevector bv)
            {
                throw new ArgumentException("bytevector-copy's first argument must be a bytevector");
            }

            int start = 0, end = bv.Count;

            if (args.Length > 1)
            {
                start = Convert.ToInt32(args[1]);
            }

            if (args.Length == 3)
            {
                end = Convert.ToInt32(args[2]);
            }

            return new Bytevector(bv.Skip(start).Take(end - start));
        }

        public static object? BytevectorCopyTo(object?[] args)
        {
            if (args.Length is < 3 or > 5)
            {
                throw new ArgumentException("bytevector-copy! requires 3 to 5 arguments");
            }

            if (args[0] is not Bytevector to)
            {
                throw new ArgumentException("bytevector-copy!'s first argument must be a bytevector");
            }

            if (args[2] is not Bytevector from)
            {
                throw new ArgumentException("bytevector-copy!'s third argument must be a bytevector");
            }

            var at = Convert.ToInt32(args[1]);
            int start = 0, end = from.Count;

            if (args.Length > 3)
            {
                start = Convert.ToInt32(args[3]);
            }

            if (args.Length == 5)
            {
                end = Convert.ToInt32(args[4]);
            }

            if ((to.Count - at) < (end - start))
            {
                throw new ArgumentException("(- (bytevector-length to) at) must not be less than (- end start)");
            }

            for (int i = start; i < end; i++)
            {
                to[at++] = from[i];
            }

            return Nil.Value; // TODO: is this correct?
        }

        public static object? BytevectorAppend(object?[] args)
        {
            if (args.Length == 0)
            {
                return new Bytevector();
            }

            return new Bytevector(args.Cast<Bytevector>().SelectMany(i => i));
        }
    }
}
