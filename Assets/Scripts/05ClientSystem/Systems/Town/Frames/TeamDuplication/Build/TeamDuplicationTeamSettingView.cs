using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationTeamSettingView : MonoBehaviour
    {

        private uint _teamDifficultyLevel = 0;
        private TeamCopyTeamModel _teamType = TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE;
        private bool _isResetEquipmentScore = false;
        private int _ownerEquipmentScore = 0;

        private int _equipmentScoreValue;
        private int _goldValueNumber;

        private ComControlData defaultEquipmentScoreData;
        private ComControlData defaultGoldValueData;
        private List<ComControlData> equipmentScoreDataList;
        private List<ComControlData> goldValueDataList;

        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text titleLabel;

        [SerializeField] private Button buildButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button settingButton;

        [Space(15)]
        [HeaderAttribute("Mode")]
        [Space(10)]
        [SerializeField] private Text typeSettingLabel;

        [SerializeField] private GameObject challengeTypeRoot;
        [SerializeField] private Text challengeTypeText;
        [SerializeField] private GameObject goldTypeRoot;
        [SerializeField] private Text goldTypeText;

        [Space(15)]
        [HeaderAttribute("EquipmentScore")]
        [Space(10)]
        [SerializeField]
        private Text equipmentScoreLabel;
        [SerializeField] private CommonNewDropDownControl equipmentScoreDropDownControl;

        [Space(15)]
        [HeaderAttribute("Gold")]
        [Space(10)]
        [SerializeField] private GameObject goldValueRoot;
        [SerializeField]
        private Text goldValueLabel;
        [SerializeField] private CommonNewDropDownControl goldValueDropDownControl;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void ClearData()
        {
            _teamType = TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE;
            _equipmentScoreValue = 0;
            _goldValueNumber = 0;
            equipmentScoreDataList = null;
            goldValueDataList = null;
            defaultEquipmentScoreData = null;
            defaultGoldValueData = null;
            _teamDifficultyLevel = 0;
        }

        private void BindUiEvents()
        {
            if (buildButton != null)
            {
                buildButton.onClick.RemoveAllListeners();
                buildButton.onClick.AddListener(OnBuildButtonClick);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCancelButtonClick);
            }

            if (settingButton != null)
            {
                settingButton.onClick.RemoveAllListeners();
                settingButton.onClick.AddListener(OnSettingButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (buildButton != null)
            {
                buildButton.onClick.RemoveAllListeners();
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
            }

            if(settingButton != null)
                settingButton.onClick.RemoveAllListeners();
        }

        public void Init(TeamCopyTeamModel teamType, 
            uint teamDifficultyLevel = 0,
            bool isResetEquipmentScore = false,
            int ownerEquipmentScore = 0)
        {

            _teamDifficultyLevel = teamDifficultyLevel;

            if (teamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
                _teamType = teamType;
            else
            {
                _teamType = TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE;
            }

            _isResetEquipmentScore = isResetEquipmentScore;
            _ownerEquipmentScore = ownerEquipmentScore;

            equipmentScoreDataList = TeamDuplicationUtility.GetTeamPropertyDataListByType(_teamType,
                TeamDuplicationTeamPropertyType.EquipmentScoreType);

            if (equipmentScoreDataList != null && equipmentScoreDataList.Count > 0)
            {
                //创建团本
                if (_isResetEquipmentScore == false)
                {
                    defaultEquipmentScoreData = equipmentScoreDataList[0];
                    _equipmentScoreValue = defaultEquipmentScoreData.Index;
                }
                else
                {
                    //修改装备评分
                    defaultEquipmentScoreData = equipmentScoreDataList[0];
                    for (var i = 0; i < equipmentScoreDataList.Count; i++)
                    {
                        var curEquipmentScoreData = equipmentScoreDataList[i];
                        if(curEquipmentScoreData == null)
                            continue;

                        if (curEquipmentScoreData.Index == _ownerEquipmentScore)
                        {
                            defaultEquipmentScoreData = curEquipmentScoreData;
                            break;
                        }
                    }
                    _equipmentScoreValue = defaultEquipmentScoreData.Index;
                }
            }
            else
            {
                Logger.LogErrorFormat("EquipmentScoreDataList is null or count is zero");
                return;
            }

            //创建团本
            if (_isResetEquipmentScore == false)
            {
                if (_teamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
                {
                    goldValueDataList = TeamDuplicationUtility.GetTeamPropertyDataListByType(
                        _teamType,
                        TeamDuplicationTeamPropertyType.GoldValueType);

                    if (goldValueDataList != null && goldValueDataList.Count > 0)
                    {
                        defaultGoldValueData = goldValueDataList[0];
                        _goldValueNumber = defaultGoldValueData.Index;
                    }
                    else
                    {
                        Logger.LogErrorFormat("goldValueDataList is null or count is zero");
                        return;
                    }
                }
            }

            InitView();
        }

        private void InitView()
        {
            InitTitle();

            InitContent();

            InitSettingDropDownControl();

            InitTeamSettingButton();
        }

        private void InitTitle()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("team_duplication_team_setting_title");
        }

        private void InitContent()
        {
            if (typeSettingLabel != null)
                typeSettingLabel.text = TR.Value("team_duplication_team_setting_model_label");

            if (equipmentScoreLabel != null)
                equipmentScoreLabel.text = TR.Value("team_duplication_team_setting_equipment_score_label");
            
            if (_teamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
            {
                CommonUtility.UpdateGameObjectVisible(challengeTypeRoot, false);
                CommonUtility.UpdateGameObjectVisible(goldTypeRoot, true);
                if (goldTypeText != null)
                    goldTypeText.text = TR.Value("team_duplication_team_build_gold_type");

                //创建团本
                if (_isResetEquipmentScore == false)
                {
                    CommonUtility.UpdateGameObjectVisible(goldValueRoot, true);
                    if (goldValueLabel != null)
                        goldValueLabel.text = TR.Value("team_duplication_team_setting_gold_value_label");
                }
                else
                {
                    CommonUtility.UpdateGameObjectVisible(goldValueRoot, false);
                }
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(goldTypeRoot, false);
                CommonUtility.UpdateGameObjectVisible(goldValueRoot, false);
                CommonUtility.UpdateGameObjectVisible(challengeTypeRoot, true);
                if (challengeTypeText != null)
                    challengeTypeText.text = TR.Value("team_duplication_team_build_challenge_Type");
            }

        }

        private void InitSettingDropDownControl()
        {
            if (equipmentScoreDropDownControl != null)
            {
                equipmentScoreDropDownControl.InitComDropDownControl(defaultEquipmentScoreData,
                    equipmentScoreDataList,
                    OnEquipmentScoreDropDownItemSelected,
                    OnResetEquipmentScoreDropDownAction);
            }

            if (goldValueDropDownControl != null)
            {
                if (_teamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD
                    && _isResetEquipmentScore == false)
                {
                    goldValueDropDownControl.InitComDropDownControl(defaultGoldValueData,
                        goldValueDataList,
                        OnGoldValueDropDownItemSelected,
                        OnResetGoldValueDropDownAction);
                }
            }
        }

        private void InitTeamSettingButton()
        {
            if (_isResetEquipmentScore == false)
            {
                CommonUtility.UpdateButtonVisible(buildButton, true);
                CommonUtility.UpdateButtonVisible(settingButton, false);
            }
            else
            {
                CommonUtility.UpdateButtonVisible(buildButton, false);
                CommonUtility.UpdateButtonVisible(settingButton, true);
            }
        }

        private void OnResetEquipmentScoreDropDownAction()
        {
            //可能重置另外一个dropDown
            if (goldValueDropDownControl != null)
            {
                if (_teamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
                {
                    goldValueDropDownControl.UpdateDropDownList(false);
                }
            }
        }

        private void OnResetGoldValueDropDownAction()
        {
            //可能重置另外一个dropDown
            if (equipmentScoreDropDownControl != null)
            {
                equipmentScoreDropDownControl.UpdateDropDownList(false);
            }
        }

        private void OnEquipmentScoreDropDownItemSelected(ComControlData comControlData)
        {
            _equipmentScoreValue = comControlData.Index;
        }

        private void OnGoldValueDropDownItemSelected(ComControlData comControlData)
        {
            _goldValueNumber = comControlData.Index;
        }

        private void OnCancelButtonClick()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamSettingFrame();
        }

        private void OnBuildButtonClick()
        {

            //检测装备评分的设置
            if (_equipmentScoreValue <= 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_build_equipment_is_zero"));
                return;
            }
            else
            {
                //金团模式下，检测佣金数量的设置
                if (_teamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
                {
                    if (_goldValueNumber <= 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_build_gold_value_is_zero"));
                        return;
                    }
                }
            }

            //发送消息
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamSettingFrame();
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamBuildFrame();

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyCreateTeamReq(
                (uint)_teamType,
                (uint)_equipmentScoreValue,
                (uint)_goldValueNumber,
                _teamDifficultyLevel);
        }

        //设置按钮
        private void OnSettingButtonClick()
        {

            var tipStr = TR.Value("team_duplication_team_Resetting_equipment_score",
                _equipmentScoreValue);

            SystemNotifyManager.SysNotifyFloatingEffect(tipStr);

            //装备分数不同，需要修改
            var currentEquipmentScore = TeamDuplicationUtility.GetTeamDuplicationEquipmentScore();
            if (currentEquipmentScore != _equipmentScoreValue)
            {
                TeamDuplicationDataManager.GetInstance().OnSendTeamCopyModifyEquipScoreReq(_equipmentScoreValue);
            }

            //关闭界面
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamSettingFrame();
        }


    }
}
