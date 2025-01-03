using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System;
using ProtoTable;

public class ComTipInfoUnit : MonoBehaviour
{
    public Image mIcon;
    public Text mText;
    public Image mBg;

    private StringBuilder mString = StringBuilderCache.Acquire(2000);
    private ProtoTable.BuffTable mData;
    private const string kKeyPrefix = "battle_buff_";

    public void SetSprite(Sprite sprite)
    {
        mIcon.sprite = sprite;
    }

    public void SetBgActive(bool isActive)
    {
        if (mBg)
        {
            mBg.enabled = isActive;
        }
    }

    public void SetData(ProtoTable.BuffTable buffData)
    {
        mData = buffData;

        _updateData();

        mText.text = mString.ToString().TrimEnd();
    }

    private int _getCellValue(Type type, string filed)
    {
        try
        { 
            PropertyInfo obj = type.GetProperty(filed, BindingFlags.Public | BindingFlags.Instance);

            object gv = obj.GetValue(mData, null);

            if (gv is ProtoTable.UnionCell)
            {
                var cv = gv as ProtoTable.UnionCell;
                if (null != cv)
                {
                    return TableManager.GetValueFromUnionCell(cv, 1);
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

    private int _getFinalValue(BuffDrugConfigInfoTable.eValueType type, int val)
    {
        switch (type)
        {
            case BuffDrugConfigInfoTable.eValueType.Rate1000:
                return val / 10;
        }

        return val;
    }

    private void _updateData()
    {
        if (mData == null)
        {
            return;
        }

        List<string> list = new List<string>();
        var buffType = (BuffType)mData.Type;
        var dataType = mData.GetType();

        mString.Clear();

        var buffs = TableManager.instance.GetTable<BuffDrugConfigInfoTable>();
        var iter = buffs.GetEnumerator();

        while (iter.MoveNext())
        {
            BuffDrugConfigInfoTable config = iter.Current.Value as BuffDrugConfigInfoTable;
            if (null != config)
            {
                int v = _getCellValue(dataType, config.Filed);
                int va = _getFinalValue(config.ValueType, v);
                if (va > 0)
                {
                    mString.Append(string.Format(TR.Value(kKeyPrefix + config.Filed) + "\n", va));
                }
            }
        }
    }
}
