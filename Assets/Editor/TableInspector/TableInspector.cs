using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;


public interface IProtoTableInspector<Table> : IProtoTableInspector
                                 where Table : class, FlatBuffers.IFlatbufferObject
{
    new Table Current { get; }
}

public interface IProtoTableInspector
{
    int CurrentID { get; }
    int CurrentIndex { get; }
    string CurrentDesc { get; }

    void AddDispalyConfig(string p, string desc);

    object Current { get; }

    int OnGUIWithID(int id, TableInspetor.EGUIFlag flag = TableInspetor.EGUIFlag.All);

    void OnGUI();
}

public interface ITableInspectorExtension
{
    void OnGUI(int id);
}

public class TableInspectorExtension<T>
                               where T : class, FlatBuffers.IFlatbufferObject
{
    public TableInspetor Inspetor { get; private set; }

    public T Current
    {
        get
        {
            if (null == Inspetor)
            {
                return default(T);
            }

            return Inspetor.Current as T;
        }
    }
    public TableInspectorExtension(TableInspetor inspetor)
    {
        Inspetor = inspetor;
    }
}

public class TableInspectorResTableExtension : TableInspectorExtension<ProtoTable.ResTable>, ITableInspectorExtension
{
    public TableInspectorResTableExtension(TableInspetor inspector) : base(inspector)
    {
    }

    public void OnGUI(int id)
    {
        if (null == Current)
        {
            return;
        }

        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(Current.ActionConfigPath[0]);

            if (GUILayout.Button(
                                   new GUIContent(EditorGUIUtility.IconContent("Folder Icon").image, "打开资源"),
                                   GUILayout.Width(25), GUILayout.Height(20)
                                )
               )
            {
                string newPath = "Assets/Resources/" + Current.ActionConfigPath[0];

                if (System.IO.Directory.Exists(newPath))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(newPath);
                    EditorGUIUtility.PingObject(obj);
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog("错误", string.Format("{0} 不存在", newPath), "OK");
                }
            }


            EditorGUILayout.EndHorizontal();
        }
    }
}

public class TableInspectorMonsterInstanceInfoTableExtension: TableInspectorExtension<ProtoTable.MonsterInstanceInfoTable>, ITableInspectorExtension
{
    public TableInspectorMonsterInstanceInfoTableExtension(TableInspetor inspector) : base(inspector)
    {
    }

    private bool ToggleBornBuff = false;
    private Vector2 mScrollPos = Vector2.zero;
    public void OnGUI(int id)
    {
        
        ToggleBornBuff = EditorGUILayout.ToggleLeft("详情", ToggleBornBuff);
        if (!ToggleBornBuff)
        {
            return;
        }

        if (null == Current)
        {
            Inspetor.IsShowDetail = true;
        }

        if (null == Current)
        {
            return;
        }

        using (var s = new EditorGUILayout.ScrollViewScope(mScrollPos))
        {
            using (new EditorGUILayout.VerticalScope("GroupBox"))
            {
                _GUIExte<ProtoTable.BuffTable>("出生Buff",     Current.BornBuffIDs);
                _GUIExte<ProtoTable.BuffInfoTable>("出生BuffInfo", Current.BornBuffInfoIDs);
                _GUIExte<ProtoTable.MechanismTable>("出生机制",     Current.BornMechismIDs);
            }

            mScrollPos = s.scrollPosition;
        }
    }

    private bool _GUIExte<T>(string name, IList<int> ids) where T : FlatBuffers.IFlatbufferObject
    {
        if (null == ids || ids.Count <= 0)
        {
            return false;
        }


        var gui = TableInspetor.Get(typeof(T), "ID");

        EditorGUILayout.LabelField(name);

        foreach (var curId in ids)
        {
            gui.OnGUIWithID(curId, TableInspetor.EGUIFlag.ReadOnly);
        }

        return true;
    }
}



    //, new MultiPropertyGUIDisplay[] {
    //            new MultiPropertyGUIDisplay("BornBuffID",      "出生Buff",     typeof(ProtoTable.BuffTable)),
    //            new MultiPropertyGUIDisplay("BornBuffInfoIDs", "出生BuffInfo", typeof(ProtoTable.BuffInfoTable)),
    //            new MultiPropertyGUIDisplay("BornMechismIDs",  "出生机制",     typeof(ProtoTable.MechanismTable))
    //        })]
