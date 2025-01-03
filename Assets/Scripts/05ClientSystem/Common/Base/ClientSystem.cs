using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;


namespace GameClient
{
    public class ClientSystem : GameBindSystem, IClientSystem, IGameBind
    {
        public eClientSystemState GetState()
        {
            return state;
        }

        public void BeforeEnter()
        {
            _clearState();
            state = eClientSystemState.onInit;
            OnBeforeEnter();
        }

        public void OnSystemError()
        {
            state = eClientSystemState.onError;
        }

        private void _clearState()
        {
            mLastState = eClientSystemState.onNone;
            mState = eClientSystemState.onNone;
        }

#region ClientSystemState
        public enum eClientSystemState
        {
            /// <summary>
            /// 无效状态
            /// </summary>
            onNone,

            /// <summary>
            /// 初始化, 在开始进入系统之前
            /// </summary>
            onInit,

            /// <summary>
            /// 开始进入当前系统
            /// </summary>
            onEnter,

            /// <summary>
            /// 已经进入当前系统，可以进行Update
            /// </summary>
            onTick,

            /// <summary>
            /// 开始推出当前系统
            /// </summary>
            onExit,
            /// <summary>
            /// 结束
            /// </summary>
            onEnd,

            /// <summary>
            /// 错误状态，等待退出中
            /// </summary>
            onError,
        }

        protected eClientSystemState mState     = eClientSystemState.onNone;
        protected eClientSystemState mLastState = eClientSystemState.onNone;

        public eClientSystemState lastState
        {
            get { return mLastState; }
        }

        public eClientSystemState state
        {
            get
            {
                return mState;
            }

            private set 
            {
                mLastState = mState;
                mState = value;
                Logger.LogProcessFormat("[ClientSystem] {0} 状态改变 {1} -> {2}", GetName(), mLastState, mState);
            }
        }

#endregion


        public void SetName(string name)
        {
            this.systemName = name;
        }

        public string GetName()
        {
            return systemName;
        }

        public ClientSystemManager SystemManager {
            set; get;
        }
        
        #region IGameBind implementation
        public T GetComponent<T>(string name) where T : Component
        {
            return GetComponentByName<T>(name);
        }

        public T GetComponentInChildren<T>(string name) where T : Component
        {
            return GetComponentInChilderByName<T>(name);
        }
#endregion


        public static bool IsTargetSystem<T>() where T : ClientSystem , new()
        {
            return ClientSystemManager.GetInstance().CurrentSystem is T && ClientSystemManager.GetInstance().TargetSystem == null ||
                ClientSystemManager.GetInstance().TargetSystem is T;
        }

        public static bool IsCurrentSystemStart()
        {
             if(ClientSystemManager.GetInstance().TargetSystem != null)
              {
                  return (ClientSystemManager.GetInstance().TargetSystem as ClientSystem).BStart;
              }
              else if(ClientSystemManager.GetInstance().CurrentSystem != null)
              {
                   return (ClientSystemManager.GetInstance().CurrentSystem as ClientSystem).BStart;
              }

              return false;
        }
        public static T GetTargetSystem<T>() where T : ClientSystem, new()
        {
            T current = null;
            if(IsTargetSystem<T>())
            {
                current = ClientSystemManager.GetInstance().CurrentSystem as T;
                if(null == current)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as T;
                }
            }
            return current;
        }

        public virtual void GetExitCoroutine(AddCoroutine exit)
        {
            exit(_baseSystemExitCoroutine);
        }

        public virtual void GetEnterCoroutine(AddCoroutine enter)
        {
            enter(_baseSystemLoadingCoroutine);
        }

        public void OnEnterSystem()
        {
			if (state != eClientSystemState.onError)
            	state = eClientSystemState.onTick;

            InvokeMethod.Enter();
            PlayerDataManager.GetInstance().OnEnterSystem();

            OnEnter();
        }

        public virtual void OnBeforeEnter()
        {
        }

        //不要实现，用于封装调用顺序
        public virtual void OnEnter(){}
        
        
        public void OnExitSystem()
        {
            state = eClientSystemState.onEnd;

            OnExit();

            BStart = false;
            //UIEventSystem.GetInstance().Clear();
            InvokeMethod.Exit();

            //ManagerController.Instance().OnExitSystem();
            PlayerDataManager.GetInstance().OnExitSystem();
            ExistBindSystem();
            DestoryMainUI();
            UIEventSystem.GetInstance().PopupLeakedEvents();

            _clearState();
        }

