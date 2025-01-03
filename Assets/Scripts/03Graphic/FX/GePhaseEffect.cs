using UnityEngine;
using System.Collections.Generic;

public class GePhaseStageEffects
{
    public GameObject m_EffectRoot;
    public ParticleSystem[] m_Particles;
    public Renderer[] m_Renderers;
}

public class GePhaseStageDesc
{
    public GameObject m_PhaseEffectRoot = null;

    public Material m_Material = null;
    public GameObject m_PhaseStageEffectsTemplate = null;
    public bool m_Glow = false;
    public Color m_GlowColor = Color.white;
    // 每个强化等级使用的特效Prefab对象池，避免在城镇中频繁创建和销毁
    private Stack<GePhaseStageEffects> m_PhaseStageEffectsPool = new Stack<GePhaseStageEffects>();

    public GePhaseStageDesc(Material mat, GameObject go,bool glow,Color glowColor, GameObject phaseEffectRoot)
    {
        m_Material = mat;
        m_Glow = glow;
        m_GlowColor = glowColor;
        m_PhaseStageEffectsTemplate = go;
        m_PhaseEffectRoot = phaseEffectRoot;
    }

    public GePhaseStageEffects GetEffects()
    {
        if(m_PhaseStageEffectsPool.Count > 0)
        {
            GePhaseStageEffects phaseStageEffects = m_PhaseStageEffectsPool.Pop();
            phaseStageEffects.m_EffectRoot.SetActive(true);

            return phaseStageEffects;
        }

        return _CreateEffects();
    }

    public void RecycleEffects(GePhaseStageEffects phaseStageEffects)
    {
        if(phaseStageEffects != null && phaseStageEffects.m_EffectRoot != null)
        {
            phaseStageEffects.m_EffectRoot.transform.SetParent(m_PhaseEffectRoot.transform, false);
            phaseStageEffects.m_EffectRoot.SetActive(false);
            m_PhaseStageEffectsPool.Push(phaseStageEffects);
        }
    }

    public void ClearEffectsPool()
    {
        Stack<GePhaseStageEffects>.Enumerator iterator = m_PhaseStageEffectsPool.GetEnumerator();
        while(iterator.MoveNext())
        {
            GePhaseStageEffects current = iterator.Current;
            Object.Destroy(current.m_EffectRoot);
        }

        m_PhaseStageEffectsPool.Clear();
    }

    private GePhaseStageEffects _CreateEffects()
    {
        if (m_PhaseStageEffectsTemplate == null)
            return null;

        GameObject newEffect = GameObject.Instantiate(m_PhaseStageEffectsTemplate);
        newEffect.transform.SetParent(m_PhaseEffectRoot.transform, false);
        newEffect.SetActive(true);

        return new GePhaseStageEffects() 
            { 
                m_EffectRoot = newEffect, 
                m_Particles = newEffect.GetComponentsInChildren<ParticleSystem>(),
                m_Renderers = newEffect.GetComponentsInChildren<Renderer>()
            };
    }
}

public class GePhaseEffect : Singleton<GePhaseEffect>
{
    // 每个职业的配置
    class GePhaseEffDesc
    {
        public string m_Name = "";
        public Shader m_Shader = null;
        public List<GePhaseStageDesc> m_StageDescList = new List<GePhaseStageDesc>();
    }

    Dictionary<string, GePhaseEffDesc> m_PhaseMatTable = new Dictionary<string, GePhaseEffDesc>();
    GameObject m_PhaseEffectRoot = null;
    
    public MaterialPropertyBlock MatPropertyBlock
    {
        get;
        private set;
    }

