using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Protocol;
using ProtoTable;
using Network;
using System.Diagnostics;
using System.Reflection;
using System;
using DG.Tweening;

namespace GameClient
{
    [LoggerModel("Chapter")]
    public class ChapterSelectFrame : ClientFrame
    {
        private enum eAnimateState
        {
            onNone,
            onSelectAnimate,
            onBackAnimate,
        }

        private eAnimateState mAnimateState = eAnimateState.onNone;
        private bool mBisFlag = false; //判断疲劳燃烧是否显示
        private ActivityLimitTime.ActivityLimitTimeData data = null;  //疲劳燃烧活动数据
        private ActivityLimitTime.ActivityLimitTimeDetailData mData = null; //疲劳燃烧活动开启的子任务数据
        private bool mFatigueCombustionTimeIsOpen = false;//是否正在倒计时
        private Text mTime; //疲劳燃烧时间Text文本
        private int mFatigueCombustionTime = -1;//用于疲劳燃烧结束时间戳
        private GameObject YiJieEffectObj = null;
        private bool bInitEffect = false;

        private int mAnniversaryPartySceneID= 6038;//周年派对地下城ID
        private int mSpringFestivalSceneID = 6039;//春节地下城ID

        #region static function
        private class Node
        {
            public string path          = "";
            public DChapterData data    = null;
            public List<int> dungeonIDs = new List<int>();
        }

        /// <summary>
        /// 保存所有章节节点
        /// </summary>
        private static List<Node> sNodes    = new List<Node>();
        public static int         sSceneID  = -1;
        public static void SetSceneID(int id)
        {
            sNodes.Clear();

            sSceneID = id;

            CitySceneTable cyTable = TableManager.instance.GetTableItem<CitySceneTable>(id);

            Logger.LogProcessFormat("[Chapter] 加载章节数据 {0}", id);

            if (null != cyTable)
            {
                for (int i = 0; i < cyTable.ChapterData.Count; ++i)
                {
                    AppendChapter(cyTable.ChapterData[i]);

                    if ((null != sNodes && i >= 0 && i < sNodes.Count) &&
                        (null != sNodes[i].dungeonIDs && sNodes[i].dungeonIDs.Count > 0) )
                    {
                        if (ChapterUtility.GetDungeonState(sNodes[i].dungeonIDs[0]) != ComChapterDungeonUnit.eState.Locked)
                        {
                            selectChapterIdx = i;
                            Logger.LogProcessFormat("[Chapter] 自动选择最后一个解锁的章节 {0}", selectChapterIdx);
                        }
                    }
                }
            }
        }

        public static void AppendChapter(string path)
        {
            DChapterData chData = AssetLoader.instance.LoadRes(path, typeof(DChapterData)).obj as DChapterData;

            if (chData != null)
            {
                Logger.LogProcessFormat("[Chapter] add chapter with path {0}", path);

                Node node = new Node();

                node.path = path;
                node.data = chData;

                for (int i = 0; i < chData.chapterList.Length; ++i)
                {
                    node.dungeonIDs.Add(chData.chapterList[i].dungeonID);
                }

                sNodes.Add(node);
            }
        }

