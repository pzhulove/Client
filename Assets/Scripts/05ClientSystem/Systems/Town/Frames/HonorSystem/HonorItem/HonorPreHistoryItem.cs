using System;
using System.Collections.Generic;
using Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorPreHistoryItem : MonoBehaviour
    {

        private HONOR_DATE_TYPE _honorDateType;
        private PlayerHistoryHonorInfo _playerHistoryHonorInfo;
        private List<PvpNumberStatistics> _totalPvpNumberStatisticsList;
        private List<PvpNumberStatistics> _curPvpNumberStatisticsList = new List<PvpNumberStatistics>();
        private int _curPvpNumber = 0;

        [Space(5)]
        [HeaderAttribute("Title")]
        [Space(5)]
        [SerializeField] private Text preHistoryTitleLabel;
        [SerializeField] private Text honorTotalValueText;

        [Space(5)]
        [HeaderAttribute("HistoryActivityItemList")]
        [Space(5)]
        [SerializeField] private ComUIListScript historyActivityItemList;

        //[Space(5)] [HeaderAttribute("HistoryActivityMoreButton")] [Space(5)]
        //[SerializeField] private GameObject historyMoreButtonRoot;
        //[SerializeField] private Button historyMoreButton;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
            //if (historyMoreButton != null)
            //{
            //    historyMoreButton.onClick.RemoveAllListeners();
            //    historyMoreButton.onClick.AddListener(OnMoreButtonClick);
            //}

            if (historyActivityItemList != null)
            {
                historyActivityItemList.Initialize();
                historyActivityItemList.onItemVisiable += OnHistoryActivityItemVisible;
            }
        }

        private void UnBindUiEvents()
        {
            //if(historyMoreButton != null)
            //    historyMoreButton.onClick.RemoveAllListeners();

            if (historyActivityItemList != null)
            {
                historyActivityItemList.onItemVisiable -= OnHistoryActivityItemVisible;
            }
        }

        private void ClearData()
        {
            _playerHistoryHonorInfo = null;
            _totalPvpNumberStatisticsList = null;
            _curPvpNumberStatisticsList.Clear();
        }

        public void InitItem(HONOR_DATE_TYPE honorDateType)
        {
            _honorDateType = honorDateType;

            _playerHistoryHonorInfo = HonorSystemUtility.GetPlayerHistoryHonorInfoByDateType(_honorDateType);
            if (_playerHistoryHonorInfo == null)
            {
                Logger.LogErrorFormat("HistoryHonorInfo is null");
                CommonUtility.UpdateGameObjectVisible(gameObject, false);
                return;
            }

            InitTitle();
            InitHistoryTotalValue();

            InitHistoryActivityInfo();
        }

        private void InitTitle()
        {
            var preHistoryStr = TR.Value("Honor_System_Today");
            if (_honorDateType == HONOR_DATE_TYPE.HONOR_DATE_TYPE_LAST_WEEK)
                preHistoryStr = TR.Value("Honor_System_Pre_Week");
            else if (_honorDateType == HONOR_DATE_TYPE.HONOR_DATE_TYPE_THIS_WEEK)
                preHistoryStr = TR.Value("Honor_System_This_Week");

            if (preHistoryTitleLabel != null)
            {
                preHistoryTitleLabel.text = preHistoryStr;
            }
        }

        private void InitHistoryTotalValue()
        {
            if (honorTotalValueText != null)
                honorTotalValueText.text = _playerHistoryHonorInfo.HonorTotalNumber.ToString();
        }

        private void InitHistoryActivityInfo()
        {
            _totalPvpNumberStatisticsList = _playerHistoryHonorInfo.PvpNumberStatisticsList;

            if (_totalPvpNumberStatisticsList == null)
                return;

            _curPvpNumber = 0;
            _curPvpNumberStatisticsList.Clear();
            //bool isShowMoreButton = false;

            if (_totalPvpNumberStatisticsList.Count > 0)
            {
                //var totalNumber = _totalPvpNumberStatisticsList.Count;
                //if (totalNumber > HonorSystemDataManager.GetInstance().HonorHistoryActivityNumber)
                //{
                //    //isShowMoreButton = true;
                //    _curPvpNumber = HonorSystemDataManager.GetInstance().HonorHistoryActivityNumber;
                //}
                //else
                //{
                //    //isShowMoreButton = false;
                //    _curPvpNumber = totalNumber;
                //}

                _curPvpNumber = _totalPvpNumberStatisticsList.Count;
                for (var i = 0; i < _curPvpNumber; i++)
                {
                    _curPvpNumberStatisticsList.Add(_totalPvpNumberStatisticsList[i]);
                }
            }

            //CommonUtility.UpdateGameObjectVisible(historyMoreButtonRoot, isShowMoreButton);

            if(historyActivityItemList != null)
                historyActivityItemList.SetElementAmount(_curPvpNumber);
        }

        private void OnHistoryActivityItemVisible(ComUIListElementScript item)
        {
            if (historyActivityItemList == null)
                return;

            if (item == null)
                return;

            if (_curPvpNumberStatisticsList == null || _curPvpNumberStatisticsList.Count < 0)
                return;

            if (item.m_index < 0 || item.m_index >= _curPvpNumberStatisticsList.Count)
                return;

            var curPvpNumberStatistics = _curPvpNumberStatisticsList[item.m_index];
            var honorCommonItem = item.GetComponent<HonorCommonItem>();

            if (honorCommonItem != null && curPvpNumberStatistics != null)
                honorCommonItem.InitItem(curPvpNumberStatistics);
        }

        //private void OnMoreButtonClick()
        //{
        //    CommonUtility.UpdateGameObjectVisible(historyMoreButtonRoot, false);

        //    if (_totalPvpNumberStatisticsList == null)
        //        return;

        //    _curPvpNumber = _totalPvpNumberStatisticsList.Count;
        //    _curPvpNumberStatisticsList.Clear();

        //    for (var i = 0; i < _curPvpNumber; i++)
        //    {
        //        _curPvpNumberStatisticsList.Add(_totalPvpNumberStatisticsList[i]);
        //    }

        //    if (historyActivityItemList != null)
        //        historyActivityItemList.SetElementAmount(_curPvpNumber);

        //}

    }
}