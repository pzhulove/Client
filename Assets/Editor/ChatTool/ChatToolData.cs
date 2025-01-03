using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct ChatToolItem
{
    public string name;
    public string format;
    public string[] pararmterNames;
    public string[] pararmterValues;
    public int priority;

    public ChatToolItem(string n, string format, string[] pn,int sortPriority)
    {
        this.name = n;
        this.format = format;
        if (pn == null)
        {
            pararmterNames = new string[0];
            pararmterValues = new string[0];
            priority = 0;
        }

        else
        {
            //pararmterNames = pn;
            pararmterNames = new string[pn.Length];
            for (int i = 0; i < pararmterNames.Length; ++i)
                pararmterNames[i] = pn[i];

            pararmterValues = new string[pararmterNames.Length];
            for (int i = 0; i < pararmterValues.Length; ++i)
                pararmterValues[i] = "";
            priority = sortPriority;
        }

    }
}

public class ChatToolData : ScriptableObject
{

    public ChatToolItem[] chatItems = new ChatToolItem[]
    {
        new ChatToolItem("改变等级", "!!uplevel num={0}", new string[] { "等级" },0)
    };

    public void AddItem(ChatToolItem item)
    {
        var newItems = new ChatToolItem[chatItems.Length + 1];
        for (int i = 0; i < chatItems.Length; ++i)
            newItems[i] = chatItems[i];

        newItems[chatItems.Length] = item;

        chatItems = newItems;

        EditorUtility.SetDirty(this);

        AssetDatabase.SaveAssets();
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= chatItems.Length)
            return;

        var newItems = new ChatToolItem[chatItems.Length - 1];

        for (int i = 0, j = 0; i < chatItems.Length; ++i)
        {
            if (i == index)
                ++i;
            else
                newItems[j++] = chatItems[i];
        }

        chatItems = newItems;

        EditorUtility.SetDirty(this);

        AssetDatabase.SaveAssets();
    }
}

public class ChatToolWindow : EditorWindow
{
    private struct OneKeyWearStruct
    {
        public int SuitId;
        public string Name;
    }

    public static ChatToolWindow chatToolWindow;

    protected string command = "";

    protected ChatToolData chatToolData = null;

    protected const string chatToolFilePath = "Assets/Editor/ChatTool/chattool.asset";

    protected bool showCommand = false;
    protected bool addCommand = false;
    protected bool deleteCommand = false;
    protected bool propSendUseID = false;

    protected string cmdName;
    protected string cmdFormat;
    protected string[] cmdPararmterNames = new string[0];
    protected string[] cmdPararmterValues = new string[0];
    protected int paraNum;
    protected int priority;

    private Vector2 m_pSelectedVec = new Vector2();

    private string mSearchName = String.Empty;

    private static Dictionary<int, List<OneKeyWearStruct>> _jobOneKeyWearSuitIdDic = new Dictionary<int, List<OneKeyWearStruct>>();

    public static void CreateAsset()
    {
        var asset = ScriptableObject.CreateInstance<ChatToolData>();

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(chatToolFilePath);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();

        EditorGUIUtility.PingObject(asset);
        Selection.activeObject = asset;

    }

    GUIStyle fontstyleUnitInfo;


    void CreateFontStyle()
    {
        if (fontstyleUnitInfo == null)
        {
            fontstyleUnitInfo = new GUIStyle(EditorStyles.label);
            fontstyleUnitInfo.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfo.fontSize = 14;
            fontstyleUnitInfo.normal.textColor = Color.green;
            fontstyleUnitInfo.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleUnitInfo.active.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleUnitInfo.focused.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }
    }


    [MenuItem("[TM工具集]/作弊工具", false, 1)]
    public static void Init()
    {
        ChatToolData asset = null;


        try
        {
            asset = AssetDatabase.LoadAssetAtPath<ChatToolData>(chatToolFilePath);
            if (asset == null)
            {
                CreateAsset();
                asset = AssetDatabase.LoadAssetAtPath<ChatToolData>(chatToolFilePath);
                if (asset == null)
                {
                    Logger.LogErrorFormat("[作弊工具]加载{0}失败", chatToolFilePath);

                }
            }
        }
        catch (System.Exception e)
        {
            Logger.LogErrorFormat("error:{0}", e.ToString());
        }

        InitOneKeyWearData();

        chatToolWindow = EditorWindow.GetWindow<ChatToolWindow>();
        if (chatToolWindow != null)
        {
            chatToolWindow.chatToolData = asset;
            chatToolWindow.titleContent.text = "作弊工具";
            chatToolWindow.Show();
        }
    }

    //public int DungeonID = -1;
    //private DungeonInspectorList mDungeonList = new DungeonInspectorList();


