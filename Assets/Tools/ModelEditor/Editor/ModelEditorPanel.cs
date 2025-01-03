using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.IO;


public class ModelEditorPanel : EditorWindow
{

#if !LOGIC_SERVER
    /// <summary>
    ///  数据变量
    /// </summary>
    readonly static string MODELEDITOR_VER_STR = "(Ver:0.0.1)";
    public static ModelEditorPanel sModelEditorLayout;
    protected DModelData m_ModelDataAsset;

    private string m_AssetPath = "Assets/Resources/";
    private string m_ActorAssetPath = "Assets/Resources/Actor";
    private string m_FragileAssetPath = "Assets/Resources/Scene";

    private Vector2 m_ScrollPos;

    protected string m_strResName;

    public static readonly float GAME_VIEW_SIZE = 6;
    protected readonly float GAME_VIEW_PINCH = 20.0f;
    protected class CameraDesc
    {
        public bool m_WantsGameViewMode = false;
        public bool m_InGameViewMode = false;

        public bool m_OriginOrthoMode = false;
        public Vector3 m_OriginPos = Vector3.zero;
        public Quaternion m_OriginOrient = Quaternion.identity;
        public float m_OriginSize = 1;
        public bool m_OriginToolsHiden = false;
    }

    protected class LightDesc
    {
        public LightDesc()
        {
        }

        public Color m_AmbientColor = new Color(0.15f, 0.18f, 0.15f, 1.0f);
        public Vector3 m_LightDir = new Vector3(1,1,0);
        public Vector3 m_BackupLightDir = new Vector3(1, 1, 0);
        public Color m_BackupAmbientColor = Color.black;
        public GameObject m_MainLightNode = null;
    }

    protected class CommonPanel
    {

        public bool m_InSaving = false;
        public string m_NewFileName = "";
        public Vector3 m_Scale = Vector3.one;
        public CameraDesc m_CamSetting = new CameraDesc();
        public LightDesc m_LightDesc = new LightDesc();
    }

    protected CommonPanel m_CommonPanel = new CommonPanel();

    protected GameObject m_ActorRootNode = null;
    protected GameObject m_GridNode = null;
    protected GridDataProxy m_GridDataProxy = null;
    protected GridAuxGeoDrawer m_GridDrawer = null;

    protected struct ModelPartDesc
    {
        public ModelPartDesc(EModelPartChannel c, GameObject o)
        {
            partChannel = c;
            partObject = o;
            partAsset.m_AssetObj = null;
            partAsset.m_AssetPath = "";
            partName = DModelData.kPartChannelLabel[(int)c];
            isRollup = false;
            shaderIndex = -1;
        }

        public EModelPartChannel partChannel;
        public GameObject partObject;
        public DAssetObject partAsset;
        public string partName;
        public bool isRollup;
        public int shaderIndex;
    }

    protected class PartPanel
    {
        public PartPanel()
        {
            Init();
        }

        public void Init()
        {
            Clear();

            for (int i = 0; i < (int)EModelPartChannel.eMaxChannelNum; ++i)
            {
                m_UnbindPartesList.Add(new ModelPartDesc((EModelPartChannel)i,
                    null));
            }
        }

        public void Clear()
        {
            m_UnbindPartesList.RemoveAll(
                f =>
                {
                    if (null != f.partObject)
                    {
                        f.partObject.transform.SetParent(null);
                        Editor.DestroyImmediate(f.partObject);
                    }
                    return true;
                }
                );
            m_BindedPartesList.RemoveAll(
                f =>
                {
                    if (null != f.partObject)
                    {
                        f.partObject.transform.SetParent(null);
                        Editor.DestroyImmediate(f.partObject);
                    }
                    return true;
                }
                );

            m_UnbindPartesList.Clear();
            m_BindedPartesList.Clear();
        }

