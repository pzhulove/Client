using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LimitTimeGift;
using System.Collections.Generic;
using ActivityLimitTime;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    public class LimitTimePetGiftFrame : ClientFrame
    {
        List<MallItemInfo> data = null;
        #region ExtraUIBind
        private LimitTimePetGiftFrameView mLimitTimePetGiftFrame = null;
        //对应商城表中的ID
        private const int MallTypeTableId = 9;
        protected sealed override void _bindExUI()
        {
            mLimitTimePetGiftFrame = mBind.GetCom<LimitTimePetGiftFrameView>("LimitTimePetGiftFrame");
        }

        protected sealed override void _unbindExUI()
        {
            mLimitTimePetGiftFrame = null;
        }
        #endregion

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LimitTimeGift/LimitTimePetGiftFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            data = (List<MallItemInfo>)userData;
            if (data != null)
            {
                if (mLimitTimePetGiftFrame != null)
                    mLimitTimePetGiftFrame.InitData(data, OnBuyClick);
            }
            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldBuyPetSucceed, OnSyncWorldBuyPetSucceed);
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            data = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldBuyPetSucceed, OnSyncWorldBuyPetSucceed);
            MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(MallTypeTableId);
        }

        private void OnSyncWorldBuyPetSucceed(UIEvent uiEvent)
        {
            List<MallItemInfo> itemInfos = uiEvent.Param1 as List<MallItemInfo>;

            if (itemInfos != null)
            {
                if (mLimitTimePetGiftFrame != null)
                {
                    mLimitTimePetGiftFrame.RefreshItemInfo(itemInfos);
                }
            }
        }

        private void OnMallShopClick()
        {
            if (ClientSystemManager.instance.IsFrameOpen<MallNewFrame>())
            {
                ClientSystemManager.instance.CloseFrame<MallNewFrame>();
            }

            MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(MallTypeTableId);

            ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.LimitTimeMall });
            frameMgr.CloseFrame(this);
        }

        private void OnBuyClick(MallItemInfo info)
        {
            if (info == null)
            {
                return;
            }

            //如果上锁不能购买
            if ((info.moneytype == (int)ItemTable.eSubType.BindPOINT || info.moneytype == (int)ItemTable.eSubType.POINT) && SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType((ItemTable.eSubType)info.moneytype);
            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(costInfo.nMoneyID);
            if(itemTableData == null)
            {
                Logger.LogErrorFormat("道具id为{0}的货币", costInfo.nMoneyID);
                return;
            }
            string moneyName = itemTableData.Name;
            costInfo.nCount = (int)info.discountprice;
            SystemNotifyManager.SysNotifyMsgBoxCancelOk(string.Format(TR.Value("pet_limittime_buy_tips"), costInfo.nCount,moneyName), null, () =>
            {
                CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                {
                    WorldMallBuy req = new WorldMallBuy();

                    req.itemId = info.id;
                    req.num = 1;

                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                });
            });
        }
    }
}
