using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterID
{
    private const int kDiffculte = 10;
    private const int kTypeID    = 10;
    private const int kLevel     = 100;
    private const int kID        = 1000;

    private int mMonsterID;
    private int mMonsterLevel;
    private int mMonsterTypeID;
    private int mMonsterDiffcute;
    private int mResID;

    public int resID
    {
        get
        {
            return mResID;
        }

        set
        {
            mResID = value;
            _decodeID();
        }
    }

    public int monsterID
    {
        get
        {
            return mMonsterID;
        }

        set
        {
            if (mMonsterID != value && value > 0)
            {
                mMonsterID = value;
                _encodeID();
            }
        }
    }

    public int monsterLevel
    {
        get
        {
            return mMonsterLevel;
        }

        set
        {
            if (mMonsterLevel != value && value >= 0)
            {
                mMonsterLevel = value;
                _encodeID();
            }
        }
    }

    public int monsterTypeID
    {
        get
        {
            return mMonsterTypeID;
        }

        set
        {
            if (mMonsterTypeID != value && value > 0)
            {
                mMonsterTypeID = value;
                _encodeID();
            }
        }
    }

    public int monsterDiffcute
    {
        get
        {
            return mMonsterDiffcute;
        }

        set
        {
            if (mMonsterDiffcute != value && value > 0)
            {
                mMonsterDiffcute = value;
                _encodeID();
            }
        }
    }

    private void _decodeID()
    {
        int tid = mResID;
        mMonsterDiffcute = tid % kDiffculte;
        tid /= kDiffculte;

        mMonsterTypeID = tid % kTypeID;
        tid /= kTypeID;

        mMonsterLevel = tid % kLevel;
        tid /= kLevel;

        mMonsterID = tid % kID;
    }

    private void _encodeID()
    {
        mResID = mMonsterID % kID;

        mResID *= kLevel;
        mResID += mMonsterLevel % kLevel;

        mResID *= kTypeID;
        mResID += mMonsterTypeID % kTypeID;

        mResID *= kDiffculte;
        mResID += mMonsterDiffcute % kDiffculte;
    }
}
