using System;
using System.Collections.Generic;
using ActivityLimitTime;
using GameClient;
using LimitTimeGift;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 商城活动礼包。(限时活动界面的限时礼包大分页)
    /// </summary>
    public class MallActivityGiftPackActivity : IActivity
    {
        protected GameObject mGameObject;
        protected bool mIsInit = false;
        protected MallActivityGiftPackActivityView mView;
        private LimitTimeGiftPackDetailModel mDataModel;
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        private OpActivityState mState = OpActivityState.OAS_IN;

        public void Init(uint activityId)
        {
            var itemInfo = ActivityDataManager.GetInstance().GetGiftPackData(MallTypeTable.eMallType.SN_ACTIVITY_GIFT, activityId);

            if (itemInfo != null)
            {
                mDataModel = new LimitTimeGiftPackDetailModel(itemInfo);
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

                mView = mGameObject.GetComponent<MallActivityGiftPackActivityView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, _OnBuyClick);
                    mIsInit = true;
                }
            }
            ActivityDataManager.GetInstance().RequestMallGiftData();
            mCheckComponent.Checked(this);
            //添加埋点
            Utility.DoStartFrameOperation("MallActivityGiftPackActivity", string.Format("ActivityId/{0}", mDataModel.Id));

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
            var itemInfo = ActivityDataManager.GetInstance().GetGiftPackData(MallTypeTable.eMallType.SN_ACTIVITY_GIFT, mDataModel.Id);
            if (itemInfo != null)
            {
                mDataModel = new LimitTimeGiftPackDetailModel(itemInfo);
                if (mView != null)
                {
                    mView.UpdateData(mDataModel);
                }
            }
            else
            {
                mState = OpActivityState.OAS_END;
            }
        }

        public void UpdateTask(int taskId)
        {
            //不会有任务更新的消息 只会活动更新
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
            return mState;
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

        protected virtual void _OnBuyClick()
        {
            var tableData = TableManager.GetInstance().GetTableItem<MallLimitTimeActivity>((int)mDataModel.Id);
            if (tableData == null)
                return;
            if (tableData.BuyLink == MallLimitTimeActivity.eBuyLink.Go_To_Mall_Limit_Buy)
            {
                ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.LimitTimeMall });
                ClientSystemManager.instance.CloseFrameByType(typeof(LimitTimeActivityFrame));
                //添加埋点
                Utility.DoStartFrameOperation("MallActivityGiftPackActivity", string.Format("GotoShop/{0}", mDataModel.Id));
            }
            else if(tableData.BuyLink == MallLimitTimeActivity.eBuyLink.Go_To_Prop_Mall_Limit)

            {
                ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.PropertyMall });
                ClientSystemManager.instance.CloseFrameByType(typeof(LimitTimeActivityFrame));
                //添加埋点
                Utility.DoStartFrameOperation("MallActivityGiftPackActivity", string.Format("GotoShop/{0}", mDataModel.Id));
            }else if (tableData.BuyLink == MallLimitTimeActivity.eBuyLink.Go_To_Dungeon)
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<BossSelectBattleFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<BossSelectBattleFrame>();
                }
            }
            else
            {
                var priceType = mDataModel.PriceType;
                uint id = mDataModel.Id;
                int price = mDataModel.GiftPrice;
                string notifyCont = string.Format(TR.Value("activity_activity_gift_pack_buy_tip"), price);
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
                {
                    if (priceType == LimitTimeGiftPriceType.Point)
                    {
                        ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.SendReqBuyGiftInMall(id, price, 1);
                    }
                });
            }
        }

        protected virtual string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/MallActivityGiftPackActivity";
        }
    }
}