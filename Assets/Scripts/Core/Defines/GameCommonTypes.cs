using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

namespace GameCommonTypes
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit), System.Serializable]
    public readonly struct UniqueKey64 : IComparable, IConvertible, IComparable<UniqueKey64>, IEquatable<UniqueKey64>
    {
        #region Constructor
        public UniqueKey64(uint uppder, uint lower)
        {
            _raw = 0;

            _upper = uppder;
            _lower = lower;
        }
        public UniqueKey64(ulong raw)
        {
            _upper = 0;
            _lower = 0;

            _raw = raw;
        }
        #endregion

        #region Getter
        public uint Upper { get {return _upper;} }
        public uint Lower { get{return _lower;} }
        public ulong Raw { get{return _raw;}}
        public override string ToString()
        {
            return $"({_upper},{_lower} : row{_raw})";
        }
        public override int GetHashCode()
        {
            return _raw.GetHashCode();
        }
        public bool isValid()
        {
            return 0 != _raw;
        }
        #endregion

        #region Interface
        public int CompareTo(object obj)
        {
            return Raw.CompareTo(obj);
        }

        public TypeCode GetTypeCode()
        {
            return Raw.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return 0 != Raw;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)Raw;
        }

        public char ToChar(IFormatProvider provider)
        {
            return '0';
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return new DateTime();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Raw;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Raw;
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)Raw;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)Raw;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (long)Raw;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)Raw;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Raw;
        }

        public string ToString(IFormatProvider provider)
        {
            return $"({_upper},{_lower} : row{_raw})";
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return null;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)Raw;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint)Raw;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Raw;
        }

        public int CompareTo(UniqueKey64 other)
        {
            return Raw.CompareTo(other.Raw);
        }

        public bool Equals(UniqueKey64 other)
        {
            return Raw.Equals(other.Raw);
        }
        #endregion

        #region Operator
        // public static implicit operator ulong(UniqueKey64 key) => key.Raw;
        // public static implicit operator UniqueKey64(ulong key) => new UniqueKey64(key);
        #endregion

        [JsonConstructor]
        public UniqueKey64(uint lower, uint upper, ulong raw)
        {
            _lower = lower;
            _upper = upper;
            _raw = raw;
        }

        public static UniqueKey64 Undefinde = new UniqueKey64();

        #region field
        [System.Runtime.InteropServices.FieldOffset(0)]
        private readonly uint _lower;
        [System.Runtime.InteropServices.FieldOffset(4)]
        private readonly uint _upper;
        [System.Runtime.InteropServices.FieldOffset(0)]
        private readonly ulong _raw;
        #endregion
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit), System.Serializable]
    public readonly struct UniqueKey32 : IComparable, IConvertible, IComparable<UniqueKey32>, IEquatable<UniqueKey32>
    {
        #region Constructor
        public UniqueKey32(ushort uppder, ushort lower)
        {
            _raw = 0;

            _upper = uppder;
            _lower = lower;
        }

        public UniqueKey32(uint raw)
        {
            _upper = 0;
            _lower = 0;

            _raw = raw;
        }
        #endregion

        #region Getter
        public ushort Upper { get {return _upper;} }
        public ushort Lower { get{return _lower;} }
        public uint Raw { get{return _raw;}}

        public override string ToString()
        {
            return $"({_upper},{_lower} : row{_raw})";
        }
        public override int GetHashCode()
        {
            return _raw.GetHashCode();
        }
        #endregion

        #region Interface
        public int CompareTo(object obj)
        {
            return Raw.CompareTo(obj);
        }

        public TypeCode GetTypeCode()
        {
            return Raw.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return 0 != Raw;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)Raw;
        }

        public char ToChar(IFormatProvider provider)
        {
            return '0';
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return new DateTime();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Raw;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Raw;
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)Raw;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)Raw;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Raw;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)Raw;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Raw;
        }

        public string ToString(IFormatProvider provider)
        {
            return $"({_upper},{_lower} : row{_raw})";
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return null;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)Raw;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Raw;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Raw;
        }

        public int CompareTo(UniqueKey32 other)
        {
            return Raw.CompareTo(other.Raw);
        }

        public bool Equals(UniqueKey32 other)
        {
            return Raw.Equals(other.Raw);
        }
        #endregion

        #region Operator
        public static implicit operator uint(UniqueKey32 key) { return key.Raw; }
        #endregion

        #region field
        [System.Runtime.InteropServices.FieldOffset(0)]
        private readonly ushort _upper;
        [System.Runtime.InteropServices.FieldOffset(2)]
        private readonly ushort _lower;
        [System.Runtime.InteropServices.FieldOffset(0)]
        private readonly uint _raw;
        #endregion
    }
}
