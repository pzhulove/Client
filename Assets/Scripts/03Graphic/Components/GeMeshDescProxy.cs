/*******************************************************************

 组件:

    网格描述组件
        表面属性 描述是否是半透属性等

 *******************************************************************/
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public struct GeSurfParamDesc
{
    public Vector3 m_WorldPos;
    public float m_ElapsedTime;
}

public class GeMeshDescProxy : MonoBehaviour
{
    [SerializeField]
    protected Material[] m_OriginMaterial = null;
    [SerializeField]
    protected Renderer m_Renderer = null;

    protected class GeAnimatSurfDesc
    {
        public GeAnimatSurfDesc(string name)
        {
            m_Name = name;
            m_Material = null;
        }

        public string m_Name = null;
        public Material[] m_Material = null;
        public DMatAnimatData m_AnimatData = null;
    }
    
    protected List<GeAnimatSurfDesc> m_AnimatSurfList = new List<GeAnimatSurfDesc>();
    protected GeAnimatSurfDesc m_CurAnimatSurfDesc = null;

    /// <summary>
    /// 表面属性：是否是不透明物体
    /// </summary>
    [SerializeField]
    public bool m_Surface_IsOpaque = true;

    /// <summary>
    /// 表面属性：表面动画关联 为空则加载公用的动画数据
    /// </summary>
    [SerializeField]
    public string m_Surface_AnimatRes = null;

    /// <summary>
    /// 着色模型，决定了材质动画使用哪个Shader
    /// </summary>
    [SerializeField]
    public MatAnimatShadeModel m_ShadeModel = MatAnimatShadeModel.Simple;

