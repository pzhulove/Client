using System;
using System.Collections.Generic;
using ActivityLimitTime;
using GameClient;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 商城活动礼包。(限时活动界面的限时礼包大分页)
    /// </summary>
    public sealed class BossExchangeActivity : IActivity
    {
        private GameObject mGameObject;
        private bool mIsInit = false;
        private BossExchangeActivityView mView;
        private BossExchangeModel mDataModel;
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        public void Init(uint activityId)
        {
            var data = ActivityDataManager.GetInstance().GetBossActivityData(activityId);
            if (data != null)
            {
                mDataModel = new BossExchangeModel(data);
            }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            //ActivityDataManager.GetInstance().SetNeedRedPointActivityDic(mDataModel.Id,false);
        }

        public void Show(Transform root)
        {
            if (mDataModel.Id == 0)
            {
                return;
            }
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

                mView = mGameObject.GetComponent<BossExchangeActivityView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick, _OnGoShop);
                    mIsInit = true;
                }
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
            var data = ActivityDataManager.GetInstance().GetBossActivityData(mDataModel.Id);
            if (data != null)
            {
                mDataModel = new BossExchangeModel(data);
                if (mView != null)
                {
                    mView.UpdateData(mDataModel);
                }
            }
        }

        public void UpdateTask(int taskId)
        {
            mDataModel.UpdateTask(taskId);
            //View刷新
            if (mView != null)
            {
                mView.UpdateData(mDataModel);
            }
        }

        public bool IsHaveRedPoint()
        {
            bool taskFinish = false;
            foreach (var task in mDataModel.ExchangeTasks.Values)
            {
                if (task.Status == TaskStatus.TASK_FINISHED)
                {
                    taskFinish = true;
                }
            }
            return taskFinish && !mCheckComponent.IsChecked();
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

        public void Dispose()
        {
            mGameObject = null;
            mIsInit = false;
            if (mView != null)
            {
                mView.Dispose();
            }

            mView = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        private void _OnItemClick(int taskId, int param,ulong param2)
        {
            ActivityDataManager.GetInstance().SendSubmitBossExchangeTask(taskId);
        }

        private void _OnGoShop()
        {
            ClientSystemManager.instance.CloseFrameByType(typeof(LimitTimeActivityFrame));
            ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.LimitTimeMall });
        }

        private string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/BossExchangeActivity";
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
            if (mView != null)
            {
                mView.UpdateData(mDataModel);
            }
        }
    }
}