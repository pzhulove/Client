using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using Network;
using System.IO;
using System;
using Client;

namespace GameClient
{
    public class ClientSystemLoginUtility
    {
        private static UnityEngine.Coroutine mLoginProcess = null;
        private static Coroutine mFetchAppPackageProcess = null;

        public static void StartLoginAfterVerify()
        {
            StopLoginAfterVerify();

            Logger.LogProcessFormat("[登录] 开始登录流程");

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginStart);

            mLoginProcess = GameFrameWork.instance.StartCoroutine(_loginAfterVerify());
        }

        public static void StopLoginAfterVerify()
        {
            if (null != mLoginProcess && IsLogining())
            {
                GameFrameWork.instance.StopCoroutine(mLoginProcess);
                mLoginProcess = null;

                Logger.LogProcessFormat("[登录] 停止登录流程");
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
            }
            else
            {
                Logger.LogProcessFormat("[登录] 不在登录过程中，无须停止");
            }
        }

        private static bool mIsLogin = false;

        public static bool IsLogining()
        {
            return mIsLogin;
        }

        private static IEnumerator _loginAfterVerify()
        {
            mIsLogin = true;

            if (string.IsNullOrEmpty(ClientApplication.adminServer.ip)
    		|| 0 == ClientApplication.adminServer.port)
            {
                Logger.LogErrorFormat("[登录] 登录地址非法");
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);


                mIsLogin = false;
                yield break;
            }

