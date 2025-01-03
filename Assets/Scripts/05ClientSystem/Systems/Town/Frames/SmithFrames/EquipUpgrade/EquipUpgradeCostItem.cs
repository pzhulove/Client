using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipUpgradeCostItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject mItemParent;

        [SerializeField]
        private Text mName;

        [SerializeField]
        private Text mCountText;

        [SerializeField]
        private GameObject mGoItemComLink;

        [SerializeField]
        private Button mBtnItemComLink;
        
        ComItemNew comItem = null;
        ItemData itemData = null;

        void Awake()
        {
            if (mBtnItemComLink)
            {
                mBtnItemComLink.onClick.RemoveAllListeners();
                mBtnItemComLink.onClick.AddListener(() => 
                {
                    ItemComeLink.OnLink(itemData.TableID, 0, false);
                } );
            }
        }

        public void OnItemVisiable(ItemSimpleData smipleData)
        {
            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(smipleData.ItemID);
            if (itemData == null)
            {
                return;
            }

            itemData.Count = 0;
            comItem.Setup(itemData, Utility.OnItemClicked);

            mName.text = itemData.GetColorName();

            int mCount = 0;

            if (itemData.Type == ProtoTable.ItemTable.eType.MATERIAL)
            {
                mCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemData.TableID);

                if (mCount >= smipleData.Count)
                {
                    mCountText.text = TR.Value("EquipUpgrade_Merial_white", mCount, smipleData.Count);
                }
                else
                {
                    mCountText.text = TR.Value("EquipUpgrade_Merial_Red", mCount, smipleData.Count);
                }
            }
            else
            {
                mCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemData.TableID);

                if (mCount >= smipleData.Count)
                {
                    mCountText.text = TR.Value("EquipUpgrade_Gold_white", smipleData.Count);
                }
                else
                {
                    mCountText.text = TR.Value("EquipUpgrade_Gold_Red", smipleData.Count);
                }
            }

            mBtnItemComLink.CustomActive(mCount < smipleData.Count);
        }

        /// <summary>
        /// 材料合成调用
        /// </summary>
        /// <param name="smipleData">材料数据</param>
        /// <param name="iSynthesisNumber">合成数量</param>
        public void OnItemVisiable(ItemSimpleData smipleData,int iSynthesisNumber)
        {
            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(smipleData.ItemID);
            if (itemData == null)
            {
                return;
            }

            comItem.Setup(itemData, Utility.OnItemClicked);

            mName.text = itemData.GetColorName();

            int mCount = 0;

            mCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemData.TableID);

            if (mCount >= smipleData.Count * iSynthesisNumber)
            {
                mCountText.text = TR.Value("EquipUpgrade_Merial_white", mCount, smipleData.Count * iSynthesisNumber);
            }
            else
            {
                mCountText.text = TR.Value("EquipUpgrade_Merial_Red", mCount, smipleData.Count * iSynthesisNumber);
            }

            mBtnItemComLink.CustomActive(mCount < smipleData.Count * iSynthesisNumber);
        }


        void OnDestroy()
        {
            comItem = null;
            itemData = null;
        }
    }
}

