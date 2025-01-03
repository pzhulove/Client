using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public delegate void OnChallengeMapItemClicked(int chapterId);

    public class ChallengeMapItem : MonoBehaviour
    {

        private ChaptertDungeonUnit _dungeonUnit;
        private int _dungeonId = 0;
        private bool _isSelected = false;       //关卡是否被选中
        private bool _isLocked = false;         //关卡是否被解锁
        private DungeonTable _dungeonTable = null;
        private ActivityDungeonSub _activityDungeonSub = null;          //活动地下城的相关数据

        private OnChallengeMapItemClicked _onMapItemClicked = null;


        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(5)]
        [SerializeField] private Text name;
        [SerializeField] private Image backImage;

        [Space(20)]
        [HeaderAttribute("ChapterState")]
        [Space(10)]
        [SerializeField] private GameObject itemLock;
        [SerializeField] private Text lockLevelText;
        [SerializeField] private Text recommendLevelText;

        //深渊活动的天数
        [SerializeField] private CanvasGroup chapterNumberRoot;
        [SerializeField] private CanvasGroup todayNumberCanvas;
        [SerializeField] private CanvasGroup weekNumberCanvas;
        [SerializeField] private Text todayNumberText;
        [SerializeField] private Text weekNumberText;

        [SerializeField] private Image specialFrame;
        //深渊掉落的介绍
        [SerializeField] private Text chapterDropText;
        //深渊掉落的介绍的背景
        [SerializeField] private CanvasGroup chapterDropCanvas;

        //剩余时间
        [SerializeField] private GameObject chapterActivityRoot;
        [SerializeField] private GameObject chapterActivityFlag;
        [SerializeField] private Text chapterActivityLeftTimeText;
        [SerializeField] private SimpleTimer simpleTimer;

        [Space(10)]
        [HeaderAttribute("Position")]
        [Space(5)]
        [SerializeField] private RectTransform sourcePosition;
        [SerializeField] private RectTransform targetPosition;
        [SerializeField] private RectTransform targetRightPosition;
        [SerializeField] private RectTransform contentRoot;

        [Space(10)]
        [HeaderAttribute("Button")]
        [Space(5)]
        [SerializeField] private Button chapterButton;
        [SerializeField] private Button chapterSettingButton;
        [SerializeField] private CommonHelpNewAssistant commonHelpNewAssistant;
        [SerializeField] private GameObject challengeFlag;
        [SerializeField] private ComChapterDungeonUnit comChapterDungeonUnit;

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
            if (chapterButton != null)
            {
                chapterButton.onClick.RemoveAllListeners();
                chapterButton.onClick.AddListener(OnChapterItemClick);
            }

            if (chapterSettingButton != null)
            {
                chapterSettingButton.onClick.RemoveAllListeners();
                chapterSettingButton.onClick.AddListener(OnChapterItemClick);
            }
        }

        private void UnBindEvents()
        {
            if(chapterButton != null)
                chapterButton.onClick.RemoveAllListeners();

            if(chapterSettingButton != null)
                chapterSettingButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _dungeonId = 0;
            _dungeonTable = null;
            _activityDungeonSub = null;

        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public void InitItem(ChaptertDungeonUnit chapterDungeonUnit,
            OnChallengeMapItemClicked onMapItemClicked,
            ActivityDungeonSub activityDungeonSub = null,
            bool isDefaultSelected = false)
        {
            if (chapterDungeonUnit == null)
            {
                Logger.LogErrorFormat("ChapterDungeonUnit is null");
                return;
            }

            _dungeonUnit = chapterDungeonUnit;
            _dungeonId = _dungeonUnit.dungeonID;
            _onMapItemClicked = onMapItemClicked;
            _activityDungeonSub = activityDungeonSub;
            _isSelected = isDefaultSelected;

            _dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_dungeonId);
            if (_dungeonTable == null)
            {
                Logger.LogErrorFormat("dungeonTable is null");
                return;
            }

            if (commonHelpNewAssistant != null)
                commonHelpNewAssistant.HelpId = _dungeonId;

            InitView();
        }

        private void InitView()
        {
            InitItemPosition();

            InitItemImage();

            InitItemContent();

            UpdateItemSelected();
        }

        private void InitItemPosition()
        {
            gameObject.transform.localPosition = _dungeonUnit.position;

            sourcePosition.localPosition = _dungeonUnit.angleSourcePosition;
            targetPosition.localPosition = _dungeonUnit.angleTargetPosition;
            targetRightPosition.localPosition = _dungeonUnit.angleTargetRightPosition;

            contentRoot.localPosition = _dungeonUnit.thumbOffset;
        }

        private void InitItemContent()
        {
            if (name != null)
                name.text = _dungeonTable.Name;

            if (comChapterDungeonUnit != null)
            {
                comChapterDungeonUnit.SetType(ComChapterDungeonUnit.eMissionType.None);
            }

            if (_dungeonTable.SubType == DungeonTable.eSubType.S_DEVILDDOM)
            {
                if (recommendLevelText != null)
                    recommendLevelText.text = string.Format(TR.Value("challenge_chapter_recommend_level"),
                          _dungeonTable.RecommendLevel);

                if(challengeFlag != null)
                {
                    challengeFlag.CustomActive(ChapterSelectFrame.IsInChallenge(_dungeonId));
                }

                if (ChapterUtility.HasMissionByDungeonID(_dungeonId))
                {
                    if (comChapterDungeonUnit != null)
                    {
                        comChapterDungeonUnit.SetType(ComChapterDungeonUnit.eMissionType.Main);     //其实就是控制任务flag显示与否
                    }
                }

                if (commonHelpNewAssistant != null)
                {
                    commonHelpNewAssistant.gameObject.CustomActive(false);
                }
            }
            else
            {
                if (recommendLevelText != null)
                    recommendLevelText.text = _dungeonTable.RecommendLevel;

                if (challengeFlag != null)
                {
                    challengeFlag.CustomActive(false);
                }

                if (commonHelpNewAssistant != null)
                {
                    commonHelpNewAssistant.gameObject.CustomActive(true);
                }
            }
           
            UpdateDungeonItemContent();
        }

        public void UpdateDungeonItemContent()
        {
            if (_dungeonId == 3101000)
            {
                chapterDropText.text = "金币掉落";
                return;
            }
            if (_dungeonId == 3201000)
            {
                chapterDropText.text = "时装兑换材料掉落";
                return;
            }
            if (_activityDungeonSub == null)
            {
                return;
            }
                

            //活动深渊，活动已经结束，或者活动不存在，活动时间已经过了，则该深渊不显示
            if (DungeonUtility.IsLimitTimeHellDungeon(_dungeonId) == true)
            {
                //活动状态
                if (_activityDungeonSub.state == eActivityDungeonState.End
                    || _activityDungeonSub.state == eActivityDungeonState.None)
                {
                    gameObject.CustomActive(false);
                    return;
                }

                //结束时间已经过了,当前时间大于结束时间
                if (TimeManager.GetInstance().GetServerTime() > _activityDungeonSub.endtime)
                {
                    gameObject.CustomActive(false);
                    return;
                }
            }

            ResetItemContent();

            if (chapterDropText != null)
            {
                chapterDropText.text = _activityDungeonSub.table.ExtraDescription;
            }

            if (chapterDropCanvas != null)
            {
                chapterDropCanvas.CustomActive(!string.IsNullOrEmpty(_activityDungeonSub.table.ExtraDescription));
            }

            UpdateDungeonItemNumberAndTimes();      //刷新下挑战次数之类的

            UpdateDungeonItemState();       //刷新下该mapitem的状态比如锁定状态之类的
        }

        //更新Item的剩余次数和时间
        private void UpdateDungeonItemNumberAndTimes()
        {
            if (_activityDungeonSub.table == null)
                return;

            //显示剩余次数
            if (chapterNumberRoot != null)
            {
                chapterNumberRoot.CustomActive(_activityDungeonSub.table.ShowCount);
            }

            UpdateItemNumbers(false);
            //显示剩余次数的具体内容
            if (_activityDungeonSub.table.ShowCount == true)
            {

                var dailyLeftTime = DungeonUtility.GetDungeonDailyLeftTimes(_dungeonId);
                var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(_dungeonId);
                UpdateDungeonDailyNumber(dailyLeftTime, dailyMaxTime);
                
                //周常深渊，只有在前置任务完成的时候，才显示次数, 否则不显示次数
                if (DungeonUtility.IsWeekHellEntryDungeon(_dungeonId) == true)
                {
                    //任务完成，并且关卡解锁，表示正常关卡显示次数，否则不显示次数
                    if (DungeonUtility.GetWeekHellPreTaskState(_dungeonId) == WeekHellPreTaskState.IsFinished
                        && ChapterUtility.GetDungeonState(_dungeonId) != ComChapterDungeonUnit.eState.Locked)
                    {
                        //每周次数
                        var weekLeftTime = DungeonUtility.GetDungeonWeekLeftTimes(_dungeonId);
                        var weekMaxTime = DungeonUtility.GetDungeonWeekMaxTimes(_dungeonId);
                        UpdateDungeonWeekNumber(weekLeftTime, weekMaxTime);

                        //每日剩余次数大于每周剩余次数，则每日剩余次数设置为每周剩余次数
                        if (dailyLeftTime > weekLeftTime)
                        {
                            dailyLeftTime = weekLeftTime;
                            UpdateDungeonDailyNumber(dailyLeftTime, dailyMaxTime);
                        }
                    }
                    else
                    {
                        UpdateItemNumbers(false);
                    }
                }
            }

            //活动深渊
            if(DungeonUtility.IsLimitTimeHellDungeon(_dungeonId))
            {
                if (chapterActivityRoot != null)
                {
                    chapterActivityRoot.gameObject.CustomActive(true);
                }

                if (simpleTimer != null)
                {
                    simpleTimer.SetCountdown((int) (_activityDungeonSub.endtime -
                                                    TimeManager.GetInstance().GetServerTime()));
                    simpleTimer.StartTimer();
                }
            }
        }

        private void UpdateDungeonItemState()
        {
            if (_activityDungeonSub == null)
                return;

            _isLocked = false;

            switch (_activityDungeonSub.state)
            {
                case eActivityDungeonState.Start:
                    if (recommendLevelText != null)
                    {
                        recommendLevelText.gameObject.CustomActive(true);
                        recommendLevelText.text = string.Format(TR.Value("challenge_chapter_recommend_level"),
                            _activityDungeonSub.GetDungeonRecommendLevel());
                    }
                    break;
                case eActivityDungeonState.None:
                case eActivityDungeonState.LevelLimit:
                    //未解锁
                    _isLocked = true;
                    if (itemLock != null)
                        itemLock.gameObject.CustomActive(true);
                    if (lockLevelText != null)
                    {
                        lockLevelText.gameObject.CustomActive(true);
                        lockLevelText.text = string.Format(TR.Value("challenge_chapter_level_unlock"),
                            _activityDungeonSub.level);
                    }

                    UpdateItemNumbers(false);
                    break;
            }
        }

        private void ResetItemContent()
        {
            if (itemLock != null)
                itemLock.gameObject.CustomActive(false);

            if (recommendLevelText != null)
                recommendLevelText.gameObject.CustomActive(false);

            if (lockLevelText != null)
                lockLevelText.gameObject.CustomActive(false);


            if (chapterNumberRoot != null)
            {
                chapterNumberRoot.CustomActive(false);
            }

            if (chapterActivityRoot != null)
                chapterActivityRoot.gameObject.CustomActive(false);
        }

        //Item的背景图片 和 Icon图片
        private void InitItemImage()
        {
            if (backImage != null)
            {
                if (string.IsNullOrEmpty(_dungeonTable.TumbPath) == true)
                {
                    backImage.sprite = null;
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref backImage, _dungeonTable.TumbPath);
                }
            }

            //if (iconImage != null)
            //{
            //    if (string.IsNullOrEmpty(_dungeonTable.TumbChPath) == true)
            //    {
            //        iconImage.sprite = null;
            //    }
            //    else
            //    {
            //        //加载Icon的图片
            //        ETCImageLoader.LoadSprite(ref iconImage, _dungeonTable.TumbChPath);

            //        //调整Icon的大小和位置
            //        RectTransform rect = iconImage.rectTransform;

            //        float originHeight = rect.sizeDelta.y;

            //        iconImage.SetNativeSize();
            //        iconImage.CustomActive(null != iconImage.sprite);

            //        float newHeight = rect.sizeDelta.y;
            //        rect.sizeDelta = rect.sizeDelta * (originHeight / newHeight);
            //        rect.localScale = Vector3.one;
            //    }
            //}

            if (_dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
            {
                UpdateSpecialFrame(true);
            }
            else
            {
                UpdateSpecialFrame(false);
            }

        }

        private void UpdateItemSelected()
        {
            UpdateItemSelectedEffect(_isSelected);
        }

        public void UpdateSelectedStateByDungeonId(int selectedDungeonId)
        {
            if (_dungeonId == selectedDungeonId)
            {
                if (_isSelected == false)
                {
                    _isSelected = true;
                    UpdateItemSelectedEffect(_isSelected);
                }
            }
            else
            {
                _isSelected = false;
                UpdateItemSelectedEffect(_isSelected);
            }
        }

        private void UpdateItemSelectedEffect(bool flag)
        {
        }

        //是否显示剩余次数
        private void UpdateItemNumbers(bool flag)
        {
            todayNumberCanvas.CustomActive(flag);
            weekNumberCanvas.CustomActive(flag);
        }

        private void OnChapterItemClick()
        {
            //未解锁
            if (_isLocked == true)
            {
                return;
            }

            //等级不足
            if (_activityDungeonSub != null)
            {
                if (PlayerBaseData.GetInstance().Level < _activityDungeonSub.level)
                {
                    SystemNotifyManager.SystemNotify(1008);
                    return;
                }
            }

            //活动深渊：次数不足或者活动结束
            if (DungeonUtility.IsLimitTimeHellDungeon(_dungeonId) == true)
            {
                //活动完成
                if (_activityDungeonSub != null && _activityDungeonSub.isfinish == true)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fallen_hell_num_des"));
                    return;
                }

                //次数为0
                if (DungeonUtility.GetDungeonDailyLeftTimes(_dungeonId) <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fallen_hell_num_des"));
                    return;
                }

                //活动结束
                if (_activityDungeonSub != null)
                {
                    if (TimeManager.GetInstance().GetServerTime() > _activityDungeonSub.endtime)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("activity_limit_time_over_tip"));
                        return;
                    }
                }
            }

            //周常深渊入口
            if (DungeonUtility.IsWeekHellEntryDungeon(_dungeonId) == true)
            {
                //得到前置任务的状态
                WeekHellPreTaskState weekHellPreTaskState = DungeonUtility.GetWeekHellPreTaskState(_dungeonId);
                if (weekHellPreTaskState == WeekHellPreTaskState.None)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("activity_week_hell_pre_task_not_exist"));
                    return;
                }
                else if (weekHellPreTaskState == WeekHellPreTaskState.UnReceived)
                {
                    ChallengeUtility.OnShowWeekHellPreTaskUnReceivedTip(_dungeonId);
                    return;
                }
                else if (weekHellPreTaskState == WeekHellPreTaskState.IsFinished)
                {
                    ////周常深渊入口活动,每周次数为0， 每日次数为0
                    if (DungeonUtility.GetDungeonWeekLeftTimes(_dungeonId) <= 0)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("activity_week_hell_week_time_zero"));
                        return;
                    }
                    if (DungeonUtility.GetDungeonDailyLeftTimes(_dungeonId) <= 0)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fallen_hell_num_des"));
                        return;
                    }
                }
            }
            
            _isSelected = true;

            UpdateItemSelectedEffect(_isSelected);
            if (_onMapItemClicked != null)
                _onMapItemClicked(_dungeonId);
        }

        private void UpdateSpecialFrame(bool flag)
        {
            if (specialFrame != null)
                specialFrame.gameObject.CustomActive(flag);
        }

        private void UpdateDungeonDailyNumber(int leftTime, int maxTime)
        {
            if (todayNumberText != null)
            {
                var todayNumberStr = string.Format(TR.Value("challenge_chapter_today_number"),
                    leftTime,
                    maxTime);

                todayNumberText.text = todayNumberStr;
                todayNumberCanvas.CustomActive(true);
            }
        }

        private void UpdateDungeonWeekNumber(int leftTime, int maxTime)
        {
            if (weekNumberText != null)
            {
                var weekNumberStr = string.Format(TR.Value("challenge_chapter_week_number"),
                    leftTime,
                    maxTime);

                weekNumberText.text = weekNumberStr;
                weekNumberCanvas.CustomActive(true);
            }
        }

        public int GetChapterDungeonId()
        {
            return _dungeonId;
        }

    }
}
