using UnityEngine;
using System.Collections.Generic;

public class AssetInstPool : Singleton<AssetInstPool>
{
    Queue<AssetInst> m_AssetInstPool = new Queue<AssetInst>();


    public AssetInst GetAssetInst(AssetDesc ad, Object obj = null,uint flag = 0u)
    {
        AssetInst newInst = null;
        if (m_AssetInstPool.Count > 0)
            newInst = m_AssetInstPool.Dequeue();
        else
            newInst = new AssetInst();

        if(null != newInst)
        {
            if (null == obj)
                newInst.Init(ad, flag);
            else
                newInst.Init(ad, obj, flag);
        }

        return newInst;
    }

    public void RecycleAssetInst(AssetInst assetInst)
    {
        if (null != assetInst)
        {
            assetInst.Reset();
            m_AssetInstPool.Enqueue(assetInst);
        }
    }


}
