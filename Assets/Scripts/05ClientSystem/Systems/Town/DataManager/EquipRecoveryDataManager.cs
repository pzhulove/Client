using Protocol;
using System.Collections.Generic;
using ProtoTable;
using Network;

namespace GameClient
{
    public class EquipRecoveryDataManager : DataManager<EquipRecoveryDataManager>
    {
        bool m_bNetBind = false;
        public bool isUpgradeing = false;//现在是否在一键提升过程中
        public List<EqRecScoreItem> submitResult = new List<EqRecScoreItem>();//提交成功后每个装备获得的积分
        public List<int> jarKeyList = new List<int>();//储存奖励罐子打开分值

        private bool haveWeekRedPoint;//每周，由服务器告诉我，出现一次红点，
        public bool HaveWeekRedPoint
        {
            get;
            set;
        }
        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public sealed override void Initialize()
        {
            Clear();
            _BindNetMsg();
        }

        public sealed override void Clear()
        {
            _UnBindNetMsg();
            m_bNetBind = false;
        }

        void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {
                NetProcess.AddMsgHandler(SceneEquipRecSubcmtRes.MsgID, _OnSceneEquipRecSubcmt);
                //NetProcess.AddMsgHandler(SceneEquipRecRedeemRes.MsgID, _OnSceneEquipRecRedeem);
                NetProcess.AddMsgHandler(SceneEquipRecUpscoreRes.MsgID, _OnSceneEquipRecUpscore);
                NetProcess.AddMsgHandler(SceneEquipRecRedeemTmRes.MsgID, _OnSceneEquipRecRedeemTmRes);
                NetProcess.AddMsgHandler(SceneEquipRecNotifyReset.MsgID, _OnSceneEquipRecNotifyReset);
                m_bNetBind = true;
            }
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(SceneEquipRecSubcmtRes.MsgID, _OnSceneEquipRecSubcmt);
            //NetProcess.RemoveMsgHandler(SceneEquipRecRedeemRes.MsgID, _OnSceneEquipRecRedeem);
            NetProcess.RemoveMsgHandler(SceneEquipRecUpscoreRes.MsgID, _OnSceneEquipRecUpscore);
            NetProcess.RemoveMsgHandler(SceneEquipRecRedeemTmRes.MsgID, _OnSceneEquipRecRedeemTmRes);
            NetProcess.RemoveMsgHandler(SceneEquipRecNotifyReset.MsgID, _OnSceneEquipRecNotifyReset);
        }

