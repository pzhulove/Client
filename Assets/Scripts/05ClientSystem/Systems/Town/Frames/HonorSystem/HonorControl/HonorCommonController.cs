using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{

    public class HonorCommonController : MonoBehaviour
    {


        [Space(5)] [HeaderAttribute("HonorLevelIcon")] [Space(5)]
        [SerializeField] private Image leftLevelIcon;
        [SerializeField] private Image rightLevelIcon;

        [Space(5)]
        [HeaderAttribute("HonorLevelName")]
        [Space(5)]
        [SerializeField] private Text honorLevelValueText;
        [SerializeField] private Text honorLevelNameText;

        [Space(5)] [HeaderAttribute("HonorExp")] [Space(5)]
        [SerializeField] private Slider honorExpSlider;
        [SerializeField] private Text honorExpText;

        [Space(5)] [HeaderAttribute("ProtectCard")] [Space(5)]
        [SerializeField] private GameObject protectCardFunctionOpenRoot;
        [SerializeField] private Text protectCardOpenTipLabel;
        [SerializeField] private GameObject protectCardFunctionUnOpenRoot;
        [SerializeField] private Text protectCardUnUsedTipLabel;
        [SerializeField] private Button protectCardUseButton;

        [Space(5)]
        [HeaderAttribute("HonorPreWeekRank")]
        [Space(5)]
        [SerializeField] private Text honorPreWeekRankText;

        [SerializeField] private GameObject upIcon;
        [SerializeField] private GameObject flatIcon;
        [SerializeField] private GameObject downIcon;

        [Space(5)]
        [HeaderAttribute("HonorHistoryRank")]
        [Space(5)]
        [SerializeField] private Text historyRankText;
        [SerializeField] private Button honorSystemPreviewButton;

        //[Space(5)]
        //[HeaderAttribute("TodayActivity")]
        //[Space(5)]
        //[SerializeField] private ComUIListScript todayActivityItemList;

        private List<PvpNumberStatistics> todayActivityDataModelList;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            //if (todayActivityItemList != null)
            //{
            //    todayActivityItemList.Initialize();
            //    todayActivityItemList.onItemVisiable += OnTodayActivityItemVisible;
            //}

            if (honorSystemPreviewButton != null)
            {
                honorSystemPreviewButton.onClick.RemoveAllListeners();
                honorSystemPreviewButton.onClick.AddListener(OnHonorSystemPreviewButtonClicked);
            }

            if (protectCardUseButton != null)
            {
                protectCardUseButton.onClick.RemoveAllListeners();
                protectCardUseButton.onClick.AddListener(OnProtectCardUseButtonClicked);
            }
        }

        private void UnBindEvents()
        {
            //if (todayActivityItemList != null)
            //    todayActivityItemList.onItemVisiable -= OnTodayActivityItemVisible;

            if(honorSystemPreviewButton != null)
                honorSystemPreviewButton.onClick.RemoveAllListeners();

            if(protectCardUseButton != null)
                protectCardUseButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            //todayActivityDataModelList = null;
        }

       
        public void InitHonorCommonController()
        {
            //角色的荣誉数据
            InitHonorPlayerInfo();

            UpdateProtectCardUsedContent();

            //今日的活动
            InitHonorTodayActivityItemList();
        }

        private void InitHonorPlayerInfo()
        {

            var playerHonorLevel = (int)HonorSystemDataManager.GetInstance().PlayerHonorLevel;
            if (HonorSystemDataManager.GetInstance().PlayerHonorLevel == 0)
                playerHonorLevel = HonorSystemDataManager.DefaultHonorSystemLevel;

            //荣誉等级
            HonorLevelTable playerHonorLevelTable = TableManager.GetInstance().GetTableItem<HonorLevelTable>(
                playerHonorLevel);

            if (playerHonorLevelTable == null)
                return;

            //荣誉等级
            InitHonorPlayerLevelInfo(playerHonorLevelTable);

            //荣誉经验
            InitHonorPlayerExpInfo(playerHonorLevelTable);

            //上周排名
            InitHonorPreWeekRankInfo();

            //历史记录
            InitHonorHistoryRankInfo();

        }

        private void InitHonorPlayerLevelInfo(HonorLevelTable playerHonorLevelTable)
        {

            //荣誉图标的等级分为两个部分(左右两个部分)
            if (leftLevelIcon != null)
                ETCImageLoader.LoadSprite(ref leftLevelIcon, playerHonorLevelTable.TitleFlag);
            if (rightLevelIcon != null)
                ETCImageLoader.LoadSprite(ref rightLevelIcon, playerHonorLevelTable.TitleFlag);

            //荣誉的名字
            if (honorLevelNameText != null)
            {
                var titleNameStr = HonorSystemUtility.GetTitleNameByTitleId(playerHonorLevelTable.Title);
                honorLevelNameText.text = titleNameStr;
            }
            
            if (honorLevelValueText != null)
            {
                honorLevelValueText.text = HonorSystemDataManager.GetInstance().PlayerHonorLevel.ToString();
            }

        }

        private void InitHonorPlayerExpInfo(HonorLevelTable playerHonorLevelTable)
        {

            //当前等级需要的经验
            var currentLevelTotalExp = playerHonorLevelTable.NeedExp;

            //当前经验 = 总经验(服务器同步) - 当前等级的经验
            var currentOwnerExp = HonorSystemDataManager.GetInstance().PlayerHonorExp - currentLevelTotalExp;
            /*if (currentOwnerExp < 0)
                currentOwnerExp = 0;*/

            var currentNeedExp = 0;
            //升级需要的经验值
            var nextLevel = (int) HonorSystemDataManager.GetInstance().PlayerHonorLevel + 1;
            var nextLevelTable = TableManager.GetInstance().GetTableItem<HonorLevelTable>(nextLevel);
            if (nextLevelTable != null)
            {
                currentNeedExp = nextLevelTable.NeedExp - currentLevelTotalExp;
            }
            else
            {
                //最大等级
                currentNeedExp = 0;
                currentOwnerExp = 0;
            }
            
            
            //if (honorExpText != null)
            //{
            //    var honorExp = TR.Value("Honor_System_Exp_Format",
            //        currentOwnerExp,
            //        currentNeedExp);
            //    honorExpText.text = honorExp;
            //}

            if (honorExpSlider != null)
            {
                var expSliderValue = 1f;
                if (currentNeedExp > 0)
                    expSliderValue = (float)currentOwnerExp / (float)currentNeedExp;
                
                //约束
                if (expSliderValue < 0)
                {
                    expSliderValue = 0.0f;
                }
                else if (expSliderValue > 1.0f)
                {
                    expSliderValue = 1.0f;
                }

                honorExpSlider.value = expSliderValue;
            }
        }

        private void InitHonorPreWeekRankInfo()
        {
            CommonUtility.UpdateGameObjectVisible(upIcon, false);
            CommonUtility.UpdateGameObjectVisible(flatIcon, false);
            CommonUtility.UpdateGameObjectVisible(downIcon, false);

            if (honorPreWeekRankText == null)
                return;
            
            var lastWeekRank = HonorSystemDataManager.GetInstance().PlayerLastWeekRank;
            var historyWeekRank = HonorSystemDataManager.GetInstance().PlayerHistoryRank;

            //上周没有排名
            if (lastWeekRank <= 0)
            {
                honorPreWeekRankText.text = TR.Value("Honor_System_Rank_Last_Week_Empty_Flag");
                //上上周有排名, 显示降;上上周没有排名，显示空
                if (historyWeekRank > 0)
                {
                    CommonUtility.UpdateGameObjectVisible(downIcon, true);
                }
                return;
            }

            //上周排名存在
            //展示上周的排名
            honorPreWeekRankText.text = HonorSystemDataManager.GetInstance().PlayerLastWeekRank.ToString();

            //上上周排名不存在
            if (historyWeekRank <= 0)
            {
                //展示Up
                CommonUtility.UpdateGameObjectVisible(upIcon, true);
                return;
            }
            
            //上周排名和上上周排名都存在（都大于0）
            if (lastWeekRank < historyWeekRank)
            {
                //上周排名较小，说明等级提升
                CommonUtility.UpdateGameObjectVisible(upIcon, true);
            }
            else if (lastWeekRank == historyWeekRank)
            {
                ////等级相同,显示空
                //CommonUtility.UpdateGameObjectVisible(flatIcon, true);
            }
            else
            {

                CommonUtility.UpdateGameObjectVisible(downIcon, true);
            }
        }

        private void InitHonorHistoryRankInfo()
        {
            var playerHighestHonorLevel = (int) HonorSystemDataManager.GetInstance().PlayerHighestHonorLevel;
            if (playerHighestHonorLevel == 0)
                playerHighestHonorLevel = HonorSystemDataManager.DefaultHonorSystemLevel;

            var playerHighestHonorLevelTable = TableManager.GetInstance().GetTableItem<HonorLevelTable>(
                playerHighestHonorLevel);

            if (playerHighestHonorLevelTable == null)
                return;

            if (historyRankText != null)
            {
                var historyTitleStr = HonorSystemUtility.GetTitleNameByTitleId(playerHighestHonorLevelTable.Title);
                historyRankText.text = historyTitleStr;
            }
        }

        private void InitHonorTodayActivityItemList()
        {
            //if (todayActivityItemList == null)
            //    return;

            //var todayActivityItemNumber = 0;

            ////今日活动的数据
            //todayActivityDataModelList = HonorSystemUtility.GetPvpNumberStaticsListByDateType(
            //        HONOR_DATE_TYPE.HONOR_DATE_TYPE_TODAY);

            //if (todayActivityDataModelList != null && todayActivityDataModelList.Count > 0)
            //    todayActivityItemNumber = todayActivityDataModelList.Count;

            //todayActivityItemList.SetElementAmount(todayActivityItemNumber);

        }

        //private void OnTodayActivityItemVisible(ComUIListElementScript item)
        //{
        //    if (todayActivityItemList == null)
        //        return;

        //    if (item == null)
        //        return;

        //    if (todayActivityDataModelList == null || todayActivityDataModelList.Count <= 0)
        //        return;

        //    if (item.m_index < 0 || item.m_index >= todayActivityDataModelList.Count)
        //        return;

        //    var todayActivityDataModel = todayActivityDataModelList[item.m_index];
        //    var todayCommonItem = item.GetComponent<HonorCommonItem>();
        //    if (todayActivityDataModel != null
        //        && todayCommonItem != null)
        //    {
        //        todayCommonItem.InitItem(todayActivityDataModel);
        //    }
        //}

        public void UpdateProtectCardUsedContent()
        {
            CommonUtility.UpdateGameObjectVisible(protectCardFunctionOpenRoot, false);
            CommonUtility.UpdateGameObjectVisible(protectCardFunctionUnOpenRoot, false);

            if (HonorSystemDataManager.GetInstance().IsAlreadyUseProtectCard == true)
            {
                //已经开启
                CommonUtility.UpdateGameObjectVisible(protectCardFunctionOpenRoot, true);
                if (protectCardOpenTipLabel != null)
                    protectCardOpenTipLabel.text = TR.Value("Honor_System_Protect_Card_Already_Open");
            }
            else
            {
                //未开启
                CommonUtility.UpdateGameObjectVisible(protectCardFunctionUnOpenRoot, true);
                if (protectCardUnUsedTipLabel != null)
                    protectCardUnUsedTipLabel.text = TR.Value("Honor_System_Protect_Card_To_Use");
            }
        }

        //荣誉保护卡按钮
        private void OnProtectCardUseButtonClicked()
        {
            HonorSystemUtility.OnOpenHonorSystemProtectCardFrame();
        }

        //预览按钮
        private void OnHonorSystemPreviewButtonClicked()
        {
            HonorSystemUtility.OnOpenHonorSystemPreviewFrame();
        }


    }
}