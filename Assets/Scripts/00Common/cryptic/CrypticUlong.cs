using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[Serializable]
public struct CrypticUlong : IFormattable, IConvertible, IComparable, IComparable<CrypticUlong>, IEquatable<CrypticUlong>
{
    [SerializeField]
    private ulong cryptoKey;

    [SerializeField]
    private ulong hiddenValue;

    [SerializeField]
    private ulong fakeValue;

    [SerializeField]
    private bool inited;

    public CrypticUlong(ulong nValue)
    {
        cryptoKey = 544443L;
        hiddenValue = Encrypt(nValue, cryptoKey);
        fakeValue = nValue;
        inited = true;
    }

    private static ulong Encrypt(ulong value, ulong key)
    {
        return value ^ key;
    }

    private static ulong Decrypt(ulong value, ulong key)
    {
        return value ^ key;
    }

    private ulong InternalDecrypt()
    {
        if (!inited)
        {
            cryptoKey = 544443L;
            hiddenValue = Encrypt(0, cryptoKey);
            fakeValue = 0;
            inited = true;
        }

        ulong decrypted = Decrypt(hiddenValue, cryptoKey);
        if (decrypted != fakeValue)
        {
//            Debug.LogError("Cheating!!!");
            CheatMonitor.instance.OnCheatingDetected();
            return 0;
        }

        return decrypted;
    }

    public static implicit operator CrypticUlong(ulong nValue)
    {
        return new CrypticUlong(nValue);
    }

    public static implicit operator ulong(CrypticUlong inData)
    {
        return inData.InternalDecrypt();
    }

    public static explicit operator int(CrypticUlong inData)
    {
        return (int)inData.InternalDecrypt();
    }

    public static explicit operator uint(CrypticUlong inData)
    {
        return (uint)inData.InternalDecrypt();
    }

    public static explicit operator ushort(CrypticUlong inData)
    {
        return (ushort)inData.InternalDecrypt();
    }

    public static explicit operator float(CrypticUlong inData)
    {
        return (float)inData.InternalDecrypt();
    }

    public static CrypticUlong operator ++(CrypticUlong inData)
    {
        ulong decrypted = inData.InternalDecrypt() + 1;
        inData.hiddenValue = Encrypt(decrypted, inData.cryptoKey);
        inData.fakeValue = decrypted;
        return inData;
    }

    public static CrypticUlong operator --(CrypticUlong inData)
    {
        ulong decrypted = inData.InternalDecrypt() - 1;
        inData.hiddenValue = Encrypt(decrypted, inData.cryptoKey);
        inData.fakeValue = decrypted;
        return inData;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is CrypticUlong))
            return false;

        return Equals((CrypticUlong)obj);
    }

    public bool Equals(CrypticUlong obj)
    {
        if (cryptoKey == obj.cryptoKey)
        {
            return hiddenValue == obj.hiddenValue;
        }

        return Decrypt(hiddenValue, cryptoKey) == Decrypt(obj.hiddenValue, obj.cryptoKey);
    }

    public override int GetHashCode()
    {
        return InternalDecrypt().GetHashCode();
    }

    public override string ToString()
    {
        return InternalDecrypt().ToString();
    }

    public string ToString(string format)
    {
        return InternalDecrypt().ToString(format);
    }

    public string ToString(IFormatProvider provider)
    {
        return InternalDecrypt().ToString(provider);
    }

    public string ToString(string format, IFormatProvider provider)
    {
        return InternalDecrypt().ToString(format, provider);
    }

    public int CompareTo(CrypticUlong other)
    {
        return InternalDecrypt().CompareTo(other.InternalDecrypt());
    }

    public int CompareTo(int other)
    {
        return InternalDecrypt().CompareTo(other);
    }

    public int CompareTo(object obj)
    {
        if (!(obj is CrypticUlong))
        {
            throw new ArgumentException("obj is not CrypticUlong");
        }
        return CompareTo((CrypticUlong)obj);
    }

    public TypeCode GetTypeCode()
    {
        return InternalDecrypt().GetTypeCode();
    }

    public bool ToBoolean(IFormatProvider provider)
    {
        return Convert.ToBoolean(InternalDecrypt(), provider);
    }

    public byte ToByte(IFormatProvider provider)
    {
        return Convert.ToByte(InternalDecrypt(), provider);
    }

    public char ToChar(IFormatProvider provider)
    {
        return Convert.ToChar(InternalDecrypt(), provider);
    }

    public DateTime ToDateTime(IFormatProvider provider)
    {
        throw new InvalidCastException();
    }

    public decimal ToDecimal(IFormatProvider provider)
    {
        return Convert.ToDecimal(InternalDecrypt(), provider);
    }

    public double ToDouble(IFormatProvider provider)
    {
        return Convert.ToDouble(InternalDecrypt(), provider);
    }

    public short ToInt16(IFormatProvider provider)
    {
        return Convert.ToInt16(InternalDecrypt(), provider);
    }

    public int ToInt32(IFormatProvider provider)
    {
        return Convert.ToInt32(InternalDecrypt(), provider);
    }

    public long ToInt64(IFormatProvider provider)
    {
        return Convert.ToInt64(InternalDecrypt(), provider);
    }

    public sbyte ToSByte(IFormatProvider provider)
    {
        return Convert.ToSByte(InternalDecrypt(), provider);
    }

    public float ToSingle(IFormatProvider provider)
    {
        return Convert.ToSingle(InternalDecrypt(), provider);
    }

    public object ToType(Type conversionType, IFormatProvider provider)
    {
        return Convert.ChangeType(InternalDecrypt(), conversionType, provider);
    }

    public ushort ToUInt16(IFormatProvider provider)
    {
        return Convert.ToUInt16(InternalDecrypt(), provider);
    }

    public uint ToUInt32(IFormatProvider provider)
    {
        return Convert.ToUInt32(InternalDecrypt(), provider);
    }

    public ulong ToUInt64(IFormatProvider provider)
    {
        return Convert.ToUInt64(InternalDecrypt(), provider);
    }
}

