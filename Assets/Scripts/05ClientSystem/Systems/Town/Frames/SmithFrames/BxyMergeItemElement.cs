using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnBxyMergeItemClick(ItemData itemData, BxyMergeItemElement element);

    public class BxyMergeItemElement : MonoBehaviour
    {
        [SerializeField] GameObject mItemParent;
        [SerializeField] Text mItemName;
        [SerializeField] Text mItemAttrs;
        [SerializeField] private GameObject mMaskRoot;
        [SerializeField] private GameObject mCheckMaskRoot;
        [SerializeField] private Text mSelectedBxyNum;
        [SerializeField] private Button mAddBxyBtn;
        [SerializeField] private UIGray mAddBxyGray;
        [SerializeField] private Button mBxyItemBtn;

        private OnBxyMergeItemClick mOnBxyMergeItemClick;
        private EnchantmentsFunctionData mDataMerge;
        private ComItemNew comItem;
        private ItemData bxyItem;
        public ItemData BxyItemData
        {
            get { return bxyItem; }
            set { bxyItem = value; }
        }

        private int currentSelectBxyQultity;//当前选择的辟邪玉品质

        private void Awake()
        {
            if (mAddBxyBtn != null)
            {
                mAddBxyBtn.onClick.RemoveAllListeners();
                mAddBxyBtn.onClick.AddListener(OnBxyItemClick);
            }

            if (mBxyItemBtn != null)
            {
                mBxyItemBtn.onClick.RemoveAllListeners();
                mBxyItemBtn.onClick.AddListener(OnBxyItemClick);
            }
        }

        private void OnDestroy()
        {
            comItem = null;
            mOnBxyMergeItemClick = null;
            mDataMerge = null;
            BxyItemData = null;
        }

        public void OnItemVisiable(ItemData itemData, int selectBxyQultity, OnBxyMergeItemClick onBxyMergeItemClick, EnchantmentsFunctionData dataMerge)
        {
            if (itemData == null)
            {
                return;
            }

            BxyItemData = itemData;
            currentSelectBxyQultity = selectBxyQultity;
            mDataMerge = dataMerge;
            mOnBxyMergeItemClick = onBxyMergeItemClick;

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            comItem.Setup(BxyItemData, Utility.OnItemClicked);

            mItemName.text = BxyItemData.GetColorName();

            List<string> texts = BxyItemData.GetComplexAttrDescs();
            string text = "";
            for (int i = 0; i < texts.Count; i++)
            {
                text = text + texts[i] + "\n";
            }
            
            mItemAttrs.text = text;

            // 置灰去掉
            if(currentSelectBxyQultity != 0)
            {
                mMaskRoot.CustomActive(false);
                UpdateSelectedBxyNumber();
            }
            else
            {
                mMaskRoot.CustomActive(false);
                SetCheckMaskRoot(false);
            }
        }

        private void UpdateSelectedBxyNumber()
        {
            if (mDataMerge != null)
            {
                int number = 0;
                if (mDataMerge.leftItem != null)
                {
                    if (mDataMerge.leftItem.GUID == BxyItemData.GUID)
                    {
                        number++;
                    }
                }
            
                if (mDataMerge.rightItem != null)
                {
                    if (mDataMerge.rightItem.GUID == BxyItemData.GUID)
                    {
                        number++;
                    }
                }

                if (number > 0)
                {
                    SetCheckMaskRoot(true);
                    mSelectedBxyNum.text = string.Format("已选");
                    mAddBxyGray.enabled = !(number < BxyItemData.Count);
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

        private void OnBxyItemClick()
        {
            if (mOnBxyMergeItemClick != null)
            {
                mOnBxyMergeItemClick.Invoke(BxyItemData, this);
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