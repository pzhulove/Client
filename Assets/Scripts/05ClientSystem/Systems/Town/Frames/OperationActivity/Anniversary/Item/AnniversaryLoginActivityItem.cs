using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;

namespace GameClient
{
    public class AnniversaryLoginActivityItem : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private Transform mIconParent;
        [SerializeField]
        private Text mNameTxt;
        [SerializeField]
        private Text mHaveRewardTxt;

        [SerializeField]
        private GameObject mTakenGo;
       

        [SerializeField]
        private Vector2 mComItemSize = new Vector2(90, 90);

    

        public void Init(bool isShow, GiftSyncInfo giftSyncInfo)
        {
            transform.localScale = Vector3.one;
            mTakenGo.CustomActive(true);
            mHaveRewardTxt.CustomActive(isShow);
            ComItem comItem = ComItemManager.Create(mIconParent.gameObject);
            ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)giftSyncInfo.itemId);
            if(comItem!=null&&itemData!=null)
            {
                itemData.Count = (int)giftSyncInfo.itemNum;
                mNameTxt.SafeSetText(itemData.Name);
                comItem.GetComponent<RectTransform>().sizeDelta = mComItemSize;
                comItem.Setup(itemData, Utility.OnItemClicked);
            }
          
        }

     
        public void ShowHaveReceivedState()
        {
            mHaveRewardTxt.CustomActive(true);
        }

        public void Dispose()
        {

        }
    }
}
  

