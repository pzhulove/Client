using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering;

public partial class GeAnimatInstPool : Singleton<GeAnimatInstPool>, IObjectPool
{
    // Material对象池
    private Dictionary<string, Material> m_PooledMat = new Dictionary<string, Material>();
    // 每组Keyword生成一个ID
    private MaterialKeyWords m_MaterialKeywords = new MaterialKeyWords();
    // 材质动画的配置文件
    private DMatAnimatConfig m_MatAnimatConfig;

    private HashSet<string> m_KeywordsSet = new HashSet<string>();

    // 材质动画更新参数都使用这一个MaterialPropertyBlock
    public MaterialPropertyBlock MatPropertyBlock
    {
        get;
        private set;
    }

    private const string poolKey = "GeAnimatInstPool";
	private const string poolName = "GeAnimatInst池";

	public string GetPoolName ()
	{
		return poolName;
	}

	public string GetPoolInfo()
	{
		return m_PooledMat.Count.ToString();
	}

	public string GetPoolDetailInfo ()
	{
		return "detailInfo";
	}

    public override void Init()
    {
#if !SERVER_LOGIC 
		CPoolManager.GetInstance ().RegisterPool (poolKey, this);

        MatPropertyBlock = new MaterialPropertyBlock();
 #endif
    }

    public Material GetMaterial(DMatAnimatData animatData, MatAnimatShadeModel shadeModel, Material originMaterial)
    {
        if(animatData != null)
        {
            // 计算动画的Keywords和材质本身的Keywords的一个唯一ID
            m_KeywordsSet.Clear();
            if (animatData.m_keywords != null)
                m_KeywordsSet.UnionWith(animatData.m_keywords);
            // Cel着色模型使用了Keyword
            if (originMaterial && shadeModel == MatAnimatShadeModel.Cel)
            {
                if (originMaterial.IsKeywordEnabled("USE_MatCap"))
                    m_KeywordsSet.Add("USE_MatCap");
                if (originMaterial.IsKeywordEnabled("USE_Reflection"))
                    m_KeywordsSet.Add("USE_Reflection");
            }
            if(originMaterial && shadeModel == MatAnimatShadeModel.Simple)
            {
                if (originMaterial.IsKeywordEnabled("_SSSSkin"))
                    m_KeywordsSet.Add("_SSSSkin");
                if (originMaterial.IsKeywordEnabled("_EnvReflect"))
                    m_KeywordsSet.Add("_EnvReflect");
                if (originMaterial.IsKeywordEnabled("_ClolorStyle"))
                    m_KeywordsSet.Add("_ClolorStyle");
                if (originMaterial.IsKeywordEnabled("_Emissive"))
                    m_KeywordsSet.Add("_Emissive");
            }

            int keywordsID = _GetMaterialID(m_KeywordsSet);

            // MaterialKey:ShadeModel + ShaderType + IsOpaque + KeywordsID
            string materialKey = string.Format("{0}:{1}:{2}:{3}",
                shadeModel,
                animatData.m_ShaderType,
                animatData.m_IsOpaque,
                keywordsID);

            Material material;
            if (!m_PooledMat.TryGetValue(materialKey, out material))
            {
                material = _CreateMaterial(shadeModel, animatData.m_ShaderType, animatData.m_IsOpaque, m_KeywordsSet,
                                animatData.m_FloatParams, animatData.m_VectorParams, animatData.m_ColorParams, animatData.m_TextureParams);
                m_PooledMat.Add(materialKey, material);
            }
            return material;
        }

        return null;
    }

    public DMatAnimatData GetAnimatData(string animatName)
    {
        if (m_MatAnimatConfig == null)
        {
            AssetInst animatAsset = AssetLoader.instance.LoadRes("Animat/MaterialAnimatConfig.asset");
            if (null != animatAsset)
                m_MatAnimatConfig = animatAsset.obj as DMatAnimatConfig;
        }

        if (m_MatAnimatConfig != null && m_MatAnimatConfig.m_Datas != null)
        {
            for (int i = 0; i < m_MatAnimatConfig.m_Datas.Count; ++i)
            {
                DMatAnimatData animatData = m_MatAnimatConfig.m_Datas[i];
                if (animatData.m_Name == animatName)
                {
                    return animatData;
                }
            }
        }

        return null;
    }

