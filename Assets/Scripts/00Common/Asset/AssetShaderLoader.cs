using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XUPorterJSON;

public class AssetShaderLoader : Singleton<AssetShaderLoader>
{
    protected readonly string m_ShaderListFile = "Shader/ShaderList.json";
    protected readonly ShaderVariantCollection m_ShaderVariantCollection = new ShaderVariantCollection();

    protected struct ShaderResDesc
    {
        public ShaderResDesc(string res, Shader shader)
        {
            m_ShaderRes = res;
            m_Shader = shader;
        }

        public string m_ShaderRes;
        public Shader m_Shader;
    }

    protected Dictionary<string, ShaderResDesc> m_ShaderNameMap = new Dictionary<string, ShaderResDesc>();

    public override void Init()
    { 
        TextAsset textAsset = AssetLoader.instance.LoadRes(m_ShaderListFile, typeof(TextAsset)).obj as TextAsset;
        if (null != textAsset)
        {
            Hashtable shaderList = new Hashtable();
            try
            {
                shaderList = MiniJSON.jsonDecode(textAsset.text) as Hashtable;

                IDictionaryEnumerator it = shaderList.GetEnumerator();
                while (it.MoveNext())
                {
                    string shaderName = it.Key as string;
                    string shaderRes = it.Value as string;
                    Shader shader = null;
                    if (!m_ShaderNameMap.ContainsKey(shaderName))
                    {
                        if (!string.IsNullOrEmpty(shaderRes))
                            shader = AssetLoader.instance.LoadRes(shaderRes, typeof(Shader)).obj as Shader;
                        
                        m_ShaderNameMap.Add(shaderName, new ShaderResDesc(shaderRes, shader));
                        /// m_ShaderVariantCollection.Add(new ShaderVariantCollection.ShaderVariant(shader, UnityEngine.Rendering.PassType.ForwardBase,));
                    }
                }

                //m_ShaderNameMap.Add("Standard", new ShaderResDesc("", Shader.Find("Standard")));
            }
            catch (System.Exception e)
            {
                Logger.LogAssetFormat("Get shader list form json has failed! Exception:" + e.ToString());
            }
        }
        else
            Logger.LogAsset("Load shader list has failed!");
    }

    public static void WarmupAllShaders()
    {
        Shader.WarmupAllShaders();
    }

    public override void UnInit()
    {
        m_ShaderNameMap.Clear();
    }

    public static Shader Find(string shaderName)
    {
        ShaderResDesc shaderDesc;
        if (instance.m_ShaderNameMap.TryGetValue(shaderName, out shaderDesc))
        {
            if (null == shaderDesc.m_Shader)
            {
                Shader shader = AssetLoader.instance.LoadRes(shaderDesc.m_ShaderRes, typeof(Shader)).obj as Shader;
                if (null != shader)
                    shaderDesc.m_Shader = shader;
            }
            return shaderDesc.m_Shader;
        }

        return Shader.Find(shaderName);
    }
}
