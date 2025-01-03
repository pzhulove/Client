using System.Collections.Generic;
///////删除linq
using ProtoTable;
using Protocol;
using Network;
using UnityEngine;

namespace GameClient
{
    //理财计划奖励和福利奖励的状态
    public enum FinancialPlanState
    {
        Invalid = -1,
        UnBuy = 0,              //未购买，
        UnAcommpolished,        //未达成
        Finished,               //达成，可以领取
        Received,               //已经领取
        UnReceived,             //不可领取的状态（其他角色购买）
    }

    public enum FinancialPlanButtonState
    {
        Invalid = -1,
        IsNotShowing = 0,
        IsShowing = 1,
        AlreadyClicked = 2,
    }


    public class FinancialPlanDataManager : DataManager<FinancialPlanDataManager>
    {
        private readonly string _financialPlanStr = "FinancialPlan";
        //需要从配置表中读取
        private readonly int _chargeMallTableId = 10;         //付费商城表中的条目ID
        public readonly int ActivityConfigId = 9380;        //理财计划所在的活动配置ID
        public readonly int ActivityTemplateId = 8600;      //理财计划的模板ID，配置在活动模板表
        public readonly int ActivityFinancialPlanUnlockLevel = 8;

        //表中读取
        public int ItemId { get; private set; }         //付费ID
        public int ItemPrice { get; private set; }      //付费价格
        public int BuyRewardItemId { get; private set; }    //付费购买的物品ID
        public int BuyRewardItemNumber { get; private set; }    //付费购买的物品数量

        public int BuyReceivedItemNumber { get; private set; }  //购买后可以领取的数量
        public int ItemRateNumber { get; private set; }
        
        //初始值为-1
        public int PreReceivedRewardIndex { get; set; } //上一次领取奖励的索引

        //服务器获得并随时更新
        public bool IsCanBuyFinancialPlan { get; private set; }  //是否可以购买
        public bool IsAlreadyBuyFinancialPlan { get; private set; } //在可以购买的情况下，是否已经购买

        public int TotalRewardNumber { get; private set; }       //从表中获得
        public int CurrentRewardNumber { get; private set; }     //由服务器的数据初始化

        private bool _isAlreadyReceivedAllReward = false; //是否已经领取了所有的奖励,用于控制tab的位置
        private bool _isClickFinancialPlanButton = false; //是否点击了对话框上面的理财计划按钮

        private List<FinancialPlanRewardModel> _rewardModelList = new List<FinancialPlanRewardModel>();

        private bool _firstRedPointTipFlag = true;      //第一次上线（每次登陆的时候）是否显示红点的标志

        private FinancialPlanButtonState _financialPlanButtonState = FinancialPlanButtonState.Invalid;  //按钮是否正在显示

        public bool FirstRedPointTipFlag
        {
            get { return _firstRedPointTipFlag; }
            set
            {
                if (_firstRedPointTipFlag != value)
                {
                    _firstRedPointTipFlag = value;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanRedPointTips);
                }
            }
        }

        #region Initialize

        public override void Initialize()
        {
            InitializeFinancialPlanData();
            BindNetEvents();
        }

