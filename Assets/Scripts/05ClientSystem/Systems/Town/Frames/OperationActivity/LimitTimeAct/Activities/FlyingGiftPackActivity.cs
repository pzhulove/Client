using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using ProtoTable;
using LimitTimeGift;

namespace GameClient
{
    public enum EGiftPackActivityType
    {
        FlyGiftPack = 1,//飞升礼包
        ThankGiving = 2,//感恩礼包
        LuckyBag = 3, //感恩福袋
        FashionSynthesis = 4,//时装合成
    }
    public class FlyingGiftPackActivity : IActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        protected GameObject mGameObject;
        protected bool mIsInit = false;
        protected LimitTimeGiftPackModel mDataModel;
        protected IGiftPackActivityView mView;
        protected MallTypeTable.eMallType mMallType;
        protected OpActivityData mData;
        public void Init(uint activityId)
        {
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (data != null)
            {
                mMallType = (MallTypeTable.eMallType)data.parm;
                var itemInfos = ActivityDataManager.GetInstance().GetGiftPackInfos(mMallType);
                mDataModel = new LimitTimeGiftPackModel(data, itemInfos, mMallType, _GetItemPrefabPath());
            }
            mData = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            ActivityDataManager.GetInstance().RequestMallGiftData(mMallType);
        }

        public void Show(Transform root)
        {
            if (mDataModel.Id == 0)
            {
                return;
            }
            if (mData == null) return;
            mCheckComponent.Checked(this);
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
                mView = mGameObject.GetComponent<IGiftPackActivityView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick);
                    mIsInit = true;
                }

                ActivityDataManager.GetInstance().RequestMallGiftData(mMallType);
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

        public void Dispose()
        {
            Close();
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

        public void Hide()
        {
            if (mGameObject != null)
            {
                mGameObject.CustomActive(false);
            }
        }

        public  bool IsHaveRedPoint()
        {
            bool result = false;
            if (mData!=null)
            {
                for (int i = 0; i < mDataModel.DetailDatas.Count; i++)
                {
                    if(mDataModel.DetailDatas[i].Limit!=(byte)ELimitiTimeGiftDataLimitType.None)
                    {
                        if (mDataModel.DetailDatas[i].Limit == (byte)ELimitiTimeGiftDataLimitType.NotRefresh)
                        {
                            result = mDataModel.DetailDatas[i].LimitPurchaseNum - CountDataManager.GetInstance().GetCount(mDataModel.DetailDatas[i].Id.ToString()) > 0;
                        }
                        else
                        {
                            result = mDataModel.DetailDatas[i].LimitNum - CountDataManager.GetInstance().GetCount(mDataModel.DetailDatas[i].Id.ToString()) > 0;
                        }
                    }
                    else if (mDataModel.DetailDatas[i].AccountLimitBuyNum>0)
                    {
                        result = mDataModel.DetailDatas[i].AccountRestBuyNum > 0;
                    }
                     if (result)
                            break;
                }
            }
            if(result)//礼包没有全部购买
            {
                return !mCheckComponent.IsChecked();
            }
            else
            {
                return result;
            }
           
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
           
        }

        protected virtual void _OnItemClick(int id, int param, ulong param2)
        {
            if (mDataModel.DetailDatas != null && id >= 0 && id < mDataModel.DetailDatas.Count)
            {
                var data = mDataModel.DetailDatas[id];

                if (data.PriceType == LimitTimeGiftPriceType.Point || data.PriceType == LimitTimeGiftPriceType.BindPint || data.PriceType == LimitTimeGiftPriceType.Gold
                    || data.PriceType == LimitTimeGiftPriceType.BindGOLD
                    )
                {
                    ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, ActivityDataManager.GetInstance().GetGiftPackData(mMallType, data.Id));
                }
                else if (data.PriceType == LimitTimeGiftPriceType.RMB)
                {
                    PayManager.GetInstance().DoPay((int)id, data.GiftPrice, ChargeMallType.Packet);
                }
            }
        }

        protected virtual string _GetItemPrefabPath()
        {
            return string.Empty;
        }

        protected virtual string _GetPrefabPath()
        {
            string path = string.Empty;
            if(mData!=null&& !string.IsNullOrEmpty(mData.prefabPath))
            {
                path = mData.prefabPath;
                return path;
            }
          
            if (mData == null) return path;
            if (mData.parm2 == null || mData.parm2.Length < 1) return path;
            switch (mData.parm2[0])
            {
                case (uint)EGiftPackActivityType.FlyGiftPack:
                    path = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/FlyingGiftPackActivity";
                    break;
                case (uint)EGiftPackActivityType.ThankGiving:
                    path = "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/AnniversaryThankgivingActivity";
                    break;
                case (uint)EGiftPackActivityType.LuckyBag:
                    path = "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/AnniversaryLuckyBagActivity";
                    break;
                case (uint)EGiftPackActivityType.FashionSynthesis:
                    path = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/FashionSyntheticActivity";
                    break;

            }
            return path;

        }
    }
}