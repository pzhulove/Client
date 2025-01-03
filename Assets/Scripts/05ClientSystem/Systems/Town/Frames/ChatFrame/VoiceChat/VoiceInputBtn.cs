/*
 * 
 * 语音输入按钮 - 组件
 * 
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VoiceSDK;
using UnityEngine.Events;

namespace GameClient
{
    public class VoiceInputBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler 
    {
        public enum VoiceInputType
        {
            None,
            ChatFrame,
            SingleChannel,
        }

        string DOWN_BTN_RECORD_START = "按 住 说 话";
        string UP_BTN_RECORD_END = "松 开 结 束";
        string DRAG_UP_SEND_CANCEL = "手指移开，取消发送";
        string DRAG_AWAY_SEND_CANCEL = "松开手指，取消发送";
        string VOICE_SEND_READY = "准 备 发 送";
        string VOICE_SEND_CANCEL = "取 消 发 送";
        string VOICE_SENDING = "正 在 发 送";
        string VOICE_SEND_FAILED = "没有听清，请重试";
        string VOICE_RECORD_FAILED = "没有听清，请重试";
        string VOICE_MESSAGE = "[语 音 消 息]";


        [SerializeField]
        private VoiceInputType voiceInputType;

        [HideInInspector]
        [SerializeField]
        private ChatType chatType;


        /*按钮状态*/
        [SerializeField]
        private UIGray currentUIGray;
        [SerializeField]
        private Button thisBtn;
        [SerializeField]
        private Text text_btn;           //录音按钮 状态提示文本 不一定存在

        [SerializeField]
        private ComFunctionInterval funcInterval;

        private bool isInitialized;
        private bool canSend;
        private bool isReadyToSend;
        private bool _isPressed = false;

        private float downBtnStartTime;     //录音有最短时长限制
        private bool bLessRecordVoice;      //在最小录音时间内的录音文件 不发送

        private bool bLargeRecordVoice;
        private bool delayRecordEnabled;

        private static VoiceButtonUsed useBtn = new VoiceButtonUsed();

        public ChatType GetChatType()
        {
            return this.chatType;
        }

        public ComFunctionInterval GetFuncInterval()
        {
            return funcInterval;
        }

        public void InitComponent()
        {
            if (isInitialized)
                return;
            isInitialized = true;//组件初始化完成

            this.gameObject.CustomActive(true);

            ResetUIAtFirst();
			ResetStateFlag ();

            DOWN_BTN_RECORD_START = TR.Value("voice_sdk_down_btn_record_start");
            UP_BTN_RECORD_END = TR.Value("voice_sdk_up_btn_record_end");
            DRAG_UP_SEND_CANCEL = TR.Value("voice_sdk_drag_up_send_cancel");
            DRAG_AWAY_SEND_CANCEL = TR.Value("voice_sdk_drag_down_send_cancel");
            VOICE_SEND_READY = TR.Value("voice_sdk_send_ready");
            VOICE_SEND_CANCEL = TR.Value("voice_sdk_send_cancel");
            VOICE_SENDING = TR.Value("voice_sdk_sending");
            VOICE_SEND_FAILED = TR.Value("voice_sdk_send_failed");
            VOICE_RECORD_FAILED = TR.Value("voice_sdk_record_failed");
            VOICE_MESSAGE = TR.Value("voice_sdk_message");
        }

        private void OnDestroy() 
        {
            UnInitComponent();
        }

        public void UnInitComponent()
        {
            isInitialized = false;

            if (onStartRecord != null)
            {
                onStartRecord.RemoveAllListeners();
            }
            if (onStopRecord != null)
            {
                onStopRecord.RemoveAllListeners();
            }
            if (onCancelRecord != null)
            {
                onCancelRecord.RemoveAllListeners();
            }
            
            chatType = ChatType.CT_ALL;
            ResetStateFlag();

            if(useBtn != null && useBtn.currActiveBtn == this)
            {                
                useBtn.Reset();
                useBtn = null;
            }

            DOWN_BTN_RECORD_START = null;
            UP_BTN_RECORD_END = null;
            DRAG_UP_SEND_CANCEL = null;
            DRAG_AWAY_SEND_CANCEL = null;
            VOICE_SEND_READY = null;
            VOICE_SEND_CANCEL = null;
            VOICE_SENDING = null;
            VOICE_SEND_FAILED = null;
            VOICE_RECORD_FAILED = null;
            VOICE_MESSAGE = null;
        }

        private void ResetStateFlag()
        {
            isReadyToSend = false;          //按下按钮，触发 准备发送语音状态
            canSend = false;                //按下按钮状态下，上划下划手指，触发，录音完成后 是否可发送语音

            bLessRecordVoice = false;
            _isPressed = false;

            bLargeRecordVoice = false;
        }

        void ResetUIAtFirst()
        {
            SetBtnText(DOWN_BTN_RECORD_START);
            downBtnStartTime = -0.1f;
            ResetStateFlag();

            _InvokeVoiceChatReset();
        }

        void SetBtnText(string text)
        {
            if (text_btn)
            {
                text_btn.text = text;
            }
        }

        #region Set Button Display

        #region 外部调用 设置按钮显示状态
        private void _SetSendStart()
        {
            EnabledCurrentBtn(false);
            SetBtnText(VOICE_SENDING);

            _InvokeVoiceChatReset();
        }

        private void _SetSendFailed()
        {
            SystemNotifyManager.SysNotifyTextAnimation(VOICE_SEND_FAILED);
            EnabledCurrentBtn(false);
            _SetSendEnd();
        }

        private void _SetSendCancel()
        {
            SystemNotifyManager.SysNotifyTextAnimation(VOICE_SEND_CANCEL);
            EnabledCurrentBtn(false);
            _SetSendEnd();

            _InvokeVoiceChatReset();
        }

        private void _SetRecordFailed()
        {
            SystemNotifyManager.SysNotifyTextAnimation(VOICE_RECORD_FAILED);
            EnabledCurrentBtn(false);
            _SetSendEnd();

            _InvokeVoiceChatReset();
        }

        private void _SetSendEnd()
        {
            EnabledCurrentBtn(true);
            ResetUIAtFirst();                                   //发送结束，重置按钮状态

            //延迟开始下一次录音
            delayRecordEnabled = true;
            Invoke("ResetDelayToRecord", 1f);
        }

        #endregion

        void ResetDelayToRecord()
        {
            delayRecordEnabled = false;
        }

        void EnabledCurrentBtn(bool bEnabled)
        {
            if (currentUIGray != null)
                currentUIGray.enabled = !bEnabled;
        }

        bool IsCurrentBtnEnable()
        {
            if (currentUIGray != null)
                return !currentUIGray.enabled;
            return true;
        }

        #endregion


        #region Implementation UI Event Interface

        void Update()
        {
            if (_isPressed && bLargeRecordVoice == false)
            {
                float currTime = Time.time;
                if (downBtnStartTime > 0f && (currTime - downBtnStartTime) > 60.0f)
                {
                    bLargeRecordVoice = true;
                    TryStopRecord();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CheckRecordEnabled() == false)
                return;

            if (delayRecordEnabled)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_too_frequent"));
                return;
            }

            if(useBtn == null || useBtn.currActiveBtn != null)
            {
                return;
            }            

            if (_isPressed)
            {
                return;
            }

            //这里才是按下状态
            _isPressed = true;

            isReadyToSend = true;
            bLessRecordVoice = false;
            downBtnStartTime = Time.time;
        
            useBtn.currActiveBtn = this;
            useBtn.pointerFirstId = eventData.pointerId;            

            //语音功能调用需要放在 状态设置 之后
            if (onStartRecord != null)
            {
                onStartRecord.Invoke(voiceInputType, chatType);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (CheckRecordEnabled() == false)
                return;

            if (IsOnlyCurrBtnPressed(eventData) == false)
                return;

            //这里才是抬起状态
            _isPressed = false;

            //异常录音结束
            float upBtnTime = Time.time;
            if (upBtnTime - downBtnStartTime < 1.0f && downBtnStartTime > 0f)//录音时长过短
            {
                _SetRecordFailed();                     //---录音失败， 录制时间过短或者初始化失败   -  6.1
                downBtnStartTime = -0.1f;
                bLessRecordVoice = true;
                canSend = false;

                if(useBtn != null)
                {
                    useBtn.Reset();
                }

                if (onStopRecord != null)
                {
                    onStopRecord.Invoke();
                }

                return;
            }

            //正常录音结束
            if (bLargeRecordVoice)
                return;

            TryStopRecord();
        }

        void TryStopRecord()
        {
            if (isReadyToSend)
            {
                //Logger.LogError ("松开手指，可以发送");
                canSend = true;
                _SetSendStart();

            }
            else
            {
                //Logger.LogError("松开手指，发送取消");
                canSend = false;
                _SetSendCancel();
            }

            //语音功能调用需要放在 状态设置 之后
            //StopRecord();
            if (onStopRecord != null)
            {
                onStopRecord.Invoke();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsOnlyCurrBtnPressed(eventData) == false)
                return;
            SetBtnText(DRAG_AWAY_SEND_CANCEL);        
            _InvokeVoiceChatReadySendCancel();
            isReadyToSend = false;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsOnlyCurrBtnPressed(eventData) == false)
                return;

            SetBtnText(UP_BTN_RECORD_END);
            _InvokeVoiceChatShowSendCancel();
            isReadyToSend = true;
        }

        bool IsOnlyCurrBtnPressed(PointerEventData eventData)
        {
            if (_isPressed == false)
            {
                //Logger.LogError("[当前语音输入按钮 - _isPressed = ]" + _isPressed);
                return false;
            }

            if (eventData == null)
                return false;
            int pointerId = 10000;
            if(useBtn == null)
                return false;
            pointerId = useBtn.pointerFirstId;
            
            if (eventData.pointerId != pointerId)
                return false;
            
            if(useBtn.currActiveBtn != this)
                return false;

            return true;
        }

        bool CheckRecordEnabled()
        {
            if(checkEnableRecord != null)
            {
                if(checkEnableRecord() == false)
                {
                    return false;
                }
            }
            if (isInitialized == false)
            {
                Logger.LogProcessFormat("[语音输入按钮初始化未完成]");
                return false;
            }
            if (IsCurrentBtnEnable() == false)
            {
                Logger.LogProcessFormat("[当前语音输入按钮不可用]");
                return false;
            }
            return true;
        }


        void _InvokeVoiceChatReset()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatReset, DRAG_UP_SEND_CANCEL);
        }

        void _InvokeVoiceChatReadySendCancel()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatReadySendCancel, DRAG_AWAY_SEND_CANCEL);
        }

        void _InvokeVoiceChatShowSendCancel()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatShowSendCancel, DRAG_UP_SEND_CANCEL);
        }

        #endregion


        #region For Audio Voice Methods

        public delegate bool CheckVButtonEnableRecord();
        public CheckVButtonEnableRecord checkEnableRecord;

        //如果在Inspector面板不可见，则需要new
        [System.Serializable]
        public class OnVButtonStartRecord : UnityEvent<VoiceInputType, ChatType> { };
        [System.Serializable]
        public class OnVButtonStopRecord : UnityEvent { };
        [System.Serializable]
        public class OnVButtonCancelRecord : UnityEvent { };

        public OnVButtonStartRecord onStartRecord = new OnVButtonStartRecord();
        public OnVButtonStopRecord onStopRecord = new OnVButtonStopRecord();
        public OnVButtonCancelRecord onCancelRecord = new OnVButtonCancelRecord();
        #endregion
    }
}