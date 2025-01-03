using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class OptionItemData
    {
        public bool IsMaterial;
        public ItemData itemData;
        public ClientFrame frame;
        public bool isLeft;
        public System.Action<ItemData> onItemRemove;
        public System.Action<ItemData,bool> onItemAdd;
        public System.Action<bool> onOpenEquipList;
        public string bg;
    }

    class OptionItem : CachedNormalObject<OptionItemData>
    {
        public ComOptionItem comOption;
        public ComItem comItem;
        static readonly string[] ms_keys = new string[]
        {
                "material_data",
                "data_is_not_null",
                "data_is_null",
        };

        public override void OnRecycle()
        {

        }

        public override void Initialize()
        {
            comOption = goLocal.GetComponent<ComOptionItem>();
            if (comItem == null)
            {
                comItem = Value.frame.CreateComItem(comOption.itemParent);
            }
        }

        public override void UnInitialize()
        {
            comOption.Value = null;
        }

        public override void OnUpdate()
        {
            comOption.Value = Value;

            if (Value.IsMaterial)
            {
                comOption.stateController.Key = ms_keys[0];
            }
            else if (Value.itemData != null)
            {
                comOption.stateController.Key = ms_keys[1];
            }
            else
            {
                comOption.stateController.Key = ms_keys[2];
            }

            comItem.Setup(Value.itemData, (GameObject obj, ItemData item) =>
            {
                if(!Value.IsMaterial)
                {
                    ItemTipManager.GetInstance().ShowTip(item);
                }
                else
                {
                    int iCostMaterial = FashionMergeManager.GetInstance().FashionMergeMaterialID;
                    int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iCostMaterial);
                    int iCostCount = 1;
                    if(iHasCount < iCostCount)
                    {
                        ItemComeLink.OnLink(iCostMaterial, 0);
                    }
                    else
                    {
                        ItemTipManager.GetInstance().ShowTip(item);
                    }
                }
            });

            if (Value.itemData != null)
            {
                if(Value.IsMaterial)
                {
                    //comOption.itemBG.sprite = AssetLoader.instance.LoadRes(Value.bg, typeof(Sprite)).obj as Sprite;
                }
                else
                {
                    //comOption.itemBG.sprite = AssetLoader.instance.LoadRes(Value.itemData.GetQualityInfo().TitleBG, typeof(Sprite)).obj as Sprite;
                }
            }
            else
            {
                //comOption.itemBG.sprite = AssetLoader.instance.LoadRes(Value.bg, typeof(Sprite)).obj as Sprite;
            }

            if (Value.itemData != null)
            {
                comOption.itemName.text =
                comOption.itemRealName.text =
                Value.itemData.GetColorName();
            }

            comOption.buttonAdd.enabled = Value.itemData == null && !Value.IsMaterial;

            if (!Value.IsMaterial && Value.itemData != null)
            {
                comOption.itemRealAttribute.text = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(Value.itemData.FashionAttributeID);
            }

            if (Value.IsMaterial && Value.itemData != null)
            {
                int iCostMaterial = FashionMergeManager.GetInstance().FashionMergeMaterialID;
                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iCostMaterial);
                int iCostCount = 1;
                comOption.itemCount.text = string.Format("{0}/{1}", iHasCount, iCostCount);
                if (iHasCount < iCostCount)
                {
                    comOption.itemCount.color = Color.red;
                }
                else
                {
                    comOption.itemCount.color = Color.white;
                }
                comItem.SetShowNotEnoughState(iHasCount < iCostCount);
                comOption.acquiredHint.CustomActive(iHasCount < iCostCount);
            }
            else
            {
                comItem.SetShowNotEnoughState(false);
                comOption.acquiredHint.CustomActive(false);
            }
        }
    }
}