using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ModelViewer : EditorWindow
{
#if !LOGIC_SERVER
    bool m_ActorFoldOut = false;

    UnityEngine.Object m_Asset = null;
    GameObject m_Actor = null;
    DModelData m_CommonBuffEff = null;
    string[] m_CommonBufNameTbl = null;
    int m_CurCommonBufIdx = -1;
    float m_TimePos = 0;

    protected class AnimatRenderCache
    {
        public AnimatRenderCache(Renderer rend,Material[] originMat,Material[] animat)
        {
            m_Renderer = rend;
            m_OriginMaterial = originMat;
            m_AnimatMaterial = animat;
        }

        public Renderer m_Renderer;
        public Material[] m_OriginMaterial;
        public Material[] m_AnimatMaterial;
    }

    protected class AnimatDesc
    {
        public AnimatDesc(Shader shader, string animatName)
        {
            m_Shader = shader;
            m_AnimatName = animatName;
        }

        public List<AnimatRenderCache> m_AnimatRenderList = new List<AnimatRenderCache>();
        public Shader m_Shader = null;
        public Material m_Material = null;
        public string m_AnimatName = null;
    }


    protected List<AnimatDesc> m_AnimatList = new List< AnimatDesc>();

    [MenuItem("[TM工具集]/ArtTools/模型查看")]
    public static void Init()
    {
        ModelViewer cw = EditorWindow.GetWindow<ModelViewer>(false, "模型查看器");
        cw.m_CommonBuffEff = AssetDatabase.LoadAssetAtPath<DModelData>("Assets/Resources/Animat/CommonBuffEffect.asset");
        cw._CleanAnimat();
    }

    void Start()
    {

    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    void OnDestroy()
    {
    }

    /// <summary>
    /// GUI消息
    /// </summary>
    void OnGUI()
    {
        m_ActorFoldOut = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), m_ActorFoldOut, "Actor ", true);
        if (m_ActorFoldOut == true)
        {
            ++EditorGUI.indentLevel;

            EditorGUILayout.Space();

            UnityEngine.Object newAsset = null;
            newAsset = EditorGUILayout.ObjectField("角色预览", m_Asset, typeof(GameObject));
            if(newAsset != m_Asset || null == m_Asset)
            {
                _ClearActor();

                if(null != newAsset)
                {
                    m_Actor = GameObject.Instantiate(newAsset as GameObject);

                    _AddAnimat(m_Actor);
                    m_Asset = newAsset;
                }
            }

            EditorGUILayout.BeginHorizontal();
            int newCommonBufIdx  = EditorGUILayout.Popup("当前BUF效果", m_CurCommonBufIdx, m_CommonBufNameTbl);
            if(newCommonBufIdx != m_CurCommonBufIdx)
            {
                _RecoverAnimat();
                m_CurCommonBufIdx = newCommonBufIdx;
                _ApplyAnimat(0.0f);
            }

            if(GUILayout.Button("恢复", GUILayout.Width(40)))
            {
                _RecoverAnimat();
                m_CurCommonBufIdx = -1;
            }
            EditorGUILayout.EndHorizontal();


//             EditorGUILayout.BeginVertical(subGroupStyle);
//             {
//                 EditorGUILayout.Space();
//                 EditorGUI.indentLevel += 1;
// 
//                 GameObject newCharacterPrefab = (GameObject)EditorGUILayout.ObjectField("角色模型:", skillData.characterPrefab, typeof(UnityEngine.GameObject), true);
//                 if (newCharacterPrefab != null && skillData.characterPrefab != newCharacterPrefab && !EditorApplication.isPlayingOrWillChangePlaymode)
//                 {
//                     if (PrefabUtility.GetPrefabType(newCharacterPrefab) != PrefabType.Prefab)
//                     {
//                         characterWarning = true;
//                         errorMsg = "not prefab";
//                     }
//                     else
//                     {
//                         characterWarning = false;
//                         skillData.characterPrefab = newCharacterPrefab;
//                         skillData.characterAsset = new DAssetObject(newCharacterPrefab);
//                         Clear(true, true);
//                         RefreashSkillData();
//                     }
//                 }
//                 else if (skillData.characterPrefab != newCharacterPrefab && EditorApplication.isPlayingOrWillChangePlaymode)
//                 {
//                     characterWarning = true;
//                     errorMsg = "in play mode";
//                 }
//                 else if (newCharacterPrefab == null)
//                 {
//                     skillData.characterPrefab = null;
//                     animClipsName = new string[0];
//                 }
// 
// 
//                 EditorGUILayout.Space();
//                 EditorGUILayout.Space();
//                 EditorGUI.indentLevel += 1;
// 
//                 int newClip = EditorGUILayout.Popup("动作:", animClipsSelected, animClipsName);
//                 if (newClip != animClipsSelected)
//                 {
//                     animClipsSelected = newClip;
//                     skillData.animationName = animClipsName[newClip];
//                     animClip = null;
//                     AnimationState state = animations[skillData.animationName];
//                     if (state)
//                     {
//                         animClip = state.clip;
//                     }
//                 }
// 
// 
//                 EditorGUILayout.LabelField("总帧数:", skillData.totalFrames.ToString());
// 
//                 if (animClip != null)
//                 {
//                     skillData.wrapMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode: ", skillData.wrapMode, enumStyle);
//                     interpolationSpeed = EditorGUILayout.Slider("Iterpolation Speed:", interpolationSpeed, 0, 2);
//                 }
// 
// 
//                 if (character == null)
//                 {
//                     if (StyledButton("Preview"))
//                     {
//                         if (skillData.characterPrefab == null)
//                         {
//                             characterWarning = true;
//                             errorMsg = "Drag a character into 'Character Prefab' first.";
//                         }
//                         else if (EditorApplication.isPlayingOrWillChangePlaymode)
//                         {
//                             characterWarning = true;
//                             errorMsg = "You can't preview animations while in play mode.";
//                         }
//                         else
//                         {
//                             characterWarning = false;
//                             Preview();
//                         }
//                     }
//                 }
//                 else
//                 {
//                     if (smoothPreview)
//                     {
//                         animFrame = GUIControls.UICommon.AnimFrameSlider("Animation Frames", animFrame, EditorGUI.indentLevel, 0, skillData.totalFrames - 1);
//                     }
//                     else
//                     {
//                         animFrame = GUIControls.UICommon.AnimFrameSlider("Animation Frames", (int)animFrame, EditorGUI.indentLevel, 0, skillData.totalFrames - 1);
//                     }
// 
//                     EditorGUILayout.BeginHorizontal();
//                     {
//                         GUILayoutUtility.GetRect(1, 1);
//                         GUILayout.Label(string.Format("{0:0.000} 秒", animFrame * (1.0f / fpsTemp)), GUILayout.Width(60.0f));
//                     }
//                     EditorGUILayout.EndHorizontal();
// 
//                     if (cameraOptions)
//                     {
//                         GUILayout.BeginHorizontal("GroupBox");
//                         GUILayout.Label("You must close 'Camera Preview' first.", "CN EntryError");
//                         GUILayout.EndHorizontal();
//                     }
// 
//                     bool bOld = EditorGUILayout.Toggle("Z轴模型(新)", bNew, toggleStyle);
// 
//                     if (bNew != bOld)
//                     {
//                         bNew = bOld;
// 
//                         if (bNew == false)
//                             character.transform.localRotation
//                                 = Quaternion.Euler(0, 90, 0);
//                         else
//                             character.transform.localRotation
//                                 = Quaternion.Euler(initRotation);
// 
//                         return;
//                     }
// 
//                     bool bMirror = DSkillData.root.transform.localScale.x < 0;
//                     bool bChange = EditorGUILayout.Toggle("镜像", bMirror, toggleStyle);
// 
//                     if (bChange != bMirror)
//                     {
//                         if (bChange)
//                         {
//                             Vector3 scale = DSkillData.root.transform.localScale;
//                             scale.x = -1.0f;
//                             DSkillData.root.transform.localScale = scale;
//                         }
//                         else
//                         {
//                             Vector3 scale = DSkillData.root.transform.localScale;
//                             scale.x = 1.0f;
//                             DSkillData.root.transform.localScale = scale;
//                         }
// 
//                         AnimationSampler();
//                         ApplyEffectMirror(bChange);
//                         return;
//                     }
// 
//                     smoothPreview = EditorGUILayout.Toggle("Smooth Preview", smoothPreview, toggleStyle);
//                     DSkillData.vfliter = (VisiableFliter)EditorGUILayout.EnumMaskField("Visable Fliter:", DSkillData.vfliter);
//                     AnimationSampler();
// 
//                     EditorGUILayout.Space();
// 
//                     EditorGUILayout.BeginHorizontal();
//                     {
//                         if (StyledButton("Editor View"))
//                         {
//                             DEditorCamera.SetPosition(Vector3.up);
//                             DEditorCamera.SetRotation(Quaternion.identity);
//                             DEditorCamera.SetOrthographic(true);
//                             DEditorCamera.SetSize(DSkillDataEditorDrawer.Camera_ViewSize);
//                             bEditorView = true;
//                         }
// 
//                         if (StyledButton("Game View"))
//                         {
//                             DEditorCamera.SetPosition(Vector3.up);
//                             DEditorCamera.SetRotation(Quaternion.Euler(GameViewAngle, 0, 0));
//                             DEditorCamera.SetOrthographic(true);
//                             DEditorCamera.SetSize(DSkillDataEditorDrawer.Camera_ViewSize);
//                             bEditorView = false;
//                         }
// 
// 
//                         if (_Play)
//                         {
//                             if (StyledButton("Pause"))
//                             {
//                                 Play(false);
//                             }
//                         }
//                         else
//                         {
//                             if (StyledButton("Play"))
//                             {
//                                 Play(true);
//                             }
//                         }
// 
// 
//                         if (StyledButton("Close Preview")) Clear(true, true);
// 
//                         if (StyledButton("Save Asset"))
//                         {
//                             AssetDatabase.SaveAssets();
//                             UnityEngine.Debug.LogWarning("Skill " + skillData.name + " Saved!");
//                         }
// 
//                     }
//                     EditorGUILayout.EndHorizontal();
// 
//                     EditorGUILayout.Space();
//                 }
// 
//                 EditorGUI.indentLevel -= 1;
//                 if (characterWarning)
//                 {
//                     GUILayout.BeginHorizontal("GroupBox");
//                     GUILayout.Label(errorMsg, "CN EntryWarn");
//                     GUILayout.EndHorizontal();
//                 }
//                 EditorGUI.indentLevel -= 1;
//             }
//             EditorGUILayout.EndVertical();

            -- EditorGUI.indentLevel;
        }
    }

    protected void _ClearActor()
    {
        if (null != m_Actor)
            GameObject.DestroyImmediate(m_Actor);
    }

    protected void _CleanAnimat()
    {
        _ResetAnimat();
        if (null != m_CommonBuffEff)
        {
            if (m_CommonBuffEff.animatChunk.Length != m_AnimatList.Count)
            {
                for (int t = 0; t < m_AnimatList.Count; ++t)
                {
                    if (null != m_AnimatList[t].m_Material)
                    {
                        DestroyImmediate(m_AnimatList[t].m_Material);
                        m_AnimatList[t].m_Material = null;
                        m_AnimatList[t].m_Shader = null;
                    }
                }

                for (int i = 0; i < m_CommonBuffEff.animatChunk.Length; ++i)
                {
                    Shader curShader = Shader.Find(m_CommonBuffEff.animatChunk[i].shaderName);
                    if (null == curShader) continue;
                    AnimatDesc curDesc = new AnimatDesc(curShader, m_CommonBuffEff.animatChunk[i].name);
                    curDesc.m_Material = new Material(curDesc.m_Shader);

                    DAnimatParamDesc[] curAnimatParam = m_CommonBuffEff.animatChunk[i].paramDesc;

                    for (int j = 0; j < curAnimatParam.Length; ++j)
                    {
                        switch (curAnimatParam[j].paramType)
                        {
                            case AnimatParamType.Color:
                                curDesc.m_Material.SetColor(curAnimatParam[j].paramName, curAnimatParam[j].paramData._color);
                                break;
                            case AnimatParamType.Range:
                                curDesc.m_Material.SetFloat(curAnimatParam[j].paramName, curAnimatParam[j].paramData._float);
                                break;
                            case AnimatParamType.Float:
                                curDesc.m_Material.SetFloat(curAnimatParam[j].paramName, curAnimatParam[j].paramData._float);
                                break;
                            case AnimatParamType.TexEnv:
                                Texture tex = AssetLoader.instance.LoadRes(curAnimatParam[j].paramObj._texAsset.m_AssetPath).obj as Texture;
                                curDesc.m_Material.SetTexture(curAnimatParam[j].paramName, tex);
                                break;
                            case AnimatParamType.Vector:
                                curDesc.m_Material.SetVector(curAnimatParam[j].paramName, curAnimatParam[j].paramData._vec4);
                                break;
                        }
                    }

                    m_AnimatList.Add(curDesc);
                }
            }
        }

        m_CommonBufNameTbl = new string[m_AnimatList.Count];
        for(int i = 0; i < m_AnimatList.Count; ++ i)
            m_CommonBufNameTbl[i] = m_AnimatList[i].m_AnimatName;

        m_CurCommonBufIdx = -1;
    }

    protected void _ResetAnimat()
    {
        for (int t = 0; t < m_AnimatList.Count; ++t)
        { 
            AnimatDesc animatDesc = m_AnimatList[t];
            for (int i = 0; i < animatDesc.m_AnimatRenderList.Count; ++i)
            {
                AnimatRenderCache arc = animatDesc.m_AnimatRenderList[i];
                if(null == arc.m_Renderer)
                    continue;

                for (int j = 0; j < arc.m_AnimatMaterial.Length; ++j)
                {
                    if (null != arc.m_AnimatMaterial[j])
                    {
                        Object.DestroyImmediate(arc.m_AnimatMaterial[j]);
                        arc.m_AnimatMaterial[j] = null;
                    }
                }

                arc.m_Renderer.materials = arc.m_OriginMaterial;

                arc.m_Renderer = null;
                arc.m_OriginMaterial = null;
                arc.m_AnimatMaterial = null;
            }
        }
    }

    protected void _ApplyAnimat(float timeOffset)
    {
        if (0 <= m_CurCommonBufIdx && m_CurCommonBufIdx < m_AnimatList.Count)
        {
            m_TimePos = timeOffset;

            AnimatDesc animatDesc = m_AnimatList[m_CurCommonBufIdx];
            for (int j = 0; j < animatDesc.m_AnimatRenderList.Count; ++j)
            {
                AnimatRenderCache rendCache = animatDesc.m_AnimatRenderList[j];
                if (null == rendCache.m_Renderer) continue;

                rendCache.m_Renderer.materials = rendCache.m_AnimatMaterial;
            }
        }
    }

    protected void _RecoverAnimat()
    {
        if (0 <= m_CurCommonBufIdx && m_CurCommonBufIdx < m_AnimatList.Count)
        {
            AnimatDesc animatDesc = m_AnimatList[m_CurCommonBufIdx];

            for (int j = 0; j < animatDesc.m_AnimatRenderList.Count; ++j)
                animatDesc.m_AnimatRenderList[j].m_Renderer.materials = animatDesc.m_AnimatRenderList[j].m_OriginMaterial;
        }
    }

    protected void _AddAnimat(GameObject go)
    {
        _ResetAnimat();
        if (null != go)
        {
            Renderer[] ar = go.GetComponentsInChildren<Renderer>();

            for (int t = 0; t < m_AnimatList.Count; ++t)
            {
                AnimatDesc animatDesc = m_AnimatList[t];
                for (int j = 0; j < ar.Length; ++j)
                {
                    Material[] aom = ar[j].materials;
                    Material[] am = new Material[aom.Length];

                    for (int k = 0; k < aom.Length; ++k)
                    {
                        am[k] = new Material(animatDesc.m_Material);

                        if (aom[k].HasProperty("_MainTex"))
                            am[k].SetTexture("_MainTex", aom[k].GetTexture("_MainTex"));

                        if (aom[k].HasProperty("_BumpMap"))
                            am[k].SetTexture("_BumpMap", aom[k].GetTexture("_BumpMap"));

                        if (aom[k].HasProperty("_Ramp"))
                            am[k].SetTexture("_Ramp", aom[k].GetTexture("_Ramp"));

                        if (aom[k].HasProperty("_DyeColor") && am[k].HasProperty("_DyeColor"))
                            am[k].SetColor("_DyeColor", aom[k].GetColor("_DyeColor"));
                    }

                    animatDesc.m_AnimatRenderList.Add(new AnimatRenderCache(ar[j], aom, am));
                }
            }
        }
    }
#endif
}

