using UnityEngine;
using System.Collections;

public class DungeonID
{
    private enum eEncodeFlag
    {
        Chapter = 1 << 0,
        Level = 1 << 1,
        Prestory = 1 << 2,
        Diff = 1 << 3,
        WithoutDiff = Chapter | Level | Prestory,
        WithoutPrestory = Chapter | Level,
        All = Chapter | Level | Prestory | Diff,
    }

    public DungeonID(int id)
    {
        dungeonID = id;
    }

    private int mChapterID;
    private int mLevelID;
    private int mPrestoryID;
    private int mDiffID;

    /// <summary>
    /// 地下城ID
    /// x,2,2,1
    /// </summary>
    protected int mDungeonID;

    public int dungeonID
    {
        get
        {
            return mDungeonID;
        }

        set
        {
            if (mDungeonID != value)
            {
                mDungeonID = value;
                _decode();
            }
        }
    }

    public int dungeonIDWithOutDiff
    {
        get
        {
            return _encode(eEncodeFlag.WithoutDiff);
        }
    }

    public int dungeonIDWithOutPrestory
    {
        get
        {
            return _encode(eEncodeFlag.WithoutPrestory);
        }
    }

    public int chapterID
    {
        get { return mChapterID; }
        set
        {
            if (mChapterID != value)
            {
                mChapterID = value;
                mDungeonID = _encode();
            }
        }
    }

    public int levelID
    {
        get { return mLevelID; }
        set
        {
            if (mLevelID != value)
            {
                mLevelID = value;
                mDungeonID = _encode();
            }
        }
    }

    public int prestoryID
    {
        get { return mPrestoryID; }
        set
        {
            if (mPrestoryID != value)
            {
                mPrestoryID = value;
                mDungeonID = _encode();
            }
        }
    }

    public int diffID
    {
        get { return mDiffID; }
        set
        {
            if (mDiffID != value)
            {
                mDiffID = value;
                mDungeonID = _encode();
            }
        }
    }

    private void _decode()
    {
        var tmpId = mDungeonID;
        mDiffID = tmpId % 10;
        tmpId /= 10;

        mPrestoryID = tmpId % 100;
        tmpId /= 100;

        mLevelID = tmpId % 100;
        tmpId /= 100;

        mChapterID = tmpId;
    }

    private int _encode(eEncodeFlag flag = eEncodeFlag.All)
    {
        int tmpID = 0;

        if ((int)(flag & eEncodeFlag.Chapter) > 0)
        {
            tmpID += mChapterID;
        }

        tmpID *= 100;
        if ((int)(flag & eEncodeFlag.Level) > 0)
        {
            tmpID += mLevelID;
        }

        tmpID *= 100;
        if ((int)(flag & eEncodeFlag.Prestory) > 0)
        {
            tmpID += mPrestoryID;
        }

        tmpID *= 10;
        if ((int)(flag & eEncodeFlag.Diff) > 0)
        {
            tmpID += mDiffID;
        }

        return tmpID;
    }
}

