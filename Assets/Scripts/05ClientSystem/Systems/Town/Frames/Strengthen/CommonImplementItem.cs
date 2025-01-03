using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonImplementItem : MonoBehaviour
    {
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemCount;
        [SerializeField] private GameObject itemParent;
        [SerializeField] private GameObject checkMarkGo;
        [Header("表示是否初始化数量")]
        [SerializeField] private bool bIsInitItemCountInfo = false;
        [Header("消耗数量")]
        [SerializeField] private int expendCount = 1;

        private ItemData itemData;
        public ItemData ItemData
        {
            get { return itemData; }
        }

        public void OnItemVisiable(ulong guid)
        {
            itemData = ItemDataManager.GetInstance().GetItem(guid);
            if(itemData != null)
            {
                if (itemName != null)
                    itemName.text = itemData.GetColorName();

                CommonNewItem commonNewItem = CommonUtility.CreateCommonNewItem(itemParent);
               
                if (bIsInitItemCountInfo)
                {
                    if (commonNewItem != null)
                        commonNewItem.InitItem(itemData.TableID);

                    if (itemCount != null)
                    {
                        itemCount.text = string.Format("{0}/{1}", itemData.Count, expendCount);

                        itemCount.color = itemData.Count >= expendCount ? Color.white : Color.red;
                    }
                }
                else
                {
                    if (commonNewItem != null)
                        commonNewItem.InitItem(itemData.TableID, itemData.Count);
                }
            }

            OnChangeDisplay(false);
        }

        public void OnChangeDisplay(bool bSelected)
        {
            if (checkMarkGo != null)
                checkMarkGo.CustomActive(bSelected);
        }

        public void OnDestroy()
        {
            itemData = null;
        }
    }
}

