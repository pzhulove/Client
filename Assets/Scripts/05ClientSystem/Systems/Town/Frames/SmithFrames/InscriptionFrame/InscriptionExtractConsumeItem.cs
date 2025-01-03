using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class InscriptionExtractConsumeItem : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private GameObject mItemComLink;
        [SerializeField] private Text mOwnNum;
        [SerializeField] private Text mExpendNum;
        [SerializeField] private Text mItemName;
        [SerializeField] private Button mItemComLinkBtn;

        private void Awake()
        {
            if (mItemComLinkBtn != null)
            {
                mItemComLinkBtn.onClick.RemoveAllListeners();
                mItemComLinkBtn.onClick.AddListener(OnItemComLinkClick);
            }

            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;
        }

        private void OnDestroy()
        {
            mInscriptionConsume = null;
            mComItem = null;

            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
        }

        private InscriptionConsume mInscriptionConsume;
        private ComItemNew mComItem;
        public void OnItemVisiable(InscriptionConsume consume)
        {
            mInscriptionConsume = consume;
            if (mInscriptionConsume == null)
            {
                return;
            }

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            var itemData = ItemDataManager.CreateItemDataFromTable(consume.itemId);
            if (itemData != null)
            {
                if(mItemName != null)
                {
                    mItemName.text = itemData.GetColorName();
                }

                mComItem.Setup(itemData, Utility.OnItemClicked);
            }

            UpdateConsumNumber();
        }

        private void UpdateConsumNumber()
        {
            int iTotalNumber = 0;
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mInscriptionConsume.itemId);
            if (itemTable != null)
            {
                if (itemTable.SubType == ItemTable.eSubType.GOLD || itemTable.SubType == ItemTable.eSubType.BindGOLD)
                {
                    iTotalNumber = ItemDataManager.GetInstance().GetOwnedItemCount(mInscriptionConsume.itemId,false);
                }
                else
                {
                    iTotalNumber = ItemDataManager.GetInstance().GetItemCountInPackage(mInscriptionConsume.itemId);
                }
            }
            
            int iNeedNumber = mInscriptionConsume.count;

            if (iTotalNumber >= iNeedNumber)
            {
                mOwnNum.color = Color.green;
                mExpendNum.color = Color.white;

                mItemComLink.CustomActive(false);
            }
            else
            {
                mOwnNum.color = Color.red;
                mExpendNum.color = Color.red;

                mItemComLink.CustomActive(true);
            }

            mOwnNum.text = iTotalNumber.ToString();
            mExpendNum.text = iNeedNumber.ToString();

            mComItem.SetShowNotEnoughState(mItemComLink.activeSelf);
        }

        private void OnItemComLinkClick()
        {
            ItemComeLink.OnLink(mInscriptionConsume.itemId, 0);
        }

        private void _OnAddNewItem(List<Item> items)
        {
            UpdateConsumNumber();
        }

        private void _OnRemoveItem(ItemData itemData)
        {
            UpdateConsumNumber();
        }

        private void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            UpdateConsumNumber();
        }
    }
}