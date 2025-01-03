using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameClient;
using Protocol;
using Network;
using System.Collections.Generic;
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    public class SkillBattleSettingFrame : ClientFrame
    {
        private Toggle dragRun;
        private Toggle doubleRun;
        private Image dragCheckImg;
        private Image doubleCheckImg;
        private MediaPlayerCtrl dragRunVideo;
        private MediaPlayerCtrl doubleRunVideo;

        private GeUISwitchButton mJoystickMode = null;
        private GeUISwitchButton mJoystickDir = null;
        private GameObject JoystickDir = null;

        private GeUISwitchButton mRunAttackMode = null;
        private GeUISwitchButton mShockMode = null;

        private RectTransform mSlideTitle = null;
        private RectTransform mSlideSetting = null;

        private RectTransform mPaladinAttack = null;
        private GeUISwitchButton mPaladinAttackMode = null;

        private ComUIListScript mSkillSlideUIList = null;

        private Button mBtnInputSetting = null; 

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Skill/BattleSettingFrame";
        }

        protected override void _bindExUI()
        {
            dragRun = mBind.GetCom<Toggle>("DragRun");
            dragRun.onValueChanged.AddListener(OnDragRunChange);
            doubleRun = mBind.GetCom<Toggle>("DoubleRun");
            doubleRun.onValueChanged.AddListener(OnDoubleRunChange);

            dragCheckImg = mBind.GetCom<Image>("DragBgCheckImg");
            doubleCheckImg = mBind.GetCom<Image>("DoubleBgCheckImh");
            dragRunVideo = mBind.GetCom<MediaPlayerCtrl>("DragRunVideo");
            doubleRunVideo = mBind.GetCom<MediaPlayerCtrl>("DoubleRunVideo");

            mJoystickMode = mBind.GetCom<GeUISwitchButton>("joystickMode");
            mJoystickMode.onValueChanged.AddListener(_onJoystickMode1ToggleValueChange);

            mJoystickDir = mBind.GetCom<GeUISwitchButton>("mJoystickDir");
            mJoystickDir.onValueChanged.AddListener(_onJoystickMoreDirToggleValueChange);
            
            JoystickDir = mBind.GetGameObject("Joystick8Dir");

            mRunAttackMode = mBind.GetCom<GeUISwitchButton>("runAttackMode");
            mRunAttackMode.onValueChanged.AddListener(_onRunAttackMode1ToggleValueChange);
            
            mShockMode = mBind.GetCom<GeUISwitchButton>("ShockMode");
            mShockMode.onValueChanged.AddListener(_onCameraShockMode1ToggleValueChange);

            mSlideTitle = mBind.GetCom<RectTransform>("SlideTitle");
            mSlideSetting = mBind.GetCom<RectTransform>("SlideSetting");
            
            mPaladinAttack = mBind.GetCom<RectTransform>("PaladinAttack");
            mPaladinAttackMode = mBind.GetCom<GeUISwitchButton>("PaladinAttackMode");
            if (null != mPaladinAttackMode)
            {
                mPaladinAttackMode.onValueChanged.AddListener(_onPaladinAttackCloseToggleValueChange);
            }

            mSkillSlideUIList = mBind.GetCom<ComUIListScript>("SkillSlideUIList");

            mBtnInputSetting = mBind.GetCom<Button>("BtInputSetting");
            if (mBtnInputSetting != null)
            {
                mBtnInputSetting.onClick.AddListener(_onBtnInputSettingClicked);
            }
        }

        protected override void _unbindExUI()
        {
            if (dragRun != null)
                dragRun.onValueChanged.RemoveListener(OnDragRunChange);
            dragRun = null;
            if (doubleRun != null)
                doubleRun.onValueChanged.RemoveListener(OnDoubleRunChange);
            doubleRun = null;

            dragCheckImg = null;
            doubleCheckImg = null;
            dragRunVideo = null;
            doubleRunVideo = null;

            if (mJoystickMode != null)
            {
                mJoystickMode.onValueChanged.RemoveListener(_onJoystickMode1ToggleValueChange);
                mJoystickMode = null;
            }

            if (mRunAttackMode != null)
            {
                mRunAttackMode.onValueChanged.RemoveListener(_onRunAttackMode1ToggleValueChange);
                mRunAttackMode = null;
            }
            
            if (mShockMode != null)
            {
                mShockMode.onValueChanged.RemoveListener(_onCameraShockMode1ToggleValueChange);
                mShockMode = null;
            }
            
            mSlideTitle = null;

            mSlideSetting = null;
            
            mPaladinAttack = null;
            if (null != mPaladinAttackMode)
            {
                mPaladinAttackMode.onValueChanged.RemoveListener(_onPaladinAttackCloseToggleValueChange);
            }
            mPaladinAttackMode = null;

            mSkillSlideUIList.UnInitialize();
            mSkillSlideUIList = null;

            if (null != mBtnInputSetting)
            {
                mBtnInputSetting.onClick.RemoveListener(_onBtnInputSettingClicked);
            }
            mBtnInputSetting = null;
        }

        private void _onJoystickMode1ToggleValueChange(bool isOn)
        {
            /* put your code in here */
            if (isOn)
            {
                SettingManager.GetInstance().SetJoystickMode(InputManager.JoystickMode.STATIC);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.STATIC);
            }
            else
            {
                SettingManager.GetInstance().SetJoystickMode(InputManager.JoystickMode.DYNAMIC);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.DYNAMIC);
            }
        }

        private void _onJoystickMoreDirToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                SettingManager.GetInstance().SetJoystickDir(InputManager.JoystickDir.EIGHT_DIR);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.DYNAMIC);
            }
            else
            {
                SettingManager.GetInstance().SetJoystickDir(InputManager.JoystickDir.MORE_DIR);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.DYNAMIC);
            }
        }
        

        private void _onRunAttackMode1ToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                SettingManager.GetInstance().SetRunAttackMode(InputManager.RunAttackMode.QTE);
            }
            else
            {
                SettingManager.GetInstance().SetRunAttackMode(InputManager.RunAttackMode.NORMAL);
            }
        }
        
        private void _onCameraShockMode1ToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                SettingManager.GetInstance().SetCameraShockMode(InputManager.CameraShockMode.CLOSE);
            }
            else
            {
                SettingManager.GetInstance().SetCameraShockMode(InputManager.CameraShockMode.OPEN);
            }
        }
        
        private void _onSummonDisplayCloseToggleValueChange(bool changed)
        {
        }
        private void _onSummonDisplayOpenToggleValueChange(bool changed)
        {
        }

        private void _onPaladinAttackCloseToggleValueChange(bool isOn)
        {
            if (isOn)
                SettingManager.GetInstance().SetPaladinAttack(InputManager.PaladinAttack.OPEN);
            else
                SettingManager.GetInstance().SetPaladinAttack(InputManager.PaladinAttack.CLOSE);
        }

        private void _onSkillSlideOpenToggleValueChange(bool isOn, string skillID)
        {
            if (isOn)
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.SLIDE, skillID);
            else
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.NORMAL, skillID);
        }

        private void _onBtnInputSettingClicked()
        {
            //ClientSystemManager.GetInstance().OpenFrame<BattleInputFrame>();
            BattleMain.OpenBattle(BattleType.InputSetting, eDungeonMode.LocalFrame, 0, "1000");
            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
        }

        protected void InitJoystickSelect()
        {
            mJoystickMode.states = false;
            if (SettingManager.GetInstance().GetJoystickMode() == InputManager.JoystickMode.STATIC)
            {
                mJoystickMode.states = true;
            }
        }

        protected void InitJoystickDir()
        {
            JoystickDir.CustomActive(SwitchFunctionUtility.IsOpen(29));
            mJoystickDir.states = false;
            if (SettingManager.GetInstance().GetJoystickDir() == InputManager.JoystickDir.EIGHT_DIR)
            {
                mJoystickDir.states = true;
            }
        }

        protected void InitRunAttackSelect()
        {
            mRunAttackMode.states = false;
            if (SettingManager.GetInstance().GetRunAttackMode() == InputManager.RunAttackMode.QTE)
            {
                mRunAttackMode.states = true;
            }
        }

        protected void InitCameraShockSelect()
        {
            mShockMode.states = false;
            if (SettingManager.GetInstance().GetCameraShockMode() == InputManager.CameraShockMode.CLOSE)
            {
                mShockMode.states = true;
            }
        }
        //初始化驱魔普攻设置
        protected void InitPaladinAttack()
        {
            SwitchClientFunctionTable paladinAttackCharge = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>((int)ClientSwitchType.PaladinAttackCharge);
            bool flag = paladinAttackCharge != null && paladinAttackCharge.Open;
            mPaladinAttack.gameObject.CustomActive(PlayerBaseData.GetInstance().JobTableID == 61 && flag);
            mPaladinAttackMode.states = false;
            if (SettingManager.GetInstance().GetPaladinAttack() == InputManager.PaladinAttack.OPEN)
            {
                mPaladinAttackMode.states = true;
            }
        }

        private Dictionary<int, List<int>> mTableSkillIdDic = new Dictionary<int, List<int>> //界面右侧添加功能需注意此Dic
        {
            { 17,new List<int> { 1204,2507} },//手雷
            { 18,new List<int> { 1007,2508} },//格林机枪
            { 19,new List<int> { 2010} },//天击
            { 20,new List<int> { 1512,1716} },//崩山击
            { 36,new List<int> { 3600} },//破魔符
            { 37,new List<int> { 3608} },//星落打
            { 54,new List<int> { 3713} },//圣光球
            { 48,new List<int> { 1216,2612 } },//加农炮
            { 56,new List<int> { 1218} },//FM榴弹男
            { 43,new List<int> { 2611} },//FM榴弹女
            { 24,new List<int> { 1901} },//里鬼设置
            { 49,new List<int> { 1910} },//背身回击设置
            { 57,new List<int> { 1107,2527} },//自动反击设置
			{ 66,new List<int> { 2301} },//圆舞棍设置
            { 68,new List<int> { 2302} },//炫纹设置
        };

        private Dictionary<int, string> mIdJsonKeyDic = new Dictionary<int, string> //非滑动本地可持久化key存放在此
        {
            { 24,SettingManager.STR_LIGUI },
            { 49,SettingManager.STR_BACKHIT },
            { 57,SettingManager.STR_AUTOHIT },
            { 68,SettingManager.STR_CHASER_SWITCH },
        };

        private List<SwitchClientFunctionTable> mSwitchClientFunctionTableList = new List<SwitchClientFunctionTable>();
        //初始化滑动设置
        protected void InitSlideSetting()
        {
            int roleId = PlayerBaseData.GetInstance().JobTableID;

            mSwitchClientFunctionTableList.Clear();
            _InitSkillSlideData();
            _InitSkillSlideUIListBind();
            mSkillSlideUIList.SetElementAmount(mSwitchClientFunctionTableList.Count);
        }

        protected void _InitSkillSlideUIListBind()
        {
            mSkillSlideUIList.Initialize();

            mSkillSlideUIList.onItemVisiable = (item) => 
            {
                if(item != null && item.m_index >= 0)
                {
                    _UpdateCharacterSelectUIListBind(item);
                }
            };

            mSkillSlideUIList.OnItemRecycle = (item) => 
            {
                if(item == null)
                {
                    return;
                }
                var tempBind = item.GetComponent<ComCommonBind>();
                if(tempBind != null)
                {
                    var switchBtn = tempBind.GetCom<GeUISwitchButton>("SwitchBtn");
                    if (switchBtn != null)
                    {
                        switchBtn.onValueChanged.RemoveAllListeners();
                    }
                }
            };
        }

        void _UpdateCharacterSelectUIListBind(ComUIListElementScript item)
        {
            if(item.m_index <0 || item.m_index >= mSwitchClientFunctionTableList.Count)
            {
                return;
            }
            var tempBind = item.GetComponent<ComCommonBind>();
            if(tempBind == null)
            {
                return;
            }
            
            var switchBtn = tempBind.GetCom<GeUISwitchButton>("SwitchBtn");
            Text mDscText = tempBind.GetCom<Text>("DscText");
            Text mNameText = tempBind.GetCom<Text>("NameText");
            var mOpenTextObj = Utility.FindChild(switchBtn.gameObject, "Background/Text (1)");
            Text mOpenText = null;
            Image mImgIcon = tempBind.GetCom<Image>("ImgIcon");
            var tableData = mSwitchClientFunctionTableList[item.m_index];
            if (switchBtn == null)
            {
                return;
            }
            if(mDscText != null)
            {
                mDscText.text = tableData.DescB;
            }
            if(mNameText != null)
            {
                mNameText.text = tableData.DescA;
            }
            
            if (mOpenTextObj != null)
            {
                mOpenText = mOpenTextObj.GetComponent<Text>();
                if (mOpenText != null && !string.IsNullOrEmpty(tableData.DescE))
                {
                    mOpenText.text = tableData.DescE;//静态关闭按钮的文本
                    switchBtn.onText = tableData.DescE;//动态按钮的文本
                }
            }

            int skillId = mTableSkillIdDic[tableData.ID][0];

            if (null != mImgIcon)
            {
                var table = TableManager.GetInstance().GetTableItem<SkillTable>(skillId);
                if (null != table)
                mImgIcon.SafeSetImage(table.Icon);
            }

            switchBtn.states = false;
            string key;
            bool toggleState;
            if (mIdJsonKeyDic.ContainsKey(tableData.ID))
            {
                key = mIdJsonKeyDic[tableData.ID];
                if (key == SettingManager.STR_CHASER_SWITCH)
                {
                    toggleState = !SettingManager.GetInstance().GetValue(key + PlayerBaseData.GetInstance().RoleID);
                }
                else
                {
                    toggleState = !SettingManager.GetInstance().GetValue(key);
                }
            }
            else
            {
                key = skillId.ToString();
                toggleState = SettingManager.GetInstance().GetSlideMode(key) == InputManager.SlideSetting.NORMAL;
            }

            switchBtn.states = !toggleState;
            
            if (!mIdJsonKeyDic.ContainsKey(tableData.ID)) //滑动设置
            {
                switchBtn.onValueChanged.AddListener(flag =>
                {
                    _onSkillSlideOpenToggleValueChange(flag, skillId.ToString());
                });
            }
            else
            {
                switchBtn.onValueChanged.AddListener(flag =>
                {
                    if (flag)
                    {
                        if (mIdJsonKeyDic[tableData.ID] == SettingManager.STR_CHASER_SWITCH)
                        {
                            SettingManager.GetInstance().SetValue(mIdJsonKeyDic[tableData.ID] + PlayerBaseData.GetInstance().RoleID, true);
                        }
                        else
                        {
                            SettingManager.GetInstance().SetValue(mIdJsonKeyDic[tableData.ID], true);
                        }
                        OnOpenSpecialOption(tableData.ID);
                    }
                    else
                    {
                        if (mIdJsonKeyDic[tableData.ID] == SettingManager.STR_CHASER_SWITCH)
                        {
                            SettingManager.GetInstance().SetValue(mIdJsonKeyDic[tableData.ID] + PlayerBaseData.GetInstance().RoleID, false);
                        }
                        else
                        {
                            SettingManager.GetInstance().SetValue(mIdJsonKeyDic[tableData.ID], false);
                        }
                        OnCloseSpecialOption(tableData.ID);
                    }
                });
            }
        }

        private void OnOpenSpecialOption(int id)
        {
            if (mIdJsonKeyDic[id] == SettingManager.STR_CHASER_SWITCH)
            {
                ClientSystemManager.GetInstance().OpenFrame<SkillChaserSettingFrame>(FrameLayer.High);
            }
        }
        
        private void OnCloseSpecialOption(int id)
        {
            if (mIdJsonKeyDic[id] == SettingManager.STR_CHASER_SWITCH)
            {
                SettingManager.GetInstance().SetChaserSetting(SettingManager.STR_CHASER_PVE, 0);
                SettingManager.GetInstance().SetChaserSetting(SettingManager.STR_CHASER_PVP, 0);
            }
        }
        
        protected void _InitSkillSlideData()
        {
            mSwitchClientFunctionTableList.Clear();
            foreach (var keyValue in mTableSkillIdDic)
            {
                bool canSlideSetting = false;
                for (int i = 0; i < keyValue.Value.Count; ++i)
                {
                    if (CanSlideSetting(keyValue.Value[i]))
                    {
                        canSlideSetting = true;
                        break;
                    }
                }
                if (canSlideSetting)
                {
                    var table = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(keyValue.Key);
                    if (table != null && table.Open)
                    {
                        mSwitchClientFunctionTableList.Add(table);
                    }
                }
            }
        }
        
        protected override void _OnOpenFrame()
        {
            InitDoublePressRun();
            InitJoystickSelect();
            InitJoystickDir();
            InitRunAttackSelect();
            InitCameraShockSelect();
            InitPaladinAttack();
            InitSlideSetting();
        }

        protected override void _OnCloseFrame()
        {
            PlayerLocalSetting.SaveConfig();

            SceneSetCustomData msg = new SceneSetCustomData();
            msg.data = (uint)(Global.Settings.hasDoubleRun ? 1 : 0);
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
        }

        void OnDragRunChange(bool isOn)
        {
            SaveDoublePressRun(isOn == false);
            if (dragCheckImg)
                dragCheckImg.gameObject.CustomActive(isOn);

            if (isOn)
            {
                if (dragRunVideo)
                {
                    dragRunVideo.Play();
                }
                if (doubleRunVideo)
                {
                    doubleRunVideo.Stop();
                }

                GameStatisticManager.GetInstance().DoStatRunning(RunningModeType.DragDropMovement);
            }
        }

        void OnDoubleRunChange(bool isOn)
        {
            SaveDoublePressRun(isOn == true);
            if (doubleCheckImg)
                doubleCheckImg.gameObject.CustomActive(isOn);

            if (isOn)
            {
                if (dragRunVideo)
                {
                    dragRunVideo.Stop();
                }
                if (doubleRunVideo)
                {
                    doubleRunVideo.Play();
                }

                GameStatisticManager.GetInstance().DoStatRunning(RunningModeType.DoubleClickMovement);
            }
        }


        void InitDoublePressRun()
        {
            bool hasDoublePress = false;
            if (PlayerLocalSetting.GetValue(SettingFrame.KEY_DOUBLE_PRESS) != null)
                hasDoublePress = (bool)PlayerLocalSetting.GetValue(SettingFrame.KEY_DOUBLE_PRESS);
            SaveDoublePressRun(hasDoublePress);

            if (dragRun)
                dragRun.isOn = !hasDoublePress;
            if (doubleRun)
                doubleRun.isOn = hasDoublePress;
            if (doubleCheckImg)
                doubleCheckImg.gameObject.CustomActive(hasDoublePress);
            if (dragCheckImg)
                dragCheckImg.gameObject.CustomActive(!hasDoublePress);

            if (dragRunVideo)
            {
                dragRunVideo.Load("controlMovie/1.mp4");
                dragRunVideo.m_bAutoPlay = true;
                dragRunVideo.Play();
                dragRunVideo.m_bLoop = true;
            }
            if (doubleRunVideo)
            {
                doubleRunVideo.Load("controlMovie/2.mp4");
                doubleRunVideo.m_bAutoPlay = true;
                doubleRunVideo.Play();
                doubleRunVideo.m_bLoop = true;
            }
        }

        void SaveDoublePressRun(bool flag)
        {
            Global.Settings.hasDoubleRun = flag;
            PlayerLocalSetting.SetValue(SettingFrame.KEY_DOUBLE_PRESS, flag);
        }

        public void LoadDoublePressConfig()
        {
            if (PlayerLocalSetting.GetValue(SettingFrame.KEY_DOUBLE_PRESS) != null)
                Global.Settings.hasDoubleRun = (bool)PlayerLocalSetting.GetValue(SettingFrame.KEY_DOUBLE_PRESS);
        }

        public void ReleaseBattleVideos()
        {
            if (dragRunVideo)
            {
                if (dragRunVideo.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
                {
                    dragRunVideo.Stop();
                    dragRunVideo.UnLoad();
                }
            }
            if (doubleRunVideo)
            {
                if (doubleRunVideo.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
                {
                    doubleRunVideo.Stop();
                    doubleRunVideo.UnLoad();
                }
            }
        }

        //能否进行滑动设置
        protected bool CanSlideSetting(int skillId)
        {
            bool haveSkill = false;
            SkillTable skillData = TableManager.instance.GetTableItem<SkillTable>(skillId);
            int roleId = PlayerBaseData.GetInstance().JobTableID;
            if(skillData == null)
            {
                return haveSkill;
            }
            for (int i = 0; i < skillData.JobID.Count; i++)
            {
                int jobId = skillData.JobID[i];
                if (jobId == roleId)
                {
                    haveSkill = true;
                }
            }
            return haveSkill;
        }
    }
}