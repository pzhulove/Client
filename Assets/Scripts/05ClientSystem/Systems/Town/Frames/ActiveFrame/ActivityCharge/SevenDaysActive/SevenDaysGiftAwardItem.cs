using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysGiftAwardItem : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mCanvasNormal = null;
        [SerializeField] private CanvasGroup mCanvasSelect = null;
        [SerializeField] private ComItemNew mAward = null;
        [SerializeField] private TextEx mTextName = null;

        public void Init(ItemData itemData, bool isGiftPack, bool isSelect)
        {
            mCanvasNormal.CustomActive(!isSelect && isGiftPack);
            mCanvasSelect.CustomActive(isSelect && isGiftPack);
            if (mAward != null && itemData != null)
            {
                mAward.Setup(itemData, null, true);
                mAward.SetCount(itemData.Count.ToString());
                mTextName.SafeSetText(itemData.Name);
                mTextName.SafeSetColor(GameUtility.Item.GetItemColor(itemData.Quality));
            }
        }
    }
}