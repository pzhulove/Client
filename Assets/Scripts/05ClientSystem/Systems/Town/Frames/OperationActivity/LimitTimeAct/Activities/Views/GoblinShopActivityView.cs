using Network;
using Protocol;
using ProtoTable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinShopActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private Button mGoToGloblinBtn = null;
        [SerializeField]
        private Image mBg = null;
        [SerializeField]private Button mPreViewBtn;

        public delegate void BuyCallBack();
        private BuyCallBack mGoToGoblinShopCallback;
        private List<ComItem> mComItems = new List<ComItem>();
        private int iGoblinShopActivityID = 0;//地精商会活动ID
        private int iGoblinShopPreviewItemID = 0; //地精商会预览物品ID

        [SerializeField]
        private Vector2 mComItemSize = new Vector2(120, 120);
        [SerializeField]
        private GameObject mTmpGo;
        [SerializeField]
        private Transform mItemsRoot;
        private int mMallItemTableId = 0;
       

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

            InitLocalData();

            if (model.ParamArray2 != null && model.ParamArray2.Length > 0)
            {
                iGoblinShopPreviewItemID = (int)model.ParamArray2[0];
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
                if (model.ParamArray.Length >= 2)
                {
                    mMallItemTableId = (int)model.ParamArray[1];

                    MallItemTable mallItemTable = TableManager.GetInstance().GetTableItem<MallItemTable>(mMallItemTableId);
                    if (mallItemTable != null)
                    {
                       string gftPackItemStr=  mallItemTable.giftpackitems;
                        string[] itemsStr = gftPackItemStr.Split('|');
                        for (int i = 0; i < itemsStr.Length; i++)
                        {
                            string[] itemStr = itemsStr[i].Split(':');
                            int id = 0;
                            if(itemsStr.Length>=1)
                            {
                                int.TryParse(itemStr[0], out id);
                                if (mTmpGo != null)
                                {
                                    GameObject go = Instantiate(mTmpGo);
                                    go.transform.SetParent(mItemsRoot);
                                    go.transform.localScale = Vector3.one;

                                    GoblinShopActivityItem goblinShopActivityItem = go.GetComponent<GoblinShopActivityItem>();
                                    if (goblinShopActivityItem != null)
                                    {
                                        goblinShopActivityItem.Init(id, mComItemSize);
                                    }
                                }
                            }
                           
                        }
                     
                    }
                    if (mTmpGo)
                    {
                        Destroy(mTmpGo);
                    }
                }
               
            }
         
            SetPreviewBtn(model);
        }

        /// <summary>
        /// 从系统数据表取 预览的地精商会活动ID 、地精商会预览的物品ID
        /// </summary>
        private void InitLocalData()
        {
            var systemTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_PREVIEW_ACTIVITY_ID);
            iGoblinShopActivityID = systemTable.Value;
            //var systemTable2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_PREVIEW_ITEM_ID);
            //iGoblinShopPreviewItemID = systemTable2.Value;
        }

        /// <summary>
        /// 设置预览按钮
        /// </summary>
        /// <param name="model"></param>
        private void SetPreviewBtn(ILimitTimeActivityModel model)
        {
            if (model == null)
            {
                return;
            }
            
            if (mPreViewBtn != null)
            {
                mPreViewBtn.gameObject.CustomActive(0 != iGoblinShopActivityID);

                PreViewDataModel mPreViewData = new PreViewDataModel();
                mPreViewData.isCreatItem = false;
                mPreViewData.preViewItemList = new List<PreViewItemData>();
                PreViewItemData preViewItem = new PreViewItemData();
                preViewItem.activityId = iGoblinShopActivityID;
                preViewItem.itemId = iGoblinShopPreviewItemID;

                mPreViewData.preViewItemList.Add(preViewItem);

                mPreViewBtn.onClick.RemoveAllListeners();
                mPreViewBtn.onClick.AddListener(() => 
                {
                    ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, mPreViewData);
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

            iGoblinShopActivityID = 0;
            iGoblinShopPreviewItemID = 0;
            mMallItemTableId = 0;
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