        public static void SetDungeonID(int id)
        {
            if (sDungeonID != id)
            {
                sDungeonID = id;

                Logger.LogProcessFormat("[Chapter] 试图开启关卡{0}", sDungeonID);

                if (ClientSystemManager.instance.IsFrameOpen<ChapterSelectFrame>() && id != ComGuidanceMainItem.LINK_BOX_INT)
                {
                    ChapterSelectFrame frame = ClientSystemManager.instance.GetFrame(typeof(ChapterSelectFrame)) as ChapterSelectFrame;
                    mDungeonID.dungeonID = sDungeonID;

                    for (int i = 0; i < sNodes.Count; ++i)
                    {
                        for (int j = 0; j < sNodes[i].dungeonIDs.Count; ++j)
                        {
                            if (sNodes[i].dungeonIDs[j] == mDungeonID.dungeonIDWithOutPrestory)
                            {
                                frame.mEnterWay = ChapterSelectFrame.eEnterWay.onGuide;
                                frame._changeChapter(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

#endregion

#region private
        private class ChapterNode
        {
            public Node node        = null;

            public int index        = -1;

            public int unlockCount  = 0;

            public void SetNode(Node node)
            {
                Logger.LogProcessFormat("[ChapterNode] set node {0}", node.path);

                this.node   = node;
                this.index  = -1;
                unlockCount = 0;
            }

            public DChapterData GetData()
            {
                if (null != node)
                {
                    return node.data;
                }

                return null;
            }

            public ChaptertDungeonUnit[] Chapters()
            {
                if (null != node)
                {
                    return node.data.chapterList;
                }

                return null;
            }

            public void UpdateIndex(int dungeonID)
            {
                if(node != null && node.dungeonIDs != null)
                {
                    for (int i = 0; i < node.dungeonIDs.Count; ++i)
                    {
                        if (node.dungeonIDs[i] == dungeonID)
                        {
                            index = i;
                        }
                    }
                }
            }

            public int CurrentDungeonID()
            {
                if (null == node || index < 0 || index >= node.dungeonIDs.Count)
                {
                    return -1 ;
                }

                return node.dungeonIDs[index];
            }
        }

        private ChapterNode mCurrent        = new ChapterNode();


        /// <summary>
        /// 选择的章节索引
        /// </summary>
        private static int mSelectChapterIdx = 0;

        private static int selectChapterIdx 
        {
            get 
            {
                mSelectChapterIdx = Mathf.Clamp(mSelectChapterIdx, 0, sNodes.Count - 1);
                return mSelectChapterIdx;
            }

            set 
            {
                int idx = Mathf.Clamp(value, 0, sNodes.Count - 1);
                if (value == idx)
                {
                    mSelectChapterIdx = idx;
                }
            }
        }

        private static int _getChapterIdByIdx(int idx)
        {
            if (sNodes.Count > idx && idx >= 0)
            {
                List<int> dlist = sNodes[idx].dungeonIDs;

                if (null != sNodes[idx].data)
                {
                    return sNodes[idx].data.nameNumberIdx;
                }
                //return 
                //return 
                //if (dlist.Count > 0)
                //{
                //    DungeonID id = new DungeonID(dlist[0]);
                //    return id.chapterID;
                //}
            }

            return -1;
        }

        public static int GetCurrentSelectChapter()
        {
            return _getChapterIdByIdx(selectChapterIdx);
        }

        public List<int> GetCurrentChapterDungeonIDs()
        {
            if(mCurrent == null || mCurrent.node == null)
            {
                return null;
            }

            List<int> ids = new List<int>();
            ids.AddRange(mCurrent.node.dungeonIDs);

            return ids;
        }

        public static bool IsCurrentSelectChapterShowReward()
        {
            //return (GetCurrentSelectChapter() <= 6 && GetCurrentSelectChapter() != 4);
            int selectChapterId = GetCurrentSelectChapter();
            var chapterTableData = TableManager.GetInstance().GetTableItem<ChapterTable>(selectChapterId);
            if (chapterTableData == null)
            {
                return false;
            }
            if(chapterTableData.RewardIsOpen == 1)
            {
                return true;
            }
            return false;
        }

        private static bool isSelectChapterLeftMost
        {
            get 
            {
                return selectChapterIdx == 0;
            }
        }

        private static bool isSelectChapterRightMost
        {
            get 
            {
                return selectChapterIdx == (sNodes.Count - 1);
            }
        }


        private static int sDungeonID       = -1;

        private static DungeonID mDungeonID = new DungeonID(0);

        private bool mIsFindMission     	= false;

        private Toggle[] mToggleList    	= new Toggle[0];
        private ComChapterDungeonUnit[] mToggleGameUnitList;
#endregion

		#region ExtraUIBind
		private ComChapterSelectUnlock mSelectUnlock = null;
		private ToggleGroup mToggleGroup = null;
		private Image mLeftBackground = null;
		private Image mLeftMid = null;
		private RectTransform mRRecttransform = null;
		private GameObject mRewardRedPoint = null;
		private UIGray mLeftButton = null;
		private UIGray mRightButton = null;
		private Button mReward = null;
		private Button mSelectLeft = null;
		private Button mSelectRight = null;
		private Button mBtnClose = null;
        private GameObject mUnitRoot = null;
        private Image mChapterNameImage = null;
        private Text mTextChapterNum = null;
        private Text mTextChapterName = null;
		private Text mChapterIndex = null;
		private GameObject mPointRoot = null;
		private ToggleGroup mPointRootGroup = null;
		private GameObject mLeftRed = null;
		private GameObject mRightRed = null;
		private GameObject mNodeRoot = null;
		private ComChapterSelectAnimate mChapterSelectAnimate = null;
		private Text mChapterRewardCount = null;
		private Text mChapterRewardSum = null;
		private GameObject mRedPoint = null;
		private Text mRedPointSum = null;
		private GameObject mRewardComplete = null;
		private GameObject mFatigueCombustionRoot = null;
		private DOTweenAnimation mRewardAnimation = null;
        private GameObject mExchangeShopRoot = null;
        private GameObject mEffectRoot = null;
        private GameObject mTitleBg = null;
        private GameObject mChapter = null;
        private GameObject mYijieTitleBg = null;
        GameObject yijieChallengeNumRoot = null;
        Text yijieChallengeNumInfo = null;
        private GameObject mChapterIndexParentGo;

        private GameObject mHellroot0 = null;
        private GameObject mHellroot1 = null;
        private GameObject mYg0 = null;
        private GameObject mYg1 = null;
        private GameObject mTicketRoot;
        private GameObject mBindTicketRoot;
        private GameObject mFatigueRoot;
        protected sealed override void _bindExUI()
		{
			mSelectUnlock = mBind.GetCom<ComChapterSelectUnlock>("SelectUnlock");
			mToggleGroup = mBind.GetCom<ToggleGroup>("ToggleGroup");
			mLeftBackground = mBind.GetCom<Image>("LeftBackground");
			mLeftMid = mBind.GetCom<Image>("LeftMid");
			mRRecttransform = mBind.GetCom<RectTransform>("RRecttransform");
			mRewardRedPoint = mBind.GetGameObject("RewardRedPoint");
			mLeftButton = mBind.GetCom<UIGray>("LeftButton");
			mRightButton = mBind.GetCom<UIGray>("RightButton");
			mReward = mBind.GetCom<Button>("Reward");
			if (null != mReward)
			{
				mReward.onClick.AddListener(_onRewardButtonClick);
			}
			mSelectLeft = mBind.GetCom<Button>("SelectLeft");
			if (null != mSelectLeft)
			{
				mSelectLeft.onClick.AddListener(_onSelectLeftButtonClick);
			}
			mSelectRight = mBind.GetCom<Button>("SelectRight");
			if (null != mSelectRight)
			{
				mSelectRight.onClick.AddListener(_onSelectRightButtonClick);
			}
            mBtnClose = mBind.GetCom<Button>("Close");
            if (null != mBtnClose)
            {
                mBtnClose.SafeAddOnClickListener(_closeFrame);
            }
            mUnitRoot = mBind.GetGameObject("UnitRoot");
			mChapterNameImage = mBind.GetCom<Image>("ChapterNameImage");
            mTextChapterNum = mBind.GetCom<Text>("ChapterNum");
            mTextChapterName = mBind.GetCom<Text>("ChapterName");
            mChapterIndex = mBind.GetCom<Text>("ChapterIndex");
            mPointRoot = mBind.GetGameObject("PointRoot");
			mPointRootGroup = mBind.GetCom<ToggleGroup>("PointRootGroup");
			mLeftRed = mBind.GetGameObject("LeftRed");
			mRightRed = mBind.GetGameObject("RightRed");
			mNodeRoot = mBind.GetGameObject("NodeRoot");
			mChapterSelectAnimate = mBind.GetCom<ComChapterSelectAnimate>("ChapterSelectAnimate");
			mChapterRewardCount = mBind.GetCom<Text>("chapterRewardCount");
			mChapterRewardSum = mBind.GetCom<Text>("chapterRewardSum");
			mRedPoint = mBind.GetGameObject("RedPoint");
			mRedPointSum = mBind.GetCom<Text>("RedPointSum");
			mRewardComplete = mBind.GetGameObject("RewardComplete");
			mFatigueCombustionRoot = mBind.GetGameObject("FatigueCombustionRoot");
			mRewardAnimation = mBind.GetCom<DOTweenAnimation>("RewardAnimation");
            mExchangeShopRoot = mBind.GetGameObject("ExchangeShopRoot");
            mEffectRoot = mBind.GetGameObject("effectRoot");
            mTitleBg = mBind.GetGameObject("TitleBg");
            mChapter = mBind.GetGameObject("Chapter");
            mYijieTitleBg = mBind.GetGameObject("YijieTitleBg");
            yijieChallengeNumRoot = mBind.GetGameObject("yijieChallengeNumRoot");
            yijieChallengeNumInfo = mBind.GetCom<Text>("numInfo");
            mChapterIndexParentGo = mBind.GetGameObject("ChapterIndexParent");

            mHellroot0 = mBind.GetGameObject("hellroot0");
            mHellroot1 = mBind.GetGameObject("hellroot1");
            mYg0 = mBind.GetGameObject("yg0");
            mYg1 = mBind.GetGameObject("yg1");
            mTicketRoot = mBind.GetGameObject("ticketRoot");
            mBindTicketRoot = mBind.GetGameObject("bindTicketRoot");
            mFatigueRoot = mBind.GetGameObject("fatigueRoot");
        }

        protected sealed override void _unbindExUI()
        {
            Array.Clear(mToggleList, 0, mToggleList.Length);
            Array.Clear(mToggleGameUnitList, 0, mToggleGameUnitList.Length);
            mSelectUnlock = null;
			mToggleGroup = null;
			mLeftBackground = null;
			mLeftMid = null;
			mRRecttransform = null;
			mRewardRedPoint = null;
			mLeftButton = null;
			mRightButton = null;
			if (null != mReward)
			{
				mReward.onClick.RemoveListener(_onRewardButtonClick);
			}
			mReward = null;
			if (null != mSelectLeft)
			{
				mSelectLeft.onClick.RemoveListener(_onSelectLeftButtonClick);
			}
			if (null != mSelectRight)
			{
				mSelectRight.onClick.RemoveListener(_onSelectRightButtonClick);
			}
            mSelectLeft = null;
            if (null != mBtnClose)
            {
                mBtnClose.SafeRemoveOnClickListener(_closeFrame);
            }
            mBtnClose = null;
            mSelectRight = null;
			mUnitRoot = null;
			mChapterNameImage = null;
            mTextChapterNum = null;
            mTextChapterName = null;
            mChapterIndex = null;
			mPointRoot = null;
			mPointRootGroup = null;
			mLeftRed = null;
			mRightRed = null;
			mNodeRoot = null;
			mChapterSelectAnimate = null;
			mChapterRewardCount = null;
			mChapterRewardSum = null;
			mRedPoint = null;
			mRedPointSum = null;
			mRewardComplete = null;
			mFatigueCombustionRoot = null;
			mRewardAnimation = null;
            mExchangeShopRoot = null;
            mEffectRoot = null;
            mTitleBg = null;
            mChapter = null;
            mYijieTitleBg = null;
            yijieChallengeNumRoot = null;
            yijieChallengeNumInfo = null;
            mChapterIndexParentGo = null;
        }
		#endregion

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _closeFrame();

        }
        private void _onRewardButtonClick()
        {
            /* put your code in here */
            if(sSceneID == mSpringFestivalSceneID)
            {
                ClientSystemManager.GetInstance().OpenFrame<LimitTimeActivityFrame>(FrameLayer.Middle, 22880);
            }
            else if(sSceneID == mAnniversaryPartySceneID)
            {
                ClientSystemManager.GetInstance().OpenFrame<LimitTimeActivityFrame>(FrameLayer.Middle, 22850);
            }
            else
            {
                _onDungeonReward();
            }

        }
        private void _onSelectLeftButtonClick()
        {
            /* put your code in here */

            if (_isUnlockChapter(selectChapterIdx - 1))
            {
                _onLeft();
            }

        }
        private void _onSelectRightButtonClick()
        {
            /* put your code in here */

            if (_isUnlockChapter(selectChapterIdx + 1))
            {
                _onRight();
            }
        }
#endregion

#region Override
        protected sealed override void _OnLoadPrefabFinish()
        {
            Logger.LogProcessFormat("[Chapter] selectChapterIdx {0}", selectChapterIdx);

            if(sNodes != null && selectChapterIdx >= 0 && selectChapterIdx < sNodes.Count)
            {
                if(mCurrent != null)
                {
                    mCurrent.SetNode(sNodes[selectChapterIdx]);
                }
                else
                {
                    Logger.LogErrorFormat("mCurrent == null,selectChapterIdx = {0}", selectChapterIdx);
                }
            }
            else
            {
                if(sNodes == null)
                {
                    Logger.LogErrorFormat("ChapterSelectFrame [_OnLoadPrefabFinish] sNodes == null, selectChapterIdx = {0}", selectChapterIdx);
                }
                else
                {
                    Logger.LogErrorFormat("ChapterSelectFrame [_OnLoadPrefabFinish] Argument is out of range, selectChapterIdx = {0},sNodes.Count = {1}", selectChapterIdx, sNodes.Count);
                }
            }
     
            if(mDungeonID != null)
            {
                mDungeonID.dungeonID = sDungeonID;

                if(mCurrent != null)
                {
                    mCurrent.UpdateIndex(mDungeonID.dungeonIDWithOutPrestory);
                }
            }

            DChapterData data = null;

            if(mCurrent != null)
            {
                data = mCurrent.GetData();
            }

            if (data == null)
            {
                Logger.LogError("mData is nil");
            }

            if(data.chapterList == null)
            {
                Logger.LogError("chapterList is nil");
            }

            int len = 0;

            if(data.chapterList != null)
            {
                len = data.chapterList.Length;
            }
         
            if(len <= 0)
            {
                Logger.LogErrorFormat("data.chapterList.Length = {0}", len);
            }

            if(len > 0)
            {
                mToggleList = new Toggle[len];
                mToggleGameUnitList = new ComChapterDungeonUnit[len];
            }

            if(mBind != null)
            {
                string unit = mBind.GetPrefabPath("unit");
                mBind.ClearCacheBinds(unit);

                for (int i = 0; i < len; ++i)
                {
                    ComCommonBind bind = mBind.LoadExtraBind(unit);
                    if (null != bind)
                    {
                        if(mNodeRoot != null)
                        {
                            Utility.AttachTo(bind.gameObject, mNodeRoot);
                        }
                        
                        bind.gameObject.name = string.Format("Level{0}", i);

                        ComChapterDungeonUnit dungeonUnit = bind.GetCom<ComChapterDungeonUnit>("DungeonUnit");
                        Toggle select = bind.GetCom<Toggle>("Select");
                        Text text = bind.GetCom<Text>("OrderNumber");

                        int idx = i;

                        if(text != null)
                        {
                            text.text = string.Format("{0}", i + 1);
                        }

                        mToggleGameUnitList[i] = dungeonUnit;
                        mToggleList[i] = select;
                    }
                }
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Select/ChapterSelect";
        }
        public sealed override bool IsNeedUpdate()
        {
            return true;
        }
        protected sealed override void _OnUpdate(float timeElapsed)
        {
            //_SetFatigueCombustionTime();
        }
               
        protected sealed override void _OnCloseFrame()
        {
            _onUnbind();
            _clearAll();

            YiJieEffectObj = null;
            bInitEffect = false;
        }

        private void _updateRewardButtonStatus()
        {
            mReward.gameObject.SetActive(IsCurrentSelectChapterShowReward());
        }

        private void _updateRewardRedPoint()
        {
            bool isShow = ChapterUtility.IsChapterCanGetChapterReward(GetCurrentSelectChapter());
            KeyValuePair<int, int> rate = ChapterUtility.GetChapterRewardByChapterIdx(GetCurrentSelectChapter());
            int CanGetSum = ChapterUtility.GetChapterCanGetByChapterIdx(GetCurrentSelectChapter());
            
            //宝箱的四种状态
            if(CanGetSum==0&&rate.Key==0)
            {
                mRewardRedPoint.CustomActive(false);
                mRewardAnimation.DOPause();
                mRewardComplete.CustomActive(false);
            }
            else if(CanGetSum!=0)
            {
                mRewardRedPoint.CustomActive(false);
                mRedPoint.CustomActive(true);
                _updateRedPointNum(CanGetSum);
                mRewardAnimation.DOPause();
                mRewardComplete.CustomActive(false);
            }
            else if(CanGetSum==0&&rate.Key>0&&isShow)
            {
                mRewardRedPoint.CustomActive(true);
                mRedPoint.CustomActive(false);
                mRewardAnimation.DOPlayForward();
                mRewardComplete.CustomActive(false);
            }
            else if(CanGetSum==0&&rate.Key==rate.Value&&!isShow)
            {
                mRewardRedPoint.CustomActive(false);
                mRedPoint.CustomActive(false);
                mRewardAnimation.DOPause();
                mRewardComplete.CustomActive(true);
            }
            else
            {
                mRewardRedPoint.CustomActive(false);
                mRedPoint.CustomActive(false);
                mRewardAnimation.DOPause();
                mRewardComplete.CustomActive(false);
            }
            mChapterRewardCount.text = rate.Key.ToString();
            mChapterRewardSum.text = rate.Value.ToString();

            mLeftRed.SetActive(ChapterUtility.IsChapterCanGetChapterReward(_getChapterIdByIdx(selectChapterIdx - 1)));
            mRightRed.SetActive(ChapterUtility.IsChapterCanGetChapterReward(_getChapterIdByIdx(selectChapterIdx + 1)));

            if(sSceneID == mAnniversaryPartySceneID || sSceneID == mSpringFestivalSceneID)
            {
                mRewardRedPoint.CustomActive(false);
                mRedPoint.CustomActive(false);
                mRewardAnimation.DOPause();
                mRewardComplete.CustomActive(false);
            }
        }

        private void _updateRedPointNum(int RedPointSum)
        {
            mRedPointSum.text = RedPointSum.ToString();
        }
        
        protected sealed override void _OnOpenFrame()
        {
            mAnimateState = eAnimateState.onNone;

            _onBind();
            _loadChapterData(mCurrent.GetData());
            _InitBgImage();
            _updateUnlockChapter();

            if (NewbieGuideManager.GetInstance().IsGuidingControl() && !NewbieGuideManager.GetInstance().IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.GuanKaGuide))
            {
                NewbieGuideManager.GetInstance().ManagerFinishGuide(NewbieGuideManager.GetInstance().GetCurTaskID());
            }

            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.GuankaFrameOpen);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuankaFrameOpen);

            _updateMissionStatus();

            _updateRewardRedPoint();
            _updateRewardButtonStatus();

            _updateTipsPoint();

            //_autoOpenFrame();

            _updateSelectButton();

            //if (mFatigueCombustionRoot != null)
            //{
            //    _InitFatigueCombustionGameObject(mFatigueCombustionRoot);
            //}

            _InitExchangeShopRoot();
            _InitYiJieEffect();

            UpdateYiJieChallengeTimes();
            GameStatisticManager.instance.DoStatLevelChoose(StatLevelChooseType.ENTER_SELECT, sSceneID);

            HideSomething();
        }
#endregion

        private void UpdateYiJieChallengeTimes()
        {
            int dungeonId = 0;
            ChaptertDungeonUnit[] chapterList = mCurrent.GetData().chapterList;
            if (chapterList != null && chapterList.Length > 0)
            {
                dungeonId = chapterList[0].dungeonID; // 所有异界关卡共用挑战次数，所以取第一个关卡id即可
            }
            if (_GetChapterIndex() != yijieChapterIndex)
            {
                return;
            }           
            if (yijieChallengeNumRoot == null)
                return;
            var dungeonDailyBaseTimes = DungeonUtility.GetDungeonDailyBaseTimes(dungeonId);
            if (dungeonDailyBaseTimes <= 0 && false)
            {
                yijieChallengeNumRoot.CustomActive(false);
            }
            else
            {
                yijieChallengeNumRoot.CustomActive(true);
                if (yijieChallengeNumInfo != null)
                {
                    var leftTimes = _getLeftTimes();
                    yijieChallengeNumInfo.text = string.Format(TR.Value("resist_magic_challenge_times"),
                        leftTimes, dungeonDailyBaseTimes);
                }
            }
        }
        private int _getLeftTimes()
        {
            int dungeonId = 0;
            ChaptertDungeonUnit[] chapterList = mCurrent.GetData().chapterList;
            if (chapterList != null && chapterList.Length > 0)
            {
                dungeonId = chapterList[0].dungeonID; // 所有异界关卡共用挑战次数，所以取第一个关卡id即可
            }
            var finishedTimes = DungeonUtility.GetDungeonDailyFinishedTimes(dungeonId);
            var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(dungeonId);
            var leftTimes = dailyMaxTime - finishedTimes;
            if (leftTimes < 0)
                leftTimes = 0;
            return leftTimes;
        }
        public const int yijieChapterIndex = 31;
        private void _InitYiJieEffect()
        {
            if(_GetChapterIndex() != yijieChapterIndex)
            {
                return;
            }

            if(bInitEffect)
            {
                return;
            }

            FunctionUnLock unLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.YijieAreaOpen);
            if (unLockData == null)
            {
                return;
            }

            if (!Utility.IsUnLockArea(unLockData.AreaID))
            {
                return;
            }
            YiJieEffectObj = AssetLoader.instance.LoadResAsGameObject("Effects/UI/Prefab/EffUI_Yijie/Prefab/Eff_UI_YiJie");
            if (YiJieEffectObj != null)
            {
                Utility.AttachTo(YiJieEffectObj, mEffectRoot);
                bInitEffect = true;
            }
        }

        private void _InitBgImage()
        {
            //mChapterNameImage.CustomActive(_GetChapterIndex() != 31);
            //mTitleBg.CustomActive(_GetChapterIndex() != 31);
            mChapter.CustomActive(_GetChapterIndex() != 31);
            mYijieTitleBg.CustomActive(_GetChapterIndex() == 31);
        }

        private void _InitExchangeShopRoot()
        {
            mExchangeShopRoot.CustomActive(false);
            var mCitySceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(sSceneID);
            if (null != mCitySceneTable && mCitySceneTable.ExchangeStoreEntrance != "")
            {
                string[] strList = mCitySceneTable.ExchangeStoreEntrance.Split('|');
                if (strList.Length == 3)
                {
                    bool isFlag = int.Parse(strList[0]) == 1;
                    int iShopID = int.Parse(strList[1]);
                    string sIconPath = strList[2];

                    mExchangeShopRoot.CustomActive(isFlag);

                    ComCommonBind mBind = mExchangeShopRoot.GetComponent<ComCommonBind>();
                    Image iIcon = mBind.GetCom<Image>("Icon");
                    Button bBtn = mBind.GetCom<Button>("ExchangeShop");
                    Text nameText = mBind.GetCom<Text>("text");
                    bBtn.onClick.RemoveAllListeners();
                    bBtn.onClick.AddListener(() =>
                    {
                        OnShopButtonClick(iShopID);
                    });
                    ETCImageLoader.LoadSprite(ref iIcon, sIconPath);

                    //特殊情况，特殊处理。设计需要重构
                    InitShopName(iShopID, ref nameText);
                }
            }
        }

        private void InitShopName(int shopId, ref Text nameText)
        {
            if(nameText != null && shopId == 23)
            {
                //虚空商店
                var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
                if (shopTable != null)
                {
                    nameText.text = shopTable.ShopName;
                }
            }
        }

        private void OnShopButtonClick(int shopId)
        {
            //直接跳转到虚空商店
            if (shopId == 23)
            {
                ShopNewDataManager.GetInstance().JumpToShopById(shopId);
            }
            else
            {
                //跳转到晶石商店
                ShopNewDataManager.GetInstance().OpenShopNewFrame(24, shopId);
            }

        }

        private void _onUpdateMission(UInt32 taskID)
        {
            _updateRewardRedPoint();
        }

#region update
        private void _updateTipsPoint()
        {
            string unit = mBind.GetPrefabPath("pointunit");
            mBind.ClearCacheBinds(unit);
            for (int i = 0; i < sNodes.Count; ++i)
            {
                if (_isUnlockChapter(i))
                {
                    ComCommonBind bind = mBind.LoadExtraBind(unit);
                    if (null != bind)
                    {
                        Utility.AttachTo(bind.gameObject, mPointRoot);

                        Toggle select = bind.GetCom<Toggle>("select");

                        select.group = mPointRootGroup;

                        select.isOn = selectChapterIdx == i;
                    }
                }
            }
        }

        /// <summary>
        /// 更新解锁的关卡
        /// </summary>
        private void _updateUnlockChapter()
        {
            List<Battle.DungeonHardInfo> hardList = BattleDataManager.GetInstance().ChapterInfo.allHardInfo;

            ChaptertDungeonUnit[] chapterList = mCurrent.GetData().chapterList;

            mCurrent.unlockCount = 0;

            DungeonID did = new DungeonID(0);

            for (int i = 0; i < chapterList.Length; ++i)
            {
                ChaptertDungeonUnit item = chapterList[i];

                Logger.LogFormat("item.id {0}", item.dungeonID);

                did.dungeonID = ChapterUtility.GetReadyDungeonID(item.dungeonID);

                ComChapterDungeonUnit.eState state = ChapterUtility.GetDungeonState(did.dungeonID);

                if (did.prestoryID > 0) { mToggleGameUnitList[i].SetDungeonType(ComChapterDungeonUnit.eDungeonType.Prestory); }
                else  { mToggleGameUnitList[i].SetDungeonType(ComChapterDungeonUnit.eDungeonType.Normal); }

                mToggleGameUnitList[i].SetDungeonID(item.dungeonID);

                if(state == ComChapterDungeonUnit.eState.Locked)
                {
                    if (TeamUtility.IsEliteDungeonID(item.dungeonID))
                    {
                        mToggleGameUnitList[i].ShowDungeonLvLimit(false);
                        mToggleGameUnitList[i].SetExtarLockTipText(TR.Value("elite_dungeon_unlock_tip"));
                    }
                    else
                    {
                        mToggleGameUnitList[i].ShowDungeonLvLimit(true);
                        mToggleGameUnitList[i].SetExtarLockTipText("");
                    }
                }
                else
                {
                    mToggleGameUnitList[i].ShowDungeonLvLimit(true);
                    mToggleGameUnitList[i].SetExtarLockTipText("");
                }
                DungeonTable tb = TableManager.instance.GetTableItem<DungeonTable>(did.dungeonID);
                if (null != tb)
                {
                    //Sprite sp = AssetLoader.instance.LoadRes(tb.TumbPath, typeof(Sprite)).obj as Sprite;
                    //mToggleGameUnitList[i].SetBackgroud(sp);
                    mToggleGameUnitList[i].SetBackgroud(tb.TumbPath);

                    //sp = AssetLoader.instance.LoadRes(tb.TumbChPath, typeof(Sprite)).obj as Sprite;
                    //mToggleGameUnitList[i].SetCharactorSprite(sp);
                    mToggleGameUnitList[i].SetCharactorSprite(tb.TumbChPath);
                }
                else 
                {
                    DungeonTable ntb = TableManager.instance.GetTableItem<DungeonTable>(item.dungeonID);
                    if (null != ntb)
                    {
                        //Sprite sp = AssetLoader.instance.LoadRes(ntb.TumbPath, typeof(Sprite)).obj as Sprite;
                        //mToggleGameUnitList[i].SetBackgroud(sp);
                        mToggleGameUnitList[i].SetBackgroud(ntb.TumbPath);

                        //sp = AssetLoader.instance.LoadRes(ntb.TumbChPath, typeof(Sprite)).obj as Sprite;
                        //mToggleGameUnitList[i].SetCharactorSprite(sp);
                        mToggleGameUnitList[i].SetCharactorSprite(ntb.TumbChPath);
                    }
                }
                mToggleGameUnitList[i].SetState(state);     //要放在这里设置状态 因为setstate里面回去设置灰态，灰态设置要在图片设置之后，不然灰态的uigray材质会被设置图片时给替换掉
            }
        }

        public static bool IsInChallenge(int dungeonID)
        {
            return CountDataManager.GetInstance().GetCount(string.Format("dp_{0}",dungeonID)) > 0;
        }
        /// <summary>
        /// 更新所有关卡信息
        /// 
        /// 任务相关标识
        /// </summary>/
        private void _updateMissionStatus()
        {
            for (int i = 0; i < mCurrent.GetData().chapterList.Length; ++i)
            {
                int originId = mCurrent.GetData().chapterList[i].dungeonID;

                int readyId = ChapterUtility.GetReadyDungeonID(originId);
                if(_GetChapterIndex() == yijieChapterIndex)
                {
                    mToggleGameUnitList[i].SetIsChallenging(IsInChallenge(originId));
                }
                else
                {
                    mToggleGameUnitList[i].SetIsChallenging(false);
                }
                mToggleGameUnitList[i].SetDungeonID(originId);
                if(TeamUtility.IsEliteDungeonID(originId))
                {
                    mToggleGameUnitList[i].ShowEliteBg(true);
                }
                else
                {
                    mToggleGameUnitList[i].ShowEliteBg(false);
                }

                mToggleGameUnitList[i].UpdateUnityType(mCurrent.GetData().chapterList[i].chapterDungeonUnitType, mCurrent.GetData().chapterList[i].iconPos);

                DungeonTable item = TableManager.instance.GetTableItem<DungeonTable>(readyId);
                if (null != item)
                {
                    mToggleGameUnitList[i].SetName(item.Name, item.RecommendLevel);

                    mToggleGameUnitList[i].SetType(ComChapterDungeonUnit.eMissionType.None);

                    if (ChapterUtility.HasMissionByDungeonID(readyId))
                    {
                        mToggleGameUnitList[i].SetType(ComChapterDungeonUnit.eMissionType.Main);
                    }
                }
                else 
                {
                    DungeonTable defitem = TableManager.instance.GetTableItem<DungeonTable>(originId);

                    if (null != defitem)
                    {
                        mToggleGameUnitList[i].SetName(defitem.Name, defitem.RecommendLevel);
                    }
                    else 
                    {
                        mToggleGameUnitList[i].SetName("", "");
                    }
                }
            }
        }

#endregion

        /// <summary>
        /// 加载关卡数据
        /// </summary>
        private void _loadChapterData(DChapterData data)
        {
            if (null == data)
            {
                return;
            }

            Sprite sprite = AssetLoader.instance.LoadRes(data.backgroundPath, typeof(Sprite)).obj as Sprite;
            mLeftBackground.enabled = false;
            if (sprite != null)
            {
                mLeftBackground.enabled = true;
                // mLeftBackground.sprite = sprite;
                ETCImageLoader.LoadSprite(ref mLeftBackground, data.backgroundPath);
            }
            Sprite midsprite = null;
            if (!string.IsNullOrEmpty(data.middlegroudnPath))
            {
                midsprite = AssetLoader.instance.LoadRes(data.middlegroudnPath, typeof(Sprite)).obj as Sprite;
            }
            mLeftMid.enabled = false;
            if (midsprite != null)
            {
                mLeftMid.enabled = true;
                // mLeftMid.sprite = midsprite;
                ETCImageLoader.LoadSprite(ref mLeftMid, data.middlegroudnPath);
                mLeftMid.SetNativeSize();
                mLeftMid.GetComponent<RectTransform>().localPosition = data.middlePos;
            }

            // mChapterNameImage.sprite = AssetLoader.instance.LoadRes(data.namePath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref mChapterNameImage, data.namePath);
            mChapterNameImage.SetNativeSize();

            ChapterTable chapterTable = TableManager.GetInstance().GetTableItem<ChapterTable>(data.nameNumberIdx);
            mTextChapterNum.SafeSetText(chapterTable.ChapterName);
            mTextChapterName.SafeSetText(data.name);

            //mChapterIndex.text = string.Format("{0}", GetCurrentSelectChapter());
            mChapterIndex.text = data.nameNumberIdx.ToString();

            int maxUnlockId = _getMaxUnlockDungeonIndex(data);

            for (int i = 0; i < data.chapterList.Length; ++i)
            {
                GameObject obj = mToggleGameUnitList[i].gameObject;
                ChaptertDungeonUnit item = data.chapterList[i];

                ComCommonBind bind = obj.GetComponent<ComCommonBind>();

                RectTransform sourcePosition = bind.GetCom<RectTransform>("sourcePosition");
                RectTransform targetPosition = bind.GetCom<RectTransform>("targetPosition");
                TriangleGraph angleGraph = bind.GetCom<TriangleGraph>("angleGraph");
                RectTransform thumbRoot = bind.GetCom<RectTransform>("thumbRoot");
                RectTransform targetRightPosition = bind.GetCom<RectTransform>("targetRightPosition");

                Toggle com = mToggleList[i];

                obj.transform.localPosition  = data.chapterList[i].position;

                sourcePosition.localPosition = data.chapterList[i].angleSourcePosition;
                targetPosition.localPosition = data.chapterList[i].angleTargetPosition;
                targetRightPosition.localPosition = data.chapterList[i].angleTargetRightPosition;
                thumbRoot.localPosition      = data.chapterList[i].thumbOffset;

                com.group = mToggleGroup;

                //int id = ChapterUtility.GetDungeonID2Enter(data.chapterList[i].dungeonID);
                //com.isOn = ChapterUtility.GetDungeonCanEnter(id, false);
				if (mCurrent.index < 0) 
				{
					com.isOn = maxUnlockId == i;
				} 
				else
				{
					com.isOn = mCurrent.index == i;
				}

                int idx = i;

                com.onValueChanged.AddListener((bool value) =>
                {
                    _onSelected(idx, value);
                });

                //派对地下城要显示次数
                Text timeTxt = bind.GetCom<Text>("Times");
                if (sSceneID == mAnniversaryPartySceneID)
                {
                    timeTxt.CustomActive(true);
                    UpdateLevelChallengeTimes(data.chapterList[i].dungeonID, timeTxt);
                }
                else
                {
                    timeTxt.CustomActive(false);
                }

                if(i == 0)
                {
                    var dungeonItem = TableManager.instance.GetTableItem<DungeonTable>(item.dungeonID);
                    if(dungeonItem != null)
                    {
                        _setConsumeMode(dungeonItem.SubType);
                    }
                }
            }
        }
        private void _setConsumeMode(DungeonTable.eSubType subType)
        {
            mYg0.CustomActive(false);
            mYg1.CustomActive(false);
            mHellroot0.CustomActive(false);
            mHellroot1.CustomActive(false);
            mTicketRoot.CustomActive(true);
            mBindTicketRoot.CustomActive(true);
            mFatigueRoot.CustomActive(true);

            switch (subType)
            {
                case DungeonTable.eSubType.S_HELL:
                case DungeonTable.eSubType.S_HELL_ENTRY:
                case DungeonTable.eSubType.S_ANNIVERSARY_HARD:
                    mHellroot0.CustomActive(true);
                    mHellroot1.CustomActive(true);
                    mTicketRoot.CustomActive(false);
                    mBindTicketRoot.CustomActive(false);
                    break;
                case DungeonTable.eSubType.S_YUANGU:
                    mYg0.CustomActive(true);
                    mYg1.CustomActive(true);
                    mTicketRoot.CustomActive(false);
                    mBindTicketRoot.CustomActive(false);
                    break;
                case DungeonTable.eSubType.S_TEAM_BOSS:
                    mFatigueRoot.SetActive(false);
                    break;
                case DungeonTable.eSubType.S_TREASUREMAP:
                    break;
                default:
                    break;
            }
        }

        private void UpdateLevelChallengeTimes(int dungeonId,Text timeTxt)
        {
            var dungeonDailyBaseTimes = DungeonUtility.GetDungeonDailyBaseTimes(dungeonId);
           
            if (timeTxt != null)
            {
                if (dungeonDailyBaseTimes <= 0)
                {
                    timeTxt.CustomActive(false);
                }
                var finishedTimes = DungeonUtility.GetDungeonDailyFinishedTimes(dungeonId);
                var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(dungeonId);
                var leftTimes = dailyMaxTime - finishedTimes;
                if (leftTimes < 0) leftTimes = 0;
                timeTxt.text = string.Format(TR.Value("AnniversaryParty_Times"),
                     leftTimes, dungeonDailyBaseTimes);
            }

        }
        private int _getMaxUnlockDungeonIndex(DChapterData data)
		{
			int unlockMaxIdx = 0;

			for (int i = 0; i < data.chapterList.Length; ++i) 
            {
				int id = ChapterUtility.GetDungeonID2Enter(data.chapterList[i].dungeonID);

				if (ChapterUtility.GetDungeonCanEnter (id, false)) 
                {
					unlockMaxIdx = i;
				}
			}
			return unlockMaxIdx;
		}



        /// <summary>
        /// 自动打开二级界面
        /// </summary>
        private void _autoOpenFrame()
        {
            _onEnterButton();
        }

        private IEnumerator _onSelectNextToggle(bool isLeft)
        {
            int len = mCurrent.GetData().chapterList.Length;
            int idx = mCurrent.index;

            if (isLeft) idx--;
            else idx++;

            idx = Mathf.Clamp(idx, 0, len - 1);

            if (idx == mCurrent.index)
            {
                yield break;
            }

            int readyId = ChapterUtility.GetReadyDungeonID(mCurrent.GetData().chapterList[idx].dungeonID);

            if (ComChapterDungeonUnit.eState.Locked == ChapterUtility.GetDungeonState(readyId))
            {
                yield break;
            }

            ChapterNormalHalfFrame normalHalfFrame = frameMgr.GetFrame(typeof(ChapterNormalHalfFrame)) as ChapterNormalHalfFrame;
            normalHalfFrame.SetMask(true);

            while (eAnimateState.onNone != mAnimateState) { yield return null; }

            mToggleList[idx].isOn = true;

            while (eAnimateState.onNone != mAnimateState) { yield return null; }

            normalHalfFrame.SetMask(false);

            if (frameMgr.IsFrameOpen<ChapterNormalHalfFrame>())
            {
                frameMgr.CloseFrame<ChapterNormalHalfFrame>();
            }

            frameMgr.OpenFrame<ChapterNormalHalfFrame>(FrameLayer.Middle, true);
        }
        public int _GetChapterIndex()
        {
            return mCurrent.GetData().nameNumberIdx;
        }
        public bool IsCanSelectLeftDungeon()
        {
            return _isCanSelectDungeon(true);
        }

        public bool IsCanSelectRightDungeon()
        {
            return _isCanSelectDungeon(false);
        }

        private bool _isCanSelectDungeon(bool isLeft)
        {
            int len = mCurrent.GetData().chapterList.Length;
            int idx = mCurrent.index;

            if (isLeft) idx--;
            else idx++;

            if (idx < 0 || idx >= len)
            {
                return false;
            }

            int readyId = ChapterUtility.GetReadyDungeonID(mCurrent.GetData().chapterList[idx].dungeonID);

            return ComChapterDungeonUnit.eState.Locked != ChapterUtility.GetDungeonState(readyId);
        }

        
        private void _onSelectNextToggle(UIEvent ui)
        {
            bool isLeft = (bool)ui.Param1;
            GameFrameWork.instance.StartCoroutine(_onSelectNextToggle(isLeft));
        }

#region UIEvent
        void _OnSelectEnterDungeon(UIEvent uiEvent)
        {
            if(uiEvent != null && uiEvent.Param1 != null && uiEvent.Param1 is int)
            {
                _onSelected((int)uiEvent.Param1, true);
            }
        }
        void _OnSceneChanged(UIEvent uiEvent)
        {
            if (uiEvent.EventParams.CurrentSceneID != sSceneID)
            {
                frameMgr.CloseFrame(this);
                mEnterWay = eEnterWay.onNone;
            }
        }
#endregion

        private void _onBind()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneChangedFinish, _OnSceneChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChapterNextDungeon, _onSelectNextToggle);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChapterNormalHalfFrameOpen, _onChapterNormalHalfOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChapterNormalHalfFrameClose, _onChapterNormalHalfClose);
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.AddSyncTaskDataChangeListener(_OnTaskChange);

            MissionManager.GetInstance().onUpdateMission += _onUpdateMission;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SelectEnterDungeon, _OnSelectEnterDungeon);
        }

