using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;


    public class Snappy
    {

#if !UNITY_EDITOR && UNITY_IPHONE
        const string dllName = "__Internal";
#else
        const string dllName = "snappy";
#endif
        [DllImport(dllName, EntryPoint = "snappy_compress", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 Compress(
                byte[] input,
                long inputLength,
                byte[] compressed,
                ref long compressedLength);

        [DllImport(dllName, EntryPoint = "snappy_uncompress", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 Uncompress(
            byte[] compressed,
            long compressedLength,
            byte[] uncompressed,
            ref long uncompressedLength);
    }

