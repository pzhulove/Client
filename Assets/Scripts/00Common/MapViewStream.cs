using System;
using System.IO;
//Create Time : 2018-4-18
//Author By: Shen si
//Description : 其原理类似于内存文件映射 MemoryMappedFile 
//Memo: 不太清楚 直接用MemoryMappedFile在unity中实际表现如何，所以采用保守的做法，遂自己实现了一个
public class MapViewStream
{
    byte[] mBuffer = null;
    int mCurFileOffset = 0;
    int mCurUsedMemoCount = 0;
    string mFileName = string.Empty; 
    FileAccessMode mOpenMode = FileAccessMode.NONE;
    public enum FileAccessMode
    {
        Write,
        Read,
        NONE,
    };
    public int Length { get { return 0; } }
    public MapViewStream(string fileName,byte[] buffer, FileAccessMode eMode, int fileOffset = 0)
    {
        if(buffer == null)
        {
            throw new Exception("MapViewStream buffer can not be empty!");
        }
        mFileName = fileName;
        mBuffer = buffer;
        mOpenMode = eMode;
        mCurFileOffset = fileOffset;
        if (mOpenMode == FileAccessMode.Read)
        {
            if(!_FlushToBuffer())
            {
                throw new Exception("first _FlushToFile occur error");
            }
        }
        else if (fileOffset == 0 && File.Exists(mFileName))
        {
            File.Delete(mFileName);
        }
    }
    private bool _FlushToFile()
    {
        if (mFileName.Equals(string.Empty)) return false;
        FileStream fs = null;
        BinaryWriter sw = null;
        try
        {
            if (!File.Exists(mFileName))
            {
                fs = new FileStream(mFileName, FileMode.Create, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(mFileName, FileMode.Append, FileAccess.Write);
            }
            sw = new BinaryWriter(fs);
            sw.Seek(0, SeekOrigin.End);
            sw.Write(mBuffer, 0, mCurUsedMemoCount);
            sw.Flush();
            sw.Close();
            fs.Close();
            mCurFileOffset += mCurUsedMemoCount;
            mCurUsedMemoCount = 0;
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("_FlushToFile Failed {0}", e.ToString());
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            return false;
        }
        return true;
    }
    private bool _FlushToBuffer()
    {
        if (mFileName.Equals(string.Empty)) return false;
        if (!File.Exists(mFileName)) return false;
        FileStream fs = null;
        fs = new FileStream(mFileName, FileMode.Open, FileAccess.Read);
        if(mCurFileOffset >= fs.Length)
        {
            fs.Close();
            return false;
        }
        BinaryReader sr = new BinaryReader(fs);
        sr.BaseStream.Seek(mCurFileOffset, SeekOrigin.Begin);
        long validLength = fs.Length - mCurFileOffset;
        int bufferLength = mBuffer.Length;
        Array.Clear(mBuffer,0, bufferLength);
        if(bufferLength >= validLength)
        {
            sr.Read(mBuffer, 0, (int)validLength);
            mCurFileOffset += (int)validLength;
        }
        else
        {
            sr.Read(mBuffer, 0, bufferLength);
            mCurFileOffset += bufferLength;
        }
        sr.Close();
        fs.Close();
        mCurUsedMemoCount = 0;
        return true;
    }
    public bool write(byte value)
    {
        bool ret = true;
        if (mCurUsedMemoCount >= mBuffer.Length)
        {
            ret = _FlushToFile();
        }
        if (!ret)
        {
            throw new Exception("_FlushToFile occur error");
            //return false;
        }
        mBuffer[mCurUsedMemoCount] = value;
        mCurUsedMemoCount++;
        return true;
    }
    public bool read(ref byte value)
    {
        bool ret = true;
        if (mCurUsedMemoCount >= mBuffer.Length)
        {
            ret = _FlushToBuffer();
        }
        if (!ret)
        {
            throw new Exception("_FlushToBuffer occur error");
            //return false;
        }
        value = mBuffer[mCurUsedMemoCount];
        mCurUsedMemoCount++;
        return true;

    }
    public bool encode_int8(byte val)
    {
        return write(val);
    }

    public bool decode_int8(ref byte val)
    {
        return read(ref val);
    }

    public bool encode_uint16(UInt16 val)
    {
        bool ret = false;
        ret = write((byte)(val & 0xff));
        ret &= write((byte)((val >> 8) & 0xff));
        return ret;
    }

    public bool decode_uint16(ref UInt16 val)
    {
        bool ret = true;
        val = 0;
        UInt16 mask = 0;
        byte curVal = 0;
        for (int i = 0; i < 2; i++)
        {
            ret &= read(ref curVal);
            UInt16 tmp = (UInt16)curVal;
            val |= (UInt16)(tmp << mask);
            mask += 8;
        }
        return ret;
    }

    public bool encode_int16(Int16 val)
    {
        return encode_uint16((UInt16)val);
    }

    public bool decode_int16(ref Int16 val)
    {
        UInt16 tmp = 0;
        bool ret = decode_uint16(ref tmp);
        val = (Int16)tmp;
        return ret;
    }

    public bool encode_uint32(UInt32 val)
    {
        bool ret = true;
        for (int i = 0; i < 4; i++)
        {
            ret &= write((byte)(val & 0xff));
            val = val >> 8;
        }

        return ret;
    }

    public bool decode_uint32(ref UInt32 val)
    {
        val = 0;
        int mask = 0;
        byte tmpByte = 0;
        UInt32 tmp;
        bool ret = true;
        for (int i = 0; i < 4; i++)
        {
            ret &= read(ref tmpByte);
            tmp = (UInt32)tmpByte;
            val |= (UInt32)(tmp << mask);
            mask += 8;
        }

        return ret;
    }

    public bool encode_int32(Int32 val)
    {
        return encode_uint32((UInt32)val);
    }

    public bool decode_int32(ref Int32 val)
    {
        UInt32 tmp = 0;
        bool ret = decode_uint32( ref tmp);
        val = (Int32)tmp;
        return ret;
    }

    public bool encode_uint64(UInt64 val)
    {
        bool bRet = true;
        for (int i = 0; i < 8; i++)
        {
            bRet &= write((byte)(val & 0xff));
            val = val >> 8;
        }

        return bRet;
    }

    public bool decode_uint64(ref UInt64 val)
    {
        val = 0;
        int mask = 0;
        byte tempByte = 0;
        bool bRet = true;
        for (int i = 0; i < 8; i++)
        {
            bRet = read(ref tempByte);
            UInt64 tmp = (UInt64)tempByte;
            val |= (UInt64)(tmp << mask);
            mask += 8;
        }

        return bRet;
    }

    public bool encode_int64(Int64 val)
    {
        return encode_uint64((UInt64)val);
    }

    public bool decode_int64(ref Int64 val)
    {
        UInt64 tmp = 0;
        bool ret = false;
        ret = decode_uint64(ref tmp);
        val = (Int64)tmp;
        return ret;
    }

    public bool encode_string(string str)
    {
        //���ﲻ����\0
        byte[] buffer = StringHelper.StringToUTF8Bytes(str);
        ushort len = 0;
        if (buffer != null && buffer.Length > 0)
            len = (ushort)(buffer.Length - 1);
        bool bRet = true;

        //Logger.LogErrorFormat("len = {0}", len);
        bRet = encode_uint16(len);
        for (ushort i = 0; i < len; i++)
        {
            bRet &= encode_int8(buffer[i]);
        }
        return bRet;
    }

    public bool encode_string(byte[] buffer)
    {
        //���ﲻ����\0
        ushort len = 0;
        if (buffer != null && buffer.Length > 0)
            len = (ushort)(buffer.Length - 1);
        bool bRet = true;

        //Logger.LogErrorFormat("len = {0}", len);
        bRet = encode_uint16(len);
        for (ushort i = 0; i < len; i++)
        {
            bRet &= encode_int8(buffer[i]);
        }
        return bRet;
    }

    public bool decode_string(ref string str)
    {
        ushort len = 0;
       
        bool bRet = decode_uint16(ref len);
        byte[] buffer = new byte[len];
        byte temp = 0;
        for(int i = 0; i < len;i++)
        {
            bRet &= decode_int8(ref temp);
            buffer[i] = temp;
        }
        str = StringHelper.UTF8BytesToString(ref buffer);
        return bRet;
    }
    public bool decode_string(byte[] buffer)
    {
        ushort len = 0;

        bool bRet = decode_uint16(ref len);
        buffer = new byte[len];
        byte temp = 0;
        for (int i = 0; i < len; i++)
        {
            bRet &= decode_int8(ref temp);
            buffer[i] = temp;
        }
        return bRet;
    }
    public bool Save()
    {
        if (mOpenMode != FileAccessMode.Write) return false;
        if (mCurUsedMemoCount <= 0) return false;
        FileStream fs = null;
        BinaryWriter sw = null;
        try
        {
            if (!File.Exists(mFileName))
            {
                fs = new FileStream(mFileName, FileMode.Create, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(mFileName, FileMode.Append, FileAccess.Write);
            }
            sw = new BinaryWriter(fs);
            sw.Seek(mCurFileOffset, SeekOrigin.Begin);
            sw.Write(mBuffer, 0, mCurUsedMemoCount);
            sw.Flush();
            sw.Close();
            fs.Close();
            return true;
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("[MapViewStream] save file failed Exception {0}", e.Message);
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            return false;
        }
    }
}

