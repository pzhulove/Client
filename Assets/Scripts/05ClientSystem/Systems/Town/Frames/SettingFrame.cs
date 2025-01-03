using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using _Settings;
using ProtoTable;


namespace GameClient
{
	public class SettingManager : Singleton<SettingManager>
	{
		public const string STR_JOYSTICKMODE = "SETTING_JOYSTICK1";
        public const string STR_JOYSTICKDIR = "STR_JOYSTICKDIR";
        public const string STR_RUNATTACKMODE = "SETTING_RUNATTACK";
        public const string STR_SHOCKEFFECT = "STR_SHOCKEFFECT";
        public const string STR_PALADINATTACK = "SETTING_PALADINATTACK";
        public const string STR_LIGUI = "SETTING_LIGUI";
        public const string STR_BACKHIT = "STR_BACKHIT";
        public const string STR_AUTOHIT = "STR_AUTOHIT";
        public const string STR_CHASER_SWITCH = "STR_CHASER_SWITCH";
        public const string STR_CHASER_PVE = "STR_CHASER_PVE";
        public const string STR_CHASER_PVP = "STR_CHASER_PVP";

        public const string STR_VIPDRUG = "SETTING_VIPDRUG";
        public const string STR_VIPPREFER = "STR_VIPPREFER";
        public const string STR_VIPHP = "STR_VIPHP";
        public const string STR_VIPREBORN = "SETTING_VIPREBORN";
        public const string STR_VIPCRYSTAL = "SETTING_VIPCRYSTAL";
        public const string STR_SUMMONDISPLAY = "SETTING_SUMMONDISSET";
        public const string STR_SKILLEFFECTDISPLAY = "SETTING_SKILLEFFECTSET";
        public const string STR_HITNUMDISPLAY = "SETTING_HITNUMSET";
        public const int vipDrugTableId = 30;
        public const int vipRebornTableId = 31;
        public const int vipUseCrystalTableId = 32;
        public const int vipPreferTableId = 28;
        #region 摇杆操作相关
        public InputManager.JoystickMode GetJoystickMode()
		{
			InputManager.JoystickMode mode = InputManager.JoystickMode.DYNAMIC;

			if (PlayerLocalSetting.GetValue(STR_JOYSTICKMODE) != null && 
				((string)PlayerLocalSetting.GetValue(STR_JOYSTICKMODE)) ==  "static")
				mode = InputManager.JoystickMode.STATIC;

			return mode;
		}

		public void SetJoystickMode(InputManager.JoystickMode mode)
		{
			PlayerLocalSetting.SetValue(STR_JOYSTICKMODE, mode == InputManager.JoystickMode.DYNAMIC?"dynamic":"static");
			PlayerLocalSetting.SaveConfig();
		}

        public void SetJoystickDir(InputManager.JoystickDir mode)
        {
            PlayerLocalSetting.SetValue(STR_JOYSTICKDIR, mode == InputManager.JoystickDir.MORE_DIR ? "moreDir" : "eightDir");
            PlayerLocalSetting.SaveConfig();
        }

        public InputManager.JoystickDir GetJoystickDir()
        {
            InputManager.JoystickDir mode = InputManager.JoystickDir.MORE_DIR;

            if (PlayerLocalSetting.GetValue(STR_JOYSTICKDIR) != null &&
                ((string)PlayerLocalSetting.GetValue(STR_JOYSTICKDIR)) == "eightDir")
                mode = InputManager.JoystickDir.EIGHT_DIR;

            return mode;
        }
        #endregion

        public InputManager.RunAttackMode GetRunAttackMode()
        {
            InputManager.RunAttackMode mode = InputManager.RunAttackMode.NORMAL;

            //if (PlayerLocalSetting.GetValue(STR_RUNATTACKMODE) != null &&
            //    ((string)PlayerLocalSetting.GetValue(STR_RUNATTACKMODE)) == "qte")
            //    mode = InputManager.RunAttackMode.QTE;

            return mode;
        }

        public void SetRunAttackMode(InputManager.RunAttackMode mode)
        {
            PlayerLocalSetting.SetValue(STR_RUNATTACKMODE, mode == InputManager.RunAttackMode.NORMAL ? "normal" : "qte");
            PlayerLocalSetting.SaveConfig();
        }

        #region CameraShock
        public InputManager.CameraShockMode GetCameraShockMode()
        {
            InputManager.CameraShockMode mode = InputManager.CameraShockMode.OPEN;
            if (PlayerLocalSetting.GetValue(STR_SHOCKEFFECT) != null &&
                ((string)PlayerLocalSetting.GetValue(STR_SHOCKEFFECT)) == "close")
                mode = InputManager.CameraShockMode.CLOSE;
            return mode;
        }
        public void SetCameraShockMode(InputManager.CameraShockMode mode)
        {
            PlayerLocalSetting.SetValue(STR_SHOCKEFFECT, mode == InputManager.CameraShockMode.OPEN ? "open" : "close");
            PlayerLocalSetting.SaveConfig();
        }
        #endregion

