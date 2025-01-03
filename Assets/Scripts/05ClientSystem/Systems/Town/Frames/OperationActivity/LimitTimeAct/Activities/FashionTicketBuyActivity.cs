using Network;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class FashionTicketBuyActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/FashionTicketBuyActivity";
        }

        public override void Show(Transform root)
        {
            if (mDataModel == null)
            {
                return;
            }

            if (mIsInit)
            {
                mGameObject.CustomActive(true);

                if (mView != null)
                {
                    mView.Show();
                }
            }
            else
            {
                if (this.mGameObject == null)
                {
                    this.mGameObject = AssetLoader.instance.LoadResAsGameObject(_GetPrefabPath());
                }

                if (this.mGameObject != null)
                {
                    this.mGameObject.transform.SetParent(root, false);
                    this.mGameObject.CustomActive(true);
                }
                else
                {
                    Logger.LogError("加载活动预制体失败，路径:" + _GetPrefabPath());
                    return;
                }

                mView = mGameObject.GetComponent<IActivityView>();

                if (mView != null)
                {
                    var tempView = mView as FashionTicketBuyActivityView;
                    if(tempView != null)
                    {
                        tempView.SetBuyCallBack(BuyFashion);
                    }
                }
                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick);
                    this.mIsInit = true;
                }
            }
            
        }

        void BuyFashion(uint mallItemTableID)
        {
            if (mDataModel.State == OpActivityState.OAS_PREPARE)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_havenot_open_tips"));
                return;
            }
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
            var mallItemTableData = TableManager.GetInstance().GetTableItem<MallItemTable>((int)mallItemTableID);
            if(mallItemTableData == null)
            {
                return;
            }
            ItemTable.eSubType moneyType = (ItemTable.eSubType)mallItemTableData.moneytype;
            if (moneyType == ItemTable.eSubType.BindPOINT)
            {
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT);
            }
            else if (moneyType == ItemTable.eSubType.POINT)
            {
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            }
            //string notifyCont = string.Format("是否确定花费{0}点券购买该礼包？", mallItemTableData.price);
            //SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
            //{
            //    costInfo.nCount = (mallItemTableData.price); CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            //    {
            //        ActivityDataManager.GetInstance().BuyGift(mallItemTableID);
            //    });
            //});

            //WorldGetMallItemByItemIdReq msg = new WorldGetMallItemByItemIdReq();
            //msg.itemId = (uint)mallItemTableData.itemid;
            //NetManager netMgr = NetManager.Instance();
            //netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            //WaitNetMessageManager.GetInstance().Wait<WorldGetMallItemByItemIdRes>(msgRet =>
            //{
            //    var mallItemInfo = msgRet.mallItem;
            //    ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
            //}, false);
            ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, ActivityDataManager.GetInstance().GetGiftPackData(ProtoTable.MallTypeTable.eMallType.SN_GIFT, mallItemTableID));
        }
    }
}