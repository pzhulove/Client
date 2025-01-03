using DG.Tweening;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class OnlineActiveAwardItem : MonoBehaviour
    {
        [SerializeField] ComItemNew mComItem;
        [SerializeField] TextEx mTextTime;
        [SerializeField] ButtonEx mBtnOperate;
        [SerializeField] GameObject mGoGotAward;
        [SerializeField] TextEx mTextGot;
        [SerializeField] CanvasGroup mCanvasItem;
        [SerializeField] float mGotAlpha = 0.6f;
        [SerializeField] StateController mStateController;
        [SerializeField] DOTweenAnimation mCanGetAwardAni = null; // 可领奖时动画
        [SerializeField] float mCanGetAwardAniStartScale = 0.3f;//可领取动画初始缩放

        private TaskStatus mTaskStatus;
        private UnityAction mGetAwardAction;
        private ActiveManager.ActivityData mActivityData;
        private bool mIsUpdating = false;

        public void Init(ItemData itemData, ActiveManager.ActivityData activityData, UnityAction getAward, bool isUpdating = false)
        {
            mGetAwardAction = getAward;
            mActivityData = activityData;
            mIsUpdating = isUpdating;

            mComItem.Setup(itemData, null, true);
            if (itemData.Count > 1)
            {
                mComItem.SetCount(itemData.Count.ToString());
            }
            _InitStatus((TaskStatus)activityData.status);
        }

        private void _InitStatus(TaskStatus taskStatus)
        {
            TaskStatus preStatus = mTaskStatus;
            mTaskStatus = taskStatus;

            switch (mTaskStatus)
            {
                case TaskStatus.TASK_UNFINISH:
                    {
                        if (!mIsUpdating)
                        {
                            mTextTime.SafeSetText(string.Format(TR.Value("online_active_time_format"), mActivityData.activeItem.Param0));
                        }
                    }
                    break;
                case TaskStatus.TASK_OVER:
                    {
                        mTextTime.SafeSetText(string.Format(TR.Value("online_active_time_format"), mActivityData.activeItem.Param0));
                    }
                    break;
                case TaskStatus.TASK_FINISHED:
                    {
                        
                    }
                    break;
            }

            if (mStateController != null)
            {
                mStateController.Key = ((int)taskStatus).ToString();
            }

            if (mCanvasItem != null)
            {
                if (taskStatus == TaskStatus.TASK_OVER)
                {
                    mCanvasItem.alpha = mGotAlpha;
                }
                else
                {
                    mCanvasItem.alpha = 1;
                }
            }

            if (mCanGetAwardAni != null)
            {
                if (taskStatus == TaskStatus.TASK_FINISHED && preStatus != TaskStatus.TASK_FINISHED)
                {
                    mCanGetAwardAni.transform.localScale = Vector3.one * mCanGetAwardAniStartScale;
                    mCanGetAwardAni.DORestart();
                }
                else
                {
                    mCanGetAwardAni.DOPause();
                    mCanGetAwardAni.transform.localEulerAngles = Vector3.zero;
                    mCanGetAwardAni.transform.localScale = Vector3.one;
                }
            }
        }

        public void UpdateTimeText(string timeLeftStr)
        {
            mTextTime.SafeSetText(timeLeftStr);
        }

        public void BtnOperateClick()
        {
            switch (mTaskStatus)
            {
                case TaskStatus.TASK_UNFINISH:
                    {

                    }
                    break;
                case TaskStatus.TASK_FINISHED:
                    {
                        if (mGetAwardAction != null)
                        {
                            mGetAwardAction();
                        }
                    }
                    break;
                case TaskStatus.TASK_OVER:
                    {

                    }
                    break;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}