    #region ShaderPropertyID
    private static readonly int m_Property__MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int m_Property__BumpMap = Shader.PropertyToID("_BumpMap");
    private static readonly int m_Property__SpecMap = Shader.PropertyToID("_SpecMap");
    private static readonly int m_Property__Ramp = Shader.PropertyToID("_Ramp");
    private static readonly int m_Property__FresnelPow = Shader.PropertyToID("_FresnelPow");
    private static readonly int m_Property__FresnelOffset = Shader.PropertyToID("_FresnelOffset");
    private static readonly int m_Property__SpecularPow = Shader.PropertyToID("_SpecularPow");
    private static readonly int m_Property__ColorStrength = Shader.PropertyToID("_ColorStrength");
    private static readonly int m_Property__LightIntensity = Shader.PropertyToID("_LightIntensity");
    private static readonly int m_Property__AmbientColor = Shader.PropertyToID("_AmbientColor");
    private static readonly int m_Property__FresnelColor = Shader.PropertyToID("_FresnelColor");
    private static readonly int m_Property__DyeColor = Shader.PropertyToID("_DyeColor");
    private static readonly int m_Property__Albedo = Shader.PropertyToID("_Albedo");
    private static readonly int m_Property__Bump = Shader.PropertyToID("_Bump");
    private static readonly int m_Property__Param = Shader.PropertyToID("_Param");
    private static readonly int m_Property__ReflectionIntensity = Shader.PropertyToID("_ReflectionIntensity");
    private static readonly int m_Property__AmbientIntensity = Shader.PropertyToID("_AmbientIntensity");
    private static readonly int m_Property__ElapsedTime = Shader.PropertyToID("_ElapsedTime");
    private static readonly int m_Property__AnimTimeLen = Shader.PropertyToID("_AnimTimeLen");
    private static readonly int m_Property__WorldRefPos = Shader.PropertyToID("_WorldRefPos");
    //<-------------------------------------------Cel------------------------------------------------------------>//
    private static readonly int m_Property__NormalValue = Shader.PropertyToID("_NormalValue");
    private static readonly int m_Property__Intensity = Shader.PropertyToID("_Intensity");
    private static readonly int m_Property__LightArea = Shader.PropertyToID("_LightArea");
    private static readonly int m_Property__SecondShadow = Shader.PropertyToID("_SecondShadow");
    private static readonly int m_Property__Shininess = Shader.PropertyToID("_Shininess");
    private static readonly int m_Property__SpecularMultiply = Shader.PropertyToID("_SpecularMultiply");
    private static readonly int m_Property__EmissionIntensity = Shader.PropertyToID("_EmissionIntensity");
    private static readonly int m_Property__Bias = Shader.PropertyToID("_Bias");
    private static readonly int m_Property__TimeOnDuration = Shader.PropertyToID("_TimeOnDuration");
    private static readonly int m_Property__TimeOffDuration = Shader.PropertyToID("_TimeOffDuration");
    private static readonly int m_Property__BlinkingTimeOffsScale = Shader.PropertyToID("_BlinkingTimeOffsScale");
    private static readonly int m_Property__NoiseAmount = Shader.PropertyToID("_NoiseAmount");
    private static readonly int m_Property__RimPower = Shader.PropertyToID("_RimPower");
    private static readonly int m_Property__RimMultiply = Shader.PropertyToID("_RimMultiply");
    private static readonly int m_Property__MatCapFactor = Shader.PropertyToID("_MatCapFactor");
    private static readonly int m_Property__MatSpecFactor = Shader.PropertyToID("_MatSpecFactor");
    private static readonly int m_Property__ReflectionStrength = Shader.PropertyToID("_ReflectionStrength");
    private static readonly int m_Property__DrawMode = Shader.PropertyToID("_DrawMode");
    private static readonly int m_Property__LightDirection = Shader.PropertyToID("_LightDirection");
    private static readonly int m_Property__MainColor = Shader.PropertyToID("_MainColor");
    private static readonly int m_Property__FirstShadowMultColor = Shader.PropertyToID("_FirstShadowMultColor");
    private static readonly int m_Property__SecondShadowMultColor = Shader.PropertyToID("_SecondShadowMultColor");
    private static readonly int m_Property__LightSpecularColor = Shader.PropertyToID("_LightSpecularColor");
    private static readonly int m_Property__EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int m_Property__LightRimColor = Shader.PropertyToID("_LightRimColor");
    private static readonly int m_Property__MatCapColor = Shader.PropertyToID("_MatCapColor");
    private static readonly int m_Property__MatCapSpecColor = Shader.PropertyToID("_MatCapSpecColor");
    private static readonly int m_Property__ReflectionCOlor = Shader.PropertyToID("_ReflectionColor");
    private static readonly int m_Property__LightMapTex = Shader.PropertyToID("_LightMapTex");
    private static readonly int m_Property__NormalTex = Shader.PropertyToID("_NormalTex");
    private static readonly int m_Property__MatCap = Shader.PropertyToID("_MatCap");
    private static readonly int m_Property__MatCapSpec = Shader.PropertyToID("_MatCapSpec");
    private static readonly int m_Property__ReflectionMap = Shader.PropertyToID("_ReflectionMap");
    //<-------------------------------------------Cel------------------------------------------------------------>//
    //<---------------------------------------Dissolution-------------------------------------------------------->//
    private static readonly int m_Property__SpecColor = Shader.PropertyToID("_SpecColor");
    private static readonly int m_Property__RimColor = Shader.PropertyToID("_RimColor");
    private static readonly int m_Property__TwinkleColor = Shader.PropertyToID("_TwinkleColor");
    private static readonly int m_Property__MainTexturer = Shader.PropertyToID("_MainTextuer");
    private static readonly int m_Property__SpeTextuer = Shader.PropertyToID("_SpeTextuer");
    private static readonly int m_Property__NoiseTex = Shader.PropertyToID("_NoiseTex");
    private static readonly int m_Property__Normal = Shader.PropertyToID("_Normal");
    private static readonly int m_Property__DiffuseIntensity = Shader.PropertyToID("_DiffuseIntensity");
    private static readonly int m_Property__SpeIntensity = Shader.PropertyToID("_SpeIntensity");
    private static readonly int m_Property__DebrisSpeed = Shader.PropertyToID("_debrisSpeed");
    private static readonly int m_Property__DebrisSize = Shader.PropertyToID("_debrisSize");
    private static readonly int m_Property__WindSpeed = Shader.PropertyToID("_windSpeed");
    private static readonly int m_Property__WindPower = Shader.PropertyToID("_windPower");
    private static readonly int m_Property__WindFrequency = Shader.PropertyToID("_windFrequency");
    private static readonly int m_Property__RimLight = Shader.PropertyToID("_rimLight");
    private static readonly int m_Property__RimExp = Shader.PropertyToID("_RimExp");
    private static readonly int m_Property__RimIntensity = Shader.PropertyToID("_RimIntensity");
    private static readonly int m_Property__Twinkle = Shader.PropertyToID("_Twinkle");
    private static readonly int m_Property__TwinkleSpeed = Shader.PropertyToID("_TwinkleSpeed");
    private static readonly int m_Property__Cutoff = Shader.PropertyToID("_Cutoff");
    //<---------------------------------------Dissolution-------------------------------------------------------->//
    //<-------------------------------------------Hair----------------------------------------------------------->//
    private static readonly int m_Property__BrightColor = Shader.PropertyToID("_BrightColor");
        private static readonly int m_Property__BrightColorOffset = Shader.PropertyToID("_BrightColorOffset");
    private static readonly int m_Property__BrightColorSharpen = Shader.PropertyToID("_BrightColorSharpen");
    private static readonly int m_Property__BrightEdge = Shader.PropertyToID("_BrightEdge");
    private static readonly int m_Property__BrightEdgeOffset = Shader.PropertyToID("_BrightEdgeOffset");
    private static readonly int m_Property__BrightEdgeSharpen = Shader.PropertyToID("_BrightEdgeSharpen");
    private static readonly int m_Property__DarkColor = Shader.PropertyToID("_DarkColor");
    private static readonly int m_Property__DarkOffset = Shader.PropertyToID("_DarkOffset");
    private static readonly int m_Property__DarkSharpen = Shader.PropertyToID("_DarkSharpen");
    private static readonly int m_Property__DarkEdge = Shader.PropertyToID("_DarkEdge");
    private static readonly int m_Property__DarkEdgeOffset = Shader.PropertyToID("_DarkEdgeOffset");
    private static readonly int m_Property__DarkEdgeSharpen = Shader.PropertyToID("_DarkEdgeSharpen");
    private static readonly int m_Property__Color = Shader.PropertyToID("_Color");
    private static readonly int m_Property__Spe_Intensity = Shader.PropertyToID("_Spe_Intensity");
    //<-------------------------------------------Hair----------------------------------------------------------->//
    //<----------------------------------------SimulatePBR------------------------------------------------------->//
    private static readonly int m_Property__DirectionGradualColor = Shader.PropertyToID("_DirectionGradualColor");
    private static readonly int m_Property__Emission = Shader.PropertyToID("_Emission");
    private static readonly int m_Property__Noise = Shader.PropertyToID("_Noise");
    private static readonly int m_Property__GradualLocation = Shader.PropertyToID("_GradualLocation");
    private static readonly int m_Property__Glittering = Shader.PropertyToID("_Glittering");
    private static readonly int m_Property__LitSphere = Shader.PropertyToID("_LitSphere");
    //<----------------------------------------SimulatePBR------------------------------------------------------->//
    //<-----------------------------------------SimplePBR/------------------------------------------------------->//
    private static readonly int m_Property__LightPos = Shader.PropertyToID("_LightPos");
    private static readonly int m_Property__ShadowColor = Shader.PropertyToID("_ShadowColor");
    private static readonly int m_Property__BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int m_Property__Metalness = Shader.PropertyToID("_Metalness");
    private static readonly int m_Property__Roughness = Shader.PropertyToID("_Roughness");
    private static readonly int m_Property__DirLightColor = Shader.PropertyToID("_DirLightColor");
    private static readonly int m_Property__SpecIntensity = Shader.PropertyToID("_SpecIntensity");
    private static readonly int m_Property__ClothSpec = Shader.PropertyToID("_ClothSpec");
    private static readonly int m_Property__FresnelTrans = Shader.PropertyToID("_FresnelTrans");
    private static readonly int m_Property__SkinBright = Shader.PropertyToID("_SkinBright");
    private static readonly int m_Property__SkinSpec = Shader.PropertyToID("_SkinSpec");
    private static readonly int m_Property__EmissiveColor = Shader.PropertyToID("_EmissiveColor");
    private static readonly int m_Property__EmissiveExposure = Shader.PropertyToID("_EmissiveExposure");
    private static readonly int m_Property__ColorSelect = Shader.PropertyToID("_ColorSelect");
    //<-----------------------------------------SimplePBR/------------------------------------------------------->//
    #endregion


