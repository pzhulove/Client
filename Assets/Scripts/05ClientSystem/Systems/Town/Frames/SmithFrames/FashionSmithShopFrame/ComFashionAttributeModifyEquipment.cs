using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class ComFashionAttributeModifyEquipment : MonoBehaviour
    {
        public static ItemData ms_selected = null;
        public Text Name;
        public Text checkName;
        public GameObject equiptedMark;
        public GameObject goItemParent;
        public GameObject goCheckMark;
        public Text redPointHint;
        public Image imageItemBack;
        ComItem comItem;
        public SmithFunctionRedBinder comFunctionBinder;
        FashionAttributesModifyFrame clientFrame;
        public ItemData ItemData
        {
            get
            {
                return comItem == null ? null : comItem.ItemData;
            }
        }

        public static bool CheckCanSeal(ItemData x)
        {
            return (x.iMaxPackTime > 0 && !x.Packing && x.RePackTime > 0);
        }

        public void OnCreate(ClientFrame frame)
        {
            clientFrame = frame as FashionAttributesModifyFrame;
            if (frame != null)
            {
                comItem = frame.CreateComItem(goItemParent);
                comItem.imgBackGround.enabled = false;
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
          Scrollbar scrollbar=gameObject.transform.parent.parent.parent.parent.transform.Find("EquipAdjust/Root/middleback/Scroll View/Scrollbar Vertical")
                .GetComponent<Scrollbar>();
            if (scrollbar!=null)
            {
                scrollbar.value = 1;
            }
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {
            if (item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        public void OnItemVisible(ItemData itemData)
        {
            comItem.Setup(itemData, OnItemClicked);
            //imageItemBack.sprite = AssetLoader.instance.LoadRes(itemData.GetQualityInfo().TitleBG, typeof(Sprite)).obj as Sprite;
            Name.text = itemData.GetColorName();
            checkName.text = itemData.GetColorName();
            equiptedMark.CustomActive(itemData.PackageType == EPackageType.WearFashion);
            gameObject.name = itemData.TableID.ToString();

            //if (eFunctionType == SmithShopFrame.FunctionType.FT_ADJUST && 
            //    eAdjustFunctionType == SmithShopFrame.AdjustFunctionType.ASF_CHANGE_QUALITY &&
            //    itemData.PackageType == EPackageType.WearEquip)
            //{
            //    comFunctionBinder.ClearCheckFunctions();
            //    comFunctionBinder.SpecialItem = itemData;
            //    comFunctionBinder.AddCheckFunction(SmithFunctionRedBinder.SmithFunctionType.SFT_ADJUST);
            //    redPointHint.text = TR.Value("SMITH_CAN_ADJUST_QUALITY");
            //}
            //else
            //{
            //    comFunctionBinder.SpecialItem = null;
            //    comFunctionBinder.ClearCheckFunctions();
            //}
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