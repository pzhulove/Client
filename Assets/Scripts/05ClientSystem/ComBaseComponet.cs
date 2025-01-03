using UnityEngine;
using System.Collections;

namespace GameClient
{
    [RequireComponent(typeof(ComCommonBind))]
    public class ComBaseComponet : MonoBehaviour 
    {
        public ComCommonBind mBind;

        private bool mIsInited = false;

        public bool isInited
        {
            get 
            {
                return mIsInited;
            }
        }

        private void _bindUI()
        {
            if (null == mBind)
            {
                mBind = this.GetComponent<ComCommonBind>();
            }

            if (!mIsInited)
            {
                Logger.LogProcessFormat("[ComBaseComponet] 基础控件 Init {0}", this.gameObject.name);

                mIsInited = true;
                _bindExUI();
                _bindEvents();
                Init();
            }
        }

        private void _unbindUI()
        {
            if (mIsInited)
            {
                Logger.LogProcessFormat("[ComBaseComponet] 基础控件 Uninit {0}", this.gameObject.name);

                UnInit();

                _unbindEvents();
                _unbindExUI();
            }

            mIsInited = false;
        }

        private void _bindEvents()
        {
            ClientEventNode[] events = GetListenEvents();
            for (int i = 0; i < events.Length; ++i)
            {
                if (null != events[i] && null != events[i].handle)
                {
                    Logger.LogProcessFormat("[ComBaseComponet] 基础控件 BindEvent {0}", events[i].id);
                    UIEventSystem.GetInstance().RegisterEventHandler(events[i].id, events[i].handle);
                }
            }
        }

        private void _unbindEvents()
        {
            ClientEventNode[] events = GetListenEvents();
            for (int i = 0; i < events.Length; ++i)
            {
                if (null != events[i] && null != events[i].handle)
                {
                    Logger.LogProcessFormat("[ComBaseComponet] 基础控件 UnBindEvent {0}", events[i].id);
                    UIEventSystem.GetInstance().UnRegisterEventHandler(events[i].id, events[i].handle);
                }
            }
        }

        protected virtual void Awake()
        {
            mIsInited = false;
            _bindUI();
        }

        protected virtual void OnDestroy()
        {
            _unbindUI();
        }

        protected virtual void OnEnable()
        {
            _bindUI();
        }

        protected virtual void OnDisable()
        {
            _unbindUI();
        }

#region 虚函数重载
        protected virtual void Init()
        {

        }

        protected virtual void UnInit()
        {

        }

        protected virtual ClientEventNode[] GetListenEvents()
        {
            return new ClientEventNode[0];
        }

        protected virtual void _bindExUI()
        {
        }

        protected virtual void _unbindExUI()
        {
        }
#endregion
}
}
