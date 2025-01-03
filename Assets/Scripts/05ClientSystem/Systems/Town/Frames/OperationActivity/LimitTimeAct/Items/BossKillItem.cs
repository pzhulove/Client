using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BossKillItem : MonoBehaviour, IDisposable
    {
        [SerializeField] private Text mTextTitle; //boss名字+随机掉落
        [SerializeField] private Text mTextRemainCout; //剩余数量
        [SerializeField] private Text mTextTimeZone; //当前波次时间区间
        [SerializeField] private RectTransform mDropItemRoot; //掉落物品根节点
        [SerializeField] Button mButtonGoChallenge; //前往挑战
        [SerializeField] GameObject mRefreshTime; //刷新剩余时间--:--:--
        [SerializeField] private Text mTextRefreshTime; //当前波次时间区间
        [SerializeField] Vector2 mComItemSize = new Vector2(100f, 100f);

        private List<ComItem> mComItems = new List<ComItem>();
        private BossKillMonsterModel mData;
        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private int mId;

        public void Init(int id, BossKillMonsterModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mId = id;
            UpdateData(data);
            _InitDropItems();
            mOnItemClick = onItemClick;
            mButtonGoChallenge.SafeAddOnClickListener(_OnButtonGoChallengeClick);
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public void UpdateData(BossKillMonsterModel data)
        {
            mData = data;
            mTextTitle.SafeSetText(string.Format(TR.Value("activity_boss_kill_item_title"), data.Name));
            mTextTimeZone.CustomActive(true);

            //小boss显示无限制，大boss显示时间
            if (data.MonsterType == MonsterType.Boss_Pos && data.IsActive)
            {
                mTextTimeZone.SafeSetText(Function.GetTime((int)data.StartTime, (int)data.EndTime));
            }
            else if (data.MonsterType == MonsterType.Elite_Pos || data.MonsterType == MonsterType.Monster_Pos)
            {
                mTextTimeZone.SafeSetText(TR.Value("activity_boss_kill_item_can_challenge_all_day"));
            }
            else
            {
                mTextTimeZone.CustomActive(false);
            }

            //设置剩余数量
            if (data.RemainNum == uint.MaxValue || (data.MonsterType == MonsterType.Boss_Pos && !data.IsActive))
            {
                mTextRemainCout.CustomActive(false);
            }
            else
            {
                mTextRemainCout.CustomActive(true);
                mTextRemainCout.SafeSetText(string.Format(TR.Value("activity_boss_kill_item_remain_count"), data.RemainNum));
            }

            //区分前往击杀按钮和时间的是否隐藏规则
            if (data.MonsterType == MonsterType.Boss_Pos)
            {
                if (!data.IsActive)
                {
                    mRefreshTime.CustomActive(true);
                    mButtonGoChallenge.CustomActive(false);
                }
				else
                {
                    if (data.RemainNum == 0)
                    {
                        mRefreshTime.CustomActive(true);
                        mButtonGoChallenge.CustomActive(false);
                    }
                    else
                    {
                        mRefreshTime.CustomActive(false);
                        mButtonGoChallenge.CustomActive(true);
                    }
                }

            }
            else if (data.MonsterType == MonsterType.Elite_Pos || data.MonsterType == MonsterType.Monster_Pos)
            {
                mRefreshTime.CustomActive(false);
                mButtonGoChallenge.CustomActive(true);
            }

        }

        public void Destroy()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }

                mComItems.Clear();
            }

            mOnItemClick = null;
            mButtonGoChallenge.SafeRemoveOnClickListener(_OnButtonGoChallengeClick);
        }

        void Update()
        {
            if (mData.NextRollStartTime > 0)
            {
                mTextRefreshTime.SafeSetText(string.Format(TR.Value("activity_boss_kill_item_refresh_time"), Function.SetShowTime((int)mData.NextRollStartTime)));
            }
            else
            {
                mTextRefreshTime.SafeSetText(TR.Value("activity_boss_kill_item_activity_end"));
            }
        }

        void _InitDropItems()
        {
            if (mData.DropList == null)
            {
                return;
            }

            for (int i = 0; i < mData.DropList.Length; ++i)
            {
                var comItem = ComItemManager.Create(mDropItemRoot.gameObject);
                if (comItem != null)
                {
                    ItemData item = ItemDataManager.CreateItemDataFromTable((int)mData.DropList[i].itemId);
                    if(item == null)
                    {
                        continue;
                    }
                    item.Count = (int)mData.DropList[i].num;
                    comItem.Setup(item, Utility.OnItemClicked);
                    mComItems.Add(comItem);
                    (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                }
            }
        }

        void _OnButtonGoChallengeClick()
        {
            if (mOnItemClick != null)
            {
                mOnItemClick(mId, 0);
            }
        }
    }
}

