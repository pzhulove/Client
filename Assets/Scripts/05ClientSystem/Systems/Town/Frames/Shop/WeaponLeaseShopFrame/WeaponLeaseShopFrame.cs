using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class WeaponLeaseShopFrame : ClientFrame
    {
        ShopData mShopData;
        #region ExtraUIBind
        private WeaponLeaseShopView mWeaponLeaseShopView = null;

        protected sealed override void _bindExUI()
        {
            mWeaponLeaseShopView = mBind.GetCom<WeaponLeaseShopView>("WeaponLeaseShopView");
        }

        protected sealed override void _unbindExUI()
        {
            mWeaponLeaseShopView = null;
        }
        #endregion
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                int weaponLeaseShopId = 0;
                int.TryParse(strParam, out weaponLeaseShopId);
                ShopDataManager.GetInstance().OpenShop(weaponLeaseShopId);
            }
            catch (System.Exception e)
            {
                Logger.LogError("WeaponLeaseShopFrame.OpenLinkFrame : ==>" + e.ToString());
            }
        }
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Shop/WeaponLeaseShopFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mShopData = userData as ShopData;
            if (mShopData != null) 
            {
                mWeaponLeaseShopView.InitView(this, mShopData, OnClickLease);
            }
        }

        void OnClickLease(GoodsData goodsData)
        {
            if (goodsData.eGoodsLimitButyType != GoodsLimitButyType.GLBT_NONE)
            {
                if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_TOWER_LEVEL)
                {
                    int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                    if (iCurValue < goodsData.iLimitValue)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_tower_level"), goodsData.iLimitValue));
                        return;
                    }
                }
                else if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_FIGHT_SCORE)
                {
                    int iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                    if (iCurValue < goodsData.iLimitValue)
                    {
                        var rankName = SeasonDataManager.GetInstance().GetRankName(goodsData.iLimitValue);
                        SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_fight_score"), rankName));
                        return;
                    }
                }
                else if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_GUILD_LEVEL)
                {
                    int iCurValue = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.SHOP);
                    if (iCurValue < goodsData.iLimitValue)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_guild_level"), goodsData.iLimitValue));
                        return;
                    }
                }
            }

            if (goodsData.VipLimitLevel > 0 && goodsData.VipLimitLevel > PlayerBaseData.GetInstance().VipLevel)
            {
                SystemNotifyManager.SystemNotify(1800011, () =>
                {
                    var vipFrame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                    vipFrame.OpenPayTab();
                });
                return;
            }
            _OnGoodsClicked(goodsData);
        }

        void _OnGoodsClicked(GoodsData goodsData)
        {
            if (!frameMgr.IsFrameOpen<PurchaseCommonFrame>())
            {
                frameMgr.OpenFrame<PurchaseCommonFrame>(FrameLayer.Middle);
            }

            UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
            if (uiEvent != null)
            {
                uiEvent.EventID = EUIEventID.PurchaseCommanUpdate;

                uiEvent.EventParams.kPurchaseCommonData.iShopID = mShopData.ID.HasValue ? mShopData.ID.Value : 0;
                uiEvent.EventParams.kPurchaseCommonData.iGoodID = goodsData.ID.HasValue ? goodsData.ID.Value : 0;
                uiEvent.EventParams.kPurchaseCommonData.iItemID = (int)goodsData.ItemData.TableID;
                uiEvent.EventParams.kPurchaseCommonData.iCount = goodsData.ItemData.Count;
                UIEventSystem.GetInstance().SendUIEvent(uiEvent);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mWeaponLeaseShopView.UnInitView();
            mShopData = null;
        }
    }
}
