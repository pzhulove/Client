using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;

namespace _Settings
{
    public enum GraphicLevel
    {
        Low = 0,
        Mid,
        High
    }

    public enum PlayerDisplayNum
    {
        NoOther = 0,
        Five = 5,
        Ten = 10,
        Fifteen = 15,
        Twenty = 20
    }

    public class SystemInfoSettings : SettingsBindUI
    {

        #region UI View

        GeUISwitchButton bgSoundSwitch;
        Slider bgVolumnSlider;
        UIGray bgVolumnSliderGray;
        GeUISwitchButton seSoundSwitch;
        Slider seVolumnSlider;
        UIGray seVolumnSliderGray;
        Toggle qualityLv0;
        Toggle qualityLv1;
        Toggle qualityLv2;
        Toggle playerDisplayLv1;
        Toggle playerDisplayLv2;
        Toggle playerDisplayLv3;
        Toggle playerDisplayLv4;
        Toggle playerDisplayLv5;
        Toggle teamAskVibrateSwitch;
        Toggle pk3v3VibrateSwitch;
        Toggle mysteryShopSwitch;

        Toggle inviteFriendLvMask1 = null;
        Toggle inviteFriendLvMask2 = null;
        Toggle inviteFriendLvMask3 = null;
        Toggle inviteFriendLvMask4 = null;
        Toggle guildMask = null;
        Toggle teamMask = null;
        Toggle pkMask = null;
        private ComCommonBind mDungeonDisSet = null;
        private GeUISwitchButton mEnvBgSoundSwitch = null;
        private Slider mEnvBgSoundSlider = null;

#endregion
        public SystemInfoSettings(GameObject root, ClientFrame frame)
            : base(root, frame)
        { 
            
        }

        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/systemSet";
        }

