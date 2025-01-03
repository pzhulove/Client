using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;


namespace GameClient
{
    public class ChapterDeadFrame : ChapterBaseFrame
    {
        public sealed override string GetPrefabPath()
        {

            return "UIFlatten/Prefabs/Activity/Dungeon/ChapterDeath";
        }

        protected sealed override void _loadBg()
        {
        }


        private enum eState
        {
            /// <summary>
            /// 没有状态
            /// </summary>
            Normal,
            /// <summary>
            /// 等待消息
            /// </summary>
            Wait,
            /// <summary>
            /// 扫荡中
            /// </summary>
            Wipeout,
            /// <summary>
            /// 扫荡完成，等待领奖
            /// </summary>
            Reaward,
            Reset,
        }

        private eState mState = eState.Normal;

        private void _setRootMutex(bool flag)
        {
            mWipeoutRoot.CustomActive(flag);
            mNormalRoot.CustomActive(!flag);
        }

        private eState state
        {
            set
            {
                if (ClientSystemManager.instance.IsFrameOpen<ChapterDeadFrame>())
                {
                    mState = value;
                    switch (mState)
                    {
                        case eState.Normal:
                            _setRootMutex(false);
                            mRewardShow.SetItems(new int[0]);
                            break;
                        case eState.Wait:
                            break;
                        case eState.Wipeout:
                            _setRootMutex(true);
                            //mWipeoutRoot.SetActive(true);
                            //mCommonBoard.mRight.SetActive(true);
                            mButtonRoot.SetActive(true);
                            mOnWipeOutTittle.SetActive(true);
                            break;
                        case eState.Reaward:
                            _setRootMutex(true);
                            mButtonRoot.SetActive(false);
                            mOnWipeOutTittle.SetActive(false);
                            //mWipeoutRoot.SetActive(true);
                            //mCommonBoard.mRight.SetActive(true);
                            break;
                    }
                }
            }

            get
            {
                return mState;
            }
        }

        private List<ComChapterDeathItem> mLevels = new List<ComChapterDeathItem>();
        private int NowLevel = 0;

#region ExtraUIBind
        private ScrollRect mScroll = null;
        private RectTransform mContent = null;
        private Image mRedpoint = null;
        private Text mViptext = null;
        private Text mLeftResetCount = null;
        private ComTime mLeftTimeRemain = null;
        private Text mMyTopLevel = null;
        private ComTime mMyTopTime = null;
        private Text mBestTopLevel = null;
        private ComTime mBestTopTime = null;
        private ComItemList mRewardShow = null;
        private Text mCurrentLevelText = null;
        private Text mWipeProcessText = null;
        private Text mCostTicket = null;
        private GameObject mButtonRoot = null;
        private GameObject mTextRoot = null;
        private Button mSelectChallenge = null;
        private Button mSelectWipeoutPanle = null;
        private Button mSelectWipeout = null;
        private Button mSelctWipeoutReward = null;
        private Button mSelctWipeoutComplete = null;
        private Button mSelectReset = null;
        private GameObject mWipeoutRoot = null;
        private GameObject mNormalRoot = null;
        private GameObject mLevelRoot = null;
        private ComCommonChapterInfo mChapterInfo = null;
        private GameObject[] mNodes = new GameObject[ChapterUtility.kDeadTowerLevelCount];
        private GameObject mNormalResetRoot = null;
        private GameObject mVipResetRoot = null;
        private GameObject mTextTopLevelRoot = null;
        private GameObject mOnWipeOutTittle = null;
        private GameObject mLimitLevel = null;
        private Text mLimitLevelText = null;
        private ScrollRect mRewardScrollView = null;


