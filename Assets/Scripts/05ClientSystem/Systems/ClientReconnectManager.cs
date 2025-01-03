using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;

namespace GameClient
{
    public sealed class ClientReconnectManager : Singleton<ClientReconnectManager>, IClientNet
    {
        public enum eState
        {
            onNormal,
            onReconnect,
            onError,
        }

        private eState mState                     = eState.onNormal;
        private List<ServerType> mDisconnectQueue = new List<ServerType>();
        private ServerType mLastReconnectType     = ServerType.INVALID;

        public void Clear()
        {
            Logger.LogProcessFormat("[Reconnect] 重置所有配置");

            mState             = eState.onNormal;
            mLastReconnectType = ServerType.INVALID;
            canReconnectGate   = false;
            canReconnectRelay  = false;
            canRelogin         = false;

            mDisconnectQueue.Clear();
        }

        public override void Init()
        {
            Clear();
        }

        public override void UnInit()
        {
            Clear();
        }


        private bool mCanRelogin = false;
        public bool canRelogin
        {
            get 
            {
                return mCanRelogin;
            }

            set 
            {
                mCanRelogin = value;
                Logger.LogProcessFormat("[Reconnect] 是否可以重登录 {0}", mCanRelogin);
            }
        }

        private bool mCanReconnectRelay = false;
        public bool canReconnectRelay
        {
            get
            {
                return mCanReconnectRelay;
            }
            set
            {
                mCanReconnectRelay = value;
                Logger.LogProcessFormat("[Reconnect] 是否重连Relay {0}", mCanReconnectRelay);
            }
        }

        private bool mCanReconnectGate = false;
        public bool canReconnectGate
        {
            get
            {
                return mCanReconnectGate;
            }

            set
            {
                mCanReconnectGate = value;
                Logger.LogProcessFormat("[Reconnect] 是否重连Gate {0}", mCanReconnectGate);
            }
        }

        public void ResumeTimeOut()
        {
            GameFrameWork.instance.StartCoroutine(_forceQuit2Login());
        }

#region 断线重连
        /// <summary>
        /// 断线
        ///  
        /// </summary>
        /// <param name="type"></param>
        public void OnDisconnect(ServerType type)
        {
            if (eState.onNormal == mState)
            {
                Logger.LogProcessFormat("[Reconnect] 当前状态 {0}, 开始重连 {1}", mState, type);

                mState = eState.onReconnect;
                mLastReconnectType = type;
                _OnDisconnect(type);
            }
            else
            {
                Logger.LogProcessFormat("[Reconnect] 在{0}状态，缓存重连消息 {1}", mState, type);
                mDisconnectQueue.Add(type);
            }
        }

        private void _OnDisconnect(ServerType type)
        {
            switch (type)
            {
                case ServerType.RELAY_SERVER:
                    if (canReconnectRelay)
                    {
                        GameFrameWork.instance.StartCoroutine(_reconnectRelayServer());
                    }
                    else
                    {
                        OnReconnect();
                    }
                    break;
                case ServerType.GATE_SERVER:
                    if (canRelogin)
                    {
                        OnReconnect();

                        if (ClientSystemLoginUtility.IsLogining())
                        {
                            Logger.LogProcessFormat("[Reconnect] 正在正常登录, 不重新登录");
                        }
                        else
                        {
                            Logger.LogProcessFormat("[Reconnect] 自动重新登录");
                            ClientSystemLoginUtility.StartLoginAfterVerify();
                        }
                    }
                    else if(canReconnectGate)
                    {
                        GameFrameWork.instance.StartCoroutine(_reconnectGateServer());
                    }
                    else
                    {
                        OnReconnect();
                    }
                    break;
                default:
                    {
                        OnReconnect();
                    }
                    break;
            }
        }

        private bool _updateDisconnectQueue()
        {
            if (eState.onNormal == mState)
            {
                mDisconnectQueue.RemoveAll(x => { return x == mLastReconnectType;});
                mLastReconnectType = ServerType.INVALID;

                int cnt = mDisconnectQueue.Count;

                if (cnt > 0)
                {
                    Logger.LogProcessFormat("[Reconnect] 执行缓存队列中的重连命令, 当前状态 {0}", mState);

                    ServerType type = mDisconnectQueue[cnt - 1];
                    mDisconnectQueue.RemoveAt(cnt - 1);
                    OnDisconnect(type);
                    return true;
                }
                else
                {
                    Logger.LogProcessFormat("[Reconnect] 缓存队列中的重连命令为空, 当前状态 {0}", mState);
                }
            }

            Logger.LogProcessFormat("[Reconnect] 无法执行缓存中的重连命令, 当前状态 {0}", mState);

            return false;
        }

        
        public void OnReconnect()
        {
            if (eState.onReconnect == mState)
            {
                mState = eState.onNormal;

                _tryPauseBattle(false);

                Logger.LogProcessFormat("[Reconnect] 重连{0}结束, 当前状态 {1}", mLastReconnectType, mState);

                _updateDisconnectQueue();
            }
        }

