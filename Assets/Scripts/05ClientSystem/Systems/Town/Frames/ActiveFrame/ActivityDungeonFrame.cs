using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
///////删除linq
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Collections;
using ProtoTable;
using Protocol;
using System.Globalization;
using Network;
using Scripts.UI;

namespace GameClient
{
    
    //ActivityDungeonStateType
    public enum eActivityDungeonState
    {
        /// <summary>
        /// 进行中
        /// </summary>
        Start,

        /// <summary>
        /// 进行中,但是等级不足
        /// </summary>
        LevelLimit,

        /// <summary>
        /// 准备中
        /// </summary>
        Prepare,

        /// <summary>
        /// 已经结束
        /// </summary>
        End,

        None,
    }

    #region Discard
    class ActivityDungeonFrameData
    {
        public int iLinkActiveID = 0;
    }
    #endregion

    public class ActivityDungeonFrame : ClientFrame
    {

        #region Data
        private class ActivityDungeonSubWithBind
        {
            public ActivityDungeonSub sub { get; set; }
            public ComCommonBind bind { get; set; }
        }

        private class ActivityDungeonTabWithBind
        {
            public ActivityDungeonTab tab { get; set; }
            public ComCommonBind bind { get; set; }
        }

        private const int kAllDailyDungeonId  = int.MaxValue;
        private static int mSelectedLastDungeonId = -1;

        private IClientFrame mLastOpenFrame = null;
        private string mLastSelectFrameName = "";
        private ActiveParams data = null;

        private ActivityDungeonTable.eActivityType mLastSwitchKindType = ActivityDungeonTable.eActivityType.None;
        private ActivityDungeonTable.eActivityType lastSwitchKindType
        {
            get
            {
                return mLastSwitchKindType;
            }
        }

        private ActivityDungeonTable.eActivityType mCurSwitchKindType = ActivityDungeonTable.eActivityType.None;
        private ActivityDungeonTable.eActivityType currentSwitchType
        {
            get
            {
                return mCurSwitchKindType;
            }

            set 
            {
                if (mCurSwitchKindType != value)
                {
                    mLastSwitchKindType = mCurSwitchKindType;
                    mCurSwitchKindType = value;
                }
            }
        }

        private List<ActivityDungeonSubWithBind> mCacheBind = new List<ActivityDungeonSubWithBind>();
        private List<ActivityDungeonTabWithBind> mCacheTabBind = new List<ActivityDungeonTabWithBind>();

        private int mGuideDungeonId = -1;
        private int GuideDungeonId
        {
            get 
            {
                return mGuideDungeonId;
            }
        }

        public const int pk3v3CrossDungeonID = 55;
        public const int guildDungeonID = 56;
        public const int chijiDungeonID = 57;
        public const int guildBattleID = 20;
        public const int guildCrossBattleID = 58;
        public const int pk2v2CrossDungeonID = 60;

        private const string dontNeedBgStrFlag = "每";

        #endregion

        #region OpenLinkFrame
        //OpenLinkFrame
        //Example: "2003000|2|2001000"; "702000" ; "702000|2|701000"
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                ActiveParams data = new ActiveParams();
                string[] strParams = strParam.Split('|');

                if (strParams.Length > 0)
                {
                    try
                    {
                        data.param0 = ulong.Parse(strParams[0]);
                    }
                    catch(Exception e)
                    {
                        Logger.LogError("ActivityDungeonFrame.OpenLinkFrame : strParams[0] ==>" + e.ToString());
                    }
                }

                if(strParams.Length > 1)
                {
                    try
                    {
                        data.type = (ActivityDungeonTable.eActivityType)ulong.Parse(strParams[1]);
                    }
                    catch(Exception e)
                    {
                        Logger.LogError("ActivityDungeonFrame.OpenLinkFrame : strParams[1] ==>" + e.ToString());
                    }
                }

                if(strParams.Length > 2)
                {
                    try
                    {
                        data.dungeonId = int.Parse(strParams[2]);
                    }
                    catch(Exception e)
                    {
                        Logger.LogError("ActivityDungeonFrame.OpenLinkFrame : strParams[2] ==>" + e.ToString());
                    }
                }

                GameClient.ClientSystemManager.GetInstance().CloseFrame<ActivityDungeonFrame>();
                GameClient.ClientSystemManager.GetInstance().OpenFrame<ActivityDungeonFrame>(FrameLayer.Middle, data);
            }
            catch(Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        
        #endregion

        #region ExtraUIBind
        private Text mName = null;
        private GameObject mTabroot = null;
        private ToggleGroup mTabrootgroup = null;
        private ToggleGroup mGridToggleGroup = null;
        private GameObject mGirldRoot = null;
        private GameObject mDailyRoot = null;
        private GameObject mOtherRoot = null;
        private GameObject mOtherRootContent = null;
        private GameObject mRewardRootContent = null;
        private Toggle mDailyKindToggle = null;
        private Toggle mTimeLimitKindToggle = null;
        private Toggle mRewardKindToggle = null;
        private Toggle mAllDailyTab = null;
        private ToggleGroup mTimeLimitToggleGroup = null;
        private GameObject mAllDialyTabRedPoint = null;
        private GameObject mDailyKindRedPoint = null;
        private GameObject mTimeLimitKindRedPoint = null;
        private GameObject mRewardKindRedPoint = null;

        protected override void _bindExUI()
        {
            mName = mBind.GetCom<Text>("name");
            mTabroot = mBind.GetGameObject("tabroot");
            mTabrootgroup = mBind.GetCom<ToggleGroup>("tabrootgroup");
            mGridToggleGroup = mBind.GetCom<ToggleGroup>("gridToggleGroup");
            mGirldRoot = mBind.GetGameObject("girldRoot");
            mDailyRoot = mBind.GetGameObject("dailyRoot");
            mOtherRoot = mBind.GetGameObject("otherRoot");
            mOtherRootContent = mBind.GetGameObject("otherRootContent");
            mRewardRootContent = mBind.GetGameObject("rewardRootContent");
            mDailyKindToggle = mBind.GetCom<Toggle>("dailyKindToggle");
            mDailyKindToggle.onValueChanged.AddListener(_onDailyKindToggleToggleValueChange);
            mTimeLimitKindToggle = mBind.GetCom<Toggle>("timeLimitKindToggle");
            mTimeLimitKindToggle.onValueChanged.AddListener(_onTimeLimitKindToggleToggleValueChange);
            mRewardKindToggle = mBind.GetCom<Toggle>("rewardKindToggle");
            mRewardKindToggle.onValueChanged.AddListener(_onRewardKindToggleToggleValueChange);
            mAllDailyTab = mBind.GetCom<Toggle>("allDailyTab");
            mAllDailyTab.onValueChanged.AddListener(_onAllDailyTabToggleValueChange);
            mTimeLimitToggleGroup = mBind.GetCom<ToggleGroup>("timeLimitToggleGroup");
            mAllDialyTabRedPoint = mBind.GetGameObject("allDialyTabRedPoint");
            mDailyKindRedPoint = mBind.GetGameObject("dailyKindRedPoint");
            mTimeLimitKindRedPoint = mBind.GetGameObject("timeLimitKindRedPoint");
            mRewardKindRedPoint = mBind.GetGameObject("rewardKindRedPoint");
        }

        protected override void _unbindExUI()
        {
            mName = null;
            mTabroot = null;
            mTabrootgroup = null;
            mGridToggleGroup = null;
            mGirldRoot = null;
            mDailyRoot = null;
            mOtherRoot = null;
            mOtherRootContent = null;
            mRewardRootContent = null;
            mDailyKindToggle.onValueChanged.RemoveListener(_onDailyKindToggleToggleValueChange);
            mDailyKindToggle = null;
            mTimeLimitKindToggle.onValueChanged.RemoveListener(_onTimeLimitKindToggleToggleValueChange);
            mTimeLimitKindToggle = null;
            mRewardKindToggle.onValueChanged.RemoveListener(_onRewardKindToggleToggleValueChange);
            mRewardKindToggle = null;
            mAllDailyTab.onValueChanged.RemoveListener(_onAllDailyTabToggleValueChange);
            mAllDailyTab = null;
            mTimeLimitToggleGroup = null;
            mAllDialyTabRedPoint = null;
            mDailyKindRedPoint = null;
            mTimeLimitKindRedPoint = null;
            mRewardKindRedPoint = null;
        }
#endregion   

        #region Callback
        private void _onHelpButtonClick()
        {
            /* put your code in here */

        }

        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _onClose();
        }

