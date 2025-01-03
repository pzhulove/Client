using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class PauseSystemSetView : MonoBehaviour
    {
        public enum GraphicLevel
        {
            Low = 0,
            Mid,
            High
        }
        [SerializeField] private GeUISwitchButton bgEnvBGMSwitch;
        [SerializeField] private Slider envBgVolumnSlider;
        [SerializeField] private GeUISwitchButton bgSoundSwitch;
        [SerializeField] private Slider bgVolumnSlider;
        [SerializeField] private GeUISwitchButton seSoundSwitch;
        [SerializeField] private Slider seVolumnSlider;

        [SerializeField] private Toggle qualityLv0;
        [SerializeField] private Toggle qualityLv1;
        [SerializeField] private Toggle qualityLv2;
        [SerializeField] private Toggle teamAskVibrateSwitch;
        [SerializeField] private Toggle pk3v3VibrateSwitch;
        [SerializeField] private Toggle mysteryShopSwitch;
        public void OnInit()
        {
            bgSoundSwitch.onValueChanged.AddListener(OnBgSoundSwitchChange);
            bgVolumnSlider.onValueChanged.AddListener(OnBgVolumnSliderChange);
            seSoundSwitch.onValueChanged.AddListener(OnSESoundSwitchChange);
            seVolumnSlider.onValueChanged.AddListener(OnSEVolumnSliderChange);
            if (bgEnvBGMSwitch != null) bgEnvBGMSwitch.onValueChanged.AddListener(OnEnvBgSoundSwitchChange);
            if (envBgVolumnSlider != null) envBgVolumnSlider.onValueChanged.AddListener(OnEnvBgVolumnSliderChange);

            InitViewSoundVolumn();

            qualityLv0.onValueChanged.AddListener(OnQualityLv0Change);
            qualityLv1.onValueChanged.AddListener(OnQualityLv1Change);
            qualityLv2.onValueChanged.AddListener(OnQualityLv2Change);
            InitGraphicQualityLevel();

            teamAskVibrateSwitch.onValueChanged.AddListener(_OnTeamAskVibrateSwitchChange);
            pk3v3VibrateSwitch.onValueChanged.AddListener(_OnPk3v3VibrateSwitchChange);
            mysteryShopSwitch.onValueChanged.AddListener(_OnMysteryShopSwitchChange);
            _InitVibrateSet();
        }

        void InitViewSoundVolumn()
        {
            if (bgSoundSwitch && bgVolumnSlider && seVolumnSlider && seSoundSwitch)
            {
                bgSoundSwitch.SetSwitch(!SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute);
                seSoundSwitch.SetSwitch(!SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Mute);

                bgVolumnSlider.value = (float)SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume;
                seVolumnSlider.value = (float)SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume;
            }

            if (bgEnvBGMSwitch != null && envBgVolumnSlider != null)
            {
                bgEnvBGMSwitch.SetSwitch(!SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Mute);
                envBgVolumnSlider.value = (float)SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Volume;
            }
        }
        void InitGraphicQualityLevel()
        {
            if (qualityLv0 && qualityLv1 && qualityLv2)
            {
                int graphicQuality = 0;
                if (GeGraphicSetting.instance.GetSetting("GraphicLevel", ref graphicQuality))
                {
                    qualityLv0.isOn = ((int)GraphicLevel.Low == graphicQuality);
                    qualityLv1.isOn = ((int)GraphicLevel.Mid == graphicQuality);
                    qualityLv2.isOn = ((int)GraphicLevel.High == graphicQuality);
                }
                else
                {
                    if (qualityLv0.isOn)
                        graphicQuality = (int)GraphicLevel.Low;
                    else if (qualityLv1.isOn)
                        graphicQuality = (int)GraphicLevel.Mid;
                    else
                        graphicQuality = (int)GraphicLevel.High;

                    _SwitchGraphicQaulity(graphicQuality);
                }
            }
        }
        void _InitVibrateSet()
        {
            if (teamAskVibrateSwitch != null)
            {
                teamAskVibrateSwitch.isOn = DeviceVibrateManager.GetInstance().CheckVibrateSwitchOpen(VibrateSwitchType.Team);
            }
            if (pk3v3VibrateSwitch != null)
            {
                pk3v3VibrateSwitch.isOn = DeviceVibrateManager.GetInstance().CheckVibrateSwitchOpen(VibrateSwitchType.Pk3v3);
            }

            if (mysteryShopSwitch != null)
            {
                mysteryShopSwitch.isOn = DeviceVibrateManager.GetInstance().CheckVibrateSwitchOpen(VibrateSwitchType.MysteryShop);
            }
        }

        public void OnUnInit()
        {
            if (bgEnvBGMSwitch != null) bgEnvBGMSwitch.onValueChanged.RemoveListener(OnEnvBgSoundSwitchChange);
            if (envBgVolumnSlider != null) envBgVolumnSlider.onValueChanged.RemoveListener(OnEnvBgVolumnSliderChange);

            bgSoundSwitch.onValueChanged.RemoveListener(OnBgSoundSwitchChange);
            bgVolumnSlider.onValueChanged.RemoveListener(OnBgVolumnSliderChange);
            seSoundSwitch.onValueChanged.RemoveListener(OnSESoundSwitchChange);
            seVolumnSlider.onValueChanged.RemoveListener(OnSEVolumnSliderChange);

            qualityLv0.onValueChanged.RemoveListener(OnQualityLv0Change);
            qualityLv1.onValueChanged.RemoveListener(OnQualityLv1Change);
            qualityLv2.onValueChanged.RemoveListener(OnQualityLv2Change);

            teamAskVibrateSwitch.onValueChanged.RemoveListener(_OnTeamAskVibrateSwitchChange);
            pk3v3VibrateSwitch.onValueChanged.RemoveListener(_OnPk3v3VibrateSwitchChange);
            mysteryShopSwitch.onValueChanged.RemoveListener(_OnMysteryShopSwitchChange);
        }

#region 控制声音
        void OnEnvBgSoundSwitchChange(bool isOn)
        {
            SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Mute = !isOn;
            AudioManager.instance.SetMute(AudioType.AudioEnvironment, !isOn);
        }

        void OnEnvBgVolumnSliderChange(float value)
        {
            if (envBgVolumnSlider != null)
            {
                SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Volume = envBgVolumnSlider.value;
                AudioManager.instance.SetVolume(AudioType.AudioEnvironment, envBgVolumnSlider.value);
            }
        }

        void OnBgSoundSwitchChange(bool isOn)
        {
            _SwitchBgSound(isOn);
        }
        private void _SwitchBgSound(bool value)
        {
            SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute = !value;
            AudioManager.instance.SetMute(AudioType.AudioStream, !value);
        }

        void OnBgVolumnSliderChange(float value)
        {
            _AdjustBgVolumn(value);
        }
        private void _AdjustBgVolumn(float value)
        {
            if (bgVolumnSlider)
            {
                SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume = bgVolumnSlider.value;
                AudioManager.instance.SetVolume(AudioType.AudioStream, bgVolumnSlider.value);
            }
        }

        void OnSESoundSwitchChange(bool isOn)
        {
            _SwitchSESound(isOn);
        }
        private void _SwitchSESound(bool value)
        {
            AudioManager.instance.SetMute(AudioType.AudioEffect, !value);
            AudioManager.instance.SetMute(AudioType.AudioVoice, !value);
            SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Mute = !value;
        }

        void OnSEVolumnSliderChange(float value)
        {
            _AdjustSEVolumn(value);
        }
        private void _AdjustSEVolumn(float value)
        {
            if (seVolumnSlider)
            {
                SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume = seVolumnSlider.value;
                AudioManager.instance.SetVolume(AudioType.AudioEffect, seVolumnSlider.value);
                AudioManager.instance.SetVolume(AudioType.AudioVoice, seVolumnSlider.value);
                NpcVoiceCachedManager.instance.SetVolume(seVolumnSlider.value);
            }
        }
#endregion

#region 画质
        void OnQualityLv0Change(bool isOn)
        {
            if (isOn)
                _SwitchGraphicQaulity((int)GraphicLevel.Low);
        }
        void OnQualityLv1Change(bool isOn)
        {
            if (isOn)
                _SwitchGraphicQaulity((int)GraphicLevel.Mid);
        }
        void OnQualityLv2Change(bool isOn)
        {
            if (isOn)
            {
                _SwitchGraphicQaulity((int)GraphicLevel.High);
            }
        }

        private void _SwitchGraphicQaulity(int level)
        {
			if (GeGraphicSetting.instance.GetGraphicLevel() == level)
				return;

			GeGraphicSetting.instance.SetGraphicLevel((global::GraphicLevel)level);
			InitPlayerDisplayLevel();
        }
        void InitPlayerDisplayLevel()
        {
            int playerDisplay = 0;
            if (GeGraphicSetting.instance.GetSetting("PlayerDisplayNum", ref playerDisplay))
                _SetPlayerDisplayNum(playerDisplay);
        }

        private void _SetPlayerDisplayNum(int number)
        {
            if (!GeGraphicSetting.instance.SetSetting("PlayerDisplayNum", number))
                GeGraphicSetting.instance.AddSetting("PlayerDisplayNum", number);
            ClientSystemTown curTown = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
            if (null != curTown)
                curTown.OnGraphicSettingChange(number);
        }
#endregion

#region 震动
        void _OnTeamAskVibrateSwitchChange(bool isOn)
        {
            _SwitchTeamAskVibrate(isOn);
        }

        void _OnPk3v3VibrateSwitchChange(bool isOn)
        {
            _SwitchPk3v3Vibrate(isOn);
        }

        void _OnMysteryShopSwitchChange(bool isOn)
        {
            _SwitchMysteryShop(isOn);
        }

        private void _SwitchTeamAskVibrate(bool isOn)
        {
            DeviceVibrateManager.GetInstance().SetVibrateSwitch(VibrateSwitchType.Team, isOn);
        }

        private void _SwitchPk3v3Vibrate(bool isOn)
        {
            DeviceVibrateManager.GetInstance().SetVibrateSwitch(VibrateSwitchType.Pk3v3, isOn);
        }

        private void _SwitchMysteryShop(bool isOn)
        {
            DeviceVibrateManager.GetInstance().SetVibrateSwitch(VibrateSwitchType.MysteryShop, isOn);
        }
#endregion
    }
}
