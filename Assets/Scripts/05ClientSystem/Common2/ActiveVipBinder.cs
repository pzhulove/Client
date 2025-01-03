using UnityEngine;
using System.Collections;

namespace GameClient
{
    class ActiveVipBinder : MonoBehaviour
    {
        public GameObject[] vipVisible = new GameObject[0];
        public GameObject[] unvipVisible = new GameObject[0];
        public int iVipLv = 0;
        // Use this for initialization
        void Start()
        {
            _Update();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);
        }

        void _OnVipLvChanged(UIEvent uiEvent)
        {
            _Update();
        }

        void _Update()
        {
            bool vipLvOk = PlayerBaseData.GetInstance().VipLevel >= iVipLv;
            for(int i = 0; i < vipVisible.Length; ++i)
            {
                vipVisible[i].CustomActive(vipLvOk);
            }
            for (int i = 0; i < unvipVisible.Length; ++i)
            {
                unvipVisible[i].CustomActive(!vipLvOk);
            }
        }
    }
}