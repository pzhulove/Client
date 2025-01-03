using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class PhaseMaterialPanel : EditorWindow
{
    bool m_MaterialFoldOut = false;
    bool m_AddMatFoldOut = false;

    DPhaseEffectData m_PhaseMaterialData = null;
    string m_PhaseMaterialAssetRes = "Assets/Resources/Animat/PhaseEffData.asset";

    List<string> m_PhaseMatNameList = new List<string>();
    GUIStyle m_GroupStyle = new GUIStyle("GroupBox");
   GUIStyle m_FoldStyle = new GUIStyle("Foldout");

    float m_TimePos = 0;

    protected string m_CurNewPhaseMatName = "新材质";

    protected string m_ShaderPath = "Assets/Resources/Shader/";
    protected string[] m_ShaderList = new string[0];

    protected class PhaseMatRenderCache
    {
        public PhaseMatRenderCache(Renderer rend, Material[] originMat, Material[] phaseMat)
        {
            m_Renderer = rend;
            m_OriginMaterial = originMat;
            m_PhaseMaterial = phaseMat;
        }

        public Renderer m_Renderer;
        public Material[] m_OriginMaterial;
        public Material[] m_PhaseMaterial;
    }

    protected class PhaseMatObjDesc
    {
        public PhaseMatObjDesc(Shader shader, string phaseMatName)
        {
            m_Shader = shader;
            m_PhaseMatName = phaseMatName;
        }

        public List<PhaseMatRenderCache> m_PhaseRenderList = new List<PhaseMatRenderCache>();
        public Shader m_Shader = null;
        public Material m_Material = null;
        public string m_PhaseMatName = null;
    }

    protected List<PhaseMatObjDesc> m_AnimatList = new List<PhaseMatObjDesc>();

    public struct PhaseMatParamDesc
    {
        public PhaseMatParamDesc(string n, string d, AnimatParamType t)
        {
            name = n;
            descript = d;
            type = t;
        }

        public string name;
        public string descript;
        public AnimatParamType type;
    }

    public struct PhaseEffParamDesc
    {
        public PhaseEffParamDesc(GameObject go,Color color)
        {
            m_Asset = new DAssetObject(go);
            m_Color = color;
        }

        public DAssetObject m_Asset;
        public Color m_Color;
    }

    protected class PhaseMatStageDesc
    {
        public PhaseMatStageDesc()
        { }

        public PhaseMatStageDesc(PhaseMatStageDesc other)
        {
            Shader shader = other.m_Material.shader;
            m_Material = new Material(shader);

            m_UnbindParamList.AddRange(other.m_UnbindParamList);
            m_BindedParamList.AddRange(other.m_BindedParamList);

            m_EffectParamList.AddRange(other.m_EffectParamList);

            m_Glow = other.m_Glow;
            m_GlowColor = other.m_GlowColor;
            m_AddingEffect = other.m_AddingEffect;
            m_AddingEffObj = other.m_AddingEffObj;
            m_AddingEffColor = other.m_AddingEffColor;
            m_AddingEffGlow = other.m_AddingEffGlow;
        }

        public void Init(Shader shader)
        {
            Clear();

            int paramNum = ShaderUtil.GetPropertyCount(shader);
            List<string> shaderParamList = new List<string>();
            for (int i = 0; i < paramNum; ++i)
            {
                m_UnbindParamList.Add(new PhaseMatParamDesc(ShaderUtil.GetPropertyName(shader, i),
                    ShaderUtil.GetPropertyDescription(shader, i),
                    (AnimatParamType)(int)ShaderUtil.GetPropertyType(shader, i)));
            }

            m_Material = new Material(shader);
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

        public void AddEffect(GameObject go,Color col)
        {
            RemoveEffect(go);
            m_EffectParamList.Add(new PhaseEffParamDesc(go, col));
        }

        public void RemoveEffect(GameObject go)
        {
            m_EffectParamList.RemoveAll(
                f =>
                {
                    if (go == f.m_Asset.m_AssetObj)
                        return true;
                    else
                        return false;
                }
                );
        }

        public void Clear()
        {
            m_UnbindParamList.Clear();
            m_BindedParamList.Clear();
            m_EffectParamList.Clear();
            m_Material = null;
        }

        public Material m_Material = null;

        protected List<PhaseMatParamDesc> m_UnbindParamList = new List<PhaseMatParamDesc>();
        protected List<PhaseMatParamDesc> m_BindedParamList = new List<PhaseMatParamDesc>();

        protected List<PhaseEffParamDesc> m_EffectParamList = new List<PhaseEffParamDesc>();
        protected bool m_Glow = false;
        protected Color m_GlowColor = Color.white;


        public bool glow
        {
            set { m_Glow = value; }
            get { return m_Glow; }
        }

        public Color glowColor
        {
            set { m_GlowColor = value; }
            get { return m_GlowColor; }
        }
        public List<PhaseEffParamDesc> effectParamList
        {
            get { return m_EffectParamList; }
        }

        public List<PhaseMatParamDesc> unbindParamList
        {
            get { return m_UnbindParamList; }
        }
        public List<PhaseMatParamDesc> bindedParamList
        {
            get { return m_BindedParamList; }
        }

        public bool m_AddingEffect = false;
        public GameObject m_AddingEffObj = null;
        public Color m_AddingEffColor = Color.white;
        public bool m_AddingEffGlow = false;
    }

    protected class PhaseMatDesc
    {
        public List<PhaseMatStageDesc> m_PhaseMatStageList;
        
        private Shader m_CurShader;
        public int m_ShaderIndex;
        public string m_Name;

        public PhaseMatDesc()
        {
            m_PhaseMatStageList = new List<PhaseMatStageDesc>();
            m_CurShader = null;
            m_ShaderIndex = -1;
            m_Name = "";
        }

        public PhaseMatDesc(PhaseMatDesc other)
        {
            if (null == other)
                return;

            for(int i = 0,icnt = other.m_PhaseMatStageList.Count;i<icnt;++i)
            {
                PhaseMatStageDesc curDesc = other.m_PhaseMatStageList[i];
                m_PhaseMatStageList.Add(new PhaseMatStageDesc(curDesc));
            }

            m_CurShader = other.m_CurShader;
            m_ShaderIndex = other.m_ShaderIndex;
            m_Name = other.m_Name;
        }

        public bool PushStage()
        {
            if (null == m_CurShader)
                return false;

            PhaseMatStageDesc newPhaseMatStage = new PhaseMatStageDesc();
            newPhaseMatStage.Init(m_CurShader);
            m_PhaseMatStageList.Add(newPhaseMatStage);

            return true;
        }

        public void PopStage()
        {
            int last = m_PhaseMatStageList.Count - 1;
            PhaseMatStageDesc remove = m_PhaseMatStageList[last];
            remove.Clear();
            m_PhaseMatStageList.RemoveAt(last);
        }

        public int GetStageNum()
        {
            return m_PhaseMatStageList.Count;
        }

        public Shader curShader
        {
            set { m_CurShader = value; }
            get { return m_CurShader; }
        }

        public void Clear()
        {
            m_PhaseMatStageList.RemoveAll(
                s =>
                {
                    s.Clear();
                    return true;
                });
            
            m_CurShader = null;
            m_Name = "";
            m_ShaderIndex = -1;
        }
    }

    protected class PhaseMatPanel
    {
        public PhaseMatPanel(List<string> phaseMatList)
        {
            Init(phaseMatList);
        }

        public void Init(List<string> phaseMatList)
        {
            Clear();
            for (int i = 0; i < phaseMatList.Count; ++i)
            {
                m_UnbindPhaseMatItemList.Add(new PhaseMatItemDesc(
                    phaseMatList[i],
                    new PhaseMatDesc()));
            }
        }

        public PhaseMatDesc BindAnimatEffect(string phaseMatName)
        {
            PhaseMatDesc phaseMat = null;
            m_UnbindPhaseMatItemList.RemoveAll(
                f =>
                {
                    if (phaseMatName == f.name)
                    {
                        phaseMat = f.phaseMat;
                        m_BindedPhaseMatItemList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );
            m_bIsDirty = true;
            return phaseMat;
        }

        public void UnbindAnimatEffect(string phaseMatName)
        {
            m_BindedPhaseMatItemList.RemoveAll(
                f =>
                {
                    if (phaseMatName == f.name)
                    {
                        m_UnbindPhaseMatItemList.Add(f);
                        return true;
                    }
                    else
                        return false;
                }
                );

            m_bIsDirty = true;
        }

        public void RemovePhaseMat(string phaseMatName)
        {
            m_BindedPhaseMatItemList.RemoveAll(
                f =>
                {
                    if (phaseMatName == f.name)
                        return true;
                    else
                        return false;
                }
                );

            m_UnbindPhaseMatItemList.RemoveAll(
                f =>
                {
                    if (phaseMatName == f.name)
                        return true;
                    else
                        return false;
                }
                );

            m_bIsDirty = true;
        }

        public void AddPhaseMat(string phaseMatName)
        {
            m_UnbindPhaseMatItemList.Add(new PhaseMatItemDesc(
                phaseMatName,
                m_BindedPhaseMatItemList.Count > 0 ? m_BindedPhaseMatItemList[0].phaseMat : null));
        }

        public void Clear()
        {
            m_UnbindPhaseMatItemList.RemoveAll(
                f =>
                {
                    f.phaseMat.Clear();
                    return true;
                }
                );
            m_BindedPhaseMatItemList.RemoveAll(
                f =>
                {
                    f.phaseMat.Clear();
                    return true;
                }
                );

            m_UnbindPhaseMatItemList.Clear();
            m_BindedPhaseMatItemList.Clear();
        }

        public bool IsDirty()
        {
            return m_bIsDirty;
        }

        public struct PhaseMatItemDesc
        {
            public PhaseMatItemDesc(string n, PhaseMatDesc e)
            {
                name = n;
                phaseMat = e;
                isRollup = false;
            }

            public string name;
            public PhaseMatDesc phaseMat;
            public bool isRollup;
        }

        public bool m_bIsRollup = true;

        public float m_AbsTime = 0.0f;

        public float m_CurTime = 0.0f;
        public float m_TimeLen = 0.0f;

        public Vector2 scroll = Vector2.zero;

        protected bool m_bIsDirty = false;
        protected List<PhaseMatItemDesc> m_UnbindPhaseMatItemList = new List<PhaseMatItemDesc>();
        protected List<PhaseMatItemDesc> m_BindedPhaseMatItemList = new List<PhaseMatItemDesc>();

        public List<PhaseMatItemDesc> unbindEffectList
        {
            get { return m_UnbindPhaseMatItemList; }
        }
        public List<PhaseMatItemDesc> bindedEffectList
        {
            get { return m_BindedPhaseMatItemList; }
        }
    }

    PhaseMatPanel m_PhaseMatPanel = null;

    [MenuItem("[TM工具集]/ArtTools/强化材质编辑器")]
    public static void Init()
    {
        PhaseMaterialPanel cw = EditorWindow.GetWindow<PhaseMaterialPanel>(false, "强化材质编辑器");

        DPhaseEffectData newModelData = AssetDatabase.LoadAssetAtPath<DPhaseEffectData>(cw.m_PhaseMaterialAssetRes);
        if (null == newModelData)
        {
            newModelData = ScriptableObject.CreateInstance<DPhaseEffectData>();
            AssetDatabase.CreateAsset(newModelData, cw.m_PhaseMaterialAssetRes);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
        }

        if (null != newModelData)
            cw.m_PhaseMaterialData = newModelData;

        string[] pathList = Directory.GetFiles(cw.m_ShaderPath, "*.shader", SearchOption.AllDirectories);
        cw.m_ShaderList = new string[pathList.Length];
        for (int i = 0; i < pathList.Length; ++i)
        {
            cw.m_ShaderList[i] = pathList[i].Replace('\\', '/').Substring(cw.m_ShaderPath.Length);
            cw.m_ShaderList[i] = "Shader/" + cw.m_ShaderList[i].Substring(0, cw.m_ShaderList[i].Length - 7);
            Shader shaderRes = Resources.Load(cw.m_ShaderList[i]) as Shader;
            cw.m_ShaderList[i] = shaderRes.name;
        }

        cw._LoadData(cw.m_PhaseMaterialData);
    }

    void Start()
    {

    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        _FlushData(m_PhaseMaterialData);
    }

    void OnDestroy()
    {
    }

    /// <summary>
    /// GUI消息
    /// </summary>
    void OnGUI()
    {
        if (null == m_PhaseMaterialData)
            return;

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("屁眼子！！！老夫只能帮你到这了，要是在误删就只能怪自己没文化了……", MessageType.Warning);

        EditorGUILayout.Space();

        if (!m_AddMatFoldOut && GUILayout.Button("添加新强化材质"))
            m_AddMatFoldOut = !m_AddMatFoldOut;

        if (m_AddMatFoldOut)
        {
            m_CurNewPhaseMatName = EditorGUILayout.TextField("强化材质效果名：", m_CurNewPhaseMatName);

            bool validName = !string.IsNullOrEmpty(m_CurNewPhaseMatName);
            int PhaseMatNameNum = m_PhaseMatNameList.Count;
            for (int i = 0; i < PhaseMatNameNum; ++i)
            {
                if (m_CurNewPhaseMatName == m_PhaseMatNameList[i])
                    validName = false;
            }

            if (validName)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("确认添加"))
                {
                    m_PhaseMatNameList.Add(m_CurNewPhaseMatName);
                    m_PhaseMatPanel.AddPhaseMat(m_CurNewPhaseMatName);
                    m_AddMatFoldOut = !m_AddMatFoldOut;
                }
                else if (GUILayout.Button("取消"))
                    m_AddMatFoldOut = !m_AddMatFoldOut;
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("强化材质名重复，或者不合法。", MessageType.Warning);
                if (GUILayout.Button("取消"))
                    m_AddMatFoldOut = !m_AddMatFoldOut;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.Separator();

        _PhaseMatEditRollup();
    }

    protected void _PhaseMatEditRollup()
    {
        if (null == m_PhaseMatPanel)
        {
            return;
        }

        ///  材质动画编辑区域
        ///  
        EditorGUILayout.BeginVertical();
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            {
                m_PhaseMatPanel.m_bIsRollup = EditorGUILayout.Foldout(m_PhaseMatPanel.m_bIsRollup, "强化材质表现", m_FoldStyle);
            }
            EditorGUILayout.EndHorizontal();

            if (m_PhaseMatPanel.m_bIsRollup)
            {
                ++ EditorGUI.indentLevel;
                int effectNum = m_PhaseMatPanel.unbindEffectList.Count;
                string[] effectList = new string[effectNum];
                for (int i = 0; i < effectNum; ++i)
                {
                    PhaseMatPanel.PhaseMatItemDesc curEffect = m_PhaseMatPanel.unbindEffectList[i];
                    effectList[i] = curEffect.name;
                }

                int newEffect = -1;
                newEffect = EditorGUILayout.Popup("添加Buff效果:", newEffect, effectList);
                if (-1 != newEffect)
                {
                    m_PhaseMatPanel.BindAnimatEffect(m_PhaseMatPanel.unbindEffectList[newEffect].name);
                }

                EditorGUI.indentLevel = 1;

                m_PhaseMatPanel.scroll = EditorGUILayout.BeginScrollView(m_PhaseMatPanel.scroll);
                int nButtonWidth = 40;
                {
                    string buttonStateDeploy = "展开编辑材质效果：";
                    string buttonStatePackUp = "收起材质效果：";
                    Color buttonColor = Color.white;
                    effectNum = m_PhaseMatPanel.bindedEffectList.Count;
                    for (int i = 0; i < effectNum; ++i)
                    {
                        buttonColor = Color.white;
                        EditorGUILayout.Space();
                        PhaseMatPanel.PhaseMatItemDesc animatEffectDesc = m_PhaseMatPanel.bindedEffectList[i];
                        if (animatEffectDesc.isRollup)
                        {
                            EditorGUI.indentLevel = 1;

                            if (-1 == animatEffectDesc.phaseMat.m_ShaderIndex)
                            {
                                if (null != animatEffectDesc.phaseMat.curShader)
                                {
                                    for (int j = 0; j < m_ShaderList.Length; ++j)
                                    {
                                        if (m_ShaderList[j].Equals(animatEffectDesc.phaseMat.curShader.name))
                                            animatEffectDesc.phaseMat.m_ShaderIndex = j;
                                    }
                                }
                            }

                            EditorGUILayout.Space();
                            {
                                ++EditorGUI.indentLevel;
                                GUILayout.BeginVertical();

                                GUILayout.Label(animatEffectDesc.name + ":");
                                int newShaderIndex = EditorGUILayout.Popup("Shader:", animatEffectDesc.phaseMat.m_ShaderIndex, m_ShaderList);
                                Shader newShadder = null;
                                if (newShaderIndex != animatEffectDesc.phaseMat.m_ShaderIndex)
                                {
                                    newShadder = AssetShaderLoader.Find(m_ShaderList[newShaderIndex]);
                                }

                                if (null != newShadder && newShadder != animatEffectDesc.phaseMat.curShader)
                                {
                                    animatEffectDesc.phaseMat.Clear();
                                    animatEffectDesc.phaseMat.m_ShaderIndex = newShaderIndex;
                                    animatEffectDesc.phaseMat.curShader = newShadder;
                                }

                                EditorGUILayout.Space();

                                int stageNum = animatEffectDesc.phaseMat.GetStageNum();
                                for (int j = 0; j < stageNum; ++ j)
                                {
                                    GUILayout.Label("阶段" + (j+1).ToString() + ":");
                                    _DrawPhaseMatStageRollup(animatEffectDesc.phaseMat.curShader, animatEffectDesc.phaseMat.m_PhaseMatStageList[j], EditorGUI.indentLevel);
                                }

                                GUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                if (animatEffectDesc.phaseMat.GetStageNum() < 20 && GUILayout.Button("添加阶段"))
                                {
                                    animatEffectDesc.phaseMat.PushStage();
                                }
                                if (animatEffectDesc.phaseMat.GetStageNum() > 0 && GUILayout.Button("删除阶段"))
                                {
                                    animatEffectDesc.phaseMat.PopStage();
                                }
                                EditorGUILayout.Space();
                                GUILayout.EndHorizontal();

                                EditorGUILayout.Space();

                                GUILayout.EndVertical();
                                --EditorGUI.indentLevel;
                            }
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

                        if (!animatEffectDesc.isRollup && GUILayout.Button("删除", GUILayout.Width(nButtonWidth)))
                        {
                            GUI.color = buttonColor;
                            m_PhaseMatPanel.UnbindAnimatEffect(m_PhaseMatPanel.bindedEffectList[i].name);
                            GUI.color = Color.white;
                        }
                        else
                            m_PhaseMatPanel.bindedEffectList[i] = animatEffectDesc;

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.EndScrollView();
                }
                --EditorGUI.indentLevel;
            }
        }
        EditorGUILayout.EndVertical();
    }

    protected void _DrawPhaseMatStageRollup(Shader shader,PhaseMatStageDesc phaseMatStageDesc, int indentLevel)
    {
        EditorGUILayout.BeginVertical();

        if (null != shader /*&& null != modelParts*/)
        {
            int paramNum = phaseMatStageDesc.unbindParamList.Count;
            string[] itemList = new string[paramNum];
            for (int i = 0; i < paramNum; ++i)
            {
                PhaseMatParamDesc curParam = phaseMatStageDesc.unbindParamList[i];
                itemList[i] = curParam.type.ToString() + ":" + curParam.name;
            }
        
            _DrawPhaseMatParam(shader, phaseMatStageDesc, EditorGUI.indentLevel);
        
            int newParam = -1;
            newParam = EditorGUILayout.Popup("添加材质参数:", newParam, itemList);
            if (-1 != newParam)
            {
                phaseMatStageDesc.BindParam(phaseMatStageDesc.unbindParamList[newParam].name);
            }

            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                phaseMatStageDesc.glow = EditorGUILayout.Toggle("是否辉光：", phaseMatStageDesc.glow);
                phaseMatStageDesc.glowColor = EditorGUILayout.ColorField("辉光颜色：", phaseMatStageDesc.glowColor);

                if (0 < phaseMatStageDesc.effectParamList.Count)
                {
                    for (int i = 0, icnt = phaseMatStageDesc.effectParamList.Count; i < icnt;++i)
                    {
                        PhaseEffParamDesc curPhaseEffParamDesc = phaseMatStageDesc.effectParamList[i];
                        EditorGUILayout.BeginHorizontal();
                        //EditorGUILayout.LabelField(curPhaseEffParamDesc.m_Asset.m_AssetObj.name);
                        GameObject go = EditorGUILayout.ObjectField(curPhaseEffParamDesc.m_Asset.m_AssetObj, typeof(GameObject)) as GameObject;
                        if(null != go)
                        {
                            curPhaseEffParamDesc.m_Asset.Set(go);
                        }
                        curPhaseEffParamDesc.m_Color = EditorGUILayout.ColorField("特效颜色：", phaseMatStageDesc.effectParamList[i].m_Color);

                        if (GUILayout.Button("删除", GUILayout.Width(40)))
                            phaseMatStageDesc.RemoveEffect(go);
                        else
                            phaseMatStageDesc.effectParamList[i] = curPhaseEffParamDesc;

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.Space();

                if (!phaseMatStageDesc.m_AddingEffect && GUILayout.Button("添加特效对象"))
                    phaseMatStageDesc.m_AddingEffect = !phaseMatStageDesc.m_AddingEffect;

                if (phaseMatStageDesc.m_AddingEffect)
                {

                    phaseMatStageDesc.m_AddingEffObj = EditorGUILayout.ObjectField("特效对象：", phaseMatStageDesc.m_AddingEffObj,typeof(GameObject)) as GameObject;
                    phaseMatStageDesc.m_AddingEffColor = EditorGUILayout.ColorField("特效颜色：", phaseMatStageDesc.m_AddingEffColor);

                    bool valid = null != phaseMatStageDesc.m_AddingEffObj;
                    int PhaseEffectNum = phaseMatStageDesc.effectParamList.Count;
                    for (int i = 0; i < PhaseEffectNum; ++i)
                    {
                        if (phaseMatStageDesc.m_AddingEffObj == phaseMatStageDesc.effectParamList[i].m_Asset.m_AssetObj)
                            valid = false;
                    }

                    if (valid)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button("确认添加"))
                        {
                            phaseMatStageDesc.effectParamList.Add(new PhaseEffParamDesc(phaseMatStageDesc.m_AddingEffObj, phaseMatStageDesc.m_AddingEffColor));
                            m_PhaseMatPanel.AddPhaseMat(m_CurNewPhaseMatName);
                            phaseMatStageDesc.m_AddingEffect = !phaseMatStageDesc.m_AddingEffect;
                        }
                        else if (GUILayout.Button("取消"))
                            phaseMatStageDesc.m_AddingEffect = !phaseMatStageDesc.m_AddingEffect;
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("强化特效对象重复，或者不合法。", MessageType.Warning);
                        if (GUILayout.Button("取消"))
                            phaseMatStageDesc.m_AddingEffect = !phaseMatStageDesc.m_AddingEffect;
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel = indentLevel + 1;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();

                if(GUILayout.Button("预览"))
                {

                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.indentLevel = indentLevel;
        }

        EditorGUILayout.EndVertical();
    }

    protected void _DrawPhaseMatParam(Shader phaseMatShader,PhaseMatStageDesc phaseMatStage, int indentLevel)
    {
        int nButtonWidth = 40;
        Material m = phaseMatStage.m_Material;
        Shader s = phaseMatShader;
        for (int i = 0; i < phaseMatStage.bindedParamList.Count; ++i)
        {
            object preVal;
            object val;
            PhaseMatParamDesc curDesc = phaseMatStage.bindedParamList[i];

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

                    bRemove = GUILayout.Button("删除", GUILayout.Width(nButtonWidth));
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

                    bRemove = GUILayout.Button("删除", GUILayout.Width(nButtonWidth));
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

                    bRemove = GUILayout.Button("删除", GUILayout.Width(nButtonWidth));
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

                    bRemove = GUILayout.Button("删除", GUILayout.Width(nButtonWidth));
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

                    bRemove = GUILayout.Button("删除", GUILayout.Width(nButtonWidth));
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            EditorGUILayout.Space();

            if (bRemove)
            {
                phaseMatStage.UnbindParam(curDesc.name);
            }
            else
                phaseMatStage.bindedParamList[i] = curDesc;
        }
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

    protected void _Clear()
    {

    }

    protected void _LoadData(DPhaseEffectData dataAsset)
    {
        if (null == dataAsset)
            return;

        if(null != dataAsset.phaseMatNameList)
        {
            m_PhaseMatNameList.Clear();
            for (int i = 0; i < dataAsset.phaseMatNameList.Length; ++i)
                m_PhaseMatNameList.Add(dataAsset.phaseMatNameList[i]);
        }

        m_PhaseMatPanel = new PhaseMatPanel(m_PhaseMatNameList);

        if (null != dataAsset.phaseMatChunk)
        {
            int phaseMatChkNum = dataAsset.phaseMatChunk.Length;
            for (int i = 0; i < phaseMatChkNum; ++i)
            {
                DPhaseEffChunk curPhaseMatChk = dataAsset.phaseMatChunk[i];

                PhaseMatDesc newPhaseMatDesc = m_PhaseMatPanel.BindAnimatEffect(curPhaseMatChk.name);
                Shader newShader = AssetShaderLoader.Find(curPhaseMatChk.shaderName);

                newPhaseMatDesc.curShader = newShader;
                for (int j = 0; j < curPhaseMatChk.phaseStageChunk.Length; ++j)
                {
                    PhaseMatStageDesc newPhaseMatStageDesc = new PhaseMatStageDesc();
                    DPhaseStageParamChunk curPhaseMatStageChk = curPhaseMatChk.phaseStageChunk[j];

                    newPhaseMatStageDesc.Init(newShader);

                    for (int k = 0; k < curPhaseMatStageChk.effectDesc.Length; ++k)
                    {
                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + curPhaseMatStageChk.effectDesc[k].effectResPath + ".prefab");
                        newPhaseMatStageDesc.AddEffect(go, curPhaseMatStageChk.effectDesc[k].effectColor);
                    }
                    newPhaseMatStageDesc.glow = curPhaseMatStageChk.needGlow;
                    newPhaseMatStageDesc.glowColor = curPhaseMatStageChk.glowColor;

                    for (int k = 0; k < curPhaseMatStageChk.paramDesc.Length; ++k)
                    {
                        newPhaseMatStageDesc.BindParam(curPhaseMatStageChk.paramDesc[k].paramName);

                        switch (curPhaseMatStageChk.paramDesc[k].paramType)
                        {
                            case AnimatParamType.Color:
                                newPhaseMatStageDesc.m_Material.SetColor(curPhaseMatStageChk.paramDesc[k].paramName, curPhaseMatStageChk.paramDesc[k].paramData._color);
                                break;
                            case AnimatParamType.Range:
                                newPhaseMatStageDesc.m_Material.SetFloat(curPhaseMatStageChk.paramDesc[k].paramName, curPhaseMatStageChk.paramDesc[k].paramData._float);
                                break;
                            case AnimatParamType.Float:
                                newPhaseMatStageDesc.m_Material.SetFloat(curPhaseMatStageChk.paramDesc[k].paramName, curPhaseMatStageChk.paramDesc[k].paramData._float);
                                break;
                            case AnimatParamType.TexEnv:
                                Texture tex = AssetLoader.instance.LoadRes(curPhaseMatStageChk.paramDesc[k].paramObj._texAsset.m_AssetPath, typeof(Texture)).obj as Texture;
                                newPhaseMatStageDesc.m_Material.SetTexture(curPhaseMatStageChk.paramDesc[k].paramName, tex);
                                break;
                            case AnimatParamType.Vector:
                                newPhaseMatStageDesc.m_Material.SetVector(curPhaseMatStageChk.paramDesc[k].paramName, curPhaseMatStageChk.paramDesc[k].paramData._vec4);
                                break;
                        }
                    }

                    newPhaseMatDesc.m_PhaseMatStageList.Add(newPhaseMatStageDesc);
                }
            }
        }
    }

    protected void _FlushData(DPhaseEffectData dataAsset)
    {
        if (null == dataAsset)
            return;

        if (null == m_PhaseMatPanel)
            return;

        int phaseMatNameNum = m_PhaseMatNameList.Count;
        dataAsset.phaseMatNameList = new string[phaseMatNameNum];
        for(int i = 0; i < phaseMatNameNum; ++ i)
            dataAsset.phaseMatNameList[i] = m_PhaseMatNameList[i];

        int phaseMatDescNum = m_PhaseMatPanel.bindedEffectList.Count;
        dataAsset.phaseMatChunk = new DPhaseEffChunk[phaseMatDescNum];

        for (int i = 0; i < phaseMatDescNum; ++i)
        {
            PhaseMatPanel.PhaseMatItemDesc curPhaseMatItem = m_PhaseMatPanel.bindedEffectList[i];
            dataAsset.phaseMatChunk[i].name = curPhaseMatItem.name;
            if (null != curPhaseMatItem.phaseMat.curShader)
                dataAsset.phaseMatChunk[i].shaderName = curPhaseMatItem.phaseMat.curShader.name;
            else
                dataAsset.phaseMatChunk[i].shaderName = "";

            dataAsset.phaseMatChunk[i].phaseStageChunk = new DPhaseStageParamChunk[curPhaseMatItem.phaseMat.m_PhaseMatStageList.Count];
            for (int j = 0; j < curPhaseMatItem.phaseMat.m_PhaseMatStageList.Count; ++j)
            {
                PhaseMatStageDesc curPhaseMatStage = curPhaseMatItem.phaseMat.m_PhaseMatStageList[j];

                if (null == dataAsset.phaseMatChunk[i].phaseStageChunk[j].effectDesc || dataAsset.phaseMatChunk[i].phaseStageChunk[j].effectDesc.Length < curPhaseMatStage.effectParamList.Count)
                {
                    dataAsset.phaseMatChunk[i].phaseStageChunk[j].effectDesc = new DPhaseStageEffectChunk[curPhaseMatStage.effectParamList.Count];
                }

                for (int k = 0; k < curPhaseMatStage.effectParamList.Count; ++k)
                {
                    dataAsset.phaseMatChunk[i].phaseStageChunk[j].effectDesc[k].effectResPath = curPhaseMatStage.effectParamList[k].m_Asset.m_AssetPath;
                    dataAsset.phaseMatChunk[i].phaseStageChunk[j].effectDesc[k].effectColor = curPhaseMatStage.effectParamList[k].m_Color;
                }

                dataAsset.phaseMatChunk[i].phaseStageChunk[j].needGlow = curPhaseMatStage.glow;
                dataAsset.phaseMatChunk[i].phaseStageChunk[j].glowColor = curPhaseMatStage.glowColor;
                if (null == dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc || dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc.Length < curPhaseMatStage.bindedParamList.Count)
                {
                    dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc = new DAnimatParamDesc[curPhaseMatStage.bindedParamList.Count];
                }

                for (int k = 0; k < curPhaseMatStage.bindedParamList.Count; ++k)
                {
                    dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc[k].paramType = curPhaseMatStage.bindedParamList[k].type;
                    dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc[k].paramName = curPhaseMatStage.bindedParamList[k].name;

                    switch (curPhaseMatStage.bindedParamList[k].type)
                    {
                        case AnimatParamType.Color:
                            Color _color = curPhaseMatStage.m_Material.GetColor(curPhaseMatStage.bindedParamList[k].name);
                            dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc[k].paramData._color = _color;
                            break;
                        case AnimatParamType.Range:
                            float _range = curPhaseMatStage.m_Material.GetFloat(curPhaseMatStage.bindedParamList[k].name);
                            dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc[k].paramData._float = _range;
                            break;
                        case AnimatParamType.Float:
                            float _float = curPhaseMatStage.m_Material.GetFloat(curPhaseMatStage.bindedParamList[k].name);
                            dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc[k].paramData._float = _float;
                            break;
                        case AnimatParamType.TexEnv:
                            Texture tex = curPhaseMatStage.m_Material.GetTexture(curPhaseMatStage.bindedParamList[k].name);
                            if (null != tex)
                            {
                                string assetPath = AssetDatabase.GetAssetPath(tex.GetInstanceID());
                                dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc[k].paramObj._texAsset = new DAssetObject(assetPath, true);
                            }
                            break;
                        case AnimatParamType.Vector:
                            Vector4 _vec4 = curPhaseMatStage.m_Material.GetVector(curPhaseMatStage.bindedParamList[k].name);
                            dataAsset.phaseMatChunk[i].phaseStageChunk[j].paramDesc[k].paramData._vec4 = _vec4;
                            break;
                    }
                }

            }

        }

        EditorUtility.SetDirty(dataAsset);
        AssetDatabase.SaveAssets();
    }
}