        public void OnReconnectError()
        {
            mState = eState.onError;
        }

        private void _tryPauseBattle(bool pause)
        {
            if (null == BattleMain.instance)
            {
                return ;
            }

            if (null == BattleMain.instance.GetDungeonManager())
            {
                return ;
            }

            Logger.LogProcessFormat("[Reconnect] 重连要去暂停战斗了 {0}", pause);

            if (pause)
            {
                BattleMain.instance.GetDungeonManager().PauseFight(true, "ClientReconnect");
            }
            else
            {
                BattleMain.instance.GetDungeonManager().ResumeFight(true, "ClientReconnect");
            }
        }
#endregion

       

#region ReconnectGateServer
        private const int kReconnectWaitGapFrameCount = 33;
        private IEnumerator _reconnectGateServerEnd()
        {
            OnReconnect();
            yield break;
        }


        private IEnumerator _reconnectGateServerProcess()
        {
            for (int i = 0; i < kReconnectWaitGapFrameCount; ++i) yield return null;

            _tryPauseBattle(true);

            bool isUserCancel = false;

            while (!isUserCancel)
            {
                WaitServerConnected waitConnect = new WaitServerConnected(
                        ServerType.GATE_SERVER,
                        ClientApplication.gateServer.ip,
                        ClientApplication.gateServer.port,
                        0);

                yield return waitConnect;

                if (waitConnect.GetResult() != WaitServerConnected.eResult.Success)
                {
                    bool waitClick = false;

                    SystemNotifyManager.SystemNotifyOkCancel(8503,
                    ()=> { waitClick = true;},
                    ()=> { waitClick = true; isUserCancel = true;},
                    FrameLayer.TopMost);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DisConnect);

                    while (!waitClick) yield return null;                
                }
                else
                {
                    break;
                }
            }

            if (isUserCancel)
            {
                yield return new NormalCustomEnumError(string.Format("[net] 用户取消重连，返回登录 {0}:{1}", ClientApplication.gateServer.ip, ClientApplication.gateServer.port), 
                        eEnumError.UserCancelReconnect2Login
                        );
                yield break;
            }