        #region 驱魔师 普攻蓄力控制
        public InputManager.PaladinAttack GetPaladinAttack()
        {
            SwitchClientFunctionTable paladinAttackCharge = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>((int)ClientSwitchType.PaladinAttackCharge);
            if (paladinAttackCharge == null || !paladinAttackCharge.Open)
                return InputManager.PaladinAttack.OPEN;
            //返回默认值
            if (PlayerLocalSetting.GetValue(STR_PALADINATTACK) == null)
                return paladinAttackCharge.ValueA == 0 ? InputManager.PaladinAttack.CLOSE : InputManager.PaladinAttack.OPEN;
            if((string)PlayerLocalSetting.GetValue(STR_PALADINATTACK) == "open")
                return InputManager.PaladinAttack.OPEN;
            return InputManager.PaladinAttack.CLOSE;
        }

        public void SetPaladinAttack(InputManager.PaladinAttack flag)
        {
            PlayerLocalSetting.SetValue(STR_PALADINATTACK, flag == InputManager.PaladinAttack.CLOSE ? "close" : "open");
            PlayerLocalSetting.SaveConfig();
        }
        #endregion

        public bool GetLiGuiValue(string key)
        {
            bool value = false;
            value = GetValue(key);

            //没有这个设置，也就是默认值。里鬼默认设置为true
            if (PlayerLocalSetting.GetValue(key) == null && key == STR_LIGUI)
            {
                value = SwitchFunctionUtility.IsOpen(25);
            }

            if (key == STR_LIGUI && !SwitchFunctionUtility.IsOpen(24))
                value = false;

             return value;
        }

        #region 滑动操作设置
        //获取滑动设置
        public InputManager.SlideSetting GetSlideMode(string key)
        {
            if (ReplayServer.GetInstance().IsReplay())
            {
                return InputManager.SlideSetting.NORMAL;
            }
            int switchTableId = 0;
            switch (key)
            {
                case "1204":
                    switchTableId = 17;
                    break;
                case "1007":
                    switchTableId = 18;
                    break;
                case "2010":
                    switchTableId = 19;
                    break;
                case "1512":
                    switchTableId = 20;
                    break;
                case "1216":
                    switchTableId = 49;
                    break;
                case "3600":
                    switchTableId = 36;
                    break;
                case "3608":
                    switchTableId = 37;
                    break;
                case "3713":
                    switchTableId = 54;
                    break;
                case "1218":
                    switchTableId = 56;
                    break;
                case "2611":
                    switchTableId = 43;
                    break;
				case "2301":
                    switchTableId = 66;
                    break;
				case "2302":
                    switchTableId = 68;
                    break;
            }
            if (switchTableId == 0)
            {
                return InputManager.SlideSetting.NORMAL;
            }

            SwitchClientFunctionTable data = TableManager.instance.GetTableItem<SwitchClientFunctionTable>(switchTableId);
            if (null == data)
            {
                return InputManager.SlideSetting.NORMAL;
            }

            bool flag = data.Open;
            if (!flag)
            {
                return InputManager.SlideSetting.NORMAL;
            }
            InputManager.SlideSetting mode = InputManager.SlideSetting.NORMAL;
            if (PlayerLocalSetting.GetValue(key) != null &&((string)PlayerLocalSetting.GetValue(key)) == "slide")
                mode = InputManager.SlideSetting.SLIDE;
            return mode;
        }

        //设置手雷滑动
        public void SetSlideMode(InputManager.SlideSetting mode, string key)
        {
            PlayerLocalSetting.SetValue(key, mode == InputManager.SlideSetting.NORMAL ? "normal" : "slide");
            PlayerLocalSetting.SaveConfig();
        }
        #endregion

        #region 炫纹设置

        public int GetChaserSetting(string key)
        {
            string realyKey = string.Format("{0}{1}", key, PlayerBaseData.GetInstance().RoleID);
            return GetValueInt(realyKey);
        }

        public void SetChaserSetting(string key, int value)
        {
            string realyKey = string.Format("{0}{1}", key, PlayerBaseData.GetInstance().RoleID);
            SetValue(realyKey,value);
        }
        #endregion
        
        #region Vip设置

        public bool GetVipSettingData(string key, string roleId)
        {
            string realyKey = string.Format("{0}{1}", key, roleId);
            return GetValue(realyKey);
        }

        public void SetVipSettingData(string key, bool value)
        {
            string realyKey = string.Format("{0}{1}", key, PlayerBaseData.GetInstance().RoleID);
            SetValue(realyKey,value);
        }
        #endregion

        #region 召唤兽设置

        /// <summary>
        /// 通用设置枚举枚举
        /// </summary>
        public enum SetCommonType
        {
            None = -1,
            Close = 0,
            Open = 1,
        }

        /// <summary>
        /// 获取通用设置数据
        /// </summary>
        public SetCommonType GetCommmonSet(string key)
        {
            string data = (string)PlayerLocalSetting.GetValue(key);
            if (string.IsNullOrEmpty(data))
            {
                if(GeGraphicSetting.instance.IsHighLevel() || GeGraphicSetting.instance.IsMiddleLevel())
                {
                    return SetCommonType.Open;
                }
                else
                {
                    return SetCommonType.Close;
                }
            }
            else if (data.Equals("open"))
            {
                return SetCommonType.Open;
            }
            else
            {
                return SetCommonType.Close;
            }
        }

        /// <summary>
        /// 设置通用数据
        /// </summary>
        public void SetCommomData(string key,string value)
        {
            PlayerLocalSetting.SetValue(key, value);
        }

