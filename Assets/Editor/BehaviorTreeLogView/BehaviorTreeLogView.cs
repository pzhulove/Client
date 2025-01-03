using System.Collections.Generic;
using System.Diagnostics;
using BehaviorTreeMechanism;
using EditorUI;
using ProtoTable;
using UnityEditor;
using UnityEngine;

public class BehaviorTreeLogView : EditorWindow
{
    public static BehaviorTreeLogView editorWindow;

    public int m_CurMechanismId;
    public List<BehaviorTreeLogInfo> m_CurSelectLogInfo;
    public BehaviorTreeLogInfo m_CurSelectLogStack;

    public SelectListView selectListView;
    public LogListView logListView;
    public StackListView stackListView;
    public bool m_showContent = false;

    private  SplitterState state3 = new SplitterState(new float[]{0.25f, 0.5f, 0.25f});
    private  SplitterState state2 = new SplitterState(new float[]{0.25f, 0.7f});
    [MenuItem("[TM工具集]/[机制编辑器]/流程日志浏览器 %q", false)]
    public static void ShowWindow()
    {
        editorWindow = EditorWindow.GetWindow<BehaviorTreeLogView>(false, "机制行为树日志", true);
        editorWindow.Show();
    }

    protected void OnDestroy()
    {
        BehaviorTreeLogData.IsOpenProcessDebug = false;
    }

    public void Awake()
    {
        BehaviorTreeLogData.Instance.Init();
        BehaviorTreeLogData.IsOpenProcessDebug = true;
        selectListView = new SelectListView(this);
        logListView = new LogListView(this);
        stackListView = new StackListView(this);
    }

    public void OnEnable()
    {
        BehaviorTreeLogData.Instance.Init();
        BehaviorTreeLogData.IsOpenProcessDebug = true;
        selectListView = new SelectListView(this);
        logListView = new LogListView(this);
        stackListView = new StackListView(this);
    }

    protected void OnDisable()
    {
        BehaviorTreeLogData.IsOpenProcessDebug = false;
    }

    public void OnGUI()
    {
        var state = m_showContent ? state3 : state2;
        EditorGUILayout.BeginHorizontal();
        EditorUI.SplitterGUILayout.BeginHorizontalSplit(ref state, new Rect(0, 0, position.width, position.height), position.height);
        selectListView?.OnGUI(state.realSizes[0]);
        logListView?.OnGUI(state.realSizes[1]);
        if(m_showContent && state.realSizes.Length > 2)
            stackListView?.OnGUI(state.realSizes[2]);
        EditorUI.SplitterGUILayout.EndHorizontalSplit();
        EditorGUILayout.EndHorizontal();
    }
}

public class SelectListView
{
    private Vector2 m_SelectedVec = new Vector2 ();
    protected EditorUI.UIListBox m_ListUI;
    
    private BehaviorTreeLogView m_mgr;
    public SelectListView(BehaviorTreeLogView mgr)
    {
        m_mgr = mgr;
        
        InitListUI();
    }

    private void InitListUI()
    {
        m_ListUI = new UIListBox(m_mgr);
        m_ListUI.SetTitles(new string[] { "ID", "索引", "描述"}, new float[] { 0.2f, 0.2f, 0.6f});
    }

    private void DrawListUI()
    {
        m_ListUI.Clear();
        var m_Data = BehaviorTreeLogData.Instance.GetData();
        foreach (var actor in m_Data)
        {
            var actorItem = m_ListUI.AddItems(string.Format("[{0}]{1}", actor.Value.actor.GetPID(), actor.Value.actor.GetName()));
            actorItem.isexpand = true;
            actorItem.userdata = actor.Key;
            foreach (var mech in actor.Value.info)
            {
                var errorCount = HasErrorCount(mech.Value.info);
                var color = "<color=green>";
                if(errorCount > 0)
                    color = "<color=red>";
                var postColor = "</color>";
                var mechData = TableManager.GetInstance().GetTableItem<MechanismTable>(mech.Key);
                var str = new string[] {mechData.ID.ToString(), mechData.Index.ToString(), mechData.Description};
                if((mech.Value.mechanism != null && mech.Value.mechanism.isRunning) || errorCount > 0)
                    str = new string[] {color + mechData.ID.ToString() + postColor, color + mechData.Index.ToString() + postColor, color + mechData.Description + postColor};
                if (mech.Value.mechanism != null && !mech.Value.mechanism.isRunning)
                    mech.Value.mechanism = null;
                var mechItem = m_ListUI.InsertItems(str, actorItem, (current, insert) => { return true; });
                mechItem.userdata = mech.Value;
                if (mech.Value.info == m_mgr.m_CurSelectLogInfo)
                    m_ListUI.selected = mechItem;
            }
        }
    }

