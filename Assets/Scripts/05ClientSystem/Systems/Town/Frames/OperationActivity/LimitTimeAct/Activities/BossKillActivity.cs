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
    public sealed class BossKillActivity : IActivity
    {
        private GameObject mGameObject;
        private bool mIsInit = false;
        private BossKillActivityView mView;
        private BossKillModel mDataModel;

        public void Init(uint activityId)
        {
            var data = ActivityDataManager.GetInstance().GetBossActivityData(activityId);
            var monsterData = ActivityDataManager.GetInstance().GetBossKillMonsterData(activityId);
            if (data != null)
            {
                mDataModel = new BossKillModel(monsterData, data);
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

                mView = mGameObject.GetComponent<BossKillActivityView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick, _OnGoShop);
                    mIsInit = true;
                }
            }
			ActivityDataManager.GetInstance().RequestBossKillData((int)mDataModel.Id);
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
            var monsterData = ActivityDataManager.GetInstance().GetBossKillMonsterData(mDataModel.Id);
            if (data != null)
            {
                mDataModel = new BossKillModel(monsterData, data);
                if (mView != null)
                {
                    mView.UpdateData(mDataModel);
                }
            }
        }

        public void UpdateTask(int taskId)
        {
            //Boss击杀活动不会有任务更新的协议下发
        }

        public bool IsHaveRedPoint()
        {
            return false;
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
        }

        private void _OnItemClick(int id, int param,ulong param2)
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<BossSelectBattleFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<BossSelectBattleFrame>();
            }
        }

        private void _OnGoShop()
        {
            
            ClientSystemManager.instance.CloseFrameByType(typeof(LimitTimeActivityFrame));
            ClientSystemManager.instance.OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.PAY);
           // ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.LimitTimeMall});
        }

        private string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/BossKillActivity";
        }
    }
}