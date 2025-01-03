//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

//using GameClient;

//public class AsyncLoadTaskManager : Singleton<AsyncLoadTaskManager>
//{
//    private List<AsyncLoadTask> mAsyncTasks = new List<AsyncLoadTask>();
    
//    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

//    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
//    {
//        AsyncLoadTaskManager _this = userData as AsyncLoadTaskManager;
//        if(null != _this)
//        {
//            GameObject go = asset as GameObject;

//            for (int i = 0, icnt = _this.mAsyncTasks.Count; i < icnt; ++i)
//            {
//                AsyncLoadTask curTask = _this.mAsyncTasks[i];
//                if ((uint)grpID == curTask.handle)
//                {
//                    curTask.OnObjectLoaded(go);
//                    return;
//                }
//            }
//            CGameObjectPool.RecycleGameObjectEx(go);
//        }
//    }

//  static void _OnLoadAssetFailure(string assetPath,int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
//  {
//  }

//    public void ClearAllAsyncTasks()
//    {
//        Logger.LogProcessFormat("[AsyncLoadTask] clear all task {0}", mAsyncTasks.Count);

//        for (int i = 0; i < mAsyncTasks.Count; ++i)
//        {
//            _abortAsyncTask(mAsyncTasks[i]);
//        }

//        mAsyncTasks.Clear();
//    }

//    public void RemoveAsyncLoadGameObjectByHandle(uint handle)
//    {
//        _removeAsyncLoadTask(handle);
//    }

//    public uint AddAsyncLoadGameObject(string tag, string path, PostLoadGameObject load, uint condition = uint.MaxValue)
//    {
//        return _addAsyncLoadGameObject(tag, path, load, condition);
//    }

//    public uint AddAsyncLoadGameObject(string tag, string path, enResourceType restype, bool reserveLast, PostLoadGameObject load, uint condition = uint.MaxValue)
//    {
//        return _addPooledAsyncLoadGameObject(tag, path, restype, reserveLast, load, condition);
//    }

//    #region Normal
//    private uint _addAsyncLoadGameObject(string tag, string path, PostLoadGameObject load, uint condition)
//    {
//        if (EngineConfig.useTMEngine)
//        {
//            uint handle = AssetLoader.LoadResAsGameObjectAsync(path, m_AssetLoadCallbacks,this);
//            _addAsyncLoadTask(tag, handle, path, condition, load, false);
//            return handle;
//        }
//        else
//        {
//            uint handle = AssetLoader.instance.LoadResAsyncAsGameObject(path, true, (uint)AssetLoadFlag.HideAfterLoad);
//            if (!AssetLoader.instance.IsValidHandle(handle))
//            {
//                Logger.LogErrorFormat("[AsyncLoadTask] add fail with {0}:{1}", tag, path);
//                return uint.MaxValue;
//            }

//            _addAsyncLoadTask(tag, handle, path, condition, load, false);
//            return handle;
//        }
//    }

//    #endregion

//    #region Pooled
//    private uint _addPooledAsyncLoadGameObject(string tag, string path, enResourceType restype, bool reserveLast, PostLoadGameObject load, uint condition)
//    {
//        if (EngineConfig.useTMEngine)
//        {
//            //uint handle = AssetLoader.LoadResAsGameObjectAsync(path, m_AssetLoadCallbacks, this);
//            uint handle = CGameObjectPool.GetGameObjectAsync(path, m_AssetLoadCallbacks, this);
//            _addAsyncLoadTask(tag, handle, path, condition, load, false);
//            return handle;
//        }
//        else
//        {
//            uint handle = CGameObjectPool.instance.GetGameObjectAsync(path, restype, reserveLast ? ((uint)GameObjectPoolFlag.ReserveLast | (uint)GameObjectPoolFlag.HideAfterLoad) : (uint)GameObjectPoolFlag.HideAfterLoad, 0x322d2312);

//            if (!CGameObjectPool.instance.IsValidHandle(handle))
//            {
//                Logger.LogErrorFormat("[AsyncLoadTask] add fail with {0}:{1}", tag, path);
//                return uint.MaxValue;
//            }

//            _addAsyncLoadTask(tag, handle, path, condition, load, true);
//            return handle;
//        }
//    }
//    #endregion

//    public void Update(float delta)
//    {
//        for (int i = 0; i < mAsyncTasks.Count; ++i)
//        {
//            switch (mAsyncTasks[i].status)
//            {
//                case eAsyncLoadTaskStatus.onNone:
//                    mAsyncTasks[i].status = eAsyncLoadTaskStatus.onLoading;
//                    Logger.LogProcessFormat("[AsyncLoadTask] status is loading {0}, {1}", mAsyncTasks[i].handle, mAsyncTasks[i].path);
//                    break;
//                case eAsyncLoadTaskStatus.onLoading:
//                    if (_isRequestDone(mAsyncTasks[i]))
//                    {
//                        mAsyncTasks[i].status = eAsyncLoadTaskStatus.onCondition;
//                        Logger.LogProcessFormat("[AsyncLoadTask] status is condition {0} {1}", mAsyncTasks[i].handle, mAsyncTasks[i].path);
//                    }
//                    break;
//                case eAsyncLoadTaskStatus.onCondition:
//                    if (_isTaskCondition(mAsyncTasks[i]))
//                    {
//                        Logger.LogProcessFormat("[AsyncLoadTask] status is finish {0} {1}", mAsyncTasks[i].handle, mAsyncTasks[i].path);
//                        mAsyncTasks[i].status = eAsyncLoadTaskStatus.onPostCall;
//                    }
//                    break;
//                case eAsyncLoadTaskStatus.onPostCall:
//                    _onPostLoad(mAsyncTasks[i]);
//                    mAsyncTasks[i].status = eAsyncLoadTaskStatus.onFinish;
//                    break;
//                case eAsyncLoadTaskStatus.onFinish:
//                    break;
//            }
//        }
//    }

