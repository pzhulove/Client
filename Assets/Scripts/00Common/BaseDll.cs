using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text;
using System.Security;

public class BaseDLL
{
#if UNITY_EDITOR

    public static int encode_int8(byte[] buf, ref int pos, byte val)
    {
        if (buf.Length <= pos) { return 0; }

        buf[pos++] = val;
        return 0;
    }

    public static int decode_int8(byte[] buf, ref int pos, ref byte val)
    {
        if (buf.Length <= pos) { return 0; }

        val = buf[pos++];
        return 0;
    }

    public static int encode_uint16(byte[] buf, ref int pos, UInt16 val)
    {
        if (buf.Length <= pos) { return 0; }
        buf[pos++] = (byte)(val & 0xff);
        buf[pos++] = (byte)((val >> 8) & 0xff);
        return 0;
    }

    public static int decode_uint16(byte[] buf, ref int pos, ref UInt16 val)
    {
        if (buf.Length <= pos) { return 0; }
        val = 0;
        UInt16 mask = 0;
        for (int i = 0; i < 2; i++)
        {
            UInt16 tmp = (UInt16)buf[pos++];
            val |= (UInt16)(tmp << mask);
            mask += 8;
        }

        return 0;
    }

    public static int encode_int16(byte[] buf, ref int pos, Int16 val)
    {
        return encode_uint16(buf, ref pos, (UInt16)val);
    }

    public static int decode_int16(byte[] buf, ref int pos, ref Int16 val)
    {
        UInt16 tmp = 0;
        decode_uint16(buf, ref pos, ref tmp);
        val = (Int16)tmp;
        return 0;
    }

    public static int encode_uint32(byte[] buf, ref int pos, UInt32 val)
    {
        if (buf.Length <= pos) { return 0; }
        for (int i = 0; i < 4; i++)
        {
            buf[pos++] = (byte)(val & 0xff);
            val = val >> 8;
        }

        return 0;
    }

    public static int decode_uint32(byte[] buf, ref int pos, ref UInt32 val)
    {
        if (buf.Length <= pos) { return 0; }
        val = 0;
        int mask = 0;
        for (int i = 0; i < 4; i++)
        {
            UInt32 tmp = (UInt32)buf[pos++];
            val |= (UInt32)(tmp << mask);
            mask += 8;
        }

        return 0;
    }

    public static int encode_int32(byte[] buf, ref int pos, Int32 val)
    {
        return encode_uint32(buf, ref pos, (UInt32)val);
    }

    public static int decode_int32(byte[] buf, ref int pos, ref Int32 val)
    {
        UInt32 tmp = 0;
        decode_uint32(buf, ref pos, ref tmp);
        val = (Int32)tmp;
        return 0;
    }

    public static int encode_uint64(byte[] buf, ref int pos, UInt64 val)
    {
        if (buf.Length <= pos) { return 0; }
        for (int i = 0; i < 8; i++)
        {
            buf[pos++] = (byte)(val & 0xff);
            val = val >> 8;
        }

        return 0;
    }

    public static int decode_uint64(byte[] buf, ref int pos, ref UInt64 val)
    {
        if (buf.Length <= pos) { return 0; }
        val = 0;
        int mask = 0;
        for (int i = 0; i < 8; i++)
        {
            UInt64 tmp = (UInt64)buf[pos++];
            val |= (UInt64)(tmp << mask);
            mask += 8;
        }

        return 0;
    }

    public static int encode_int64(byte[] buf, ref int pos, Int64 val)
    {
        return encode_uint64(buf, ref pos, (UInt64)val);
    }

    public static int decode_int64(byte[] buf, ref int pos, ref Int64 val)
    {
        if (buf.Length <= pos) { return 0; }
        UInt64 tmp = 0;
        decode_uint64(buf, ref pos, ref tmp);
        val = (Int64)tmp;
        return 0;
    }

