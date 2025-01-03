using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class ComFashionMergeItemNew : MonoBehaviour
    {
        public static ItemData ms_selected_left = null;
        public static ItemData ms_selected_right = null;
        public GameObject goItemParent;
        public Text Name;
        public Text Atrribute;
        public GameObject goCheckMark;
        public Image imageItemBack;
        ComItem comItem;
        public ItemData ItemData
        {
            get
            {
                return comItem == null ? null : comItem.ItemData;
            }
        }
        FashionMergeNewFrame clientFrame;
        FashionMergeType eFashionMergeType = FashionMergeType.MT_COUNT;

        public void OnCreate(ClientFrame frame)
        {
            clientFrame = frame as FashionMergeNewFrame;
            if (frame != null)
            {
                comItem = frame.CreateComItem(goItemParent);
                comItem.imgBackGround.enabled = false;
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(false);
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {
            if (item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        public static FashionMergeType GetFashionMergeType(ItemData itemData)
        {
            var eFashionMergeType = FashionMergeType.MT_COUNT;
            if(itemData != null)
            {
                switch (itemData.FashionWearSlotType)
                {
                    case EFashionWearSlotType.Head:
                        {
                            eFashionMergeType = FashionMergeType.FMT_HEAD;
                            break;
                        }
                    case EFashionWearSlotType.Chest:
                        {
                            eFashionMergeType = FashionMergeType.FMT_CHEST;
                            break;
                        }
                    case EFashionWearSlotType.UpperBody:
                        {
                            eFashionMergeType = FashionMergeType.FMT_UPPER_BODY;
                            break;
                        }
                    case EFashionWearSlotType.LowerBody:
                        {
                            eFashionMergeType = FashionMergeType.FMT_LOWER_BODY;
                            break;
                        }
                    case EFashionWearSlotType.Waist:
                        {
                            eFashionMergeType = FashionMergeType.FMT_WAIST;
                            break;
                        }
                    default:
                        {
                            eFashionMergeType = FashionMergeType.MT_COUNT;
                            break;
                        }
                }
            }
            return eFashionMergeType;
        }

        public void OnItemVisible(ItemData itemData)
        {
            this.eFashionMergeType = GetFashionMergeType(itemData);
            Name.text = itemData.GetColorName();
            Atrribute.text = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(itemData.FashionAttributeID);
            //imageItemBack.sprite = AssetLoader.instance.LoadRes(itemData.GetQualityInfo().TitleBG, typeof(Sprite)).obj as Sprite;
            comItem.Setup(itemData, OnItemClicked);
            gameObject.name = itemData.TableID.ToString();
        }

        void OnDestroy()
        {
            if(comItem != null)
            {
                comItem.imgBackGround.enabled = true;
                comItem = null;
            }
            clientFrame = null;
        }
    }
}