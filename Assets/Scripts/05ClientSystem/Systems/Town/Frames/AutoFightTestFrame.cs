using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using ProtoTable;

namespace GameClient
{
#if ROBOT_TEST
    /// <summary>
    /// 自动战斗测试类
    /// </summary>
    public class AutoFightTestFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComUIListScript mTabUIList = null;
        private ComUIListScript mDungeonUIList = null;
        private ComUIListScript mSelectList = null;
        private Text mStartButtonText = null;
        private Button mBtnStart = null;
        private Button mBtnClose = null;
        private Button mBtnAllChapter = null;
        private Button mBtnCancelAllChapter = null;
        private Button mBtnCurChapter = null;
        private Button mBtnCancelCurChapter = null;
        private Text mTextTitle = null;

        protected override void _bindExUI()
        {
            mTabUIList = mBind.GetCom<ComUIListScript>("TabUIList");
            mDungeonUIList = mBind.GetCom<ComUIListScript>("DungeonUIList");
            mSelectList = mBind.GetCom<ComUIListScript>("SelectList");
            mStartButtonText = mBind.GetCom<Text>("StartButtonText");
            mBtnStart = mBind.GetCom<Button>("BtnStart");
            mBtnStart.onClick.AddListener(_onBtnStartButtonClick);
            mBtnClose = mBind.GetCom<Button>("BtnClose");
            mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
            mBtnAllChapter = mBind.GetCom<Button>("BtnAllChapter");
            mBtnAllChapter.onClick.AddListener(_onBtnAllChapterButtonClick);
            mBtnCancelAllChapter = mBind.GetCom<Button>("BtnCancelAllChapter");
            mBtnCancelAllChapter.onClick.AddListener(_onBtnCancelAllChapterButtonClick);
            mBtnCurChapter = mBind.GetCom<Button>("BtnCurChapter");
            mBtnCurChapter.onClick.AddListener(_onBtnCurChapterButtonClick);
            mBtnCancelCurChapter = mBind.GetCom<Button>("BtnCancelCurChapter");
            mBtnCancelCurChapter.onClick.AddListener(_onBtnCancelCurChapterButtonClick);
            mTextTitle = mBind.GetCom<Text>("TextTitle");
        }

        protected override void _unbindExUI()
        {
            mTabUIList = null;
            mDungeonUIList = null;
            mSelectList = null;
            mStartButtonText = null;
            mBtnStart.onClick.RemoveListener(_onBtnStartButtonClick);
            mBtnStart = null;
            mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
            mBtnClose = null;
            mBtnAllChapter.onClick.RemoveListener(_onBtnAllChapterButtonClick);
            mBtnAllChapter = null;
            mBtnCancelAllChapter.onClick.RemoveListener(_onBtnCancelAllChapterButtonClick);
            mBtnCancelAllChapter = null;
            mBtnCurChapter.onClick.RemoveListener(_onBtnCurChapterButtonClick);
            mBtnCurChapter = null;
            mBtnCancelCurChapter.onClick.RemoveListener(_onBtnCancelCurChapterButtonClick);
            mBtnCancelCurChapter = null;
            mTextTitle = null;
        }
        #endregion

        #region Callback
        private void _onBtnStartButtonClick()
        {
            AutoFightRunTime.instance.BuildRunTimeData(AutoFightTestDataManager.GetInstance().SelectDungeonDic);
            AutoFightTestDataManager.GetInstance().IsStart = !AutoFightTestDataManager.GetInstance().IsStart;
            if (!mStartButtonText.IsNull())
            {
                mStartButtonText.text = AutoFightTestDataManager.GetInstance().IsStart ? "结束" : "开始";
            }
        }
        private void _onBtnCloseButtonClick()
        {
            Close();
        }
        private void _onBtnAllChapterButtonClick()
        {
            ClearSelectDungeonDic();
            var enumerator = GetChapterDataDic().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                for (int i = 0; i < current.Value.Count; i++)
                {
                    AddSelectDungeonData(current.Value[i].DungeonId, current.Value[i].Name);
                }
            }
            RefreshSelectDungeonListUI();
            mDungeonUIList.UpdateElement();

