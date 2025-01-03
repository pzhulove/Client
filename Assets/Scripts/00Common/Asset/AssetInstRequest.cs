using UnityEngine;


/// <summary>
/// AssetLoader的异步请求接口
/// </summary>
public interface IAssetInstRequest
{
    /// <summary>
    /// 查询请求的资源知否加载完毕
    /// </summary>
    /// <returns>true为加载完毕</returns>
    bool IsDone();

    /// <summary>
    /// 提取加载完成的资源实例，只能提取一次
    /// 注意：加载完毕的资源提取后，才会从加载列表中移除
    /// </summary>
    /// <returns>
    ///     返回加载目标资源实例
    /// </returns>
    AssetInst Extract();

    /// <summary>
    /// 终止目标资源的异步加载过程
    /// 注意：调用后目标资源加载请求将从请求列表中移除
    /// </summary>
    void Abort();
}

public class AssetInstRequest : IAssetInstRequest
{
    public AssetInstRequest()
    {
        Reset();
    }

    public void Reset()
    {
        m_HasExtract = false;
        m_AssetInst = null;
        m_IsDone = false;
        m_IsAbort = false;
        m_flag = 0u;
        m_waterMark = 0u;
    }

    public bool IsDone()
    {
        return m_IsDone;
    }

    public AssetInst Extract()
    {
        if (!m_IsDone)
            return null;

        if (!m_HasExtract)
        {
            AssetInst target = m_AssetInst;
            m_AssetInst = null;
            m_HasExtract = true;

//#if UNITY_EDITOR
//            Logger.LogError("AssetInstReq Extract:" + m_waterMark.ToString("x"));
//#endif

            return target;
        }
        else
        {
            return null;
        }
    }

    public void Abort()
    {
        //#if UNITY_EDITOR
        //        Logger.LogError("Abort:" + m_waterMark.ToString("x"));
        //#endif
        m_IsAbort = true;

        if (null != m_AssetInst && m_AssetInst.isGameObject)
            Object.Destroy(m_AssetInst.obj);

    }

    public bool m_HasExtract = false;
    public AssetInst m_AssetInst = null;
    public bool m_IsDone = false;
    public bool m_IsAbort = false;
    public uint m_flag = 0u;
    public uint m_waterMark = 0u;
}


