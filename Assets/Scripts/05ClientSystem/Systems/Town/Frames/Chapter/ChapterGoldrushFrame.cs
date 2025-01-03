using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;


namespace GameClient
{
    public class ChapterGoldRushFrame : ChapterBaseFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/Dungeon/ChapterGoldRush";
        }
#region ExtraUIBind
        private ComCommonChapterInfo mChapterInfo = null;
        private Button mStart = null;
        private Text mLeftCount = null;
        private Text mDifNorDropDesc = null;
        private Text mDifAdventureDropDesc = null;
        private Text mDifWarriorDropDesc = null;
        private UIGray mGroupStartGray = null;

        protected sealed override void _bindExUI()
        {
	        mChapterInfo = mBind.GetCom<ComCommonChapterInfo>("chapterInfo");
	        mStart = mBind.GetCom<Button>("start");
	        mStart.onClick.AddListener(_onStartButtonClick);
	        mLeftCount = mBind.GetCom<Text>("leftCount");
            mDifNorDropDesc = mBind.GetCom<Text>("norDropDesc");
            mDifAdventureDropDesc = mBind.GetCom<Text>("adventureDropDesc");
            mDifWarriorDropDesc = mBind.GetCom<Text>("warriorDropDesc");
            mGroupStartGray = mBind.GetCom<UIGray>("GroupStartGray");
        }

        protected sealed override void _unbindExUI()
        {
	        mChapterInfo = null;
	        mStart.onClick.RemoveListener(_onStartButtonClick);
	        mStart = null;
	        mLeftCount = null;
            mDifNorDropDesc = null;
            mDifAdventureDropDesc = null;
            mDifWarriorDropDesc = null;
            mGroupStartGray = null;
        }
#endregion   
#region Callback
        private void _onStartButtonClick()
        {
            /* put your code in here */

            _onStartButton();
        }
#endregion

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (null != mDungeonTable)
            {
                int leftCount = mDungeonTable.DailyMaxTime - CountDataManager.GetInstance().GetCount(CounterKeys.DUNGEON_DAILY_COUNT_PREFIX, (int)mDungeonTable.SubType);
                mLeftCount.text = leftCount.ToString();
            }
        }

        protected sealed override void _loadLeftPanel()
        {
            if (null != mChapterInfo)
            {
                mChapterInfoCommon    = mChapterInfo;
                mChapterInfoDiffculte = mChapterInfo;
                mChapterInfoDrops     = mChapterInfo;
                mChapterPassReward    = mChapterInfo;
                mChapterScore         = mChapterInfo;
                mChapterMonsterInfo   = mChapterInfo;
                mChapterActivityTimes = mChapterInfo;
                mChapterNodeState     = mChapterInfo;
            }

            mChapterInfoDiffculte.SetDiffculte(mChapterInfoDiffculte.GetDiffculte(),mDungeonID.dungeonID);

            List<eChapterNodeState> nodeState = new List<eChapterNodeState>();
            List<int> nodeLimitLevel = new List<int>();
            
            int curTopHard = ChapterUtility.GetDungeonTopHard(mDungeonID.dungeonIDWithOutDiff);

            DungeonID id = new DungeonID(mDungeonID.dungeonID);
            for (int i = 0; i <= curTopHard; ++i)
            {
                id.diffID = i;
                
                var table = TableManager.instance.GetTableItem<DungeonTable>(id.dungeonID);

                if (null != table)
                {
                    if (i == 0)
                    {
                        mDifNorDropDesc.SafeSetText(table.HardDescription);
                    }
                    else if (i == 1)
                    {
                        mDifAdventureDropDesc.SafeSetText(table.HardDescription);
                    }
                    else if (i == 2)
                    {
                        mDifWarriorDropDesc.SafeSetText(table.HardDescription);
                    }
                }

                if (null != table)
                {
                    nodeLimitLevel.Add(table.MinLevel);
                }
                else
                {
                    nodeLimitLevel.Add(0);
                }

                ComChapterDungeonUnit.eState state = ChapterUtility.GetDungeonState(id.dungeonID);
                switch (state)
                {
                    case ComChapterDungeonUnit.eState.Locked:
                        if (PlayerBaseData.GetInstance().Level >= nodeLimitLevel[i])
                        {
                            nodeState.Add(eChapterNodeState.Lock);
                        }
                        else
                        {
                            nodeState.Add(eChapterNodeState.LockLevel);
                        }
                        break;
                    case ComChapterDungeonUnit.eState.LockPassed:
                    case ComChapterDungeonUnit.eState.Passed:
                        nodeState.Add(eChapterNodeState.Passed);
                        break;
                    case ComChapterDungeonUnit.eState.Unlock:
                        if (PlayerBaseData.GetInstance().Level >= nodeLimitLevel[i])
                        {
                            nodeState.Add(eChapterNodeState.Unlock);
                        }
                        else
                        {
                            nodeState.Add(eChapterNodeState.LockLevel);
                        }
                        break;
                }
            }

            for (int i = curTopHard + 1; i < 4; ++i)
            {
                nodeState.Add(eChapterNodeState.Miss);
            }

            if (null != mChapterNodeState)
            {
                mChapterNodeState.SetChapterState(nodeState.ToArray(), nodeLimitLevel.ToArray());
            }
        }


        private void _onStartButton()
        {
            GameFrameWork.instance.StartCoroutine(_commonStart());
        }

        
        protected sealed override void _onDiffChange(int idx)
        {
            mDungeonID.diffID = idx;
            int id = mDungeonID.dungeonID;

            var state = ChapterUtility.GetDungeonState(id);

            var mTab = TableManager.GetInstance().GetTableItem<DungeonTable>(id);
            if (mTab == null)
            {
                return;
            }
            bool isLock = (state == ComChapterDungeonUnit.eState.Locked || mTab.MinLevel > PlayerBaseData.GetInstance().Level);
            mGroupStartGray.enabled = isLock;
        }
    }
}
