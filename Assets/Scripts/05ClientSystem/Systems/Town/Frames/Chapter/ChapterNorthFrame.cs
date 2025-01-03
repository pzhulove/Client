using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;


namespace GameClient
{
    public class ChapterNorthFrame : ChapterBaseFrame
    {
        public static int sMuti = 1;

#region ExtraUIBind
        private ComCommonChapterInfo mChapterInfo = null;
        private Button mStart = null;
        private Toggle mCostSelect0 = null;
        private Toggle mCostSelect1 = null;
        private Text mLeftCount = null;
        private Text mVipText = null;


        private Text[] mGetTexts = new Text[2];
        private Text[] mCostTexts = new Text[2];


        private Text mName = null;

        private Text mCurrentDiffText = null;
        private Text mCurrentDropText = null;

        protected override void _bindExUI()
        {
            mChapterInfo = mBind.GetCom<ComCommonChapterInfo>("chapterInfo");
            mStart = mBind.GetCom<Button>("start");
            mStart.onClick.AddListener(_onStartButtonClick);
            mCostSelect0 = mBind.GetCom<Toggle>("costSelect0");
            mCostSelect0.onValueChanged.AddListener(_onCostSelect0ToggleValueChange);
            mCostSelect1 = mBind.GetCom<Toggle>("costSelect1");
            mCostSelect1.onValueChanged.AddListener(_onCostSelect1ToggleValueChange);
            mLeftCount = mBind.GetCom<Text>("leftCount");
            mVipText = mBind.GetCom<Text>("vipText");

            mGetTexts[0] = mBind.GetCom<Text>("getText0");
            mGetTexts[1] = mBind.GetCom<Text>("getText1");

            mCostTexts[0] = mBind.GetCom<Text>("costText0");
            mCostTexts[1] = mBind.GetCom<Text>("costText1");

            mCurrentDiffText = mBind.GetCom<Text>("currentDiffText");
            mCurrentDropText = mBind.GetCom<Text>("currentDropText");

            mName = mBind.GetCom<Text>("name");
            
        }

        protected override void _unbindExUI()
        {
            mChapterInfo = null;
            mStart.onClick.RemoveListener(_onStartButtonClick);
            mStart = null;
            mCostSelect0.onValueChanged.RemoveListener(_onCostSelect0ToggleValueChange);
            mCostSelect0 = null;
            mCostSelect1.onValueChanged.RemoveListener(_onCostSelect1ToggleValueChange);
            mCostSelect1 = null;
            mLeftCount = null;
            mVipText = null;
            mGetTexts[0] = null;
            mGetTexts[1] = null;
            mCostTexts[0] = null;
            mCostTexts[1] = null;
            mCurrentDiffText = null;
            mCurrentDropText = null;
            mName = null;
        }
#endregion   

#region Callback
        private void _onStartButtonClick()
        {
            /* put your code in here */
            _onStartButton();
        }

        private void _onCostSelect0ToggleValueChange(bool changed)
        {
            /* put your code in here */
            _onHandle(0, changed);

        }
        private void _onCostSelect1ToggleValueChange(bool changed)
        {
            /* put your code in here */
            _onHandle(1, changed);
        }
#endregion

        protected override void _loadLeftPanel()
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

            mChapterInfoDiffculte.SetDiffculte(mChapterInfoDiffculte.GetDiffculte(), mDungeonID.dungeonID);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/Dungeon/ChapterNorth";
        }

        private int mSelect = -1;

        private void _onHandle(int idx, bool status)
        {
            mSelect = -1;
            sMuti = 1;

            if (status)
            {
                mSelect = idx;

                if (!int.TryParse(mGetTexts[idx].text, out sMuti))
                {
                    sMuti = 1;
                }
            }
        }

        private void _onStartButton()
        {
            if (mSelect >= 0 && mSelect < 2)
            {
                int cost = 0;

                if (!int.TryParse(mCostTexts[mSelect].text, out cost))
                {
                    return;
                }

                CostItemManager.CostInfo info = new CostItemManager.CostInfo();

                info.nMoneyID         = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT);
                info.nCount           = (int)cost;

                CostItemManager.GetInstance().TryCostMoneyDefault(info, ()=>
                {
                    GameFrameWork.instance.StartCoroutine(_commonStart());
                });
            }
            else
            {
                GameFrameWork.instance.StartCoroutine(_commonStart());
            }
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            _updateInfo();
        }

        

        private int _getMaxCount()
        {
            return mDungeonTable.DailyMaxTime + _getVipCount();
        }

        private int _getVipCount()
        {
            float num = Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.MAGIC_VEIN_NUM);
            if (num <= 0.0f)
            {
                return 0;
            }
            else 
            {
                return (int)num;
            }
        }


        private void _updateInfo()
        {
            sMuti = 1;
            mSelect = -1;

            if (mDungeonTable != null)
            {
                mName.text = mDungeonTable.Name;    

                var st = mDungeonTable.RaceEndDropMultiCost;

                for (int i = 0; i < st.Count && i < 2; ++i)
                {
                    var item = st[i];
                    var kv = item.Split(':');
                    if (kv.Length >= 2)
                    {
                        mGetTexts[i].text = kv[0];
                        mCostTexts[i].text = kv[1];
                    }
                }

                // rate
//                var list = mDungeonTable.RaceEndDropBaseMulti.ToArray();

                var newList = new List<int>(mDungeonTable.RaceEndDropBaseMulti);

                newList.RemoveAll(x => x <= 0);

                if (newList.Count > 0)
                {
                }

                int maxCount = _getMaxCount();
                int leftCount = maxCount - CountDataManager.GetInstance().GetCount(CounterKeys.DUNGEON_DAILY_COUNT_PREFIX, (int)mDungeonTable.SubType);
                mLeftCount.text = string.Format("{0}/{1}", leftCount, maxCount);

                int vipCount = _getVipCount();

                if (vipCount <= 0)
                {
                    KeyValuePair<int, float> kv = Utility.GetFirstValidVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.MAGIC_VEIN_NUM);
                    if (kv.Key > 0)
                    {
                        mVipText.text = string.Format("贵族 {0} 可将挑战上限提升至 {1}", kv.Key, (int)kv.Value);
                    }
                }
                else 
                {
                    mVipText.text = string.Format("贵族 {0} 挑战上限提升至 {1}", PlayerBaseData.GetInstance().VipLevel, vipCount);
                }
            }
        }

        protected override void _onDiffChange(int idx)
        {
            mCurrentDiffText.text = ChapterUtility.GetHardString(idx);

            if (null != mDungeonTable)
            {
                mCurrentDropText.text = mDungeonTable.HardDescription;
            }
        }
    }
}
