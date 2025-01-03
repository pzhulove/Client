using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonItemInfo : MonoBehaviour
    {
        [SerializeField] private Image mBackGround;
        [SerializeField] private Image mIcon;
        [SerializeField] private Button mItemBtn;
        [SerializeField] private Text mItemName;
        [SerializeField] private Text mItemType;
        [SerializeField] private Text mItemAttr;
        
        private ItemData mItemData = null;
        private void Awake()
        {
            if (mItemBtn != null)
            {
                mItemBtn.onClick.RemoveAllListeners();
                mItemBtn.onClick.AddListener(() => 
                {
                    ItemTipManager.GetInstance().ShowTip(mItemData);
                });
            }
        }

        public void InitInterface(object data,ItemData equipmentItemData)
        {
            if (data == null || equipmentItemData == null)
            {
                return;
            }

            string backgroundPath = string.Empty;
            string iconPath = string.Empty;
            string attrDesc = string.Empty;
            string itemName = string.Empty;

            if (data is PrecBead)
            {
                var precBead = data as PrecBead;
                
                mItemData = ItemDataManager.CreateItemDataFromTable(precBead.preciousBeadId);

                backgroundPath = mItemData.GetQualityInfo().Background;

                iconPath = mItemData.Icon;

                itemName = mItemData.GetColorName();

                attrDesc = BeadCardManager.GetInstance().GetAttributesDesc(precBead.preciousBeadId);
                if (precBead.randomBuffId > 0)
                {
                    attrDesc += string.Format("\n附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(precBead.randomBuffId));
                }
            }
            else if (data is PrecEnchantmentCard)
            {
                var preEnchantmentCard = data as PrecEnchantmentCard;
               
                mItemData = ItemDataManager.CreateItemDataFromTable(preEnchantmentCard.iEnchantmentCardID);

                backgroundPath = mItemData.GetQualityInfo().Background;

                iconPath = mItemData.Icon;

                string levelDesc = string.Empty;
                if (preEnchantmentCard.iEnchantmentCardLevel > 0)
                {
                    levelDesc = string.Format("+{0}", preEnchantmentCard.iEnchantmentCardLevel);
                }
                itemName = TR.Value("tip_magic_attribute_desc", mItemData.GetColorName(), levelDesc, mItemData.GetQualityInfo().ColStr);

                attrDesc = equipmentItemData.GetMagicDescs();
            }
            else if (data is InscriptionHoleData)
            {
                var inscriptionHoleData = data as InscriptionHoleData;
                
                mItemData = ItemDataManager.CreateItemDataFromTable(inscriptionHoleData.InscriptionId);
                if (mItemData == null)
                {
                    mIcon.CustomActive(false);

                    var inscriptionHoleSetTable = TableManager.GetInstance().GetTableItem<InscriptionHoleSetTable>(inscriptionHoleData.Type);
                    if (inscriptionHoleSetTable != null)
                    {
                        backgroundPath = inscriptionHoleSetTable.InscriptionHolePicture;

                        ETCImageLoader.LoadSprite(ref mBackGround, backgroundPath);
                    }

                    mItemName.text = "暂未镶嵌铭文";
                    return;
                }

                backgroundPath = mItemData.GetQualityInfo().Background;

                iconPath = mItemData.Icon;

                itemName = mItemData.GetColorName();

                attrDesc = mItemData.GetInscriptionAttrDesc();
            }

            ETCImageLoader.LoadSprite(ref mBackGround, backgroundPath);

            ETCImageLoader.LoadSprite(ref mIcon, iconPath);
            
            mItemName.text = itemName;

            mItemType.text = GetItemTypeDesc(mItemData);

            mItemAttr.text = attrDesc;
        }

        private string GetItemTypeDesc(ItemData item)
        {
            string desc = string.Empty;
            if (item.SubType == (int)ItemTable.eSubType.Bead)
            {
                desc = "宝珠";
            }
            else if (item.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
            {
                desc = "附魔";
            }
            else if (item.SubType == (int)ItemTable.eSubType.ST_INSCRIPTION)
            {
                desc = "铭文";
            }

            return desc;
        }
        
        private void OnDestroy()
        {
            mItemData = null;
        }
    }
}

