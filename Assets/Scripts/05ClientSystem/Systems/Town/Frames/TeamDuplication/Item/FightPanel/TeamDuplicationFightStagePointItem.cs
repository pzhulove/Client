using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;
using Random = System.Random;

namespace GameClient
{

    public delegate void OnFightPointButtonClickAction(TeamDuplicationFightPointDataModel fightPointDataModel);

    //据点
    public class TeamDuplicationFightStagePointItem : MonoBehaviour
    {

        private bool _isTeamDuplicationFightPointStatusChanged = false;     //判断据点的状态是否改变
        private bool _isTeamDuplicationFightPointIdChanged = false;         //判断据点的Id是否改变，用于第一次初始化图片
        private TeamDuplicationFightPointDataModel _fightPointDataModel;
        private TeamCopyFieldTable _teamCopyFieldTable;

        private int _fightPointCurrentBossPhase = 0;                //boss当前的阶段，用于boss据点的icon替换

        private OnFightPointButtonClickAction _onFightPointButtonClickAction;

        //特殊状态控制器
        private TeamDuplicationFightPointRebornStatusControl _fightPointRebornStatusControl;
        private GameObject _fightPointUrgentStatusGameObject;
        private TeamDuplicationFightPointUnlockStatusControl _fightPointUnlockStatusControl;
        private TeamDuplicationFightPointEnergyAccumulationStatusControl _fightPointEnergyAccumulationStatusControl;

        //boss血量控制器
        private TeamDuplicationFightPointBossBloodControl _fightPointBossBloodControl;

        [Space(10)]
        [HeaderAttribute("Normal")]
        [Space(5)]
        [SerializeField] private Text fightPointNormalNameText;
        [SerializeField] private Text fightPointSelectedNameText;

        [SerializeField] private Button fightPointButton;
        [SerializeField] private GameObject fightPointSelectedRoot;


        [Space(15)]
        [HeaderAttribute("NormalFightPoint")]
        [Space(15)]
        [SerializeField] private GameObject normalBg;
        [SerializeField] private Image normalIcon;
        [SerializeField] private GameObject selectedBg;
        [SerializeField] private Image selectedIcon;

        [Space(15)] [HeaderAttribute("BossFightPoint")] [Space(15)]
        [SerializeField] private Image bossNormalIcon;
        [SerializeField] private Image bossSelectedIcon;

        [Space(10)]
        [HeaderAttribute("TeamAndNumberControl")]
        [Space(5)]
        [SerializeField] private TeamDuplicationFightPointTeamAndNumberControl teamAndNumberControl;

        [Space(10)]
        [HeaderAttribute("Status")]
        [Space(5)]
        [SerializeField] private GameObject rebornStatusRoot;
        [SerializeField] private GameObject urgentStatusRoot;
        [SerializeField] private GameObject unlockStatusRoot;
        [SerializeField] private GameObject energyAccumulationStatusRoot;

        [Space(10)]
        [HeaderAttribute("BossPoint")]
        [Space(5)]
        [SerializeField] private GameObject bossBloodStatusRoot;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void OnEnable()
        {
            //据点boss信息
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightPointBossDataMessage,
                OnReceiveTeamDuplicationFightPointBossDataMessage);

            //据点解锁进度的通知
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointUnlockRateMessage,
                OnReceiveTeamDuplicationFightPointUnlockRateMessage);
            
