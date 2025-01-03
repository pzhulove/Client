using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysTargetScoreItem : MonoBehaviour
    {
        [SerializeField] private ImageEx mImageIcon = null;
        [SerializeField] private TextEx mTextNum = null;
        [SerializeField] private TextEx mTextScore = null;
        [SerializeField] private StateController mStateController = null;
        [SerializeField] private CanvasGroup mCanvasItem = null;
        [SerializeField] private CanvasGroup mCanvasMain = null;
        [SerializeField] private float mMainGotAlpha = 0.6f;

        private int mActiveId = -1;
        private SevenDaysFrame mSevenDaysFrame = null;
        private ItemData mItemData = null;

        public void Init(SevenDaysScoreData sevenDaysData, SevenDaysFrame sevenDaysFrame)
        {
            if (sevenDaysData == null)
            {
                return;
            }

            if (mStateController != null)
            {
                mStateController.Key = ((int)sevenDaysData.taskStatus).ToString();
            }

            if (mCanvasMain != null)
            {
                mCanvasMain.alpha = sevenDaysData.taskStatus == TaskStatus.TASK_OVER ? mMainGotAlpha : 1;
            }

            if (sevenDaysData.itemDatas != null && sevenDaysData.itemDatas.Count > 0)
            {
                mItemData = sevenDaysData.itemDatas[0];
                mImageIcon.SafeSetImage(mItemData.Icon);
                mTextNum.SafeSetText(mItemData.Count.ToString());
                mTextScore.SafeSetText(sevenDaysData.targetScore.ToString());
            }

            mSevenDaysFrame = sevenDaysFrame;
            mActiveId = sevenDaysData.activeId;
        }

        public void SetItemActive(bool isShow)
        {
            mCanvasItem.CustomActive(isShow);
        }

        public void GetAwardClick()
        {
            if (mSevenDaysFrame == null || mActiveId < 0)
            {
                return;
            }

            mSevenDaysFrame.SubmitActive(mActiveId);
        }

        public void ShowTips()
        {
            if (mSevenDaysFrame != null)
            {
                mSevenDaysFrame.ShowTips(mItemData);
            }
        }
    }
}
