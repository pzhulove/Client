using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ShowAwardItem : MonoBehaviour
    {
        [SerializeField] private ComItemNew mItem;
        [SerializeField] private Text mTextName;

        public void OnInit(ItemData itemData)
        {
            mItem.Setup(itemData, null, true);
            mTextName.SafeSetText(itemData.Name);
            mTextName.color = GameUtility.Item.GetItemColor(itemData.Quality);
        }
    }
}
