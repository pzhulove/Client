using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouMe;
using XUPorterJSON;

namespace VoiceSDK
{
    public class SDKVoiceCallback : MonoSingleton<SDKVoiceCallback>
    {
        SDKVoiceEventArgs args = null;

        public void Start()
        { 
            GameObject.DontDestroyOnLoad(gameObject);            
        }

        public override void Init()
        {
            //Logger.LogErrorFormat("SDKVoiceCallback init");
            args = new SDKVoiceEventArgs(SDKVoiceEventType.None);
        }

        public void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                SDKVoiceManager.GetInstance().OnPause();
                SDKVoiceManager.GetInstance().PauseChannel();
            }
            else
            {
                SDKVoiceManager.GetInstance().OnResume();
                SDKVoiceManager.GetInstance().ResumeChannel();
            }
        }

        protected override void OnApplicationQuit()
        {
            //Logger.LogError("[SDKVoiceCallback] - Start OnDestroy !!!");

            OnVoiceDestroy();

            //Logger.LogError("[SDKVoiceCallback] - End OnDestroy !!!");

            base.OnApplicationQuit();
        }

        protected override void OnDestroy()
        {
            //Logger.LogError("[SDKVoiceCallback] - Start OnDestroy !!!");

            OnVoiceDestroy();

            //Logger.LogError("[SDKVoiceCallback] - End OnDestroy !!!");

            base.OnDestroy();
        }

        private void OnVoiceDestroy()
        {
            if (!SDKVoiceManager.GetInstance().IsInited)
            {
                //Logger.LogError("[SDKVoiceCallback] - OnYoumiVoiceDestroy SDKVoiceManager is not init !!!");
                return;
            }

            //需要在登出之前清理 不同于ClearVoiceCache()
            //SDKVoiceManager.GetInstance().ClearVoiceChatCache();

            //聊天语音登出
            SDKVoiceManager.GetInstance().LeaveVoiceSDK(true);

            SDKVoiceManager.GetInstance().UnInit();
        }


        #region YM Talk Voice Callback Listener

        public void OnEvent(string strParam)
        {
            string[] strSections = strParam.Split(new char[] { ',' }, 4);
            if (strSections == null)
            {
                Logger.LogError("SDKVoiceCallback onEvent strParams split is error");
                return;
            }

            int strSectionsOne;
            YouMe.YouMeEvent eventType = YouMeEvent.YOUME_EVENT_INIT_FAILED;
            if (int.TryParse(strSections[0], out strSectionsOne))
            {
                eventType = (YouMeEvent)strSectionsOne;
                Logger.LogProcessFormat("Talk Real YouMe OnEvent Callback | EventType : {0}", eventType.ToString());
            }
            else
            {
                Logger.LogProcessFormat(" SDKVoiceCallback onEvent YouMeEvent res is error !!!!!!");
            }
            int strSectionsTwo;
            YouMe.YouMeErrorCode errorCode = YouMeErrorCode.YOUME_ERROR_NOT_INIT;
            if (int.TryParse(strSections[1], out strSectionsTwo))
            {
                errorCode = (YouMeErrorCode)strSectionsTwo;
            }
            else
            {
                Logger.LogProcessFormat(" SDKVoiceCallback onEvent YouMeErrorCode res is error !!!!!!");
            }
            string channelID = strSections[2];
            string param = strSections[3];

            
            switch (eventType)
            {
                //对eventType的case列举请查询枚举类型YouMeEvent的定义，以下只是部分列举
                //使用者请按需自行添加或删除
                case YouMe.YouMeEvent.YOUME_EVENT_INIT_OK:
                    //"初始化成功";

                    args.Reset(SDKVoiceEventType.TalkInitSucc);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_INIT_FAILED:
                    //"初始化失败，错误码：" + errorCode;

                    args.Reset(SDKVoiceEventType.TalkInitFailed);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_JOIN_OK:
                    //加入频道成功

                    args.Reset(SDKVoiceEventType.TalkJoinChannel, true, channelID);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LEAVED_ALL:
                    //"离开所有频道成功";

                    args.Reset(SDKVoiceEventType.TalkLeaveChannel, true);

                    //Logger.LogProcessFormat("SDK Voice - lastRealTalkVoiceScene is {0}",SDKVoiceManager.GetInstance().lastRealTalkVoiceScene.ToString());

                    //设置退出场景为完全离开状态  上个状态是
                    //if (SDKVoiceManager.GetInstance().realTalkVocieScene == RealTalkVoiceScene.TeamBattle ||
                    //    (SDKVoiceManager.GetInstance().realTalkVocieScene == RealTalkVoiceScene.Pvp3v3Room && 
                    //    SDKVoiceManager.GetInstance().lastRealTalkVoiceScene == RealTalkVoiceScene.Pvp3v3Battle))
                    //{
                    //    SDKVoiceManager.GetInstance().SetCurrRealVoiceScene(RealTalkVoiceScene.None);
                    //}
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LEAVED_ONE:
                    //退出单个语音频道完成

                    //if (onLeaveChannelSucc != null)
                    //{
                    //    onLeaveChannelSucc();
                    //}
                    args.Reset(SDKVoiceEventType.TalkLeaveChannel, true, channelID);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_JOIN_FAILED:
                    //进入语音频道失败

                    args.Reset(SDKVoiceEventType.TalkJoinChannel, false, channelID);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_REC_PERMISSION_STATUS:
                    //"录音启动失败（此时不管麦克风mute状态如何，都没有声音输出";
                    //通知录音权限状态，成功获取权限时错误码为YOUME_SUCCESS，获取失败为YOUME_ERROR_REC_NO_PERMISSION（此时不管麦克风mute状态如何，都没有声音输出）
                    if(errorCode != YouMe.YouMeErrorCode.YOUME_SUCCESS)
                    {
                        args.Reset(SDKVoiceEventType.TalkMicAuthNoPermission);
                    }
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_RECONNECTING:
                    //"断网了，正在重连";
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_RECONNECTED:
                    //"断网重连成功";
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_MIC_OFF:
                    //其他用户的麦克风关闭：param是关闭用户的userid

                    args.Reset(SDKVoiceEventType.TalkChannelOtherMicChanged, false, param);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_MIC_ON:
                    //其他用户的麦克风打开

                    args.Reset(SDKVoiceEventType.TalkChannelOtherMicChanged, true, param);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_SPEAKER_ON:
                    //其他用户的扬声器打开
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_SPEAKER_OFF:
                    //其他用户的扬声器关闭
                    break;

                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_VOICE_ON:
                    //其他用户开始讲话

                    args.Reset(SDKVoiceEventType.TalkChannelOtherSpeak, true, param);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_VOICE_OFF:
                    //其他用户结束讲话

                    args.Reset(SDKVoiceEventType.TalkChannelOtherSpeak, false, param);

                    break;

                case YouMe.YouMeEvent.YOUME_EVENT_MY_MIC_LEVEL:
                    //麦克风的语音级别，把errorCode转为整形即是音量值
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_MIC_CTR_ON:
                    //麦克风被其他用户打开

                    args.Reset(SDKVoiceEventType.TalkMicChangeByOther, true, param);
                
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_MIC_CTR_OFF:
                    //麦克风被其他用户关闭

                    args.Reset(SDKVoiceEventType.TalkMicChangeByOther, false, param);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_SPEAKER_CTR_ON:
                    //扬声器被其他用户打开
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_SPEAKER_CTR_OFF:
                    //扬声器被其他用户关闭
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LISTEN_OTHER_ON:
                    //取消屏蔽某人语音
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LISTEN_OTHER_OFF:
                    //屏蔽某人语音
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_PAUSED:
                    //暂停语音频道完成
                    args.Reset(SDKVoiceEventType.TalkPauseChannel);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_RESUMED:
                    //恢复语音频道完成
                    args.Reset(SDKVoiceEventType.TalkResumeChannel);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_MIC_OFF:

                    args.Reset(SDKVoiceEventType.TalkMicSwitch, false);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_MIC_ON:

                    args.Reset(SDKVoiceEventType.TalkMicSwitch, true);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_SPEAKER_OFF:

                    args.Reset(SDKVoiceEventType.TalkPlayerSwitch, false);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_SPEAKER_ON:

                    args.Reset(SDKVoiceEventType.TalkPlayerSwitch, true);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_SPEAK_SUCCESS:
                    //成功切入到指定语音频道

                    args.Reset(SDKVoiceEventType.TalkSpeakChannelChange, true, channelID);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_SPEAK_FAILED:
                    //切入指定语音频道失败，可能原因是网络或服务器有问题

                    args.Reset(SDKVoiceEventType.TalkSpeakChannelChange, false, channelID);

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_SEND_MESSAGE_RESULT:
                    //sendMessage的结果，param为回传requestID
                case YouMe.YouMeEvent.YOUME_EVENT_MESSAGE_NOTIFY:
                    //频道内其他用户收到消息通知，param为content内容

                    args.Reset(SDKVoiceEventType.TalkChannelBroadcastMsg, channelID, param);
                
                    break;
                default:
                    // "事件类型" + eventType + ",错误码" + errorCode;
                    break;
            }

            SDKVoiceManager.GetInstance().InvokeVoiceEvent(args);
        }

        public void OnRequestRestApi(string strParam)
        {
            Logger.LogProcessFormat("YOUMI VOICE - OnRequestRestApi json is {0}", strParam);
        }

        //获取成员列表及成员变更回调
        //strParam：json串,
        //包含: channelid：字符串，频道ID 
        //      memchange：数字类型，成员列表，或者变更列表。（userid 字符串，用户ID ； isJoin， bool类型，false为离开）
        public void OnMemberChange(string strParam)
        {
            Logger.LogProcessFormat("YOUMI VOICE - OnMemberChange json is {0}", strParam);

            if (string.IsNullOrEmpty(strParam))
            {
                return;
            }
            try
            {
                Hashtable jsonData = MiniJSON.jsonDecode(strParam) as Hashtable; 
                string channelId = "";
                if(jsonData != null && jsonData.ContainsKey("channelid"))
                {
                    channelId = jsonData["channelid"] as string;
                }
                if (jsonData != null && jsonData.ContainsKey("memchange"))
                {
                    var members = jsonData["memchange"] as ArrayList;
                    if (members == null)
                    {
                        return;
                    }
                    var talkChannelMembers = GamePool.ListPool<TalkChannelMemberInfo>.Get();                   
                    for (int i = 0; i < members.Count; i++)
                    {
                        var member = members[i] as Hashtable;
                        if (member == null)
                            continue;
                        TalkChannelMemberInfo memInfo = new TalkChannelMemberInfo();
                        memInfo.talkChannelId = channelId;
                        if (member.ContainsKey("isJoin"))
                        {
                            memInfo.isJoined = (bool)member["isJoin"];
                        }
                        if (member.ContainsKey("userid"))
                        {
                            memInfo.userId = member["userid"] as string;
                        }
                        if (talkChannelMembers != null && !talkChannelMembers.Contains(memInfo))
                        {
                            talkChannelMembers.Add(memInfo);
                        }     
                    }

                    args.Reset(SDKVoiceEventType.TalkChannelMemberChanged, talkChannelMembers);
                    SDKVoiceManager.GetInstance().InvokeVoiceEvent(args);

                    GamePool.ListPool<TalkChannelMemberInfo>.Release(talkChannelMembers);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("[SDKVoiceCallback] - OnMemberChange decode res json failed, json : {0}, error : {1}", strParam, e.ToString());
            }
        }

        #endregion

    }
}
