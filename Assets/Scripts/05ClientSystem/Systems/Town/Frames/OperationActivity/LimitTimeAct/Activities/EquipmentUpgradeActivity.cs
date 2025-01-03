using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class EquipmentUpgradeActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/EquipmentUpgradeActivity";
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
                    var tempView = mView as EquipmentUpgradeActivityView;
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
            SmithShopNewLinkData linkData = new SmithShopNewLinkData();
            linkData.itemData = null;
            linkData.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_STRENGTHEN;

            ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true);
            ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, linkData);
        }
    }
}