        #endregion

        #region 通用方法 

        /// <summary>
        /// 获取存储的持久化数据
        /// </summary>
        /// <param name="key">该值在外部设置 并且保证值唯一</param>
        /// <returns></returns>
        public bool GetValue(string key)
        {
            bool value = false;
            if (PlayerLocalSetting.GetValue(key) != null &&
               ((string)PlayerLocalSetting.GetValue(key)) == "true")
                value = true;
            return value;
        }

        public int GetValueInt(string key)
        {
            int value = 0;
            if (PlayerLocalSetting.GetValue(key) != null)
                int.TryParse(PlayerLocalSetting.GetValue(key).ToString(),out value);
            return value;

        }

        /// <summary>
        /// 存储持久化数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key,bool value)
        {
            PlayerLocalSetting.SetValue(key, value ? "true":"false");
            PlayerLocalSetting.SaveConfig();
        }

        public void SetValue(string key, int value)
        {
            PlayerLocalSetting.SetValue(key, value);
            PlayerLocalSetting.SaveConfig();
        }
        #endregion
    }


    public class SettingFrame : ClientFrame
    {
        public enum TabType
        {
            ROLE_INFO,
            SYS_SET,
            BATTLE_CTRL,
            VIP,
            PUSH_SET,
            VOICE_CHAT,
            CDK,
            MOBILE_BIND,
            DEBUG,
            ACCOUNT_LOCK,
        }
        public const string KEY_DOUBLE_PRESS = "KEY_DOUBLE";
        public const string KEY_ATTACK_REPLACE = "KEY_ATTACK_REPLACE"; 
        private Toggle tabRoleInfo;
        private Toggle tabSysSet;
        private Toggle tabBattleCtrl;
        private Toggle vipTab;
        private Toggle tabPushSet;
        private Toggle tabVoiceChat;
        private Toggle tabCDK;
        private Toggle tabMobileBind;
		private Toggle tabDebug;
        private Toggle tabAccountLock;
        private Toggle tabSystemHelp;
        private Toggle tabBattleHelp;

        private Button mUploadCompress = null;
        

        private Button btnRoleChange;
        private Button btnAccChange;
        private GameObject goGlobalBtns;
        /*
        private GameObject contRoleInfo;
        private GameObject contSysSet;
        private GameObject contBattleCtrl;
        private GameObject contPushSet;
        private GameObject contVoiceChat;
        private GameObject contCDK;
        private GameObject contMobileBind;
        */
        private bool isTabRoleInfoOn;
        private bool isTabSysSetOn;
        private bool isTabBattleCtrlOn;
        private bool isTabVipCtrlOn;
        private bool isTabPushSetOn;
        private bool isTabVoiceChatOn;
        private bool isTabCDKOn;
        private bool isTabMobileBindOn;
		private bool isTabDebugOn;
        private bool isTabAccountLockOn;
        private bool isTabSystemHelpOn;
        private bool isTabBattleHelpOn;

        private RoleInfoSettings roleInfoSettings;
        //private MobileBindSettings mobileBindSettings;
        private SystemInfoSettings systemInfoSettings;
      //  private BattleRunCtrlSettings battleRunCtrlSettings;
        private VipSettings vipSettings;
        private VoiceChatSettings voiceChatSettings;
        private CDKRewardSettings cdkRewardSettings;
        private AccountLockSettings accountLockSettings;
        private SystemHelpSettings systemHelpSettings;
        private BattleHelpSettings battleHelpSettings;
   
        TabType eStartUpTabType = TabType.ROLE_INFO;

		//private HelpDebugSettings debugSettings;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SettingPanel/SettingPanel";
        }