    public void OnGUI()
    {
        if (chatToolData == null)
        {
            return;
        }

        CreateFontStyle();

        EditorGUILayout.BeginVertical("GroupBox");
        {
            showCommand = EditorGUILayout.Toggle("是否显示命令", showCommand);
            deleteCommand = EditorGUILayout.Toggle("是否显示删除按钮", deleteCommand);
            addCommand = EditorGUILayout.Toggle("添加作弊指令", addCommand);
            if (addCommand)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                cmdName = EditorGUILayout.TextField("名字", cmdName);
                cmdFormat = EditorGUILayout.TextField("作弊指令格式化字符串", cmdFormat);
                paraNum = EditorGUILayout.IntField("参数个数", paraNum);
                priority = EditorGUILayout.IntField("排序优先级", priority);

                if (paraNum > 0 && paraNum <= 10)
                {
                    EditorGUILayout.BeginVertical("GroupBox");
                    if (paraNum != cmdPararmterNames.Length)
                    {
                        var newTemps = new string[paraNum];
                        for (int i = 0; i < paraNum; ++i)
                            newTemps[i] = "";
                        for (int i = 0; i < paraNum && i < cmdPararmterNames.Length; ++i)
                        {
                            newTemps[i] = cmdPararmterNames[i];
                        }

                        cmdPararmterNames = newTemps;
                    }

                    for (int i = 0; i < paraNum; ++i)
                    {
                        cmdPararmterNames[i] = EditorGUILayout.TextField(string.Format("参数{0}名字", (i + 1)), cmdPararmterNames[i]);
                    }
                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("添加命令", "button"))
                {
                    if (chatToolData != null)
                    {
                        chatToolData.AddItem(new ChatToolItem(cmdName, cmdFormat, cmdPararmterNames, priority));
                    }
                }

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("排序", "button"))
            {
                List<ChatToolItem> list = new List<ChatToolItem>(chatToolData.chatItems);

                list.Sort((a, b) =>
                {
                    return b.priority.CompareTo(a.priority);
                });

                chatToolData.chatItems = list.ToArray();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("搜索作弊指令名称：", GUILayout.Width(150));
            mSearchName = EditorGUILayout.TextField(mSearchName);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("GroupBox");
        {
            m_pSelectedVec = EditorGUILayout.BeginScrollView(m_pSelectedVec);
            for (int i = 0; i < chatToolData.chatItems.Length; ++i)
            {
                var label = chatToolData.chatItems[i].name;
                var format = chatToolData.chatItems[i].format;
                var parameters = chatToolData.chatItems[i].pararmterNames;
                var parametersContent = chatToolData.chatItems[i].pararmterValues;
                if (label == null || format == null || parameters == null || parametersContent == null)
                {
                    continue;
                }

                object[] objects = null;
                if (parameters != null)
                {
                    objects = new object[parameters.Length];
                    for (int j = 0; j < parameters.Length; ++j)
                        objects[j] = parametersContent[j];
                    command = string.Format(format, objects);
                }

                if (!mSearchName.Equals(string.Empty) && !label.Contains(mSearchName))
                {
                    continue;
                }

                EditorGUILayout.BeginVertical("GroupBox");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (label.Equals("添加道具") && propSendUseID == false)
                        {
                            if (GUILayout.Button("搜索", "button", GUILayout.Width(50)))
                            {
                                CreateProp(format, parametersContent[0], objects);
                            }
                        }
                        else if (label.Equals("添加套装"))
                        {
                            if (GUILayout.Button("发放套装", "button", GUILayout.Width(70)))
                            {
                                _SendSuitCommand(int.Parse(parametersContent[1]), int.Parse(parametersContent[2]));
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("发送", "button", GUILayout.Width(50)))
                            {
                                _sendGM(command);
                            }
                        }

                        EditorGUILayout.LabelField(label + ":", fontstyleUnitInfo, GUILayout.Width(200));

                        if (showCommand)
                            EditorGUILayout.LabelField(command);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (label.Equals("添加道具"))
                        {
                            if (parameters != null && parameters.Length > 0)
                            {
                                parameters[0] = propSendUseID ? "道具ID:" : "道具名称:";
                            }
                            for (int j = 0; j < parameters.Length; ++j)
                            {
                                EditorGUILayout.LabelField(parameters[j], GUILayout.Width(60));
                                parametersContent[j] = EditorGUILayout.TextField(parametersContent[j], GUILayout.Width(100));
                            }
                            propSendUseID = EditorGUILayout.Toggle(propSendUseID, GUILayout.Width(30));
                            EditorGUILayout.LabelField("使用道具ID创建", GUILayout.Width(100));
                        }
                        else if(label.Equals("转职"))
                        {
                            ActorOccupation curOccupation = (ActorOccupation)int.Parse((parametersContent[0]));
                            var occupationNew = TMEnumHelper.GetChangeEnumType<ActorOccupation>("职业:", curOccupation);
                            parametersContent[0] = ((int)occupationNew).ToString();
                        }
                        else if(label.Equals("添加套装"))
                        {
                            ActorOccupation curOccupation = (ActorOccupation)int.Parse((parametersContent[0]));
                            var occupationNew = TMEnumHelper.GetChangeEnumType<ActorOccupation>("职业:", curOccupation);
                            parametersContent[0] = ((int)occupationNew).ToString();

                            var nameArr = GetSuitNameArrByJobId((int)occupationNew);
                            if (nameArr != null)
                            {
                                int jobId = int.Parse(parametersContent[0]);
                                int lastIndex = GetIndexBySuitId(jobId, int.Parse(parametersContent[1]));
                                var index = EditorGUILayout.Popup("套装:", lastIndex, nameArr, GUILayout.Width(250));
                                parametersContent[1] = GetSuitIdByIndex(jobId, index).ToString();

                                EditorGUILayout.LabelField(parameters[2], GUILayout.Width(60));
                                parametersContent[2] = EditorGUILayout.TextField(parametersContent[2], GUILayout.Width(100));
                            }
                        }
                        else
                        {
                            for (int j = 0; j < parameters.Length; ++j)
                            {
                                parametersContent[j] = EditorGUILayout.TextField(parameters[j], parametersContent[j]);
                            }
                        }

                        //command = EditorGUILayout.TextField(label, command);

                        if (deleteCommand)
                        {
                            if (GUILayout.Button("删除", "button"))
                            {
                                chatToolData.RemoveItem(i);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }


    protected void CreateProp(string format, string parameterContent, object[] objects)
    {
        if (format == null)
            return;

        if (parameterContent == null)
            return;

        if (objects == null)
            return;

        if (objects.Length <= 0)
            return;

        var itemTable = TableManagerEditor.GetInstance().GetTable<ProtoTable.ItemTable>();
        if (itemTable != null)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var item in itemTable)
            {
                var current = item.Value as ProtoTable.ItemTable;
                if (System.Text.RegularExpressions.Regex.IsMatch(current.Name, parameterContent, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    menu.AddItem(new GUIContent(current.Name + "ID:" + current.ID), false, (object obj) =>
                    {
                        objects[0] = obj;
                        command = string.Format(format, objects);
                        _sendGM(command);
                    }, current.ID);
                }

            }
            menu.ShowAsContext();
        }
    }

    public static void _sendGM(string command)
    {
        Protocol.SceneChat req = new Protocol.SceneChat();
        req.channel = 0;

        if (!command.StartsWith("!!"))
            command = "!!" + command;

        req.word = command;
        Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, req);
    }

    /// <summary>
    /// 初始化一键穿戴数据
    /// </summary>
    private static void InitOneKeyWearData()
    {
        _jobOneKeyWearSuitIdDic.Clear();

        var wearTableDic = TableManager.instance.GetTable<OneKeyWearTable>();
        var enumerator = wearTableDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var key = enumerator.Current.Key;
            var value = enumerator.Current.Value as OneKeyWearTable;

            OneKeyWearStruct data = new OneKeyWearStruct();
            data.SuitId = key;
            data.Name = value.Name;

            if (_jobOneKeyWearSuitIdDic.ContainsKey(value.Job))
            {
                _jobOneKeyWearSuitIdDic[value.Job].Add(data);
            }
            else
            {
                List<OneKeyWearStruct> suitList = new List<OneKeyWearStruct>();
                suitList.Add(data);
                _jobOneKeyWearSuitIdDic.Add(value.Job, suitList);
            }
        }
    }

    private string[] GetSuitNameArrByJobId(int jobId)
    {
        if (_jobOneKeyWearSuitIdDic.Count <= 0)
            InitOneKeyWearData();
        if (!_jobOneKeyWearSuitIdDic.ContainsKey(jobId)) return null;
        var list = _jobOneKeyWearSuitIdDic[jobId];
        string[] arr = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            arr[i] = list[i].Name;
        }

        return arr;
    }

    private int GetSuitIdByIndex(int jobId,int index)
    {
        if (!_jobOneKeyWearSuitIdDic.ContainsKey(jobId)) return -1;
        var list = _jobOneKeyWearSuitIdDic[jobId];
        if (list.Count <= index) return -1;
        return list[index].SuitId;
    }

    private int GetIndexBySuitId(int jobId,int suitId)
    {
        if (!_jobOneKeyWearSuitIdDic.ContainsKey(jobId)) return -1;
        var list = _jobOneKeyWearSuitIdDic[jobId];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].SuitId == suitId)
                return i;
        }

        return 0;
    }

    /// <summary>
    /// 发送套装指令
    /// </summary>
    private void _SendSuitCommand(int suitId,int strength)
    {
        if(suitId<0) return;
        
        var onKeyWearData = TableManager.GetInstance().GetTableItem<OneKeyWearTable>(suitId);
        Utility.UseOneKeySuitWear(onKeyWearData.ID, strength, (int)onKeyWearData.EquipType, (int)onKeyWearData.EnhanceType);

    }

}
