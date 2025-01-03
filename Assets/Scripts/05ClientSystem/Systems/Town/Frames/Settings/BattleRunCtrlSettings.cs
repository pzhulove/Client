using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameClient;
using Protocol;
using Network;
using System.Collections.Generic;
using ProtoTable;

namespace _Settings
{
    public class BattleRunCtrlSettings : SettingsBindUI
    {
        private Toggle dragRun;
        private Toggle doubleRun;
        private Image dragCheckImg;
        private Image doubleCheckImg;
        private MediaPlayerCtrl dragRunVideo;
        private MediaPlayerCtrl doubleRunVideo;
        
		private Toggle mJoystickMode1 = null;
		private Toggle mJoystickMode2 = null;

        private Toggle mJoystickMoreDir = null;
        private Toggle mJoystick8Dir = null;
        private GameObject joystick8Dir = null;

        private Toggle mRunAttackMode1 = null;
        private Toggle mRunAttackMode2 = null;

        private Toggle mGrenadeSlideClose = null;
        private Toggle mGrenadeSlideOpen = null;
        private Toggle mMachineGunSlideClose = null;
        private Toggle mMachineGunSlideOpen = null;
        private Toggle mTanjiSlideClose = null;
        private Toggle mTanjiSlideOpen = null;
        private Toggle mBengshanjiClose = null;
        private Toggle mBengshanjiOpen = null;
        private RectTransform mGrenadeSetting = null;
        private RectTransform mMachineGunSetting = null;
        private RectTransform mTianjiSetting = null;
        private RectTransform mBengshanjiSetting = null;
        private RectTransform mSlideTitle = null;

        private RectTransform mSlideSetting = null;

        private Text mGrenadeName = null;
        private Text mGrenadeDsc = null;
        private Text mMachineGunName = null;
        private Text mMachineGunDsc = null;
        private Text mTianJiName = null;
        private Text mTianJiDsc = null;
        private Text mBengshanjiName = null;
        private Text mBengshanjiDsc = null;

        private Toggle mLiguiClose = null;
        private Toggle mLiguiOpen = null;
        private RectTransform mLiguiSetting = null;

        private Toggle mBackHitClose = null;
        private Toggle mBackHitOpen = null;
        private RectTransform mBackHitSetting = null;

        private Toggle mAutoHitClose = null;
        private Toggle mAutoHitOpen = null;
        private RectTransform mAutoHitSetting = null;


        private Text mPoMoFuName = null;
        private Text mPoMoFuDsc = null;
        private Text mXingLuoDaName = null;
        private Text mXingLuoDaDsc = null;
        private Toggle mPoMoFuOpen = null;
        private Toggle mPoMoFuClose = null;
        private RectTransform mPoMoFuSetting = null;
        private Toggle mXingLuoDaOpen = null;
        private Toggle mXingLuoDaClose = null;
        private RectTransform mXingLuoDaSetting = null;

        private RectTransform mPaladinAttack = null;
        private Toggle mPaladinAttackClose = null;
        private Toggle mPaladinAttackOpen = null;

        public BattleRunCtrlSettings(GameObject root, ClientFrame frame)
            : base(root, frame)
        {

        }

        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/battleCtrl";
        }

