using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnEquipJichengItemClick(ItemData itemData, EquipJichengItemElement element);

    public class EquipJichengItemElement : MonoBehaviour
    {
        [SerializeField] GameObject mItemParent;
        [SerializeField] Text mItemName;
        [SerializeField] Text mItemAttrs;
        [SerializeField] private GameObject mMaskRoot;
        [SerializeField] private GameObject mCheckMaskRoot;
        [SerializeField] private Text mSelectedEquipJichengNum;
        [SerializeField] private Button mAddEquipJichengBtn;
        [SerializeField] private UIGray mAddEquipJichengGray;
        [SerializeField] private Button mEquipJichengItemBtn;

        private OnEquipJichengItemClick mOnEquipJichengItemClick;
        private EnchantmentsFunctionData mDataMerge;
        private ComItemNew comItem;
        private ItemData equipJichengItem;
        public ItemData EquipJichengItemData
        {
            get { return equipJichengItem; }
            set { equipJichengItem = value; }
        }

        private int currentSelectEquipJichengSubType;//当前选择的装备部位
        private int currentSelectEquipJichengQuality;//当前选择的装备品质

        private void Awake()
        {
            if (mAddEquipJichengBtn != null)
            {
                mAddEquipJichengBtn.onClick.RemoveAllListeners();
                mAddEquipJichengBtn.onClick.AddListener(OnEquipJichengItemClick);
            }

            if (mEquipJichengItemBtn != null)
            {
                mEquipJichengItemBtn.onClick.RemoveAllListeners();
                mEquipJichengItemBtn.onClick.AddListener(OnEquipJichengItemClick);
            }
        }

        private void OnDestroy()
        {
            comItem = null;
            mOnEquipJichengItemClick = null;
            mDataMerge = null;
            EquipJichengItemData = null;
        }

        public void OnItemVisiable(ItemData itemData, int selectEquipJichengSubType, int selectEquipJichengQuality, OnEquipJichengItemClick onEquipJichengMergeItemClick, EnchantmentsFunctionData dataMerge)
        {
            if (itemData == null)
            {
                return;
            }

            EquipJichengItemData = itemData;
            currentSelectEquipJichengSubType = selectEquipJichengSubType;
            currentSelectEquipJichengQuality = selectEquipJichengQuality;
            mDataMerge = dataMerge;
            mOnEquipJichengItemClick = onEquipJichengMergeItemClick;

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            comItem.Setup(EquipJichengItemData, Utility.OnItemClicked);

            mItemName.text = EquipJichengItemData.GetColorName();

            // List<string> texts = EquipJichengItemData.GetComplexAttrDescs();
            // string text = "";
            // for (int i = 0; i < texts.Count; i++)
            // {
            //     text = text + texts[i] + "\n";
            // }
            
            // mItemAttrs.text = text;

            // 置灰去掉
            if(currentSelectEquipJichengSubType != 0)
            {
                if (currentSelectEquipJichengSubType == (int)EquipJichengItemData.SubType && currentSelectEquipJichengQuality == (int)EquipJichengItemData.Quality)
                {
                    mMaskRoot.CustomActive(false);
                    UpdateSelectedEquipJichengNumber();
                }
                else
                {
                    mMaskRoot.CustomActive(true);
                    SetCheckMaskRoot(false);
                }
                // if (currentSelectEquipJichengSubType != (int)EquipJichengItemData.SubType)
                // {
                //     mMaskRoot.CustomActive(true);
                //     SetCheckMaskRoot(false);
                // }
                // else
                // {
                //     mMaskRoot.CustomActive(false);
                //     UpdateSelectedEquipJichengNumber();
                // }
            }
            else
            {
                mMaskRoot.CustomActive(false);
                SetCheckMaskRoot(false);
            }
        }

        private void UpdateSelectedEquipJichengNumber()
        {
            if (mDataMerge != null)
            {
                int number = 0;
                if (mDataMerge.leftItem != null)
                {
                    if (mDataMerge.leftItem.GUID == EquipJichengItemData.GUID)
                    {
                        number++;
                    }
                }
            
                if (mDataMerge.rightItem != null)
                {
                    if (mDataMerge.rightItem.GUID == EquipJichengItemData.GUID)
                    {
                        number++;
                    }
                }

                if (number > 0)
                {
                    SetCheckMaskRoot(true);
                    mSelectedEquipJichengNum.text = string.Format("已选");
                    mAddEquipJichengGray.enabled = !(number < EquipJichengItemData.Count);
                }
                else
                {
                    SetCheckMaskRoot(false);
                }
            }
            else
            {
                SetCheckMaskRoot(false);
            }
        }

        private void OnEquipJichengItemClick()
        {
            if (mOnEquipJichengItemClick != null)
            {
                mOnEquipJichengItemClick.Invoke(EquipJichengItemData, this);
            }
        }

        public void SetCheckMaskRoot(bool value)
        {
            if (mCheckMaskRoot != null)
            {
                mCheckMaskRoot.CustomActive(value);
            }
        }
    }
}