        protected sealed override void _bindExUI()
        {
            mScroll = mBind.GetCom<ScrollRect>("scroll");
            mContent = mBind.GetCom<RectTransform>("content");
            mRedpoint = mBind.GetCom<Image>("redpoint");
            mViptext = mBind.GetCom<Text>("viptext");
            mLeftResetCount = mBind.GetCom<Text>("leftResetCount");
            mLeftTimeRemain = mBind.GetCom<ComTime>("leftTimeRemain");
            mMyTopLevel = mBind.GetCom<Text>("myTopLevel");
            mMyTopTime = mBind.GetCom<ComTime>("myTopTime");
            mBestTopLevel = mBind.GetCom<Text>("bestTopLevel");
            mBestTopTime = mBind.GetCom<ComTime>("bestTopTime");
            mRewardShow = mBind.GetCom<ComItemList>("rewardShow");
            mCurrentLevelText = mBind.GetCom<Text>("currentLevelText");
            mWipeProcessText = mBind.GetCom<Text>("wipeProcessText");
            mCostTicket = mBind.GetCom<Text>("costTicket");
            mButtonRoot = mBind.GetGameObject("buttonRoot");
            mTextRoot = mBind.GetGameObject("textRoot");
            mSelectChallenge = mBind.GetCom<Button>("selectChallenge");
            mSelectChallenge.onClick.AddListener(_onSelectChallengeButtonClick);
            mSelectWipeoutPanle = mBind.GetCom<Button>("selectWipeoutPanle");
            mSelectWipeoutPanle.onClick.AddListener(_onSelectWipeoutPanleButtonClick);
            mSelectWipeout = mBind.GetCom<Button>("selectWipeout");
            mSelectWipeout.onClick.AddListener(_onSelectWipeoutButtonClick);
            mSelctWipeoutReward = mBind.GetCom<Button>("selctWipeoutReward");
            mSelctWipeoutReward.onClick.AddListener(_onSelctWipeoutRewardButtonClick);
            mSelctWipeoutComplete = mBind.GetCom<Button>("selctWipeoutComplete");
            mSelctWipeoutComplete.onClick.AddListener(_onSelctWipeoutCompleteButtonClick);
            mSelectReset = mBind.GetCom<Button>("selectReset");
            mSelectReset.onClick.AddListener(_onSelectResetButtonClick);
            mWipeoutRoot = mBind.GetGameObject("wipeoutRoot");
            mNormalRoot = mBind.GetGameObject("normalRoot");
            mLevelRoot = mBind.GetGameObject("levelRoot");
            mChapterInfo = mBind.GetCom<ComCommonChapterInfo>("chapterInfo");
            for (int i = 0; i < ChapterUtility.kDeadTowerLevelCount; ++i) { mNodes[i] = mBind.GetGameObject(string.Format("node{0}", i)); }
            mNormalResetRoot = mBind.GetGameObject("normalResetRoot");
            mVipResetRoot = mBind.GetGameObject("vipResetRoot");
            mTextTopLevelRoot = mBind.GetGameObject("textTopLevelRoot");
            mOnWipeOutTittle = mBind.GetGameObject("onWipeOutTittle");
            mLimitLevel = mBind.GetGameObject("LimitLevel");
            mLimitLevelText = mBind.GetCom<Text>("LimitLevelText");
            mRewardScrollView = mBind.GetCom<ScrollRect>("RewardScrollView");
        }

        protected sealed override void _unbindExUI()
        {
            mScroll = null;
            mContent = null;
            mRedpoint = null;
            mViptext = null;
            mLeftResetCount = null;
            mLeftTimeRemain = null;
            mMyTopLevel = null;
            mMyTopTime = null;
            mBestTopLevel = null;
            mBestTopTime = null;
            mRewardShow = null;
            mCurrentLevelText = null;
            mWipeProcessText = null;
            mCostTicket = null;
            mButtonRoot = null;
            mTextRoot = null;
            mSelectChallenge.onClick.RemoveListener(_onSelectChallengeButtonClick);
            mSelectChallenge = null;
            mSelectWipeoutPanle.onClick.RemoveListener(_onSelectWipeoutPanleButtonClick);
            mSelectWipeoutPanle = null;
            mSelectWipeout.onClick.RemoveListener(_onSelectWipeoutButtonClick);
            mSelectWipeout = null;
            mSelctWipeoutReward.onClick.RemoveListener(_onSelctWipeoutRewardButtonClick);
            mSelctWipeoutReward = null;
            mSelctWipeoutComplete.onClick.RemoveListener(_onSelctWipeoutCompleteButtonClick);
            mSelctWipeoutComplete = null;
            mSelectReset.onClick.RemoveListener(_onSelectResetButtonClick);
            mSelectReset = null;
            mWipeoutRoot = null;
            mNormalRoot = null;
            mLevelRoot = null;
            mChapterInfo = null;
            for (int i = 0; i < ChapterUtility.kDeadTowerLevelCount; ++i) { mNodes[i] = null; }
            mNormalResetRoot = null;
            mVipResetRoot = null;
            mTextTopLevelRoot = null;
            mOnWipeOutTittle = null;
            mLimitLevel = null;
            mLimitLevelText = null;
            mRewardScrollView = null;
        }
#endregion    

#region Callback
        private void _onSelectChallengeButtonClick()
        {
            /* put your code in here */
            //if (NowLevel != 0 && NowLevel != 60)
            //{
            //    var DeathTowerAwardTableData = TableManager.GetInstance().GetTableItem<DeathTowerAwardTable>(NowLevel + 1);
            //    if (null == DeathTowerAwardTableData)
            //    {
            //        Logger.LogErrorFormat("DeathTowerAwardTableData is null");
            //        return;
            //    }
            //    int LimitLevel = DeathTowerAwardTableData.LimitLevel;
            //    if (0 == LimitLevel)
            //    {
            //        Logger.LogErrorFormat("LimitLevel is null");
            //        return;
            //    }
            //    if (0 != LimitLevel && LimitLevel > PlayerBaseData.GetInstance().Level)
            //    {
            //        SystemNotifyManager.SysNotifyFloatingEffect("您的等级不足，请先升级再来");
            //        return;
            //    }
            //}
            _onStartButton();
        }

