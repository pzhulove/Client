using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using System;
using DG.Tweening;
using Scripts.UI;

namespace GameClient
{
    public class ChapterRewardFrame : ClientFrame
    {
        List<int> chapterIDList = new List<int>();
        Toggle[] chapterToggleList = new Toggle[0];
        Dictionary<string, Toggle> toggles = new Dictionary<string, Toggle>();
        GameObject[] rewardBoxArr = new GameObject[3];
        private int curSelectToggle = 0;
        private int maxSCount = 0;
        private int defaultChapterIdx = 1;
        List<MissionManager.SingleMissionInfo> allRewardList;
        List<bool> isChapterLockList = new List<bool>();
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/SelectReward/ChapterSelectRewardFrame.prefab";
        }

#region ExtraUIBind
        private Button mClose = null;
        private Text mFinishcount = null;
        private Text mSumcount = null;
        private GameObject mContent = null;
        private Slider mProcess = null;
        //private Text mChapterNum = null;
        private ComUIListScript mToggleUIList = null;
        private GameObject mChapterSelectRewardBox1 = null;
        private GameObject mChapterSelectRewardBox2 = null;
        private GameObject mChapterSelectRewardBox3 = null;
        private Text mScoreCount = null;
        private ComUIListScript mTaskItemComUIList = null;
        private Button mChapterReward = null;
        private Text mChapterRewardText = null;
        private UIGray mCharpterRewardGray = null;
        private GameObject mChapterRewardRoot = null;
        private Image mChapterRewardBoxStatus = null;
        private Button mChapterRewardBox = null;
        private Button mChapterRewardBoxBg = null;
        private Button mChapterRewardBoxClose = null;
        private GameObject mChapterRewardRed = null;
        private GameObject mEffectRoot = null;
        private DOTweenAnimation mEffectAnimate = null;
        private ComItemList mAllreward = null;
        private ScrollRect mTaskScroll = null;

        private ComUIListScript mToggleComUIList = null;
        private Image mChapterIcon = null;
        private Text mChapterText = null;
        private ComUIListScript mPandectUIList = null;
        private GameObject mPandectroot = null;
        private GameObject mRightroot = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mFinishcount = mBind.GetCom<Text>("finishcount");
            mSumcount = mBind.GetCom<Text>("sumcount");
            mContent = mBind.GetGameObject("content");
            mProcess = mBind.GetCom<Slider>("process");
            //mChapterNum = mBind.GetCom<Text>("chapterNum");
            mToggleUIList = mBind.GetCom<ComUIListScript>("ToggleUIList");
            mChapterSelectRewardBox1 = mBind.GetGameObject("ChapterSelectRewardBox1");
            mChapterSelectRewardBox2 = mBind.GetGameObject("ChapterSelectRewardBox2");
            mChapterSelectRewardBox3 = mBind.GetGameObject("ChapterSelectRewardBox3");
            mScoreCount = mBind.GetCom<Text>("ScoreCount");
            mTaskItemComUIList = mBind.GetCom<ComUIListScript>("TaskItemComUIList");
            mChapterReward = mBind.GetCom<Button>("chapterReward");
            mChapterReward.onClick.AddListener(_onChapterRewardButtonClick);
            mChapterRewardText = mBind.GetCom<Text>("chapterRewardText");
            mCharpterRewardGray = mBind.GetCom<UIGray>("charpterRewardGray");
            mChapterRewardRoot = mBind.GetGameObject("chapterRewardRoot");
            mChapterRewardBoxStatus = mBind.GetCom<Image>("chapterRewardBoxStatus");
            mChapterRewardBox = mBind.GetCom<Button>("chapterRewardBox");
            mChapterRewardBox.onClick.AddListener(_onChapterRewardBoxButtonClick);
            mChapterRewardBoxBg = mBind.GetCom<Button>("chapterRewardBoxBg");
            mChapterRewardBoxBg.onClick.AddListener(_onChapterRewardBoxBgButtonClick);
            mChapterRewardBoxClose = mBind.GetCom<Button>("chapterRewardBoxClose");
            mChapterRewardBoxClose.onClick.AddListener(_onChapterRewardBoxCloseButtonClick);
            mChapterRewardRed = mBind.GetGameObject("chapterRewardRed");
            mEffectRoot = mBind.GetGameObject("effectRoot");
            mEffectAnimate = mBind.GetCom<DOTweenAnimation>("effectAnimate");
            mAllreward = mBind.GetCom<ComItemList>("allreward");
            mTaskScroll = mBind.GetCom<ScrollRect>("TaskScroll");

