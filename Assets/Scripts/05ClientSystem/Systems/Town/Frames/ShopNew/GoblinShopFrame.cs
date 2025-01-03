using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinShopData
    {
        public int activityId;//对应的活动id
        public AccountShopQueryIndex accountShopItem = new AccountShopQueryIndex();
    }
    public class GoblinShopFrame : ClientFrame
    {
        private GoblinShopData thisGoblinShopData;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ShopNew/GoblinShopFrame";
        }
        protected override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            thisGoblinShopData = userData as GoblinShopData;
            if(thisGoblinShopData != null)
            {
                AccountShopQueryIndex accountShopQueryIndex = new AccountShopQueryIndex();
                accountShopQueryIndex.shopId = thisGoblinShopData.accountShopItem.shopId;
                accountShopQueryIndex.jobType = 0;
                accountShopQueryIndex.tabType = 0;
                AccountShopDataManager.GetInstance().SendWorldAccountShopItemQueryReq(accountShopQueryIndex);
            }
        }
        protected override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
        }
        #region 事件
        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountShopUpdate, _AccountShopUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SpeicialItemUpdate, _SpecialItemUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountShopItemUpdata, _AccountShopItemUpdata);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountShopUpdate, _AccountShopUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SpeicialItemUpdate, _SpecialItemUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountShopItemUpdata, _AccountShopItemUpdata);
        }

        void _AccountShopUpdate(UIEvent uiEvent)
        {
            AccountShopQueryIndex accountShopIndex = (AccountShopQueryIndex)uiEvent.Param1;
            //mGoblinShopView.UpdateShop(accountShopIndex);
            if (mGoblinShopView != null && 
                accountShopIndex.shopId == thisGoblinShopData.accountShopItem.shopId &&
                accountShopIndex.tabType == thisGoblinShopData.accountShopItem.tabType &&
                accountShopIndex.jobType == thisGoblinShopData.accountShopItem.jobType)
            {
                mGoblinShopView.InitShop(thisGoblinShopData);
            }
        }

        void _SpecialItemUpdate(UIEvent uiEvent)
        {
            int id = (int)uiEvent.Param1;
            mGoblinShopView.UpdateSpecialNum(id);
            mGoblinShopView.UpdateShopItem(thisGoblinShopData.accountShopItem);
        }

        void _AccountShopItemUpdata(UIEvent uiEvent)
        {
            AccountShopQueryIndex accountShopIndex = (AccountShopQueryIndex)uiEvent.Param1;
            if (mGoblinShopView != null && 
                accountShopIndex.shopId == thisGoblinShopData.accountShopItem.shopId &&
                accountShopIndex.tabType == thisGoblinShopData.accountShopItem.tabType &&
                accountShopIndex.jobType == thisGoblinShopData.accountShopItem.jobType)
            {
                mGoblinShopView.UpdateShopItem(accountShopIndex);
            }
        }
        #endregion
        #region ExtraUIBind
        private GoblinShopView mGoblinShopView = null;
		
		protected override void _bindExUI()
		{
			mGoblinShopView = mBind.GetCom<GoblinShopView>("GoblinShopView");
		}
		
		protected override void _unbindExUI()
		{
			mGoblinShopView = null;
		}
		#endregion


        
    }

}
