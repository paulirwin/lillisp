using System;
using System.Collections.Generic;

namespace Lillisp.Core.Syntax
{
    public class Atom : Node, IConvertible
    {
        public Atom(AtomType atomType, object? value)
            : base(NodeType.Atom)
        {
            AtomType = atomType;
            Value = value;
        }

        public AtomType AtomType { get; }

        public object? Value { get; }

        public override string ToString() => Value?.ToString() ?? "null";

        public TypeCode GetTypeCode()
        {
            return Value switch
            {
                null => TypeCode.Empty,
                IConvertible convertible => convertible.GetTypeCode(),
                _ => TypeCode.Object,
            };
        }

        public bool ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(Value, provider);

        public byte ToByte(IFormatProvider? provider) => Convert.ToByte(Value, provider);

        public char ToChar(IFormatProvider? provider) => Convert.ToChar(Value, provider);

        public DateTime ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(Value, provider);

        public decimal ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(Value, provider);

        public double ToDouble(IFormatProvider? provider) => Convert.ToDouble(Value, provider);

        public short ToInt16(IFormatProvider? provider) => Convert.ToInt16(Value, provider);

        public int ToInt32(IFormatProvider? provider) => Convert.ToInt32(Value, provider);

        public long ToInt64(IFormatProvider? provider) => Convert.ToInt64(Value, provider);

        public sbyte ToSByte(IFormatProvider? provider) => Convert.ToSByte(Value, provider);

        public float ToSingle(IFormatProvider? provider) => Convert.ToSingle(Value, provider);

        public string ToString(IFormatProvider? provider) => Convert.ToString(Value, provider)!;

        public object ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(Value, conversionType, provider)!;

        public ushort ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(Value, provider);

        public uint ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(Value, provider);

        public ulong ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(Value, provider);
    }
}