            {
                Protocol.GateReconnectGameReq req = new Protocol.GateReconnectGameReq
                {
                    roleId = PlayerBaseData.GetInstance().RoleID,
                    sequence = NetProcess.Instance().recvMaxSequence,
                    accid = ClientApplication.playerinfo.accid,
                    session = ClientApplication.playerinfo.hashValue
                };
                MessageEvents events = new MessageEvents();
                Protocol.GateReconnectGameRes res = new Protocol.GateReconnectGameRes();

#if false//!LOGIC_SERVER && MG_TEST
               int tryCount = Global.Settings.gateReconnectSendTryCount;
                do
                {
                    tryCount--;
                    yield return MessageUtility.Wait<Protocol.GateReconnectGameReq, Protocol.GateReconnectGameRes>(ServerType.GATE_SERVER, events, req, res, true, Global.Settings.gateReconnectTimeOut);
                } while (tryCount > 0 && !events.IsAllMessageReceived());
#else
                yield return MessageUtility.Wait<Protocol.GateReconnectGameReq, Protocol.GateReconnectGameRes>(ServerType.GATE_SERVER, events, req, res, true);
#endif
                if (events.IsAllMessageReceived())
                {
                    if (res.result != 0)
                    {
                        if (res.result == (uint)ProtoErrorCode.RECONNECT_PLAYER_ONLINE)
                        {
                            yield return new NormalCustomEnumError(string.Format("[net] 玩家已经在线 {0}", res.result), eEnumError.ReconnectPlayerOnline);
                        }
                        else if (res.result == (uint)ProtoErrorCode.RECONNECT_PLAYER_DELETED)
                        {
                            yield return new NormalCustomEnumError(string.Format("[net] 玩家已经离线 {0}", res.result), eEnumError.ReconnectPlayerOffline);
                        }
                        else if (res.result == (uint)ProtoErrorCode.RECONNECT_INVALID_SEQUENCE)
                        {
                            yield return new NormalCustomEnumError(string.Format("[net] 重连发送的Seq无效 {0}", res.result), eEnumError.ReconnectPlayerInvalidSequence);
                        }
                        else
                        {
                            yield return new NormalCustomEnumError(string.Format("[net] 重连消息超时 {0}", res.result), eEnumError.ReconnectOtherError);
                            SystemNotifyManager.SystemNotify((int)res.result);
                        }

                        yield break;
                    }
                    else
                    {
                   //     if (ClientApplication.isOpenNewReconnectAlgo)
                   //     {
#if !LOGIC_SERVER && MG_TEST
                            RecordServer.instance.PushReconnectCmd("[断线重连]_reconnectGateServerProcess");
#endif
                            if (!NetManager.Instance().ReSendCommand(res.lastRecvSequence))
                            {
                                NetManager.Instance().ClearReSendData();
#if MG_TEST_EXTENT
                                NetManager.Instance().ResetResend();
#endif
                            yield return new NormalCustomEnumError(string.Format("[net] 重连后重发消息失败 sequence:{0}", res.lastRecvSequence), eEnumError.ProcessError);
                                yield break;
                            }
#if !LOGIC_SERVER && MG_TEST
                            RecordServer.instance.PushReconnectCmd("[断线重连成功]_reconnectGateServerProcess");
#endif
                            NetManager.Instance().OnCanSendData(ServerType.GATE_SERVER);
                   //     }
                        //else
                        //{
                        //    NetManager.Instance().ClearReSendData();
                        //}

                        //if (!ClientApplication.isOpenNewReconnectAlgo && null != BattleMain.instance && null != BattleMain.instance.GetDungeonManager())
                        //{
                        //    BattleMain.instance.GetDungeonManager().GetDungeonDataManager().ResetLastFrameIndexClientSendedToServerRecived();
                        //}
                        // TODO normalProcess
                        //NetManager.Instance().ReSendCommand(res.lastRecvSequence);
                    }
                }
                else
                {
                    yield return new NormalCustomEnumError(string.Format("[net] 重连消息超时 GateReconnectGameRes {0}", res.result), eEnumError.ProcessError);
                }
            }
        }

        private IEnumerator _reconnectServerErrorHandle(eEnumError error, string msg)
        {
            Logger.LogErrorFormat("{0}, {1}", error, msg);

            OnReconnectError();

            switch (error)
            {
                case eEnumError.NetworkErrorDisconnect:
                    ClientSystemManager.instance.QuitToLoginSystem(8401);
                    break;
                case eEnumError.ProcessError:
                    ClientSystemManager.instance.QuitToLoginSystem(8402);
                    break;
                case eEnumError.ReconnectPlayerInvalidSequence:
                    ClientSystemManager.instance.QuitToLoginSystem(8403);
                    break;
                case eEnumError.ReconnectOtherError:
                    ClientSystemManager.instance.QuitToLoginSystem(8404);
                    break;
                case eEnumError.ReconnectPlayerOnline:
                    ClientSystemManager.instance.QuitToLoginSystem(8405);
                    break;
                case eEnumError.ReconnectPlayerOffline:
                    ClientSystemManager.instance.QuitToLoginSystem(8406);
                    break;
                case eEnumError.ResumeTimeOut:
                case eEnumError.UserCancelReconnect2Login:
                case eEnumError.ReloginFail:
                    // 这里开启一个携程主要是为了解决
                    // _QuitToLoginImpl中会清楚所有迭代器的管理
                    // 会导致最后一步的MoveNext的出错
                    // 所以这个开启一个携程，等到下一帧去执行
                    GameFrameWork.instance.StartCoroutine(_forceQuit2Login());
                    break;
            }
            yield break;
        }

        private IEnumerator _forceQuit2Login()
        {
            yield return null;
            ClientSystemManager.instance._QuitToLoginImpl();
        }

        private IEnumerator _reconnectGateServer()
        {
            IEnumerator process = _reconnectGateServerProcess();

            ThreeStepProcess _3step = new ThreeStepProcess(
                    "ReconnectGateServer",
                    ClientSystemManager.instance.enumeratorManager,
                    process,
                    null,
                    _reconnectGateServerEnd());

            _3step.SetErrorProcessHandle(_reconnectServerErrorHandle);

            return _3step;
        }

#endregion

#region ReconnectRelayServer()

