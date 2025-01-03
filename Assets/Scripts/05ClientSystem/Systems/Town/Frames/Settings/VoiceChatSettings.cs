using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using VoiceSDK;

namespace _Settings
{
    public class VoiceChatSettings : SettingsBindUI
    {
        #region UI View

        Toggle worldToggle;
        Toggle teamToggle;
        Toggle guildToggle;
        Toggle nearbyToggle;
        Toggle privateToggle;
        GeUISwitchButton switchMic;
        GeUISwitchButton switchPlayer;
        private Slider mVolumeSlider = null;
        private Toggle mPrivateTenToggle = null;
        private Toggle mPrivateTwentyToggle = null;
        private Toggle mPrivateThirtyToggle = null;
        private Toggle mPrivateFortyToggle = null;

        #endregion

        bool isFrameShowed = false;

        public VoiceChatSettings(GameObject root, ClientFrame frame)
            : base(root, frame)
        {

        }

        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/voiceChatSet";
        }

        protected override void InitBind()
        {
            worldToggle = mBind.GetCom<Toggle>("WorldToggle");
            worldToggle.onValueChanged.AddListener(OnWorldToggleChange);
            teamToggle = mBind.GetCom<Toggle>("TeamToggle");
            teamToggle.onValueChanged.AddListener(OnTeamToggleChange);
            guildToggle = mBind.GetCom<Toggle>("GuildToggle");
            guildToggle.onValueChanged.AddListener(OnGuildToggleChange);
            nearbyToggle = mBind.GetCom<Toggle>("NearbyToggle");
            nearbyToggle.onValueChanged.AddListener(OnNearbyToggleChange);
            privateToggle = mBind.GetCom<Toggle>("PrivateToggle");
            privateToggle.onValueChanged.AddListener(OnPrivateToggleChange);
            switchMic = mBind.GetCom<GeUISwitchButton>("SwitchMic");
            switchMic.onValueChanged.AddListener(OnSwitchMicChange);
            switchPlayer = mBind.GetCom<GeUISwitchButton>("SwitchPlayer");
            switchPlayer.onValueChanged.AddListener(OnSwitchPlayerChange);
            mVolumeSlider = mBind.GetCom<Slider>("VolumeSlider");
            mVolumeSlider.onValueChanged.AddListener(OnVolumeSliderChange);
            mPrivateTenToggle = mBind.GetCom<Toggle>("PrivateTenToggle");
            mPrivateTenToggle.onValueChanged.AddListener(_onPrivateTenToggleToggleValueChange);
            mPrivateTwentyToggle = mBind.GetCom<Toggle>("PrivateTwentyToggle");
            mPrivateTwentyToggle.onValueChanged.AddListener(_onPrivateTwentyToggleToggleValueChange);
            mPrivateThirtyToggle = mBind.GetCom<Toggle>("PrivateThirtyToggle");
            mPrivateThirtyToggle.onValueChanged.AddListener(_onPrivateThirtyToggleToggleValueChange);
            mPrivateFortyToggle = mBind.GetCom<Toggle>("PrivateFortyToggle");
            mPrivateFortyToggle.onValueChanged.AddListener(_onPrivateFortyToggleToggleValueChange);
        }

