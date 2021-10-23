﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lillisp.Core.Expressions
{
    public static class TypeExpressions
    {
        public static object? TypeOf(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("typeof requires one argument");
            }

            if (args[0] is null)
            {
                return null;
            }

            if (args[0] is Type type)
            {
                return type;
            }

            return args[0].GetType();
        }

        public static object? Cast(object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("cast requires two arguments");
            }

            if (args[0] is null)
            {
                return null;
            }

            if (args[1] is not Type type)
            {
                throw new ArgumentException("Second parameter must be a Type");
            }

            return Convert.ChangeType(args[0], type);
        }

        public static object? IsBoolean(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("boolean? requires one argument");
            }

            return args[0] is bool;
        }

        public static object? IsChar(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("char? requires one argument");
            }

            return args[0] is char;
        }

        public static object? IsNull(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("null? requires one argument");
            }

            return args[0] is Nil;
        }

        public static object? IsNumber(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("number? requires one argument");
            }

            return args[0].IsNumber();
        }

        public static object? IsString(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("string? requires one argument");
            }

            return args[0] is string;
        }

        public static object? IsPair(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("pair? requires one argument");
            }

            return args[0] is not Nil && args[0] is Pair or object?[]; // HACK: eventually remove "or object?[]"
        }

        public static object? IsProcedure(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("procedure? requires one argument");
            }

            return args[0] is Delegate or Procedure; // TODO: this is most certainly not correct or exhaustive
        }

        public static object? IsSymbol(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("symbol? requires one argument");
            }
            
            return args[0] is Symbol;
        }

        public static object? IsVector(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("vector? requires one argument");
            }

            return args[0] is Vector;
        }

        public static object? CharacterToInteger(object?[] args)
        {
            if (args.Length == 0 || args[0] is not char c)
            {
                throw new ArgumentException("char->integer requires one char argument");
            }

            return (int)c;
        }

        public static object? IntegerToCharacter(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("integer->char requires one integer argument");
            }

            int value = Convert.ToInt32(args[0]);

            return (char)value;
        }

        public static object? StringToList(object?[] args)
        {
            if (args.Length is 0 or > 3)
            {
                throw new ArgumentException("string->list requires one to three arguments");
            }

            if (args[0] is not string str)
            {
                if (args[0] is StringBuilder sb)
                {
                    str = sb.ToString();
                }
                else
                {
                    throw new ArgumentException("string->list's first argument must be a string");
                }
            }

            int start = 0, end = str.Length;

            if (args.Length > 1)
            {
                start = Convert.ToInt32(args[1]);
            }

            if (args.Length == 3)
            {
                end = Convert.ToInt32(args[2]);
            }

            return str[start..end].Cast<object>().ToArray();
        }

        public static object? ListToString(object?[] args)
        {
            if (args.Length is 0 or > 1 || args[0] is not IEnumerable<object?> list)
            {
                throw new ArgumentException("list->string requires one list argument");
            }

            return new string(list.Cast<char>().ToArray());
        }

        public static object? IsPromise(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("promise? requires one argument");
            }

            return args[0] is Lazy<object?> or Task<object?>;
        }

        public static object? IsComplex(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("complex? requires one argument");
            }

            return args[0].IsComplex();
        }

        public static object? IsReal(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("real? requires one argument");
            }

            return args[0].IsRealNumber();
        }

        public static object? IsRational(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("rational? requires one argument");
            }

            return args[0].IsRationalNumber();
        }

        public static object? IsInteger(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("number? requires one argument");
            }

            return args[0].IsInteger();
        }

        public static object? IsBytevector(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("bytevector? requires one argument");
            }

            return args[0] is Bytevector; // TODO: should we support byte[] here too?
        }

        public static object? Utf8ToString(object?[] args)
        {
            if (args.Length is 0 or > 3)
            {
                throw new ArgumentException("utf8->string requires one to three arguments");
            }

            if (args[0] is not Bytevector bv)
            {
                throw new ArgumentException("utf8->string's first argument must be a bytevector");
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

            var slice = bv[start..end];

            // TODO: reduce extra allocations here
            return Encoding.UTF8.GetString(slice.ToByteArray());
        }

        public static object? StringToUtf8(object?[] args)
        {
            if (args.Length is 0 or > 3)
            {
                throw new ArgumentException("string->utf8 requires one to three arguments");
            }

            if (args[0] is not string str)
            {
                if (args[0] is StringBuilder sb)
                {
                    str = sb.ToString();
                }
                else
                {
                    throw new ArgumentException("string->utf8's first argument must be a string");
                }
            }

            int start = 0, end = str.Length;

            if (args.Length > 1)
            {
                start = Convert.ToInt32(args[1]);
            }

            if (args.Length == 3)
            {
                end = Convert.ToInt32(args[2]);
            }

            return new Bytevector(Encoding.UTF8.GetBytes(str[start..end]));
        }

        public static object? IsPort(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("port? requires one argument");
            }

            return args[0] is Stream or TextReader or TextWriter;
        }
    }
}