        protected override void _bindExUI()
        {
            tabRoleInfo = mBind.GetCom<Toggle>("TabRoleInfo");
            if (tabRoleInfo)
            {
                tabRoleInfo.onValueChanged.RemoveListener(TabRoleInfoChanged);
                tabRoleInfo.onValueChanged.AddListener(TabRoleInfoChanged);
            }

            mUploadCompress = mBind.GetCom<Button>("UploadCompress");
            if (null != mUploadCompress)
            {
                mUploadCompress.onClick.AddListener(_onUploadCompressButtonClick);
            }

            tabSysSet = mBind.GetCom<Toggle>("TabSysSet");
            if (tabSysSet)
            {
                tabSysSet.onValueChanged.RemoveListener(TabSysSetChanged);
                tabSysSet.onValueChanged.AddListener(TabSysSetChanged);
            }
            tabBattleCtrl = mBind.GetCom<Toggle>("TabBattleCtrl");
            if (tabBattleCtrl)
            {
                tabBattleCtrl.onValueChanged.RemoveListener(TabBattleCtrlChanged);
                tabBattleCtrl.onValueChanged.AddListener(TabBattleCtrlChanged);
            }
            vipTab = mBind.GetCom<Toggle>("VipTab");
            if (vipTab)
            {
                vipTab.onValueChanged.RemoveListener(TabVipChanged);
                vipTab.onValueChanged.AddListener(TabVipChanged);
            }
            tabPushSet = mBind.GetCom<Toggle>("TabPushMsg");
            if (tabPushSet)
            {
                tabPushSet.onValueChanged.RemoveListener(TabPushSetChanged);
                tabPushSet.onValueChanged.AddListener(TabPushSetChanged);
            }
            tabVoiceChat = mBind.GetCom<Toggle>("TabVoiceChat");
            if (tabVoiceChat)
            {
                bool isVoiceEnabled = VoiceSDK.SDKVoiceManager.GetInstance().OpenChatVoice || VoiceSDK.SDKVoiceManager.GetInstance().OpenTalkRealVocie;
                tabVoiceChat.gameObject.CustomActive(isVoiceEnabled);

                tabVoiceChat.onValueChanged.RemoveListener(TabVoiceChatChanged);
                tabVoiceChat.onValueChanged.AddListener(TabVoiceChatChanged);
            }
            tabCDK = mBind.GetCom<Toggle>("TabCDK");
            if (tabCDK)
            {
                tabCDK.onValueChanged.RemoveListener(TabCDKChanged);
                tabCDK.onValueChanged.AddListener(TabCDKChanged);
            }
            tabMobileBind = mBind.GetCom<Toggle>("TabBindMobile");
            if (tabMobileBind)
            {
                tabMobileBind.onValueChanged.RemoveListener(TabMobileBindChanged);
                tabMobileBind.onValueChanged.AddListener(TabMobileBindChanged);
            }
            tabAccountLock = mBind.GetCom<Toggle>("TabAccountLock");
            if (tabAccountLock)
            {
                tabAccountLock.onValueChanged.RemoveListener(TabAccountLockChanged);
                tabAccountLock.onValueChanged.AddListener(TabAccountLockChanged);
            }
            tabSystemHelp = mBind.GetCom<Toggle>("TabSystemHelp");
            if (tabSystemHelp)
            {
                tabSystemHelp.onValueChanged.RemoveListener(TabSystemHelpChanged);
                tabSystemHelp.onValueChanged.AddListener(TabSystemHelpChanged);
            }
            tabBattleHelp = mBind.GetCom<Toggle>("TabBattleHelp");
            if (tabBattleHelp)
            {
                tabBattleHelp.onValueChanged.RemoveListener(TabBattleHelpChanged);
                tabBattleHelp.onValueChanged.AddListener(TabBattleHelpChanged);
            }

            tabDebug = mBind.GetCom<Toggle>("TabDebug");
			if (tabDebug)
			{
				tabDebug.onValueChanged.RemoveListener(TabDebugChanged);
				tabDebug.onValueChanged.AddListener(TabDebugChanged);
			}
            btnRoleChange = mBind.GetCom<Button>("btnRoleChange");
            btnAccChange = mBind.GetCom<Button>("btnAccChange");
            goGlobalBtns = mBind.GetGameObject("goGlobalBtns");


			#if !DEBUG_REPORT_ROOT
			if (tabDebug != null)
				tabDebug.gameObject.CustomActive(false);
			#endif

            /*
            contRoleInfo = mBind.GetGameObject("ContRoleInfo");
            contSysSet = mBind.GetGameObject("ContSysSet");
            contBattleCtrl = mBind.GetGameObject("ContBattleCtrl");
            contPushSet = mBind.GetGameObject("ContPushSet");
            contVoiceChat = mBind.GetGameObject("ContVoiceChat");
            contCDK = mBind.GetGameObject("ContCDK");
            contMobileBind = mBind.GetGameObject("ContMobile");
             * */
        }

        protected override void _unbindExUI()
        {
            if (tabRoleInfo)
            {
                tabRoleInfo.onValueChanged.RemoveListener(TabRoleInfoChanged);
            }
            tabRoleInfo =null;
            if (tabSysSet)
            {
                tabSysSet.onValueChanged.RemoveListener(TabSysSetChanged);
            }
            tabSysSet =null;
            if (tabBattleCtrl)
            {
                tabBattleCtrl.onValueChanged.RemoveListener(TabBattleCtrlChanged);
            }
            tabBattleCtrl =null;
            if (vipTab)
            {
                vipTab.onValueChanged.RemoveListener(TabVipChanged);
            }
            vipTab = null;
            if (tabPushSet)
            {
                tabPushSet.onValueChanged.RemoveListener(TabPushSetChanged);
            }
            tabPushSet =null;
            if (tabVoiceChat)
            {
                tabVoiceChat.onValueChanged.RemoveListener(TabVoiceChatChanged);
            }
            tabVoiceChat  =null;
            if (tabCDK)
            {
                tabCDK.onValueChanged.RemoveListener(TabCDKChanged);
            }
            tabCDK  =null;
            if (tabMobileBind)
            {
                tabMobileBind.onValueChanged.RemoveListener(TabMobileBindChanged);
            }
            tabMobileBind  =null;

            if (tabAccountLock)
            {
                tabAccountLock.onValueChanged.RemoveListener(TabAccountLockChanged);
            }
            tabAccountLock = null;
			if (tabDebug)
			{
				tabDebug.onValueChanged.RemoveListener(TabDebugChanged);
			}

            if (tabSystemHelp)
            {
                tabSystemHelp.onValueChanged.RemoveListener(TabSystemHelpChanged);
            }
            tabSystemHelp = null;

            if (tabBattleHelp)
            {
                tabBattleHelp.onValueChanged.RemoveListener(TabBattleHelpChanged);
            }
            tabBattleHelp = null;

            if (null != mUploadCompress)
            {
                mUploadCompress.onClick.RemoveListener(_onUploadCompressButtonClick);
            }
            mUploadCompress = null;

			tabDebug = null;
            btnRoleChange = null;
            btnAccChange = null;
            goGlobalBtns = null;

            /*
            contRoleInfo = null;
            contSysSet = null;
            contBattleCtrl = null;
            contPushSet = null;
            contVoiceChat = null;
            contCDK = null;
            contMobileBind = null;
             * */
        }