    #region 运行时方法
    public void Apply(string name,float fTimeLength,bool enableAnim = true)
    {
        if (null == m_Renderer || null == m_OriginMaterial)
            return;

        GeAnimatSurfDesc dstAnimatSurf = null;
        for (int i = 0,icnt = m_AnimatSurfList.Count;i<icnt;++i)
        {
            GeAnimatSurfDesc curAnimatSurf = m_AnimatSurfList[i];
            if(null != curAnimatSurf)
            {
                if(curAnimatSurf.m_Name == name)
                {
                    dstAnimatSurf = curAnimatSurf;
                    break;
                }
            }
        }

        if(null == dstAnimatSurf)
        {
            dstAnimatSurf = new GeAnimatSurfDesc(name);
            dstAnimatSurf.m_Material = new Material[m_OriginMaterial.Length];
            dstAnimatSurf.m_AnimatData = GeAnimatInstPool.instance.GetAnimatData(name);

            for (int j = 0, jcnt = m_OriginMaterial.Length; j < jcnt; ++j)
            {
                Material curOriginMat = m_OriginMaterial[j];
                if (null == curOriginMat)
                    continue;

                dstAnimatSurf.m_Material[j] = GeAnimatInstPool.instance.GetMaterial(dstAnimatSurf.m_AnimatData, m_ShadeModel, curOriginMat);
            }
            m_AnimatSurfList.Add(dstAnimatSurf);
        }
        else
        {
            // 缓存的材质可能被删掉
            for(int j = 0;j < dstAnimatSurf.m_Material.Length;++j)
            {
                if(dstAnimatSurf.m_Material[j] == null)
                    dstAnimatSurf.m_Material[j] = GeAnimatInstPool.instance.GetMaterial(dstAnimatSurf.m_AnimatData, m_ShadeModel, m_OriginMaterial[j]);
            }
        }


        if(null != dstAnimatSurf)
        {
            for (int j = 0, jcnt = dstAnimatSurf.m_Material.Length; j < jcnt; ++j)
            {
                Material curOriginMat = m_OriginMaterial[j];
                if (null == curOriginMat)
                    continue;

                MaterialPropertyBlock block = GeAnimatInstPool.instance.MatPropertyBlock;

                switch (m_ShadeModel)
                {
                    case MatAnimatShadeModel.PBR:
                        _CopyPropertyToBlockPBR(curOriginMat, block, j);
                        break;
                    case MatAnimatShadeModel.Cel:
                        _CopyPropertyToBlockCel(curOriginMat, block, j);
                        break;
                    case MatAnimatShadeModel.Dissolusion:
                        _CopyPropertyToBlockDissolution(curOriginMat, block, j);
                        break;
                    case MatAnimatShadeModel.Hair:
                        _CopyPropertyToBlockHair(curOriginMat, block, j);
                        break;
                    case MatAnimatShadeModel.SimulatePBR:
                        _CopyPropertyToBlockSimulatePBR(curOriginMat, block, j);
                        break;
                    case MatAnimatShadeModel.Simple:
                        _CopyPropertyToBlockSimplePBR(curOriginMat, block, j);
                        break;
                }

                if (!enableAnim)
                {
                    m_Renderer.GetPropertyBlock(block, j);
                    block.SetFloat(m_Property__ElapsedTime, 360000.0f);
                    m_Renderer.SetPropertyBlock(block, j);
                }
                else
                {
                    if(fTimeLength > 0.0f)
                    {
                        m_Renderer.GetPropertyBlock(block, j);
                        block.SetFloat(m_Property__AnimTimeLen, 360000.0f);
                        m_Renderer.SetPropertyBlock(block, j);
                    }
                }

                // 设置动画参数
                DMatAnimatData animatData = dstAnimatSurf.m_AnimatData;
                if(animatData != null)
                {
                    if(animatData.m_FloatParams != null)
                    {
                        for(int i = 0;i < animatData.m_FloatParams.Count;++i)
                        {
                            m_Renderer.GetPropertyBlock(block, j);
                            block.SetFloat(animatData.m_FloatParams[i].name, animatData.m_FloatParams[i].value);
                            m_Renderer.SetPropertyBlock(block, j);
                        }
                    }

                    if (animatData.m_VectorParams != null)
                    {
                        for (int i = 0; i < animatData.m_VectorParams.Count; ++i)
                        {
                            m_Renderer.GetPropertyBlock(block, j);
                            block.SetVector(animatData.m_VectorParams[i].name, animatData.m_VectorParams[i].value);
                            m_Renderer.SetPropertyBlock(block, j);
                        }
                    }

                    if (animatData.m_ColorParams != null)
                    {
                        for (int i = 0; i < animatData.m_ColorParams.Count; ++i)
                        {
                            m_Renderer.GetPropertyBlock(block, j);
                            block.SetColor(animatData.m_ColorParams[i].name, animatData.m_ColorParams[i].value);
                            m_Renderer.SetPropertyBlock(block, j);
                        }
                    }

                    if (animatData.m_TextureParams != null)
                    {
                        for (int i = 0; i < animatData.m_TextureParams.Count; ++i)
                        {
                            Texture tex = AssetLoader.instance.LoadRes(animatData.m_TextureParams[i].value).obj as Texture;
                            if (tex != null)
                            {
                                m_Renderer.GetPropertyBlock(block, j);
                                block.SetTexture(animatData.m_TextureParams[i].name, tex);
                                m_Renderer.SetPropertyBlock(block, j);
                            }
                        }
                    }
                }
            }

            m_Renderer.sharedMaterials = dstAnimatSurf.m_Material;
            m_CurAnimatSurfDesc = dstAnimatSurf;
        }
    }