        protected override void InitBind()
        {
            bgSoundSwitch = mBind.GetCom<GeUISwitchButton>("BgSoundSwitch");
            bgSoundSwitch.onValueChanged.AddListener(OnBgSoundSwitchChange);
            bgVolumnSlider = mBind.GetCom<Slider>("BgVolumnSlider");
            bgVolumnSlider.onValueChanged.AddListener(OnBgVolumnSliderChange);
            bgVolumnSliderGray = mBind.GetCom<UIGray>("BgVolumnSliderGray");

            seSoundSwitch = mBind.GetCom<GeUISwitchButton>("SESoundSwitch");
            seSoundSwitch.onValueChanged.AddListener(OnSESoundSwitchChange);
            seVolumnSlider = mBind.GetCom<Slider>("SEVolumnSlider");
            seVolumnSlider.onValueChanged.AddListener(OnSEVolumnSliderChange);
            seVolumnSliderGray = mBind.GetCom<UIGray>("SEVolumnSliderGray");

            qualityLv0 = mBind.GetCom<Toggle>("QualityLv0");
            qualityLv0.onValueChanged.AddListener(OnQualityLv0Change);
            qualityLv1 = mBind.GetCom<Toggle>("QualityLv1");
            qualityLv1.onValueChanged.AddListener(OnQualityLv1Change);
            qualityLv2 = mBind.GetCom<Toggle>("QualityLv2");
            qualityLv2.onValueChanged.AddListener(OnQualityLv2Change);

            playerDisplayLv1 = mBind.GetCom<Toggle>("PlayerDisplayLv1");
            playerDisplayLv1.onValueChanged.AddListener(OnPlayerDisplayLv1Change);
            playerDisplayLv2 = mBind.GetCom<Toggle>("PlayerDisplayLv2");
            playerDisplayLv2.onValueChanged.AddListener(OnPlayerDisplayLv2Change);
            playerDisplayLv3 = mBind.GetCom<Toggle>("PlayerDisplayLv3");
            playerDisplayLv3.onValueChanged.AddListener(OnPlayerDisplayLv3Change);
            playerDisplayLv4 = mBind.GetCom<Toggle>("PlayerDisplayLv4");
            playerDisplayLv4.onValueChanged.AddListener(OnPlayerDisplayLv4Change);
            playerDisplayLv5 = mBind.GetCom<Toggle>("PlayerDisplayLv5");
            playerDisplayLv5.onValueChanged.AddListener(OnPlayerDisplayLv5Change);

            teamAskVibrateSwitch = mBind.GetCom<Toggle>("TeamVibrateSwitch");
            teamAskVibrateSwitch.onValueChanged.AddListener(_OnTeamAskVibrateSwitchChange);
            pk3v3VibrateSwitch = mBind.GetCom<Toggle>("Pk3v3VibrateSwitch");
            pk3v3VibrateSwitch.onValueChanged.AddListener(_OnPk3v3VibrateSwitchChange);
            mysteryShopSwitch = mBind.GetCom<Toggle>("MysteryShopSwitch");
            mysteryShopSwitch.onValueChanged.AddListener(_OnMysteryShopSwitchChange);

            inviteFriendLvMask1 = mBind.GetCom<Toggle>("inviteFriendLvMask1");
            inviteFriendLvMask1.SafeAddOnValueChangedListener((bool value) => 
            {
                if(value)
                {
                    InviteFriendLvLimit = 10;
                    SendChangeGameInviteSet();
                }
            });
            inviteFriendLvMask2 = mBind.GetCom<Toggle>("inviteFriendLvMask2");
            inviteFriendLvMask2.SafeAddOnValueChangedListener((bool value) => 
            {
                if (value)
                {
                    InviteFriendLvLimit = 20;
                    SendChangeGameInviteSet();
                }
            });
            inviteFriendLvMask3 = mBind.GetCom<Toggle>("inviteFriendLvMask3");
            inviteFriendLvMask3.SafeAddOnValueChangedListener((bool value) => 
            {
                if (value)
                {
                    InviteFriendLvLimit = 30;
                    SendChangeGameInviteSet();
                }
            });
            inviteFriendLvMask4 = mBind.GetCom<Toggle>("inviteFriendLvMask4");
            inviteFriendLvMask4.SafeAddOnValueChangedListener((bool value) => 
            {
                if (value)
                {
                    InviteFriendLvLimit = 40;
                    SendChangeGameInviteSet();
                }
            });
            guildMask = mBind.GetCom<Toggle>("guildMask");
            guildMask.SafeAddOnValueChangedListener((bool value) => 
            {
                MaskGuildInvite = value;
                SendChangeGameSecretSet();
            });
            teamMask = mBind.GetCom<Toggle>("teamMask");
            teamMask.SafeAddOnValueChangedListener((bool value) => 
            {
                MaskTeamInvite = value;
                SendChangeGameSecretSet();
            });
            pkMask = mBind.GetCom<Toggle>("pkMask");
            pkMask.SafeAddOnValueChangedListener((bool value) => 
            {
                MaskPkInvite = value;
                SendChangeGameSecretSet();
            });
            mDungeonDisSet = mBind.GetCom<ComCommonBind>("DungeonDisSet");

            InitDungeonDisBind();

            mEnvBgSoundSwitch = mBind.GetCom<GeUISwitchButton>("EnvBgSoundSwitch");
            if (mEnvBgSoundSwitch != null)
                mEnvBgSoundSwitch.onValueChanged.AddListener(OnEnvBgSoundSwitchChange);

	        mEnvBgSoundSlider = mBind.GetCom<Slider>("EnvBgSoundSlider");
            if (mEnvBgSoundSlider != null)
                mEnvBgSoundSlider.onValueChanged.AddListener(OnEnvBgVolumnSliderChange);

        }

