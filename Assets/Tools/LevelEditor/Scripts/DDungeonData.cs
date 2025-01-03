using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class DSceneDataConnect : IDungeonConnectData
{
    ///<summary>
    /// 表示四个方向的传送门是否连接
    ///</summary>
    public bool[] isconnect = new bool[4];

    ///<summary>
    /// index of the arealist AND areaconnectlist
    ///</summary>
    public int areaindex = -1;

    public int[] linkAreaIndex = new int[0];

    ///<summary>
    /// 区域id
    ///</summary>
    public int id;

    //public int sceneareaid = -1;
    public string sceneareapath = "";

    [System.NonSerialized]
    public DSceneData scenedata = null;

    // position of the map
    #region Position
    public int positionx = -1;
    public int positiony = -1;

    public bool isboss;
    public bool isstart;
    public bool isegg;
    //public int positionmap = -1;

    /// <summary>
    /// 是否不是深渊
    /// </summary>
    public bool isnothell = false;

    /// <summary>
    /// 藏宝地下城地图类型
    /// </summary>
    public byte treasureType = 0;

    #endregion

#if UNITY_EDITOR
    public void ResetConnect()
    {
        for (int i = 0; i < 4; i++)
        {
            isconnect[i] = false;
        }
    }
#endif

    public bool[] GetIsConnect()
    {
        return isconnect;
    }

    public int GetAreaIndex()
    {
        return areaindex;
    }

    public int GetId()
    {
        return id;
    }

    public string GetSceneAreaPath()
    {
        return sceneareapath;
    }

    /// <summary>
    /// 这个函数只能给新手引导用！！！！！！！
    /// 得干掉
    /// </summary>
    /// <param name="path"></param>
    public void SetSceneAreaPath(string path)
    {
        sceneareapath = path;
    }

    public int GetPositionX()
    {
        return positionx;
    }

    public int GetPositionY()
    {
        return positiony;
    }

    public bool IsBoss()
    {
        return isboss;
    }

    public bool IsStart()
    {
        return isstart;
    }

    public bool IsEgg()
    {
        return isegg;
    }

    public bool IsNotHell()
    {
        return isnothell;
    }

    public int GetIsConnectLength()
    {
        if (null == isconnect) return 0;

        return isconnect.Length;
    }

    public bool GetIsConnect(int i)
    {
        if (null == isconnect) return false;
        if (i < 0) return false;
        if (i >= GetIsConnectLength()) return false;

        return isconnect[i];
    }

    public void SetSceneData(ISceneData sceneData)
    {
        scenedata = sceneData as DSceneData;
    }

    public ISceneData GetSceneData()
    {
        return scenedata;
    }

    public int GetLinkAreaIndexesLength()
    {
        if (null == linkAreaIndex)
        {
            return 0;
        }

        return linkAreaIndex.Length;
    }

    public int GetLinkAreaIndex(int i)
    {
        int len = GetLinkAreaIndexesLength();
        if (i < 0) return -1;
        if (i >= len) return -1;

        return linkAreaIndex[i];
    }

    public byte GetTreasureType()
    {
        return treasureType;
    }
}

public class DDungeonData : ScriptableObject, IDungeonData
{
    [System.NonSerialized]
    public string name = "";

    public int height = 3;
    public int weidth = 3;

    // index of the areaconnectlist
    public int startindex = 0;

    public DSceneDataConnect[] areaconnectlist = new DSceneDataConnect[0];

    //public DDungeonCeilData[] areaconnectlist = new DDungeonCeilData[0];
    //#if UNITY_EDITOR
    //[System.NonSerialized]
    //public DSceneData[] arealist = new DSceneData[0];
    //#endif

    #region IDungeonDataGetter

    public string GetName()
    {
        return name;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWeidth()
    {
        return weidth;
    }

    public int GetStartIndex()
    {
        return startindex;
    }

    public int GetAreaConnectListLength()
    {
        if (null == areaconnectlist) return 0;
        return areaconnectlist.Length;
    }

    public IDungeonConnectData GetAreaConnectList(int i)
    {
        if (null == areaconnectlist) return null;
        if (i < 0 || i >= GetAreaConnectListLength()) return null;

        return areaconnectlist[i];
    }

    IDungeonConnectData IDungeonData.GetSideByType(int idx, TransportDoorType fromtype)
    {
        if (idx < 0 || idx > areaconnectlist.Length)
        {
            Logger.LogError("areaidx out of range");
            return null;
        }

        return GetSideByType(areaconnectlist[idx], fromtype);
    }

    public void GetSideByType(IDungeonConnectData condata, TransportDoorType fromtype, out int index)
    {
        index = -1;

        int x = condata.GetPositionX();
        int y = condata.GetPositionY();

        switch(fromtype)
        {
            case TransportDoorType.Buttom:  ++y; break;
            case TransportDoorType.Top:     --y; break;
            case TransportDoorType.Right:   ++x; break;
            case TransportDoorType.Left:    --x; break;
        }

        for(int i = 0; i < areaconnectlist.Length; ++i)
        {
            var item = areaconnectlist[i];
            if (item.positionx == x && item.positiony == y)
            {
                index = i;
                return;
            }
        }
    }

    public int GetConnectStatus(IDungeonConnectData from, IDungeonConnectData to)
    {
        if (from == null || to == null)
        {
            return -1;
        }

        for (int i = 0; i < from.GetIsConnectLength(); ++i)
        {
            if (from.GetIsConnect(i))
            {
                IDungeonConnectData con = GetSideByType(from, (TransportDoorType)i);

                if (con.GetAreaIndex() == to.GetAreaIndex() &&
                    con.GetPositionX() == to.GetPositionX() &&
                    con.GetPositionY() == to.GetPositionY())
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public IDungeonConnectData GetSideByType(IDungeonConnectData condata, TransportDoorType fromtype)
    {
        int x = condata.GetPositionX();
        int y = condata.GetPositionY();

        switch(fromtype)
        {
            case TransportDoorType.Buttom:  ++y; break;
            case TransportDoorType.Top:     --y; break;
            case TransportDoorType.Right:   ++x; break;
            case TransportDoorType.Left:    --x; break;
        }

        for (int i = 0; i < GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData item = GetAreaConnectList(i);

            if (item.GetPositionX() == x &&
                item.GetPositionY() == y)
            {
                return item;
            }

        }

        return null;
    }

    public void GetSideByType(int x, int y, TransportDoorType fromtype, out int index)
    {
        index = -1;
        
        switch(fromtype)
        {
            case TransportDoorType.Buttom:  ++y; break;
            case TransportDoorType.Top:     --y; break;
            case TransportDoorType.Right:   ++x; break;
            case TransportDoorType.Left:    --x; break;
        }

        for (int i = 0; i < GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData item = GetAreaConnectList(i);

            if (item.GetPositionX() == x && item.GetPositionY() == y)
            {
                index = i;
                return;
            }
        }
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    #endregion
}
