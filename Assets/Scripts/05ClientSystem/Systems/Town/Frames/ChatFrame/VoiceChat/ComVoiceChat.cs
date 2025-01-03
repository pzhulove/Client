using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class VoiceButtonUsed
    {
        public VoiceInputBtn currActiveBtn;
        public int pointerFirstId;                   //手指第一次触发UI时的Id

        public VoiceButtonUsed()
        {
            Reset();
        }

        public void Reset()
        {
            currActiveBtn = null;
            pointerFirstId = 10000;
        }
    }


    /// <summary>
    /// 即时语音（语音聊天） View
    /// </summary>
    public class ComVoiceChat : MonoBehaviour
    {
        public enum ComVoiceChatType
        {
            None,
            Frame = 1,
            Global = 2,
        }

        [SerializeField]
        private List<VoiceInputBtn> voiceBtns;

        private VoiceStatusPopup voiceStatusPopup;

        private ComVoiceChatType comVoiceChatType = ComVoiceChatType.None;
        private ChatFrameData chatFrameData = null;
        private VoiceChatModule voiceChatModule = null;
        static VoiceButtonUsed currentUsedButton = null;                        //当前使用中的按钮

        private void Awake() 
        {
            _AddVoiceInputBtnEvents();
        }

        private void OnDestroy() 
        {
            _RemoveVoiceInputBtnEvents();

            UnInit();
            _DestroyPopup();
        }


        private void _InitVoiceInputBtns()
        {
            if(voiceBtns != null && voiceBtns.Count > 0)
            {
                for (int i = 0; i < voiceBtns.Count; i++)
                {
                    var vBtn = voiceBtns[i];
                    if(vBtn == null)
                        continue;
                    vBtn.InitComponent();
                }
            }
        }

        private void _AddVoiceInputBtnEvents()
        {
            if(null == voiceBtns || voiceBtns.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < voiceBtns.Count; i++)
            {
                var vBtn = voiceBtns[i];
                if(vBtn == null)
                    continue;
                vBtn.onStartRecord.AddListener(_OnVoiceBtnStartRecord);
                vBtn.onStopRecord.AddListener(_OnVoiceBtnStopRecord);
                vBtn.onCancelRecord.AddListener(_OnVoiceBtnCancelRecord);

                //按钮间隔时间注册
                ComIntervalGroup.GetInstance().Register(this, (int)vBtn.GetChatType(), vBtn.GetFuncInterval());
            }
        }

        private void _RemoveVoiceInputBtnEvents()
        {
            if(null == voiceBtns || voiceBtns.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < voiceBtns.Count; i++)
            {
                var vBtn = voiceBtns[i];
                if(vBtn == null)
                    continue;
                vBtn.onStartRecord.RemoveListener(_OnVoiceBtnStartRecord);
                vBtn.onStopRecord.RemoveListener(_OnVoiceBtnStopRecord);
                vBtn.onCancelRecord.RemoveListener(_OnVoiceBtnCancelRecord);
            }

            ComIntervalGroup.GetInstance().UnRegister(this);
        }

        private void _OnVoiceBtnStartRecord(VoiceInputBtn.VoiceInputType voiceInputType, ChatType chatType)
        {
            ChatType _chatType = chatType;
            ulong _targetUserId = 0;
            if(voiceInputType == VoiceInputBtn.VoiceInputType.ChatFrame)
            {
                if(chatFrameData != null)
                {
                    if(chatFrameData.curPrivate != null)
                    {
                        _targetUserId = chatFrameData.curPrivate.uid;
                    }
                    _chatType = chatFrameData.eChatType;
                }
            }
            if(voiceChatModule != null)
            {
                voiceChatModule.StartRecordVoice(_chatType, _targetUserId);
            }
            _CreatePopup(this.gameObject);
        }

        private void _OnVoiceBtnStopRecord()
        {
            if(voiceChatModule != null)
            {
                voiceChatModule.StopRecordVoice();
            }
        }

        private void _OnVoiceBtnCancelRecord()
        {
            if(voiceChatModule != null)
            {
                voiceChatModule.CancelRecordVoice();
            }            
        }

        private void _CreatePopup(GameObject goParent)
        {
            if(voiceStatusPopup == null)
            {
                GameObject go = AssetLoader.instance.LoadResAsGameObject(VoiceStatusPopup.RES_PATH);
                if(go != null)
                {
                    voiceStatusPopup = go.SafeAddComponent<VoiceStatusPopup>();
                }
                if(voiceStatusPopup != null)
                {
                    Utility.AttachTo(voiceStatusPopup.gameObject, goParent);
                }
            }
        }

        private void _DestroyPopup()
        {
            if (voiceStatusPopup != null)
            {
                UnityEngine.Object.Destroy(voiceStatusPopup);
                voiceStatusPopup = null;
            }
        }

        public void Init(ComVoiceChatType voiceChatType, bool otherSwitch = true, 
                VoiceChatModule.IsSatisfiedWithChatCondition isSatisfiedWithChat = null,
                VoiceChatModule.IsRecordLimitCondition isRecordLimitCondition= null)
        {
            this.comVoiceChatType = voiceChatType;
            VoiceChatConfig voiceChatConfig = new VoiceChatConfig();
            if(isSatisfiedWithChat != null)
            {
                voiceChatConfig.isSatisfiedWithChat = isSatisfiedWithChat;
                voiceChatConfig.isRecordLimitCondition = isRecordLimitCondition;
                voiceChatConfig.voiceSDKSwitchType = (VoiceSDK.SDKVoiceManager.VoiceSDKSwitch)voiceChatType;
            }
            voiceChatModule = VoiceSDK.SDKVoiceManager.GetInstance().VoiceChatModule;
            if(voiceChatModule != null)
            {
                voiceChatModule.Reset(voiceChatConfig, otherSwitch);
            }
            currentUsedButton = new VoiceButtonUsed();
            _InitVoiceInputBtns();
        }

        public void UnInit()
        {
            chatFrameData = null;

            if (voiceChatModule != null)
            {
                voiceChatModule.UnInit();
                voiceChatModule = null;
            }

            if(voiceBtns != null)
            {
                for (int i = 0; i < voiceBtns.Count; i++)
                {
                    var vBtn = voiceBtns[i];
                    if(vBtn == null)
                        continue;
                    if(currentUsedButton != null && vBtn == currentUsedButton.currActiveBtn)
                    {
                        currentUsedButton.Reset();
                        currentUsedButton = null;
                        break;
                    }            
                    vBtn.UnInitComponent();        
                }
            }
        }


        public void PlayVoice(string voiceKey)
        {
            if(string.IsNullOrEmpty(voiceKey))
            {
                return;
            }
            if(voiceChatModule != null)
            {
                voiceChatModule.PlayVoice(voiceKey);
            }
        }
    }
}