        private void _onSelectWipeoutPanleButtonClick()
        {
            /* put your code in here */
            _onCloseWipeout();
        }

        private void _onSelectWipeoutButtonClick()
        {
            /* put your code in here */
            _onWipeOut();
        }

        private void _onSelctWipeoutRewardButtonClick()
        {
            /* put your code in here */
            _onWipeOutResulte();
        }

        private void _onSelctWipeoutCompleteButtonClick()
        {
            /* put your code in here */

            int cost = _getCostTicket((int)mLeftTimeInSec);

            CostItemManager.CostInfo info = new CostItemManager.CostInfo()
            {

                nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT),
                nCount = cost
            };

            CostItemManager.GetInstance().TryCostMoneyDefault(info, _onWipeOutQuickFinish);

            //_onWipeOutQuickFinish();
        }

        private void _onSelectResetButtonClick()
        {
            /* put your code in here */
            _onResetWipeOut();
            
        }
#endregion
        
        private int _getNormalLeftCount()
        {
            return CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_RESET_REMAIN_TIMES);
        }

        private int _getVipResetCount()
        {
            float totalCount = Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.WARRIOR_TOWER_REBEGIN_NUM);
            if (totalCount <= 0.0f)
            {
                return 0;
            }
            else 
            {
                return (int)totalCount - CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_VIP_TOWER_PAY_TIMES);
            }
        }

        private void _updateRedPoint()
        {
            mRedpoint.enabled = (_getNormalLeftCount() + _getVipResetCount()) > 0;
        }

        private void _updateVipText()
        {
            //float num = Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.WARRIOR_TOWER_REBEGIN_NUM);
            //if (num <= 0.0f)
            //{
            //    KeyValuePair<int, float> kv = Utility.GetFirstValidVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.WARRIOR_TOWER_REBEGIN_NUM);
            //    if (kv.Key > 0)
            //    {
            //        mViptext.text = string.Format("贵族 {0} 激活额外次数 {1} 购买重置", kv.Key, (int)kv.Value);
            //    }
            //}
            //else 
            //{
            //    int n = _getVipResetCount();
            //    mViptext.text = string.Format("贵族 {0} 剩余购买次数{1}", PlayerBaseData.GetInstance().VipLevel, n);
            //}
        }

        private void _updateResetCount()
        {
            int leftCount = _getNormalLeftCount();
            int vipLeftCount = _getVipResetCount();

            mLeftResetCount.text = string.Format("{0}", leftCount);
            mViptext.text = string.Format("{0}", vipLeftCount);

            mNormalResetRoot.SetActive(false);
            mVipResetRoot.SetActive(false);


            if (leftCount == 0 && vipLeftCount > 0)
            {
                mVipResetRoot.SetActive(true);
            }
            else
            {
                mNormalResetRoot.SetActive(true);
            }
        }

        protected UnityEngine.Coroutine mGetTopRecord = null;

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            NowLevel = 0;
            _bindEvent();

            mLeftTimeInSec = (PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime - TimeManager.GetInstance().GetServerTime());
            _updateResetCount();
            //_updateVipText();
            _updateRedPoint();


            mMyTopLevel.text = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR_TOTAL).ToString();
            mMyTopTime.SetTime(CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_USED_TIME_TOTAL));

            _updateTimeRelate();

            _updateState();

            _loadAllLevels();

            _startGetTopRecord();

            _updateChallengeBtnState();

            _updateScrollRectPosition();
        }

        protected void _stopGetTopRecord()
        {
            if (null != mGetTopRecord) 
            { 
                GameFrameWork.instance.StopCoroutine(mGetTopRecord);
            }
            mGetTopRecord = null;
        }

        protected void _startGetTopRecord()
        {
            _stopGetTopRecord();
            mGetTopRecord = GameFrameWork.instance.StartCoroutine(_getTopRecord());
        }

        protected void _updateChallengeBtnState()
        {
            mLimitLevel.CustomActive(false);
            if (NowLevel != 0 && NowLevel != 60)
            {
                var DeathTowerAwardTableData = TableManager.GetInstance().GetTableItem<DeathTowerAwardTable>(NowLevel + 1);
                if (null == DeathTowerAwardTableData)
                {
                    return;
                }
                int LimitLevel = DeathTowerAwardTableData.LimitLevel;
                if (0 == LimitLevel)
                {
                    return;
                }
                if (0 != LimitLevel && LimitLevel > PlayerBaseData.GetInstance().Level)
                {
                    //SystemNotifyManager.SysNotifyFloatingEffect("您的等级不足，请先升级再来");
                    mLimitLevel.CustomActive(true);
                    mLimitLevelText.text = string.Format("{0}级开启", LimitLevel);
                    return;
                }
            }
        }

        private void _updateScrollRectPosition()
        {
            float scrollPositon = NowLevel * 1.0f / 5 / 14;
            if(scrollPositon > 1.0f)
            {
                scrollPositon = 1.0f;
            }
            mRewardScrollView.verticalNormalizedPosition = NowLevel * 1.0f / 5 / 14;
        }

        protected IEnumerator _getTopRecord()
        {
            WorldSortListReq req = new WorldSortListReq()
            {
                type = SortListDecoder.MakeSortListType(SortListType.SORTLIST_TOWER),
                start = 0,
                num = 1
            };

            MessageEvents msg = new MessageEvents();
            WorldSortListRet res = new WorldSortListRet();

            mBestTopLevel.text = "";
            mBestTopTime.SetTime(0);

            yield return MessageUtility.Wait<WorldSortListReq, WorldSortListRet>(ServerType.GATE_SERVER, msg, req, res);

            if (msg.IsAllMessageReceived())
            {
                var data = msg.GetMessageData(res.GetMsgID());
                int pos = 0;
                BaseSortList arrRecods = SortListDecoder.Decode(data.bytes, ref pos, data.bytes.Length);

                if (arrRecods.entries.Count > 0)
                {
                    DeathTowerSortListEntry en = arrRecods.entries[0] as DeathTowerSortListEntry;
                    if (null != en)
                    {
                        mBestTopLevel.text = en.layer.ToString();
                        mBestTopTime.SetTime((int)en.usedTime);
                    }
                }
                _updateChallengeBtnState();
            }
        }


        private void _updateTimeRelate()
        {
            mLeftTimeRemain.SetTime((int)mLeftTimeInSec * 1000);// ; Utility.SecondsToTimeText(PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime);
            mCostTicket.text = _getCostTicket((int)mLeftTimeInSec).ToString();
        }

        private void _loadAllLevels()
        {
            mLevels.Clear();

            GameObject root = mLevelRoot;


            string unitpath = mBind.GetPrefabPath("level");

            for (int i = 0; i < ChapterUtility.kDeadTowerLevelCount; ++i)
            {
                GameObject item = AssetLoader.instance.LoadResAsGameObject(unitpath);

                Utility.AttachTo(item, mNodes[i]);

                //item.transform.SetAsFirstSibling();

                ComChapterDeathItem com = item.GetComponent<ComChapterDeathItem>();
                com.SetIndex(i);

                List<AwardItemData> list = new List<AwardItemData>();
                int st = i * ChapterUtility.kDeadTowerLevelSplit + 1;
                for (int j = st; j < st + ChapterUtility.kDeadTowerLevelSplit; ++j)
                {
                    DeathTowerAwardTable t = TableManager.instance.GetTableItem<DeathTowerAwardTable>(j);
                    if (null != t && t.AwardItem > 0)
                    {
                        AwardItemData citem = new AwardItemData();

                        if (null != citem)
                        {
                            citem.Num = t.AwardItemNum;
                            citem.ID = (int)t.AwardItem;
                        }

                        list.Add(citem);
                    }
                }

                com.SetMask(CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_AWARD_MASK));

                int idx = i;
                com.SetClick((ComChapterDeathItem.eState state)=> 
                {
                    switch (state)
                    {
                        case ComChapterDeathItem.eState.Pass:
                            {
                                _sendRewardReq(idx);
                            }
                            break;
                        case ComChapterDeathItem.eState.Unlock:
                        case ComChapterDeathItem.eState.Lock:
                            {
                                ActiveAwardFrameData data = new ActiveAwardFrameData
                                {
                                    title = "首次通关奖励",
                                    datas = list
                                };

                                ClientSystemManager.GetInstance().OpenFrame<ActiveAwardFrame>(FrameLayer.Middle, data);
                            }
                            break;
                    }
                });

                mLevels.Add(com);
            }

            _updateCurrentLevel();

        }

        private bool _isTop()
        {
            return CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR) == ChapterUtility.kDeadTowerTopLevel;
        }

        private void _updateCurrentLevel()
        {
            int topf = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR);
            int topfa = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR_TOTAL);
            int cnt = topf / ChapterUtility.kDeadTowerLevelSplit;
            int toplevel = topfa / ChapterUtility.kDeadTowerLevelSplit;
            int unlockLevel = toplevel + 1;

			LayoutRebuilder.ForceRebuildLayoutImmediate (mContent);

            for (int i = 0; i < ChapterUtility.kDeadTowerLevelCount; i++)
            {
                mLevels[i].SetSelect(false);
				int realIdx = i + 1;

				if (unlockLevel == realIdx)
                {
                    mLevels[i].SetState(ComChapterDeathItem.eState.Unlock);
                }
                else
                {
					if (realIdx <= toplevel)
                    {
                        mLevels[i].SetState(ComChapterDeathItem.eState.Pass);
                    }
                    else
                    {
                        mLevels[i].SetState(ComChapterDeathItem.eState.Lock);
                    }
                }

                if (i == cnt)
                {
                    mLevels[i].SetSelect(true);
                    NowLevel = i * 5;
                    mCurrentLevelText.text = string.Format("{0}-{1}", i * 5 + 1, i * 5 + 5);
                    mWipeProcessText.text = string.Format("{0}/{1}", topf, topfa);
                    _updateChallengeBtnState();
                }
            }

            mTextRoot.SetActive(true);
            mTextTopLevelRoot.SetActive(false);

            if (cnt == (ChapterUtility.kDeadTowerLevelCount))
            {
                mCurrentLevelText.text = "";
                mTextRoot.SetActive(false);
                mTextTopLevelRoot.SetActive(true);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();

            _unbindEvent();

            _stopGetTopRecord();
        }

        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        private void _sendRewardReq(int f)
        {
            SceneTowerFloorAwardReq req = new SceneTowerFloorAwardReq
            {
                floor = (uint)(f * ChapterUtility.kDeadTowerLevelSplit + ChapterUtility.kDeadTowerLevelSplit)
            };
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        private float mTickTime = 0.0f;
        protected sealed override void _OnUpdate(float timeElapsed)
        {
            if (state == eState.Wipeout)
            {
                mTickTime += Time.unscaledDeltaTime;

                if (mTickTime > 1.0f)
                {
                    mTickTime -= 1.0f;

                    if (mLeftTimeInSec > 0)
                    {
                        mLeftTimeInSec--;

                        _updateTimeRelate();
                        _updateState();
                    }
                }
            }
        }


        private void _updateState()
        {
            uint endtime = PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime;
            uint servertime = TimeManager.GetInstance().GetServerTime();

            if (endtime == 0)
            {
                state = eState.Normal;
            }
            else
            {
                if (endtime > servertime)
                {
                    state = eState.Wipeout;
                }
                else
                {
                    state = eState.Reaward;
                    _onWipeOutResulte();
                }
            }
        }

        protected sealed override void _loadLeftPanel()
        {
            if (null != mChapterInfo)
            {
                var com = mChapterInfo;
                mChapterInfoCommon    = com;
                mChapterInfoDiffculte = com;
                mChapterInfoDrops     = com;
                mChapterPassReward    = com;
                mChapterScore         = com;
                mChapterMonsterInfo   = com;
                mChapterActivityTimes = com;
            }

            mChapterInfoDiffculte.SetDiffculte(mChapterInfoDiffculte.GetDiffculte(),mDungeonID.dungeonID);

            
        }


        protected override void _updateDropInfo()
        {
            base._updateDropInfo();

            // sendmsg
            SceneTowerWipeoutQueryResultReq msg = new SceneTowerWipeoutQueryResultReq();
            int topf = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR);
            int cnt = topf / ChapterUtility.kDeadTowerLevelSplit;
            msg.beginFloor = (uint)cnt * 5 +1;
            msg.endFloor = (uint)CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR_TOTAL);
            NetManager netMgr = NetManager.Instance();
            if (msg.endFloor >= msg.beginFloor)
            {
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
            

            WaitNetMessageManager.GetInstance().Wait<SceneTowerWipeoutQueryResultRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }
                List<ComItemList.Items> tempItems = new List<ComItemList.Items>();
                for(int i=0;i<msgRet.items.Length;i++)
                {
                    bool haveElement = false;
                    for(int j = 0;j<tempItems.Count;j++)
                    {
                        if((int)msgRet.items[i].typeId == tempItems[j].id)
                        {
                            tempItems[j].count += msgRet.items[i].num;
                            haveElement = true;
                        }
                    }
                    if(!haveElement)
                    {
                        ComItemList.Items item = new ComItemList.Items();
                        item.count = msgRet.items[i].num;
                        item.id = (int)msgRet.items[i].typeId;

                        tempItems.Add(item);
                    }
                }
                mChapterInfoDrops.UpdateDropCount(tempItems);

            }, false);
            // on msg recv
            
        }


        //



        private void _bindEvent( )
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDeadTowerWipeoutTimeChange, _onDeadTowerWipeoutTimeChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _onCountValueChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, UpdateLevel);
        }

        private void _unbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDeadTowerWipeoutTimeChange, _onDeadTowerWipeoutTimeChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _onCountValueChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, UpdateLevel);
        }

        private uint mLeftTimeInSec = 0;

        private void _onCountValueChange(UIEvent ui)
        {
            string key = ui.Param1 as string;


            if (key == CounterKeys.COUNTER_TOWER_RESET_REMAIN_TIMES)
            {
                _updateResetCount();
                //_updateVipText();
                _updateRedPoint();
            }
            else if (key == CounterKeys.COUNTER_TOWER_TOP_FLOOR_TOTAL)
            {
                mMyTopLevel.text = CountDataManager.GetInstance().GetCount(key).ToString();
                _updateCurrentLevel();
                _updateDropInfo();
            }
            else if (key == CounterKeys.COUNTER_TOWER_TOP_FLOOR)
            {
                _updateCurrentLevel();
                _updateDropInfo();
            }
            else if (key == CounterKeys.COUNTER_TOWER_USED_TIME_TOTAL)
            {
                mMyTopTime.SetTime(CountDataManager.GetInstance().GetCount(key));
            }
            else if (key == CounterKeys.COUNTER_TOWER_AWARD_MASK)
            {
                for (int i = 0; i < mLevels.Count; ++i)
                {
                    mLevels[i].SetMask(CountDataManager.GetInstance().GetCount(key));
                }
                _updateCurrentLevel();
            }
            else if (key == CounterKeys.COUNTER_VIP_TOWER_PAY_TIMES)
            {
                _updateResetCount();
                //_updateVipText();
                _updateRedPoint();
            }

        }
        private void UpdateLevel(UIEvent uievent)
        {
            _UpdateAllLevels();
        }

        private void _UpdateAllLevels()
        {
            GameObject root = mLevelRoot;
            string unitpath = mBind.GetPrefabPath("level");
            for (int i = 0; i < ChapterUtility.kDeadTowerLevelCount; ++i)
            {
                //GameObject item = AssetLoader.instance.LoadResAsGameObject(unitpath);

                //Utility.AttachTo(item, mNodes[i]);

                ComCommonBind item = mNodes[i].GetComponentInChildren<ComCommonBind>();
                if(item != null)
                {
                    ComChapterDeathItem com = item.gameObject.GetComponent<ComChapterDeathItem>();
                    com.UpdateLimitLevel(i);
                }

                //item.transform.SetAsFirstSibling();

                
            }

        }
        private int _getCostTicket(int leftSecond)
        {
            SystemValueTable value0 = TableManager.instance.GetTableItem<SystemValueTable>(19);
            SystemValueTable value = TableManager.instance.GetTableItem<SystemValueTable>(20);

            if (value != null)
            {
                int v = leftSecond / value0.Value + (leftSecond % value0.Value > 0 ? 1 : 0);
                return v * value.Value;
            }

            return 0;
        }

        private void _onDeadTowerWipeoutTimeChange(UIEvent ui)
        {
            //Logger.LogErrorFormat("{0}, {1}", PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime, TimeManager.GetInstance().GetServerTimeStamp());
            mLeftTimeInSec = 0;
            
            if (PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime > TimeManager.GetInstance().GetServerTime())
            {
                mLeftTimeInSec = PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime - TimeManager.GetInstance().GetServerTime();
                _updateTimeRelate();
                //mLeftTimeRemain.SetTime((int)(mLeftTimeInSec * 1000));// ; Utility.SecondsToTimeText(PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime);
                //mCostTicket.text = _getCostTicket((int)mLeftTimeInSec).ToString();
            }
            else
            {
                mLeftTimeRemain.SetTime(0);
                mCostTicket.text = "";
            }

            _updateState();
        }

        private IEnumerator _onWipeOutIter()
        {
            var req = new SceneTowerWipeoutReq();
            var res = new SceneTowerWipeoutRes();
            var msg = new MessageEvents();

            yield return MessageUtility.Wait<SceneTowerWipeoutReq, SceneTowerWipeoutRes>(ServerType.GATE_SERVER, msg, req, res);

            state = eState.Normal;

            if (msg.IsAllMessageReceived())
            {
                if (res.result != 0)
                {
                    SystemNotifyManager.SystemNotify((int)res.result);
                }
                else
                {
                    state = eState.Wipeout;
                }
            }
        }
        private IEnumerator _onWipeOutResulteIter()
        {
            var req = new SceneTowerWipeoutResultReq();
            var res = new SceneTowerWipeoutResultRes();
            var msg = new MessageEvents();

            yield return MessageUtility.Wait<SceneTowerWipeoutResultReq, SceneTowerWipeoutResultRes>(ServerType.GATE_SERVER, msg, req, res);

            state = eState.Reaward;

            if (msg.IsAllMessageReceived())
            {
                if (res.result != 0)
                {
                    SystemNotifyManager.SystemNotify((int)res.result);
                }
                else
                {
                    state = eState.Reset;

                    List<ComItemList.Items> items = new List<ComItemList.Items>();

                    for (int i = 0; i < res.items.Length; ++i)
                    {
                        ComItemList.Items it = items.Find(x => { return x.id == (int)res.items[i].typeId; });
                        if (it != null)
                        {
                            it.count += res.items[i].num;
                        }
                        else
                        {
                            ComItemList.Items ie = new ComItemList.Items
                            {
                                count = res.items[i].num,
                                type = ComItemList.eItemType.Custom,
                                id = (int)res.items[i].typeId,
                                flag = ComItemList.eItemExtraFlag.Normal
                            };
                            items.Add(ie);
                        }
                    }
                    if(mRewardShow!=null)
                    mRewardShow.SetItems(items.ToArray());

                    //List<AwardItemData> awardItemData = new List<AwardItemData>();
                    for (int i = 0; i < items.Count; ++i)
                    {
                        //                         AwardItemData data;
                        //                         data.ID = items[i].id;
                        //                         data.Num = (int)items[i].count;
                        //                         awardItemData.Add(data);

                        ItemTable TableData = TableManager.GetInstance().GetTableItem<ItemTable>(items[i].id);
                        if (TableData == null)
                        {
                            continue;
                        }

                        string str = string.Format("{0} * {1}", TableData.Name, items[i].count);
                        SystemNotifyManager.SysNotifyFloatingEffect(str, CommonTipsDesc.eShowMode.SI_QUEUE, items[i].id);
                    }

                    //SystemNotifyManager.CreateSysNotifyCommonItemAwardFrame().SetAwardItems(awardItemData);
                }
            }
        }
        private IEnumerator _onWipeOutQuickFinisheIter()
        {
            var req = new SceneTowerWipeoutQuickFinishReq();
            var res = new SceneTowerWipeoutQuickFinishRes();
            var msg = new MessageEvents();

            yield return MessageUtility.Wait<SceneTowerWipeoutQuickFinishReq, SceneTowerWipeoutQuickFinishRes>(ServerType.GATE_SERVER, msg, req, res);

            state = eState.Wipeout;

            if (msg.IsAllMessageReceived())
            {
                if (res.result != 0)
                {
                    SystemNotifyManager.SystemNotify((int)res.result);
                }
                else
                {
                    state = eState.Reaward;
                    _onWipeOutResulte();
                }
            }
        }


        private enum eResetType
        {
            /// <summary>
            /// 无法重置
            /// </summary>
            CannotReset,
            /// <summary>
            /// 普通重置次数
            /// </summary>
            Normal,
            /// <summary>
            /// 购买重置次数
            /// </summary>
            Vip,
        }

        private eResetType _getResetType()
        {
            if (_canResetCount())
            {
                return eResetType.Normal;
            }

            if (_canBuyReset())
            {
                return eResetType.Vip;
            }


            return eResetType.CannotReset;
        }


        private bool _canResetCount()
        {
            return _getNormalLeftCount() > 0;
        }

        private bool _canBuyReset()
        {
            return _getVipResetCount() > 0;
        }

        private IEnumerator _onWipeOutReseteIter()
        {
            var req = new SceneTowerResetReq();
            var res = new SceneTowerResetRes();
            var msg = new MessageEvents();

            yield return MessageUtility.Wait<SceneTowerResetReq, SceneTowerResetRes>(ServerType.GATE_SERVER, msg, req, res);

            state = eState.Normal;

            if (msg.IsAllMessageReceived())
            {
                if (res.result != 0)
                {
                    SystemNotifyManager.SystemNotify((int)res.result);
                }
                else
                {
                }
            }
        }

        [ProtocolHandle(typeof(SceneTowerFloorAwardRes))]
        private void _onSceneTowerFloorAwardRes(SceneTowerFloorAwardRes res)
        {
            if (0 != res.result)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
            }
            else
            {
                for (int i = 0; i < res.items.Length; ++i)
                {
                    ItemTable TableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)res.items[i].typeId);
                    if (TableData == null)
                    {
                        continue;
                    }

                    string str = string.Format("{0} * {1}", TableData.Name, res.items[i].num);
                    SystemNotifyManager.SysNotifyFloatingEffect(str, CommonTipsDesc.eShowMode.SI_QUEUE, (int)res.items[i].typeId);
                }
            }
        }

        #region 按钮

        //[UIEventHandle("Challenge")]
        private void _onStartButton()
        {
            if (!_isTop())
            {
                GameFrameWork.instance.StartCoroutine(_commonStart());
            }
            else 
            {
                SystemNotifyManager.SystemNotify(5019);
            }
        }


        //[UIEventHandle("WipeoutPanle")]
        private void _onCloseWipeout()
        {
            if (state == eState.Reset)
            {
                state = eState.Normal;
            }
            else if (state == eState.Wipeout)
            {
                //ClientSystemManager.instance.CloseFrame(this);
            }
        }

        //[UIEventHandle("Wipeout")]
        private void _onWipeOut()
        {
            SystemNotifyManager.SystemNotify(5003, () =>
            {
                if (state == eState.Normal)
                {
                    state = eState.Wait;
                    GameFrameWork.instance.StartCoroutine(_onWipeOutIter());
                }
                else
                {
                    Logger.LogErrorFormat("错误状态 {0}", state);
                }
            });
        }

        //[UIEventHandle("WipeoutPanle/Rt/BR/Reward")]
        private void _onWipeOutResulte()
        {
            if (state == eState.Reaward)
            {
                state = eState.Wait;
                GameFrameWork.instance.StartCoroutine(_onWipeOutResulteIter());
            }
            else
            {
                Logger.LogErrorFormat("错误状态 {0}", state);
            }
        }

        //[UIEventHandle("WipeoutPanle/Rt/BR/Complete")]
        private void _onWipeOutQuickFinish()
        {
            if (state == eState.Wipeout)
            {
                state = eState.Wait;
                GameFrameWork.instance.StartCoroutine(_onWipeOutQuickFinisheIter());
            }
            else
            {
                Logger.LogErrorFormat("错误状态 {0}", state);
            }
        }

        private void _onResetWipeOutCB()
        {
            if (state == eState.Normal)
            {
                state = eState.Wait;
                GameFrameWork.instance.StartCoroutine(_onWipeOutReseteIter());
            }
            else
            {
                Logger.LogErrorFormat("错误状态 {0}", state);
            }

        }

        //[UIEventHandle("Reset")]
        private void _onResetWipeOut()
        {
            eResetType type = _getResetType();

            switch (type)
            {
                case eResetType.CannotReset:
                    SystemNotifyManager.SystemNotify(5014);
                    break;
                case eResetType.Normal:
                    SystemNotifyManager.SystemNotify(5004, _onResetWipeOutCB);
                    break;
                case eResetType.Vip:
                    {
                        ulong cost = 0;

                        SystemValueTable table = TableManager.instance.GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIP_RESET_TOWER_COST);

                        if (null != table)
                        {
                            cost = (ulong)table.Value;
                        }

                        CostItemManager.CostInfo info = new CostItemManager.CostInfo
                        {
                            nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT),
                            nCount = (int)cost
                        };

                        CostItemManager.GetInstance().TryCostMoneyDefault(info, _onResetWipeOutCB);

                        //Utility.TryCostMoneyDefault(CostInfo a_costInfo, Action a_delDefaultCanCost);
                        //if (PlayerBaseData.GetInstance().CanUseTicket(cost))
                        //{
                        //    SystemNotifyManager.SystemNotify(5013, _onResetWipeOutCB, null, new object[] {cost});
                        //}
                        //else 
                        //{
                        //    SystemNotifyManager.SystemNotify(1048);
                        //}
                    }
                    break;

            }
        }
        #endregion
    }
}
