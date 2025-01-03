using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class GoblinShopActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/GoblinShopActivity";
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
                    var tempView = mView as GoblinShopActivityView;
                    if (tempView != null)
                    {
                        tempView.SetCallBack(GoToGoblinShop);
                    }
                }
                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick);
                    this.mIsInit = true;
                }
            }

        }

        void GoToGoblinShop()
        {
            GoblinShopData goblinShopData = new GoblinShopData();
            goblinShopData.activityId = (int)mDataModel.Id;
            goblinShopData.accountShopItem.shopId = (byte)mDataModel.ParamArray[0];
            goblinShopData.accountShopItem.jobType = 0;
            goblinShopData.accountShopItem.tabType = (byte)ShopTable.eSubType.ST_NONE;
            ClientSystemManager.GetInstance().OpenFrame<GoblinShopFrame>(FrameLayer.Middle,goblinShopData);
        }
    }
}