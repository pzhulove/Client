using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum VibrateSwitchType
    {
        Team,           //组队询问触发
        Pk3v3,          //3v3匹配成功
        MysteryShop,    //神秘商店
    }

    public class DeviceVibrateManager : Singleton<DeviceVibrateManager>
    {
        private string _GetVibrateSwitchKeyByType(VibrateSwitchType type)
        {
            string serverId = "";
            string roleId = "";
            if (null != ClientApplication.playerinfo)
            {
                serverId = ClientApplication.playerinfo.serverID.ToString();
            }
            if (null != PlayerBaseData.GetInstance())
            {
                roleId = PlayerBaseData.GetInstance().RoleID.ToString();
            }
            return string.Format(TR.Value("vibrate_settings_key"), serverId, roleId, type.ToString());
        }

        public bool CheckVibrateSwitchOpen(VibrateSwitchType type)
        {
            string typeKey = _GetVibrateSwitchKeyByType(type);
            if (PlayerPrefs.HasKey(typeKey))
            {
                return PlayerPrefs.GetInt(typeKey) == 1 ? true : false;
            }
            return true;                //默认开启 同对应预制体默认状态
        }

        public void SetVibrateSwitch(VibrateSwitchType type, bool bOpen)
        {
            string typeKey = _GetVibrateSwitchKeyByType(type);
            PlayerPrefs.SetInt(typeKey, bOpen ? 1 : 0);

            if(PlayerPrefs.HasKey(typeKey))
            {
                Logger.LogProcessFormat("[DeviceVibrateManager] - SetVibrateSwitch : {0}, {1}", typeKey, PlayerPrefs.GetInt(typeKey).ToString());
            }
        }

        public void TriggerDeviceVibrateByType(VibrateSwitchType type)
        {
            if (CheckVibrateSwitchOpen(type))
            {
                PluginManager.GetInstance().TriggerMobileVibrate();
            }
        }
    }
}