//    private bool _removeAllFinishTask(AsyncLoadTask task)
//    {
//        if (null == task)
//        {
//            return true;
//        }

//        if (task.isFinish)
//        {
//            _removeAsyncLoadTask(task);
//            return true;
//        }

//        return false;
//    }

//    private bool _isTaskCondition(AsyncLoadTask task)
//    {
//        if (null == task)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] task is nil");
//            return true;
//        }

//        if (uint.MaxValue == task.waithandle)
//        {
//            return true;
//        }

//        AsyncLoadTask waittask = _findTask(task.waithandle);


//        if (null == waittask)
//        {
//            return true;
//        }

//        return waittask.isFinish;
//    }

//    private AsyncLoadTask _findTask(uint handle)
//    {
//        for (int i = 0; i < mAsyncTasks.Count; ++i)
//        {
//            if (mAsyncTasks[i].handle == handle )
//            {
//                return mAsyncTasks[i];
//            }
//        }

//        return null;
//    }

//    private bool _isRequestDone(AsyncLoadTask task)
//    {
//        if (EngineConfig.useTMEngine)
//        {
//            return task.IsObjectLoaded();
//        }
//        else
//        {
//            if (null == task)
//            {
//                Logger.LogProcessFormat("[AsyncLoadTask] task is nil");
//                return false;
//            }

//            if (task.isPooled)
//            {
//                return CGameObjectPool.instance.IsRequestDone(task.handle);
//            }
//            else
//            {
//                return AssetLoader.instance.IsRequestDone(task.handle);
//            }
//        }
//    }

//    private void _onPostLoad(AsyncLoadTask task)
//    {
//        if (null == task)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] task is nil");
//            return;
//        }

//        if (null != task.load)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] call post load {0} {1}", task.handle, task.path);
//            task.load(_extraGameObjectByHandle(task));
//        }
//    }

//    private GameObject _extraGameObjectByHandle(AsyncLoadTask task)
//    {
//        if (EngineConfig.useTMEngine)
//        {
//            return task.ExtractObject();
//        }
//        else
//        {
//            if (null == task)
//            {
//                Logger.LogProcessFormat("[AsyncLoadTask] task is nil");
//                return null;
//            }

//            if (task.isPooled)
//            {
//                Logger.LogProcessFormat("[AsyncLoadTask] get pool gameobject {0} {1}", task.handle, task.path);
//                return CGameObjectPool.instance.ExtractAsset(task.handle) as GameObject;
//            }
//            else
//            {
//                Logger.LogProcessFormat("[AsyncLoadTask] get normal gameobject {0} {1}", task.handle, task.path);
//                return AssetLoader.instance.Extract(task.handle).obj as GameObject;
//            }
//        }
//    }
    
//    private void _addAsyncLoadTask(string tag, uint handle, string path, uint condition, PostLoadGameObject load, bool isPooled)
//    {
//        AsyncLoadTask task = new AsyncLoadTask(tag, handle, path, condition, load, isPooled);
//        mAsyncTasks.Add(task);

//        Logger.LogProcessFormat("[AsyncLoadTask] add task ID {0}, count left {1}, {2}", task.handle, mAsyncTasks.Count, task.path);
//    }

//    private void _removeAsyncLoadTask(AsyncLoadTask task)
//    {
//        if (null == task)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] task is nil");
//            return;
//        }

//        if (EngineConfig.useTMEngine)
//        {
//            GameObject go = task.ExtractObject();
//            if(null != go)
//            {
//                if (task.isPooled)
//                    CGameObjectPool.RecycleGameObjectEx(go);
//                else
//                    GameObject.Destroy(go);
//            }

//        }
//        else
//            _abortAsyncTask(task);

//        mAsyncTasks.Remove(task);

//        Logger.LogProcessFormat("[AsyncLoadTask] remove task {0}, count left {1}", task.handle, mAsyncTasks.Count);
//    }

//    private void _removeAsyncLoadTask(uint handle)
//    {
//        AsyncLoadTask task = _findTask(handle);

//        if (null == task)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] task is nil");
//            return;
//        }

//        _removeAsyncLoadTask(task);
//    }

//    private void _abortAsyncTask(AsyncLoadTask task)
//    {
//        if (null == task)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] task is nil");
//            return ;
//        }

//        if (task.isFinish)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] finish task {0}, {1}", task.handle, task.path);
//            return;
//        }

//        if (task.isPooled)
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] finish task with pool abort {0}, {1}", task.handle, task.path);
//            CGameObjectPool.instance.AbortRequest(task.handle);
//        }
//        else
//        {
//            Logger.LogProcessFormat("[AsyncLoadTask] finish task with normal abort {0}, {1}", task.handle, task.path);
//            AssetLoader.instance.AbortRequest(task.handle);
//        }
//    }
//}