        private void _onUnbind()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneChangedFinish, _OnSceneChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChapterNextDungeon, _onSelectNextToggle);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChapterNormalHalfFrameOpen, _onChapterNormalHalfOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChapterNormalHalfFrameClose, _onChapterNormalHalfClose);
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RemoveSyncTaskDataChangeListener(_OnTaskChange);

            MissionManager.GetInstance().onUpdateMission -= _onUpdateMission;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SelectEnterDungeon, _OnSelectEnterDungeon);
        }


        protected UnityEngine.Coroutine mRevertAnimate = null;

        private void _onChapterNormalHalfOpen(UIEvent ui)
        {
        }

        private void _onChapterNormalHalfClose(UIEvent ui)
        {
            if (mAnimateState != eAnimateState.onBackAnimate)
            {
                if (null != mRevertAnimate)
                {
                    GameFrameWork.instance.StopCoroutine(mRevertAnimate);
                    mRevertAnimate = null;
                }

                mRevertAnimate = GameFrameWork.instance.StartCoroutine(_revertAnimate());
            }

            if (_GetChapterIndex() == 31)
            {
                if (YiJieEffectObj != null)
                {
                    YiJieEffectObj.CustomActive(true);
                }
            }
        }

        private IEnumerator _revertAnimate()
        {
            while (mAnimateState != eAnimateState.onNone) { yield return null; }

            mAnimateState = eAnimateState.onBackAnimate;

            yield return mChapterSelectAnimate.RevertAnimate();

            mAnimateState = eAnimateState.onNone;
        }

        private void _clearAll()
        {
            for (int i = 0, icnt = mToggleList.Length; i < icnt; ++i)
            {
                Toggle curToggle = mToggleList[i];
                if (null != curToggle)
                    curToggle.onValueChanged.RemoveAllListeners();
            }

            for (int i = 0; i < mToggleGameUnitList.Length; ++i)
            {
                if (mToggleGameUnitList[i] != null)
                {

                    mToggleGameUnitList[i].SetCharactorSprite(null);
                    mToggleGameUnitList[i].SetBackgroud(null);
                    GameObject.Destroy(mToggleGameUnitList[i].gameObject);
                    mToggleGameUnitList[i] = null;
                }
            }
            mBisFlag = false;
            data = null;

            //mCurrent.node = null;
            //mCurrent.index = -1;
            //sNodes.Clear();
            //mSelectUnlock.SetVisible(false);
        }

        private DungeonTable _getDungeonTable(int id)
        {
            DungeonTable dungeonItem = TableManager.instance.GetTableItem<DungeonTable>(id);
            if (dungeonItem != null)
            {
                return dungeonItem;
            }

            Logger.LogErrorFormat("can't find item with id {0} in DungeonTable", id);
            return null;
        }

