using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class AdventurerPassCardPreviewItem : MonoBehaviour
    {
        [SerializeField] private AdventurerPassCardComItem mItem;
        [SerializeField] private string mCangetImgPath = "UIFlatten/Prefabs/AdventurerPassCard/ImgCanGet";
        [SerializeField] private string mLockImgPath = "UIFlatten/Prefabs/AdventurerPassCard/ImgLock";
        private GameObject mObjCanGet;
        private GameObject mObjLock;
        public void OnInit(ItemData model, bool canGet, bool isLock)
        {
            mItem.OnInit(model);

            if (isLock)
            {
                if (null == mObjLock)
                {
                    mObjLock = AssetLoader.instance.LoadResAsGameObject(mLockImgPath);
                    Utility.AttachTo(mObjLock, this.gameObject);
                }
                mObjCanGet.CustomActive(false);
                mObjLock.CustomActive(true);
            }
            else if (canGet)
            {
                if (null == mObjCanGet)
                {
                    mObjCanGet = AssetLoader.instance.LoadResAsGameObject(mCangetImgPath);
                    Utility.AttachTo(mObjCanGet, this.gameObject);
                }
                mObjLock.CustomActive(false);
                mObjCanGet.CustomActive(true);
            }
            else
            {
                mObjLock.CustomActive(false);
                mObjCanGet.CustomActive(false);
            }
        }
    }
}