    public static int encode_string(byte[] buf, ref int pos, byte[] str, UInt16 max_length)
    {
        //���ﲻ����\0
        ushort len = 0;
        if (str != null && str.Length > 0)
            len = (ushort)(str.Length - 1);
        //Logger.LogErrorFormat("len = {0}", len);
        encode_uint16(buf, ref pos, len);
        for (ushort i = 0; i < len; i++)
        {
            encode_int8(buf, ref pos, str[i]);
        }
        return 0;
    }

    public static int decode_string(byte[] buf, ref int pos, byte[] str, UInt16 max_length)
    {
        return 0;
    }
#else
#if !UNITY_EDITOR && UNITY_IPHONE
    const string baseDLL = "__Internal";
#else
    const string baseDLL = "base";
#endif
 
    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_int8(byte[] buf, ref int pos, byte val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_int8(byte[] buf, ref int pos, ref byte val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_uint16(byte[] buf, ref int pos, UInt16 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_uint16(byte[] buf, ref int pos, ref UInt16 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_int16(byte[] buf, ref int pos, Int16 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_int16(byte[] buf, ref int pos, ref Int16 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_uint32(byte[] buf, ref int pos, UInt32 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_uint32(byte[] buf, ref int pos, ref UInt32 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_int32(byte[] buf, ref int pos, Int32 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_int32(byte[] buf, ref int pos, ref Int32 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_uint64(byte[] buf, ref int pos, UInt64 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_uint64(byte[] buf, ref int pos, ref UInt64 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_int64(byte[] buf, ref int pos, Int64 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_int64(byte[] buf, ref int pos, ref Int64 val);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int encode_string(byte[] buf, ref int pos, byte[] str, UInt16 max_length);

    [DllImport(baseDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int decode_string(byte[] buf, ref int pos, byte[] str, UInt16 max_length);
#endif
    public static int encode_int8(MapViewStream st, ref int pos, byte val)
    {
        st.encode_int8(val);
        return 0;
    }

    public static int decode_int8(MapViewStream st, ref int pos, ref byte val)
    {
        st.decode_int8(ref val);
        return 0;
    }

    public static int encode_uint16(MapViewStream st, ref int pos, UInt16 val)
    {
        st.encode_uint16(val);
        return 0;
    }

    public static int decode_uint16(MapViewStream st, ref int pos, ref UInt16 val)
    {
        st.decode_uint16(ref val);
        return 0;
    }

    public static int encode_int16(MapViewStream st, ref int pos, Int16 val)
    {
        st.encode_int16(val);
        return 0;
    }

    public static int decode_int16(MapViewStream st, ref int pos, ref Int16 val)
    {
        st.decode_int16(ref val);
        return 0;
    }

    public static int encode_uint32(MapViewStream st, ref int pos, UInt32 val)
    {
        st.encode_uint32(val);
        return 0;
    }

    public static int decode_uint32(MapViewStream st, ref int pos, ref UInt32 val)
    {
        st.decode_uint32(ref val);
        return 0;
    }

    public static int encode_int32(MapViewStream st, ref int pos, Int32 val)
    {
        st.encode_int32(val);
        return 0;
    }

    public static int decode_int32(MapViewStream st, ref int pos, ref Int32 val)
    {
        st.decode_int32(ref val);
        return 0;
    }

    public static int encode_uint64(MapViewStream st, ref int pos, UInt64 val)
    {
        st.encode_uint64(val);
        return 0;
    }

    public static int decode_uint64(MapViewStream st, ref int pos, ref UInt64 val)
    {
        st.decode_uint64(ref val);
        return 0;
    }

    public static int encode_int64(MapViewStream st, ref int pos, Int64 val)
    {
        st.encode_int64(val);
        return 0;
    }

    public static int decode_int64(MapViewStream st, ref int pos, ref Int64 val)
    {
        st.decode_int64(ref val);
        return 0;
    }

    public static int encode_string(MapViewStream st, ref int pos, byte[] str, UInt16 max_length)
    {
        st.encode_string(str);
        return 0;
    }

    public static int decode_string(MapViewStream st, ref int pos, byte[] str, UInt16 max_length)
    {
        st.decode_string(str);
        return 0;
    }
}