        protected override void UnInitBind()
        {
            if (mEnvBgSoundSwitch != null)
                mEnvBgSoundSwitch.onValueChanged.RemoveListener(OnEnvBgSoundSwitchChange);
            if (mEnvBgSoundSlider != null)
                mEnvBgSoundSlider.onValueChanged.RemoveListener(OnEnvBgVolumnSliderChange);

            if(bgSoundSwitch!=null)
               bgSoundSwitch.onValueChanged.RemoveListener(OnBgSoundSwitchChange);
            bgSoundSwitch = null;
            if (bgVolumnSlider != null)
                bgVolumnSlider.onValueChanged.RemoveListener(OnBgVolumnSliderChange);
            bgVolumnSlider = null;
            bgVolumnSliderGray = null;
            if (seSoundSwitch!=null)
                seSoundSwitch.onValueChanged.RemoveListener(OnSESoundSwitchChange); 
            seSoundSwitch = null;
            if(seVolumnSlider!=null)
               seVolumnSlider.onValueChanged.RemoveListener(OnSEVolumnSliderChange);
            seVolumnSlider = null;
            seVolumnSliderGray = null;

            if (qualityLv0!=null)
                qualityLv0.onValueChanged.RemoveListener(OnQualityLv0Change);
            qualityLv0 = null;
            if (qualityLv1!=null)
                qualityLv1.onValueChanged.RemoveListener(OnQualityLv1Change);
            qualityLv1 = null;
            if(qualityLv2!=null)
               qualityLv2.onValueChanged.RemoveListener(OnQualityLv2Change);
            qualityLv2 = null;

            if(playerDisplayLv1!=null )
               playerDisplayLv1.onValueChanged.RemoveListener(OnPlayerDisplayLv1Change);
            playerDisplayLv1 = null;
            if(playerDisplayLv2!=null)
                playerDisplayLv2.onValueChanged.RemoveListener(OnPlayerDisplayLv2Change);
            playerDisplayLv2 = null;
            if (playerDisplayLv3 != null)
                 playerDisplayLv3.onValueChanged.RemoveListener(OnPlayerDisplayLv3Change);
            playerDisplayLv3 = null;
            if (playerDisplayLv4 != null)
                 playerDisplayLv4.onValueChanged.RemoveListener(OnPlayerDisplayLv4Change);
            playerDisplayLv4 = null;
            if (playerDisplayLv5 != null)
                playerDisplayLv5.onValueChanged.RemoveListener(OnPlayerDisplayLv5Change);
            playerDisplayLv5 = null;

            if (teamAskVibrateSwitch != null)
                teamAskVibrateSwitch.onValueChanged.RemoveListener(_OnTeamAskVibrateSwitchChange);
            teamAskVibrateSwitch = null;
            if (pk3v3VibrateSwitch != null)
                pk3v3VibrateSwitch.onValueChanged.RemoveListener(_OnPk3v3VibrateSwitchChange);
            pk3v3VibrateSwitch = null;
            if (mysteryShopSwitch != null)
                mysteryShopSwitch.onValueChanged.RemoveListener(_OnMysteryShopSwitchChange);
            mysteryShopSwitch = null;

            inviteFriendLvMask1 = null;
            inviteFriendLvMask2 = null;
            inviteFriendLvMask3 = null;
            inviteFriendLvMask4 = null;
            guildMask = null;
            teamMask = null;
            pkMask = null;

            if (mDungeonDisSet != null)
            {
                mDungeonDisSet = null;
                UnInitDungeonDisBind();
            }

            mEnvBgSoundSwitch = null;
	        mEnvBgSoundSlider = null;
        }

        protected override void OnShowOut()
        {
            GeGraphicSetting.CreateInstance();
            InitViewSoundVolumn();
            InitGraphicQualityLevel();
            InitPlayerDisplayLevel();
            _InitVibrateSet();
            _InitInviteSet();
            _InitPrivacySet();
            InitDataByType(SettingManager.STR_SUMMONDISPLAY);
            InitDataByType(SettingManager.STR_SKILLEFFECTDISPLAY);
            InitDataByType(SettingManager.STR_HITNUMDISPLAY);
        }

        protected override void OnHideIn()
        {
            SystemConfigManager.GetInstance().SaveConfig();
            PlayerLocalSetting.SaveConfig();
            GeGraphicSetting.instance.SaveSetting();
        }

