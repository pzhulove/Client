using System;
using System.Collections.Generic;
using System.IO;
using Tenmove.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    /// <summary>
    /// UI的对象池管理
    /// </summary>
    public class UIObjectPoolManager
    {
        protected class LoadParam
        {
            public object UserData;
            public UnityAction<GameObject, string, object> CallBack;
            public GameObject Parent;
            public bool WorldPositionStays;
        }

        protected Dictionary<string, Stack<GameObject>> mObjectsDic = new Dictionary<string, Stack<GameObject>>();
        protected Dictionary<string, HashSet<LoadParam>> mLoadingDic = new Dictionary<string, HashSet<LoadParam>>();
        protected Transform mRoot;
        protected Stack<LoadParam> mLoadParamCache = new Stack<LoadParam>();

        protected Dictionary<string, GameObject> mPredefineAnimation = new Dictionary<string, GameObject>();
        protected AssetLoadCallbacks<object> mPredefineAnimationCallBacks;
        protected AssetLoadCallbacks<object> mLoadObjectCallBacks;

        public void Initialize(Transform root)
        {
            mRoot = root;
            if (mPredefineAnimationCallBacks == null)
            {
                mPredefineAnimationCallBacks = new AssetLoadCallbacks<object>(OnPredefineAnimationLoadSuccess, OnPredefineAnimationLoadFailure);
            }
            if (mLoadObjectCallBacks == null)
            {
                mLoadObjectCallBacks = new AssetLoadCallbacks<object>(OnAssetLoadSuccess, OnAssetLoadFailure);
            }
            LoadPredeineAnimations();
        }

        public void LoadPredeineAnimations()
        {
            if (mRoot != null)
            {
                UIPredefineAnimations predefineAnimation = mRoot.GetComponent<UIPredefineAnimations>();
                if (predefineAnimation != null)
                {
                    foreach (var assetName in predefineAnimation.m_PredefineAnimations)
                    {
                        AssetLoader.LoadResAsync(assetName.m_Res, typeof(GameObject), mPredefineAnimationCallBacks, this);
                    }
                }
            }
        }

        void OnPredefineAnimationLoadSuccess(string path, object asset, int taskGrpID, float duration, object userData)
        {
            GameObject prefabInstance = asset as GameObject;
            // todo: 修改显隐
            prefabInstance.SetActive(true);

            string fileName = Path.GetFileNameWithoutExtension(path);
            mPredefineAnimation.Add(fileName, prefabInstance);
            // prefabInstance.transform.parent = mRoot;
            prefabInstance.transform.SetParent(mRoot);
        }

        void OnPredefineAnimationLoadFailure(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
        {
        }


        public GameObject GetPredefinedAnimationObject(string animationName)
        {
            GameObject obj = null;
//编辑器下每次重新加载，这样便于修改测试
#if UNITY_EDITOR
            foreach(var anim in mPredefineAnimation.Values)
            {
                GameObject.Destroy(anim);
            }
            mPredefineAnimation.Clear();
#endif
            if (!mPredefineAnimation.TryGetValue(animationName, out obj))
            {
                string filePath = "UIFlatten/Prefabs/Animation/" + animationName + ".prefab";
                obj = AssetLoader.GetInstance().LoadResAsGameObject(filePath) as GameObject;
                OnPredefineAnimationLoadSuccess(filePath, obj, 0, 0, null);

                mPredefineAnimation.TryGetValue(animationName, out obj);
            }

            return obj;
        }

        public void Clear()
        {
            foreach (var data in mLoadingDic.Values)
            {
                var enumerator = data.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.CallBack = null;
                }
                data.Clear();
            }
            mLoadingDic.Clear();

            foreach (var poolList in mObjectsDic.Values)
            {
                while(poolList.Count > 0)
                {
                    var go = poolList.Pop();
                    GameObject.Destroy(go);
                }
                poolList.Clear();
            }
            mObjectsDic.Clear();
        }

        public void PreloadObjects(string path, int count)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.LogWarning("Preload object path is null or empty");
                return;
            }
            if (mObjectsDic.ContainsKey(path))
            {
                count -= mObjectsDic[path].Count;
            }

            
            if (count > 0)
            {
                AssetLoader.LoadResAsync(path, typeof(GameObject), mLoadObjectCallBacks, count);
            }
        }

        public void GetObject(string path, UnityAction<GameObject, string, object> callBack, object userData, GameObject parent, bool worldPositionStays)
        {
            Logger.LogProcessFormat("UIObjectPoolManager GetObject Path:{0}", path);
            if (string.IsNullOrEmpty(path))
            {
                try
                {
                    callBack(null, path, userData);
                }
                catch (Exception e)
                {
                    Logger.LogWarningFormat("UIObjectPoolManager GetObject Call Back Error. Path is null, Message {0}{1}", e.Message, e.StackTrace);
                }
                return;
            }

            if (!mObjectsDic.ContainsKey(path) || mObjectsDic[path].Count == 0)
            {
                LoadParam param;
                if (mLoadParamCache.Count <= 0)
                {
                    param = new LoadParam();
                }
                else
                {
                    param = mLoadParamCache.Pop();
                }
                param.UserData = userData;
                param.CallBack = callBack;
                param.Parent = parent;
                param.WorldPositionStays = worldPositionStays;
                if (mLoadingDic.ContainsKey(path))
                {
                    mLoadingDic[path].Add(param);
                }
                else
                {
                    mLoadingDic.Add(path, new HashSet<LoadParam>());
                    mLoadingDic[path].Add(param);
                    uint taskId = AssetLoader.LoadResAsync(path, typeof(GameObject), mLoadObjectCallBacks, mLoadingDic[path]);
                    Logger.LogProcessFormat("UIObjectPoolManager 加载预制体 路径:{0} taskId {1}", path, taskId);
                }
            }
            else
            {
                var go = mObjectsDic[path].Pop();
                if (parent != null)
                    go.transform.SetParent(parent.transform, false);
                try
                {
                   callBack(go, path, userData);
                }
                catch (Exception e)
                {
                    Logger.LogWarningFormat("UIObjectPoolManager GetObject 回调异常. 预制体路径 {0}, 报错信息:{1}{2}", path, e.Message, e.StackTrace);
                }
            }
        }

        public void ReturnObject(GameObject obj)
        {
            var script = obj.GetComponent<UIPoolObject>();
            if (script == null)
                return;
            if (!mRoot || string.IsNullOrEmpty(script.Path))
            {
                GameObject.Destroy(obj);
                return;
            }

            if (!obj)
                return;
            Logger.LogProcessFormat("UIObjectPoolManager 归还预制体 路径:{0} obj name: {1}", script.Path, script.name);
            if (!mObjectsDic.ContainsKey(script.Path))
                mObjectsDic.Add(script.Path, new Stack<GameObject>());
            mObjectsDic[script.Path].Push(obj);
            if (mRoot && mRoot.gameObject)
            {
                obj.transform.SetParent(mRoot, false);
            }
            else
            {
                GameObject.Destroy(obj);
            }
        }

        protected void OnAssetLoadSuccess(string path, object asset, int taskGrpID, float duration, object userData)
        {
            Logger.LogProcessFormat("UIObjectPoolManager Load asset success Path:{0} task {1}", path, taskGrpID);

            if (!mObjectsDic.ContainsKey(path))
            {
                mObjectsDic.Add(path, new Stack<GameObject>());
            }
            GameObject go = asset as GameObject;
            // todo: 修改显隐
            go.SetActive(true);
            if (userData is int)
            {
                int count = (int)userData;
                mObjectsDic[path].Push(go);
                go.transform.SetParent(mRoot, false);
                var script = go.GetOrAddComponent<UIPoolObject>();
                script.Path = path;
                for (int i = 1; i < count; ++i)
                {
                    var obj = GameObject.Instantiate(go, mRoot, false);
                    mObjectsDic[path].Push(obj);
                    script = obj.GetOrAddComponent<UIPoolObject>();
                    script.Path = path;
                }

            }
            else if (userData is HashSet<LoadParam>)
            {
                var dataSet = userData as HashSet<LoadParam>;
                var enumerator = dataSet.GetEnumerator();
                var script = go.GetOrAddComponent<UIPoolObject>();
                script.Path = path;
                bool isFirst = true;
                Queue<GameObject> objs = new Queue<GameObject>();
                objs.Enqueue(go);
                //先复制 保证复制出来的是干净的
                while (enumerator.MoveNext())
                {
                    var param = enumerator.Current;

                    if (!isFirst)
                    {
                        objs.Enqueue(InstantiatePoolObject(go, param.Parent, param.WorldPositionStays));
                    }
                    else
                    {
                        isFirst = false;
                    }
                }
                //再callBack
                enumerator = dataSet.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var param = enumerator.Current;
                    var obj = objs.Dequeue();


                    if (param.CallBack != null)
                    {
                        param.CallBack(obj, path, param.UserData);
                    }

                    if (!mLoadParamCache.Contains(param))
                        mLoadParamCache.Push(param);
                }

                if (mLoadingDic.ContainsKey(path))
                {
                    mLoadingDic.Remove(path);
                }
                dataSet.Clear();
            }
            else
            {
                GameObject.Destroy(go);
            }
        }

        protected void OnAssetLoadFailure(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
        {
            Logger.LogProcessFormat("UIObjectPoolManager Load asset failed Path:{0}", path);

            if (userData is LoadParam)
            {
                var param = userData as LoadParam;
                if (param.CallBack != null)
                    param.CallBack(null, path, param.UserData);

                if (mLoadingDic.ContainsKey(path))
                {
                    mLoadingDic.Remove(path);
                }

                if (!mLoadParamCache.Contains(param))
                    mLoadParamCache.Push(param);
            }
        }

        protected GameObject InstantiatePoolObject(GameObject template, GameObject parent, bool worldPositionStays)
        {
            GameObject go = null;
            if (parent != null)
                go = GameObject.Instantiate(template, parent.transform, worldPositionStays);
            else
                go = GameObject.Instantiate(template);
            var script1 = template.GetOrAddComponent<UIPoolObject>();
            var script2 = go.GetOrAddComponent<UIPoolObject>();
            script2.Path = script1.Path;
            return go;
        }

    }

}