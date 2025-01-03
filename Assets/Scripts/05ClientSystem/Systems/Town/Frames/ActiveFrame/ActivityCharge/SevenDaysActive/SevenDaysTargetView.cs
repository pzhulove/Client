using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysTargetView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mTabList = null;
        [SerializeField] private ComUIListScript mContentList = null;
        [SerializeField] private TextEx mTextScore = null;
        [SerializeField] private Slider mSliderScore = null;
        [SerializeField] private Transform mTrScoreItemParent = null;
        [SerializeField] private SevenDaysTargetScoreItem mScoreItem = null;

        private List<SevenDaysTargetData> mSevenDaysTargetDatas = null;
        private List<SevenDaysScoreData> mSevenDaysScoreDatas = null;
        private List<SevenDaysTargetScoreItem> mSevenDaysTargetScoreItems = null;
        private SevenDaysFrame mSevenDaysFrame = null;
        private int mCurSelectDay = -1;
        private bool mInited = false;
        public void Init(SevenDaysFrame sevenDaysFrame)
        {
            if (!mInited)
            {
                if (mTabList != null)
                {
                    mTabList.Initialize();
                    mTabList.onItemVisiable = _OnTabItemUpdate;
                    mTabList.OnItemUpdate = _OnTabItemUpdate;
                    mTabList.onItemChageDisplay = _OnChageDisplay;
                    mTabList.onItemSelected = _OnSelect;
                }

                if (mContentList != null)
                {
                    mContentList.Initialize();
                    mContentList.onItemVisiable = _OnContentItemUpdate;
                    mContentList.OnItemUpdate = _OnContentItemUpdate;
                }

                mSevenDaysFrame = sevenDaysFrame;
                mInited = true;
            }

            if (mTabList != null)
            {
                mTabList.SetElementAmount(SevendaysDataManager.MaxDay);
                int pre = mCurSelectDay < 0 ? 1 : mCurSelectDay;
                mTabList.SelectElement(pre - 1);
            }

            _UpdateScorePorgress();
            UpdateScore();
        }

        public void UpdateView()
        {
            _UpdateContent();

            if (mTabList != null)
            {
                mTabList.SetElementAmount(SevendaysDataManager.MaxDay);
            }
        }

        /// <summary>
        /// 积分奖励相关的状态改编
        /// </summary>
        public void UpdateScoreAndTab()
        {
            if (mTabList != null)
            {
                mTabList.SetElementAmount(SevendaysDataManager.MaxDay);
            }


            _UpdateScorePorgress();
        }

        /// <summary>
        /// 刷新积分
        /// </summary>
        public void UpdateScore()
        {
            mSevenDaysScoreDatas = SevendaysDataManager.GetInstance().GetSevenDaysScoreDatas();
            int curValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_SEVENDAYS_ACTIVITY_POINT);
            int maxScore = 0;
            if (mSevenDaysScoreDatas != null && mSevenDaysScoreDatas.Count > 0 && mSevenDaysScoreDatas[mSevenDaysScoreDatas.Count - 1] != null)
            {
                maxScore = mSevenDaysScoreDatas[mSevenDaysScoreDatas.Count - 1].targetScore;
            }
            int minScore = 0;
            if (mSevenDaysScoreDatas != null && mSevenDaysScoreDatas.Count > 0 && mSevenDaysScoreDatas[0] != null)
            {
                minScore = mSevenDaysScoreDatas[0].targetScore;
            }
            if (maxScore - minScore > 0)
            {
                mSliderScore.SafeSetValue((curValue - minScore) / ((float)maxScore - minScore));
            }

            mTextScore.SafeSetText(curValue.ToString());
        }

        private void _UpdateScorePorgress()
        {
            mSevenDaysScoreDatas = SevendaysDataManager.GetInstance().GetSevenDaysScoreDatas();
            if (mSevenDaysScoreDatas == null || mSevenDaysScoreDatas.Count <= 0)
            {
                return;
            }

            if (mSevenDaysTargetScoreItems == null)
            {
                mSevenDaysTargetScoreItems = new List<SevenDaysTargetScoreItem>();

                for (int i = 0; i < mSevenDaysScoreDatas.Count; i++)
                {
                    SevenDaysTargetScoreItem item = Instantiate(mScoreItem, mTrScoreItemParent);
                    mSevenDaysTargetScoreItems.Add(item);
                    if (item != null && mSevenDaysScoreDatas.Count > i)
                    {
                        item.Init(mSevenDaysScoreDatas[i], mSevenDaysFrame);
                        item.SetItemActive(true);
                        _UpdateScoreItemPos(mSevenDaysScoreDatas[i], item);
                    }
                }
            }
            else
            {
                int index = 0;
                foreach (var v in mSevenDaysTargetScoreItems)
                {
                    if (mSevenDaysScoreDatas.Count <= index)
                    {
                        break;
                    }

                    if (v == null)
                    {
                        continue;
                    }

                    v.Init(mSevenDaysScoreDatas[index], mSevenDaysFrame);
                    index++;
                }
            }
        }

        private void _UpdateScoreItemPos(SevenDaysScoreData sevenDaysData, SevenDaysTargetScoreItem item)
        {
            if (sevenDaysData == null || item == null)
            {
                return;
            }

            int maxScore = 0;
            if (mSevenDaysScoreDatas != null && mSevenDaysScoreDatas.Count > 0 && mSevenDaysScoreDatas[mSevenDaysScoreDatas.Count - 1] != null)
            {
                maxScore = mSevenDaysScoreDatas[mSevenDaysScoreDatas.Count - 1].targetScore;
            }
            int minScore = 0;
            if (mSevenDaysScoreDatas != null && mSevenDaysScoreDatas.Count > 0 && mSevenDaysScoreDatas[0] != null)
            {
                minScore = mSevenDaysScoreDatas[0].targetScore;
            }

            int curScore = sevenDaysData.targetScore;

            if (maxScore - minScore > 0)
            {
                float rate = (curScore - minScore) / ((float)maxScore - minScore);
                item.transform.localPosition = new Vector3(item.transform.localPosition.x, -mSliderScore.transform.rectTransform().rect.height * rate, 0);
            }
        }

        private void _UpdateContent()
        {
            mCurSelectDay = mCurSelectDay < 0 ? 1 : mCurSelectDay;
            mSevenDaysTargetDatas = SevendaysDataManager.GetInstance().GetSevenDaysTargetDatas(mCurSelectDay);
            if (mContentList != null && mSevenDaysTargetDatas != null)
            {
                mContentList.SetElementAmount(mSevenDaysTargetDatas.Count);
            }
        }

        private void _OnTabItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (item.m_index >= SevendaysDataManager.MaxDay)
            {
                return;
            }

            SevenDaysTargetTabItem tabItem = item.GetComponent<SevenDaysTargetTabItem>();
            if (tabItem != null)
            {
                int day = item.m_index + 1;
                tabItem.Init(mCurSelectDay == day, !SevendaysDataManager.GetInstance().IsTheDayUnLock(day), day, SevendaysDataManager.GetInstance().IsTargetShowRedPointByDay(day));
            }
        }

        private void _OnChageDisplay(ComUIListElementScript item, bool isSelect)
        {
            if (item == null)
            {
                return;
            }

            SevenDaysTargetTabItem tabItem = item.GetComponent<SevenDaysTargetTabItem>();
            int day = item.m_index + 1;

            if (tabItem != null)
            {
                tabItem.Init(isSelect, !SevendaysDataManager.GetInstance().IsTheDayUnLock(day), day, SevendaysDataManager.GetInstance().IsTargetShowRedPointByDay(day));
            }
        }

        private void _OnSelect(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mCurSelectDay == item.m_index + 1)
            {
                return;
            }

            mCurSelectDay = item.m_index + 1;
            _UpdateContent();
            if (mContentList != null)
            {
                mContentList.ScrollToItem(0);
            }
        }

        private void _OnContentItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysTargetDatas == null || item.m_index >= mSevenDaysTargetDatas.Count)
            {
                return;
            }

            SevenDaysTargetContentItem sevenDaysTargetContentItem = item.GetComponent<SevenDaysTargetContentItem>();
            if (sevenDaysTargetContentItem != null)
            {
                sevenDaysTargetContentItem.Init(mSevenDaysTargetDatas[item.m_index], mSevenDaysFrame);
            }
        }

        private void _OnScoreItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysScoreDatas == null || mSevenDaysScoreDatas.Count <= item.m_index)
            {
                return;
            }

            SevenDaysData scoreData = mSevenDaysScoreDatas[item.m_index];
            if (scoreData == null || scoreData.itemDatas == null
                || scoreData.itemDatas.Count <= 0 || scoreData.itemDatas[0] == null)
            {
                return;
            }


        }
    }
}
