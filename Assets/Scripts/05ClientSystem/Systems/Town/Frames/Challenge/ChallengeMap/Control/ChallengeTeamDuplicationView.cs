using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ChallengeTeamDuplicationView : MonoBehaviour
    {

        private List<int> _dropItemIdList;
        private DungeonModelTable _dungeonModelTable;

        [Space(25)]
        [HeaderAttribute("")]
        [SerializeField] private Text levelLabel;

        [SerializeField] private ComUIListScriptEx dropItemList;

        [SerializeField] private ComButtonWithCd forwardButton;
        [SerializeField] private UIGray forwardButtonUiGray;

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
            if (forwardButton != null)
            {
                forwardButton.ResetListener();
                forwardButton.SetButtonListener(OnForwardButtonClick);
            }

            if (dropItemList != null)
            {
                dropItemList.Initialize();
                dropItemList.onItemVisiable += OnItemVisible;
            }
        }

        private void UnBindUiEvents()
        {
            if (forwardButton != null)
            {
                forwardButton.ResetButtonListener();
            }

            if (dropItemList != null)
            {
                dropItemList.onItemVisiable -= OnItemVisible;
            }
        }


        private void OnEnable()
        {
            BindUiMessages();
        }

        private void OnDisable()
        {
            UnBindUiMessages();
        }


        private void BindUiMessages()
        {
            //团本功能解锁
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewFuncUnlock, OnReceiveNewFuncUnLockMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateUnlockFunc, OnUpdateUnlockFuncMessage);
        }

        private void UnBindUiMessages()
        {

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewFuncUnlock, OnReceiveNewFuncUnLockMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateUnlockFunc, OnUpdateUnlockFuncMessage);
        }

        private void ClearData()
        {
            _dropItemIdList = null;
            _dungeonModelTable = null;
        }

        //需要传递默认的参数
        public void InitView()
        {
            _dungeonModelTable = ChallengeUtility.GetDungeonModelTableOfTeamDuplication();
            if (_dungeonModelTable == null)
                return;

            _dropItemIdList = _dungeonModelTable.DropShow.ToList();

            InitCommonView();

        }

        private void InitCommonView()
        {
            if (levelLabel != null)
            {
                levelLabel.text = string.Format(TR.Value("team_duplication_unlock_level_format"),
                    _dungeonModelTable.Level);
            }

            if (dropItemList != null && _dropItemIdList != null)
            {
                dropItemList.SetElementAmount(_dropItemIdList.Count);
            }
            
            UpdateForwardButtonStatus();
        }

        public void OnEnableView()
        {
            UpdateForwardButtonStatus();
        }

        private void OnForwardButtonClick()
        {
            ChallengeUtility.OnCloseChallengeMapFrame();
            TeamDuplicationUtility.EnterToTeamDuplicationSceneFromTown();
            
        }

        //团本商店
        private void OnTeamDuplicationShopButtonClick()
        {
            ShopNewDataManager.GetInstance().OpenShopNewFrame(TeamDuplicationDataManager.TeamDuplicationShopId);
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (dropItemList == null)
                return;

            if (_dropItemIdList == null || _dropItemIdList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _dropItemIdList.Count)
                return;

            var dropItemId = _dropItemIdList[item.m_index];

            var chapterDropItem = item.GetComponent<ChallengeChapterDropItem>();

            if (chapterDropItem != null && dropItemId > 0)
            {
                chapterDropItem.InitItem(dropItemId);
            }
        }

        #region UIEvent
        //新的功能解锁
        private void OnReceiveNewFuncUnLockMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            FunctionUnLock.eFuncType funcType = (FunctionUnLock.eFuncType)uiEvent.Param2;

            //不是团本
            if (funcType != FunctionUnLock.eFuncType.TeamCopy)
                return;

            UpdateForwardButtonStatus();
        }

        //功能更新
        private void OnUpdateUnlockFuncMessage(UIEvent uiEvent)
        {
            UpdateForwardButtonStatus();
        }

        #endregion

        private void UpdateForwardButtonStatus()
        {
            var isShowTeamDuplication = TeamDuplicationUtility.IsShowTeamDuplication();

            forwardButton.UpdateButtonState(isShowTeamDuplication);
            CommonUtility.UpdateGameObjectUiGray(forwardButtonUiGray, !isShowTeamDuplication);
        }

    }
}