            // 连接登录服务器
            {
                WaitServerConnected conAdminServer = new WaitServerConnected(
                        ServerType.ADMIN_SERVER,
                        ClientApplication.adminServer.ip,
                        ClientApplication.adminServer.port,
                        0
                );

                yield return conAdminServer;

                if (conAdminServer.GetResult() != WaitServerConnected.eResult.Success)
                {
                    Logger.LogErrorFormat("[登录] 登录服务器连接失败");
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFailWithServerConnect);
                    mIsLogin = false;
                    yield break;
                }
            }

            // 给登录服务器发送认证消息
            {
                AdminLoginVerifyReq req = new AdminLoginVerifyReq();
                AdminLoginVerifyRet res = new AdminLoginVerifyRet();
                MessageEvents message   = new MessageEvents();


                byte[] tableBytes = new byte[0];
                {
                    string dataPath           = "AssetBundles/data_table.pck";
#if USE_FB_TABLE
                    dataPath = "AssetBundles/data_table_fb.pck";
#endif

                    string persistentDataPath = System.IO.Path.Combine(Application.persistentDataPath, dataPath);
                    string path               = string.Empty;
#if UNITY_ANDROID
                    path = "jar:file://" + Application.dataPath + "!/assets/" + dataPath;
#else
                    path = System.IO.Path.Combine(Application.streamingAssetsPath, dataPath);
#endif
                    UnityEngine.Debug.LogFormat("table-data-path {0}", path);

                    if (File.Exists(persistentDataPath))
                    {
                        tableBytes = File.ReadAllBytes(persistentDataPath);
                    }
                    else
                    {
#if UNITY_ANDROID
                        WWW www = new WWW(path);

                        yield return www;

                        if (www.isDone)
                        {
                            tableBytes = www.bytes;
                        }
#else
                        if (File.Exists(path))
                        {
                            tableBytes = System.IO.File.ReadAllBytes(path);
                        }
#endif 
                    }

                    UnityEngine.Debug.LogFormat("table-data len:{0}", tableBytes.Length);
                }

                byte[] skillBytes = new byte[0];
                {
                    string dataPath           = "AssetBundles/data_skilldata.pck";

                    string persistentDataPath = System.IO.Path.Combine(Application.persistentDataPath, dataPath);
                    string path               = string.Empty;
#if UNITY_ANDROID
                    path = "jar:file://" + Application.dataPath + "!/assets/" + dataPath;
#else
                    path = System.IO.Path.Combine(Application.streamingAssetsPath, dataPath);
#endif
                    UnityEngine.Debug.LogFormat("skill-data path {0}", path);


                    if (File.Exists(persistentDataPath))
                    {
                        skillBytes = File.ReadAllBytes(persistentDataPath);
                    }
                    else
                    {
#if UNITY_ANDROID
                        WWW www = new WWW(path);

                        yield return www;

                        if (www.isDone)
                        {
                            skillBytes = www.bytes;
                        }
#else
                        if (File.Exists(path))
                        {
                            skillBytes = System.IO.File.ReadAllBytes(path);
                        }
#endif 
                    }
                    UnityEngine.Debug.LogFormat("skill-data len:{0}", skillBytes.Length);
                }


                req.param               = ClientApplication.playerinfo.param;
                req.hashValue           = ClientApplication.playerinfo.hashValue;
                req.source1             = "a";
                req.source2             = "b";
                req.version             = VersionManager.instance.ServerVersion();

                string fileMd5Str = DungeonUtility.GetMD5Str(tableBytes).ToLower() + 
                                    DungeonUtility.GetMD5Str(skillBytes).ToLower() + req.param;

                Logger.LogProcessFormat("[登录] 原来 {0}", fileMd5Str);

                req.tableMd5            = DungeonUtility.GetMD5(fileMd5Str);

#if UNITY_EDITOR
                if (tableBytes.Length == 0)
                {
                    req.tableMd5            = new byte[16];
                }
#endif

                yield return MessageUtility.Wait<AdminLoginVerifyReq, AdminLoginVerifyRet>(ServerType.ADMIN_SERVER, message, req, res, true);

                // 超时
                if (!message.IsAllMessageReceived())
                {
                    Logger.LogErrorFormat("[登录] 登录服务器消息超时");
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFailWithServerConnect);
                    mIsLogin = false;
                    yield break;
                }

                // 错误返回
                if (res.result != 0)
                {
                    if (res.errMsg != null && res.errMsg.Length > 0)
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOK(res.errMsg);
                    } 
                    else
                    {
                
                        if ((int)res.result == (int)ProtoErrorCode.LOGIN_ERROR_VERSION)
                        {
#if UNITY_ANDROID
                                /// 询问用户是否跳转
                                SystemNotifyManager.SysNotifyMsgBoxOkCancel(ClientConfig.AppPackageMessageLoginFailed,
                                    ClientConfig.AppPackageButTextRetryLogin,
                                    ClientConfig.AppPackageButTextOpenURL,
                                    () =>
                                    {
                                        //SDKInterface.instance.GetNewVersionInAppstore();
                                        ClientSystemManager.instance.SwitchSystem<ClientSystemVersion>();
                                        Client.AppPackageFetcher.SkipFetchAgain(true);
                                    },
                                    () =>
                                    {
                                        mFetchAppPackageProcess = GameFrameWork.instance.StartCoroutine(_OpenAppPackageUrl());
                                        Client.AppPackageFetcher.SkipFetchAgain(true);
                                    }
                                    );
#else
                                SystemNotifyManager.SystemNotify((int)ProtoErrorCode.LOGIN_ERROR_VERSION, () =>
                                {
                                    //SDKInterface.instance.GetNewVersionInAppstore();
                                    ClientSystemManager.instance.SwitchSystem<ClientSystemVersion>();
                                });
#endif
                         }
				 		else
                         {
                             SystemNotifyManager.SystemNotify((int)res.result);
                         }
                 
                     }
                 
                     Logger.LogErrorFormat("[登录] 登录服务器消息结果出错 {0}", res.result);
                 
                     UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                     mIsLogin = false;
                     yield break;
                 }

                // 正常返回
                ClientApplication.playerinfo.accid = res.accid;
                ClientApplication.gateServer.ip    = res.addr.ip;
                ClientApplication.gateServer.port  = res.addr.port;
                ClientApplication.adminServer.dirSig = res.dirSig;
#if APPLE_STORE
				Network.NetManager.Instance().SetIsTcp(res.battleUseTcp == 1 ? true : false);
				ClientApplication.isOpenNewReconnectAlgo = res.openNewReconnectAlgo != 0;
                ClientApplication.isOpenNewReportFrameAlgo = res.openNewReportFrameAlgo != 0;