        private void _onUploadCompressButtonClick()
        {
            SystemNotifyManager.BaseMsgBoxOkCancel("是否上传数据", ()=>{
                    ClientSystemManager.instance.OpenFrame<UploadingCompressFrame>();
            }, null, "确定", "取消");
        }

        #region UI Class

        void InitViewSettings()
        {
            roleInfoSettings = new RoleInfoSettings(frame, this);
            //mobileBindSettings = new MobileBindSettings(frame, this);
            systemInfoSettings = new SystemInfoSettings(frame,this);
           // battleRunCtrlSettings = new BattleRunCtrlSettings(frame,this);
            vipSettings = new VipSettings(frame, this);
            voiceChatSettings = new VoiceChatSettings(frame,this);
            cdkRewardSettings = new CDKRewardSettings(frame,this);
            accountLockSettings = new AccountLockSettings(frame, this);
            systemHelpSettings = new SystemHelpSettings(frame, this);
            battleHelpSettings = new BattleHelpSettings(frame, this);

            //debugSettings = new HelpDebugSettings(frame, this);

            //if (tabMobileBind)
            //    tabMobileBind.gameObject.CustomActive(MobileBindManager.GetInstance().IsMobileBindOpen());

            InitTabsShow();
        }
        void UpDateViewSettings()
        {
            //if (mobileBindSettings != null)
            //    mobileBindSettings.Update();
        }

        void UnInitViewSettings()
        {
            if (roleInfoSettings != null)
                roleInfoSettings.CloseFrame();
            roleInfoSettings = null;
            //if (mobileBindSettings != null)
            //    mobileBindSettings.HideIn();
            //mobileBindSettings = null;
            if (systemInfoSettings != null)
                systemInfoSettings.CloseFrame();
            systemInfoSettings = null;
            //if (battleRunCtrlSettings != null)
            //{
            //    battleRunCtrlSettings.ReleaseBattleVideos();
            //    battleRunCtrlSettings.HideIn();
            //}
            //battleRunCtrlSettings = null;
            if (vipSettings != null)
            {
                vipSettings.CloseFrame();
            }
            vipSettings = null;
            if (voiceChatSettings != null)
                voiceChatSettings.CloseFrame();
            voiceChatSettings = null;
            if (cdkRewardSettings != null)
                cdkRewardSettings.CloseFrame();
            cdkRewardSettings = null;
            if (accountLockSettings != null)
            {
                accountLockSettings.CloseFrame();
            }
            accountLockSettings = null;
        }

        #endregion

        #region Tabs Callback

