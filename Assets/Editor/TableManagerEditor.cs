using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
//using ProtoBuf;
using System;
using ProtoTable;
using System.ComponentModel;
//using Tenmove.Runtime.Game;
using Tenmove.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableManagerEditor : Singleton<TableManagerEditor>
{
#if USE_FB_TABLE
    private const string kTablePath = "Data/table_fb/";
#else
    private const string kTablePath = "Data/table/";
#endif

    private bool bInit = false;
    static public bool bNeedUninit = false;

    private Dictionary<Type, Dictionary<int, object>> mTypeTableDict = new Dictionary<Type, Dictionary<int, object>>();

    private Dictionary<int, object> monsterAttributeMap = new Dictionary<int, object>();

    public override void Init()
    {
        if (bInit)
        {
            return;
        }

        bInit = true;

        LoadMonsterAttributeInfo();
        _loadSkillInfo();                         //加载技能信息
    }

    public override void UnInit()
    {
        //加载游戏表格
        //---------------------------------------------------------------------
        mTypeTableDict.Clear();
        mSkillidDict.Clear();

        bInit = false;
    }

    public Dictionary<int, object> GetTable<T>()
    {
        return GetTable(typeof(T));
    }

    public void ReloadTable(Type type)
    {
        if (mTypeTableDict.ContainsKey(type))
        {
            mTypeTableDict[type].Clear();
            mTypeTableDict[type] = _loadTable(type);
        }
        else 
        {
            Dictionary<int, object> tableData = _loadTable(type);
            mTypeTableDict.Add(type, tableData);
        }
    }

    public IEnumerator GetTableIEnumerator(Type type)
    {
        Dictionary<int, object> table = GetTable(type);

        if (null == table || skNullTable == table)
        {
            Dictionary<int, object> tableData = _loadTable(type);
            table = tableData;
            mTypeTableDict.Add(type, tableData);
        }

        if (null == table || skNullTable == table)
        {
            yield break;
        }

        Dictionary<int, object>.Enumerator iter = table.GetEnumerator();

        while (iter.MoveNext())
        {
            yield return (iter.Current.Value);
        }
    }

    public IEnumerator<Table> GetTableIEnumerator<Table>() where Table : class, FlatBuffers.IFlatbufferObject
    {
        var iter = GetTableIEnumerator(typeof(Table));
        while (iter.MoveNext())
        {
            yield return iter.Current as Table;
        }
    }
    private static Dictionary<int, object> skNullTable = new Dictionary<int, object>();

    public Dictionary<int, object> GetTable(Type curType)
    {
        if (!mTypeTableDict.ContainsKey(curType))
        {
            var tb = _loadTable(curType);
            mTypeTableDict.Add(curType, tb);
        }

        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return skNullTable;
        }

        Dictionary<int, object> table = mTypeTableDict[curType];
        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return skNullTable;
        }

        return table;
    }

    public T GetTableItem<T>(int id, string who = "", string dowhat = "")
    {
        var curType = typeof(T);

        Dictionary<int, object> curTblDict = GetTable(curType);
        if (null != curTblDict)
        {
            object curItem = null;
            if (curTblDict.TryGetValue(id, out curItem))
            {
                return (T)curItem;
            }
            else
            {
                return default(T);
            }
        }
        else
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return default(T);
        }
    }

    public object GetTableItem(Type curType, int id, string who = "", string dowhat = "")
    {
        Dictionary<int, object> curTblDict = GetTable(curType);
        if (null != curTblDict)
        {
            object curItem = null;
            if (curTblDict.TryGetValue(id, out curItem))
            {
                return curItem;
            }
            else
            {
                //Logger.LogWarningFormat("{0}表里没有id为{1}的项", curType.Name, id);
                return null;
            }
        }
        else
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return null;
        }
    }

    public T GetTableItemByIndex<T>(int iIndex)
    {
        var curType = typeof(T);
        var table = GetTable(curType);

        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return default(T);
        }

        int iCount = 0;
        foreach (var TableID in table.Keys)
        {
            if (iCount == iIndex)
            {
                return (T)(table[TableID]);
            }

            iCount++;
        }

        Logger.LogWarningFormat("{0}表里没有第{1}项索引", curType.Name, iIndex);
        return default(T);
    }

    public int GetTableItemCount<T>()
    {
        var curType = typeof(T);

        var table = GetTable(curType);
        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return -1;
        }

        return table.Count;
    }

    public T GetTableItem<T>(string key)
    {
        var item = GetTableItem<T>(key.GetHashCode());

        if (item == null)
        {
            Logger.LogWarningFormat("表里没有key为{0}的项", key);
        }

        return item;
    }

    private string _getTablePath(Type type)
    {
        return kTablePath + type.Name;
    }
    private Dictionary<int, object> _loadTable(Type type)
    {
        Dictionary<int, object> table = new Dictionary<int, object>();
        string filepath = _getTablePath(type);

#if !USE_FB_TABLE
        


        byte[] data = null;
#if USE_FB
        filepath = Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(filepath, Utility.kRawDataExtension)).ToLower();
        if(File.Exists(filepath))
            data = System.IO.File.ReadAllBytes(filepath);
        Logger.LogProcessFormat(filepath + "\n");
