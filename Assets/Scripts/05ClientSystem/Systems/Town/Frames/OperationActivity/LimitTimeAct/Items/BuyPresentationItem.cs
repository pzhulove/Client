using System;
using System.Collections;
using System.Collections.Generic;
using ActivityLimitTime;
using Network;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BuyPresentationItem : ActivityItemBase
    {
        [SerializeField]
        private Text mTextDescription;
        [SerializeField]
        private RectTransform mRewardItemRoot;
        [SerializeField]
        private GameObject mMaterialRoot;
        [SerializeField]
        Button mButtonUnFinish;
        [SerializeField]
        Button mButtonTakeReward;
        [SerializeField]
        GameObject mHasTakenReward;
        //[SerializeField]
        //GameObject mUnTakeReward;
        [SerializeField]
        GameObject mUnFinish;
        [SerializeField]
        GameObject mCanTakeReward;
        [SerializeField]
        private ScrollRect mAwardsScrollRect;
        //[SerializeField]
        //private Text mGoldNum;
        //[SerializeField]
        //private Image mGoldIcon;
        [SerializeField]
        private Text mBuyDes;
        [SerializeField]
        Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField]
        private int mScrollCount = 5;//超过多少时才能滑动

        private List<ComItem> mComItems = new List<ComItem>();

        const string buyPresentationMaterialItemPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/BuyPresentationMaterialItem";
        private List<GameObject> mMaterialItemList = new List<GameObject>();

        [SerializeField]
        private Text mAccountLimitTxt;

        private ILimitTimeActivityTaskDataModel mData;

        private bool mIsLeftMinus0 = false;
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if(!mIsLeftMinus0)
            {
                mCanTakeReward.CustomActive(false);
                mButtonUnFinish.CustomActive(false);
                mHasTakenReward.CustomActive(false);
                switch (data.State)
                {
                    case OpActTaskState.OATS_UNFINISH:
                        mButtonUnFinish.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_OVER:
                        mHasTakenReward.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_FINISHED:
                        mCanTakeReward.CustomActive(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mCanTakeReward.CustomActive(false);
                mButtonUnFinish.CustomActive(false);
                mHasTakenReward.CustomActive(true);
            }
          

            //3种参数同样多
            if (data.ParamNums.Count > 0 && data.ParamNums.Count == data.ParamNums2.Count && data.ParamNums2.Count == data.ParamProgress.Count)
            {
                for (int i = 0; i < mMaterialItemList.Count; i++)
                {
                    var mBind = mMaterialItemList[i].GetComponent<ComCommonBind>();
                    if (mBind == null)
                    {
                        continue;
                    }
                    var materialItemCount = mBind.GetCom<Text>("ItemCount");
                    var materialItemFinished = mBind.GetGameObject("ItemFinished");
                    int tempNum = 0;
                    for(int j = 0;j<data.ParamProgressList.Count;j++)
                    {
                        if(data.ParamProgressList[j].key == data.ParamProgress[i])
                        {
                            tempNum = (int)data.ParamProgressList[j].value;
                        }
                    }
                    int tempTotalNum = (int)data.ParamNums2[i];
                    if(tempNum >= tempTotalNum)
                    {
                        materialItemFinished.CustomActive(true);
                        tempNum = tempTotalNum;
                        materialItemCount.color= Color.green;
                    }
                    else
                    {
                        materialItemFinished.CustomActive(false);
                        materialItemCount.color = Color.white;
                    }
                    materialItemCount.text = string.Format("{0}/{1}", tempNum, tempTotalNum);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }
            mIsLeftMinus0 = false;
            mData = null;
            mButtonTakeReward.SafeRemoveOnClickListener(_OnMyItemClick);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }



        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data.AwardDataList != null)
            {
                for (int i = 0; i < data.AwardDataList.Count; ++i)
                {
                    var comItem = ComItemManager.Create(mRewardItemRoot.gameObject);
                    if (comItem != null)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
                        item.Count = (int)data.AwardDataList[i].num;
                        comItem.Setup(item, Utility.OnItemClicked);
                        mComItems.Add(comItem);
                        (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                    }
                }
                mAwardsScrollRect.enabled = data.AwardDataList.Count > mScrollCount;
            }
            mTextDescription.SafeSetText(data.Desc);
            mButtonTakeReward.SafeAddOnClickListener(_OnMyItemClick);
            mButtonUnFinish.onClick.RemoveAllListeners();
            mButtonUnFinish.onClick.AddListener(() =>
            {
                var mallFrame = ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.LimitTimeMall }) as MallNewFrame;
            });
                //3种参数同样多
            if(data.ParamNums.Count > 0 && data.ParamNums.Count == data.ParamNums2.Count && data.ParamNums2.Count == data.ParamProgress.Count)
            {
                mMaterialItemList.Clear();
                for (int i = 0; i < data.ParamNums.Count; i++)
                {
                    var buyPresentationItem = AssetLoader.GetInstance().LoadResAsGameObject(buyPresentationMaterialItemPath);
                    if (buyPresentationItem != null)
                    {
                        var mBind = buyPresentationItem.GetComponent<ComCommonBind>();
                        if (mBind == null)
                        {
                            Logger.LogErrorFormat("11111111");
                            continue;
                        }
                        //这里用参数1（ParamNums）中的商城道具表，读取礼包的第一个道具，作为itemdata
                        var mItemDataRoot = mBind.GetGameObject("ItemRoot");
                        var comItem = ComItemManager.Create(mItemDataRoot);
                        if (comItem == null)
                        {
                            Logger.LogErrorFormat("22222222222");
                            continue;
                        }
                        var mallItemTableData = TableManager.GetInstance().GetTableItem<MallItemTable>((int)data.ParamNums[i]);
                        if (mallItemTableData == null)
                        {
                            Logger.LogErrorFormat("33333333333333");
                            continue;
                        }
                        string materialItemIdStr = mallItemTableData.giftpackitems.Split(':')[0];
                        int materialItemId = -1;
                        int.TryParse(materialItemIdStr, out materialItemId);
                        if(materialItemId == -1)
                        {
                            Logger.LogErrorFormat("4444444444444444");
                            continue;
                        }
                        ItemData item = ItemDataManager.CreateItemDataFromTable(materialItemId);
                        if(item == null)
                        {
                            Logger.LogErrorFormat("5555555555555");
                            continue;
                        }
                        comItem.Setup(item, Utility.OnItemClicked);
                        mComItems.Add(comItem);
                        (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                        Utility.AttachTo(buyPresentationItem, mMaterialRoot);
                        mMaterialItemList.Add(buyPresentationItem);
                    }
                }
            }
            mData = data;
            ShowHaveUsedNumState(data);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }

        private void _OnMyItemClick()
        {
            _OnItemClick();
            if (mData != null)
            {
                if (mData.AccountDailySubmitLimit > 0)
                {
                    ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                }
                if (mData.AccountTotalSubmitLimit > 0)
                {
                    ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                }
            }
        }
        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mData != null)
            {
                if ((uint)uiEvent.Param1 == mData.DataId)
                {
                    ShowHaveUsedNumState(mData);
                }

            }
        }



        /// <summary>
        /// 显示账号次数
        /// </summary>
        private void ShowHaveUsedNumState(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null)
            {
                int totalNum = 0;
                int haveNum = 0;

                if (data.AccountDailySubmitLimit > 0)
                {
                    haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(data.DataId,
                        ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                    totalNum = data.AccountDailySubmitLimit;
                }
                else if (data.AccountTotalSubmitLimit > 0)
                {
                    haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(data.DataId,
                       ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                    totalNum = data.AccountTotalSubmitLimit;
                }

                int leftNum = totalNum - haveNum;
                if (leftNum <= 0&&totalNum>0)
                {
                    mCanTakeReward.CustomActive(false);
                    mButtonUnFinish.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                    mIsLeftMinus0 = true;
                    leftNum = 0;
                }
                mAccountLimitTxt.CustomActive(totalNum > 0);
                mAccountLimitTxt.SafeSetText(string.Format(TR.Value("ConsumeRebate_AccountLimt_Content"), leftNum, totalNum));

            }
        }
    }
}
