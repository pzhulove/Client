using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace GameClient
{
    public class BeadHoleItem : MonoBehaviour
    {
        [SerializeField] private GameObject mCanBeSetRoot;
        [SerializeField] private GameObject mHasBeenSetRoot;
        [SerializeField] private GameObject mBeadCardParent;
        [SerializeField] private Text mBeadCardAttr;

        private ComItemNew beadCardComItem;
        public void OnItemVisiable(PrecBead precBead)
        {
            if(precBead == null)
            {
                return;
            }

            mCanBeSetRoot.CustomActive(false);
            mHasBeenSetRoot.CustomActive(false);

            ItemData beadItemData = ItemDataManager.CreateItemDataFromTable(precBead.preciousBeadId);
            if (beadItemData != null)
            {
                if (beadCardComItem == null)
                {
                    beadCardComItem = ComItemManager.CreateNew(mBeadCardParent);
                }

                beadCardComItem.Setup(beadItemData, Utility.OnItemClicked);

                string attribute = BeadCardManager.GetInstance().GetAttributesDesc(precBead.preciousBeadId);
                int Count = Regex.Matches(attribute, "\n").Count;

                if (Count > 2)
                {
                    mBeadCardAttr.alignment = TextAnchor.UpperLeft;
                }
                else
                {
                    mBeadCardAttr.alignment = TextAnchor.MiddleLeft;
                }

                if (mBeadCardAttr != null)
                {
                    mBeadCardAttr.text = BeadCardManager.GetInstance().GetAttributesDesc(precBead.preciousBeadId);
                    if (precBead.randomBuffId > 0)
                    {
                        mBeadCardAttr.text += string.Format("\n附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(precBead.randomBuffId));
                    }
                }

                mHasBeenSetRoot.CustomActive(true);
            }
            else
            {
                mCanBeSetRoot.CustomActive(true);
            }
        }
    }
}