    private int HasErrorCount(List<BehaviorTreeLogInfo> info)
    {
        int ret = 0;
        foreach (var item in info)
        {
            if (item.content.type == BTMLogType.Error)
                ret++;
        }

        return ret;
    }
    public void OnGUI(int width)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(width));
        {
            DrawListUI();
            m_ListUI.OnLayoutGUI(m_mgr.position.height, width);
            if (m_ListUI.selected != null && m_ListUI.selected.userdata != null && m_ListUI.selected.userdata is BTMLogInfo)
            {
                var data = m_ListUI.selected.userdata as BTMLogInfo;
                m_mgr.m_CurSelectLogInfo = data.info;
                m_mgr.m_CurMechanismId = data.mechanismId;
            }
            else
            {
                m_mgr.m_CurSelectLogInfo = null;
                m_mgr.m_CurMechanismId = 0;
            }
        }
        EditorGUILayout.EndVertical();
    }
}

public class LogListView
{
    private BehaviorTreeLogView m_mgr;
    protected EditorUI.UIListBox m_ListUI;
    private List<BehaviorTreeLogInfo> m_Data;

    private bool m_showEvent = true;
    private bool m_showError = true;
    private bool m_showMech = true;

    private int m_eventCount;
    private int m_errorCount;
    private int m_mechCount;
    
    private string m_lastSearchFilter;
    public LogListView(BehaviorTreeLogView mgr)
    {
        m_mgr = mgr;
        m_ListUI = new UIListBox(m_mgr);
        m_ListUI.SetTitles(new string[] { "节点", "来源", "信息"}, new float[] { 0.15f, 0.15f, 0.7f});
    }
    private void DrawListUI(int width)
    {
        if (m_Data == null)
        {
            m_ListUI.OnLayoutGUI(m_mgr.position.height - 18, width);
            return;
        }

        m_eventCount = 0;
        m_errorCount = 0;
        m_mechCount = 0;
        m_ListUI.Clear();
        foreach (var log in m_Data)
        {
            if (log.content.type == BTMLogType.Error)
            {
                m_errorCount++;
                if(!m_showError) 
                    continue;
            }
            
            if (log.content.type == BTMLogType.Event)
            {
                m_eventCount++;
                if(!m_showEvent) 
                    continue;
            }
            
            if (log.content.type == BTMLogType.Mechanism)
            {
                m_mechCount++;
                if(!m_showMech) 
                    continue;
            }

            if (!string.IsNullOrEmpty(m_lastSearchFilter))
            {
                string SearchFilter = m_lastSearchFilter.ToLower();
                if (SearchFilter.StartsWith("t:"))
                {
                    var typeSearch = SearchFilter.Substring(2);
                    if(!log.content.from.ToLower().Contains(typeSearch))
                        continue;
                }
                else if (SearchFilter.StartsWith("n:"))
                {
                    var typeSearch = SearchFilter.Substring(2);
                    if(!log.content.node.GetId().ToString().Contains(typeSearch))
                        continue;
                }
                else if(!log.content.log.ToLower().Contains(SearchFilter))
                    continue;
            }
                
            
            var actorItem = m_ListUI.AddItems(log.content.ToStringArray());
            actorItem.userdata = log;
            if (log == m_mgr.m_CurSelectLogStack)
                m_ListUI.selected = actorItem;
        }
        
        m_ListUI.OnLayoutGUI(m_mgr.position.height, width);
    }

    public void OnGUI(int width)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(width));
        {
            m_Data = m_mgr.m_CurSelectLogInfo;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("清空日志", EditorStyles.toolbarButton))
            {
                m_Data?.Clear();
            }
            m_mgr.m_showContent = GUILayout.Toggle(m_mgr.m_showContent, "详情", EditorStyles.toolbarButton);
            
            if (GUILayout.Button("打开行为树", EditorStyles.toolbarButton))
            {
                if (m_mgr.m_CurMechanismId != 0)
                {
                    var data = TableManager.GetInstance().GetTableItem<MechanismTable>(m_mgr.m_CurMechanismId);
                    string workspace = Application.dataPath + "/../../ExternalTool/BehaviacDesigner";
                    Process p = new Process();
                    p.StartInfo.FileName = workspace + "/behaviac/tools/designer/out/BehaviacDesigner.exe";  //确定程序名
                    p.StartInfo.Arguments = workspace + "/workspace " + "/bt=" + workspace + "/workspace/behaviors/" + data.BTPath + ".xml";
                    p.Start();
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("最多:", GUILayout.MaxWidth(30));
            BehaviorTreeLogData.MaxLogCount = EditorGUILayout.IntField(BehaviorTreeLogData.MaxLogCount, EditorStyles.toolbarTextField, GUILayout.MaxWidth(50));
            this.m_lastSearchFilter = EditorGUILayout.TextField("", m_lastSearchFilter, "ToolbarSeachTextField");
            if (GUILayout.Button("", "ToolbarSeachCancelButton"))
                this.m_lastSearchFilter = string.Empty;
            GUILayout.Space(10);
            m_showMech = GUILayout.Toggle(m_showMech, "机制:" + m_mechCount, EditorStyles.toolbarButton);
            m_showEvent = GUILayout.Toggle(m_showEvent, "事件:" + m_eventCount, EditorStyles.toolbarButton);
            m_showError = GUILayout.Toggle(m_showError, "错误:" + m_errorCount, EditorStyles.toolbarButton);
            EditorGUILayout.EndHorizontal();


            DrawListUI(width);
            if (m_ListUI.selected != null)
                m_mgr.m_CurSelectLogStack = m_ListUI.selected.userdata as BehaviorTreeLogInfo;
            else
                m_mgr.m_CurSelectLogStack = null;
        }
        EditorGUILayout.EndVertical();
    }
}

