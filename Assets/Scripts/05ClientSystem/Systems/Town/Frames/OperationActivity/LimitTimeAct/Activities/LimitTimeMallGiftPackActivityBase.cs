using LimitTimeGift;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 商城礼包，子分页
    /// </summary>
    public abstract class LimitTimeMallGiftPackActivityBase : IActivity
    {
        protected GameObject mGameObject;
        protected bool mIsInit = false;
        protected LimitTimeMallGiftPackActivityView mView;
        protected LimitTimeGiftPackModel mDataModel;
        protected readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        protected MallTypeTable.eMallType mMallType;

		public void Init(uint activityId)
		{
			var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (data != null)
            {
	            mMallType = (MallTypeTable.eMallType)data.parm;
	            var itemInfos = ActivityDataManager.GetInstance().GetGiftPackInfos(mMallType);
                mDataModel = new LimitTimeGiftPackModel(data, itemInfos, mMallType, _GetItemPrefabPath());
            }
        }

        public void Show(Transform root)
        {
            if (mDataModel.Id == 0)
            {
                return;
            }

            if (mIsInit)
            {
                mGameObject.CustomActive(true);
            }
            else
            {
                if (mGameObject == null)
                {
                    mGameObject = AssetLoader.instance.LoadResAsGameObject(_GetPrefabPath());
                }

                if (mGameObject != null)
                {
                    mGameObject.transform.SetParent(root, false);
                    mGameObject.CustomActive(true);
                }
                else
                {
                    Logger.LogError("加载活动预制体失败，路径:" + _GetPrefabPath());
                    return;
                }

                mView = mGameObject.GetComponent<LimitTimeMallGiftPackActivityView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick);
                    mIsInit = true;
                }
            }

            ActivityManager.GetInstance().RequestGiftDatas();
            mCheckComponent.Checked(this);
            if(mDataModel.Id== 5000)
            {
                //添加埋点
                Utility.DoStartFrameOperation("MallGiftPackActivity", "MallGiftPackActivityBtn");
            }
        }

        public void Hide()
        {
            if (mGameObject != null)
            {
                mGameObject.CustomActive(false);
            }
        }

        public void Close()
        {
            mIsInit = false;
            if (mView != null)
            {
                mView.Close();
            }

            mView = null;
            mGameObject = null;
        }

        public void UpdateData()
        {
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(mDataModel.Id);
            var itemInfos = ActivityDataManager.GetInstance().GetGiftPackInfos(mMallType);
            if (data != null)
            {
                mDataModel = new LimitTimeGiftPackModel(data, itemInfos, mMallType, _GetItemPrefabPath());
                if (mView != null)
                {
                    mView.UpdateData(mDataModel);
                }
            }
        }

        public void UpdateTask(int taskId)
        {
            //数据更新
            mDataModel.UpdateItem(taskId);

            //View刷新
            if (mView != null)
            {
                mView.UpdateData(mDataModel);
            }
        }

        public bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }

        public uint GetId()
        {
            return mDataModel.Id;
        }

        public string GetName()
        {
            return mDataModel.Name;
        }

        public OpActivityState GetState()
        {
            return mDataModel.State;
        }

        public virtual void Dispose()
        {
            mGameObject = null;
            mIsInit = false;
            if (mView != null)
            {
                mView.Dispose();
            }

            mView = null;
        }

        protected virtual void _OnItemClick(int id, int param,ulong param2)
        {
            if (mDataModel.DetailDatas != null && id >= 0 && id < mDataModel.DetailDatas.Count)
            {
                var data = mDataModel.DetailDatas[id];

                if (data.PriceType == LimitTimeGiftPriceType.Point|| data.PriceType == LimitTimeGiftPriceType.BindPint|| data.PriceType == LimitTimeGiftPriceType.Gold
                    ||data.PriceType==LimitTimeGiftPriceType.BindGOLD
                    )
                {
	                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, ActivityDataManager.GetInstance().GetGiftPackData(mMallType, data.Id));
					//string notifyCont = string.Format(TR.Value("activity_activity_gift_pack_buy_tip"), data.GiftPrice);
     //               SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
     //               {
	    //                ActivityDataManager.GetInstance().SendReqBuyGiftInMall(mDataModel.DetailDatas[id].Id, 1, mDataModel.DetailDatas[id].GiftPrice);
     //               });
                }
                else if (data.PriceType == LimitTimeGiftPriceType.RMB)
                {
                    PayManager.GetInstance().DoPay((int)id, data.GiftPrice, ChargeMallType.Packet);
                }
            }
        }

        protected abstract string _GetItemPrefabPath();

        protected abstract string _GetPrefabPath();

    }
}