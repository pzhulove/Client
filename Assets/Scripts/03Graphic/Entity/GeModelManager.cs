using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;



/// <summary>
/// 临时引用的程序集
/// </summary>
/// 
using GameClient;

#if LOGIC_SERVER

public class GeModelManager
{
	public GeModelManager(GeActorEx actor, int resIDOrigin, GameObject objOrigi){}
	public bool PreChangeModel(int resID, bool changeWhenLoadOk = false){return false;}
	public void TryChangeModel(int resID){}
	public void Update(){}
	public void RmoveModel(GameObject go){}
	public void Clear(){}
}

#else 

// 多段变身模型加载管理
public class GeModelManager
{
    public enum LoadState
    {
        None,
        Ing,
        Done
    }

    class LoadTask
    {
        public int resID;
        public PostLoadCommand cb;
        public LoadState state; // 0:normal 1:加载中 -1:加载结束
        public uint asyncRequest = ~0u;
        public bool changeWhenLoadOk = false;
        public string modelPath = null;
        public GameObject m_LoadGameObject = null;
        public LoadTask(int resIDIn, PostLoadCommand cbIn = null, bool changeWhenLoadOkIn = false)
        {
            resID = resIDIn;
            cb = cbIn;
            state = LoadState.None;
            changeWhenLoadOk = changeWhenLoadOkIn;

            //
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
            if (data == null)
                return;
            modelPath = data.ModelPath;
        }
    }

    class modelInfo
    {
        public int resID = 0;
        public GameObject go = null;
    }

    bool m_IsStarted = false;

    GeActorEx m_Actor = null;

    // 加载好的
    modelInfo m_OriginModelInfo = new modelInfo(); // TODO::人物的处理
    List<modelInfo> m_ExtraModelInfoList = new List<modelInfo>();

    // 
    List<LoadTask> m_TaskList = new List<LoadTask>();

    public GeModelManager(GeActorEx actor, int resIDOrigin, GameObject objOrigi)
    {
        if (actor == null)
            return;

        m_Actor = actor;
        m_OriginModelInfo.resID = resIDOrigin;
        m_OriginModelInfo.go = objOrigi;

        Start();
    }
    void Start()
    {
        // 开始加载第一个
        m_IsStarted = true;

        //// 测试
        ////m_ExtraModels[1] = CGameObjectPool.instance.GetGameObject(m_ExtraShowPaths[1], enResourceType.BattleScene, true);
        //PreChangeModel(1);
    }

    public bool PreChangeModel(int resID, bool changeWhenLoadOk = false)
    {
        modelInfo t = GetModelLoaded(resID);
        if (t != null)
            return false;

        LoadTask lt = GetInTaskList(resID);
        if (lt != null)
            return false;

        SetInTask(resID, changeWhenLoadOk);
        return true;
    }

    public void TryChangeModel(int resID)
    {
        // 直接替换
        modelInfo mi = GetModelLoaded(resID);
        if (mi != null)
        {
            m_Actor.ChangeModel(mi.go);
            return;
        }

        // 在Task中，设置变身延迟处理
        LoadTask lt = GetInTaskList(resID);
        if (lt != null)
        {
            lt.changeWhenLoadOk = true;
            return;
        }

        SetInTask(resID, true);
    }

    modelInfo GetModelLoaded(int resID)
    {
        int count = m_ExtraModelInfoList.Count;
        if (count < 1)
            return null;

        modelInfo t = null;
        for (int i = 0; i < count; i++)
        {
            t = m_ExtraModelInfoList[i];
            if (t.resID == resID)
                return t;
        }

        return null;
    }

    LoadTask GetInTaskList(int resID)
    {
        int count = m_TaskList.Count;
        LoadTask curTask = null;
        for (int i = 0; i < count; i++)
        {
            curTask = m_TaskList[i];
            if (curTask.resID == resID)
                return curTask;
        }

        return null;
    }

    void SetInTask(int resID, bool changeWhenLoadOk = false)
    {
        LoadTask t = new LoadTask(resID, null, changeWhenLoadOk);
        m_TaskList.Add(t);
    }

