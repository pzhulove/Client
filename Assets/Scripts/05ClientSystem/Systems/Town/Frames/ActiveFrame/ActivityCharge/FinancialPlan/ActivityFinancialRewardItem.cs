using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class ActivityFinancialRewardItem : MonoBehaviour
    {

        private readonly string numberPathPrefix = "UI/Image/Packed/p_UI_Fuli.png:UI_Fuli_Qiandao_Teshuzi_0";
        private FinancialPlanRewardModel dataItem = null;

        //奖励内容
        [SerializeField] private Text levelNeedReachedText = null;

        //奖励的状态
        [SerializeField] private Button receivedButton = null;
        [SerializeField] private Image mImageReceived;
        [SerializeField] private Text mTextCount = null;    //奖励的数量
        [SerializeField] private Text mTextState;
        [SerializeField] private UIGray mButtonGray;
        [SerializeField] private GameObject mGoBtnEffectParent;

        private ComItem _comRewardItem = null;

        private void Start()
        {
            if (receivedButton != null)
            {
                receivedButton.onClick.RemoveAllListeners();
                receivedButton.onClick.AddListener(OnReceiveButtonClick);
            }
            //mButtonGray.Restore();
        }

        private void OnDestroy()
        {
            if (receivedButton != null)
            {
                receivedButton.onClick.RemoveAllListeners();
            }
        }

        public void Init(FinancialPlanRewardModel curData)
        {
            this.dataItem = curData;
            InitRewardItemContent();
            InitRewardItemState();
        }

        //初始化：奖励列表(由表决定，固定不变）和领奖状态（由对应的数据模型决定）
        private void InitRewardItemContent()
        {
            if (dataItem.LevelLimit <= 1)
            {
                levelNeedReachedText.SafeSetText(TR.Value("financial_plan_buy_can_get"));
            }
            else
            {
                levelNeedReachedText.SafeSetText(string.Format(TR.Value("financial_plan_level_limit"), dataItem.LevelLimit));
            }
            mTextCount.SafeSetText(dataItem.RewardItemList[0].Count.ToString());
        }

        private void InitRewardItemState()
        {
            switch (dataItem.ShowRewardState)
            {
                case FinancialPlanState.UnBuy:
                    mTextState.SafeSetText(TR.Value("financial_plan_not_buy_text"));
                    receivedButton.CustomActive(true);
                    mButtonGray.SetEnable(true);
                    mGoBtnEffectParent.CustomActive(false);
                    //receivedButton.interactable = false;
                    mImageReceived.enabled = false;
                    break;
                case FinancialPlanState.UnAcommpolished:
                    mTextState.SafeSetText(TR.Value("financial_plan_unfinished_text"));
                    receivedButton.CustomActive(true);
                    mButtonGray.SetEnable(true);
                    mGoBtnEffectParent.CustomActive(false);
                    receivedButton.interactable = false;
                    mImageReceived.enabled = false;
                    break;
                case FinancialPlanState.Finished:
                    mTextState.SafeSetText(TR.Value("financial_plan_buy"));
                    receivedButton.CustomActive(true);
                    mImageReceived.enabled = false;
                    receivedButton.interactable = true;
                    mButtonGray.SetEnable(false);
                    mGoBtnEffectParent.CustomActive(true);
                    break;
                case FinancialPlanState.Received:
                    mTextState.SafeSetText(TR.Value("financial_plan_not_buy_text"));
                    mImageReceived.enabled = true;
                    receivedButton.CustomActive(false);
                    break;
                case FinancialPlanState.UnReceived:
                    mTextState.SafeSetText(TR.Value("financial_plan_cannot_received"));
                    receivedButton.CustomActive(true);
                    receivedButton.interactable = false;
                    mButtonGray.SetEnable(true);
                    mGoBtnEffectParent.CustomActive(false);
                    mImageReceived.enabled = false;
                    break;
                default:
                    mTextState.SafeSetText(TR.Value("financial_plan_buy"));
                    receivedButton.CustomActive(true);
                    mImageReceived.enabled = true;
                    receivedButton.interactable = false;
                    mButtonGray.SetEnable(true);
                    mGoBtnEffectParent.CustomActive(true);
                    break;
            }
        }

        private void ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        private void OnReceiveButtonClick()
        {
            if (dataItem != null)
            {
                if (dataItem.ShowRewardState == FinancialPlanState.UnBuy)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("financial_plan_unbuy"));
                    return;
                }

                FinancialPlanDataManager.GetInstance().PreReceivedRewardIndex = dataItem.Index;
                FinancialPlanDataManager.GetInstance().SendReceivedRewardItemReq(dataItem.Id);
            }
        }
    }
}
