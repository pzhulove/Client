using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class HeadPortraitItem : MonoBehaviour
    {
        [SerializeField]private Image mHeadPortrait; 
        [SerializeField]private GameObject mGoSelectBack;
        [SerializeField]private GameObject mNotGet;
        [SerializeField]private GameObject mWear;
        [SerializeField]private GameObject mNewMark;

        private HeadPortraitItemData mHeadPortraitItem;

        public HeadPortraitItemData HeadPortraitItemData
        {
            get { return mHeadPortraitItem; }
            set { mHeadPortraitItem = value; }
        }
        public void OnItemVisiable(HeadPortraitItemData itemdata)
        {
            mHeadPortraitItem = itemdata;

            //头像框
            ETCImageLoader.LoadSprite(ref mHeadPortrait, mHeadPortraitItem.iconPath);

            mNotGet.CustomActive(!mHeadPortraitItem.isObtain);

            if (HeadPortraitFrameDataManager.WearHeadPortraitFrameID == 0)
            {
                //穿戴默认头像框
                mWear.CustomActive(mHeadPortraitItem.itemID == HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
            }
            else
            {
                //穿戴的头像框
                mWear.CustomActive(mHeadPortraitItem.itemID == HeadPortraitFrameDataManager.WearHeadPortraitFrameID);
            }
            
            mNewMark.CustomActive(mHeadPortraitItem.isNew);

            if (mHeadPortraitItem.isNew)
            {
                HeadPortraitFrameDataManager.GetInstance().NotifyItemBeOld(mHeadPortraitItem);
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            mGoSelectBack.CustomActive(bSelected);
        }

        private void OnDestroy()
        {
            mHeadPortraitItem = null;
        }
    }
}