public class TableInspectorExtensionFactory
{
    public static ITableInspectorExtension Create(TableInspetor inspector, Type type)
    {
        ITableInspectorExtension extension = null;

        if (type == typeof(ProtoTable.ResTable))
        {
            extension = new TableInspectorResTableExtension(inspector);
        }
        else if (type == typeof(ProtoTable.MonsterInstanceInfoTable))
        {
            extension = new TableInspectorMonsterInstanceInfoTableExtension(inspector);
        }

        return extension;
    }
}


public class TableInspetor<Table> : TableInspetor, IProtoTableInspector<Table>
                    where Table : class, FlatBuffers.IFlatbufferObject
{
    public TableInspetor(string idDisplayName, string nameKey = "Name") : base(typeof(Table), idDisplayName, nameKey)
    {
    }

    public TableInspetor(string idDisplayName, bool keepData) : base(typeof(Table), idDisplayName, "Name")
    {
        KeepData = keepData;
    }

    public TableInspetor() : base(typeof(Table), "", "Name")
    {

    }


    public new Table Current
    {
        get
        {
            return base.Current as Table;
        }
    }
}

public class TableInspetor : IProtoTableInspector
{
    private static Dictionary<Type, TableInspetor> skTableInspector = new Dictionary<Type, TableInspetor>();

    public static TableInspetor Get(Type type, string name)
    {
        TableInspetor cur = null;

        if (skTableInspector.ContainsKey(type))
        {
            cur = skTableInspector[type];
        }
        else 
        {
            cur = new TableInspetor(type, name);
            skTableInspector.Add(type, cur);
        }

        cur.AddDispalyConfig("ID", name);

        return cur;
    }

    public bool KeepData { get; protected set; }

    public object Current
    {
        get
        {
            if (null == CurrentTableNode)
            {
                return null;
            }

            return CurrentTableNode.Data;
        }
    }

    public int CurrentID
    {
        get
        {
            if (null == CurrentTableNode)
            {
                return 0;
            }

            return CurrentTableNode.ID;
        }

        set
        {
            CurrentIndex = -1;

            for (int i = 0; i < mNodes.Count; ++i)
            {
                var item = mNodes[i];
                if (item.ID == value)
                {
                    CurrentIndex = i;
                    break;
                }
            }
        }
    }

    public string CurrentDesc
    {
        get
        {
            if (null == CurrentTableNode)
            {
                return string.Empty;
            }

            return CurrentTableNode.Name;
        }
    }

    object IProtoTableInspector.Current
    {
        get
        {
            return CurrentTableNode;
        }
    }

    TableNode CurrentTableNode
    {
        get
        {
            if (null == mNodes)
            {
                return null;
            }

            if (CurrentIndex < 0)
            {
                return null;
            }

            if (CurrentIndex >= mNodes.Count)
            {
                return null;
            }

            return mNodes[CurrentIndex];
        }
    }

    public int CurrentIndex
    {
        get; private set;
    }


    class DisplayConfig
    {
        public string DisplayName;
        public string PropertyName;
    }
    private List<DisplayConfig> mConfigs = new List<DisplayConfig>();

    public void AddDispalyConfig(string pName, string desc)
    {
        if (string.IsNullOrEmpty(pName))
        {
            return;
        }

        if (string.IsNullOrEmpty(desc))
        {
            return;
        }


        foreach (var p in mConfigs)
        {
            if (p.PropertyName == pName)
            {
                p.DisplayName = desc;
                return;
            }
        }

        mConfigs.Add(new DisplayConfig() { DisplayName = desc, PropertyName = pName });
    }

    class TableNode
    {
        public int ID;
        public string Name;
        public object Data;
    }

    private List<TableNode> mNodes = new List<TableNode>();
    private string[] mNodesCache = new string[0];
    private string[] mNodesCacheSearch = new string[0];
    private string mCurrentInput = string.Empty;
    private int mSearchedNumber = 0;
    private string mNameKey = "Name";


