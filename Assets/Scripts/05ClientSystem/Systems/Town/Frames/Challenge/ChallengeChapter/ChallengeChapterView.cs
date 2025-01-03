using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{

    public class ChallengeChapterView : MonoBehaviour
    {

        private ChallengeChapterParamDataModel _chapterParamDataModel = null;
        private DungeonModelTable.eType _modelType;
        private int _mapItemId;         //挑战地图中Item对应的ID；
        private int _baseChapterId;
        private int _selectedChapterId;
        private List<int> _chapterIdList;
        private int _chapterShopId;

        private int _baseChapterIndex = 0;          //用于控制左右滑动的按钮

        private DungeonTable _dungeonTable;
        private List<ChallengeChapterLevelDataModel> _chapterLevelDataModelList;

        [Space(10)]
        [HeaderAttribute("Close")]
        [Space(10)]
        [SerializeField] private Button closeButton;

        [Space(10)] [HeaderAttribute("LeftPanel")] [Space(10)] [SerializeField]
        private Button leftChapterButton;
        [SerializeField] private UIGray leftChapterButtonGray;
        [SerializeField] private Button rightChapterButton;
        [SerializeField] private UIGray rightChapterButtonGray;
        [SerializeField] private GameObject frameMask;

        [Space(10)] [HeaderAttribute("Control")] [Space(10)]
        [SerializeField] private ChallengeChapterMoneyControl chapterMoneyControl;
        [SerializeField] private ChallengeChapterNormalControl chapterNormalControl;
        [SerializeField] private ChallengeChapterLevelControl chapterLevelControl;
        [SerializeField] private ChallengeChapterDropControl chapterDropControl;
        [SerializeField] private ChallengeChapterEntryControl chapterEntryControl;
        [SerializeField] private ChallengeChapterDrugControl chapterDrugControl;
        [SerializeField] private ChallengeChapterRewardControl chapterRewardControl;

        [SerializeField]private Text mFailDesc;
        [SerializeField]private string failDesc = "(失败返回{0})";

        [Space(10)] [HeaderAttribute("YIJieInfo")] [Space(10)]
        [SerializeField] private GameObject mMissionInfoRoot;
        [SerializeField] private Text mMissionInfo;
        [SerializeField] private LinkParse mMissionContent;
        [SerializeField] private ComChapterDungeonUnit mDungeonUnitInfo;
        [SerializeField] private GameObject mDetailButtonRoot;
        [SerializeField] private GameObject mDropProgress;
        [SerializeField] private Button mDropButton;
        [SerializeField] private GameObject mDropButtonEffect;
        [SerializeField] private GameObject mlevelChallengeTimesRoot;
        [SerializeField] private Text mLevelChallengeTimesNumber;
        [SerializeField] private GameObject mTicketConsumeRoot;
        [SerializeField] private GameObject mStartButtonRoot;
        [SerializeField] private GameObject mStartContinueButtonRoot;
        [SerializeField] private Button mStartContinueButton;
        [SerializeField] private Button mStrategySkillsButton;
        [SerializeField] private TextEx mTextName;

        [SerializeField] private GameObject chapterLevelControlRoot;
        [SerializeField] private GameObject chapterSelected;
        [SerializeField] private GameObject sinanListRoot;
        [SerializeField] private GameObject sinanRoot;
        [SerializeField] private SinanItem mSinanItemA;

        [SerializeField] private ComUIListScript mSinanUIListScript;
        [SerializeField] private ComDropDownControl mSinanQulityDrop;

        private int failureReturnValue = 0;
        private uint mAreaIndex = 0;

        private EnchantmentsFunctionData dataMerge = new EnchantmentsFunctionData();
        private List<ItemData> mAllSinanItems = new List<ItemData>();
        private List<ComControlData> mSinanQulityTabDataList = new List<ComControlData>();
        private int mCurrentSelectedSinanQuality = 0;//当前选择的品质
        private int iDefaultSinanQuality = 0;//默认品质

        #region Bxy
        private void InitSinanUIListScript()
        {
            if (mSinanUIListScript != null)
            {
                mSinanUIListScript.Initialize();
                mSinanUIListScript.onBindItem += OnBindItemDelegate;
                mSinanUIListScript.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitSinanUIListScript()
        {
            if (mSinanUIListScript != null)
            {
                mSinanUIListScript.onBindItem -= OnBindItemDelegate;
                mSinanUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private SinanItemElement OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<SinanItemElement>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as SinanItemElement;
            if (element != null && item.m_index >= 0 && item.m_index < mAllSinanItems.Count)
            {
                element.OnItemVisiable(mAllSinanItems[item.m_index], mCurrentSelectedSinanQuality, UpdatePutSinanInfo, dataMerge);
            }
        }

        private void InitSinanQulityDrop()
        {
            mSinanQulityTabDataList.Clear();
            mSinanQulityTabDataList.Add(new ComControlData(0, 0, "全部品阶", true));
            mSinanQulityTabDataList.Add(new ComControlData(3, 3, "1-3阶", false));
            mSinanQulityTabDataList.Add(new ComControlData(4, 4, "4-6阶", false));
            mSinanQulityTabDataList.Add(new ComControlData(5, 5, "7-9阶", false));
            mSinanQulityTabDataList.Add(new ComControlData(6, 6, "10-12阶", false));

            if (mSinanQulityTabDataList != null && mSinanQulityTabDataList.Count > 0)
            {
                var sinanQulityTabData = mSinanQulityTabDataList[0];
                for (int i = 0; i < mSinanQulityTabDataList.Count; i++)
                {
                    if (iDefaultSinanQuality == mSinanQulityTabDataList[i].Id)
                    {
                        sinanQulityTabData = mSinanQulityTabDataList[i];
                        break;
                    }
                }

                if (mSinanQulityDrop != null)
                {
                    mSinanQulityDrop.InitComDropDownControl(sinanQulityTabData, mSinanQulityTabDataList, OnSinanQulityDropDownItemClicked);
                }
            }
        }

        private void OnSinanQulityDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (iDefaultSinanQuality == comControlData.Id)
                return;

            //赋值选中的品质
            iDefaultSinanQuality = comControlData.Id;

            //根据选中的品质进行更新 
            LoadAllSinan();
        }

        public void LoadAllSinan()
        {
            if (mAllSinanItems == null)
            {
                mAllSinanItems = new List<ItemData>();
            }

            mAllSinanItems.Clear();

            var itemIds = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.MATERIAL);
            for (int i = 0; i < itemIds.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                if (itemData == null)
                {
                    continue;
                }
                if (itemData.SubType != (int)ProtoTable.ItemTable.eSubType.ST_SINAN)
                {
                    continue;
                }

                if (itemData.PackageType == EPackageType.Storage)
                {
                    continue;
                }

                if (itemData.PackageType == EPackageType.RoleStorage)
                {
                    continue;
                }

                if (iDefaultSinanQuality != 0)
                {
                    if ((int)itemData.Quality != iDefaultSinanQuality)
                        continue;
                }

                mAllSinanItems.Add(itemData);
            }

            mAllSinanItems.Sort(Sort);

            SetElementAmount();
        }

        private void SetElementAmount()
        {
            mSinanUIListScript.SetElementAmount(mAllSinanItems.Count);
        }

        private void UpdatePutSinanInfo(ItemData itemData, SinanItemElement element)
        {
            if (itemData == null)
            {
                return;
            }

            if (dataMerge.leftItem != null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("选择数量已达上限");
                return;
            }

            int allCount = itemData.Count;
            int count = 0;
            if (dataMerge.leftItem != null)
            {
                if (dataMerge.leftItem.GUID == itemData.GUID)
                {
                    count++;
                }
            }

            //如果同样的司南已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation("放入失败，该司南已放入融合区");
                return;
            }

            mCurrentSelectedSinanQuality = (int)itemData.Quality;
            dataMerge.leftItem = itemData;

            mSinanItemA.UpdateSinanItem(dataMerge.leftItem);
            UpdateChapterEntry();

            if (element != null)
            {
                element.SetCheckMaskRoot(true);
            }

            SetElementAmount();
        }

        public int Sort(ItemData left, ItemData right)
        {
            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            return right.LevelLimit - left.LevelLimit;
        }

        /// <summary>
        /// 清空槽位数据
        /// </summary>
        /// <param name="isBxyA"></param>
        private void OnSinanEmptyClick()
        {
            dataMerge.leftItem = null;

            if (dataMerge.leftItem == null)
            {
                mCurrentSelectedSinanQuality = 0;
            }
            UpdateChapterEntry();

            SetElementAmount();
        }

        #endregion

        private void Awake()
        {
            BindEvents();
            InitSinanUIListScript();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            UnInitSinanUIListScript();
            ClearData();
        }

        private void BindEvents()
        {

            if (leftChapterButton != null)
            {
                leftChapterButton.onClick.RemoveAllListeners();
                leftChapterButton.onClick.AddListener(OnLeftChapterButtonClick);
            }

            if (rightChapterButton != null)
            {
                rightChapterButton.onClick.RemoveAllListeners();
                rightChapterButton.onClick.AddListener(OnRightChapterButtonClick);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (mDropButton != null)
            {
                mDropButton.onClick.RemoveAllListeners();
                mDropButton.onClick.AddListener(OnDropButtonClick);
            }

            if (mStartContinueButton != null)
            {
                mStartContinueButton.onClick.RemoveAllListeners();
                mStartContinueButton.onClick.AddListener(OnStartContinueButtonClick);
            }

            if (mStrategySkillsButton != null)
            {
                mStrategySkillsButton.onClick.RemoveAllListeners();
                mStrategySkillsButton.onClick.AddListener(OnStrategySkillsBtnClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnChallengeChapterFinishChange,
                OnChallengeChapterFinishChange);

            var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_WEEK_HELL_FAIL_RETURN_TICKETS);
            if (systemValueTable != null)
            {
                failureReturnValue = systemValueTable.Value;
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, OnUpdateChapterDropProgress);
        }

        private void UnBindEvents()
        {
            if(leftChapterButton != null)
                leftChapterButton.onClick.RemoveAllListeners();

            if(rightChapterButton != null)
                rightChapterButton.onClick.RemoveAllListeners();

            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (mDropButton != null)
                mDropButton.onClick.RemoveAllListeners();

            if (mStartContinueButton != null)
                mStartContinueButton.onClick.RemoveAllListeners();

            if (mStrategySkillsButton != null)
                mStrategySkillsButton.onClick.RemoveAllListeners();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnChallengeChapterFinishChange,
            OnChallengeChapterFinishChange);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, OnUpdateChapterDropProgress);
        }

        private void ClearData()
        {
            _chapterParamDataModel = null;
            _chapterIdList = null;
            _baseChapterIndex = 0;
            _mapItemId = 0;
            _baseChapterId = 0;
            _selectedChapterId = 0;
            _dungeonTable = null;
            _chapterLevelDataModelList = null;
            _modelType = DungeonModelTable.eType.Type_None;
            _chapterShopId = 0;
            failureReturnValue = 0;
        }

        public void InitView(ChallengeChapterParamDataModel chapterParamDataModel)
        {
            _chapterParamDataModel = chapterParamDataModel;

            if (chapterParamDataModel != null)
            {
                _mapItemId = chapterParamDataModel.BaseChapterId;
                _baseChapterId = chapterParamDataModel.BaseChapterId;
                _selectedChapterId = chapterParamDataModel.SelectedChapterId;
                _chapterIdList = chapterParamDataModel.ChapterIdList;
                _modelType = chapterParamDataModel.ModelType;

                UpdateChapterIdOfWeekHell();
            }
            else
            {
                Logger.LogErrorFormat("ChapterParamDataModel is null");
                return;
            }
            _dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_selectedChapterId);
            if (_dungeonTable == null)
            {
                Logger.LogErrorFormat("DungeonTable is null and selectedChapterId is {0}", _selectedChapterId);
                return;
            }

            mTextName.SafeSetText(_dungeonTable.Name);

            InitContent();
            if (ChallengeUtility.isYunShangChangAn(_dungeonTable.ID))
            {
                chapterLevelControlRoot.SetActive(false);
                chapterSelected.SetActive(false);
                mTicketConsumeRoot.CustomActive(false);
                sinanListRoot.SetActive(true);
                sinanRoot.SetActive(true);
                mSinanItemA.InitSinanItem(OnSinanEmptyClick);
                InitSinanQulityDrop();
                LoadAllSinan();
            }
            else
            {
                chapterLevelControlRoot.SetActive(true);
                chapterSelected.SetActive(true);
                // mTicketConsumeRoot.CustomActive(true);
                sinanListRoot.SetActive(false);
                sinanRoot.SetActive(false);
            }
        }

        private void InitContent()
        {

            InitChapterMoney();

            InitChapterLevel();

            InitChapterSelectButtonInfo();

            InitFailDesc();

        }

        private void InitChapterMoney()
        {
            if (chapterMoneyControl != null)
                chapterMoneyControl.InitMoneyControl(_dungeonTable);
        }

        #region ChapterChange
        private void InitChapterSelectButtonInfo()
        {
            InitChapterSelectIndex();
            UpdateChapterSelectButton();
        }

        private void InitChapterSelectIndex()
        {
            if (_chapterIdList == null || _chapterIdList.Count <= 0)
                return;

            _baseChapterIndex = 0;
            for (var i = 0; i < _chapterIdList.Count; i++)
            {
                if (_chapterIdList[i] == _baseChapterId)
                {
                    _baseChapterIndex = i;
                    return;
                }
            }

            return;
        }
        #endregion

        //初始化章节的等级信息: 普通，王者等
        private void InitChapterLevel()
        {
            UpdateChapterLevelControl();

            UpdateChapterContent();
        }

        #region ChapterLevelInfo
        //更新章节的难度等级
        private void UpdateChapterLevelControl()
        {
            var mDungeonId = new DungeonID(0);
            mDungeonId.dungeonID = _baseChapterId;

            if (_chapterLevelDataModelList == null)
                _chapterLevelDataModelList = new List<ChallengeChapterLevelDataModel>();
            _chapterLevelDataModelList.Clear();

            //周常深渊入口，或者周常深渊的前置关卡或者限时活动中的堕落深渊
            if(DungeonUtility.IsWeekHellEntryDungeon(mDungeonId.dungeonID) == true
               || DungeonUtility.IsWeekHellPreDungeon(mDungeonId.dungeonID) == true
               || DungeonUtility.IsLimitTimeHellDungeon(mDungeonId.dungeonID) == true)
            {
                //只有一个王者难度，特殊处理
                var chapterLevelDataModel = new ChallengeChapterLevelDataModel(
                    0,
                    mDungeonId.dungeonID,
                    false);
                chapterLevelDataModel.IsSelected = true;
                
                _chapterLevelDataModelList.Add(chapterLevelDataModel);
            }
            else
            {
                var curTopHard = ChapterUtility.GetDungeonTopHard(mDungeonId.dungeonIDWithOutDiff);
                for (var i = 0; i <= curTopHard; i++)
                {
                    mDungeonId.diffID = i;
                    var chapterLevelDataModel = new ChallengeChapterLevelDataModel(
                        i,
                        mDungeonId.dungeonID,
                        false);

                    if (_selectedChapterId == mDungeonId.dungeonID)
                        chapterLevelDataModel.IsSelected = true;

                    _chapterLevelDataModelList.Add(chapterLevelDataModel);
                }
            }

            if (chapterLevelControl != null)
                chapterLevelControl.InitLevelControl(_chapterLevelDataModelList, OnChapterLevelButtonClick);
        }

        private void OnChapterLevelButtonClick(int levelId, int dungeonId)
        {
            _selectedChapterId = dungeonId;
            _dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);

            if (_dungeonTable != null)
            {
                UpdateChapterContent();
            }
        }
        #endregion

        #region ChapterDetailInfo
        //根据关卡的ID，更新关卡的相关内容
        private void UpdateChapterContent()
        {
            UpdateChapterNormalControl();
            UpdateChapterDropControl();
            UpdateChapterEntry();
            UpdateChapterDrug();
            RefreshYiJieShowInfo();
        }

        //更新关卡名字，图片和描述
        private void UpdateChapterNormalControl()
        {
            if (chapterNormalControl != null)
            {
                chapterNormalControl.UpdateNormalControl(_selectedChapterId, _dungeonTable);
            }
        }

        //更新关卡掉落信息
        private void UpdateChapterDropControl()
        {
            if (chapterDropControl != null)
                chapterDropControl.InitDropControl(_selectedChapterId, _mapItemId);
        }

        //更新关卡的入口
        private void UpdateChapterEntry()
        {
            if (chapterEntryControl != null)
                chapterEntryControl.UpdateEntryControl(_selectedChapterId, _dungeonTable, _baseChapterId, OnStartContinueButtonClick, dataMerge.leftItem);
        }

        private void UpdateChapterDrug()
        {
            if (chapterDrugControl != null)
                chapterDrugControl.UpdateDrugControl(_baseChapterId);
        }

        #endregion

        #region ChapterChangeButton
        private void OnLeftChapterButtonClick()
        {
            if (_chapterIdList == null || _chapterIdList.Count <= 0)
            {
                Logger.LogError("ChapterIDList is null");
                return;
            }

            if (_baseChapterIndex <= 0)
                return;

            DealWithStateByChapterButtonClick(true);

        }
        
        private void OnRightChapterButtonClick()
        {
            if (_chapterIdList == null || _chapterIdList.Count <= 0)
            {
                Logger.LogError("ChapterIDList is null");
                return;
            }

            if (_baseChapterIndex >= _chapterIdList.Count - 1)
                return;

            DealWithStateByChapterButtonClick(false);
        }

        private void DealWithStateByChapterButtonClick(bool isLeft)
        {
            if (isLeft == true)
                _baseChapterIndex -= 1;
            else
            {
                _baseChapterIndex += 1;
            }

            if (_baseChapterIndex < 0)
                _baseChapterIndex = 0;
            if (_baseChapterIndex > _chapterIdList.Count - 1)
                _baseChapterIndex = _chapterIdList.Count - 1;

            UpdateFrameMask(true);
            UpdateChapterSelectButton();

            var curSelectChapterId = _chapterIdList[_baseChapterIndex];
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChallengeChapterBeginChange, _modelType,curSelectChapterId);
        }

        private void UpdateFrameMask(bool flag)
        {
            if (frameMask != null)
                frameMask.gameObject.CustomActive(flag);
        }

        private void OnChallengeChapterFinishChange(UIEvent uiEvent)
        {
            UpdateFrameMask(false);

            if (_chapterIdList == null || _chapterIdList.Count <= 0
                                       || _baseChapterIndex < 0 || _baseChapterIndex >= _chapterIdList.Count)
            {
                Logger.LogErrorFormat("ChapterFinishChange is Error");
                return;
            }

            //更新关卡Id，
            _mapItemId = _chapterIdList[_baseChapterIndex];
            _baseChapterId = _chapterIdList[_baseChapterIndex];
            _selectedChapterId = _chapterIdList[_baseChapterIndex];
            UpdateChapterIdOfWeekHell();

            _dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_selectedChapterId);
            if (_dungeonTable == null)
            {
                Logger.LogErrorFormat(
                    "ChapterFinishChange and dungeonTable is null selectedChapterIndex is {0}, _chapterId is {1}",
                    _baseChapterIndex, _selectedChapterId);
                return;
            }


            InitChapterLevel();
            RefreshYiJieShowInfo();
        }

        private void OnUpdateChapterDropProgress(UIEvent ui)
        {
            if (_modelType == DungeonModelTable.eType.VoidCrackModel)
                UpdateLevelChallengeTimes(_selectedChapterId);

            UpdateDropProgress(_selectedChapterId);
        }

        //更新左右选择按钮
        private void UpdateChapterSelectButton()
        {
            if (_chapterIdList == null || _chapterIdList.Count <= 0)
            {
                ChallengeUtility.UpdateButtonState(leftChapterButton, leftChapterButtonGray, false);
                ChallengeUtility.UpdateButtonState(rightChapterButton, rightChapterButtonGray, false);
                return;
            }

            if (_baseChapterIndex <= 0)
            {
                ChallengeUtility.UpdateButtonState(leftChapterButton, leftChapterButtonGray, false);
            }
            else
            {
                ChallengeUtility.UpdateButtonState(leftChapterButton, leftChapterButtonGray, true);
            }

            if (_baseChapterIndex >= _chapterIdList.Count - 1)
            {
                ChallengeUtility.UpdateButtonState(rightChapterButton, rightChapterButtonGray, false);
            }
            else
            {
                ChallengeUtility.UpdateButtonState(rightChapterButton, rightChapterButtonGray, true);
            }
        }
        #endregion

        //如果关卡是周常深渊的入口，可能需要更新关卡的ID，
        private void UpdateChapterIdOfWeekHell()
        {
            //如果是周常深渊的入口地下城，设置选择的关卡未默认的关卡
            //判断前置任务是否正在进行中，
            //如果正在进行前置任务，则打开前置任务的地下城界面， 基本的地下城ID，为前置关卡的ID
            if (DungeonUtility.IsWeekHellEntryDungeon(_baseChapterId) == true)
            {
                _selectedChapterId = _baseChapterId;
                if (DungeonUtility.GetWeekHellPreTaskState(_baseChapterId) == WeekHellPreTaskState.IsProcessing)
                {
                    _baseChapterId = DungeonUtility.GetWeekHellPreTaskDungeonId(_baseChapterId);
                    _selectedChapterId = _baseChapterId;
                }
            }
        }


        private void OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChallengeChapterFrameClose, _modelType);
            ChallengeUtility.OnCloseChallengeChapterFrame();
        }

        private void OnDropButtonClick()
        {
            var frame = ClientSystemManager.instance.OpenFrame<ChapterDropProgressFrame>() as ChapterDropProgressFrame;
            frame.SetData(_selectedChapterId, mAreaIndex);
        }

        private void OnStartContinueButtonClick()
        {
            if(Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("can_not_enter_dungeon_in_cross_scene"));
                return;
            }
            if (mDropProgress != null && mDropProgress.activeSelf && GetLeftTimes() <= 0)
            {
                do
                {
                    if (ChapterNormalHalfFrame.IsYiJieDungeon(_selectedChapterId) && IsCurrentDungeonInChallenge())
                    {
                        break;
                    }

                    _usePassItem();
                    return;
                }
                while (false);
            }

            if (chapterEntryControl != null)
            {
                chapterEntryControl.OnStartButtonClick();
            }
        }

        private void OnStrategySkillsBtnClick()
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_selectedChapterId);
            if (mDungeonTable == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<CheckPointHelpFrame>(FrameLayer.Middle, mDungeonTable.PlayingDescription);
        }

        /// <summary>
        /// 初始化失败描述  只有时之漩涡副本显示
        /// </summary>
        private void InitFailDesc()
        {
            Logger.LogError("====================>_modelType=" + _modelType);
            if (mFailDesc != null)
            {
                if (_modelType == DungeonModelTable.eType.WeekHellModel || _modelType == DungeonModelTable.eType.ZhengzhanAntuenModel)
                {
                    // mFailDesc.gameObject.CustomActive(_modelType == DungeonModelTable.eType.WeekHellModel);
                    mFailDesc.gameObject.CustomActive(true);
                }
                else
                {
                    mFailDesc.gameObject.CustomActive(false);
                }

                mFailDesc.text = string.Format(failDesc, failureReturnValue);

                if (_modelType == DungeonModelTable.eType.ZhengzhanAntuenModel)
                {
                    mFailDesc.text = string.Format(failDesc, 5);
                }
            }
        }

        private void UpdateDungeonMissionInfo()
        {
            mMissionInfoRoot.CustomActive(false);

            if (ChapterUtility.GetDungeonMissionState(_selectedChapterId))
            {
                int missionID = (int)ChapterUtility.GetMissionIDByDungeonID(_selectedChapterId);

                mMissionInfoRoot.CustomActive(true);

                mMissionInfo.text = ChapterUtility.GetDungeonMissionInfo(_selectedChapterId);
                mDungeonUnitInfo.SetType(ChapterUtility.GetMissionType(missionID));
                mMissionContent.SetText(Utility.ParseMissionText(missionID, true), true);
            }
            else
            {
                mDungeonUnitInfo.SetType(ComChapterDungeonUnit.eMissionType.None);
            }

            mDungeonUnitInfo.SetEffect("Effects/UI/Prefab/EffUI_Yijie/Prefab/Eff_UI_YiJie_fangjian");
        }

        private void UpdateLevelChallengeTimes(int dungeonId)
        {
            if (mlevelChallengeTimesRoot == null)
                return;

            var dungeonDailyBaseTimes = DungeonUtility.GetDungeonDailyBaseTimes(dungeonId);
            if (dungeonDailyBaseTimes <= 0)
            {
                mlevelChallengeTimesRoot.CustomActive(false);
            }
            else
            {
                mlevelChallengeTimesRoot.CustomActive(true);
                if (mLevelChallengeTimesNumber != null)
                {
                    var leftTimes = GetLeftTimes();
                    mLevelChallengeTimesNumber.text = string.Format(TR.Value("resist_magic_challenge_times"),
                        leftTimes, dungeonDailyBaseTimes);
                }
            }
        }

        private int GetLeftTimes()
        {
            var finishedTimes = DungeonUtility.GetDungeonDailyFinishedTimes(_selectedChapterId);
            var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(_selectedChapterId);
            var leftTimes = dailyMaxTime - finishedTimes;
            if (leftTimes < 0)
                leftTimes = 0;

            return leftTimes;
        }

        private void UpdateDropProgress(int dungeonId)
        {
            if (mDropProgress == null)
                return;

            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return;

            if (dungeonTable.SubType != DungeonTable.eSubType.S_DEVILDDOM)
            {
                mDropProgress.CustomActive(false);
            }
            else
            {
                mDropProgress.CustomActive(true);
                GameFrameWork.instance.StartCoroutine(_onWorldDungeonGetAreaIndex());
            }
        }

        private IEnumerator _onWorldDungeonGetAreaIndex()
        {
            var req = new WorldDungeonGetAreaIndexReq();
            req.dungeonId = (uint)_selectedChapterId;
            var res = new WorldDungeonGetAreaIndexRes();
            var msg = new MessageEvents();

            yield return MessageUtility.Wait(ServerType.GATE_SERVER, msg, req, res);

            if (msg.IsAllMessageReceived())
            {
                mAreaIndex = res.areaIndex >> 1;
                mDropButtonEffect.CustomActive(mAreaIndex > 0);
            }
        }

        public bool IsCurrentDungeonInChallenge()
        {
            DungeonID dungeonID = new DungeonID(_selectedChapterId);
            if (dungeonID != null)
            {
                dungeonID.diffID = 0;
                bool isInChallenge = ChapterSelectFrame.IsInChallenge(dungeonID.dungeonID);
                return isInChallenge;
            }

            return false;
        }

        private void RefreshYiJieShowInfo()
        {
            if (_modelType == DungeonModelTable.eType.VoidCrackModel || _modelType == DungeonModelTable.eType.ZhengzhanAntuenModel)
            {
                UpdateDungeonMissionInfo();
                UpdateLevelChallengeTimes(_selectedChapterId);
                UpdateDropProgress(_selectedChapterId);
                UpdateStrategySkillsBtn();

                if (ChapterNormalHalfFrame.IsYiJieDungeon(_selectedChapterId))
                {
                    bool bShow = IsCurrentDungeonInChallenge();
                    mStartContinueButtonRoot.CustomActive(bShow);
                    mStartButtonRoot.CustomActive(!bShow);
                }
                
                mDetailButtonRoot.CustomActive(false);
                if (_modelType == DungeonModelTable.eType.VoidCrackModel)
                {
                    mTicketConsumeRoot.CustomActive(false);
                }
            }
            else if(_modelType == DungeonModelTable.eType.YunShangChangAnModel)
            {
                mMissionInfoRoot.CustomActive(false);
                mDropProgress.CustomActive(false);
                mStrategySkillsButton.CustomActive(false);
                mDetailButtonRoot.CustomActive(false);
            }
            else
            {
                mMissionInfoRoot.CustomActive(false);
                mDropProgress.CustomActive(false);
                mStrategySkillsButton.CustomActive(false);
                mDetailButtonRoot.CustomActive(true);
                //mTicketConsumeRoot.CustomActive(true);
            }
        }

        private void _usePassItem()
        {
            bool useFlag = false;
            int[] itemIdArray = new int[] { 800000798, 330000200, 330000194 };//虚空通行证
            for (int i = 0; i < itemIdArray.Length; i++)
            {
                var itemId = itemIdArray[i];
                if (ItemDataManager.GetInstance().GetOwnedItemCount(itemId) >= 1)
                {
                    var item = ItemDataManager.GetInstance().GetItemByTableID(itemId);
                    if (item != null)
                    {
                        if (item.GetCurrentRemainUseTime() <= 0)
                            useFlag = true;
                        else
                        {
                            SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("drop_progress_challenge_times_not_enough_has_item"), () =>
                            {
                                ItemDataManager.GetInstance().UseItemWithoutDoubleCheck(item);
                            });
                            return;
                        }
                    }
                }
            }
            if (useFlag)
                SystemNotifyManager.SystemNotify(1226);
            else
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_redpacket_has_no_cost_time"));
        }

        private void UpdateStrategySkillsBtn()
        {
            bool isFlag = StrategySkillsBtnIsShow();
            mStrategySkillsButton.CustomActive(isFlag);
        }

        /// <summary>
        /// 攻略技巧按钮是否显示
        /// </summary>
        /// <returns></returns>
        private bool StrategySkillsBtnIsShow()
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_selectedChapterId);
            if (mDungeonTable == null)
            {
                return false;
            }

            if (mDungeonTable.PlayingDescription == "" || mDungeonTable.PlayingDescription == null)
            {
                return false;
            }

            return true;
        }
    }
}
