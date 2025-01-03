using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///////删除linq
using Network;
using Protocol;
using ProtoTable;
using System;

namespace GameClient
{

    /// <summary>
    /// 黑市商人数据
    /// </summary>
    public class BlackMarketMerchantDataModel
    {
        /// <summary>
        /// 竞拍列表
        /// </summary>
        public List<BlackMarketAuctionInfo> mBlackMarketAuctionInfoList;

        /// <summary>
        /// 商人类型
        /// </summary>
        public BlackMarketType mBlackMarketType;
    }

    /// <summary>
    /// 黑市商人数据管理器
    /// </summary>
    public class BlackMarketMerchantDataManager : DataManager<BlackMarketMerchantDataManager>
    {
        BlackMarketMerchantDataModel mBlackMarketMerchantDataModel = new BlackMarketMerchantDataModel();

        //创建黑市商人NPC的城镇ID
        private int mSceneID = 0;

        private const float NpcPositionCoefficient = 10000.0f; //转化系数

        private static BlackMarketType blackMarketType;
        /// <summary>
        /// 黑市商人类型
        /// </summary>
        public static BlackMarketType BlackMarketType
        {
            get { return blackMarketType; }
            set { blackMarketType = value; }
        }

        private static float mBlackMarketMerchantBornPos;
        /// <summary>
        /// 黑市商人出生位置
        /// </summary>
        public static float BlackMarketMerchantBornPos
        {
            get { return mBlackMarketMerchantBornPos; }
            set { mBlackMarketMerchantBornPos = value; }
        }

        private static float mBlackMarketMerchantEndPos;
        /// <summary>
        /// 黑市商人移动终点位置
        /// </summary>
        public static float BlackMarketMerchantEndPos
        {
            get { return mBlackMarketMerchantEndPos; }
            set { mBlackMarketMerchantEndPos = value; }
        }

        private static float mBlackMarketMerchantRandomPlayerNextAnimationMinTime;
        /// <summary>
        /// 黑市商人随机播放下段动画最小时间
        /// </summary>
        public static float BlackMarketMerchantRandomPlayerNextAniamtionMinTime
        {
            get { return mBlackMarketMerchantRandomPlayerNextAnimationMinTime; }
            set { mBlackMarketMerchantRandomPlayerNextAnimationMinTime = value; }
        }

        private static float mBlackMarketMerchantRandomPlayerNextAnimationMaxTime;
        /// <summary>
        /// 黑市商人随机播放下段动画最大时间
        /// </summary>
        public static float BlackMarketMerchantRandomPlayerNextAniamtionMaxTime
        {
            get { return mBlackMarketMerchantRandomPlayerNextAnimationMaxTime; }
            set { mBlackMarketMerchantRandomPlayerNextAnimationMaxTime = value; }
        }

        public sealed override EEnterGameOrder GetOrder()
        {
            return base.GetOrder();
        }
      
        /// <summary>
        /// 得到黑市商人数据
        /// </summary>
        /// <returns></returns>
        public BlackMarketMerchantDataModel GetBlackMarketMerchantDataModel()
        {
            return mBlackMarketMerchantDataModel;
        }

        public sealed override void Initialize()
        {
            mBlackMarketMerchantDataModel = new BlackMarketMerchantDataModel();
            blackMarketType = BlackMarketType.BmtInvalid;
            int.TryParse(TR.Value("BlackMarketMerchantSceneID"), out mSceneID);

            InitLocalData();
            _RegisterNetHandler();
        }

        public sealed override void Clear()
        {
            _UnRegisterNetHandler();

            if (mBlackMarketMerchantDataModel != null)
            {
                if (mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList != null)
                {
                    mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList.Clear();
                }
                mBlackMarketMerchantDataModel = null;
            }
        }

        private void InitLocalData()
        {
            var system1 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLACKMARKET_BORN_POSITION);
            if (system1 != null)
            {
                mBlackMarketMerchantBornPos = system1.Value;
            }

            var system2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLACKMARKET_MOBILE_POSITION);
            if (system2 != null)
            {
                mBlackMarketMerchantEndPos = system2.Value / NpcPositionCoefficient;
            }

