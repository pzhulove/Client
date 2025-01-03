using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{

    public delegate void OnTeamDifficultyClickedAction(uint hardLevel);

    //团本创建难度控制器
    public class TeamDuplicationTeamDifficultyControl : MonoBehaviour
    {

        private enum SelectedButtonType
        {
            Invalid = -1,
            NormalButtonType,
            HardButtonType,
        };


        private SelectedButtonType _selectedButtonType;

        private delegate void PointerDownMethod(BaseEventData baseEventData);

        private OnTeamDifficultyClickedAction _onTeamDifficultyClickedAction;
        private TeamDuplicationPlayerInformationDataModel _playerInformationDataModel;
        private bool _isHardLevelUnlock = false;

        private GameObject _normalSelectedEffectPrefab;
        private GameObject _hardSelectedEffectPrefab;
        private GameObject _pointerDownEffectPrefab;

        [Space(15)]
        [HeaderAttribute("NormalRoot")]
        [Space(5)]
        [SerializeField] private Button normalLevelButton;
        [SerializeField] private GameObject normalSelectedEffectRoot;
        [SerializeField] private GameObject normalPointerDownEffectRoot;

        [Space(15)]
        [HeaderAttribute("HardRoot")]
        [Space(5)]
        [SerializeField] private Button hardLevelButton;
        [SerializeField] private UIGray hardLevelButtonUIGray;
        [SerializeField] private GameObject hardSelectedEffectRoot;
        [SerializeField] private GameObject hardPointerDownEffectRoot;
        [Space(15)]
        [HeaderAttribute("HardLevelDescription")]
        [Space(5)]
        [SerializeField] private GameObject hardLevelDescriptionRoot;
        [SerializeField] private Text hardLevelDescriptionLabel;

        [Space(15)]
        [HeaderAttribute("pointerDownRoot")]
        [Space(5)]
        [SerializeField] private GameObject pointerDownEffectRoot;

        private void Awake()
        {
            if (normalLevelButton != null)
            {
                normalLevelButton.onClick.RemoveAllListeners();
                normalLevelButton.onClick.AddListener(OnNormalLevelButtonClicked);

                AddPointerDownTriggerListener(normalLevelButton.gameObject, 
                    OnNormalLevelButtonPointerDownClicked);
            }

            if (hardLevelButton != null)
            {
                hardLevelButton.onClick.RemoveAllListeners();
                hardLevelButton.onClick.AddListener(OnHardLevelButtonClicked);

                AddPointerDownTriggerListener(hardLevelButton.gameObject,
                    OnHardLevelButtonPointerDownClicked);
            }
        }

        private void OnDestroy()
        {
            if(normalLevelButton != null)
                normalLevelButton.onClick.RemoveAllListeners();

            if(hardLevelButton != null)
                hardLevelButton.onClick.RemoveAllListeners();

            _onTeamDifficultyClickedAction = null;
            _playerInformationDataModel = null;
            _isHardLevelUnlock = false;
            _normalSelectedEffectPrefab = null;
            _hardSelectedEffectPrefab = null;
            _pointerDownEffectPrefab = null;
            _selectedButtonType = SelectedButtonType.Invalid;
        }

        public void Init(OnTeamDifficultyClickedAction onTeamDifficultyClickedAction)
        {
            _onTeamDifficultyClickedAction = onTeamDifficultyClickedAction;
            _playerInformationDataModel = TeamDuplicationDataManager.GetInstance().GetPlayerInformationDataModel();

            if (_playerInformationDataModel == null)
                return;

            InitControlView();
        }

        private void InitControlView()
        {
            //默认选中普通难度
            CommonUtility.UpdateGameObjectVisible(normalSelectedEffectRoot, true);
            UpdateNormalSelectedEffect(normalSelectedEffectRoot);
            _selectedButtonType = SelectedButtonType.NormalButtonType;


            InitHardLevelContent();
        }

        //噩梦难度相关
        private void InitHardLevelContent()
        {
            CommonUtility.UpdateGameObjectVisible(hardSelectedEffectRoot, false);

            _isHardLevelUnlock = IsHardLevelUnLock();
            if (_isHardLevelUnlock == true)
            {
                //已经解锁
                CommonUtility.UpdateGameObjectVisible(hardLevelDescriptionRoot, false);
                CommonUtility.UpdateButtonState(hardLevelButton, hardLevelButtonUIGray, true);
            }
            else
            {
                //没有解锁
                CommonUtility.UpdateButtonState(hardLevelButton, hardLevelButtonUIGray, false);

                //难度解锁介绍
                CommonUtility.UpdateGameObjectVisible(hardLevelDescriptionRoot, true);
                if (hardLevelDescriptionLabel != null)
                {
                    hardLevelDescriptionLabel.text = string.Format(
                        TR.Value("team_duplication_team_difficulty_unlock_hard_require"),
                        _playerInformationDataModel.CommonLevelPassNumber,
                        _playerInformationDataModel.UnLockCommonLevelTotalNumber);
                }
            }
        }

        #region Button
        //普通难度按钮
        private void OnNormalLevelButtonClicked()
        {

            _selectedButtonType = SelectedButtonType.NormalButtonType;

            CommonUtility.UpdateGameObjectVisible(normalSelectedEffectRoot, true);
            UpdateNormalSelectedEffect(normalSelectedEffectRoot);

            //已经解锁
            if (_isHardLevelUnlock == true)
            {
                CommonUtility.UpdateGameObjectVisible(hardSelectedEffectRoot, false);
            }

            if (_onTeamDifficultyClickedAction != null)
            {
                _onTeamDifficultyClickedAction((uint) TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_COMMON);
            }
        }

        //噩梦难度按钮
        private void OnHardLevelButtonClicked()
        {
            if (_isHardLevelUnlock == false)
                return;

            CommonUtility.UpdateGameObjectVisible(normalSelectedEffectRoot, false);

            _selectedButtonType = SelectedButtonType.HardButtonType;

            CommonUtility.UpdateGameObjectVisible(hardSelectedEffectRoot, true);
            UpdateHardSelectedEffect(hardSelectedEffectRoot);

            if (_onTeamDifficultyClickedAction != null)
                _onTeamDifficultyClickedAction((uint) TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_DIFF);
        }

        #endregion

        #region Helper
        //噩梦难度是否解锁
        private bool IsHardLevelUnLock()
        {
            if (_playerInformationDataModel == null)
                return false;

            if (_playerInformationDataModel.AlreadyOpenDifficultyList == null
                || _playerInformationDataModel.AlreadyOpenDifficultyList.Count <= 0)
                return false;

            for (var i = 0; i < _playerInformationDataModel.AlreadyOpenDifficultyList.Count; i++)
            {
                var curUnlockLevel = _playerInformationDataModel.AlreadyOpenDifficultyList[i];
                if ((TeamCopyTeamGrade)curUnlockLevel == TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_DIFF)
                    return true;
            }

            return false;
        }
        #endregion

        #region ButtonSelectedEffect

        //普通按钮的特效
        private void UpdateNormalSelectedEffect(GameObject effectRoot)
        {
            if (effectRoot == null)
                return;

            if (_normalSelectedEffectPrefab != null)
                return;

            _normalSelectedEffectPrefab = CommonUtility.LoadGameObject(effectRoot);
        }

        //难度按钮的特效
        private void UpdateHardSelectedEffect(GameObject effectRoot)
        {
            if (effectRoot == null)
                return;

            if (_hardSelectedEffectPrefab != null)
                return;

            _hardSelectedEffectPrefab = CommonUtility.LoadGameObject(effectRoot);
        }

        #endregion

        #region EventTrigger

        //噩梦难度按钮
        private void OnHardLevelButtonPointerDownClicked(BaseEventData baseEventData)
        {
            //没有解锁
            if (_isHardLevelUnlock == false)
                return;

            if (_selectedButtonType == SelectedButtonType.HardButtonType)
                return;

            SetPointerDownEffectPrefab(hardPointerDownEffectRoot);
        }

        //普通难度按钮
        private void OnNormalLevelButtonPointerDownClicked(BaseEventData baseEventData)
        {
            //已经选中NormalButton
            if (_selectedButtonType == SelectedButtonType.NormalButtonType)
                return;

            SetPointerDownEffectPrefab(normalPointerDownEffectRoot);
        }

        //设置点击特效的位置
        private void SetPointerDownEffectPrefab(GameObject buttonEffectRoot)
        {
            if (buttonEffectRoot == null)
                return;

            if (pointerDownEffectRoot == null)
                return;

            if (_pointerDownEffectPrefab == null)
                _pointerDownEffectPrefab = CommonUtility.LoadGameObject(pointerDownEffectRoot);

            if (_pointerDownEffectPrefab != null)
            {
                _pointerDownEffectPrefab.CustomActive(false);
                _pointerDownEffectPrefab.CustomActive(true);
            }

            pointerDownEffectRoot.transform.SetParent(buttonEffectRoot.transform, false);
            pointerDownEffectRoot.transform.localPosition = Vector3.zero;
        }

        //对某一个物体添加PointerDown点击事件
        private void AddPointerDownTriggerListener(GameObject clickObject,
            PointerDownMethod pointerDownMethod)
        {
            EventTrigger eventTrigger = clickObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = clickObject.AddComponent<EventTrigger>();

            eventTrigger.triggers = new List<EventTrigger.Entry>();

            UnityAction<BaseEventData> callBack = new UnityAction<BaseEventData>(pointerDownMethod);
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(callBack);

            eventTrigger.triggers.Add(entry);
        }
        #endregion
    }
}
