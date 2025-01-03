using System;
using System.Collections.Generic;
using System.Collections;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class WeekSignInCommonControl : MonoBehaviour
    {
        private WeekSignInType _weekSignInType = WeekSignInType.None;
        private List<WeekSignInPreviewAwardDataModel> _previewAwardItemDataModelList;

        [Space(15)]
        [HeaderAttribute("CommonControl")]
        [Space(10)]

        [SerializeField] private Text weekSignInTimeText;
        [SerializeField] private Button weekSignInAwardRecordButton;
        [SerializeField] private CommonHelpNewAssistant weekSignInHelpButton;
        [SerializeField] private ComUIListScript weekSignInDetailItemList;

        #region Init
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }
        
        private void ClearData()
        {
            _weekSignInType = WeekSignInType.None;
            _previewAwardItemDataModelList = null;
        }

        private void BindEvents()
        {
            if (weekSignInAwardRecordButton != null)
            {
                weekSignInAwardRecordButton.onClick.RemoveAllListeners();
                weekSignInAwardRecordButton.onClick.AddListener(OnAwardRecordButtonClick);
            }

            if (weekSignInDetailItemList != null)
            {
                weekSignInDetailItemList.onItemVisiable += OnItemVisible;
            }
        }

        private void UnBindEvents()
        {
            if (weekSignInAwardRecordButton != null)
                weekSignInAwardRecordButton.onClick.RemoveAllListeners();

            if (weekSignInDetailItemList != null)
                weekSignInDetailItemList.onItemVisiable -= OnItemVisible;
        }

        #endregion

        public void InitCommonControl(WeekSignInType weekSignInType)
        {
            _weekSignInType = weekSignInType;
            _previewAwardItemDataModelList = WeekSignInDataManager.GetInstance()
                .GetPreviewAwardItemDataModelListByWeekSignInType(_weekSignInType);

            InitCommonView();
        }

        private void InitCommonView()
        {
            //签到持续的时间
            if (weekSignInTimeText != null)
                weekSignInTimeText.text = WeekSignInDataManager.GetInstance()
                    .GetWeekSignInTimeLabelByWeekSignInType(_weekSignInType);

            //新人周签到
            if (_weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                if (weekSignInHelpButton != null)
                    weekSignInHelpButton.HelpId = (int)WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId;
            }
            else
            {
                if (weekSignInHelpButton != null)
                    weekSignInHelpButton.HelpId = (int)WeekSignInDataManager.ActivityWeekSignInOpActTypeId;
            }

            var previewAwardItemCount = 0;
            if (_previewAwardItemDataModelList != null && _previewAwardItemDataModelList.Count > 0)
                previewAwardItemCount = _previewAwardItemDataModelList.Count;

            if (weekSignInDetailItemList != null)
            {
                weekSignInDetailItemList.Initialize();
                weekSignInDetailItemList.SetElementAmount(previewAwardItemCount);
            }

        }

        public void OnEnableCommonControl()
        {

        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_previewAwardItemDataModelList == null || _previewAwardItemDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _previewAwardItemDataModelList.Count)
                return;

            var previewAwardItemDataModel = _previewAwardItemDataModelList[item.m_index];
            var previewAwardItem = item.GetComponent<WeekSignInPreviewAwardItem>();

            if (previewAwardItemDataModel != null && previewAwardItem != null)
                previewAwardItem.InitItem(previewAwardItemDataModel);
        }

        private void OnAwardRecordButtonClick()
        {
            if (_weekSignInType == WeekSignInType.None)
            {
                Logger.LogErrorFormat("WeekSingInType is None");
                return;
            }

            WeekSignInUtility.OnOpenWeekSignInAwardRecordFrame((int)_weekSignInType);
        }

    }
}