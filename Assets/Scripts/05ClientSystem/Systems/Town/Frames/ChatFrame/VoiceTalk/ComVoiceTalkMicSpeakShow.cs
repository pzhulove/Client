using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComVoiceTalkMicSpeakShow : MonoBehaviour
    {
        public const string RES_PATH = "UIFlatten/Prefabs/Common/ComVoice/VoiceTalkMicSpeakShow";

        [SerializeField]
        private Image micImg;
        [SerializeField]
        private Image leftVoiceImg;
        [SerializeField]
        private Image rightVoiceImg;
        public float voiceAnimDuration = 0.5f;

        private float timer;
        private bool bShow = false;
        private void Start()
        {
            _ResetShow();
        }

        private void Update()
        {
            if(!bShow)
            {
                return;
            }
            if(timer <= voiceAnimDuration)
            {
                timer += Time.deltaTime;
            }
            if(timer > voiceAnimDuration)
            {
                _ChangeVoiceShow();
                timer = 0f;
            }
        }

        private void _ResetShow()
        {
            timer = 0f;
            Hide();
        }

        private void _ChangeVoiceShow()
        {
            if(leftVoiceImg)
            {
                leftVoiceImg.enabled = !leftVoiceImg.enabled;
            }
            if(rightVoiceImg)
            {
                rightVoiceImg.enabled = !rightVoiceImg.enabled;
            }
        }

        private void _SetShow(bool bShow)
        {
            if(micImg)
            {
                micImg.enabled = bShow;
            }
            if(leftVoiceImg)
            {
                leftVoiceImg.enabled = bShow;
            }
            if(rightVoiceImg)
            {
                rightVoiceImg.enabled = bShow;
            }
        }

        public void Show()
        {
            _SetShow(true);
            bShow = true;
        }

        public void Hide()
        {
            _SetShow(false);
            bShow = false;
        }
    }
}
