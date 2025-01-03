using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MatAnimatEditor : EditorWindow
{
    private enum AnimatEffectType
    {
        None,
        Tint,
        TintFlash,
        OutlineFlash
    }

    private enum ShaderPropertyType
    {
        Float,
        Vector,
        Color,
        Texture
    }

    private class ShaderParam
    {
        public string propertyName;
        public ShaderPropertyType type;
        public ShaderParam(string name, ShaderPropertyType type)
        {
            this.propertyName = name;
            this.type = type;
        }
    }

    private readonly string[] m_BuffLabel = new string[]
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
    private readonly ShaderParam[] m_ShaderParams = new ShaderParam[] 
    {
        new ShaderParam( "_AlphaFactor", ShaderPropertyType.Float),

        new ShaderParam( "_FadeBegin", ShaderPropertyType.Float),
        new ShaderParam( "_FadeLen", ShaderPropertyType.Float),
        new ShaderParam( "_FadeOut", ShaderPropertyType.Float),

        new ShaderParam( "_TintColor", ShaderPropertyType.Color),
        new ShaderParam( "_TintFactor", ShaderPropertyType.Float),

        new ShaderParam( "_TintFactorUpper", ShaderPropertyType.Float),
        new ShaderParam( "_TintFactorLower", ShaderPropertyType.Float),
        new ShaderParam( "_TintFreq", ShaderPropertyType.Float),

        new ShaderParam( "_FlashFreq", ShaderPropertyType.Float),
        new ShaderParam( "_OutlineColorBegin", ShaderPropertyType.Color),
        new ShaderParam( "_OutlineColorEnd", ShaderPropertyType.Color),

        new ShaderParam( "_ScaleTimeLen", ShaderPropertyType.Float),
        new ShaderParam( "_ScaleWidth", ShaderPropertyType.Float),
        new ShaderParam( "_BeginScale", ShaderPropertyType.Float),
        new ShaderParam( "_Outline", ShaderPropertyType.Float)
    };
    private readonly Dictionary<string, string> m_DisplayLabels = new Dictionary<string, string>()
    {
        { "_AlphaFactor",   "Alpha factor" },

        { "_FadeBegin",   "Fade Begin Time" },
        { "_FadeLen",     "Fade Time Length" },
        { "_FadeOut",       "Is Fade Out" },

        { "_TintColor",     "Tint Color" },
        { "_TintFactor",    "Tint Degree" },

        { "_TintFactorUpper",   "Tint Degree Upper" },
        { "_TintFactorLower",   "Tint Degree Lower" },
        { "_TintFreq",          "Tint Freq" },

        { "_FlashFreq",         "Flash frequncy" },
        { "_OutlineColorBegin", "Begin Outline Color" },
        { "_OutlineColorEnd",   "End Outline Color" },

        { "_ScaleTimeLen",  "Scale time length" },
        { "_ScaleWidth",    "Scale width" },
        { "_BeginScale",    "Begin Scale" },
        { "_Outline",       "Outline Width" }
    };
    private HashSet<AnimatEffectType>[] m_EffectTypes;

    //*************************Keyword*********************************//
    private const string ALPHABLEND_KEYWORD = "_ALPHABLEND_ON";
    private const string FADE_KEYWORD = "_FADE";
    private const string TINT_KEYWORD = "_TINT";
    private const string TINTFLASH_KEYWORD = "_TINTFLASH";
    private const string OUTLINEFLASH_KEYWORD = "_OUTLINE_FLASH";
    //*************************Keyword*********************************//


    private DMatAnimatConfig m_Config;
    public HashSet<string> m_UnSetupBuff = new HashSet<string>();


    //***************************UI***********************************//
    private string m_NewBuffName;
    private bool[] m_Foldouts;
    private Vector2 m_ScrollPos;
    //***************************UI***********************************//

    [MenuItem("[TM工具集]/ArtTools/材质动画编辑器")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<MatAnimatEditor>("材质动画编辑器");
        window.Show();
    }

    private void OnEnable()
    {
        m_Config = AssetDatabase.LoadAssetAtPath<DMatAnimatConfig>("Assets/Resources/Animat/MaterialAnimatConfig.asset");

        foreach(string buff in m_BuffLabel)
        {
            m_UnSetupBuff.Add(buff);
        }

        m_Foldouts = new bool[m_BuffLabel.Length];
        m_EffectTypes = new HashSet<AnimatEffectType>[m_BuffLabel.Length];
        for(int i = 0;i < m_BuffLabel.Length;++i)
        {
            m_EffectTypes[i] = new HashSet<AnimatEffectType>();
        }

        // 读取配置
        if (m_Config != null)
        {
            for (int i = 0; i < m_Config.m_Datas.Count;++i)
            {
                DMatAnimatData animatData = m_Config.m_Datas[i];

                m_UnSetupBuff.Remove(animatData.m_Name);

                // 使用的Shader是Outline，只有Outline效果
                if(animatData.m_ShaderType == MatAnimatShaderType.General)
                {
                    // 使用的Shader是General，根据Keyword读取每个动画配置的效果,比如有"_FADE"和"_TINT"，说明是_Tint_FadeIn或_Tint_fadeOut效果
                    foreach (string keyword in animatData.m_keywords)
                    {
                        if(keyword == TINT_KEYWORD)
                        {
                            m_EffectTypes[i].Add(AnimatEffectType.Tint);
                        }
                        else if(keyword == TINTFLASH_KEYWORD)
                        {
                            m_EffectTypes[i].Add(AnimatEffectType.TintFlash);
                        }
                        else if(keyword == OUTLINEFLASH_KEYWORD)
                        {
                            m_EffectTypes[i].Add(AnimatEffectType.OutlineFlash);
                        }
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        if(m_Config != null)
        {
            EditorGUILayout.BeginVertical();

            if(GUILayout.Button("保存"))
            {
                _SaveConfig();
            }

            if(GUILayout.Button("添加Buff效果"))
            {
                GenericMenu menu = new GenericMenu();

                HashSet<string> unBindBuffs = new HashSet<string>(m_BuffLabel);
                foreach(DMatAnimatData animatData in m_Config.m_Datas)
                {
                    unBindBuffs.Remove(animatData.m_Name);
                }

                foreach(string unbindBuff in unBindBuffs)
                {
                    menu.AddItem(new GUIContent(unbindBuff), false, 
                        (buffNameParam) => 
                        {
                            string buffName = (string)buffNameParam;
                            m_Config.m_Datas.Add(new DMatAnimatData() { m_Name = buffName });
                        }, 
                        unbindBuff);
                }

                menu.ShowAsContext();
            }

            EditorGUILayout.BeginHorizontal();
            m_NewBuffName = EditorGUILayout.TextField("BuffName:", m_NewBuffName);
            if(GUILayout.Button("添加自定义Buff"))
            {
                if(!string.IsNullOrEmpty(m_NewBuffName))
                {
                    bool isNewBuff = true;
                    foreach(var animatData in m_Config.m_Datas)
                    {
                        if(m_NewBuffName == animatData.m_Name)
                        {
                            isNewBuff = false;
                        }
                    }
                    if(isNewBuff)
                    {
                        m_Config.m_Datas.Add(new DMatAnimatData() { m_Name = m_NewBuffName });
                    }
                }
                    
            }
            EditorGUILayout.EndHorizontal();

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 500;

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            for (int i = 0;i < m_Config.m_Datas.Count;++i)
            {
                DMatAnimatData animatData = m_Config.m_Datas[i];

                EditorGUILayout.BeginHorizontal();
                m_Foldouts[i] = EditorGUILayout.Foldout(m_Foldouts[i], animatData.m_Name);
                if(GUILayout.Button("-"))
                {
                    m_Config.m_Datas.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                if (m_Foldouts[i])
                {
                    EditorGUILayout.BeginVertical("box");

                    // Shader类型
                    animatData.m_ShaderType = (MatAnimatShaderType)EditorGUILayout.EnumPopup("Shader类型（Outline效果使用Outline，其他动画效果使用General）", animatData.m_ShaderType);

                    // 动画效果，Outline的Shader没有其他动画效果
                    AnimatEffectType animatEffectType = AnimatEffectType.None; 
                    if(animatData.m_ShaderType == MatAnimatShaderType.General )
                    {
                        // 选择动画效果
                        if(animatData.m_keywords != null)
                        {
                            if (animatData.m_keywords.Contains(TINT_KEYWORD))
                                animatEffectType = AnimatEffectType.TintFlash;
                            else if (animatData.m_keywords.Contains(TINTFLASH_KEYWORD))
                                animatEffectType = AnimatEffectType.TintFlash;
                            else if (animatData.m_keywords.Contains(OUTLINEFLASH_KEYWORD))
                                animatEffectType = AnimatEffectType.OutlineFlash;
                        }

                        animatEffectType = (AnimatEffectType)EditorGUILayout.EnumPopup("动画效果", animatEffectType);
                    }
                    else if(animatData.m_ShaderType == MatAnimatShaderType.Outline)
                    {
                        animatEffectType = AnimatEffectType.None;
                    }

                    // Fade
                    bool fade = animatData.m_keywords!=null ? animatData.m_keywords.Contains(FADE_KEYWORD) : false;
                    fade = EditorGUILayout.Toggle("Fade", fade);
                    if (fade)
                        animatData.m_IsOpaque = false;

                    // 半透明
                    animatData.m_IsOpaque = EditorGUILayout.Toggle("不透明", animatData.m_IsOpaque);


                    //**********************更新Keyword*******************//
                    if(animatData.m_ShaderType == MatAnimatShaderType.General)
                    {
                        //*************动画效果的keyword***************//
                        _TryRemoveKeyword(animatData.m_keywords, TINT_KEYWORD);
                        _TryRemoveKeyword(animatData.m_keywords, TINTFLASH_KEYWORD);
                        _TryRemoveKeyword(animatData.m_keywords, OUTLINEFLASH_KEYWORD);
                        switch (animatEffectType)
                        {
                            case AnimatEffectType.Tint:
                                _TryAddKeyword(ref animatData.m_keywords, TINT_KEYWORD);
                                break;
                            case AnimatEffectType.TintFlash:
                                _TryAddKeyword(ref animatData.m_keywords, TINTFLASH_KEYWORD);
                                break;
                            case AnimatEffectType.OutlineFlash:
                                _TryAddKeyword(ref animatData.m_keywords, OUTLINEFLASH_KEYWORD);
                                break;
                        }
                        //*************动画效果的keyword***************//


                        //**************Fade的keyword****************//
                        if (fade)
                            _TryAddKeyword(ref animatData.m_keywords, FADE_KEYWORD);
                        else
                            _TryRemoveKeyword(animatData.m_keywords, FADE_KEYWORD);
                        //**************Fade的keyword****************//

                        //*************半透明的keyword****************//
                        if (animatData.m_IsOpaque)
                            _TryRemoveKeyword(animatData.m_keywords, ALPHABLEND_KEYWORD);
                        else
                            _TryAddKeyword(ref animatData.m_keywords, ALPHABLEND_KEYWORD);
                        //*************半透明的keyword****************//
                    }
                    // Outline没有Keyword
                    else if(animatData.m_ShaderType == MatAnimatShaderType.Outline)
                    {
                        animatData.m_keywords = null;
                    }
                    //**********************更新Keyword*******************//


                    // 材质参数
                    if(animatData.m_FloatParams != null)
                    {
                        for(int j = 0; j < animatData.m_FloatParams.Count;++j)
                        {
                            DMatAnimatFloatParam floatParam = animatData.m_FloatParams[j];

                            EditorGUILayout.BeginHorizontal();
                            floatParam.value = EditorGUILayout.FloatField(m_DisplayLabels[floatParam.name], floatParam.value);
                            if(GUILayout.Button("-"))
                            {
                                animatData.m_FloatParams.RemoveAt(j);
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    if(animatData.m_VectorParams != null)
                    {
                        for(int j = 0; j < animatData.m_VectorParams.Count;++j)
                        {
                            DMatAnimatVectorParam vectorParam = animatData.m_VectorParams[j];
                            EditorGUILayout.BeginHorizontal();
                            vectorParam.value = EditorGUILayout.Vector4Field(m_DisplayLabels[vectorParam.name], vectorParam.value);
                            if (GUILayout.Button("-"))
                            {
                                animatData.m_VectorParams.RemoveAt(j);
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    if(animatData.m_ColorParams != null)
                    {
                        for(int j = 0;j < animatData.m_ColorParams.Count;++j)
                        {
                            DMatAnimatColorParam colorParam = animatData.m_ColorParams[j];
                            EditorGUILayout.BeginHorizontal();
                            colorParam.value = EditorGUILayout.ColorField(m_DisplayLabels[colorParam.name], colorParam.value);
                            if (GUILayout.Button("-"))
                            {
                                animatData.m_ColorParams.RemoveAt(j);
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    if(animatData.m_TextureParams != null)
                    {
                        for(int j = 0;j < animatData.m_TextureParams.Count;++j)
                        {
                            DMatAnimatTextureParam texParam = animatData.m_TextureParams[j];
                            EditorGUILayout.BeginHorizontal();
                            texParam.value = EditorGUILayout.TextField(m_DisplayLabels[texParam.name], texParam.value);
                            if (GUILayout.Button("-"))
                            {
                                animatData.m_TextureParams.RemoveAt(j);
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    // 添加参数
                    if(GUILayout.Button("添加材质参数"))
                    {
                        GenericMenu menu = new GenericMenu();

                        foreach (ShaderParam shaderParam in m_ShaderParams)
                        {
                            menu.AddItem(new GUIContent(m_DisplayLabels[shaderParam.propertyName]), false, 
                                (userData) => 
                                {
                                    ShaderParam shaderParamIn = userData as ShaderParam;
                                    switch(shaderParamIn.type)
                                    {
                                        case ShaderPropertyType.Float:
                                            _TryAddFloatShaderParam(ref animatData.m_FloatParams, shaderParamIn.propertyName);
                                            break;
                                        case ShaderPropertyType.Vector:
                                            _TryAddVectorShaderParam(ref animatData.m_VectorParams, shaderParamIn.propertyName);
                                            break;
                                        case ShaderPropertyType.Color:
                                            _TryAddColorShaderParam(ref animatData.m_ColorParams, shaderParamIn.propertyName);
                                            break;
                                        case ShaderPropertyType.Texture:
                                            _TryAddTextureShaderParam(ref animatData.m_TextureParams, shaderParamIn.propertyName);
                                            break;
                                    }
                                }, 
                                shaderParam);
                        }

                        menu.ShowAsContext();
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUIUtility.labelWidth = oldLabelWidth;

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField("找不到材质动画文件！！！");
        }
    }

    private void _SaveConfig()
    {
        if(m_Config != null)
        {
            EditorUtility.SetDirty(m_Config);
            AssetDatabase.SaveAssets();
        }
    }

    private void _TryAddKeyword(ref List<string> keywords, string keywordToAdd)
    {
        if (keywords == null)
            keywords = new List<string>();

        if (!keywords.Contains(keywordToAdd))
            keywords.Add(keywordToAdd);
    }

    private void _TryRemoveKeyword(List<string> keywords, string keywordToRemove)
    {
        if (keywords != null && keywords.Contains(keywordToRemove))
            keywords.Remove(keywordToRemove);
    }

    private void _TryAddFloatShaderParam(ref List<DMatAnimatFloatParam> floatParams, string newParamName)
    {
        if(floatParams != null)
        {
            foreach(DMatAnimatFloatParam floatParam in floatParams)
            {
                if (floatParam.name == newParamName)
                    return;
            }
        }

        if (floatParams == null)
            floatParams = new List<DMatAnimatFloatParam>();

        floatParams.Add(new DMatAnimatFloatParam() { name = newParamName, value = 0f });
    }

    private void _TryAddVectorShaderParam(ref List<DMatAnimatVectorParam> vectorParams, string newParamName)
    {
        if (vectorParams != null)
        {
            foreach (DMatAnimatVectorParam vectorParam in vectorParams)
            {
                if (vectorParam.name == newParamName)
                    return;
            }
        }

        if (vectorParams == null)
            vectorParams = new List<DMatAnimatVectorParam>();

        vectorParams.Add(new DMatAnimatVectorParam() { name = newParamName, value = Vector4.zero });
    }

    private void _TryAddColorShaderParam(ref List<DMatAnimatColorParam> colorParams, string newParamName)
    {
        if (colorParams != null)
        {
            foreach (DMatAnimatColorParam colorParam in colorParams)
            {
                if (colorParam.name == newParamName)
                    return;
            }
        }

        if (colorParams == null)
            colorParams = new List<DMatAnimatColorParam>();

        colorParams.Add(new DMatAnimatColorParam() { name = newParamName, value = Color.black });
    }

    private void _TryAddTextureShaderParam(ref List<DMatAnimatTextureParam> texParams, string newParamName)
    {
        if (texParams != null)
        {
            foreach (DMatAnimatTextureParam texParam in texParams)
            {
                if (texParam.name == newParamName)
                    return;
            }
        }

        if (texParams == null)
            texParams = new List<DMatAnimatTextureParam>();

        texParams.Add(new DMatAnimatTextureParam() { name = newParamName });
    }
}
