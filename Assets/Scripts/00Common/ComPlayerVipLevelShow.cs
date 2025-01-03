using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public enum ShowMode
    {
        MySelf,
        Other,
    }

    class ComPlayerVipLevelShow : MonoBehaviour
    {
        static string[] vipNumPath = new string[] 
        {
            "UI/Image/NewPacked/Vip_Icon.png:Vip_00",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_01",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_02",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_03",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_04",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_05",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_06",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_07",    
            "UI/Image/NewPacked/Vip_Icon.png:Vip_08",
            "UI/Image/NewPacked/Vip_Icon.png:Vip_09",      
        };      

        [SerializeField]
        Image vipLv0 = null;

        [SerializeField]
        Image vipLv1 = null;
    
        [SerializeField]
        Button btnVip = null;

        [SerializeField]
        bool vipBtnCanClick = true;

        [SerializeField]
        ShowMode showMode = ShowMode.MySelf;      

        private void Awake()
        {
            if (showMode == ShowMode.Other)
            {
                vipBtnCanClick = false;
            }

            btnVip.SafeSetOnClickListener(() =>
            {
                if (!vipBtnCanClick)
                {
                    return;
                }

                if (ClientSystemManager.instance.IsFrameOpen<VipFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<VipFrame>();
                }

                var frame = ClientSystemManager.instance.OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                frame.OpenPayTab();
                GameStatisticManager.GetInstance().DoStartUIButton("VIP");
            });

            if (showMode == ShowMode.MySelf)
            {
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);

                UpdateUI(PlayerBaseData.GetInstance().VipLevel);
            }
        }

        private void Start()
        {
            
        }   

        private void OnDestroy()
        {
            if(showMode == ShowMode.MySelf)
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);
            }            
        }

        void _OnVipLvChanged(UIEvent uiEvent)
        {      
            UpdateUI(PlayerBaseData.GetInstance().VipLevel);
        }       

        void UpdateUI(int level)
        {
            if(level >= 0)
            {
                int lv0 = level / 10;
                int lv1 = level % 10;
                if (lv0 < vipNumPath.Length)
                {
                    vipLv0.SafeSetImage(vipNumPath[lv0]);
                }
                if (lv1 < vipNumPath.Length)
                {
                    vipLv1.SafeSetImage(vipNumPath[lv1]);
                }

                vipLv0.CustomActive(lv0 > 0);
            }       
        }

        void _OnSystemChanged(UIEvent a_event)
        {
            var system = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if(system == null)
            {
                return;
            }

            UpdateUI(PlayerBaseData.GetInstance().VipLevel);
        }
        
        public void SetVipLevel(int level)
        {
            UpdateUI(level);
        }

        public void SetUp(ShowMode mode,bool canClickVipBtn)
        {
            showMode = mode;
            vipBtnCanClick = canClickVipBtn;

            if (showMode == ShowMode.Other)
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);

                vipBtnCanClick = false;
            }
            else
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);

                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerVipLvChanged, _OnVipLvChanged);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);
            }
        }
    }
}