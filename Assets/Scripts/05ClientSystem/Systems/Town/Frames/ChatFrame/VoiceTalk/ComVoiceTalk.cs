using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 实时语音 View
    /// </summary>
    public class ComVoiceTalk : MonoBehaviour
    {
        public static ComVoiceTalk msComVoiceTalk = null;
        public const string RES_PATH = "UIFlatten/Prefabs/Common/ComVoice/ComVoiceTalk";

        public enum ComVoiceTalkType
        {
            None,
            TeamDuplicationMainBuild,               //团本
            TeamDungeon,                            //组队地下城
            Pk3v3Battle,                            //pk 3v3 
            Pk3v3Room,                  
        }

        public struct LocalCacheData
        {
            public bool hasSetMicStatus;
            public bool hasSetPlayerStatus;
            public bool isMicOn;
            public bool isPlayerOn;

            public void Clear()
            {
                hasSetMicStatus = hasSetPlayerStatus = false;
                isMicOn = isPlayerOn = false;
            }
        }

        private static Dictionary<ComVoiceTalkType, VoiceTalkConfig> talkTypeWithConfig = new Dictionary<ComVoiceTalkType, VoiceTalkConfig>()
        {
            {
                ComVoiceTalkType.TeamDuplicationMainBuild,
                new VoiceTalkConfig{ resPath =  "UIFlatten/Prefabs/Common/ComVoice/TeamDuplicationComVoiceTalk",
                                     switchType = VoiceSDK.SDKVoiceManager.VoiceSDKSwitch.TalkVoiceInTeamDuplication,
                                     isMicOnAtFirst = false, isPlayerOnAtFirst = true}
            },
            {
                ComVoiceTalkType.TeamDungeon,
                new VoiceTalkConfig{ resPath = "UIFlatten/Prefabs/Common/ComVoice/TeamComVoiceTalk",
                                     switchType = VoiceSDK.SDKVoiceManager.VoiceSDKSwitch.TalkVoiceInTeam,
                                     isMicOnAtFirst = false, isPlayerOnAtFirst = false}
            },
            {
                ComVoiceTalkType.Pk3v3Battle,
                new VoiceTalkConfig{ resPath = "UIFlatten/Prefabs/Common/ComVoice/TeamComVoiceTalk",
                                     switchType = VoiceSDK.SDKVoiceManager.VoiceSDKSwitch.TalkVoiceIn3v3Pvp,
                                     isMicOnAtFirst = false, isPlayerOnAtFirst = true}
            },
            {
                ComVoiceTalkType.Pk3v3Room,
                new VoiceTalkConfig{ resPath = "UIFlatten/Prefabs/Common/ComVoice/Pk3v3ComVoiceTalk",
                                     switchType = VoiceSDK.SDKVoiceManager.VoiceSDKSwitch.TalkVoiceIn3v3Room,
                                     isMicOnAtFirst = false, isPlayerOnAtFirst = true}
            }
        };

        //新增的需要加入到tempTalkBtnList
        [SerializeField]
        private ComVoiceTalkButton micBtn;
        [SerializeField]
        private ComVoiceTalkButton playerBtn;
        [SerializeField]
        private ComVoiceTalkButton limitMicBtn;
        [SerializeField]
        private ComVoiceTalkButtonGroup micBtnGroup;
        //新增的需要加入到tempTalkBtnList
        private List<ComVoiceTalkButton> tempTalkBtnList = new List<ComVoiceTalkButton>();

        private VoiceTalkModule mVoiceTalkModule;
        private bool bMicBtnGroupShow;
        public bool HasHide {get; private set;}

        public GameObject GoParent{
            get{
                if(this.gameObject == null || this.gameObject.transform == null)
                {
                    return null;
                }
                var trans = this.gameObject.transform.parent;
                if(trans == null)
                {
                    return null;
                }
                return trans.gameObject;
            }   
        }
        private GameObject lastGoParent;

        private void Awake()
        {           
            GameObject.DontDestroyOnLoad(this);
            _InitView();                    
        }

        private void OnDestroy()
        {
            _ClearView();
        }

        private void _InitView()
        {
            _BindUIEvent();
            _InitAllTalkBtns();
            _AddTalkBtnsEvent();
        }

        void _InitAllTalkBtns()
        {
            if (tempTalkBtnList != null)
            {
                tempTalkBtnList.Add(micBtn);
                tempTalkBtnList.Add(playerBtn);
                tempTalkBtnList.Add(limitMicBtn);
                if (micBtnGroup != null)
                {
                    tempTalkBtnList.AddRange(micBtnGroup.GetAllTalkBtns());
                }
                tempTalkBtnList.ForEach(_SetAllTalkBtnOff);
            }

            _ShowMicBtnGroup(false);
            ShowLimitAllNotSpeakBtn(false);
        }

        public void AddTempTalkBtnList(List<ComVoiceTalkButton> btns)
        {
            if (tempTalkBtnList != null && btns != null)
            {
                tempTalkBtnList.AddRange(btns);
            }
        }

        private void _ClearView()
        {
            _UnBindUIEvent();
            _RemoveTalkBtnsEvent();

            if (mVoiceTalkModule != null)
            {
                mVoiceTalkModule.UnInit();
                mVoiceTalkModule = null;
            }

            if (msComVoiceTalk == this)
            {
                msComVoiceTalk = null;
            }

            if (tempTalkBtnList != null)
            {
                tempTalkBtnList.Clear();
                tempTalkBtnList = null;
            }

            bMicBtnGroupShow = false;
            lastGoParent = null;      
            HasHide = false;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceTalkMicSwitch, _OnVoiceTalkMicSwitch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceTalkPlayerSwitch, _OnVoiceTalkPlayerSwitch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceTalkLimitAllNotSpeak, _VoiceTalkLimitAllNotSpeak);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceTalkMicClosedByOther, _VoiceTalkMicClosedByOther);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceTalkChannelChanged, _VoiceTalkChannelChanged);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceTalkMicSwitch, _OnVoiceTalkMicSwitch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceTalkPlayerSwitch, _OnVoiceTalkPlayerSwitch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceTalkLimitAllNotSpeak, _VoiceTalkLimitAllNotSpeak);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceTalkMicClosedByOther, _VoiceTalkMicClosedByOther);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceTalkChannelChanged, _VoiceTalkChannelChanged);
        }

        private void _OnVoiceTalkMicSwitch(UIEvent uiEvent)
        {
            bool status = _TryGetUIEventParam1(uiEvent);
            if (tempTalkBtnList != null)
            {
                if (status)
                {
                    tempTalkBtnList.ForEach(_SetTalkMicBtnOn);
                }
                else
                {
                    tempTalkBtnList.ForEach(_SetTalkMicBtnOff);
                }
            }
            if(bMicBtnGroupShow && micBtnGroup)
            {
                //如果mic被他人禁言
                if(mVoiceTalkModule != null && !mVoiceTalkModule.IsSelfMicEnable())
                {
                    _UpdateAllMicEnable(false);
                    return;
                }
                if(!status)
                {
                    micBtnGroup.SetMicOffTalkBtnSelected(true);
                }
            }
        }

        private void _OnVoiceTalkPlayerSwitch(UIEvent uiEvent)
        {
            bool status = _TryGetUIEventParam1(uiEvent);
            if (tempTalkBtnList != null)
            {
                if (status)
                {
                    tempTalkBtnList.ForEach(_SetTalkPlayerBtnOn);
                }
                else
                {
                    tempTalkBtnList.ForEach(_SetTalkPlayerBtnOff);
                }
            }
        }

        private void _VoiceTalkLimitAllNotSpeak(UIEvent uiEvent)
        {
            bool status = _TryGetUIEventParam1(uiEvent);
            if (tempTalkBtnList != null)
            {
                if (status)
                {
                    tempTalkBtnList.ForEach(_SetTalkLimitSpeakBtnOn);
                }
                else
                {
                    tempTalkBtnList.ForEach(_SetTalkLimitSpeakBtnOff);
                }
            }
        }

        private void _VoiceTalkMicClosedByOther(UIEvent uiEvent)
        {
            bool status = _TryGetUIEventParam1(uiEvent);
            _UpdateAllMicEnable(status);
        }

        private bool _TryGetUIEventParam1(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
            {
                return false;
            }
            bool[] status = uiEvent.Param1 as bool[];
            if (status == null || status.Length <= 0)
            {
                return false;
            }
            return status[0];
        }

        private void _VoiceTalkChannelChanged(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
            {
                return;
            }
            string channelId = uiEvent.Param1 as string;
            if(string.IsNullOrEmpty(channelId))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_talk_set_channel_failed"));
                return;
            }
            //TODO
            if(bMicBtnGroupShow && micBtnGroup)
            {
                micBtnGroup.SetMicChannelOnTalkBtnSelected(channelId, true);
            }
        }

        private void _SetAllTalkBtnOn(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null)
            {
                talkBtn.SetBtnStatus(true);
            }
        }

        private void _SetAllTalkBtnOff(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null)
            {
                talkBtn.SetBtnStatus(false);
            }
        }

        private void _SetTalkMicBtnOn(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isMicType)
            {
                talkBtn.SetBtnStatus(true);
            }
        }

        private void _SetTalkMicBtnOff(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isMicType)
            {
                talkBtn.SetBtnStatus(false);
            }
        }

        private void _SetTalkPlayerBtnOn(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isPlayerType)
            {
                talkBtn.SetBtnStatus(true);
            }
        }

        private void _SetTalkPlayerBtnOff(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isPlayerType)
            {
                talkBtn.SetBtnStatus(false);
            }
        }

        private void _SetTalkLimitSpeakBtnOn(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isLimitSpeakType)
            {
                talkBtn.SetBtnStatus(false);
            }
        }

        private void _SetTalkLimitSpeakBtnOff(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isLimitSpeakType)
            {
                talkBtn.SetBtnStatus(true);
            }
        }

        private void _SetTalkMicOnByOther(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isMicType)
            {
                talkBtn.SetMarkIconShow(false);
                talkBtn.SetBtnEnable(true);
            }
        }

        private void _SetTalkMicOffByOther(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null && talkBtn.isMicType)
            {
                talkBtn.SetMarkIconShow(true);
                talkBtn.SetBtnEnable(false);
            }
        }

        private void _AddTalkBtnsEvent()
        {
            if (tempTalkBtnList != null)
            {
                tempTalkBtnList.ForEach(_AddTalkBtnEvent);
            }
        }

        private void _RemoveTalkBtnsEvent()
        {
            if (tempTalkBtnList != null)
            {
                tempTalkBtnList.ForEach(_RemoveTalkBtnEvent);
            }
        }

        private void _AddTalkBtnEvent(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null)
            {
                talkBtn.onClick.AddListener(_OnTalkBtnClick);
            }
        }

        private void _RemoveTalkBtnEvent(ComVoiceTalkButton talkBtn)
        {
            if (talkBtn != null)
            {
                talkBtn.onClick.RemoveAllListeners();
            }
        }

        private void _OnTalkBtnClick(ComVoiceTalkButton.TalkBtnType btnType, ComVoiceTalkButton comBtn, object param1)
        {
            if (mVoiceTalkModule == null)
            {
                return;
            }
            bool btnCanOperate = true;
            switch (btnType)
            {
                case ComVoiceTalkButton.TalkBtnType.PlayerSwitch:
                    mVoiceTalkModule.ControlPlayer();
                    break;
                case ComVoiceTalkButton.TalkBtnType.MicSwitch:
                    mVoiceTalkModule.ControlMic();
                    break;
                case ComVoiceTalkButton.TalkBtnType.MicChannelOn:
                    string param = param1 as string;
                    //需要主动开启mic
                    btnCanOperate = mVoiceTalkModule.SwitchSpeakChannel(param);
                    break;
                case ComVoiceTalkButton.TalkBtnType.MicAllOff:
                    btnCanOperate = mVoiceTalkModule.CloseMic();
                    break;
                case ComVoiceTalkButton.TalkBtnType.MicAllNotSpeak:
                    mVoiceTalkModule.ControlGlobalSilence();
                    break;
            }
            if(!btnCanOperate)
            {
                _SetComVoiceBtnGroupSelectBtn(comBtn, false);
            }
        }

        private void _SetComVoiceBtnGroupSelectBtn(ComVoiceTalkButton comBtn, bool bSelected)
        {
            if(null == comBtn || null == comBtn.group)
            {
                return;
            }
            comBtn.group.SetComVoiceTalkBtnSelected(comBtn, bSelected);
        }

        public void UpdateMicBtnGroup(bool showGroup)
        {
            if (mVoiceTalkModule == null)
            {
                return;
            }
            if (micBtnGroup != null)
            {
                micBtnGroup.UpdateAllTalkBtns(mVoiceTalkModule.GetMultipleTalkChannels());
            }
            _ShowMicBtnGroup(showGroup);

            if(showGroup && micBtnGroup)
            {
                micBtnGroup.SetMicChannelOnTalkBtnSelected(mVoiceTalkModule.GetCurrentTalkChanneld(), true);
            }
        }

        public void UpdateAllMicStatus()
        {
            if (mVoiceTalkModule == null)
            {
                return;
            }
            bool isEnable = mVoiceTalkModule.IsSelfMicEnable();
            _UpdateAllMicEnable(isEnable);
        }

        private void _UpdateAllMicEnable(bool bEnable)
        {
            if (tempTalkBtnList != null)
            {
                if (bEnable)
                {
                    tempTalkBtnList.ForEach(_SetTalkMicOnByOther);
                }
                else
                {
                    tempTalkBtnList.ForEach(_SetTalkMicOffByOther);
                }
            }
        }

        //自动切换
        //当mic开启时 才能切换说话频道
        //当mic关闭时 刷新按钮表现 为关闭状态
        //不同于手动切换
        //手动切换说话频道，需要把麦开启  
        public void TrySwitchTalkChannelIdByCond(string channelId, bool canSwitch = false)
        {
            if(canSwitch)
            {
                if (mVoiceTalkModule != null && !string.IsNullOrEmpty(channelId))
                {
                    //不主动开启mic
                    mVoiceTalkModule.SwitchSpeakChannel(channelId, false);
                }
            }
            else
            {
                if (bMicBtnGroupShow && micBtnGroup)
                {
                    micBtnGroup.SetMicOffTalkBtnSelected(true);
                }
            }
        }

        private void _ShowMicBtnGroup(bool isShow)
        {
            micBtnGroup.CustomActive(isShow);
            micBtn.CustomActive(!isShow);
            bMicBtnGroupShow = isShow;
        }

        public void ShowLimitAllNotSpeakBtn(bool isShow)
        {
            limitMicBtn.CustomActive(isShow);
        }

        public void SetPlayerMicEnable(string accId, bool bEnable)
        {
            if(mVoiceTalkModule != null)
            {
                mVoiceTalkModule.SetMicEnable(accId, bEnable);
            }
        }

        public void ResetGlobalSilence()
        {
            if(mVoiceTalkModule != null)
            {
                mVoiceTalkModule.ResetGlobalSilence();
            }
        }

        public void Enable(VoiceTalkConfig vtConfig)
        {        
            if (null == mVoiceTalkModule)
            {
                mVoiceTalkModule = new VoiceTalkModule();
            }

            if (mVoiceTalkModule != null)
            {                            
                mVoiceTalkModule.Reset(vtConfig);
            }
        }

        // public void Disable()
        // {
        //     if (mVoiceTalkModule != null)
        //     {
        //         mVoiceTalkModule.UnInit();
        //     }

        //     Hide();
        // }

        public void Hide()
        {
            AttachToParent(null);        
        }

        public void AttachToParent(GameObject goParent)
        {
            HasHide = goParent == null ? true : false;
            if(goParent == null)
            {
                this.gameObject.transform.SetParent(null);    
            }
            else
            {
                Utility.AttachTo(gameObject, goParent);
            }
        }

        public void UpdateTalkChannelId(List<string> channelIds)
        {
            if (mVoiceTalkModule != null)
            {
                mVoiceTalkModule.UpdateMultipleTalkChannel(channelIds);
            }
        }

        public void AddGameSceneId(string sId)
        {
            if (mVoiceTalkModule != null)
            {
                mVoiceTalkModule.AddMultipleTalkChannel(sId);
            }
        }

        public void RemoveGameSceneId(string sId)
        {
            if (mVoiceTalkModule != null)
            {
                mVoiceTalkModule.RemoveMultipleTalkChannel(sId);
            }
        }

        public void CtrlMic()
        {
            if(mVoiceTalkModule != null)
            {
                mVoiceTalkModule.ControlMic();
            }
        }

        public void CtrlPlayer()
        {
            if(mVoiceTalkModule != null)
            {
                mVoiceTalkModule.ControlPlayer();
            }
        }

        public bool IsMicOn()
        {
            if(mVoiceTalkModule != null)
            {
                return mVoiceTalkModule.IsMicOn();
            }
            return false;
        }

        public bool IsPlayerOn()
        {
            if(mVoiceTalkModule != null)
            {
                return mVoiceTalkModule.IsPlayerOn();
            }
            return false;
        }

        public bool IsJoinedTalkChannel(string talkChannelId)
        {
            if(mVoiceTalkModule != null)
            {
                return mVoiceTalkModule.IsJoinedTalkChannel(talkChannelId);
            }
            return false;
        }

        #region Static Method
        public static bool CheckJoinedTalkChannel(string talkChannelId)
        {
            if(msComVoiceTalk != null)
            {
                return msComVoiceTalk.IsJoinedTalkChannel(talkChannelId);
            }
            return false;
        }

        public static void SetSelctPlayerMicEnable(string accId, bool bEnable)
        {
            if(msComVoiceTalk != null)
            {
                msComVoiceTalk.SetPlayerMicEnable(accId, bEnable);
            }
        }

        public static void ResetCurrentGlobalSilence()
        {
            if(msComVoiceTalk != null)
            {
                msComVoiceTalk.ResetGlobalSilence();
            }
        }

        public static void AddChannelID(string cId)
        {
            if (msComVoiceTalk != null)
            {
                msComVoiceTalk.AddGameSceneId(cId);
            }
        }

        public static void RemoveChannelID(string cId)
        {
            if (msComVoiceTalk != null)
            {
                msComVoiceTalk.RemoveGameSceneId(cId);
            }
        }

        public static void UpdateChannelID(List<string> cIds)
        {
            if (msComVoiceTalk != null)
            {
                msComVoiceTalk.UpdateTalkChannelId(cIds);
            }
        }

        public static void Hidden()
        {
            if (msComVoiceTalk != null)
            {
                msComVoiceTalk.Hide();
            }
        }
        
        //彻底回收
        public static void ForceDestroy()
        {
            if (msComVoiceTalk != null)
            {
                //msComVoiceTalk.Disable();
                UnityEngine.Object.Destroy(msComVoiceTalk.gameObject);
                msComVoiceTalk = null;
            }
        }
        public static void ShowLimitSpeakBtn(bool isShow)
        {
            if (msComVoiceTalk != null)
            {
                msComVoiceTalk.ShowLimitAllNotSpeakBtn(isShow);
            }
        }

        public static void UpdateMicBtnGroupStatus(bool showGroup)
        {
            if (msComVoiceTalk != null)
            {
                msComVoiceTalk.UpdateMicBtnGroup(showGroup);
            }
        }

        public static void UpdateAllMicShowStatus()
        {
            if(msComVoiceTalk != null)
            {
                msComVoiceTalk.UpdateAllMicStatus();
            }
        }

        public static void TrySwitchTalkChannelId(string channelId, bool canSwitch = false)
        {
            if (msComVoiceTalk != null)
            {
                 msComVoiceTalk.TrySwitchTalkChannelIdByCond(channelId, canSwitch);
            }
        }

        public static void ControlMic()
        {
            if(msComVoiceTalk != null)
            {
                msComVoiceTalk.CtrlMic();
            }
        }

        public static void ControlPlayer()
        {
            if(msComVoiceTalk != null)
            {
                msComVoiceTalk.CtrlPlayer();
            }
        }

        public static bool IsVoiceTalkMicOn()
        {
            if(msComVoiceTalk != null)
            {
                return msComVoiceTalk.IsMicOn();
            }
            return false;
        }

        public static bool IsVoiceTalkPlayerOn()
        {
            if(msComVoiceTalk != null)
            {
                return msComVoiceTalk.IsPlayerOn();
            }
            return false;
        }

        public static ComVoiceTalk Bind(GameObject goParent)
        {
            if(msComVoiceTalk == null)
            {
                return null;
            }
            msComVoiceTalk.lastGoParent = msComVoiceTalk.GoParent;            
            msComVoiceTalk.AttachToParent(goParent);
            return msComVoiceTalk;
        }

        public static void UnBind()
        {
            if(msComVoiceTalk == null)
            {
                return;
            }
            if(msComVoiceTalk.HasHide)
            {
                return;
            }
            msComVoiceTalk.AttachToParent(msComVoiceTalk.lastGoParent);
        }

        public static ComVoiceTalk Create(GameObject goParent, LocalCacheData localData, ComVoiceTalkType cvTalkType= ComVoiceTalkType.None, bool otherSwitch = true, bool openGlobalSilence = false)
        {
            if (cvTalkType == ComVoiceTalkType.None)
            {
                return null;
            }
            VoiceTalkConfig config = config = talkTypeWithConfig.SafeGetValue(cvTalkType);

            //语音功能不开启时 不创建UI
            if (!VoiceSDK.SDKVoiceManager.GetInstance().GetVoiceSDKSwitch(config.switchType) || !otherSwitch)
            {
                return null;
            }

            if (msComVoiceTalk != null)
            {                
                msComVoiceTalk.AttachToParent(goParent);
            }
            else
            {
                GameObject voiceTalkGo = null;
                if (!string.IsNullOrEmpty(config.resPath))
                {
                    voiceTalkGo = AssetLoader.instance.LoadResAsGameObject(config.resPath);
                }
                else
                {
                    voiceTalkGo = AssetLoader.instance.LoadResAsGameObject(RES_PATH);
                }
                if (voiceTalkGo)
                {
                    Utility.AttachTo(voiceTalkGo, goParent);
                    msComVoiceTalk = voiceTalkGo.SafeAddComponent<ComVoiceTalk>(false);
                }

                //第一次 创建时 
                //如果本地存在配置 则使用配置 否则 使用默认配置
                if (localData.hasSetMicStatus)
                {
                    config.isMicOnAtFirst = localData.isMicOn;
                }
                if (localData.hasSetPlayerStatus)
                {
                    config.isPlayerOnAtFirst = localData.isPlayerOn;
                }
                config.isGlobalSilenceAtFirst = openGlobalSilence;
            }            

            if (msComVoiceTalk != null)
            {
                msComVoiceTalk.gameObject.CustomActive(true);
                msComVoiceTalk.gameObject.transform.localScale = Vector3.one;
                (msComVoiceTalk.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                //TODO 调整按钮相对位置等表现

                msComVoiceTalk.Enable(config);
            }

            return msComVoiceTalk;
        }

        #endregion
    }
}
