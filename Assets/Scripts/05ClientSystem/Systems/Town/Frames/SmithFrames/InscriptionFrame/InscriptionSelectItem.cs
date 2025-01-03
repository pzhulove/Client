using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;


namespace GameClient
{
    public delegate void OnSelectInscriptionItemClick(ItemData itemData, InscriptionSelectItem inscriptionSelectItem);
    // 用于显示在选择区的铭文item
    public class InscriptionSelectItem : MonoBehaviour
    {
        [SerializeField] GameObject itemParent = null;

        [SerializeField] Text itemName = null;

        [SerializeField] Text itemAttrs = null;

        [SerializeField] private GameObject maskRoot;

        [SerializeField] private GameObject mCheckMaskRoot;
        [SerializeField] private Text mSelectedInscrptionNum;
        [SerializeField] private Button mAddInscriptionBtn;
        [SerializeField] private UIGray mAddInscriptionGray;
        [SerializeField] private Button mInscriptionItemBtn;

        private List<ItemData> mPutInscriptionItemList;
        private OnSelectInscriptionItemClick mOnSelectInscriptionItemClick;
        private ComItemNew comItem;
        private ItemData inscriptionItem;
        public ItemData InscriptionItemData
        {
            get { return inscriptionItem; }
            set { inscriptionItem = value; }
        }
        private int currentselectIncriptionQultity;
        private void Awake()
        {
            if (mInscriptionItemBtn != null)
            {
                mInscriptionItemBtn.onClick.RemoveAllListeners();
                mInscriptionItemBtn.onClick.AddListener(OnInscriptionItemClick);
            }

            if (mAddInscriptionBtn != null)
            {
                mAddInscriptionBtn.onClick.RemoveAllListeners();
                mAddInscriptionBtn.onClick.AddListener(OnInscriptionItemClick);
            }
        }

        private void OnDestroy()
        {
            comItem = null;
            inscriptionItem = null;
            mOnSelectInscriptionItemClick = null;
            mPutInscriptionItemList = null;
        }

        public void OnItemVisiable(ItemData data, int selectIncriptionQultity, OnSelectInscriptionItemClick onSelectInscriptionItemClick, List<ItemData> putIncriptionList)
        {
            if (data == null)
            {
                return;
            }

            currentselectIncriptionQultity = selectIncriptionQultity;
            mOnSelectInscriptionItemClick = onSelectInscriptionItemClick;
            mPutInscriptionItemList = putIncriptionList;

            inscriptionItem = data;

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(itemParent);
            }

            comItem.Setup(inscriptionItem, Utility.OnItemClicked);

            itemName.text = inscriptionItem.GetColorName();

            itemAttrs.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(inscriptionItem.TableID);

            if (currentselectIncriptionQultity != 0)
            {
                //处理品质大于currentselectIncriptionQultity的置灰
                if (currentselectIncriptionQultity != (int)data.Quality)
                {
                    maskRoot.CustomActive(true);
                    SetCheckMaskRoot(false);
                }
                else
                {
                    maskRoot.CustomActive(false);
                    UpdateSelectedInscriptionNumber();
                }
            }
            else
            {
                //置灰去掉
                maskRoot.CustomActive(false);
                SetCheckMaskRoot(false);
            }
        }

        private void UpdateSelectedInscriptionNumber()
        {
            if (mPutInscriptionItemList != null && mPutInscriptionItemList.Count > 0)
            {
                int number = 0;
                for (int i = 0; i < mPutInscriptionItemList.Count; i++)
                {
                    var item = mPutInscriptionItemList[i];
                    if (item == null)
                    {
                        continue;
                    }

                    if (item.TableID != InscriptionItemData.TableID)
                    {
                        continue;
                    }

                    number++;
                }

                if (number > 0)
                {
                    SetCheckMaskRoot(true);
                    mSelectedInscrptionNum.text = string.Format("已选:{0}", number);
                    mAddInscriptionGray.enabled = !(number < inscriptionItem.Count);
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

        private void OnInscriptionItemClick()
        {
            if (mOnSelectInscriptionItemClick != null)
            {
                mOnSelectInscriptionItemClick.Invoke(InscriptionItemData,this);
            }
        }

        public void SetCheckMaskRoot(bool value)
        {
            mCheckMaskRoot.CustomActive(value);
        }
    }
}