            mToggleComUIList = mBind.GetCom<ComUIListScript>("Toggle");
            mChapterIcon = mBind.GetCom<ImageEx>("ChapterIcon");
            mChapterText = mBind.GetCom<TextEx>("ChapterText");
            mPandectUIList = mBind.GetCom<ComUIListScript>("PandectUIList");
            mPandectroot = mBind.GetGameObject("Pandectroot");
            mRightroot = mBind.GetGameObject("Right");
    }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mFinishcount = null;
            mSumcount = null;
            mContent = null;
            mProcess = null;
            //mChapterNum = null;
            mToggleUIList = null;
            mChapterSelectRewardBox1 = null;
            mChapterSelectRewardBox2 = null;
            mChapterSelectRewardBox3 = null;
            mScoreCount = null;
            mTaskItemComUIList = null;
            mChapterReward.onClick.RemoveListener(_onChapterRewardButtonClick);
            mChapterReward = null;
            mChapterRewardText = null;
            //mChapterNum = null;
            mChapterRewardRoot = null;
            mChapterRewardBoxStatus = null;
            mChapterRewardBox.onClick.RemoveListener(_onChapterRewardBoxButtonClick);
            mChapterRewardBox = null;
            mChapterRewardBoxBg.onClick.RemoveListener(_onChapterRewardBoxBgButtonClick);
            mChapterRewardBoxBg = null;
            mChapterRewardBoxClose.onClick.RemoveListener(_onChapterRewardBoxCloseButtonClick);
            mChapterRewardBoxClose = null;
            mChapterRewardRed = null;
            mEffectRoot = null;
            mEffectAnimate = null;
            mAllreward = null;
            mTaskScroll = null;

            mToggleComUIList = null;
            mChapterIcon = null;
            mChapterText = null;
            mPandectUIList = null;
            mPandectroot = null;
            mRightroot = null;
        }
#endregion   
 

