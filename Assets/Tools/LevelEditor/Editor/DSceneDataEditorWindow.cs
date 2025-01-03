using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;
using UnityEditor.AnimatedValues;
using System.IO;
using ProtoTable;
using Tenmove.Runtime.Unity;


[InitializeOnLoad]
public class DEditorTool
{
    static DEditorTool()
    {
        EditorApplication.playmodeStateChanged += DSceneDataEditorWindow.PreviewInPlayerHandle;
    }
}

static class DArrayTools
{
    public static T[] RemoveElement<T>(T[] elements, T element)
    {
        List<T> elementsList = new List<T>(elements);
        elementsList.Remove(element);
        return elementsList.ToArray();
    }

    public static T[] AddElement<T>(T[] elements, T element)
    {

        List<T> elementsList;


        if (elements == null)
        {
            elementsList = new List<T>();
        }
        else
        {
            elementsList = new List<T>(elements);
        }

        elementsList.Add(element);
        return elementsList.ToArray();
    }

    public static T[] InsertElement<T>(T[] elements, T element,int index)
    {

        List<T> elementsList;


        if (elements == null)
        {
            elementsList = new List<T>();
        }
        else
        {
            elementsList = new List<T>(elements);
        }

        elementsList.Insert(index,element);
        //elementsList.Add(element);
        return elementsList.ToArray();
    }

