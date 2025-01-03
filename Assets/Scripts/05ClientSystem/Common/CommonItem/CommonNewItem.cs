using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class CommonNewItemDataModel
    {
        public int ItemId;
        public int ItemCount;
        public int ItemStrengthenLevel;

        public string GetItemColorName()
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(ItemId);
            if (itemTable != null)
            {
                return CommonUtility.GetItemColorName(itemTable);
            }
            else
            {
                return "";
            }
        }
    }

    public delegate void OnItemClicked(CommonNewItemDataModel dataModel);

    public class CommonNewItem : MonoBehaviour
    {

        private CommonNewItemDataModel _commonNewItemDataModel;
        private int _itemId;
        private int _itemCount;
        private int _itemStrengthenLevel;

        private ItemTable _itemTable;
        private ItemData _itemData;

        [Space(10)]
        [HeaderAttribute("Item")]
        [Space(10)]
        [SerializeField] private Image itemBackground;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Button itemButton;

        [Space(10)]
        [HeaderAttribute("Text")]
        [Space(10)]
        [SerializeField] private Text itemLevelText;
        [SerializeField] private Text itemCountText;
        [SerializeField] private Text itemStrengthenLevelText;
        [SerializeField] private GameObject itemLimitTimeGo;//限时图标


        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (itemButton != null)
            {
                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(OnItemClicked);
            }
        }

        private void UnBindEvents()
        {
            if (itemButton != null)
                itemButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _commonNewItemDataModel = null;
            _itemId = 0;
            _itemCount = 0;
            _itemTable = null;
            _itemData = null;
        }

        public void InitItem(int itemId, int itemCount = 1)
        {
            CommonNewItemDataModel commonNewItemDataModel = new CommonNewItemDataModel()
            {
                ItemId = itemId,
                ItemCount = itemCount,
            };

            InitItem(commonNewItemDataModel);
        }

        public void InitItem(CommonNewItemDataModel dataModel)
        {
            _commonNewItemDataModel = dataModel;
            if (_commonNewItemDataModel == null)
                return;

            _itemId = _commonNewItemDataModel.ItemId;
            _itemCount = _commonNewItemDataModel.ItemCount;
            _itemStrengthenLevel = _commonNewItemDataModel.ItemStrengthenLevel;

            _itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_itemId);

            if (_itemTable == null)
            {
                Logger.LogErrorFormat("ItemTable is null and itemId is {0}", _itemId);
                return;
            }

            InitItemView();
        }


        private void InitItemView()
        {
            //背景边框
            var qualityInfo = ItemData.GetQualityInfo(_itemTable.Color);
            if (itemBackground != null && qualityInfo != null)
                ETCImageLoader.LoadSprite(ref itemBackground, qualityInfo.Background);

            //图片
            if (itemIcon != null)
                ETCImageLoader.LoadSprite(ref itemIcon, _itemTable.Icon);

            UpdateItemCount();
            UpdateItemLevel();
            UpdateItemStrengthenLevel();
            UpdateItemLimitTimeGo();
        }

        private void UpdateItemCount()
        {
            //数量
            if (itemCountText != null)
            {
                if (_itemCount <= 1)
                    itemCountText.gameObject.CustomActive(false);
                else
                {
                    itemCountText.gameObject.CustomActive(true);
                    itemCountText.text = _itemCount.ToString();
                }
            }

        }
        private void UpdateItemLevel()
        {
            if (itemLevelText != null)
            {
                if (_itemTable.Type == ItemTable.eType.EQUIP
                    && _itemTable.NeedLevel > 0)
                {
                    itemLevelText.gameObject.CustomActive(true);
                    itemLevelText.text = string.Format("Lv.{0}", _itemTable.NeedLevel);
                }
                else
                {
                    itemLevelText.gameObject.CustomActive(false);
                }
            }
        }

        private void UpdateItemStrengthenLevel()
        {
            if (itemStrengthenLevelText != null)
            {
                if (_itemStrengthenLevel <= 0)
                {
                    itemStrengthenLevelText.gameObject.CustomActive(false);
                }
                else
                {
                    itemStrengthenLevelText.gameObject.CustomActive(true);
                    itemStrengthenLevelText.text = string.Format("+{0}",
                        _itemStrengthenLevel);
                }
            }
        }

        private void UpdateItemLimitTimeGo()
        {
            _itemData = ItemDataManager.CreateItemDataFromTable(_itemId);

            if (itemLimitTimeGo != null)
            {
                int nTimeLeft;
                bool bStarted;
                _itemData.GetTimeLeft(out nTimeLeft, out bStarted);
                if ((bStarted == true && nTimeLeft > 0) || _itemData.IsTimeLimit == true)
                {
                    itemLimitTimeGo.CustomActive(true);
                }
                else
                {
                    itemLimitTimeGo.CustomActive(false);
                }
            }
        }

        private void OnItemClicked()
        {
            if (_commonNewItemDataModel == null)
                return;

            if (_itemTable == null)
                return;

            ShowItemTipFrame();

        }

        private void ShowItemTipFrame()
        {
            _itemData = ItemDataManager.CreateItemDataFromTable(_itemId);
            _itemData.Count = _itemCount;
            _itemData.StrengthenLevel = _itemStrengthenLevel;
            
            if (_itemData != null)
                ItemTipManager.GetInstance().ShowTip(_itemData);
        }

        //等级的字体大小
        public void SetItemLevelFontSize(int fontSize)
        {
            if (itemLevelText != null)
                itemLevelText.fontSize = fontSize;
        }

        //数量的字体大小
        public void SetItemCountFontSize(int fontSize)
        {
            if (itemCountText != null)
                itemCountText.fontSize = fontSize;
        }

        //重置ItemIcon的Sprite
        private void ResetItemIconSprite()
        {
            if (itemIcon != null)
                itemIcon.sprite = null;
        }

        public void Reset()
        {
            var rect = gameObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(0, 0);
                rect.sizeDelta = new Vector2(0, 0);
                rect.pivot = new Vector2(0.5f, 0.5f);
            }

            ResetItemIconSprite();
        }
    }
}
