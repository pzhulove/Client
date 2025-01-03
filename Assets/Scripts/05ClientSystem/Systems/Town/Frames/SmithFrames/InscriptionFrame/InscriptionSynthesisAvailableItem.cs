using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class InscriptionSynthesisAvailableItem : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mInscriptionName;
        [SerializeField] private Text mProbability;

        private ComItem mComItem;
        private void OnDestroy()
        {
            mComItem = null;
        }
        public void OnItemVisiable(CanBeObtainedInscriptionItemData data)
        {
            if (data == null)
            {
                return;
            }

            if (data.inscriptionItemData == null)
            {
                return;
            }

            if (mComItem == null)
            {
                mComItem = ComItemManager.Create(mItemParent);
            }

            mComItem.Setup(data.inscriptionItemData, Utility.OnItemClicked);

            mInscriptionName.text = data.inscriptionItemData.GetColorName();

            mProbability.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionExtractSuccessRateDesc(data.probability);
        }
    }
}