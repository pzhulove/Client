using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;
using System.Reflection;

namespace GameClient
{
    public class ChapterMouFrame : ChapterBaseFrame
    {
        public enum ChapterSelectType
        {
            MouChapter = 1000,
        }
        public override string GetPrefabPath()
        {

            return "UIFlatten/Prefabs/Activity/Dungeon/ChapterMou";
        }

#region ExtraUIBind
        private ComCommonChapterInfo mChapterInfo = null;
        private Button mStart = null;
        private Text mLeftCount = null;

        protected override void _bindExUI()
        {
            mChapterInfo = mBind.GetCom<ComCommonChapterInfo>("chapterInfo");
            mStart = mBind.GetCom<Button>("start");
            mStart.onClick.AddListener(_onStartButtonClick);
            mLeftCount = mBind.GetCom<Text>("leftCount");
        }

        protected override void _unbindExUI()
        {
            mChapterInfo = null;
            mStart.onClick.RemoveListener(_onStartButtonClick);
            mStart = null;
            mLeftCount = null;
        }
#endregion   


#region Callback
        private void _onStartButtonClick()
        {
            /* put your code in here */
            _onStartButton();
        }
#endregion

        protected override void _loadBg()
        {
        }


        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();


            if (null != mDungeonTable)
            {
                int leftCount = mDungeonTable.DailyMaxTime - CountDataManager.GetInstance().GetCount(CounterKeys.DUNGEON_DAILY_COUNT_PREFIX, (int)mDungeonTable.SubType);
                mLeftCount.text = leftCount.ToString();
            }
        }


        protected override void _loadLeftPanel()
        {
            if (null != mChapterInfo)
            {
                //leftPanel.name = kNameInfo;

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
            List<ComItemList.Items> items = new List<ComItemList.Items>();
            ComItemList.Items item = new ComItemList.Items();
            
            var levelAdapterTableData = TableManager.GetInstance().GetTableItem<LevelAdapterTable>((int)ChapterSelectType.MouChapter);
            if(levelAdapterTableData == null)
            {
                Logger.LogErrorFormat("can not get levelAdapterTable id is {0}", (int)ChapterSelectType.MouChapter);
                return;
            }
            PropertyInfo info = levelAdapterTableData.GetType().GetProperty(string.Format("Level{0}", PlayerBaseData.GetInstance().Level), (BindingFlags.Instance | BindingFlags.Public));
            item.id = mDungeonTable.DropItems[0];
            int itemCount = 0;
            if (info != null)
            {
                itemCount = (int)info.GetValue(levelAdapterTableData, null);
            }

            item.id = mDungeonTable.DropItems[0];
            item.count = (uint)itemCount;
            items.Add(item);
            mChapterInfoDrops.UpdateDropCount(items);
        }

        private void _onStartButton()
        {
            GameFrameWork.instance.StartCoroutine(_commonStart());
        }
    }
}
