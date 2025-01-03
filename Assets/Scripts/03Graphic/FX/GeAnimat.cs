using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeAnimat
{
    protected struct GeAnimatRendDesc
    {
        public GeAnimatRendDesc(Renderer r, Material[] o,Material[] a)
        {
            meshRenderer = r;
            orginMatArray = o;
            animatMatArray = a;
        }

        public Renderer meshRenderer;
        public Material[] orginMatArray;
        public Material[] animatMatArray;
    }


    protected struct GeAnimatObjDesc
    {
        public GeAnimatObjDesc(GameObject o, GeAnimatRendDesc[] a)
        {
            obj = o;
            rendDescArray = a;
        }
        public GameObject obj;
        public GeAnimatRendDesc[] rendDescArray;
    }

    public void Init(string AnimatName, string shaderName, DAnimatParamDesc[] animatParam, GameObject[] obj)
    {
        Clear();
        m_AnimatName = AnimatName;

        if (!string.IsNullOrEmpty(shaderName))
        {
            m_AnimatShader = AssetShaderLoader.Find(shaderName);
            m_AnimatMaterial = new Material(m_AnimatShader);

            for (int j = 0; j < animatParam.Length; ++j)
            {
                switch (animatParam[j].paramType)
                {
                    case AnimatParamType.Color:
                        m_AnimatMaterial.SetColor(animatParam[j].paramName, animatParam[j].paramData._color);
                        break;
                    case AnimatParamType.Range:
                        m_AnimatMaterial.SetFloat(animatParam[j].paramName, animatParam[j].paramData._float);
                        break;
                    case AnimatParamType.Float:
                        m_AnimatMaterial.SetFloat(animatParam[j].paramName, animatParam[j].paramData._float);
                        break;
                    case AnimatParamType.TexEnv:
                        Texture tex = AssetLoader.instance.LoadRes(animatParam[j].paramObj._texAsset.m_AssetPath).obj as Texture;
                        m_AnimatMaterial.SetTexture(animatParam[j].paramName, tex);
                        break;
                    case AnimatParamType.Vector:
                        m_AnimatMaterial.SetVector(animatParam[j].paramName, animatParam[j].paramData._vec4);
                        break;
                }
            }
        }

        AppendObject(obj);
    }

    public void AppendObject(GameObject[] obj)
    {
        if (null == m_AnimatMaterial)
        {
            Logger.LogErrorFormat("Animat \"{0}\" does not specify shader!", m_AnimatName);
            return;
        }

        if (null == obj)
            return;

        for(int i = 0; i < obj.Length;++i)
        {
            if(null == obj[i])
                continue;

            if(obj[i].CompareTag("EffectModel"))
                continue;

            Renderer[] smr = obj[i].GetComponentsInChildren<Renderer>();
            //List<Renderer> aMeshRendLst = new List<Renderer>();
            List<Renderer> aMeshRendLst = GamePool.ListPool<Renderer>.Get();
            for (int j = 0,mrcnt = smr.Length; j < mrcnt; ++ j)
            {
                if (smr[j].gameObject.CompareTag("EffectModel"))
                    continue;
                aMeshRendLst.Add(smr[j]);
            }
            smr = aMeshRendLst.ToArray();
            GamePool.ListPool<Renderer>.Release(aMeshRendLst);

            GeAnimatRendDesc[] animatRendDescArray = new GeAnimatRendDesc[smr.Length];

            for (int j = 0, mrcnt = smr.Length; j < mrcnt; ++j)
            {
                Renderer sr = smr[j];
                if (sr.GetType() != typeof(SkinnedMeshRenderer) &&
                    sr.GetType() != typeof(MeshRenderer) )
                    continue;
                //if (sr.GetType() != typeof(SkinnedMeshRenderer) )
                //    continue;

                Material[] tms = sr.materials;
                Material[] msBackup = new Material[tms.Length];
                Material[] msAnimat = new Material[tms.Length];
                for (int k = 0; k < tms.Length; ++k)
                    msBackup[k] = tms[k];

                for (int k = 0; k < tms.Length; ++k)
                {
                    if(null == m_AnimatMaterial)
                        continue;

                    msAnimat[k] = new Material(m_AnimatMaterial);
                    msAnimat[k].name = string.Format("{0}_{1}", sr.name, m_AnimatName);

                    if (tms[k].HasProperty("_MainTex"))
                        msAnimat[k].SetTexture("_MainTex", tms[k].GetTexture("_MainTex"));
                    
                    if(tms[k].HasProperty("_BumpMap"))
                        msAnimat[k].SetTexture("_BumpMap", tms[k].GetTexture("_BumpMap"));

                    if (msAnimat[k].HasProperty("_SpecMap"))
                    {
                        if (tms[k].HasProperty("_SpecMap"))
                            msAnimat[k].SetTexture("_SpecMap", tms[k].GetTexture("_SpecMap"));
                        else
                        {
                            Texture spec = AssetLoader.instance.LoadRes("Shader/HeroGo/Res/black.tga", typeof(Texture2D)).obj as Texture2D;
                            msAnimat[k].SetTexture("_SpecMap", spec);
                        }
                    }

                    if (msAnimat[k].HasProperty("_Ramp"))
                    {
                        Texture ramp = null;
                        if(tms[k].HasProperty("_Ramp"))
                            ramp = tms[k].GetTexture("_Ramp");
                        if (null == ramp)
                            ramp = AssetLoader.instance.LoadRes("Actor/Monster_Kelahe/Monster_Kelahe_bingshuangklh/Textures/T_Monster_Kelahe_bingshuangklh_ra.png", typeof(Texture2D)).obj as Texture2D;
                        msAnimat[k].SetTexture("_Ramp", ramp);
                    }
                    
		            if (tms[k].HasProperty("_FresnelPow"))
                        msAnimat[k].SetFloat("_FresnelPow", tms[k].GetFloat("_FresnelPow"));
                    if (tms[k].HasProperty("_FresnelOffset"))
                        msAnimat[k].SetFloat("_FresnelOffset", tms[k].GetFloat("_FresnelOffset"));
                    if (tms[k].HasProperty("_SpecularPow"))
                        msAnimat[k].SetFloat("_SpecularPow", tms[k].GetFloat("_SpecularPow"));
                    if (tms[k].HasProperty("_ColorStrength"))
                        msAnimat[k].SetFloat("_ColorStrength", tms[k].GetFloat("_ColorStrength"));
                    if (tms[k].HasProperty("_LightIntensity"))
                        msAnimat[k].SetFloat("_LightIntensity", tms[k].GetFloat("_LightIntensity"));
                    if (tms[k].HasProperty("_AmbientColor"))
                        msAnimat[k].SetColor("_AmbientColor", tms[k].GetColor("_AmbientColor"));
                    if (tms[k].HasProperty("_FresnelColor"))
                        msAnimat[k].SetColor("_FresnelColor", tms[k].GetColor("_FresnelColor"));

                    if (tms[k].HasProperty("_DyeColor") && msAnimat[k].HasProperty("_DyeColor"))
                        msAnimat[k].SetColor("_DyeColor", tms[k].GetColor("_DyeColor"));
                }

                animatRendDescArray[j].meshRenderer = sr;
                animatRendDescArray[j].orginMatArray = msBackup;
                animatRendDescArray[j].animatMatArray = msAnimat;
            }

            m_AnimatObjDescList.Add(new GeAnimatObjDesc(obj[i], animatRendDescArray));
        }
    }

    public void RemoveObject(GameObject[] obj)
    {
        for (int i = 0; i < obj.Length; ++i)
        {
            m_AnimatObjDescList.RemoveAll(
                f=>
                {
                    if (f.obj == obj[i])
                    {
                        for (int j = 0; j < f.rendDescArray.Length; ++j)
                        {
                            GeAnimatRendDesc rendDesc = f.rendDescArray[j];
                            if(null == rendDesc.meshRenderer) continue;

                            Material[] tms = rendDesc.meshRenderer.materials;
                            if (null != tms)
                            {
                                for (int k = 0; k < tms.Length; ++k)
                                    tms[k] = rendDesc.orginMatArray[k];
                                rendDesc.meshRenderer.materials = tms;
                            }

                            for (int k = 0; k < rendDesc.animatMatArray.Length; ++k)
                            {
                                if (rendDesc.animatMatArray[k].HasProperty("_MainTex"))
                                    rendDesc.animatMatArray[k].SetTexture("_MainTex", null);

                                if (rendDesc.animatMatArray[k].HasProperty("_BumpMap"))
                                    rendDesc.animatMatArray[k].SetTexture("_BumpMap", null);

                                if (rendDesc.animatMatArray[k].HasProperty("_SpecMap"))
                                    rendDesc.animatMatArray[k].SetTexture("_SpecMap", null);

                                if (rendDesc.animatMatArray[k].HasProperty("_Ramp"))
                                    rendDesc.animatMatArray[k].SetTexture("_Ramp", null);

                                UnityEngine.Object.Destroy(rendDesc.animatMatArray[k]);
                                rendDesc.animatMatArray[k] = null;
                            }

                            rendDesc.meshRenderer = null;
                            rendDesc.orginMatArray = null;
                            rendDesc.animatMatArray = null;

                        }
                        f.rendDescArray = null;

                        return true;
                    }
                    else
                        return false;
                }
                );
        }
    }

    public void Apply(float timeLen,float timeOffset,bool enableAnim,bool recover)
    {
        m_EnableAnim = enableAnim;
        m_RecoverWhenEnd = recover;
        m_TimeLen = timeLen;
        if (!m_EnableAnim)
            m_TimePos = m_TimeLen;
        else
            m_TimePos = timeOffset;

        for (int i = 0; i < m_AnimatObjDescList.Count; ++i)
        { 
            for (int j = 0; j < m_AnimatObjDescList[i].rendDescArray.Length; ++j)
            {
                GeAnimatRendDesc rendDesc = m_AnimatObjDescList[i].rendDescArray[j];
                if (null == rendDesc.meshRenderer) continue;

                Material[] tms = rendDesc.meshRenderer.materials;
                if(null == tms) continue;
                for (int k = 0; k < tms.Length; ++k)
                {
                    tms[k] = rendDesc.animatMatArray[k];
                    if (!enableAnim)
                    {
                        if (tms[k].HasProperty("_ElapsedTime"))
                            tms[k].SetFloat("_ElapsedTime", 360000.0f);
                    }
                    else
                    {
                        if (tms[k].HasProperty("_AnimTimeLen") && m_TimeLen > 0.0f)
                            tms[k].SetFloat("_AnimTimeLen", m_TimeLen);
                    }
                }
                rendDesc.meshRenderer.materials = tms;
            }
        }
        m_IsPlaying = true;
        m_IsEnd = false;
    }
    public void Recover()
    {
        if (!m_RecoverWhenEnd)
            return;

        /// 前题 是Renderer里面的材质数量不要改变
        for (int i = 0; i < m_AnimatObjDescList.Count; ++i)
        {
            for (int j = 0; j < m_AnimatObjDescList[i].rendDescArray.Length; ++j)
            {
                GeAnimatRendDesc rendDesc = m_AnimatObjDescList[i].rendDescArray[j];
                if (null == rendDesc.meshRenderer) continue;

                Material[] tms = rendDesc.meshRenderer.materials;
                if (null == tms) continue;
                for (int k = 0; k < tms.Length; ++k)
                    tms[k] = rendDesc.orginMatArray[k];
                rendDesc.meshRenderer.materials = tms;
            }
        }

        m_IsPlaying = false;
        m_IsEnd = true;
    }

    public void Update(bool timeOnly,float deltaTime,GameObject obj)
    {
        if (m_IsPlaying)
        {
            m_TimePos += deltaTime * 0.001f;
            if (timeOnly)
                return;

            if (m_TimeLen > 0.0f && m_TimePos > m_TimeLen)
            {
                Recover();
                m_IsPlaying = false;
                m_IsEnd = true;
                return;
            }
            
            for (int i = 0; i < m_AnimatObjDescList.Count; ++i)
            {
                for (int j = 0; j < m_AnimatObjDescList[i].rendDescArray.Length; ++j)
                {
                    GeAnimatRendDesc rendDesc = m_AnimatObjDescList[i].rendDescArray[j];
                    if(null == rendDesc.animatMatArray)
                        continue;
                    for (int k = 0; k < rendDesc.animatMatArray.Length; ++k)
                    {
                        if (m_EnableAnim && m_AnimatMaterial.HasProperty("_ElapsedTime"))
                            rendDesc.animatMatArray[k].SetFloat("_ElapsedTime", m_TimePos);

                        if (m_AnimatMaterial.HasProperty("_WorldRefPos"))
                            rendDesc.animatMatArray[k].SetVector("_WorldRefPos", obj.transform.position);
                    }
                }
            }
        }
    }

    public void Pause()
    {
    }

    public void Resume()
    {

    }

    public void SetFinish()
    {
        m_IsEnd = true;
    }

    public void Clear(bool bNeedRecover = false)
    {
        if(bNeedRecover)
            Recover();

        for (int i = 0; i < m_AnimatObjDescList.Count; ++i)
        {
            for (int j = 0; j < m_AnimatObjDescList[i].rendDescArray.Length; ++j)
            {
                GeAnimatRendDesc rendDesc = m_AnimatObjDescList[i].rendDescArray[j];
                if (null == rendDesc.animatMatArray) continue;
                for (int k = 0; k < rendDesc.animatMatArray.Length; ++k)
                {
                    if (null != rendDesc.animatMatArray[k])
                    {
                        if (rendDesc.animatMatArray[k].HasProperty("_MainTex"))
                            rendDesc.animatMatArray[k].SetTexture("_MainTex", null);

                        if (rendDesc.animatMatArray[k].HasProperty("_BumpMap"))
                            rendDesc.animatMatArray[k].SetTexture("_BumpMap", null);

                        if (rendDesc.animatMatArray[k].HasProperty("_SpecMap"))
                            rendDesc.animatMatArray[k].SetTexture("_SpecMap", null);

                        if (rendDesc.animatMatArray[k].HasProperty("_Ramp"))
                            rendDesc.animatMatArray[k].SetTexture("_Ramp", null);

                        UnityEngine.Object.Destroy(rendDesc.animatMatArray[k]);
                        rendDesc.animatMatArray[k] = null;
                    }
                }

                rendDesc.meshRenderer = null;
                rendDesc.orginMatArray = null;
                rendDesc.animatMatArray = null;
            }
        }

        m_AnimatObjDescList.Clear();
        UnityEngine.Object.Destroy(m_AnimatMaterial);
        m_AnimatMaterial = null;
        m_AnimatShader = null;
    }

    public float GetElapsedTime()
    {
        return m_TimePos;
    }
    public float GetTimeLen()
    {
        return m_TimeLen;
    }

    public bool IsAnimate()
    {
        return m_EnableAnim;
    }

    public bool IsFinished()
    {
        return m_IsEnd;
    }

    public bool IsRecover()
    {
        return m_RecoverWhenEnd;
    }

    /// <summary>
    /// 
    /// </summary>

    protected List<GeAnimatObjDesc> m_AnimatObjDescList = new List<GeAnimatObjDesc>();

    protected Shader m_AnimatShader = null;
    protected Material m_AnimatMaterial = null;
    protected string m_AnimatName = null;
    
    protected float m_TimeLen = 0.0f;
    protected float m_TimePos = 0.0f;
    protected bool m_IsPlaying = false;
    protected bool m_IsEnd = false;
    protected bool m_EnableAnim = true;
    protected bool m_RecoverWhenEnd = false;

}