            SetContent();
        }
        private void _onBtnCancelAllChapterButtonClick()
        {
            ClearSelectDungeonDic();
            RefreshSelectDungeonListUI();
            mDungeonUIList.UpdateElement();

            SetContent();
        }
        private void _onBtnCurChapterButtonClick()
        {
            var list = GetDungeonListByChapterId();
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                AddSelectDungeonData(list[i].DungeonId, list[i].Name);
            }
            RefreshSelectDungeonListUI();
            mDungeonUIList.UpdateElement();
            mSelectList.UpdateElement();

            SetContent();
        }
        private void _onBtnCancelCurChapterButtonClick()
        {
            var list = GetDungeonListByChapterId();
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                RemoveSelectDungeonData(list[i].DungeonId);
            }
            RefreshSelectDungeonListUI();
            mDungeonUIList.UpdateElement();
            mSelectList.UpdateElement();

            SetContent();
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Robot/AutoFightTestFrame";
        }


        protected override void _OnOpenFrame()
        {
            InitChapter();
            SetContent();
        }

        void InitChapter()
        {
            if (!mStartButtonText.IsNull())
            {
                mStartButtonText.text = AutoFightTestDataManager.GetInstance().IsStart ? "结束" : "开始";
            }
            mTabUIList.Initialize();
            mTabUIList.onItemVisiable = OnChapterItemVisiable;
            mTabUIList.onItemVisiable = OnChapterItemVisiable;
            mTabUIList.SetElementAmount(GetChapterIdList().Count);

            mDungeonUIList.Initialize();
            mDungeonUIList.onItemVisiable = OnDungeonItemUpdate;
            mDungeonUIList.OnItemUpdate = OnDungeonItemUpdate;
            mDungeonUIList.SetElementAmount(0);

            mSelectList.Initialize();
            mSelectList.onItemVisiable = OnSelectDungeonListUpdate;
            mSelectList.OnItemUpdate = OnSelectDungeonListUpdate;
            mSelectList.SetElementAmount(GetSelectDungeonDicCount());
        }

        /// <summary>
        /// 章节UI列表刷新时
        /// </summary>
        /// <param name="item"></param>
        void OnChapterItemVisiable(ComUIListElementScript item)
        {
            int index = item.m_index;
            if (index < 0 || index >= GetChapterIdList().Count)
                return;
            var bind = item.GetComponent<ComCommonBind>();
            if (bind.IsNull())
                return;
            var chapterTxt = bind.GetCom<Text>("TabName");
            if (chapterTxt.IsNull())
                return;
            if (index >= GetChapterIdList().Count)
                return;
            var table = GetChapterIdList()[index];
            if (table != null)
            {
                chapterTxt.text = table.Name;
            }

            var selectChapter = bind.GetCom<Toggle>("SelectToggle");
            if (selectChapter != null)
            {
                selectChapter.onValueChanged.RemoveAllListeners();
                selectChapter.onValueChanged.AddListener((value) => { OnChapterSelect(table.ChapterId, value); });
            }
        }

        /// <summary>
        /// 选中章节时
        /// </summary>
        void OnChapterSelect(int chapterId, bool value)
        {
            if (value)
            {
                SetSelChapterId(chapterId);
                var data = GetDungeonListByChapterId();
                mDungeonUIList.ResetSelectedElementIndex();
                mDungeonUIList.SetElementAmount(data.Count);
                mDungeonUIList.UpdateElement();
            }
        }

        /// <summary>
        /// 地下城列表刷新
        /// </summary>
        void OnDungeonItemUpdate(ComUIListElementScript item)
        {
            var bind = item.GetComponent<ComCommonBind>();
            if (bind.IsNull())
                return;
            var dungeonNameText = bind.GetCom<Text>("Name");
            if (dungeonNameText.IsNull())
                return;
            var data = GetDungeonListByChapterId();
            if (item.m_index > data.Count)
                return;
            string name = data[item.m_index].Name;
            int dungeonId = data[item.m_index].DungeonId;
            dungeonNameText.text = name;
            var toggle = bind.GetCom<Toggle>("Toggle");
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                if (CheckHaveSelect(data[item.m_index].DungeonId))
                {
                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
                toggle.onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        AddSelectDungeonData(dungeonId, name);
                    }
                    else
                    {
                        RemoveSelectDungeonData(dungeonId);
                    }
                    RefreshSelectDungeonListUI();
                });
            }

        }

        /// <summary>
        /// 在选中地下城UI改变时
        /// </summary>
        /// <param name="item"></param>
        protected void OnSelectDungeonListUpdate(ComUIListElementScript item)
        {
            var bind = item.GetComponent<ComCommonBind>();
            if (bind.IsNull())
                return;
            var chapterTxt = bind.GetCom<Text>("Name");
            if (chapterTxt.IsNull())
                return;
            if (item.m_index > GetSelectDungeonDicCount())
                return;
            int dungeonId = GetSelectDungeonIdByIndex(item.m_index);
            string name = GetSelectDungeonName(dungeonId);
            chapterTxt.text = name;

            var toggle = bind.GetCom<Toggle>("Toggle");
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                if (CheckHaveSelect(dungeonId))
                {
                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
                toggle.onValueChanged.AddListener((value) =>
                {
                    if (!value)
                    {
                        RemoveSelectDungeonData(dungeonId);
                    }
                    mDungeonUIList.UpdateElement();
                    RefreshSelectDungeonListUI();
                });
            }
        }

        /// <summary>
        /// 刷新选中的地下城列表
        /// </summary>
        protected void RefreshSelectDungeonListUI()
        {
            mSelectList.ResetSelectedElementIndex();
            mSelectList.SetElementAmount(GetSelectDungeonDicCount());
            mSelectList.UpdateElement();
        }

        /// <summary>
        /// 通过下标获取已经选择关卡ID
        /// </summary>
        protected int GetSelectDungeonIdByIndex(int index)
        {
            var enumerator = AutoFightTestDataManager.instance.SelectDungeonDic.GetEnumerator();
            int curIndex = 0;
            while (enumerator.MoveNext())
            {
                if (curIndex == index)
                {
                    return enumerator.Current.Key;
                }
                else
                {
                    curIndex++;
                }
            }
            return 0;
        }

        protected List<AutoFightTest> GetChapterIdList()
        {
            return AutoFightTestDataManager.instance.ChapterIdList;
        }

        protected Dictionary<int, List<AutoFightTest>> GetChapterDataDic()
        {
            return AutoFightTestDataManager.instance.ChapterDataDic;
        }

        protected List<AutoFightTest> GetDungeonListByChapterId()
        {
            int chapterId = AutoFightTestDataManager.instance.SelChapterId;
            if (chapterId <= 0)
                return null;
            return AutoFightTestDataManager.instance.ChapterDataDic[chapterId];
        }

        protected bool CheckHaveSelect(int dungeonId)
        {
            if (AutoFightTestDataManager.instance.SelectDungeonDic.ContainsKey(dungeonId))
                return true;
            return false;
        }

        protected string GetSelectDungeonName(int dungeonId)
        {
            if (!AutoFightTestDataManager.instance.SelectDungeonDic.ContainsKey(dungeonId))
                return null;
            return AutoFightTestDataManager.instance.SelectDungeonDic[dungeonId];
        }

        protected void AddSelectDungeonData(int dungeonId, string name)
        {
            if (AutoFightTestDataManager.instance.SelectDungeonDic.ContainsKey(dungeonId))
                return;
            AutoFightTestDataManager.instance.SelectDungeonDic.Add(dungeonId, name);
        }

        protected void RemoveSelectDungeonData(int dungeonId)
        {
            if (!AutoFightTestDataManager.instance.SelectDungeonDic.ContainsKey(dungeonId))
                return;
            AutoFightTestDataManager.instance.SelectDungeonDic.Remove(dungeonId);
        }

        protected int GetSelectDungeonDicCount()
        {
            return AutoFightTestDataManager.instance.SelectDungeonDic.Count;
        }

        protected void ClearSelectDungeonDic()
        {
            AutoFightTestDataManager.instance.SelectDungeonDic.Clear();
        }

        protected void SetSelChapterId(int curSelId)
        {
            AutoFightTestDataManager.instance.SelChapterId = curSelId;
        }

        protected void SetContent()
        {
            int totalCount = AutoFightTestDataManager.instance.CurBattleRunCount;
            string lastName = AutoFightTestDataManager.instance.LastRunningDungeonName;
            mTextTitle.text = string.Format("当前选择关卡数量:{0} 当前运行局数:{1} 上一次运行关卡:{2}",GetSelectDungeonDicCount(), totalCount, lastName);
        }
    }
#endif
}