using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PackHeaderItemInfo
{
    private int nFileIndex;
    private int nPeekStart;
    private int nPeekLength;

    public int _FileIndex
    {
        get
        {
            return this.nFileIndex;
        }
        set
        {
            this.nFileIndex = value;
        }
    }

    public int _nPeekStart
    {
        get
        {
            return this.nPeekStart;
        }
    }
    public int _nPeekLength
    {
        get
        {
            return this.nPeekLength;
        }
    }

    public PackHeaderItemInfo(int nFileIndex, int nPeekStart, int nPeekLength)
    {
        this.nFileIndex = nFileIndex;
        this.nPeekStart = nPeekStart;
        this.nPeekLength = nPeekLength;
    }

    public bool Equal(PackHeaderItemInfo _other)
    {
        return this.nFileIndex == _other.nFileIndex
            && this.nPeekStart == _other.nPeekStart
            && this.nPeekLength == _other.nPeekLength;
    }
}
