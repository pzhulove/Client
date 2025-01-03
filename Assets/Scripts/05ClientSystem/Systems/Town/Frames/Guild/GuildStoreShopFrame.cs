using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildStoreShopFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildStoreShopFrame";
        }

        ShopFrame shopFrame = null;
        int m_iShopFrameId = 0;
        void OnOpenChildShopFrame(int iShopID, ShopFrame child,int iId)
        {
            if(m_iShopFrameId != iId)
            {
                return;
            }

            shopFrame = child;
        }

        protected override void _OnOpenFrame()
        {
            m_iShopFrameId = ShopDataManager.GetInstance().RegisterMainFrame();

            ShopDataManager.GetInstance().onOpenChildShopFrame += OnOpenChildShopFrame;
            ShopDataManager.GetInstance().OpenShop(11, 0, -1, null, frame, ShopFrame.ShopFrameMode.SFM_GUILD_CHILD_FRAME, m_iShopFrameId);      
        }

        protected override void _OnCloseFrame()
        {
            ShopDataManager.GetInstance().UnRegisterMainFrame(m_iShopFrameId);
            ShopDataManager.GetInstance().onOpenChildShopFrame -= OnOpenChildShopFrame;

            if(shopFrame != null)
            {
                shopFrame.Close(true);
                shopFrame = null;
            }
        }
    }
}