#endif
                // add by mjx on 170804 for mobile bind
                // etond: move from ClientSystemLogin.cs at 170908
                MobileBind.MobileBindManager.GetInstance().BindMobileRoleId = res.phoneBindRoleId;
                bool isBind = PluginManager.instance.BindOtherNameForServicePush("android_" + res.accid);
				
				ClientApplication.isEncryptProtocol = res.isEncryptProtocol != 0;
                if(res.replayAgentAddr == null || res.replayAgentAddr.Trim().Equals(""))
                {
                    ClientApplication.replayServer = "127.0.0.1";
                }
                else
                {
                    ClientApplication.replayServer = res.replayAgentAddr;
                }
                ClientApplication.channelRankListServer = res.activityYearSortListUrl;
                ClientApplication.operateAdsServer = res.webActivityUrl;
                ClientApplication.commentServerAddr = res.commentServerAddr;
                ClientApplication.redPackRankServerPath = res.redPacketRankUrl;
                ClientApplication.convertAccountInfoUrl = res.convertUrl;
                ClientApplication.reportPlayerUrl = res.reportUrl;
                ClientApplication.questionnaireUrl = res.writeQuestionnaireUrl;

                if (ClientApplication.playerinfo != null) {
                    ClientApplication.playerinfo.serverID = res.serverId;
                    Logger.LogProcessFormat("[登录] 登录服务器返回，服务器id{0}", res.serverId);
                }

                //set voice init flag
                VoiceSDK.SDKVoiceManager.GetInstance().InitVoiceSDK(res.voiceFlag);
            }

            // 连接Gate服务器
            {
                WaitServerConnected conGateServer = new WaitServerConnected(
                        ServerType.GATE_SERVER, 
                        ClientApplication.gateServer.ip, 
                        ClientApplication.gateServer.port,
                        ClientApplication.playerinfo.accid);

                yield return conGateServer;

                if (conGateServer.GetResult() != WaitServerConnected.eResult.Success)
                {
                    Logger.LogErrorFormat("[登录] 连接Gate服务器消息结果出错 {0}", conGateServer.GetResult() );
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                    mIsLogin = false;
                    yield break;
                }
            }

            // 发消息给Gate服务器 登录消息
            {
                GateClientLoginReq req = new GateClientLoginReq();
                GateClientLoginRet res = new GateClientLoginRet();
                MessageEvents msg      = new MessageEvents();

                req.accid              = ClientApplication.playerinfo.accid;
                req.hashValue          = ClientApplication.playerinfo.hashValue;

                yield return MessageUtility.Wait<GateClientLoginReq, GateClientLoginRet>(ServerType.GATE_SERVER, msg, req, res, true);

                // 超时
                if (!msg.IsAllMessageReceived())
                {
                    Logger.LogErrorFormat("[登录] 连接Gate服务器消息超时");

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                    mIsLogin = false;
                    yield break;
                }

                // 更新开服时间
                ClientApplication.serverStartTime = res.serverStartTime;

                ClientApplication.veteranReturn = res.notifyVeteranReturn;

                if(ClientApplication.veteranReturn == 1 && ClientSystemManager.GetInstance().IsFrameOpen<SelectRoleFrame>())
                {
                    if(!ClientSystemManager.GetInstance().IsFrameOpen<OldPlayerFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<OldPlayerFrame>();
                    }
                }
                //xzl
                //try
                //{
                //    //add by mjx for server time
                //    AdsPush.AdsPushServerDataManager.GetInstance().RecStartServerTime(ClientApplication.serverStartTime);
                //}
                //catch (System.Exception e)
                //{
                //    Logger.LogErrorFormat("[登录] 推送相关初始化 出错 {0}", e.ToString());
                //}


                // 错误返回
                if (res.result != 0)
                {
                    if ((uint)ProtoErrorCode.LOGIN_WAIT == res.result )
                    {
                        if (res.waitPlayerNum > 0)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginQueueWait, res.waitPlayerNum);
                        }
                        else
                        {
                            Logger.LogErrorFormat("[登录] WaitQueue服务 等待人数为0 ");
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("[登录] Gate消息 错误返回{0}", res.result);

                        SystemNotifyManager.SystemNotify((int)res.result);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                    }

                    mIsLogin = false;
                    yield break;
                }

                // 正常返回
                Logger.LogProcessFormat("[登录] 登录成功");
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginSuccess);
                mIsLogin = false;

                SecurityLockDataManager.GetInstance().InitiallizeSystem();
            }
        }

        private static IEnumerator _OpenAppPackageUrl()
        {
            yield return Client.AppPackageFetcher.FetchFullAppPackage();
            AppPackageFetcher.OpenAppPackageURL();
        }
    }
}
