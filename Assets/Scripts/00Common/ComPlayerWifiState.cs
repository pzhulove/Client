using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class ComPlayerWifiState : MonoBehaviour
    {
        static string[] statePath = new string[]
       {
           "",
           "UI/Image/NewPacked/MainUI01.png:MainUI01_Wifi_Xinhao_01",
           "UI/Image/NewPacked/MainUI01.png:MainUI01_Wifi_Xinhao_02",
           "UI/Image/NewPacked/MainUI01.png:MainUI01_Wifi_Xinhao_03",
       };

        [SerializeField]
        Image imgWifiState = null;

        BattlePlayer.eNetQuality netOldQuality = BattlePlayer.eNetQuality.Off;

        private void Awake()
        {
            netOldQuality = BattlePlayer.eNetQuality.Off;
        }

        private void Start()
        {
            
        }   

        private void OnDestroy()
        {
            netOldQuality = BattlePlayer.eNetQuality.Off;
        }
      

        public void SetUp(BattlePlayer.eNetQuality netQuality)
        {
            if(statePath == null)
            {
                return;
            }

            if(netOldQuality == netQuality)
            {
                return;
            }

            netOldQuality = netQuality;

            int index = (int)netQuality;
            if(index >= statePath.Length)
            {
                return;
            }

            imgWifiState.SafeSetImage(statePath[index]);
        }
    }
}