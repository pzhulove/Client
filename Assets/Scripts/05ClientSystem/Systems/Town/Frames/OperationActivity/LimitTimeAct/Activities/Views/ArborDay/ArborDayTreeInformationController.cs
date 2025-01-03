using System.Collections.Generic;
using Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ArborDayTreeInformationController : MonoBehaviour
    {

        private ILimitTimeActivityModel _model;

        private PlantOpActSate _treePlantState;

        #region ServerDefine

        //成熟度相关
        private string _treeProficiencyStr;            //  成熟度
        private string _treeGrowingLastTimeStr;        //持续时间
        private string _treePlantStateStr;             //树木种植状态
        private string _treeGrowingEndTimeStampStr;       //树木种植结束的时间戳

        //8个待鉴定的树木
        private List<int> _treeIdentifyIdList;
        //对应的字符串，两者一一对应
        private List<string> _treeIdentifyIdStrList;            
        #endregion

        private uint _treeGrowingEndTimeStamp = 0;
        private float _timeInterval = 0.0f;

        [Space(10)] [HeaderAttribute("ActivityTime")] [Space(10)]
        [SerializeField] private Text activityTimeLabel;

        [Space(10)]
        [HeaderAttribute("Proficiency")]
        [Space(10)]
        [SerializeField] private Text treeProficiencyTitleLabel;
        [SerializeField] private Text treeProficiencyContentLabel;
        [SerializeField] private Text treeProficiencyValueText;

        [Space(10)] [HeaderAttribute("TreeGrowingLeftTime")] [Space(10)]
        [SerializeField] private Text treeGrowingLeftTimeLabel;
        [SerializeField] private GameObject treeGrowingLeftTimeRoot;

        [Space(10)]
        [HeaderAttribute("TreeIdentifyFinishRoot")]
        [Space(10)]
        [SerializeField] private Text treeIdentifyFinishLabel;
        [SerializeField] private GameObject treeIdentifyFinishRoot;

        [Space(10)]
        [HeaderAttribute("treeImageRoot")]
        [Space(10)]
        [SerializeField] private GameObject plantingTreeImage;
        [SerializeField] private GameObject growingTreeImage;
        [SerializeField] private GameObject identifyTreeImage;

        [Space(10)]
        [HeaderAttribute("TreePlantButton")]
        [Space(10)]
        [SerializeField] private ComButtonWithCd treePlantingButton;
        [SerializeField] private ComButtonWithCd treeIdentifyButton;

        [Space(10)] [HeaderAttribute("identifyTree")] [Space(10)]
        [SerializeField] private Text treeIdentifyTitleLabel;
        [SerializeField] private ComUIListScriptEx treeIdentifyItemList;

        #region Init
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
            _timeInterval = 0;
            _treeGrowingEndTimeStamp = 0;

            if (_treeIdentifyIdList != null)
            {
                _treeIdentifyIdList.Clear();
                _treeIdentifyIdList = null;
            }

            if (_treeIdentifyIdStrList != null)
            {
                _treeIdentifyIdStrList.Clear();
                _treeIdentifyIdStrList = null;
            }
            _model = null;

            _treePlantState = PlantOpActSate.POPS_NONE;
        }

        private void BindUiEvents()
        {
            if (treePlantingButton != null)
            {
                treePlantingButton.ResetButtonListener();
                treePlantingButton.SetButtonListener(OnTreePlantingButtonClicked);
            }

            if (treeIdentifyButton != null)
            {
                treeIdentifyButton.ResetButtonListener();
                treeIdentifyButton.SetButtonListener(OnTreeIdentifyButtonClicked);
            }
            
            if (treeIdentifyItemList != null)
            {
                treeIdentifyItemList.onItemVisiable += OnTreeIdentifyItemVisible;
                treeIdentifyItemList.OnItemUpdate += OnTreeIdentifyItemUpdate;
                treeIdentifyItemList.OnItemRecycle += OnTreeIdentifyItemRecycle;
            }
        }

        private void UnBindUiEvents()
        {
            if(treePlantingButton != null)
                treePlantingButton.ResetButtonListener();

            if(treeIdentifyButton != null)
                treeIdentifyButton.ResetButtonListener();

            if (treeIdentifyItemList != null)
            {
                treeIdentifyItemList.onItemVisiable -= OnTreeIdentifyItemVisible;
                treeIdentifyItemList.OnItemUpdate -= OnTreeIdentifyItemUpdate;
                treeIdentifyItemList.OnItemRecycle -= OnTreeIdentifyItemRecycle;
            }
        }

        private void OnEnable()
        {
            //count值改变
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCountValueChangeChanged);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCountValueChangeChanged);
        }

        #endregion


        #region UIEvent

        private void OnCountValueChangeChanged(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            string countKey = (string) uiEvent.Param1;

            //成熟度
            if (string.Equals(countKey, _treeProficiencyStr) == true)
            {
                UpdateTreeProficiencyValue();
                return;
            }

            //种植状态
            if (string.Equals(countKey, _treePlantStateStr) == true)
            {
                UpdateTreePlantState();
                return;
            }

            if (string.Equals(countKey, _treeGrowingEndTimeStampStr) == true)
            {
                _treeGrowingEndTimeStamp = (uint) ArborDayUtility.GetCounterValueByCounterStr(_treeGrowingEndTimeStampStr);
                UpdateTreeGrowingLastTimeContent();
            }
        }
        #endregion

        private void InitTreeInformationDataFromActivityModel()
        {
            var strParams = _model.StrParam;

            if (strParams != null && strParams.Length == 4)
            {
                _treeProficiencyStr = strParams[0];
                _treeGrowingLastTimeStr = strParams[1];
                _treePlantStateStr = strParams[2];
                _treeGrowingEndTimeStampStr = strParams[3];
            }
            
            _treeIdentifyIdList = new List<int>();
            if (_model.ParamArray != null && _model.ParamArray.Length > 0)
            {
                for (var i = 0; i < _model.ParamArray.Length; i++)
                {
                    var treeId = _model.ParamArray[i];
                    if (treeId > 0)
                        _treeIdentifyIdList.Add((int)treeId);
                }
            }
            _treeIdentifyIdStrList = CommonUtility.GetStrListBySplitString(_model.CountParam);
        }

        public void InitTreeInformationController(ILimitTimeActivityModel model)
        {
            _model = model;
            if (_model == null)
                return;

            InitTreeInformationDataFromActivityModel();
            
            InitTreeInformationBaseContent();
            OnUpdateTreeInformationContent();
        }

        private void InitTreeInformationBaseContent()
        {
            if (activityTimeLabel != null)
            {
                var activityTimeStr = string.Format(TR.Value("Limit_Time_Activity_Time_Interval_Format"),
                    TimeUtility.GetTimeFormatByYearMonthDay(_model.StartTime),
                    TimeUtility.GetTimeFormatByYearMonthDay(_model.EndTime));

                activityTimeLabel.text = activityTimeStr;
            }
            
            if (treeProficiencyTitleLabel != null)
                treeProficiencyTitleLabel.text = TR.Value("Arbor_Day_Proficiency_Title");

            if (treeProficiencyContentLabel != null)
                treeProficiencyContentLabel.text = TR.Value("Arbor_Day_Proficiency_Introduction");

            if (treeIdentifyTitleLabel != null)
                treeIdentifyTitleLabel.text = TR.Value("Arbor_Day_Get_Tree_Title");

            if (treeIdentifyFinishLabel != null)
                treeIdentifyFinishLabel.text = TR.Value("Arbor_Day_Already_Identify_All_Tree");
        }

        private void OnUpdateTreeInformationContent()
        {
            //成熟度
            UpdateTreeProficiencyValue();

            //树木状态
            UpdateTreePlantState();

            //鉴定树木的列表
            UpdateTreeIdentifyItemList();
        }

        private void UpdateTreeProficiencyValue()
        {
            if (treeProficiencyValueText != null)
            {
                var treeProficiency = ArborDayUtility.GetCounterValueByCounterStr(_treeProficiencyStr);
                treeProficiencyValueText.text = treeProficiency.ToString();
            }
        }

        public void OnUpdateTreeInformationController(ILimitTimeActivityModel model)
        {
            _model = model;
            if (_model == null)
                return;

           OnUpdateTreeInformationContent();
        }

        //树木种植的状态
        private void UpdateTreePlantState()
        {

            CommonUtility.UpdateGameObjectVisible(plantingTreeImage, false);
            CommonUtility.UpdateGameObjectVisible(growingTreeImage, false);
            CommonUtility.UpdateGameObjectVisible(identifyTreeImage, false);

            _treePlantState = (PlantOpActSate)ArborDayUtility.GetCounterValueByCounterStr(
                _treePlantStateStr);

            switch (_treePlantState)
            {
                case PlantOpActSate.POPS_PLANTING:
                    //树木已经种植，正在成长中

                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treePlantingButton, false);

                    CommonUtility.UpdateGameObjectVisible(treeGrowingLeftTimeRoot, true);
                    _treeGrowingEndTimeStamp =
                        (uint) ArborDayUtility.GetCounterValueByCounterStr(_treeGrowingEndTimeStampStr);
                    UpdateTreeGrowingLastTimeContent();
                    CommonUtility.UpdateGameObjectVisible(growingTreeImage, true);

                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treeIdentifyButton, false);

                    CommonUtility.UpdateGameObjectVisible(treeIdentifyFinishRoot, false);
                    break;
                case PlantOpActSate.POPS_CAN_APP:
                    //树木可以鉴定

                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treePlantingButton, false);

                    CommonUtility.UpdateGameObjectVisible(treeGrowingLeftTimeRoot, false);

                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treeIdentifyButton, true);
                    CommonUtility.UpdateGameObjectVisible(identifyTreeImage, true);

                    CommonUtility.UpdateGameObjectVisible(treeIdentifyFinishRoot, false);
                    break;
                case PlantOpActSate.POPS_ALLGET:
                    //树木全部鉴定完全
                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treePlantingButton, false);
                    CommonUtility.UpdateGameObjectVisible(treeGrowingLeftTimeRoot, false);
                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treeIdentifyButton, false);

                    CommonUtility.UpdateGameObjectVisible(identifyTreeImage, true);
                    CommonUtility.UpdateGameObjectVisible(treeIdentifyFinishRoot, true);
                    break;
                default:
                    //默认情况，可以种植
                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treePlantingButton, true);
                    CommonUtility.UpdateGameObjectVisible(plantingTreeImage, true);

                    CommonUtility.UpdateGameObjectVisible(treeGrowingLeftTimeRoot, false);
                    CommonUtility.UpdateButtonWithCdVisibleAndReset(treeIdentifyButton, false);
                    CommonUtility.UpdateGameObjectVisible(treeIdentifyFinishRoot, false);
                    break;
            }

        }

        //鉴定树木
        private void UpdateTreeIdentifyItemList()
        {
            if (treeIdentifyItemList == null)
                return;

            //没有初始化
            if (treeIdentifyItemList.IsInitialised() == false)
            {
                treeIdentifyItemList.Initialize();

                var treeIdentifyItemCount = 0;
                if (_treeIdentifyIdList != null)
                    treeIdentifyItemCount = _treeIdentifyIdList.Count;

                treeIdentifyItemList.SetElementAmount(treeIdentifyItemCount);
            }
            else
            {
                //更新
                treeIdentifyItemList.UpdateElement();
            }
        }

        #region TreeIdentifyItemList
        private void OnTreeIdentifyItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_treeIdentifyIdList == null || _treeIdentifyIdList.Count <= 0)
                return;
            if (_treeIdentifyIdStrList == null || _treeIdentifyIdStrList.Count <= 0)
                return;

            if (item.m_index < 0 
                || item.m_index >= _treeIdentifyIdList.Count
                || item.m_index >= _treeIdentifyIdStrList.Count)
                return;


            var treeIdentifyItemId = _treeIdentifyIdList[item.m_index];
            var treeIdentifyItemStr = _treeIdentifyIdStrList[item.m_index];

            var arborDayTreeIdentifyItem = item.GetComponent<ArborDayTreeIdentifyItem>();
            if (arborDayTreeIdentifyItem != null && treeIdentifyItemId > 0)
                arborDayTreeIdentifyItem.InitItem(treeIdentifyItemId,
                    treeIdentifyItemStr,
                    item.m_index + 1);
        }
        private void OnTreeIdentifyItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var arborDayTreeIdentifyItem = item.GetComponent<ArborDayTreeIdentifyItem>();
            if(arborDayTreeIdentifyItem != null)
                arborDayTreeIdentifyItem.UpdateItem();
        }

        private void OnTreeIdentifyItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var arborDayTreeIdentifyItem = item.GetComponent<ArborDayTreeIdentifyItem>();
            if(arborDayTreeIdentifyItem != null)
                arborDayTreeIdentifyItem.RecycleItem();
        }

        #endregion

        #region ButtonClicked
        private void OnTreePlantingButtonClicked()
        {
            ActivityDataManager.GetInstance().OnSendSceneActivePlantReq();
        }

        private void OnTreeIdentifyButtonClicked()
        {
            ActivityDataManager.GetInstance().OnSendSceneActivePlantAppraReq();
        }
        #endregion

        #region Update

        private void Update()
        {
            //不在成长的状态
            if (_treePlantState != PlantOpActSate.POPS_PLANTING)
                return;

            //结束时间小于0
            if (_treeGrowingEndTimeStamp <= 0)
                return;

            //结束时间小于当前时间戳
            if (_treeGrowingEndTimeStamp < TimeManager.GetInstance().GetServerTime())
                return;

            _timeInterval += Time.deltaTime;
            if (_timeInterval >= 1.0f)
            {
                UpdateTreeGrowingLastTimeContent();
            }
        }

        //树木种植的内容
        private void UpdateTreeGrowingLastTimeContent()
        {
            _timeInterval = 0.0f;
            UpdateTreeGrowingLeftTimeLabel();
        }

        //更新树木成长的倒计时
        private void UpdateTreeGrowingLeftTimeLabel()
        {
            if (treeGrowingLeftTimeLabel == null)
                return;

            var countDownTimeStr = CountDownTimeUtility.GetCountDownTimeByMinuteSecondFormat(
                (uint)_treeGrowingEndTimeStamp,
                TimeManager.GetInstance().GetServerTime());

            treeGrowingLeftTimeLabel.text = TR.Value("Arbor_Day_Tree_is_in_Growing",
                countDownTimeStr);
        }

        #endregion

      


    }
}
