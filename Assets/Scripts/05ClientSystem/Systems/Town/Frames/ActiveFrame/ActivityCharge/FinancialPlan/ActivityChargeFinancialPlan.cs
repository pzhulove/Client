using System;
using System.Collections.Generic;
using System.Collections;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ActivityChargeFinancialPlan : MonoBehaviour
    {

        #region Data

        //购买
        [SerializeField]
        private Text buyDesText = null;
        [SerializeField]
        private Text buyRewardNumberText = null;
        [SerializeField]
        private GameObject buyItemRoot = null;
        [SerializeField]
        private Text roleLimitText = null;         //角色购买限制
        [SerializeField]
        private Button buyButton = null;

        //领取奖励
        [SerializeField]
        private Text receivedRewardLabelText = null;
        [SerializeField]
        private Text receivedRewardRateText = null;
        [SerializeField]
        private Text mTextTotalReward;
        [SerializeField]
        private Text mTextLevel;

        //不能领取
        [SerializeField]
        private Text otherRoleOwnedText = null;   //其他角色拥有

        [SerializeField]
        private ComUIListScript rewardScrollList = null;

        [SerializeField] private GameObject unBuyContent = null;
        [SerializeField] private GameObject receivedContent = null;
        [SerializeField] private GameObject unReceivedContent = null;

        #endregion

        #region Init
        private void Awake()
        {
            InitUiTextInfo();
            BindUiEventSystem();
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void InitUiTextInfo()
        {
            receivedRewardLabelText.text = TR.Value("financial_plan_received_text");
            buyDesText.text = TR.Value("financial_plan_buy_received");
            FinancialPlanRewardModel financialPlanRewardModel = FinancialPlanDataManager.GetInstance().GetRewardModelByIndex(0);
            if (financialPlanRewardModel != null && financialPlanRewardModel.RewardItemList != null && financialPlanRewardModel.RewardItemList.Count > 0)
            {
                buyRewardNumberText.text = string.Format("{0}", financialPlanRewardModel.RewardItemList[0].Count.ToString());
            }
            otherRoleOwnedText.text = TR.Value("financial_plan_other_role_owned");
            roleLimitText.text = TR.Value("financial_plan_role_limit");
        }

        private void BindUiEventSystem()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyFinancialPlan);
            }

            if (rewardScrollList != null)
            {
                rewardScrollList.Initialize();
                rewardScrollList.onItemSelected += OnItemSelected;
                rewardScrollList.onItemVisiable += OnItemVisiable;
            }
        }

        private void UnBindUiEventSystem()
        {
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
            }

            if (rewardScrollList != null)
            {
                rewardScrollList.onItemSelected -= OnItemSelected;
                rewardScrollList.onItemVisiable -= OnItemVisiable;
            }
        }
        #endregion

        private void OnEnable()
        {
            FinancialPlanDataManager.GetInstance().ResetRedPointTip();
            BindEvents();
            UpdateFinancialPlanContent();
        }

        private void OnDisable()
        {
            UnBindEvents();
            FinancialPlanDataManager.GetInstance().PreReceivedRewardIndex = -1;
        }

        #region UIEventSystem
        private void BindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanBuyRes, OnFinancialPlanBuyRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanReceivedRes, OnFinancialPlanReceivedRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanLevelSync, OnFinancialPlanLevelSync);
        }

        private void UnBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanBuyRes, OnFinancialPlanBuyRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanReceivedRes, OnFinancialPlanReceivedRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanLevelSync, OnFinancialPlanLevelSync);
        }
        
        private void OnFinancialPlanBuyRes(UIEvent eventData)
        {
            //更新购买的状态和rewardItem列表
            UpdateRewardBoughtStatus();
            UpdateRewardItemList();
        }

        private void OnFinancialPlanReceivedRes(UIEvent eventData)
        {
            //更新购买的数量和rewardItem列表
            UpdateRewardReceivedNumber();
            UpdateRewardItemList();
        }

        private void OnFinancialPlanLevelSync(UIEvent eventData)
        {
            //更新rewardItem列表
            UpdateRewardItemList();
        }
        #endregion

        //三种状态：购买状态，领取数量，奖励列表
        private void UpdateFinancialPlanContent()
        {
            UpdateRewardReceivedNumber();
            UpdateRewardBoughtStatus();

            //更新rewardModel的展示状态
            FinancialPlanDataManager.GetInstance().UpdateShowRewardState();
            UpdateRewardItemList();
        }

        #region RewardReceivedNumber

        private void UpdateRewardReceivedNumber()
        {
            receivedRewardRateText.text = FinancialPlanDataManager.GetInstance().CurrentRewardNumber.ToString();
            mTextTotalReward.SafeSetText(FinancialPlanDataManager.GetInstance().TotalRewardNumber.ToString());
            mTextLevel.SafeSetText(PlayerBaseData.GetInstance().Level.ToString());
        }
        #endregion

        #region RewardBoughtStatus

        private void UpdateRewardBoughtStatus()
        {
            if (false == FinancialPlanDataManager.GetInstance().IsCanBuyFinancialPlan)
            {
                unBuyContent.CustomActive(false);
                receivedContent.CustomActive(false);
                unReceivedContent.CustomActive(true);
            }
            else
            {
                if (false == FinancialPlanDataManager.GetInstance().IsAlreadyBuyFinancialPlan)
                {
                    unBuyContent.CustomActive(true);
                    receivedContent.CustomActive(false);
                    unReceivedContent.CustomActive(false);
                }
                else
                {
                    unBuyContent.CustomActive(false);
                    receivedContent.CustomActive(true);
                    unReceivedContent.CustomActive(false);
                }
            }
        }
        #endregion

        #region ItemList
        //更新奖励列表
        private void UpdateRewardItemList()
        {
            if(rewardScrollList == null)
                return;

            //重置
            rewardScrollList.SetElementAmount(0);
            rewardScrollList.ResetContentPosition();

            //赋值移动
            rewardScrollList.SetElementAmount(FinancialPlanDataManager.GetInstance().GetRewardModelCount());
            MoveScrollListToFirstReward();
        }

        private void MoveScrollListToFirstReward()
        {
            var firstRewardIndex = FinancialPlanDataManager.GetInstance().GetFirstReceivedRewardModelIndex();
            if (firstRewardIndex <= 1)
            {
                //前两个
                rewardScrollList.MoveElementInScrollArea(0, true);
            }
            else if (firstRewardIndex >= FinancialPlanDataManager.GetInstance().GetRewardModelCount() - 2)
            {
                //最后两个
                rewardScrollList.MoveElementInScrollArea(FinancialPlanDataManager.GetInstance().GetRewardModelCount() - 1, true);
            }
            else
            {
                //中间
                rewardScrollList.MoveElementInScrollArea(firstRewardIndex + 1, true);
            }
        }

        protected virtual void OnItemVisiable(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            var rewardItem = item.GetComponent<ActivityFinancialRewardItem>();
            var rewardModel =
                FinancialPlanDataManager.GetInstance().GetRewardModelByIndex(item.m_index);
            if (rewardModel != null && rewardItem != null)
            {
                rewardItem.Init(rewardModel);
            }
        }

        protected virtual void OnItemSelected(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
        }
        #endregion

        #region OnBuyFinancialPlan
        private void OnBuyFinancialPlan()
        {
            //购买理财计划，付费
            FinancialPlanDataManager.GetInstance().SendBuyFinancialPlanReq();
        }
        #endregion

        private void ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

    }
}