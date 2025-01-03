using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;

public class UdpDLL
{

#if !UNITY_EDITOR && UNITY_IPHONE
    const string udpDLL = "__Internal";
#else
#if DEBUG
    const string udpDLL = "enet";
#else
    const string udpDLL = "enet";
#endif
#endif

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr avalon_udpconnector_initialize(byte[] logFileName);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int avalon_udpconnector_connect(IntPtr network, byte[] ip, UInt16 port, UInt32 uid, UInt32 timeout);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int avalon_udpconnector_senddata(IntPtr network, byte[] buff, UInt32 buffSize, byte reliable);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int avalon_udpconnector_checkdata(IntPtr network, byte[] data, ref UInt32 dataLength);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int avalon_udpconnector_disconnect(IntPtr network, UInt32 uid);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int avalon_udpconnector_deinitialize(IntPtr network);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int avalon_udpconnector_ping(IntPtr network);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void avalon_udpconnector_save_log(byte[] logFileName);

    [DllImport(udpDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void avalon_udpconnector_open_log(byte[] logFileName);
}
