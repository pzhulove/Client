using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using Protocol;
using Network;
using System;

namespace _Settings
{

	public class HelpDebugSettings : SettingsBindUI
    {
		static string[] DEBUG_NAMES = {
			"DummyEffect",
			"DisableAnimMaterial",
			"DisableGameObjPool",
			"DisableAnimation",
			"DisableHitText",
			"DisableAudio",
			"DisableChatDisplay",
			"DisableModuleLoad",
			"DisableSnap",
			"DisableAI",
			"DisablePreload",
			"EnableActionFrameCache",
			"EnableDSkillDataCache",
			"EnableTestFashionEquip"
		};

		static int debugCount = 14;

		Toggle[] debugSettings = new Toggle[debugCount]; 

		public HelpDebugSettings(GameObject root, ClientFrame frame)
            : base(root, frame)
        {

        }

        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/debug";
        }

        protected override void InitBind()
        {
			for(int i=0; i<debugCount; ++i)
			{
				var name = string.Format("DebugSetting{0}", (i+1));
				var toggle = mBind.GetCom<Toggle>(name);

				int index = i;

				toggle.onValueChanged.AddListener((bool isOn)=>{
					#if DEBUG_REPORT_ROOT
					DebugSettings.instance.SetDebugEnable(DEBUG_NAMES[index], isOn);
					#endif
				});

				debugSettings[i] = toggle;


				toggle.isOn = false;
				#if DEBUG_REPORT_ROOT
				toggle.isOn = DebugSettings.instance.IsDebugEnable(DEBUG_NAMES[index]);
				#endif
			}
        }

        protected override void UnInitBind()
        {
			for(int i=0; i<debugCount; ++i)
			{
				if (debugSettings[i] != null)
				{
					debugSettings[i].onValueChanged.RemoveAllListeners();
				}
			}
        }

        protected override void OnShowOut()
        {
           
        }

        protected override void OnHideIn()
        {

        }
    }
}