        protected override void InitBind()
        {
            dragRun = mBind.GetCom<Toggle>("DragRun");
            dragRun.onValueChanged.AddListener(OnDragRunChange);
            doubleRun = mBind.GetCom<Toggle>("DoubleRun");
            doubleRun.onValueChanged.AddListener(OnDoubleRunChange);

            dragCheckImg = mBind.GetCom<Image>("DragBgCheckImg");
            doubleCheckImg = mBind.GetCom<Image>("DoubleBgCheckImh");
            dragRunVideo = mBind.GetCom<MediaPlayerCtrl>("DragRunVideo");
            doubleRunVideo = mBind.GetCom<MediaPlayerCtrl>("DoubleRunVideo");

			mJoystickMode1 = mBind.GetCom<Toggle>("joystickMode1");
			mJoystickMode1.onValueChanged.AddListener(_onJoystickMode1ToggleValueChange); 

            mJoystickMode2 = mBind.GetCom<Toggle>("joystickMode2");
			mJoystickMode2.onValueChanged.AddListener(_onJoystickMode2ToggleValueChange);

            mJoystickMoreDir = mBind.GetCom<Toggle>("mJoystickMoreDir");
            mJoystickMoreDir.onValueChanged.AddListener(_onJoystickMoreDirToggleValueChange);

            mJoystick8Dir = mBind.GetCom<Toggle>("mJoystick8Dir");
            mJoystick8Dir.onValueChanged.AddListener(_onJoystick8DirToggleValueChange);

            joystick8Dir = mBind.GetGameObject("joystick8Dir");

            mRunAttackMode1 = mBind.GetCom<Toggle>("runAttackMode1");
            mRunAttackMode1.onValueChanged.AddListener(_onRunAttackMode1ToggleValueChange);

            mRunAttackMode2 = mBind.GetCom<Toggle>("runAttackMode2");
            mRunAttackMode2.onValueChanged.AddListener(_onRunAttackMode2ToggleValueChange);

            mGrenadeSlideClose = mBind.GetCom<Toggle>("grenadeSlideClose");
            mGrenadeSlideClose.onValueChanged.AddListener(_onGrenadeSlideCloseToggleValueChange);

            mGrenadeSlideOpen = mBind.GetCom<Toggle>("grenadeSlideOpen");
            mGrenadeSlideOpen.onValueChanged.AddListener(_onGrenadeSlideOpenToggleValueChange);

            mMachineGunSlideClose = mBind.GetCom<Toggle>("machineGunSlideClose");
            mMachineGunSlideClose.onValueChanged.AddListener(_onMachineGunSlideCloseToggleValueChange);

            mMachineGunSlideOpen = mBind.GetCom<Toggle>("machineGunSlideOpen");
            mMachineGunSlideOpen.onValueChanged.AddListener(_onMachineGunSlideOpenToggleValueChange);

            mTanjiSlideClose = mBind.GetCom<Toggle>("TanjiSlideClose");
            mTanjiSlideClose.onValueChanged.AddListener(_onTanjiSlideCloseToggleValueChange);

            mTanjiSlideOpen = mBind.GetCom<Toggle>("TanjiSlideOpen");
            mTanjiSlideOpen.onValueChanged.AddListener(_onTanjiSlideOpenToggleValueChange);

            mBengshanjiClose = mBind.GetCom<Toggle>("BengshanjiClose");
            mBengshanjiClose.onValueChanged.AddListener(_onBengshanjiCloseToggleValueChange);

            mBengshanjiOpen = mBind.GetCom<Toggle>("BengshanjiOpen");
            mBengshanjiOpen.onValueChanged.AddListener(_onBengshanjiOpenToggleValueChange);

            mGrenadeSetting = mBind.GetCom<RectTransform>("GrenadeSetting");
            mMachineGunSetting = mBind.GetCom<RectTransform>("MachineGunSetting");
            mTianjiSetting = mBind.GetCom<RectTransform>("TianjiSetting");
            mBengshanjiSetting = mBind.GetCom<RectTransform>("BengshanjiSetting");

            mSlideTitle = mBind.GetCom<RectTransform>("SlideTitle");

            mSlideSetting = mBind.GetCom<RectTransform>("SlideSetting");

            mGrenadeName = mBind.GetCom<Text>("GrenadeName");
            mGrenadeDsc = mBind.GetCom<Text>("GrenadeDsc");
            mMachineGunName = mBind.GetCom<Text>("MachineGunName");
            mMachineGunDsc = mBind.GetCom<Text>("MachineGunDsc");
            mTianJiName = mBind.GetCom<Text>("TianJiName");
            mTianJiDsc = mBind.GetCom<Text>("TianJiDsc");
            mBengshanjiName = mBind.GetCom<Text>("BengshanjiName");
            mBengshanjiDsc = mBind.GetCom<Text>("BengshanjiDsc");

            mLiguiClose = mBind.GetCom<Toggle>("LiguiClose");
            mLiguiClose.onValueChanged.AddListener(_onLiguiCloseToggleValueChange);
            mLiguiOpen = mBind.GetCom<Toggle>("LiguiOpen");
            mLiguiOpen.onValueChanged.AddListener(_onLiguiOpenToggleValueChange);
            mLiguiSetting = mBind.GetCom<RectTransform>("LiguiSetting");

            mBackHitClose = mBind.GetCom<Toggle>("BackHitClose");
            mBackHitClose.onValueChanged.AddListener(_onBackHitCloseToggleValueChange);
            mBackHitOpen = mBind.GetCom<Toggle>("BackHitOpen");
            mBackHitOpen.onValueChanged.AddListener(_onBackHitOpenToggleValueChange);
            mBackHitSetting = mBind.GetCom<RectTransform>("BackHitSetting");

            mAutoHitClose = mBind.GetCom<Toggle>("AutoHitClose");
            mAutoHitClose.onValueChanged.AddListener(_onAutoHitCloseToggleValueChange);
            mAutoHitOpen = mBind.GetCom<Toggle>("AutoHitOpen");
            mAutoHitOpen.onValueChanged.AddListener(_onAutoHitOpenToggleValueChange);
            mAutoHitSetting = mBind.GetCom<RectTransform>("AutoHitSetting");

            mPoMoFuName = mBind.GetCom<Text>("PoMoFuName");
            mPoMoFuDsc = mBind.GetCom<Text>("PoMoFuDsc");
            mXingLuoDaName = mBind.GetCom<Text>("XingLuoDaName");
            mXingLuoDaDsc = mBind.GetCom<Text>("XingLuoDaDsc");
            mPoMoFuOpen = mBind.GetCom<Toggle>("PoMoFuOpen");
            if (null != mPoMoFuOpen)
            {
                mPoMoFuOpen.onValueChanged.AddListener(_onPoMoFuOpenToggleValueChange);
            }
            mPoMoFuClose = mBind.GetCom<Toggle>("PoMoFuClose");
            if (null != mPoMoFuClose)
            {
                mPoMoFuClose.onValueChanged.AddListener(_onPoMoFuCloseToggleValueChange);
            }
            mPoMoFuSetting = mBind.GetCom<RectTransform>("PoMoFuSetting");
            mXingLuoDaOpen = mBind.GetCom<Toggle>("XingLuoDaOpen");
            if (null != mXingLuoDaOpen)
            {
                mXingLuoDaOpen.onValueChanged.AddListener(_onXingLuoDaOpenToggleValueChange);
            }
            mXingLuoDaClose = mBind.GetCom<Toggle>("XingLuoDaClose");
            if (null != mXingLuoDaClose)
            {
                mXingLuoDaClose.onValueChanged.AddListener(_onXingLuoDaCloseToggleValueChange);
            }
            mXingLuoDaSetting = mBind.GetCom<RectTransform>("XingLuoDaSetting");

            mPaladinAttack = mBind.GetCom<RectTransform>("PaladinAttack");
            mPaladinAttackClose = mBind.GetCom<Toggle>("PaladinAttackClose");
            if (null != mPaladinAttackClose)
            {
                mPaladinAttackClose.onValueChanged.AddListener(_onPaladinAttackCloseToggleValueChange);
            }
            mPaladinAttackOpen = mBind.GetCom<Toggle>("PaladinAttackOpen");
            if (null != mPaladinAttackOpen)
            {
                mPaladinAttackOpen.onValueChanged.AddListener(_onPaladinAttackOpenToggleValueChange);
            }
        }

