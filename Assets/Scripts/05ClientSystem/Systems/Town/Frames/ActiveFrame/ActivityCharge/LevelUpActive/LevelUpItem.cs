using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class LevelUpItem : MonoBehaviour
    {
        [SerializeField] private StateController mStateController = null;
        [SerializeField] private ComUIListScript mComUIList = null;
        [SerializeField] private TextEx mTextDesc = null;

        private bool mInited = false;
        private ActiveManager.ActivityData mActivityData = null;
        private List<ItemData> mAwards = null;
        private UnityAction<int> mGetAwardClick = null;

        public void Init(List<ItemData> itemDatas, ActiveManager.ActivityData activityData, UnityAction<int> getAwardClick, string desc)
        {
            if (!mInited)
            {
                if (mComUIList != null)
                {
                    mComUIList.Initialize();
                    mComUIList.onItemVisiable = _OnItemUpdate;
                    mComUIList.OnItemUpdate = _OnItemUpdate;
                }

                mInited = true;
            }

            mAwards = itemDatas;
            mActivityData = activityData;
            mGetAwardClick = getAwardClick;

            mTextDesc.SafeSetText(desc);

            if (mComUIList != null)
            {
                mComUIList.SetElementAmount(mAwards.Count);
            }

            if (mStateController != null)
            {
                mStateController.Key = ((int)activityData.status).ToString();
            }
        }

        private void _OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mAwards == null || item.m_index >= mAwards.Count || mAwards[item.m_index] == null)
            {
                return;
            }

            ComItemNew com = item.GetComponentInChildren<ComItemNew>();
            if (com != null)
            {
                com.Setup(mAwards[item.m_index]);
                if (mAwards[item.m_index].Count > 1)
                {
                    com.SetCount(mAwards[item.m_index].Count.ToString());
                }
            }
        }

        public void GetAwardClick()
        {
            if (mGetAwardClick != null && mActivityData != null)
            {
                mGetAwardClick(mActivityData.ID);
            }
        }
    }
}