    public TableInspetor(Type type, string idDisplayName = "", string nameKey = "Name")
    {
        if(type == null)
        {
            return;
        }

        KeepData = false;
        mNameKey = nameKey;

        _UpdateCurrentType(type);

        if (string.IsNullOrEmpty(idDisplayName))
        {
            mConfigs.Add(new DisplayConfig() { DisplayName = "ID", PropertyName = "ID" });
        }
        else
        {
            mConfigs.Add(new DisplayConfig() { DisplayName = idDisplayName, PropertyName = "ID" });
        }

        mConfigs.Add(new DisplayConfig() { DisplayName = "", PropertyName = nameKey });

        CurrentIndex = -1;
        Extension = TableInspectorExtensionFactory.Create(this, type);
    }

    private Type skCurrentType = null;

    public void _UpdateCache()
    {
        var dt = TableManagerEditor.GetInstance().GetTable(skCurrentType);

        mNodes.Clear();

        foreach(var t in dt)
        {
            TableNode node = new TableNode();
            node.Data = t.Value;
            node.ID = _GetID(node.Data);
            node.Name = _GetDesc(node.Data, mNameKey);

            mNodes.Add(node);
        }

        mNodesCache = new string[mNodes.Count];
        for (int i = 0; i < mNodes.Count; ++i)
        {
            TableNode node = mNodes[i];
            mNodesCache[i] = _GetCacheDesc(node);

            if (!KeepData)
            {
                node.Data = null;
            }
        }
    }

    private StringBuilder builder = new StringBuilder(1024);
    private string mIDNameLabel = string.Empty;

    private string _GetCacheDesc(TableNode n)
    {
        builder.Clear();

        foreach (var p in mConfigs)
        {
            if (p.PropertyName == "ID")
            {
                builder.AppendFormat("{0}:", n.ID);
                mIDNameLabel = p.DisplayName + ":";
            }
            else if (p.PropertyName == mNameKey)
            {
                builder.AppendFormat("{0}", n.Name);
            }
            else
            {
                builder.AppendFormat("{0}({1})", p.DisplayName, _GetDesc(n.Data, p.PropertyName));
            }
        }

        return builder.ToString();
    }


    private int _GetID(object t)
    {
        var v = _GetProValue(t, "ID");

        if (v is int)
        {
            return (int)v;
        }

        return 0;
    }

    private string _GetUnitCell(ProtoTable.UnionCell cell)
    {
        if (null == cell)
        {
            return string.Empty;
        }

        switch (cell.valueType)
        {
            case ProtoTable.UnionCellType.union_fix:
                return string.Format("固定值 {0}", cell.fixValue);
            case ProtoTable.UnionCellType.union_fixGrow:
                //return ucell.fixInitValue + (level - 1) * ucell.fixLevelGrow;
                return string.Format("成长 {0}+{1}*(等级-1)", cell.fixInitValue, cell.fixLevelGrow);
            case ProtoTable.UnionCellType.union_everyvalue:
                {
                    if (cell.eValues.everyValues.Count > 0)
                    {
                        List<string> ar = new List<string>(cell.eValues.everyValues.Count);
                        cell.eValues.everyValues.ForEach(x => { ar.Add(x.ToString()); });
                        return string.Format("每一级指定 {0}", String.Join(",", ar.ToArray()));
                    }
                    else
                    {
                        return string.Format("每一级指定");
                    }
                }
        }

        return string.Empty;
    }

    public float GetFloatValue10000(string key)
    {
        return GetFloatValue(key, 10000);
    }

    public float GetFloatValue1000(string key)
    {
        return GetFloatValue(key, 1000);
    }

    public float GetFloatValue(string key, float basenum)
    {
        var v = GetValue<float>(key);
        return v / basenum;
    }

    public T GetValue<T>(string key)
    {
        if (null == CurrentTableNode)
        {
            UnityEngine.Debug.LogErrorFormat("CurrentTableNode is null {0}", key);
            return default(T);
        }

        var v = _GetProValue(CurrentTableNode.Data, key);
        if (null == v)
        {
            return default(T);
        }

        Type targetType = typeof(T);

        if (targetType == typeof(Vec3))
        {
            var iter = _ValueIter(v);
            float fstValue = 0.0f;
            float sndValue = 0.0f;
            if (iter.MoveNext()) { fstValue = (int)iter.Current / 1000.0f; }
            if (iter.MoveNext()) { sndValue = (int)iter.Current / 1000.0f; }

            return (T)Activator.CreateInstance(typeof(Vec3), fstValue, sndValue, 0.0f);
        }
        else if (targetType == typeof(VFactor))
        {
            var target = (int)v;
            return (T)Activator.CreateInstance(typeof(VFactor), target, 1000);
        }

        return (T)Convert.ChangeType(v, typeof(T));
    }

