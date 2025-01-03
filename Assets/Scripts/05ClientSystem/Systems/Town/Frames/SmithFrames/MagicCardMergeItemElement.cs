using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnMagicCardMergeItemClick(ItemData itemData, MagicCardMergeItemElement element);
    public class MagicCardMergeItemElement : MonoBehaviour
    {
        [SerializeField] GameObject mItemParent;
        [SerializeField] Text mItemName;
        [SerializeField] Text mItemAttrs;
        [SerializeField] private GameObject mMaskRoot;
        [SerializeField] private GameObject mCheckMaskRoot;
        [SerializeField] private Text mSelectedMagicCardNum;
        [SerializeField] private Button mAddMagicCardBtn;
        [SerializeField] private UIGray mAddMagicCardGray;
        [SerializeField] private Button mMagicCardItemBtn;


        private OnMagicCardMergeItemClick mOnMagicCardMergeItemClick;
        private EnchantmentsFunctionData mDataMerge;
        private ComItemNew comItem;
        private ItemData magicCardItem;
        public ItemData MagicCardItemData
        {
            get { return magicCardItem; }
            set { magicCardItem = value; }
        }

        private int currentSelectMagicCardQultity;//当前选择的卡牌品质

        private void Awake()
        {
            if (mAddMagicCardBtn != null)
            {
                mAddMagicCardBtn.onClick.RemoveAllListeners();
                mAddMagicCardBtn.onClick.AddListener(OnMagicCardItemClick);
            }

            if (mMagicCardItemBtn != null)
            {
                mMagicCardItemBtn.onClick.RemoveAllListeners();
                mMagicCardItemBtn.onClick.AddListener(OnMagicCardItemClick);
            }
        }

        private void OnDestroy()
        {
            comItem = null;
            mOnMagicCardMergeItemClick = null;
            mDataMerge = null;
            MagicCardItemData = null;
        }

        public void OnItemVisiable(ItemData itemData, int selectMagicCardQultity, OnMagicCardMergeItemClick onMagicCardMergeItemClick, EnchantmentsFunctionData dataMerge)
        {
            if (itemData == null)
            {
                return;
            }

            MagicCardItemData = itemData;
            currentSelectMagicCardQultity = selectMagicCardQultity;
            mDataMerge = dataMerge;
            mOnMagicCardMergeItemClick = onMagicCardMergeItemClick;

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            comItem.Setup(MagicCardItemData, Utility.OnItemClicked);

            mItemName.text = MagicCardItemData.GetColorName();

            mItemAttrs.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(MagicCardItemData.TableID, MagicCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);

            if (currentSelectMagicCardQultity != 0)
            {
                if (currentSelectMagicCardQultity != (int)MagicCardItemData.Quality)
                {
                    mMaskRoot.CustomActive(true);
                    SetCheckMaskRoot(false);
                }
                else
                {
                    mMaskRoot.CustomActive(false);
                    UpdateSelectedMagicCardNumber();
                }
            }
            else
            {
                //置灰去掉
                mMaskRoot.CustomActive(false);
                SetCheckMaskRoot(false);
            }
        }

        private void UpdateSelectedMagicCardNumber()
        {
            if (mDataMerge != null)
            {
                int number = 0;
                if (mDataMerge.leftItem != null)
                {
                    if (mDataMerge.leftItem.GUID == MagicCardItemData.GUID)
                    {
                        number++;
                    }
                }
            
                if (mDataMerge.rightItem != null)
                {
                    if (mDataMerge.rightItem.GUID == MagicCardItemData.GUID)
                    {
                        number++;
                    }
                }

                if (number > 0)
                {
                    SetCheckMaskRoot(true);
                    mSelectedMagicCardNum.text = string.Format("已选:{0}", number);
                    mAddMagicCardGray.enabled = !(number < MagicCardItemData.Count);
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

        private void OnMagicCardItemClick()
        {
            if (mOnMagicCardMergeItemClick != null)
            {
                mOnMagicCardMergeItemClick.Invoke(MagicCardItemData, this);
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