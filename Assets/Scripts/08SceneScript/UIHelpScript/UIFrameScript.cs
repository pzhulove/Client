using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

namespace GameClient
{
    public class ScriptPool
    {
        static public Dictionary<Int32, List<GameObject>> poolObjects = new Dictionary<int, List<GameObject>>();

        public static void RecyclePrefab(GameObject go,string path)
        {
            GameObject root = GameObject.Find("TemplateScriptPool");
            if (root == null)
            {
                ClearAll();
                root = new GameObject("TemplateScriptPool");
                GameObject.DontDestroyOnLoad(root);
            }

            Int32 key = path.GetHashCode();
            List<GameObject> cachedObjects = null;

            if (!poolObjects.TryGetValue(key, out cachedObjects))
            {
                cachedObjects = new List<GameObject>();
                poolObjects.Add(key, cachedObjects);
            }

            cachedObjects.Add(go);
            Utility.AttachTo(go, root);
            go.SetActive(false);
        }

        public static void ClearAll()
        {
            Dictionary<Int32, List<GameObject>>.Enumerator it = poolObjects.GetEnumerator();
            while(it.MoveNext())
            {
                KeyValuePair<Int32, List<GameObject>> cur = it.Current;
                List<GameObject> lst = cur.Value;

                if(null == lst)
                    continue;

                for(int i = 0,icnt = lst.Count;i<icnt;++i)
                {
                    if (null != lst[i])
                        GameObject.Destroy(lst[i]);
                }
            }

            poolObjects.Clear();
        }

        public static GameObject CreatePrefab(string path,bool bUsedCopyComponent)
        {
            if(path != null && path.Length > 0)
            {
                GameObject root = GameObject.Find("TemplateScriptPool");
                if(root == null)
                {
                    poolObjects.Clear();
                }
                if (root == null)
                {
                    root = new GameObject("TemplateScriptPool");
                    GameObject.DontDestroyOnLoad(root);
                }

                Int32 key = path.GetHashCode();
                GameObject goPrefab = null;
                List<GameObject> cachedObjects = null;
                if (bUsedCopyComponent)
                {
                    if (!poolObjects.TryGetValue(key, out cachedObjects))
                    {
                        cachedObjects = new List<GameObject>();
                        poolObjects.Add(key, cachedObjects);
                    }

                    if(cachedObjects.Count <= 0)
                    {
                        goPrefab = AssetLoader.instance.LoadResAsGameObject(path);
                        if (goPrefab != null)
                        {
                            cachedObjects.Add(goPrefab);
                            Utility.AttachTo(goPrefab, root);
                            goPrefab.SetActive(false);
                        }
                    }
                    else
                    {
                        goPrefab = cachedObjects[0];
                    }
                }
                else
                {
                    if (!poolObjects.TryGetValue(key, out cachedObjects) || cachedObjects.Count <= 0)
                    {
                        goPrefab = AssetLoader.instance.LoadResAsGameObject(path);
                    }
                    else
                    {
                        goPrefab = cachedObjects[0];
                        cachedObjects.RemoveAt(0);
                    }
                }
                return goPrefab;
            }

            return null;
        }
    }

    // 1.5项目非全屏界面打开的弹出效果不再使用该组件，统一使用2.0的功能
    public class UIFrameScript : MonoBehaviour
    {
        public static string ms_effect_root = "UIFlatten/Prefabs/CommEffect/";
        public enum FunctionType
        {
            FT_FADEIN = 0,
            FT_FADEOUT,
            FT_ADDBACK,
            FT_REMOVEBACK,
            FT_COUNT,
        }

        [System.Serializable]
        public class FunctionBase
        {
            #region Declare
            protected bool bHasCreate;
            protected GameObject goPrefab;

            protected FunctionType eFunctionType;
            public FunctionType Function
            {
                get{ return eFunctionType; }
            }
            public string prefabs;

            public FunctionBase(FunctionType eFunctionType)
            {
                this.eFunctionType = eFunctionType;
            }
            #endregion

            public void Initialize()
            {
                bHasCreate = false;
                goPrefab = null;
                comTweens.Clear();
                comAnimations.Clear();
                fTotalCompleteTime = 0.0f;
            }

            #region calculateAssign
            List<DOTweenAnimation> comTweens = new List<DOTweenAnimation>();
            List<AnimationController> comAnimations = new List<AnimationController>();
            float fTotalCompleteTime;
            #endregion

            public virtual float DoPlay(GameObject go)
            {
                if(!bHasCreate)
                {
                    Create(go);
                }

                fTotalCompleteTime = 0.0f;

                for (int i = 0; i < comTweens.Count; ++i)
                {
                    if (comTweens[i].onComplete == null)
                    {
                        comTweens[i].onComplete = new UnityEngine.Events.UnityEvent();
                    }
                    comTweens[i].onComplete.RemoveAllListeners();
                    comTweens[i].DOPlay();

                    fTotalCompleteTime = Mathf.Max(comTweens[i].delay + comTweens[i].duration * comTweens[i].loops, fTotalCompleteTime);
                }

                for(int i = 0; i < comAnimations.Count; ++i)
                {
                    if(comAnimations[i] != null)
                    {
                        comAnimations[i].DoPlay(null);
                        fTotalCompleteTime = Mathf.Max(fTotalCompleteTime, comAnimations[i].GetTotalRunTime());
                    }
                }

                return fTotalCompleteTime;
            }