        void InitTabsShow()
        {
            isTabRoleInfoOn = false;
            isTabSysSetOn = false;
            isTabBattleCtrlOn = false;
            isTabVipCtrlOn = false;
            isTabPushSetOn = false;
            isTabVoiceChatOn = false;
            isTabCDKOn = false;
            isTabMobileBindOn = false;
			isTabDebugOn = false;
            isTabVipCtrlOn = false;
            isTabAccountLockOn = false;
            isTabSystemHelpOn = false;
            isTabBattleHelpOn = false;
            Toggle toggle = null;
            SettingsBindUI bindUI = null;

            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_SECURITY_LOCK))
            {
                tabAccountLock.CustomActive(false);

                if (eStartUpTabType == TabType.ACCOUNT_LOCK)
                {
                    eStartUpTabType = TabType.ROLE_INFO;
                }
            }

            switch (eStartUpTabType)
            {
                case TabType.ROLE_INFO:
                    isTabRoleInfoOn = true;
                    toggle = tabRoleInfo;
                    bindUI = roleInfoSettings;              
                    break;
                case TabType.SYS_SET:
                    isTabSysSetOn = true;
                    toggle = tabSysSet;
                    bindUI = systemInfoSettings;
                    break;
                //case TabType.BATTLE_CTRL:
                //    isTabBattleCtrlOn = true;
                //    toggle = tabBattleCtrl;
                //    bindUI = battleRunCtrlSettings;
                //    break;
                case TabType.VIP:
                    isTabVipCtrlOn = true;
                    toggle = vipTab;
                    bindUI = vipSettings;
                    break;
                case TabType.PUSH_SET:
                    isTabPushSetOn = true;
                    toggle = tabPushSet;
                    bindUI = null;
                    break;
                case TabType.VOICE_CHAT:
                    isTabVoiceChatOn = true;
                    toggle = tabVoiceChat;
                    bindUI = voiceChatSettings;
                    break;
                case TabType.CDK:
                    isTabCDKOn = true;
                    toggle = tabCDK;
                    bindUI = cdkRewardSettings;
                    break;
                case TabType.MOBILE_BIND:
                    isTabMobileBindOn = true;
                    toggle = tabMobileBind;
                    bindUI = null;
                    break;
                case TabType.DEBUG:
                    isTabDebugOn = true;
                    toggle = tabDebug;
                    bindUI = null;
                    break;
                case TabType.ACCOUNT_LOCK:
                    isTabAccountLockOn = true;
                    toggle = tabAccountLock;
                    bindUI = accountLockSettings;
                    break;
            }

            SetSelectedTabActive(tabRoleInfo, isTabRoleInfoOn);
            SetSelectedTabActive(tabSysSet, isTabSysSetOn);
            SetSelectedTabActive(tabBattleCtrl, isTabBattleCtrlOn);
            SetSelectedTabActive(tabPushSet, isTabPushSetOn);
            SetSelectedTabActive(tabVoiceChat, isTabVoiceChatOn);
            SetSelectedTabActive(tabCDK, isTabCDKOn);
            SetSelectedTabActive(tabMobileBind, isTabMobileBindOn);
			SetSelectedTabActive(tabDebug, isTabDebugOn);
            SetSelectedTabActive(vipTab, isTabVipCtrlOn);
            SetSelectedTabActive(tabAccountLock, isTabAccountLockOn);
            SetSelectedTabActive(tabSystemHelp, isTabSystemHelpOn);
            SetSelectedTabActive(tabBattleHelp, isTabBattleHelpOn);

            //             if (tabRoleInfo)
            //                 tabRoleInfo.isOn = isTabRoleInfoOn;
            if (toggle != null)
            {
                toggle.isOn = true;
            }
            
            if (bindUI != null)
            {
                bindUI.ShowOut();
            }

            if(goGlobalBtns != null)
            {
                if (isTabAccountLockOn)
                {
                    goGlobalBtns.CustomActive(false);
                }
                else
                {
                    goGlobalBtns.CustomActive(true);
                }
            }
          
            vipTab.CustomActive(PlayerBaseData.GetInstance().VipLevel > 0 && CheckVipData());
        }

        protected bool CheckVipData()
        {
            if (TableManager.instance.GetTableItem<SwitchClientFunctionTable>(SettingManager.vipDrugTableId).Open)
                return true;
            if (TableManager.instance.GetTableItem<SwitchClientFunctionTable>(SettingManager.vipRebornTableId).Open)
                return true;
            if (TableManager.instance.GetTableItem<SwitchClientFunctionTable>(SettingManager.vipUseCrystalTableId).Open)
                return true;
            if (TableManager.instance.GetTableItem<SwitchClientFunctionTable>(SettingManager.STR_VIPPREFER).Open)
                return true;
            return false;
        }

        void TabRoleInfoChanged(bool isOn)
        {
            if (isTabRoleInfoOn == isOn)
                return;
            isTabRoleInfoOn = isOn;

            SetSelectedTabActive(tabRoleInfo, isOn);
            if (roleInfoSettings != null)
            {
                if (isOn)
                    roleInfoSettings.ShowOut();
                else
                    roleInfoSettings.HideIn();
            }
        }

        void TabSysSetChanged(bool isOn)
        {
            if (isTabSysSetOn == isOn)
                return;
            isTabSysSetOn = isOn;

            SetSelectedTabActive(tabSysSet, isOn);
            if (systemInfoSettings != null)
            {
                if (isOn)
                    systemInfoSettings.ShowOut();
                else
                    systemInfoSettings.HideIn();
            }
        }

        void TabBattleCtrlChanged(bool isOn)
        {
            if (isTabBattleCtrlOn == isOn)
                return;
            isTabBattleCtrlOn = isOn;

            SetSelectedTabActive(tabBattleCtrl, isOn);
            //if (battleRunCtrlSettings != null)
            //{
            //    if (isOn)
            //        battleRunCtrlSettings.ShowOut();
            //    else
            //        battleRunCtrlSettings.HideIn();
            //}
        }

        void TabVipChanged(bool isOn)
        {
            if (isTabVipCtrlOn == isOn)
                return;
            isTabVipCtrlOn = isOn;

            SetSelectedTabActive(vipTab, isOn);
            if (vipSettings != null)
            {
                if (isOn)
                    vipSettings.ShowOut();
                else
                    vipSettings.HideIn();
            }
        }

        void TabPushSetChanged(bool isOn)
        {
            if (isTabPushSetOn == isOn)
                return;
            isTabPushSetOn = isOn;

            SetSelectedTabActive(tabPushSet, isOn);
            //if (contPushSet)
            //    contPushSet.CustomActive(isOn);
        }

        void TabVoiceChatChanged(bool isOn)
        {
            if (isTabVoiceChatOn == isOn)
                return;
            isTabVoiceChatOn = isOn;

            SetSelectedTabActive(tabVoiceChat, isOn);
            if (voiceChatSettings != null)
            {
                if (isOn)
                    voiceChatSettings.ShowOut();
                else
                    voiceChatSettings.HideIn();
            }
        }

        void TabCDKChanged(bool isOn)
        {
            if (isTabCDKOn == isOn)
                return;
            isTabCDKOn = isOn;

            SetSelectedTabActive(tabCDK, isOn);
            if (cdkRewardSettings != null)
            {
                if (isOn)
                    cdkRewardSettings.ShowOut();
                else
                    cdkRewardSettings.HideIn();
            }
        }

        void TabAccountLockChanged(bool isOn)
        {
            if (isTabAccountLockOn == isOn)
                return;
            isTabAccountLockOn = isOn;
            if(isOn)
            {
                GameStatisticManager.GetInstance().DoStartUIButton("SecurityLock");
            }
            SetSelectedTabActive(tabAccountLock, isOn);
            if (accountLockSettings != null)
            {
                if (isOn)
                {                    
                    accountLockSettings.ShowOut();
                }
                else
                    accountLockSettings.HideIn();
            }
        }
        void TabMobileBindChanged(bool isOn)
        {
            if (isTabMobileBindOn == isOn)
                return;
            isTabMobileBindOn = isOn;

            SetSelectedTabActive(tabMobileBind, isOn);
            //if (mobileBindSettings != null)
            //{
            //    if (isOn)
            //    {
            //        mobileBindSettings.ShowOut();
            //    }
            //    else
            //    {
            //        mobileBindSettings.HideIn();

            //        if (MobileBindManager.GetInstance().IsMobileBindOpen() == false)
            //        {
            //            if (tabMobileBind)
            //            {
            //                tabMobileBind.gameObject.CustomActive(isOn);
            //            }
            //        }
            //    }
            //}
        }

		void TabDebugChanged(bool isOn)
		{
			if (isTabDebugOn == isOn)
				return;

			isTabDebugOn = isOn;
			SetSelectedTabActive(tabDebug, isOn);

            //if (debugSettings != null)
            //{
            //    if (isOn)
            //        debugSettings.ShowOut();
            //    else
            //        debugSettings.HideIn();
            //}
		}

        void TabSystemHelpChanged(bool isOn)
        {
            if (isTabSystemHelpOn == isOn)
                return;

            isTabSystemHelpOn = isOn;
            SetSelectedTabActive(tabSystemHelp, isOn);

            if (systemHelpSettings != null)
            {
                if (isOn)
                    systemHelpSettings.ShowOut();
                else
                    systemHelpSettings.HideIn();
            }
        }

        void TabBattleHelpChanged(bool isOn)
        {
            if (isTabBattleHelpOn == isOn)
                return;

            isTabBattleHelpOn = isOn;
            SetSelectedTabActive(tabBattleHelp, isOn);

            if (battleHelpSettings != null)
            {
                if (isOn)
                    battleHelpSettings.ShowOut();
                else
                    battleHelpSettings.HideIn();
            }
        }

        void SetSelectedTabActive(Toggle selectTab, bool isSelected)
        {
            if (selectTab)
            {
                if (selectTab.graphic)
                {
                    selectTab.graphic.gameObject.CustomActive(isSelected);
                }
            }

            if (goGlobalBtns != null)
            {
                if (isTabAccountLockOn)
                {
                    goGlobalBtns.CustomActive(false);
                }
                else
                {
                    goGlobalBtns.CustomActive(true);
                }
            }
        }

        #endregion

        //private GameObject 

        protected override void _OnOpenFrame()
        {
            eStartUpTabType = TabType.ROLE_INFO;
            if(userData != null)
            {
                eStartUpTabType = (TabType)userData;
            }
            InitViewSettings();

            GameObject changeRoleButton = Utility.FindGameObject(frame, "Panel/GlobalBtns/RoleChangeBtn");
            if (null != changeRoleButton)
            {
                //changeRoleButton.SetActive(false);
            }
#if APPLE_STORE
            //add by mjx for ios appstore
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.GAME_CDK))
            {
                GameObject cdkButton = Utility.FindGameObject(frame, "Panel/Tabs/Viewport/Content/CDKTab");
                if (null != cdkButton)
                {
                    cdkButton.SetActive(false);
                }
            }