    public static T[] MoveElement<T>(T[] elements,ref int index,bool bUp)
    {
        if(elements == null)
        {
            return elements;
        }

        if(bUp)
        {
            if(index > 0)
            {
                T value = elements[index - 1];
                elements[index - 1] = elements[index];
                elements[index] = value;
                index -= 1;
            }
        }
        else
        {
            if(index < elements.Length - 1)
            {
                T value = elements[index + 1];
                elements[index + 1] = elements[index];
                elements[index] = value;
                index += 1;
            }

        }
        
        return elements;
    }
    public static DEntityInfo NewEntityInfo(this DSceneData data, DEntityType type, out int size)
    {
        switch (type)
        {
            case DEntityType.NPC:
                {
                    DNPCInfo info = new DNPCInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._npcinfo = AddElement<DNPCInfo>(data._npcinfo, info);
                    int lsize = data._npcinfo.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.MONSTER:
            case DEntityType.ELITE:
            case DEntityType.BOSS:
	        case DEntityType.MONSTERDESTRUCT:
			case DEntityType.ACTIVITY_BOSS_POS:
			case DEntityType.ACTIVITY_ELITE_POS:
			case DEntityType.ACTIVITY_MONSTER_POS:
                {
                    DMonsterInfo info = new DMonsterInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._monsterinfo = AddElement<DMonsterInfo>(data._monsterinfo, info);
                    int lsize = data._monsterinfo.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.DECORATOR:
                {
                    DDecoratorInfo info = new DDecoratorInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._decoratorinfo = AddElement<DDecoratorInfo>(data._decoratorinfo, info);
                    int lsize = data._decoratorinfo.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.DESTRUCTIBLE:
                {
                    DDestructibleInfo info = new DDestructibleInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._desructibleinfo = AddElement<DDestructibleInfo>(data._desructibleinfo, info);
                    int lsize = data._desructibleinfo.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.REGION:
                {
                    DRegionInfo info = new DRegionInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._regioninfo = AddElement<DRegionInfo>(data._regioninfo, info);
                    int lsize = data._regioninfo.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.TRANSPORTDOOR:
                {
                    DTransportDoor info = new DTransportDoor();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._transportdoor = AddElement<DTransportDoor>(data._transportdoor, info);
                    int lsize = data._transportdoor.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.TOWNDOOR:
                {
                    DTownDoor info = new DTownDoor();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._townDoor = AddElement<DTownDoor>(data._townDoor, info);
                    int lsize = data._townDoor.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.HELLBIRTHPOSITION:
                {
                    DEntityInfo info = new DEntityInfo();
                    info.type = type;
                    info.typename = type.GetDescription();

                    if (data._hellbirthposition != null)
                    {
                        // remove from
                        //entityList.RemoveItems(data._birthposition);

                    }

                    data._hellbirthposition = info;

                    size = 1;
                    return info;
                }

            case DEntityType.BIRTHPOSITION:
                {
                    DEntityInfo info = new DEntityInfo();
                    info.type = type;
                    info.typename = type.GetDescription();

                    if (data._birthposition != null)
                    {
                        // remove from
                        //entityList.RemoveItems(data._birthposition);

                    }

                    data._birthposition = info;

                    size = 1;
                    return info;
                }

            case DEntityType.FUNCTION_PREFAB:
                {
                    FunctionPrefab info = new FunctionPrefab();
                    info.type = type;
                    info.typename = type.GetDescription();

                    data._FunctionPrefab = AddElement<FunctionPrefab>(data._FunctionPrefab, info);
                    int lsize = data._FunctionPrefab.Length;
                    size = lsize;
                    return info;
                }
            case DEntityType.FIGHTER_BORN_POS:
                {
                    var info = new DTransferInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._fighterBornPosition = AddElement<DTransferInfo>(data._fighterBornPosition, info);
                    info.color = Color.cyan;
                    size = data._fighterBornPosition.Length;
                    return info;
                }
            case DEntityType.RESOURCE_POS:
                {
                    var info = new DResourceInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._resourcePosition = AddElement<DResourceInfo>(data._resourcePosition, info);
                    info.color = Color.blue;
                    size = data._resourcePosition.Length;
                    return info;
                }
            case DEntityType.ECOSYSTEM_RESOURCE_POS:
                {
                    var info = new DEcosystemResourceInfo();
                    info.type = type;
                    info.typename = type.GetDescription();
                    data._ecosystemResoucePosition = AddElement<DEcosystemResourceInfo>(data._ecosystemResoucePosition, info);
                    info.color = Color.yellow;
                    size = data._resourcePosition.Length;
                    return info;
                }
            default:
                Debug.LogErrorFormat("未处理的类型 {0}", type);

                break;
        }

        size = 0;
        return null;
    }

    public static void DeleteEntityInfo(this DSceneData data, DEntityInfo info)
    {
        data._entityinfo = RemoveElement<DEntityInfo>(data._entityinfo, info);
        switch (info.type)
        {
            case DEntityType.NPC:
                {
                    data._npcinfo = RemoveElement<DNPCInfo>(data._npcinfo, info as DNPCInfo);
                    return;
                }
            case DEntityType.MONSTER:
            case DEntityType.BOSS:
            case DEntityType.ELITE:
            case DEntityType.MONSTERDESTRUCT:
			case DEntityType.ACTIVITY_BOSS_POS:
			case DEntityType.ACTIVITY_ELITE_POS:
			case DEntityType.ACTIVITY_MONSTER_POS:
                {
                    data._monsterinfo = RemoveElement<DMonsterInfo>(data._monsterinfo, info as DMonsterInfo);
                    return;
                }
            case DEntityType.DECORATOR:
                {
                    data._decoratorinfo = RemoveElement<DDecoratorInfo>(data._decoratorinfo, info as DDecoratorInfo);
                    return;
                }
            case DEntityType.DESTRUCTIBLE:
                {
                    data._desructibleinfo = RemoveElement<DDestructibleInfo>(data._desructibleinfo, info as DDestructibleInfo);
                    return;
                }
            case DEntityType.REGION:
                {
                    data._regioninfo = RemoveElement<DRegionInfo>(data._regioninfo, info as DRegionInfo);
                    return;
                }
            case DEntityType.TRANSPORTDOOR:
                {
                    data._transportdoor = RemoveElement<DTransportDoor>(data._transportdoor, info as DTransportDoor);
                    return;
                }
            case DEntityType.TOWNDOOR:
                {
                    data._townDoor = RemoveElement<DTownDoor>(data._townDoor, info as DTownDoor);
                    return;
                }
            case DEntityType.HELLBIRTHPOSITION:
                {
                    data._hellbirthposition = null;
                    return;
                }
            case DEntityType.BIRTHPOSITION:
                {
                    data._birthposition = null;
                    return;
                }
            case DEntityType.FUNCTION_PREFAB:
                {
                    data._FunctionPrefab = RemoveElement<FunctionPrefab>(data._FunctionPrefab, info as FunctionPrefab);
                    return;
                }
            case DEntityType.RESOURCE_POS:
                {
                    data._resourcePosition = RemoveElement<DResourceInfo>(data._resourcePosition, info as DResourceInfo);
                    return;
                }
            case DEntityType.FIGHTER_BORN_POS:
                {
                    data._fighterBornPosition = RemoveElement<DTransferInfo>(data._fighterBornPosition, info as DTransferInfo);
                    return;
                }
            case DEntityType.ECOSYSTEM_RESOURCE_POS:
                {
                    data._ecosystemResoucePosition = RemoveElement<DEcosystemResourceInfo>(data._ecosystemResoucePosition, info as DEcosystemResourceInfo);
                    return;
                }
            default:
                Debug.LogErrorFormat("未处理的类型 {0}", info.type);
                break;
        }
    }
}

public class EntityConfigInfo
{
    public EntityConfigInfo(DEntityType type, string path, string name, string description, int id)
    {
        this.type = type;
        this.resourcepath = path;
        this.name = name;
        this.description = description;
        this.id = id;
    }

    public DEntityType type;
    public string resourcepath;
    public string name;
    public string description;
    public int id;
}

public class EntityFunctionDataInfo : EntityConfigInfo
{
    Vector3 kPostion;

    public EntityFunctionDataInfo(DEntityType type, string path, string name, string description, int id)
        : base(type, path, name, description, id)
    {
        kPostion = Vector3.zero;
    }

    public Vector3 Position
    {
        get { return kPostion; }
        set { kPostion = value; }
    }
}

public class EntityMonsterInfo : EntityConfigInfo
{

    public EntityMonsterInfo(DEntityType type, string path, string name, string description, int id)
        : base(type, path, name, description, id)
    {
        _decodeID();
    }

    #region Const
    private const int kDiffculte = 10;
    private const int kTypeID = 10;
    private const int kLevel = 100;
    private const int kID = 10000;
    #endregion

    #region Get && Set
    private int monsterID;
    private int monsterLevel;
    private int monsterTypeID;
    private int monsterDiffcute;

    public int monID
    {
        get
        {
            return monsterID;
        }

        set
        {
            if (monsterID != value && value > 0)
            {
                monsterID = value;
                _encodeID();
            }
        }
    }

    public int monLevel
    {
        get
        {
            return monsterLevel;
        }

        set
        {
            if (monsterLevel != value && value > 0)
            {
                monsterLevel = value;
                _encodeID();
            }
        }
    }

    public int monTypeID
    {
        get
        {
            return monsterTypeID;
        }

        set
        {
            if (monsterTypeID != value && value > 0)
            {
                monsterTypeID = value;
                _encodeID();
            }
        }
    }

    public int monDiffcute
    {
        get
        {
            return monsterDiffcute;
        }

        set
        {
            if (monsterDiffcute != value && value > 0)
            {
                monsterDiffcute = value;
                _encodeID();
            }
        }
    }


    public void SetID(int id)
    {
        this.id = id;

        _decodeID();
    }
    #endregion

    #region Decode && Encode
    private void _decodeID()
    {
        var tid = id;
        monsterDiffcute = tid % kDiffculte;
        tid /= kDiffculte;

        monsterTypeID = tid % kTypeID;
        tid /= kTypeID;

        monsterLevel = tid % kLevel;
        tid /= kLevel;

        monsterID = tid;
    }

    private void _encodeID()
    {
        id = monsterID;

        id *= kLevel;
        id += monsterLevel % kLevel;

        id *= kTypeID;
        id += monsterTypeID % kTypeID;

        id *= kDiffculte;
        id += monsterDiffcute % kDiffculte;
    }
    #endregion
}


public class DSceneDataEditorWindow : EditorWindow
{

    #region Property
    readonly static string EDITOR_V = "V1.2Alpha";
    static DSceneDataEditorWindow levelDataEditorWindow;

    private GameObject editorRoot;
    private GameObject sceneRoot;
    private GameObject entityRoot;
    private Editor editorproxy;

    private DSceneData sceneData;
    public DSceneData sendData;
    public DDungeonData dugeonData;
    public Editor dugeonDataEditor;
    public int editorSelectX;
    public int editorSelectY;
    public bool mAutoLoadSceneData = false;
    public DDungeonData mParamDungeonData = null;

    private string mSearchString;
    private string mSearchListString;
    private bool mSearchDirty = false;

    public DSceneDataEditorDrawer proxy;

    private Dictionary<DEntityType, EditorUI.UIListItem> mEntityListTypeDict = new Dictionary<DEntityType, EditorUI.UIListItem>();

    private Vector2 _GridSizeBack;
    private Vector2 _LogicXSizeBack;
    private Vector2 _LogicZSizeBack;

    private Texture2D _GridTexture;
    private Texture2D _BlockTexture;

    private GUIStyle fontStyle;
    private GUIStyle fontStyle2;
    private GUIStyle fontStyle3;

    private bool dataWarning = false;
    private string errorMsg;

    private bool showtoggle = true;

    protected bool bInfoToggle = true;
    protected bool bMonsterResourceToggle = true;
    protected bool bMonsterListToggle = true;
    protected bool bEntityEditor = false;
    protected bool bEntityGroupEditor = false;
    private int globalLevel = 1;

    protected bool bMonsterInfoToggle = true;

    protected bool bCameraSetting = false;

    protected int MonsterResourceHeight;
    protected int MonsterListHeight;

    protected ushort grassId = 0;
    protected ushort curPointGrassId = 0;

    protected ushort ecosystemId = 0;
    protected ushort curEcosytemId = 0;

    //protected EditorUI.UIListItem[] item_root = new EditorUI.UIListItem[(int)DEntityType.MAX];

    int MonsterPicSize = 200;
    EditorUI.GUIVerticalFlexBlock MonsterResoucrceBlock;
    EditorUI.GUIVerticalFlexBlock MonsterListBlock;

    public bool m_bInArtistMode = false;
    public bool bInArtistMode
    {
        get { return m_bInArtistMode; }
        set
        {
            if (value != m_bInArtistMode && proxy != null)
            {
                proxy.SetArtistMode(value);
                m_bInArtistMode = value;
            }
        }
    }

    #endregion

    public enum LevelEditorTags
    {
        Dungoen = 1,
        SceneData = 0,
    }

    LevelEditorTags editorTags;

    #region Config
    protected void SaveMonsterConfig()
    {
        EditorPrefs.SetInt("LevelDataEditor.MonsterResourceHeight", MonsterResourceHeight);
        EditorPrefs.SetInt("LevelDataEditor.MonsterListHeight", MonsterListHeight);
    }

    protected void LoadMonsterConfig()
    {
        if (MonsterResoucrceBlock != null || MonsterListBlock != null)
        {
            return;
        }

        MonsterResourceHeight = EditorPrefs.GetInt("LevelDataEditor.MonsterResourceHeight");
        MonsterListHeight = EditorPrefs.GetInt("LevelDataEditor.MonsterListHeight");

        if (MonsterResourceHeight == 0)
        {
            MonsterResourceHeight = 200;
        }

        if (MonsterListHeight == 0)
        {
            MonsterListHeight = 300;
        }

        MonsterResoucrceBlock = new EditorUI.GUIVerticalFlexBlock(MonsterResourceHeight, MonsterPicSize);
        MonsterListBlock = new EditorUI.GUIVerticalFlexBlock(MonsterListHeight);
    }
    #endregion

    #region Static Tool -- Create
    [MenuItem("[关卡编辑器]/区域/新建区域数据", false, 1)]
    public static void Create()
    {
        DSceneData data = FileTools.CreateAsset<DSceneData>("SceneData");
        data._CameraNearClip = Global.Settings.battleCameraNearClip;
        data._CameraFarClip = Global.Settings.battleCameraFarClip;
        DSceneDataEditorWindow.Init();
    }

    [MenuItem("[关卡编辑器]/区域/编辑区域数据", false, 1)]
    public static void Init()
    {
        if (levelDataEditorWindow == null)
            levelDataEditorWindow = EditorWindow.GetWindow<DSceneDataEditorWindow>(false,
                    "区域编辑器" + DSceneDataEditorWindow.EDITOR_V, true);

        levelDataEditorWindow.Show();
        EditorWindow.FocusWindowIfItsOpen<SceneView>();
        //BackSceneCamera(levelDataEditorWindow);
        levelDataEditorWindow.Populate();
        //levelDataEditorWindow.Initial();
    }
    public static void ResetAutoLoadSceneData(bool autoLoadSceneData)
    {
        if (levelDataEditorWindow != null)
        {
            levelDataEditorWindow.mAutoLoadSceneData = autoLoadSceneData;
        }
    }

    public static void SetSendData(DSceneData data, DDungeonData dungeonData, int x, int y, bool autoLoadSceneData = false, DDungeonData dungeonDataParam = null)
    {
        if (levelDataEditorWindow == null)
            levelDataEditorWindow = EditorWindow.GetWindow<DSceneDataEditorWindow>(false,
                    "区域编辑器" + DSceneDataEditorWindow.EDITOR_V, true);

        levelDataEditorWindow.Show();
        EditorWindow.FocusWindowIfItsOpen<SceneView>();
        //BackSceneCamera(levelDataEditorWindow);

        levelDataEditorWindow.Clear(true, true);
        levelDataEditorWindow.dugeonData = dungeonData;
        levelDataEditorWindow.editorSelectX = x;
        levelDataEditorWindow.editorSelectY = y;
        levelDataEditorWindow.mAutoLoadSceneData = autoLoadSceneData;
        levelDataEditorWindow.mParamDungeonData = dungeonDataParam;
        levelDataEditorWindow.SendData = data;

    }
    #endregion

    public ResTable GetResTableByID(int resID)
    {
        return TableManagerEditor.instance.GetTableItem<ResTable>(resID);
    }

    // DEntityType -> UnitTable.Type
    Dictionary<DEntityType, int> TypeMap = new Dictionary<DEntityType, int>
    {
        {DEntityType.MONSTER,         (int)UnitTable.eType.MONSTER},
        {DEntityType.BOSS,            (int)UnitTable.eType.BOSS},
        {DEntityType.ELITE,           (int)UnitTable.eType.ELITE},
		{DEntityType.ACTIVITY_MONSTER_POS,         (int)UnitTable.eType.MONSTER},
		{DEntityType.ACTIVITY_BOSS_POS,            (int)UnitTable.eType.BOSS},
		{DEntityType.ACTIVITY_ELITE_POS,           (int)UnitTable.eType.ELITE},
        {DEntityType.MONSTERDESTRUCT, (int)UnitTable.eType.SKILL_MONSTER},
        {DEntityType.NPC,             (int)UnitTable.eType.NPC},
		{DEntityType.DESTRUCTIBLE,    (int)UnitTable.eType.HELL},
        {DEntityType.DECORATOR,       0},
        {DEntityType.REGION,          -1},
    };

    public List<EntityConfigInfo> GetEntityListByType(DEntityType type)
    {
        List<EntityConfigInfo> configInfo = new List<EntityConfigInfo>();

        if (type == DEntityType.REGION)
        {
            var tables = TableManagerEditor.instance.GetTable<SceneRegionTable>();
            if (null != tables)
            {
                foreach (var item in tables)
                {
                    var data = item.Value as SceneRegionTable;
                    if (data.Type == SceneRegionTable.eType.BUFF ||
                            data.Type == SceneRegionTable.eType.TRAP)
                    {
                        var resData = GetResTableByID(data.ResID);
                        if (null == resData)
                        {
                            continue;
                        }

                        configInfo.Add(new EntityConfigInfo(type, resData.ModelPath + ".prefab", data.Name, "", data.ID));
                    }
                }
            }
        }
        else if (type == DEntityType.TRANSPORTDOOR)
        {
            var tables = TableManagerEditor.instance.GetTable<SceneRegionTable>();
            if (null != tables)
            {
                foreach (var item in tables)
                {
                    var data = item.Value as SceneRegionTable;
                    if (data.Type != SceneRegionTable.eType.DOOR)
                    {
                        continue;
                    }

                    var resData = GetResTableByID(data.ResID);
                    if (null == resData)
                    {
                        continue;
                    }

                    configInfo.Add(new EntityConfigInfo(type, resData.ModelPath + ".prefab", data.Name, "", data.ID));
                }
            }

        }
        else if (type == DEntityType.TOWNDOOR)
        {
            var tables = TableManagerEditor.instance.GetTable<SceneRegionTable>();
            if (null != tables)
            {
                foreach (var item in tables)
                {
                    var data = item.Value as SceneRegionTable;
                    if (data.Type != SceneRegionTable.eType.TOWNDOOR)
                    {
                        continue;
                    }

                    var resData = GetResTableByID(data.ResID);
                    if (null == resData)
                    {
                        continue;
                    }

                    configInfo.Add(new EntityConfigInfo(type, resData.ModelPath + ".prefab", data.Name, "", data.ID));
                }
            }
        }
        else if (type == DEntityType.HELLBIRTHPOSITION)
        {
            configInfo.Add(new EntityConfigInfo(type, "", "深渊出生点", "", 0));
        }
        else if (type == DEntityType.BIRTHPOSITION)
        {
            configInfo.Add(new EntityConfigInfo(type, "", "出生点", "", 0));
        }
        else if (type == DEntityType.RESOURCE_POS)
        {
            configInfo.Add(new EntityConfigInfo(type, "", "吃鸡道具投放点", "", 0));
        }
        else if (type == DEntityType.FIGHTER_BORN_POS)
        {
            configInfo.Add(new EntityConfigInfo(type, "Sphere", "吃鸡人物降落点", "", 0));
        }
        else if(type == DEntityType.ECOSYSTEM_RESOURCE_POS)
        {
            configInfo.Add(new EntityConfigInfo(type, "Sphere", "生态资源点", "", 0));
        }
        else if (type == DEntityType.NPC)
        {
            var tables = TableManagerEditor.instance.GetTable<NpcTable>();
            if (null != tables)
            {
                foreach (var item in tables)
                {
                    var data = item.Value as NpcTable;
                    var resData = GetResTableByID(data.ResID);
                    if (null == resData)
                    {
                        continue;
                    }

                    configInfo.Add(new EntityConfigInfo(type, resData.ModelPath + ".prefab", data.NpcName, "", data.ID));
                }
            }
        }
        else if (type == DEntityType.DECORATOR)
        {
            var tables = TableManagerEditor.instance.GetTable<ResTable>();
            if (null != tables)
            {
                foreach (var item in tables)
                {
                    var data = item.Value as ResTable;
                    if (null == data)
                    {
                        continue;
                    }

                    if (data.Type != ResTable.eType.DECORATOR)
                    {
                        continue;
                    }

                    configInfo.Add(new EntityConfigInfo(type, data.ModelPath + ".prefab", data.Name, "", data.ID));
                }
            }
        }
        else if (type == DEntityType.DESTRUCTIBLE)
        {
            var tables = TableManagerEditor.instance.GetTable<DestrucTable>();
            if (null != tables)
            {
                foreach (var item in tables)
                {
                    var data = item.Value as DestrucTable;
                    var resData = GetResTableByID(data.Mode);
                    if (null == resData)
                    {
                        continue;
                    }
                    configInfo.Add(new EntityConfigInfo(type, resData.ModelPath + ".prefab", data.Name, data.Desc, data.ID));
                }
            }
        }
        else if(type == DEntityType.FUNCTION_PREFAB)
        {
            configInfo.Add(new EntityConfigInfo(type, "", "功能预设", "空的功能数据", 0));
        }

        else
        {
            var tables = TableManagerEditor.instance.GetTable<UnitTable>();
            if (null != tables)
            {
                foreach (var item in tables)
                {
                    var data = item.Value as UnitTable;
                    var resData = GetResTableByID(data.Mode);
                    if (null == resData)
                    {
                        continue;
                    }

                    if ((int)data.Type == TypeMap[type])
                    {
                        if (type == DEntityType.MONSTER
                         || type == DEntityType.ELITE 
                         || type == DEntityType.BOSS
						 || type == DEntityType.ACTIVITY_MONSTER_POS
						 || type == DEntityType.ACTIVITY_ELITE_POS
						 || type == DEntityType.ACTIVITY_BOSS_POS
                         || type == DEntityType.MONSTERDESTRUCT)
                        {
                            var tmp = new EntityMonsterInfo(type, resData.ModelPath + ".prefab", data.Name, data.Desc, data.ID);
                            var fItem = configInfo.Find(x =>
                            {
                                var sitem = x as EntityMonsterInfo;
                                if (sitem != null && sitem.monID == tmp.monID)
                                {
                                    return true;
                                }
                                return false;
                            });

                            if (null == fItem)
                            {
                                configInfo.Add(tmp);
                            }
                        }
                        else
                        {
                            configInfo.Add(new EntityConfigInfo(type, resData.ModelPath + ".prefab", data.Name, data.Desc, data.ID));
                        }
                    }
                }
            }
        }

        return configInfo;
    }

    private EditorUI.UIListItem _addExpandNode(EditorUI.UIListBox boxList, string[] names, bool isExpand = true, EditorUI.UIListItem parent = null)
    {
        var node = boxList.AddItems(names, parent);
        node.isexpand = isExpand;
        return node;
    }

    private EditorUI.UIListItem _addTypeNodes(EditorUI.UIListBox boxList, DEntityType[] type, bool isExpand = true)
    {
        var root = _addExpandNode(boxList, new string[] { type[0].GetDescription() }, isExpand);
        var subRoot = root;

        EntityConfigInfo[] infos = null;

        bool isSubRoot = type.Length > 1;

        for (int j = 0; j < type.Length; ++j)
        {
            if (isSubRoot)
                subRoot = _addExpandNode(boxList, new string[] { type[j].GetDescription() }, isExpand, root);

            infos = GetEntityListByType(type[j]).ToArray();
            for (int i = 0; i < infos.Length; ++i)
            {
                var child = entityResourceList.AddItems(new string[] { infos[i].name, infos[i].description }, subRoot);
                child.userdata = infos[i];
            }
        }

        return root;
    }

    public void CreateUI()
    {
        if (entityResourceList == null)
        {
            entityResourceList = new EditorUI.UIListBox(this);
            entityResourceList.SetTitles(new string[] { "名字", "描述" }, new float[] { 0.6f, 0.4f });
            entityResourceList.s_entryOdd = entityResourceList.s_entryEven;

            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.BIRTHPOSITION }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.HELLBIRTHPOSITION }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.NPC }, false);
			_addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.MONSTER, DEntityType.ELITE, DEntityType.BOSS, DEntityType.MONSTERDESTRUCT, DEntityType.ACTIVITY_MONSTER_POS, DEntityType.ACTIVITY_ELITE_POS, DEntityType.ACTIVITY_BOSS_POS }, true);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.DECORATOR }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.DESTRUCTIBLE }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.REGION, DEntityType.TRANSPORTDOOR, DEntityType.TOWNDOOR }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.FUNCTION_PREFAB }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.RESOURCE_POS }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.FIGHTER_BORN_POS }, false);
            _addTypeNodes(entityResourceList, new DEntityType[] { DEntityType.ECOSYSTEM_RESOURCE_POS }, false);

            entityResourceList.dragCallBack = (EditorUI.UIListItem dragitem) =>
            {

                if (dragitem != null && dragitem.userdata != null)
                {
                    EntityConfigInfo info = dragitem.userdata as EntityConfigInfo;
                    SceneView.FocusWindowIfItsOpen<SceneView>();
                    DragAndDrop.PrepareStartDrag();
                    var obj = LoadAssetAtPath(info.resourcepath);
                    DragAndDrop.objectReferences = new UnityEngine.Object[] { obj };
                    DragAndDrop.SetGenericData("MonsterInfo", info);
                    DragAndDrop.StartDrag("");
                }
            };

        }

        if (entityList == null)
        {

            entityList = new EditorUI.UIListBox(this);
            entityList.SetTitles(new string[] { "name", "type", "description" }, new float[] { 0.3f, 0.3f, 0.4f });

            entityList.deleteCallBack = (EditorUI.UIListItem dragitem) =>
            {
                if (dragitem.hasChildren)
                {
                    if (EditorUtility.DisplayDialog("警告!!", string.Format("确认删除所有*{0}*节点及其子节点?", dragitem.texts), "是", "否"))
                    {
                        var iter = dragitem.childrenroot;
                        while (iter != null)
                        {
                            if (null != iter.userdata)
                            {
                                DEntityInfo userinfo = iter.userdata as DEntityInfo;
                                sceneData.DeleteEntityInfo(userinfo);

                                EntityDataStruct userdata = FindEntityData(userinfo);
                                if (userdata != null) userdata.item = null;
                                RemoveEntityData(userdata, true);
                                iter.userdata = null;
                            }

                            var titer = iter.next;
                            entityList.RemoveItems(iter);
                            iter = titer;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (dragitem.userdata is DEntityInfo)
                {
                    DEntityInfo info = dragitem.userdata as DEntityInfo;
                    if (info == sceneData._birthposition)
                    {
                        return;
                    }

                    sceneData.DeleteEntityInfo(info);
                    EntityDataStruct data = FindEntityData(info);
                    if (data != null) data.item = null;
                    RemoveEntityData(data, true);
                    dragitem.userdata = null;
                }
                else if (dragitem.userdata is DEntityType)
                {
                    mEntityListTypeDict.Remove((DEntityType)dragitem.userdata);
                }

                entityList.RemoveItems(dragitem);
            };

            entityList.duplicateCallBack = (EditorUI.UIListItem dragitem) =>
            {
                if (dragitem == null)
                {
                    return;
                }

                if (dragitem.userdata is DEntityInfo)
                {
                    DEntityInfo info = dragitem.userdata as DEntityInfo;
                    DuplicateEntity(info);
                }
            };

            /*entityList.onSelectionChange = (EditorUI.UIListItem dragitem) =>
            {
                if (dragitem.userdata is DEntityInfo)
                {
                    DEntityInfo info = dragitem.userdata as DEntityInfo;
                    DSceneEntitySelection.Select(new []{info});
                    DEditorCamera.SetPosition(info.position);
                }
            };*/

            entityList.onSelectionsChange = (items) =>
            {
                List<DEntityInfo> list = new List<DEntityInfo>();
                foreach (var item in items)
                {
                    if (item.userdata is DEntityInfo)
                    {
                        list.Add(item.userdata as DEntityInfo);
                    }
                }

                if (list.Count > 0)
                {
                    if (!m_isCtrlSelect)
                    {
                        DEditorCamera.SetPosition(list[list.Count - 1].position);
                    }
                    
                    DSceneEntitySelection.Select(list.ToArray());
                }
               
                /*if (items.userdata is DEntityInfo)
                {
                    DEntityInfo info = dragitem.userdata as DEntityInfo;
                    DSceneEntitySelection.Select(info);
                    DEditorCamera.SetPosition(info.position);
                }*/
            };
        }
    }

    bool CheckSelectedIsDDugeon()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DDungeonData), UnityEditor.SelectionMode.Assets);
        if (selection.Length > 0)
        {
            FocusWindowIfItsOpen<DSceneDataEditorWindow>();
            //SetSendData(null,selection[0] as DDungeonData,0,0);
            dugeonData = selection[0] as DDungeonData;
            editorSelectX = 0;
            editorSelectY = 0;
            SendData = null;

            return true;
        }
        return false;
    }

    bool CheckSelectedNoChange()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DSceneData), UnityEditor.SelectionMode.Assets);
        if (selection.Length > 0)
        {
            FocusWindowIfItsOpen<DSceneDataEditorWindow>();
            return sceneData == (DSceneData)selection[0];
        }
        return true;
    }
    GameObject FindEntityRoot(GameObject obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj.GetComponent<DEntityInfoComponent>() != null)
        {
            return obj;
        }

        if (obj.transform.parent == null)
        {
            return null;
        }

        return FindEntityRoot(obj.transform.parent.gameObject);
    }

    bool SelectedDirty = false;
    GameObject SelectedGameObject = null;

    bool FindEntitySelection()
    {
        if (proxy == null)
        {
            return false;
        }

        if (proxy.editmode != SceneEditorMode.ENTITYS)
        {
            return false;
        }

        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            GameObject obj = FindEntityRoot(selection[0] as GameObject);
            if (obj.GetComponent<DEntityInfoComponent>() != null)
            {
                SelectedDirty = true;
                SelectedGameObject = obj;
                //UpdateEntitySelectionDrity();
                SceneView.RepaintAll();
            }
            return true;
        }

        return false;
    }

    public DSceneData SendData
    {
        set
        {
            //if( sendData != value) 
            {
                sendData = value;
                OnReview(true);
            }
        }
    }

    void OnReview(bool bSend = false)
    {
        //Clear(true, false);
        Populate(bSend);
        Repaint();
        Clear(false, false);
        Preview();
    }

    void OnSelectionChange()
    {
        //FindEntitySelection();

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Clear(true, false);
            return;
        }

        if (CheckSelectedIsDDugeon())
        {
            return;
        }
        if (CheckSelectedNoChange())
        {
            return;
        }

        Clear(true, false);
        OnReview();
    }

    void ResetCamera()
    {
        DEditorCamera.SetPosition(sceneData._CenterPostionNew + Vector3.up);
        DEditorCamera.SetRotation(Quaternion.Euler(20, 0, 0));
        DEditorCamera.SetOrthographic(true);
        DEditorCamera.SetSize(8);

        UpdateEditorCamera();
    }

    void PreviewInPlay()
    {
        Global.Settings.startSystem = GameClient.EClientSystem.Battle;
        Global.Settings.scenePath = EditorUtility.GetAssetPath(sceneData).Replace("Assets/Resources/", "");
        Global.Settings.isLocalDungeon = false;
        Global.Settings.isCreateMonsterLocal = true;

        if (!EditorApplication.isPlaying)
        {
            EditorApplication.playmodeStateChanged += PreviewInPlayerHandle;
            EditorApplication.isPlaying = true;
        }
        else
        {
            // TODO there will 
        }
    }

    public static void PreviewInPlayerHandle()
    {
        if (!EditorApplication.isPlaying)
        {
            Global.Settings.isCreateMonsterLocal = false;
        }

        //Debug.LogError("curstate " + EditorApplication.isPlaying);
        //Debug.LogError("curstate " + EditorApplication.isPlayingOrWillChangePlaymode);

        //EditorApplication.playmodeStateChanged -= PreviewInPlayerHandle;
    }

    void Preview()
    {
        if (sceneData == null)
        {
            return;
        }

        if (sceneData._prefab == null)
        {
            UnityEngine.Debug.Log("Drag a scene into 'Scene Prefab' first.");
            return;
        }
        else if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            UnityEngine.Debug.Log("You can't preview scene while in play mode.");
            return;
        }

        CreateScene();
        InitEntityData();
        ResetCamera();
    }

    void ClearEntityData()
    {
        for (int i = 0; i < entityData.Count; ++i)
        {
            RemoveEntityData(entityData[i].info, true);
        }

        if (entityList != null)
        {
            entityList.Clear();
        }

        if (entityData != null)
        {
            entityData.Clear();
        }

        if (mainCamera)
        {
            mainCamera.transform.localPosition = Vector3.zero;
        }

        mainCameraObj = null;
        mainCamera = null;
        //animCameraPostion = new AnimVector3();

        DSceneEntitySelection.Select(null);
    }

    void Clear(bool bDestoryChar, bool bResetCamera)
    {
        if (bDestoryChar)
        {
            if (editorRoot)
            {
                Editor.DestroyImmediate(editorRoot);
                editorRoot = null;
                entityRoot = null;
                sceneRoot = null;
            }

            if (dugeonDataEditor)
            {
                Editor.DestroyImmediate(dugeonDataEditor);
                dugeonDataEditor = null;
            }

            sendData = null;
            dugeonData = null;
            mParamDungeonData = null;
            ClearEntityData();

            // clear the type dict
            mEntityListTypeDict.Clear();

            UnLockTools();
            if (gameObjPreview != null)
            {
                Editor.DestroyImmediate(gameObjPreview);
                gameObjPreview = null;
            }

            if (entityEditor != null)
            {
                Editor.DestroyImmediate(entityEditor);
                entityEditor = null;
            }
        }
    }

    static public Ray Screen2Ray(Vector2 mousePos, SceneView sceneview)
    {
        return sceneview.camera.ScreenPointToRay(new Vector3(mousePos.x, sceneview.camera.pixelHeight - mousePos.y));
    }

    static public Vector3 ScreenPickGround(SceneView sceneview)
    {
        Event evt = Event.current;
        Vector2 mousePos = evt.mousePosition;

        Ray r = Screen2Ray(mousePos, sceneview);

        float fdis = -(r.origin.y / r.direction.y);
        return r.GetPoint(fdis);
    }

    class EntityDataStruct
    {
        public DEntityInfo info;
        public GameObject obj;
        public EditorUI.UIListItem item;
    }

    private List<EntityDataStruct> entityData = new List<EntityDataStruct>();

    private EntityDataStruct FindEntityData(GameObject obj)
    {
        for (int i = 0; i < entityData.Count; ++i)
        {
            if (entityData[i].obj == obj)
            {
                return entityData[i];
            }
        }

        return null;
    }
    private EntityDataStruct FindEntityData(DEntityInfo info)
    {
        for (int i = 0; i < entityData.Count; ++i)
        {
            if (entityData[i].info == info)
            {
                return entityData[i];
            }
        }

        return null;
    }
    private EntityDataStruct FindEntityData(EditorUI.UIListItem item)
    {
        for (int i = 0; i < entityData.Count; ++i)
        {
            if (entityData[i].item == item)
            {
                return entityData[i];
            }
        }

        return null;
    }
    private void _ChangeEntityPrefab(DEntityInfo info, EntityDataStruct userData,string path)
    {
        info.path = path + ".prefab";
        var obj = CreateEntityObject(info);
        if (obj != null)
        {
            SetDyeColor(info.Color, new GameObject[] { obj });
        }
        obj.GetComponent<DEntityInfoComponent>().info = info;
        if (obj != null)
        {
            obj.transform.SetParent(entityRoot.transform, false);
            obj.transform.localScale = info.Scale * Vector3.one;
            SetDyeColor(info.Color, new GameObject[] { obj });
        }
        if (info.obj != userData.obj)
        {
            Editor.DestroyImmediate(userData.obj);
        }
        userData.obj = null;
        Editor.DestroyImmediate(info.obj);
        info.obj = null;
        info.obj = obj;
        userData.obj = obj;
    }

    private void ResetEntityPrefab()
    {
        for (int i = 0; i < sceneData._monsterinfo.Length; i++)
        {
            var monster = sceneData._monsterinfo[i];
            EntityDataStruct userdata = FindEntityData(monster);
            if (userdata == null) continue;
            MonsterIDData monsterID = new MonsterIDData(monster.resid);
            var unit = TableManagerEditor.instance.GetTableItem<ProtoTable.UnitTable>(monsterID.mid);
            if (unit == null)
            {
                unit = TableManagerEditor.instance.GetTableItem<ProtoTable.UnitTable>(monster.resid);
            }
            if (unit == null) continue;
            var resData = GetResTableByID(unit.Mode);
            if (null == resData) continue;
            _ChangeEntityPrefab(monster, userdata, resData.ModelPath);
        }
        for (int i = 0; i < sceneData._npcinfo.Length; i++)
        {
            var npc = sceneData._npcinfo[i];
            EntityDataStruct userdata = FindEntityData(npc);
            if (userdata == null) continue;
            var curTable = TableManagerEditor.instance.GetTableItem<NpcTable>(npc.resid);
            if (null == curTable) continue;
            var resTable = GetResTableByID(curTable.ResID);
            if (resTable == null) continue;
            _ChangeEntityPrefab(npc, userdata, resTable.ModelPath);
        }
        for (int i = 0; i < sceneData._transportdoor.Length; i++)
        {
            var door = sceneData._transportdoor[i];
            EntityDataStruct userdata = FindEntityData(door);
            if (userdata == null) continue;
            var curTable = TableManagerEditor.instance.GetTableItem<SceneRegionTable>(door.resid);
            if (null == curTable) continue;
            var resTable = GetResTableByID(curTable.ResID);
            if (resTable == null) continue;
            _ChangeEntityPrefab(door, userdata, resTable.ModelPath);
            EntityInfoProcess(door);
        }
        for (int i = 0; i < sceneData._desructibleinfo.Length; i++)
        {
            var destruct = sceneData._desructibleinfo[i];
            EntityDataStruct userdata = FindEntityData(destruct);
            if (userdata == null) continue;
            var curTable = TableManagerEditor.instance.GetTableItem<DestrucTable>(destruct.resid);
            if (null == curTable) continue;
            var resTable = GetResTableByID(curTable.Mode);
            if (resTable == null) continue;
            _ChangeEntityPrefab(destruct, userdata, resTable.ModelPath);
        }
        for(int i = 0; i < sceneData._decoratorinfo.Length;i++)
        {
            var decorate = sceneData._decoratorinfo[i];
            EntityDataStruct userdata = FindEntityData(decorate);
            if (userdata == null) continue;
            var resTable = GetResTableByID(decorate.resid);
            if (resTable == null) continue;
            _ChangeEntityPrefab(decorate, userdata, resTable.ModelPath);
        }
    }

    private EntityDataStruct AddEntity2Editor(DEntityInfo info, GameObject obj = null, EditorUI.UIListItem item = null)
    {
        EntityDataStruct find = FindEntityData(info);
        if (find == null)
        {
            find = new EntityDataStruct();
            find.info = info;

            if (obj == null)
            {
                obj = CreateEntityObject(info);
                SetDyeColor(info.Color, new GameObject[] { obj });
            }

            obj.GetComponent<DEntityInfoComponent>().info = info;

            if (obj != null)
            {
                obj.transform.SetParent(entityRoot.transform, false);
                obj.transform.localScale = info.Scale * Vector3.one;
                SetDyeColor(info.Color, new GameObject[] { obj });
            }

            if (item == null)
            {
                item = AddEntity2ListControl(info);
            }

            info.obj = obj;
            find.obj = obj;
            find.item = item;

            EntityInfoProcess(info);

            entityData.Add(find);
            RegisterEntityEvent(find);
        }

        return find;
    }

    private void EntityInfoProcess(DEntityInfo info)
    {
        switch(info.type)
        {
            case DEntityType.TRANSPORTDOOR:
                var transportInfo = info as DTransportDoor;
                if(transportInfo != null && info.obj != null)
                {
                    var materReplacer = transportInfo.obj.AddComponent<MaterialReplacerComponent>();
                    if(!string.IsNullOrEmpty(transportInfo.materialAsset.m_AssetPath))
                    {
                        var material = AssetDatabase.LoadAssetAtPath<Material>(transportInfo.materialAsset.m_AssetPath);
                        materReplacer.SetDoorMaterial(material);
                    }
                }
                break;
        }
    }

    private void RemoveEntityData(EntityDataStruct data, bool bClearData)
    {
        if (bClearData)
        {
            if (data.obj != null)
            {
                GameObject.DestroyImmediate(data.obj);
                data.obj = null;
            }

            if (data.item != null)
            {
                data.item.Remove();
                data.item = null;
            }
        }

        if (DSceneEntitySelection.IsSelected(data.info))
        {
            DSceneEntitySelection.Select(null);
        }

        entityData.Remove(data);
    }

    private void RemoveEntityData(DEntityInfo info, bool bClearData)
    {
        EntityDataStruct data = FindEntityData(info);

        if (data == null)
        {
            return;
        }
        RemoveEntityData(data, bClearData);
        entityData.Remove(data);
    }
	 

    void OnSceneUI_Entity(SceneView sceneview, DEntityInfo selected)
    {
        switch (selected.type)
        {
            case DEntityType.NPC:
            case DEntityType.DECORATOR:
                {
                    if (toolCurrent == Tool.Rotate)
                    {
                        var bp = (selected as DDecoratorInfo);
                        bp.Rotation = Handles.RotationHandle(bp.Rotation, bp.Position);
                    }
                }
                break;
            case DEntityType.DESTRUCTIBLE:
                {
                    //DDestructibleInfo info = selected as DDestructibleInfo;

                    //if (info.bflowRegion)
                    //{
                    //    if (info.regionInfo.regiontype == DRegionInfo.RegionType.Circle)
                    //    {
                    //        Handles.color = Color.yellow;
                    //        info.regionInfo.radius = GUIControls.UICommon.Radius2DHandle(selected.position, info.regionInfo.radius);
                    //    }
                    //    else
                    //    {
                    //        Handles.color = Color.yellow;
                    //        info.regionInfo.rect = GUIControls.UICommon.RectHandles(Quaternion.identity, selected.position, info.regionInfo.rect);
                    //    }
                    //}
                }
                break;
			case DEntityType.ACTIVITY_BOSS_POS:
			case DEntityType.ACTIVITY_ELITE_POS:
			case DEntityType.ACTIVITY_MONSTER_POS:
			case DEntityType.BOSS:
			case DEntityType.ELITE:
            case DEntityType.MONSTER:
                {
                    DMonsterInfo info = selected as DMonsterInfo;

                    /*if (info.flowRegionType != DMonsterInfo.FlowRegionType.None)
                    {
                        if (info.regionInfo.regiontype == DRegionInfo.RegionType.Circle)
                        {
                            Handles.color = Color.yellow;
                            info.regionInfo.radius = GUIControls.UICommon.Radius2DHandle(selected.Position, info.regionInfo.radius);
                        }
                        else
                        {
                            Handles.color = Color.yellow;
                            info.regionInfo.rect = GUIControls.UICommon.RectHandles(Quaternion.identity, selected.Position, info.regionInfo.rect);
                        }
                    }*/
                }
                break;
            case DEntityType.REGION:
                {
                    DRegionInfo info = selected as DRegionInfo;

                    if (info.regiontype == DRegionInfo.RegionType.Circle)
                    {
                        Handles.color = Color.yellow;
                        info.radius = GUIControls.UICommon.Radius2DHandle(selected.Position, info.radius);
                    }
                    else
                    {
                        Handles.color = Color.yellow;
                        info.rect = GUIControls.UICommon.RectHandles(Quaternion.identity, selected.Position, info.rect);
                    }

                    if (DEntityType.TRANSPORTDOOR == selected.type)
                    {
                        var bp = (selected as DTransportDoor);
                        bp.birthposition = Handles.PositionHandle(bp.birthposition,
                                Quaternion.identity);
                    }
                }
                break;
             case DEntityType.TRANSPORTDOOR:
                {
                     DTransportDoor info = selected as DTransportDoor;
                    info.UpdateData();
                    if (info.regiontype == DRegionInfo.RegionType.Circle)
                    {
                        Handles.color = Color.yellow;
                        info.radius = GUIControls.UICommon.Radius2DHandle(info.GetDoorTransCenter(), info.radius);
                    }
                    else
                    {
                        Handles.color = Color.yellow;
                        info.rect = GUIControls.UICommon.RectHandles(Quaternion.identity, info.GetDoorTransCenter(), info.rect);
                    }

                    if (DEntityType.TRANSPORTDOOR == selected.type)
                    {
                        var bp = (selected as DTransportDoor);
                        bp.birthposition = Handles.PositionHandle(bp.birthposition,
                                Quaternion.identity);
                    }
                }
                break;
            case DEntityType.TOWNDOOR:
                {
                    DRegionInfo info = selected as DRegionInfo;

                    if (info.regiontype == DRegionInfo.RegionType.Circle)
                    {
                        Handles.color = Color.yellow;
                        info.radius = GUIControls.UICommon.Radius2DHandle(selected.Position, info.radius);
                    }
                    else
                    {
                        Handles.color = Color.yellow;
                        info.rect = GUIControls.UICommon.RectHandles(Quaternion.identity, selected.Position, info.rect);
                    }

                    var bp = (selected as DTownDoor);
                    Vector3 pos = Handles.PositionHandle(bp.BirthPos + bp.Position, Quaternion.identity);
                    bp.BirthPos = pos - bp.Position;
                }
                break;
            case DEntityType.HELLBIRTHPOSITION:
                {
                    DEntityInfo info = selected as DEntityInfo;
                }
                break;
            case DEntityType.BIRTHPOSITION:
                {
                    DEntityInfo info = selected as DEntityInfo;
                }
                break;
            case DEntityType.FUNCTION_PREFAB:
                {
                    FunctionPrefab info = selected as FunctionPrefab;
                }
                break;
            case DEntityType.RESOURCE_POS:
                { }
                break;
            case DEntityType.FIGHTER_BORN_POS:
                {

                }
                break;
            case DEntityType.ECOSYSTEM_RESOURCE_POS:
                {

                }
                break;
            default:
                Debug.LogErrorFormat("未处理类型 {0}", selected.type);
                break;
        }
    }

    void OnSceneUI_PickEntity()
    {
        Event current = Event.current;

        Handles.BeginGUI();
        Vector2 mousePosition = current.mousePosition;
        EventType typeForControl = current.GetTypeForControl(s_RectSelectionID);
        switch (typeForControl)
        {
            case EventType.MouseDown:
                {
                    if (HandleUtility.nearestControl == s_RectSelectionID && current.button == 0)
                    {
                        GUIUtility.hotControl = s_RectSelectionID;
                    }
                }
                break;
            case EventType.MouseUp:
                {
                    if (GUIUtility.hotControl == s_RectSelectionID && current.button == 0)
                    {
                        GUIUtility.hotControl = 0;
                        GameObject obj = HandleUtility.PickGameObject(Event.current.mousePosition, true);

                        GameObject root = FindEntityRoot(obj);

                        if (root != null)
                        {
                            SelectedDirty = true;
                            SelectedGameObject = root;
                            UpdateEntitySelectionDrity();
                            Event.current.Use();
                        }

                    }
                }
                break;
            case EventType.Layout:
                //if(toolCurrent != Tool.View)
                {
                    HandleUtility.AddDefaultControl(s_RectSelectionID);
                }
                break;
        }
        Handles.EndGUI();
    }

    void OnSceneUI_Base(SceneView sceneview, Event evt)
    {
        if (proxy.editmode == SceneEditorMode.NORMAL && toolCurrent != Tool.View)
        {
            Handles.color = Color.yellow;
            Vector4 vSize = new Vector4(_LogicXSizeBack.x, _LogicXSizeBack.y, _LogicZSizeBack.x, _LogicZSizeBack.y);
            vSize = GUIControls.UICommon.RectHandles(Vector3.zero, vSize);
            _LogicXSizeBack.x = vSize.x;
            _LogicXSizeBack.y = vSize.y;
            _LogicZSizeBack.x = vSize.z;
            _LogicZSizeBack.y = vSize.w;

            if (toolCurrent == Tool.Move)
            {
                /*
                   Vector3 delta = Handles.PositionHandle(Vector3.zero, Quaternion.identity);

                   if(delta != Vector3.zero)
                   {
                   sceneData._ScenePostion -= delta;
                   }
                   */
                sceneData._ScenePostion = Handles.PositionHandle(sceneData._ScenePostion, Quaternion.identity);
            }
            //Vector3 pos = Handles.PositionHandle(Vector3.zero, Quaternion.identity);
        }
    }
    void OnSceneUI_Layer(SceneView sceneview, Event evt)
    {
        if (proxy.editmode == SceneEditorMode.BLOCKLAYER)
        {
            if (evt.type == EventType.MouseDrag && (evt.alt || evt.shift))
            {
                Vector3 v = ScreenPickGround(sceneview);
                if(proxy.ShowEcoSystem)
                {
                    if (evt.alt)
                    {
                        proxy.SetEcosystemLayer(proxy.brushSize, v.x, v.z, grassId);
                    }
                    else
                    {
                        proxy.SetEcosystemLayer(proxy.brushSize, v.x, v.z, 0);
                    }
                }
                else if (proxy.ShowGrass)
                {
                    if (evt.alt)
                    {
                        proxy.SetGrassLayer(proxy.brushSize, v.x, v.z, grassId);
                    }
                    else
                    {
                        proxy.SetGrassLayer(proxy.brushSize, v.x, v.z, 0);
                    }
                }
                else if (proxy.ShowEventArea)
                {
                    if (evt.alt)
                    {
                        proxy.SetEventAreaLayer(proxy.brushSize, v.x, v.z, 1);
                    }
                    else
                    {
                        proxy.SetEventAreaLayer(proxy.brushSize, v.x, v.z, 0);
                    }
                }
                else
                {
                    if (evt.alt)
                    {
                        proxy.SetBlockLayer(proxy.brushSize, v.x, v.z, 1);
                    }
                    else
                    {
                        proxy.SetBlockLayer(proxy.brushSize, v.x, v.z, 0);
                    }
                }

                evt.Use();
            }

            if (evt.type == EventType.MouseMove && (evt.alt || evt.shift))
            {
                sceneview.Repaint();
            }

            if (evt.type == EventType.MouseMove)
            {
                Vector3 v = ScreenPickGround(sceneview);
                if (proxy.ShowGrass)
                {
                    ushort grassPtId = proxy.GetGrassId(v.x, v.z);
                    if (this.curPointGrassId != grassPtId)
                    {
                        curPointGrassId = grassPtId;
                        Repaint();
                    }
                }
                else if(proxy.ShowEcoSystem)
                {
                    ushort ecosystemPtId = proxy.GetEcosystemId(v.x, v.z);
                    if (this.curEcosytemId != ecosystemPtId)
                    {
                        curEcosytemId = ecosystemPtId;
                        Repaint();
                    }
                }
            }

            if (evt.type == EventType.ScrollWheel && (evt.alt || evt.shift))
            {
                float num = Event.current.delta.x + Event.current.delta.y;
                num = -num;

                if (num > 0)
                {
                    proxy.brushSize += 1;
                }
                else
                {
                    proxy.brushSize -= 1;
                }

                proxy.brushSize = Mathf.Max(Mathf.Min(proxy.brushSize, 20), 1);

                Repaint();
                Event.current.Use();
            }
        }
    }

    private Vector3 kDragOffset = new Vector3(2f, 0, 2f);
    private Vector3 _dragPositionClamp(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, sceneData._LogicXSize.x - kDragOffset.x, sceneData._LogicXSize.y + kDragOffset.x);
        pos.z = Mathf.Clamp(pos.z, sceneData._LogicZSize.x - kDragOffset.z, sceneData._LogicZSize.y + kDragOffset.z);
        //pos.y = 0;
        return pos;
    }

    void OnSceneUI(SceneView sceneview)
    {
        Event evt = Event.current;
        if (proxy == null)
        {
            return;
        }

        if (sceneData == null)
        {
            return;
        }

        if (sceneRoot == null)
        {
            return;
        }

        EditorGUI.BeginChangeCheck();

        if (Event.current.type == EventType.Repaint)
        {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.Arrow, 0);
        }

        if(bInArtistMode == false)
        OnSceneUI_PickEntity();

        if (Event.current.type == EventType.MouseDrag)
        {
            if (toolCurrent != Tool.View && (Event.current.button == 0) && GUIUtility.hotControl == s_ViewToolID)
            {
                Event.current.Use();
            }
        }

        OnSceneUI_Base(sceneview, evt);
        OnSceneUI_Layer(sceneview, evt);

        if (proxy.editmode == SceneEditorMode.ENTITYS)
        {
            var selected = DSceneEntitySelection.GetSelected();
            if (selected != null)
            {
                if (selected.Length > 0 && toolCurrent == Tool.Move)
                {
                    var centerPos = Vector3.zero;
                    foreach (var s in selected)
                    {
                        centerPos += s.Position;
                    }
                    centerPos /= selected.Length;
                    
                    Vector3 position = Handles.PositionHandle(centerPos,
                        Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity
                            : Quaternion.identity);

                    if (centerPos != position)
                    {
                        foreach (var s in selected)
                        {
                            Vector3 offsetPos = position - centerPos;
                            s.Position = _dragPositionClamp(s.Position + offsetPos);
                        }
                    }
                }
                
                foreach (var s in selected)
                {
                    OnSceneUI_Entity(sceneview, s);
                }
            }
        }

        if (evt.type == EventType.DragUpdated)
        {
            EntityConfigInfo cfg = DragAndDrop.GetGenericData("MonsterInfo") as EntityConfigInfo;
            if (cfg != null)
            {
                if (proxy.dragObject == null && DragAndDrop.objectReferences.Length > 0)
                {
                    proxy.dragObject = CreateEntityObject(cfg.resourcepath, Vector3.zero);
                }
                if (proxy.dragObject)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    proxy.dragObject.transform.position = ScreenPickGround(sceneview);
                    evt.Use();
                }
            }
        }

        if (evt.type == EventType.DragPerform)
        {
            if (proxy.dragObject != null)
            {
                EntityConfigInfo cfg = DragAndDrop.GetGenericData("MonsterInfo") as EntityConfigInfo;
                CreateEntity(cfg, proxy.dragObject.transform.position, proxy.dragObject);
                proxy.dragObject = null;
                DragAndDrop.AcceptDrag();
                evt.Use();
            }
        }

        if (evt.type == EventType.DragExited)
        {
            if (proxy.dragObject != null)
            {
                GameObject.DestroyImmediate(proxy.dragObject);
                proxy.dragObject = null;
                DragAndDrop.AcceptDrag();
                evt.Use();
            }
        }


        if (evt.type == EventType.MouseDown)
        {
            //evt.Use();
        }

        if (evt.type == EventType.MouseUp)
        {
            //DragAndDrop.AcceptDrag();
            //evt.Use();
        }

        if (evt.type == EventType.MouseMove)
        {

        }

        if (EditorGUI.EndChangeCheck())
        {
            Repaint();
            if (sceneData != null)
            {
                EditorUtility.SetDirty(sceneData);
            }
        }
    }

    private void _UpdateKeyEvent()
    {
        if(Event.current.type != EventType.KeyDown)
        {
            return;
        }

        if (null == mainCameraObj)
        {
            return;
        }

        Vector3 targetPosition = Vector3.zero;
        targetPosition = mainCameraObj.transform.position;

        KeyCode code = Event.current.keyCode;

        bool hasCode = false;

        switch (code)
        {
            case KeyCode.UpArrow:
                targetPosition.z += 1f;
                hasCode = true;
                break;
            case KeyCode.DownArrow:
                targetPosition.z -= 1f;
                hasCode = true;
                break;
            case KeyCode.LeftArrow:
                targetPosition.x -= 1f;
                hasCode = true;
                break;
            case KeyCode.RightArrow:
                targetPosition.x += 1f;
                hasCode = true;
                break;
            default:
                break;
        }

        //Debug.LogErrorFormat("1 targetPos {0}", targetPosition);

        if (hasCode)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, sceneData.GetCameraXRange().x, sceneData.GetCameraXRange().y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, sceneData.GetCameraZRange().x, sceneData.GetCameraZRange().y);

            sceneData.animCameraPostion = new AnimVector3(targetPosition);
            //FocusGameView();
            Event.current.Use();
        }

    }

    public static GameObject LoadAssetAtPath(string path)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + path);

        string ext = Path.GetExtension(path);
        if (ext == ".prefab")
        {
            string assetFile = Path.ChangeExtension(path, null);
            assetFile += "_ModelData.asset";

            DModelData data = null;
#if LOGIC_SERVER
            FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(assetFile, Utility.kRawDataExtension)), out data);