        #region UI Callback

        void InitViewSoundVolumn()
        {
            if (bgSoundSwitch && bgVolumnSlider && seVolumnSlider && seSoundSwitch)
            {
                bgSoundSwitch.SetSwitch(!SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute);
                seSoundSwitch.SetSwitch(!SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Mute);

                bgVolumnSlider.value = (float)SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume;
                seVolumnSlider.value = (float)SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume;
            }

            if (mEnvBgSoundSwitch != null && mEnvBgSoundSlider != null)
            {
                mEnvBgSoundSwitch.SetSwitch(!SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Mute);
                mEnvBgSoundSlider.value = (float)SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Volume;
            }
        }

        void OnEnvBgSoundSwitchChange(bool isOn)
        {
            SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Mute = !isOn;
            AudioManager.instance.SetMute(AudioType.AudioEnvironment, !isOn);
        }

        void OnEnvBgVolumnSliderChange(float value)
        {
            if (mEnvBgSoundSlider != null)
            {
                SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Volume = mEnvBgSoundSlider.value;
                AudioManager.instance.SetVolume(AudioType.AudioEnvironment, mEnvBgSoundSlider.value);
            }
        }

        void OnBgSoundSwitchChange(bool isOn)
        {
            _SwitchBgSound(isOn);
        }

        void OnBgVolumnSliderChange(float value)
        {
            _AdjustBgVolumn(value);
        }

        void OnSESoundSwitchChange(bool isOn)
        {
            _SwitchSESound(isOn);
        }

        void OnSEVolumnSliderChange(float value)
        {
            _AdjustSEVolumn(value);
        }

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

        void OnPlayerDisplayLv1Change(bool isOn)
        {
            if (isOn)
            {
                _SetPlayerDisplayNum((int)PlayerDisplayNum.NoOther);
            }
        }

        void OnPlayerDisplayLv2Change(bool isOn)
        {
            if (isOn)
            {
                _SetPlayerDisplayNum((int)PlayerDisplayNum.Five);
            }
        }

        void OnPlayerDisplayLv3Change(bool isOn)
        {
            if (isOn)
                _SetPlayerDisplayNum((int)PlayerDisplayNum.Ten);
        }

        void OnPlayerDisplayLv4Change(bool isOn)
        {
            if (isOn)
                _SetPlayerDisplayNum((int)PlayerDisplayNum.Fifteen);
        }

        void OnPlayerDisplayLv5Change(bool isOn)
        {
            if (isOn)
                _SetPlayerDisplayNum((int)PlayerDisplayNum.Twenty);
        }

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