    private IEnumerator _ValueIter(object t)
    {
        var lst = t as IEnumerable;
        if (null == lst)
        {
            yield return t;
        }


        var iter = lst.GetEnumerator();
        if (null != iter)
        {
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }
    }


    private string _GetDesc(object t, string key)
    {
        var v = _GetProValue(t, key);
        if (v is string)
        {
            return v as string;
        }
        else if (v is ProtoTable.UnionCell)
        {
            return _GetUnitCell(v as ProtoTable.UnionCell);
        }
        else if (null != v)
        {
            var lst = v as IEnumerable;
            if (null != lst)
            {
                var iter = lst.GetEnumerator();
                if (null != iter)
                {
                    List<string> caches = new List<string>();

                    while (iter.MoveNext())
                    {
                        if (null != iter.Current)
                        {
                            if (iter.Current is ProtoTable.UnionCell)
                            {
                                caches.Add(_GetUnitCell(iter.Current as ProtoTable.UnionCell));
                            }
                            else
                            {
                                caches.Add(iter.Current.ToString());
                            }
                        }
                        //else
                        //{
                        //    return string.Empty;
                        //}
                    }

                    return string.Join("|", caches.ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }

            return v.ToString();
        }

        return string.Empty;
    }

    private object _GetProValue(object t, string pn)
    {
        var pf = _GetPro(t, pn);

        if (null == pf)
        {
            return null;
        }

        return pf.GetValue(t, null);
    }

    private Dictionary<string, PropertyInfo> mDic = new Dictionary<string, PropertyInfo>();

    private PropertyInfo _GetPro(object t, string pn)
    {
        if (mDic.ContainsKey(pn))
        {
            return mDic[pn];
        }

        _UpdateCurrentType(t.GetType());


        var pf = skCurrentType.GetProperty(pn);
        if (!mDic.ContainsKey(pn))
        {
            mDic.Add(pn, pf);
        }

        return pf;
    }

    private string XlsName
    {
        get
        {
            if (null == skCurrentType)
            {
                return string.Empty;
            }

            return Xls2FBWindow.GetXlsNameBySheetName(skCurrentType.Name);
        }
    }

    private void _UpdateCurrentType(Type t)
    {
        if (null == t)
        {
            return;
        }

        if (skCurrentType == null)
        {
            skCurrentType = t;
        }
    }

    private static Regex MSearchedNumber = new Regex(@"^(\d+)");

    private int _GetSreachNumber(string str)
    {
        int res = 0;
        var matchs = MSearchedNumber.Match(str);
        if (matchs.Groups.Count > 0)
        {
            int.TryParse(matchs.Groups[0].Value, out res);
        }
        
        return res;
    }

    public enum EGUIFlag
    {
        None = 0,
        Search = 1,
        Ditail = 2,
        Error  = 4,
        OpOpen = 8,
        OpConvert = 16,
        OpReload = 32,
        OpNextPre = 64,
        All = Search | Ditail | Error | OpAll,
        OpAll = OpOpen | OpConvert | OpReload | OpNextPre,
        OpReadOnly = OpOpen | OpConvert | OpReload,
        ReadOnly = OpReadOnly | Ditail,
    }

    public int OnGUIWithID(int id, EGUIFlag flag = EGUIFlag.All)
    {
        if(skCurrentType == null)
        {
            var newID = EditorGUILayout.IntField(id, GUILayout.Width(120));

            if (newID != id)
            {
                id = newID;
            }
            return id;
        }

        if (mNodes.Count <= 0)
        {
            _UpdateCache();
        }

        if (CurrentID != id)
        {
            CurrentID = id;
        }

        EditorGUILayout.BeginVertical("GroupBox");
        {
            EditorGUILayout.BeginHorizontal();
            {
                //IsShowDetail = EditorGUILayout.Toggle(IsShowDetail, GUILayout.Width(35));
                //IsShowDetail = GUILayout.Toggle(IsShowDetail, EditorGUIUtility.IconContent("Animation.Play").image, GUILayout.Width(40), GUILayout.MinHeight(20));
                //EditorGUILayout.LabelField(mIDNameLabel, GUILayout.Width(20));

                if ((flag & EGUIFlag.Ditail) > 0)
                {
                    IsShowDetail = GUILayout.Toggle(IsShowDetail, mIDNameLabel, GUILayout.Width(mIDNameLabel.Length * 10 + 10));
                }
                else 
                {
                    IsShowDetail = false;
                }

                int idWidth = 80;
                if (CurrentID > 1000000)
                {
                    idWidth += 40;
                }

                if (CurrentIndex <= -1)
                {
                    _BeginRed();
                }

                var newID = EditorGUILayout.IntField(id, GUILayout.Width(idWidth));

                _EndRed();
                if (newID != id)
                {
                    id = newID;
                    CurrentID = id;
                }

                string des = string.Empty;
                if(CurrentIndex >= 0)
                {
                    var desSplit = mNodesCache[CurrentIndex].Split(':');
                    des = desSplit.Length <= 1 ? desSplit[0] : desSplit[desSplit.Length - 1];
                }

                EditorGUILayout.LabelField(des, GUILayout.Width(120));
                

                if (CurrentIndex <= -1 && (flag & EGUIFlag.OpReload) > 0)
                {
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, "重新加载"), GUILayout.Width(25), GUILayout.Height(20)))
                    {
                        TableManagerEditor.GetInstance().ReloadTable(skCurrentType);
                        mNodes.Clear();
                    }
                }
                else 
                {
                    EditorGUILayout.LabelField("", GUILayout.Width(25));
                }

                if ((flag & EGUIFlag.OpOpen) > 0 && GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Folder Icon").image, string.Format("打开{0}表格", XlsName)), GUILayout.Width(25), GUILayout.Height(20)))
                {
                    var array = Xls2FBWindow.GetXlsNamesBySheetName(skCurrentType.Name);
                    if (array.Length == 1)
                    {
                        Xls2FBWindow.OpenXlsByFileName(array[0]);
                    }
                    else
                    {
                        GenericMenu menu = new GenericMenu();
                        for (int i = 0; i < array.Length; i++)
                        {
                            menu.AddItem(new GUIContent(array[i]), false, obj =>
                            {
                                var name = obj as string;
                                Xls2FBWindow.OpenXlsByFileName(name);
                            }, array[i]);
                        }
                        menu.ShowAsContext();
                    }
                }

                if ((flag & EGUIFlag.OpConvert) > 0 && GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("RotateTool On").image, string.Format("转{0}表", XlsName)), GUILayout.Width(25), GUILayout.Height(20)))
                {
                    Xls2FBWindow.ConvertXlsBySheetName(skCurrentType.Name);
                    TableManagerEditor.GetInstance().ReloadTable(skCurrentType);
                    mNodes.Clear();
                }

                if ((flag & EGUIFlag.OpNextPre) > 0 && GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Animation.PrevKey").image), GUILayout.Width(25), GUILayout.Height(20)))
                {
                    CurrentIndex = Mathf.Clamp(CurrentIndex - 1, 0, mNodesCache.Length - 1);
                    id = CurrentID;
                }

                if ((flag & EGUIFlag.OpNextPre) > 0 && GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Animation.NextKey").image), GUILayout.Width(25), GUILayout.Height(20)))
                {
                    CurrentIndex = Mathf.Clamp(CurrentIndex + 1, 0, mNodesCache.Length - 1);
                    id = CurrentID;
                }


                //int idx = EditorGUILayout.Popup(CurrentIndex, mNodesCache, GUILayout.Width(500));

                /*int idx = EditorGUILayout.Popup(CurrentIndex, mNodesCache);
                if (idx != CurrentIndex)
                {
                    CurrentIndex = idx;
                    id = CurrentID;
                }*/
            }
            EditorGUILayout.EndHorizontal();

            if ((flag & EGUIFlag.Search) > 0)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("ViewToolZoom").image, string.Format("搜索")), GUILayout.Width(25), GUILayout.Height(20)))
                    {
                        if (!string.IsNullOrEmpty(mCurrentInput))
                        {
                            List<string> nodesCacheSearch = new List<string>();
                            int maxCount = 50;
                            int count = 0;
                            for (int i = 0; i < mNodesCache.Length; i++)
                            {
                                if(count >= maxCount)
                                {
                                    break;
                                }
                                if (mNodesCache[i].Contains(mCurrentInput))
                                {
                                    nodesCacheSearch.Add(mNodesCache[i]);
                                    count++;
                                }
                            }
                            mNodesCacheSearch = nodesCacheSearch.ToArray();

                            GenericMenu menu = new GenericMenu();
                            for (int i = 0; i < mNodesCacheSearch.Length; i++)
                            {
                                string node = mNodesCacheSearch[i];
                                menu.AddItem(new GUIContent(node), false, () =>
                                {
                                    int number = _GetSreachNumber(node);
                                    if (number != 0 && number != id)
                                    {
                                        mSearchedNumber = number;
                                    }
                                });
                            }
                            menu.ShowAsContext();
                        }
                    }
                    string search = EditorGUILayout.TextField(mCurrentInput, GUILayout.Width(150));
                    if (search != mCurrentInput)
                    {
                        mCurrentInput = search;
                    }

                }
                EditorGUILayout.EndHorizontal();
            }

            if (IsShowDetail && CurrentIndex >= 0)
            {
                if (null == mProperty)
                {
                    mProperty = new CacheProperty(this);
                }

                if (null != mProperty)
                {
                    mProperty.TableNode = CurrentTableNode;
                }

                mProperty.OnGUI();
            }

            if (null != Extension)
            {
                Extension.OnGUI(CurrentID);
            }

            if (id > 0 && (flag & EGUIFlag.Error) > 0 && CurrentIndex <= -1)
            {
                EditorGUILayout.HelpBox(string.Format("{0}无法在{1}:{2}中找到", id, XlsName, skCurrentType.Name), MessageType.Error);
            }
        }
        EditorGUILayout.EndVertical();

        if (mSearchedNumber != 0)
        {
            id = mSearchedNumber;
            mSearchedNumber = 0;
        }
        return id;
    }


    public bool IsShowDetail { get; set; }


    public enum PropertyType
    {
        None,
        Path,
        RefOtherTable,
        ID,
    }

    private class PropertyInfoNode
    {
        public PropertyInfo info;
        public PropertyType Type = PropertyType.None;
        public string cache;
        public string refType;
        public Texture texture;
    }

    private class CacheProperty
    {
        public List<PropertyInfoNode> Infos = null;
        private TableNode mNode = null;

        public string Filter { get; private set; }

        public TableNode TableNode
        {
            get
            {
                return mNode;
            }

            set
            {
                if (mNode == value)
                {
                    return;
                }

                mNode = value;

                if (null == mNode)
                {
                    return;
                }

                if (null == mNode.Data)
                {
                    mNode.Data = TableManagerEditor.GetInstance().GetTableItem(Type, mNode.ID);
                }

                foreach (var i in Infos)
                {
                    i.cache = Inspector._GetDesc(mNode.Data, i.info.Name);
                    i.texture = null;
                    if (i.info.Name == "ID")
                    {
                        i.Type = PropertyType.ID;
                    }
                    if (i.cache.Contains("/"))
                    {
                        i.Type = PropertyType.Path;
                        if (i.cache.EndsWith("png")
                         || i.cache.EndsWith("jpg"))
                        {
                            i.texture = _LoadUnityObject(i.cache) as Texture;
                        }
                    }
                    /*string n = Xls2FBWindow.GetTableBy(Type.Name, i.info.Name);
                    if (!string.IsNullOrEmpty(n))
                    {
                        i.Type = PropertyType.RefOtherTable;
                        i.refType = n;
                    }*/
                }

                //Infos.Sort((l, r) =>
                //{
                //    //if (r.Type != l.Type)
                //    //{
                //    //    return r.Type - l.Type;
                //    //}

                //    return 0;//l.info.Name.CompareTo(r.info.Name);
                //});
            }
        }

        public Type Type
        {
            get
            {
                return Inspector.skCurrentType;
            }
        }
        public TableInspetor Inspector { get; private set; }

        public CacheProperty(TableInspetor ins)
        {
            Inspector = ins;
            var ps = Type.GetProperties();
            Infos = new List<PropertyInfoNode>(ps.Length);
            for (int i = 0; i < ps.Length; ++i)
            {
                Infos.Add(new PropertyInfoNode());
                Infos[i].info = ps[i];
            }
        }

        private Vector2 mScroll;
        public void OnGUI()
        {
            if (null == Infos)
            {
                return;
            }

            mScroll = EditorGUILayout.BeginScrollView(mScroll, GUILayout.MinHeight(120), GUILayout.MaxHeight(500));
            foreach (var p in Infos)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _DrawOneField(p);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        void _DrawOneField(PropertyInfoNode p)
        {
            if (p.Type == PropertyType.Path)
            {
                EditorGUILayout.LabelField(p.info.Name, GUILayout.Width(140));

                bool isClick = false;
                if (null != p.texture)
                {
                    isClick = GUILayout.Button(p.texture, GUILayout.Width(100), GUILayout.Height(100));
                }
                else
                {
                    isClick = (GUILayout.Button("引用" + p.cache, GUILayout.MinWidth(300)));
                }

                if (isClick)
                {
                    var obj = _LoadUnityObject(p.cache);
                    if (null == obj)
                    {
                        EditorUtility.DisplayDialog("无法找到文件", p.cache, "ok");
                    }
                    else
                    {
                        p.texture = obj as Texture;
                        EditorGUIUtility.PingObject(obj);
                    }
                }
            }
            /*else if (p.Type == PropertyType.RefOtherTable)
            {
            EditorGUILayout.LabelField(p.info.Name, GUILayout.Width(140));
            if (GUILayout.Button("显示" + p.refType + p.cache, GUILayout.Width(300)))
            {
            int refID = 0;
            int.TryParse(p.cache, out refID);
            Xls2FBWindow.RefID = refID;
            Xls2FBWindow.RefType = p.refType;
            }
            }*/
            else
            {
                EditorGUILayout.LabelField(p.info.Name, p.cache);
            }
        }

        private void _TryOnPath()
        {

        }

        private UnityEngine.Object _LoadUnityObject(string file)
        {
            string newPath = "Assets/Resources/" + file;
            string altlasName = "";

            if (newPath.Contains(":"))
            {
                var sp = newPath.Split(':');
                newPath = sp[0];
                altlasName = sp[1];
            }

            string tagetPath = newPath;

            if (!newPath.Contains("."))
            {
                do
                {
                    tagetPath = newPath + ".prefab";
                    if (System.IO.File.Exists(tagetPath))
                    {
                        break;
                    }

                    tagetPath = newPath + ".asset";
                    if (System.IO.File.Exists(tagetPath))
                    {
                        break;
                    }

                    tagetPath = newPath + ".png";
                    if (System.IO.File.Exists(tagetPath))
                    {
                        break;
                    }

                    tagetPath = newPath + ".jpg";
                    if (System.IO.File.Exists(tagetPath))
                    {
                        break;
                    }

                } while (false);
            }


            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(tagetPath);

            return obj;
        }
    }


    CacheProperty mProperty = null;



    public ITableInspectorExtension Extension { get; private set; }


    private void _BeginRed()
    {
        GUI.color = Color.red;
    }

    private void _EndRed()
    {
        GUI.color = Color.white;
    }

    public void OnGUI()
    {
        if (mNodes.Count <= 0)
        {
            _UpdateCache();
        }

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(mIDNameLabel, GUILayout.Width(100));

            int idWidth = 100;
            if (CurrentID > 1000000)
            {
                idWidth += 50;
            }

            var newID = EditorGUILayout.IntField(CurrentID, GUILayout.Width(idWidth));
            if (newID != CurrentID)
            {
                CurrentID = newID;
            }

            int idx = EditorGUILayout.Popup(CurrentIndex, mNodesCache, GUILayout.Width(500));
            if (idx != CurrentIndex)
            {
                CurrentIndex = idx;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