    public void Recover()
    {
        if(m_Renderer != null && m_OriginMaterial != null)
            m_Renderer.sharedMaterials = m_OriginMaterial;
    }

    public void DoUpdate(GeSurfParamDesc param)
    {
        _UpdateInternal(param);
    }

    protected void _UpdateInternal(GeSurfParamDesc param)
    {
        if (m_Renderer != null && m_OriginMaterial != null)
        {
            for (int i = 0; i < m_OriginMaterial.Length; ++i)
            {
                MaterialPropertyBlock block = GeAnimatInstPool.instance.MatPropertyBlock;
                m_Renderer.GetPropertyBlock(block, i);
                block.SetFloat(m_Property__ElapsedTime, param.m_ElapsedTime);
                block.SetVector(m_Property__WorldRefPos, param.m_WorldPos);
                m_Renderer.SetPropertyBlock(block, i);

            }
        }
    }

    protected void _ReinitBase()
    {
        m_Renderer = gameObject.GetComponent<Renderer>();
        if (null != m_Renderer)
        {
            m_OriginMaterial = m_Renderer.sharedMaterials;
        }
    }

#if UNITY_EDITOR
    public string surfaceAnimatRes
    {
        set
        {
            m_Surface_AnimatRes = value;
        }
    }

#endif

