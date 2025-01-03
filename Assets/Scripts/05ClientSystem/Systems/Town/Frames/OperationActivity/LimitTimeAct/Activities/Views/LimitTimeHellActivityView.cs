using Network;
using Protocol;
using ProtoTable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class LimitTimeHellActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private Button mGoToGloblinBtn = null;
        [SerializeField]
        private Image mBg = null;

        public delegate void CallBack();
        private CallBack mGoToCallback;
        private List<ComItem> mComItems = new List<ComItem>();
        IActivityCommonItem mActivityItemData;
        public void SetCallBack(CallBack callback)
        {
            mGoToCallback = callback;
        }
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mOnItemClick = onItemClick;
            _InitItems(model);
            mNote.Init(model, true, GetComponent<ComCommonBind>());
            mGoToGloblinBtn.onClick.RemoveAllListeners();
            mGoToGloblinBtn.onClick.AddListener(() =>
            {
                if (mGoToCallback != null)
                {
                    mGoToCallback();
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

        protected override sealed void _InitItems(ILimitTimeActivityModel data)
        {

            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
            if (go == null)
            {
                Logger.LogError("加载预制体失败，路径:" + data.ItemPath);
                return;
            }

            if (go.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }
            mItems.Clear();

            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            mActivityItemData = item.GetComponent<IActivityCommonItem>();
            mActivityItemData.InitFromMode(data, mOnItemClick);
            //mItems.Add(data.TaskDatas[0].DataId, item.GetComponent<IActivityCommonItem>());
            Destroy(go);
        }
        public override void UpdateData(ILimitTimeActivityModel data)
        {
            (mActivityItemData as LimitTimeHellItem).UpdateFromMode(data);
        }
        public override void Dispose()
        {
            
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