        private IEnumerator _reconnectRelayServerProcess()
        {
            _tryPauseBattle(true);

            for (int i = 0; i < kReconnectWaitGapFrameCount; ++i) yield return null;

            bool isUserCancel = false;

            while (!isUserCancel)
            {
                WaitServerConnected waitConnect = new WaitServerConnected(ServerType.RELAY_SERVER,
                        ClientApplication.relayServer.ip,
                        ClientApplication.relayServer.port,
                        ClientApplication.playerinfo.accid
                        );

                yield return waitConnect;

                if (waitConnect.GetResult() != WaitServerConnected.eResult.Success)
                //if (!NetManager.instance.IsConnected(ServerType.RELAY_SERVER))
                {
                    bool waitClick = false;

                    SystemNotifyManager.SystemNotifyOkCancel(8503,
                    () => { waitClick = true; },
                    () => { waitClick = true; isUserCancel = true; },
                    FrameLayer.TopMost);

                    while (!waitClick) yield return null;
                }
                else
                {
                    break;
                }
            }

            if (isUserCancel)
            {
                yield return new NormalCustomEnumError(string.Format("[net] 用户取消重连，返回登录"), eEnumError.UserCancelReconnect2Login);
                yield break;
            }

            if (null == BattleMain.instance)
            {
                yield return new NormalCustomEnumError(string.Format("[net] relay 异常返回 BattleMain.instance == null"), eEnumError.ProcessError);
                yield break;
            }

            if (null == BattleMain.instance.GetPlayerManager())
            {
                yield return new NormalCustomEnumError(string.Format("[net] relay 异常返回 BattleMain.instance.GetPlayerManager() == null"), eEnumError.ProcessError);
                yield break;
            }

            var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            if (!BattlePlayer.IsDataValidBattlePlayer(mainPlayer))
            {
                yield return new NormalCustomEnumError(string.Format("[net] relay 异常返回 BattleMain.instance.GetPlayerManager().GetMainPlayer() == null"), eEnumError.ProcessError);
                yield break;
            }

            var req = new RelaySvrReconnectReq
            {
                accid = ClientApplication.playerinfo.accid,
                lastFrame = FrameSync.instance.lastSvrFrame,
                seat = mainPlayer.playerInfo.seat,
                roleid = PlayerBaseData.GetInstance().RoleID,
                session = ClientApplication.playerinfo.session
            };
            var msg = new MessageEvents();
            var res = new RelaySvrReconnectRes();
#if false//!LOGIC_SERVER && MG_TEST
            if (RecordServer.GetInstance() != null)
                RecordServer.GetInstance().PushReconnectCmd(string.Format("RelaySvrReconnectReq lastFrame {0}", req.lastFrame));
           
            int tryCount = Global.Settings.relayReconnectSendTryCount;
            do
            {

                tryCount--;
                yield return (MessageUtility.Wait<RelaySvrReconnectReq, RelaySvrReconnectRes>(ServerType.RELAY_SERVER, msg, req, res, true, Global.Settings.relayReconnectTimeOut));

            } while (!msg.IsAllMessageReceived() && tryCount > 0);
#else
            yield return (MessageUtility.Wait<RelaySvrReconnectReq, RelaySvrReconnectRes>(ServerType.RELAY_SERVER, msg, req, res, true, 10));
#endif
            if (msg.IsAllMessageReceived())
            {
                if (res.result != 0)
                {
                    yield return new NormalCustomEnumError(string.Format("[net] relay 异常返回"), eEnumError.ProcessError);
                    yield break;
                }
                else
                {
                }
            }
            else
            {
                yield return new NormalCustomEnumError(string.Format("[net] relay超时"), eEnumError.ProcessError);
                yield break;
            }
        }

        private IEnumerator _reconnectRelayServer()
        {
            IEnumerator process = _reconnectRelayServerProcess();

            ThreeStepProcess _3step = new ThreeStepProcess(
                    "ReconnectRelayServer",
                    ClientSystemManager.instance.enumeratorManager,
                    process,
                    null,
                    _reconnectGateServerEnd());

            _3step.SetErrorProcessHandle(_reconnectServerErrorHandle);

            return _3step;
        }
#endregion
    }
}
