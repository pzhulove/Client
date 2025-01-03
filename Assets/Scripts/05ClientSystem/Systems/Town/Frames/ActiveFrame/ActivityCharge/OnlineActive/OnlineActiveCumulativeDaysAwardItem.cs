using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class OnlineActiveCumulativeDaysAwardItem : MonoBehaviour
    {
        [SerializeField] private ComItemNew mComItem;
        [SerializeField] private TextEx mTextDays;
        [SerializeField] private StateController mStateController;

        private TaskStatus mTaskStatus;
        private UnityAction mGetAwardAction;
        private ActiveManager.ActivityData mActivityData;

        public void Init(ItemData itemData, ActiveManager.ActivityData activityData, UnityAction getAward)
        {
            mGetAwardAction = getAward;
            mActivityData = activityData;
            mTaskStatus = (TaskStatus)activityData.status;

            mStateController.Key = activityData.status.ToString();
            mTextDays.SafeSetText(string.Format(TR.Value("online_active_cumulative_days_format"), activityData.activeItem.Param0));
            mComItem.Setup(itemData, null, true);
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