    protected void _Clear()
    {
        m_AnimatSurfList.Clear();
        m_CurAnimatSurfDesc = null;

        Recover();
    }

    protected void _CopyPropertyToBlockPBR(Material material, MaterialPropertyBlock block, int materialIndex)
    {
        m_Renderer.GetPropertyBlock(block, materialIndex);

        _CopyCommonPropertyToBlock(material, block);
        _CopyTextureProperty(material, block, m_Property__Albedo);
        _CopyTextureProperty(material, block, m_Property__Bump);
        _CopyTextureProperty(material, block, m_Property__Param);
        _CopyFloatProperty(material, block, m_Property__ReflectionIntensity);
        _CopyFloatProperty(material, block, m_Property__AmbientIntensity);

        m_Renderer.SetPropertyBlock(block, materialIndex);
    }

    protected void _CopyPropertyToBlockCel(Material material, MaterialPropertyBlock block, int materialIndex)
    {
        m_Renderer.GetPropertyBlock(block, materialIndex);

        _CopyCommonPropertyToBlock(material, block);
        _CopyFloatProperty(material, block, m_Property__NormalValue);
        _CopyFloatProperty(material, block, m_Property__Intensity);
        _CopyFloatProperty(material, block, m_Property__LightArea);
        _CopyFloatProperty(material, block, m_Property__SecondShadow);
        _CopyFloatProperty(material, block, m_Property__Shininess);
        _CopyFloatProperty(material, block, m_Property__SpecularMultiply);
        _CopyFloatProperty(material, block, m_Property__EmissionIntensity);
        _CopyFloatProperty(material, block, m_Property__Bias);
        _CopyFloatProperty(material, block, m_Property__TimeOnDuration);
        _CopyFloatProperty(material, block, m_Property__TimeOffDuration);
        _CopyFloatProperty(material, block, m_Property__BlinkingTimeOffsScale);
        _CopyFloatProperty(material, block, m_Property__NoiseAmount);
        _CopyFloatProperty(material, block, m_Property__RimPower);
        _CopyFloatProperty(material, block, m_Property__RimMultiply);
        _CopyFloatProperty(material, block, m_Property__MatCapFactor);
        _CopyFloatProperty(material, block, m_Property__MatSpecFactor);
        _CopyFloatProperty(material, block, m_Property__ReflectionIntensity);
        _CopyFloatProperty(material, block, m_Property__ReflectionStrength);
        _CopyFloatProperty(material, block, m_Property__DrawMode);
        _CopyVectorProperty(material, block, m_Property__LightDirection);
        _CopyColorProperty(material, block, m_Property__MainColor);
        _CopyColorProperty(material, block, m_Property__FirstShadowMultColor);
        _CopyColorProperty(material, block, m_Property__SecondShadowMultColor);
        _CopyColorProperty(material, block, m_Property__LightSpecularColor);
        _CopyColorProperty(material, block, m_Property__EmissionColor);
        _CopyColorProperty(material, block, m_Property__LightRimColor);
        _CopyColorProperty(material, block, m_Property__MatCapColor);
        _CopyColorProperty(material, block, m_Property__MatCapSpecColor);
        _CopyColorProperty(material, block, m_Property__ReflectionCOlor);
        _CopyTextureProperty(material, block, m_Property__LightMapTex);
        _CopyTextureProperty(material, block, m_Property__NormalTex);
        _CopyTextureProperty(material, block, m_Property__MatCap);
        _CopyTextureProperty(material, block, m_Property__MatCapSpec);
        _CopyTextureProperty(material, block, m_Property__ReflectionMap);

        m_Renderer.SetPropertyBlock(block, materialIndex);
    }