        protected override void UnInitBind()
        {
            if(dragRun != null)
               dragRun.onValueChanged.RemoveListener(OnDragRunChange);
            dragRun = null;
            if (doubleRun != null)
                doubleRun.onValueChanged.RemoveListener(OnDoubleRunChange);
            doubleRun = null;

            dragCheckImg = null;
            doubleCheckImg = null;
            dragRunVideo = null;
            doubleRunVideo = null;

			if (mJoystickMode1 != null)
			{
				mJoystickMode1.onValueChanged.RemoveListener(_onJoystickMode1ToggleValueChange);
				mJoystickMode1 = null;	
			}
			if (mJoystickMode2 != null)
			{
				mJoystickMode2.onValueChanged.RemoveListener(_onJoystickMode2ToggleValueChange);
				mJoystickMode2 = null;
			}

            if (mRunAttackMode1 != null)
            {
                mRunAttackMode1.onValueChanged.RemoveListener(_onRunAttackMode1ToggleValueChange);
                mRunAttackMode1 = null;
            }
            if (mRunAttackMode2 != null)
            {
                mRunAttackMode2.onValueChanged.RemoveListener(_onRunAttackMode2ToggleValueChange);
                mRunAttackMode2 = null;
            }

            if (mGrenadeSlideClose != null)
            {
                mGrenadeSlideClose.onValueChanged.RemoveListener(_onGrenadeSlideCloseToggleValueChange);
                mGrenadeSlideClose = null;
            }

            if (mGrenadeSlideOpen != null)
            {
                mGrenadeSlideOpen.onValueChanged.RemoveListener(_onGrenadeSlideOpenToggleValueChange);
                mGrenadeSlideOpen = null;
            }

            if (mMachineGunSlideClose != null)
            {
                mMachineGunSlideClose.onValueChanged.RemoveListener(_onMachineGunSlideCloseToggleValueChange);
                mMachineGunSlideClose = null;
            }

            if (mMachineGunSlideOpen != null)
            {
                mMachineGunSlideOpen.onValueChanged.RemoveListener(_onMachineGunSlideOpenToggleValueChange);
                mMachineGunSlideOpen = null;
            }

            if (mTanjiSlideClose != null)
            {
                mTanjiSlideClose.onValueChanged.RemoveListener(_onTanjiSlideCloseToggleValueChange);
                mTanjiSlideClose = null;
            }

            if (mTanjiSlideOpen != null)
            {
                mTanjiSlideOpen.onValueChanged.RemoveListener(_onTanjiSlideOpenToggleValueChange);
                mTanjiSlideOpen = null;
            }

            if (mBengshanjiClose != null)
            {
                mBengshanjiClose.onValueChanged.RemoveListener(_onBengshanjiCloseToggleValueChange);
                mBengshanjiClose = null;
            }

            if (mBengshanjiOpen != null)
            {
                mBengshanjiOpen.onValueChanged.RemoveListener(_onBengshanjiOpenToggleValueChange);
                mBengshanjiOpen = null;
            }
            
            mGrenadeSetting = null;
            mMachineGunSetting = null;
            mTianjiSetting = null;
            mBengshanjiSetting = null;
            mSlideTitle = null;

            mSlideSetting = null;
            
            mGrenadeName = null;
            mGrenadeDsc = null;
            mMachineGunName = null;
            mMachineGunDsc = null;
            mTianJiName = null;
            mTianJiDsc = null;
            mBengshanjiName = null;
            mBengshanjiDsc = null;

            if (mLiguiClose != null)
            {
                mLiguiClose.onValueChanged.RemoveListener(_onLiguiCloseToggleValueChange);
                mLiguiClose = null;
            }
            if (mLiguiOpen != null)
            {
                mLiguiOpen.onValueChanged.RemoveListener(_onLiguiOpenToggleValueChange);
                mLiguiOpen = null;
            }
            
            mLiguiSetting = null;

            mPoMoFuName = null;
            mPoMoFuDsc = null;
            mXingLuoDaName = null;
            mXingLuoDaDsc = null;
            if (null != mPoMoFuOpen)
            {
                mPoMoFuOpen.onValueChanged.RemoveListener(_onPoMoFuOpenToggleValueChange);
            }
            mPoMoFuOpen = null;
            if (null != mPoMoFuClose)
            {
                mPoMoFuClose.onValueChanged.RemoveListener(_onPoMoFuCloseToggleValueChange);
            }
            mPoMoFuClose = null;
            mPoMoFuSetting = null;
            if (null != mXingLuoDaOpen)
            {
                mXingLuoDaOpen.onValueChanged.RemoveListener(_onXingLuoDaOpenToggleValueChange);
            }
            mXingLuoDaOpen = null;
            if (null != mXingLuoDaClose)
            {
                mXingLuoDaClose.onValueChanged.RemoveListener(_onXingLuoDaCloseToggleValueChange);
            }
            mXingLuoDaClose = null;
            mXingLuoDaSetting = null;
            mPaladinAttack = null;
            if (null != mPaladinAttackClose)
            {
                mPaladinAttackClose.onValueChanged.RemoveListener(_onPaladinAttackCloseToggleValueChange);
            }
            mPaladinAttackClose = null;
            if (null != mPaladinAttackOpen)
            {
                mPaladinAttackOpen.onValueChanged.RemoveListener(_onPaladinAttackOpenToggleValueChange);
            }
            mPaladinAttackOpen = null;
        }

