using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class AssetManager
{
    static public Object LoadAsset(string strResPath, System.Type type, bool usePool,uint flag, bool mustExist = false)
    {
        if (usePool)
        {
            return CGameObjectPool.instance.GetGameObject(strResPath, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
        }
        else
        {
            return AssetLoader.instance.LoadRes(strResPath, type, mustExist).obj;
        }
    }

    /// <summary>
    /// 加载游戏运行是对象(该过程包含实例化)
    /// </summary>
    /// <param name="strResPath"></param>
    /// <returns></returns>
    static public uint LoadAssetRequest(string strResPath, System.Type type,bool usePool,uint flag,bool mustExist = false,uint waterMark = 0x0)
    {
        if(usePool)
        {
            return CGameObjectPool.instance.GetGameObjectAsync(strResPath, enResourceType.BattleScene, flag,waterMark);
        }
        else
        {
            return AssetLoader.instance.LoadResAync(strResPath,type,mustExist, flag,waterMark);
        }
    }

    static public void RecycleAsset(Object obj)
    {
        GameObject go = obj as GameObject;
        if (null != go)
            CGameObjectPool.instance.RecycleGameObject(go);
    }

    static public bool IsRequestDone(uint handle)
    {
        uint typeReq = (handle >> 30) & 0x3;
        switch(typeReq)
        {
            case 0:
                return AssetLoader.instance.IsRequestDone(handle);
            case 1:
                return CGameObjectPool.instance.IsRequestDone(handle);
            default:
                Logger.LogErrorFormat("Handle type [{0}] is invalid!", typeReq);
                break;
        }
        return true;
    }

    static public Object ExtractAsset(uint handle)
    {
        uint typeReq = (handle >> 30) & 0x3;
        switch (typeReq)
        {
            case 0:
                return AssetLoader.instance.Extract(handle).obj;
            case 1:
                return CGameObjectPool.instance.ExtractAsset(handle);
            default:
                Logger.LogErrorFormat("Handle type [{0}] is invalid!", typeReq);
                break;
        }
        return null;
    }

    static public void AbortRequest(uint handle)
    {
        uint typeReq = (handle >> 30) & 0x3;
        switch (typeReq)
        {
            case 0:
                AssetLoader.instance.AbortRequest(handle);
                return;
            case 1:
                CGameObjectPool.instance.AbortRequest(handle);
                return;
            default:
                Logger.LogErrorFormat("Handle type [{0}] is invalid!", typeReq);
                return;
        }
    }

    static public bool IsValidHandle(uint handle)
    {
        uint typeReq = (handle >> 30) & 0x3;
        switch (typeReq)
        {
            case 0:
                return AssetLoader.instance.IsValidHandle(handle);
            case 1:
                return CGameObjectPool.instance.IsValidHandle(handle);
            default:
                Logger.LogErrorFormat("Handle type [{0}] is invalid!", typeReq);
                return false;
        }
    }
}



