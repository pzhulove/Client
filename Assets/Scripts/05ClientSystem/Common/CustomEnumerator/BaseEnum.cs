using UnityEngine;
using System.Collections;
using System;

namespace GameClient
{
    public interface ICustomEnumResult<T> where T : IComparable
    {
        T GetResult();
    }

    public interface ICustomEnumError
    {
        string GetErrorMsg();

        void SetErrorMsg(string msg);

        eEnumError GetErrorType();
    }

    public enum eEnumError
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        UnkownError,

        /// <summary>
        /// 网络回包慢的
        /// </summary>
        NetworkErrorSlowResponse,

        /// <summary>
        /// 网络连接错误
        /// </summary>
        NetworkErrorDisconnect,

        /// <summary>
        /// 流程逻辑错误
        /// </summary>
        ProcessError,

        /// <summary>
        /// 玩家还在线
        /// </summary>
        ReconnectPlayerOnline,

        /// <summary>
        /// 玩家不在线
        /// </summary>
        ReconnectPlayerOffline,

        /// <summary>
        /// 玩家重连seq非法
        /// </summary>
        ReconnectPlayerInvalidSequence,

        /// <summary>
        /// 其他错误
        /// </summary>
        ReconnectOtherError,

        /// <summary>
        /// 用户取消重连请求 
        /// </summary>
        UserCancelReconnect2Login,

        /// <summary>
        /// 重新登录失败
        /// </summary>
        ReloginFail,

        /// <summary>
        /// 从后台返回过久 
        /// </summary>
        ResumeTimeOut,
    }

    public class NormalCustomEnumError : ICustomEnumError
    {
        private string mErrorMsg;
        private eEnumError mErrorType;

        public NormalCustomEnumError(string error, eEnumError type = eEnumError.ProcessError)
        {
            mErrorMsg = error;
            mErrorType = type;
        }

        public string GetErrorMsg()
        {
            return mErrorMsg;
        }

        public void SetErrorMsg(string msg)
        {
            mErrorMsg = msg;
        }

        public eEnumError GetErrorType()
        {
            return mErrorType;
        }
    }

    /// <summary>
    /// 所有自定义的迭代器
    /// 需要继承自这个基类
    ///
    /// 传入结果的枚举
    /// </summary>
    public class BaseCustomEnum<T> : ICustomEnumResult<T> where T : IComparable
    {
        protected T mResult = default(T);

        public T GetResult()
        {
            return mResult;
        }

        public void SetResult(T res)
        {
            Logger.LogProcessFormat("[BaseCustomEnum] state {0} -> {1}", mResult, res);
            mResult = res;
        }
    }
}

