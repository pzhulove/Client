using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class StorageItem : MonoBehaviour
    {

        private ulong _itemGuid;

        [Space(10)]
        [HeaderAttribute("ItemBg")]
        [Space(10)]
        [SerializeField] private GameObject itemBgPrefab;
        [SerializeField] private GameObject itemBgRoot;

        [Space(10)]
        [HeaderAttribute("Root")]
        [Space(10)]
        [SerializeField] private GameObject comItemRoot;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _itemGuid = 0;
        }

        public void InitStorageItem(StorageItemDataModel storageItemDataModel)
        {
            _itemGuid = 0;
            if (storageItemDataModel != null)
                _itemGuid = storageItemDataModel.ItemGuid;

            InitItemView();
        }

        public void OnItemRecycle()
        {
            _itemGuid = 0;
        }


        private void InitItemView()
        {
            var itemData = ItemDataManager.GetInstance().GetItem(_itemGuid);

            //if (itemData == null)
            //{
            //    CommonUtility.UpdateGameObjectVisible(comItemRoot, false);

            //    CommonUtility.UpdateGameObjectVisible(itemBgRoot, true);

            //    if (itemBgRoot != null && itemBgPrefab != null)
            //    {
            //        var itemBgGo = itemBgRoot.GetComponentInChildren<Image>();
            //        if (itemBgGo == null)
            //        {
            //            var bgGo = Instantiate(itemBgPrefab) as GameObject;

            //            if (bgGo != null)
            //            {
            //                bgGo.transform.SetParent(itemBgRoot.transform, false);
            //                CommonUtility.UpdateGameObjectVisible(bgGo, true);
            //            }
            //        }
            //    }
            //    return;
            //}

            CommonUtility.UpdateGameObjectVisible(itemBgRoot, false);
            CommonUtility.UpdateGameObjectVisible(comItemRoot, true);

            var comItem = comItemRoot.GetComponentInChildren<ComItem>();
            if (comItem == null)
                comItem = ComItemManager.Create(comItemRoot);

            if (comItem != null)
                comItem.Setup(itemData, OnStorageItemClick, true, _OnDoubleClickStorageItem);
        }

        private void OnStorageItemClick(GameObject obj, ItemData itemData)
        {
            List<TipFuncButon> funcButtonList = new List<TipFuncButon>();
            var tempFunc = new TipFuncButonSpecial()
            {
                text = TR.Value("tip_take"),
                callback = OnTakeStorageItem,
            };
            funcButtonList.Add(tempFunc);

            ItemTipManager.GetInstance().ShowTip(itemData, funcButtonList, TextAnchor.MiddleRight);
        }

        private void _OnDoubleClickStorageItem(ItemData itemData)
        {
            if (itemData != null)
            {
                ItemDataManager.GetInstance().TakeItem(itemData, itemData.Count);
            }
        }

        private void OnTakeStorageItem(ItemData itemData, object data)
        {
            if (itemData != null)
            {
                if (itemData.Count > 1)
                {
                    var storeItemFrame =
                        ClientSystemManager.GetInstance().OpenFrame<StoreItemFrame>(FrameLayer.Middle) as StoreItemFrame;
                    if (storeItemFrame != null)
                        storeItemFrame.TakeItem(itemData);
                }
                else
                {
                    ItemDataManager.GetInstance().TakeItem(itemData, itemData.Count);

                }
            }

            ItemTipManager.GetInstance().CloseAll();
        }

    }
}