using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnEquipItemClickDelegate(ulong guid,bool isAdd);
    public class ChijiHandInEquipmentItem : MonoBehaviour
    {
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private GameObject mCheckMarkRoot;
        [SerializeField]private Text mEquipName;
        [SerializeField]private Toggle mAddItemToggle;
        [SerializeField]private int mMaxHandInNumber = 5;//最大上交数量

        private ComItem mComItem;
        private ItemData mCurrent;
        private OnEquipItemClickDelegate mOnEquipItemClick;
        public void OnItemVisiable(ItemData itemData, OnEquipItemClickDelegate onEquipItemClick)
        {
            mCurrent = itemData;
            mOnEquipItemClick = onEquipItemClick;

            if (mComItem == null)
            {
                mComItem = ComItemManager.Create(mItemParent);
            }

            mComItem.Setup(mCurrent, Utility.OnItemClicked);

            mEquipName.text = mCurrent.GetColorName();

            bool isAdd = ChijiHandInEquipmentView.mHandInEquipmentList.Contains(mCurrent.GUID);
            mCheckMarkRoot.CustomActive(isAdd);

            mAddItemToggle.onValueChanged.RemoveAllListeners();
            mAddItemToggle.onValueChanged.AddListener(OnAddItemTogClick);
            
        }

        private void OnAddItemTogClick(bool value)
        {
            if (value)
            { 
                //超过5个不能选择
                if (ChijiHandInEquipmentView.hasSelectNumber >= mMaxHandInNumber)
                {
                    mAddItemToggle.isOn = false;
                    return;
                }

                if (mOnEquipItemClick != null)
                {
                    mOnEquipItemClick.Invoke(mCurrent.GUID, true);
                }
            }
            else
            {
                if (mOnEquipItemClick != null)
                {
                    mOnEquipItemClick.Invoke(mCurrent.GUID, false);
                }
            }

            mCheckMarkRoot.CustomActive(value);
        }
        
        private void OnDestroy()
        {
            mComItem = null;
            mCurrent = null;
            mOnEquipItemClick = null;
    }
    }
}