    private Material _CreateMaterial(MatAnimatShadeModel shadeModel, MatAnimatShaderType shaderType, 
        bool isOpaque, IEnumerable<string> keywords, List<DMatAnimatFloatParam> floatParams,
        List<DMatAnimatVectorParam> vectorParams, List<DMatAnimatColorParam> colorParams,
        List<DMatAnimatTextureParam> textureParams)
    {
        Shader animatShader = null;
        switch(shadeModel)
        {
            case MatAnimatShadeModel.PBR:
                if(shaderType == MatAnimatShaderType.General)
                    animatShader = AssetShaderLoader.Find("HeroGo/PBR/Surface/General");
                else
                    animatShader = AssetShaderLoader.Find("HeroGo/PBR/Surface/Outline");
                break;

            case MatAnimatShadeModel.Cel:
                if (shaderType == MatAnimatShaderType.General)
                    animatShader = AssetShaderLoader.Find("HeroGo/Cel/Surface/General");
                else
                    animatShader = AssetShaderLoader.Find("HeroGo/Cel/Surface/Outline");
                break;

            case MatAnimatShadeModel.Dissolusion:
                if (shaderType == MatAnimatShaderType.General)
                    animatShader = AssetShaderLoader.Find("HeroGo/Dissolution/General");
                else
                    animatShader = AssetShaderLoader.Find("HeroGo/Dissolution/Outline");
                break;

            case MatAnimatShadeModel.Hair:
                if (shaderType == MatAnimatShaderType.General)
                    animatShader = AssetShaderLoader.Find("HeroGo/Hair/General");
                else
                    animatShader = AssetShaderLoader.Find("HeroGo/Hair/Outline");
                break;

            case MatAnimatShadeModel.SimulatePBR:
                if (shaderType == MatAnimatShaderType.General)
                    animatShader = AssetShaderLoader.Find("HeroGo/SimulatePBR/General");
                else
                    animatShader = AssetShaderLoader.Find("HeroGo/SimulatePBR/Outline");
                break;
            case MatAnimatShadeModel.Simple:
                if (shaderType == MatAnimatShaderType.General)
                    animatShader = AssetShaderLoader.Find("HeroGo/SimplePBR/General");
                else
                    animatShader = AssetShaderLoader.Find("HeroGo/SimplePBR/Outline");
                break;
        }

        if(animatShader != null)
        {
            Material animatMaterial = new Material(animatShader);

            if(!isOpaque)
            {
                // Queue
                animatMaterial.renderQueue = (int)RenderQueue.Transparent;
                // Blend State
                animatMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                animatMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                // ZWrite off
                //animatMaterial.SetInt("_ZWrite", 0);
            }
            else
            {
                // Queue
                animatMaterial.renderQueue = (int)RenderQueue.Geometry;
                // Blend State
                animatMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
                animatMaterial.SetInt("_DstBlend", (int)BlendMode.Zero);
                // ZWrite on
                //animatMaterial.SetInt("_ZWrite", 1);
            }

            foreach (string keyword in keywords)
                animatMaterial.EnableKeyword(keyword);

            return animatMaterial;
        }

        Debugger.LogError("Animat material create failed: {0}:{1}", shadeModel, shaderType);
        return null;
    }

    private int _GetMaterialID(HashSet<string> keywords)
    {
        if (keywords.Count > 0)
            return m_MaterialKeywords.GetKeywordsUniqueID(keywords);
        else
            return -1;
    }

    public void ClearAll()
    {
        var enumerator = m_PooledMat.GetEnumerator();
        while(enumerator.MoveNext())
        {
            Material material = enumerator.Current.Value;
            if (material != null)
                Object.Destroy(material);
        }

        m_PooledMat.Clear();

        m_MatAnimatConfig = null;
    }
}