#endif

            if (null != mUploadCompress)
            {
                bool isShow = false;
#if MG_TEST
                isShow = true;
#endif
                mUploadCompress.gameObject.CustomActive(isShow);
            }
        }

        protected override void _OnCloseFrame()
        {
            eStartUpTabType = TabType.ROLE_INFO;
            UnInitViewSettings();
        }

        public override bool IsNeedUpdate()
        {
            if (tabMobileBind)
                return tabMobileBind.isOn;
            else
                return false;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            UpDateViewSettings();
        }


        public static void LoadDoublePressConfig()
        {
            if (PlayerLocalSetting.GetValue(KEY_DOUBLE_PRESS) != null)
                Global.Settings.hasDoubleRun = (bool)PlayerLocalSetting.GetValue(KEY_DOUBLE_PRESS);
        }

       

        [UIEventHandle("Panel/BtnClose")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Panel/GlobalBtns/TakePhotoMode")]
        void OnTakeFhotoMode()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            bool bCanUse = false;
            if (scenedata.SceneType == CitySceneTable.eSceneType.NORMAL
            || scenedata.SceneType == CitySceneTable.eSceneType.SINGLE)
            {
                bCanUse = true;
            }

            if(!bCanUse)
            {
                SystemNotifyManager.SystemNotify(10042);
                return;
            }

            frameMgr.CloseFrame(this);
            ClientSystemManager.GetInstance().OpenFrame<TakePhotoModeFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("Panel/GlobalBtns/RoleChangeBtn")]
        void OnChangeRole()
        {
            //ClientSystemManager.instance._QuitToSelectRoleImpl();
            //SystemNotifyManager.SysNotifyTextAnimation(SysNotifyMsgText.CHANGE_ROLE_TIP);
            //ClientSystemManager.instance.OpenFrame<ActivityLimitTimeFrame>(FrameLayer.Middle);

            RoleSwitchReq req = new RoleSwitchReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            ClientSystemLogin.mSwitchRole = true;

            //脕脛脤矛脫茂脪么碌脟鲁枚
            VoiceSDK.SDKVoiceManager.GetInstance().LeaveVoiceSDK();

            //ClientSystemLoginUtility.StartLoginAfterVerify();

            //             if (!ClientSystemLogin.mLoginAuto)
            //             {
            //                 SystemSwitchEventManager.GetInstance().RegisterEvent(SystemEventType.SYSETM_EVENT_SELECT_ROLE, SystemSwitchEventFunction.OnEventSelectRole);
            //                 SystemSwitchEventManager.GetInstance().RegisterEvent(SystemEventType.SYSTEM_EVENT_ON_SWITCH_FAILED, SystemSwitchEventFunction.OnEventSwitchFailed);
            //                 ClientSystemLogin.mLoginAuto = true;
            //                 ClientSystemManager.GetInstance()._QuitToLoginImpl();
            //             }
        }

        [UIEventHandle("Panel/GlobalBtns/AccChangeBtn")]
        void OnLoginOut()
        {
            GateLeaveGameReq req = new GateLeaveGameReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            NetManager.Instance().Update();

            //锟斤拷陆
            SDKInterface.Instance.UpdateRoleInfo(
                4, ClientApplication.adminServer.id, ClientApplication.adminServer.name,
                PlayerBaseData.GetInstance().RoleID.ToString(),
                PlayerBaseData.GetInstance().Name,
                PlayerBaseData.GetInstance().JobTableID, PlayerBaseData.GetInstance().Level, PlayerBaseData.GetInstance().VipLevel,
                (int)PlayerBaseData.GetInstance().Ticket);

            ClientSystemManager.instance._QuitToLoginImpl();

            
			
			SDKInterface.Instance.NeedLogoutSDK();
        }
    }


    public class SettingsBindUI
    {
        protected ComCommonBind mBind;
        protected string comBindPath;
        protected ClientFrame currFrame;

        private bool isBinded = false;
        private bool isLoadPrefab = false;
        private GameObject mRoot;

        public SettingsBindUI(GameObject root,ClientFrame frame)
        {
            this.currFrame = frame;
            mRoot = root;
            comBindPath = GetCurrGameObjectPath();
            if(string.IsNullOrEmpty(comBindPath))
            {
                Logger.LogError("Please set your bindUI's gameObject path!!!!!");
            }
            if (root == null)
            {
                Logger.LogError("Your bindUI's root is null !!!!!");
            }
            if (frame == null)
            {
                Logger.LogError("Your bindUI's root frame is null !!!!!");
            }
            if (mBind != null)
            {
                return;
            }
            if (root != null && frame != null)
            {
                string frameName = frame.GetName().Replace("(Clone)", "");
                string[] formatPath = System.Text.RegularExpressions.Regex.Split(comBindPath, frameName);
                if (formatPath == null)
                {
                    return;
                }
                if (formatPath.Length < 2)
                {
                    return;
                }
                if (formatPath[1].StartsWith("/"))
                {
                    comBindPath = formatPath[1].TrimStart('/');

                }
            }
        }

        public void ShowOut()
        {
            if (!isLoadPrefab)
            {
                LoadPrefab();
            }

            if (isBinded == false)
                InitBind();
            if (mBind)
                mBind.gameObject.CustomActive(true);
            OnShowOut();
        }

        public void HideIn()
        {
            OnHideIn();
            UnInitBind();
            isBinded = false;
            if (mBind)
                mBind.gameObject.CustomActive(false);
        }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void LoadPrefab()
        {
            if (isBinded == false)
            {
                if (mRoot != null)
                {
                    UIPrefabWrapper uiPrefabWrapper = Utility.FindComponent<UIPrefabWrapper>(mRoot, comBindPath, false);
                    if (uiPrefabWrapper != null)
                    {
                        var rootGameObject = Utility.FindGameObject(mRoot, comBindPath);
                        rootGameObject = uiPrefabWrapper.gameObject;
                        GameObject prefab = uiPrefabWrapper.LoadUIPrefab();
                        if (prefab != null)
                        {
                            prefab.name = rootGameObject.name;
                            prefab.transform.SetParent(rootGameObject.transform.parent, false);
                            GameObject.Destroy(rootGameObject);
                        }

                        mBind = prefab.GetComponent<ComCommonBind>();
                        isLoadPrefab = true;
                    }
                }
            }

        }

        public void CloseFrame()
        {
            HideIn();
            Close();
        }

        protected virtual void Close()
        {

        }

        protected virtual void InitBind() 
        { 
        }
        protected virtual void UnInitBind()
        {
        }
        protected virtual void OnShowOut()
        {
        }
        protected virtual void OnHideIn()
        {
        }

        protected virtual void OnUpdate()
        { 
        }

        protected virtual string GetCurrGameObjectPath()
        {
            return "";
        }
    }
}
