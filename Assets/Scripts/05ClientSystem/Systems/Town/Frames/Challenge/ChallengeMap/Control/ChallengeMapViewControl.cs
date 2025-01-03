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

    public class ChallengeMapViewControl : MonoBehaviour
    {
        private DungeonModelTable.eType _modelType;

        private ChallengeMapParamDataModel _challengeParamDataModel;
        private int _challengeSceneId;          //场景ID
        private int _defaultChapterId;          //默认选中的关卡ID， 对应的MapItemID
        private List<ChallengeMapItem> _mapItemList;
        private List<int> _chapterIdList;           //可以选择的章节Id;

        private DChapterData _chapterData;      //章节数据，根据表中的数据，加载进来

        private List<ActivityDungeonSub> _activityDungeonSubList;

        private EChapterSelectAnimateState _eChapterSelectAnimateState = EChapterSelectAnimateState.OnNone;

        private Coroutine _delayOpenFrameCoroutine = null;
        private Coroutine _chapterFrameCloseCoroutine = null;
        private Coroutine _chapterFrameChangeCoroutine = null;

        private OnContentEffectAction _onContentEffectAction = null;

        [Space(10)]
        [HeaderAttribute("Image")]
        [Space(10)]
        [SerializeField] private Image titleImage;
        [SerializeField] private Image backgroundImage;

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]
        [SerializeField] private GameObject contentRoot;
        [SerializeField] private GameObject mapItemRoot;
        [SerializeField] private GameObject mapItemPrefab;

        [Space(10)]
        [HeaderAttribute("Action")]
        [Space(10)]
        [SerializeField]
        private ComChapterSelectAnimate comChapterSelectAnimate;

        [Space(10)]
        [HeaderAttribute("YiJieChallengeNum")]
        [SerializeField]private GameObject yijieChallengeNumRoot;
        [SerializeField]private Text yijieChallengeNumInfo;
        [SerializeField]private Text yijieChallengeNumInfoPre;
        [SerializeField]private Text yijieChallengeNumInfoTip;

        [SerializeField]private GameObject effectRoot;

        [Space(10)] [HeaderAttribute("TopRight")] [Space(10)]
        [SerializeField] private GameObject topRightRoot;
        [SerializeField] private CommonShopButtonControl commonShopButtonControl;
        [SerializeField] private Button legendButton;

        GameObject YiJieEffectObj = null;
        private void Awake()
        {
            InitData();
            BindUiEvents();
        }

        private void OnDestroy()
        {
            ClearData();
            UnBindUiEvents();
        }

        private void BindUiEvents()
        {
            if (legendButton != null)
            {
                legendButton.onClick.RemoveAllListeners();
                legendButton.onClick.AddListener(OnLegendButtonClicked);
            }
        }

        private void UnBindUiEvents()
        {
            if(legendButton != null)
                legendButton.onClick.RemoveAllListeners();
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
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityDungeonUpdate, OnActivityDungeonUpdate);
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnChallengeChapterFrameClose, OnChallengeChapterFrameClose);
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnChallengeChapterBeginChange, OnChallengeChapterBeginChange);
        }

        private void UnBindUiMessages()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityDungeonUpdate, OnActivityDungeonUpdate);
            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnChallengeChapterFrameClose, OnChallengeChapterFrameClose);
            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnChallengeChapterBeginChange, OnChallengeChapterBeginChange);
        }

        private void InitData()
        {
            if (_chapterIdList == null)
                _chapterIdList = new List<int>();

            if (_mapItemList == null)
                _mapItemList = new List<ChallengeMapItem>();
        }

        private void ClearData()
        {
            _modelType = DungeonModelTable.eType.Type_None;

            _challengeParamDataModel = null;
            _challengeSceneId = 0;
            _defaultChapterId = 0;
            _chapterData = null;

            _activityDungeonSubList = null;

            if (_mapItemList != null)
            {
                _mapItemList.Clear();
                _mapItemList = null;
            }

            if (_chapterIdList != null)
            {
                _chapterIdList.Clear();
                _chapterIdList = null;
            }

            _eChapterSelectAnimateState = EChapterSelectAnimateState.OnNone;
            _delayOpenFrameCoroutine = null;
            _chapterFrameCloseCoroutine = null;
            _chapterFrameChangeCoroutine = null;
            _onContentEffectAction = null;

            effectRoot = null;
        }

        //需要传递默认的参数
        public void InitMapModelControl(DungeonModelTable.eType modelType,
            ChallengeMapParamDataModel paramDataModel,
            OnContentEffectAction onContentEffectAction = null)
        {
            _modelType = modelType;
            _challengeParamDataModel = paramDataModel;
            _challengeSceneId = ChallengeUtility.GetChallengeDungeonMapIdByModelType(_modelType); 
            _onContentEffectAction = onContentEffectAction;

            if (_challengeParamDataModel != null)
            {
                _defaultChapterId = _challengeParamDataModel.BaseDungeonId;
            }

            //右上角的商店和传奇之路按钮
            InitTopRightContent();

            //SceneTable是否存在
            var citySceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(_challengeSceneId);
            if (citySceneTable == null)
            {
                Logger.LogErrorFormat("CitySceneTable is null and challengeId is {0}", _challengeSceneId);
                return;
            }

            //ChapterData是否存在
            if (citySceneTable.ChapterData == null || citySceneTable.ChapterData.Count <= 0)
            {
                Logger.LogErrorFormat("CitySceneTable ChapterData is null or count is zero");
                return;
            }

            string chapterDataPath = citySceneTable.ChapterData[0];
            _chapterData = AssetLoader.instance.LoadRes(chapterDataPath, typeof(DChapterData)).obj as DChapterData;
            //资源是否存在
            if (_chapterData == null)
            {
                Logger.LogErrorFormat("ChapterData is null and chapterDataPath is {0}", chapterDataPath);
                return;
            }

            //章节列表是否存在
            if (_chapterData.chapterList == null || _chapterData.chapterList.Length <= 0)
            {
                Logger.LogErrorFormat("ChapterList is null or count is zero");
                return;
            }

            yijieChallengeNumRoot.CustomActive(false);      //虚空地下城挑战次数显示的root

            //如果是虚空地下城
            if (_modelType == DungeonModelTable.eType.VoidCrackModel)
            {
                var dungeonDailyBaseTimes = DungeonUtility.GetDungeonDailyBaseTimes(_chapterData.chapterList[0].dungeonID);
                if (dungeonDailyBaseTimes <= 0)
                {
                    yijieChallengeNumRoot.CustomActive(false);
                }
                else
                {
                    yijieChallengeNumRoot.CustomActive(true);
                    if (yijieChallengeNumInfo != null)
                    {
                        var leftTimes = GetLeftTimes();
                        yijieChallengeNumInfo.text = string.Format(TR.Value("resist_magic_challenge_times"),
                            leftTimes, dungeonDailyBaseTimes);
                        yijieChallengeNumInfoPre.text = "虚空裂缝每日挑战次数：";
                        yijieChallengeNumInfoTip.text = "虚空裂缝所有地下城共享挑战次数";
                    }
                }

                YiJieEffectObj = AssetLoader.instance.LoadResAsGameObject("Effects/UI/Prefab/EffUI_Yijie/Prefab/Eff_UI_YiJie");
                if (YiJieEffectObj != null)
                {
                    Utility.AttachTo(YiJieEffectObj, effectRoot);
                }
            }

            //如果是征战安图恩
            if (_modelType == DungeonModelTable.eType.ZhengzhanAntuenModel)
            {
                var dungeonDailyBaseTimes = DungeonUtility.GetDungeonDailyBaseTimes(_chapterData.chapterList[0].dungeonID);
                if (dungeonDailyBaseTimes <= 0)
                {
                    yijieChallengeNumRoot.CustomActive(false);
                }
                else
                {
                    yijieChallengeNumRoot.CustomActive(true);
                    if (yijieChallengeNumInfo != null)
                    {
                        var leftTimes = GetLeftTimes();
                        yijieChallengeNumInfo.text = string.Format(TR.Value("resist_magic_challenge_times"),
                            leftTimes, dungeonDailyBaseTimes);
                        yijieChallengeNumInfoPre.text = "安图恩每日挑战次数：";
                        yijieChallengeNumInfoTip.text = "安图恩所有地下城共享挑战次数";
                    }
                }
            }

            InitDefaultChapterId();

            _activityDungeonSubList = ChallengeUtility.GetDailyUnitActivitySubs(_defaultChapterId);

            InitMapModelView();

            DefaultOpenChallengeDungeonFrame();
        }

        //右上角的商店和传奇之路按钮
        private void InitTopRightContent()
        {
            CommonUtility.UpdateGameObjectVisible(topRightRoot, true);
            //远古地下城，展示传奇之路
            if (_modelType == DungeonModelTable.eType.AncientModel)
            {
                if (commonShopButtonControl != null)
                {
                    CommonUtility.UpdateGameObjectVisible(commonShopButtonControl.gameObject, false);
                }
                CommonUtility.UpdateButtonVisible(legendButton, true);
            }
            else
            {
                CommonUtility.UpdateButtonVisible(legendButton, false);
                if (commonShopButtonControl != null)
                {
                    CommonUtility.UpdateGameObjectVisible(commonShopButtonControl.gameObject, true);
                    if (_modelType == DungeonModelTable.eType.DeepModel)
                    {
                        //深渊地下城：展示深渊商店
                        commonShopButtonControl.SetShopId(9);
                    }
                    else if (_modelType == DungeonModelTable.eType.WeekHellModel)
                    {
                        //混沌地下城：展示混沌商店
                        commonShopButtonControl.SetShopId(28);
                    }
                    else if (_modelType == DungeonModelTable.eType.VoidCrackModel)
                    {
                        //虚空地下城：展示虚空商店
                        commonShopButtonControl.SetShopId(23);
                    }
                    else if (_modelType == DungeonModelTable.eType.YunShangChangAnModel)
                    {
                        //云上长安：展示云上长安商店
                        commonShopButtonControl.SetShopId(206);
                    }
                    else
                    {
                        CommonUtility.UpdateGameObjectVisible(commonShopButtonControl.gameObject, false);
                    }
                }
            }
        }


        private int GetLeftTimes()
        {
            var finishedTimes = DungeonUtility.GetDungeonDailyFinishedTimes(_chapterData.chapterList[0].dungeonID);
            var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(_chapterData.chapterList[0].dungeonID);
            var leftTimes = dailyMaxTime - finishedTimes;
            if (leftTimes < 0)
                leftTimes = 0;

            return leftTimes;
        }

        public void OnEnableView()
        {
            UpdateActivityDungeonItemList();
        }

        private void InitDefaultChapterId()
        {
            if (_defaultChapterId == 0)
            {
                SetDefaultChapterId();
            }
            else
            {

                var defaultDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_defaultChapterId);
                if (defaultDungeonTable == null)
                {
                    SetDefaultChapterId();
                    return;
                }

                //如果关卡是周常深渊的前置关卡，则定位到周常深渊的入口ID
                if (defaultDungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER)
                {
                    for (var i = 0; i < _chapterData.chapterList.Length; i++)
                    {
                        var chapterUnit = _chapterData.chapterList[i];
                        if (chapterUnit == null)
                            continue;

                        var curDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(chapterUnit.dungeonID);
                        if (curDungeonTable == null)
                            continue;

                        //是否为周常深渊的入口
                        if (curDungeonTable.SubType != DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
                            continue;

                        //是否存在前置任务
                        var taskTable = TableManager.GetInstance()
                            .GetTableItem<MissionTable>(curDungeonTable.PreTaskID);
                        if (taskTable == null)
                            continue;

                        //前置任务的地下城ID是否为当前的地下城ID
                        if (taskTable.MapID != _defaultChapterId)
                            continue;

                        //赋值为周常深渊的入口ID
                        _defaultChapterId = chapterUnit.dungeonID;
                        return;
                    }
                }
                else if (defaultDungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL)
                {
                    _defaultChapterId = defaultDungeonTable.ownerEntryId;
                }

                //找到具体的ChapterID
                bool isFind = false;
                for (var i = 0; i < _chapterData.chapterList.Length; i++)
                {
                    var chapterUnit = _chapterData.chapterList[i];
                    if (chapterUnit == null)
                        continue;
                    if (chapterUnit.dungeonID == _defaultChapterId)
                    {
                        isFind = true;
                        break;
                    }
                }
                if (isFind == false)
                    SetDefaultChapterId();
            }

        }

        private void SetDefaultChapterId()
        {
            if (_chapterData.chapterList[0] != null)
                _defaultChapterId = _chapterData.chapterList[0].dungeonID;
        }

        private void InitMapModelView()
        {
            //挑战地图
            if (backgroundImage != null)
            {
                backgroundImage.gameObject.CustomActive(true);
                ETCImageLoader.LoadSprite(ref backgroundImage, _chapterData.backgroundPath);
            }

            //关卡名字
            if (titleImage != null)
            {
                titleImage.gameObject.CustomActive(true);
                ETCImageLoader.LoadSprite(ref titleImage, _chapterData.namePath);
            }

            InitChallengeMapItem();

        }

        //加载具体的章节Item
        private void InitChallengeMapItem()
        {
            if (mapItemPrefab == null || mapItemRoot == null)
            {
                Logger.LogErrorFormat("ChapterItemPrefab is null or ChapterItemRoot is null");
                return;
            }

            for (var i = 0; i < _chapterData.chapterList.Length; i++)
            {
                var chapterUnit = _chapterData.chapterList[i];
                if (chapterUnit == null)
                    continue;

                var activityDungeonSub =
                    ChallengeUtility.GetDungeonSubDataByDungeonId(chapterUnit.dungeonID, _activityDungeonSubList);

                if (_modelType !=  DungeonModelTable.eType.VoidCrackModel && _modelType !=  DungeonModelTable.eType.ZhengzhanAntuenModel)
                {
                    //数据不存在，跳过
                    if (activityDungeonSub == null)
                    {
                        Logger.LogErrorFormat(
                            "ActivityDungeonSub is null and dungeonId is {0}",
                            chapterUnit.dungeonID);
                        continue;
                    }
                }
                
                //活动深渊，活动已经结束，或者活动不存在；活动时间已经过了
                if (DungeonUtility.IsLimitTimeHellDungeon(chapterUnit.dungeonID) == true)
                {
                    //活动状态
                    if (activityDungeonSub.state == eActivityDungeonState.End
                        || activityDungeonSub.state == eActivityDungeonState.None)
                    {
                        continue;
                    }

                    //结束时间已经过了,当前时间大于结束时间
                    if (TimeManager.GetInstance().GetServerTime() > activityDungeonSub.endtime)
                    {
                        continue;
                    }
                }

                //复制预制体
                var mapItemGo = Instantiate(mapItemPrefab) as GameObject;
                if (mapItemGo == null)
                    continue;
                mapItemGo.CustomActive(true);

                //添加到对应的根节点
                Utility.AttachTo(mapItemGo, mapItemRoot);

                var mapItem = mapItemGo.GetComponent<ChallengeMapItem>();
                //组件不存在，直接隐藏
                if (mapItem == null)
                {
                    Logger.LogErrorFormat("ChallengeChapterItem is null");
                    mapItemGo.CustomActive(false);
                    continue;
                }

                var isSelected = chapterUnit.dungeonID == _defaultChapterId;

                //初始化Item
                mapItem.InitItem(chapterUnit,
                    OnChallengeMapItemClick,
                    activityDungeonSub,
                    isSelected);

                //添加到List中
                _mapItemList.Add(mapItem);

                if (_modelType != DungeonModelTable.eType.VoidCrackModel)
                {
                    //添加参与滑动的章节ID
                    if (ChallengeUtility.IsChallengeChapterCanSlider(activityDungeonSub) == true)
                        AddSliderChapterId(activityDungeonSub.dungeonId);

                    ChallengeUtility.SortChapterIdListByLevel(_chapterIdList);
                }
                else
                {
                    AddSliderChapterId(chapterUnit.dungeonID);
                }
                    
            }
        }

        private void OnActivityDungeonUpdate(UIEvent uiEvent)
        {
            UpdateActivityDungeonItemList();
        }

        private void UpdateActivityDungeonItemList()
        {
            if (_mapItemList == null || _mapItemList.Count <= 0)
                return;

            for (var i = 0; i < _mapItemList.Count; i++)
            {
                var mapItem = _mapItemList[i];
                if (mapItem != null)
                    mapItem.UpdateDungeonItemContent();
            }
            UpdateSliderChapterIdList();
        }

        //更新可以参与滑动章节的ID
        private void UpdateSliderChapterIdList()
        {
            for (var i = 0; i < _activityDungeonSubList.Count; i++)
            {
                var activityDungeonSub = _activityDungeonSubList[i];
                if(activityDungeonSub == null)
                    continue;

                if (_modelType != DungeonModelTable.eType.VoidCrackModel)
                {
                    //如果可以滑动，则添加
                    if (ChallengeUtility.IsChallengeChapterCanSlider(activityDungeonSub) == true)
                        AddSliderChapterId(activityDungeonSub.dungeonId);
                }
            }
        }

        private void AddSliderChapterId(int dungeonId)
        {
            if (_chapterIdList == null)
                return;

            var isFind = false;
            for (var i = 0; i < _chapterIdList.Count; i++)
            {
                if (_chapterIdList[i] == dungeonId)
                {
                    isFind = true;
                    break;
                }
            }

            if (isFind == false)
                _chapterIdList.Add(dungeonId);
        }


        //ChapterFrame打开
        private void OnChallengeMapItemClick(int chapterId)
        {
            OnOpenChallengeDungeonFrame(chapterId, chapterId);
        }

        //打开ChapterFrame
        private void OnOpenChallengeDungeonFrame(int baseDungeonId, int selectDungeonId)
        {
            if (effectRoot != null)
            {
                effectRoot.CustomActive(false);
            }

            CommonUtility.UpdateGameObjectVisible(topRightRoot, false);
            
            //更新Item的状态
            OnUpdateChallengeMapItemList(baseDungeonId);
            UpdateContentEffectAction(false);

            //没用动画效果的话，直接打开
            if (comChapterSelectAnimate == null)
            {
                ChallengeUtility.OnOpenChallengeChapterFrame(_modelType,
                    baseDungeonId, 
                    selectDungeonId,
                    _chapterIdList);
                return;
            }

            //正在选中
            if (_eChapterSelectAnimateState == EChapterSelectAnimateState.OnSelectAnimate)
            {
                return;
            }

            if (_delayOpenFrameCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(_delayOpenFrameCoroutine);
                _delayOpenFrameCoroutine = null;
            }

            _delayOpenFrameCoroutine = GameFrameWork.instance.StartCoroutine(DelayOpenFrame(baseDungeonId, selectDungeonId));
        }

        private IEnumerator DelayOpenFrame(int baseDungeonId, int selectDungeonId)
        {
            //是否满足条件
            if (baseDungeonId > 0)
            {
                var chapterMapItem = GetChallengeMapItemByChapterId(baseDungeonId);
                if (chapterMapItem != null)
                {

                    while (_eChapterSelectAnimateState != EChapterSelectAnimateState.OnNone)
                    {
                        yield return null;
                    }

                    _eChapterSelectAnimateState = EChapterSelectAnimateState.OnSelectAnimate;

                    //直接播放动画
                    yield return comChapterSelectAnimate.NormalAnimate(chapterMapItem.transform.localPosition);

                    _eChapterSelectAnimateState = EChapterSelectAnimateState.OnNone;

                }
            }

            //播放完动画，打开界面
            ChallengeUtility.OnOpenChallengeChapterFrame(_modelType,
                baseDungeonId, 
                selectDungeonId,
                _chapterIdList);
            yield break;
        }

        //ChapterFrame关闭
        private void OnChallengeChapterFrameClose(UIEvent uiEvent)
        {
            //首先更新MapItem的状态
            UpdateActivityDungeonItemList();

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            if (_modelType != (DungeonModelTable.eType)uiEvent.Param1)
                return;

            if (comChapterSelectAnimate == null)
                return;

            if (_eChapterSelectAnimateState != EChapterSelectAnimateState.OnBackAnimate)
            {
                if (_chapterFrameCloseCoroutine != null)
                {
                    GameFrameWork.instance.StopCoroutine(_chapterFrameCloseCoroutine);
                    _chapterFrameCloseCoroutine = null;
                }
                _chapterFrameCloseCoroutine = GameFrameWork.instance.StartCoroutine(RevertMapAnimate());
            }

            if (_modelType == DungeonModelTable.eType.VoidCrackModel || _modelType == DungeonModelTable.eType.ZhengzhanAntuenModel)
            {
                if (effectRoot != null)
                {
                    effectRoot.CustomActive(true);
                }
            }

            CommonUtility.UpdateGameObjectVisible(topRightRoot, true);

        }

        private IEnumerator RevertMapAnimate()
        {
            while (_eChapterSelectAnimateState != EChapterSelectAnimateState.OnNone)
            {
                yield return null;
            }

            _eChapterSelectAnimateState = EChapterSelectAnimateState.OnBackAnimate;
            yield return comChapterSelectAnimate.RevertAnimate();
            _eChapterSelectAnimateState = EChapterSelectAnimateState.OnNone;
            UpdateContentEffectAction(true);
            yield break;
        }

        //ChapterFrame改变
        private void OnChallengeChapterBeginChange(UIEvent uiEvent)
        {

            if (uiEvent == null 
                || uiEvent.Param1 == null 
                                || uiEvent.Param2 == null
                                || comChapterSelectAnimate == null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChallengeChapterFinishChange);
                return;
            }

            if (_modelType != (DungeonModelTable.eType) uiEvent.Param1)
                return;

            var selectChapterId = (int)uiEvent.Param2;

            OnUpdateChallengeMapItemList(selectChapterId);

            if (_eChapterSelectAnimateState == EChapterSelectAnimateState.OnSelectAnimate)
                return;

            if (_chapterFrameChangeCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(_chapterFrameChangeCoroutine);
                _chapterFrameChangeCoroutine = null;
            }

            _chapterFrameChangeCoroutine = GameFrameWork.instance.StartCoroutine(DelayChapterFrameChange(selectChapterId));
        }

        private IEnumerator DelayChapterFrameChange(int chapterId)
        {
            //是否满足条件
            if (chapterId > 0)
            {
                var chapterMapItem = GetChallengeMapItemByChapterId(chapterId);
                if (chapterMapItem != null)
                {

                    while (_eChapterSelectAnimateState != EChapterSelectAnimateState.OnNone)
                    {
                        yield return null;
                    }

                    _eChapterSelectAnimateState = EChapterSelectAnimateState.OnSelectAnimate;

                    //直接播放动画
                    yield return comChapterSelectAnimate.NormalAnimate(chapterMapItem.transform.localPosition);

                    _eChapterSelectAnimateState = EChapterSelectAnimateState.OnNone;

                }
            }

            //播放完动画，打开界面
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChallengeChapterFinishChange);
            yield break;
        }

        private void OnUpdateChallengeMapItemList(int chapterId)
        {
            if (_mapItemList != null)
            {
                for (var i = 0; i < _mapItemList.Count; i++)
                {
                    var mapItem = _mapItemList[i];
                    if (mapItem != null)
                        mapItem.UpdateSelectedStateByDungeonId(chapterId);
                }
            }
        }

        private ChallengeMapItem GetChallengeMapItemByChapterId(int chapterId)
        {
            if (_mapItemList != null)
            {
                for (var i = 0; i < _mapItemList.Count; i++)
                {
                    var mapItem = _mapItemList[i];
                    if (mapItem != null && mapItem.GetChapterDungeonId() == chapterId)
                        return mapItem;
                }
            }

            return null;
        }

        #region DefaultOpenChallengeDungeonFrame
        //默认打开关卡详情页面
        private void DefaultOpenChallengeDungeonFrame()
        {
            if (_challengeParamDataModel == null)
                return;
            if (_challengeParamDataModel.DetailDungeonId <= 0)
                return;

            //var baseDungeonId = _challengeParamDataModel.BaseDungeonId;
            var baseDungeonId = _defaultChapterId;

            var activityDungeonSub =
                ChallengeUtility.GetDungeonSubDataByDungeonId(baseDungeonId, _activityDungeonSubList);

            //数据不存在，跳过
            if (activityDungeonSub == null)
                return;

            //等级不足，跳过
            if (PlayerBaseData.GetInstance().Level < activityDungeonSub.level)
            {
                return;
            }

            //周常入口
            if (DungeonUtility.IsWeekHellEntryDungeon(baseDungeonId) == true)
            {
                //前置活动关卡
                if (DungeonUtility.IsWeekHellPreDungeon(_challengeParamDataModel.DetailDungeonId) == true)
                {
                    WeekHellPreTaskState weekHellPreTaskState = DungeonUtility.GetWeekHellPreTaskState(baseDungeonId);
                    //前置关卡完成或者未领取, 没有处在正在进行的状态中
                    if (weekHellPreTaskState != WeekHellPreTaskState.IsProcessing)
                    {
                        return;
                    }
                }
                else
                {
                    ////周常深渊入口活动,每周次数为0， 每日次数为0
                    if (DungeonUtility.GetDungeonWeekLeftTimes(baseDungeonId) <= 0)
                        return;
                    if (DungeonUtility.GetDungeonDailyLeftTimes(baseDungeonId) <= 0)
                        return;
                }
            }

            //活动关卡结束，跳过(今天的挑战次数为0)
            if (DungeonUtility.IsLimitTimeHellDungeon(baseDungeonId) == true)
            {
                //次数为0；
                if (activityDungeonSub.isfinish == true)
                    return;

                //入口活动,次数为0
                if (DungeonUtility.GetDungeonDailyLeftTimes(baseDungeonId) <= 0)
                    return;

                //挑战时间结束
                if (TimeManager.GetInstance().GetServerTime() > activityDungeonSub.endtime)
                    return;
            }

            //打开关卡界面
            if (_challengeParamDataModel != null && _challengeParamDataModel.DetailDungeonId > 0)
            {
                OpenChallengeDungeonFrameByDetailDungeonId(_defaultChapterId,
                    _challengeParamDataModel.DetailDungeonId);
                _challengeParamDataModel.DetailDungeonId = 0;
            }
        }

        private void OpenChallengeDungeonFrameByDetailDungeonId(int baseDungeonId, int selectDungeonId)
        {
            //更新MapItem的状态
            OnUpdateChallengeMapItemList(baseDungeonId);

            //没用动画效果的话，直接打开
            if (comChapterSelectAnimate != null)
            {
                var chapterMapItem = GetChallengeMapItemByChapterId(baseDungeonId);
                if (chapterMapItem != null)
                    comChapterSelectAnimate.NormalAnimateWithAction(chapterMapItem.rectTransform());
            }

            UpdateContentEffectAction(false);
            ChallengeUtility.OnOpenChallengeChapterFrame(_modelType,
                baseDungeonId, 
                selectDungeonId, 
                _chapterIdList);
        }

        private void UpdateContentEffectAction(bool flag)
        {
            if (_onContentEffectAction != null)
                _onContentEffectAction(flag);
        }

        #endregion

        private void OnLegendButtonClicked()
        {
            //立刻关闭
            if(ClientSystemManager.GetInstance().IsFrameOpen<LegendFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<LegendFrame>(null, true);

            LegendFrame.CommandOpen();
        }

    }
}