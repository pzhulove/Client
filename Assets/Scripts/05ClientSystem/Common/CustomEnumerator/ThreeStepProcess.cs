using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    /// <summary>
    /// 协程3步流程
    /// 
    /// 开始（init） -> 处理 -------------> 结束（uninit）
    ///                  |                    ^
    ///                  | 出错(异常流程)     |
    ///                  |                    |
    ///                  |----> 错误处理------
    /// </summary>
    public class ThreeStepProcess : BaseCustomEnum<ThreeStepProcess.eResult>, IEnumerator
    {
        public delegate IEnumerator ErrorProcessHandle(eEnumError errorType, string errorMsg);

        public enum eResult
        {
            None,
            Success,
            /// error list
            FailStart,
            FailProcess,
            FailEnd,
            FailError,
        }

        private enum eState 
        {
            None          = -1,

            onError       = 0,
            onStart       = 1,
            onProcess     = 2,
            onEnd         = 3,
            onFinish      = 4,

            /// <summary>
            /// 正在迭代等待结果
            /// </summary>
            onIterEnumrator,
        }

        private IEnumeratorManager mManager   = null;
        private eState             mState     = eState.None;
        private eState             mLastState = eState.None;
        private string             mTag       = "";

        /// <summary>
        /// 兼容Unity自带的WaitForSeconds, WaitForEndOfFrame, ...
        ///
        /// WARNING: 在战斗逻辑中无效
        /// </summary>
        private object                   mCurrentValue;

        private eState state
        {
            get 
            {
                return mState;
            }

            set 
            {
                Logger.LogProcessFormat("[3step] 从 {0} 进入 {1} 流程", mLastState, mState);

                mLastState = mState;
                mState = value;
            }
        }

        private eState lastState
        {
            get 
            {
                return mLastState;
            }
        }


        private ErrorProcessHandle      mErrorHandle = null;
        private IEnumerator[]           mProcessList = new IEnumerator[4];
        private eState[]                mProcessNextStep = new eState[] 
        {
            eState.onEnd,
            eState.onProcess,
            eState.onEnd,
            eState.onFinish,
        };

        /// <summary>
        ///
        /// <param name="processManager">协程管理类</param>
        /// <param name="process">处理逻辑流程</param>
        /// <param name="error">错误处理流程</param>
        /// <param name="start">开始，初始化流程</param>
        /// <param name="end">结束，销毁流程</param>
        ///
        /// </summary>
        public ThreeStepProcess(string tag, IEnumeratorManager processManager, 
                IEnumerator process, 
                IEnumerator start = null, 
                IEnumerator end = null)
        {
            Logger.LogProcessFormat("[3step] 开始3步流程");

            mManager                            = processManager;
            mTag                                = tag;

            mErrorHandle                        = _commonError;

            mProcessList[(int)eState.onStart]   = start == null ? _commonStart(): start;
            mProcessList[(int)eState.onProcess] = process;
            mProcessList[(int)eState.onEnd]     = end == null ? _commonEnd() : end;
        }

        public void SetErrorProcessHandle(ErrorProcessHandle handle)
        {
            if (mState == eState.None)
            {
                if (null != handle)
                {
                    mErrorHandle = handle;
                }
            }
        }

#region IEnumerator implementation

        private IEnumerator _commonStart()
        {
            Logger.LogProcessFormat("[3step] {0} 通用开始(init)流程", mTag);
            yield break;
        }

        private IEnumerator _commonEnd()
        {
            Logger.LogProcessFormat("[3step] {0} 通用结束(uninit)流程", mTag);
            yield break;
        }

        private IEnumerator _commonError(eEnumError errorType, string errorMsg)
        {
            Logger.LogErrorFormat("[3step] {0} 通用异常处理流程 {1} {2}", mTag, errorMsg, errorType);
            yield break;
        }

        private bool _currentProceesIsRunning()
        {
            if (lastState != eState.None &&
                lastState != eState.onIterEnumrator &&
                lastState != eState.onFinish)
            {
                IEnumerator iter = mProcessList[(int)lastState];

                return mManager.IsEnumeratorRunning(iter);
            }

            return false;
        }

        private bool _currentProceesIsError()
        {
            bool isError = false;


            if (lastState != eState.None &&
                lastState != eState.onIterEnumrator &&
                lastState != eState.onFinish)
            {
                IEnumerator iter = mProcessList[(int)lastState];
                isError = mManager.IsEnumeratorError(iter);

            }

            return isError;
        }

        private void _updateCurrentProceesValue()
        {
            mCurrentValue = null;

            if (lastState != eState.None &&
                lastState != eState.onIterEnumrator &&
                lastState != eState.onFinish)
            {
                IEnumerator iter = mProcessList[(int)lastState];
                mCurrentValue = mManager.GetEnumeratorCurrent(iter);
            }

            if (!(mCurrentValue is UnityEngine.YieldInstruction))
            {
                mCurrentValue = null;
            }

        }

        private void _updateErrorProcess()
        {

            if (lastState != eState.None &&
                lastState != eState.onIterEnumrator &&
                lastState != eState.onFinish)
            {
                IEnumerator iter = mProcessList[(int)lastState];

                eEnumError errorType = mManager.GetEnumeratorErrorType(iter);
                string errorMsg = mManager.GetEnumeratorError(iter);

                Logger.LogProcessFormat("[3step] {0} 设置错误处理函数 {1}({2})", mTag, errorType, errorMsg);
                try 
                {
                    if (null != mErrorHandle)
                    {
                        mProcessList[(int)eState.onError] = mErrorHandle(errorType, errorMsg);
                    }
                }
                catch 
                {
                    mProcessList[(int)eState.onError] = _commonError(errorType, errorMsg);
                }
            }
        }

        private void _beforeProcess(eState curstate)
        {
            if (curstate != eState.None &&
                curstate != eState.onIterEnumrator &&
                curstate != eState.onFinish)
            {
                IEnumerator iter = mProcessList[(int)curstate];

                if (null != iter)
                {
                    Logger.LogProcessFormat("[3step] {0} 加入迭代 {1}, {2}", mTag, curstate, iter);
                    mManager.AddEnumerator(iter, this);
                }
            }
        }

        public bool MoveNext()
        {
            if (null == mManager)
            {
                return false;
            }

            switch(state)
            {
                case eState.None:
                    state = eState.onStart;
                    break;
                case eState.onProcess:
                case eState.onStart:
                case eState.onError:
                case eState.onEnd:
                    _beforeProcess(state);
                    state = eState.onIterEnumrator;
                    break;
                case eState.onIterEnumrator:
                    if (lastState != eState.onError && _currentProceesIsError())
                    {
						_updateErrorProcess();
                        state = eState.onError;
                    }
                    else if (!_currentProceesIsRunning())
                    {
                        state = mProcessNextStep[(int)lastState];
                    }
                    break;
                case eState.onFinish:
                    //_setNull();
                    return false;

            }

            //Logger.LogProcessFormat("[3step] {0} 迭代 {1}", mTag, lastState);

            _updateCurrentProceesValue();

            return true;
        }

        private void _setNull()
        {
            //mManager      = null;
            mCurrentValue = null;

            mErrorHandle  = null;

            mProcessList[(int)eState.onError]   = null;
            mProcessList[(int)eState.onStart]   = null;
            mProcessList[(int)eState.onProcess] = null;
            mProcessList[(int)eState.onEnd]     = null;
        }

        public void Reset()
        {
            mState        = eState.None;
            mLastState    = eState.None;

            mResult       = eResult.None;

            _setNull();
        }

        public object Current
        {
            get
            {
                return mCurrentValue;
            }
        }
#endregion
    }

}
