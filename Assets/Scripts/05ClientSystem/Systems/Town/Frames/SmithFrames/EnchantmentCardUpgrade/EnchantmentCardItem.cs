using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public delegate void OnEnchantmentCardItemClick(EnchantmentCardItemDataModel model);
    public class EnchantmentCardItem : MonoBehaviour
    {
        [SerializeField]private Text mEnchantmentCardName;
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private GameObject mEquipItemParent;
        [SerializeField]private GameObject mCheckMarkRoot;
        [SerializeField]private StateController mStateControl;
        [SerializeField]private Text mEnchantmentCardAttr;
        [SerializeField]private string sMounted = "Mounted";
        [SerializeField]private string sUnMounted = "UnMounted";

        private EnchantmentCardItemDataModel mEnchantmentCardItemDataModel;
        public EnchantmentCardItemDataModel MEnchantmentCardItemDataModel
        {
            get { return mEnchantmentCardItemDataModel; }
        }
        private OnEnchantmentCardItemClick mOnEnchantmentCardItemClick;
        private ComItem mEnchantmentCardItem;
        private ComItemNew mEquipItem;
        public void OnItemVisiable(EnchantmentCardItemDataModel model, OnEnchantmentCardItemClick onEnchantmentCardItemClick)
        {
            if (model == null || onEnchantmentCardItemClick == null)
            {
                return;
            }

            mEnchantmentCardItemDataModel = model;
            mOnEnchantmentCardItemClick = onEnchantmentCardItemClick;

            InitEnchantmentCardItem();
        }

        private void InitEnchantmentCardItem()
        {
            if (mEnchantmentCardItemDataModel == null)
            {
                return;
            }

            //附魔卡名字
            if (mEnchantmentCardName != null)
            {
                mEnchantmentCardName.text  = mEnchantmentCardItemDataModel.mEnchantmentCardItemData.GetColorName();
            }
            
            //创建附魔卡Icon
            if (mItemParent != null)
            {
                if (mEnchantmentCardItem == null)
                {
                    mEnchantmentCardItem = ComItemManager.Create(mItemParent);
                }

                mEnchantmentCardItem.Setup(mEnchantmentCardItemDataModel.mEnchantmentCardItemData, Utility.OnItemClicked);
            }
            
            if (mEnchantmentCardItemDataModel.mUpgradePrecType == UpgradePrecType.Mounted)
            {
                if (mStateControl != null)
                {
                    mStateControl.Key = sMounted;
                }

                //创建装备Icon
                if (mEquipItemParent != null)
                {
                    if (mEquipItem == null)
                    {
                        mEquipItem = ComItemManager.CreateNew(mEquipItemParent);
                    }

                    mEquipItem.Setup(mEnchantmentCardItemDataModel.mEquipItemData, Utility.OnItemClicked);
                }
            }
            else
            {
                if (mStateControl != null)
                {
                    mStateControl.Key = sUnMounted;
                }
            }

            //附魔卡属性
            if (mEnchantmentCardAttr != null)
            {
                if (mEnchantmentCardItemDataModel.mEnchantmentCardItemData.mPrecEnchantmentCard != null)
                {
                    mEnchantmentCardAttr.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc((int)mEnchantmentCardItemDataModel.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardID, mEnchantmentCardItemDataModel.mEnchantmentCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                }
                
            }
        }

        /// <summary>
        /// 选择附魔卡Item回调
        /// </summary>
        public void OnSelectEnchantmentCardItemClick()
        {
            if (mOnEnchantmentCardItemClick != null)
            {
                mOnEnchantmentCardItemClick.Invoke(mEnchantmentCardItemDataModel);
            }
        }

        public void OnEnchantmentCardItemChageDisplay(bool bSelected)
        {
            if (mCheckMarkRoot != null)
            {
                mCheckMarkRoot.CustomActive(bSelected);
            }
        }

        private void OnDestroy()
        {
            mEnchantmentCardItemDataModel = null;
            mOnEnchantmentCardItemClick = null;
        }
    }
}
