using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;


namespace GameClient
{


    //吃鸡商店数据管理器
    public class ChijiShopDataManager : DataManager<ChijiShopDataManager>
    {

        public const string GloryCoinCounterStr = "chi_ji_shop_coin";

        public const int GloryCoinId = 402000005;           //荣耀币对应的道具Id;

        public int ChijiShopRefreshTimeStamp = 0;       //下次刷新的时间戳
        public int ChijiShopRefreshCostValue = 0;       //刷新消耗的数值
        public int ChijiShopId = 0;                     //由服务器下发

        public List<int> ChijiShopItemIdList = new List<int>();       //商店中商品的列表
        public List<int> ChijiAlreadyBuyShopItemIdList = new List<int>(); //商店中，已经购买的商品列表

        #region Initialize
        public override void Initialize()
        {
            BindNetEvents();
        }

        public override void Clear()
        {
            UnBindNetEvents();
            ResetChijiShopData();
        }
        
        public void ResetChijiShopData()
        {
            ChijiShopItemIdList.Clear();
            ChijiAlreadyBuyShopItemIdList.Clear();
            ChijiShopRefreshTimeStamp = 0;
            ChijiShopRefreshCostValue = 0;
            ChijiShopId = 0;
        }

        private void BindNetEvents()
        {
            //请求商店商品
            NetProcess.AddMsgHandler(SceneShopQueryRet.MsgID, OnReceiveSceneShopQueryRet);

            //商店商品数据同步
            NetProcess.AddMsgHandler(SceneShopSync.MsgID, OnReceiveSceneShopSync);

            //商店商品刷新
            NetProcess.AddMsgHandler(SceneShopRefreshRet.MsgID, OnReceiveSceneShopRefreshRet);

            //商品购买返回
            NetProcess.AddMsgHandler(SceneShopBuyRet.MsgID, OnReceiveSceneShopBuyRet);
           
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneShopQueryRet.MsgID, OnReceiveSceneShopQueryRet);

            NetProcess.RemoveMsgHandler(SceneShopSync.MsgID, OnReceiveSceneShopSync);

            NetProcess.RemoveMsgHandler(SceneShopRefreshRet.MsgID, OnReceiveSceneShopRefreshRet);

            NetProcess.RemoveMsgHandler(SceneShopBuyRet.MsgID, OnReceiveSceneShopBuyRet);
        }
        #endregion

        #region ShopItem

        //自动刷新不需要商店Id
        public void OnSendSceneShopQueryReq(int shopId = 0)
        {
            SceneShopQuery sceneShopQuery = new SceneShopQuery();
            sceneShopQuery.shopId = (byte)shopId;
            sceneShopQuery.cache = (byte)0;

            NetManager netMgr = NetManager.Instance();
            if(netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneShopQuery);
        }

        private void OnReceiveSceneShopQueryRet(MsgDATA msgData)
        {
            if (CommonUtility.IsInGameBattleScene() == false)
                return;

            if (msgData == null)
                return;

            SceneShopQueryRet sceneShopQueryRet = new SceneShopQueryRet();
            sceneShopQueryRet.decode(msgData.bytes);

            if (sceneShopQueryRet.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) sceneShopQueryRet.code);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveSceneShopQuerySucceed);

        }


        //商店刷新
        public void OnSendSceneShopRefreshReq(int shopId)
        {

            SceneShopRefresh msg = new SceneShopRefresh();
            msg.shopId = (byte) shopId;

            NetManager netMgr = NetManager.Instance();
            if(netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        private void OnReceiveSceneShopRefreshRet(MsgDATA msgData)
        {
            if (CommonUtility.IsInGameBattleScene() == false)
                return ;

            if (msgData == null)
                return;

            SceneShopRefreshRet sceneShopRefreshRet = new SceneShopRefreshRet();
            sceneShopRefreshRet.decode(msgData.bytes);

            if (sceneShopRefreshRet.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)sceneShopRefreshRet.code);
            }
            else
            {
                //SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Chiji_Shop_RefreshSucceed"));

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveSceneShopRefreshSucceed);
                
            }
        }

        private void OnReceiveSceneShopSync(MsgDATA msg)
        {
            if (CommonUtility.IsInGameBattleScene() == false)
                return;

            if (msg == null)
                return;

            CustomDecoder.ProtoShop msgRet;
            var pos = 0;

            if (CustomDecoder.DecodeShop(out msgRet, msg.bytes, ref pos, msg.bytes.Length) == false)
            {
                Logger.LogErrorFormat("Open ShopNewFrame OnSyncShopItem Decode is error");
                return;
            }

            if (msgRet == null)
            {
                Logger.LogErrorFormat("Open ShopNewFrame OnSyncShopItem Decode msgRet is null");
                return;
            }

            ChijiShopId = (int) msgRet.shopID;
            ChijiShopRefreshCostValue = (int) msgRet.refreshCost;
            ChijiShopRefreshTimeStamp = (int) msgRet.restRefreshTime;

            ChijiShopItemIdList.Clear();
            ChijiAlreadyBuyShopItemIdList.Clear();
            
            for (var i = 0; i < msgRet.shopItemList.Count; i++)
            {
                var protoShopItem = msgRet.shopItemList[i];

                var shopItemId = (int) protoShopItem.shopItemId;
                var chijiShopItemTable = TableManager.GetInstance().GetTableItem<ChiJiShopItemTable>(
                    shopItemId);

                if(chijiShopItemTable == null)
                    continue;

                ChijiShopItemIdList.Add(shopItemId);
            }
        }

        #endregion

        #region BuyGoods

        public void OnSendBuyShopItemReq(int shopId, int shopItemId)
        {
            var sceneShopBuy = new SceneShopBuy();
            sceneShopBuy.shopId = (byte) shopId;
            sceneShopBuy.shopItemId = (uint) shopItemId;
            sceneShopBuy.num = 1;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneShopBuy);
        }

        private void OnReceiveSceneShopBuyRet(MsgDATA msgData)
        {
            //非吃鸡场景
            if (CommonUtility.IsInGameBattleScene() == false)
                return;

            if (msgData == null)
                return;

            SceneShopBuyRet sceneShopBuyRet = new SceneShopBuyRet();
            sceneShopBuyRet.decode(msgData.bytes);

            if (sceneShopBuyRet.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) sceneShopBuyRet.code);
            }
            else
            {
                ////商品购买成功
                //SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Chiji_Shop_Item_Buy_Succeed"));

                ChijiAlreadyBuyShopItemIdList.Add((int) sceneShopBuyRet.shopItemId);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveSceneShopItemBuySucceed,
                    (int)sceneShopBuyRet.shopItemId);
            }
        }

        #endregion
        
    }
}