#region Toggle


        private void _onSelected(int id, bool bstate)
        {
            if (mAnimateState == eAnimateState.onSelectAnimate)
            {
                return;
            }

            if (YiJieEffectObj != null)
            {
                YiJieEffectObj.CustomActive(false);
            }

            mCurrent.index = -1;
            if (null != mSetRewardBtCD) GameFrameWork.instance.StopCoroutine(mSetRewardBtCD);
            mSetRewardBtCD = GameFrameWork.instance.StartCoroutine(_setRewardBtCD());
            if (bstate)
            {
                mCurrent.index = id;

                // 如果玩家通过任务引导打开章节界面后，手动选择了一个关卡，则sDungeonID应该替换为玩家手动选择那个关卡
                // add by qxy 2019-05-14
                if (sDungeonID <= 0 || (mEnterWay == eEnterWay.onGuide && sDungeonID > 0))
                {
                    DungeonID dungeonID = new DungeonID(sDungeonID);
                    int curDungeonID = ChapterUtility.GetReadyDungeonID(mCurrent.CurrentDungeonID());
                    if(dungeonID != null && dungeonID.dungeonIDWithOutDiff != curDungeonID)
                    {
                    sDungeonID = ChapterUtility.GetReadyDungeonID(mCurrent.CurrentDungeonID());
                    }                    
                }
                //mToggleGameUnitList[id].transform.SetAsLastSibling();

                if (null != mDelayOpen) GameFrameWork.instance.StopCoroutine(mDelayOpen);

                mEnterWay = eEnterWay.onClick;

                mDelayOpen = GameFrameWork.instance.StartCoroutine(_delayOpenButton(id));
            }

            
        }
        private UnityEngine.Coroutine mSetRewardBtCD = null;
        private UnityEngine.Coroutine mDelayOpen = null;
        #endregion

        #region Button
        /// <summary>
        /// 点击关卡后不能点击宝箱按钮，故加一个cd限制
        /// </summary>
        /// <returns></returns>
        private IEnumerator _setRewardBtCD()
        {
            if (mReward != null)
                mReward.interactable = false;

            yield return new WaitForSeconds(2f);

            if (mReward != null)
                mReward.interactable = true;
        }
        private IEnumerator _delayOpenButton(int idx)
        {
            if (sDungeonID > 0 && _canEnterDungeonByID(sDungeonID))
            {
                while (mAnimateState != eAnimateState.onNone) { yield return null; }

                mAnimateState = eAnimateState.onSelectAnimate;

                Vector3 vector3 = Vector3.zero;
                if (mToggleGameUnitList != null && mToggleGameUnitList.Length > idx && mToggleGameUnitList[idx] != null && mToggleGameUnitList[idx].thumbRoot != null)
                {
                    vector3 = mToggleGameUnitList[idx].transform.localPosition + mToggleGameUnitList[idx].thumbRoot.localPosition;
                }
                yield return mChapterSelectAnimate.NormalAnimate(vector3);

                mAnimateState = eAnimateState.onNone;
            }

            _onEnterButton();
        }

        private bool _canEnterDungeonByID(int dungeonID)
        {
            DungeonID id = new DungeonID(dungeonID);

            if (ChapterUtility.GetDungeonState(id.dungeonIDWithOutDiff) != ComChapterDungeonUnit.eState.Locked)
            {
                return true;
            }

            return false;
        }

        private void _onEnterButton()
        {
            if (sDungeonID > 0)
            {
                if (sDungeonID == ComGuidanceMainItem.LINK_BOX_INT)
                {
                    _onDungeonReward();
                    sDungeonID = -1;
                    return;
                }

                DungeonID id = new DungeonID(sDungeonID);
                mCurrent.UpdateIndex(id.dungeonIDWithOutPrestory);

                int dungeonID = mCurrent.CurrentDungeonID();

                dungeonID = ChapterUtility.GetReadyDungeonID(dungeonID);

                if (dungeonID > 0 && _canEnterDungeonByID(dungeonID) && eEnterWay.onClick == mEnterWay)
                {
                    Logger.LogProcessFormat("[ChapterNode] 打开关卡 {0}", sDungeonID);

                    _openFrame(sDungeonID);
                }
                else 
                {
                    Logger.LogErrorFormat("[ChapterNode] 妄图打开为未解锁关卡 {0}, PS: 肯定是任务的解锁和关卡的解锁不匹配，原因有2\n1. 使用不当的GM命令解锁r任务，但是对应副本么有解锁\n2.新加的主线任务在老的账号中突然出现！！", sDungeonID);
                }

                sDungeonID = -1;
            }
        }

        private void _openFrame(int dungeonID)
        {
            if (ClientSystemManager.instance.IsFrameOpen<ChapterSelectFrame>())
            {
                ChapterBaseFrame.sDungeonID = dungeonID;
                ClientSystemManager.instance.OpenFrame<ChapterNormalHalfFrame>();
                //ClientSystemManager.instance.OpenFrame<ChapterNormalFrame>();

                //DungeonUIConfigTable item = TableManager.instance.GetTableItem<DungeonUIConfigTable>(dungeonID);
                //if (null != item)
                //{
                //    System.Type type = System.Type.GetType(item.ClassName);
                //    if (null != type)
                //    {
                //        ClientSystemManager.instance.OpenFrame(type);
                //    }
                //    else
                //    {
                //        Logger.LogErrorFormat("can't find {0}", item.ClassName);
                //    }
                //}
                //else
                //{
                //    ClientSystemManager.instance.OpenFrame<ChapterNormalFrame>();
                //}
            }
        }


        private void _closeFrame()
        {
            if(sSceneID == mAnniversaryPartySceneID || sSceneID== mSpringFestivalSceneID)
            {
                ClientSystemManager.GetInstance().CloseFrame<ChapterSelectFrame>();
            }
            else
            {

                if (sSceneID < 0)
                {
                    ClientSystemManager.instance.CloseFrame(this);
                    return;
                }

                sDungeonID = -1;

                ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
                if (systemTown != null)
                {
                    systemTown.ReturnToTown();
                }
                else
                {
                    Logger.LogError("Current System is not Town!!!");
                }
            }
           
        }

        private bool _isUnlockChapter(int chapterIdx)
        {
            if (chapterIdx >= 0 && chapterIdx < sNodes.Count)
            {
                if (sNodes[chapterIdx].dungeonIDs.Count > 0)
                {
                    int dungeonID = ChapterUtility.GetReadyDungeonID(sNodes[chapterIdx].dungeonIDs[0]);
                    return (ChapterUtility.GetDungeonCanEnter(dungeonID, false));
                }
            }

            return false;
        }


        private void _updateSelectButton()
        {
            mLeftButton.gameObject.SetActive(_isUnlockChapter(selectChapterIdx - 1));
            mRightButton.gameObject.SetActive(_isUnlockChapter(selectChapterIdx + 1));
        }

        private IEnumerator _reloadChapter()
        {
            Logger.LogProcessFormat("[Chapter] selectChapterIdx {0}", selectChapterIdx);

            if (ClientSystemManager.instance.IsFrameOpen<ChapterNormalHalfFrame>())
            {
                ClientSystemManager.instance.CloseFrame<ChapterNormalHalfFrame>();
            }

            _onUnbind();
            _clearAll();
            _OnLoadPrefabFinish();
            _OnOpenFrame();

            yield return Yielders.EndOfFrame;
        }


        private void _changeChapter(int val)
        {
            selectChapterIdx = val;

            if (val == selectChapterIdx)
            {
                Logger.LogProcessFormat("[Chapter] selectChapterIdx {0}", selectChapterIdx);
                GameFrameWork.instance.StartCoroutine(_reloadChapter());
            }
        }

        private void _onLeft()
        {
            _changeChapter(selectChapterIdx-1);
        }

        private void _onRight()
        {
            _changeChapter(selectChapterIdx+1);
        }

        public enum eEnterWay
        {
            onNone,
            onClick,
            onGuide,
        }

        private eEnterWay mEnterWay = eEnterWay.onNone;

        private void _onDungeonReward()
        {
            if (IsCurrentSelectChapterShowReward())
            {
                int curChapterIdx = GetCurrentSelectChapter();
                ClientSystemManager.instance.OpenFrame<ChapterRewardFrame>(FrameLayer.Middle, curChapterIdx);
            }
        }

