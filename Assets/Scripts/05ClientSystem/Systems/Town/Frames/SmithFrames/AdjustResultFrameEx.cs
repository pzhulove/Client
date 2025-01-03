using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ResultItemData
    {
        public ItemData itemData;
        public string desc;
    }

    class CommonGetItemData
    {
        public string title;
        public List<ResultItemData> itemDatas;
        public ComItem.OnItemClicked itemClickCallback;
    }

    class AdjustResultFrameEx : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/AdjustResultFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            CommonGetItemData getItemData = userData as CommonGetItemData;
            if (getItemData == null)
            {
                Logger.LogError("open CommonGetItemFrame, userdata is invalid");
                return;
            }

            ETCImageLoader.LoadSprite(ref mTitle, getItemData.title);

            for (int i = 0; i < getItemData.itemDatas.Count; i++)
            {
                var item = getItemData.itemDatas[i];
                GameObject prefab = GameObject.Instantiate(mItem);
                if (prefab != null)
                {
                    prefab.CustomActive(true);
                    Utility.AttachTo(prefab, mItemParent);

                    ComCommonBind comBind = prefab.GetComponent<ComCommonBind>();
                    if (comBind != null)
                    {
                        GameObject itemParent = comBind.GetGameObject("itemParent");
                        Text itemName = comBind.GetCom<Text>("itemname");

                        CreateComItem(itemParent).Setup(item.itemData, getItemData.itemClickCallback);
                        if (itemName != null)
                        {
                            itemName.text = item.desc;
                        }
                    }
                }
            }
            
			AudioManager.instance.PlaySound(12);
        }

        protected override void _OnCloseFrame()
        {

        }

        #region ExtraUIBind
        private GameObject mItemParent = null;
        private Image mTitle = null;
        private GameObject mItem = null;

        protected sealed override void _bindExUI()
        {
            mItemParent = mBind.GetGameObject("ItemParent");
            mTitle = mBind.GetCom<Image>("Title");
            mItem = mBind.GetGameObject("item");
        }

        protected sealed override void _unbindExUI()
        {
            mItemParent = null;
            mTitle = null;
            mItem = null;
        }
        #endregion

        [UIEventHandle("Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
