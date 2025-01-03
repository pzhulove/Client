using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnSinanItemClick(ItemData itemData, SinanItemElement element);

    public class SinanItemElement : MonoBehaviour
    {
        [SerializeField] GameObject mItemParent;
        [SerializeField] Text mItemName;
        [SerializeField] Text mItemAttrs;
        [SerializeField] private GameObject mMaskRoot;
        [SerializeField] private GameObject mCheckMaskRoot;
        [SerializeField] private Text mSelectedSinanNum;
        [SerializeField] private Button mAddSinanBtn;
        [SerializeField] private UIGray mAddSinanGray;
        [SerializeField] private Button mSinanItemBtn;

        private OnSinanItemClick mOnSinanItemClick;
        private EnchantmentsFunctionData mDataMerge;
        private ComItemNew comItem;
        private ItemData sinanItem;
        public ItemData SinanItemData
        {
            get { return sinanItem; }
            set { sinanItem = value; }
        }

        private int currentSelectSinanQultity;//当前选择的辟邪玉品质

        private void Awake()
        {
            if (mAddSinanBtn != null)
            {
                mAddSinanBtn.onClick.RemoveAllListeners();
                mAddSinanBtn.onClick.AddListener(OnSinanItemClick);
            }

            if (mSinanItemBtn != null)
            {
                mSinanItemBtn.onClick.RemoveAllListeners();
                mSinanItemBtn.onClick.AddListener(OnSinanItemClick);
            }
        }

        private void OnDestroy()
        {
            comItem = null;
            mOnSinanItemClick = null;
            mDataMerge = null;
            SinanItemData = null;
        }

        public void OnItemVisiable(ItemData itemData, int selectSinanQultity, OnSinanItemClick onSinanItemClick, EnchantmentsFunctionData dataMerge)
        {
            if (itemData == null)
            {
                return;
            }

            SinanItemData = itemData;
            currentSelectSinanQultity = selectSinanQultity;
            mDataMerge = dataMerge;
            mOnSinanItemClick = onSinanItemClick;

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            comItem.Setup(SinanItemData, Utility.OnItemClicked);

            mItemName.text = SinanItemData.GetColorName();

            List<string> texts = SinanItemData.GetSinanBuffDescs();
            string text = "";
            for (int i = 0; i < texts.Count; i++)
            {
                text = text + texts[i] + "\n";
            }
            
            mItemAttrs.text = text;

            // 置灰去掉
            if(currentSelectSinanQultity != 0)
            {
                mMaskRoot.CustomActive(false);
                UpdateSelectedSinanNumber();
            }
            else
            {
                mMaskRoot.CustomActive(false);
                SetCheckMaskRoot(false);
            }
        }

        private void UpdateSelectedSinanNumber()
        {
            if (mDataMerge != null)
            {
                int number = 0;
                if (mDataMerge.leftItem != null)
                {
                    if (mDataMerge.leftItem.GUID == SinanItemData.GUID)
                    {
                        number++;
                    }
                }

                if (number > 0)
                {
                    SetCheckMaskRoot(true);
                    mSelectedSinanNum.text = string.Format("已选");
                    mAddSinanGray.enabled = !(number < SinanItemData.Count);
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

        private void OnSinanItemClick()
        {
            if (mOnSinanItemClick != null)
            {
                mOnSinanItemClick.Invoke(SinanItemData, this);
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