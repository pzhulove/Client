using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace GameClient
{
    public class InscriptionExtractItem : MonoBehaviour
    {
        [SerializeField] private Image mBackGround;
        [SerializeField] private Image mIcon;
        [SerializeField] private Toggle mItemTog;
        [SerializeField] private Text mName;

        private InscriptionExtractItemData mInscriptionExtractData;
        private UnityAction<InscriptionExtractItemData> mOnClick;
        private void Awake()
        {
            if (mItemTog != null)
            {
                mItemTog.onValueChanged.RemoveAllListeners();
                mItemTog.onValueChanged.AddListener(OnInscriptionItemClick);
            }
        }

        private void OnDestroy()
        {
            mInscriptionExtractData = null;
            mOnClick = null;
        }

        public void OnItemVisiable(InscriptionExtractItemData inscriptionExtractData,UnityAction<InscriptionExtractItemData> onClick,bool isSelected = false,bool togIsOpen = false)
        {
            mInscriptionExtractData = inscriptionExtractData;
            mOnClick = onClick;

            if (mInscriptionExtractData == null)
            {
                return;
            }

            if (mBackGround != null)
            {
                mBackGround.color = mInscriptionExtractData.inscriptionItem.GetQualityInfo().Col;
            }

            if (mIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mIcon, mInscriptionExtractData.inscriptionItem.Icon);
            }

            if (mName != null)
            {
                mName.text = mInscriptionExtractData.inscriptionItem.GetColorName();
            }

            if (togIsOpen == false)
            {
                mItemTog.enabled = false;
            }

            if (isSelected == true)
            {
                if (mItemTog != null)
                {
                    mItemTog.isOn = true;
                }
            }
        }

        public void SetTogState()
        {
            if (mItemTog != null)
                mItemTog.enabled = false;
        }

        private void OnInscriptionItemClick(bool value)
        {
            if (value == true)
            {
                if (mOnClick != null)
                {
                    mOnClick(mInscriptionExtractData);
                }
            }
        }
    }
}