        private void _onDailyKindToggleToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                _switch2KindTab(ActivityDungeonTable.eActivityType.Daily);
            }
        }

        private void _onTimeLimitKindToggleToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                _switch2KindTab(ActivityDungeonTable.eActivityType.TimeLimit);
            }
        }

        private void _onRewardKindToggleToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed)
            {
                _switch2KindTab(ActivityDungeonTable.eActivityType.Reward);
            }
        }

        private void _onAllDailyTabToggleValueChange(bool changed)
        {
            /* put your code in here */

            if (changed)
            {
                _switch2DailyTab(kAllDailyDungeonId);
            }
        }
        #endregion

        #region ClientFrame
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/Dungeon/ActivityDungeon";
        }
        
        protected override void _OnOpenFrame()
        {
            _clearBeforeOpenAndClose();
            _tryGuideActivity();
            _updateAllKindRedState();

            //是否打开其他页面
            _OnOpenChapterFrame();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityDungeonUpdate, _updateUnitState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDeadTowerWipeoutTimeChange, _updateUnitState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityDungeonStateUpdate, _updateStateUpdate);
        }

		protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityDungeonUpdate, _updateUnitState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDeadTowerWipeoutTimeChange, _updateUnitState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityDungeonStateUpdate, _updateStateUpdate);

            _clearAllKindTabContent();
            _clearBeforeOpenAndClose();
        }
        #endregion

        #region InitEvent
        private void _clearBeforeOpenAndClose()
        {
            mCacheBind.Clear();
            mCacheTabBind.Clear();

            mLastSelectFrameName = string.Empty;
            mLastSwitchKindType = ActivityDungeonTable.eActivityType.None;
            mCurSwitchKindType = ActivityDungeonTable.eActivityType.None;
        }

        private void _updateUnitState(UIEvent ui)
        {
            for (int i = 0; i < mCacheBind.Count; ++i)
            {
                _updateRedState(mCacheBind[i].sub, mCacheBind[i].bind);
                _updateTimeLimitUnitState(mCacheBind[i].sub, mCacheBind[i].bind);
            }

            if (currentSwitchType == ActivityDungeonTable.eActivityType.Daily)
            {
                for (int i = 0; i < mCacheTabBind.Count; ++i)
                {
                    _updateOneDailyTabRedPoint(mCacheTabBind[i].tab, mCacheTabBind[i].bind);
                }
            }

            _updateAllKindRedState();          
        }

        private void _updateStateUpdate(UIEvent ui)
        {
            if (mSelectedLastDungeonId == -1)
            {
                _switch2DailyTab(kAllDailyDungeonId);
            }
            else
            {
                _switch2DailyTab(mSelectedLastDungeonId);
            }
        }

        private void _updateRedState(ActivityDungeonSub sub, ComCommonBind bind)
        {
            switch (currentSwitchType)
            {
                case ActivityDungeonTable.eActivityType.Daily:
                    {
                        _updateDailyUnitRedPoint(sub, bind);
                        _updateDailyUnitState(sub, bind);
                    }
                    break;
                case ActivityDungeonTable.eActivityType.TimeLimit:
                    {
                        _updateTimeLimitUnitRedPoint(sub, bind);
                    }
                    break;
            }
        }

        private void _updateAllKindRedState()
        {
            _updateKindRedState(ActivityDungeonTable.eActivityType.Daily);
            _updateKindRedState(ActivityDungeonTable.eActivityType.Reward);
            _updateKindRedState(ActivityDungeonTable.eActivityType.TimeLimit);
        }

        private void _updateKindRedState(ActivityDungeonTable.eActivityType type)
        {
            switch (type)
            {
                case ActivityDungeonTable.eActivityType.Daily:
                    bool isShow = ActivityDungeonDataManager.GetInstance().IsShowRedByActivityType(type);
                    mDailyKindRedPoint.SetActive(isShow);
                    mAllDialyTabRedPoint.SetActive(isShow);
                    break;
                case ActivityDungeonTable.eActivityType.Reward:
                    mRewardKindRedPoint.SetActive(MissionDailyFrame.CheckRedPoint());
                    break;
                case ActivityDungeonTable.eActivityType.TimeLimit:
                    mTimeLimitKindRedPoint.SetActive(ActivityDungeonDataManager.GetInstance().IsShowRedByActivityType(type));

                    ScoreWarStatus scoreState = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
                    if (scoreState == ScoreWarStatus.SWS_PREPARE || scoreState == ScoreWarStatus.SWS_BATTLE)
                    {
                        mTimeLimitKindRedPoint.SetActive(true);
                    }

                    break;
            }
        }

        #endregion

        #region GuideRelation
        private void _tryGuideActivity()
        {
            _updateGuideDungeonId();

            _doGuideByActivityType();

            _resetGuideDungeonId();
        }

        private void _doGuideByActivityType()
        {
            ActivityDungeonTable.eActivityType type = _getGuideType();

            switch (type)
            {
                case ActivityDungeonTable.eActivityType.None:
                    Logger.LogErrorFormat("[活动副本] 错误的引导类型类型");
                    break;
                case ActivityDungeonTable.eActivityType.Daily:
                    mDailyKindToggle.isOn = true;
                    break;
                case ActivityDungeonTable.eActivityType.TimeLimit:
                    mTimeLimitKindToggle.isOn = true;
                    break;
                case ActivityDungeonTable.eActivityType.Reward:
                    mRewardKindToggle.isOn = true;
                    break;
            }
        }

        private ActivityDungeonTable.eActivityType _getGuideType()
        {
            if (null != data && data.type == ActivityDungeonTable.eActivityType.Reward)
            {
                return ActivityDungeonTable.eActivityType.Reward;
            }

            ActivityDungeonSub sub = ActivityDungeonDataManager.GetInstance().GetSubByDungeonID(GuideDungeonId);
            if (null != sub)
            {
                return sub.table.ActivityType;
            }

            return ActivityDungeonTable.eActivityType.Reward;
        }

        private void _updateGuideDungeonId()
        {
            /// 类型，以及地下城ID
            data = userData as ActiveParams;

            _resetGuideDungeonId();

            if (null == data)
            {
                return ;
            }

            int dungeonId = (int)data.param0;

            ActivityDungeonSub sub = ActivityDungeonDataManager.GetInstance().GetSubByDungeonID(dungeonId);

            if (null != sub)
            {
                mGuideDungeonId = sub.dungeonId;
            }

            Logger.LogProcessFormat("[活动副本] 限时活动 {0} type:{1}, ID{2}", GuideDungeonId, data.type, data.param0);

            // TODO figure it out, why the code mea
            // `我要变强`问题
            //if (_hasNoGuideActivity(pm))
            //{
            //    guideActivityId = 0;  
            //}
        }

        private void _resetGuideDungeonId()
        {
            mGuideDungeonId = mSelectedLastDungeonId;
        }

        #endregion

        #region OpenChapterFrame
        //打开Chapter相关页面
        //由userData.dungeonId决定是否打开其他页面
        private void _OnOpenChapterFrame()
        {
            if (userData != null)
            {
                data = userData as ActiveParams;
                if (data != null && data.dungeonId > 0)
                {
                    _bindAllButton(data.dungeonId);
                }
            }
        }
        #endregion

        #region MainTabsSwitch
        //主Tabs位于界面的左边，包含三个Tabs
        private void _switch2KindTab(ActivityDungeonTable.eActivityType type)
        {
            if (!_isSwitchKindTab(type))
            {
                Logger.LogProcessFormat("[活动副本] 已经选择 {0}, {1}", type, GuideDungeonId);
                return ;
            }

            _clearAllKindTabContent();
            _switch2KindContent();
            _loadSwitchedKindContent(GuideDungeonId);
        }

        private bool _isSwitchKindTab(ActivityDungeonTable.eActivityType type)
        {
            if (currentSwitchType != type)
            {
                currentSwitchType = type;
                return true;
            }

            return false;
        }

        private void _clearAllKindTabContent()
        {
            string tabnameunit = mBind.GetPrefabPath("tab");
            mBind.ClearCacheBinds(tabnameunit);

            string timelimitunit = mBind.GetPrefabPath("timelimitunit");
            mBind.ClearCacheBinds(timelimitunit);

            string dailyunit = mBind.GetPrefabPath("dailyunit");
            mBind.ClearCacheBinds(dailyunit);

            string rottenetterDailyUnit = mBind.GetPrefabPath("rottenetterDailyUnit");
            mBind.ClearCacheBinds(rottenetterDailyUnit);

            mLastSelectFrameName = string.Empty;

            _clearRewradFrame();
            _clearInfoFrame();
        }

        private void _clearRewradFrame()
        {
            if (null != mLastOpenFrame)
            {
                ClientSystemManager.instance.CloseFrame(mLastOpenFrame, true);
            }
            mLastOpenFrame = null;
        }

        private void _clearInfoFrame()
        {
            ClientSystemManager.instance.CloseFrame<ActivityDungeonInfoFrame>();
        }

        private void _switch2KindContent()
        {
            mOtherRoot.SetActive(false);
            mDailyRoot.SetActive(false);
            switch (currentSwitchType)
            {
                case ActivityDungeonTable.eActivityType.Daily:
                    mDailyRoot.SetActive(true);
                    break;
                case ActivityDungeonTable.eActivityType.TimeLimit:
                case ActivityDungeonTable.eActivityType.Reward:
                    mOtherRoot.SetActive(true);
                    break;
            }
        }

        private void _loadSwitchedKindContent(int dungeonId)
        {
            Logger.LogProcessFormat("[活动副本] 加载选择内容 {0}, {1}", currentSwitchType, dungeonId);

            switch (currentSwitchType)
            {
                case ActivityDungeonTable.eActivityType.Daily:
                    _initDailyTabs(dungeonId);
                    //_bindAllButton(dungeonId);
                    break;
                case ActivityDungeonTable.eActivityType.TimeLimit:
                    _initTimeLimitUnit(dungeonId);
                    break;
                case ActivityDungeonTable.eActivityType.Reward:
                    _loadDailyRewardFrame();
                    break;
            }
        }
        #endregion

        #region DailyTabInfo
        //日常Tab相关内容
        private void _initDailyTabs(int dungeonId)
        {
            if (null == mBind)
            {
                return;
            }

            List<ActivityDungeonTab> tabs    = ActivityDungeonDataManager.GetInstance().GetTabByActivityType(ActivityDungeonTable.eActivityType.Daily);
            //ActivityDungeonTab selectedTab = ActivityDungeonDataManager.GetInstance().GetTabByDungeonID(ActivityDungeonTable.eActivityType.Daily, dungeonId);
            var selectedTab = ActivityDungeonDataManager.GetInstance()
                .GetDailyDungeonTab();

            string tabnameunit = mBind.GetPrefabPath("tab");

            mBind.ClearCacheBinds(tabnameunit);

            mCacheTabBind.Clear();
            
            Logger.LogProcessFormat("[ActivityDungeon] 开始初始化{0}个标签 id{1}", tabs.Count, dungeonId);

            for (int i = 0; i < tabs.Count; ++i)
            {
                _loadOneDailyTab(tabs[i], tabnameunit, i, selectedTab == tabs[i]);
            }

            if (null == selectedTab)
            {
                mAllDailyTab.isOn = false;
                mAllDailyTab.isOn = true;
            }
            else
            {
                dungeonId = selectedTab.subs[0].dungeonId;
                _switch2DailyTab(dungeonId);
            }
        }

        private void _loadOneDailyTab(ActivityDungeonTab tab, string tabnameunit, int index, bool isselected)
        {
            Logger.LogProcessFormat("[ActivityDungeon] 初始化标签 {0} ", tab.name);

            ComCommonBind bind = mBind.LoadExtraBind(tabnameunit);

            if (null != bind)
            {
                Utility.AttachTo(bind.gameObject, mTabroot);

                bind.gameObject.name = string.Format("Activity{0}", index);

                Text   name   = bind.GetCom<Text>("tabname");
                Toggle toggle = bind.GetCom<Toggle>("toggle");

                Button help         = bind.GetCom<Button>("help");
                GameObject helpRoot = bind.GetGameObject("helpRoot");
                helpRoot.SetActive(false);

                ActivityDungeonTabWithBind tabbind = new ActivityDungeonTabWithBind();
                tabbind.tab = tab;
                tabbind.bind = bind;
                mCacheTabBind.Add(tabbind);

                _updateOneDailyTabRedPoint(tab, bind);

                name.text     = tab.name;

                toggle.group  = mTabrootgroup;
                toggle.isOn   = isselected;

                int dungeonId = tab.subs[0].dungeonId;

                toggle.onValueChanged.AddListener((isOn)=> 
                {
                    if (isOn)
                    {
                        _switch2DailyTab(dungeonId);
                    }
                });
            }
        }

        private void _updateOneDailyTabRedPoint(ActivityDungeonTab tab, ComCommonBind bind)
        {
            GameObject redpoint = bind.GetGameObject("redpoint");
            if(redpoint != null)
            {
                redpoint.SetActive(ActivityDungeonDataManager.GetInstance().IsTabShowRed(tab));
            }
        }

        private void _switch2DailyTab(int dungeonId)
        {
            if (null == mBind)
            {
                return;
            }

            mSelectedLastDungeonId = dungeonId;
            
            Logger.LogProcessFormat("[ActivityDungeon] 切换日常Tab ID: {0}", dungeonId);

            List<ActivityDungeonSub> subs = _getDailyUnitActivitySubs(dungeonId);
            string tabname = _getDailyUnitActivityTabName(dungeonId);

            mCacheBind.Clear();

            //if (tabname == mLastSelectFrameName )
            //{
            //    Logger.LogProcessFormat("[ActivityDungeon] 已经选择该标签 {0} ", tabname);
            //    return ;
            //}

            if (subs.Count <= 0)
            {
                Logger.LogErrorFormat("[ActivityDungeon] 单位列表为空 {0} ", dungeonId);
                return ;
            }

            mLastSelectFrameName = tabname;

            Logger.LogProcessFormat("[ActivityDungeon] 选择标签 {0}, ID {1}", tabname, dungeonId);

            string dailyunit     = mBind.GetPrefabPath("dailyunit");
            string rottenetterDailyUnit = mBind.GetPrefabPath("rottenetterDailyUnit");

            mBind.ClearCacheBinds(dailyunit);
            mBind.ClearCacheBinds(rottenetterDailyUnit);

            for (int i = 0; i < subs.Count; ++i)
            {
                //如果是活动堕落深渊放在第一个显示
                if (ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(subs[i].dungeonTable.ID))
                {
                    //活动状态检查
                    if (subs[i].state != eActivityDungeonState.End && subs[i].state != eActivityDungeonState.None)
                    {
                        _loadDailyUnitAndInit(subs[i], rottenetterDailyUnit);
                    }
                    break;
                }
            }

            for (int i = 0; i < subs.Count; i++)
            {
                //不是活动堕落深渊
                if (!ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(subs[i].dungeonTable.ID))
                {
                    _loadDailyUnitAndInit(subs[i], dailyunit);
                }
            }
        }

        private List<ActivityDungeonSub> _getDailyUnitActivitySubs(int dungeonId)
        {
            if (kAllDailyDungeonId == dungeonId)
            {
                return ActivityDungeonDataManager.GetInstance().GetSubByActivityType(ActivityDungeonTable.eActivityType.Daily);
            }

            ActivityDungeonTab tab = ActivityDungeonDataManager.GetInstance().GetTabByDungeonIDDefaultFirst(ActivityDungeonTable.eActivityType.Daily, dungeonId);

            if (null != tab)
            {
                return tab.subs;
            }

            return new List<ActivityDungeonSub>();
        }

        private string _getDailyUnitActivityTabName(int dungeonId)
        {
            if (kAllDailyDungeonId == dungeonId)
            {
                return "全部";
            }

            ActivityDungeonTab tab = ActivityDungeonDataManager.GetInstance().GetTabByDungeonIDDefaultFirst(ActivityDungeonTable.eActivityType.Daily, dungeonId);

            if (null != tab)
            {
                return tab.name;
            }

            return string.Empty;
        }
        
        private void _loadDailyUnitAndInit(ActivityDungeonSub sub, string dailyunit)
        {
            ComCommonBind bind = mBind.LoadExtraBind(dailyunit);

            if (!_attach2GridRoot(sub, bind))
            {
                return ;
            }

            _add2CacheBind(sub, bind);

            _bindDailyGoAndSelectEvent(sub, bind);

            _updateDailyUnitBaseInfo(sub, bind);
            
            _updateDailyUnitState(sub, bind);

            _updateDailyUnitRedPoint(sub, bind);

            //如果是活动堕落深渊
            if (ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(sub.dungeonTable.ID))
            {
                _updateRotteneterHellTimes(sub, bind);
            }
        }

        private bool _attach2GridRoot(ActivityDungeonSub sub, ComCommonBind bind)
        {
            if (null != bind)
            {
                bind.gameObject.name = sub.id.ToString();
                Utility.AttachTo(bind.gameObject, mGirldRoot);
                return true;
            }

            return false;
        }

        private void _add2CacheBind(ActivityDungeonSub sub, ComCommonBind bind)
        {
            ActivityDungeonSubWithBind cacheBind = new ActivityDungeonSubWithBind();
            cacheBind.sub = sub;
            cacheBind.bind = bind;
            mCacheBind.Add(cacheBind);
        }
        
        private void _bindDailyGoAndSelectEvent(ActivityDungeonSub sub, ComCommonBind bind)
        {
            Button goButton = bind.GetCom<Button>("goButton");

            int dungeonId = sub.dungeonId;

            goButton.onClick.AddListener(()=>
            {
                if(ActivityDungeonDataManager.IsUltimateChallengeActivity(sub.table))
                {
                    if (!ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_ZJSL_TOWER))
                    {
                        SystemNotifyManager.SystemNotify(4500005);
                        return;
                    }

                    ClientSystemManager.GetInstance().CloseFrame<UltimateChallengeFrame>();
                    ClientSystemManager.GetInstance().OpenFrame<UltimateChallengeFrame>();
                    return;
                }
                _bindAllButton(dungeonId);
            });

            Toggle toggle = bind.GetCom<Toggle>("toggle");
            toggle.group  = mGridToggleGroup;
            toggle.isOn = dungeonId == GuideDungeonId;
            toggle.onValueChanged.AddListener((isOn)=>
            {
                if (isOn)
                {
                    _openActivityDungeonInfoFrame(dungeonId);
                }
            });
        }

        private void _updateDailyUnitBaseInfo(ActivityDungeonSub sub, ComCommonBind bind)
        {
            _updateDailyUnitBaseTextInfo(sub, bind);
            _updateDailyUnitBaseSpritInfo(sub, bind);
            _updateDailyUnitBaseCounterInfo(sub, bind);
            _updateDailyUnitBaseShowHiddenInfo(sub, bind);
        }

        private void _updateDailyUnitBaseTextInfo(ActivityDungeonSub sub, ComCommonBind bind)
        {
            Text collectDesc = bind.GetCom<Text>("collectDesc");
            Text reclevel = bind.GetCom<Text>("reclevel");
            Text unlockLevel = bind.GetCom<Text>("unlockLevel");
            Text countDesc = bind.GetCom<Text>("countDesc");
            Text modeDesc = bind.GetCom<Text>("modeDesc");

            unlockLevel.text = sub.level.ToString();
            collectDesc.text = sub.table.ExtraDescription;
            modeDesc.text = sub.table.Mode;
            countDesc.text = sub.table.ShowCountDesc;
            reclevel.text = sub.GetDungeonRecommendLevel();
        }

        private void _updateDailyUnitBaseSpritInfo(ActivityDungeonSub sub, ComCommonBind bind)
        {
            Image boardIcon = bind.GetCom<Image>("boardIcon");
            Image typeIcon = bind.GetCom<Image>("typeIcon");
            Image unitIcon = bind.GetCom<Image>("unitIcon");

            //unitIcon.sprite  = AssetLoader.instance.LoadRes(sub.table.SingleTabIcon, typeof(Sprite)).obj as Sprite;
            //typeIcon.sprite  = AssetLoader.instance.LoadRes(sub.table.ModeIconPath,  typeof(Sprite)).obj as Sprite;
            //boardIcon.sprite = AssetLoader.instance.LoadRes(sub.table.ModeBoardPath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref unitIcon, sub.table.SingleTabIcon);
            ETCImageLoader.LoadSprite(ref typeIcon, sub.table.ModeIconPath);
            ETCImageLoader.LoadSprite(ref boardIcon, sub.table.ModeBoardPath);
        }

        private void _updateDailyUnitBaseCounterInfo(ActivityDungeonSub sub, ComCommonBind bind)
        {
            ComCommonConsume countCosume = bind.GetCom<ComCommonConsume>("countCosume");

            ComCommonConsume.eCountType type = _getDailyUnitCountType(sub.dungeonId);

            _updateDailyUnitBaseCounterSumInfo(type, bind);

            countCosume.SetData(ComCommonConsume.eType.Count, type, sub.dungeonId);
        }

        private void _updateDailyUnitBaseCounterSumInfo(ComCommonConsume.eCountType type, ComCommonBind bind)
        {
            GameObject counterSplitRoot = bind.GetGameObject("counterSplitRoot");
            GameObject counterSumRoot = bind.GetGameObject("counterSumRoot");

            counterSplitRoot.SetActive(true);
            counterSumRoot.SetActive(true);

            if (ComCommonConsume.eCountType.DeadTower == type)
            {
                counterSplitRoot.SetActive(false);
                counterSumRoot.SetActive(false);
            }
        }

        private ComCommonConsume.eCountType _getDailyUnitCountType(int dungeonID)
        {
            DungeonTable tb = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);

            if (null != tb)
            {
                switch (tb.SubType)
                {
                    case DungeonTable.eSubType.S_NANBUXIGU:
                        return ComCommonConsume.eCountType.NorthCount;
                    case DungeonTable.eSubType.S_NIUTOUGUAI:
                        return ComCommonConsume.eCountType.MouCount;
                    case DungeonTable.eSubType.S_SIWANGZHITA:
                        return ComCommonConsume.eCountType.DeadTower;
                    case DungeonTable.eSubType.S_FINALTEST_PVE:
                        return ComCommonConsume.eCountType.Final_Test;
                }
            }

            return ComCommonConsume.eCountType.MouCount;
        }

        private void _updateDailyUnitBaseShowHiddenInfo(ActivityDungeonSub sub, ComCommonBind bind)
        {
            GameObject battleTypeRoot = bind.GetGameObject("battleTypeRoot");
            GameObject countRoot = bind.GetGameObject("countRoot");

            battleTypeRoot.SetActive(sub.table.ShowModeFlag);
            countRoot.SetActive(sub.table.ShowCount);
        }

        private void _updateDailyUnitState(ActivityDungeonSub sub, ComCommonBind bind)
        {
            GameObject buttonRoot = bind.GetGameObject("buttonRoot");
            GameObject recLevelRoot = bind.GetGameObject("recLevelRoot");
            GameObject finishRoot = bind.GetGameObject("finishRoot");
            GameObject lockRoot = bind.GetGameObject("lockRoot");
            UIGray buttonGray = bind.GetCom<UIGray>("goButtonUIGray");
            GameObject countRoot = bind.GetGameObject("countParentRoot");

            buttonRoot.SetActive(false);
            recLevelRoot.SetActive(false);
            finishRoot.SetActive(false);
            lockRoot.SetActive(false);
            if (buttonGray)
            {
                buttonGray.enabled = false;
            }
            if (countRoot)
            {
                countRoot.CustomActive(false);
            }

            switch (sub.state)
            {
                case eActivityDungeonState.Prepare:
                    Logger.LogErrorFormat("[活动副本] 日常不可能在准备状态 {0}, {1}", sub.id, sub.name);
                    break;
                case eActivityDungeonState.Start:

                    recLevelRoot.SetActive(true);

                    if (ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(sub.dungeonTable.ID))
                    {
                        buttonRoot.SetActive(true);

                        if (countRoot)
                        {
                            countRoot.CustomActive(true);
                        }

                        if (sub.table.ShowCount && sub.isfinish)
                        {
                            if (buttonGray)
                                buttonGray.enabled = true;
                        }
                        else
                        {
                            if (buttonGray)
                                buttonGray.enabled = false;
                        }
                    }
                    else
                    {
                        if (sub.table.ShowCount && sub.isfinish)
                        {
                            finishRoot.SetActive(true);
                        }
                        else
                        {
                            buttonRoot.SetActive(true);
                        }
                    }
                   
                    break;
                case eActivityDungeonState.None:
                case eActivityDungeonState.LevelLimit:

                    lockRoot.SetActive(true);

                    if (ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(sub.dungeonTable.ID))
                    {
                        buttonRoot.SetActive(true);

                        if (buttonGray)
                            buttonGray.enabled = true;

                        if (countRoot)
                        {
                            countRoot.CustomActive(false);
                        }
                    }

                    break;
                case eActivityDungeonState.End:
                    break;
            }
        }

        private void _updateDailyUnitRedPoint(ActivityDungeonSub sub, ComCommonBind bind)
        {
            GameObject redpoint = bind.GetGameObject("redpoint");

            redpoint.SetActive(sub.isshowred);
        }

        /// <summary>
        /// 活动堕落深渊时间显示
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="bind"></param>
        private void _updateRotteneterHellTimes(ActivityDungeonSub sub, ComCommonBind bind)
        {
            SimpleTimer mSimpleTimer = bind.GetCom<SimpleTimer>("simpleTimer");
            if (mSimpleTimer)
            {
                mSimpleTimer.SetCountdown((int)(sub.endtime - TimeManager.GetInstance().GetServerTime()));
                mSimpleTimer.StartTimer();
            }
        }
        
        private void _loadDailyRewardFrame()
        {
            mLastOpenFrame = MissionDailyFrame.Open((isOn) => { _updateAllKindRedState(); }, mRewardRootContent);
        }
        #endregion

        #region TimeLimitTabInfo
        private void _initTimeLimitUnit(int dungeonId)
        {
            string timelimitunit = mBind.GetPrefabPath("timelimitunit");

            List<ActivityDungeonTab> tabs = ActivityDungeonDataManager.GetInstance().GetTabByActivityType(ActivityDungeonTable.eActivityType.TimeLimit);

            mCacheBind.Clear();

            Logger.LogProcessFormat("[活动副本] 开始 限时活动加载 {0}, {1}个Tab", dungeonId, tabs.Count);

            for (int i = 0; i < tabs.Count; ++i)
            {
                for (int j = 0; j < tabs[i].subs.Count; ++j)
                {
                    _loadTimeLimitUnit(tabs[i].subs[j], timelimitunit);
                }
            }
        }

        private void _loadTimeLimitUnit(ActivityDungeonSub sub, string timelimitunit)
        {
            ComCommonBind bind = mBind.LoadExtraBind(timelimitunit);
            if (null != bind)
            {
                Logger.LogProcessFormat("[活动副本] 限时活动加载 {0}", sub.dungeonId);
#if APPLE_STORE
                //add for ios apptore
                if (TryHideBountyLeagueForIOS(sub))
                {
                    return;
                }
#endif
                Utility.AttachTo(bind.gameObject, mOtherRootContent);

                Image icon = bind.GetCom<Image>("icon");
                // icon.sprite = AssetLoader.instance.LoadRes(sub.table.ImagePath, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref icon, sub.table.ImagePath);

                _add2CacheBind(sub, bind);

                _bindTimeLimitUnitEvent(sub, bind);
                _updateTimeLimitUnitBaseInfo(sub, bind);
                _updateTimeLimitUnitState(sub, bind);
                _updateTimeLimitUnitRedPoint(sub, bind);
            }
        }

        private void _bindTimeLimitUnitEvent(ActivityDungeonSub sub, ComCommonBind bind)
        {
            Button go = bind.GetCom<Button>("go");
            int dungeonId = sub.dungeonId;
            go.onClick.AddListener(()=>
            {
                if (null != sub && null != sub.table && !string.IsNullOrEmpty(sub.table.GoLinkInfo))
                {
                    //如果是攻城怪物类型
                    if (ActivityDungeonDataManager.GetInstance().IsActivityDungeonBeAttackCityMonster(sub.table) ==
                        true)
                    {
                        ActivityDungeonDataManager.GetInstance().ActivityDungeonFindAttackCityMonster();
                    }
                    else if (ActivityDungeonDataManager.GetInstance().IsActivityDungeonBeHonorBattleField(sub.table) == true)
                    {
                        if (TeamDataManager.GetInstance().HasTeam())
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Chiji_has_team"));
                            return;
                        }
                        ClientSystemManager.GetInstance().OpenFrame<ChijiEntranceFrame>(FrameLayer.Middle);
                    }else if(ActivityDungeonDataManager.GetInstance().IsActivityDungeonFairDuelField(sub.table))
                    {
                        ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                        if (systemTown == null) return;

                        PkWaitingRoomData roomData = new PkWaitingRoomData
                        {
                            CurrentSceneID = systemTown.CurrentSceneID,
                            TargetTownSceneID = 6033,
                            SceneSubType = CitySceneTable.eSceneSubType.FairDuelPrepare
                        };
                        ClientSystemManager.GetInstance().OpenFrame<FairDuelEntranceFrame>(FrameLayer.Middle, roomData);


                    }
                    else
                    {
                        ActiveManager.GetInstance().OnClickLinkInfo(sub.table.GoLinkInfo);
                        _onClose();
                    }
                }
                else
                {
                    _bindAllButton(dungeonId);
                }

                UpdateSubTabRedFlagByClick(sub);
            });

            //默认是进入，怪物攻城限时活动是前往
            var buttonText = go.transform.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                if (ActivityDungeonDataManager.GetInstance().IsActivityDungeonBeAttackCityMonster(sub.table) == true)
                {
                    buttonText.text = TR.Value("limit_activity_button_description_go");
                }
                else
                {
                    buttonText.text = TR.Value("limit_activity_button_description_enter");
                }
            }
            
            Toggle toggle = bind.GetCom<Toggle>("toggle");
            toggle.group = mTimeLimitToggleGroup;
            toggle.isOn = dungeonId == GuideDungeonId;
            toggle.onValueChanged.AddListener((isOn)=>
            {
                if (isOn)
                {
                    _openActivityDungeonInfoFrame(dungeonId);
                    UpdateSubTabRedFlagByClick(sub);
                }
            });
        }

        private void UpdateSubTabRedFlagByClick(ActivityDungeonSub sub)
        {
            if(sub == null)
                return;

            sub.SetIsAlreadyShowRedFlag();
            
        }

        private void _updateTimeLimitUnitBaseInfo(ActivityDungeonSub sub, ComCommonBind bind)
        {
            Text openTime = bind.GetCom<Text>("openTime");
            Text limitLevel = bind.GetCom<Text>("limitLevel");
            ActivityTimeLimitDayList activityTimeLimitDayList = bind.GetCom<ActivityTimeLimitDayList>("ActivityTimeLimitDayList");

            bool isNeedBg = true;
            if (sub.table.OpenTimeHaveBg.Contains(dontNeedBgStrFlag))
            {
                isNeedBg = false;
            }

            string[] strTemps = sub.table.OpenTimeHaveBg.Split('\n');
            if (strTemps != null && strTemps.Length > 1)
            {
                string textDay = strTemps[1];
                string textTime = strTemps[0];

                if (activityTimeLimitDayList != null)
                {
                    activityTimeLimitDayList.Init(textDay, isNeedBg);
                }

                openTime.SafeSetText(textTime);
            }
            else
            {
                activityTimeLimitDayList.CustomActive(false);
                openTime.SafeSetText(sub.table.OpenTimeHaveBg);
            }
            
            limitLevel.text = sub.level.ToString();

            if(sub.dungeonId == pk3v3CrossDungeonID)
            {
                var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
                if (SystemValueTableData != null)
                {
                    limitLevel.text = SystemValueTableData.Value.ToString();
                }                    
            }
            else if(sub.dungeonId == guildDungeonID)
            {
                limitLevel.text = GuildDataManager.GetGuildDungeonActivityPlayerLvLimit().ToString();
            }
            else if (sub.dungeonId == chijiDungeonID)
            {
                limitLevel.text = sub.activityInfo.level.ToString();
            }
            else if (sub.dungeonId == pk2v2CrossDungeonID)
            {
                var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
                if (SystemValueTableData != null)
                {
                    limitLevel.text = SystemValueTableData.Value.ToString();
                }
            }
        }

        private void _updateTimeLimitUnitState(ActivityDungeonSub sub, ComCommonBind bind)
        {
            if(bind == null)
                return;

            GameObject levelLimitRoot = bind.GetGameObject("levelLimitRoot");
            GameObject startRoot = bind.GetGameObject("startRoot");
            GameObject notOpenRoot = bind.GetGameObject("notOpenRoot");
            GameObject lockRoot = bind.GetGameObject("lockRoot");

            if(levelLimitRoot != null)
            {
                levelLimitRoot.SetActive(false);
            }

            if(startRoot != null)
            {
                startRoot.SetActive(false);
            }

            if(notOpenRoot != null)
            {
                notOpenRoot.SetActive(false);
            }
           
            if(lockRoot != null)
            {
                lockRoot.SetActive(false);
            }

            // TODO 限时活动的等级限制在活动没有开始的时候是无法显示的
            // 由于限时活动只在开始活动之后才同步信息下来  
            if(sub == null)
                return;

            eActivityDungeonState state = sub.state;
            if (sub.dungeonId == pk3v3CrossDungeonID)
            {
                state = Get3v3CrossDungeonActivityState(); 
            }
            else if (sub.dungeonId == guildDungeonID)
            {
                state = GetGuildDungeonActivityState();
            }
            else if(sub.dungeonId == guildBattleID)
            {
                state = GetGuildBattleActivityState();
            }
            else if (sub.dungeonId == guildCrossBattleID)
            {
                state = GetGuildCrossBattleActivityState();
            }
            else if (sub.dungeonId == pk2v2CrossDungeonID)
            {
                state = Get2v2CrossDungeonActivityState();
            }

            switch (state)
            {
                case eActivityDungeonState.None:
                case eActivityDungeonState.Prepare:
                case eActivityDungeonState.End:
                    if(notOpenRoot != null)
                    {
                        notOpenRoot.SetActive(true);
                    }      
                    break;
                case eActivityDungeonState.Start:
                    if(startRoot != null)
                    {
                        startRoot.SetActive(true);
                    }
                    break;
                case eActivityDungeonState.LevelLimit:
                    if(levelLimitRoot != null)
                    {
                        levelLimitRoot.SetActive(true);
                    }
                    if(lockRoot != null)
                    {
                        lockRoot.SetActive(true);
                    }          
                    break;
            }
        }

        private void _updateTimeLimitUnitRedPoint(ActivityDungeonSub sub, ComCommonBind bind)
        {
            GameObject redpoint = bind.GetGameObject("redpoint");
            
            redpoint.SetActive(sub.isshowred);

            if (sub.dungeonId == pk3v3CrossDungeonID)
            {
                ScoreWarStatus scoreState = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
                if (scoreState == ScoreWarStatus.SWS_PREPARE || scoreState == ScoreWarStatus.SWS_BATTLE)
                {
                    redpoint.SetActive(true);

                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
                    if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
                    {
                        redpoint.SetActive(false);
                    }
                }
                else
                {
                    redpoint.SetActive(false);
                }
            }
            else if(sub.dungeonId == guildDungeonID)
            {
                GuildDungeonStatus status = GuildDataManager.GetInstance().GetGuildDungeonActivityStatus();
                redpoint.SetActive(GuildDataManager.CheckActivityLimit() && status != GuildDungeonStatus.GUILD_DUNGEON_END);
            }
            else if(sub.dungeonId == guildBattleID)
            {
                if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL || GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL)
                {
                    EGuildBattleState status = GuildDataManager.GetInstance().GetGuildBattleState();
                    if (status == EGuildBattleState.LuckyDraw || status == EGuildBattleState.Firing)
                    {
                        redpoint.SetActive(true);
                    }
                    else
                    {
                        redpoint.SetActive(false);
                    }
                }
            }
            else if(sub.dungeonId == guildCrossBattleID)
            {
                if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                {
                    EGuildBattleState status = GuildDataManager.GetInstance().GetGuildBattleState();
                    if (status == EGuildBattleState.LuckyDraw || status == EGuildBattleState.Firing)
                    {
                        redpoint.SetActive(true);
                    }
                    else
                    {
                        redpoint.SetActive(false);
                    }
                }
            }
            else if (sub.dungeonId == pk2v2CrossDungeonID)
            {
                ScoreWar2V2Status scoreState = Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus();
                if (scoreState == ScoreWar2V2Status.SWS_2V2_PREPARE || scoreState == ScoreWar2V2Status.SWS_2V2_BATTLE)
                {
                    redpoint.SetActive(true);
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
                    if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
                    {
                        redpoint.SetActive(false);
                    }
                }
                else
                {
                    redpoint.SetActive(false);
                }
            }
        }
        #endregion

        #region Common
        public void _bindAllButton(int dungeonId)
        {
            ActivityDungeonSub sub = ActivityDungeonDataManager.GetInstance().GetSubByDungeonID(dungeonId);

            if (null == sub)
            {
                return ;
            }

            ActivityDungeonTable table = sub.table;

            if (!_checkCondition(sub))
            {
                return ;
            }

            if (ActivityDungeonTable.eActivityType.Daily == table.ActivityType)
            {
                mSelectedLastDungeonId = dungeonId;
                ChapterUtility.OpenChapterFrameByID(dungeonId);
                sub.updateData.Reset();
            }
            else if(table.ActivityType == ActivityDungeonTable.eActivityType.TimeLimit)
            {
                // TODO 这里需要改成一个有意义的ID
                if (0 == sub.dungeonId)
                {
                    ActivityDungeonDataManager.GetInstance().mIsLimitActivityRedPoint = false;
                    
                    BudoManager.GetInstance().TryBeginActive();
                    _onClose();
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshLimitTimeState);
                }
                else if (DungeonTable.eSubType.S_GUILDPK == sub.dungeonTable.SubType)
                {
                    if (GuildDataManager.GetInstance().HasSelfGuild())
                    {
                        ClientSystemManager.instance.OpenFrame<GuildMainFrame>();
                    }
                    else
                    {
                        ClientSystemManager.instance.OpenFrame<GuildListFrame>();
                    }
                    _onClose();
                }
                else 
                {
                    Logger.LogErrorFormat("[活动副本] 限时活动错误类型");
                }
            }
            else
            {
                Logger.LogErrorFormat("[活动副本] 限时活动错误类型");
            }
        }

        private void _openActivityDungeonInfoFrame(int dungeonId)
        {
            mSelectedLastDungeonId = dungeonId;
            ClientSystemManager.instance.OpenFrame<ActivityDungeonInfoFrame>(FrameLayer.Middle, dungeonId);
        }

        private bool _checkCondition(ActivityDungeonSub sub)
        {
            if (sub == null || sub.dungeonTable == null)
            {
                return false;
            }

            //如果地下城ID等于活动堕落深渊 ，点击前往按钮特殊处理。
            if (ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(sub.dungeonTable.ID))
            {
                //每日次数检查
                if (sub.isfinish)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fallen_hell_num_des"));
                    return false;
                }
            }

            if (sub.state == eActivityDungeonState.Start)
            {
                return true;
            }

            if (PlayerBaseData.GetInstance().Level < sub.level)
            {
                SystemNotifyManager.SystemNotify(1008);
                return false;
            }

            return false;
        }
        #endregion

        private void _onClose()
        {
            frameMgr.CloseFrame(this);
        }


        //add for ios appstore
        private bool TryHideBountyLeagueForIOS(ActivityDungeonSub actSub)
        {
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.Bounty_League))
            {
                if (actSub == null)
                    return false;
                int dungeonId = actSub.dungeonId;
                var dungeonItem = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
                if (dungeonItem != null)
                {
                    if (dungeonItem.SubType == DungeonTable.eSubType.S_MONEYREWARDS_PVP)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region Utility

        public static eActivityDungeonState Get2v2CrossDungeonActivityState()
        {
            eActivityDungeonState state = eActivityDungeonState.None;
            ScoreWar2V2Status scoreState = Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus();
            if (scoreState == ScoreWar2V2Status.SWS_2V2_PREPARE || scoreState == ScoreWar2V2Status.SWS_2V2_BATTLE)
            {
                state = eActivityDungeonState.Start;
                var systemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
                if (systemValueTableData != null && PlayerBaseData.GetInstance().Level < systemValueTableData.Value)
                {
                    state = eActivityDungeonState.LevelLimit;
                }
            }
            else
            {
                state = eActivityDungeonState.None;
            }
            return state;
        }

        public static eActivityDungeonState Get3v3CrossDungeonActivityState()
        {
            eActivityDungeonState state = eActivityDungeonState.None;
            ScoreWarStatus scoreState = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
            if (scoreState == ScoreWarStatus.SWS_PREPARE || scoreState == ScoreWarStatus.SWS_BATTLE)
            {
                state = eActivityDungeonState.Start;

                var systemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
                if (systemValueTableData != null && PlayerBaseData.GetInstance().Level < systemValueTableData.Value)
                {
                    state = eActivityDungeonState.LevelLimit;
                }
            }
            else
            {
                state = eActivityDungeonState.None;
            }

            return state;
        }

        public static eActivityDungeonState GetGuildDungeonActivityState()
        {
            eActivityDungeonState state = eActivityDungeonState.None;
            GuildDungeonStatus status = GuildDataManager.GetInstance().GetGuildDungeonActivityStatus();
            if (status == GuildDungeonStatus.GUILD_DUNGEON_PREPARE || status == GuildDungeonStatus.GUILD_DUNGEON_START || status == GuildDungeonStatus.GUILD_DUNGEON_REWARD)
            {
                state = eActivityDungeonState.Start;
                if (PlayerBaseData.GetInstance().Level < GuildDataManager.GetGuildDungeonActivityPlayerLvLimit())
                {
                    state = eActivityDungeonState.LevelLimit;
                }
            }
            else
            {
                state = eActivityDungeonState.None;
            }

            return state;
        }

        public static eActivityDungeonState GetGuildBattleActivityState()
        {
            eActivityDungeonState state = eActivityDungeonState.None;
            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL || GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL)
            {
                EGuildBattleState status = GuildDataManager.GetInstance().GetGuildBattleState();
                if (status == EGuildBattleState.LuckyDraw || status == EGuildBattleState.Firing)
                {
                    state = eActivityDungeonState.Start;
                }
                else
                {
                    state = eActivityDungeonState.None;
                }
            }
            return state;
        }

        public static eActivityDungeonState GetGuildCrossBattleActivityState()
        {
            eActivityDungeonState state = eActivityDungeonState.None;
            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
            {
                EGuildBattleState status = GuildDataManager.GetInstance().GetGuildBattleState();
                if (status == EGuildBattleState.LuckyDraw || status == EGuildBattleState.Firing)
                {
                    state = eActivityDungeonState.Start;
                }
                else
                {
                    state = eActivityDungeonState.None;
                }
            }
            return state;
        }

        #endregion

        #region Discard
        bool _IsSpecialMode(ActivityDungeonSub sub)
        {
            if (sub.activityId == BudoManager.ActiveID)
            {
                if (PlayerBaseData.GetInstance().Level < sub.level)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
        #endregion

    }
}