        protected override void UnInitBind()
        {
            if(worldToggle)
                worldToggle.onValueChanged.RemoveListener(OnWorldToggleChange);
            worldToggle = null;
            if (teamToggle)
                 teamToggle.onValueChanged.RemoveListener(OnTeamToggleChange);
            teamToggle = null;
            if(guildToggle)
                guildToggle.onValueChanged.RemoveListener(OnGuildToggleChange);
            guildToggle = null;
            if(nearbyToggle)
                nearbyToggle.onValueChanged.RemoveListener(OnNearbyToggleChange);
            nearbyToggle = null;
            if(privateToggle)
                 privateToggle.onValueChanged.RemoveListener(OnPrivateToggleChange);
            privateToggle = null;
            if (switchMic)
                switchMic.onValueChanged.RemoveListener(OnSwitchMicChange);
            switchMic = null;
            if(switchPlayer)
                 switchPlayer.onValueChanged.RemoveListener(OnSwitchPlayerChange);
            switchPlayer = null;
            if (mVolumeSlider)
                mVolumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChange);
            mVolumeSlider = null;
            if (mPrivateTenToggle)
                mPrivateTenToggle.onValueChanged.RemoveListener(_onPrivateTenToggleToggleValueChange);
            mPrivateTenToggle = null;
            if(mPrivateTwentyToggle)
                mPrivateTwentyToggle.onValueChanged.RemoveListener(_onPrivateTwentyToggleToggleValueChange);
            mPrivateTwentyToggle = null;
            if (mPrivateThirtyToggle)
                mPrivateThirtyToggle.onValueChanged.RemoveListener(_onPrivateThirtyToggleToggleValueChange);
            mPrivateThirtyToggle = null;
            if (mPrivateFortyToggle)
                mPrivateFortyToggle.onValueChanged.RemoveListener(_onPrivateFortyToggleToggleValueChange);
            mPrivateFortyToggle = null;
        }

        protected override void OnShowOut()
        {
            SetVoiceSDKParams();

            isFrameShowed = true;
        }

        protected override void OnHideIn()
        {
            isFrameShowed = false;
        }

        private void SetVoiceSDKParams()
        {
            switchMic.states = SDKVoiceManager.GetInstance().IsRecordVoiceEnabled;
            switchPlayer.states = SDKVoiceManager.GetInstance().IsPlayVoiceEnabled;
            worldToggle.isOn = SDKVoiceManager.GetInstance().IsAutoPlayInWorld;
            teamToggle.isOn = SDKVoiceManager.GetInstance().IsAutoPlayInTeam;
            guildToggle.isOn = SDKVoiceManager.GetInstance().IsAutoPlayInGuild;
            nearbyToggle.isOn = SDKVoiceManager.GetInstance().IsAutoPlayInNearby;
            privateToggle.isOn = SDKVoiceManager.GetInstance().IsAutoPlayInPrivate;
            mVolumeSlider.value = SDKVoiceManager.GetInstance().VoicePlayerVolume;

            mPrivateTenToggle.isOn = ChatManager.GetInstance().GetIsPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelTen);
            mPrivateTwentyToggle.isOn = ChatManager.GetInstance().GetIsPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelTwenty);
            mPrivateThirtyToggle.isOn = ChatManager.GetInstance().GetIsPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelThirty);
            mPrivateFortyToggle.isOn = ChatManager.GetInstance().GetIsPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelForty);
        }

        #region UI Callback

        void OnWorldToggleChange(bool isOn)
        {
            SDKVoiceManager.GetInstance().SaveWorldAutoPlayValue(isOn);
        }
        void OnTeamToggleChange(bool isOn)
        {
            SDKVoiceManager.GetInstance().SaveTeamAutoPlayValue(isOn);
        }
        void OnGuildToggleChange(bool isOn)
        {
            SDKVoiceManager.GetInstance().SaveGuildAutoPlayValue(isOn);
        }
        void OnNearbyToggleChange(bool isOn)
        {
            SDKVoiceManager.GetInstance().SaveNearbyAutoPlayValue(isOn);
        }
        void OnPrivateToggleChange(bool isOn)
        {
            SDKVoiceManager.GetInstance().SavePrivateAutoPlayValue(isOn);
        }

        void OnSwitchMicChange(bool isOn)
        {
            if (!isFrameShowed)
                return;

            bool isMicOn = SDKVoiceManager.GetInstance().IsTalkRealMicOn();
            if(!isOn && isMicOn)
            {
                SDKVoiceManager.GetInstance().CloseRealMic();
            }

            SDKVoiceManager.GetInstance().SaveMicPref(isOn);
        }
        void OnSwitchPlayerChange(bool isOn)
        {
            if (!isFrameShowed)
                return;

            bool isPlayerOn = SDKVoiceManager.GetInstance().IsTalkRealPlayerOn();
            if(!isOn && isPlayerOn)
            {
                SDKVoiceManager.GetInstance().CloseRealPlayer();
            }
            SDKVoiceManager.GetInstance().SavePlayerPref(isOn);
        }

        void OnVolumeSliderChange(float volumn)
        {
            SDKVoiceManager.GetInstance().SetVoiceVolume(volumn);
            SDKVoiceManager.GetInstance().SetPlayerVolume(volumn);
            SDKVoiceManager.GetInstance().SavePlayerVolumnPref(volumn);
        }

        private void _onPrivateTenToggleToggleValueChange(bool changed)
        {
            if (!isFrameShowed)
                return;
            if (changed)
            {
                ChatManager.GetInstance().SetPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelTen);
            }
        }
        private void _onPrivateTwentyToggleToggleValueChange(bool changed)
        {
            if (!isFrameShowed)
                return;
            if (changed)
            {
                ChatManager.GetInstance().SetPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelTwenty);
            }
        }
        private void _onPrivateThirtyToggleToggleValueChange(bool changed)
        {
            if (!isFrameShowed)
                return;
            if (changed)
            {
                ChatManager.GetInstance().SetPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelThirty);
            }
        }
        private void _onPrivateFortyToggleToggleValueChange(bool changed)
        {
            if (!isFrameShowed)
                return;
            if (changed)
            {
                ChatManager.GetInstance().SetPrivateChatLevelLimit(PrivateChatLevelLimit.LessLevelForty);
            }
        }

        #endregion
    }
}
