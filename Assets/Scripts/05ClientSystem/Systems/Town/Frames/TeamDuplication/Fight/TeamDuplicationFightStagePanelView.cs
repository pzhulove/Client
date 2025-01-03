using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightStagePanelView : MonoBehaviour
    {

        private int _fightStageId;
        private TeamCopyStageTable _fightStageTable = null;

        //目标数据
        private List<ComControlData> _fightGoalDataModelList = null;

        //选中的据点
        private TeamDuplicationFightPointDataModel _selectedFightPointDataModel = null;
        //据点的Item，展示据点
        private Dictionary<int, TeamDuplicationFightStagePointItem> _teamDuplicationFightStagePointItemDic = new Dictionary<
            int, TeamDuplicationFightStagePointItem>();

        //临时保存增加的TeamDuplicationFightPointStagePointItem;
        private Dictionary<int, TeamDuplicationFightStagePointItem> _tempAddNewTeamDuplicationFightStagePointItemDic =
            new Dictionary<
                int, TeamDuplicationFightStagePointItem>();
        //当前阶段据点位置的List
        private List<TeamDuplicationFightPointItemWithPosition> _curStageFightPointItemWithPositionList;

        [Space(25)]
        [HeaderAttribute("Background")]
        [Space(10)]
        [SerializeField] private Image normalBackgroundImage;
        [SerializeField] private Image extraBackgroundImage;

        [Space(25)]
        [HeaderAttribute("Normal")]
        [Space(10)]
        [SerializeField] private Text titleLabel;
        [SerializeField] private Button closeButton;
        [SerializeField] private ComButtonWithCd startFightButton;
        [SerializeField] private UIGray startFightButtonUIGray;
        [SerializeField] private GameObject startButtonEffectRoot;

        [Space(25)]
        [HeaderAttribute("FightPointItem")]
        [Space(10)]
        [SerializeField] private GameObject fightPointItemTemplate;

        //据点的位置，可以在预制体上调节
        [Space(5)]
        [HeaderAttribute("FirstStageFightPointItemPositionList")]
        [Space(5)]
        [SerializeField]
        private List<TeamDuplicationFightPointItemWithPosition> firstStageFightPointItemWithPositionList
            = new List<TeamDuplicationFightPointItemWithPosition>();

        [Space(5)]
        [HeaderAttribute("SecondStageFightPointItemPositionList")]
        [Space(5)]
        [SerializeField]
        private List<TeamDuplicationFightPointItemWithPosition> secondStageFightPointItemWithPositionList
            = new List<TeamDuplicationFightPointItemWithPosition>();

        [Space(25)]
        [HeaderAttribute("FightStageGoalControl")]
        [Space(10)]
        [SerializeField]
        private TeamDuplicationFightStageGoalControl fightStageGoalControl; 

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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (startFightButton != null)
            {
                startFightButton.ResetButtonListener();
                startFightButton.SetButtonListener(OnStartFightButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }

            if (startFightButton != null)
            {
                startFightButton.ResetButtonListener();
            }
        }

        private void ClearData()
        {
            _fightStageId = 0;
            _fightStageTable = null;
            _fightGoalDataModelList = null;
            _selectedFightPointDataModel = null;

            _teamDuplicationFightStagePointItemDic.Clear();
            _tempAddNewTeamDuplicationFightStagePointItemDic.Clear();
            _curStageFightPointItemWithPositionList = null;
        }

        private void OnEnable()
        {
            BindUiMessage();
        }

        private void OnDisable()
        {
            UnBindUiMessage();
        }

        private void BindUiMessage()
        {
            //据点数据的通知
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightPointNotifyMessage,
                OnReceiveTeamDuplicationFightPointNotifyMessage);

            //小队目标数据的通知
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightGoalNotifyMessage,
                OnReceiveTeamDuplicationFightGoalNotifyMessage);
            //小队目标ID改变的通知
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightCaptainGoalChangeMessage,
                OnReceiveTeamDuplicationFightCaptainGoalChangeMessage);

            //团本数据的通知
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);

            //能量蓄积据点时间刷新
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage,
                OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage);
        }

        private void UnBindUiMessage()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointNotifyMessage,
                OnReceiveTeamDuplicationFightPointNotifyMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightGoalNotifyMessage,
                OnReceiveTeamDuplicationFightGoalNotifyMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightCaptainGoalChangeMessage,
                OnReceiveTeamDuplicationFightCaptainGoalChangeMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage,
                OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage);
        }

        public void Init(int fightStageId)
        {
            _fightStageId = fightStageId;

            _fightStageTable = TableManager.GetInstance().GetTableItem<TeamCopyStageTable>(_fightStageId);
            if (_fightStageTable == null)
                return;

            //根据阶段ID，确定据点位置的List,默认选择第一阶段的据点
            _curStageFightPointItemWithPositionList = firstStageFightPointItemWithPositionList;
            if (_fightStageId == 2)
            {
                _curStageFightPointItemWithPositionList = secondStageFightPointItemWithPositionList;
            }

            InitView();
        }

        private void InitView()
        {
            InitCommonView();

            InitFightPointItemPosition();

            InitFightPanelContent();
        }

        private void InitCommonView()
        {
            if (titleLabel != null)
                titleLabel.text = _fightStageTable.Description;

            if (normalBackgroundImage != null)
            {
                ETCImageLoader.LoadSprite(ref normalBackgroundImage, _fightStageTable.NormalBackgroundPath);
            }

            if (extraBackgroundImage != null)
            {
                if (string.IsNullOrEmpty(_fightStageTable.ExtraBackgroundPath) == true)
                {
                    CommonUtility.UpdateImageVisible(extraBackgroundImage, false);
                }
                else
                {
                    CommonUtility.UpdateImageVisible(extraBackgroundImage, true);
                    ETCImageLoader.LoadSprite(ref extraBackgroundImage, _fightStageTable.ExtraBackgroundPath);
                }
            }
        }

        //初始化据点的位置
        private void InitFightPointItemPosition()
        {
            if (fightPointItemTemplate != null)
                fightPointItemTemplate.CustomActive(false);

            if (_curStageFightPointItemWithPositionList == null
                || _curStageFightPointItemWithPositionList.Count <= 0)
                return;

            if (_teamDuplicationFightStagePointItemDic == null)
                return;
            _teamDuplicationFightStagePointItemDic.Clear();

            //添加据点的位置
            for (var i = 0; i < _curStageFightPointItemWithPositionList.Count; i++)
            {
                var fightPointItemWithPosition = _curStageFightPointItemWithPositionList[i];
                if (fightPointItemWithPosition == null)
                    continue;

                var fightItemPositionIndex = fightPointItemWithPosition.PositionIndex;
                _teamDuplicationFightStagePointItemDic[fightItemPositionIndex] = null;
            }
        }

        //更新据点
        private void UpdateFightPointItemList()
        {
            //团本据点的数据
            var fightPointDataModelList =
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightPointDataModelList;

            _tempAddNewTeamDuplicationFightStagePointItemDic.Clear();

            //遍历Dic，只读类型，Dic里的数据不能add，remove和改变
            foreach (var curItem in _teamDuplicationFightStagePointItemDic)
            {
                var positionIndex = curItem.Key;
                var fightPointItem = curItem.Value;

                var isFindFightPointDataModel = false;
                TeamDuplicationFightPointDataModel fightPointDataModel = null;

                //查找据点的数据
                if (fightPointDataModelList != null)
                {
                    for (var i = 0; i < fightPointDataModelList.Count; i++)
                    {
                        var curFightPointDataModel = fightPointDataModelList[i];
                        if (curFightPointDataModel == null)
                            continue;

                        if (curFightPointDataModel.FightPointPosition == positionIndex)
                        {
                            isFindFightPointDataModel = true;
                            fightPointDataModel = curFightPointDataModel;
                            break;
                        }
                    }
                }

                if (isFindFightPointDataModel == false)
                {
                    //不存在相应的据点
                    if (fightPointItem != null)
                        CommonUtility.UpdateGameObjectVisible(fightPointItem.gameObject, false);
                }
                else
                {
                    //存在相应的据点，并且需要显示,创建
                    if (TeamDuplicationUtility.IsFightPointShowInFightPanel(fightPointDataModel) == true)
                    {
                        //加载LoadPointItem
                        if (fightPointItem == null)
                        {
                            fightPointItem = LoadFightPointItem(positionIndex);
                            //新增加的Item，先保存到临时缓存中，当Dic遍历完之后，保存到对应的Dic中
                            _tempAddNewTeamDuplicationFightStagePointItemDic[positionIndex] = fightPointItem;
                        }

                        if (fightPointItem != null)
                        {
                            CommonUtility.UpdateGameObjectVisible(fightPointItem.gameObject, true);
                            fightPointItem.Init(fightPointDataModel,
                                OnFightPointButtonClick);

                        }
                    }
                    else
                    {
                        //存在相应的据点，并且不显示
                        if (fightPointItem != null)
                            CommonUtility.UpdateGameObjectVisible(fightPointItem.gameObject, false);
                    }
                }
            }

            //将临时缓存中的数据保存到对应的Dic中
            foreach (var addItem in _tempAddNewTeamDuplicationFightStagePointItemDic)
            {
                var addItemPositionIndex = addItem.Key;
                var addFightPointItem = addItem.Value;
                //据点数据不为null
                if (addFightPointItem != null)
                    _teamDuplicationFightStagePointItemDic[addItemPositionIndex] = addFightPointItem;
            }
            _tempAddNewTeamDuplicationFightStagePointItemDic.Clear();
            
        }


        private void InitFightPanelContent()
        {
            UpdateFightPointItemList();

            _selectedFightPointDataModel = TeamDuplicationUtility.GetDefaultSelectedFightPointDataModel();
            UpdateFightPointItemSelectedState();

            UpdateFightPanelButton();
            InitFightGoalControl();
        }

        #region Update
        //点击据点
        private void OnFightPointButtonClick(TeamDuplicationFightPointDataModel fightPointDataModel)
        {
            if (fightPointDataModel == null)
                return;

            //重复点击，直接返回，不用刷新
            if (_selectedFightPointDataModel != null
                && _selectedFightPointDataModel.FightPointId == fightPointDataModel.FightPointId)
                return;

            _selectedFightPointDataModel = fightPointDataModel;
            UpdateFightPointItemSelectedState();

            UpdateFightPanelButton();

            //更新据点的描述
            UpdateFightPointDescriptionView(_selectedFightPointDataModel);
        }

        //更新进入战斗的按钮
        private void UpdateFightPanelButton()
        {
            UpdateFightStartButton();
        }

        private void UpdateFightStartButton()
        {
            if (startFightButton == null)
                return;

            //没有选中据点，不显示
            if (_selectedFightPointDataModel == null)
            {
                CommonUtility.UpdateGameObjectVisible(startFightButton.gameObject, false);
                return;
            }

            //非队长不显示
            if (TeamDuplicationUtility.IsSelfPlayerIsCaptainInTeamDuplication() == false)
            {
                CommonUtility.UpdateGameObjectVisible(startFightButton.gameObject, false);
                return;
            }

            //重生状态不显示
            if (_selectedFightPointDataModel.FightPointStatusType 
                == TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_REBORN)
            {
                CommonUtility.UpdateGameObjectVisible(startFightButton.gameObject, false);
                return;
            }

            //状态解锁中
            if (_selectedFightPointDataModel.FightPointStatusType ==
                TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_UNLOCKING)
            {
                //倒计时暂停
                startFightButton.StopCountDown();

                //按钮显示，不可点击并且置灰
                CommonUtility.UpdateGameObjectVisible(startFightButton.gameObject, true);
                startFightButton.UpdateButtonState(false);
                CommonUtility.UpdateUIGrayVisible(startFightButtonUIGray, true);
                //特效不显示
                CommonUtility.UpdateGameObjectVisible(startButtonEffectRoot, false);
                return;
            }

            //能量蓄积状态
            if (_selectedFightPointDataModel.FightPointStatusType
                == TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_ENERGY_REVIVE)
            {
                UpdateFightPointStartButtonByEnergyAccumulationStatus();
                return;
            }

            //展示按钮，展示特效
            CommonUtility.UpdateGameObjectVisible(startFightButton.gameObject, true);
            startFightButton.Reset();
            CommonUtility.UpdateUIGrayVisible(startFightButtonUIGray, false);
            CommonUtility.UpdateGameObjectVisible(startButtonEffectRoot, true);
        }

        //能量蓄积对按钮的影响
        private void UpdateFightPointStartButtonByEnergyAccumulationStatus()
        {
            if (TimeManager.GetInstance().GetServerTime() -
                _selectedFightPointDataModel.FightPointEnergyAccumulationStartTime > 50)
            {
                //按钮正常，可以点击
                CommonUtility.UpdateGameObjectVisible(startFightButton.gameObject, true);
                startFightButton.Reset();
                CommonUtility.UpdateUIGrayVisible(startFightButtonUIGray, false);

                //展示特效
                CommonUtility.UpdateGameObjectVisible(startButtonEffectRoot, true);
            }
            else
            {
                //倒计时暂停
                startFightButton.StopCountDown();

                //按钮显示，不可点击并且置灰
                CommonUtility.UpdateGameObjectVisible(startFightButton.gameObject, true);
                startFightButton.UpdateButtonState(false);
                CommonUtility.UpdateUIGrayVisible(startFightButtonUIGray, true);

                //不展示特效
                CommonUtility.UpdateGameObjectVisible(startButtonEffectRoot, false);
            }
        }

        //更新据点选中的状态
        private void UpdateFightPointItemSelectedState()
        {
            var selectedFightPointDataModel = _selectedFightPointDataModel;
            foreach (var curItem in _teamDuplicationFightStagePointItemDic)
            {
                var fightPointItem = curItem.Value;
                if (fightPointItem == null)
                    continue;

                var curFightPointDataModel = fightPointItem.GetFightPointDataModel();
                if (curFightPointDataModel == null)
                    continue;

                if (selectedFightPointDataModel == null)
                {
                    fightPointItem.UpdateFightPointSelectedView(false);
                }
                else
                {
                    if (curFightPointDataModel.FightPointId == selectedFightPointDataModel.FightPointId)
                    {
                        fightPointItem.UpdateFightPointSelectedView(true);
                    }
                    else
                    {
                        fightPointItem.UpdateFightPointSelectedView(false);
                    }
                }
            }
        }

        #endregion

        #region FightStagePanelGoalControl
        //初始化目标Control
        private void InitFightGoalControl()
        {
            _fightGoalDataModelList = TeamDuplicationUtility.GetFightGoalDataModelList(_selectedFightPointDataModel);
            if (fightStageGoalControl != null)
                fightStageGoalControl.Init(_fightGoalDataModelList);
        }

        //更新目标的内容
        private void UpdateFightGoalView()
        {
            if (fightStageGoalControl != null)
                fightStageGoalControl.UpdateFightStageGoalView();
        }

        //更新据点的描述内容
        private void UpdateFightPointDescriptionView(
            TeamDuplicationFightPointDataModel selectedFightPointDataModel)
        {
            if (selectedFightPointDataModel == null)
                return;

            if (fightStageGoalControl != null)
                fightStageGoalControl.UpdateFightStageFightPointDescriptionView(selectedFightPointDataModel);
        }

        #endregion


        #region UIEvent

        //据点数据修改
        private void OnReceiveTeamDuplicationFightPointNotifyMessage(UIEvent uiEvent)
        {
            UpdateFightPointItemList();

            //选中的据点不存在或者选择的据点被隐藏，重新选择展示的据点
            if (_selectedFightPointDataModel == null || (_selectedFightPointDataModel != null
                && TeamDuplicationUtility.IsSelectFightPointInFightPointList(_selectedFightPointDataModel
                    .FightPointId) == false))
            {
                _selectedFightPointDataModel = TeamDuplicationUtility.GetDefaultSelectedFightPointDataModel();
                UpdateFightPointItemSelectedState();

                UpdateFightPanelButton();
                UpdateFightPointDescriptionView(_selectedFightPointDataModel);
            }
            else
            {
                //可能需要按钮重置，（据点的状态改变）
                UpdateFightPanelButton();
            }
        }

        //团本目标的内容改变，更新展示目标的内容
        private void OnReceiveTeamDuplicationFightGoalNotifyMessage(UIEvent uiEvent)
        {
            UpdateFightGoalView();
        }

        //小队目标Id改变
        private void OnReceiveTeamDuplicationFightCaptainGoalChangeMessage(UIEvent uiEvent)
        {
            //默认选中的据点不存在
            if (_selectedFightPointDataModel == null)
            {
                //默认选中某个据点
                _selectedFightPointDataModel = TeamDuplicationUtility.GetDefaultSelectedFightPointDataModel();
                UpdateFightPointItemSelectedState();
                UpdateFightPanelButton();
                UpdateFightPointDescriptionView(_selectedFightPointDataModel);
            }
        }

        //团本的数据改变，更新按钮（是否为小队队长改变）
        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            UpdateFightPanelButton();
        }

        //能量蓄积
        private void OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage(UIEvent uiEvent)
        {
            if (_selectedFightPointDataModel == null)
                return;

            if (_selectedFightPointDataModel.FightPointStatusType !=
                TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_ENERGY_REVIVE)
                return;
            
            UpdateFightPanelButton();
        }

        #endregion

        #region Button
        private void OnCloseButtonClick()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationFightStagePanelFrame();
        }

        private void OnStartFightButtonClick()
        {
            if (_selectedFightPointDataModel == null)
                return;

            TeamDuplicationDataManager.GetInstance()
                .OnSendTeamCopyStartChallengeReq(_selectedFightPointDataModel.FightPointId);
        }

        #endregion

        #region Helper
        //加载据点并设置位置
        private TeamDuplicationFightStagePointItem LoadFightPointItem(int fightPointIndex)
        {
            //据点的模板
            if (fightPointItemTemplate == null)
                return null;

            //据点所在的位置
            TeamDuplicationFightPointItemWithPosition fightPointItemWithPosition = null;
            for (var i = 0; i < _curStageFightPointItemWithPositionList.Count; i++)
            {
                var curFightPointItemWithPosition = _curStageFightPointItemWithPositionList[i];
                if (curFightPointItemWithPosition != null &&
                    curFightPointItemWithPosition.PositionIndex == fightPointIndex)
                {
                    fightPointItemWithPosition = curFightPointItemWithPosition;
                    break;
                }
            }

            if (fightPointItemWithPosition == null || fightPointItemWithPosition.FightPointItemRoot == null)
                return null;

            //创建新的据点
            var curFightPointItemGo = GameObject.Instantiate(fightPointItemTemplate) as GameObject;
            if (curFightPointItemGo == null)
                return null;

            //设置据点的位置
            curFightPointItemGo.transform.name = curFightPointItemGo.transform.name + "_" + fightPointIndex;
            curFightPointItemGo.transform.SetParent(fightPointItemWithPosition.FightPointItemRoot.transform,
                false);

            var teamDuplicationFightStagePointItem =
                curFightPointItemGo.GetComponent<TeamDuplicationFightStagePointItem>();
            if (teamDuplicationFightStagePointItem == null)
                return null;

            return teamDuplicationFightStagePointItem;
        }

        #endregion 

    }
}