        public void BindPartes(EModelPartChannel partChannel, GameObject obj, DAssetObject asset)
        {
            m_UnbindPartesList.RemoveAll(
                f =>
                {
                    if (partChannel == f.partChannel)
                    {
                        if (null != f.partObject)
                        {
                            f.partObject.transform.SetParent(null);
                            Editor.DestroyImmediate(f.partObject);
                        }
                        f.partObject = obj;
                        f.partAsset = asset;
                        m_BindedPartesList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );
        }

        public void UnbindPartes(EModelPartChannel partChannel)
        {
            m_BindedPartesList.RemoveAll(
                f =>
                {
                    if (partChannel == f.partChannel)
                    {
                        m_UnbindPartesList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );
        }

        public bool m_bIsRollup = true;
        public bool m_bSuitMode = false;
        public DAssetObject m_Avatar;

        protected List<ModelPartDesc> m_UnbindPartesList = new List<ModelPartDesc>();
        protected List<ModelPartDesc> m_BindedPartesList = new List<ModelPartDesc>();

        public List<ModelPartDesc> unbindPartesList
        {
            get { return m_UnbindPartesList; }
        }
        public List<ModelPartDesc> bindedPartesList
        {
            get { return m_BindedPartesList; }
        }

    }
    protected PartPanel m_PartesPanel = new PartPanel();

    public struct AnimatParamDesc
    {
        public AnimatParamDesc(string n, string d, AnimatParamType t)
        {
            name = n;
            descript = d;
            type = t;
        }

        public string name;
        public string descript;
        public AnimatParamType type;
    }

    public enum AnimatEffectType
    {

    }

    protected class AnimatEffect
    {
        public void Init(Shader shader)
        {
            List<AnimatParamDesc> lastBinded = new List<AnimatParamDesc>(m_BindedParamList);
            Material lastMaterial = m_Material;
            Shader lastShader = m_CurShader;
            Clear();

            int paramNum = ShaderUtil.GetPropertyCount(shader);
            List<string> shaderParamList = new List<string>();
            for (int i = 0; i < paramNum; ++i)
            {
                m_UnbindParamList.Add(new AnimatParamDesc(ShaderUtil.GetPropertyName(shader, i),
                    ShaderUtil.GetPropertyDescription(shader, i),
                    (AnimatParamType)(int)ShaderUtil.GetPropertyType(shader, i)));
            }

            m_Material = new Material(shader);
            m_CurShader = shader;

            /// 拷贝上次的参数
            for(int i = 0,icnt = lastBinded.Count;i<icnt;++i)
            {
                _CopyShaderProperty(lastBinded[i], lastMaterial, m_Material);
                BindParam(lastBinded[i].name);
            }
        }

        public void BindParam(string param)
        {
            m_UnbindParamList.RemoveAll(
                f =>
                {
                    if (param == f.name)
                    {
                        m_BindedParamList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );
        }

        public void UnbindParam(string param)
        {
            m_BindedParamList.RemoveAll(
                f =>
                {
                    if (param == f.name)
                    {
                        m_UnbindParamList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );
        }

        public void Clear()
        {
            m_UnbindParamList.Clear();
            m_BindedParamList.Clear();
            m_Material = null;
            m_CurShader = null;
            m_Name = "";
            m_ShaderIndex = -1;
        }

        public string m_Name = "";

        public Material m_Material = null;
        public Shader m_CurShader = null;
        public int m_ShaderIndex = -1;

        protected List<AnimatParamDesc> m_UnbindParamList = new List<AnimatParamDesc>();
        protected List<AnimatParamDesc> m_BindedParamList = new List<AnimatParamDesc>();

        public List<AnimatParamDesc> unbindParamList
        {
            get { return m_UnbindParamList; }
        }
        public List<AnimatParamDesc> bindedParamList
        {
            get { return m_BindedParamList; }
        }
        public Shader curShader
        {
            get { return m_CurShader; }
        }
    }

    protected class AnimatPanel
    {
        public readonly string[] BuffLabel = new string[]
        {
            "出血",
            "中毒",
            "感电",
            "灼烧",
            "冰冻",
            "晕眩",
            "束缚",
            "睡眠",
            "石化",
            "失明",
            "减速",
            "混乱",
            "霸体",
            "持续霸体",
            "透明",
            "迅捷",
            "祝福",
            "死亡",
            "死亡1",
            "死亡2",
            "受击",
            "隐身",
            "血之狂暴",
            "选中高亮",
            "自爆",
            "物理反伤",
            "魔法反伤",
            "无敌",
            "阳光普照",
            "无敌1",
            "鞭挞狂暴",
            "觉醒",
            "闪白",
            "渐渐出现",
            "寸拳",
            "强力霸体",
            "隐匿",
        };

        public AnimatPanel()
        {
            Clear();
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < BuffLabel.Length; ++i)
            {
                m_UnbindAnimatEffectList.Add(new AnimatEffectDesc(
                    BuffLabel[i],
                    new AnimatEffect()));
            }
        }

        public AnimatEffect BindAnimatEffect(string effectName)
        {
            AnimatEffect animatEff = null;
            m_UnbindAnimatEffectList.RemoveAll(
                f =>
                {
                    if (effectName == f.name)
                    {
                        animatEff = f.effect;
                        m_BindedAnimatEffectList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );
            m_bIsDirty = true;
            return animatEff;
        }

        public void UnbindAnimatEffect(string effectName)
        {
            m_BindedAnimatEffectList.RemoveAll(
                f =>
                {
                    if (effectName == f.name)
                    {
                        m_UnbindAnimatEffectList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );

            m_bIsDirty = true;
        }

        public void Clear()
        {
            m_UnbindAnimatEffectList.RemoveAll(
                f =>
                {
                    f.effect.Clear();
                    return true;
                }
                );
            m_BindedAnimatEffectList.RemoveAll(
                f =>
                {
                    f.effect.Clear();
                    return true;
                }
                );

            m_UnbindAnimatEffectList.Clear();
            m_BindedAnimatEffectList.Clear();
        }

        public bool IsDirty()
        {
            return m_bIsDirty;
        }

        public struct AnimatEffectDesc
        {
            public AnimatEffectDesc(string n, AnimatEffect e)
            {
                name = n;
                effect = e;
                isRollup = false;
            }

            public string name;
            public AnimatEffect effect;
            public bool isRollup;
        }

        public bool m_bIsRollup = true;

        public float m_AbsTime = 0.0f;

        public float m_CurTime = 0.0f;
        public float m_TimeLen = 0.0f;

        protected bool m_bIsDirty = false;
        protected List<AnimatEffectDesc> m_UnbindAnimatEffectList = new List<AnimatEffectDesc>();
        protected List<AnimatEffectDesc> m_BindedAnimatEffectList = new List<AnimatEffectDesc>();

        public List<AnimatEffectDesc> unbindEffectList
        {
            get { return m_UnbindAnimatEffectList; }
        }
        public List<AnimatEffectDesc> bindedEffectList
        {
            get { return m_BindedAnimatEffectList; }
        }
    }

    protected AnimatPanel m_AnimatPanel = new AnimatPanel();

    protected string m_ShaderPath = "Assets/Resources/Shader/";
    protected string[] m_ShaderList = new string[0];

    protected bool m_EditCommonBuffEffect = false;

    protected class BlockGridPanel
    {
        public bool m_bIsRollup = true;
        public bool m_bEditBlock = false;
        public bool m_bEditBBox = false;
    }
    protected BlockGridPanel m_BlockPanel = new BlockGridPanel();



    ///
    GUIStyle m_GroupStyle;
    GUIStyle m_FoldStyle;

    /// <summary>
    /// 外部接口
    /// </summary>

    /// 初始化函数
    public static void Init(UnityEngine.Object asset,string resName)
    {
        sModelEditorLayout = EditorWindow.GetWindow<ModelEditorPanel>(true, "ModelEditor" + ModelEditorPanel.MODELEDITOR_VER_STR, true);
        sModelEditorLayout.Show();
        sModelEditorLayout.m_ModelDataAsset = asset as DModelData;
        sModelEditorLayout.m_strResName = resName;
        sModelEditorLayout.m_CommonPanel.m_NewFileName = resName;

        sModelEditorLayout._LoadCommonData(sModelEditorLayout.m_ModelDataAsset);
        sModelEditorLayout._LoadPartesData(sModelEditorLayout.m_ModelDataAsset);
        sModelEditorLayout._LoadAnimatData(sModelEditorLayout.m_ModelDataAsset);

        string[] pathList = Directory.GetFiles(sModelEditorLayout.m_ShaderPath, "*.shader", SearchOption.AllDirectories);
        sModelEditorLayout.m_ShaderList = new string[pathList.Length];
        for (int i = 0; i < pathList.Length; ++i)
        {
            sModelEditorLayout.m_ShaderList[i] = pathList[i].Replace('\\', '/').Substring(sModelEditorLayout.m_ShaderPath.Length);
            sModelEditorLayout.m_ShaderList[i] = "Shader/" + sModelEditorLayout.m_ShaderList[i].Substring(0, sModelEditorLayout.m_ShaderList[i].Length - 7);
            Shader shaderRes = Resources.Load(sModelEditorLayout.m_ShaderList[i]) as Shader;
            sModelEditorLayout.m_ShaderList[i] = shaderRes.name;
        }

        sModelEditorLayout.m_EditCommonBuffEffect = false;

        if(null == sModelEditorLayout.m_ActorRootNode)
        {
            sModelEditorLayout.m_ActorRootNode = GameObject.Find("ActorRoot");
            if (null == sModelEditorLayout.m_ActorRootNode)
                sModelEditorLayout.m_ActorRootNode = new GameObject("ActorRoot");
        }

        if (null == sModelEditorLayout.m_GridNode)
        {
            sModelEditorLayout.m_GridNode = GameObject.Find("GridNode");
            if (null == sModelEditorLayout.m_GridNode)
            {
                sModelEditorLayout.m_GridNode = new GameObject("GridNode");
                sModelEditorLayout.m_GridDataProxy = sModelEditorLayout.m_GridNode.AddComponent<GridDataProxy>();
            }
        }

        if (null != sModelEditorLayout.m_GridNode)
        {
            sModelEditorLayout.m_GridDrawer = Editor.CreateInstance<GridAuxGeoDrawer>();
            sModelEditorLayout.m_GridDrawer.Init();

            sModelEditorLayout.m_GridNode.SetActive(false);
        }
        sModelEditorLayout._LoadBlockData(sModelEditorLayout.m_ModelDataAsset);

        sModelEditorLayout.m_CommonPanel.m_LightDesc.m_BackupAmbientColor = RenderSettings.ambientLight;
        sModelEditorLayout.m_CommonPanel.m_LightDesc.m_MainLightNode = GameObject.Find("Environment/Directional light");
        if (null != sModelEditorLayout.m_CommonPanel.m_LightDesc.m_MainLightNode)
            sModelEditorLayout.m_CommonPanel.m_LightDesc.m_BackupLightDir = sModelEditorLayout.m_CommonPanel.m_LightDesc.m_MainLightNode.transform.eulerAngles;
    }

    void Start()
    {

    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        _ExitGameModeView();
        _Clear();

        m_ModelDataAsset = null;
    }

    /// <summary>
    /// GUI消息
    /// </summary>
    void OnGUI()
    {
        m_GroupStyle = new GUIStyle("GroupBox");
        m_FoldStyle = new GUIStyle("Foldout");

        if (m_ModelDataAsset != null)
        {
            this.Repaint();
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            {
                /// 模型部件
                if (!m_EditCommonBuffEffect)
                    _DrawCommonRollup();
                if (!m_EditCommonBuffEffect)
                    _DrawModelPartsRollup();
                _DrawAnimatRollup();
                if (!m_EditCommonBuffEffect)
                    _DrawBlockRollup();
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            _DrawInfoPanel("Please create or select a DModelData file first!");

            EditorGUILayout.Space();

            GUIStyle butStyle = new GUIStyle("Button");
            butStyle.fontSize = 12;
            butStyle.font = m_FoldStyle.font;

            if (GUILayout.Button("打开上次编辑的文件", butStyle))
            {
            }

            if (GUILayout.Button("定位资源路径", butStyle))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(m_ActorAssetPath, typeof(UnityEngine.Object));
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }

            if (GUILayout.Button("编辑共享Buff材质集", butStyle))
            {
                string commonBufEffName = "CommonBuffEffect";
                string commonBuffEffPath = "Assets/Resources/Animat/" + commonBufEffName + ".asset";
                DModelData newModelData = AssetDatabase.LoadAssetAtPath<DModelData>(commonBuffEffPath);
                if (null == newModelData)
                {
                    newModelData = _CreateAsset<DModelData>(commonBuffEffPath) as DModelData;

                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();

                }

                if (null != newModelData)
                {
                    _Clear();
                    ModelEditorPanel.Init(newModelData, commonBufEffName);
                    m_EditCommonBuffEffect = true;
                }
            }

            if (GUILayout.Button("编辑共享Buff材质集（PBR）", butStyle))
            {
                string commonBufEffName = "CommonBuffEffectPBR";
                string commonBuffEffPath = "Assets/Resources/Animat/" + commonBufEffName + ".asset";
                DModelData newModelData = AssetDatabase.LoadAssetAtPath<DModelData>(commonBuffEffPath);
                if (null == newModelData)
                {
                    newModelData = _CreateAsset<DModelData>(commonBuffEffPath) as DModelData;

                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();

                }

                if (null != newModelData)
                {
                    _Clear();
                    ModelEditorPanel.Init(newModelData, commonBufEffName);
                    m_EditCommonBuffEffect = true;
                }
            }


            if (GUILayout.Button("打开Buff材质集", butStyle))
            {
                string filePath = EditorUtility.OpenFilePanel("新建Buff材质集", "Assets/Resources/Animat/", "asset");
                filePath = filePath.Replace(Application.dataPath.Replace("Assets", null), null);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                DModelData newModelData = AssetDatabase.LoadAssetAtPath<DModelData>(filePath);
                if (null != newModelData)
                {
                    _Clear();
                    ModelEditorPanel.Init(newModelData, fileName);
                    m_EditCommonBuffEffect = true;
                }
            }

            if (GUILayout.Button("新建Buff材质集", butStyle))
            {
                string filePath = EditorUtility.SaveFilePanel("新建Buff材质集", "Assets/Resources/Animat/", "UnnamedBuffEffect", "asset");
                filePath = filePath.Replace(Application.dataPath.Replace("Assets", null), null);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                DModelData newModelData = _CreateAsset<DModelData>(filePath) as DModelData;
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();

                if (null != newModelData)
                {
                    _Clear();
                    ModelEditorPanel.Init(newModelData, fileName);
                    m_EditCommonBuffEffect = true;
                }
            }
        }
        //_DrawModelPartsRollup();

        if (GUI.changed && null != m_ModelDataAsset)
            EditorUtility.SetDirty(m_ModelDataAsset);
    }

    void OnSelectionChange()
    {
        //         if (_CheckSelectionChanged())
        //         {
        //         }
        //         else
        {/// 创建新的资源
            string resName = "";
            string selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            //if (selectPath.Contains(m_ActorAssetPath))
            {
                string[] subPath = selectPath.Split('/');

                resName = Path.ChangeExtension(subPath[subPath.Length - 1], null);
                int postfix = resName.LastIndexOf("_ModelData");
                if (0 <= postfix && postfix < resName.Length)
                    resName = resName.Substring(0, postfix);

                UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DModelData), UnityEditor.SelectionMode.Assets);
                if ((subPath[subPath.Length - 1].Contains("Hero_") || subPath[subPath.Length - 1].Contains("Monster_")) || (subPath[subPath.Length - 1].Contains("Player_")))
                {
                    DModelData newModelData = null;
                    if (selection.Length > 0)
                    {
                        EditorGUIUtility.PingObject(selection[0]);
                        newModelData = selection[0] as DModelData;
                    }
                    else
                    {
                        // if ("" == Path.GetExtension(selectPath) && Directory.Exists(selectPath + "/Prefabs"))
                        // {
                        //     string dstDataPath = selectPath + "/" + subPath[subPath.Length - 1] + ".asset";
                        // 
                        //     newModelData = AssetDatabase.LoadAssetAtPath<DModelData>(dstDataPath);
                        //     if (null == newModelData)
                        //     {
                        //         newModelData = _CreateAsset<DModelData>(dstDataPath) as DModelData;
                        // 
                        //         AssetDatabase.SaveAssets();
                        //         EditorUtility.FocusProjectWindow();
                        //     }
                        // }
                        // else
                        // {
                        // 
                        // }

                        if ("" == Path.GetExtension(selectPath) && Directory.Exists(selectPath + "/Prefabs"))
                        {
                            string dstDataPath = selectPath + "/" + subPath[subPath.Length - 1] + ".asset";

                            newModelData = AssetDatabase.LoadAssetAtPath<DModelData>(dstDataPath);
                            if (null == newModelData)
                            {
                                newModelData = _CreateAsset<DModelData>(dstDataPath) as DModelData;

                                AssetDatabase.SaveAssets();
                                EditorUtility.FocusProjectWindow();
                            }
                        }
                        else
                        {

                        }

                    }

                    if (newModelData)
                    {
                        _Clear();
                        ModelEditorPanel.Init(newModelData, resName);
                    }
                }
            }

            if (selectPath.Contains(m_FragileAssetPath) && (selectPath.Contains("LogicObject") || selectPath.Contains("Decorator")))
            {
                if(m_ModelDataAsset)
                {
                    _Clear();
                }

                string[] subPath = selectPath.Split('/');
                UnityEngine.Object[] selection = Selection.objects;

                DModelData newModelData = null;
                if (selection.Length > 0)
                {
                    string ext = Path.GetExtension(subPath[subPath.Length - 1]);
                    if (".asset" == ext)
                    {
                        resName = Path.ChangeExtension(subPath[subPath.Length - 1], null);

                        int postfix = resName.LastIndexOf("_ModelData");
                        if (0 <= postfix && postfix < resName.Length)
                            resName = resName.Substring(0, postfix);

                        EditorGUIUtility.PingObject(selection[0]);
                        newModelData = selection[0] as DModelData;
                    }
                    else if (".prefab" == ext)
                    {
                        string assetFile = Path.ChangeExtension(selectPath, null);
                        assetFile += "_ModelData.asset";
                        resName = Path.ChangeExtension(subPath[subPath.Length - 1], null);

                        newModelData = AssetDatabase.LoadAssetAtPath<DModelData>(assetFile);
                        if (null == newModelData)
                        {
                            newModelData = _CreateAsset<DModelData>(assetFile) as DModelData;
                            AssetDatabase.SaveAssets();
                            EditorUtility.FocusProjectWindow();
                        }
                    }

                    if (newModelData)
                    {
                        ModelEditorPanel.Init(newModelData, resName);
                    }
                }
            }
        }
    }

    void Update()
    {
//         if(m_CommonPanel.m_CamSetting.m_InGameViewMode)
//         {
//             HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
// 
//         }
        //         if (null != m_GridDrawer)
        //             m_GridDrawer.OnUpdate();
    }

    void OnDestroy()
    {
        RenderSettings.ambientLight = m_CommonPanel.m_LightDesc.m_BackupAmbientColor;
        if (null != m_CommonPanel.m_LightDesc.m_MainLightNode)
            m_CommonPanel.m_LightDesc.m_MainLightNode.transform.eulerAngles = sModelEditorLayout.m_CommonPanel.m_LightDesc.m_BackupLightDir;

        if (null != m_GridNode)
        {
            GameObject.DestroyImmediate(m_GridNode);
            m_GridNode = null;
        }

        if (null != m_ActorRootNode)
        {
            GameObject.DestroyImmediate(m_ActorRootNode);
            m_ActorRootNode = null;
        }
    }

    /// <summary>
    /// 私有方法
    /// </summary>

    protected void _InitStyle()
    {
           
    }

    protected void _Clear()
    {
        _FlushCommonData(m_ModelDataAsset);
        _FlushPartesData(m_ModelDataAsset);
        _FlushAnimatData(m_ModelDataAsset);
        _FlushBlockData(m_ModelDataAsset);
        AssetDatabase.SaveAssets();
        m_AnimatPanel.Clear();
        m_PartesPanel.Clear();
        GC.Collect();
    }

    protected bool _CheckSelectionChanged()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DModelData), UnityEditor.SelectionMode.Assets);
        if (selection.Length > 0)
        {
            FocusWindowIfItsOpen<ModelEditorPanel>();
            if (m_ModelDataAsset == (DModelData)selection[0])
                return true;
            else
                return false;
        }
        return true;
    }

    public static T _CreateAsset<T>(string assetPath) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, assetPath);

        return asset;
    }

    void _DrawEnvironmentRollup()
    {

    }

    void _DrawModelPartsRollup()
    {
        ///  套装模型编辑区域
        EditorGUILayout.BeginVertical(m_GroupStyle);
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            {
                m_PartesPanel.m_bIsRollup = EditorGUILayout.Foldout(m_PartesPanel.m_bIsRollup, "角色模型", m_FoldStyle);
            }
            EditorGUILayout.EndHorizontal();

            if (m_PartesPanel.m_bIsRollup)
            {
                EditorGUILayout.BeginVertical();
                {
                    string buttonStateDeploy = "展开套装编辑：";
                    string buttonStatePackUp = "收起套装编辑：";

                    EditorGUI.indentLevel = 0;

                    UnityEngine.Object newAvatarObj = EditorGUILayout.ObjectField("Avatar", m_PartesPanel.m_Avatar.m_AssetObj, typeof(GameObject), true);
                    if(newAvatarObj != m_PartesPanel.m_Avatar.m_AssetObj)
                        m_PartesPanel.m_Avatar.Set(newAvatarObj);

                    m_PartesPanel.m_bSuitMode = EditorGUILayout.Toggle("编辑套装", m_PartesPanel.m_bSuitMode);
                    if (!m_PartesPanel.m_bSuitMode)
                    {
                        int wholePartIdx = -1;
                        for(int i = 0; i < m_PartesPanel.bindedPartesList.Count; ++i)
                        {
                            if (m_PartesPanel.bindedPartesList[i].partChannel == EModelPartChannel.eModelWholeBody)
                                wholePartIdx = i;
                        }
                        if(-1 == wholePartIdx)
                        {
                            m_PartesPanel.BindPartes(EModelPartChannel.eModelWholeBody,null, new DAssetObject());
                            for (int i = 0; i < m_PartesPanel.bindedPartesList.Count; ++i)
                            {
                                if (m_PartesPanel.bindedPartesList[i].partChannel == EModelPartChannel.eModelWholeBody)
                                    wholePartIdx = i;
                            }
                        }

                        ModelPartDesc modelPartDesc = m_PartesPanel.bindedPartesList[wholePartIdx];
                        UnityEngine.Object newPrefab = modelPartDesc.partAsset.m_AssetObj;
                        if (null == newPrefab)
                        {
                            string path = AssetDatabase.GetAssetPath(m_ModelDataAsset);
                            if (path.Contains("LogicObjects") || path.Contains("Decorator"))
                            {
                                int postfix = path.LastIndexOf(m_strResName);
                                if (0 < postfix && postfix < path.Length)
                                    path = path.Substring(0, postfix);

                                path = path + m_strResName /*+ ".prefab"*/;
                                path = path.Substring(m_AssetPath.Length);

                                // UnityEngine.Object newPrefabRes = Resources.Load(path);
                                // GameObject newPrefab = UnityEngine.Object.Instantiate(newPrefabRes) as GameObject;
                                newPrefab = Resources.Load<UnityEngine.Object>(path);
                            }
                            else if(path.Contains("Monster") || path.Contains("NPC"))
                            {
                                int postfix = path.LastIndexOf(m_strResName);
                                if (0 < postfix && postfix < path.Length)
                                    path = path.Substring(0, postfix);

                                path = path + "Prefabs/p_" + m_strResName;
                                path = path.Substring(m_AssetPath.Length);

                                // UnityEngine.Object newPrefabRes = Resources.Load(path);
                                // GameObject newPrefab = UnityEngine.Object.Instantiate(newPrefabRes) as GameObject;
                                newPrefab = Resources.Load<UnityEngine.Object>(path);
                            }
                        }

                        EditorGUI.indentLevel = 1;
                        UnityEngine.Object newPartObj = EditorGUILayout.ObjectField(modelPartDesc.partName, newPrefab, typeof(GameObject), true);
                        if (newPartObj != null && newPartObj != modelPartDesc.partAsset.m_AssetObj)
                        {
                            modelPartDesc.partAsset.Set(newPartObj);

                            if (null != modelPartDesc.partObject)
                            {
                                modelPartDesc.partObject.transform.SetParent(null);
                                Editor.DestroyImmediate(modelPartDesc.partObject);
                                modelPartDesc.partObject = null;
                            }

                            m_ActorRootNode.transform.localScale = Vector3.one;
                            if (modelPartDesc.partAsset.m_AssetObj)
                            { 
                                modelPartDesc.partObject = PrefabUtility.InstantiatePrefab(modelPartDesc.partAsset.m_AssetObj) as GameObject;
                                //modelPartDesc.partObject = modelPartDesc.partAsset.m_AssetObj as GameObject;

                                modelPartDesc.partObject.transform.SetParent(m_ActorRootNode.transform);
                                modelPartDesc.partObject.transform.localPosition = new Vector3(0, 0, 0);
                                if (modelPartDesc.partObject.name.Contains("Hero_") || modelPartDesc.partObject.name.Contains("Monster_"))
                                    modelPartDesc.partObject.transform.Rotate(0, 90, 0);
                                m_ActorRootNode.transform.localScale = m_CommonPanel.m_Scale;
                            }

                            modelPartDesc.isRollup = true;

                            this.Repaint();
                            SceneView.RepaintAll();
                        }

                        modelPartDesc = _DrawModelMaterialRollup(modelPartDesc.partObject, modelPartDesc, EditorGUI.indentLevel);
                        m_PartesPanel.bindedPartesList[wholePartIdx] = modelPartDesc;
                    }
                    else
                    { 
                        int partsNum = m_PartesPanel.unbindPartesList.Count;
                        string[] partesList = new string[partsNum];
                        for (int i = 0; i < partsNum; ++i)
                        {
                            ModelPartDesc curPart = m_PartesPanel.unbindPartesList[i];
                            if(EModelPartChannel.eModelWholeBody == curPart.partChannel)
                                continue;
                            partesList[i] = curPart.partName;
                        }

                        Color buttonColor = Color.white;
                        
                        int nButtonWidth = 20;
                        partsNum = m_PartesPanel.bindedPartesList.Count;
                        for (int i = 0; i < partsNum; ++i)
                        {
                            buttonColor = Color.white;
                            EditorGUILayout.Space();
                            ModelPartDesc modelPartDesc = m_PartesPanel.bindedPartesList[i];
                            if (modelPartDesc.isRollup)
                            {
                                EditorGUI.indentLevel = 1;

                                UnityEngine.Object newPartObj = EditorGUILayout.ObjectField(modelPartDesc.partName, modelPartDesc.partAsset.m_AssetObj, typeof(GameObject), true);
                                if (newPartObj != null && newPartObj != modelPartDesc.partAsset.m_AssetObj)
                                {
                                    modelPartDesc.partAsset.Set(newPartObj);

                                    if (null != modelPartDesc.partObject)
                                    {
                                        modelPartDesc.partObject.transform.SetParent(null);
                                        Editor.DestroyImmediate(modelPartDesc.partObject);
                                        modelPartDesc.partObject = null;
                                    }

                                    m_ActorRootNode.transform.localScale = Vector3.one;
                                    modelPartDesc.partObject = PrefabUtility.InstantiatePrefab(modelPartDesc.partAsset.m_AssetObj) as GameObject;
                                    modelPartDesc.partObject.transform.SetParent(m_ActorRootNode.transform);
                                    modelPartDesc.partObject.transform.localPosition = new Vector3(0, 0, 0);
                                    if (modelPartDesc.partObject.name.Contains("Hero_") || modelPartDesc.partObject.name.Contains("Monster_"))
                                        modelPartDesc.partObject.transform.Rotate(0, 90, 0);
                                    m_ActorRootNode.transform.localScale = m_CommonPanel.m_Scale;
                                    modelPartDesc.isRollup = true;

                                    this.Repaint();
                                    SceneView.RepaintAll();
                                }

                                modelPartDesc = _DrawModelMaterialRollup(modelPartDesc.partObject, modelPartDesc, EditorGUI.indentLevel);
                            }


                            EditorGUILayout.BeginHorizontal();
                            string title = buttonStateDeploy;
                            if (modelPartDesc.isRollup)
                            {
                                title = buttonStatePackUp;
                                buttonColor = Color.white;
                            }

                            GUI.color = buttonColor;
                            if (GUILayout.Button(title + modelPartDesc.partName))
                            {
                                modelPartDesc.isRollup = !modelPartDesc.isRollup;
                            }
                            GUI.color = Color.white;

                            if (!modelPartDesc.isRollup && GUILayout.Button("-", GUILayout.Width(nButtonWidth)))
                            {
                                GUI.color = buttonColor;
                                m_PartesPanel.UnbindPartes(modelPartDesc.partChannel);
                                GUI.color = Color.white;
                            }
                            else
                                m_PartesPanel.bindedPartesList[i] = modelPartDesc;

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.Space();
                        int newPart = -1;
                        newPart = EditorGUILayout.Popup("添加套装部件:", newPart, partesList);
                        if (-1 != newPart)
                        {
                            m_PartesPanel.BindPartes(m_PartesPanel.unbindPartesList[newPart].partChannel, null, m_PartesPanel.unbindPartesList[newPart].partAsset);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

        }

        EditorGUILayout.EndVertical();

    }

    ModelPartDesc _DrawModelMaterialRollup(GameObject character, ModelPartDesc modelPart, int indentLevel)
    {
        if (null == character)
            return modelPart;
        /// 材质编辑面板
        EditorGUILayout.BeginVertical();
        {
            EditorGUI.indentLevel = indentLevel + 1;

            SkinnedMeshRenderer[] mr = character.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int r = 0; r < mr.Length; ++r)
            {
                SkinnedMeshRenderer sr = mr[r];
                Material[] ms = sr.sharedMaterials;
                for (int i = 0; i < ms.Length; i++)
                {
                    Material m = ms[i];
                    if (m == null)
                        continue;

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;

                    Shader s = m.shader;

                    if (-1 == modelPart.shaderIndex)
                    {
                        for (int j = 0; j < m_ShaderList.Length; ++j)
                        {
                            if (m_ShaderList[j].Equals(s.name))
                                modelPart.shaderIndex = j;
                        }
                    }

                    int newShader = EditorGUILayout.Popup("Shader:", modelPart.shaderIndex, m_ShaderList);
                    if (newShader != modelPart.shaderIndex)
                    {
                        s = AssetShaderLoader.Find(m_ShaderList[newShader]);
                        if (null != s)
                        {
                            m.shader = s;
                        }
                        modelPart.shaderIndex = newShader;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("部件名称：" + m.name);
                    EditorGUILayout.Space();

                    for (int j = 0; j < ShaderUtil.GetPropertyCount(s); j++)
                    {
                        string propName = ShaderUtil.GetPropertyName(s, j);
                        string descript = ShaderUtil.GetPropertyDescription(s, j) + ":";
                        object preVal;
                        object val;

                        switch (ShaderUtil.GetPropertyType(s, j))
                        {
                            case ShaderUtil.ShaderPropertyType.Range:
                                EditorGUILayout.BeginHorizontal();
                                float min = ShaderUtil.GetRangeLimits(s, j, 1);
                                float max = ShaderUtil.GetRangeLimits(s, j, 2);
                                preVal = m.GetFloat(propName);
                                val = EditorGUILayout.Slider(descript, (float)preVal, min, max);
                                if (val != preVal)
                                {
                                    Undo.RecordObject(m, m.name);
                                    m.SetFloat(propName, (float)val);
                                }
                                EditorGUILayout.EndHorizontal();
                                break;
                            case ShaderUtil.ShaderPropertyType.Float:
                                preVal = m.GetFloat(propName);
                                val = EditorGUILayout.FloatField(descript, (float)preVal);
                                if (val != preVal)
                                {
                                    Undo.RecordObject(m, m.name);
                                    m.SetFloat(propName, (float)val);
                                }
                                break;
                            case ShaderUtil.ShaderPropertyType.Color:
                                preVal = m.GetColor(propName);
                                val = EditorGUILayout.ColorField(descript, (Color)preVal);
                                if (val != preVal)
                                {
                                    Undo.RecordObject(m, m.name);
                                    m.SetColor(propName, (Color)val);
                                }
                                break;
                            case ShaderUtil.ShaderPropertyType.TexEnv:
                                preVal = m.GetTexture(propName);
                                val = EditorGUILayout.ObjectField(descript, (Texture)preVal, typeof(Texture), false);
                                if (val != preVal)
                                {
                                    Undo.RecordObject(m, m.name);
                                    m.SetTexture(propName, (Texture)val);
                                }
                                break;
                            case ShaderUtil.ShaderPropertyType.Vector:
                                preVal = m.GetVector(propName);
                                val = EditorGUILayout.Vector4Field(descript, (Vector4)preVal);
                                if (val != preVal)
                                {
                                    Undo.RecordObject(m, m.name);
                                    m.SetVector(propName, (Vector4)val);
                                }
                                break;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel = indentLevel;

        return modelPart;
    }

    void _DrawCommonRollup()
    {
        EditorGUILayout.BeginVertical(m_GroupStyle);
        {
            EditorGUI.indentLevel = 0;

            GUILayout.Label("模型文件：" + m_strResName);
            string buttonStateDeploy = "实体另存为";
            string buttonStatePackUp = "保存并打开";
            string BtnText = m_CommonPanel.m_InSaving ? buttonStatePackUp : buttonStateDeploy;

            if (m_CommonPanel.m_InSaving)
            {
                m_CommonPanel.m_NewFileName = GUILayout.TextArea(m_CommonPanel.m_NewFileName);
            }
            else
            {
                if (null != m_CommonPanel.m_NewFileName && m_strResName != m_CommonPanel.m_NewFileName)
                {
                    string assetPath = AssetDatabase.GetAssetPath(m_ModelDataAsset);
                    int nameIdx = assetPath.LastIndexOf(m_strResName);

                    if (0 < nameIdx && nameIdx < assetPath.Length)
                    {
                        assetPath = assetPath.Substring(0, nameIdx);
                        DModelData newModelDataAsset = _CreateAsset<DModelData>(assetPath + m_CommonPanel.m_NewFileName + "_ModelData.asset");

                        _FlushCommonData(newModelDataAsset);
                        _FlushPartesData(newModelDataAsset);
                        _FlushAnimatData(newModelDataAsset);
                        _FlushBlockData(newModelDataAsset);
                        EditorUtility.SetDirty(newModelDataAsset);
                        AssetDatabase.SaveAssets();

                        Init(newModelDataAsset, m_CommonPanel.m_NewFileName);
                    }
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button(BtnText))
            {
                m_CommonPanel.m_InSaving = !m_CommonPanel.m_InSaving;
            }
            buttonStateDeploy = "开启游戏视角";
            buttonStatePackUp = "关闭游戏视角";
            BtnText = m_CommonPanel.m_CamSetting.m_WantsGameViewMode ? buttonStatePackUp : buttonStateDeploy;

            m_CommonPanel.m_Scale = EditorGUILayout.Vector3Field("整体缩放比",m_CommonPanel.m_Scale);
            if(m_CommonPanel.m_Scale != m_ActorRootNode.transform.localScale)
                m_ActorRootNode.transform.localScale = m_CommonPanel.m_Scale;

            EditorGUILayout.Space();
            if (GUILayout.Button(BtnText))
            {
                m_CommonPanel.m_CamSetting.m_WantsGameViewMode = !m_CommonPanel.m_CamSetting.m_WantsGameViewMode;
                if (m_CommonPanel.m_CamSetting.m_WantsGameViewMode)
                {
                    _EnterGameModeView();
                }
                else
                {
                    _ExitGameModeView();
                }
            }

            EditorGUILayout.Space();
            m_CommonPanel.m_LightDesc.m_LightDir = EditorGUILayout.Vector3Field("主光方向:", m_CommonPanel.m_LightDesc.m_LightDir);


            while (m_CommonPanel.m_LightDesc.m_LightDir.x > 360)
                m_CommonPanel.m_LightDesc.m_LightDir.x -= 360;
            while (m_CommonPanel.m_LightDesc.m_LightDir.x < 0)
                m_CommonPanel.m_LightDesc.m_LightDir.x += 360;

            while (m_CommonPanel.m_LightDesc.m_LightDir.y > 360)
                m_CommonPanel.m_LightDesc.m_LightDir.y -= 360;
            while (m_CommonPanel.m_LightDesc.m_LightDir.y < 0)
                m_CommonPanel.m_LightDesc.m_LightDir.y += 360;

            while (m_CommonPanel.m_LightDesc.m_LightDir.z > 360)
                m_CommonPanel.m_LightDesc.m_LightDir.z -= 360;
            while (m_CommonPanel.m_LightDesc.m_LightDir.z < 0)
                m_CommonPanel.m_LightDesc.m_LightDir.z += 360;

            EditorGUILayout.Space();
            m_CommonPanel.m_LightDesc.m_AmbientColor = EditorGUILayout.ColorField("环境光颜色", m_CommonPanel.m_LightDesc.m_AmbientColor);

            RenderSettings.ambientLight = m_CommonPanel.m_LightDesc.m_AmbientColor;
            if(null != m_CommonPanel.m_LightDesc.m_MainLightNode)
                m_CommonPanel.m_LightDesc.m_MainLightNode.transform.eulerAngles = m_CommonPanel.m_LightDesc.m_LightDir;
        }
        EditorGUILayout.EndVertical();
    }

    void _DrawAnimatRollup()
    {
        ///  材质动画编辑区域
        ///  
        EditorGUILayout.BeginVertical(m_GroupStyle);
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            {
                m_AnimatPanel.m_bIsRollup = EditorGUILayout.Foldout(m_AnimatPanel.m_bIsRollup, "材质动画", m_FoldStyle);
            }
            EditorGUILayout.EndHorizontal();

            if (m_AnimatPanel.m_bIsRollup)
            {
                int effectNum = m_AnimatPanel.unbindEffectList.Count;
                string[] effectList = new string[effectNum];
                for (int i = 0; i < effectNum; ++i)
                {
                    AnimatPanel.AnimatEffectDesc curEffect = m_AnimatPanel.unbindEffectList[i];
                    effectList[i] = curEffect.name;
                }

                int newEffect = -1;
                newEffect = EditorGUILayout.Popup("添加Buff效果:", newEffect, effectList);
                if (-1 != newEffect)
                {
                    m_AnimatPanel.BindAnimatEffect(m_AnimatPanel.unbindEffectList[newEffect].name);
                }

                EditorGUI.indentLevel = 1;

                int nButtonWidth = 20;
                {
                    string buttonStateDeploy = "展开编辑材质效果：";
                    string buttonStatePackUp = "收起材质效果：";
                    Color buttonColor = Color.white;
                    effectNum = m_AnimatPanel.bindedEffectList.Count;
                    for (int i = 0; i < effectNum; ++i)
                    {
                        buttonColor = Color.white;
                        EditorGUILayout.Space();
                        AnimatPanel.AnimatEffectDesc animatEffectDesc = m_AnimatPanel.bindedEffectList[i];
                        if (animatEffectDesc.isRollup)
                        {
                            EditorGUI.indentLevel = 1;
                            _DrawAnimatRollup(null, animatEffectDesc.effect, EditorGUI.indentLevel);
                        }

                        EditorGUILayout.BeginHorizontal();

                        string title = buttonStateDeploy;
                        if (animatEffectDesc.isRollup)
                        {
                            title = buttonStatePackUp;
                            buttonColor = Color.white;
                        }

                        GUI.color = buttonColor;
                        if (GUILayout.Button(title + animatEffectDesc.name))
                        {
                            animatEffectDesc.isRollup = !animatEffectDesc.isRollup;
                        }
                        GUI.color = Color.white;

                        if (!animatEffectDesc.isRollup && GUILayout.Button("-", GUILayout.Width(nButtonWidth)))
                        {
                            GUI.color = buttonColor;
                            m_AnimatPanel.UnbindAnimatEffect(m_AnimatPanel.bindedEffectList[i].name);
                            GUI.color = Color.white;
                        }
                        else
                            m_AnimatPanel.bindedEffectList[i] = animatEffectDesc;

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
    }
    void _DrawBlockRollup()
    {
        EditorGUILayout.BeginVertical(m_GroupStyle);
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            {
                m_BlockPanel.m_bIsRollup = EditorGUILayout.Foldout(m_BlockPanel.m_bIsRollup, "动态阻挡信息", m_FoldStyle);
            }
            EditorGUILayout.EndHorizontal();

            if (m_BlockPanel.m_bIsRollup)
            {

                EditorGUILayout.BeginVertical();
                {
                    EditorGUI.indentLevel = 0;
                    Color buttonColor = Color.white;
                    {/// 阻挡格子
                        string buttonStateDeploy = "编辑阻挡信息";
                        string buttonStatePackUp = "完成阻挡编辑";

                        string BtnText = m_BlockPanel.m_bEditBlock ? buttonStatePackUp : buttonStateDeploy;

                        if (null != m_GridNode)
                        {
                            if (m_BlockPanel.m_bEditBlock)
                            {/// 开始编辑 
                             /// 读取数据
                             /// 
                                EditorGUILayout.BeginHorizontal();
                                m_GridDataProxy.m_Width = EditorGUILayout.IntField("Grid Width:", m_GridDataProxy.m_Width);
                                m_GridDataProxy.m_Height = EditorGUILayout.IntField("Grid Height:", m_GridDataProxy.m_Height);
                                EditorGUILayout.EndHorizontal();

                                if (!m_GridNode.activeInHierarchy)
                                {
                                    m_GridNode.SetActive(true);
                                    EditorGUIUtility.PingObject(m_GridNode);
                                }
                                if (Selection.activeGameObject != m_GridNode)
                                {
                                    Selection.activeGameObject = m_GridNode;
                                }
                                m_GridDataProxy.m_DrawBlock = true;
                            }
                            else
                            {/// 完成编辑 存储数据
                                m_GridDataProxy.m_DrawBlock = false;
                            }
                        }

                        EditorGUILayout.Space();
                        if (GUILayout.Button(BtnText))
                        {
                            m_BlockPanel.m_bEditBlock = !m_BlockPanel.m_bEditBlock;
                            if (m_BlockPanel.m_bEditBBox)
                                m_BlockPanel.m_bEditBBox = false;
                        }
                    }

                    {/// PickBox

                        string buttonStateDeploy = "编辑包围盒信息";
                        string buttonStatePackUp = "完成包围盒编辑";

                        string BtnText = m_BlockPanel.m_bEditBBox ? buttonStatePackUp : buttonStateDeploy;

                        if (null != m_GridNode)
                        {
                            if (m_BlockPanel.m_bEditBBox)
                            {/// 开始编辑 
                             /// 读取数据
                             /// 
                                m_GridDataProxy.m_Min = EditorGUILayout.Vector3Field("Min", m_GridDataProxy.m_Min);
                                m_GridDataProxy.m_Max = EditorGUILayout.Vector3Field("Max", m_GridDataProxy.m_Max);

                                if (!m_GridNode.activeInHierarchy)
                                {
                                    m_GridNode.SetActive(true);
                                    EditorGUIUtility.PingObject(m_GridNode);
                                }
                                if (Selection.activeGameObject != m_GridNode)
                                {
                                    Selection.activeGameObject = m_GridNode;
                                }
                                m_GridDataProxy.m_DrawBBox = true;

                            }
                            else
                            {/// 完成编辑 存储数据
                                m_GridDataProxy.m_DrawBBox = false;
                            }
                        }

                        EditorGUILayout.Space();
                        if (GUILayout.Button(BtnText))
                        {
                            m_BlockPanel.m_bEditBBox = !m_BlockPanel.m_bEditBBox;
                            if(m_BlockPanel.m_bEditBlock)
                                m_BlockPanel.m_bEditBlock = false;
                        }
                    }

                    if(!m_BlockPanel.m_bEditBBox && !m_BlockPanel.m_bEditBlock)
                        if (Selection.activeGameObject == m_GridNode)
                            Selection.activeGameObject = null;

                }
                EditorGUILayout.EndVertical();
            }

        }
    }

    protected void _DrawAnimatRollup(Material[] matList, AnimatEffect animat, int indentLevel)
    {
        EditorGUILayout.BeginVertical(m_GroupStyle);

        if (-1 == animat.m_ShaderIndex)
        {
            if (null != animat.curShader)
            {
                for (int j = 0; j < m_ShaderList.Length; ++j)
                {
                    if (m_ShaderList[j].Equals(animat.curShader.name))
                        animat.m_ShaderIndex = j;
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        int newShaderIndex = EditorGUILayout.Popup("Shader:", animat.m_ShaderIndex, m_ShaderList);
        EditorGUILayout.Space();
        Shader newShadder = null;
        if (newShaderIndex != animat.m_ShaderIndex)
        {
            newShadder = AssetShaderLoader.Find(m_ShaderList[newShaderIndex]);
        }

        if (null != newShadder && newShadder != animat.curShader)
        {
            animat.m_ShaderIndex = newShaderIndex;
            animat.Init(newShadder);
        }

        if (null != animat.curShader /*&& null != modelParts*/)
        {

            int paramNum = animat.unbindParamList.Count;
            string[] itemList = new string[paramNum];
            for (int i = 0; i < paramNum; ++i)
            {
                AnimatParamDesc curParam = animat.unbindParamList[i];
                itemList[i] = curParam.type.ToString() + ":" + curParam.name;

            }

            _DrawAnimatParam(animat, EditorGUI.indentLevel);

            int newParam = -1;
            newParam = EditorGUILayout.Popup("添加参数:", newParam, itemList);
            if (-1 != newParam)
            {
                animat.BindParam(animat.unbindParamList[newParam].name);
            }

            EditorGUI.indentLevel = indentLevel + 1;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.indentLevel = indentLevel;
        }

        if (m_AnimatPanel.m_TimeLen < 0.0f)
            m_AnimatPanel.m_TimeLen = 0.0f;
        m_AnimatPanel.m_TimeLen = EditorGUILayout.FloatField("材质动画时长：", m_AnimatPanel.m_TimeLen);
        EditorGUILayout.Space();
        m_AnimatPanel.m_CurTime = GUIControls.UICommon.AnimFrameSlider("Animation Frames", m_AnimatPanel.m_CurTime, EditorGUI.indentLevel, 0, m_AnimatPanel.m_TimeLen);
        if (m_AnimatPanel.m_TimeLen > 0.0f)
            m_AnimatPanel.m_AbsTime = m_AnimatPanel.m_CurTime / m_AnimatPanel.m_TimeLen;

        EditorGUILayout.EndVertical();
    }

    protected void _DrawAnimatParam(AnimatEffect animat, int indentLevel)
    {
        int nButtonWidth = 20;
        Material m = animat.m_Material;
        Shader s = animat.curShader;
        for (int i = 0; i < animat.bindedParamList.Count; ++i)
        {
            object preVal;
            object val;
            AnimatParamDesc curDesc = animat.bindedParamList[i];

            EditorGUILayout.Space();

            bool bRemove = false;
            switch (curDesc.type)
            {
                case AnimatParamType.Range:
                    EditorGUILayout.BeginHorizontal();

                    int propIdx = _GetPropertyIndex(s, curDesc.name);
                    float min = -10;
                    float max = 10;
                    if (-1 != propIdx)
                    {
                        min = ShaderUtil.GetRangeLimits(s, propIdx, 1);
                        max = ShaderUtil.GetRangeLimits(s, propIdx, 2);
                    }

                    preVal = m.GetFloat(curDesc.name);
                    val = EditorGUILayout.Slider(curDesc.descript, (float)preVal, min, max);
                    if (val != preVal)
                    {
                        Undo.RecordObject(m, m.name);
                        m.SetFloat(curDesc.name, (float)val);
                    }

                    bRemove = GUILayout.Button("-", GUILayout.Width(nButtonWidth));
                    EditorGUILayout.EndHorizontal();
                    break;
                case AnimatParamType.Float:
                    preVal = m.GetFloat(curDesc.name);
                    EditorGUILayout.BeginHorizontal();
                    val = EditorGUILayout.FloatField(curDesc.descript, (float)preVal);
                    if (val != preVal)
                    {
                        Undo.RecordObject(m, m.name);
                        m.SetFloat(curDesc.name, (float)val);
                    }

                    bRemove = GUILayout.Button("-", GUILayout.Width(nButtonWidth));
                    EditorGUILayout.EndHorizontal();
                    break;
                case AnimatParamType.Color:
                    preVal = m.GetColor(curDesc.name);
                    EditorGUILayout.BeginHorizontal();
                    val = EditorGUILayout.ColorField(curDesc.descript, (Color)preVal);
                    if (val != preVal)
                    {
                        Undo.RecordObject(m, m.name);
                        m.SetColor(curDesc.name, (Color)val);
                    }

                    bRemove = GUILayout.Button("-", GUILayout.Width(nButtonWidth));
                    EditorGUILayout.EndHorizontal();
                    break;
                case AnimatParamType.TexEnv:
                    preVal = m.GetTexture(curDesc.name);
                    EditorGUILayout.BeginHorizontal();
                    val = EditorGUILayout.ObjectField(curDesc.descript, (Texture)preVal, typeof(Texture), false);
                    if (val != preVal)
                    {
                        Undo.RecordObject(m, m.name);
                        m.SetTexture(curDesc.name, (Texture)val);
                    }

                    bRemove = GUILayout.Button("-", GUILayout.Width(nButtonWidth));
                    EditorGUILayout.EndHorizontal();
                    break;
                case AnimatParamType.Vector:
                    preVal = m.GetVector(curDesc.name);
                    EditorGUILayout.BeginHorizontal();
                    val = EditorGUILayout.Vector4Field(curDesc.descript, (Vector4)preVal);
                    if (val != preVal)
                    {
                        Undo.RecordObject(m, m.name);
                        m.SetVector(curDesc.name, (Vector4)val);
                    }

                    bRemove = GUILayout.Button("-", GUILayout.Width(nButtonWidth));
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            EditorGUILayout.Space();

            if (bRemove)
            {
                animat.UnbindParam(curDesc.name);
            }
            else
                animat.bindedParamList[i] = curDesc;
        }
    }

    protected void _DrawInfoPanel(string info)
    {
        GUIStyle labelStyle = new GUIStyle("CN EntryInfo");
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.red;
        labelStyle.fontSize = 12;

        GUILayout.BeginHorizontal("GroupBox");
        GUILayout.Label(info, labelStyle);
        GUILayout.EndHorizontal();
    }


    int _GetPropertyIndex(Shader s, string propertyName)
    {
        if (null != s)
        {
            int propNum = ShaderUtil.GetPropertyCount(s);
            for (int n = 0; n < propNum; ++n)
                if (ShaderUtil.GetPropertyName(s, n) == propertyName)
                    return n;
        }

        return -1;
    }
    /// 
    /// 
    /// 
    protected void _FlushCommonData(DModelData dataAsset)
    {
        if (null == dataAsset)
            return;

        dataAsset.modelScale = m_CommonPanel.m_Scale;
        dataAsset.previewLightDir = m_CommonPanel.m_LightDesc.m_LightDir;
        dataAsset.previewAmbient = m_CommonPanel.m_LightDesc.m_AmbientColor;

        EditorUtility.SetDirty(dataAsset);
    }

    protected void _LoadCommonData(DModelData dataAsset)
    {
        if (null == dataAsset)
            return;

        m_CommonPanel.m_Scale = dataAsset.modelScale;
        m_CommonPanel.m_LightDesc.m_LightDir = dataAsset.previewLightDir;
        m_CommonPanel.m_LightDesc.m_AmbientColor = dataAsset.previewAmbient;

        m_ActorRootNode.transform.localScale = m_CommonPanel.m_Scale;
        RenderSettings.ambientLight = m_CommonPanel.m_LightDesc.m_AmbientColor;
        if (null != m_CommonPanel.m_LightDesc.m_MainLightNode)
            m_CommonPanel.m_LightDesc.m_MainLightNode.transform.eulerAngles = m_CommonPanel.m_LightDesc.m_LightDir;
    }

    protected void _FlushPartesData(DModelData dataAsset)
    {
        if (null == dataAsset)
            return;

        int partesNum = m_PartesPanel.bindedPartesList.Count;
        dataAsset.partsChunk = new DModelPartChunk[partesNum];
        dataAsset.modelAvatar = m_PartesPanel.m_Avatar;

        for (int i = 0; i < partesNum; ++i)
        {
            ModelPartDesc curPartDesc = m_PartesPanel.bindedPartesList[i];

            dataAsset.partsChunk[i].partAsset = curPartDesc.partAsset;
            dataAsset.partsChunk[i].partChannel = curPartDesc.partChannel;
        }

        EditorUtility.SetDirty(dataAsset);
    }

    protected void _LoadPartesData(DModelData dataAsset)
    {
        if (null == dataAsset)
            return;

        m_PartesPanel.Init();
        m_PartesPanel.m_Avatar = dataAsset.modelAvatar;

        int partesChkNum = dataAsset.partsChunk.Length;
        for (int i = 0; i < partesChkNum; ++i)
        {
            DModelPartChunk curPartesChk = dataAsset.partsChunk[i];
            GameObject partObject = null;
            if (null != curPartesChk.partAsset.m_AssetObj)
            {
                partObject = PrefabUtility.InstantiatePrefab(curPartesChk.partAsset.m_AssetObj) as GameObject;

                m_ActorRootNode.transform.localScale = Vector3.one;
                partObject.transform.SetParent(m_ActorRootNode.transform);
                partObject.transform.localPosition = new Vector3(0, 0, 0);
                if (partObject.name.Contains("Hero_") || partObject.name.Contains("Monster_"))
                    partObject.transform.Rotate(0, 90, 0);
                m_ActorRootNode.transform.localScale = m_CommonPanel.m_Scale;
            }

            if (EModelPartChannel.eModelWholeBody == curPartesChk.partChannel && partesChkNum == 1)
                m_PartesPanel.m_bSuitMode = false;
            else
                m_PartesPanel.m_bSuitMode = true;

            m_PartesPanel.BindPartes(curPartesChk.partChannel, partObject, curPartesChk.partAsset);
        }
    }

    /// 
    protected void _FlushAnimatData(DModelData dataAsset)
    {
#if !LOGIC_SERVER
        if (null == dataAsset)
            return;

        int animatEffNum = m_AnimatPanel.bindedEffectList.Count;
        dataAsset.animatChunk = new DAnimatChunk[animatEffNum];

        for (int i = 0; i < animatEffNum; ++i)
        {
            AnimatPanel.AnimatEffectDesc curAnimatEff = m_AnimatPanel.bindedEffectList[i];
            dataAsset.animatChunk[i].name = curAnimatEff.name;
            if (null != curAnimatEff.effect.curShader)
                dataAsset.animatChunk[i].shaderName = curAnimatEff.effect.curShader.name;
            else
                dataAsset.animatChunk[i].shaderName = "";

            if (null == dataAsset.animatChunk[i].paramDesc || dataAsset.animatChunk[i].paramDesc.Length < curAnimatEff.effect.bindedParamList.Count)
            {
                dataAsset.animatChunk[i].paramDesc = new DAnimatParamDesc[curAnimatEff.effect.bindedParamList.Count];
            }

            for (int j = 0; j < curAnimatEff.effect.bindedParamList.Count; ++j)
            {
                dataAsset.animatChunk[i].paramDesc[j].paramType = curAnimatEff.effect.bindedParamList[j].type;
                dataAsset.animatChunk[i].paramDesc[j].paramName = curAnimatEff.effect.bindedParamList[j].name;


                switch (curAnimatEff.effect.bindedParamList[j].type)
                {
                    case AnimatParamType.Color:
                        Color _color = curAnimatEff.effect.m_Material.GetColor(curAnimatEff.effect.bindedParamList[j].name);
                        dataAsset.animatChunk[i].paramDesc[j].paramData._color = _color;
                        break;
                    case AnimatParamType.Range:
                        float _range = curAnimatEff.effect.m_Material.GetFloat(curAnimatEff.effect.bindedParamList[j].name);
                        dataAsset.animatChunk[i].paramDesc[j].paramData._float = _range;
                        break;
                    case AnimatParamType.Float:
                        float _float = curAnimatEff.effect.m_Material.GetFloat(curAnimatEff.effect.bindedParamList[j].name);
                        dataAsset.animatChunk[i].paramDesc[j].paramData._float = _float;
                        break;
                    case AnimatParamType.TexEnv:
                        Texture tex = curAnimatEff.effect.m_Material.GetTexture(curAnimatEff.effect.bindedParamList[j].name);
                        string assetPath = AssetDatabase.GetAssetPath(tex.GetInstanceID());
                        dataAsset.animatChunk[i].paramDesc[j].paramObj._texAsset = new DAssetObject(assetPath,true);
                        break;
                    case AnimatParamType.Vector:
                        Vector4 _vec4 = curAnimatEff.effect.m_Material.GetVector(curAnimatEff.effect.bindedParamList[j].name);
                        dataAsset.animatChunk[i].paramDesc[j].paramData._vec4 = _vec4;
                        break;
                }
            }

            EditorUtility.SetDirty(dataAsset);
        }
#endif
    }

    static protected void _CopyShaderProperty(AnimatParamDesc desc, Material src,Material dst)
    {
        switch (desc.type)
        {
            case AnimatParamType.Color:
                _CopyColor(desc.name, src, dst);
                break;
            case AnimatParamType.Range:
            case AnimatParamType.Float:
                _CopyFloat(desc.name, src, dst);
                break;
            case AnimatParamType.TexEnv:
                _CopyTexture(desc.name, src, dst);
                break;
            case AnimatParamType.Vector:
                _CopyVector(desc.name, src, dst);
                break;
        }
    }

    static protected void _CopyTexture(string property, Material src, Material dst)
    {
        if (src.HasProperty(property))
            dst.SetTexture(property, src.GetTexture(property));
    }

    static protected void _CopyVector(string property, Material src, Material dst)
    {
        if (src.HasProperty(property))
            dst.SetVector(property, src.GetVector(property));
    }

    static protected void _CopyFloat(string property, Material src, Material dst)
    {
        if (src.HasProperty(property))
            dst.SetFloat(property, src.GetFloat(property));
    }

    static protected void _CopyColor(string property, Material src, Material dst)
    {
        if (src.HasProperty(property))
            dst.SetColor(property, src.GetColor(property));
    }

    protected void _LoadAnimatData(DModelData dataAsset)
    {
        if (null == dataAsset)
            return;

        m_AnimatPanel.Init();

        int animatChkNum = dataAsset.animatChunk.Length;
        for (int i = 0; i < animatChkNum; ++i)
        {
            DAnimatChunk curAnimatChk = dataAsset.animatChunk[i];

            AnimatEffect newAnimatEff = m_AnimatPanel.BindAnimatEffect(curAnimatChk.name);
            Shader newShader = AssetShaderLoader.Find(curAnimatChk.shaderName);
            if (null != newShader)
            {
                newAnimatEff.Init(newShader);
                for (int j = 0; j < curAnimatChk.paramDesc.Length; ++j)
                {
                    newAnimatEff.BindParam(curAnimatChk.paramDesc[j].paramName);

                    switch (curAnimatChk.paramDesc[j].paramType)
                    {
                        case AnimatParamType.Color:
                            newAnimatEff.m_Material.SetColor(curAnimatChk.paramDesc[j].paramName, curAnimatChk.paramDesc[j].paramData._color);
                            break;
                        case AnimatParamType.Range:
                            newAnimatEff.m_Material.SetFloat(curAnimatChk.paramDesc[j].paramName, curAnimatChk.paramDesc[j].paramData._float);
                            break;
                        case AnimatParamType.Float:
                            newAnimatEff.m_Material.SetFloat(curAnimatChk.paramDesc[j].paramName, curAnimatChk.paramDesc[j].paramData._float);
                            break;
                        case AnimatParamType.TexEnv:
                            Texture tex = AssetLoader.instance.LoadRes(curAnimatChk.paramDesc[j].paramObj._texAsset.m_AssetPath,typeof(Texture)).obj as Texture;
                            newAnimatEff.m_Material.SetTexture(curAnimatChk.paramDesc[j].paramName, tex);
                            break;
                        case AnimatParamType.Vector:
                            newAnimatEff.m_Material.SetVector(curAnimatChk.paramDesc[j].paramName, curAnimatChk.paramDesc[j].paramData._vec4);
                            break;
                    }
                }

            }
        }
    }

    protected void _FlushBlockData(DModelData dataAsset)
    {
        if (null == dataAsset || null == m_GridDataProxy)
            return;

        int byteLen = m_GridDataProxy.m_Height * m_GridDataProxy.m_Width;
        dataAsset.blockGridChunk.gridBlockData = new byte[byteLen];
        dataAsset.blockGridChunk.gridWidth = m_GridDataProxy.m_Width;
        dataAsset.blockGridChunk.gridHeight = m_GridDataProxy.m_Height;

        dataAsset.blockGridChunk.boundingBoxMin = m_GridDataProxy.m_Min;
        dataAsset.blockGridChunk.boundingBoxMax = m_GridDataProxy.m_Max;

        byte[] srcData = m_GridDataProxy.GetBlockData();
        for (int i = 0; i < srcData.Length; ++i)
            dataAsset.blockGridChunk.gridBlockData[i] = srcData[i];
    }
    protected void _LoadBlockData(DModelData dataAsset)
    {
        if (null == dataAsset || null == m_GridDataProxy)
            return;

        m_GridDataProxy.m_Width = dataAsset.blockGridChunk.gridWidth;
        m_GridDataProxy.m_Height = dataAsset.blockGridChunk.gridHeight;

        if (0 == m_GridDataProxy.m_Width)
            m_GridDataProxy.m_Width = 4;
        if (0 == m_GridDataProxy.m_Height)
            m_GridDataProxy.m_Height = 4;

        m_GridDataProxy.m_Min = dataAsset.blockGridChunk.boundingBoxMin;
        m_GridDataProxy.m_Max = dataAsset.blockGridChunk.boundingBoxMax;

        m_GridDataProxy.AllocBlockData();
        byte[] srcData = dataAsset.blockGridChunk.gridBlockData;
        if (null != srcData)
        {
            byte[] dstData = m_GridDataProxy.GetBlockData();
            for (int i = 0; i < srcData.Length; ++i)
                dstData[i] = srcData[i];
        }
    }

    protected void _EnterGameModeView()
    {
        if (m_CommonPanel.m_CamSetting.m_InGameViewMode)
            return;

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            UnityEngine.Debug.Log("You can't preview animations while in play mode.");
            return;
        }
        m_CommonPanel.m_CamSetting.m_InGameViewMode = true;

        m_CommonPanel.m_CamSetting.m_OriginOrthoMode = DEditorCamera.GetOrthographic();
        m_CommonPanel.m_CamSetting.m_OriginPos = DEditorCamera.GetPosition();
        m_CommonPanel.m_CamSetting.m_OriginOrient = DEditorCamera.GetRotation();
        m_CommonPanel.m_CamSetting.m_OriginSize = DEditorCamera.GetSize();

        DEditorCamera.SetPosition(Vector3.up);
        if (m_CommonPanel.m_CamSetting.m_InGameViewMode)
        {
            DEditorCamera.SetRotation(Quaternion.Euler(GAME_VIEW_PINCH, 0, 0));
        }
        else
        {
            DEditorCamera.SetRotation(Quaternion.identity);
        }

        m_CommonPanel.m_CamSetting.m_OriginToolsHiden = Tools.hidden;
        Tools.hidden = true;
         
        DEditorCamera.SetOrthographic(true);
        DEditorCamera.SetSize(GAME_VIEW_SIZE);
    }

    protected void _ExitGameModeView()
    {
        if (!m_CommonPanel.m_CamSetting.m_InGameViewMode)
            return;

        DEditorCamera.SetOrthographic( m_CommonPanel.m_CamSetting.m_OriginOrthoMode );
        DEditorCamera.SetPosition(m_CommonPanel.m_CamSetting.m_OriginPos);
        DEditorCamera.SetRotation(m_CommonPanel.m_CamSetting.m_OriginOrient);
        DEditorCamera.SetSize(m_CommonPanel.m_CamSetting.m_OriginSize);

        m_CommonPanel.m_CamSetting.m_InGameViewMode = false;

        Tools.hidden = m_CommonPanel.m_CamSetting.m_OriginToolsHiden;
    }

	public GameObject _GetSceneGroundObj(GameObject sceneRoot)
	{
		if ("Ground" == sceneRoot.tag/* || (sceneRoot.name.Contains("ground") && null != sceneRoot.GetComponents<MeshRenderer>())*/)
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
#endif
}