        void _OnSceneEquipRecEvaRes(MsgDATA msg)
        {
            //SceneEquipRecEvaRes msgData = new SceneEquipRecEvaRes();
            //msgData.decode(msg.bytes);

            //if (msgData.code != (uint)ProtoErrorCode.SUCCESS)
            //{
            //    SystemNotifyManager.SystemNotify((int)msgData.code);
            //    return;
            //}

            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipRecoveryPriceReqSuccess, msgData.eqItemId, msgData.price);
        }
        /// <summary>
        /// 提交需要回收的列表
        /// </summary>
        /// <param name="submitList"></param>
        public void _SubmitEquip(List<ulong> submitList)
        {
            SceneEquipRecSubcmtReq req = new SceneEquipRecSubcmtReq();
            ulong[] tempItemUids = new ulong[submitList.Count];
            for (int i = 0; i < submitList.Count; i++)
            {
                tempItemUids[i] = submitList[i];
            }
            req.itemUids = tempItemUids;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        
        /// <summary>
        /// 提交请求赎回剩余时间
        /// </summary>
        public void _SendReturnTimeReq()
        {
            SceneEquipRecRedeemTmReq req = new SceneEquipRecRedeemTmReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 发送赎回请求
        /// </summary>
        /// <param name="uid"></param>
        public void _RedeemEquip(ulong uid)
        {
            SceneEquipRecRedeemReq req = new SceneEquipRecRedeemReq();
            req.equid = uid;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

            WaitNetMessageManager.GetInstance().Wait<SceneEquipRecRedeemRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                    return;
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipRedeemSuccess);

                EquipReturnResultItem resultItem = new EquipReturnResultItem();
                ItemData itemData = ItemDataManager.GetInstance().GetItem(uid);
                if(itemData != null)
                {
                    resultItem.itemdata = itemData;
                    resultItem.Score = (int)msgRet.consScore;
                    ClientSystemManager.GetInstance().OpenFrame<EquipReturnResultFrame>(FrameLayer.Middle, resultItem);
                }
            }, false);
        }

        /// <summary>
        /// 发送装备提升的请求
        /// </summary>
        /// <param name="uid"></param>
        public void _UpgradeEquip(ulong uid)
        {
            SceneEquipRecUpscoreReq req = new SceneEquipRecUpscoreReq();
            req.equid = uid;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        

        /// <summary>
        /// 接收回收结果
        /// </summary>
        /// <param name="msg"></param>
        void _OnSceneEquipRecSubcmt(MsgDATA msg)
        {
            SceneEquipRecSubcmtRes msgData = new SceneEquipRecSubcmtRes();
            msgData.decode(msg.bytes);
            if (msgData.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.code);
                return;
            }
            submitResult.Clear();
            for (int i = 0; i < msgData.items.Length; i++)
            {
                submitResult.Add(msgData.items[i]);
            }
            if (submitResult.Count != 0)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipSubmitSuccess,msgData.score, msgData.items);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.EquipRecovery);
            }
        }

        void _OnSceneEquipRecRedeem(MsgDATA msg)
        {
            //SceneEquipRecRedeemRes msgData = new SceneEquipRecRedeemRes();
            //msgData.decode(msg.bytes);
            //if (msgData.code != (uint)ProtoErrorCode.SUCCESS)
            //{
            //    SystemNotifyManager.SystemNotify((int)msgData.code);
            //    return;
            //}
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipRedeemSuccess);
        }

        /// <summary>
        /// 提升一次的返回
        /// </summary>
        /// <param name="msg"></param>
        void _OnSceneEquipRecUpscore(MsgDATA msg)
        {
            SceneEquipRecUpscoreRes msgData = new SceneEquipRecUpscoreRes();
            msgData.decode(msg.bytes);
            if (msgData.code != (uint)ProtoErrorCode.SUCCESS)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipUpgradeFail);
                SystemNotifyManager.SystemNotify((int)msgData.code);
                return;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipUpgradeSuccess, (int)msgData.upscore);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.EquipRecovery);
        }

        /// <summary>
        /// 接收返回剩余时间
        /// </summary>
        /// <param name="msg"></param>
        void _OnSceneEquipRecRedeemTmRes(MsgDATA msg)
        {
            SceneEquipRecRedeemTmRes msgData = new SceneEquipRecRedeemTmRes();
            msgData.decode(msg.bytes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipRecoveryUpdateTime, msgData.timestmap);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.EquipRecovery);
        }
        
        void _OnSceneEquipRecNotifyReset(MsgDATA msg)
        {
            HaveWeekRedPoint = true;
        }

        /// <summary>
        /// 根据id和数量请求开某个罐子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        public void _OpenRewardJar(int id, int count = 1)
        {
            var jarData = JarDataManager.GetInstance().GetJarData(id);
            JarBuyInfo jarBuyInfo = new JarBuyInfo();
            jarBuyInfo.nBuyCount = count;
            JarDataManager.GetInstance().RequestBuyJar(jarData, jarBuyInfo);
            //SceneUseMagicJarReq msg = new SceneUseMagicJarReq();
            //msg.type = (uint)id;
            //msg.combo = (byte)count;
            //NetManager netMgr = NetManager.Instance();
            //netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            
        }

        /// <summary>
        /// 在装备背包里寻找符合subType和品质颜色的道具
        /// </summary>
        /// <param name="subType"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public List<ulong> GetItemForType(int level, ProtoTable.ItemTable.eColor color,int minLevel)
        {
            List<ulong> resultList = new List<ulong>();
            var equipList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            resultList.AddRange(equipList);
            resultList.RemoveAll(x =>
            {
                ItemData data = ItemDataManager.GetInstance().GetItem(x);
                if (null != data)
                {
                    //限时装备去掉
                    if(data.DeadTimestamp != 0)
                    {
                        return true;
                    }
                    if(data.isInSidePack)
                    {
                        return true;
                    }
                    //等级小于最小等级去掉
                    if (data.LevelLimit < minLevel)
                    {
                        return true;
                    }
                    return needItem(data, level, color);
                }
                return true;
            });
            return resultList;
            //LayoutRebuilder.ForceRebuildLayoutImmediate(mDungeonScrollList.content);
        }

        /// <summary>
        /// 根据subType和品质返回是否为我们需要的道具true为不需要
        /// </summary>
        /// <param name="subType"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        bool needItem(ItemData data, int level, ItemTable.eColor color)
        {
            
            if (level != 0 && (int)color != 0)
            {
                return data.Quality != color || data.LevelLimit != level;
            }
            else if (level != 0 && (int)color == 0)
            {
                return data.Quality != ItemTable.eColor.BLUE && data.Quality != ItemTable.eColor.PINK && data.Quality != ItemTable.eColor.PURPLE || data.LevelLimit != level;
            }
            else if (level == 0 && (int)color != 0)
            {
                return data.Quality != color;
            }
            else
            {
                return data.Quality != ItemTable.eColor.BLUE && data.Quality != ItemTable.eColor.PINK && data.Quality != ItemTable.eColor.PURPLE;
            }
        }

        public void _ClearJarKeyList()
        {
            jarKeyList.Clear();
        }

        public void _AddJarKeyList(int key)
        {
            jarKeyList.Add(key);
        }

        public string _GetEquipPrice(ItemData itemData)
        {
            int tempLevel = itemData.LevelLimit;
            var color = itemData.Quality;
            string result = "";
            var tableData = TableManager.GetInstance().GetTableItem<EquipRecoveryPriceTable>(tempLevel);
            if (tableData != null)
            {
                if (color == ItemTable.eColor.BLUE)
                {
                    result = tableData.Blue;
                }
                if (color == ItemTable.eColor.PURPLE)
                {
                    result = tableData.Purple;
                }
                if (color == ItemTable.eColor.PINK)
                {
                    result = tableData.Pink;
                }
            }
            return result;
        }

        public int _GetEquipPrice(ItemData itemData, bool getMin)
        {
            if(itemData == null)
            {
                return 0;
            }
            int tempLevel = itemData.LevelLimit;
            var color = itemData.Quality;
            int result = 0;
            var tableData = TableManager.GetInstance().GetTableItem<EquipRecoveryPriceTable>(tempLevel);
            if (tableData != null)
            {
                string resultStr = "";
                if (color == ItemTable.eColor.BLUE)
                {
                    resultStr = tableData.Blue;
                }
                if (color == ItemTable.eColor.PURPLE)
                {
                    resultStr = tableData.Purple;
                }
                if (color == ItemTable.eColor.PINK)
                {
                    resultStr = tableData.Pink;
                }
                string[] str = resultStr.Split('-');
                if (str.Length != 2)
                {
                    return 0;
                }
                if (getMin)
                {
                    int.TryParse(str[0], out result);
                }
                else
                {
                    int.TryParse(str[1], out result);
                }
            }
            return result;
        }

        public RewardJarStatic _GetJarState(int index)
        {
            string tempKeyStr = "eqreco_jarstate_" + index;
            int rewardJarScore = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_REWARD_SCORE);
            int rewardJarStatic = CountDataManager.GetInstance().GetCount(tempKeyStr);
            RewardJarStatic jarStatic = RewardJarStatic.None;
            switch (rewardJarStatic)
            {
                case 0:
                    jarStatic = RewardJarStatic.UnOpen;
                    break;
                case 1:
                    jarStatic = RewardJarStatic.CanOpen;
                    break;
                case 2:
                    jarStatic = RewardJarStatic.HaveOpen;
                    break;
            }
            return jarStatic;
        }

        /// <summary>
        /// 请求罐子记录
        /// </summary>
        /// <param name="jarIDList"></param>
        public void RequestJarRecord(List<int> jarIDList)
        {
            WorldEquipRecoOpenJarsRecordReq msg = new WorldEquipRecoOpenJarsRecordReq();
            //uint[] tempJarIdList = new uint[jarIDList.Count];
            //for (int i=0;i<jarIDList.Count;i++)
            //{
            //    tempJarIdList[i] = (uint)jarIDList[i];
            //}
            //msg.jarIds = tempJarIdList;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldEquipRecoOpenJarsRecordRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipJarListUpdate, msgRet);
            }, false);
        }

        public bool HaveRedPoint()
        {
            var requipRecoveryRewardTableData = TableManager.GetInstance().GetTable<EquipRecoveryRewardTable>();
            var enumerator = requipRecoveryRewardTableData.GetEnumerator();
            int index = 1;
            bool haveRedPoint = false;
            while (enumerator.MoveNext())
            {
                var curStatic = EquipRecoveryDataManager.GetInstance()._GetJarState(index);
                if(curStatic == RewardJarStatic.CanOpen)
                {
                    haveRedPoint = true;
                    break;
                }
                index++;
            }
            return haveRedPoint || HaveWeekRedPoint;
        }
    }
}