    public override void Init()
    {
        UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: All");

        UnInit();

        UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: Create Root");

        if (null == m_PhaseEffectRoot)
        {
            m_PhaseEffectRoot = new GameObject("PhaseEffectRoot");
            GameObject.DontDestroyOnLoad(m_PhaseEffectRoot);
        }

        UnityEngine.Profiling.Profiler.EndSample();

        if(null == MatPropertyBlock)
        {
            MatPropertyBlock = new MaterialPropertyBlock();
        }

        UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: Load Asset");

        AssetInst phaseMatAsset = AssetLoader.instance.LoadRes("Animat/PhaseEffData.asset");
        DPhaseEffectData phaseMatData = phaseMatAsset.obj as DPhaseEffectData;

        UnityEngine.Profiling.Profiler.EndSample();

        if (null != phaseMatData)
        {
            // 每个职业的配置
            for (int i = 0; i < phaseMatData.phaseMatChunk.Length; ++i)
            {
                DPhaseEffChunk curPhaseMatChk = phaseMatData.phaseMatChunk[i];

                if (!string.IsNullOrEmpty(curPhaseMatChk.shaderName))
                {
                    UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: Load Shader");

                    Shader shader = AssetShaderLoader.Find(curPhaseMatChk.shaderName);

                    UnityEngine.Profiling.Profiler.EndSample();

                    if(null != shader)
                    {
                        UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: New GePhaseEffDesc");

                        GePhaseEffDesc newPhaseMatDesc = new GePhaseEffDesc();

                        UnityEngine.Profiling.Profiler.EndSample();

                        newPhaseMatDesc.m_Name = curPhaseMatChk.name;
                        newPhaseMatDesc.m_Shader = shader;

                        if(null == curPhaseMatChk.phaseStageChunk)
                            continue;

                        // 每个强化等级的配置
                        for (int j = 0; j < curPhaseMatChk.phaseStageChunk.Length; ++j)
                        {
                            UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: Load Material");

                            DPhaseStageParamChunk curStageChk = curPhaseMatChk.phaseStageChunk[j];
                            Material newStageMat = new Material(shader);

                            // 设置材质参数
                            for (int k = 0; k < curStageChk.paramDesc.Length; ++k)
                            {
                                switch (curStageChk.paramDesc[k].paramType)
                                {
                                    case AnimatParamType.Color:
                                        newStageMat.SetColor(curStageChk.paramDesc[k].paramName, curStageChk.paramDesc[k].paramData._color);
                                        break;
                                    case AnimatParamType.Range:
                                        newStageMat.SetFloat(curStageChk.paramDesc[k].paramName, curStageChk.paramDesc[k].paramData._float);
                                        break;
                                    case AnimatParamType.Float:
                                        newStageMat.SetFloat(curStageChk.paramDesc[k].paramName, curStageChk.paramDesc[k].paramData._float);
                                        break;
                                    case AnimatParamType.TexEnv:
                                        Texture tex = AssetLoader.instance.LoadRes(curStageChk.paramDesc[k].paramObj._texAsset.m_AssetPath).obj as Texture;
                                        newStageMat.SetTexture(curStageChk.paramDesc[k].paramName, tex);
                                        break;
                                    case AnimatParamType.Vector:
                                        newStageMat.SetVector(curStageChk.paramDesc[k].paramName, curStageChk.paramDesc[k].paramData._vec4);
                                        break;
                                }
                            }

                            UnityEngine.Profiling.Profiler.EndSample();

                            // 加载特效Prefab

                            GameObject effParent = null;


                            UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: Load Prefab");

                            if (curStageChk.effectDesc.Length > 0)
                            {
                                effParent = new GameObject(curPhaseMatChk.name + j.ToString());
                                effParent.transform.SetParent(m_PhaseEffectRoot.transform, false);
                                effParent.SetActive(false);

                                for (int k = 0, kcnt = curStageChk.effectDesc.Length; k < kcnt; ++k)
                                {
                                    GameObject curEff = AssetLoader.instance.LoadResAsGameObject(curStageChk.effectDesc[k].effectResPath);
                                    if (null != curEff)
                                    {
                                        ParticleSystem curPart = curEff.GetComponentInChildren<ParticleSystem>();
                                        if (curPart)
                                            curPart.startColor = curStageChk.effectDesc[k].effectColor;

                                        MeshRenderer curMeshRend = curEff.GetComponentInChildren<MeshRenderer>();
                                        if (curMeshRend)
                                        {
                                            curMeshRend.GetPropertyBlock(MatPropertyBlock);

                                            MatPropertyBlock.SetColor("_TintColor", curStageChk.effectDesc[k].effectColor);
                                            MatPropertyBlock.SetColor("_Color", curStageChk.effectDesc[k].effectColor);

                                            curMeshRend.SetPropertyBlock(MatPropertyBlock);
                                        }

                                        curEff.transform.SetParent(effParent.transform, false);
                                    }
                                }
                            }

                            UnityEngine.Profiling.Profiler.EndSample();

                            UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: New GePhaseStageDesc");

                            newPhaseMatDesc.m_StageDescList.Add( new GePhaseStageDesc(newStageMat, effParent, curStageChk.needGlow, curStageChk.glowColor, m_PhaseEffectRoot));
        
                            UnityEngine.Profiling.Profiler.EndSample();
                        }

                        UnityEngine.Profiling.Profiler.BeginSample("PhaseEffectInit: Add To Dictionary");

                        m_PhaseMatTable.Add(curPhaseMatChk.name, newPhaseMatDesc);

                        UnityEngine.Profiling.Profiler.EndSample();
                    }
                }
            }
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void ClearPhaseStageEffectPool()
    {
        Dictionary<string, GePhaseEffDesc>.Enumerator it = m_PhaseMatTable.GetEnumerator();
        while (it.MoveNext())
        {
            GePhaseEffDesc cur = it.Current.Value;

            for(int i = 0;i < cur.m_StageDescList.Count;++i)
            {
                if (cur.m_StageDescList[i] != null)
                {
                    cur.m_StageDescList[i].ClearEffectsPool();
                }
            }
        }
    }

    public override void UnInit()
    {
        Dictionary<string, GePhaseEffDesc>.Enumerator it = m_PhaseMatTable.GetEnumerator();
        while(it.MoveNext())
        {
            GePhaseEffDesc cur = it.Current.Value;

            for(int i = 0;i < cur.m_StageDescList.Count;++i)
            {
                GePhaseStageDesc phaseStageDesc = cur.m_StageDescList[i];

                if(phaseStageDesc != null && phaseStageDesc.m_Material)
                {
                    Object.Destroy(phaseStageDesc.m_Material);
                }
            }
        }

        m_PhaseMatTable.Clear();
        GameObject.Destroy(m_PhaseEffectRoot);
        m_PhaseEffectRoot = null;
        MatPropertyBlock = null;
    }

    public GePhaseStageDesc CreatePhaseEffect(string phaseMatName,int stageIdx)
    {
        GePhaseEffDesc curPhaseMatDesc = null;
        if(m_PhaseMatTable.TryGetValue(phaseMatName, out curPhaseMatDesc))
        {
            if (stageIdx < curPhaseMatDesc.m_StageDescList.Count)
                return curPhaseMatDesc.m_StageDescList[stageIdx];
            else
            {
                if (curPhaseMatDesc.m_StageDescList.Count > 0)
                    return curPhaseMatDesc.m_StageDescList[curPhaseMatDesc.m_StageDescList.Count - 1];
                else
                    return null;
            }
        }

        return null;
    }
}
