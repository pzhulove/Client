using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eItemSourceType
{
    eNone           = 0x00,

    /// <summary>
    /// 罐子
    /// </summary>
    eJar            = 0x10,
    
    /// <summary>
    /// 地下城
    /// </summary>
    eDungeon        = 0x20,

    /// <summary>
    /// 地下城活动
    /// </summary>
    eDungeonActivity = 0x21,

    /// <summary>
    /// 拍卖行
    /// </summary>
    eAuction         = 0x30,

    /// <summary>
    /// 传奇之路
    /// </summary>
    eLegend         = 0x40,

    /// <summary>
    /// 商店
    /// </summary>
    eShop           = 0x50,

    /// <summary>
    /// 商城
    /// </summary>
    eMall           = 0x60,

    /// <summary>
    /// 团队副本
    /// </summary>
    eTeamDuplicate  = 0x70,

    /// <summary>
    /// 60神器罐
    /// </summary>
    eArtifact_Tank60 = 0x80
}

public class ItemSourceInfoUtility
{
    public static string GetStringFromTable(ItemSourceInfoTable tb, int index)
    {
        if (null == tb || null == tb.strTables)
        {
            return null;
        }

        if (index < 0 || index >= tb.strTables.Length)
        {
            return null;
        }

        return tb.strTables[index];
    }

    public static string GetLinkName(ItemSourceInfoTable tb, ISourceInfo info)
    {
        if (null == info || null == tb)
        {
            return string.Empty;
        }

        return GetStringFromTable(tb, info.GetNameIndex());
    }

    public static string GetLinkInfo(ItemSourceInfoTable tb, ISourceInfo info)
    {
        if (null == info || null == tb)
        {
            return string.Empty;
        }

        string linkParm = GetStringFromTable(tb, info.GetParmIndex());

        switch (info.GetType())
        {
            case eItemSourceType.eDungeon:
                return string.Format("<type=mapid value={0}>", info.GetData());
            case eItemSourceType.eDungeonActivity:
            case eItemSourceType.eJar:
            case eItemSourceType.eAuction:
            case eItemSourceType.eLegend:
            case eItemSourceType.eMall:
            case eItemSourceType.eShop:
            case eItemSourceType.eTeamDuplicate:
            case eItemSourceType.eArtifact_Tank60:
                return linkParm;
            default:
                break;
        }

        return string.Empty;
    }

    /// <summary>
    /// 判断是否开启
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static bool IsLinkFunctionOpen(ISourceInfo info)
    {
        var openFunction = TableManager.instance.GetTableItem<ProtoTable.FunctionUnLock>(info.GetOpenFunctionID());
        int openLevel = 1;

        if (null != openFunction)
        {
            openLevel = Mathf.Max(openLevel, openFunction.FinishLevel);
        }

        if (info.GetType() == eItemSourceType.eJar)
        {
            openLevel = Mathf.Max(openLevel, info.GetData());
        }

        if (info.GetType() == eItemSourceType.eArtifact_Tank60)
        {
            openLevel = 60;
        }

        int playerLevel = GameClient.PlayerBaseData.GetInstance().Level;

        switch (info.GetType())
        {
            case eItemSourceType.eJar:
            case eItemSourceType.eShop:
            case eItemSourceType.eDungeonActivity:
            case eItemSourceType.eMall:
            case eItemSourceType.eAuction:
            case eItemSourceType.eLegend:
            case eItemSourceType.eTeamDuplicate:
            case eItemSourceType.eArtifact_Tank60:
                return openLevel <= playerLevel;
            case eItemSourceType.eDungeon:
                return GameClient.ChapterUtility.GetDungeonCanEnter(info.GetData(), false, false);
        }

        return true;
    }
}


public interface ISourceInfo
{
    eItemSourceType GetType();

    int GetNameIndex();

    int GetParmIndex();

    int GetData();

    int GetOpenFunctionID();
}

[System.Serializable]
public class SourceInfo : System.IComparable<SourceInfo>, ISourceInfo
{ 
    public eItemSourceType type;

    public int data = -1;
    public int openFunctionID = -1;

    [System.NonSerialized]
    public string linkParm;

    [System.NonSerialized]
    public string name;

    /// <summary>
    /// ItemSourceInfoTable.stringTables index
    /// </summary>
    public int linkParmIndex;

    /// <summary>
    /// ItemSourceInfoTable.stringTables index
    /// </summary>
    public int nameIndex;

    public int CompareTo(SourceInfo other)
    {
        if ((int)type != (int)other.type)
        {
            return (int)type - (int)other.type;
        }

        if (data != other.data)
        {
            return data - other.data;
        }

        if (openFunctionID != other.openFunctionID)
        {
            return openFunctionID - other.openFunctionID;
        }

        int linkCmp = string.Compare(linkParm, other.linkParm);
        if (0 != linkCmp)
        {
            return linkCmp;
        }

        int nameCmp = string.Compare(name, other.name);
        if (0 != nameCmp)
        {
            return nameCmp;
        }

        if (linkParmIndex != other.linkParmIndex)
        {
            return linkParmIndex - other.linkParmIndex;
        }

        return nameIndex - other.nameIndex;
    }

    public int GetData()
    {
        return data;
    }

    public int GetOpenFunctionID()
    {
        return openFunctionID;
    }

    public int GetNameIndex()
    {
        return nameIndex;
    }

    public int GetParmIndex()
    {
        return linkParmIndex;
    }

    public override string ToString()
    {
        return string.Format("{0} {1}", type, name);
    }

    eItemSourceType ISourceInfo.GetType()
    {
        return type;
    }
}

[System.Serializable]
public class ItemSourceInfo : System.IComparable<ItemSourceInfo>
{
    public int itemID;
    public SourceInfo[] sources = new SourceInfo[0];

    public int CompareTo(ItemSourceInfo other)
    {
        return itemID - other.itemID;
    }
}

[System.Serializable]
public class ItemScoreInfo : System.IComparable<ItemScoreInfo>
{
    public int itemID;
    public int score;

    public int CompareTo(ItemScoreInfo other)
    {
        return itemID - other.itemID;
    }
}

public class ItemSourceInfoTable : ScriptableObject
{
    public ItemSourceInfo[] sources = new ItemSourceInfo[0];
    public string[] strTables = new string[0];
    public ItemScoreInfo[] scores = new ItemScoreInfo[0];
}