		private void _onJoystickMode1ToggleValueChange(bool changed)
		{
			/* put your code in here */
			if (changed)
            {
                SettingManager.GetInstance().SetJoystickMode(InputManager.JoystickMode.DYNAMIC);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.DYNAMIC);
            }			
		}
		private void _onJoystickMode2ToggleValueChange(bool changed)
		{
			/* put your code in here */
			if (changed)
            {
                SettingManager.GetInstance().SetJoystickMode(InputManager.JoystickMode.STATIC);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.STATIC);
            }			
		}

        private void _onJoystickMoreDirToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetJoystickDir(InputManager.JoystickDir.MORE_DIR);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.DYNAMIC);
            }
        }
        private void _onJoystick8DirToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetJoystickDir(InputManager.JoystickDir.EIGHT_DIR);
                GameStatisticManager.GetInstance().DoStatJoyStick(InputManager.JoystickMode.DYNAMIC);
            }
        }

        private void _onRunAttackMode1ToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetRunAttackMode(InputManager.RunAttackMode.NORMAL);
            }
        }
        private void _onRunAttackMode2ToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetRunAttackMode(InputManager.RunAttackMode.QTE);
            }
        }

        private void _onSummonDisplayCloseToggleValueChange(bool changed)
        {
        }

        private void _onSummonDisplayOpenToggleValueChange(bool changed)
        {
        }

        private void _onGrenadeSlideCloseToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.NORMAL, "1204");
            }
        }
        private void _onGrenadeSlideOpenToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.SLIDE,"1204");
            }
        }
        private void _onMachineGunSlideCloseToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.NORMAL, "1007");
            }
        }
        private void _onMachineGunSlideOpenToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.SLIDE, "1007");
            }
        }

        private void _onTanjiSlideCloseToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.NORMAL, "2010");
            }
        }
        private void _onTanjiSlideOpenToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.SLIDE, "2010");
            }
        }
        private void _onBengshanjiCloseToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.NORMAL, "1512");
            }
        }
        private void _onBengshanjiOpenToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.SLIDE, "1512");
            }
        }

        private void _onLiguiCloseToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetValue(SettingManager.STR_LIGUI, false);
                GameStatisticManager.GetInstance().DoStatSlideOperation(InfiniteSwordType.Close);
            }
        }
        private void _onLiguiOpenToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                SettingManager.GetInstance().SetValue(SettingManager.STR_LIGUI, true);
                GameStatisticManager.GetInstance().DoStatSlideOperation(InfiniteSwordType.Open);
            }
        }

        private void _onBackHitCloseToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetValue(SettingManager.STR_BACKHIT, false);
            }
        }
        private void _onBackHitOpenToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                SettingManager.GetInstance().SetValue(SettingManager.STR_BACKHIT, true);
            }
        }

        private void _onAutoHitCloseToggleValueChange(bool changed)
        {
            if (changed)
            {
                SettingManager.GetInstance().SetValue(SettingManager.STR_AUTOHIT, false);
            }
        }
        private void _onAutoHitOpenToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                SettingManager.GetInstance().SetValue(SettingManager.STR_AUTOHIT, true);
            }
        }



        private void _onPoMoFuOpenToggleValueChange(bool changed)
        {
            if (changed)
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.SLIDE, "3600");
        }

        private void _onPoMoFuCloseToggleValueChange(bool changed)
        {
            if (changed)
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.NORMAL, "3600");
        }
        private void _onXingLuoDaOpenToggleValueChange(bool changed)
        {
            if (changed)
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.SLIDE, "3608");
        }
        private void _onXingLuoDaCloseToggleValueChange(bool changed)
        {
            if (changed)
                SettingManager.GetInstance().SetSlideMode(InputManager.SlideSetting.NORMAL, "3608");
        }
        private void _onPaladinAttackCloseToggleValueChange(bool changed)
        {
            if (changed)
                SettingManager.GetInstance().SetPaladinAttack(InputManager.PaladinAttack.CLOSE);
        }
        private void _onPaladinAttackOpenToggleValueChange(bool changed)
        {
            if (changed)
                SettingManager.GetInstance().SetPaladinAttack(InputManager.PaladinAttack.OPEN);
        }

        protected void InitJoystickSelect()
		{
			mJoystickMode1.isOn = false;
			mJoystickMode2.isOn = false;
			if (SettingManager.GetInstance().GetJoystickMode() == InputManager.JoystickMode.DYNAMIC)
			{
				mJoystickMode1.isOn = true;
			}
			else {
				mJoystickMode2.isOn = true;
			}
		}

        protected void InitJoystickDir()
        {
            joystick8Dir.CustomActive(SwitchFunctionUtility.IsOpen(29));
            mJoystickMoreDir.isOn = false;
            mJoystick8Dir.isOn = false;
            if (SettingManager.GetInstance().GetJoystickDir() == InputManager.JoystickDir.MORE_DIR)
            {
                mJoystickMoreDir.isOn = true;
            }
            else
            {
                mJoystick8Dir.isOn = true;
            }
        }

        protected void InitRunAttackSelect()
        {
            mRunAttackMode1.isOn = false;
            mRunAttackMode2.isOn = false;
            if (SettingManager.GetInstance().GetRunAttackMode() == InputManager.RunAttackMode.NORMAL)
            {
                mRunAttackMode1.isOn = true;
            }
            else
            {
                mRunAttackMode2.isOn = true;
            }
        }

        //初始化驱魔普攻设置
        protected void InitPaladinAttack()
        {
            SwitchClientFunctionTable paladinAttackCharge = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>((int)ClientSwitchType.PaladinAttackCharge);
            bool flag = paladinAttackCharge != null && paladinAttackCharge.Open;
            mPaladinAttack.gameObject.CustomActive(PlayerBaseData.GetInstance().JobTableID == 61 && flag);
            mPaladinAttackClose.isOn = false;
            mPaladinAttackOpen.isOn = false;
            if (SettingManager.GetInstance().GetPaladinAttack() == InputManager.PaladinAttack.OPEN)
            {
                mPaladinAttackOpen.isOn = true;
            }
            else
            {
                mPaladinAttackClose.isOn = true;
            }
        }

        //初始化滑动设置
        protected void InitSlideSetting()
        {
            SwitchClientFunctionTable grenadeData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(17);
            SwitchClientFunctionTable machineGunData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(18);
            SwitchClientFunctionTable tianjiData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(19);
            SwitchClientFunctionTable bengshanjiData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(20);
            SwitchClientFunctionTable poMoFuData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(36);
            SwitchClientFunctionTable xingLuoDaData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(37);

            //初始化名字
            //mGrenadeName.text = grenadeData.DescA;
            //mGrenadeDsc.text = grenadeData.DescB;
            //mMachineGunName.text = machineGunData.DescA;
            //mMachineGunDsc.text = machineGunData.DescB;
            //mTianJiName.text = tianjiData.DescA;
            //mTianJiDsc.text = tianjiData.DescB;
            //mBengshanjiName.text = bengshanjiData.DescA;
            //mBengshanjiDsc.text = bengshanjiData.DescB;
            mPoMoFuName.text = poMoFuData.DescA;
            mPoMoFuDsc.text = poMoFuData.DescB;
            mXingLuoDaName.text = xingLuoDaData.DescA;
            mXingLuoDaDsc.text = xingLuoDaData.DescB;

            int roleId = PlayerBaseData.GetInstance().JobTableID;
            //手雷设置
            bool grenadeSwitchFlag = bengshanjiData.Open;
            bool canGrenadeSlide = (CanSlideSetting(1204) || CanSlideSetting(2507))&& grenadeSwitchFlag;
            if (canGrenadeSlide)
                InitGrenadeSlideSelect();
            mGrenadeSetting.gameObject.CustomActive(canGrenadeSlide && grenadeSwitchFlag);
            //格林机枪设置
            bool machineGunSwitchFlag = machineGunData.Open;
            bool canMachineGunSlide = (CanSlideSetting(1007) || CanSlideSetting(2508))&& machineGunSwitchFlag;
            if (canMachineGunSlide)
                InitMachineGunSlideSelect();
            mMachineGunSetting.gameObject.CustomActive(canMachineGunSlide && machineGunSwitchFlag);
            //天击设置
            bool tianjiSwitchFlag = tianjiData.Open;
            bool canTianjiSlide = CanSlideSetting(2010) && tianjiSwitchFlag;
            if (canTianjiSlide)
                InitTianjiSlideSelect();
            mTianjiSetting.gameObject.CustomActive(canTianjiSlide);
            //崩山击设置
            bool bengshanjiSwitchFlag = bengshanjiData.Open;
            bool canBengshanjiSlide = (CanSlideSetting(1512)|| CanSlideSetting(1716)) && bengshanjiSwitchFlag;
            if (canBengshanjiSlide)
                InitBengshanjiSlideSelect();
            mBengshanjiSetting.gameObject.CustomActive(canBengshanjiSlide);

            //里鬼设置
            bool liguiSwitchFlag = SwitchFunctionUtility.IsOpen(24);
            bool canLiguiSetting = CanSlideSetting(1901) && liguiSwitchFlag;
            if (canLiguiSetting)
                InitLiguiSettingSelect();
            mLiguiSetting.gameObject.CustomActive(canLiguiSetting);

            //背身回击设置
            bool backHitSwitchFlag = SwitchFunctionUtility.IsOpen(51);
            bool canbackHitSetting = CanSlideSetting(1910) && backHitSwitchFlag;
            if (canbackHitSetting)
                InitBackHitSettingSelect();
            mBackHitSetting.gameObject.CustomActive(canbackHitSetting);

            //自动反击设置
            bool autoHitSwitchFlag = SwitchFunctionUtility.IsOpen(52);
            bool canAutoHitSetting = CanSlideSetting(1107) || CanSlideSetting(2527) && autoHitSwitchFlag;
            if (canAutoHitSetting)
                InitAutoHitSettingSelect();
            mAutoHitSetting.gameObject.CustomActive(canAutoHitSetting);

            //破魔符设置
            bool poMoFuSwitchFlag = poMoFuData.Open;
            bool canPoMoFuSlide = CanSlideSetting(3600) && poMoFuSwitchFlag;
            if (canPoMoFuSlide)
                InitPoMoFuSlideSelect();
            mPoMoFuSetting.gameObject.CustomActive(canPoMoFuSlide);
            //星落打设置
            bool xingLuoDaSwitchFlag = xingLuoDaData.Open;
            bool canXingLuoDaSlide = CanSlideSetting(3608) && xingLuoDaSwitchFlag;
            if (canXingLuoDaSlide)
                InitXingLuoDaSlideSelect();
            mXingLuoDaSetting.gameObject.CustomActive(canXingLuoDaSlide);

            mSlideTitle.gameObject.CustomActive(canGrenadeSlide | canMachineGunSlide | canTianjiSlide | canBengshanjiSlide | canLiguiSetting | canPoMoFuSlide | canXingLuoDaSlide);
        }

        //初始化手雷滑动设置
        protected void InitGrenadeSlideSelect()
        {
            mGrenadeSlideClose.isOn = false;
            mGrenadeSlideOpen.isOn = false;
            if (SettingManager.GetInstance().GetSlideMode("1204") == InputManager.SlideSetting.NORMAL)
            {
                mGrenadeSlideClose.isOn = true;
            }
            else
            {
                mGrenadeSlideOpen.isOn = true;
            }
        }

        //初始化格林机枪滑动设置
        protected void InitMachineGunSlideSelect()
        {
            mMachineGunSlideClose.isOn = false;
            mMachineGunSlideOpen.isOn = false;
            if (SettingManager.GetInstance().GetSlideMode("1007") == InputManager.SlideSetting.NORMAL)
            {
                mMachineGunSlideClose.isOn = true;
            }
            else
            {
                mMachineGunSlideOpen.isOn = true;
            }
        }

        //初始化天击滑动设置
        protected void InitTianjiSlideSelect()
        {
            mTanjiSlideClose.isOn = false;
            mTanjiSlideOpen.isOn = false;
            if (SettingManager.GetInstance().GetSlideMode("2010") == InputManager.SlideSetting.NORMAL)
            {
                mTanjiSlideClose.isOn = true;
            }
            else
            {
                mTanjiSlideOpen.isOn = true;
            }
        }

        //初始化崩山击滑动设置
        protected void InitBengshanjiSlideSelect()
        {
            mBengshanjiClose.isOn = false;
            mBengshanjiOpen.isOn = false;
            if (SettingManager.GetInstance().GetSlideMode("1512") == InputManager.SlideSetting.NORMAL)
            {
                mBengshanjiClose.isOn = true;
            }
            else
            {
                mBengshanjiOpen.isOn = true;
            }
        }

        //初始化破魔符滑动设置
        protected void InitPoMoFuSlideSelect()
        {
            mPoMoFuClose .isOn = false;
            mPoMoFuOpen .isOn = false;
            if (SettingManager.GetInstance().GetSlideMode("3600") == InputManager.SlideSetting.NORMAL)
            {
                mPoMoFuClose.isOn = true;
            }
            else
            {
                mPoMoFuOpen.isOn = true;
            }
        }

        //初始化星落打滑动设置
        protected void InitXingLuoDaSlideSelect()
        {
            mXingLuoDaClose.isOn = false;
            mXingLuoDaOpen.isOn = false;
            if (SettingManager.GetInstance().GetSlideMode("3608") == InputManager.SlideSetting.NORMAL)
            {
                mXingLuoDaClose.isOn = true;
            }
            else
            {
                mXingLuoDaOpen.isOn = true;
            }
        }

        protected void InitLiguiSettingSelect()
        {
            mLiguiClose.isOn = false;
            mLiguiOpen.isOn = false;
            if (SettingManager.GetInstance().GetValue(SettingManager.STR_LIGUI))
                mLiguiOpen.isOn = true;
            else
                mLiguiClose.isOn = true;
        }

        protected void InitBackHitSettingSelect()
        {
            mBackHitClose.isOn = false;
            mBackHitOpen.isOn = false;
            if (SettingManager.GetInstance().GetValue(SettingManager.STR_BACKHIT))
                mBackHitOpen.isOn = true;
            else
                mBackHitClose.isOn = true;
        }

        protected void InitAutoHitSettingSelect()
        {
            mAutoHitClose.isOn = false;
            mAutoHitOpen.isOn = false;
            if (SettingManager.GetInstance().GetValue(SettingManager.STR_AUTOHIT))
                mAutoHitOpen.isOn = true;
            else
                mAutoHitClose.isOn = true;
        }


        protected override void OnShowOut()
        {
            InitDoublePressRun();
			InitJoystickSelect();
            InitJoystickDir();
            InitRunAttackSelect();
            InitPaladinAttack();
            InitSlideSetting();
        }

        protected override void OnHideIn()
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
            for(int i = 0; i < skillData.JobID.Count; i++)
            {
                int jobId = skillData.JobID[i];
                if(jobId == roleId)
                {
                    haveSkill = true;
                }
            }
            return haveSkill;
        }
    }
}