    protected void _CopyPropertyToBlockDissolution(Material material, MaterialPropertyBlock block, int materialIndex)
    {
        m_Renderer.GetPropertyBlock(block, materialIndex);

        _CopyCommonPropertyToBlock(material, block);
        _CopyColorProperty(material, block, m_Property__MainColor);
        _CopyColorProperty(material, block, m_Property__SpecColor);
        _CopyColorProperty(material, block, m_Property__RimColor);
        _CopyColorProperty(material, block, m_Property__TwinkleColor);
        _CopyTextureProperty(material, block, m_Property__MainTexturer);
        _CopyTextureProperty(material, block, m_Property__SpeTextuer);
        _CopyTextureProperty(material, block, m_Property__NoiseTex);
        _CopyTextureProperty(material, block, m_Property__Normal);
        _CopyFloatProperty(material, block, m_Property__DiffuseIntensity);
        _CopyFloatProperty(material, block, m_Property__SpeIntensity);
        _CopyFloatProperty(material, block, m_Property__DebrisSpeed);
        _CopyFloatProperty(material, block, m_Property__DebrisSize);
        _CopyFloatProperty(material, block, m_Property__WindSpeed);
        _CopyFloatProperty(material, block, m_Property__WindPower);
        _CopyFloatProperty(material, block, m_Property__WindFrequency);
        _CopyFloatProperty(material, block, m_Property__RimLight);
        _CopyFloatProperty(material, block, m_Property__RimExp);
        _CopyFloatProperty(material, block, m_Property__RimIntensity);
        _CopyFloatProperty(material, block, m_Property__Twinkle);
        _CopyFloatProperty(material, block, m_Property__TwinkleSpeed);
        _CopyFloatProperty(material, block, m_Property__Cutoff);

        m_Renderer.SetPropertyBlock(block, materialIndex);
    }

