using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SkillChaserSettingFrame : ClientFrame
    {
	    #region ExtraUIBind
		private Toggle mPVPToggle5 = null;
		private Toggle mPVPToggle4 = null;
		private Toggle mPVPToggle3 = null;
		private Toggle mPVPToggle2 = null;
		private Toggle mPVPToggle1 = null;
		private Toggle mPVEToggle5 = null;
		private Toggle mPVEToggle4 = null;
		private Toggle mPVEToggle3 = null;
		private Toggle mPVEToggle2 = null;
		private Toggle mPVEToggle1 = null;
		private Button mSureBtn = null;
		private Button mCloseBtn = null;

		protected override void _bindExUI()
		{
			mPVPToggle5 = mBind.GetCom<Toggle>("PVPToggle5");
			mPVPToggle4 = mBind.GetCom<Toggle>("PVPToggle4");
			mPVPToggle3 = mBind.GetCom<Toggle>("PVPToggle3");
			mPVPToggle2 = mBind.GetCom<Toggle>("PVPToggle2");
			mPVPToggle1 = mBind.GetCom<Toggle>("PVPToggle1");
			mPVEToggle5 = mBind.GetCom<Toggle>("PVEToggle5");
			mPVEToggle4 = mBind.GetCom<Toggle>("PVEToggle4");
			mPVEToggle3 = mBind.GetCom<Toggle>("PVEToggle3");
			mPVEToggle2 = mBind.GetCom<Toggle>("PVEToggle2");
			mPVEToggle1 = mBind.GetCom<Toggle>("PVEToggle1");
			mSureBtn = mBind.GetCom<Button>("sureBtn");
			mSureBtn.onClick.AddListener(_onSureBtnButtonClick);
			mCloseBtn = mBind.GetCom<Button>("closeBtn");
			mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
		}

		protected override void _unbindExUI()
		{
			mPVPToggle5 = null;
			mPVPToggle4 = null;
			mPVPToggle3 = null;
			mPVPToggle2 = null;
			mPVPToggle1 = null;
			mPVEToggle5 = null;
			mPVEToggle4 = null;
			mPVEToggle3 = null;
			mPVEToggle2 = null;
			mPVEToggle1 = null;
			mSureBtn.onClick.RemoveListener(_onSureBtnButtonClick);
			mSureBtn = null;
			mCloseBtn.onClick.RemoveListener(_onCloseBtnButtonClick);
			mCloseBtn = null;
		}
		#endregion

		public override string GetPrefabPath()
		{
			return "UIFlatten/Prefabs/Skill/SkillChaserSettingFrame";
		}

		protected override void _OnOpenFrame()
		{
			_InitUI();
		}

		private void _InitUI()
		{
			var data = DecodeToArray(SettingManager.GetInstance().GetChaserSetting(SettingManager.STR_CHASER_PVE));
			mPVEToggle1.isOn = data[0];
			mPVEToggle2.isOn = data[1];
			mPVEToggle3.isOn = data[2];
			mPVEToggle4.isOn = data[3];
			mPVEToggle5.isOn = data[4];
			
			data = DecodeToArray(SettingManager.GetInstance().GetChaserSetting(SettingManager.STR_CHASER_PVP));
			mPVPToggle1.isOn = data[0];
			mPVPToggle2.isOn = data[1];
			mPVPToggle3.isOn = data[2];
			mPVPToggle4.isOn = data[3];
			mPVPToggle5.isOn = data[4];
		}

		#region Callback
		
		private void _onSureBtnButtonClick()
		{
			bool[] settingValue = new bool[5];
			settingValue[0] = mPVEToggle1.isOn;
			settingValue[1] = mPVEToggle2.isOn;
			settingValue[2] = mPVEToggle3.isOn;
			settingValue[3] = mPVEToggle4.isOn;
			settingValue[4] = mPVEToggle5.isOn;
			SettingManager.GetInstance().SetChaserSetting(SettingManager.STR_CHASER_PVE, EncodeToInt(settingValue));
			
			settingValue[0] = mPVPToggle1.isOn;
			settingValue[1] = mPVPToggle2.isOn;
			settingValue[2] = mPVPToggle3.isOn;
			settingValue[3] = mPVPToggle4.isOn;
			settingValue[4] = mPVPToggle5.isOn;
			SettingManager.GetInstance().SetChaserSetting(SettingManager.STR_CHASER_PVP, EncodeToInt(settingValue));
			
			frameMgr.CloseFrame(this);
		}

		private int EncodeToInt(bool[] value)
		{
			int ret = 0;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i])
				{
					ret |= 1 << i;
				}
			}

			return 32 - 1 - ret;
		}

		private bool[] DecodeToArray(int code)
		{
			bool[] ret = new bool[5];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = (code & (1 << i)) == 0;
			}
			return ret;
		}
		
		private void _onCloseBtnButtonClick()
		{
			frameMgr.CloseFrame(this);
		}
		#endregion
    }
}