#region 精力燃烧
        /// <summary>
        /// 初始化疲劳燃烧是否显示
        /// </summary>
        private void _InitFatigueCombustionGameObject(GameObject mFatigueCombustionRoot)
        {
            ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.FindFatigueCombustionActivityIsOpen(ref mBisFlag, ref data);

            if (mBisFlag == true && data != null)
            {
                mFatigueCombustionRoot.CustomActive(true);
                
                _InitFatigueCombustionInfo(mFatigueCombustionRoot, data);

            }
            else
            {
                mFatigueCombustionRoot.CustomActive(false);
            }
            
        }

        /// <summary>
        /// 初始化疲劳燃烧信息
        /// </summary>
        /// <param name="go"></param>
        /// <param name="activityData"></param>
        private void _InitFatigueCombustionInfo(GameObject go, ActivityLimitTime.ActivityLimitTimeData activityData)
        {
            ComCommonBind mBind = go.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            var activityId = activityData.DataId;
            Text mTime = mBind.GetCom<Text>("Time");
            Button mOpen = mBind.GetCom<Button>("Open");
            Button mStop = mBind.GetCom<Button>("Stop");
            GameObject mOrdinaryName = mBind.GetGameObject("OrdinaryName");
            GameObject mSeniorName = mBind.GetGameObject("SeniorName");
            SetButtonGrayCD mCDGray = mBind.GetCom<SetButtonGrayCD>("CDGray");
            mOrdinaryName.CustomActive(false);
            mSeniorName.CustomActive(false);

            for (int i = 0; i < activityData.activityDetailDataList.Count; i++)
            {
                if (activityData.activityDetailDataList[i].ActivityDetailState == ActivityLimitTime.ActivityTaskState.Init ||
                    activityData.activityDetailDataList[i].ActivityDetailState == ActivityLimitTime.ActivityTaskState.UnFinish)
                {
                    continue;
                }

                mData = activityData.activityDetailDataList[i];

                var mTaskId = mData.DataId;
               
                string mStrID = mTaskId.ToString();
                string mStr = mStrID.Substring(mStrID.Length - 1);
                int mIndex = 0;

                if (int.TryParse(mStr, out mIndex))
                {
                    if (mIndex == 1)
                    {
                        mOrdinaryName.CustomActive(true);
                        mSeniorName.CustomActive(false);
                    }
                    else
                    {
                        mSeniorName.CustomActive(true);
                        mOrdinaryName.CustomActive(false);
                    }
                }
                
                mOpen.onClick.RemoveAllListeners();
                mOpen.onClick.AddListener(() =>
                {
                    ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(activityId, mTaskId);
                    mCDGray.StartGrayCD();
                });

                mStop.onClick.RemoveAllListeners();
                mStop.onClick.AddListener(() => 
                {
                    ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(activityId, mTaskId);
                    mCDGray.StartGrayCD();
                });

                _UpdateFatigueCombustionData(go, mData);
                if (activityData.activityDetailDataList[i].ActivityDetailState != ActivityLimitTime.ActivityTaskState.Failed)
                    return;
            }

           
        }

        /// <summary>
        /// 更新疲劳燃烧数据状态
        /// </summary>
        /// <param name="go"></param>
        /// <param name="activityData"></param>
        private void _UpdateFatigueCombustionData(GameObject go, ActivityLimitTime.ActivityLimitTimeDetailData activityData)
        {
            if (go == null || activityData == null)
            {
                return;
            }

            var mBind = go.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
           
            mTime = mBind.GetCom<Text>("Time");
            Button mOpen = mBind.GetCom<Button>("Open");
            Button mStop = mBind.GetCom<Button>("Stop");
            mOpen.CustomActive(false);
            mStop.CustomActive(false);
            //是否在燃烧
            mFatigueCombustionTimeIsOpen = false;
            switch (activityData.ActivityDetailState)
            {
                case ActivityLimitTime.ActivityTaskState.Init:
                case ActivityLimitTime.ActivityTaskState.UnFinish:
                    mOpen.CustomActive(true);
                    mTime.text = Function.GetLastsTimeStr(mData.DoneNum);
                    break;
                case ActivityLimitTime.ActivityTaskState.Finished:
                    mStop.CustomActive(true);
                    mFatigueCombustionTimeIsOpen = true;
                    mFatigueCombustionTime = mData.DoneNum;
                    break;
                case ActivityLimitTime.ActivityTaskState.Failed:
                    mTime.text = Function.GetLastsTimeStr(mData.DoneNum);
                    mOpen.CustomActive(true);
                    break;
            }
        }

        /// <summary>
        /// 设置疲劳燃烧的时间
        /// </summary>
        private void _SetFatigueCombustionTime()
        {
            if (mFatigueCombustionTimeIsOpen && mTime != null)
            {
                if (mFatigueCombustionTime - (int)TimeManager.GetInstance().GetServerTime() > 0)
                {
                    mTime.text = Function.GetLastsTimeStr(mFatigueCombustionTime - (int)TimeManager.GetInstance().GetServerTime());
                }
                else
                {
                    mFatigueCombustionRoot.CustomActive(false);
                }
            }
        }

        /// <summary>
        /// 疲劳燃烧活动任务变化
        /// </summary>
        private void _OnTaskChange()
        {
            //_UpdateFatigueCombustionData(mFatigueCombustionRoot, mData);
        }
        #endregion
        #endregion

        /// <summary>
        /// 周年派对界面隐藏一些UI
        /// </summary>
        private void HideSomething()
        {
            if(sSceneID == mAnniversaryPartySceneID)
            {
                // mReward.CustomActive(false);
                mChapterIndexParentGo.CustomActive(false);
                // mRedPoint.CustomActive(false);
            }
            else if(sSceneID == mSpringFestivalSceneID)
            {
                mChapterIndexParentGo.CustomActive(false);
                mRedPoint.CustomActive(false);
            }
        }
    }
}