    protected void _CopyPropertyToBlockHair(Material material, MaterialPropertyBlock block, int materialIndex)
    {
        m_Renderer.GetPropertyBlock(block, materialIndex);
     
        _CopyCommonPropertyToBlock(material, block);
        _CopyColorProperty(material, block, m_Property__BrightColor);
        _CopyColorProperty(material, block, m_Property__BrightEdge);
        _CopyColorProperty(material, block, m_Property__DarkColor);
        _CopyColorProperty(material, block, m_Property__DarkEdge);
        _CopyColorProperty(material, block, m_Property__Color);
        _CopyFloatProperty(material, block, m_Property__Spe_Intensity);
        _CopyFloatProperty(material, block, m_Property__BrightColorOffset);
        _CopyFloatProperty(material, block, m_Property__BrightColorSharpen);
        _CopyFloatProperty(material, block, m_Property__BrightEdgeOffset);
        _CopyFloatProperty(material, block, m_Property__BrightEdgeSharpen);
        _CopyFloatProperty(material, block, m_Property__DarkOffset);
        _CopyFloatProperty(material, block, m_Property__DarkSharpen);
        _CopyFloatProperty(material, block, m_Property__DarkEdgeOffset);
        _CopyFloatProperty(material, block, m_Property__DarkEdgeSharpen);
        _CopyTextureProperty(material, block, m_Property__Albedo);
        _CopyTextureProperty(material, block, m_Property__Param);

        m_Renderer.SetPropertyBlock(block, materialIndex);
    }

    protected void _CopyPropertyToBlockSimulatePBR(Material material, MaterialPropertyBlock block, int materialIndex)
    {
        m_Renderer.GetPropertyBlock(block, materialIndex);

        _CopyCommonPropertyToBlock(material, block);
        _CopyColorProperty(material, block, m_Property__BrightColor);
        _CopyColorProperty(material, block, m_Property__BrightEdge);
        _CopyColorProperty(material, block, m_Property__DarkColor);
        _CopyColorProperty(material, block, m_Property__DarkEdge);
        _CopyColorProperty(material, block, m_Property__Color);
        _CopyFloatProperty(material, block, m_Property__BrightColorOffset);
        _CopyFloatProperty(material, block, m_Property__BrightColorSharpen);
        _CopyFloatProperty(material, block, m_Property__BrightEdgeOffset);
        _CopyFloatProperty(material, block, m_Property__BrightEdgeSharpen);
        _CopyFloatProperty(material, block, m_Property__DarkOffset);
        _CopyFloatProperty(material, block, m_Property__DarkSharpen);
        _CopyFloatProperty(material, block, m_Property__DarkEdgeOffset);
        _CopyFloatProperty(material, block, m_Property__DarkEdgeSharpen);
        _CopyFloatProperty(material, block, m_Property__DirectionGradualColor);
        _CopyFloatProperty(material, block, m_Property__GradualLocation);
        _CopyFloatProperty(material, block, m_Property__Glittering);
        _CopyFloatProperty(material, block, m_Property__EmissionIntensity);
        _CopyTextureProperty(material, block, m_Property__Albedo);
        _CopyTextureProperty(material, block, m_Property__Param);
        _CopyTextureProperty(material, block, m_Property__Emission);
        _CopyTextureProperty(material, block, m_Property__Noise);
        _CopyTextureProperty(material, block, m_Property__LitSphere);

        m_Renderer.SetPropertyBlock(block, materialIndex);
    }

