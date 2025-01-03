using Network;
using Protocol;
using ProtoTable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipmentUpgradeActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private Button mGoToGloblinBtn = null;
        [SerializeField]
        private Image mBg = null;

        public delegate void BuyCallBack();
        private BuyCallBack mGoToGoblinShopCallback;
        private List<ComItem> mComItems = new List<ComItem>();

        public void SetCallBack(BuyCallBack callback)
        {
            mGoToGoblinShopCallback = callback;
        }
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mOnItemClick = onItemClick;
            //_InitItems(model);
            mNote.Init(model, true, GetComponent<ComCommonBind>());
            mGoToGloblinBtn.onClick.RemoveAllListeners();
            mGoToGloblinBtn.onClick.AddListener(() =>
            {
                if (mGoToGoblinShopCallback != null)
                {
                    mGoToGoblinShopCallback();
                }
            });
            if(model.ParamArray != null && model.ParamArray.Length > 1)
            {
                var mallLimitTimeActivityData = TableManager.GetInstance().GetTableItem<MallLimitTimeActivity>((int)model.ParamArray[1]);
                if(mallLimitTimeActivityData != null && mBg != null)
                {
                    ETCImageLoader.LoadSprite(ref mBg, mallLimitTimeActivityData.BackgroundImgPath);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }
        }

        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }
    }
}