        void InitPlayerDisplayLevel()
        {
            if (playerDisplayLv1 && playerDisplayLv2 && playerDisplayLv3 && playerDisplayLv4 && playerDisplayLv5)
            {
                int playerDisplay = 0;
                if (GeGraphicSetting.instance.GetSetting("PlayerDisplayNum", ref playerDisplay))
                {
                    playerDisplayLv1.isOn = ((int)PlayerDisplayNum.NoOther == playerDisplay);
                    playerDisplayLv2.isOn = ((int)PlayerDisplayNum.Five == playerDisplay);
                    playerDisplayLv3.isOn = ((int)PlayerDisplayNum.Ten == playerDisplay);
                    playerDisplayLv4.isOn = ((int)PlayerDisplayNum.Fifteen == playerDisplay);
                    playerDisplayLv5.isOn = ((int)PlayerDisplayNum.Twenty == playerDisplay);
                }
                else
                {
                    if (playerDisplayLv1.isOn)
                        playerDisplay = (int)PlayerDisplayNum.NoOther;
                    else if (playerDisplayLv2.isOn)
                        playerDisplay = (int)PlayerDisplayNum.Five;
                    else if (playerDisplayLv3.isOn)
                        playerDisplay = (int)PlayerDisplayNum.Ten;
                    else if (playerDisplayLv4.isOn)
                        playerDisplay = (int)PlayerDisplayNum.Fifteen;
                    else
                        playerDisplay = (int)PlayerDisplayNum.Twenty;

                    _SetPlayerDisplayNum(playerDisplay);
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

        void _InitInviteSet()
        {
            Toggle toggle = null;
            InviteFriendLvLimit = SystemConfigManager.GetInstance().InviteFriendLvLimit;
            if (InviteFriendLvLimit == 10)
            {
                toggle = inviteFriendLvMask1;
            }
            else if(InviteFriendLvLimit == 20)
            {
                toggle = inviteFriendLvMask2;
            }
            else if(InviteFriendLvLimit == 30)
            {
                toggle = inviteFriendLvMask3;
            }
            else if(InviteFriendLvLimit == 40)
            {
                toggle = inviteFriendLvMask4;
            }
            toggle.SafeSetToggleOnState(true);
        }
        void _InitPrivacySet()
        {
            MaskGuildInvite = SystemConfigManager.GetInstance().MaskGuildInvite;
            MaskTeamInvite = SystemConfigManager.GetInstance().MaskTeamInvite;
            MaskPkInvite = SystemConfigManager.GetInstance().MaskPkInvite;
            guildMask.SafeSetToggleOnState(MaskGuildInvite);
            teamMask.SafeSetToggleOnState(MaskTeamInvite);
            pkMask.SafeSetToggleOnState(MaskPkInvite);
        }
        void SendChangeGameInviteSet()
        {
            if(InviteFriendLvLimit == SystemConfigManager.GetInstance().InviteFriendLvLimit)
            {
                return;
            }
            SystemConfigManager.GetInstance().SendSceneGameSetReq(Protocol.GameSetType.GST_FRIEND_INVATE, (uint)InviteFriendLvLimit);
        }
        void SendChangeGameSecretSet()
        {
            if(MaskGuildInvite == SystemConfigManager.GetInstance().MaskGuildInvite &&
                MaskTeamInvite == SystemConfigManager.GetInstance().MaskTeamInvite &&
                MaskPkInvite == SystemConfigManager.GetInstance().MaskPkInvite)
            {
                return;
            }
            uint setValue = 0;
            if(MaskGuildInvite)
            {
                setValue |= (uint)Protocol.SecretSetType.SST_GUILD_INVATE;
            }
            if(MaskTeamInvite)
            {
                setValue |= (uint)Protocol.SecretSetType.SST_TEAM_INVATE;
            }
            if(MaskPkInvite)
            {
                setValue |= (uint)Protocol.SecretSetType.SST_DUEL_INVATE;
            }
            SystemConfigManager.GetInstance().SendSceneGameSetReq(Protocol.GameSetType.GST_SECRET, setValue);
        }

        #endregion

        #region Func Ctrl 

        private void _AdjustBgVolumn(float value)
        {
            if (bgVolumnSlider)
            {
                SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume = bgVolumnSlider.value;
                AudioManager.instance.SetVolume(AudioType.AudioStream, bgVolumnSlider.value);
            }
        }

        private void _SwitchBgSound(bool value)
        {
            SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute = !value;
            AudioManager.instance.SetMute(AudioType.AudioStream, !value);
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

        private void _SwitchSESound(bool value)
        {
            AudioManager.instance.SetMute(AudioType.AudioEffect, !value);
            AudioManager.instance.SetMute(AudioType.AudioVoice, !value);
            SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Mute = !value;
        }


        private void _SwitchGraphicQaulity(int level)
        {
			if (GeGraphicSetting.instance.GetGraphicLevel() == level)
				return;

			GeGraphicSetting.instance.SetGraphicLevel((global::GraphicLevel)level);
			InitPlayerDisplayLevel();
            SetDataByGraphicLevel();
#if !LOGIC_SERVER
            SetActorSimpleModeEnable(level != (int)global::GraphicLevel.NORMAL);
#endif
        }

        private void _SetPlayerDisplayNum(int number)
        {
            if (!GeGraphicSetting.instance.SetSetting("PlayerDisplayNum", number))
                GeGraphicSetting.instance.AddSetting("PlayerDisplayNum", number);

            ClientSystemTown curTown = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
            if (null != curTown)
                curTown.OnGraphicSettingChange(number);
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

        // 游戏设置数据（缓存数据,通过UI交互修改）
        #region game set value
        private static int InviteFriendLvLimit
        {
            set;
            get;
        }
        private static bool MaskGuildInvite
        {
            set;
            get;
        }
        private static bool MaskTeamInvite
        {
            set;
            get;
        }
        private static bool MaskPkInvite
        {
            set;
            get;
        }
        #endregion
        #endregion

        #region 地下城显示设置相关

        private Toggle m_SummonOpenToggle = null;
        private Toggle m_SummonCloseToggle = null;
        private Toggle m_SkillEffectOpenToggle = null;
        private Toggle m_SkillEffectCloseToggle = null;
        private Toggle m_HitNumOpenToggle = null;
        private Toggle m_HitNumCloseToggle = null;

        private void InitDungeonDisBind()
        {
            if (mDungeonDisSet == null)
                return;
            m_SummonOpenToggle = mDungeonDisSet.GetCom<Toggle>("SummonOpen");
            m_SummonCloseToggle = mDungeonDisSet.GetCom<Toggle>("SummonClose");
            m_SkillEffectOpenToggle = mDungeonDisSet.GetCom<Toggle>("SkillEffectOpen");
            m_SkillEffectCloseToggle = mDungeonDisSet.GetCom<Toggle>("SkillEffectClose");
            m_HitNumOpenToggle = mDungeonDisSet.GetCom<Toggle>("HitNumOpen");
            m_HitNumCloseToggle = mDungeonDisSet.GetCom<Toggle>("HitNumClose");

            if (m_SummonOpenToggle != null)
                m_SummonOpenToggle.onValueChanged.AddListener(OnSummonSetOpenDis);
            if (m_SummonCloseToggle != null)
                m_SummonCloseToggle.onValueChanged.AddListener(OnSummonSetCloseDis);
            if (m_SkillEffectOpenToggle != null)
                m_SkillEffectOpenToggle.onValueChanged.AddListener(OnSkillEffectOpenDis);
            if (m_SkillEffectCloseToggle != null)
                m_SkillEffectCloseToggle.onValueChanged.AddListener(OnSkillEffectCloseDis);
            if (m_HitNumOpenToggle != null)
                m_HitNumOpenToggle.onValueChanged.AddListener(OnHitNumOpenDis);
            if (m_HitNumCloseToggle != null)
                m_HitNumCloseToggle.onValueChanged.AddListener(OnHitNumCloseDis);
        }

        private void UnInitDungeonDisBind()
        {
            if (mDungeonDisSet == null)
                return;
            if (m_SummonOpenToggle != null)
            {
                m_SummonOpenToggle.onValueChanged.RemoveListener(OnSummonSetOpenDis);
                m_SummonOpenToggle = null;
            }
            if (m_SummonCloseToggle != null)
            {
                m_SummonCloseToggle.onValueChanged.RemoveListener(OnSummonSetCloseDis);
                m_SummonCloseToggle = null;
            }
            if (m_SkillEffectOpenToggle != null)
            {
                m_SkillEffectOpenToggle.onValueChanged.RemoveListener(OnSkillEffectOpenDis);
                m_SkillEffectOpenToggle = null;
            }
            if (m_SkillEffectCloseToggle != null)
            {
                m_SkillEffectCloseToggle.onValueChanged.RemoveListener(OnSkillEffectCloseDis);
                m_SkillEffectCloseToggle = null;
            }
            if (m_HitNumOpenToggle != null)
            {
                m_HitNumOpenToggle.onValueChanged.RemoveListener(OnHitNumOpenDis);
                m_HitNumOpenToggle = null;
            }
            if (m_HitNumCloseToggle != null)
            {
                m_HitNumCloseToggle.onValueChanged.RemoveListener(OnHitNumCloseDis);
                m_HitNumCloseToggle = null;
            }
        }

        protected void OnSummonSetOpenDis(bool value)
        {
            OnDisSetValueChange(value, SettingManager.STR_SUMMONDISPLAY, "open");
        }

        protected void OnSummonSetCloseDis(bool value)
        {
            OnDisSetValueChange(value, SettingManager.STR_SUMMONDISPLAY, "close");
        }

        protected void OnSkillEffectOpenDis(bool value)
        {
            OnDisSetValueChange(value, SettingManager.STR_SKILLEFFECTDISPLAY, "open");
        }

        protected void OnSkillEffectCloseDis(bool value)
        {
            OnDisSetValueChange(value, SettingManager.STR_SKILLEFFECTDISPLAY, "close");
        }

        protected void OnHitNumOpenDis(bool value)
        {
            OnDisSetValueChange(value, SettingManager.STR_HITNUMDISPLAY, "open");
        }

        protected void OnHitNumCloseDis(bool value)
        {
            OnDisSetValueChange(value,SettingManager.STR_HITNUMDISPLAY, "close");
        }

        /// <summary>
        /// 显示设置按钮点击触发
        /// </summary>
        protected void OnDisSetValueChange(bool flag, string key,string value)
        {
            if (!flag)
                return;
            SettingManager.GetInstance().SetCommomData(key, value);
        }

        private void SetDataByGraphicLevel()
        {
            RefreshDungeonDisByLevel(SettingManager.STR_SUMMONDISPLAY);
            RefreshDungeonDisByLevel(SettingManager.STR_SKILLEFFECTDISPLAY);
            RefreshDungeonDisByLevel(SettingManager.STR_HITNUMDISPLAY);
        }
#if !LOGIC_SERVER
        private void SetActorSimpleModeEnable(bool value)
        {
            ClientSystemTown curTown = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
            if (null != curTown)
            {
                curTown.SetActorSimpleModeEnable(value);
            }
        }
#endif       
        /// <summary>
        /// 根据类型初始化开关数据
        /// </summary>
        private void InitDataByType(string type)
        {
            var openToggle = GetDungeonDisToggleByType(type,true);
            var closeToggle = GetDungeonDisToggleByType(type, false);

            var data = SettingManager.GetInstance().GetCommmonSet(type);
            if (data == SettingManager.SetCommonType.None)
            {
                SetDataByGraphicLevel();
            }
            else if (data == SettingManager.SetCommonType.Open)
            {
                openToggle.isOn = true;
            }
            else
            {
                closeToggle.isOn = true;
            }
        }

        /// <summary>
        /// 由画质设置按钮那边触发
        /// </summary>
        private void RefreshDungeonDisByLevel(string type)
        {
            var openToggle = GetDungeonDisToggleByType(type, true);
            var closeToggle = GetDungeonDisToggleByType(type, false);

            if (GeGraphicSetting.instance.IsHighLevel() || GeGraphicSetting.instance.IsMiddleLevel())
            {
                openToggle.isOn = true;
            }
            else
            {
                closeToggle.isOn = true;
            }
        }

        //根据类型获取对应的开关按钮
        private Toggle GetDungeonDisToggleByType(string type, bool isOpen)
        {
            Toggle toggle = null;
            switch (type)
            {
                case SettingManager.STR_SUMMONDISPLAY:
                    toggle = isOpen ? m_SummonOpenToggle : m_SummonCloseToggle;
                    break;
                case SettingManager.STR_SKILLEFFECTDISPLAY:
                    toggle = isOpen ? m_SkillEffectOpenToggle : m_SkillEffectCloseToggle;
                    break;
                case SettingManager.STR_HITNUMDISPLAY:
                    toggle = isOpen ? m_HitNumOpenToggle : m_HitNumCloseToggle;
                    break;
            }
            return toggle;
        }

        #endregion
    }
}