#else
            data = AssetDatabase.LoadAssetAtPath<DModelData>("Assets/Resources/" + assetFile);
#endif
            if (data != null)
            {
                AddModInfo(path, prefab, data);
            }
        }


        if (prefab == null)
        {
            prefab = EditorGUIUtility.Load("PointPreb.prefab") as GameObject;
        }
        return prefab;
    }

    class ModInfo
    {
        public string path;
        public GameObject prefab;
        public DModelData mod;

    }
    static List<ModInfo> resource_cache = new List<ModInfo>();

    static ModInfo FindModInfoByPath(string path)
    {
        return resource_cache.Find(x => x.path == path);
    }

    static ModInfo FindModInfoByPrefab(GameObject prefab)
    {
        return resource_cache.Find(x => x.prefab == prefab);
    }

    static void AddModInfo(string path, GameObject prefab, DModelData mod)
    {
        ModInfo info = new ModInfo();
        info.path = path;
        info.prefab = prefab;
        info.mod = mod;

        resource_cache.Add(info);
    }


    GameObject CreateEntityObject(DEntityInfo info)
    {
        return CreateEntityObject(info.path, info.Position);
    }

    private void SetLayer(UnityEngine.Object[] roots, int layer, bool includeChildren = true)
    {
        UnityEngine.Object[] objects = SceneModeUtility.GetObjects(roots, includeChildren);
        UnityEngine.Object[] array = objects;
        for (int i = 0; i < array.Length; i++)
        {
            GameObject gameObject = (GameObject)array[i];
            gameObject.layer = layer;
        }
    }

    GameObject CreateEntityObject(String path, Vector3 position)
    {
        var prefab = LoadAssetAtPath(path);

        var modinfo = FindModInfoByPrefab(prefab);
        UnityEngine.Object instance = PrefabUtility.InstantiatePrefab(prefab);

        var root = new GameObject();
        root.name = instance.name;

        root.AddComponent<DEntityInfoComponent>();
        root.transform.localPosition = position;

        var modroot = new GameObject();
        (instance as GameObject).transform.SetParent(modroot.transform, false);
        modroot.transform.SetParent(root.transform, false);

        GameObject[] ago = new GameObject[] { modroot };

        SetDyeColor(sceneData._ObjectDyeColor, ago);

        if (modinfo != null && modinfo.mod != null)
        {
            var drawer = root.AddComponent<GridBlockDrawer>();
            if (drawer != null)
            {
                drawer.RefreshGridData(modinfo.mod.blockGridChunk.gridWidth,
                        modinfo.mod.blockGridChunk.gridHeight,
                        modinfo.mod.blockGridChunk.gridBlockData);
                modroot.transform.localScale = modinfo.mod.modelScale;
            }

        }

        SetHideFlags(root as GameObject, HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor);

        SetLayer(new UnityEngine.Object[] { root }, (int)8);
        return root;
    }

    void _fixEntity(DEntityInfo info)
    {
        switch (info.type)
        {
            case DEntityType.NPC:
                break;
            case DEntityType.MONSTER:
                break;
            case DEntityType.DECORATOR:
                break;
            case DEntityType.DESTRUCTIBLE:
                break;
            case DEntityType.REGION:
                break;
            case DEntityType.TRANSPORTDOOR:
                {
                    var door = info as DTransportDoor;
                    if (door != null)
                    {
                        var td = TableManagerEditor.instance.GetTableItem<ProtoTable.SceneRegionTable>(info.resid);
                        if (td != null)
                        {
                            door.DoorType = (TransportDoorType)(td.DoorType);
                        }
                    }
                }
                break;
            case DEntityType.BOSS:
                break;
            case DEntityType.ELITE:
                break;
            case DEntityType.BIRTHPOSITION:
                break;
            case DEntityType.HELLBIRTHPOSITION:
                break;
            case DEntityType.TOWNDOOR:
                break;
            case DEntityType.FUNCTION_PREFAB:
                break;
            case DEntityType.MAX:
                break;
            default:
                break;
        }

    }

    void CreateEntity(EntityConfigInfo cfginfo, Vector3 pos, GameObject obj)
    {
        if (sceneData == null)
        {
            return;
        }
        int size = 0;
        DEntityInfo info = sceneData.NewEntityInfo(cfginfo.type, out size);
        info.resid = cfginfo.id;
        info.Name = cfginfo.name;
        info.path = cfginfo.resourcepath;
        info.Description = cfginfo.description;
        info.position = pos;
        info.globalid = info.resid * 100 + size;

        _fixEntity(info);

        var data = AddEntity2Editor(info, obj, null);
        DSceneEntitySelection.Select(new []{info});
        entityList.selected = data.item;
        entityList.EnsureSelectedShow();
    }

    public void SetDyeColor(Color dyeColor, GameObject[] modelRoot)
    {
        
        for (int i = 0; i < modelRoot.Length; ++i)
        {
            List<Renderer> renders =new List<Renderer>();
            SpriteRenderer[] smr = modelRoot[i].transform.GetComponentsInChildren<SpriteRenderer>();
            MeshRenderer[] mmr = modelRoot[i].transform.GetComponentsInChildren<MeshRenderer>();
            renders.AddRange(mmr);
            renders.AddRange(smr);
            bool flag = false;
            for (int j = 0; j < renders.Count; ++j)
            {
                Material[] am = renders[j].sharedMaterials;

                if (null != am)
                {
                    for (int k = 0; k < am.Length; ++k)
                    {
                        if (null != am[k] && am[k].HasProperty("_DyeColor"))
                        {
                            am[k] = new Material(am[k]);
                            am[k].SetColor("_DyeColor", dyeColor);
                            flag = true;
                        }
                    }

                    renders[j].sharedMaterials = am;
                }
            }

            if (!flag)
            {
                Logger.LogErrorFormat("meterials is missing with root: {0}", modelRoot[i].name);
            }
        }
    }

    void DuplicateEntity(DEntityInfo entity)
    {
        if (sceneData == null)
        {
            return;
        }
        int size = 0;
        DEntityInfo info = sceneData.NewEntityInfo(entity.type, out size);
        info.Duplicate(entity);
        info.globalid = info.resid * 100 + size;

        var data = AddEntity2Editor(info, null, null);
        info.Duplicate(entity);
        info.DuplicateObjInfo(entity);
        DSceneEntitySelection.Select(new []{info});
        entityList.selected = data.item;
        entityList.EnsureSelectedShow();
    }
    void DuplicateEntityVoid(DEntityInfo entity)
    {
        if (sceneData == null)
        {
            return;
        }
        int size = 0;
        DEntityInfo info = sceneData.NewEntityInfo(entity.type, out size);
        info.resid = entity.resid;
        info.Name = entity.Name;
        info.path = entity.path;
        info.Description = entity.Description;
        info.position = entity.position;
        info.globalid = info.resid * 100 + size;


        var data = AddEntity2Editor(info, obj, null);
        DSceneEntitySelection.Select(new []{info});
        entityList.selected = data.item;
        entityList.EnsureSelectedShow();
    }


    EditorUI.UIListItem AddEntity2ListControl(DEntityInfo info)
    {
        if (!mEntityListTypeDict.ContainsKey(info.type) )
        {
            var fitem = entityList.AddItems(new string[] { info.type.GetDescription() });
            mEntityListTypeDict.Add(info.type, fitem);
            fitem.isexpand = true;
            fitem.userdata = info.type;
        }

        var item = entityList.InsertItems(
                new string[] { info.Name, info.type.GetDescription(), info.Description },
                mEntityListTypeDict[info.type],
                (current, insert) =>
                {
                    return true;
                });
        item.userdata = info;
        return item;
    }

    void InitEntityData()
    {
        if (sceneData)
        {
            if (sceneData._birthposition != null)
            {
                AddEntity2Editor(sceneData._birthposition);
            }

            for (int i = 0; i < sceneData._entityinfo.Length; ++i)
            {
                AddEntity2Editor(sceneData._entityinfo[i]);
            }

            for (int i = 0; i < sceneData._npcinfo.Length; ++i)
            {
                AddEntity2Editor(sceneData._npcinfo[i]);
            }

            for (int i = 0; i < sceneData._monsterinfo.Length; ++i)
            {
                AddEntity2Editor(sceneData._monsterinfo[i]);
            }

            for (int i = 0; i < sceneData._decoratorinfo.Length; ++i)
            {
                var data = AddEntity2Editor(sceneData._decoratorinfo[i]);
                data.obj.transform.localScale = sceneData._decoratorinfo[i].LocalScale;
                data.obj.transform.localRotation = sceneData._decoratorinfo[i].Rotation;
            }

            for (int i = 0; i < sceneData._desructibleinfo.Length; ++i)
            {
                var data = AddEntity2Editor(sceneData._desructibleinfo[i]);
            }

            for (int i = 0; i < sceneData._regioninfo.Length; ++i)
            {
                AddEntity2Editor(sceneData._regioninfo[i]);
            }

            for (int i = 0; i < sceneData._transportdoor.Length; ++i)
            {
                AddEntity2Editor(sceneData._transportdoor[i]);
            }

            for (int i = 0; i < sceneData._townDoor.Length; ++i)
            {
                AddEntity2Editor(sceneData._townDoor[i]);
            }

            for(int i = 0; i < sceneData._FunctionPrefab.Length; ++i)
            {
                AddEntity2Editor(sceneData._FunctionPrefab[i]);
            }

            for (int i = 0; i < sceneData._fighterBornPosition.Length; ++i)
            {
                AddEntity2Editor(sceneData._fighterBornPosition[i]);
            }

            for (int i = 0; i < sceneData._resourcePosition.Length; ++i)
            {
                AddEntity2Editor(sceneData._resourcePosition[i]);
            }

            for (int i = 0; i < sceneData._ecosystemResoucePosition.Length; ++i)
            {
                AddEntity2Editor(sceneData._ecosystemResoucePosition[i]);
            }

            RegisterEvent();
        }
    }
    void OnEnable()
    {
        DSceneEntitySelection.onMarkDirty = () =>
        {
            if (sceneData)
            {
                EditorUtility.SetDirty(sceneData);
            }
        };

        Populate();
        SceneView.onSceneGUIDelegate += OnSceneUI;
    }

    void OnDisable()
    {
        DSceneEntitySelection.onMarkDirty = null;
        SceneView.onSceneGUIDelegate -= OnSceneUI;
        Clear(true, true);
        SaveMonsterConfig();
    }

    private void RegisterEntityEvent(EntityDataStruct entity)
    {
        entity.info.OnNameChanged = (d, name) =>
        {
            var data = FindEntityData(d);
            if (data != null)
            {
                data.item.texts[0] = name;
            }
            Repaint();
        };
            
        entity.info.OnDescriptionChanged = (d, desc) =>
        {
            var data = FindEntityData(d);
            if (data != null)
            {
                data.item.texts[2] = desc;
            }
            Repaint();
        };
        
        entity.info.OnPositionChange = (d, pos) =>
        {
            var data = FindEntityData(d);
            if (data != null)
            {
                data.obj.transform.localPosition = pos;
            }
        };
        
        entity.info.OnScaleChange = (d, scale) =>
        {
            var data = FindEntityData(d);
            if (data != null)
            {
                data.obj.transform.localScale = Vector3.one * scale;
            }
        };
        
        entity.info.OnColorChange = (d, color) =>
        {
            var data = FindEntityData(d);
            if (data != null)
            {
                MeshRenderer[] amr = data.obj.transform.GetComponentsInChildren<MeshRenderer>();
                for (int j = 0; j < amr.Length; ++j)
                {
                    Material[] am = amr[j].materials;
                    for (int k = 0; k < am.Length; ++k)
                    {
                        if (am[k].HasProperty("_DyeColor"))
                            am[k].SetColor("_DyeColor", color);
                    }
                }
            }
        };

        var decoratorInfo = entity.info as DDecoratorInfo;
        if (decoratorInfo != null)
        {
            decoratorInfo.OnDecoratorRotationChange = (d, quaternion) =>
            {
                var data = FindEntityData(d);
                if (data != null)
                {
                    data.obj.transform.localRotation = quaternion;
                }
            };
            
            decoratorInfo.OnDecoratorScaleChange = (d, scale) =>
            {
                var data = FindEntityData(d);
                if (data != null)
                {
                    data.obj.transform.localScale = scale;
                }
            };
        }
    }
    
    private void RegisterEvent()
    {
        for (int i = 0; i < entityData.Count; i++)
        {
            RegisterEntityEvent(entityData[i]);
        }
    }
    
    private void UnRegisterEvent()
    {
        for (int i = 0; i < entityData.Count; i++)
        {
            entityData[i].info.OnNameChanged = null;
            entityData[i].info.OnDescriptionChanged = null;
            entityData[i].info.OnPositionChange = null;
            entityData[i].info.OnScaleChange = null;
        }
    }

    void OnDestroy()
    {
        UnRegisterEvent();
        SceneView.onSceneGUIDelegate -= OnSceneUI;

        Clear(true, true);
        SaveMonsterConfig();
    }
    void SetHideFlags(GameObject obj, HideFlags flag)
    {
        obj.hideFlags = flag;
        foreach (Transform child in obj.transform)
        {
            SetHideFlags(child.gameObject, flag);
        }
    }

    public GameObject _GetSceneGroundObj(GameObject sceneRoot)
    {
        if ("Ground" == sceneRoot.tag /*|| (sceneRoot.name.Contains("ground") && null != sceneRoot.GetComponents<MeshRenderer>())*/)
            return sceneRoot;

        GameObject groundObj = null;
        int childNum = sceneRoot.transform.childCount;
        for (int i = 0; i < childNum; ++i)
        {
            Transform curChild = sceneRoot.transform.GetChild(i);

            groundObj = _GetSceneGroundObj(curChild.gameObject);
            if (null != groundObj)
                return groundObj;
        }


        return groundObj;
    }

    public void _NormalizeSceneWaterLevel(GameObject sceneRoot)
    {
        /// Auto adjust scene ground to zero-plane.
        GameObject ground = _GetSceneGroundObj(sceneRoot);
        if (null != ground)
        {
            Vector3 sceneRootPos = sceneRoot.transform.position;
            sceneRootPos.y -= ground.transform.position.y + 0.05f;
            sceneRoot.transform.position = sceneRootPos;

            Logger.LogErrorFormat("Ground:{0}", ground.transform.position.y);
        }
    }

    private GameObject sr;
    void CreateScene()
    {
        sr = (GameObject)PrefabUtility.InstantiatePrefab(sceneData._prefab);

        /// Auto adjust scene ground to zero-plane.
        _NormalizeSceneWaterLevel(sr);

        editorRoot = new GameObject();
        editorRoot.name = "editorRoot";
        editorRoot.transform.localPosition = sceneData._LogicPos;
        sceneRoot = new GameObject();
        sceneRoot.name = "sceneRoot";
        sceneRoot.transform.SetParent(editorRoot.transform, false);
        sceneData._ScenePostion.y = 0;
        sceneRoot.transform.localPosition = sceneData._ScenePostion;
        proxy = editorRoot.AddComponent<DSceneDataEditorDrawer>();
        sr.transform.SetParent(sceneRoot.transform, false);
        editorRoot.transform.SetSiblingIndex(0);
        proxy.SceneData = sceneData;
        entityRoot = new GameObject();
        entityRoot.name = "EntitysRoot";
        entityRoot.transform.SetParent(editorRoot.transform, false);
        //SetHideFlags(editorRoot, HideFlags.DontSaveInEditor);

        if (sceneData._lightmapsetting != null)
        {
            sceneData._lightmapsetting.Apply();
        }

        UpdateMainCamera();

        UpdateEditorCamera();
    }

    private void UpdateEditorCamera()
    {
        var xS = sceneData._LogicXSize;
        var yS = sceneData._LogicZSize;

        if (SceneView.lastActiveSceneView != null)
        {
            var obj = new GameObject();
            obj.transform.localPosition = new Vector3((xS.x + xS.y) / 2, 1, (yS.x + yS.y) / 2);
            obj.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
            SceneView.lastActiveSceneView.AlignViewToObject(obj.transform);
            GameObject.DestroyImmediate(obj);
        }
    }

    void ClearScene()
    {

    }

    void UpdateEntitySelectionDrity()
    {
        if (SelectedDirty)
        {
            SelectedDirty = false;
            //Selection.activeGameObject = FindRoot(Selection.activeGameObject);
            if (SelectedGameObject)
            {
                DEntityInfoComponent entityInfo = SelectedGameObject.GetComponent<DEntityInfoComponent>();

                if (entityInfo)
                {
                    EntityDataStruct data = FindEntityData(SelectedGameObject);
                    if (data != null)
                    {
                        if (Event.current.control)
                        {
                            m_isCtrlSelect = true;
                            entityList.MultiSelect(data.item);
                            m_isCtrlSelect = false;
                            entityList.EnsureSelectedShow();
                        }
                        else
                        {
                            DSceneEntitySelection.Select(new []{data.info});
                            entityList.selected = data.item;
                            entityList.EnsureSelectedShow();
                        }
                        Event.current.Use();
                    }
                }
            }

        }
    }
    void Update()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode && editorRoot != null)
        {
            Clear(true, false);
        }

        UpdateEntitySelectionDrity();

        if (mainCameraObj && sceneData != null)
        {
            mainCameraObj.transform.localPosition = sceneData.animCameraPostion.value;
            //EditorUtility.SetDirty(mainCameraObj);
        }
    }

    void CameraSettingToggle<T>(string text, ref T data, T BattleValue, T TownValue, bool isNeedReview = false)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(text);

        bool isBattle = data.Equals(BattleValue);
        bool isTown = data.Equals(TownValue);

        bool hasChange = false;
        var temp1 = EditorGUILayout.Toggle(string.Format("战斗（{0}）", BattleValue.ToString()), isBattle);
        if(temp1 != isBattle)
        {
            data = BattleValue;
            hasChange = true;
        }

        var temp2 = EditorGUILayout.Toggle(string.Format("城镇（{0}）", TownValue.ToString()), isTown);
        if (temp2 != isTown)
        {
            data = TownValue;
            hasChange = true;
        }
        if(isNeedReview && hasChange)
        {
            Clear(true, false);
            OnReview();
        }
        EditorGUILayout.EndHorizontal();
    }

    void CameraSettingLable(string text1, string text2)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(text1);
        EditorGUILayout.LabelField("", text2);
        EditorGUILayout.EndHorizontal();
    }

    bool Vector2FiledDelay(string text, ref Vector2 data, ref Vector2 databack)
    {
        EditorGUILayout.BeginHorizontal();
        bool bDirty = (data != databack);
        databack = EditorGUILayout.Vector2Field(text + (bDirty ? "*" : ""), databack);
        if (databack != data)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            GUILayout.Space(18);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", "miniButton", GUILayout.Height(16)))
            {
                data = databack;
                UpdateBlockSizeData();
                return true;
            }
            if (GUILayout.Button("Cancle", "miniButton", GUILayout.Height(16)))
            {
                databack = data;
                return true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        return false;
    }

    bool Vector2FiledDelay2(string text, ref Vector2 data, ref Vector2 databack)
    {
        EditorGUILayout.BeginHorizontal();
        bool bDirty = (data != databack);
        databack = EditorGUILayout.Vector2Field(text + (bDirty ? "*" : ""), databack);
        if (databack != data)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            GUILayout.Space(18);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", "miniButton", GUILayout.Height(16)))
            {
                databack = FloorGirdSize(databack);
                data = databack;
                UpdateBlockSizeData();
                return true;
            }
            if (GUILayout.Button("Cancle", "miniButton", GUILayout.Height(16)))
            {
                databack = data;
                return true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        return false;
    }
    bool FixedWidthButton(string text, float width)
    {
        return GUILayout.Button(text, "miniButton", GUILayout.Width(width));
    }

    bool FixedWidthToggle(string text, bool Toggle, float width)
    {
        return GUILayout.Toggle(Toggle, text, GUILayout.Width(width));
    }

    bool VisiableToggle(string text, bool toggle)
    {
        return FixedWidthToggle(text, toggle, 70.0f);
    }

    bool TabEditorMode(ref SceneEditorMode mode, string text, SceneEditorMode moderef)
    {
        SceneEditorMode premode = mode;
        if (GUILayout.Toggle(mode == moderef, text, "miniButtonMid", GUILayout.Width(60)))
        {
            mode = moderef;
        }

        if (premode == mode)
        {
            return false;
        }
        else
        {
            SceneView.RepaintAll();
            return true;
        }
    }

    LevelEditorTags TabEditorTags(LevelEditorTags mode, LevelEditorTags moderef)
    {
        LevelEditorTags premode = mode;
        if (GUILayout.Toggle(mode == moderef, moderef.GetDescription(true), "miniButtonMid", GUILayout.Width(120)))
        {
            premode = moderef;
        }

        if (premode != mode)
        {
            SceneView.RepaintAll();
        }

        return premode;
    }

    void CreateFontStyle()
    {
        if (fontStyle == null)
        {
            fontStyle = new GUIStyle();
            fontStyle.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle.fontSize = 24;
            fontStyle.alignment = TextAnchor.UpperLeft;
            fontStyle.normal.textColor = Color.gray;
            fontStyle.hover.textColor = Color.gray;
        }

        if (fontStyle2 == null)
        {
            fontStyle2 = new GUIStyle();
            fontStyle2.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle2.fontSize = 18;
            fontStyle2.alignment = TextAnchor.UpperLeft;
            fontStyle2.normal.textColor = Color.green;
            fontStyle2.hover.textColor = Color.green;
        }

        if (fontStyle3 == null)
        {
            fontStyle3 = new GUIStyle();
            fontStyle3.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle3.fontSize = 14;
            fontStyle3.alignment = TextAnchor.UpperLeft;
            fontStyle3.normal.textColor = Color.cyan;
            fontStyle3.hover.textColor = Color.cyan;
        }
    }

    Vector2 scroll_pos;
    protected string[] ButtonTitle = new string[] { "name", "id", "Descriptions" };
    protected EditorUI.SplitterState state = new EditorUI.SplitterState(new float[3] { 0.3f, 0.3f, 0.4f });
    protected EditorUI.UIListBox entityResourceList;
    protected EditorUI.UIListBox entityList;
    protected bool m_isCtrlSelect = false;
    protected Texture2D lutTex;
    protected Texture2D lightmapTex;

    void OnGUI_Header()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);

        EditorGUILayout.LabelField("", sceneData.name, fontStyle);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUIControls.UICommon.subArrayElementStyle);

        EditorGUI.indentLevel -= 1;
        if (dataWarning)
        {
            GUILayout.BeginHorizontal("GroupBox");
            GUILayout.Label(errorMsg, "CN EntryWarn");
            if (GUILayout.Button("ok", "TL Tab Mid"))
            {
                dataWarning = false;
                return;
            }
            GUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel += 1;

        sceneData._id = EditorGUILayout.IntField("ID :", sceneData._id);
        Color newColor = EditorGUILayout.ColorField("颜色 :", sceneData._ObjectDyeColor);

        if (newColor != sceneData._ObjectDyeColor)
        {
            sceneData._ObjectDyeColor = newColor;
            GameObject[] ago = new GameObject[1];
            if (sr != null)
            {
                ago[0] = sr;
                SetDyeColor(newColor, ago);
            }
            for (int i = 0; i < sceneData._decoratorinfo.Length; ++i)
            {
                DDecoratorInfo cur = sceneData._decoratorinfo[i];
                cur.color = newColor;
                if (null != cur.obj)
                {
                    ago[0] = cur.obj;
                    SetDyeColor(newColor, ago);
                }
            }
            for (int i = 0; i < sceneData._desructibleinfo.Length; ++i)
            {
                DDestructibleInfo cur = sceneData._desructibleinfo[i];
                cur.color = newColor;
                if (null != cur.obj)
                {
                    ago[0] = cur.obj;
                    SetDyeColor(newColor, ago);
                }
            }
        }

        GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField("场景模型:", sceneData._prefab, typeof(UnityEngine.GameObject), false);
        if (newPrefab != null && sceneData._prefab != newPrefab && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            _NormalizeSceneWaterLevel(newPrefab);

            if (PrefabUtility.GetPrefabType(newPrefab) == PrefabType.Prefab
                    && FileTools.IsResourcesAsset(newPrefab))
            {
                sceneData._prefab = newPrefab;
                dataWarning = false;
            }
            else
            {
                dataWarning = true;
                errorMsg = FileTools.GetAssetPath(newPrefab) + "\n 资源不是Prefeb或者路径不在Resources目录下";
            }
        }

        if (sceneData._prefab == null && !string.IsNullOrEmpty(sceneData._prefabpath))
        {
            if (GUIControls.UICommon.StyledButton("加载下场景"))
            {
                UnityEngine.GameObject loadScene = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>("Assets/Resources/" + sceneData._prefabpath);
                sceneData._prefab = loadScene;
            }
        }

       
        if (sceneData._prefab != null)
        {
            if (sceneData._lightmapsetting == null)
            {
                if (!string.IsNullOrEmpty(sceneData._LightmapsettingsPath))
                {
                    sceneData._lightmapsetting = AssetDatabase.LoadAssetAtPath<DynSceneSetting>("Assets/Resources/"+sceneData._LightmapsettingsPath);
                }
            }

            var sceneSetting = (DynSceneSetting)EditorGUILayout.ObjectField("光照信息:", sceneData._lightmapsetting, typeof(DynSceneSetting), false);
            if (sceneSetting != sceneData._lightmapsetting)
            {
                sceneData._lightmapsetting = sceneSetting;
                sceneData._LightmapsettingsPath = FileTools.GetAssetLoadPath(sceneData._lightmapsetting);
            }
            EditorGUILayout.LabelField("lightingpath:", sceneData._LightmapsettingsPath);
            var path = FileTools.GetAssetPath(sceneData._prefab);

            if (sceneData._prefabpath != path)
            {
                sceneData._prefabpath = path;
                EditorUtility.SetDirty(sceneData);
                AssetDatabase.SaveAssets();
            }

            sceneData._WeatherMode = (EWeatherMode)EditorGUILayout.EnumPopup("天气模式:", sceneData._WeatherMode);

            // ***********************Lut********************
            Texture2D lastLutTex = lutTex;
            string lastLutPath = lutTex == null ? string.Empty : FileTools.GetAssetPath(lutTex);
            lutTex = EditorGUILayout.ObjectField("Lut:", lutTex, typeof(Texture2D), false) as Texture2D;
            if(lastLutTex != lutTex)
            {
                sceneData._LutTexPath = FileTools.GetAssetPath(lutTex);
            }
            else if(lastLutPath != sceneData._LutTexPath)
            {
                lutTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/" + sceneData._LutTexPath);
            }
            EditorGUILayout.LabelField("Lut Path:", sceneData._LutTexPath);
            if(lutTex == null)
            {
                Shader.DisableKeyword("COLOR_LUT");
            }
            else
            {
                Shader.EnableKeyword("COLOR_LUT");
                Shader.SetGlobalTexture("_LutTexture", lutTex);
            }
            // ***********************Lut********************

            // *********************Litghmap*****************
            Texture2D lastlightmapTex = lightmapTex;
            string lastlightmapPath = lightmapTex == null ? string.Empty : FileTools.GetAssetPath(lightmapTex);
            lightmapTex = EditorGUILayout.ObjectField("Lightmap:", lightmapTex, typeof(Texture2D), false) as Texture2D;
            if (lastlightmapTex != lightmapTex)
            {
                sceneData._LightmapTexPath = FileTools.GetAssetPath(lightmapTex);
            }
            else if (lastlightmapPath != sceneData._LightmapTexPath)
            {
                lightmapTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/" + sceneData._LightmapTexPath);
            }
            EditorGUILayout.LabelField("Lightmap Path:", sceneData._LightmapTexPath);
            if(lightmapTex == null)
            {
                Shader.DisableKeyword("LIGHT_ENV");
            }
            else
            {
                Shader.EnableKeyword("LIGHT_ENV");
                Shader.SetGlobalTexture("_SrcTex", lightmapTex);
            }
            // *********************Litghmap*****************

            sceneData._LightmapPosition = EditorGUILayout.Vector4Field("LightmapPosition", sceneData._LightmapPosition);
            Shader.SetGlobalVector("_LigntingMapPosition", sceneData._LightmapPosition);

            EditorGUILayout.LabelField("path:", sceneData._prefabpath);
            if (editorRoot == null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUIControls.UICommon.StyledButton("Editor"))
                    {
                        Preview();
                    }

                    if (GUIControls.UICommon.StyledButton("Fight"))
                    {
                        PreviewInPlay();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                showtoggle = EditorGUILayout.Foldout(showtoggle, "Visibility:", EditorStyles.foldout);
                if (showtoggle)
                {
                    EditorGUILayout.BeginHorizontal(GUIControls.UICommon.subGroupStyle);

                    EditorGUI.BeginChangeCheck();
                    proxy.ShowBlock = VisiableToggle("block", proxy.ShowBlock);
                    if (EditorGUI.EndChangeCheck() && entityRoot != null)
                    {
                        GridBlockDrawer[] list = entityRoot.GetComponentsInChildren<GridBlockDrawer>();
                        for (int i = 0; i < list.Length; ++i)
                        {
                            list[i].bShow = proxy.ShowBlock;
                        }
                    }
                    proxy.ShowGrass = VisiableToggle("grass(不导出到验证)", proxy.ShowGrass);
                    bool bOldShowEntity = proxy.showentity;
                    proxy.showgrid = VisiableToggle("grid", proxy.showgrid);
                    proxy.showentity = VisiableToggle("entity", proxy.showentity);
                    proxy.showmonster = VisiableToggle("monster", proxy.showmonster);
                    proxy.ShowEcoSystem = VisiableToggle("ecosystem", proxy.ShowEcoSystem);
                    proxy.ShowEventArea = VisiableToggle("event", proxy.ShowEventArea);

                    if (bOldShowEntity != proxy.showentity)
                    {
                        entityRoot.SetActive(proxy.showentity);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                {
                    DoToolButtons();
                    GUILayoutUtility.GetRect(1, 1);
                    if (FixedWidthButton("ResetView", 90.0f))
                    {
                        ResetCamera();
                    }
                    
                    if (FixedWidthButton("ResetEntity", 110.0f))
                    {
                        ResetEntityPrefab();
                        UpdateDoorName(entityList);
                    }

                    if (FixedWidthButton("CloseView", 90.0f))
                    {
                        Clear(true, true);
                    }

                    if (FixedWidthButton("SaveAssets", 90.0f))
                    {
                        AssetDatabase.SaveAssets();
                        UnityEngine.Debug.LogWarning("Scene " + sceneData.name + " Saved!");
                    }

                    if (FixedWidthButton("RebuildGrass", 110.0f))
                    {
                        if (sceneData._grasslayer != null)
                        {
                            sceneData._grasslayer = new ushort[sceneData.LogicX * sceneData.LogicZ];
                            Array.Clear(sceneData._grasslayer, 0, sceneData._grasslayer.Length);
                            proxy.OnLogicSizeChange();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

#region Block Layer
    float size;
    Vector2 FloorGirdSize(Vector2 value)
    {
        if (sceneData == null)
        {
            return value;
        }

        int delta;

        delta = (int)(value.x / sceneData._GridSize.x);
        value.x = delta * sceneData._GridSize.x;

        delta = (int)(value.y / sceneData._GridSize.y);
        value.y = delta * sceneData._GridSize.y;

        return value;
    }

    //AnimVector3 animCameraPostion = new AnimVector3();
    bool autoSetCamera;

    Type gameViewType;
    void FocusGameView()
    {
        if (gameViewType == null)
        {
            var ass = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            gameViewType = ass.GetType("UnityEditor.GameView");
        }

        if (gameViewType == null)
        {
            Debug.LogError("FocusGameView Error,not find Type\n");
        }

        EditorWindow.FocusWindowIfItsOpen(gameViewType);
    }


    private const float mBattleCameraLookHeight = 1.0f;
    private const float mBattleCameraDistance = 10.0f;

    private const float mTownCameraLookHeight = 2.0f;
    private const float mTownCameraDistance = 35.0f;

    private const float mCameraAngle = 20.0f;
    private const float mCameraSize = 3.05f;


    void OnGUI_BlockLayer()
    {
        EditorGUI.indentLevel += 1;
        EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("", "关卡信息", fontStyle3);
		EditorGUILayout.Space();
		sceneData._TipsID = EditorGUILayout.IntField("关卡提示ID:", sceneData._TipsID);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", "场景信息", fontStyle3);
        EditorGUILayout.Space();
        sceneData._ScenePostion = EditorGUILayout.Vector3Field("位置:", sceneData._ScenePostion);
        sceneData._ScenePostion.y = 0;
        sceneRoot.transform.localPosition = sceneData._ScenePostion;
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        sceneData._SceneUScale = EditorGUILayout.FloatField("缩放系数:", sceneData._SceneUScale);
        sceneRoot.transform.localScale = new Vector3(sceneData._SceneUScale, sceneData._SceneUScale, sceneData._SceneUScale);
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", "逻辑范围", fontStyle3);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        sceneData._LogicPos = EditorGUILayout.Vector3Field("逻辑原点:", sceneData._LogicPos);

        editorRoot.transform.localPosition = sceneData._LogicPos;

        CameraSettingToggle("GridSize", ref sceneData._GridSize, new Vector2(0.25f, 0.25f), new Vector2(0.5f, 0.5f), true);
        /*if (Vector2FiledDelay("GridSize", ref sceneData._GridSize, ref _GridSizeBack))
        {
            return;
        }*/

        if (Vector2FiledDelay2("XSize", ref sceneData._LogicXSize, ref _LogicXSizeBack))
        {
            return;
        }

        if (Vector2FiledDelay2("YSize", ref sceneData._LogicZSize, ref _LogicZSizeBack))
        {
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (proxy.editmode == SceneEditorMode.BLOCKLAYER)
        {
            proxy.brushSize = EditorGUILayout.IntSlider("画刷大小:", proxy.brushSize, 1, 20);
            if (bInArtistMode && proxy.ShowGrass)
            {
                ushort value = (ushort)EditorGUILayout.IntField("草丛ID(小于65535):", grassId);
                if (value > 0 && value <= 65535)
                {
                    grassId = value;
                }

                if (!proxy.IsGrassSetted() && GUILayout.Button("增加草丛设置", "minibutton"))
                {
                    CreateGrassSizeData();
                }
            }
            if (bInArtistMode && proxy.ShowEcoSystem)
            {
                ushort value = (ushort)EditorGUILayout.IntField("生态ID(小于65535):", ecosystemId);
                if (value > 0 && value <= 65535)
                {
                    ecosystemId = value;
                }
                if (!proxy.IsEcosystemSetted() && GUILayout.Button("增加生态设置", "minibutton"))
                {
                    CreateEcosystemSizeData();
                }
            }
            if (bInArtistMode && proxy.ShowEventArea)
            {
                if (!proxy.IsEventAreaSet() && GUILayout.Button("增加事件设置", "minibutton"))
                {
                    CreateEventAreaSizeData();
                }
            }
            if (proxy.ShowEcoSystem)
            {
                EditorGUILayout.LabelField(string.Format("当前生态ID:" + curEcosytemId));
            }
            if (proxy.ShowGrass)
            {
                EditorGUILayout.LabelField(string.Format("当前草丛ID:" + curPointGrassId));
            }
        }

        if (proxy.editmode == SceneEditorMode.NORMAL)
        {

            //sceneData._CenterPostionNew  = EditorGUILayout.Vector2Field("场景位置：", sceneData._CenterPostionNew);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", "相机范围设置", fontStyle3);
            bCameraSetting = EditorGUILayout.Foldout(bCameraSetting, "");
            if(!bCameraSetting)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            if(bCameraSetting)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                {
                    EditorGUI.BeginChangeCheck();
                    CameraSettingLable(string.Format("LookAt高度（战斗标准值：{0}，城镇标准值：{1}）", mBattleCameraLookHeight, mTownCameraLookHeight), sceneData._CameraLookHeight.ToString());
                    CameraSettingLable(string.Format("LookAt距离（战斗标准值：{0}，城镇标准值：{1}）", mBattleCameraDistance, mTownCameraDistance), sceneData._CameraDistance.ToString());

                    var battleNearClip = Global.Settings.battleCameraNearClip;
                    var battleFarClip = Global.Settings.battleCameraFarClip;
                    var townNearClip = Global.Settings.townCameraNearClip;
                    var townFarClip = Global.Settings.townCameraFarClip;

                    CameraSettingLable(string.Format("LookAt角度(标准值：{0}）", mCameraAngle), sceneData._CameraAngle.ToString());
                    CameraSettingLable(string.Format("近裁剪（战斗标准值：{0}，城镇标准值：{1}）", battleNearClip, townNearClip), sceneData._CameraNearClip.ToString());
                    CameraSettingLable(string.Format("远裁剪（战斗标准值：{0}，城镇标准值：{1}）", battleFarClip, townFarClip), sceneData._CameraFarClip.ToString());
                    CameraSettingLable(string.Format("视域大小(标准值：{0}）", mCameraSize), sceneData._CameraSize.ToString());

                    EditorGUILayout.BeginHorizontal();
                    bool hasChange = false;
                    if(GUILayout.Button("一键设置战斗标准数值"))
                    {
                        sceneData._CameraLookHeight = mBattleCameraLookHeight;
                        sceneData._CameraDistance = mBattleCameraDistance;
                        sceneData._CameraNearClip = battleNearClip;
                        sceneData._CameraFarClip = battleFarClip;
                        hasChange = true;
                    }
                    if (GUILayout.Button("一键设置城镇标准数值"))
                    {
                        sceneData._CameraLookHeight = mTownCameraLookHeight;
                        sceneData._CameraDistance = mTownCameraDistance;
                        sceneData._CameraNearClip = townNearClip;
                        sceneData._CameraFarClip = townFarClip;
                        hasChange = true;
                    }
                    if(hasChange)
                    {
                        sceneData._CameraAngle = mCameraAngle;
                        sceneData._CameraSize = mCameraSize;
                    }
                    EditorGUILayout.EndHorizontal();

                    sceneData._CameraPersp = EditorGUILayout.Toggle("开启透视投影：", sceneData._CameraPersp);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (GUILayout.Button("设置相机", "minibutton"))
                    {
                        UpdateMainCamera();
                        FocusGameView();
                    }
                    if (EditorGUI.EndChangeCheck() && autoSetCamera)
                    {
                        UpdateMainCamera();
                        FocusGameView();
                    }

                    autoSetCamera = EditorGUILayout.Toggle("自动更新相机：", autoSetCamera);
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", "相机范围设置", fontStyle3);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (mainCameraObj)
            {
                //X值
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Vector3 cameraPosX = mainCameraObj.transform.localPosition;
                cameraPosX.x = EditorGUILayout.FloatField("相机X值：", cameraPosX.x);

                if (EditorGUI.EndChangeCheck())
                {
                    sceneData.animCameraPostion = new AnimVector3(cameraPosX);
                    FocusGameView();

                }
                if (GUILayout.Button("设置X左限", "minibutton"))
                {
                    sceneData._CameraXRange.x = cameraPosX.x;
                    sceneData._CameraXRange.x = Mathf.Clamp(sceneData._CameraXRange.x, -1000, sceneData._CameraXRange.y - 0.00001f);
                    FocusGameView();
                }

                if (GUILayout.Button("设置X右限", "minibutton"))
                {
                    sceneData._CameraXRange.y = cameraPosX.x;
                    sceneData._CameraXRange.y = Mathf.Clamp(sceneData._CameraXRange.y, sceneData._CameraXRange.x + 0.00001f, 1000);
                    FocusGameView();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                sceneData._CameraXRange = EditorGUILayout.Vector2Field("相机X范围：", sceneData._CameraXRange);
                sceneData._CameraXRange.x = Mathf.Clamp(sceneData._CameraXRange.x, -1000, sceneData._CameraXRange.y - 0.00001f);
                sceneData._CameraXRange.y = Mathf.Clamp(sceneData._CameraXRange.y, sceneData._CameraXRange.x + 0.00001f, 1000);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("定位X左限", "minibutton"))
                {
                    Vector3 pos = mainCameraObj.transform.localPosition;
                    pos.x = sceneData._CameraXRange.x;
                    sceneData.animCameraPostion.target = pos;
                    FocusGameView();
                }

                if (GUILayout.Button("定位X右限", "minibutton"))
                {
                    Vector3 pos = mainCameraObj.transform.localPosition;
                    pos.x = sceneData._CameraXRange.y;
                    sceneData.animCameraPostion.target = pos;
                    FocusGameView();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                //Z值
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Vector3 cameraPosZ = mainCameraObj.transform.localPosition;
                cameraPosZ.z = EditorGUILayout.FloatField("相机Z值：", cameraPosZ.z);

                if (EditorGUI.EndChangeCheck())
                {
                    sceneData.animCameraPostion = new AnimVector3(cameraPosZ);
                    FocusGameView();
                }

                if (GUILayout.Button("设置Z下限", "minibutton"))
                {
                    sceneData._CameraZRange.x = cameraPosZ.z;
                    sceneData._CameraZRange.x = Mathf.Clamp(sceneData._CameraZRange.x, -1000, sceneData._CameraZRange.y - 0.00001f);
                    FocusGameView();
                }

                if (GUILayout.Button("设置Z上限", "minibutton"))
                {
                    sceneData._CameraZRange.y = cameraPosZ.z;
                    sceneData._CameraZRange.y = Mathf.Clamp(sceneData._CameraZRange.y, sceneData._CameraZRange.x + 0.00001f, 1000);
                    FocusGameView();
                }


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                sceneData._CameraZRange = EditorGUILayout.Vector2Field("相机Z范围：", sceneData._CameraZRange);

                sceneData._CameraZRange.x = Mathf.Clamp(sceneData._CameraZRange.x, -1000, sceneData._CameraZRange.y - 0.00001f);
                sceneData._CameraZRange.y = Mathf.Clamp(sceneData._CameraZRange.y, sceneData._CameraZRange.x + 0.00001f, 1000);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("定位Z下限", "minibutton"))
                {
                    Vector3 pos = mainCameraObj.transform.localPosition;
                    pos.z = sceneData._CameraZRange.x;
                    //mainCameraObj.transform.localPosition = pos;
                    sceneData.animCameraPostion.target = pos;
                    FocusGameView();
                }

                if (GUILayout.Button("定位Z上限", "minibutton"))
                {
                    Vector3 pos = mainCameraObj.transform.localPosition;
                    pos.z = sceneData._CameraZRange.y;
                    //mainCameraObj.transform.localPosition = pos;
                    //animCameraPostion = new AnimVector3(pos);
                    sceneData.animCameraPostion.target = pos;
                    FocusGameView();
                }



                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel -= 1;
    }
#endregion

    public void DragMainCamera()
    {
        if (mainCameraObj == null)
        {
            return;
        }

    }
    void UpdateMainCamera(bool bReset = true)
    {
        if (mainCamera == null || mainCameraObj == null)
        {
            mainCameraObj = GameObject.Find("Main Camera");
            if (mainCameraObj)
            {
                mainCamera = mainCameraObj.GetComponent<Camera>();
            }
        }

        Vector3 cameraPosition = new Vector3(0, sceneData._CameraLookHeight, 0);
        cameraPosition.y += Mathf.Sin(sceneData._CameraAngle * Mathf.Deg2Rad) * sceneData._CameraDistance;
        cameraPosition.z -= Mathf.Cos(sceneData._CameraAngle * Mathf.Deg2Rad) * sceneData._CameraDistance;

        if (mainCamera && mainCameraObj)
        {
            sceneData.animCameraPostion.target = cameraPosition;
            //animCameraPostion = new AnimVector3(cameraPosition);
            //mainCameraObj.transform.localPosition = cameraPosition;
            mainCameraObj.transform.localRotation = Quaternion.Euler(sceneData._CameraAngle, 0, 0);

            if (bReset)
            {
                Vector3 pos = cameraPosition;
                pos.x = (sceneData._CameraXRange.x + sceneData._CameraXRange.y) / 2;
                pos.z = (sceneData._CameraZRange.x + sceneData._CameraZRange.y) / 2;
                mainCameraObj.transform.localPosition = pos;
                sceneData.animCameraPostion.target = pos;
                RepaintAll();
            }


            if (sceneData._CameraPersp)
            {
                mainCamera.orthographic = false;
                mainCamera.fieldOfView = 10.0f;
                mainCamera.orthographicSize = sceneData._CameraSize;
                mainCamera.nearClipPlane = sceneData._CameraNearClip;
                mainCamera.farClipPlane = sceneData._CameraFarClip;
            }
            else
            {
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = sceneData._CameraSize;
                mainCamera.nearClipPlane = sceneData._CameraNearClip;
                mainCamera.farClipPlane = sceneData._CameraFarClip;
            }
        }
    }

#region Static Tool -- Annotation
    [MenuItem("[关卡编辑器]/区域/AnnotationsDisableIcons", false, 1)]
    static void AnnotationsDisableIcons()
    {
        AnnotationsIcon(0);
    }

    [MenuItem("[关卡编辑器]/区域/AnnotationsEnableIcons", false, 1)]
    static void AnnotationsEnableIcons()
    {
        AnnotationsIcon(1);
    }

    static void AnnotationsIcon(int iValue)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        object obj = assembly.CreateInstance("UnityEditor.AnnotationUtility");
        System.Type type = obj.GetType();
        object result = type.InvokeMember("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[] { });
        foreach (var item in result as IEnumerable)
        {
            object classID = item.GetType().GetField("classID", BindingFlags.Instance | BindingFlags.Public).GetValue(item);
            object scriptClass = item.GetType().GetField("scriptClass", BindingFlags.Instance | BindingFlags.Public).GetValue(item);
            object[] paramters =
                new object[] {
                    classID,
                    scriptClass,
                    iValue
                };

            type.InvokeMember("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, paramters);
        }
    }
#endregion

#region Unity Tool
    public delegate void OnToolChangedFunc(Tool from, Tool to);
    bool toolsLocked = false;

    void OnToolsChange(Tool from, Tool to)
    {
        //if(proxy.)
        if (toolCurrent == Tool.View)
        {
            ToolsRawSet(Tool.View);
        }
        else
        {
            ToolsRawSet(Tool.View);
        }
    }

    void ToolsRawSet(Tool value)
    {
        System.Type type = typeof(Tools);
        PropertyInfo pinfo = type.GetProperty("get", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo minfo = pinfo.GetGetMethod(true);

        Tools tools = minfo.Invoke(null, new object[] { }) as Tools;
        FieldInfo finfo = type.GetField("currentTool", BindingFlags.Instance | BindingFlags.NonPublic);
        finfo.SetValue(tools, value, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField, null, null);

        RepaintAll();
    }

    void HookToolsChange(bool bHook)
    {
        System.Object onChangeFunc = null;
        if (bHook)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var types = assembly.GetType("UnityEditor.Tools+OnToolChangedFunc");
            var minfo = typeof(DSceneDataEditorWindow).GetMethod("OnToolsChange", BindingFlags.Instance | BindingFlags.NonPublic);
            onChangeFunc = Delegate.CreateDelegate(types, this, minfo);
        }

        System.Type type = typeof(Tools);
        FieldInfo onTCI = type.GetField("onToolChanged", BindingFlags.Static | BindingFlags.NonPublic);
        onTCI.SetValue(null, onChangeFunc, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null);
    }

    private static int s_ViewToolID;
    private static int s_RectSelectionID;
    private static int s_RectSelectionHash = "RectSelectionHash".GetHashCode();
    void LockTools()
    {
        if (bInArtistMode == false)
        {
            //Tools.hidden = true;
            toolsLocked = true;
            HookToolsChange(true);
            ToolsRawSet(Tool.View);

            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.SceneViewMotion");
            FieldInfo finfo = type.GetField("s_ViewToolID", BindingFlags.Static | BindingFlags.NonPublic);
            s_ViewToolID = (int)finfo.GetValue(null);

            //type = assembly.GetType("UnityEditor.RectSelection");
            //finfo = type.GetField("s_RectSelectionID", BindingFlags.Static | BindingFlags.NonPublic);
            s_RectSelectionID = GUIUtility.GetControlID(s_RectSelectionHash, FocusType.Passive);
        }
    }

    void UnLockTools()
    {
        //Tools.hidden = false;
        toolsLocked = false;
        HookToolsChange(false);
    }

    void LockToolsUpdate()
    {
        UnLockTools();
        if(bInArtistMode == false && toolsLocked == true)
        {
            LockTools();
        }
    }

    private static GUIContent[] s_ToolIcons;
    private static GUIContent[] s_ViewToolIcons;
    private GameObject[] prefabs;
    private Texture2D[] prefabPreviews;
    private static void InitializeToolIcons()
    {
        if (s_ToolIcons != null)
        {
            return;
        }
        s_ToolIcons = new GUIContent[]
        {
            EditorGUIUtility.IconContent("MoveTool"),
                EditorGUIUtility.IconContent("RotateTool"),
                EditorGUIUtility.IconContent("ScaleTool"),
                EditorGUIUtility.IconContent("RectTool"),
                EditorGUIUtility.IconContent("MoveTool On"),
                EditorGUIUtility.IconContent("RotateTool On"),
                EditorGUIUtility.IconContent("ScaleTool On"),
                EditorGUIUtility.IconContent("RectTool On")
        };

        s_ViewToolIcons = new GUIContent[]
        {
            EditorGUIUtility.IconContent("ViewToolOrbit"),
                EditorGUIUtility.IconContent("ViewToolMove"),
                EditorGUIUtility.IconContent("ViewToolZoom"),
                EditorGUIUtility.IconContent("ViewToolOrbit"),
                EditorGUIUtility.IconContent("ViewToolOrbit On"),
                EditorGUIUtility.IconContent("ViewToolMove On"),
                EditorGUIUtility.IconContent("ViewToolZoom On"),
                EditorGUIUtility.IconContent("ViewToolOrbit On")
        };
    }

    private void RepaintAll()
    {
        System.Type type = typeof(Tools);
        type.GetMethod("RepaintAllToolViews", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
    }

    private Tool toolCurrent;
    private static GUIContent[] s_ShownToolIcons = new GUIContent[5];
    private void DoToolButtons()
    {
        InitializeToolIcons();
        Rect rt = GUILayoutUtility.GetRect(1, 30f);
        bool changed = GUI.changed;
        GUI.changed = false;
        int num = (int)(toolCurrent);
        for (int i = 1; i < 5; i++)
        {
            s_ShownToolIcons[i] = s_ToolIcons[i - 1 + ((i != num) ? 0 : 4)];
            s_ShownToolIcons[i].tooltip = s_ToolIcons[i - 1].tooltip;
        }
        s_ShownToolIcons[0] = s_ViewToolIcons[(int)(Tools.viewTool + ((num != 0) ? 0 : 4))];
        num = GUI.Toolbar(new Rect(rt.x + 5f, rt.y + 5f, 160f, 24f), num, s_ShownToolIcons, "Command");
        if (GUI.changed)
        {
            if (toolCurrent != (Tool)num)
            {
                RepaintAll();
            }
            toolCurrent = (Tool)num;
            OnToolsChange(toolCurrent, toolCurrent);
        }
        GUI.changed |= changed;
    }
    private void OnInit()
    {
        if (prefabs == null)
        {
            prefabs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            prefabPreviews = new Texture2D[prefabs.Length];
        }

    }
#endregion

#region Monster Update
    Vector2 monsterScroll;
    GameObject obj;

    Editor gameObjPreview;
    Editor entityEditor;


    bool Is2DMode(EntityConfigInfo info)
    {
        return info.type != DEntityType.BOSS
            && info.type != DEntityType.MONSTER
            && info.type != DEntityType.NPC 
            && info.type != DEntityType.ELITE 
            && info.type != DEntityType.MONSTERDESTRUCT;
    }
    void InitEditor(GameObject obj, EntityConfigInfo cinfo)
    {
        if (gameObjPreview != null)
        {
            Type t = gameObjPreview.GetType();

            var info = t.GetField("previewDir", BindingFlags.NonPublic | BindingFlags.Instance);


            Vector2 dir = new Vector2(120f, -20f);
            if (obj && Is2DMode(cinfo))
            {
                dir = new Vector2(obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.y > 0 ? -obj.transform.localEulerAngles.x : obj.transform.localEulerAngles.x);
            }

            if (null != info)
            {
                info.SetValue(gameObjPreview, dir);
            }
        }
    }

    void SampleEditor()
    {
        if (gameObjPreview != null)
        {
            Type t = gameObjPreview.GetType();

            var info = t.GetField("previewDir", BindingFlags.NonPublic | BindingFlags.Instance);

            var value = info.GetValue(gameObjPreview);
            Debug.Log(value.ToString());
        }
    }


    void OnGUI_Monster()
    {
        LoadMonsterConfig();

        //sceneData._BirthPositionOffset = EditorGUILayout.Vector2Field("出生点偏移:", sceneData._BirthPositionOffset);
        var birth = new Vector2(sceneData._birthposition.position.x, sceneData._birthposition.position.z);
        EditorGUILayout.Vector2Field("出生点:", birth);

        var hellbirth = new Vector2(sceneData._hellbirthposition.position.x, sceneData._hellbirthposition.position.z);
        EditorGUILayout.Vector2Field("深渊柱子出生点:", hellbirth);

        //_updateBirthPosition();

        //monsterScroll = EditorGUILayout.BeginScrollView(monsterScroll);

        EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);
        bMonsterResourceToggle = EditorGUILayout.Foldout(bMonsterResourceToggle, "Resource: ", EditorStyles.foldout);
        if (bMonsterResourceToggle)
        {
            EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            var str = EditorGUILayout.TextField(mSearchString, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.MinWidth(1), GUILayout.MaxWidth(1000) });
            if (str != mSearchString)
            {
                mSearchString = str;
                mSearchDirty = true;

                if (str.Length > 0)
                {
                    entityResourceList.filter = findItem =>
                    {
                        if (findItem.hasChildren)
                        {
                            return true;
                        }
                        else if (findItem.texts.Length > 0)
                        {
                            return findItem.texts[0].Contains(mSearchString);
                        }

                        return false;
                    };
                    entityResourceList.bExpandAll = true;
                }
                else
                {
                    entityResourceList.filter = null;
                    entityResourceList.bExpandAll = false;
                }

                entityResourceList.MarkVisibleDirty();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical(GUILayout.Width(MonsterPicSize));
            GUILayout.Space(2);
            EditorGUILayout.Popup(0, new string[] { }, GUILayout.Width(MonsterPicSize));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


            MonsterResourceHeight = (int)MonsterResoucrceBlock.OnLayoutGUI();
            EditorGUILayout.BeginHorizontal();
            entityResourceList.OnLayoutGUI(MonsterResourceHeight);
            EditorUI.UIListItem selecteditem = entityResourceList.selected;
            if (selecteditem != null && selecteditem.userdata != null)
            {
                EntityConfigInfo info = selecteditem.userdata as EntityConfigInfo;
                obj = LoadAssetAtPath(info.resourcepath);
                var pre = gameObjPreview;
                //SampleEditor();
                Editor.CreateCachedEditor(obj, null, ref gameObjPreview);
                if (pre != gameObjPreview)
                {
                    InitEditor(obj, info);
                }
            }

            EditorGUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(MonsterResourceHeight), GUILayout.Height(MonsterResourceHeight) });
            if (gameObjPreview != null)
            {
                Rect rt = GUILayoutUtility.GetRect(1, MonsterResourceHeight);
                gameObjPreview.OnPreviewGUI(rt, "OL Box");
            }
            else
            {
                Rect rt = GUILayoutUtility.GetRect(1, MonsterResourceHeight);
                EditorGUI.DrawPreviewTexture(rt, Texture2D.blackTexture, null, ScaleMode.ScaleToFit);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);
        bMonsterListToggle = EditorGUILayout.Foldout(bMonsterListToggle, "List: ", EditorStyles.foldout);
        if (bMonsterListToggle)
        {

            EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            var str = EditorGUILayout.TextField(mSearchListString, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.MinWidth(1), GUILayout.MaxWidth(1000) });
            if (str != mSearchListString)
            {
                mSearchListString = str;

                if (mSearchListString.Length > 0)
                {
                    entityList.filter = findItem =>
                    {
                        if (findItem.hasChildren)
                        {
                            return true;
                        }
                        else if (findItem.texts.Length > 0)
                        {
                            return findItem.texts[0].Contains(mSearchListString);
                        }

                        return false;
                    };
                    entityList.bExpandAll = true;
                }
                else
                {
                    entityList.filter = null;
                    entityList.bExpandAll = false;
                }

                entityList.MarkVisibleDirty();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical(GUILayout.Width(MonsterPicSize));
            GUILayout.Space(2);
            EditorGUILayout.Popup(0, new string[] { }, GUILayout.Width(MonsterPicSize));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            MonsterListHeight = (int)MonsterListBlock.OnLayoutGUI();
            entityList.OnLayoutGUI(MonsterListHeight);
            EditorGUILayout.EndVertical();
        }

        bEntityGroupEditor = EditorGUILayout.Foldout(bEntityGroupEditor, "EntityGroupEditor: ", EditorStyles.foldout);
        if (bEntityGroupEditor)
        {
            EditorGUI.indentLevel++;
            {
                for (int i = 0; i < sceneData._monsterinfo.Length; ++i)
                {
                    var item = sceneData._monsterinfo[i];
                    item.SetID(item.resid);

                    EditorGUILayout.LabelField(string.Format("{0},{1},{2}", item.monID, item.monLevel, item.Name));
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    globalLevel = EditorGUILayout.IntField("集体修改等级为", globalLevel);
                    globalLevel %= 100;

                    if (GUILayout.Button("设置等级", "minibutton"))
                    {
                        for (int i = 0; i < sceneData._monsterinfo.Length; ++i)
                        {
                            var item = sceneData._monsterinfo[i];
                            var level = item.monLevel;
                            item.monLevel = globalLevel;

                            //var cell = TableManagerEditor.instance.GetTableItem<UnitTable>(item.resid);
                            //if (cell != null)
                            {
                                item.globalid = item.resid * 100 + i;
                            }
                            //else
                            //{
                            //    Debug.LogErrorFormat("没有等级为{0}的{1}，ID {2}", item.monLevel, item.name, item.resid);
                            //    item.monLevel = level;
                            //}
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        bEntityEditor = EditorGUILayout.Foldout(bEntityEditor, "Entity: ", EditorStyles.foldout);
        if (bEntityEditor)
        {
            if (entityEditor == null)
            {
                entityEditor = Editor.CreateEditor(DSceneEntitySelection.Instance, null);
            }

            if (entityEditor)
            {
                entityEditor.OnInspectorGUI();
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void UpdateDoorName(EditorUI.UIListBox entityList)
    {
        var itemList = entityList.GetVisibleRows();
        DEntityType[] doorType = new DEntityType[] { DEntityType.TRANSPORTDOOR, DEntityType.TOWNDOOR, DEntityType.REGION };
        List<EntityConfigInfo> infoList = new List<EntityConfigInfo>();
        for (int i = 0; i < doorType.Length; i++)
        {
            infoList.AddRange(GetEntityListByType(doorType[i]));
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            var item = itemList[i];
            if (item.userdata == null)
            {
                continue;
            }
            var userdata = item.userdata as DEntityInfo;
            if (userdata == null)
            {
                continue;
            }
            var info = infoList.Find(x => x.id == userdata.resid);
            if (info == null)
            {
                continue;
            }
            item.texts = new string[] { info.name, info.description };
            userdata.Name = info.name;
        }
    }

#endregion


    string[] entityTitles;
    Vector2 baseScroll;

    Vector2 tagScroll;
    Vector2 dataScroll;
    void OnGUI()
    {

        _UpdateKeyEvent();

        GUIControls.UIStyles.UpdateEditorStyles();

        CreateFontStyle();
        EditorGUILayout.Separator();
        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(7f);

            if (dugeonData && dugeonDataEditor)
                editorTags = (LevelEditorTags)TabEditorTags(editorTags, LevelEditorTags.Dungoen);

            editorTags = (LevelEditorTags)TabEditorTags(editorTags, LevelEditorTags.SceneData);
        }

        if (dugeonData && dugeonDataEditor && editorTags == LevelEditorTags.Dungoen)
        {
            tagScroll = EditorGUILayout.BeginScrollView(tagScroll);
            dugeonDataEditor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
            return;
        }
        else
        {
            editorTags = LevelEditorTags.SceneData;
        }


        if (sceneData == null)
        {
            GUILayout.BeginHorizontal("GroupBox");
            GUILayout.Label("创建或选择区域文件", "CN EntryInfo");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("创建区域文件"))
                FileTools.CreateAsset<DSceneData>("New DSceneData");

            if (GUILayout.Button("定位默认路径"))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Data/SceneData", typeof(UnityEngine.Object));
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }

            return;
        }

        if (Event.current.type == EventType.MouseDown && EditorWindow.focusedWindow == this)
        {
            GUIUtility.keyboardControl = 0;
        }

        if (sceneData == null)
        {
            return;
        }



        dataScroll = EditorGUILayout.BeginScrollView(dataScroll);

        EditorGUILayout.BeginHorizontal("GroupBox");
        {
            GUI.color = Color.green;
            if (GUILayout.Button("显示区域文件"))
            {
                EditorGUIUtility.PingObject(sceneData);
            }

            GUI.color = Color.red;
            if (GUILayout.Button("返回主页面"))
            {
                sceneData = null;
                return;
            }
            GUI.color = Color.white;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 200;
        OnGUI_Header();

        if (editorRoot == null)
        {
            EditorGUILayout.EndScrollView();
            return;
        }

        LockTools();
        EditorGUILayout.Separator();

        bool bOld = bInArtistMode;
        bInArtistMode = VisiableToggle("ArtistMode", bInArtistMode);

        if(bOld != bInArtistMode)
        {
            LockToolsUpdate();
        }

        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(7f);
            TabEditorMode(ref proxy.editmode, SceneEditorMode.NORMAL.GetDescription(), SceneEditorMode.NORMAL);
            TabEditorMode(ref proxy.editmode, SceneEditorMode.BLOCKLAYER.GetDescription(), SceneEditorMode.BLOCKLAYER);
            TabEditorMode(ref proxy.editmode, SceneEditorMode.ENTITYS.GetDescription(), SceneEditorMode.ENTITYS);
        }

        EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);

        EditorGUILayout.LabelField("", proxy.editmode.GetDescription(), fontStyle2);
        EditorGUILayout.Space();

        if (proxy.editmode == SceneEditorMode.NORMAL
                || proxy.editmode == SceneEditorMode.BLOCKLAYER)
        {
            //baseScroll = EditorGUILayout.BeginScrollView(baseScroll);
            OnGUI_BlockLayer();
            //EditorGUILayout.EndScrollView();
        }
        else if (proxy.editmode == SceneEditorMode.ENTITYS)
        {
            OnGUI_Monster();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(sceneData);
        }
    }

    private Vector2 _convertPosition2LogicPosition(Vector2 vec)
    {
        vec.x = Mathf.Clamp(vec.x, sceneData._LogicXSize.x, sceneData._LogicXSize.y);
        vec.y = Mathf.Clamp(vec.y, sceneData._LogicZSize.x, sceneData._LogicZSize.y);

        return new Vector2() { x = (int)(vec.x / sceneData._GridSize.x), y = (int)(vec.y / sceneData._GridSize.y) };
    }

    private bool _checkIsInBlock(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x > sceneData.LogicX)
        {
            return false;
        }

        if (y >= sceneData.LogicZ)
        {
            return false;
        }

        int idx = y + x * sceneData.LogicZ;

        if (idx >= sceneData._blocklayer.Length)
        {

            return false;
        }

        if (idx < 0)
        {
            return false;
        }

        return sceneData._blocklayer[idx] > 0;
    }

    //private void _updateBirthPosition()
    //{
    //    //sceneData._BirthPosition.x = Mathf.Clamp(sceneData._BirthPosition.x, sceneData._LogicXSize.x + sceneData._BirthPositionOffset.x, sceneData._LogicXSize.y - sceneData._BirthPositionOffset.x);
    //    //sceneData._BirthPosition.y = Mathf.Clamp(sceneData._BirthPosition.y, sceneData._LogicZSize.x + sceneData._BirthPositionOffset.y, sceneData._LogicZSize.y - sceneData._BirthPositionOffset.y);
    //    _checkBirthPosition();
    //}

    private void _checkBirthPosition()
    {
        //var logicPos = _convertPosition2LogicPosition(sceneData._BirthPosition);
        //if (_checkIsInBlock(logicPos))
        //{
        //    Debug.LogError("出生位置在阻挡中");
        //}
    }

    private T[] _changeBlockLayer<T>(T[] source, LogicRange origin, LogicRange target)
    {
        T[] originByte = source;
        T[] newByte = new T[target.Length];
        for (int i = 0; i < originByte.Length; ++i)
        {
            var pos = origin.Index2Pos(i);
            int targetIdx = target.Pos2Index(pos);

            if (targetIdx >= 0 && targetIdx < newByte.Length)
            {
                newByte[targetIdx] = originByte[i];
            }
        }
        return newByte;
    }


    void CreateGrassSizeData()
    {
        sceneData._grasslayer = new ushort[sceneData.LogicX * sceneData.LogicZ];
        Array.Clear(sceneData._grasslayer, 0, sceneData._grasslayer.Length);
    }
    void CreateEcosystemSizeData()
    {
        sceneData._ecosystemLayer = new ushort[sceneData.LogicX * sceneData.LogicZ];
        Array.Clear(sceneData._ecosystemLayer, 0, sceneData._ecosystemLayer.Length);
    }

    void CreateEventAreaSizeData()
    {
        sceneData._eventAreaLayer = new byte[sceneData.LogicX * sceneData.LogicZ];
        Array.Clear(sceneData._eventAreaLayer, 0, sceneData._eventAreaLayer.Length);
    }
    
    private struct LogicRange
    {
        public LogicRange(int xmin, int xmax, int zmin, int zmax)
        {
            XMin = xmin;
            XMax = xmax;
            ZMin = zmin;
            ZMax = zmax;
        }

        public int XSize
        {
            get
            {
                return XMax - XMin;
            }
        }

        public int ZSize
        {
            get
            {
                return ZMax - ZMin;
            }
        }

        public int Length
        {
            get
            {
                return XSize * ZSize;
            }
        }


        public int Pos2Index(KeyValuePair<int, int> kv)
        {
            return Pos2Index(kv.Key, kv.Value);
        }

        /// <summary>
        /// 全局坐标 转换到 索引
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Pos2Index(int x, int y)
        {
            if (x < XMin || y < ZMin)
            {
                return -1;
            }

            if (x >= XMax || y >= ZMax)
            {
                return -1;
            }

            x -= XMin;
            y -= ZMin;

            return y * XSize + x;
        }

        /// <summary>
        /// 索引 转换到 全局坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public KeyValuePair<int, int> Index2Pos(int index)
        {
            if (index >= 0 && index < Length)
            {
                return new KeyValuePair<int, int>(index % XSize + XMin, index / XSize + ZMin);
            }

            return new KeyValuePair<int, int>(-1, -1);
        }

        public int XMin { get; private set; }
        public int XMax { get; private set; }

        public int ZMin { get; private set; }
        public int ZMax { get; private set; }
    }

    void UpdateBlockSizeData()
    {
        int ow = sceneData.LogicX;
        int oh = sceneData.LogicZ;

        LogicRange origin = new LogicRange(sceneData._LogicXmin, sceneData._LogicXmax, sceneData._LogicZmin, sceneData._LogicZmax);

        sceneData._LogicXmin = (int)(sceneData._LogicXSize.x / sceneData._GridSize.x);
        sceneData._LogicXmax = (int)(sceneData._LogicXSize.y / sceneData._GridSize.x);
        sceneData._LogicZmin = (int)(sceneData._LogicZSize.x / sceneData._GridSize.y);
        sceneData._LogicZmax = (int)(sceneData._LogicZSize.y / sceneData._GridSize.y);

        LogicRange target = new LogicRange(sceneData._LogicXmin, sceneData._LogicXmax, sceneData._LogicZmin, sceneData._LogicZmax);

        //sceneData._blocklayer = new byte[sceneData.LogicX * sceneData.LogicZ];
        //sceneData._blocklayer = _changeBlockLayer(ow, oh, sceneData.LogicX, sceneData.LogicZ, new byte[sceneData.LogicX * sceneData.LogicZ]);
        sceneData._blocklayer = _changeBlockLayer(sceneData._blocklayer, origin, target);

        if (sceneData._grasslayer != null)
        {
            sceneData._grasslayer = _changeBlockLayer(sceneData._grasslayer, origin, target);
        }
        if (sceneData._ecosystemLayer != null)
        {
            sceneData._ecosystemLayer = _changeBlockLayer(sceneData._ecosystemLayer, origin, target);
        }
        if (sceneData._eventAreaLayer != null)
        {
            sceneData._eventAreaLayer = _changeBlockLayer(sceneData._eventAreaLayer, origin, target);
        }
        //_updateBirthPosition();

        proxy.OnLogicSizeChange();
    }

    // Update is called once per frame
    void CreateTexture()
    {
        if (_GridTexture == null)
        {
            _GridTexture = AssetDatabase.LoadAssetAtPath(
                    "Assets/ThirdParty/00ExternalTools/UFE/Stages/Textures/training_grid_small.png", typeof(Texture2D)) as Texture2D;
        }

        if (_BlockTexture == null)
        {
            _BlockTexture = AssetDatabase.LoadAssetAtPath(
                    "Assets/ThirdParty/00ExternalTools/UFE/Stages/Textures/training_grid_red_small.png", typeof(Texture2D)) as Texture2D;
        }
    }

    Camera mainCamera;
    GameObject mainCameraObj;

    void Populate(bool bSend = false)
    {
        this.titleContent = new GUIContent("LevelEditor" + DSceneDataEditorWindow.EDITOR_V);

        CreateTexture();
        CreateUI();

        if (bSend)
        {
            sceneData = sendData;
            if (sceneData != null)
            {
                _GridSizeBack = sceneData._GridSize;
                _LogicXSizeBack = sceneData._LogicXSize;
                _LogicZSizeBack = sceneData._LogicZSize;
                UpdateMainCamera(true);
            }

            sendData = null;

            if (dugeonDataEditor)
            {
                Editor.DestroyImmediate(dugeonDataEditor);
            }
            if (dugeonData)
            {
                dugeonDataEditor = Editor.CreateEditor(dugeonData);
                (dugeonDataEditor as DDungeonInspector).mCurSelectX = editorSelectX;
                (dugeonDataEditor as DDungeonInspector).mCurSelectY = editorSelectY;
                (dugeonDataEditor as DDungeonInspector).mLastSelectX = editorSelectX;
                (dugeonDataEditor as DDungeonInspector).mLastSelectY = editorSelectY;
                (dugeonDataEditor as DDungeonInspector).mAutoLoadSceneData = this.mAutoLoadSceneData;
                (dugeonDataEditor as DDungeonInspector).mDungeonData = mParamDungeonData;
                if (mParamDungeonData != null && mAutoLoadSceneData)
                {
                    editorTags = LevelEditorTags.SceneData;
                }
                else
                {
                    editorTags = LevelEditorTags.Dungoen;
                }
            }

            return;
        }

        editorTags = LevelEditorTags.SceneData;

        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DSceneData), UnityEditor.SelectionMode.Assets);
        if (selection.Length > 0)
        {
            if (selection[0] == null) return;
            sceneData = (DSceneData)selection[0];

            _GridSizeBack = sceneData._GridSize;
            _LogicXSizeBack = sceneData._LogicXSize;
            _LogicZSizeBack = sceneData._LogicZSize;
            UpdateMainCamera(true);
        }
    }
}