#region Callback
        private void _onCloseButtonClick()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }
        private void _onChapterRewardButtonClick()
        {
            _onChapterRewardButtonClickByIdx(mCurSelectChapterIdx);
        }

        private void _onChapterRewardBoxButtonClick()
        {
            /* put your code in here */
            mChapterRewardRoot.CustomActive(true);
        }

        private void _onChapterRewardBoxBgButtonClick()
        {
            mChapterRewardRoot.CustomActive(false);
        }
        private void _onChapterRewardBoxCloseButtonClick()
        {
            mChapterRewardRoot.CustomActive(false);
        }
        #endregion

        private void _onChapterRewardButtonClickByIdx(int chidx)
        {
            List<MissionManager.SingleMissionInfo> allReward = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Chest, chidx);
            if (null != allReward && allReward.Count > 0)
            {
                if (allReward[0].status == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    MissionManager.GetInstance().sendCmdSubmitTask(
                            (UInt32)allReward[0].missionItem.ID, 
                            Protocol.TaskSubmitType.TASK_SUBMIT_AUTO,
                            0);
                }
            }
        }

        private class Node : IComparable<Node>
        {
            public ComCommonBind bind;

            public MissionManager.SingleMissionInfo info;

#region IComparable<Node>
            public int CompareTo(GameClient.ChapterRewardFrame.Node other)
            {
                return info.CompareTo(other.info);
            }
#endregion
        }
        
        private int mCurSelectChapterIdx = 1;
   
        IEnumerator MoveToChapter(int id)
        {
            ClientSystemManager.GetInstance().CloseFrame<ChapterRewardFrame>();
            if(ClientSystemManager.GetInstance().IsFrameOpen<ChapterNormalHalfFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChapterNormalHalfFrame>();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChapterNormalHalfFrameClose);
            }
            yield return new WaitForSeconds(0.5f);
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown != null)
            {
                systemTown.ReturnToTown();
            }
            yield return new WaitForSeconds(2f);
            MissionManager.GetInstance().AutoTraceTask(id);
        }
        private void _unloadChapter()
        {
            MissionManager.GetInstance().onUpdateMission -= _onUpdateMission;
        }

        protected override void _OnOpenFrame()
        {
            rewardBoxArr[0] = mChapterSelectRewardBox1;
            rewardBoxArr[1] = mChapterSelectRewardBox2;
            rewardBoxArr[2] = mChapterSelectRewardBox3;
            defaultChapterIdx = 1;

            try 
            {
                defaultChapterIdx = (int)userData;
            }
            catch (Exception e)
            {

            }
            curSelectToggle = defaultChapterIdx;
            _InitData();
            _InitChapterList();
            _InitComUIList();
            _UpdateChapterList();
            
            
        }
        void _InitData()
        {
            MissionManager.GetInstance().onUpdateMission += _onUpdateMission;
            chapterIDList.Clear();
            //chapterToggleList.Clear();
            //Array.Clear(chapterToggleList);
            //GameObject[] rewardBoxArr = new GameObject[3];
            rewardBoxArr[0] = mChapterSelectRewardBox1;
            rewardBoxArr[1] = mChapterSelectRewardBox2;
            rewardBoxArr[2] = mChapterSelectRewardBox3;
        }
        void _InitChapterList()
        {
            chapterIDList.Clear();
            var chapterList = TableManager.GetInstance().GetTable<ChapterTable>();
            var enumerator = chapterList.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var chapterItemData = enumerator.Current.Value as ChapterTable;
                if(chapterItemData.RewardIsOpen == 1)
                {
                    chapterIDList.Add(chapterItemData.ID);
                }
            }
            chapterToggleList = new Toggle[chapterIDList.Count];
        }

        void _UpdateChapterList()
        {
            Toggle toggle = null;
            //mToggleUIList.SetElementAmount(chapterIDList.Count);
            mToggleComUIList.SetElementAmount(chapterIDList.Count);            
            for (int i = 0; i < chapterIDList.Count; i++)
            {
                if (chapterIDList[i] == curSelectToggle)
                {
                    mToggleComUIList.MoveElementInScrollArea(i,true);
                    toggles.TryGetValue(TableManager.GetInstance().GetTableItem<ChapterTable>(i).ChapterName, out toggle);
                    if (toggle == null)
                    {
                        if (chapterIDList[i] == 0)
                        {
                            continue;
                        }
                        mCurSelectChapterIdx = chapterIDList[i];
                        allRewardList = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chapterIDList[i]);
                        allRewardList.Sort(SortList);
                        mTaskItemComUIList.SetElementAmount(allRewardList.Count);
                        _updateProcess(chapterIDList[i]);
                        _initAllReward(chapterIDList[i]);
                        //mChapterNum.text = chapterIDList[i].ToString();

                        //mCurSelectChapterIdx = chapterIDList[item.m_index];
                        //allRewardList = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chapterIDList[item.m_index]);
                        //allRewardList.Sort(SortList);
                        //mTaskItemComUIList.SetElementAmount(allRewardList.Count);
                        //_updateProcess(chapterIDList[item.m_index]);
                        //_initAllReward(chapterIDList[item.m_index]);
                        //_updateAllRewardStaus(chapterIDList[item.m_index]);
                        //mChapterNum.text = chapterIDList[item.m_index].ToString();
                        //mTaskScroll.verticalNormalizedPosition = 1.0f;

                        //mToggleComUIList.MoveElementInScrollArea(i, true);
                        //mToggleComUIList.ScrollToItem(i);
                        //mToggleComUIList.SelectElement(i);
                    }
                    else
                    {
                        toggle.isOn = true;
                    }
                }
            }
        }
        void _InitComUIList()
        {
            mToggleComUIList.Initialize();
            mToggleComUIList.onItemVisiable = _OnContentItemVisiableDelegate;
            mToggleComUIList.onItemSelected = _OnContentItemSelectDelegate;
            mToggleComUIList.OnItemRecycle = _OnContentItemRecycleDelegate;

            mTaskItemComUIList.Initialize();
            mTaskItemComUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateTaskBind(item);
                }
            };
            mTaskItemComUIList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    Button getbutton = combind.GetCom<Button>("getbutton");
                    if(getbutton != null)
                    {
                        getbutton.onClick.RemoveAllListeners();
                    }
                    Button mGoChapter = combind.GetCom<Button>("GoChapter");
                    if(mGoChapter != null)
                    {
                        mGoChapter.onClick.RemoveAllListeners();
                    }
                }
            };
            mPandectUIList.Initialize();
            mPandectUIList.onItemVisiable = (item) =>
            {
                _UpdatePandectList(item);
            };
            mPandectUIList.OnItemRecycle = (item) =>
            {
                _PandectListRecycle(item);
            };
        }

        void _UpdateBind(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            if (item.m_index < 0)
            {
                return;
            }
            if (chapterIDList.Count <= 0)
            {
                return;
            }
            var mToggle = mBind.GetCom<Toggle>("Func");
            var mTabText = mBind.GetCom<Text>("TabText");
            var mSelect = mBind.GetGameObject("Select");
            var mFinished = mBind.GetGameObject("Finished");
            var mNoOpen = mBind.GetGameObject("NoOpen");
            var mSchedule = mBind.GetCom<Text>("Schedule");
            var mBg = mBind.GetCom<Image>("bg");

            mTabText.text = string.Format("第{0}章", chapterIDList[item.m_index]);
            chapterToggleList[item.m_index] = mToggle;
            mToggle.onValueChanged.RemoveAllListeners();
            mToggle.onValueChanged.AddListener((value) =>
            {
                if(value)
                {
                    mCurSelectChapterIdx = chapterIDList[item.m_index];
                    allRewardList = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chapterIDList[item.m_index]);
                    allRewardList.Sort(SortList);
                    mTaskItemComUIList.SetElementAmount(allRewardList.Count);

                    if(mCurSelectChapterIdx <= 0)
                    {
                        Logger.LogErrorFormat("[关卡宝箱] 页签索引错误，mCurSelectChapterIdx = {0}", mCurSelectChapterIdx);
                    }

                    _updateProcess(chapterIDList[item.m_index]);
                    _initAllReward(chapterIDList[item.m_index]);
                    _updateAllRewardStaus(chapterIDList[item.m_index]);
                    //mChapterNum.text = chapterIDList[item.m_index].ToString();
                    //mTaskScroll.verticalNormalizedPosition = 1.0f;
                }
                mSelect.CustomActive(value);
                
            });
            if(chapterIDList[item.m_index] == mCurSelectChapterIdx)
            {
                mToggle.isOn = true;
                mSelect.CustomActive(true);
            }
            else
            {
                mSelect.CustomActive(false);
            }

            //KeyValuePair<int, int> rate = ChapterUtility.GetChapterRewardByChapterIdxNew(chapterIDList[item.m_index]);
            KeyValuePair<int, int> rate = ChapterUtility.GetChapterRewardByChapterIdx(chapterIDList[item.m_index]);
            int finish = rate.Key;
            int sum = rate.Value;

            mFinished.CustomActive(false);
            mNoOpen.CustomActive(false);
            mSchedule.CustomActive(false);
            mSchedule.text = string.Format("{0}/{1}", finish, sum);
            if(finish == sum)
            {
                mFinished.CustomActive(true);
            }
            else
            {
                mSchedule.CustomActive(true);
            }
            var chapterTableItem = TableManager.GetInstance().GetTableItem<ChapterTable>(chapterIDList[item.m_index]);
            if(chapterTableItem != null)
            {
                ETCImageLoader.LoadSprite(ref mBg,chapterTableItem.ChapterIconPath);
            }
            //todo未解锁
        }

        void _UpdateTaskBind(ComUIListElementScript item)
        {
            var cur = allRewardList[item.m_index];
            ComCommonBind bind = item.GetComponent<ComCommonBind>();
            if (bind == null)
            {
                return;
            }
            if (item.m_index < 0)
            {
                return;
            }
            if (null != bind)
            {
                Utility.AttachTo(bind.gameObject, mContent);

                bind.gameObject.name = string.Format("{0}", cur.taskID);
                Text desc = bind.GetCom<Text>("desc");
                ComItemList reward = bind.GetCom<ComItemList>("reward");

                Button getbutton = bind.GetCom<Button>("getbutton");
                UIGray gray = bind.GetCom<UIGray>("gray");
                Text mNowScore = bind.GetCom<Text>("NowScore");
                Button mGoChapter = bind.GetCom<Button>("GoChapter");
                var mS1 = bind.GetCom<UIGray>("S1");
                var mS2 = bind.GetCom<UIGray>("S2");
                var mS3 = bind.GetCom<UIGray>("S3");
                mNowScore.CustomActive(true);

                int curTaskID = cur.missionItem.ID;
                string tempParam = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)curTaskID, "parameter");//读当前分数
                string tempScore = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)curTaskID, "score");//读当前分数
                int score = -1;
                int param = -1;
                int.TryParse(tempScore, out score);
                int.TryParse(tempParam, out param);
                mS1.enabled = false;
                mS2.enabled = false;
                mS3.enabled = false;
                if (score != 0)
                {
                    switch (score)
                    {
                        case (int)Protocol.DungeonScore.SSS:
                            mNowScore.text = "SSS";
                            break;
                        case (int)Protocol.DungeonScore.SS:
                            mNowScore.text = "SS";
                            mS1.enabled = true;
                            break;
                        case (int)Protocol.DungeonScore.S:
                            mNowScore.text = "S";
                            mS1.enabled = true;
                            mS2.enabled = true;
                            break;
                        default:
                            mNowScore.CustomActive(false);
                            mS1.enabled = true;
                            mS2.enabled = true;
                            mS3.enabled = true;
                            break;
                    }
                }
                else
                {
                    switch (param)
                    {
                        case 1:
                            mNowScore.text = "SSS";
                            break;
                        default:
                            mNowScore.CustomActive(false);
                            mS1.enabled = true;
                            mS2.enabled = true;
                            mS3.enabled = true;
                            break;
                    }
                }

                Node node = new Node() { bind = bind, info = cur };

                _updateSingleItemStatus(node);
                
                var taskTableData = TableManager.GetInstance().GetTableItem<MissionTable>(curTaskID);

                if (taskTableData != null)
                {
                    desc.text = taskTableData.TaskName;
                }

                {
                    List<AwardItemData> awards = MissionManager.GetInstance().GetMissionAwards(curTaskID);

                    List<ComItemList.Items> list = new List<ComItemList.Items>();
                    for (int j = 0; j < awards.Count; ++j)
                    {
                        list.Add(new ComItemList.Items()
                        {
                            id = awards[j].ID,
                            count = (uint)awards[j].Num,
                        });
                    }
                    reward.SetItems(list.ToArray());
                }
                getbutton.onClick.RemoveAllListeners();
                getbutton.onClick.AddListener(() =>
                {
                    MissionManager.GetInstance().sendCmdSubmitTask(
                            (UInt32)curTaskID,
                            Protocol.TaskSubmitType.TASK_SUBMIT_AUTO,
                            0);
                });
                mGoChapter.onClick.RemoveAllListeners();
                int tempMissionId = curTaskID;
                mGoChapter.onClick.AddListener(() =>
                {
                    if (tempMissionId == 0)
                    {
                        //SystemNotifyManager.SysNotifyFloatingEffect(string.Format("请找宫德强先生在任务表id为{0}的任务的寻路id", tempMissionId));
                    }
                    else
                    {
                        if (mCurSelectChapterIdx == defaultChapterIdx)
                        {
                            ClientSystemManager.GetInstance().CloseFrame<ChapterRewardFrame>();
                            if (ClientSystemManager.GetInstance().IsFrameOpen<ChapterNormalHalfFrame>())
                            {
                                ClientSystemManager.GetInstance().CloseFrame<ChapterNormalHalfFrame>();
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChapterNormalHalfFrameClose);
                            }
                            ChapterSelectFrame.SetDungeonID(taskTableData.DungeonTableID);
                        }
                        else
                        {
                            StartCoroutine(MoveToChapter(tempMissionId));
                            //MissionManager.GetInstance().AutoTraceTask(tempMissionId);
                        }
                    }
                });


                //Utility.ContentProcess process = Utility.ParseMissionProcess(curTaskID);
            }
        }
        private class SelectNode
        {
            public int           chapterID;
            public ComCommonBind bind;
        }
        

        protected override void _OnCloseFrame()
        {
            _unloadChapter();
            chapterIDList.Clear();
            isChapterLockList.Clear();
            toggles.Clear();
        }
        

        private void _onUpdateMission(UInt32 taskID)
        {
            //mToggleUIList.SetElementAmount(chapterIDList.Count);
            mToggleComUIList.SetElementAmount(chapterIDList.Count);
            allRewardList = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, mCurSelectChapterIdx);
            allRewardList.Sort(SortList);
            mTaskItemComUIList.SetElementAmount(allRewardList.Count);
            _updateProcess(mCurSelectChapterIdx);
            _updateAllRewardStaus(mCurSelectChapterIdx);
            _initAllReward(mCurSelectChapterIdx);
            //_UpdateChapterList();
        }
        private int SortList(MissionManager.SingleMissionInfo a, MissionManager.SingleMissionInfo b) //a b表示列表中的元素
        {
            if(a.status == (byte)Protocol.TaskStatus.TASK_FINISHED)
            {
                if(b.status != (byte)Protocol.TaskStatus.TASK_FINISHED)
                {
                    return -1;
                }
            }
            if(a.status == (byte)Protocol.TaskStatus.TASK_UNFINISH)
            {
                if(b.status == (byte)Protocol.TaskStatus.TASK_FINISHED)
                {
                    return 1;
                }
                if (b.status == (byte)Protocol.TaskStatus.TASK_OVER)
                {
                    return -1;
                }
            }
            if (a.status == (byte)Protocol.TaskStatus.TASK_OVER)
            {
                if (b.status != (byte)Protocol.TaskStatus.TASK_OVER)
                {
                    return 1;
                }
            }
            if(a.taskID > b.taskID)
            {
                return 1;
            }
            if(a.taskID < b.taskID)
            {
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 总的章节宝箱的状态
        /// </summary>
        private void _updateAllRewardStaus(int chidx)
        {
            List<MissionManager.SingleMissionInfo> allReward = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Chest, chidx);

            if (null != allReward && allReward.Count > 0)
            {
                MissionManager.SingleMissionInfo info = allReward[0];

                Logger.LogProcessFormat("[关卡宝箱] 更新总章节奖励状态 {0}", (Protocol.TaskStatus)info.status);

                switch ((Protocol.TaskStatus)info.status)
                {
                    case Protocol.TaskStatus.TASK_FINISHED:
                        // mChapterRewardBoxStatus.sprite = mBind.GetSprite("close");
                        mBind.GetSprite("close", ref mChapterRewardBoxStatus);
                        mChapterReward.gameObject.SetActive(true);
                        mChapterRewardText.text = "可领取";
                        mCharpterRewardGray.enabled = false;
                        mChapterRewardRed.CustomActive(true);
                        mEffectRoot.SetActive(true);
                        mEffectAnimate.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        mEffectAnimate.isActive = true;
                        mEffectAnimate.DOPlay();
                        break;
                    case Protocol.TaskStatus.TASK_OVER:
                        // mChapterRewardBoxStatus.sprite = mBind.GetSprite("open");
                        mBind.GetSprite("open", ref mChapterRewardBoxStatus);
                        mChapterRewardRed.CustomActive(false);
                        mChapterReward.gameObject.SetActive(false);
                        mEffectRoot.SetActive(false);
                        mEffectAnimate.DOKill();
                        mEffectAnimate.isActive = false;
                        mEffectAnimate.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        break;
                    default:
                        // mChapterRewardBoxStatus.sprite = mBind.GetSprite("close");
                        mBind.GetSprite("close", ref mChapterRewardBoxStatus);
                        mChapterRewardRed.CustomActive(false);
                        mChapterReward.gameObject.SetActive(true);
                        mChapterRewardText.text = "未达成";
                        mCharpterRewardGray.enabled = true;
                        mEffectRoot.SetActive(false);
                        mEffectAnimate.DOKill();
                        mEffectAnimate.isActive = false;
                        mEffectAnimate.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        break;
                }
            }
            else
            {
                mChapterRewardText.text = "";
                mCharpterRewardGray.enabled = true;

                if(allReward == null)
                {
                    Logger.LogErrorFormat("[关卡宝箱] 总章节奖励未配置,allReward == null,chidx = {0}", chidx);
                }
                else
                {
                    Logger.LogErrorFormat("[关卡宝箱] 总章节奖励未配置,allReward != null,allReward.Count = {0},chidx = {1}", allReward.Count, chidx);
                }
            }
        }

        private void _updateSingleItemStatus(Node node)
        {
            ComCommonBind bind = node.bind;
            if (null != bind)
            {
                Button getbutton = bind.GetCom<Button>("getbutton");
                UIGray gray = bind.GetCom<UIGray>("gray");
                GameObject mComplete = bind.GetGameObject("complete");
                GameObject mUnfinish = bind.GetGameObject("unfinish");
                Button mGoChapter = bind.GetCom<Button>("GoChapter");

                Protocol.TaskStatus status = (Protocol.TaskStatus)node.info.status;

                switch (status)
                {
                    case Protocol.TaskStatus.TASK_FINISHED:
                        getbutton.gameObject.CustomActive(true);
                        //bind.gameObject.transform.SetAsFirstSibling();
                        //gray.enabled = false;
                        mGoChapter.CustomActive(false);
                        break;
                    case Protocol.TaskStatus.TASK_OVER:
                        getbutton.gameObject.SetActive(false);
                        //bind.gameObject.transform.SetAsLastSibling();
                        mComplete.CustomActive(true);
                        mUnfinish.CustomActive(false);
                        mGoChapter.CustomActive(false);
                        break;
                    default:
                        //gray.enabled = true;
                        getbutton.gameObject.CustomActive(false);
                        mComplete.CustomActive(false);
                        mGoChapter.CustomActive(false);
                        mUnfinish.CustomActive(false);
                        var taskTableData = TableManager.GetInstance().GetTableItem<MissionTable>((int)node.info.taskID);
                        if (taskTableData != null)
                        {
                            if (taskTableData.DungeonTableID != 0)
                            {
                                ComChapterDungeonUnit.eState state = ChapterUtility.GetDungeonState(taskTableData.DungeonTableID);
                                if (state == ComChapterDungeonUnit.eState.Locked)
                                {
                                    mUnfinish.CustomActive(true);
                                }
                                else
                                {
                                    mGoChapter.CustomActive(true);
                                }
                            }
                            else
                            {
                                //会删掉
                                //SystemNotifyManager.SysNotifyFloatingEffect(string.Format("请找宫德强先生在任务表id为{0}的任务的DungeonTableID列里填入对应的地下城id", node.info.taskID));
                            }
                        }
                        
                        break;
                }
            }

            //Logger.LogProcessFormat("[关卡宝箱] 更新章节奖励 顺序");
        }


        private void _updateProcess(int chidx)
        {
            //KeyValuePair<int, int> rate = ChapterUtility.GetChapterRewardByChapterIdxNew(chidx);
            KeyValuePair<int, int> rate = ChapterUtility.GetChapterRewardByChapterIdx(chidx);

            int finish = rate.Key;
            int sum    = rate.Value;

            mSumcount.text    = string.Format("{0}", sum);
            mFinishcount.text = string.Format("{0}", finish);
            mScoreCount.text = finish.ToString();
            maxSCount = sum;
            if (sum <= 0)
            {
                mProcess.value = 1.0f;
            }
            else 
            {
                mProcess.value = finish * 1.0f / sum;
            }

            Logger.LogProcessFormat("[关卡宝箱] 设置更新总章节奖励进度 {0}", mProcess.value);

            var chapterTableItem = TableManager.GetInstance().GetTableItem<ChapterTable>(chidx);
            if (chapterTableItem != null)
            {
                ETCImageLoader.LoadSprite(ref mChapterIcon, chapterTableItem.ChapterIconPath);
                mChapterText.text = chapterTableItem.ChapterName;
            }
        }


        /// <summary>
        /// 总章节奖励
        /// </summary>
        /// <param name="chid"></param>
        private void _initAllReward(int chid)
        {
            List<MissionManager.SingleMissionInfo> allReward  = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Chest, chid);
            for(int i = 0;i< rewardBoxArr.Length;i++)
            {
                rewardBoxArr[i].CustomActive(false);
            }
            for (int i = 0; i < allReward.Count; i++)
            {
                uint taskID = allReward[i].taskID;
                if (i >= rewardBoxArr.Length)
                {
                    continue;
                }
                int tempIndex = 3 - (allReward.Count - (i + 1)) - 1;
                List<AwardItemData> awards = MissionManager.GetInstance().GetMissionAwards(allReward[i].missionItem.ID);
                if (awards.Count > 0)
                {
                    rewardBoxArr[tempIndex].CustomActive(true);
                    var mBind = rewardBoxArr[tempIndex].GetComponent<ComCommonBind>();
                    if (mBind == null)
                    {
                        continue;
                    }
                    var mItemRoot = mBind.GetGameObject("itemRoot");
                    var mItemBtn = mBind.GetCom<Button>("itemBtn");
                    var mFinishImage = mBind.GetGameObject("finishImage");
                    var mEffectRoot = mBind.GetGameObject("effectRoot");
                    var mScore = mBind.GetCom<Text>("Score");

                    mItemBtn.onClick.RemoveAllListeners();
                    mItemBtn.onClick.AddListener(() =>
                    {
                        MissionManager.GetInstance().sendCmdSubmitTask(
                                (UInt32)taskID,
                                Protocol.TaskSubmitType.TASK_SUBMIT_AUTO,
                                0);
                        mItemBtn.CustomActive(false);
                        mEffectRoot.CustomActive(false);
                        mFinishImage.CustomActive(true);
                    });
                    mItemBtn.CustomActive(false);
                    mFinishImage.CustomActive(false);
                    mEffectRoot.CustomActive(false);
                    Protocol.TaskStatus status = (Protocol.TaskStatus)allReward[i].status;
                    switch (status)
                    {
                        case Protocol.TaskStatus.TASK_FINISHED:
                            mItemBtn.CustomActive(true);
                            mEffectRoot.CustomActive(true);
                            break;
                        case Protocol.TaskStatus.TASK_OVER:
                            mFinishImage.CustomActive(true);
                            break;
                        default:
                            break;
                    }
                    if(tempIndex + 1 == 3)
                    {
                        mScore.text = maxSCount.ToString();
                    }
                    else
                    {
                        mScore.text = (maxSCount / 3 * (tempIndex + 1)).ToString();
                    }

                    ComItem comitem = mItemRoot.GetComponentInChildren<ComItem>();
                    if (comitem == null)
                    {
                        comitem = CreateComItem(mItemRoot);
                    }
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(awards[0].ID);
                    if (null == ItemDetailData)
                    {
                        return;
                    }
                    int tempCount = awards[0].Num;
                    if (tempCount > 10000 && tempCount % 10000 == 0)
                    {
                        ItemDetailData.Count = 0;
                        string tempStrCount = (tempCount / 10000).ToString();
                        comitem.SetCountFormatter((var) =>
                        {
                            return tempStrCount + "万";
                        });
                    }
                    else
                    {
                        ItemDetailData.Count = awards[0].Num;
                        comitem.SetCountFormatter(null);
                    }
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });
                }
            }
            if (null != allReward && allReward.Count > 0)
            {
                List<AwardItemData> awards = MissionManager.GetInstance().GetMissionAwards(allReward[0].missionItem.ID);

                Logger.LogProcessFormat("[关卡宝箱] 设置更新总章节奖励物品");
                //nodeScore.Add(ChapterUtility.GetDungeonBestScore(id.dungeonID));

                List<ComItemList.Items> list = new List<ComItemList.Items>();
                for (int j = 0; j < awards.Count; ++j)
                {
                    list.Add(new ComItemList.Items()
                    {
                        id = awards[j].ID,
                        count = (uint)awards[j].Num,
                    });
                }
                mAllreward.SetItems(list.ToArray());
            }
        }

        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        void _OnContentItemVisiableDelegate(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            if (item.m_index < 0)
            {
                return;
            }
            if (chapterIDList.Count <= 0)
            {
                return;
            }
            var mToggle = mBind.GetCom<Toggle>("Func");
            var mTabText_UnSelect= mBind.GetCom<Text>("TabText_UnSelect");
            var mTabText_Select = mBind.GetGameObject("TabText_Select");
            var mSelect = mBind.GetGameObject("Select");
            var chapterTableItem = TableManager.GetInstance().GetTableItem<ChapterTable>(chapterIDList[item.m_index]);
            if (chapterTableItem != null)
            {
                mTabText_Select.GetComponent<Text>().text = chapterTableItem.ChapterName.ToString();
                mTabText_UnSelect.text = chapterTableItem.ChapterName.ToString();
            }            
            if (!toggles.ContainsKey(mTabText_UnSelect.text))
            {
                toggles.Add(mTabText_UnSelect.text, mToggle);
            }


            mToggle.onValueChanged.RemoveAllListeners();
            mToggle.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    if (chapterIDList[item.m_index] == 0)
                    {
                        mRightroot.CustomActive(false);
                        mPandectroot.CustomActive(true);
                        mPandectUIList.SetElementAmount(chapterIDList.Count - 1);
                    }
                    else
                    {
                        mRightroot.CustomActive(true);
                        mPandectroot.CustomActive(false);
                        mCurSelectChapterIdx = chapterIDList[item.m_index];
                        allRewardList = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chapterIDList[item.m_index]);
                        allRewardList.Sort(SortList);
                        mTaskItemComUIList.SetElementAmount(allRewardList.Count);

                        if (mCurSelectChapterIdx <= 0)
                        {
                            Logger.LogErrorFormat("[关卡宝箱] 页签索引错误，mCurSelectChapterIdx = {0}", mCurSelectChapterIdx);
                        }

                        _updateProcess(chapterIDList[item.m_index]);
                        _initAllReward(chapterIDList[item.m_index]);
                        _updateAllRewardStaus(chapterIDList[item.m_index]);
                    }

                }
                mTabText_Select.CustomActive(value);
                mSelect.CustomActive(value);

            });

            //todo未解锁
        }

        void _OnContentItemSelectDelegate(ComUIListElementScript item)
        {

        }

        void _OnContentItemRecycleDelegate(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind != null)
            {
                var mToggle = mBind.GetCom<Toggle>("Func");
                var mSelect = mBind.GetGameObject("Select");
                if (mToggle != null)
                {
                    mToggle.isOn = false;
                    mToggle.onValueChanged.RemoveAllListeners();
                }
                if (mSelect != null)
                {
                    mSelect.CustomActive(false);
                }
                return;
            }
        }

        
        void _UpdatePandectList(ComUIListElementScript item)
        {
            int index = item.m_index + 1;
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            var cbtn = mBind.GetCom<ButtonEx>("ChapterBtn");
            var Icon = mBind.GetCom<Image>("Image");
            var cimgroot = mBind.GetGameObject("Imageroot");
            var icon = cimgroot.GetComponent<Image>();
            var name = mBind.GetCom<Text>("TabText");
            var point = mBind.GetGameObject("point");
            point.CustomActive(false);
            var unlockbg = mBind.GetGameObject("bg");
            var finshbg = mBind.GetGameObject("bg2");
            var slider = mBind.GetCom<Slider>("Slider");
            var sliderroot = mBind.GetGameObject("Sliderroot");
            var curt = mBind.GetCom<Text>("cur");
            var sumt = mBind.GetCom<Text>("sum");
            var chapterTableItem = TableManager.GetInstance().GetTableItem<ChapterTable>(chapterIDList[index]);
            if (chapterTableItem != null)
            {
                ETCImageLoader.LoadSprite(ref icon, chapterTableItem.ChapterIconPath);
                name.text = chapterTableItem.ChapterName.ToString();
            }

            KeyValuePair<int, int> rate = ChapterUtility.GetChapterRewardByChapterIdx(index);

            int finish = rate.Key;
            int sum = rate.Value;

            sumt.text = string.Format("{0}", sum);
            curt.text = string.Format("{0}", finish);
            if (sum <= 0)
            {
                slider.value = 1.0f;
            }
            else
            {
                slider.value = finish * 1.0f / sum;
            }
            finshbg.CustomActive(finish == sum);

            isChapterLockList.Add(true);
            if (item.m_index > 0)
            {
                if (!isChapterLockList[item.m_index - 1] || !ChapterUtility.IsChapterOpen(index))
                {
                    isChapterLockList[item.m_index] = false;
                    cimgroot.GetComponent<UIGray>().enabled = true;
                    cimgroot.GetComponent<UIGray>().Refresh();
                    unlockbg.GetComponent<UIGray>().enabled = true;
                    unlockbg.GetComponent<UIGray>().Refresh();
                    sliderroot.CustomActive(false);
                    finshbg.CustomActive(false);
                }
            }
            cbtn.onClick.RemoveAllListeners();
            cbtn.onClick.AddListener(() =>
            {
                mToggleComUIList.MoveElementInScrollArea(index, true);
                string cname = TableManager.GetInstance().GetTableItem<ChapterTable>(index).ChapterName;
                if (!toggles.ContainsKey(cname))
                {
                    mRightroot.CustomActive(true);
                    mPandectroot.CustomActive(false);
                    mCurSelectChapterIdx = chapterIDList[index];
                    allRewardList = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chapterIDList[index]);
                    allRewardList.Sort(SortList);
                    mTaskItemComUIList.SetElementAmount(allRewardList.Count);
                    _updateProcess(chapterIDList[index]);
                    _initAllReward(chapterIDList[index]);
                }
                else
                {
                    //chapterToggleList[index].isOn = true;
                    Toggle toggle;
                    toggles.TryGetValue(cname, out toggle);
                    toggle.isOn = true;
                }

            });

            allRewardList = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chapterIDList[index]);
            Protocol.TaskStatus status = Protocol.TaskStatus.TASK_INIT;
            for (int i = 0; i < allRewardList.Count; i++)
            {
                status = (Protocol.TaskStatus)allRewardList[i].status;
                if (status == Protocol.TaskStatus.TASK_FINISHED)
                    break;
            }
            
            List<MissionManager.SingleMissionInfo> allReward = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Chest, index);

            if (null != allReward && allReward.Count > 0)
            {
                MissionManager.SingleMissionInfo info = allReward[0];

                Logger.LogProcessFormat("[关卡宝箱] 更新总章节奖励状态 {0}", (Protocol.TaskStatus)info.status);

                if (((Protocol.TaskStatus)info.status == Protocol.TaskStatus.TASK_FINISHED) || (status == Protocol.TaskStatus.TASK_FINISHED))
                {
                    point.CustomActive(true);
                }
            }       
        }

        void _PandectListRecycle(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                var cbtn = mBind.GetCom<ButtonEx>("ChapterBtn");
                if (cbtn != null)
                {
                    cbtn.onClick.RemoveAllListeners();
                }
            }
        }
    }
}
