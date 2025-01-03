using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace GameClient
{
    class ComBeadEquipment : MonoBehaviour
    {
        public static ItemData ms_selected = null;
        public Text Name;
        public GameObject equiptedMark;
        public GameObject goItemParent;
        public GameObject goCheckMark;
        public UIGray mGrayRoot;
        public GameObject goHoleParent;
        public string sBeadHoleItemPath = "UIFlatten/Prefabs/SmithShop/FunctionPrefab/BeadHoleItem";

        List<GameObject> beadHoleItemList = new List<GameObject>();
        ComItemNew comItem;
        private ItemData itemData;
        public ItemData ItemData
        {
            get
            {
                return itemData;
            }
        }
        
        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
        }
        
        public void OnItemVisible(ItemData itemData)
        {
            this.itemData = itemData;
            Name.text = itemData.GetColorName();

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(goItemParent);
            }
            comItem.Setup(itemData, Utility.OnItemClicked);

            for (int i = 0; i < beadHoleItemList.Count; i++)
            {
                beadHoleItemList[i].CustomActive(false);
            }

            if (itemData != null)
            {
                for (int i = 0; i < itemData.PreciousBeadMountHole.Length; i++)
                {
                    var mBeadHole = itemData.PreciousBeadMountHole[i];
                    if (mBeadHole == null)
                    {
                        continue;
                    }

                    if (i < beadHoleItemList.Count)
                    {
                        GameObject gameObject = beadHoleItemList[i];
                        gameObject.CustomActive(true);
                        BeadHoleItem beadHoleItem = gameObject.GetComponent<BeadHoleItem>();
                        if (beadHoleItem != null)
                        {
                            beadHoleItem.OnItemVisiable(mBeadHole);
                        }
                    }
                    else
                    {
                        GameObject gameObject = AssetLoader.instance.LoadResAsGameObject(sBeadHoleItemPath);
                        BeadHoleItem beadHoleItem = gameObject.GetComponent<BeadHoleItem>();
                        if (beadHoleItem != null)
                        {
                            beadHoleItem.OnItemVisiable(mBeadHole);
                        }

                        Utility.AttachTo(gameObject, goHoleParent);

                        beadHoleItemList.Add(gameObject);
                    }
                }
            }
            
            equiptedMark.CustomActive(itemData.PackageType == EPackageType.WearEquip);
            gameObject.name = itemData.TableID.ToString();
            
            if (itemData.Type == ItemTable.eType.FUCKTITTLE)
            {
                bool isFlag = (itemData.Packing || itemData.iMaxPackTime > 0);
                mGrayRoot.enabled = isFlag;
            }
            else
            {
                mGrayRoot.enabled = false;
            }
        }

        void OnDestroy()
        {
            if(comItem != null)
            {
                comItem = null;
            }

            beadHoleItemList.Clear();
        }
    }
}