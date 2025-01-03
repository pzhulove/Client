using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class WeekSignInTotalAwardItem : MonoBehaviour
    {
        private WeekSignInType _weekSignInType;
        private WeekSignSumTable _weekSignSumTable;
        private WeekSignInAwardState _weekSignInAwardState;

        private CommonNewItem _commonNewItem;

        [Space(10)] [HeaderAttribute("item")] [Space(10)] [SerializeField]
        private GameObject itemRoot;

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]
        [SerializeField] private Text totalWeekLabel;
        [SerializeField] private GameObject totalAwardReceivedFlag;

        [SerializeField] private GameObject totalAwardFinishFlag;
        [SerializeField] private Button totalAwardItemButton;

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
            if (totalAwardItemButton != null)
            {
                totalAwardItemButton.onClick.RemoveAllListeners();
                totalAwardItemButton.onClick.AddListener(OnTotalAwardItemClicked);
            }
        }

        private void UnBindEvents()
        {
            if(totalAwardItemButton != null)
                totalAwardItemButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _weekSignInType = WeekSignInType.None;
            _weekSignSumTable = null;
            _weekSignInAwardState = WeekSignInAwardState.None;
            _commonNewItem = null;
        }


        public void InitItem(WeekSignInType weekSignInType,WeekSignSumTable weekSignSumTable)
        {
            _weekSignInType = weekSignInType;
            _weekSignSumTable = weekSignSumTable;

            if (_weekSignSumTable == null)
            {
                Logger.LogErrorFormat("WeekSignSumTable null");
                return;
            }

            if (_weekSignInType != WeekSignInType.ActivityWeekSignIn &&
                _weekSignInType != WeekSignInType.NewPlayerWeekSignIn)
                return;

            _weekSignInAwardState = WeekSignInUtility.GetWeekSignInTotalAwardState(_weekSignInType, _weekSignSumTable);

            InitItemView();
        }

        public void OnItemUpdate()
        {
            if (_weekSignSumTable == null)
                return;

            var curWeekSignInAwardState =
                WeekSignInUtility.GetWeekSignInTotalAwardState(_weekSignInType, _weekSignSumTable);
            if (curWeekSignInAwardState != _weekSignInAwardState)
            {
                _weekSignInAwardState = curWeekSignInAwardState;
                UpdateAwardItemState();
            }
        }

        private void InitItemView()
        {
            var commonNewItemDataModel = new CommonNewItemDataModel()
            {
                ItemId = _weekSignSumTable.rewardId,
                ItemCount = _weekSignSumTable.rewardNum
            };

            if (itemRoot != null)
            {
                _commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                if (_commonNewItem == null)
                    _commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);
            }

            if (_commonNewItem != null)
            {
                _commonNewItem.InitItem(commonNewItemDataModel);
                _commonNewItem.SetItemCountFontSize(28);
                _commonNewItem.SetItemLevelFontSize(28);
            }

            if (totalWeekLabel != null)
                totalWeekLabel.text = TR.Value("week_sing_in_total_week", _weekSignSumTable.weekSum);

            UpdateAwardItemState();
        }

        private void UpdateAwardItemState()
        {
            ResetItemFlag();

            if (_weekSignSumTable == null)
                return;

            if (_weekSignInAwardState == WeekSignInAwardState.Received)
            {
                UpdateFlag(totalAwardReceivedFlag, true);
            }
            else if (_weekSignInAwardState == WeekSignInAwardState.Finished)
            {
                UpdateFlag(totalAwardFinishFlag, true);
                UpdateTotalAwardItemButton(true);
            }
        }

        private void ResetItemFlag()
        {
            UpdateFlag(totalAwardFinishFlag, false);
            UpdateFlag(totalAwardReceivedFlag, false);
            UpdateTotalAwardItemButton(false);
        }

        private void UpdateTotalAwardItemButton(bool flag)
        {
            if (totalAwardItemButton != null)
                totalAwardItemButton.gameObject.CustomActive(flag);
        }

        private void UpdateFlag(GameObject flag, bool state)
        {
            if (flag != null)
                flag.CustomActive(state);
        }

        //显示ItemTipFrame
        private void OnTotalAwardItemClicked()
        {
            if (_weekSignSumTable == null)
                return;

            if (_weekSignInAwardState == WeekSignInAwardState.Finished)
            {
                //发送协议
                WeekSignInDataManager.GetInstance().SendSceneWeekSignRewardReq((uint)_weekSignSumTable.opActType,
                    (uint)_weekSignSumTable.weekSum);
            }
        }

    }
}
