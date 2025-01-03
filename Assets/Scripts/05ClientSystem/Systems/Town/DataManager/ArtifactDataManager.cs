using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///////删除linq
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public class ArtifactJarDiscountData
    {
        /// <summary>
		///  运营活动id
		/// </summary>
		public int opActId;
        /// <summary>
        ///  抽取折扣状态(ArtifactJarDiscountExtractStatus)
        /// </summary>
        public ArtifactJarDiscountExtractStatus extractDiscountStatus;
        /// <summary>
        ///  折扣率
        /// </summary>
        public int discountRate;
        /// <summary>
        ///  折扣生效次数
        /// </summary>
        public int discountEffectTimes;
    }

    public class ArtifactDataManager : DataManager<ArtifactDataManager>
    {
        //罐子打折的数据
        ArtifactJarDiscountData artifactJarDiscountData = new ArtifactJarDiscountData();

        //所有神器罐子的数据
        private List<ArtifactJarBuy> artifactJarBuyData = new List<ArtifactJarBuy>();

        //private List<ArtifactJarLotteryRecord> artifactRecordData = new List<ArtifactJarLotteryRecord>();
        
        //所有抽奖的记录
        private Dictionary<int, List<ArtifactJarLotteryRecord>> artifactJarRecordDic = new Dictionary<int, List<ArtifactJarLotteryRecord>>();
        public bool isNotNotify
        {
            get;
            set;
        }
        public bool isArtifactRecordNew
        {
            get;
            set;
        }
        Dictionary<uint, List<ItemReward>> artifactJarLotteryRewardDic = new Dictionary<uint, List<ItemReward>>();
        public List<ItemReward> GetArtifactJarLotteryRewards(uint jarID)
        {
            if(artifactJarLotteryRewardDic == null)
            {
                return new List<ItemReward>();
            }
            if(artifactJarLotteryRewardDic.ContainsKey(jarID))
            {
                return artifactJarLotteryRewardDic[jarID];
            }
            return new List<ItemReward>();
        }
        public override void Initialize()
        {
            artifactJarRecordDic.Clear();
            artifactJarBuyData.Clear();
            artifactJarDiscountData = new ArtifactJarDiscountData();
            isNotNotify = false;
            isArtifactRecordNew = true;
            _RegisterNetHandler();
            artifactJarLotteryRewardDic = new Dictionary<uint, List<ItemReward>>();
        }
        
        public override void Clear()
        {
            _UnRegisterNetHandler();
            artifactJarLotteryRewardDic = null;
        }
        
        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }
        void _RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneArtifactJarBuyCntRes.MsgID, _OnSceneArtifactJarBuyCntRes);//活动神器罐购买次数返回。
            NetProcess.AddMsgHandler(GASArtifactJarLotteryRecordRes.MsgID, _OnGASArtifactJarLotteryRecordRes); //神器罐活动抽奖记录返回
            NetProcess.AddMsgHandler(SceneArtifactJarDiscountInfoSync.MsgID, _OnSceneArtifactJarDiscountInfoSync); //神器罐子折扣信息同步
            NetProcess.AddMsgHandler(SceneArtifactJarExtractDiscountRes.MsgID, _OnSceneArtifactJarExtractDiscountRes);// 神器罐子折扣抽取返回
            NetProcess.AddMsgHandler(GASArtifactJarLotteryNotify.MsgID, _OnGASArtifactJarLotteryNotify);//罐子记录刷新的消息
            NetProcess.AddMsgHandler(GASArtifactJarLotteryCfgRes.MsgID, _OnGASArtifactJarLotteryCfgRes);//罐子奖池预览消息

        }

        void _UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneArtifactJarBuyCntRes.MsgID, _OnSceneArtifactJarBuyCntRes);//活动神器罐购买次数返回。
            NetProcess.RemoveMsgHandler(GASArtifactJarLotteryRecordRes.MsgID, _OnGASArtifactJarLotteryRecordRes); //神器罐活动抽奖记录返回
            NetProcess.RemoveMsgHandler(SceneArtifactJarDiscountInfoSync.MsgID, _OnSceneArtifactJarDiscountInfoSync); //神器罐子折扣信息同步
            NetProcess.RemoveMsgHandler(SceneArtifactJarExtractDiscountRes.MsgID, _OnSceneArtifactJarExtractDiscountRes); //神器罐子折扣抽取返回
            NetProcess.RemoveMsgHandler(GASArtifactJarLotteryNotify.MsgID, _OnGASArtifactJarLotteryNotify);//罐子记录刷新的消息
            NetProcess.RemoveMsgHandler(GASArtifactJarLotteryCfgRes.MsgID, _OnGASArtifactJarLotteryCfgRes);//罐子奖池预览消息
        }
        
        #region 共有方法
        //获得所有派奖罐子的数据
        public List<ArtifactJarBuy> getArtiFactJarBuyData()
        {
            List<ArtifactJarBuy> tempArtiFactJarBuyData = new List<ArtifactJarBuy>();
            for (int i = 0;i<artifactJarBuyData.Count;i++)
            {
                tempArtiFactJarBuyData.Add(artifactJarBuyData[i]);
            }
            return tempArtiFactJarBuyData;
        }

        public List<ArtifactJarLotteryRecord> getArtifactRecord(int jarID)
        {
            var enumerator = artifactJarRecordDic.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current.Key == jarID)
                {
                    return enumerator.Current.Value;
                }
            }
            return null;
        }

        public Dictionary<int, List<ArtifactJarLotteryRecord>> getArtifactRecordDic()
        {
            return artifactJarRecordDic;
        }

        //获取神器罐子活动数据，可能为null
        public OpActivityData getArtifactJarActData()
        {
            var artifactAct = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_ARTIFACT_JAR);
            return artifactAct;
        }

        //获取神器派奖活动数据，可能为null
        public OpActivityData getArtifactAwardActData()
        {
            var artifactAct = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_JAR_DRAW_LOTTERY);
            return artifactAct;
        }

        public bool IsArtifactActivityOpen()
        {
            var artifactAct = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_ARTIFACT_JAR_SHOW);
            if(artifactAct != null && artifactAct.state == (int)OpActivityState.OAS_IN)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取神器罐子打折界面的数据
        /// </summary>
        /// <returns></returns>
        public ArtifactJarDiscountData getArtifactDiscountData()
        {
            return artifactJarDiscountData;
        }
        #endregion

        #region 发送协议
        /// <summary>
        /// 请求派奖罐子的协议
        /// </summary>
        public void SendArtifactJar()
        {
            SceneArtifactJarBuyCntReq req = new SceneArtifactJarBuyCntReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        /// <summary>
        /// 神器罐活动抽奖记录请求
        /// </summary>
        public void SendArtifactJarRecord(int jarId)
        {
            GASArtifactJarLotteryRecordReq req = new GASArtifactJarLotteryRecordReq();
            req.jarId = (uint)jarId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        ///// <summary>
        ///// 神器罐子折扣信息请求
        ///// </summary>
        //public void SendArtifactJarDiscount()
        //{
        //    SceneArtifactJarDiscountInfoReq req = new SceneArtifactJarDiscountInfoReq();
        //    OpActivityData data = getArtifactAwardActData();
        //    if (data != null)
        //    {
        //        req.opActId = data.dataId;
        //    }

        //    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        //}

        /// <summary>
        /// 抽取
        /// </summary>
        public void SendGetDiscount()
        {
            SceneArtifactJarExtractDiscountReq req = new SceneArtifactJarExtractDiscountReq();
            OpActivityData data = getArtifactJarActData();
            if (data.state == (int)OpActivityState.OAS_IN)
            {
                req.opActId = data.dataId;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }

        }

        // 请求神器罐奖池预览道具数据
        public void SendGASArtifactJarLotteryCfgReq()
        {
            GASArtifactJarLotteryCfgReq req = new GASArtifactJarLotteryCfgReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion

        #region 接受协议
        /// <summary>
        /// 活动神器罐购买次数返回
        /// </summary>
        /// <param name="msgData"></param>
        void _OnSceneArtifactJarBuyCntRes(MsgDATA msgData)
        {
            SceneArtifactJarBuyCntRes res = new SceneArtifactJarBuyCntRes();
            if(res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            artifactJarBuyData.Clear();
            for(int i = 0;i<res.artifactJarBuyList.Length;i++)
            {
                artifactJarBuyData.Add(res.artifactJarBuyList[i]);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ArtifactDailyRewardUpdate);
        }
        /// <summary>
        /// //神器罐活动抽奖记录返回
        /// </summary>
        /// <param name="msgData"></param>
        void _OnGASArtifactJarLotteryRecordRes(MsgDATA msgData)
        {
            GASArtifactJarLotteryRecordRes res = new GASArtifactJarLotteryRecordRes();
            if(res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            artifactJarRecordDic[(int)res.jarId] = res.lotteryRecordList.ToList();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ArtifactDailyRecordUpdate,(int)res.jarId);
        }

        void _OnSceneArtifactJarDiscountInfoSync(MsgDATA msgData)
        {
            SceneArtifactJarDiscountInfoSync res = new SceneArtifactJarDiscountInfoSync();
            if(res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            artifactJarDiscountData = new ArtifactJarDiscountData();
            artifactJarDiscountData.opActId = (int)res.opActId;
            artifactJarDiscountData.extractDiscountStatus = (ArtifactJarDiscountExtractStatus)res.extractDiscountStatus;
            artifactJarDiscountData.discountRate = (int)res.discountRate;
            artifactJarDiscountData.discountEffectTimes = (int)res.discountEffectTimes;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ArtifactJarDataUpdate);
        }

        void _OnSceneArtifactJarExtractDiscountRes(MsgDATA msgData)
        {
            SceneArtifactJarExtractDiscountRes res = new SceneArtifactJarExtractDiscountRes();
            if(res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            if (res.errorCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.errorCode);
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<ShowArtifactJarDiscountFrame>();
        }

        void _OnGASArtifactJarLotteryNotify(MsgDATA msgData)
        {
            GASArtifactJarLotteryNotify res = new GASArtifactJarLotteryNotify();
            if(res == null)
            {
                return; ;
            }
            isArtifactRecordNew = true;
        }
        void _OnGASArtifactJarLotteryCfgRes(MsgDATA msgData)
        {
            GASArtifactJarLotteryCfgRes res = new GASArtifactJarLotteryCfgRes();
            if (res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            if(artifactJarLotteryRewardDic != null && res.artifactJarCfgList != null)
            {
                artifactJarLotteryRewardDic.Clear();
                for(int i = 0;i < res.artifactJarCfgList.Length;i++)
                {
                    if(artifactJarLotteryRewardDic.ContainsKey(res.artifactJarCfgList[i].jarId))
                    {
                        if(res.artifactJarCfgList[i].rewardList != null)
                        {
                            artifactJarLotteryRewardDic[res.artifactJarCfgList[i].jarId] = res.artifactJarCfgList[i].rewardList.ToList();
                        }                        
                    }
                    else
                    {
                        if (res.artifactJarCfgList[i].rewardList != null)
                        {
                            artifactJarLotteryRewardDic.Add(res.artifactJarCfgList[i].jarId, res.artifactJarCfgList[i].rewardList.ToList());
                        }                            
                    }
                }
            }
        }
        #endregion
    }
}