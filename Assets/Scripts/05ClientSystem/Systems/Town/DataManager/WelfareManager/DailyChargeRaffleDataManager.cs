using Network;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class DailyChargeCounter
    {
        public uint dailyChargeActivityId;
        public AcivtityCounterRes activityCounter;

        public DailyChargeCounter(uint activityId, uint counterId, uint counterValue)
        {
            this.dailyChargeActivityId = activityId;
            activityCounter = new AcivtityCounterRes(counterId, counterValue);
        }
    }

    /// <summary>
    /// 每日充值礼包 跟随福利界面开启而开启
    /// </summary>
    public class DailyChargeRaffleDataManager : DataManager<DailyChargeRaffleDataManager>
    {
        #region Model Params

        public const int ACTIVITY_CONFIG_ID = 9380;         //活动配置ID
        public const int ACTIVITY_TEMPLATE_ID = 8700;       //活动ID (即活动模版ID)

        private bool mFirstRedPointFlag = true;
        public bool FirstRedPointFlag
        {
            get { return mFirstRedPointFlag; }
            set {
                if (mFirstRedPointFlag != value)
                {
                    mFirstRedPointFlag = value;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DailyChargeRedPointChanged);
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);
            NetProcess.AddMsgHandler(SceneOpActivityGetCounterRes.MsgID, _OnSceneOpActivityGetCounterRes);
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);
            NetProcess.RemoveMsgHandler(SceneOpActivityGetCounterRes.MsgID, _OnSceneOpActivityGetCounterRes);
        }

        private void OnRecvSceneNotifyActiveTaskStatus(MsgDATA data)
        {
            SceneNotifyActiveTaskStatus kRecv = new SceneNotifyActiveTaskStatus();
            kRecv.decode(data.bytes);

            //SystemNotifyManager.SysNotifyTextAnimation("活动任务状态改变了 , 任务ID:" + kRecv.taskId);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DailyChargeResultNotify, (int)kRecv.taskId, (int)kRecv.status);
        }

        private void _OnSceneOpActivityGetCounterRes(MsgDATA data)
        {
            int pos = 0;
            SceneOpActivityGetCounterRes res = new SceneOpActivityGetCounterRes();
            res.decode(data.bytes, ref pos);

            DailyChargeCounter counter = new DailyChargeCounter(res.opActId, res.counterId, res.counterValue);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DailyChargeCounterChanged, counter);
        }

        #endregion

        #region  PUBLIC METHODS

        public override void Initialize()
        {
            BindNetEvents();
        }

        public override void Clear()
        {
            UnBindNetEvents();
        }

        #region Member Methods

        public List<DailyChargeRaffleModel> GetDailyChargeModels()
        {
            List<DailyChargeRaffleModel> mDailyChargeModelList = null;

            var dailyChargeData = ActiveManager.GetInstance().GetActiveData(ACTIVITY_TEMPLATE_ID);
            if (dailyChargeData == null)
            {
                return mDailyChargeModelList;
            }
            if (dailyChargeData.akChildItems == null)
            {
                return mDailyChargeModelList;
            }

            for (int i = 0; i < dailyChargeData.akChildItems.Count; i++)
            {
                var activityData = dailyChargeData.akChildItems[i];
                if (activityData == null)
                {
                    continue;
                }
                DailyChargeRaffleModel model = new DailyChargeRaffleModel();
                model.Id = activityData.ID;
                model.Status = (TaskStatus)activityData.status;

                Dictionary<uint, int> awards = activityData.GetAwards();
                if (awards != null)
                {
                    var enumerator = awards.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        uint id = enumerator.Current.Key;
                        int count = enumerator.Current.Value;

                        ItemSimpleData sData = new ItemSimpleData();
                        sData.ItemID = (int)id;
                        sData.Count = count;
                        if (model.AwardItemDataList == null)
                        {
                            model.AwardItemDataList = new List<ItemSimpleData>();
                        }
                        var findData = model.AwardItemDataList.Find((x)=>x.ItemID==sData.ItemID);
                        if(findData != null)
                        {
                            findData.Count += sData.Count;
                        }
                        else
                        {
                            model.AwardItemDataList.Add(sData);
                        }
                    }
                }

                var activeTable = activityData.activeItem;
                if (activeTable != null)
                {
                    int param1 = activeTable.Param1;
                    var chargeMallTable = TableManager.GetInstance().GetTableItem<ProtoTable.ChargeMallTable>(param1);
                    if (chargeMallTable != null)
                    {
                        model.ChargeItemId = param1;
                        model.ChargePrice = chargeMallTable.ChargeMoney;
                        model.ChargeMallType = ChargeMallType.DayChargeWelfare;
                    }
                    int tableId = 0;
                    if(int.TryParse(activeTable.Param2, out tableId))
                    {
                        model.RaffleTableId = tableId;
                        var drawAwardTable = TableManager.GetInstance().GetTableItem<ProtoTable.DrawPrizeTable>(tableId);
                        if (drawAwardTable != null)
                        {
                            model.RaffleTicketType = drawAwardTable.RaffleTicketType;
                        }
                    }
                    model.SortIndex = activeTable.SortPriority2;
                }

                //帐号限购次数
                var systemValueTable = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_DAILY_CHARGE_ACC_LIMIT);
                if (systemValueTable != null)
                {
                    model.accLimitChargeMax = systemValueTable.Value;
                }

                if (mDailyChargeModelList == null)
                {
                    mDailyChargeModelList = new List<DailyChargeRaffleModel>();
                }
                if (mDailyChargeModelList.Contains(model))
                {
                    mDailyChargeModelList.Remove(model);
                }
                mDailyChargeModelList.Add(model);
            }

            //根据表中配置的 排序2 标志 从小到大 排序
            if (mDailyChargeModelList != null)
            {
                mDailyChargeModelList.Sort((x, y) => x.SortIndex.CompareTo(y.SortIndex));
            }

            return mDailyChargeModelList;
        }

        /// <summary>
        /// 根据抽奖ID 获取抽奖券的数量
        /// </summary>
        /// <param name="raffleTableId">抽奖ID</param>
        /// <returns></returns>
        public int GetRaffleTicketCountByRaffleTableId(int raffleTableId)
        {
            var drawAwardTable = TableManager.GetInstance().GetTableItem<ProtoTable.DrawPrizeTable>(raffleTableId);
            if (drawAwardTable == null)
            {
                return 0;
            }
            string countKey = drawAwardTable.GetCountKey;
            int count = CountDataManager.GetInstance().GetCount(countKey);
            return count;
        }

        #endregion

        //前往充值
        public void SendBuyDailyChargeReq(DailyChargeRaffleModel model)
        {
            if (model == null)
            {
                Logger.LogError("pay model is null !!!");
            }
            PayManager.GetInstance().DoPay(model.ChargeItemId, model.ChargePrice, model.ChargeMallType);
        }
        
        //打开转盘界面
        public void OpenRaffleTurnTableFrame(int raffleTableId)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<DailyChargeRaffleTurnTable>())
            {
                ClientSystemManager.GetInstance().CloseFrame<DailyChargeRaffleTurnTable>();
            }
            ClientSystemManager.GetInstance().OpenFrame<DailyChargeRaffleTurnTable>(FrameLayer.Middle, raffleTableId);
        }

        public void OpenRaffleTurnTableFrame(List<DailyChargeRaffleModel> models)
        {
            if (models == null)
            {
                return;
            }
            models.Sort((x, y) => x.SortIndex.CompareTo(y.SortIndex));
            int selectModelTicketsCount = 0;
            for (int i = 0; i < models.Count; i++)
            {
                int rTableId = models[i].RaffleTableId;
                selectModelTicketsCount = GetRaffleTicketCountByRaffleTableId(rTableId);
                if (selectModelTicketsCount > 0)
                {
                    OpenRaffleTurnTableFrame(rTableId);
                    //ClientSystemManager.GetInstance().OpenFrame<DailyChargeRaffleTurnTable>(FrameLayer.Middle, rTableId);
                    break;
                }
            }
            if (selectModelTicketsCount <= 0 && models.Count >= 0)
            {
                OpenRaffleTurnTableFrame(models[0].RaffleTableId);
                //ClientSystemManager.GetInstance().OpenFrame<DailyChargeRaffleTurnTable>(FrameLayer.Middle, models[0].RaffleTableId);
            }
        }

        //红点
        public bool IsRedPointShow()
        {
            return AdsPush.LoginPushManager.GetInstance().IsFirstLogin() && mFirstRedPointFlag;
        }

        public void ResetRedPoint()
        {
            if (FirstRedPointFlag)
            {
                FirstRedPointFlag = false;
            }
        }

        public void ReqDailyChargeCounter(int dailyChargeTaskId)
        {
            SceneOpActivityGetCounterReq req = new SceneOpActivityGetCounterReq();
            req.opActId = (uint)dailyChargeTaskId;
            req.counterId = (uint)ActivityLimitTimeFactory.EActivityCounterType.QAT_SUMMER_DAILY_CHARGE;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        #endregion
    }
}