using Protocol;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class LevelUpActiveView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIList;
        [SerializeField] private CanvasGroup mCanvasBtnLeft;
        [SerializeField] private CanvasGroup mCanvasBtnRight;
        [SerializeField] private CanvasGroup mCanvasRedpointLeft;
        [SerializeField] private CanvasGroup mCanvasRedpointRight;


        private Dictionary<int, List<ActiveManager.ActivityData>> mDicActivityDatas;  //key 页码  value  活动
        private ActiveManager.ActiveData mActiveData;
        private int mCurIndex = -1;
        private const string mKey = "DayOnline";
        private const int mTemplateId = 4000;
        private const int mPerPageNum = 4;

        private ActiveManager.ActiveMainUpdateKey mFindKey;
        private float mTime;
        private int mIAccumulatedTime = 0;
        private Double mDRecievedTime = 0;
        private bool mInited = false;

        private void _InitFindKey()
        {
            if (mActiveData == null)
            {
                return;
            }

            mFindKey = null;
            for (int i = 0; i < mActiveData.updateMainKeys.Count; ++i)
            {
                if (mActiveData.updateMainKeys[i].key == mKey)
                {
                    mFindKey = mActiveData.updateMainKeys[i];
                    break;
                }
            }

            mDRecievedTime = mFindKey.fRecievedTime;
            mIAccumulatedTime = ActiveManager.GetInstance().GetTemplateUpdateValue(mTemplateId, mFindKey.key);
        }

        private void _Init(bool isFirst = false)
        {
            mActiveData = ActiveManager.GetInstance().GetActiveData(mTemplateId);

            _InitDicActivityDatas();

            if (isFirst)
            {
                mCurIndex = _GetDefaultPageIndex();
            }
            else
            {
                mCurIndex = _UpdatePageIndexWhenGetAward();
            }

            _InitAwards();
            //_InitFindKey();
        }

        private void _InitDicActivityDatas()
        {
            if (mActiveData == null || mActiveData.akChildItems == null)
            {
                return;
            }

            if (mDicActivityDatas == null)
            {
                mDicActivityDatas = new Dictionary<int, List<ActiveManager.ActivityData>>();
            }
            else
            {
                mDicActivityDatas.Clear();
            }

            int pageIndex = 0;
            for (int i = 0; i < mActiveData.akChildItems.Count; i++)
            {
                ActiveManager.ActivityData activityData = mActiveData.akChildItems[i];
                if (activityData == null)
                {
                    continue;
                }

                if (mDicActivityDatas.ContainsKey(pageIndex))
                {
                    if (mDicActivityDatas[pageIndex] == null)
                    {
                        mDicActivityDatas[pageIndex] = new List<ActiveManager.ActivityData>();
                    }

                    if (mDicActivityDatas[pageIndex].Count >= 4)
                    {
                        pageIndex++;
                        i--;

                        continue;
                    }

                    mDicActivityDatas[pageIndex].Add(activityData);
                }
                else
                {
                    mDicActivityDatas.Add(pageIndex, new List<ActiveManager.ActivityData>());
                    mDicActivityDatas[pageIndex].Add(activityData);
                }
            }
        }

        private void _InitAwards()
        {
            if (mDicActivityDatas == null || mCurIndex < 0 || mDicActivityDatas.Count <= mCurIndex)
            {
                return;
            }

            _UpdatePage();
        }

        private List<ItemData> _GetItemDatas(ActiveManager.ActivityData data)
        {
            Dictionary<uint, int> dicAwards = data.GetAwards();
            List<ItemData> itemDatas = new List<ItemData>();
            if (dicAwards != null)
            {
                foreach (uint key in dicAwards.Keys)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)key);
                    itemData.Count = dicAwards[key];
                    itemDatas.Add(itemData);
                }
            }

            return itemDatas;
        }

        private void _GetAward(int id)
        {
            if (id >= 0)
            {
                ActiveManager.GetInstance().SendSubmitActivity(id);
            }
        }

        private void OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            if (data.activeItem.TemplateID == mTemplateId)
            {
                _Init();
            }
        }

        private void _OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mDicActivityDatas == null || mCurIndex < 0 || mDicActivityDatas.Count < mCurIndex)
            {
                return;
            }

            if (item.m_index >= mDicActivityDatas[mCurIndex].Count)
            {
                return;
            }

            ActiveManager.ActivityData activityData = mDicActivityDatas[mCurIndex][item.m_index];

            var script = item.GetComponent<LevelUpItem>();
            if (script != null)
            {
                script.Init(_GetItemDatas(activityData), activityData, _GetAward, _GetActiveTableDesc(activityData.activeItem.InitDesc));
            }
        }

        private void _UpdatePage()
        {
            if (mDicActivityDatas == null || mCurIndex < 0 || mDicActivityDatas.Count <= mCurIndex)
            {
                return;
            }

            mCanvasRedpointLeft.CustomActive(_HaveRedPoint(true));
            mCanvasRedpointRight.CustomActive(_HaveRedPoint(false));
            mCanvasBtnLeft.CustomActive(mCurIndex > 0);
            mCanvasBtnRight.CustomActive(mCurIndex < mDicActivityDatas.Count - 1);

            if (mComUIList != null)
            {
                mComUIList.SetElementAmount(mDicActivityDatas[mCurIndex].Count);
            }
        }

        public void BtnArrowClick(bool isLeft)
        {
            if (isLeft)
            {
                if (mCurIndex <= 0)
                {
                    mCurIndex = 0;
                }
                else
                {
                    mCurIndex--;
                }
            }
            else
            {
                int maxNum = mDicActivityDatas == null ? 0 : mDicActivityDatas.Count;
                if (mCurIndex >= maxNum - 1)
                {
                    mCurIndex = maxNum - 1;
                }
                else
                {
                    mCurIndex++;
                }

            }

            _UpdatePage();
        }

        private string _GetActiveTableDesc(string strResolved, int firstIndex = 0, int secondIndex = 4)
        {
            string strDesc = string.Empty;

            if (string.IsNullOrEmpty(strResolved))
            {
                return strDesc;
            }

            var descs = strResolved.Split(new char[] { '\r', '\n' });
            if (descs != null && descs.Length > firstIndex && !string.IsNullOrEmpty(descs[firstIndex]))
            {
                Regex s_regex_tabinit = new Regex(@"<prefabkey=(\w+) localpath=([A-Za-z0-9/]+) type=(\w+) value=(.+)>");
                Match match = s_regex_tabinit.Match(descs[firstIndex]);

                if (match != null && match.Groups != null && match.Groups.Count > secondIndex)
                {
                    strDesc = match.Groups[secondIndex].Value;
                }
            }

            return strDesc;
        }

        private bool _HaveRedPoint(bool isLeft)
        {
            if (mDicActivityDatas == null || mDicActivityDatas.Count - 1 < mCurIndex || mCurIndex < 0)
            {
                return false;
            }

            int start = 0;
            int end = 0;
            if (isLeft)
            {
                start = 0;
                end = mCurIndex;
            }
            else
            {
                start = mCurIndex + 1;
                end = mDicActivityDatas.Count;
            }

            for (int i = start; i < end; i++)
            {
                if (!mDicActivityDatas.ContainsKey(i) || mDicActivityDatas[i] == null)
                {
                    continue;
                }

                foreach (var v in mDicActivityDatas[i])
                {
                    if (v == null)
                    {
                        continue;
                    }

                    if (v.status == (int)TaskStatus.TASK_FINISHED)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int _UpdatePageIndexWhenGetAward()
        {
            int index = 0;

            if (mDicActivityDatas == null)
            {
                return index;
            }

            if (mDicActivityDatas.Count - 1 <= mCurIndex)
            {
                return mDicActivityDatas.Count - 1;
            }

            if (mDicActivityDatas.ContainsKey(mCurIndex))
            {
                for(int i = 0; i < mDicActivityDatas[mCurIndex].Count; i++)
                {
                    if (mDicActivityDatas[mCurIndex][i].status != (int)TaskStatus.TASK_OVER)
                    {
                        return mCurIndex;
                    }
                }

                return mCurIndex + 1;
            }

            return index;
        }

        private int _GetDefaultPageIndex()
        {
            int index = 0;

            if (mDicActivityDatas == null)
            {
                return index;
            }

            for (int i = 0; i < mDicActivityDatas.Count; i++)
            {
                if (!mDicActivityDatas.ContainsKey(i))
                {
                    continue;
                }

                if (mDicActivityDatas[i] == null)
                {
                    continue;
                }

                for(int j = 0; j < mDicActivityDatas[i].Count; j ++)
                {
                    if (mDicActivityDatas[i][j] == null)
                    {
                        continue;
                    }

                    if (mDicActivityDatas[i][j].status == (int)TaskStatus.TASK_FINISHED)
                    {
                        return i;
                    }
                }
            }

            return index;
        }

        // Start is called before the first frame update
        void Start()
        {
            mTime = Time.time;
            ActiveManager.GetInstance().onActivityUpdate += OnActivityUpdate;

            if (mComUIList != null && !mInited)
            {
                mComUIList.Initialize();
                mComUIList.OnItemUpdate = _OnItemUpdate;
                mComUIList.onItemVisiable = _OnItemUpdate;

                mInited = true;
            }

            _Init(true);
        }

        private void OnDestroy()
        {
            ActiveManager.GetInstance().onActivityUpdate -= OnActivityUpdate;
        }
    }
}