            //能量蓄积状态的据点刷新
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage,
                OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointBossDataMessage,
                OnReceiveTeamDuplicationFightPointBossDataMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointUnlockRateMessage,
                OnReceiveTeamDuplicationFightPointUnlockRateMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage,
                OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage);
        }

        private void BindUiEvents()
        {
            if (fightPointButton != null)
            {
                fightPointButton.onClick.RemoveAllListeners();
                fightPointButton.onClick.AddListener(OnFightPointButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (fightPointButton != null)
                fightPointButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _fightPointDataModel = null;
            _onFightPointButtonClickAction = null;
            _teamCopyFieldTable = null;

            _isTeamDuplicationFightPointStatusChanged = false;
            _isTeamDuplicationFightPointIdChanged = false;

            _fightPointRebornStatusControl = null;
            _fightPointUrgentStatusGameObject = null;
            _fightPointUnlockStatusControl = null;
            _fightPointEnergyAccumulationStatusControl = null;

            _fightPointCurrentBossPhase = 0;

            if(teamAndNumberControl != null)
                teamAndNumberControl.ClearControl();
        }

        public void Init(TeamDuplicationFightPointDataModel fightPointDataModel,
            OnFightPointButtonClickAction fightPointButtonClickAction = null)
        {
            //数据判断
            InitFightPointStatusChange(fightPointDataModel);
            InitFightPointIdChange(fightPointDataModel);

            _fightPointDataModel = fightPointDataModel;
            _onFightPointButtonClickAction = fightPointButtonClickAction;

            if (_fightPointDataModel == null)
            {
                Logger.LogError("FightPointDataModel is null");
                return;
            }

            _teamCopyFieldTable = TableManager.GetInstance()
                .GetTableItem<TeamCopyFieldTable>((int)_fightPointDataModel.FightPointId);
            if (_teamCopyFieldTable == null)
            {
                Logger.LogErrorFormat("TeamCopyFieldTable is null and pointId is {0}",
                    _fightPointDataModel.FightPointId);
                return;
            }

            InitItem();
        }

        private void InitFightPointIdChange(TeamDuplicationFightPointDataModel fightPointDataModel)
        {
            //默认改变
            _isTeamDuplicationFightPointIdChanged = true;
            if (fightPointDataModel == null)
                return;

            if (_fightPointDataModel == null)
                return;

            if (_fightPointDataModel.FightPointId == fightPointDataModel.FightPointId)
            {
                _isTeamDuplicationFightPointIdChanged = false;
            }
        }

        //据点状态改变的数据
        private void InitFightPointStatusChange(TeamDuplicationFightPointDataModel fightPointDataModel)
        {
            //默认状态没有改变
            _isTeamDuplicationFightPointStatusChanged = false;

            if (fightPointDataModel == null)
                return;

            //原始据点数据不存在
            if (_fightPointDataModel == null)
            {
                _isTeamDuplicationFightPointStatusChanged = true;
                return;
            }

            //据点数据的ID改变
            if (_fightPointDataModel.FightPointId != fightPointDataModel.FightPointId)
            {
                _isTeamDuplicationFightPointStatusChanged = true;
                return;
            }

            //据点数据的状态改变
            if (_fightPointDataModel.FightPointStatusType != fightPointDataModel.FightPointStatusType)
            {
                _isTeamDuplicationFightPointStatusChanged = true;
                return;
            }
        }

        private void InitItem()
        {
            InitCommonView();

            //更新据点状态
            UpdateFightPointStatusView();

            //更新boss据点数据
            UpdateFightPointBossItemView();

            UpdateFightPointTeamItem();
            UpdateFightPointFightNumberItem();
        }

        //据点的图片只在第一次初始化的时候，加载一次，避免重复的加载
        private void InitCommonView()
        {
            //只初始化一次
            if (_isTeamDuplicationFightPointIdChanged == true)
            {
                //名字
                InitFightPointName();

                //boss据点
                if (_teamCopyFieldTable.PresentedType == TeamCopyFieldTable.ePresentedType.BossFightPoint)
                {
                    //boss据点
                    UpdateFightPointIcon(false);
                }
                else
                {
                    //其他据点
                    UpdateFightPointIcon(true);

                    if (normalIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref normalIcon, _teamCopyFieldTable.NormalIconPath);
                    }

                    if (selectedIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref selectedIcon, _teamCopyFieldTable.SelectedIconPath);
                    }
                }
            }
        }

        //初始化据点的名字
        private void InitFightPointName()
        {
            if (_teamCopyFieldTable == null)
                return;

            if (fightPointNormalNameText != null)
            {
                var normalNameStr = string.Format(TR.Value("team_duplication_fight_point_normal_name"),
                    _teamCopyFieldTable.Name);
                fightPointNormalNameText.text = normalNameStr;
            }

            if (fightPointSelectedNameText != null)
            {
                var selectedNameStr = string.Format(TR.Value("team_duplication_fight_point_selected_name"),
                    _teamCopyFieldTable.Name);
                fightPointSelectedNameText.text = selectedNameStr;
            }
        }


        //更新据点的状态：状态的名字，状态的倒计时，状态的特效
        public void UpdateFightPointStatusView()
        {
            CommonUtility.UpdateGameObjectVisible(urgentStatusRoot, false);
            CommonUtility.UpdateGameObjectVisible(rebornStatusRoot, false);
            CommonUtility.UpdateGameObjectVisible(unlockStatusRoot, false);
            CommonUtility.UpdateGameObjectVisible(energyAccumulationStatusRoot, false);

            //重生状态
            if (_fightPointDataModel.FightPointStatusType == TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_REBORN)
            {
                if (_fightPointRebornStatusControl == null)
                {
                    _fightPointRebornStatusControl = LoadFightPointRebornStatusView(rebornStatusRoot);
                }

                if (_fightPointRebornStatusControl != null)
                {
                    //处在重生的时间段中
                    if (_fightPointDataModel.FightPointRebornTime > TimeManager.GetInstance().GetServerTime())
                    {
                        var totalCountDownTime = _fightPointDataModel.FightPointRebornTime -
                                                 (int)TimeManager.GetInstance().GetServerTime();

                        _fightPointRebornStatusControl.Init(totalCountDownTime);

                        CommonUtility.UpdateGameObjectVisible(rebornStatusRoot, true);
                    }
                }
            }
            else if (_fightPointDataModel.FightPointStatusType 
                     == TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_URGENT)
            {
                //紧急状态
                if (_isTeamDuplicationFightPointStatusChanged == true)
                {
                    //播放音效
                    AudioManager.instance.PlaySound(TeamDuplicationDataManager.TeamDuplicationAudioUrgentStateId);
                }

                //第一次加载
                if (_fightPointUrgentStatusGameObject == null)
                {
                    _fightPointUrgentStatusGameObject = LoadFightPointUrgentStatusView(urgentStatusRoot);
                }

                //存在，直接显示
                if (_fightPointUrgentStatusGameObject != null)
                {
                    CommonUtility.UpdateGameObjectVisible(urgentStatusRoot, true);
                    CommonUtility.UpdateGameObjectVisible(_fightPointUrgentStatusGameObject, true);
                }
            }
            else if (_fightPointDataModel.FightPointStatusType == TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_UNLOCKING)
            {
                var fightPointUnlockRate = 0;
                if (_teamCopyFieldTable != null && _teamCopyFieldTable.PreFieldPointId > 0)
                {
                    //由前置据点的战斗次数决定解锁的数据
                    fightPointUnlockRate = TeamDuplicationDataManager.GetInstance()
                        .GetTeamDuplicationFightPointUnlockRateByPreFightPointId(
                            _teamCopyFieldTable.PreFieldPointId);
                }
                else
                {
                    //直接获得解锁数据
                    fightPointUnlockRate = TeamDuplicationDataManager.GetInstance()
                        .GetTeamDuplicationFightPointUnlockRateByFightPointId(_fightPointDataModel.FightPointId);
                }

                if (_fightPointUnlockStatusControl == null)
                {
                    _fightPointUnlockStatusControl = LoadFightPointUnlockStatusView(unlockStatusRoot);
                }

                if (_fightPointUnlockStatusControl != null)
                {
                    CommonUtility.UpdateGameObjectVisible(unlockStatusRoot, true);
                    _fightPointUnlockStatusControl.UpdateUnlockRate(fightPointUnlockRate);
                }
            }
            else if (_fightPointDataModel.FightPointStatusType ==
                     TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_ENERGY_REVIVE)
            {
                //能量蓄积状态的据点
                if (_fightPointEnergyAccumulationStatusControl == null)
                    _fightPointEnergyAccumulationStatusControl = LoadFightPointEnergyAccumulationStatusView(
                        energyAccumulationStatusRoot);

                if (_fightPointEnergyAccumulationStatusControl != null)
                {
                    _fightPointEnergyAccumulationStatusControl.Init(_fightPointDataModel
                        .FightPointEnergyAccumulationStartTime);

                    CommonUtility.UpdateGameObjectVisible(energyAccumulationStatusRoot, true);
                }
            }
        }

        //更新boss据点View（只有boss据点才进行)
        public void UpdateFightPointBossItemView()
        {
            CommonUtility.UpdateGameObjectVisible(bossBloodStatusRoot, false);

            //非boss据点，直接返回
            if (_teamCopyFieldTable.PresentedType != TeamCopyFieldTable.ePresentedType.BossFightPoint)
                return;

            //boss阶段为0，直接返回
            if (TeamDuplicationDataManager.GetInstance().BossPhase <= 0)
                return;

            //更新据点的显示Icon
            UpdateFightPointBossItemIcon();

            //更新据点的血量
            UpdateFightPointBossBlood();
        }

        private void UpdateFightPointBossBlood()
        {
            //boss据点的血量
            if (_fightPointBossBloodControl == null)
            {
                _fightPointBossBloodControl = LoadFightPointBossBloodView(bossBloodStatusRoot);
            }

            if (_fightPointBossBloodControl != null)
            {
                var bossBloodRate = TeamDuplicationDataManager.GetInstance().BossBloodRate;
                //设置血量
                _fightPointBossBloodControl.UpdateBossBloodRate(bossBloodRate);
                //存在血量
                if (bossBloodRate > 0)
                {
                    CommonUtility.UpdateGameObjectVisible(bossBloodStatusRoot, true);
                }
            }
        }

        private void UpdateFightPointBossItemIcon()
        {
            //已经更新，直接返回
            if (_fightPointCurrentBossPhase == TeamDuplicationDataManager.GetInstance().BossPhase)
                return;

            //更新boss阶段
            _fightPointCurrentBossPhase = TeamDuplicationDataManager.GetInstance().BossPhase;

            //默认第一阶段
            var bossNormalIconPath = _teamCopyFieldTable.NormalIconPath;
            var bossSelectedIconPath = _teamCopyFieldTable.SelectedIconPath;

            //第二阶段
            if (_fightPointCurrentBossPhase == 2)
            {
                bossNormalIconPath = _teamCopyFieldTable.BossSecondStageNormalIconPath;
                bossSelectedIconPath = _teamCopyFieldTable.BossSecondStageSelectedIconPath;
            }
            else if (_fightPointCurrentBossPhase == 3)
            {
                bossNormalIconPath = _teamCopyFieldTable.BossThirdStageNormalIconPath;
                bossSelectedIconPath = _teamCopyFieldTable.BossThirdStageSelectedIconPath;
            }

            //更新boss据点的Icon
            if (bossNormalIcon != null)
                ETCImageLoader.LoadSprite(ref bossNormalIcon, bossNormalIconPath);
            if (bossSelectedIcon != null)
                ETCImageLoader.LoadSprite(ref bossSelectedIcon, bossSelectedIconPath);
        }

        //更新据点展示的Icon，区分boss据点和普通据点
        private void UpdateFightPointIcon(bool isNormalFightPoint)
        {
            CommonUtility.UpdateGameObjectVisible(normalBg, isNormalFightPoint);
            CommonUtility.UpdateGameObjectVisible(selectedBg, isNormalFightPoint);

            CommonUtility.UpdateImageVisible(bossNormalIcon, !isNormalFightPoint);
            CommonUtility.UpdateImageVisible(bossSelectedIcon, !isNormalFightPoint);
        }


        private void UpdateFightPointTeamItem()
        {
            if (teamAndNumberControl != null)
            {
                teamAndNumberControl.UpdateFightPointTeamItemList(_fightPointDataModel.FightPointTeamList);
            }
        }

        private void UpdateFightPointFightNumberItem()
        {
            var finishNumber = _fightPointDataModel.FightPointTotalFightNumber -
                               _fightPointDataModel.FightPointLeftFightNumber;
            if (finishNumber < 0)
                finishNumber = 0;

            if (teamAndNumberControl != null)
            {
                teamAndNumberControl.UpdateFightPointFightNumberItemList(finishNumber,
                    _fightPointDataModel.FightPointTotalFightNumber);
            }
        }

        private void OnFightPointButtonClick()
        {
            if (_onFightPointButtonClickAction != null
                && _fightPointDataModel != null)
            {
                UpdateFightPointSelectedView(true);
                _onFightPointButtonClickAction(_fightPointDataModel);
            }
        }

        public void UpdateFightPointSelectedView(bool flag)
        {
            UpdateFightPointName(flag);
            if (fightPointSelectedRoot == null)
                return;

            CommonUtility.UpdateGameObjectVisible(fightPointSelectedRoot, flag);            
        }

        //更新据点的NormalName
        private void UpdateFightPointName(bool isSelected = false)
        {
            CommonUtility.UpdateTextVisible(fightPointNormalNameText, !isSelected);
        }

        public TeamDuplicationFightPointDataModel GetFightPointDataModel()
        {
            return _fightPointDataModel;
        }

        #region FightPointStatus

        //重生状态
        private TeamDuplicationFightPointRebornStatusControl LoadFightPointRebornStatusView(GameObject contentRoot)
        {
            var rebornStatusViewPrefab = CommonUtility.LoadGameObject(contentRoot);
            if (rebornStatusViewPrefab == null)
                return null;

            var rebornStatusControl =
                rebornStatusViewPrefab.GetComponent<TeamDuplicationFightPointRebornStatusControl>();
            return rebornStatusControl;
        }

        //能量蓄积状态
        private TeamDuplicationFightPointEnergyAccumulationStatusControl LoadFightPointEnergyAccumulationStatusView(GameObject contentRoot)
        {
            var energyAccumulationStatusViewPrefab = CommonUtility.LoadGameObject(contentRoot);
            if (energyAccumulationStatusViewPrefab == null)
                return null;

            var energyAccumulationStatusControl =
                energyAccumulationStatusViewPrefab.GetComponent<TeamDuplicationFightPointEnergyAccumulationStatusControl>();
            return energyAccumulationStatusControl;
        }

        //紧急状态
        private GameObject LoadFightPointUrgentStatusView(GameObject contentRoot)
        {
            var urgentStatusViewPrefab = CommonUtility.LoadGameObject(contentRoot);
            return urgentStatusViewPrefab;
        }

        //解锁状态
        private TeamDuplicationFightPointUnlockStatusControl LoadFightPointUnlockStatusView(GameObject contentRoot)
        {
            var unlockStatusRootPrefab = CommonUtility.LoadGameObject(contentRoot);
            if (unlockStatusRootPrefab == null)
                return null;

            var unlockStatusControl =
                unlockStatusRootPrefab.GetComponent<TeamDuplicationFightPointUnlockStatusControl>();
            return unlockStatusControl;
        }

        //boss据点血量
        private TeamDuplicationFightPointBossBloodControl LoadFightPointBossBloodView(GameObject bossBloodRoot)
        {
            var bossBloodPrefab = CommonUtility.LoadGameObject(bossBloodRoot);
            if (bossBloodPrefab == null)
                return null;

            var bossBloodControl = bossBloodPrefab.GetComponent<TeamDuplicationFightPointBossBloodControl>();
            return bossBloodControl;
        }

        #endregion

        #region UIEvent
        //能量蓄积状态据点时间刷新
        private void OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage(UIEvent uiEvent)
        {
            if (_fightPointDataModel == null)
                return;

            if (_teamCopyFieldTable == null)
                return;

            //非能量蓄积状态
            if (_fightPointDataModel.FightPointStatusType 
                != TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_ENERGY_REVIVE)
                return;

            if(_fightPointEnergyAccumulationStatusControl != null)
                _fightPointEnergyAccumulationStatusControl.UpdateStatusView();
        }

        private void OnReceiveTeamDuplicationFightPointBossDataMessage(UIEvent uiEvent)
        {
            //数据为null，直接返回
            if (_fightPointDataModel == null)
                return;

            if (_teamCopyFieldTable == null)
                return;

            //非boss据点，直接返回
            if (_teamCopyFieldTable.PresentedType != TeamCopyFieldTable.ePresentedType.BossFightPoint)
                return;

            UpdateFightPointBossItemView();
        }

        //据点进度的通知
        private void OnReceiveTeamDuplicationFightPointUnlockRateMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var fightPointId = (int)uiEvent.Param1;

            UpdateFightPointUnlockRateByFightPointId(fightPointId);
        }

        private void UpdateFightPointUnlockRateByFightPointId(int fightPointId)
        {
            if (_fightPointDataModel == null)
                return;

            if (_fightPointDataModel.FightPointId != fightPointId)
                return;

            if (_fightPointDataModel.FightPointStatusType != TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_UNLOCKING)
                return;

            UpdateFightPointStatusView();
        }
        #endregion 

    }
}
