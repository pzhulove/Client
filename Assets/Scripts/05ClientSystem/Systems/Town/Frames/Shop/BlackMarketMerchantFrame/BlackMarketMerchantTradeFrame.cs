using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 黑市商人交易界面
    /// </summary>
    public class BlackMarketMerchantTradeFrame : ClientFrame
    {
        #region ExtraUIBind
        private BlackMarketMerchantTradeView mBlackMarketMerchanTradeView = null;

        protected sealed override void _bindExUI()
        {
            mBlackMarketMerchanTradeView = mBind.GetCom<BlackMarketMerchantTradeView>("BlackMarketMerchanTradeView");
        }

        protected sealed override void _unbindExUI()
        {
            mBlackMarketMerchanTradeView = null;
        }
        #endregion
        ApplyTradData mData = null;
        ulong GUID = 0;
        uint mPrice = 0;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Shop/BlackMarketMerchantFrame/BlackMarketMerchanTradeSettingFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mData = userData as ApplyTradData;

            if (mData != null)
            {
                mBlackMarketMerchanTradeView.InitView(mData, OnItemSelect, OnSetPrice, OnConfirmClick);
            }
        }

        void OnItemSelect(ulong guid)
        {
            GUID = guid;
        }

        void OnSetPrice(uint price)
        {
            mPrice = price;
        }

        void OnConfirmClick()
        {
            //未选择要交易的装备
            if (GUID == 0)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("blackMarketMerchan_selectEquip"));
                return;
            }

            ItemData mItemData = ItemDataManager.GetInstance().GetItem(GUID);
            if (mItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("blackMarketMerchan_selectEquip"));
                return;
            }

            //装备锁住不能交易
            if (mItemData.bLocked)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("blackMarketMerchan_bLockEquip"));
                return;
            }

            //时限装备
            if (mItemData.Type == ItemTable.eType.EQUIP)
            {
                if (mItemData.DeadTimestamp > 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("blackMarketMerchan_deadTimestamp"));
                    return;
                }
            }
           
            //增幅装备，增加二次弹框确认
            if (mItemData.EquipType == EEquipType.ET_REDMARK)
            {
                string mContent = TR.Value("growth_equip_desc", "确定要申请交易吗");
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () => 
                {
                    OnOnSendWorldBlackMarketAuctionReq(mData);
                });
            }
            else
            {
                OnOnSendWorldBlackMarketAuctionReq(mData);
            }
        }

        //请求竞拍
        private void OnOnSendWorldBlackMarketAuctionReq(ApplyTradData mData)
        {
            //如果商人回购类型是固定价格
            if (mData.mInfo.back_buy_type == (uint)BlackMarketType.BmtFixedPrice)
            {
                BlackMarketMerchantDataManager.GetInstance().OnSendWorldBlackMarketAuctionReq(mData.mInfo.guid, GUID, mData.mInfo.price);
            }
            else//商人回购类型是竞拍价格
            {
                //为输入竞拍价格
                if (mPrice == 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("blackMarketMerchan_inputPrice"));
                    return;
                }

                BlackMarketMerchantDataManager.GetInstance().OnSendWorldBlackMarketAuctionReq(mData.mInfo.guid, GUID, mPrice);
            }

            Close();
        }

        protected sealed override void _OnCloseFrame()
        {
            mData = null;
            GUID = 0;
            mPrice = 0;
        }
    }
}
