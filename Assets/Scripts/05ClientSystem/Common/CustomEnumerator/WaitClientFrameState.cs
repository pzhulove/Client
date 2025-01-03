using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameClient
{
    public class WaitClientFrameState : BaseCustomEnum<WaitClientFrameState.eResult>, IEnumerator 
    {
        public enum eResult
        {
            None,
            Success,
            /// <summary>
            /// error list
            /// </summary>
            InvalidFrame,
        }

        private IClientFrame mFrame = null;
        private EFrameState  mState = EFrameState.Close;

        public WaitClientFrameState(Type type, EFrameState state)
        {
            mFrame  = ClientSystemManager.instance.GetFrame(type);
            mState  = state;
            mResult = eResult.None;
        }

        public WaitClientFrameState(IClientFrame frame, EFrameState state)
        {
            mFrame  = frame;
            mState  = state;
            mResult = eResult.None;

            Logger.LogProcessFormat("[WaitForClientFrame] 等待界面 {0} 到 {1}", mFrame.GetFrameName(), mState);
        }

#region IEnumerator implementation
        public bool MoveNext()
        {
            if (mFrame == null)
            {
                Logger.LogProcessFormat("[WaitForClientFrame] 等待无效界面");
                mResult = eResult.InvalidFrame;
                return false;
            }

            if (mFrame.GetState() != mState)
            {
                return true;
            }

            Logger.LogProcessFormat("[WaitForClientFrame] 等待界面 {0} 到 {1} 成功", mFrame.GetFrameName(), mState);

            mResult = eResult.Success;
            mFrame = null;

            return false;
        }

        public void Reset()
        {
            mResult = eResult.None;
            mFrame = null;
        }

        public object Current
        {
            get
            {
                return mResult;
            }
        }
#endregion
    }

    public class WaitClientFrameOpen : WaitClientFrameState
    {
        public WaitClientFrameOpen(Type type) : base (type, EFrameState.Open) { }

        public WaitClientFrameOpen(IClientFrame frame) : base (frame, EFrameState.Open) { }
    }

    public class WaitClientFrameClose : WaitClientFrameState
    {
        public WaitClientFrameClose(Type type) : base (type, EFrameState.Close) { }

        public WaitClientFrameClose(IClientFrame frame) : base (frame, EFrameState.Close) { }
    }
}
