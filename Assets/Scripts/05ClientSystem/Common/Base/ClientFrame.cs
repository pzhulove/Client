using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    public class ClientFrame : GameBindSystem, IClientFrame, IGameBind
    {
        #region 协程的简单管理  
        List<IEnumerator> m_akActivedCoroutines = new List<IEnumerator>();
        protected IEnumerator StartCoroutine(IEnumerator enumerator)
        {
            if (enumerator != null)
            {
                GameFrameWork.instance.StartCoroutine(enumerator);
                m_akActivedCoroutines.Add(enumerator);
            }
            return enumerator;
        }

        public void StopCoroutine(IEnumerator enumerator)
        {
            GameFrameWork.instance.StopCoroutine(enumerator);
            m_akActivedCoroutines.Remove(enumerator);
        }

        void StopAllCoroutine()
        {
            for (int i = 0; i < m_akActivedCoroutines.Count; ++i)
            {
                GameFrameWork.instance.StopCoroutine(m_akActivedCoroutines[i]);
            }
            m_akActivedCoroutines.Clear();
        }
        #endregion 
        private List<UIEventNew.UIEventHandleNew> m_UIEventHandlers = null;
        public UIEventNew.UIEventHandleNew RegisterUIEvent(EUIEventID type, UIEventNew.UIEventHandleNew.Function function)
        {
            if(m_UIEventHandlers == null)
            {
                m_UIEventHandlers = new List<UIEventNew.UIEventHandleNew>();
            }
            var handler = UIEventManager.GetInstance().RegisterUIEvent(type, function);
            m_UIEventHandlers.Add(handler);
            return handler;
        }
        public void UnRegisterUIEvent(UIEventNew.UIEventHandleNew handler)
        {
            if (handler == null) return;
            if (m_UIEventHandlers != null)
            {
                m_UIEventHandlers.Remove(handler);
            }
            UIEventManager.GetInstance().UnRegisterUIEvent(handler);
        }
        public static IClientFrame OpenTargetFrame<T>(FrameLayer eLayer = FrameLayer.Middle, object userData = null) where T : ClientFrame, new()
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<T>())
            {
                return ClientSystemManager.GetInstance().OpenFrame<T>(eLayer, userData);
            }
            return ClientSystemManager.GetInstance().GetFrame(typeof(T));
        }

        protected List<object> buttons = null;
        protected void _AddButton(string path,UnityAction callBack)
        {
            var button = Utility.FindComponent<Button>(frame, path);
            if(null != button)
            {
                button.onClick.AddListener(callBack);
            }
            if (null == buttons)
            {
                buttons = new List<object>();
            }
            buttons.Add(button);
        }

        protected void _RemoveAllButtons()
        {
            if(null == buttons)
            {
                return;
            }

            for(int i = 0; i < buttons.Count; ++i)
            {
                var button = buttons[i] as Button;
                if(null != button)
                {
                    button.onClick.RemoveAllListeners();
                }
            }

            buttons.Clear();
        }

        protected List<KeyValuePair<int, IClientFrame>> childFrames = null;
        protected void _AddChildFrame(int id, IClientFrame clientFrame)
        {
            if (null == childFrames)
            {
                childFrames = new List<KeyValuePair<int, IClientFrame>>();
            }

            if (null != clientFrame)
            {
                int iFindIndex = _FindChildFrame(id);
                for(int i = 0; i < childFrames.Count; ++i)
                {
                    if(childFrames[i].Key == id)
                    {
                        iFindIndex = i;
                        break;
                    }
                }
                if(iFindIndex == -1)
                {
                    childFrames.Add(new KeyValuePair<int, IClientFrame>(id,clientFrame));
                }
                else
                {
                    Logger.LogErrorFormat("add child frame repeated id = {0}", id);
                }
            }
        }

        protected int _FindChildFrame(int id)
        {
            int iFindIndex = -1;

            if(null != childFrames)
            for (int i = 0; i < childFrames.Count; ++i)
            {
                if (childFrames[i].Key == id)
                {
                    iFindIndex = i;
                    break;
                }
            }

            return iFindIndex;
        }

        protected IClientFrame _GetChildFrame(int id)
        {
            var findIndex = _FindChildFrame(id);
            if(-1 == findIndex)
            {
                return null;
            }

            return childFrames[findIndex].Value;
        }

        protected void _CloseAllChildFrames()
        {
            if(null != childFrames)
            {
                for (int i = 0; i < childFrames.Count; ++i)
                {
                    var childFrame = childFrames[i];
                    if (null != childFrame.Value)
                    {
                        childFrame.Value.Close(true);
                    }
                }
                childFrames.Clear();
            }
        }

        protected IClientFrameManager frameMgr;
        protected GameObject frame;
        protected GameObject content;            //子窗口根节点 add by Jermaine
        protected GameObject root;

        protected GameObject blackMask;
        protected GameObject glassMask;

        protected object userData;

        protected string mFrameName;

        protected EFrameState m_state = EFrameState.Close;
        protected EFadeDelayState m_eEFadeDelayState = EFadeDelayState.EFDS_INVALID;

        private float m_fFadeStart = 0.0f;

        protected DoTweenTrigger tweenTriggers = null;
        protected UIFrameScript animationTrigger = null;

        protected ClientFrameBinder mClientFrameBinder = null;
        protected IComClientFrame mComClienFrame = null;

        protected ComCommonBind mBind = null;

        protected bool _global = false;
        protected bool m_ForbidFadeIn = false;

        public delegate void OnFadeInEnd();
        protected OnFadeInEnd onFadeInEnd;

        protected List<ComItem> m_akComItemList = null;
        protected List<ComItemNew> m_akComItemNewList = null;

        public ComItem CreateComItem(GameObject goParent)
        {
            var comItem = ComItemManager.Create(goParent);
            if (comItem != null)
            {
                if (m_akComItemList == null)
                {
                    m_akComItemList = new List<ComItem>();
                }
                m_akComItemList.Add(comItem);
            }

            return comItem;
        }

        public ComItemNew CreateComItemNew(GameObject goParent)
        {
            var comItem = ComItemManager.CreateNew(goParent);
            if (comItem != null)
            {
                if (m_akComItemNewList == null)
                {
                    m_akComItemNewList = new List<ComItemNew>();
                }
                m_akComItemNewList.Add(comItem);
            }

            return comItem;
        }

        void DestroyComItems()
        {
            if (m_akComItemList != null)
            {
                for (int i = 0; i < m_akComItemList.Count; ++i)
                {
                    if (m_akComItemList[i] != null)
                    {
                        ComItemManager.Destroy(m_akComItemList[i]);
                    }
                }
                m_akComItemList.Clear();
            }

            if (m_akComItemNewList != null)
            {
                for (int i = 0; i < m_akComItemNewList.Count; ++i)
                {
                    if (m_akComItemNewList[i] != null)
                    {
                        ComItemManager.DestroyNew(m_akComItemNewList[i]);
                    }
                }
                m_akComItemNewList.Clear();
            }
        }

        public ClientFrame()
        {
            _global = false;
            m_ForbidFadeIn = false;
        }

        public virtual void Init()
        {

        }

        public virtual void Clear()
        {

        }

        /// <summary>
        /// GameObject名字
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            if (frame != null)
            {
                return frame.name;
            }
            return "UnKnown";
        }

        public string GetFrameName()
        {
            return mFrameName;
        }

        public void SetFrameName(string name)
        {
            mFrameName = name;
        }

        public void SetGlobal(bool isGlobal)
        {
            if (_global != isGlobal)
            {
                _global = isGlobal;

                if (frame != null && _global)
                {
                    frame.tag = ClientSystemManager.kGlobalTag;
                }
            }
        }

        public bool IsGlobal()
        {
            return _global;
        }



        public void SetVisible(bool bVisible)
        {
            if (frame != null)
            {
                frame.CustomActive(bVisible);
            }
        }

        public void SetForbidFadeIn(bool Forbid)
        {
            m_ForbidFadeIn = Forbid;
        }

        public void SetManager(IClientFrameManager mgr)
        {
            this.frameMgr = mgr;
        }

        public bool IsOpen()
        {
            return m_state != EFrameState.Close;
        }

        public bool IsHidden()
        {
            return m_state != EFrameState.Close && m_state == EFrameState.Hidden;
        }

        public virtual bool NeedMutex()
        {
            return true;
        }

        public virtual bool RemoveRefOnClose()
        {
            return ClientSystemManager.sRemoveRefOnClose;
        }

        public void Show(bool isShow, Type type = null)
        {
            if (frame != null)
            {
                if (isShow)
                {
                    if (true || m_state == EFrameState.Hidden || m_state == EFrameState.FadeOut)
                    {
                        //if (mHiddenFrameType == type)
                        {
                            Logger.LogProcessFormat("[ClientFrame] 显示界面 {0}", mFrameName);

                            if (mComClienFrame != null && mComClienFrame.IsNeedFade())
                            {
                                FadeInSpecial(true);
                                // state is fadein
                                // while fadein over then state is open
                            }
                            else
                            {
                                frame.SetActive(isShow);
                                m_state = EFrameState.Open;
                            }

                            //_updateClientFrameMutex(!isShow);
                        }
                        //else
                        //{
                        //    Logger.LogProcessFormat("not match type to show frame");
                        //}
                    }
                }
                else
                {
                    if (true || m_state == EFrameState.Open || m_state == EFrameState.FadeIn)
                    {
                        Logger.LogProcessFormat("[ClientFrame] 隐藏界面 {0}", mFrameName);

                        //mHiddenFrameType = type;

                        if (mComClienFrame != null && mComClienFrame.IsNeedFade())
                        {
                            FadeOutSpecial();
                            // state is fadeout
                        }
                        else
                        {
                            frame.CustomActive(isShow);
                        }

                        m_state = EFrameState.Hidden;
                    }
                    else
                    {
                        //mHiddenFrameType = type;
                        Logger.LogProcessFormat("error state, it's already hidden, update the mHiddenFrameType");
                    }
                }
            }
        }

        public virtual bool IsNeedUpdate()
        {
            return false;
        }

        public virtual EFrameState GetState()
        {
            return m_state;
        }

        public string GetGroupTag()
        {
            if (null != mComClienFrame)
            {
                return mComClienFrame.GetGroupTag();
            }

            return "";
        }

        public eFrameType GetFrameType()
        {
            if(null != mComClienFrame)
            {
                return mComClienFrame.GetFrameType();
            }

            return eFrameType.Null;
        }

        public bool IsNeedClearWhenChangeScene()
        {
            if (null != mComClienFrame)
            {
                return mComClienFrame.IsNeedClearWhenChangeScene();
            }

            return false;
        }

        public bool IsFullScreenFrameNeedBeClose()
        {
            if (null == mComClienFrame)
            {
                return false;
            }

            if (!mComClienFrame.IsFullScreenFrame())
            {
                return false;
            }


            return mComClienFrame.IsFullScreenFrameNeedBeClose();
        }

        public void SetIsNeedClearWhenChangeScene(bool bFlag)
        {
            if (null != mComClienFrame)
            {
                mComClienFrame.SetIsNeedClearWhenChangeScene(bFlag);
            }
        }

        public FrameLayer GetLayer()
        {
            if (null != mComClienFrame)
            {
                return mComClienFrame.GetLayer();
            }

            return FrameLayer.Invalid;
        }

        private void _initCommonBind()
        {
            if (null != frame)
            {
                mBind = frame.GetComponent<ComCommonBind>();
                _bindExUI();
            }
        }

        private void _uninitCommonBind()
        {
            if (null != mBind)
            {
                _unbindExUI();

                mBind.ClearAllCacheBinds();
                mBind = null;
            }
        }

        protected Canvas mCanvas;
        protected CanvasGroup mCanvasGroup;

        public void Open(GameObject root, object userData = null, FrameLayer layer = FrameLayer.Invalid)
        {

            if (IsOpen())
            {
                return;
            }

#if PRINT_UI_TIME_COST
            var beginTime = Time.realtimeSinceStartup;
#endif
            m_state = EFrameState.Open;

            this.userData = userData;
            this.root = root;

            frame = _loadResGameObject();

            if (frame == null)
            {
                Logger.LogErrorFormat("OpenFrame时 [_loadResGameObject] 失败, mFrameName = {0}", mFrameName);
                return;
            }

#if PRINT_UI_TIME_COST
            var endLoadGameObjectTime = Time.realtimeSinceStartup;
#endif

            mClientFrameBinder = frame.AddComponent<ClientFrameBinder>();
            if(mClientFrameBinder != null)
            {
                mClientFrameBinder.clientFrame = this;
                mClientFrameBinder.frame = frame;
            }

            _initClientFrameCom();

            mCanvas = frame.GetOrAddComponent<Canvas>();
            mCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
            mCanvasGroup = frame.GetOrAddComponent<CanvasGroup>();
            var graphicray = frame.GetOrAddComponent<GraphicRaycaster>();
            if (frame.layer != LayerMask.GetMask("UI"))
            {
                frame.SetLayer("UI");
            }

            if (mComClienFrame != null)
            {
                ComClientFrame frameScript = mComClienFrame as ComClientFrame;
                if (frameScript != null && layer != FrameLayer.Invalid)
                {
                    frameScript.mLayer = layer;
                }

                if (frameScript != null && frameScript.mFrameType == eFrameType.Popup && frameScript.IsUseScreenShotMask)
                {
                    UIManager.GetInstance().EnableScreenShot(this);
                }

                if (frameScript != null && frameScript.bUseBlackMask && LeanTween.instance.frameBlackMask)
                {
                    blackMask = GameObject.Instantiate(LeanTween.instance.frameBlackMask);
                    blackMask.transform.SetParent(root.transform, false);

                    if (frameScript.bBlackMaskClickAutoClose)
                    {
                        bool closeWithGC = frameScript.bNewCloseNeedGC;

                        Button btnClose = blackMask.GetComponent<Button>();
                        btnClose.onClick.RemoveAllListeners();
                        btnClose.onClick.AddListener(() =>
                        {
                            frameMgr.CloseFrame(this);

                            if (closeWithGC)
                            {
                                AssetGabageCollector.instance.ClearUnusedAsset();
                            }

                            AssetGabageCollectorHelper.instance.AddGCPurgeTick(AssetGCTickType.UIFrame);
                        });
                    }
                }

                if (frameScript != null && frameScript.mNeedGlass && LeanTween.instance.frameGlassMask)
                {
                    glassMask = GameObject.Instantiate(LeanTween.instance.frameGlassMask);
                    glassMask.transform.SetParent(root.transform, false);
                }
                
            }

            //if(mComClienFrame as ComClientFrame).
            frame.transform.SetParent(root.transform, false);
            if (_global)
            {
                frame.tag = ClientSystemManager.kGlobalTag;
            }

            _initCommonBind();

            AttachContent();

            _OnLoadPrefabFinish();

            if (_isInitWithGameBindSystem())
            {
                Logger.LogProcessFormat("[ClientFrame] 用 GameBindSystem 初始化绑定");
                InitBindSystem(frame);
            }

            _InitFrameAnimation();
            _TryAddBack2Frame();
            _InitFrameTweenTrigger();
            _Open();

            if (null != mComClienFrame)
            {
                mComClienFrame.SetCurrentFrame(this.GetType().FullName);
                mComClienFrame.InitAnimator();
            }

            FadeIn();
#if PRINT_UI_TIME_COST
            var endTime = Time.realtimeSinceStartup;
            var loadGameObjectCostTime = endLoadGameObjectTime - beginTime;
            var totalCostTime = endTime - beginTime;

            //ExceptionManager.GetInstance().PrintOpenFrameTime(mFrameName,
            //    beginTime, endTime, totalCostTime);
            ExceptionManager.GetInstance().PrintOpenFrameTime(mFrameName, loadGameObjectCostTime, totalCostTime);
            ExceptionManager.GetInstance().PrintFrameTimeOneFile(mFrameName, loadGameObjectCostTime, totalCostTime);
#endif
        }

        private GameObject _loadResGameObject()
        {
            if (_isLoadFromPool())
            {
                Logger.LogProcessFormat("[ClientFrame] _loadResGameObject 从资源池子加载");
                return CGameObjectPool.instance.GetGameObject(GetPrefabPath(), enResourceType.UIPrefab, (uint)GameObjectPoolFlag.None);
            }
            else
            {
                Logger.LogProcessFormat("[ClientFrame] _loadResGameObject 直接加载");
                return AssetLoader.instance.LoadResAsGameObject(GetPrefabPath());
            }
        }

        private void _unloadResGameObject()
        {
            if (frame != null)
            {
                frame.transform.SetParent(null, false);
            }

            if (_isLoadFromPool())
            {
                Logger.LogProcessFormat("[ClientFrame] _unloadResGameObject 放回资源池子");
                CGameObjectPool.instance.RecycleGameObject(frame);
            }
            else
            {
                Logger.LogProcessFormat("[ClientFrame] _unloadResGameObject 直接删除");
                GameObject.Destroy(frame);
            }

            frame = null;
        }

        protected virtual bool _isLoadFromPool()
        {
            return false;
        }

        private bool _isInitWithGameBindSystem()
        {
            if (null == mComClienFrame)
            {
                return true;
            }

            return mComClienFrame.IsInitWithGameBindSystem();
        }


        protected virtual void _bindExUI()
        {

        }

        protected virtual void _unbindExUI()
        {

        }


        protected virtual bool _IsLoadingFrame()
        {
            return false;
        }

        private void _initClientFrameCom()
        {
            if (frame != null)
            {
                var com = frame.GetComponent<ComClientFrame>();
                mComClienFrame = com;

                if (com != null)
                {
                    com.SetClientFrame(this);
                }
            }

            //_updateClientFrameCom();

            UpdateRoot();
        }

        public void UpdateRoot()
        {
            //_updateRoot();
            //_updateClientFrameAttr();
        }

        protected void _updateRoot()
        {
            if (null != mComClienFrame)
            {
                // 这里找到 根路径
                GameObject fakeFrame = frame;
                while (fakeFrame.transform.parent != null && fakeFrame.transform.parent != root.transform)
                {
                    fakeFrame = fakeFrame.transform.parent.gameObject;
                    ComClientFrame com = fakeFrame.GetComponent<ComClientFrame>();
                    if (com != null)
                    {
                        // 这里返回
                        // 如果有子界面嵌入了另一个界面的prefab之中
                        // 那么子界面不更新
                        return;
                    }
                }

                // 若这里为空，则是非正常情况
                // 例如手动在Hierarchy中修改过层级
                if (fakeFrame.transform.parent != null)
                {
                    root = ClientSystemManager.instance.GetLayer(mComClienFrame.GetLayer());
                    if (null != fakeFrame && null != root)
                    {
                        fakeFrame.transform.SetParent(root.transform, false);
                    }
                }
            }
        }

        protected void _updateClientFrameAttr()
        {
            if (frame != null && mComClienFrame != null)
            {
                frame.transform.SetSiblingIndex(mComClienFrame.GetZOrder());
            }
        }


        public void Close(bool bCloseImmediately = false)
        {
            if (IsOpen() == false)
            {
                return;
            }

            _notifyFrameIsOpen(false);

            if (!bCloseImmediately)
            {
                bool bNeedClose = FadeOut();
                if (bNeedClose)
                {
                    _Close();
                }
            }
            else
            {
                _Close();
            }
        }

        protected virtual bool GetNeedAutoFadeIn()
        {
            return true;
        }

        protected virtual bool GetNeedOpenSound()
        {
            return false;
        }

        protected virtual bool GetNeedCloseSound()
        {
            return false;
        }

        class InvokeDelayFade : InvokeMethod.TaskManager.Invoke
        {
            public EFadeDelayState eEFadeDelayState = EFadeDelayState.EFDS_INVALID;
            public InvokeDelayFade(float fStart, float fDelay, UnityAction callback) : base(fStart, fDelay, callback)
            {

            }
        }

        InvokeDelayFade invokeDelayFade = new InvokeDelayFade(0, 0, null);

        public void FadeInSpecial(bool bForce = false)
        {
            Logger.LogProcessFormat("FadeInSpecial Name = {0}", GetName());
            invokeDelayFade.eEFadeDelayState = EFadeDelayState.EFDS_IN;
            invokeDelayFade.fStart = Time.time;
            invokeDelayFade.fDelay = 0.050f;
            invokeDelayFade.callback = () => { FadeIn(bForce); };

            InvokeMethod.AddUniqueInvoke(invokeDelayFade);
        }

        public void FadeOutSpecial()
        {
            Logger.LogProcessFormat("FadeOutSpecial Name = {0}", GetName());
            invokeDelayFade.eEFadeDelayState = EFadeDelayState.EFDS_OUT;
            invokeDelayFade.fStart = Time.time;
            invokeDelayFade.fDelay = 0.10f;
            invokeDelayFade.callback = () => { FadeOut(false); };

            InvokeMethod.AddUniqueInvoke(invokeDelayFade);
        }

        protected bool FadeIn(bool bForce = false)
        {
            try
            {
                if (m_ForbidFadeIn)
                {
                    return false;
                }

                if (bForce || GetNeedAutoFadeIn())
                {
                    if (animationTrigger != null)
                    {
                        if (m_state != EFrameState.FadeIn)
                        {
                            m_state = EFrameState.FadeIn;
                            Logger.LogProcessFormat("[ClientFrame] 播放淡入动画 {0}", GetName());
                            animationTrigger.DoPlay(UIFrameScript.FunctionType.FT_FADEIN, OnFadeInOver);
                            _TryFadeInBack();
                        }

                        return false;
                    }
                    else if (tweenTriggers != null)
                    {
                        if (m_state != EFrameState.FadeIn)
                        {
                            m_state = EFrameState.FadeIn;
                            tweenTriggers.FadeIn(OnFadeInOver);
                            _TryFadeInBack();
                        }

                        return false;
                    }
                    else if (mComClienFrame != null)
                    {
                        ComClientFrame comClient = mComClienFrame as ComClientFrame;
                        //if (comClient.bUseFadeIn)
                        //{
                            //m_state = EFrameState.FadeIn;
                            //LeanTween.playFrameOpen(frame, blackMask, OnFadeInOver);
                            //_TryFadeInBack();
                            //return false;
                        //}
                        if (comClient.IsPredefineFade)
                        {
                            m_state = EFrameState.FadeIn;
                            comClient.FadeIn(OnFadeInOver);
                            _TryFadeInBack();
                            return false;
                        }
                    }
                }

                if (m_state == EFrameState.Open)
                {
                    Logger.LogProcessFormat("[ClientFrame] 完全打开界面 不含动画");
                    _notifyFrameIsOpen(true);
                }
            }
            catch(System.Exception e)
            {
                Logger.LogErrorFormat("[FadeIn]FadeIn出错:{0}", e.ToString());
            }
            

            return true;
        }

        void OnFadeInOver()
        {
            switch (m_state)
            {
                case EFrameState.FadeIn:
                    {
                        m_state = EFrameState.Open;
                        Logger.LogProcessFormat("[ClientFrame] 完全打开界面 包含动画");
                        _notifyFrameIsOpen(true);
                    }
                    break;
            }

            if (onFadeInEnd != null)
            {
                onFadeInEnd.Invoke();
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FadeInOver, mFrameName);
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.FadeInOver, mFrameName);
        }

        void _CoreOpen()
        {

        }

        void _CoreClose()
        {

        }



        void _Open()
        {
            UWAProfilerUtility.Mark("[tm]ClientFrame_Open");

            m_state = EFrameState.Open;
            _CoreOpen();
            _OnOpenFrame();

            if (GetNeedOpenSound())
            {
                AudioManager.instance.PlaySound(Global.COMMON_SOUND_OPEN_FRAME, AudioType.AudioEffect);
            }
        }

        void _InitFrameAnimation()
        {
            animationTrigger = frame.transform.GetComponent<UIFrameScript>();
            if (animationTrigger != null)
            {
                animationTrigger.Initialize();
            }

            if (!_IsLoadingFrame())
            {
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneLoadFinish, OnSceneLoadFinish);
            }
        }

        void _InitFrameTweenTrigger()
        {
            tweenTriggers = frame.transform.GetComponent<DoTweenTrigger>();
            if (tweenTriggers != null)
            {
                tweenTriggers.Initialize();
            }
        }

        void _TryAddBack2Frame()
        {
            if (animationTrigger != null)
            {
                animationTrigger.DoPlay(UIFrameScript.FunctionType.FT_ADDBACK);
            }
        }

        void _TryFadeInBack()
        {
            if (frame.transform.parent != null && frame.transform.name + "parent" == frame.transform.parent.name)
            {
                DoTweenTrigger tweenTriggerBack = frame.transform.parent.GetComponent<DoTweenTrigger>();
                if (tweenTriggerBack != null)
                {
                    tweenTriggerBack.FadeIn();
                }
            }
        }

        void _TryFadeOutBack()
        {
            if (frame.transform.parent != null && frame.transform.name + "parent" == frame.transform.parent.name)
            {
                DoTweenTrigger tweenTriggerBack = frame.transform.parent.GetComponent<DoTweenTrigger>();
                if (tweenTriggerBack != null)
                {
                    tweenTriggerBack.FadeOut();
                }
            }
        }

        // if need call _Close return true
        protected bool FadeOut(bool bClose = true)
        {
            UWAProfilerUtility.Mark("[tm]ClientFrame_FadeOut");

            bool bReturn = true;
            try
            {
                if (GetNeedCloseSound())
                {
                    AudioManager.instance.PlaySound(Global.COMMON_SOUND_CLOSE_FRAME, AudioType.AudioEffect);
                }

                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneLoadFinish, OnSceneLoadFinish);

                if (animationTrigger != null)
                {
                    m_state = bClose ? EFrameState.FadeOut : EFrameState.Hidden;
                    Logger.LogProcessFormat("[ClientFrame] 播放淡出动画 {0}", GetName());
                    animationTrigger.DoPlay(UIFrameScript.FunctionType.FT_FADEOUT, OnFadeOutOver);
                    _TryFadeOutBack();

                    bReturn = false;
                }
                else if (tweenTriggers != null)
                {
                    m_state = bClose ? EFrameState.FadeOut : EFrameState.Hidden;
                    tweenTriggers.FadeOut(OnFadeOutOver);
                    _TryFadeOutBack();
                    bReturn = false;
                }
                else if (mComClienFrame != null)
                {
                    ComClientFrame comClient = mComClienFrame as ComClientFrame;

                    //if (comClient.bUseFadeOut)
                    //{
                        //m_state = bClose ? EFrameState.FadeOut : EFrameState.Hidden;
                        //LeanTween.playFrameClose(frame, blackMask, OnFadeOutOver);
                        //_TryFadeOutBack();
                        //bReturn = false;
                    //}
                    if (comClient.IsPredefineFade)
                    {
                        m_state = bClose ? EFrameState.FadeOut : EFrameState.Hidden;
                        comClient.FadeOut(OnFadeOutOver);
                        _TryFadeOutBack();
                        bReturn = false;
                    }
                }

                if (m_state == EFrameState.Hidden || m_state == EFrameState.FadeOut)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FadeOutOver, mFrameName);
                }
            }
            catch(System.Exception e)
            {
                Logger.LogErrorFormat("[FADEOUT]fadeout出错:{0}", e.ToString());
            }
            

            return bReturn;
        }

        void OnFadeOutOver()
        {
            Logger.LogProcessFormat("[ClientFrame] 淡出动画执行完的回调 {0}, 当前状态 {1}", mFrameName, m_state);
            switch (m_state)
            {
                case EFrameState.FadeOut:
                    {
                        _Close();
                        break;
                    }
                case EFrameState.Hidden:
                    {
                        frame.CustomActive(false);
                        break;
                    }
            }
        }

        private Action<IClientFrame> closeCallBack;

        public Action<IClientFrame> CloseCallBack { get { return closeCallBack; } set { closeCallBack = value; } }

        private void _notifyFrameIsOpen(bool isOpen)
        {
            if (null == mComClienFrame)
            {
                Logger.LogProcessFormat("[ClientFrame] 通知界面 配置为空格");
                return ;
            }

            if (!mComClienFrame.IsFullScreenFrame())
            {
                Logger.LogProcessFormat("[ClientFrame] 通知界面 不是全屏界面");
                return ;
            }

            if (isOpen)
            {
                ClientSystemManager.instance.NotifyFrameIsOpen(this);
            }
            else
            {
                ClientSystemManager.instance.NotifyFrameIsClose(this);
            }
        }


        void _Close()
        {
            UWAProfilerUtility.Mark("[tm]ClientFrame_Close");

            if (frameMgr != null)
            {
                frameMgr.OnFrameClose(this, RemoveRefOnClose());
            }

            m_state = EFrameState.Close;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneLoadFinish, OnSceneLoadFinish);
            StopAllCoroutine();

            InvokeMethod.RemoveUniqueInvoke(invokeDelayFade);
            _CoreClose();
            //暂时只调用一次
            if (CloseCallBack != null)
            {
                CloseCallBack(this);
                CloseCallBack = null;
            }
            if(m_UIEventHandlers != null)
            {
                for(int i = 0; i < m_UIEventHandlers.Count;i++)
                {
                    UIEventManager.GetInstance().UnRegisterUIEvent(m_UIEventHandlers[i]);
                }
                m_UIEventHandlers.Clear();
            }
            _OnCloseFrame();
            userData = null;

            _uninitCommonBind();

            if (null != mClientFrameBinder)
            {
                mClientFrameBinder.OnCloseFrame();
            }

            if (this.GetLayer() == FrameLayer.Middle)
            {
                if(null != frame)
                {
                    ComClientFrame com = frame.GetComponent<ComClientFrame>();
                    if (com != null)
                    {
                        if (com.mFrameType == eFrameType.FullScreen)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MiddleFrameClose);
                        }
                    }
                }
            }

            DestroyComItems();
            _RemoveAllButtons();
            _CloseAllChildFrames();

            if (_isInitWithGameBindSystem())
            {
                Logger.LogProcessFormat("[ClientFrame] 用 GameBindSystem 反初始化绑定");
                ExistBindSystem();
            }

            onFadeInEnd = null;

            if (animationTrigger != null)
            {
                animationTrigger.DoPlay(UIFrameScript.FunctionType.FT_REMOVEBACK);
            }

            if (blackMask != null)
            {
                blackMask.transform.SetParent(null, false);
                GameObject.Destroy(blackMask);
                blackMask = null;
            }

            if (null != glassMask)
            {
                glassMask.transform.SetParent(null, false);
                GameObject.Destroy(glassMask);
                glassMask = null;
            }

            ComClientFrame comClient = mComClienFrame as ComClientFrame;

            if (comClient != null && comClient.mFrameType == eFrameType.Popup && comClient.IsUseScreenShotMask)
            {
                UIManager.GetInstance().DisableScreenShot(this);
            }

            // _OnCloseFrame()最好不要放在frame关掉之后
            _unloadResGameObject();

            root = null;
            mComClienFrame = null;


            UIEvent uiEvent = new UIEvent();
            uiEvent.EventID = EUIEventID.FrameClose;
            uiEvent.Param1 = mFrameName;
            uiEvent.Param2 = this.GetType();
            uiEvent.Param3 = this;
            GlobalEventSystem.GetInstance().SendUIEvent(uiEvent);

            //AssetLoader.instance.ScanAndLogResourceStates();
            //AssetGabageCollector.instance.ClearUnusedAsset();
        }

        public void Update(float timeElapsed)
        {
            _OnUpdate(timeElapsed);
        }

        public virtual string GetPrefabPath()
        {
            return "";
        }

        protected virtual bool AttachContent()
        {
            return false;
        }

        protected virtual void _OnOpenFrame()
        {
        }

        void OnSceneLoadFinish(UIEvent uiEvent)
        {
            OnSceneLoadFinish();
        }

        protected virtual void OnSceneLoadFinish()
        {

        }

        protected virtual void _OnDoTweenEnd()
        {
            frame.gameObject.SetActive(false);
        }

        protected virtual void _OnLoadPrefabFinish()
        {

        }

        protected virtual void _OnCloseFrame()
        {

        }

        protected virtual void _OnUpdate(float timeElapsed)
        {

        }

        public T GetComponent<T>(string name) where T : Component
        {
            if (_isInitWithGameBindSystem())
            {
                return GetComponentByName<T>(name);
            }
            else
            {
                var go = Utility.FindChild(frame, name);
                if (go == null)
                {
                    //Logger.LogError("cant find node" + name);
                    return null;
                }

                return go.GetComponent<T>();
            }
        }
        public T GetComponentInChildren<T>(string name) where T : Component
        {
            if (_isInitWithGameBindSystem())
            {
                return GetComponentInChilderByName<T>(name);
            }
            else
            {
                return Utility.GetComponetInChild<T>(frame, name);
            }
        }
        
        public virtual IEnumerator LoadingOpenPost()
        {
            yield return null;
        }

        //每个frame会创建一个父节点挂在在父节点下，所以实际需要的是父节点的index
        public int GetSiblingIndex()
        {
            return frame.transform.parent.GetSiblingIndex();
        }

        public void SetSiblingIndex(int index)
        {
            frame.transform.parent.SetSiblingIndex(index);
        }

        public GameObject GetFrame()
        {
            return this.frame;
        }
    }
}
