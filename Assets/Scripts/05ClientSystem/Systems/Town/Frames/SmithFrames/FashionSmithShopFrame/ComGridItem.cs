using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    class ComGridItem : MonoBehaviour
    {
        public GameObject goItemParent;
        public GameObject goBack;
        public GameObject goCheck;
        public StateController comState;
        public Text itemName;
        static string ms_enable = "Enable";
        static string ms_disable = "Disable";

        ComItem comItem;
        public ItemData Value
        {
            get
            {
                return null == comItem ? null : comItem.ItemData;
            }
        }

        ComUIListElementSelectionScript comSelect = null;

        public void OnItemVisible(ItemData itemData)
        {
            if(null == comItem)
            {
                comItem = ComItemManager.Create(goItemParent);
            }

            if(null != comItem)
            {
                comItem.Setup(itemData,null);
            }

            if(null == comSelect)
            {
                comSelect = GetComponent<ComUIListElementSelectionScript>();
            }

            if(null != comSelect)
            {
                comSelect.enabled = null != itemData;
            }

            if(null != itemName)
            {
                if(null != itemData)
                {
                    itemName.text = itemData.GetColorName();
                }
                else
                {
                    itemName.text = string.Empty;
                }
            }

            goBack.CustomActive(null == itemData);
            comItem.CustomActive(null != itemData);
        }

        void _OnItemClicked(GameObject go,ItemData item)
        {
            if(null != item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            if (null != comItem)
            {
                comItem.SetEnable(!bSelected);
            }
            goCheck.CustomActive(bSelected);
            if (null != comState)
            {
                comState.Key = bSelected ? ms_enable : ms_disable;
            }
        }
    }
}