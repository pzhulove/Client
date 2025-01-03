using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VoiceSDK;

namespace GameClient
{
    public class ComVoiceTalkButton : MonoBehaviour
    {
        #region MODEL PARAMS

        public enum TalkBtnType
        {
            None,
            //麦
            MicChannelOn,                //麦克风指定频道打开  （只有开启状态）
            MicAllOff,                   //麦克风关闭 （只有关闭状态）  
            MicSwitch,                   //开关麦克风
            //其他
            MicAllNotSpeak,              //全员禁言
            //扬声器
            PlayerSwitch,                //开关扬声器 
        }

        [System.Serializable]
        public class OnTalkButtonClick : UnityEvent<TalkBtnType, ComVoiceTalkButton, object> { };

        private OnTalkButtonClick _onClick;
        public OnTalkButtonClick onClick
        {
            get
            {
                if (_onClick == null)
                {
                    _onClick = new OnTalkButtonClick();
                }
                return _onClick;
            }
        }

        public TalkBtnType bType
        {
            get
            {
                if (talkBtnUnit != null)
                {
                    return talkBtnUnit.talkBtnType;
                }
                return TalkBtnType.None;
            }
        }
       
        public bool isMicType
        {
            get {
                if (bType != TalkBtnType.None && (int)bType <= (int)TalkBtnType.MicSwitch)
                {
                    return true;
                }
                return false;
            }
        }

        public bool isPlayerType
        {
            get
            {
                if (bType != TalkBtnType.None && (int)bType >= (int)TalkBtnType.PlayerSwitch)
                {
                    return true;
                }
                return false;
            }
        }

        public bool isLimitSpeakType
        {
            get
            {
                if (bType == TalkBtnType.MicAllNotSpeak)
                {
                    return true;
                }
                return false;
            }
        }

        public object param1 { get; set; }
        public ComVoiceTalkButtonGroup group { get; set; }
        public int sortIndex{ get; set;}

        #endregion

        #region VIEW PARAMS

        [System.Serializable]
        public class ComVoiceTalkButtonUnit
        {
            public Button talkBtn;            
            public Image talkOnImg;
            public Image talkOffImg;
            public Text talkMark;           //角标名称
            public Image talkMarkImg;       //角标图片
            public TalkBtnType talkBtnType = TalkBtnType.None;

            public ComVoiceTalkButtonUnit(Button btn, Image onImg, Image offImg, Text mark, Image markImg, TalkBtnType btnType)
            {
                talkBtn = btn;
                talkOnImg = onImg;
                talkOffImg = offImg;
                talkMark = mark;
                talkMarkImg = markImg;
                talkBtnType = btnType;
            }

            public bool IsNull()
            {
                if (null == talkBtn || null == talkOnImg || null == talkOffImg || talkBtnType == TalkBtnType.None)
                {
                    return true;
                }
                return false;
            }
        }

        [SerializeField] private ComVoiceTalkButtonUnit talkBtnUnit;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake()
        {
            if (talkBtnUnit != null && talkBtnUnit.talkBtn != null)
            {
                talkBtnUnit.talkBtn.onClick.AddListener(_OnTalkBtnClick);
            }
            _InitView();
        }
        
        //Unity life cycle
        void OnDestroy ()
        {
            if (talkBtnUnit != null && talkBtnUnit.talkBtn != null)
            {
                talkBtnUnit.talkBtn.onClick.RemoveListener(_OnTalkBtnClick);
            }
            talkBtnUnit = null;
            _onClick = null;
            param1 = null;
            group = null;
            sortIndex = -1;
        }

        private void _InitView()
        {
            if (isLimitSpeakType)
            {
                SetBtnStatus(true);
            }
            else
            {
                SetBtnStatus(false);
            }
        }

        #region EVENT

        private void _OnTalkBtnClick()
        {
            if (talkBtnUnit == null || talkBtnUnit.talkBtnType == TalkBtnType.None)
            {
                return;
            }
            if (onClick != null)
            {
                onClick.Invoke(talkBtnUnit.talkBtnType, this, param1);
            }
        }

        #endregion

        #endregion

        #region  PUBLIC METHODS

        public void SetMarkIconShow(bool isShow)
        {
            if (null == talkBtnUnit)
            {
                return;
            }
            if (talkBtnUnit.talkBtnType == TalkBtnType.MicChannelOn ||
                talkBtnUnit.talkBtnType == TalkBtnType.MicSwitch ||
                talkBtnUnit.talkBtnType == TalkBtnType.PlayerSwitch ||
                talkBtnUnit.talkBtnType == TalkBtnType.MicAllOff)
            {
                if(talkBtnUnit.talkMark)
                {
                    talkBtnUnit.talkMark.enabled = !isShow;
                }
                if(talkBtnUnit.talkMarkImg)
                {
                    talkBtnUnit.talkMarkImg.enabled = isShow;
                }
            }
        }

        public void SetBtnEnable(bool isEnable)
        {
            if (null == talkBtnUnit || null == talkBtnUnit.talkBtn)
            {
                return;
            }
            talkBtnUnit.talkBtn.interactable = isEnable;
        }

        public void SetBtnStatus(bool isOn)
        {
            if (null == talkBtnUnit)
            {
                return;
            }

            //如果显示 禁用按钮标志 则不进行按钮的开关状态切换 
            if (talkBtnUnit.talkMarkImg && talkBtnUnit.talkMarkImg.enabled)
            {
                 return;
            }

            if (talkBtnUnit.talkBtnType != TalkBtnType.MicAllOff)
            {
                if (talkBtnUnit.talkBtnType == TalkBtnType.MicChannelOn && !isOn)
                {
                    return;
                }
                if (talkBtnUnit.talkOnImg != null)
                {
                    talkBtnUnit.talkOnImg.enabled = isOn;
                }                
            }
            if (talkBtnUnit.talkBtnType != TalkBtnType.MicChannelOn)
            {
                if (talkBtnUnit.talkBtnType == TalkBtnType.MicAllOff && isOn)
                {
                    return;
                }
                if (talkBtnUnit.talkOffImg != null)
                {
                    talkBtnUnit.talkOffImg.enabled = !isOn;
                }
            }
        }

        public void SetBtnMark(string mark)
        {
            if (null == talkBtnUnit)
            {
                return;
            }
            if (talkBtnUnit.talkMark)
            {
                talkBtnUnit.talkMark.text = mark;
            }
        }

        public void InitView(ComVoiceTalkButtonUnit btnUnit)
        {
            talkBtnUnit = btnUnit;            
        }

        #region PUBLIC STATIC METHODS
        #endregion
        
        #endregion
    }
}