#else
        AssetBinary textAsset = AssetLoader.instance.LoadRes(filepath, typeof(AssetBinary)).obj as AssetBinary;
        if (null == textAsset)
        {
            Logger.LogError("Load table has failed, table resource:" + filepath);
            return table;
        }
        
        data = textAsset.m_DataBytes;
#endif
        if (null != data)
        {
            bool bCanParse = Serializer.CanParse(type);
            var IDMap = type.GetProperty("ID").GetGetMethod();
            for (int i = 0; i < data.Length;)
            {
                int len = 0;
                for (int j = i; j < i + 8; ++j)
                {
                    if (data[j] > 0)
                        len = len * 10 + (data[j] - '0');
                }

            i += 8;
            MemoryStream stream = new MemoryStream(data, i, len);
            try
            {
                object tableData = null;

                if(bCanParse)
                {
                    tableData = Serializer.ParseEx(type,stream);
                }
                else
                {
                    tableData = Serializer.DeserializeEx(type, stream);
                }
                
                if (tableData == null)
                {
                    Logger.LogErrorFormat("table data is nil {0}, {1}", filepath, i);
                }
                else
                {
                    var id = (int)IDMap.Invoke(tableData, null);
                    if (!table.ContainsKey(id))
                    {
                        table.Add(id, tableData);

                        if(type == typeof(GlobalSettingTable))
                        {
                            gst = tableData as GlobalSettingTable;
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("{0} : already contain the id : {1}", filepath, id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0} : *.cs don't match the *.xls, delete the *.proto, regenerate the *.cs", filepath);
                Logger.LogErrorFormat("error deserialize at line {0}, with error {1}", i + 1, e.ToString());

                string ErrorMsg = "表格："+filepath+" 加载失败，原因："+e.Message;

#if UNITY_EDITOR && !LOGIC_SERVER
                EditorUtility.DisplayDialog("【读表错误!】",ErrorMsg,"确定","");
#else
                Logger.LogErrorFormat("【读表错误!】 {0}",ErrorMsg);
#endif
                return table;
            }

                i += len;
            }
        }
        else
            Logger.LogErrorFormat("表格：" + filepath + " 资源不存在!");





#else
        do
        {

            byte[] data = null;

#if USE_FB
            filepath = Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(filepath, Utility.kRawDataExtension)).ToLower();
            if(File.Exists(filepath))
                data = System.IO.File.ReadAllBytes(filepath);
            Logger.LogProcessFormat(filepath + "\n");
#else
            string pathWithOutExt = Path.ChangeExtension(filepath, null);
            TextAsset textAssetFB = Resources.Load(pathWithOutExt, typeof(TextAsset)) as TextAsset;
            if (null == textAssetFB)
            {
                Logger.LogError("Load table has failed, table resource:" + filepath);
                return table;
            }

            data = textAssetFB.bytes;
#endif

            FlatBuffers.Table ftable = new FlatBuffers.Table();
            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(data);

            ftable.bb_pos = 0;
            ftable.bb = buffer;

            int length = ftable.__vector_len(0);

            for (int index = 0; index < length; ++index)
            {
                ;
                int offset = ftable.__vector(index);

                //                        CreateInstance();

                var fobj = Activator.CreateInstance(type);

                MethodInfo __assign = type.GetMethod("__assign");
                var IDMap = type.GetProperty("ID").GetGetMethod();

                BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
                //GetValue方法的参数
                object[] parameters = new object[] { ftable.__indirect(ftable.__vector(0) + index * 4), ftable.bb };
                __assign.Invoke(fobj, flag, Type.DefaultBinder, parameters, null);

                int id = (int)IDMap.Invoke(fobj, null);

                if (!table.ContainsKey(id))
                {
                    table.Add(id, fobj);
                    //                     if (type == typeof(GlobalSettingTable))
                    //                     {
                    //                         gst = fobj as GlobalSettingTable;
                    //                     }
                }
                else
                {
                    Logger.LogErrorFormat("id 相同,严重错误 表格：{0} id={1}", type.ToString(), id);
                }


            }

            return table;
        } while (false);

#endif


        return table;
    }


    void LoadMonsterAttributeInfo()
    {
        var table = GetTable(typeof(MonsterAttributeTable));

        foreach (var aItem in table)
        {
            var item = (ProtoTable.MonsterAttributeTable)aItem.Value;
            int m = item.MonsterMode;
            int d = item.Difficulty;
            int t = item.MonsterType;
            int level = item.level;

            int key = d * GlobalLogic.VALUE_100000 + t * GlobalLogic.VALUE_10000 + m * GlobalLogic.VALUE_1000 + level;

            if (!monsterAttributeMap.ContainsKey(key))
                monsterAttributeMap.Add(key, item);
        }
    }

    public object GetMonsterAttribute(int mode, int difficulty, int type, int level)
    {
        object obj = null;
        try
        {
            int key = difficulty * GlobalLogic.VALUE_100000 + type * GlobalLogic.VALUE_10000 + mode * GlobalLogic.VALUE_1000 + level;

            obj = monsterAttributeMap[key];
        }
        catch
        {
            Logger.LogErrorFormat("[获取怪物属] 出错 mode : {0}, diff {1}, type {2}, level {3}", mode, difficulty, type, level);
        }

        return obj;
    }

    //public string GetShaderEffectPath(string name)
    //{
    //    string ret = "";
    //    var table = mTypeTableDict[typeof(FrameShaderEffectTable)];
    //    if (table != null)
    //    {
    //        foreach (var item in table)
    //        {
    //            var data = (ProtoTable.FrameShaderEffectTable)item.Value;
    //            if (data.Name == name)
    //                return data.Path;
    //        }
    //    }

    //    return ret;
    //}

    public static int GetValueFromUnionCell(UnionCell ucell, int level, bool bNeedBaseLevel = true)
    {
        if (bNeedBaseLevel && level <= 0)
        {
            level = 1;
        }

        if (level > 0)
        {
            var valueType = ucell.valueType;

            if (valueType == UnionCellType.union_fix)
            {
                return ucell.fixValue;
            }

            if (valueType == UnionCellType.union_fixGrow)
            {
                return ucell.fixInitValue + (level - 1) * ucell.fixLevelGrow;
            }

            if (valueType == UnionCellType.union_everyvalue)
            {
                if (level - 1 < ucell.eValues.everyValues.Count)
                {
                    return ucell.eValues.everyValues[level - 1];
                }
                //超过就返回最后那个
                else
                {
                    return ucell.eValues.everyValues[ucell.eValues.everyValues.Count - 1];
                }
            }

            return 0;
        }

        return 0;
    }

    #region dd: Skill Info Map

    // 提前整理好各职业对应的技能列表
    private Dictionary<int, List<int>> mSkillidDict = new Dictionary<int, List<int>>(); // key1 = 职业id, key2 = 技能id列表

    private void _loadSkillInfo()
    {
        var skillDatas = GetTable(typeof(SkillTable));

        var data = skillDatas.GetEnumerator();

        while (data.MoveNext())
        {
            SkillTable skillItem = data.Current.Value as SkillTable;

            if (skillItem.SkillCategory >= 5)
            {
                continue;
            }

            for (int i = 0; i < skillItem.JobID.Count; i++)
            {
                int pid = skillItem.JobID[i];

                if (!mSkillidDict.ContainsKey(pid))
                {
                    mSkillidDict.Add(pid, new List<int>());
                }

                int index = mSkillidDict[pid].FindIndex((value) => { return value == skillItem.ID; });

                if (index < 0)
                {
                    mSkillidDict[pid].Add(skillItem.ID);
                }
            }
        }
    }

    public List<int> GetSkillidListByOccu(int occu)
    {
        if (mSkillidDict.ContainsKey(occu))
        {
            return mSkillidDict[occu];
        }

        return null;
    }
    #endregion

    #region NOT NEED

    public int GetMonsterTableProperty(AttributeType attributeType, ProtoTable.UnitTable data)
    {
        int value = 0;
        switch (attributeType)
        {
            case AttributeType.maxHp:
                value = data.maxHp;
                break;
            case AttributeType.maxMp:
                value = data.maxMp;
                break;
            case AttributeType.hpRecover:
                value = data.hpRecover;
                break;
            case AttributeType.mpRecover:
                value = data.mpRecover;
                break;
            case AttributeType.attack:
                value = data.attack;
                break;
            case AttributeType.magicAttack:
                value = data.magicAttack;
                break;
            case AttributeType.defence:
                value = data.defence;
                break;
            case AttributeType.magicDefence:
                value = data.magicDefence;
                break;
            case AttributeType.attackSpeed:
                value = data.attackSpeed;
                break;
            case AttributeType.spellSpeed:
                value = data.spellSpeed;
                break;
            case AttributeType.moveSpeed:
                value = data.moveSpeed;
                break;
            case AttributeType.ciriticalAttack:
                value = data.ciriticalAttack;
                break;
            case AttributeType.ciriticalMagicAttack:
                value = data.ciriticalMagicAttack;
                break;
            case AttributeType.dex:
                value = data.dex;
                break;
            case AttributeType.dodge:
                value = data.dodge;
                break;
            case AttributeType.frozen:
                value = data.frozen;
                break;
            case AttributeType.hard:
                value = data.hard;
                break;
            case AttributeType.cdReduceRate:
                value = data.cdReduceRate;
                break;
            /*
		case AttributeType.abnormalResist:
			value = data.abnormalResist;
			break;
            
            case AttributeType.criticalPercent:
                value = data.criticalPercent;
                break;

            case AttributeType.cdReduceRateMagic:
                value = data.cdReduceRateMagic;
                break;
            case AttributeType.mpCostReduceRate:
                value = data.mpCostReduceRate;
                break;
            case AttributeType.mpCostReduceRateMagic:
                value = data.mpCostReduceRateMagic;
                break;
            case AttributeType.attackAddRate:
                value = data.attackAddRate;
                break;
            case AttributeType.magicAttackAddRate:
                value = data.magicAttackAddRate;
                break;
            */
            case AttributeType.baseAtk:
                value = data.baseAtk;
                break;
            case AttributeType.baseInt:
                value = data.baseInt;
                break;
            case AttributeType.baseSpr:
                value = data.spr;
                break;
            case AttributeType.baseSta:
                value = data.sta;
                break;
            case AttributeType.ignoreDefAttackAdd:
                value = data.ignoreDefAttackAdd;
                break;
            case AttributeType.ignoreDefMagicAttackAdd:
                value = data.ignoreDefMagicAttackAdd;
                break;
                /*
                case AttributeType.attackReduceRate:
                    value = data.attackReduceRate;
                    break;
                case AttributeType.magicAttackReduceRate:
                    value = data.magicAttackReduceRate;
                    break;
                case AttributeType.attackReduceFix:
                    value = data.attackReduceFix;
                    break;
                case AttributeType.magicAttackReduceFix:
                    value = data.magicAttackReduceFix;
                    break;
                case AttributeType.defenceAddRate:
                    value = data.defenceAddRate;
                    break;
                case AttributeType.magicDefenceAddRate:
                    value = data.magicDefenceAddRate;
                    break;*/
        }
        return value;
    }

    public int GetMonsterAttributeTableProperty(AttributeType attributeType, ProtoTable.MonsterAttributeTable data)
    {
        int value = 0;
        switch (attributeType)
        {
            case AttributeType.maxHp:
                value = data.maxHp;
                break;
            case AttributeType.maxMp:
                value = data.maxMp;
                break;
            case AttributeType.hpRecover:
                value = data.hpRecover;
                break;
            case AttributeType.mpRecover:
                value = data.mpRecover;
                break;
            case AttributeType.attack:
                value = data.attack;
                break;
            case AttributeType.magicAttack:
                value = data.magicAttack;
                break;
            case AttributeType.defence:
                value = data.defence;
                break;
            case AttributeType.magicDefence:
                value = data.magicDefence;
                break;
            case AttributeType.attackSpeed:
                value = data.attackSpeed;
                break;
            case AttributeType.spellSpeed:
                value = data.spellSpeed;
                break;
            case AttributeType.moveSpeed:
                value = data.moveSpeed;
                break;
            case AttributeType.ciriticalAttack:
                value = data.ciriticalAttack;
                break;
            case AttributeType.ciriticalMagicAttack:
                value = data.ciriticalMagicAttack;
                break;
            case AttributeType.dex:
                value = data.dex;
                break;
            case AttributeType.dodge:
                value = data.dodge;
                break;
            case AttributeType.frozen:
                value = data.frozen;
                break;
            case AttributeType.hard:
                value = data.hard;
                break;
            case AttributeType.cdReduceRate:
                value = data.cdReduceRate;
                break;
            /*
        case AttributeType.abnormalResist:
            value = data.abnormalResist;
            break;
            
            case AttributeType.criticalPercent:
                value = data.criticalPercent;
                break;
            case AttributeType.cdReduceRateMagic:
                value = data.cdReduceRateMagic;
                break;
            case AttributeType.mpCostReduceRate:
                value = data.mpCostReduceRate;
                break;
            case AttributeType.mpCostReduceRateMagic:
                value = data.mpCostReduceRateMagic;
                break;
            case AttributeType.attackAddRate:
                value = data.attackAddRate;
                break;
            case AttributeType.magicAttackAddRate:
                value = data.magicAttackAddRate;
                break;
            */
            case AttributeType.baseAtk:
                value = data.baseAtk;
                break;
            case AttributeType.baseInt:
                value = data.baseInt;
                break;
            case AttributeType.baseSpr:
                value = data.spr;
                break;
            case AttributeType.baseSta:
                value = data.sta;
                break;
            case AttributeType.ignoreDefAttackAdd:
                value = data.ignoreDefAttackAdd;
                break;
            case AttributeType.ignoreDefMagicAttackAdd:
                value = data.ignoreDefMagicAttackAdd;
                break;
                /*
                case AttributeType.attackReduceRate:
                    value = data.attackReduceRate;
                    break;
                case AttributeType.magicAttackReduceRate:
                    value = data.magicAttackReduceRate;
                    break;
                case AttributeType.attackReduceFix:
                    value = data.attackReduceFix;
                    break;
                case AttributeType.magicAttackReduceFix:
                    value = data.magicAttackReduceFix;
                    break;
                case AttributeType.defenceAddRate:
                    value = data.defenceAddRate;
                    break;
                case AttributeType.magicDefenceAddRate:
                    value = data.magicDefenceAddRate;
                    break;*/
        }
        return value;
    }

    public ProtoTable.UnionCell GetBuffTableProperty(AttributeType attributeType, ProtoTable.BuffTable data)
    {
        ProtoTable.UnionCell value = null;
        switch (attributeType)
        {
            case AttributeType.baseAtk:
                value = data.baseAtk;
                break;
            case AttributeType.baseInt:
                value = data.baseInt;
                break;
  //          case AttributeType.maxHp:
  //              value = data.maxHp;
  //              break;
  //          case AttributeType.maxMp:
  //              value = data.maxMp;
  //              break;
  //          case AttributeType.hpRecover:
  //              value = data.hpRecover;
  //              break;
  //          case AttributeType.mpRecover:
  //              value = data.mpRecover;
  //              break;
  //          case AttributeType.attack:
  //              value = data.attack;
  //              break;
  //          case AttributeType.magicAttack:
  //              value = data.magicAttack;
  //              break;
  //          case AttributeType.indieAttack:
  //              value = data.indieAttack;
  //              break;
  //          case AttributeType.defence:
  //              value = data.defence;
  //              break;
  //          case AttributeType.magicDefence:
  //              value = data.magicDefence;
  //              break;
  //          case AttributeType.attackSpeed:
  //              value = data.attackSpeed;
  //              break;
  //          case AttributeType.spellSpeed:
  //              value = data.spellSpeed;
  //              break;
  //          case AttributeType.moveSpeed:
  //              value = data.moveSpeed;
  //              break;
  //          case AttributeType.ciriticalAttack:
  //              value = data.ciriticalAttack;
  //              break;
  //          case AttributeType.ciriticalMagicAttack:
  //              value = data.ciriticalMagicAttack;
  //              break;
  //          case AttributeType.dex:
  //              value = data.dex;
  //              break;
  //          case AttributeType.dodge:
  //              value = data.dodge;
  //              break;
  //          case AttributeType.frozen:
  //              value = data.frozen;
  //              break;
  //          case AttributeType.hard:
  //              value = data.hard;
  //              break;
  //          case AttributeType.abnormalResist:
  //              value = data.abnormalResist;
  //              break;
  //          case AttributeType.criticalPercent:
  //              value = data.criticalPercent;
  //              break;
  //          case AttributeType.cdReduceRate:
  //              value = data.cdReduceRate;
  //              break;
  //          /*
		//case AttributeType.cdReduceRateMagic:
		//	value = data.cdReduceRateMagic;
		//	break;
		//case AttributeType.mpCostReduceRate:
		//	value = data.mpCostReduceRate;
		//	break;
		//case AttributeType.mpCostReduceRateMagic:
		//	value = data.mpCostReduceRateMagic;
		//	break;
		//	*/
  //          case AttributeType.attackAddRate:
  //              value = data.attackAddRate;
  //              break;
  //          case AttributeType.magicAttackAddRate:
  //              value = data.magicAttackAddRate;
  //              break;
  //          /*
		//case AttributeType.ignoreDefAttackAdd:
		//	value = data.ignoreDefAttackAdd;
		//	break;
		//case AttributeType.ignoreDefMagicAttackAdd:
		//	value = data.ignoreDefMagicAttackAdd;
		//	break;
		//case AttributeType.attackReduceRate:
		//	value = data.attackReduceRate;
		//	break;
		//case AttributeType.magicAttackReduceRate:
		//	value = data.magicAttackReduceRate;
		//	break;
		//case AttributeType.attackReduceFix:
		//	value = data.attackReduceFix;
		//	break;

		//case AttributeType.magicAttackReduceFix:
		//	value = data.magicAttackReduceFix;
		//	break;
		//	*/
  //          case AttributeType.defenceAddRate:
  //              value = data.defenceAddRate;
  //              break;
  //          case AttributeType.magicDefenceAddRate:
  //              value = data.magicDefenceAddRate;
  //              break;
        }
        return value;
    }

    public ProtoTable.UnionCell GetBuffTablePropertyByName(string itemName, ProtoTable.BuffTable data)
    {
        ProtoTable.UnionCell cell = null;
        //switch (itemName)
        //{
        //    case "atkAddRate":
        //        cell = GetBuffTableProperty(AttributeType.atkAddRate, data);
        //        break;
        //    case "intAddRate":
        //        cell = GetBuffTableProperty(AttributeType.intAddRate, data);
        //        break;
        //    case "staAddRate":
        //        cell = GetBuffTableProperty(AttributeType.staAddRate, data);
        //        break;
        //    case "sprAddRate":
        //        cell = GetBuffTableProperty(AttributeType.sprAddRate, data);
        //        break;
        //    case "maxHpAddRate":
        //        cell = GetBuffTableProperty(AttributeType.maxHpAddRate, data);
        //        break;
        //    case "maxMpAddRate":
        //        cell = GetBuffTableProperty(AttributeType.maxMpAddRate, data);
        //        break;
        //    case "ignoreDefAttackAddRate":
        //        cell = GetBuffTableProperty(AttributeType.ignoreDefAttackAddRate, data);
        //        break;
        //    case "ignoreDefMagicAttackAddRate":
        //        cell = GetBuffTableProperty(AttributeType.ignoreDefMagicAttackAddRate, data);
        //        break;
        //}

        return cell;
    }

    #endregion
}
