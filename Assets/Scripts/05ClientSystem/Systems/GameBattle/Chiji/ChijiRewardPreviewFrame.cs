using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ChijiRewardPreviewFrame : ClientFrame
    {
        private int ItemNum = 0;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiRewardPreviewFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _InitInterface();
            _RefreshRewardItemListCount();
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
        }

        private void _ClearData()
        {
            ItemNum = 0;
        }

        private void _InitInterface()
        {
            var table = TableManager.GetInstance().GetTable<ChijiRewardTable>();
            if(table != null)
            {
                ItemNum = table.Count;
            }
      
            _InitRewardItemScrollListBind();
        }

        void _InitRewardItemScrollListBind()
        {
            mComScrollViewList.Initialize();

            mComScrollViewList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateRewardItemScrollListBind(item);
                }
            };

            mComScrollViewList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }
            };
        }

        void _UpdateRewardItemScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= ItemNum)
            {
                return;
            }

            ChijiRewardTable tableData = TableManager.GetInstance().GetTableItem<ChijiRewardTable>(item.m_index + 1);
            if (tableData == null)
            {
                return;
            }

            Text Name = combind.GetCom<Text>("Name");
            GameObject objRoot = combind.GetGameObject("IconRoot");

            if(Name != null)
            {
                Name.text = tableData.Name;
            }

            if(objRoot != null)
            {
                ComItem AwardItem = objRoot.GetComponentInChildren<ComItem>();

                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(tableData.RewardBoxID);
                if (ItemDetailData == null)
                {
                    if (AwardItem != null)
                    {
                        AwardItem.CustomActive(false);
                    }
                }
                else
                {
                    if (AwardItem == null)
                    {
                        AwardItem = CreateComItem(objRoot);
                    }

                    AwardItem.CustomActive(true);
                    AwardItem.Setup(ItemDetailData, _ShowItemTip);
                }
            }
        }

        void _ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        void _RefreshRewardItemListCount()
        {
            mComScrollViewList.SetElementAmount(ItemNum);
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mComScrollViewList = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mComScrollViewList = mBind.GetCom<ComUIListScript>("comScrollViewList");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mComScrollViewList = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
