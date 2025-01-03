using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class LimitTimeHellActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/LimitTimeHellActivity";
        }
        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/LimitTimeHellItem";
        }
        public override void Show(Transform root)
        {
            if (mDataModel == null)
            {
                return;
            }

            if (mIsInit)
            {
                mGameObject.CustomActive(true);

                if (mView != null)
                {
                    mView.Show();
                }
            }
            else
            {
                if (this.mGameObject == null)
                {
                    this.mGameObject = AssetLoader.instance.LoadResAsGameObject(_GetPrefabPath());
                }

                if (this.mGameObject != null)
                {
                    this.mGameObject.transform.SetParent(root, false);
                    this.mGameObject.CustomActive(true);
                }
                else
                {
                    Logger.LogError("加载活动预制体失败，路径:" + _GetPrefabPath());
                    return;
                }

                mView = mGameObject.GetComponent<IActivityView>();

                if (mView != null)
                {
                    var tempView = mView as LimitTimeHellActivityView;
                    if (tempView != null)
                    {
                        tempView.SetCallBack(GoToHellFrame);
                    }
                }
                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick);
                    this.mIsInit = true;
                }
            }

        }
        void GoToHellFrame()
        {
            //等接口
            //打开地下城界面 
            ClientSystemManager.GetInstance().OpenFrame<ChallengeMapFrame>();
            ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
        }
    }
}