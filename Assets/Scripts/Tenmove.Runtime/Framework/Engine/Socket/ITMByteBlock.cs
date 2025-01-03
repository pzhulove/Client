

using System;
using System.IO;

namespace Tenmove.Runtime
{
    public interface ITMByteBlock
    {
        uint Capacity { get; }
        uint Length { get; }
        uint Position { get; }
        void Seek(int offset, SeekOrigin origin);

        uint FromStream(Stream stream, uint dataBytes = 0);
        void ToStream(Stream stream, uint dataBytes = 0);

        uint FromBytes(byte[] bytes,uint offset = 0, uint length = 0);
        uint ToBytes(byte[] bytes, uint offset = 0, uint length = 0);

        bool ReadBool();
        short ReadInt16();
        ushort ReadUInt16();
        int ReadInt32();
        uint ReadUInt32();
        Int64 ReadInt64();
        UInt64 ReadUInt64();
        float ReadFloat32();
        double ReadFloat64();
        uint ReadChars(char[] chars);

        void Write(bool value);
        void Write(short value);
        void Write(ushort value);
        void Write(int value);
        void Write(uint value);
        void Write(Int64 value);
        void Write(UInt64 value);
        void Write(float value);
        void Write(double value);
        void Write(char[] value);

        void Recycle();
    }
}