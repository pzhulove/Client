using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

#if UNITY_EDITOR  && !LOGIC_SERVER
using UnityEditor;
#endif

[System.Serializable]
public struct DAssetObject
{
    public Object m_AssetObj;
    public string m_AssetPath;

    public DAssetObject(Object go, string path)
    {
        m_AssetObj = go;
        m_AssetPath = path;
    }

/*    static T CreateAsset<T>(DAssetObject assetObj) where T : UnityEngine.Object
    {
        if (assetObj.m_AssetObj)
            return GameObject.Instantiate(assetObj.m_AssetObj) as T;

        assetObj.m_AssetObj = AssetLoader.instance.LoadRes(assetObj.m_AssetPath).obj as GameObject;
        return assetObj.m_AssetObj as T;
    }*/

    public DAssetObject(string path,bool loadObject = false)
    {
#if LOGIC_SERVER
		m_AssetObj = null;
		m_AssetPath = path;
#else
        path = GetValidFilePath(path);
        if (loadObject)
            m_AssetObj = AssetLoader.instance.LoadRes(path).obj;
        else
            m_AssetObj = null;
        m_AssetPath = path;
#endif
    }

    public static string GetValidFilePath(string path)
    {
#if LOGIC_SERVER
	return "";
#else
        path = Path.ChangeExtension(path, null);
        StringBuilder pathBuilder = new StringBuilder(path);
        pathBuilder = pathBuilder.Replace("Assets/Resources/", "");
        return pathBuilder.ToString();
#endif
		
    }

    public void Set(string path)
    {
#if !LOGIC_SERVER		
        path = GetValidFilePath(path);
        m_AssetObj = AssetLoader.instance.LoadRes(path).obj;
        m_AssetPath = path;
#endif
    }

    public bool IsValid()
    {
#if LOGIC_SERVER		
	return true;
#else
        return m_AssetPath != null && m_AssetPath.Length > 1;
#endif
    }

#if UNITY_EDITOR

    public DAssetObject(Object go)
    {
#if LOGIC_SERVER	
		m_AssetObj= null;
		m_AssetPath = "";
#else
        m_AssetObj = go;
        m_AssetPath = GetValidFilePath(AssetDatabase.GetAssetPath(go));       
#endif
    }

    

    public void Set(Object go)
    {
#if !LOGIC_SERVER			
        m_AssetObj = go;
        m_AssetPath = GetValidFilePath(AssetDatabase.GetAssetPath(go));
#endif
    }

#endif
}