public class StackListView
{
    private BehaviorTreeLogView m_mgr;
    private BehaviorTreeLogInfo m_Data;
    Vector2 InfoScrollPos;

    public StackListView(BehaviorTreeLogView mgr)
    {
        m_mgr = mgr;
    }
    
    private void DrawListUI()
    {
        if (m_Data == null)
            return;
        
        foreach (var stack in m_Data.frames)
        {
            string filePath = stack.GetFileName();
            if (filePath != null)
            {
                int startIndex = filePath.IndexOf("Assets");
                if(startIndex > 0)
                    filePath = filePath.Substring(startIndex);  
            }
            else
            {
                filePath = "<filename unknown>";
            }
            string content = stack.GetMethod().DeclaringType + "." + stack.GetMethod().Name + "()(" + filePath + ":" + stack.GetFileLineNumber() + ")";
            if(HeroGo.CustomGUIUtility.LinkLabel(content))
            {
            	AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(filePath), stack.GetFileLineNumber());  
            }
        }
    }

    private int m_CurSelectIndex = 0;
    public void OnGUI(int width)
    {
        m_Data = m_mgr.m_CurSelectLogStack;
        EditorGUILayout.BeginVertical( GUILayout.Width(width));
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Toggle(this.m_CurSelectIndex == 0, "机制数据", EditorStyles.toolbarButton))
            m_CurSelectIndex = 0;
        if (GUILayout.Toggle(this.m_CurSelectIndex == 1, "局部变量", EditorStyles.toolbarButton))
            m_CurSelectIndex = 1;
        if (GUILayout.Toggle(this.m_CurSelectIndex == 2, "信息堆栈", EditorStyles.toolbarButton))
            m_CurSelectIndex = 2;
        EditorGUILayout.EndHorizontal();
        InfoScrollPos = EditorGUILayout.BeginScrollView(InfoScrollPos, GUILayout.Width(width));
        if (m_CurSelectIndex == 0)
        {
            if (m_mgr.m_CurMechanismId != 0)
            {
                var data = TableManager.GetInstance().GetTableItem<MechanismTable>(m_mgr.m_CurMechanismId);
                EditorGUILayout.LabelField("ID:" + data.ID);
                EditorGUILayout.LabelField("索引:" + data.Index);
                EditorGUILayout.LabelField("路径:" + data.BTPath);
                EditorGUILayout.LabelField("描述:" + data.Description);
                EditorGUILayout.LabelField("[StringA] " + data.StringDescriptionA + ":" + data.StringValueA[0]);
                EditorGUILayout.LabelField("[ValueA] " + data.DescriptionA + ":" + FlatBufferDisplay(data.ValueA));
                EditorGUILayout.LabelField("[ValueB] " + data.DescriptionB + ":" + FlatBufferDisplay(data.ValueB));
                EditorGUILayout.LabelField("[ValueC] " + data.DescriptionC + ":" + FlatBufferDisplay(data.ValueC));
                EditorGUILayout.LabelField("[ValueD] " + data.DescriptionD + ":" + FlatBufferDisplay(data.ValueD));
                EditorGUILayout.LabelField("[ValueE] " + data.DescriptionE + ":" + FlatBufferDisplay(data.ValueE));
                EditorGUILayout.LabelField("[ValueF] " + data.DescriptionF + ":" + FlatBufferDisplay(data.ValueF));
                EditorGUILayout.LabelField("[ValueG] " + data.DescriptionG + ":" + FlatBufferDisplay(data.ValueG));
                EditorGUILayout.LabelField("[ValueH] " + data.DescriptionH + ":" + FlatBufferDisplay(data.ValueH));
            }
        }
        else if (m_CurSelectIndex == 1)
        {
            if (m_Data != null)
            {
                foreach (var localVar in m_Data.localVars)
                {
                    EditorGUILayout.LabelField(localVar);
                }
            }
        }
        else if (m_CurSelectIndex == 2)
        {
            DrawListUI();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private string FlatBufferDisplay(FlatBufferArray<UnionCell> array)
    {
        string ret = string.Empty;
        for (int i = 0; i < array.Count; i++)
        {
            ret += array[i].fixValue;
            if (i != array.Count - 1)
                ret += "|";
        }
        return ret;
    }
}