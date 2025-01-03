using System;
using GameClient;
using Protocol;
using UnityEngine;

namespace GameClient
{
    public enum EadditionBuffType
    {
        None=0,
        XuKong=1,//虚空
        HunDun=2,//混沌
    }
    public class VanityBonusActivity : IActivity
    {
        protected GameObject mGameObject;
        protected bool mIsInit = false;
        protected IDungeonBuffView mView;
        protected ILimitTimeActivityModel mDataModel;

        protected EadditionBuffType eAdditionBuffType = EadditionBuffType.None;
        public void Init(uint activityId)
        {
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (data != null)
            {
                eAdditionBuffType = (EadditionBuffType)data.parm;
                mDataModel = new LimitTimeActivityModel(data, _GetItemPrefabPath());
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
               
                mView = mGameObject.GetComponent<IDungeonBuffView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, GoBtnOnClick);
                    mIsInit = true;
                }
            }
        }

        private void GoBtnOnClick()
        {
            if(eAdditionBuffType==EadditionBuffType.XuKong)
            {
                Utility.PathfindingYiJieMap();
            }
            else
            {
                ChallengeUtility.OnOpenChallengeMapFrame(ProtoTable.DungeonModelTable.eType.WeekHellModel, 0);
                if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
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
            if (mDataModel == null)
                return;

            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(mDataModel.Id);
            if (data != null)
            {
                mDataModel = new LimitTimeActivityModel(data, _GetItemPrefabPath());
            }
        }

        public void UpdateTask(int taskId)
        {
            //数据更新
            mDataModel.UpdateTask(taskId);
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

        private string _GetPrefabPath()
        {
            string path = string.Empty;
            if(eAdditionBuffType==EadditionBuffType.XuKong)
            {
                path= "UIFlatten/Prefabs/OperateActivity/YiJie/Activities/VanityBonusActivity";
            }else if(eAdditionBuffType==EadditionBuffType.HunDun)
            {
                if(mDataModel!=null&& !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
                {
                    path = mDataModel.ActivityPrefafPath;
                }
                else
                {
                    path = "UIFlatten/Prefabs/OperateActivity/Chaos/Activity/ChaosAdditionActivity";
                }
            }

            return path;
            
        }

        private string _GetItemPrefabPath()
        {
            string path = string.Empty;
            if(eAdditionBuffType == EadditionBuffType.XuKong)
            {
                path = "UIFlatten/Prefabs/OperateActivity/YiJie/Items/VanityBonusItemLeft";
            }else if(eAdditionBuffType==EadditionBuffType.HunDun)
            {
                path = "UIFlatten/Prefabs/OperateActivity/Chaos/Item/ChaosAdditionItemLeft";
            }
            return path;
        }
    }
}