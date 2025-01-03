using UnityEngine;
using System.Collections.Generic;

public class GeMaterialPool : Singleton<GeMaterialPool>
{
    private Dictionary<string, Queue<Material> > m_pooledMatInstMap = new Dictionary<string, Queue<Material>>();

    public Material CreateMaterialInstance(string shaderName)
    {
        string materialKey = CFileManager.EraseExtension(shaderName);

        Queue<Material> queue = null;
        if (!this.m_pooledMatInstMap.TryGetValue(materialKey, out queue))
        {
            queue = new Queue<Material>();
            this.m_pooledMatInstMap.Add(materialKey , queue);
        }

        Material matInst = null;
        while (queue.Count > 0)
        {
            matInst = queue.Dequeue();

            if (matInst != null)
                break;

            matInst = null;
        }

        if (matInst != null)
        {
            return matInst;
        }

        Shader newShader = AssetShaderLoader.Find(shaderName);
        if (null != newShader)
        {
            Material cur = new Material(newShader);
            return cur;
        }

        Logger.LogWarningFormat("Can not create with shader resource:[{0}]", shaderName);
        return null;
    }

    public void RecycleMaterialInstance(string materialKey, Material matInst)
    {
        if (null == matInst)
            return;

        Queue<Material> queue = null;
        if (m_pooledMatInstMap.TryGetValue(materialKey, out queue))
            queue.Enqueue(matInst);
        else
            Material.Destroy(matInst);

    }

    public void ClearAll()
    {
        Logger.Log("Execute clear pool materials!");
        Dictionary<string, Queue<Material>>.Enumerator enumerator = m_pooledMatInstMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<Material>> current = enumerator.Current;

            Queue<Material> queue = current.Value;
            while (queue.Count > 0)
            {
                Material mat = queue.Dequeue();
                if (null != mat)
                    Material.Destroy(mat);
            }
        }

        m_pooledMatInstMap.Clear();
    }
}
