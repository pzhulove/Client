using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Scripts.UI;
using Protocol;

namespace GameClient
{
    public delegate void OnAmberPrivilegeItemClick(int iActivityId);

    public class AmberPrivilegeItem : MonoBehaviour
    {
        [SerializeField]private GameObject mItemRoot;
        [SerializeField]private Text mReceiveCondition;
        [SerializeField]private Button mReceiveBtn;
        [SerializeField]private UIGray mReceiveGray;
        [SerializeField]private GameObject mReceivedRoot;

        private OnAmberPrivilegeItemClick mOnAmberPrivilegeItemClick;
        private ActiveManager.ActivityData mActivityData;
        private ComItem comItem = null;
        /// <summary>
        /// 初始化琥珀item
        /// </summary>
        /// <param name="activityData">活动数据</param>
        /// <param name="onAmberPrivilegeItemClick">领取回调</param>
        public void OnItemVisiable(ActiveManager.ActivityData activityData ,OnAmberPrivilegeItemClick onAmberPrivilegeItemClick)
        {
            mActivityData = activityData;
            mOnAmberPrivilegeItemClick = onAmberPrivilegeItemClick;

            List<AwardItemData> awardList = ActiveManager.GetInstance().GetActiveAwards(mActivityData.ID);
            if (awardList.Count > 0)
            {
                //只有一个元素
                if (awardList[0] == null)
                {
                    return;
                }

                int itemId = awardList[0].ID;
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemId);
                if (mItemRoot != null)
                {
                    if (comItem == null)
                    {
                        comItem = ComItemManager.Create(mItemRoot);
                    }

                    comItem.Setup(itemData, Utility.OnItemClicked);
                }
            }

            UpdateStatus(mActivityData);

            if (mReceiveBtn != null)
            {
                mReceiveBtn.onClick.RemoveAllListeners();
                mReceiveBtn.onClick.AddListener(() => 
                {
                    if (mOnAmberPrivilegeItemClick != null)
                    {
                        mOnAmberPrivilegeItemClick.Invoke(mActivityData.ID);
                    }
                });
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

            SetReceiveCondition(activityData);

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

        /// <summary>
        /// 设置领取条件描述
        /// </summary>
        /// <param name="activityData"></param>
        private void SetReceiveCondition(ActiveManager.ActivityData activityData)
        {
            if (mReceiveCondition != null)
            {
                mReceiveCondition.text = activityData.activeItem.Desc;
            }
        }
        
        private void OnDestroy()
        {
            mActivityData = null;
            mOnAmberPrivilegeItemClick = null;
            comItem = null;
        }
    }
}

