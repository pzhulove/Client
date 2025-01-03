using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class PickBeadExpendItem : MonoBehaviour
    {
        [SerializeField]
        private StateController mOwnNumState;
        [SerializeField]
        private GameObject mItemPos;
        [SerializeField]
        private Text mOwnNum;
        [SerializeField]
        private Text mExpendNum;
        [SerializeField]
        private Text mPickSuccesRate;
        [SerializeField]
        private Button mItemClickBtn;
        [SerializeField]
        private Toggle mItemTog;
        [SerializeField]
        private Text mPickNumber;
        [SerializeField]
        private string mStr;
        [SerializeField]
        private string mPickNumberDes;
        BeadPickItemModel mBeadPickItemModel;
        BeadPickModel mBeadPickModel;
        ComItemNew mBeadPickExpendItem;
        string mStrEnable = "Enable";
        string mStrDisable = "Disable";
        int mCureentSelectExpendItemID = 0;
        int mCurrentSelectExpendItemSuccessRate = 0;
        int mBeadPickNumber = 0;//宝珠摘取了几次
        int mBeadRemainPickNumber = -1;//宝珠剩余摘取次数
        void Start()
        {
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
        }
        public void Init(BeadPickItemModel item,ToggleGroup group, BeadPickModel beadPickModel)
        {
            if (item == null)
            {
                return;
            }
            mBeadPickItemModel = item;
            mBeadPickExpendItem = ComItemManager.CreateNew(mItemPos);
            mItemTog.group = group;
            this.mBeadPickModel = beadPickModel;
            this.mBeadPickNumber = this.mBeadPickModel.mPrecBead.pickNumber;

            UpdatePickBeadExpendItem();
            SetPickSeccessRate();
            OnItemClickLink();
            ToggleOnClick();
        }

        void SetPickSeccessRate()
        {
            if (mPickSuccesRate != null)
            {
                mPickSuccesRate.text = string.Format(mStr, mBeadPickItemModel.mPickSuccessRate / 100);
            }
        }

        void OnItemClickLink()
        {
            if (mItemClickBtn != null)
            {
                mItemClickBtn.SafeAddOnClickListener(() =>
                {
                    ItemComeLink.OnLink(mBeadPickItemModel.mExpendItemID, 0);
                });
            }
        }

        void ToggleOnClick()
        {
            if (mItemTog != null)
            {
                mItemTog.onValueChanged.RemoveAllListeners();
                mItemTog.onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        mCureentSelectExpendItemID = mBeadPickItemModel.mExpendItemID;
                        mCurrentSelectExpendItemSuccessRate = mBeadPickItemModel.mPickSuccessRate;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSelectPickBeadExpendItem, mCureentSelectExpendItemID, mCurrentSelectExpendItemSuccessRate,mBeadRemainPickNumber);
                    }
                });
            }
        }

        public void UpdatePickBeadExpendItem()
        {
            if (mBeadPickExpendItem != null)
            {
                int iOwnedCount = 0;
                int iNeedCount = mBeadPickItemModel.mExpendCount;

                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mBeadPickItemModel.mExpendItemID);
                iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(mBeadPickItemModel.mExpendItemID);
                mBeadPickExpendItem.Setup(itemData, Utility.OnItemClicked);

                if (mBeadPickExpendItem != null)
                {
                    mBeadPickExpendItem.SetEnable(iNeedCount <= iOwnedCount);
                }

                if (mOwnNum != null)
                {
                    mOwnNum.text = iOwnedCount.ToString();
                }

                if (mExpendNum != null)
                {
                    mExpendNum.text = iNeedCount.ToString();
                }

                if (mOwnNumState != null)
                {
                    mOwnNumState.Key = iNeedCount <= iOwnedCount ? mStrEnable : mStrDisable;
                }

                if (itemData.SubType == (int)ItemTable.eSubType.ST_PEARL_HAMMER)
                {
                    if (mBeadPickItemModel.mBeadPickTotleNumber > 0)
                    {
                        if (mBeadPickNumber >= mBeadPickItemModel.mBeadPickTotleNumber)
                        {
                            mBeadRemainPickNumber = 0;
                        }
                        else
                        {
                            mBeadRemainPickNumber = mBeadPickItemModel.mBeadPickTotleNumber - mBeadPickNumber;
                        }
                        
                        mPickNumber.text = string.Format(mPickNumberDes, mBeadRemainPickNumber);
                        mPickNumber.CustomActive(true);
                    }
                    else
                    {
                        mPickNumber.CustomActive(false);
                    }
                }
                else
                {
                    mPickNumber.CustomActive(false);
                }
            }
        }

        void OnDestroy()
        {
            if (mBeadPickExpendItem != null)
            {
                ComItemManager.DestroyNew(mBeadPickExpendItem);
                mBeadPickExpendItem = null;
            }

            if (mItemClickBtn != null)
            {
                mItemClickBtn.onClick.RemoveAllListeners();
            }

            if (mItemTog != null)
            {
                mItemTog.onValueChanged.RemoveAllListeners();
            }

            mCureentSelectExpendItemID = 0;
            mCurrentSelectExpendItemSuccessRate = 0;
            mBeadPickItemModel = null;
            mBeadRemainPickNumber = -1;

            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
        }

        void _OnAddNewItem(List<Item> items)
        {
            UpdatePickBeadExpendItem();
        }
        void _OnRemoveItem(ItemData data)
        {
            UpdatePickBeadExpendItem();
        }
        void _OnUpdateItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var mItemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (mItemData == null)
                {
                    continue;
                }

                if (mItemData.GUID == mBeadPickModel.mEquipItemData.GUID)
                {
                    mBeadPickNumber = mItemData.PreciousBeadMountHole[mBeadPickModel.mPrecBead.index - 1].pickNumber;
                    break;
                }

            }

            UpdatePickBeadExpendItem();
        }
    }
}