            var system3 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLACKMARKET_IDLE_MINTIME);
            if (system3 != null)
            {
                mBlackMarketMerchantRandomPlayerNextAnimationMinTime = system3.Value;
            }

            var system4 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLACKMARKET_IDLE_MAXTIME);
            if (system4 != null)
            {
                mBlackMarketMerchantRandomPlayerNextAnimationMaxTime = system4.Value;
            }
        }

        private void _RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldBlackMarketAuctionListRes.MsgID, OnSyncWorldBlackMarketAuctionListRes);
            NetProcess.AddMsgHandler(WorldBlackMarketSyncType.MsgID, OnSyncWorldBlackMarketSyncType);
            NetProcess.AddMsgHandler(WorldBlackMarketAuctionRes.MsgID, OnSyncWorldBlackMarketAuctionRes);
            NetProcess.AddMsgHandler(WorldBlackMarketNotifyRefresh.MsgID, OnSyncWorldBlackMarketNotifyRefresh);
            NetProcess.AddMsgHandler(WorldBlackMarketAuctionCancelRes.MsgID, OnSyncWorldBlackMarketAuctionCancelRes);
        }

        private void _UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldBlackMarketAuctionListRes.MsgID, OnSyncWorldBlackMarketAuctionListRes);
            NetProcess.RemoveMsgHandler(WorldBlackMarketSyncType.MsgID, OnSyncWorldBlackMarketSyncType);
            NetProcess.RemoveMsgHandler(WorldBlackMarketAuctionRes.MsgID, OnSyncWorldBlackMarketAuctionRes);
            NetProcess.RemoveMsgHandler(WorldBlackMarketNotifyRefresh.MsgID, OnSyncWorldBlackMarketNotifyRefresh);
            NetProcess.RemoveMsgHandler(WorldBlackMarketAuctionCancelRes.MsgID, OnSyncWorldBlackMarketAuctionCancelRes);
        }

        /// <summary>
        /// 请求拍卖列表返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncWorldBlackMarketAuctionListRes(MsgDATA msg)
        {
            var res = new WorldBlackMarketAuctionListRes();
            res.decode(msg.bytes);

            mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList = new List<BlackMarketAuctionInfo>();

            if (res.items.Length > 0)
            {
                for (int i = 0; i < res.items.Length; i++)
                {
                    var mItem = res.items[i];
                    mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList.Add(mItem);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BlackMarketMerchanRetSuccess);
        }

        /// <summary>
        /// 同步黑市商人类型
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncWorldBlackMarketSyncType(MsgDATA msg)
        {
            WorldBlackMarketSyncType res = new WorldBlackMarketSyncType();
            res.decode(msg.bytes);

            mBlackMarketMerchantDataModel.mBlackMarketType = (BlackMarketType)res.type;
            blackMarketType= (BlackMarketType)res.type;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SyncBlackMarketMerchantNPCType);
        }

        /// <summary>
        /// 竞拍返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncWorldBlackMarketAuctionRes(MsgDATA msg)
        {
            WorldBlackMarketAuctionRes res = new WorldBlackMarketAuctionRes();
            res.decode(msg.bytes);

            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
            }
            else
            {
                if (mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList != null)
                {
                    int index = 0;

                    for (int i = 0; i < mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList.Count; i++)
                    {
                        var mCurrent = mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList[i];
                        if (mCurrent.guid != res.item.guid)
                        {
                            continue;
                        }

                        index = i;
                    }

                    mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList[index] = res.item;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BlackMarketMerchantItemUpdate);
            }
        }

        /// <summary>
        /// 取消竞拍返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncWorldBlackMarketAuctionCancelRes(MsgDATA msg)
        {
            WorldBlackMarketAuctionCancelRes res = new WorldBlackMarketAuctionCancelRes();
            res.decode(msg.bytes);

            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
            }
            else
            {
                if (mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList != null)
                {
                    int index = 0;

                    for (int i = 0; i < mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList.Count; i++)
                    {
                        var mCurrent = mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList[i];
                        if (mCurrent.guid != res.item.guid )
                        {
                            continue;
                        }

                        index = i;
                    }

                    mBlackMarketMerchantDataModel.mBlackMarketAuctionInfoList[index] = res.item;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BlackMarketMerchantItemUpdate);
            }
        }

        /// <summary>
        /// 重新拉取黑市商人回购商品列表
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncWorldBlackMarketNotifyRefresh(MsgDATA msg)
        {
            WorldBlackMarketNotifyRefresh res = new WorldBlackMarketNotifyRefresh();
            res.decode(msg.bytes);

            OnSendWorldBlackMarketAuctionListReq();
        }

        /// <summary>
        /// 请求竞拍列表
        /// </summary>
        public void OnSendWorldBlackMarketAuctionListReq()
        {
            WorldBlackMarketAuctionListReq req = new WorldBlackMarketAuctionListReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 请求竞拍
        /// </summary>
        /// <param name="auction_guid">竞拍item项guid</param>
        /// <param name="item_guid">装备id</param>
        /// <param name="auction_price">竞拍价格</param>
        public void OnSendWorldBlackMarketAuctionReq(UInt64 auction_guid, UInt64 item_guid, UInt32 auction_price)
        {
            WorldBlackMarketAuctionReq req = new WorldBlackMarketAuctionReq();
            req.auction_guid = auction_guid;
            req.item_guid = item_guid;
            req.auction_price = auction_price;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 取消竞拍请求
        /// </summary>
        /// <param name="auction_guid">竞拍道具序号id</param>
        public void OnSendWorldBlackMarketAuctionCancelReq(UInt64 auction_guid)
        {
            WorldBlackMarketAuctionCancelReq req = new WorldBlackMarketAuctionCancelReq();
            req.auction_guid = auction_guid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 根据价格长度转换为问号
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string SwichQuestionMarkCharacter(int count)
        {
            string str = "";
            switch (count)
            {
                case 1:
                    str = "?";
                    break;
                case 2:
                    str = "??";
                    break;
                case 3:
                    str = "???";
                    break;
                case 4:
                    str = "????";
                    break;
                case 5:
                    str = "?????";
                    break;
                case 6:
                    str = "??????";
                    break;
                case 7:
                    str = "???????";
                    break;
                case 8:
                    str = "????????";
                    break;
                case 9:
                    str = "?????????";
                    break;
                case 10:
                    str = "??????????";
                    break;
            }

            return str;
        }

        /// <summary>
        /// 查找背包是否有回购道具
        /// </summary>
        /// <returns></returns>
        public bool FindBackPackBuyBackItem(int tableId)
        {
            bool isFind = false;
            var mItemList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            if (mItemList != null)
            {

                for (int i = 0; i < mItemList.Count; i++)
                {
                    var guid = mItemList[i];
                    var itemData = ItemDataManager.GetInstance().GetItem(guid);
                    if (itemData.TableID != tableId)
                    {
                        continue;
                    }

                    isFind = true;
                    break;
                }
            }

            var mMaterialItemList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
            if (mMaterialItemList != null)
            {
                for (int i = 0; i < mMaterialItemList.Count; i++)
                {
                    var guid = mMaterialItemList[i];
                    var itemData = ItemDataManager.GetInstance().GetItem(guid);
                    if (itemData.TableID != tableId)
                    {
                        continue;
                    }

                    isFind = true;
                    break;
                }
            }

            return isFind;
        }

        /// <summary>
        /// 得到背包中所有相同的道具
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public List<ItemData> GetBackPackAllItem(int tableId)
        {
            List<ItemData> mAllItem = new List<ItemData>();

            var mItemList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            if (mItemList != null)
            {
                for (int i = 0; i < mItemList.Count; i++)
                {
                    var guid = mItemList[i];
                    var itemData = ItemDataManager.GetInstance().GetItem(guid);
                    if (itemData.TableID != tableId)
                    {
                        continue;
                    }

                    mAllItem.Add(itemData);
                }
            }

            var mMaterialItemList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
            if (mMaterialItemList != null)
            {
                for (int i = 0; i < mMaterialItemList.Count; i++)
                {
                    var guid = mMaterialItemList[i];
                    var itemData = ItemDataManager.GetInstance().GetItem(guid);
                    if (itemData.TableID != tableId)
                    {
                        continue;
                    }

                    mAllItem.Add(itemData);
                }
            }

            return mAllItem;
        }

        /// <summary>
        /// 是否可以创建黑市商人NPC
        /// </summary>
        /// <param name="currentSceneID"></param>
        /// <returns></returns>
        public bool CreatBlackMarketMerchantNPC(int currentSceneID)
        {
            var mCitySceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(currentSceneID);
            if (mCitySceneTable == null)
            {
                return false;
            }

            if (mCitySceneTable.ID != mSceneID)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 将表中NPC位置转换为当前场景的位置坐标
        /// </summary>
        /// <param name="npcTable"></param>
        /// <returns></returns>
        public Vector3 GetBlackMarketMerchantPostion(NpcTable npcTable)
        {
            if (npcTable == null)
            {
                return Vector3.zero;
            }
            
            float y = -30000f;
            
            Vector3 vct = new Vector3((float)BlackMarketMerchantBornPos / NpcPositionCoefficient, 0, (float)y / NpcPositionCoefficient);

            return vct;
        }

        public static void OnClickLinkByNpcId(string param)
        {
            var mBlackMarketMerchantTable = TableManager.GetInstance().GetTableItem<BlackMarketTable>((int)BlackMarketType);
            if (mBlackMarketMerchantTable == null)
            {
                return;
            }
            Parser.NpcParser.OnClickLinkByNpcId(mBlackMarketMerchantTable.NpcID);
        }
    }
}

