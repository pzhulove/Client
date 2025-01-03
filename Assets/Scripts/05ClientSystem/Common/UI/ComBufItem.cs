using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using ProtoBuf;
using Protocol;
using ProtoTable;
using System.Reflection;
using System;


public class ComBufItem : MonoBehaviour
{

    [SerializeField]
    Text bufName = null;

    [SerializeField]
    Image bufIcon = null;

    [SerializeField]
    Text bufAttrs = null;

    int id = 0;
    int lv = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetBufData(int bufID,int bufLv = 1)
    {
        id = bufID;
        lv = bufLv;

        BuffTable buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(bufID);
        if(buffTable == null)
        {
            return;
        }

        bufName.SafeSetText(buffTable.Name);
        bufIcon.SafeSetImage(buffTable.Icon);
        bufAttrs.SafeSetText(buffTable.Description);
    }

    public int GetBufID()
    {
        return id;
    }

    public int GetBufLv()
    {
        return lv;
    }

    private static int _getCellValue(Type type, string filed, BuffTable buf, int lv)
    {
        try
        {
            PropertyInfo obj = type.GetProperty(filed, BindingFlags.Public | BindingFlags.Instance);

            object gv = obj.GetValue(buf, null);

            if (gv is ProtoTable.UnionCell)
            {
                var cv = gv as ProtoTable.UnionCell;
                if (null != cv)
                {
                    return TableManager.GetValueFromUnionCell(cv, lv);
                }
            }
            else if (gv is int)
            {
                return (int)gv;
            }
        }
        catch
        {

        }

        return -1;
    }

    private static int _getFinalValue(BuffDrugConfigInfoTable.eValueType type, int val)
    {
        switch (type)
        {
            case BuffDrugConfigInfoTable.eValueType.Rate1000:
                return val / 10;
        }

        return val;
    }

    public static string GetBuffAddUpInfo(int iBufID, int iBufLv)
    {
        string txtInfo = "";
        var tableBuf = TableManager.instance.GetTableItem<ProtoTable.BuffTable>(iBufID);
        if (tableBuf == null)
        {
            return "";
        }

        var buffType = (BuffType)tableBuf.Type;
        var dataType = tableBuf.GetType();

        var buffs = TableManager.instance.GetTable<BuffDrugConfigInfoTable>();
        var iter = buffs.GetEnumerator();

        while (iter.MoveNext())
        {
            BuffDrugConfigInfoTable config = iter.Current.Value as BuffDrugConfigInfoTable;
            if (null != config)
            {
                int v = _getCellValue(dataType, config.Filed, tableBuf, iBufLv);
                int va = _getFinalValue(config.ValueType, v);
                if (va > 0)
                {
                    txtInfo += string.Format(TR.Value(config.Filed), va);
                    txtInfo += "  ";
                }
            }
        }        

        return txtInfo;
    }
}
