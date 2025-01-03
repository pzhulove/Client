using UnityEngine;
using System.Collections;
using System;

namespace GameClient
{
    public class BaseWaitHttpRequest : BaseCustomEnum<BaseWaitHttpRequest.eState>, IEnumerator
    {
        public enum eState
        {
            None,
            Wait,
            GapWait,
            Success,
            Error,
            TimeOut,
        }

        private string mUrl = null;
        private WWW    mWWW = null;


        private int    mReconnectCnt = 0;

        private int    mTimeOut      = 0;
        private int    mTickTimeOut  = 0;
        private int    mGapTime      = 0;

#if !APPLE_STORE
        public const int kDefaultTimeOut        = 3000;
        public const int kDefaultGapTimeOut     = 1000;
        public const int kDefaultReconnectCount = 3;
#endif
        public BaseWaitHttpRequest()
        {
            mResult       = eState.None;

#if APPLE_STORE
            mTickTimeOut  = LoginConfigManager.instance.GetHttpDefaultTimeOut();
            mTimeOut      = LoginConfigManager.instance.GetHttpDefaultTimeOut();
            mGapTime      = LoginConfigManager.instance.GetHttpDefaultGapTimeOut();
            mReconnectCnt = LoginConfigManager.instance.GetHttpDefaultReconnectCount();
#else
            mTickTimeOut = mTimeOut = kDefaultTimeOut;
            mGapTime = kDefaultGapTimeOut;
            mReconnectCnt = kDefaultReconnectCount;
#endif
        }

        public string url
        {
            get 
            {
                return mUrl;
            }

            set 
            {
                if (eState.None == mResult)
                {
                    mUrl = value;
                    mWWW = new WWW(mUrl);
                }
                else
                {
                    UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] 错误状态 {0}, 无法设置Url地址 {1}", mResult, value);
                }
            }
        }

        public int timeout
        {
            get 
            {
                return mTimeOut;
            }

            set 
            {
                if (eState.None == mResult)
                {
                    mTickTimeOut = mTimeOut = value;
                }
                else
                {
                    UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] 错误状态 {0}, 无法设置超时时间 {1} ms", mResult, value);
                }
            }
        }

        public int gaptime
        {
            get
            {
                return mGapTime;
            }

            set
            {
                if (eState.None == mResult)
                {
                    mGapTime = value;
                }
                else
                {
                    UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] 错误状态 {0}, 无法设置间隔时间 {1} ms", mResult, value);
                }
            }
        }

        public int reconnectCnt
        {
            get
            {
                return mReconnectCnt;
            }

            set 
            {
                if (eState.None == mResult)
                {
                    mReconnectCnt = value;
                }
                else 
                {
                    UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] 错误状态 {0}, 无法设置重试次数 {1} 次", mResult, value);
                }
            }

        }

        protected void SetRequestWaitResult()
        {
            //mResult = eState.Wait;
        }

        public byte[] GetResultBytes()
        {
            if (mResult == eState.Success)
            {
                if (null != mWWW)
                {
                    return mWWW.bytes;
                }
            }

            return null;
        }

        public string GetResultString()
        {
            if (mResult == eState.Success)
            {
                if (null != mWWW)
                {
                    return mWWW.text;
                }
            }

            return null;
        }

        public Hashtable GetResultHashJson()
        {
            UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] 获取值 url {0}, {1}", url, GetResult());

            if (GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                try 
                {
                    return XUPorterJSON.MiniJsonExtensions.hashtableFromJson(mWWW.text);
                }
                catch(Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }

            return null;
        }

        public T GetResultJson<T>()
        {
            try
            {
                return LitJson.JsonMapper.ToObject<T>(GetResultString());
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[BaseWaitHttpRequest] Json解析出错 {0}", e.ToString());
            }

            return default(T);
        }

        public ArrayList GetResultJson()
        {
            UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] 获取值 url {0}, {1}", url, GetResult());

            try 
            {
                return XUPorterJSON.MiniJsonExtensions.arrayListFromJson(mWWW.text);
            }
            catch(Exception e)
            {
                Logger.LogError(e.ToString());
            }

            return null;
        }

#region IEnumerator implementation
        public bool MoveNext()
        {
            if (mWWW == null)
            {
                return false;
            }

            switch (mResult)
            {
                case eState.None:
                    mResult = eState.Wait;
                    break;
                case eState.Wait:
                    if (_isTimeUpInCurState())
                    {
                        if (_tryNextReconnect())
                        {
                            UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] HTTP地址{0}超时, 等待下一次连接, 剩余{1}次", url, reconnectCnt);
                            mResult = eState.GapWait;
                        }
                        else 
                        {
                            mResult = eState.TimeOut;
                        }
                    }
                    else if (mWWW.isDone)
                    {
                        if (!string.IsNullOrEmpty(mWWW.error))
                        {
                            if (_tryNextReconnect())
                            {
                                UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] HTTP地址{0}出错({1}), 等待下一次连接 ", url, mWWW.error);
                                mResult = eState.GapWait;
                            }
                            else
                            {
                                mResult = eState.Error;
                            }
                        }
                        else if (mWWW.responseHeaders.Count > 0)
                        {
                            mResult = eState.Success;
                        }
                    }
                    break;
                case eState.GapWait:
                    if (_isTimeUpInCurState())
                    {
                        _clearWWW();

                        mTickTimeOut = mTimeOut;
                        mWWW = new WWW(mUrl);

                        mResult = eState.Wait; 

                        UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] HTTP地址{0}, 重新请求地址, 设置超时时间: {1}", url, mTickTimeOut);
                    }
                    break;
                case eState.TimeOut:
                    UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] HTTP地址{0}, 超时", url);
                    _clearWWW();
                    return true;
                case eState.Error:
                    UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] HTTP地址{0}, 错误:{1}", url, mWWW.error);
                    _clearWWW();
                    return false;
                case eState.Success:
                    UnityEngine.Debug.LogFormat("[BaseWaitHttpRequest] HTTP地址{0}, 成功", url);
                    return false;
            }

            return true;
        }

        private bool _isTimeUpInCurState()
        {
            mTickTimeOut -= (int)(Time.unscaledDeltaTime * 1000);
            return mTickTimeOut < 0;
        }

        private bool _tryNextReconnect()
        {
            if (mReconnectCnt > 0)
            {
                mTickTimeOut = mGapTime;
                mReconnectCnt--;
                return true;
            }

            return false;
        }

        private void _clearWWW()
        {
            if (null != mWWW)
            {
                mWWW.Dispose();
            }

            mWWW = null;
        }

        public void Reset()
        {
            _clearWWW();
            mWWW = null;
            mUrl = null;
        }

        public object Current
        {
            get 
            { 
                return null;
            }
        }
#endregion
    }
}
