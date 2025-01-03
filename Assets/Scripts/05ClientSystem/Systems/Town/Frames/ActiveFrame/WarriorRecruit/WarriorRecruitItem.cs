using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class WarriorRecruitItem : MonoBehaviour
    {
        [SerializeField] private Text mTaskName;
        [SerializeField] private Text mTaskProgress;
        [SerializeField] private GameObject mRewardParent;
        [SerializeField] private Button mReceiveBtn;
        [SerializeField] private Button mGoBtn;
        [SerializeField] private GameObject mUnfinished;
        [SerializeField] private GameObject mHaveRreceive;
        [SerializeField] private GameObject mItemPrefab;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Button mGlassBtn;
        [SerializeField] private GameObject mBgGo;

        private WarriorRecruitTaskDataModel mTaskDataModel;

        private List<ComCommonBind> rewardsComBindList = new List<ComCommonBind>();

        public void OnItemVisiable(WarriorRecruitTaskDataModel taskData,int index)
        {
            mTaskDataModel = taskData;

            if (mTaskDataModel == null)
            {
                return;
            }
            
            if (mTaskName != null)
            {
                mTaskName.text = mTaskDataModel.taskDesc;
            }

            if (mGlassBtn != null)
            {
                mGlassBtn.CustomActive(mTaskDataModel.taskType == 9 && mTaskDataModel.identify == (int)RecruitIdentify.RI_OLDMAN);
            }

            mGlassBtn.SafeRemoveAllListener();
            mGlassBtn.SafeAddOnClickListener(OnGlassBtnClick);

            mReceiveBtn.SafeRemoveAllListener();
            mReceiveBtn.SafeAddOnClickListener(OnReceiveBtnClick);

            mGoBtn.SafeRemoveAllListener();
            mGoBtn.SafeAddOnClickListener(OnGoBtnClick);

            CreatRewardItem(mTaskDataModel);
            UpdateTaskInfo(mTaskDataModel);

            mBgGo.CustomActive(index % 2 == 0);
        }

        private void CreatRewardItem(WarriorRecruitTaskDataModel data)
        {
            for (int i = 0; i < rewardsComBindList.Count; i++)
            {
                rewardsComBindList[i].CustomActive(false);
            }

            for (int i = 0; i < data.rewards.Count; i++)
            {
                var reward = data.rewards[i];
                if (reward == null)
                {
                    continue;
                }

                if (i < rewardsComBindList.Count)
                {
                    var bind = rewardsComBindList[i];
                    RefreshTaskRewardInfo(bind, reward);
                }
                else
                {
                    GameObject comItem = GameObject.Instantiate(mItemPrefab);
                    ComCommonBind mBind = comItem.GetComponent<ComCommonBind>();
                    if (mBind == null)
                    {
                        continue;
                    }

                    RefreshTaskRewardInfo(mBind, reward);
                    Utility.AttachTo(comItem, mItemParent);
                    rewardsComBindList.Add(mBind);
                }
            }
        }

        private void RefreshTaskRewardInfo(ComCommonBind bind,ItemSimpleData reward)
        {
            bind.CustomActive(true);

            Image backgroud = bind.GetCom<Image>("backgroud");
            Image icon = bind.GetCom<Image>("Icon");
            Button iconBtn = bind.GetCom<Button>("Iconbtn");
            Text count = bind.GetCom<Text>("Count");

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(reward.ItemID);
            itemData.Count = reward.Count;

            if (backgroud != null)
            {
                ETCImageLoader.LoadSprite(ref backgroud, itemData.GetQualityInfo().Background);
            }

            if (icon != null)
            {
                ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
            }

            if (count != null)
            {
                count.text = itemData.Count.ToString();
            }

            iconBtn.SafeRemoveAllListener();
            iconBtn.SafeAddOnClickListener(() => { ItemTipManager.GetInstance().ShowTip(itemData); });
        }

        private void UpdateTaskInfo(WarriorRecruitTaskDataModel data)
        {
            if (mTaskProgress != null)
            {
                mTaskProgress.text = string.Format("{0}/{1}", data.cnt, data.fullcnt);
            }

            mReceiveBtn.CustomActive(false);
            mUnfinished.CustomActive(false);
            mHaveRreceive.CustomActive(false);
            mGoBtn.CustomActive(false);

            switch (data.state)
            {
                case (int)OpActTaskState.OATS_INIT:
                case (int)OpActTaskState.OATS_UNFINISH:
                    if (data.linkId != 0)
                    {
                        mGoBtn.CustomActive(true);
                    }
                    else
                    {
                        mUnfinished.CustomActive(true);
                    }
                    break;
                case (int)OpActTaskState.OATS_OVER:
                    mHaveRreceive.CustomActive(true);
                    break;
                case (int)OpActTaskState.OATS_FINISHED:
                    mReceiveBtn.CustomActive(true);
                    break;
            }
        }

        private void OnReceiveBtnClick()
        {
            if (mTaskDataModel != null)
            {
                if (mTaskDataModel.identify == (int)RecruitIdentify.RI_NEWBIE && !WarriorRecruitDataManager.isBindInviteCode)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("请先接受玩家招募");
                    return;
                }

                WarriorRecruitDataManager.GetInstance().SendSubmitHireTaskReq(mTaskDataModel.taskId);
            }
        }

        private void OnGlassBtnClick()
        {
            if (mTaskDataModel != null)
            {
                WarriorRecruitDataManager.GetInstance().SendQueryHireTaskAccidListReq(mTaskDataModel.taskId);
            }
        }

        private void OnGoBtnClick()
        {
            AcquiredMethodTable acquiredMethodTable = TableManager.GetInstance().GetTableItem<AcquiredMethodTable>(mTaskDataModel.linkId);
            if (acquiredMethodTable != null && acquiredMethodTable.IsLink != 0)
            {
                if (acquiredMethodTable.FuncitonID != 0)
                {
                    FunctionUnLock functionUnLock = TableManager.GetInstance().GetTableItem<FunctionUnLock>(acquiredMethodTable.FuncitonID);
                    if (functionUnLock != null)
                    {
                        if (functionUnLock.FinishLevel > PlayerBaseData.GetInstance().Level)
                        {
                            SystemNotifyManager.SystemNotify(functionUnLock.CommDescID);
                            return;
                        }
                    }
                }

                ActiveManager.GetInstance().OnClickLinkInfo(acquiredMethodTable.LinkInfo);
                ActiveChargeFrame.CloseMe();
            }
        }

        private void OnDestroy()
        {
            mTaskDataModel = null;
            rewardsComBindList.Clear();
        }
    }
}