            protected virtual void Create(GameObject go)
            {
                if(!bHasCreate)
                {
                    comTweens.Clear();
                    comAnimations.Clear();

                    if (prefabs != null)
                    {
                        var tokens = prefabs.Split('/');
                        if(tokens.Length > 0)
                        {
                            goPrefab = ScriptPool.CreatePrefab(ms_effect_root + tokens[tokens.Length - 1],true);
                        }
                    }

                    if(goPrefab != null && go != null)
                    {
                        Component[] components = goPrefab.GetComponents(typeof(DOTweenAnimation));
                        if (components.Length > 0)
                        {
                            for(int j = 0; j < components.Length; ++j)
                            {
                               var current = Utility.CopyComponent<DOTweenAnimation>(components[j] as DOTweenAnimation, go,true);
                                if(current != null)
                                {
                                    current.target = go.transform;
                                    current.CreateTween();
                                    comTweens.Add(current);
                                }
                            }
                        }

                        components = goPrefab.GetComponents(typeof(AnimationController));
                        if (components.Length > 0)
                        {
                            for (int j = 0; j < components.Length; ++j)
                            {
                                var current = Utility.CopyComponent<AnimationController>(components[j] as AnimationController, go, true);
                                if (current != null)
                                {
                                    current.Initialize();
                                    comAnimations.Add(current);
                                }
                            }
                        }
                    }
                    bHasCreate = true;
                }
            }
        }

        [System.Serializable]
        public class FunctionAddBack : FunctionBase
        {
            public FunctionAddBack():base(FunctionType.FT_ADDBACK)
            {

            }

            protected override void Create(GameObject go)
            {
                if(!bHasCreate)
                {
                    if(go != null && go.transform.parent != null && prefabs != null)
                    {
                        var tokens = prefabs.Split('/');
                        if (tokens.Length > 0)
                        {
                            goPrefab = ScriptPool.CreatePrefab(ms_effect_root + tokens[tokens.Length - 1], false);
                        }
                        if(goPrefab != null)
                        {
                            GameObject goRoot = go.transform.parent.gameObject;
                            GameObject current = goPrefab;
                            current.SetActive(true);
                            current.name = go.name + "parent";
                            Utility.AttachTo(current, goRoot);
                            Utility.AttachTo(go, current);

                            var trigger = current.GetComponent<DoTweenTrigger>();
                            if(trigger != null)
                            {
                                trigger.Initialize();
                            }
                        }
                    }
                    bHasCreate = true;
                }
            }

            public override float DoPlay(GameObject go)
            {
                if(!bHasCreate)
                {
                    Create(go);
                }
                return 0.0f;
            }
        }

        public class FunctionRemoveBack : FunctionBase
        {
            public FunctionAddBack orgAddBack = null;
            public FunctionRemoveBack() : base(FunctionType.FT_REMOVEBACK)
            {
                orgAddBack = null;
            }

            protected override void Create(GameObject go)
            {
                if(!bHasCreate)
                {
                    if (go != null && go.transform.parent != null && go.transform.parent.parent != null)
                    {
                        GameObject goParent = go.transform.parent.gameObject;
                        GameObject goRoot = goParent.transform.parent.gameObject;
                        if(goParent.name == go.name + "parent" && goParent != null && goRoot != null)
                        {
                            Utility.AttachTo(go, goRoot);

                            if(orgAddBack != null && orgAddBack.prefabs != null)
                            {
                                var tokens = orgAddBack.prefabs.Split('/');
                                if(tokens.Length > 0)
                                {
                                    ScriptPool.RecyclePrefab(goParent, ms_effect_root + tokens[tokens.Length - 1]);
                                }
                            }
                            else
                            {
                                goParent.SetActive(false);
                                GameObject.Destroy(goParent);
                            }
                        }
                    }
                    bHasCreate = true;
                }
            }

            public override float DoPlay(GameObject go)
            {
                if (!bHasCreate)
                {
                    Create(go);
                }
                return 0.0f;
            }
        }

        public FunctionBase FadeIn = new FunctionBase(FunctionType.FT_FADEIN);
        public FunctionBase FadeOut = new FunctionBase(FunctionType.FT_FADEOUT);
        public FunctionAddBack OpenFrameBack = new FunctionAddBack();
        FunctionRemoveBack RemoveFrameBack = new FunctionRemoveBack();
        List<FunctionBase> akFunctions = new List<FunctionBase>();

        // Use this for initialization
        public void Initialize()
        {
            akFunctions.Clear();
            akFunctions.Add(FadeIn);
            akFunctions.Add(FadeOut);
            akFunctions.Add(OpenFrameBack);
            akFunctions.Add(RemoveFrameBack);

            RemoveFrameBack.orgAddBack = OpenFrameBack;

            if (akFunctions != null)
            {
                for (int i = 0; i < akFunctions.Count; ++i)
                {
                    akFunctions[i].Initialize();
                }
            }
        }

        public void DoPlay(FunctionType eFunctionType,UnityEngine.Events.UnityAction callback = null)
        {
            if (akFunctions != null)
            {
                for (int i = 0; i < akFunctions.Count; ++i)
                {
                    if (akFunctions[i] != null && akFunctions[i].Function == eFunctionType)
                    {
                        float fDelayTime = akFunctions[i].DoPlay(gameObject);
                        if(callback != null)
                        {
                            InvokeMethod.Invoke(this,fDelayTime, callback);
                        }
                        return;
                    }
                }
            }
        }

        void OnDestroy()
        {
            InvokeMethod.RemoveInvokeCall(this);
        }
    }
}