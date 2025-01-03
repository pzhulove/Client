using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResJungle;
using ProtoTable;
using System.Reflection;
using FlatBuffers;
using System;
using GameClient;

[ResJungleDesc(false)]
public abstract class FBTableManager<T> : RJClassManager<T>
    where T : IFlatbufferObject

{
    public Type FBType { get; private set; }
    public PropertyInfo ID { get; private set; }
    public FBTableManager()
    {
        FBType = typeof(T);
        ID = FBType.GetProperty("ID", BindingFlags.Instance | BindingFlags.Public);
    }

    public override int ResCount
    {
        get
        {
             var tb = TableManager.instance.GetTable(typeof(T));
             return tb.Count;
        }
    }

    public override IEnumerator<ResObject> GetEnumerator()
    {
        var tb = TableManager.instance.GetTable(typeof(T));
        foreach (var item in tb)
        {
            yield return GetResIns(item.Value);
        }
    }
    public override object LoadFromResObject(ResObject obj)
    {
        return TableManager.instance.GetTableItem(FBType, obj.Value.Index);
    }

    struct ModifyNode
    {
        public ModifyNode(int id, RJField f, ResObject v)
        {
            ID = id;
            Field = f;
            Value = v;
        }

        public int ID { get; private set; }

        public RJField Field { get; private set; }
        public ResObject Value { get; private set; }
    }

    private List<ModifyNode> ModifyNodes = new List<ModifyNode>();

    public override void OnModifyInstance(object o, ResObject n)
    {
        var f = Type.FindField(n.FieldID);
        int id = (int)ID.GetValue(o);
        ModifyNodes.Add(new ModifyNode(id, f, n));
    }

    protected override ResValue _GetCustomResInsValue(object o)
    {
        return new ResValue((int)ID.GetValue(o));
    }

    public override void PostLoadAll()
    {

    }

    public override void SaveModify()
    {
        var filename = Xls2FBWindow.GetXlsNameBySheetName(FBType.Name);
        string path = "../Share/table/xls/";

        XlsxDataUnit u = new XlsxDataUnit(path + filename);

        foreach (var n in ModifyNodes)
        {
            var data = u.GetRowData(n.ID);
            var cell = data[n.Field.FieldInsList[0].Desc.Name];
            if (n.Field.ArrayFieldCount > 0)
            {
                var originV = cell.StringCellValue;
                string[] v = originV.Split('|');
                int idx = n.Value.ArrayIndex[0];
                v[idx] = n.Value.Value.PathURLRelativeAssetResource;

                cell.SetCellValue(string.Join("|", v));
            }
            else
            {
                cell.SetCellValue(n.Value.Value.PathURLRelativeAssetResource);
            }
        }

        if (u.Write())
        {
            ModifyNodes.Clear();
            Xls2FBWindow.ConvertXlsBySheetName(FBType.Name);
            TableManager.instance.ReloadTable(FBType);
        }
    }

    protected override void _OnPreLoadFromResource()
    {
    }

    protected override void _OnUnInit()
    {
    }
}

[ResJungleDesc(true)]
public class RJMechanismTable : FBTableManager<MechanismTable>
{
    public override uint TypeID
    {
        get
        {
            return RJConst.ID.TypeClassBase + 4;
        }
    }
}

[ResJungleDesc(true)]
public class RJBuffTable : FBTableManager<BuffTable>
{
    public override uint TypeID
    {
        get
        {
            return RJConst.ID.TypeClassBase + 3;
        }
    }
}

[ResJungleDesc(true)]
public class RJEffectInfoTable : FBTableManager<EffectInfoTable>
{
    public override uint TypeID
    {
        get
        {
            return RJConst.ID.TypeClassBase + 2;
        }
    }
}


[ResJungleDesc(true)]
public class RJResTableManager : FBTableManager<ResTable>
{
    public override uint TypeID
    {
        get
        {
            return RJConst.ID.TypeClassBase + 1;
        }
    }
}
