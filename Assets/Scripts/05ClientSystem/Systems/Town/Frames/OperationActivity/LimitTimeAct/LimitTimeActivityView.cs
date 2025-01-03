using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(ComTwoLevelToggleGroup))]
    public sealed class LimitTimeActivityView : MonoBehaviour, IDisposable
    {
        [SerializeField] private ComTwoLevelToggleGroup mToggleGroup;
        [SerializeField] private RectTransform mFrameRoot;
        [SerializeField] private Button mButtonClose;
        [Header("大页签数量大于多少时可以滑动")]
        [SerializeField] private int mCanScrollCount = 3;

        private bool mIsParentToggleGroupScrolled = false;
        public RectTransform FrameRoot
        {
            get { return mFrameRoot; }
        }

        public UnityAction OnButtonCloseClick
        {
            set
            {
                mButtonClose.SafeRemoveAllListener();
                mButtonClose.SafeAddOnClickListener(value);
            }
        }

        public void Init(List<ITwoLevelToggleData> parentToggleDatas, List<List<ITwoLevelToggleData>> subToggleDatas, ComTwoLevelToggleGroup.OnValueChanged onParentValueChanged, ComTwoLevelToggleGroup.OnValueChanged onSubValueChanged)
        {
            if (mToggleGroup == null)
            {
                return;
            }

            mToggleGroup.Init(parentToggleDatas, subToggleDatas, onParentValueChanged, onSubValueChanged);
            mToggleGroup.SetSubGroupCanScroll(false, true);
            if (parentToggleDatas.Count >= mCanScrollCount)
            {
                mToggleGroup.SetParentGroupCanScroll(false, true);
                mIsParentToggleGroupScrolled = true;
            }
#if TEST_DELETE_ACTIVITY
            //Invoke("TestDelete", 3f);
#endif
        }

#if TEST_DELETE_ACTIVITY
        void TestDelete()
        {
            ActivityDataManager.GetInstance().TestDeleteSummaryActivities();
        }
#endif

        public void AddTopToggle(List<ITwoLevelToggleData> subToggleDatas, ITwoLevelToggleData data, int insertPosition = -1)
        {
            if (mToggleGroup == null)
            {
                return;
            }

            mToggleGroup.AddParentToggle(subToggleDatas, data, insertPosition);
            if (!mIsParentToggleGroupScrolled && mToggleGroup.GetParentToggleCount() > mCanScrollCount)
            {
                mToggleGroup.SetParentGroupCanScroll(true, false);
                mIsParentToggleGroupScrolled = true;
            }
        }

        //添加子页签节点
        public void AddSubToggle(uint parentId, ITwoLevelToggleData data, int insertPosition = -1)
        {
            if (mToggleGroup == null)
            {
                return;
            }
            mToggleGroup.AddSubToggle(parentId, data, insertPosition);
        }

        //设置子页签红点
        public void SetSubRedPoint(int parentId, int id, bool value)
        {
            if (mToggleGroup == null)
            {
                return;
            }
            mToggleGroup.SetSubRedPoint(parentId, id, value);
        }

        //设置父页签节红点
        public void SetParentRedPoint(int id, bool value)
        {
            if (mToggleGroup == null)
            {
                return;
            }
            mToggleGroup.SetParentRedPoint(id, value);
        }

        //选中某子页签
        public void GoToToggleFromID(int parentId,int subId)
        {
            if(mToggleGroup == null)
            {
                return;
            }
            mToggleGroup.SelectToggle(parentId, subId);
        }

        //移除父页签
        public void RemoveParentToggle(int id)
        {
            if (mToggleGroup == null)
            {
                return;
            }
            mToggleGroup.RemoveParentToggle(id);
        }

        //移除子页签
        public void RemoveSubToggle(uint parentId, uint id)
        {
            if (mToggleGroup == null)
            {
                return;
            }
            mToggleGroup.RemoveSubToggle(parentId, id);
        }

        public void Dispose()
        {
            if (mToggleGroup != null)
            {
                mToggleGroup.Dispose();
            }
            mIsParentToggleGroupScrolled = false;
        }
    }
}