    protected void _CopyPropertyToBlockSimplePBR(Material material, MaterialPropertyBlock block, int materialIndex)
    {
        m_Renderer.GetPropertyBlock(block, materialIndex);

        _CopyCommonPropertyToBlock(material, block);
        _CopyColorProperty(material, block, m_Property__ShadowColor);
        _CopyColorProperty(material, block, m_Property__BaseColor);
        _CopyColorProperty(material, block, m_Property__DirLightColor);
        _CopyColorProperty(material, block, m_Property__EmissiveColor);

        _CopyFloatProperty(material, block, m_Property__Metalness);
        _CopyFloatProperty(material, block, m_Property__Roughness);
        _CopyFloatProperty(material, block, m_Property__SpecIntensity);
        _CopyFloatProperty(material, block, m_Property__ClothSpec);
        _CopyFloatProperty(material, block, m_Property__FresnelTrans);
        _CopyFloatProperty(material, block, m_Property__SkinBright);
        _CopyFloatProperty(material, block, m_Property__SkinSpec);
        _CopyFloatProperty(material, block, m_Property__EmissiveExposure);
        _CopyFloatProperty(material, block, m_Property__ColorSelect);

        _CopyVectorProperty(material, block, m_Property__LightPos);
        _CopyTextureProperty(material, block, m_Property__Albedo);
        _CopyTextureProperty(material, block, m_Property__Bump);
        _CopyTextureProperty(material, block, m_Property__Param);
        _CopyTextureProperty(material, block, m_Property__LitSphere);


        m_Renderer.SetPropertyBlock(block, materialIndex);
    }

    protected void _CopyCommonPropertyToBlock(Material material, MaterialPropertyBlock block)
    {
        _CopyTextureProperty(material, block, m_Property__MainTex);
        _CopyTextureProperty(material, block, m_Property__BumpMap);
        if (material.HasProperty(m_Property__SpecMap))
        {
            block.SetTexture(m_Property__SpecMap, material.GetTexture(m_Property__SpecMap));
        }

        if (material.HasProperty(m_Property__Ramp))
        {
            block.SetTexture(m_Property__Ramp, material.GetTexture(m_Property__Ramp));
        }

        _CopyFloatProperty(material, block, m_Property__FresnelPow);
        _CopyFloatProperty(material, block, m_Property__FresnelOffset);
        _CopyFloatProperty(material, block, m_Property__SpecularPow);
        _CopyFloatProperty(material, block, m_Property__ColorStrength);
        _CopyFloatProperty(material, block, m_Property__LightIntensity);
        _CopyColorProperty(material, block, m_Property__AmbientColor);
        _CopyColorProperty(material, block, m_Property__FresnelColor);
        _CopyColorProperty(material, block, m_Property__DyeColor);
    }

    protected void _CopyFloatProperty(Material material, MaterialPropertyBlock block, int propertyID)
    {
        if (material.HasProperty(propertyID))
            block.SetFloat(propertyID, material.GetFloat(propertyID));
    }

    protected void _CopyColorProperty(Material material, MaterialPropertyBlock block, int propertyID)
    {
        if (material.HasProperty(propertyID))
            block.SetColor(propertyID, material.GetColor(propertyID));
    }

    protected void _CopyVectorProperty(Material material, MaterialPropertyBlock block, int propertyID)
    {
        if (material.HasProperty(propertyID))
            block.SetVector(propertyID, material.GetVector(propertyID));
    }

    protected void _CopyTextureProperty(Material material, MaterialPropertyBlock block, int propertyID)
    {
        if (material.HasProperty(propertyID))
        {
            Texture tex = material.GetTexture(propertyID);
            if(tex != null)
                block.SetTexture(propertyID, material.GetTexture(propertyID));
        }
    }


    protected void OnEnable()
    {
        if (null == m_Renderer)
            _ReinitBase();
    }

    public void OnDestroy()
    {
        Recover();
        _Clear();
    }

#endregion

#region 编辑器方法

    public void RebakeAnimat()
    {
        //_Clear();
        _ReinitBase();
    }

#endregion
}
