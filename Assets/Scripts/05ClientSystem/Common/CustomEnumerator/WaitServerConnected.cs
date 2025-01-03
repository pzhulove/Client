using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Protocol;
using Network;

namespace GameClient
{
    public class WaitServerConnected : BaseCustomEnum<WaitServerConnected.eResult>, IEnumerator
    {
        private const int kTimeOutSecond    = 4;
        private const int kGapTimeOutSecond = 1;
        private const int kReconnectCount   = 3;
        private const int kTimeOut          = kTimeOutSecond * 1000;

        protected ServerType mServerType  = ServerType.INVALID;

        protected float mWaitTime         = -1.0f;
        protected float mGapTime          = -1.0f;
        protected float mTickTime         = -1.0f;

        protected string mIp              = "";
        protected ushort mPort  		  = 0;
        protected uint   mAccid 		  = 0;  

        protected int   mReCnt 		      = 0;  

        private eState mState   		  = eState.Start;

        public enum eResult
        {
            Invalid,
            TimeOut,
            Success,
            Fail,
        }

        private enum eState
        {
            None,
            Start,
            Wait,
            Gap,
            Finish,
        }

        public WaitServerConnected(ServerType type, string ip, ushort port, uint accid, float time = kTimeOutSecond, int reconnectcnt = kReconnectCount, float gaptime = kGapTimeOutSecond)
        {
            mServerType = type;
            mWaitTime   = time;
            mGapTime    = gaptime;
            mTickTime   = time;

            mAccid      = accid;
            mPort       = port;
            mIp         = ip;

            mReCnt      = reconnectcnt;

            mState      = eState.None;
            mResult     = eResult.Invalid;
        }

        public object Current
        {
            get
            {
                return mState;
            }
        }

        private void _connectGateServer(string ip, ushort port)
        {
            NetManager.instance.ConnectToGateServerAsync(ip, port, kTimeOut, s =>
                    {
                    if (s) { mResult = eResult.Success; }
                    else { mResult = eResult.Fail; }
                    });
        }

        private void _connectAdminServer(string ip, ushort port)
        {
            NetManager.instance.ConnectToAdmainServerAsync(ip, port, kTimeOut, s =>
                    {
                    if (s) { mResult = eResult.Success; }
                    else { mResult = eResult.Fail; }
                    });
        }

        private void _connectRelayServer(string ip, ushort port, uint accid)
        {
            bool resulte = NetManager.instance.ConnectToRelayServer(ip, port, accid, kTimeOut);

            if (resulte)
            {
                mResult = eResult.Success;
            }
            else
            {
                mResult = eResult.Fail;
            }
        }

        public bool MoveNext()
        {
            switch (mState)
            {
                case eState.None:
                    WaitNetMessageFrame.TryOpen();
                    mState = eState.Start;

                    Logger.LogProcessFormat("[Connect] 开始连接服务器");
                    break;
                case eState.Start:
                    switch (mServerType)
                    {
                        case ServerType.ADMIN_SERVER:
                            _connectAdminServer(mIp, mPort);
                            break;
                        case ServerType.GATE_SERVER:
                            _connectGateServer(mIp, mPort);
                            break;
                        case ServerType.RELAY_SERVER:
                            _connectRelayServer(mIp, mPort, mAccid);
                            break;
                    }
                    Logger.LogProcessFormat("[Connect] 连接服务器 {0} {1}:{2}", mServerType, mIp, mPort);
                    mState = eState.Wait;
                    break;
                case eState.Gap:
                    {
                        if (_tryTickTime())
                        {
                            mState = eState.Start;
                            mTickTime = mWaitTime;

                            mResult = eResult.Invalid;

                            Logger.LogProcessFormat("[Connect] 开始下一次连接 剩余次数 {0}", mReCnt);
                        }
                        break;
                    }
                case eState.Wait:
                    if (mResult == eResult.Invalid)
                    {
                        if (_tryTickTime() && !_tryNextReconnect())
                        {
                            mResult = eResult.TimeOut;
                            mState = eState.Finish;
                        }
                    }
                    else
                    {
                        if (mResult != eResult.Success)
                        {
                            if (!_tryNextReconnect())
                            {
                                mState = eState.Finish;
                            }
                        }
                        else
                        {
                            mState = eState.Finish;
                        }
                    }
                    break;
                case eState.Finish:
                    Logger.LogProcessFormat("[Connect] 连接服务器结束 {0}", mResult);
                    WaitNetMessageFrame.TryClose();

                    if (mResult == eResult.Success)
                    {
                        switch (mServerType)
                        {
                            case ServerType.ADMIN_SERVER:
                                ClientReconnectManager.instance.canRelogin = true;
                                break;
                            case ServerType.RELAY_SERVER:
                                ClientReconnectManager.instance.canReconnectRelay = true;
                                break;
                        }
                    }

                    return false;
            }

            return true;
        }

        private bool _tryTickTime()
        {
            if (mTickTime > 0.0f)
            {
                mTickTime -= Time.unscaledDeltaTime;
                return false;
            }
            else
            {
                Logger.LogProcessFormat("[Connect] {0}状态 计时结束", mState);
                return true;
            }
        }

        private bool _tryNextReconnect()
        {
            if (mReCnt > 0)
            {
                mReCnt--;
                mTickTime = mGapTime;
                mState = eState.Gap;
                Logger.LogProcessFormat("[Connect] 等待 {0}秒 进入下一次连接", mGapTime);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            mState    = eState.None;
            mResult   = eResult.Invalid;

            mTickTime = mWaitTime;
        }

        //public eResult GetResult()
        //{
        //    return mResult;
        //}
    }

}