        //不要实现，用于封装调用顺序
        public virtual void OnExit(){}

        bool bStart = false;
        
        public bool BStart { get{return bStart;} set{bStart = value;} }

        public void OnStartSystem(SystemContent systemContent)
        {
            BStart = true;
            OnStart(systemContent);
        }

        public virtual void OnStart(SystemContent systemContent)
        {

        }
        
#region MainFrame
        protected string systemName;
        protected GameObject _mainFrame;
        protected ComCommonBind mBind;

        /// <summary>
        /// 临时给界面互斥用
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowMainFrame(bool isShow)
        {
            if (null != _mainFrame)
            {
                _mainFrame.SetActive(isShow);
            }
        }

        public void CreateMainUI()
        {

            if (_mainFrame == null)
            {
                string name = GetMainUIPrefabName();
                if (string.IsNullOrEmpty(name) == false)
                {
                    _mainFrame = AssetLoader.instance.LoadResAsGameObject(name);
                    if (_mainFrame != null)
                    {
                        mBind = _mainFrame.GetComponent<ComCommonBind>();

                        _mainFrame.transform.SetParent(ClientSystemManager.instance.BottomLayer.transform, false);

                        _bindExUI();

                        _OnMainFrameOpen();
                    }
                    else
                    {
                        Logger.LogErrorFormat("[ClientSystem] {0} 创建主界面失败 {1}", GetName(), name);
                    }
                }
            }
        }

        protected virtual void _bindExUI()
        {
        }

        protected virtual void _unbindExUI()
        {
        }

        protected virtual void _OnMainFrameOpen()
        {
        }

        protected virtual void _OnMainFrameClose()
        {

        }

        protected virtual void _OnDoTweenEnd()
        {
            _mainFrame.gameObject.SetActive(false);
        }

        public virtual string GetMainUIPrefabName()
        {
            return "";
        }

        public void DestoryMainUI()
        {
            if (_mainFrame != null)
            {
                _unbindExUI();

                _OnMainFrameClose();

                GameObject.Destroy(_mainFrame);
                _mainFrame = null;
            }

        }
#endregion

#region IClientSystem

        public bool IsSystem<T>() where T : IClientSystem
        {
            return this is T;
        }

        public IEnumerator _baseSystemExitCoroutine(IASyncOperation systemOperation)
        {
            state = eClientSystemState.onExit;
            yield break;
        }

        public IEnumerator _baseSystemLoadingCoroutine(IASyncOperation systemOperation)
        {
            state = eClientSystemState.onEnter;

            //切换场景
            string levelName = _GetLevelName();
            if (string.IsNullOrEmpty(levelName) == false)
            {
                if (Application.loadedLevelName != levelName)
                {
                    AsyncOperation empty = Application.LoadLevelAsync("GCEmpty");
                    while (empty.isDone == false)
                    {
                        yield return Yielders.EndOfFrame;
                    }
                    AssetGabageCollector.instance.ClearUnusedAsset();

                    AsyncOperation operation = Application.LoadLevelAsync(levelName);
                    while (operation.isDone == false)
                    {
                        systemOperation.SetProgress(operation.progress * 0.3f);
                        yield return Yielders.EndOfFrame;
                    }
                }
            }

            AssetGabageCollector.instance.ClearUnusedAsset();
 
            //创建UI
            CreateMainUI();
            InitBindSystem(_mainFrame);
            systemOperation.SetProgress(0.5f);
            yield return Yielders.EndOfFrame;

            CGameObjectPool.instance.RebuildRoot();
        }
        
        public void Update(float timeElapsed)
        {
            if (state == eClientSystemState.onTick)
            {
                _OnUpdate(timeElapsed);
                InvokeMethod.Update();
            }
        }

#endregion

#region 子类实现

        // U3D工程名字
        protected virtual string _GetLevelName()
        {
            return "";
        }


        // 在_OnSystemInit之后开始调用
        // 在_OnSystemDestory之前停止调用
        protected virtual void _OnUpdate(float timeElapsed)
        {

        }
#endregion
    }
}
