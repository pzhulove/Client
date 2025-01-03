using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class AnniversaryLoginActivityView : MonoBehaviour, IActivityView
    {

        [SerializeField]
        private Button mRewardBtn;
        [SerializeField]
        private GameObject mHaveRewardGo;
     
        [SerializeField]
        private Transform mItemRoot;

        [SerializeField]
        private Text mTimeTxt;

        [SerializeField]

        private Text mRuleTxt;

        private int mId;
        protected ActivityItemBase.OnActivityItemClick<int> mOnItemClick;

        private ILimitTimeActivityModel mData;

        protected readonly Dictionary<uint, IActivityCommonItem> mItems = new Dictionary<uint, IActivityCommonItem>();

        private readonly List<int> mRequestedGiftPackIds = new List<int>();

        private List<AnniversaryLoginActivityItem> itemList = new List<AnniversaryLoginActivityItem>();

        private List<GiftSyncInfo> mGiftPackItemData = new List<GiftSyncInfo>();

        private bool mStateIsChanged = false;
        public void Init(ILimitTimeActivityModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (data.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(data.StartTime), _TransTimeStampToStr(data.EndTime)));
            mRuleTxt.SafeSetText(data.RuleDesc);

            mData = data;
            mOnItemClick = onItemClick;
            mRewardBtn.SafeAddOnClickListener(_OnRewardBtnClick);
            mId = (int)data.TaskDatas[0].DataId;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            for (int i = 0; i < mData.TaskDatas.Count; i++)
            {
                for (int j = 0; j < mData.TaskDatas[i].AwardDataList.Count; j++)
                {
                    int giftId = (int)mData.TaskDatas[i].AwardDataList[j].id;
                    if (!mRequestedGiftPackIds.Contains(giftId))
                    {
                        mRequestedGiftPackIds.Add(giftId);
                    }

                    GiftPackDataManager.GetInstance().GetGiftPackItem(giftId);
                   
                }
            }
           


        }
      


        public void UpdateData(ILimitTimeActivityModel data)
        {

            UpdateMyDate(data, false);


        }

        private void UpdateMyDate(ILimitTimeActivityModel data,bool isInit=true)
        {
            ILimitTimeActivityTaskDataModel taskData = data.TaskDatas[0];
            if (taskData != null)
            {
                bool isOver = false;
                if (taskData.State == Protocol.OpActTaskState.OATS_FINISHED)
                {
                    mRewardBtn.CustomActive(true);
                    mHaveRewardGo.CustomActive(false);
                    isOver = false;
                }
                else if (taskData.State == Protocol.OpActTaskState.OATS_OVER)
                {
                    mRewardBtn.CustomActive(false);
                    mHaveRewardGo.CustomActive(true);
                    isOver =true ;
                }
                if(isInit)
                {
                    for (int j = 0; j < mGiftPackItemData.Count; j++)
                    {
                        GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
                        if (go == null) return;
                        AnniversaryLoginActivityItem item = go.GetComponent<AnniversaryLoginActivityItem>();
                        if (item == null) return;
                        item.Init(isOver, mGiftPackItemData[j]);
                        go.transform.SetParent(mItemRoot, false);
                        go.transform.localScale = Vector3.one;
                    }
                }
                else
                {
                    if(isOver)
                    {
                        for (int i = 0; i < mItemRoot.childCount; i++)
                        {
                            AnniversaryLoginActivityItem item = mItemRoot.GetChild(i).GetComponent<AnniversaryLoginActivityItem>();
                            if(item!=null)
                            {
                                item.ShowHaveReceivedState();
                            }
                           
                        }
                    }
                   
                }
               
            }
        }

        public void Close()
        {
            
            mRewardBtn.SafeRemoveOnClickListener(_OnRewardBtnClick);
            mRewardBtn = null;
            mOnItemClick = null;
            mRequestedGiftPackIds.Clear();
            mGiftPackItemData.Clear();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            Destroy(gameObject);
        }

        public void Show()
        {
            gameObject.CustomActive(true);
        }

        public void Hide()
        {
            gameObject.CustomActive(false);
        }

        public void Dispose()
        {
           
        }


      


       

        private void _OnRewardBtnClick()
        {
            //二次确认面板
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("AnniversaryLogin_Content"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("AnniversaryLogin_Cancel"),
                RightButtonText = TR.Value("AnniversaryLogin_OK"),
                OnRightButtonClickCallBack = OnOkBtnClick,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        private void OnOkBtnClick()
        {
            if (mOnItemClick != null)
            {
                //任务的id
                mOnItemClick(mId, 0, 0);
            }
        }

        private void _OnGetGiftData(UIEvent param)
        {
            if (param == null || param.Param1 == null)
            {
                Logger.LogError("礼包数据为空");
                return;
            }

            GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;
            if (!mRequestedGiftPackIds.Contains((int)data.id))
            {
                return;
            }
       
            if (data != null)
            {
                for (int i = 0; i < data.gifts.Length; ++i)
                {
                    GiftPackItemData giftTable = GiftPackDataManager.GetGiftDataFromNet(data.gifts[i]);
                    if (giftTable.ItemID > 0)
                    {
                       if(!mGiftPackItemData.Contains(data.gifts[i]))
                        {
                            mGiftPackItemData.Add(data.gifts[i]);
                        }
                    }
                }
              
            }
            UpdateMyDate(mData);
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }

    }
}
