using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using Protocol;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnOPPOGrowthHaoLiItemClick(int iActivityId);
    public class OPPOGrowthHaoLiItem : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mRewardItemUIList;
        [SerializeField]private Text mReceiveCondition;
        [SerializeField]private Button mReceiveBtn;
        [SerializeField]private UIGray mReceiveGray;
        [SerializeField]private GameObject mReceivedRoot;
        [SerializeField]private Slider mSlider;
        [SerializeField]private Text mSliderText;

        private OnOPPOGrowthHaoLiItemClick mOnOPPOGrowthHaoLiItemClick;
        private List<AwardItemData> mRewardItemList = new List<AwardItemData>();
        private void Awake()
        {
            if (mRewardItemUIList != null)
            {
                mRewardItemUIList.Initialize();
                mRewardItemUIList.onBindItem += OnBindItemDelegate;
                mRewardItemUIList.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        /// <summary>
        /// 初始化item
        /// </summary>
        /// <param name="activityData">活动数据</param>
        /// <param name="onAmberPrivilegeItemClick">领取回调</param>
        public void OnItemVisiable(ActiveManager.ActivityData activityData, OnOPPOGrowthHaoLiItemClick onOPPOGrowthHaoLiItemClick)
        {
            if (activityData == null)
            {
                return;
            }

            mOnOPPOGrowthHaoLiItemClick = onOPPOGrowthHaoLiItemClick;

            if (mReceiveCondition != null)
            {
                mReceiveCondition.text = activityData.activeItem.Desc;
            }

            SetSlider(activityData);

            CreatRewardItem(activityData);

            UpdateStatus(activityData);

            if (mReceiveBtn != null)
            {
                mReceiveBtn.onClick.RemoveAllListeners();
                mReceiveBtn.onClick.AddListener(() =>
                {
                    if (mOnOPPOGrowthHaoLiItemClick != null)
                    {
                        mOnOPPOGrowthHaoLiItemClick.Invoke(activityData.ID);
                    }
                });
            }
        }

        private void SetSlider(ActiveManager.ActivityData activityData)
        {
            if (activityData == null)
            {
                return;
            }

            int value = 0;
            for (int i = 0; i < activityData.akActivityValues.Count; i++)
            {
                if (activityData.akActivityValues[i].key == "num")
                {
                    int.TryParse(activityData.akActivityValues[i].value, out value);
                }
            }

            if (mSlider != null)
            {
                mSlider.value = Mathf.Clamp01(value * 1.0f / activityData.activeItem.Param1);
            }

            if (mSliderText != null)
            {
                mSliderText.text = string.Format("{0}/{1}", value, activityData.activeItem.Param1);
            }
        }

        private void CreatRewardItem(ActiveManager.ActivityData activityData)
        {
            if (activityData == null)
            {
                return;
            }

            mRewardItemList = ActiveManager.GetInstance().GetActiveAwards(activityData.ID);

            if (mRewardItemUIList != null)
            {
                mRewardItemUIList.SetElementAmount(mRewardItemList.Count);
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="activityData"></param>
        private void UpdateStatus(ActiveManager.ActivityData activityData)
        {
            if (activityData == null)
            {
                return;
            }
            
            if (activityData.status == (int)TaskStatus.TASK_UNFINISH ||
                activityData.status == (int)TaskStatus.TASK_INIT)
            {
                SetReceiveGray(true);

                SetReceiveBtn(true);

                SetReceivedRoot(false);
            }
            else if (activityData.status == (int)TaskStatus.TASK_FINISHED)
            {
                SetReceiveGray(false);

                SetReceiveBtn(true);

                SetReceivedRoot(false);
            }
            else if (activityData.status == (int)TaskStatus.TASK_OVER ||
                     activityData.status == (int)TaskStatus.TASK_SUBMITTING)
            {
                SetReceiveBtn(false);

                SetReceivedRoot(true);
            }
        }

        private void SetReceiveGray(bool isFlag)
        {
            if (mReceiveGray != null)
            {
                mReceiveGray.enabled = isFlag;
            }
        }

        private void SetReceiveBtn(bool isFlag)
        {
            if (mReceiveBtn != null)
            {
                mReceiveBtn.gameObject.CustomActive(isFlag);
            }
        }

        private void SetReceivedRoot(bool isFlag)
        {
            if (mReceivedRoot != null)
            {
                mReceivedRoot.CustomActive(isFlag);
            }
        }
        
        private OPPOGrowthHaoLiRewardItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<OPPOGrowthHaoLiRewardItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            OPPOGrowthHaoLiRewardItem mOPPOGrowthHaoLiRewardItem = item.gameObjectBindScript as OPPOGrowthHaoLiRewardItem;
            if (mOPPOGrowthHaoLiRewardItem != null && item.m_index >= 0 && item.m_index < mRewardItemList.Count)
            {
                mOPPOGrowthHaoLiRewardItem.OnItemVisiable(mRewardItemList[item.m_index]);
            }
        }

        private void OnDestroy()
        {
            if (mRewardItemUIList != null)
            {
                mRewardItemUIList.onBindItem -= OnBindItemDelegate;
                mRewardItemUIList.onItemVisiable -= OnItemVisiableDelegate;
            }
            mOnOPPOGrowthHaoLiItemClick = null;
            mRewardItemList.Clear();
        }
    }
}
