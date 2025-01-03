using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class SpringFestivalDungeonItem : MonoBehaviour
    {

        [SerializeField]
        private GameObject mItemRoot;
        [SerializeField]
        private Text mNameTxt;

        public void Init(int id, Vector2 size,int i, ComItem.OnItemClicked callback)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);
            ComItem comItem = ComItemManager.Create(mItemRoot);
            if (itemData != null && comItem != null)
            {
                comItem.GetComponent<RectTransform>().sizeDelta = size;
                mNameTxt.SafeSetText(itemData.Name);
                if(i==0)
                {
                    comItem.Setup(itemData, callback);
                }
                else
                {
                    comItem.Setup(itemData, Utility.OnItemClicked);
                }
            }
        }
    }
}
