using Protocol;
using ProtoTable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChangeFashionActivityView : LimitTimeActivityViewCommon
    {
        //前往合成
        [SerializeField]
        private Button mGoFashionMerge = null;
        //预览时装
        [SerializeField]
        private Button mLookUp = null;
        //item的根节点
        [SerializeField]
        private GameObject mFashionRoot = null;
        [SerializeField]
        private Text mFashionName = null;

        private List<ComItem> mComItems = new List<ComItem>();

        public delegate void GoFashionMergeCallBack();
        private GoFashionMergeCallBack mGoFashionCallBack;

        public delegate void LookUpCallBack(int itemID);
        private LookUpCallBack mLookUpCallBack;
        public void SetGoFashionCallBack(GoFashionMergeCallBack callback)
        {
            mGoFashionCallBack = callback;
        }

        public void SetLookUpCallBack(LookUpCallBack callback)
        {
            mLookUpCallBack = callback;
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

            if(model.ParamArray != null && model.ParamArray.Length != 0)
            {
                int itemID = (int)model.ParamArray[0];
                var itemTableItem = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
                if(itemTableItem == null)
                {
                    Logger.LogErrorFormat("扩展参数{0}在道具表中无法被找到");
                    return;
                }
                ComItem comitem = mFashionRoot.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    var comItem = ComItemManager.Create(mFashionRoot);//可以这样写吗需要确认
                    comitem = comItem;
                    mComItems.Add(comitem);
                }
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemID);
                if (null == ItemDetailData)
                {
                    return;
                }
                ItemDetailData.Count = 1;
                comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });
                mFashionName.text = ItemDetailData.Name;
            }
            if(mGoFashionMerge != null)
            {
                mGoFashionMerge.onClick.RemoveAllListeners();
                mGoFashionMerge.onClick.AddListener(() =>
                {
                    //前往时装合成
                    if (mGoFashionCallBack != null)
                    {
                        mGoFashionCallBack();
                    }
                });
            }
            if(mLookUp != null)
            {
                int itemID = (int)model.ParamArray[0];
                mLookUp.onClick.RemoveAllListeners();
                mLookUp.onClick.AddListener(() =>
                {
                    //查看时装模型
                    if(mLookUpCallBack != null)
                    {
                        mLookUpCallBack(itemID);
                    }
                });
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