        private void InitializeFinancialPlanData()
        {
            //初始值
            _firstRedPointTipFlag = true;
            _isAlreadyReceivedAllReward = false;

            IsCanBuyFinancialPlan = true;
            IsAlreadyBuyFinancialPlan = false;
            CurrentRewardNumber = 0;
            TotalRewardNumber = 0;
            PreReceivedRewardIndex = -1;


            #region InitTable
            _rewardModelList.Clear();
            var moneyManagerTables = TableManager.GetInstance().GetTable<MoneyManageTable>();
            //从table表中获得理财奖励的数据
            if (moneyManagerTables != null)
            {
                var moneyManagerTableList = moneyManagerTables.ToList();
                int index = 0;
                foreach (var curItem in moneyManagerTableList)
                {
                    var moneyManagerItem = curItem.Value as MoneyManageTable;
                    if (moneyManagerItem != null)
                    {
                        var curModel = new FinancialPlanRewardModel();
                        curModel.Index = index++;
                        curModel.Id = moneyManagerItem.ID;
                        curModel.LevelLimit = moneyManagerItem.Level;

                        //奖励列表
                        foreach (var curRewardItem in moneyManagerItem.ItemReward)
                        {
                            var curRewardItemData = curRewardItem.Split('_');
                            var rewardItemId = -1;
                            var rewardItemCount = -1;

                            if (int.TryParse(curRewardItemData[0], out rewardItemId) == true
                                && int.TryParse(curRewardItemData[1], out rewardItemCount) == true)
                            {
                                var itemSimpleData = new ItemSimpleData
                                {
                                    ItemID = rewardItemId,
                                    Count = rewardItemCount,
                                };
                                TotalRewardNumber += rewardItemCount;
                                curModel.RewardItemList.Add(itemSimpleData);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        curModel.RewardState = FinancialPlanState.UnBuy;
                        curModel.ShowRewardState = FinancialPlanState.UnBuy;
                        _rewardModelList.Add(curModel);
                    }
                }
            }
            #endregion

            //数组进行排序
            if (_rewardModelList != null && _rewardModelList.Count > 1)
            {
                _rewardModelList.Sort((x, y) => x.LevelLimit.CompareTo(y.LevelLimit));
                for (var i = 0; i < _rewardModelList.Count; i++)
                {
                    if (_rewardModelList[i] != null)
                        _rewardModelList[i].Index = i;
                }
            }

            //从充值商城的表中（ChargeMallTable)获得相应的物品的价格条目。ID为4
            var curMallTable = TableManager.GetInstance().GetTableItem<ChargeMallTable>(_chargeMallTableId);
            if (curMallTable != null)
            {
                ItemId = curMallTable.ID;
                ItemPrice = curMallTable.ChargeMoney;
                BuyRewardItemNumber = curMallTable.itemNum;
                BuyRewardItemId = curMallTable.itemID;
            }
            else
            {
                //默认情况
                ItemId = 4;
                ItemPrice = 198;
                BuyRewardItemNumber = 980;
                BuyRewardItemId = 600000002;
            }

            BuyReceivedItemNumber = BuyRewardItemNumber;

            //计算奖励倍数
            ItemRateNumber = (int)(TotalRewardNumber / (float)BuyRewardItemNumber + 0.5);
        }

        public override void Clear()
        {
            _rewardModelList.Clear();
            IsAlreadyBuyFinancialPlan = false;
            IsCanBuyFinancialPlan = false;
            _financialPlanButtonState = FinancialPlanButtonState.Invalid;

            UnBindNetEvents();
        }

        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneGiveMoneyManageRewardRes.MsgID, OnReceivedRewardItemResp);
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            ActiveManager.GetInstance().onAddMainActivity += OnAddMainActivity;
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneGiveMoneyManageRewardRes.MsgID, OnReceivedRewardItemResp);
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            ActiveManager.GetInstance().onAddMainActivity -= OnAddMainActivity;
        }
        #endregion

        private void OnAddMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == ActivityConfigId
                && data.mainItem.ID == ActivityTemplateId)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanButtonUpdateByLevel);
            }
        }

        #region SyncNetMessage
        //同步购买理财计划的角色ID
        public void SyncBuyFinancialPlanBoughtStatus(byte status)
        {

            MoneyManageStatus curMoneyManageStatus = (MoneyManageStatus) status;

            switch (curMoneyManageStatus)
            {
                case MoneyManageStatus.MMS_NOT_BUY:
                    //账号没有购买，角色可以购买
                    IsCanBuyFinancialPlan = true;
                    IsAlreadyBuyFinancialPlan = false;
                    break;
                case MoneyManageStatus.MMS_PLAYER_BUY:
                    //该账号已经购买，该角色购买的
                    IsCanBuyFinancialPlan = true;
                    IsAlreadyBuyFinancialPlan = true;
                    break;
                case MoneyManageStatus.MMS_NEW_PLAYER_BUY:
                    //该账号已经购买，该角色购买的
                    IsCanBuyFinancialPlan = true;
                    IsAlreadyBuyFinancialPlan = true;
                    break;
                case MoneyManageStatus.MMS_ACCOUNT_BUY:
                    //该账号已经购买，其他角色购买的
                    IsCanBuyFinancialPlan = false;
                    IsAlreadyBuyFinancialPlan = false;
                    break;
                default:
                    //默认情况，没有人购买
                    IsCanBuyFinancialPlan = true;
                    IsAlreadyBuyFinancialPlan = false;
                    break;
            }

            OnBuySyncFinancialPlanBoughtStatus();
        }

        //处理数据并发送UI事件
        private void OnBuySyncFinancialPlanBoughtStatus()
        {
            UpdateShowRewardState();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanBuyRes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanRedPointTips);
        }

        //同步领取奖励的数据
        public void SyncFinancialPlanMaskProperty(MoneyManageMaskProperty property)
        {
            //同步相应的数据
            CurrentRewardNumber = 0;
            foreach (var rewardModel in _rewardModelList)
            {
                var rewardItemId = rewardModel.Id;
                if (property.CheckMask((uint)rewardItemId))
                {
                    rewardModel.RewardState = FinancialPlanState.Received;
                    CurrentRewardNumber += rewardModel.GetRewardItemCount();
                }
            }

            //判断是否全部领取完成
            _isAlreadyReceivedAllReward = true;
            foreach (var rewardModel in _rewardModelList)
            {
                if (rewardModel.RewardState != FinancialPlanState.Received)
                {
                    _isAlreadyReceivedAllReward = false;
                }
            }

            OnSyncFinancialPlanMaskProperty();
        }
        
        //发送UI事件
        private void OnSyncFinancialPlanMaskProperty()
        {
            if (IsCanBuyFinancialPlan == true
                && IsAlreadyBuyFinancialPlan == true)
            {
                UpdateShowRewardState();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanReceivedRes);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanRedPointTips);
            }
        }

        //角色升级进行数据同步
        private void OnLevelChanged(int preLevel, int curLevel)
        {
            //只有可以购买并且已经购买的情况下，才执行数据刷新
            if (IsCanBuyFinancialPlan == true
                && IsAlreadyBuyFinancialPlan == true)
            {
                bool isCanReceived = false;

                foreach (var rewardModel in _rewardModelList)
                {
                    if (rewardModel.ShowRewardState != FinancialPlanState.UnReceived
                        && rewardModel.ShowRewardState != FinancialPlanState.UnBuy
                        && rewardModel.ShowRewardState != FinancialPlanState.Received)
                    {
                        if (preLevel < rewardModel.LevelLimit && curLevel >= rewardModel.LevelLimit)
                        {
                            isCanReceived = true;
                            rewardModel.ShowRewardState = FinancialPlanState.Finished;
                        }
                    }
                }

                UpdateShowRewardState();
                //存在可以新领取的rewardItem
                if (isCanReceived == true)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanLevelSync);
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanRedPointTips);
            }

            //解锁
            if (preLevel < ActivityFinancialPlanUnlockLevel && curLevel >= ActivityFinancialPlanUnlockLevel)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanButtonUpdateByLevel);
                return;
            }

            //角色等级升级，更新理财按钮
            foreach (var rewardModel in _rewardModelList)
            {
                if (preLevel < rewardModel.LevelLimit && curLevel >= rewardModel.LevelLimit)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanButtonUpdateByLevel);
                    return;
                }
            }

        }
        #endregion

        #region ReceiveReward
        //发送领取奖励请求
        public void SendReceivedRewardItemReq(int rewardId)
        {
            var req = new SceneGiveMoneyManageRewardReq
            {
                rewardId = (byte) rewardId,
            };

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //领取奖励完成回调
        private void OnReceivedRewardItemResp(MsgDATA data)
        {
            var res = new SceneGiveMoneyManageRewardRes();
            res.decode(data.bytes);

            if (res.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            //需要配置表
            SystemNotifyManager.SystemNotify(1266);
        }
        #endregion

        public void SendBuyFinancialPlanReq()
        {
            PayManager.GetInstance().DoPay(ItemId, ItemPrice, ChargeMallType.FinancialPlan);
        }

        #region Helper
        public FinancialPlanRewardModel GetRewardModelByIndex(int index)
        {
            if (index < 0 || index >= _rewardModelList.Count)
                return null;

            return _rewardModelList[index];
        }

        //得到展示奖励的索引
        public int GetFirstReceivedRewardModelIndex()
        {
            //不能购买，显示第一条
            if (IsCanBuyFinancialPlan == false)
                return 0;

            //能够购买，但是没有购买，显示第一条
            if (IsAlreadyBuyFinancialPlan == false)
                return 0;

            //如果存在可以领取的奖励，则返回最上面一条
            for (var i = 0; i < _rewardModelList.Count; i++)
            {
                var curRewardModel = _rewardModelList[i];
                if (curRewardModel.ShowRewardState == FinancialPlanState.Finished)
                    return i;
            }
            
            //没有可以领取的奖励，返回上一次领取奖励的Index(PreReceivedRewardIndex)
            //PreReceivedRewardIndex 在OnDisable的时候重置为-1
            if (PreReceivedRewardIndex != -1)
            {
                return PreReceivedRewardIndex;
            }            

            //如果PreReceivedRewardIndex为重置值(既第一次或者再次打开界面）根据用户等级来决定显示那一条记录
            for (var i = 0; i < _rewardModelList.Count; i++)
            {
                var curRewardModel = _rewardModelList[i];
                if (curRewardModel.LevelLimit > PlayerBaseData.GetInstance().Level)
                    return i;
            }

            //展示最后一条
            return _rewardModelList.Count - 1;

        }

        public int GetRewardModelCount()
        {
            return _rewardModelList.Count;
        }

        public void UpdateShowRewardState()
        {
            if(_rewardModelList == null || _rewardModelList.Count <= 0)
                return;

            if (false == IsCanBuyFinancialPlan)
            {
                //不能购买
                foreach (var rewardModel in _rewardModelList)
                {
                    rewardModel.ShowRewardState = FinancialPlanState.UnReceived;
                }    
            }
            else
            {
                if (false == IsAlreadyBuyFinancialPlan)
                {
                    //没有购买
                    foreach (var rewardModel in _rewardModelList)
                    {
                        rewardModel.ShowRewardState = FinancialPlanState.UnBuy;
                    }
                }
                else
                {
                    //已经购买
                    foreach (var rewardModel in _rewardModelList)
                    {
                        if (rewardModel.RewardState == FinancialPlanState.Received)
                        {
                            rewardModel.ShowRewardState = rewardModel.RewardState;
                        }
                        else
                        {
                            if (PlayerBaseData.GetInstance().Level >= rewardModel.LevelLimit)
                            {
                                rewardModel.ShowRewardState = FinancialPlanState.Finished;
                            }
                            else
                            {
                                rewardModel.ShowRewardState = FinancialPlanState.UnAcommpolished;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region RedPoint

        //判断是否领取了所有奖励,用于展示左侧的理财计划页签的位置
        public bool IsAlreadyReceivedAllReward()
        {
            //不可领取，其他角色已经领取，页签放在最后
            if (IsCanBuyFinancialPlan == false)
            {
                return true;
            }
            //可以领取并且领取完成
            if (_isAlreadyReceivedAllReward == true)
            {
                return true;
            }
            //可以领取，没有领取完成
            return false;
        }

        private bool IsExistRewardItemCanReceived()
        {
            foreach (var rewardModel in _rewardModelList)
            {
                if (rewardModel.ShowRewardState == FinancialPlanState.Finished)
                    return true;
            }

            return false;
        }

        //tab是否显示红点
        public bool IsShowRedPoint()
        {
            if (IsCanBuyFinancialPlan == false)
            {
                return false;
            }
            else
            {
                if (IsAlreadyBuyFinancialPlan == false)
                {
                    if (PlayerBaseData.GetInstance().Level >= 30)
                    {
                        _firstRedPointTipFlag = false;
                        return false;
                    }
                    else
                    {
                        return FirstRedPointTipFlag;
                    }
                }
                else
                {
                    return IsExistRewardItemCanReceived();
                }
            }
        }

        //重置红点
        public void ResetRedPointTip()
        {
            if (FirstRedPointTipFlag == true)
            {
                FirstRedPointTipFlag = false;
            }
        }
        #endregion

        #region FinancialPlanButton
        //是否可以显示理财计划按钮
        public bool IsShowFinancialPlanButton()
        {
#if APPLE_STORE
            //IOS屏蔽功能 理财计划入口
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
            {
                return false;
            }
#endif
            //等级不满足
            if (PlayerBaseData.GetInstance().Level < ActivityFinancialPlanUnlockLevel)
                return false;

            //没有充值，不显示
            if (PlayerBaseData.GetInstance().VipLevel <= 0)
                return false;

            //不能购买，不显示
            if (IsCanBuyFinancialPlan == false)
                return false;

            //已经购买，不显示
            if (IsAlreadyBuyFinancialPlan == true)
                return false;

            //不存在理财计划
            if (IsExistFinancialPlanActivity() == false)
                return false;

            return true;
        }

        //判断是否存在理财计划活动
        public bool IsExistFinancialPlanActivity()
        {
            var activities = ActiveManager.GetInstance().ActiveDictionary.Values.ToList();
           
            foreach (var activity in activities)
            {
                if (activity.iActiveID == ActivityTemplateId)
                    return true;
            }
            return false;
        }

        public void ShowFinancialPlanActivity()
        {
#if APPLE_STORE
            //IOS屏蔽功能 理财计划入口
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
            {
                return;
            }
#endif

            ActiveManager.GetInstance().OpenActiveFrame(ActivityConfigId, ActivityTemplateId);
        }

        public bool IsPlayerAlreadyShowOnceFinancialPlanInLogin()
        {
            //keyname由KeyID 和"FinancialPlan"组成
            var keyName = PlayerBaseData.GetInstance().RoleID.ToString() + _financialPlanStr;
            if (PlayerPrefs.HasKey(keyName) == false)
                return false;

            return PlayerPrefs.GetInt(keyName) >= 1 ? true : false;
        }

        public void SetPlayerAlreadyShowFinancialPlanInLogin()
        {
            var keyName = PlayerBaseData.GetInstance().RoleID.ToString() + _financialPlanStr;
            PlayerPrefs.SetInt(keyName, 1);
        }

        public void SetFinancialPlanButtonState(FinancialPlanButtonState buttonState)
        {
            _financialPlanButtonState = buttonState;
        }

        public FinancialPlanButtonState GetFinancialPlanButtonShowState()
        {
            return _financialPlanButtonState;
        }

        #endregion

    }
}
