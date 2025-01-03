using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class LimitTimeGroupBuyActivity : IActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        protected GameObject mGameObject;
        protected bool mIsInit = false;
        protected LimitTimeGiftPackModel mDataModel;
        protected IGiftPackActivityView mView;
        protected MallTypeTable.eMallType mMallType;
        protected OpActivityData mData;

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

        public bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
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

                UpdateData();

                mView = mGameObject.GetComponent<IGiftPackActivityView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick);
                    mIsInit = true;
                }

                ActivityDataManager.GetInstance().RequestMallGiftData(mMallType);
            }
        }

        public void UpdateData()
        {
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(mDataModel.Id);
            var itemInfos = ActivityDataManager.GetInstance().GetGiftPackInfos(mMallType);
            if (data != null)
            {
                mDataModel = new LimitTimeGiftPackModel(data, itemInfos, mMallType, _GetItemPrefabPath());
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

                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, ActivityDataManager.GetInstance().GetGiftPackData(mMallType, data.Id));
            }
        }

        protected virtual string _GetItemPrefabPath()
        {
            return string.Empty;
        }

        protected virtual string _GetPrefabPath()
        {
            string path = string.Empty;
            if (mData != null && !string.IsNullOrEmpty(mData.prefabPath))
            {
                path = mData.prefabPath;
                return path;
            }

            path = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/LimitTimeGroupBuyActivity";
           
            return path;

        }
    }
}