using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public class ShopNewMoneyItem : MonoBehaviour
    {

        private int _itemId = 0;
        private ItemData _itemData = null;

        [SerializeField] private Image itemIcon;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Text itemCountText;
        [SerializeField] private Button itemTipButton;

        [SerializeField] private ItemComeLink itemComeLink;

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (itemTipButton != null)
            {
                itemTipButton.onClick.RemoveAllListeners();
                itemTipButton.onClick.AddListener(OnItemTipButtonClick);
            }

            if (itemComeLink != null)
                itemComeLink.onClick += OnItemLinkClick;

        }

        private void OnEnable()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        private void OnDisable()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {           
            if (itemTipButton != null)
            {
                itemTipButton.onClick.RemoveAllListeners();
            }

            if (itemComeLink != null)
                itemComeLink.onClick -= OnItemLinkClick;
        }

        public void InitMoneyItem(int moneyId)
        {
            _itemId = moneyId;

            InitMoneyItemView();
        }

        private void InitMoneyItemView()
        {
            _itemData = ItemDataManager.CreateItemDataFromTable(_itemId);
            if (_itemData == null)
            {
                Logger.LogErrorFormat("InitMoneyItemView itemData is null and itemId is {0}", _itemId);
                gameObject.CustomActive(false);
                return;
            }

            //显示消耗品的名字 or Icon
            bool isMoneyItemShowName = ShopNewDataManager.GetInstance().IsMoneyItemShowName(_itemId);
            if (isMoneyItemShowName == false)
            {
                itemIcon.gameObject.CustomActive(true);
                ETCImageLoader.LoadSprite(ref itemIcon, _itemData.Icon);
                itemNameText.gameObject.CustomActive(false);
            }
            else
            {
                itemIcon.gameObject.CustomActive(false);
                itemNameText.gameObject.CustomActive(true);
                itemNameText.text = _itemData.Name;
            }

            UpdateItemCount();

            itemComeLink.bNotEnough = false;
            itemComeLink.iItemLinkID = _itemId;

        }

        private void OnItemTipButtonClick()
        {
            if(_itemData == null)
                return;

            ItemTipManager.GetInstance().CloseAll();
            ItemTipManager.GetInstance().ShowTip(_itemData);
        }

        private void OnItemLinkClick()
        {
        }

        private void OnAddNewItem(List<Item> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData != null && itemData.TableID == _itemId)
                {
                    UpdateItemCount();
                    break;
                }
            }
        }

        private void OnRemoveItem(ItemData data)
        {
            if (data != null && data.TableID == _itemId)
            {
                UpdateItemCount();
            }
        }

        private void OnUpdateItem(List<Item> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData != null && itemData.TableID == _itemId)
                {
                    UpdateItemCount();
                    break;
                }
            }
        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
        {
            UpdateItemCount();
        }

        private void UpdateItemCount()
        {
            var itemCount = ItemDataManager.GetInstance().GetOwnedItemCount(_itemId, false);
            if (itemCountText != null)
            {
                itemCountText.text = itemCount.ToString();
                itemCountText.text = Utility.ToThousandsSeparator((ulong)itemCount);        //转化为2,500,900类型
            } 
        }
    }
}