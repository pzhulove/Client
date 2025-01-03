using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonReplaceItem : MonoBehaviour
    {
        [SerializeField] private Text titleName;
        [SerializeField] private Text itemName;
        [SerializeField] private Text attrDesc;
        [SerializeField] private GameObject itemParent;

        private CommonReplaceData commonReplaceData;
        private bool bIsNew = false;
        public void InitItem(CommonReplaceData commonReplaceData ,bool isNew)
        {
            if (commonReplaceData == null)
                return;

            this.commonReplaceData = commonReplaceData;
            bIsNew = isNew;
            InitContent();
        }

        private void InitContent()
        {
            string title = string.Empty;
            string attrDesc = string.Empty;
            CommonNewItem commonNewItem = CommonUtility.CreateCommonNewItem(itemParent);
            ItemData itemData = null;
            switch (commonReplaceData.commonReplaceType)
            {
                case CommonReplaceType.CRT_BEAD:
                    if (bIsNew)
                    {
                        title = TR.Value("common_replace_new_bead_title_name");
                        if (commonReplaceData.newItemData == null)
                            return;

                        itemData = commonReplaceData.newItemData;
                        attrDesc = BeadCardManager.GetInstance().GetAttributesDesc(commonReplaceData.newItemData.TableID, true);
                        if(commonReplaceData.newItemData.BeadAdditiveAttributeBuffID > 0)
                        {
                            attrDesc += string.Format("[<color=#809CB3FF>附加属性:</color>{0}]", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(commonReplaceData.newItemData.BeadAdditiveAttributeBuffID));
                        }
                    }
                    else
                    {
                        title = TR.Value("common_replace_old_bead_title_name");
                        if (commonReplaceData.oldItemData == null)
                            return;

                        int beadId = commonReplaceData.oldItemData.PreciousBeadMountHole[commonReplaceData.holeIndex - 1].preciousBeadId;
                        int randomBuffId = commonReplaceData.oldItemData.PreciousBeadMountHole[commonReplaceData.holeIndex - 1].randomBuffId;
                        attrDesc = BeadCardManager.GetInstance().GetAttributesDesc(beadId, true);
                        if(randomBuffId > 0)
                        {
                            attrDesc += BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(randomBuffId);
                        }

                        itemData = ItemDataManager.CreateItemDataFromTable(beadId);
                    }
                    break;
                case CommonReplaceType.CRT_MAGICCARD:
                    if (bIsNew)
                    {
                        title = TR.Value("common_replace_new_magic_card_title_name");
                        if (commonReplaceData.newItemData == null)
                            return;

                        itemData = commonReplaceData.newItemData;
                        attrDesc = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(itemData.TableID, itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                    }
                        
                    else
                    {
                        title = TR.Value("common_replace_old_magic_card_title_name");
                        if (commonReplaceData.oldItemData == null)
                            return;

                        itemData = ItemDataManager.CreateItemDataFromTable(commonReplaceData.oldItemData.mPrecEnchantmentCard.iEnchantmentCardID);
                        attrDesc = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(commonReplaceData.oldItemData.mPrecEnchantmentCard.iEnchantmentCardID, commonReplaceData.oldItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);

                    }
                    break;
                case CommonReplaceType.CRT_INSCRIPTIONMOSAIC:
                    if (bIsNew)
                    {
                        title = TR.Value("common_replace_new_inscription_title_name");
                        if (commonReplaceData.newItemData == null)
                            return;

                        itemData = commonReplaceData.newItemData;
                        attrDesc = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(itemData.TableID, true);
                    }
                    else
                    {
                        title = TR.Value("common_replace_old_inscription_title_name");
                        if (commonReplaceData.oldItemData == null)
                            return;

                        itemData = commonReplaceData.oldItemData;
                        attrDesc = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(itemData.TableID, true);
                    }
                    break;
            }

            if (titleName != null)
                titleName.text = title;

            if(commonNewItem != null)
            {
                if (itemData != null)
                    commonNewItem.InitItem(itemData.TableID);
            }

            if (itemData != null)
                OnSetItemName(itemData.GetColorName());

            OnSetAttrDesc(attrDesc);
           
        }

        private void OnSetItemName(string name)
        {
            if (itemName != null)
                itemName.text = name;
        }

        private void OnSetAttrDesc(string attr)
        {
            if (attrDesc != null)
                attrDesc.text = attr;
        }
    }
}