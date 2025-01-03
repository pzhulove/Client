using UnityEngine;

public class AssetProxy : MonoBehaviour
{
    public bool m_LogWhenDispose = false;
    protected AssetInst m_AssetInst = null;

    public void AddResRef(AssetProxy assetProxy)
    {
        if (null != assetProxy && null != assetProxy.m_AssetInst)
        {
            if (Global.Settings.enableAssetInstPool)
            {
                m_AssetInst = AssetInstPool.instance.GetAssetInst(assetProxy.m_AssetInst.assetDesc, gameObject,1);
            }
            else
            {
                m_AssetInst = new AssetInst();
                m_AssetInst.Init(assetProxy.m_AssetInst.assetDesc, gameObject, 1);
            }
        }

        if(null != m_AssetInst)
        {
            if (m_AssetInst.obj as GameObject == assetProxy.m_AssetInst.obj as GameObject)
                ExceptionManager.instance.RecordLog("m_AssetInst.obj == assetProxy.m_AssetInst.obj reference copy error!");
        }
    }



    public bool Init(AssetInst assetInst)
    {
        if (assetInst == m_AssetInst)
            return true;
        else
        {
            if (null == m_AssetInst)
            {
                if (null != assetInst)
                {
                    if (null == assetInst.obj)
                        Logger.LogAsset("Invalid asset instance!");

                    m_AssetInst = assetInst;
                    return true;
                }
                else
                    ExceptionManager.instance.RecordLog("null == assetInst!");
            }
            else
                ExceptionManager.instance.RecordLog("null == m_AssetInst!");
        }

        m_AssetInst = null;
        return false;
    }

    void OnEnable()
    {
        do 
        {
        } while (false);
    }

    void OnDestroy()
    {
        if (m_LogWhenDispose)
            Logger.LogErrorFormat("AssetProxy OnDestroy![{0}]", m_AssetInst.assetDesc.m_FullPath);

        if (null != m_AssetInst)
        {
            m_AssetInst.Release();
            m_AssetInst = null;
        }
        else
            Logger.LogAsset( "Destroy proxy without init!");

        m_LogWhenDispose = false;
    }
}