using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Protocol;
using System.Collections;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    // 冒险者通行证数据管理器
    class AdventurerPassCardDataManager : DataManager<AdventurerPassCardDataManager>
    {
        #region inner def

        public enum PassCardType
        {
            Normal,         // 普通
            King,           // 王者 
            HighKing,       // 高级王者
        }

        public class ItemInfo
        {
            public int itemID;
            public int itemNum;
            public bool highValue;
        }

        public class AwardItemInfo
        {
            public int lv;
            public List<ItemInfo> normalAwards = new List<ItemInfo>();
            public List<ItemInfo> highAwards = new List<ItemInfo>();
            public int exp;
        }

        public enum ExpPackState
        {
            Lock, // 未解锁
            CanGet, // 可以领取
            Got, // 已经领取
        }

        public enum RedPointShowType
        {
            UnLockPassCard,                 // 通行证解锁
            SeasonIDOnLineUpdate,           // 赛季在线开启
            SeasonInTheOpenWhenLogin,       // 赛季进行中玩家进行登录            
            CanGetExpPack,                  // 可以领取经验包
            CardLvUp,                       // 通行证等级提升    
            CanGetAward,                    // 可以获取奖励
        }

        #endregion

        #region val

        string normalReward;
        string highReward;

        int activeToday = 0;
        int seasonStartTime = 0;
        int seasonEndTime = 0;

        ExpPackState expPackState = ExpPackState.Lock;

        // 奖励数据 seasonID => lv => AwardItemInfo
        Dictionary<int, Dictionary<int, AwardItemInfo>> seasonID2LevelAward = new Dictionary<int, Dictionary<int, AwardItemInfo>>();      

        uint cardLv = 0;
        public uint CardLv
        {  
            get { return cardLv; }
        }
        uint cardExp = 0;
        public uint CardExp
        {  
            get { return cardExp; }
        }
        uint seasonID = 0;
        public uint SeasonID
        {
            get { return seasonID; }
        }
        PassCardType passCardType = PassCardType.Normal;
        public PassCardType GetPassCardType
        {
            get { return passCardType; }
        }

        public int KingCardItemID
        {
            get;
            private set;
        }
        public int KingCardItemPrice
        {
            get;
            private set;
        }
        const int ChargeTableID = 14; // ChargeMallTable表中的id 给购买高级通行证使用
        #endregion

        #region override
        public override void Initialize()
        {
            Clear();
            _BindNetMsg();

            cardLv = 0;
            cardExp = 0;
            seasonID = 0;
            passCardType = PassCardType.Normal;
            normalReward = "";
            highReward = "";
            activeToday = 0;
            seasonStartTime = 0;
            seasonEndTime = 0;
            expPackState = ExpPackState.Lock;
            KingCardItemID = 0;
            KingCardItemPrice = 0;

            seasonID2LevelAward = null;
            {
                seasonID2LevelAward = new Dictionary<int, Dictionary<int, AwardItemInfo>>();
                if(seasonID2LevelAward != null)
                {
                    Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassRewardTable>();
                    if (dicts != null)
                    {
                        Func<string, List<ItemInfo>> GetAwardItems = (awardText) => 
                        {
                            if(awardText == null)
                            {
                                return null;
                            }

                            List<ItemInfo> itemInfos = new List<ItemInfo>();
                            if(itemInfos == null)
                            {
                                return null;
                            }

                            string[] items = awardText.Split(new char[] { ',' });
                            for (int j = 0; j < items.Length; j++)
                            {
                                string[] contents = items[j].Split(new char[] { '_' });
                                if (contents.Length >= 3)
                                {
                                    int id = 0;
                                    int.TryParse(contents[0], out id);

                                    int num = 0;
                                    int.TryParse(contents[1], out num);

                                    int highValue = 0;
                                    int.TryParse(contents[2], out highValue);

                                    ItemInfo itemInfo = new ItemInfo();
                                    if(itemInfo != null)
                                    {
                                        itemInfo.itemID = id;
                                        itemInfo.itemNum = num;
                                        itemInfo.highValue = highValue > 0;

                                        itemInfos.Add(itemInfo);
                                    }
                                }
                            }

                            return itemInfos;
                        };

                        var iter = dicts.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            AdventurePassRewardTable adt = iter.Current.Value as AdventurePassRewardTable;
                            if (adt == null)
                            {
                                continue;
                            }

                            if(!seasonID2LevelAward.ContainsKey(adt.Season))
                            {
                                seasonID2LevelAward.Add(adt.Season, new Dictionary<int, AwardItemInfo>());                                
                            }

                            Dictionary<int, AwardItemInfo> keyValuePairs = seasonID2LevelAward[adt.Season];
                            if (keyValuePairs == null)
                            {
                                continue;                               
                            }

                            if (!keyValuePairs.ContainsKey(adt.Lv))
                            {
                                keyValuePairs.Add(adt.Lv, new AwardItemInfo());
                            }

                            AwardItemInfo awardItemInfo = keyValuePairs[adt.Lv];
                            if(awardItemInfo == null)
                            {
                                continue;
                            }

                            awardItemInfo.lv = adt.Lv;
                            awardItemInfo.normalAwards = GetAwardItems(adt.Normal);
                            awardItemInfo.highAwards = GetAwardItems(adt.High);
                            awardItemInfo.exp = adt.Exp;
                        }
                    }
                }
                
            }
            var table = TableManager.GetInstance().GetTableItem<ChargeMallTable>(ChargeTableID);
            if(table != null)
            {
                KingCardItemID = table.ID;
                KingCardItemPrice = table.ChargeMoney;
            }
        }
        

        public override void Clear()
        {
            _UnBindNetMsg();

            cardLv = 0;
            cardExp = 0;
            seasonID = 0;
            passCardType = PassCardType.Normal;
            normalReward = "";
            highReward = "";
            activeToday = 0;
            seasonStartTime = 0;
            seasonEndTime = 0;
            expPackState = ExpPackState.Lock;
            KingCardItemID = 0;
            KingCardItemPrice = 0;

            seasonID2LevelAward = null;
        }
        #endregion

        #region method
        //获取赛季最大等级
        public int GetAdventurerPassCardMaxLv(uint seasonID = 0)
        {
            if (0 == seasonID)
                seasonID = this.seasonID;
            Dictionary<int,AwardItemInfo> awards = GetAdventurePassAwardsBySeasonID(seasonID);
            if(awards != null)
            {
                return awards.Count;
            }

            return 0;
        }

        //获取赛季奖励列表
        public Dictionary<int,AwardItemInfo> GetAdventurePassAwardsBySeasonID(uint seasonID)
        {
            if (seasonID2LevelAward == null)
            {
                return null;
            }

            if (seasonID2LevelAward.ContainsKey((int)seasonID))
            {
                return seasonID2LevelAward[(int)seasonID];
            }

            return null;
        }
        
        //获取赛季某等级的奖励
        public AwardItemInfo GetAwardItemInfo(uint seasonID,int lv)
        {
            Dictionary<int, AwardItemInfo> keyValuePairs = GetAdventurePassAwardsBySeasonID(seasonID);
            if(keyValuePairs == null)
            {
                return null;
            }

            if(keyValuePairs.ContainsKey(lv))
            {
                return keyValuePairs[lv];
            }

            return null;
        }

        // 某个等级的普通奖励是否领取了
        public bool IsNormalAwardReceived(int  lv)
        {
            int maxLv = GetAdventurerPassCardMaxLv(seasonID);
            if(lv <= 0 || lv > maxLv)
            {
                return false;
            }
            
            if(lv > normalReward.Length)
            {
                return false;
            }

            // 这里做个处理,如果某个等级没有奖励道具则认为已经领取过了
            AwardItemInfo awardItemInfo = GetAwardItemInfo(seasonID, lv);
            if(awardItemInfo != null && awardItemInfo.normalAwards.Count == 0)
            {
                return true;
            }

            return normalReward[lv] == '0';
        }

        // 某个等级的高级奖励是否领取了
        public bool IsHighAwardReceived(int lv)
        {
            int maxLv = GetAdventurerPassCardMaxLv(seasonID);
            if (lv <= 0 || lv > maxLv)
            {
                return false;
            }

            if (lv > highReward.Length)
            {
                return false;
            }

            return highReward[lv] == '0';
        }

        // 获取升级到下一级需要的exp
        public int GetNeedExpToNextLv (int lv)
        {
            AwardItemInfo awardItemInfo = GetAwardItemInfo(seasonID, lv);
            if(awardItemInfo == null)
            {
                return 0;
            }

            if(lv >= GetAdventurerPassCardMaxLv(seasonID))
            {
                return 0;
            }

            return awardItemInfo.exp;
        }

        //获取某等级段的所有奖励（包含高级奖励）
        public List<ItemInfo> GetSeasonLvRangeAwardItems(int minLv, int maxLv, uint seasonid = 0)
        {
            if (0 == seasonid)
                seasonid = SeasonID;
            var dic = GetAdventurePassAwardsBySeasonID(seasonid);
            if (null == dic)
            {
                Logger.LogErrorFormat("赛季id={0}有误 无法获取到奖品", seasonid);
                return null;
            }
            var dicItems = new Dictionary<int, ItemInfo>();
            for (int lv = minLv; lv <= maxLv; ++lv)
            {
                if (dic.ContainsKey(lv))
                {
                    var info = dic[lv];
                    foreach (var item in info.normalAwards)
                    {
                        if (dicItems.ContainsKey(item.itemID))
                        {
                            dicItems[item.itemID].itemNum += item.itemNum;
                        }
                        else
                        {
                            dicItems.Add(item.itemID, new ItemInfo(){itemID = item.itemID, itemNum = item.itemNum});
                        }
                    }
                    foreach (var item in info.highAwards)
                    {
                        if (dicItems.ContainsKey(item.itemID))
                        {
                            dicItems[item.itemID].itemNum += item.itemNum;
                        }
                        else
                        {
                            dicItems.Add(item.itemID, new ItemInfo(){itemID = item.itemID, itemNum = item.itemNum});
                        }
                    }
                }
            }
            return dicItems.Values.ToList();
        }

        //是否是用rmb购买王者通行证 true为rmb false为点券
        public bool IsBuyKingCardUseRMB(int seasonID)
        {
            Dictionary<int, object> dicts = TableManager.instance.GetTable<AdventurePassBuyRewardTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    AdventurePassBuyRewardTable adt = iter.Current.Value as AdventurePassBuyRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    if (adt.Season == seasonID)
                    {
                        return adt.buyType == 1;                     
                    }
                }
            }
            return false;
        }   
        //用rmb购买王者通行证
        public void SendBuyKingCardUseRmb()
        {         
            PayManager.GetInstance().DoPay(KingCardItemID, KingCardItemPrice, ChargeMallType.AdventurePassKing);
        }
        public int GetTodayActive()
        {
            return activeToday;
        }

        //赛季开始时间
        public int GetSeasonStartTime()
        {
            return seasonStartTime;
        }

        //赛季结束时间
        public int GetSeasonEndTime()
        {
            return seasonEndTime;
        }

        //经验包状态
        public ExpPackState GetExpPackState()
        {
            return expPackState;
        }

        //获取赛季剩余时间
        public int GetSeasonLeftTime()
        {
            int time = seasonEndTime - (int)GameClient.TimeManager.GetInstance().GetServerTime();
            return time;// / 86400;
        }

        // 高级通行证奖励会因为合服的原因导致会有多份，这个函数可以获取
        public int GetCanGetHighAwardItemCopies(int lv)
        {
            int maxLv = GetAdventurerPassCardMaxLv(seasonID);
            if (lv <= 0 || lv > maxLv)
            {
                return 0;
            }

            if (lv > highReward.Length)
            {
                return 0;
            }

            return Utility.ToInt(highReward[lv].ToString());
        }

        public static bool GetShowFlag(RedPointShowType redPointShowType)
        {
            if(!PlayerPrefs.HasKey(redPointShowType.ToString()))
            {
                return false;
            }

            DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)TimeManager.GetInstance().GetServerTime());
            string str = UnityEngine.PlayerPrefs.GetString(redPointShowType.ToString());
            if(string.IsNullOrEmpty(str))
            {
                return false;
            }

            string[] contents = str.Split('|');
            if(contents.Length == 2)
            {
                if(contents[0] == string.Format("{0}-{1}-{2}",dateTime.Year,dateTime.Month,dateTime.Day))
                {
                    return Utility.ToInt(contents[1]) > 0;
                }
            }
            return false;
        }

        public static void SetShowFlag(RedPointShowType redPointShowType,bool value)
        {
            DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)TimeManager.GetInstance().GetServerTime());
            PlayerPrefs.SetString(redPointShowType.ToString(), string.Format("{0}-{1}-{2}", dateTime.Year, dateTime.Month, dateTime.Day) + "|" + value.ToString());
            PlayerPrefs.Save();

            return;
        }

        public static void ClearShowFlag(RedPointShowType redPointShowType)
        {
            PlayerPrefs.DeleteKey(redPointShowType.ToString());
            PlayerPrefs.Save();

            return;
        }

        #endregion

        #region bind net msg
        void _BindNetMsg()
        {
            NetProcess.AddMsgHandler(WorldAventurePassStatusRet.MsgID, _OnWorldAventurePassStatusRet);
            NetProcess.AddMsgHandler(WorldAventurePassBuyRet.MsgID, _OnWorldAventurePassBuyRet);
            NetProcess.AddMsgHandler(WorldAventurePassBuyLvRet.MsgID, _OnWorldAventurePassBuyLvRet);
            NetProcess.AddMsgHandler(WorldAventurePassRewardRet.MsgID, _OnWorldAventurePassRewardRet);
            NetProcess.AddMsgHandler(WorldAventurePassExpPackRet.MsgID, _OnWorldAventurePassExpPackRet);

            //充值成功发货通知 监听
            NetProcess.AddMsgHandler(SceneBillingSendGoodsNotify.MsgID, OnSyncPayRes);
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldAventurePassStatusRet.MsgID, _OnWorldAventurePassStatusRet);
            NetProcess.RemoveMsgHandler(WorldAventurePassBuyRet.MsgID, _OnWorldAventurePassBuyRet);
            NetProcess.RemoveMsgHandler(WorldAventurePassBuyLvRet.MsgID, _OnWorldAventurePassBuyLvRet);
            NetProcess.RemoveMsgHandler(WorldAventurePassRewardRet.MsgID, _OnWorldAventurePassRewardRet);
            NetProcess.RemoveMsgHandler(WorldAventurePassExpPackRet.MsgID, _OnWorldAventurePassExpPackRet);

            //充值成功发货通知 监听
            NetProcess.RemoveMsgHandler(SceneBillingSendGoodsNotify.MsgID, OnSyncPayRes);
        }
        #endregion

        #region net msg proc
        void _OnWorldAventurePassStatusRet(MsgDATA a_data)
        {
            WorldAventurePassStatusRet msgData = new WorldAventurePassStatusRet();
            msgData.decode(a_data.bytes);

            uint oldLv = cardLv;
            uint oldSeasonID = seasonID;

            cardLv = msgData.lv;
            cardExp = msgData.exp;
            seasonID = msgData.seasonID;
            var oldType = passCardType;
            passCardType = (PassCardType)msgData.type;
            normalReward = msgData.normalReward;
            highReward = msgData.highReward;
            activeToday = (int)msgData.activity;      
            seasonStartTime = (int)msgData.startTime;
            seasonEndTime = (int)msgData.endTime;

            //解锁高级版
            if (oldType != passCardType && passCardType != PassCardType.Normal)
            {
                //通知通行证界面
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AdventureUnlockKing);
            }

            if(AdventurerPassCardFrame.CanOneKeyGetAwards() || expPackState == ExpPackState.CanGet)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassButtonRedPoint, true);
            }
            else
            {       
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassButtonRedPoint, false);
            }

            uint newLv = cardLv;
            uint newSeasonID = seasonID;

            if(oldLv > 0 && newLv > oldLv) // 等级提升
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassButtonRedPoint,true);
            }

            if(newSeasonID != oldSeasonID && newSeasonID > 0) // 赛季更新
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassButtonRedPoint, true);
            }

            if (AdsPush.LoginPushManager.GetInstance().IsFirstLogin())
            {
                DeadLineReminderDataManager.GetInstance().CheckCurrencyDeadlineStatus();
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassStatus);

            if(cardLv == 0) // 赛季重置了
            {
                ClientSystemManager.GetInstance().CloseFrame<AdventurerPassCardFrame>();           
            }            
        }

        void _OnWorldAventurePassBuyRet(MsgDATA a_data)
        {
            WorldAventurePassBuyRet msgData = new WorldAventurePassBuyRet();
            msgData.decode(a_data.bytes);

            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_buy_high_card_success"));

            SendWorldAventurePassStatusReq();
            SendWorldAventurePassExpPackReq(0);
        }

        void _OnWorldAventurePassBuyLvRet(MsgDATA a_data)
        {
            WorldAventurePassBuyLvRet msgData = new WorldAventurePassBuyLvRet();
            msgData.decode(a_data.bytes);

            if(msgData.lv > 0)
            {
                long nToLv = Math.Min(msgData.lv + AdventurerPassCardDataManager.GetInstance().CardLv, AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID));
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_buy_lv_up_tip", nToLv));
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BuyAdventureLevelSucc);
            }

            SendWorldAventurePassStatusReq();
        }

        void _OnWorldAventurePassRewardRet(MsgDATA a_data)
        {
            WorldAventurePassRewardRet msgData = new WorldAventurePassRewardRet();
            msgData.decode(a_data.bytes);

            SendWorldAventurePassStatusReq();
        }

        void _OnWorldAventurePassExpPackRet(MsgDATA a_data)
        {
            WorldAventurePassExpPackRet msgData = new WorldAventurePassExpPackRet();
            msgData.decode(a_data.bytes);

            expPackState = (ExpPackState)msgData.type;

            SendWorldAventurePassStatusReq();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassExpPackStatus);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassButtonRedPoint, null);
        }

        //商城支付回调 刷新支付界面
        void OnSyncPayRes(MsgDATA data)
        {
            if (SDKInterface.Instance.IsPayResultNotify())
            {
                UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnPayResultNotify, "0");
            }
            SendWorldAventurePassStatusReq();
            SendWorldAventurePassExpPackReq(0);
        }
        #endregion

        #region send msg
        public void SendWorldAventurePassStatusReq()
        {
            WorldAventurePassStatusReq req = new WorldAventurePassStatusReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendWorldAventurePassBuyReq(byte buyType)
        {
            WorldAventurePassBuyReq req = new WorldAventurePassBuyReq();
            req.type = buyType;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendWorldAventurePassBuyLvReq(uint lv)
        {
            WorldAventurePassBuyLvReq req = new WorldAventurePassBuyLvReq();
            req.lv = lv;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendWorldAventurePassRewardReq(uint lv)
        {
            WorldAventurePassRewardReq req = new WorldAventurePassRewardReq();
            req.lv = lv;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendWorldAventurePassExpPackReq(byte opType)
        {
            WorldAventurePassExpPackReq req = new WorldAventurePassExpPackReq();
            req.op = opType;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion

    }
}
