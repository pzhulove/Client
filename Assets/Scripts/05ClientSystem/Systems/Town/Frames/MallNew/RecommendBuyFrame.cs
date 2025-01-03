using System;
using Protocol;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine;
using Network;
using System.Collections.Generic;
using System.Globalization;
using Scripts.UI;
using ActivityLimitTime;

namespace GameClient
{
    class RecommendBuyFrame : ClientFrame
    {
        private MallNewItemContent mContent;
        [UIEventHandle("BtnClose")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MallNew/RecommendBuyFrame";
        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnCloseFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, OnCloseFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetCommendAccountShopItemSucc, OnCloseFrame);
            mContent = frame.GetComponent<MallNewItemContent>();
            if (null == mContent)
            {
                Logger.LogError("找不到对应预制体脚本 MallNewItemContent");
                return;
            }
            if (userData is MallItemInfo)
            {
                var mallItemData = userData as MallItemInfo;
                mContent.OnInitMallItem(mallItemData);
                return;
            }
            if (userData is ShopNewShopItemInfo)
            {
                var shopItemData = userData as ShopNewShopItemInfo;
                mContent.OnInitShopItem(shopItemData);
                return;
            }
            if (userData is AccountShopItemInfo)
            {
                var accShopItemData = userData as AccountShopItemInfo;
                mContent.OnInitAccountShopItem(accShopItemData);
                return;
            }
        }

        private void OnCloseFrame(UIEvent uiEvent)
        {
            frameMgr.CloseFrame(this);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnCloseFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, OnCloseFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetCommendAccountShopItemSucc, OnCloseFrame);
            if (ClientSystemManager.GetInstance().IsFrameOpen<VirtualKeyboardFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<VirtualKeyboardFrame>();
            }
        }
        // public override bool IsNeedUpdate()
        // {
        //     return _isUpdate;
        // }
        // protected override void _OnUpdate(float timeElapsed)
        // {
        //     if (mMallItemData == null)
        //     {
        //         return;
        //     }
        //     int time = (int)(mMallItemData.multipleEndTime - TimeManager.GetInstance().GetServerTime());
        //     if (time <= 0)
        //     {
        //         UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSendQueryMallItemInfo, mMallItemData.itemid);
        //         _isUpdate = false;
        //     }
        // }

    }
}