    public void Update()
    {
        if (!m_IsStarted)
            return;

        int count = m_TaskList.Count;
        if (count < 1)
            return;

        LoadTask curTask = m_TaskList[0];
        LoadState curState = curTask.state;
        int resID = curTask.resID;
        string modelPath = curTask.modelPath;

        switch (curState)
        {
            case LoadState.None:
                {
                    if (modelPath == null)
                        curTask.state = LoadState.Done;
                    else
                    {
                        if(EngineConfig.useTMEngine)
                        {
                            curTask.asyncRequest = CGameObjectPool.GetGameObjectAsync(modelPath,m_AssetLoadCallbacks,this, (uint)GameObjectPoolFlag.None, 0xacdeee32);
                        }
                        else
                        {
                            curTask.asyncRequest = CGameObjectPool.instance.GetGameObjectAsync(modelPath, enResourceType.BattleScene, (uint)GameObjectPoolFlag.None, 0xacdeee32);
                        }
                        curTask.state = LoadState.Ing;
                    }
                    break;
                }
            case LoadState.Ing:
                {
                    if(EngineConfig.useTMEngine)
                    {
                        if (null != curTask.m_LoadGameObject)
                        {
                            curTask.state = LoadState.Done;
                            GameObject go = curTask.m_LoadGameObject;
                            curTask.m_LoadGameObject = null;
                            if (go != null)
                            {
                                modelInfo t = new modelInfo();
                                t.resID = resID;
                                t.go = go;
                                go.SetActive(false);
                                if (curTask.changeWhenLoadOk)// 到这里说明已经滞后了
                                {
                                    m_Actor.ChangeModel(go);
                                    Logger.LogProcessFormat("[GeModelManager] 异步加载滞后了 {0}", modelPath);
                                }
                            }
                        }
                    }
                    else
                    {
                        uint asyncRequest = curTask.asyncRequest;
                        if (~0u != asyncRequest)
                        {
                            if (CGameObjectPool.instance.IsRequestDone(asyncRequest))
                            {
                                curTask.state = LoadState.Done;
                                GameObject go = CGameObjectPool.instance.ExtractAsset(asyncRequest) as GameObject;
                                if (go != null)
                                {
                                    modelInfo t = new modelInfo();
                                    t.resID = resID;
                                    t.go = go;
                                    go.SetActive(false);
                                    if (curTask.changeWhenLoadOk)// 到这里说明已经滞后了
                                    {
                                        m_Actor.ChangeModel(go);
                                        Logger.LogProcessFormat("[GeModelManager] 异步加载滞后了 {0}", modelPath);
                                    }
                                }
                            }
                        }
                        else// 错误的资源，停止
                            curTask.state = LoadState.Done;
                    }

                    break;
                }
            case LoadState.Done:
                {
                    m_TaskList.RemoveAt(0);
                    break;
                }
            default: break;
        }
    }

    public void RmoveModel(GameObject go)
    {
        //if (go == null)
        //    return;

        //GameObject obj = null;
        //for (int i = 0; i < m_Max; i++)
        //{
        //    obj = m_ExtraModels[i];
        //    if (obj != null && obj == go)
        //    {
        //        m_ExtraModels[i] = null;
        //        CGameObjectPool.instance.RecycleGameObject(go);
        //        break;
        //    }
        //}
    }

    public void Clear()
    {
        if (!m_IsStarted)
            return;

        // 
        //m_IsStarted = false;
        //m_Actor = null;
        //m_ModelIdx = 0;
        //m_ExtraShowPaths.Clear();

        //// 清理全部模型
        //GameObject obj = null;
        //for (int i = 0; i < m_Max; i++)
        //{
        //    obj = m_ExtraModels[i];
        //    m_ExtraModels[i] = null;
        //    CGameObjectPool.instance.RecycleGameObject(obj);
        //}
        //m_Max = 0;

        ////
        for (int i = 0; i < m_TaskList.Count; i++)
        {
            LoadTask curTask = m_TaskList[i];
            LoadState curState = curTask.state;
            if (curState == LoadState.Ing)
            {
                if (~0u != curTask.asyncRequest)
                {
                    CGameObjectPool.instance.AbortRequest(curTask.asyncRequest);
                    curTask.asyncRequest = ~0u;
                }
            }
            if (curTask.m_LoadGameObject != null)
            {
                CGameObjectPool.instance.RecycleGameObject(curTask.m_LoadGameObject);
                curTask.m_LoadGameObject = null;
            }
        }
        m_TaskList.Clear();
    }

    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
    {
        if (null != asset)
        {
            GameObject go = asset as GameObject;
            if (null != go)
            {
                // todo: 修改显隐
                go.SetActive(true);
                if (null != userData)
                {
                    GeModelManager _this = userData as GeModelManager;
                    if (null != _this)
                    {
                        for (int i = 0, icnt = _this.m_TaskList.Count; i < icnt; ++i)
                        {
                            LoadTask cur = _this.m_TaskList[i];
                            if (cur.asyncRequest == (uint)grpID)
                            {
                                cur.m_LoadGameObject = go;
                                cur.asyncRequest = ~0u;
                                return;
                            }
                        }
                    }
                    else
                        Tenmove.Runtime.Debugger.LogError("User data type '{0}' is NOT GeModelManager!");
                }
                else
                    Tenmove.Runtime.Debugger.LogError("User data can not be null!");

                CGameObjectPool.RecycleGameObjectEx(go);
            }
            else
                Tenmove.Runtime.Debugger.LogError("Asset '{0}' is nil or type '{1}' error!", assetPath, asset.GetType());
        }
        else
            Tenmove.Runtime.Debugger.LogError("Asset '{0}' load error!", assetPath);
    }

    static void _OnLoadAssetFailure(string assetPath, int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
        GeModelManager _this = userData as GeModelManager;
        if (null == _this)
        {
            Tenmove.Runtime.Debugger.LogError("User data type '{0}' is NOT GeModelManager!");
            return;
        }

        if(null != _this.m_TaskList && _this.m_TaskList.Count > 0)
        {
            _this.m_TaskList[0].state = LoadState.Done;
        }
    }

}
#endif