using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class HeadPortraitInfoMationView : MonoBehaviour
    {
        [SerializeField]private Image mHeadPortrait;
        [SerializeField]private Text mName;
        [SerializeField]private Text mGetConditions;
        [SerializeField]private Text mUsePeriod;
        [SerializeField]private Button mWearBtn;
        [SerializeField]private GameObject mDressedInGo;
        [SerializeField]private GameObject mGameObjectRoot;
        [SerializeField]private string mDesc="未获得";
        [SerializeField]private string mPermanent="永久";

        private void Awake()
        {
            if (mWearBtn != null)
            {
                mWearBtn.onClick.RemoveAllListeners();
                mWearBtn.onClick.AddListener(OnWearBtnClick);
            }
        }

        private HeadPortraitItemData mCurrentData;

        public void SetGameobjectRoot(bool isFlag)
        {
            if (mGameObjectRoot != null)
            {
                mGameObjectRoot.CustomActive(isFlag);
            }
        }

        public void RefreshHeadPortraitInfoMation(HeadPortraitItemData data)
        {
            if (data == null)
            {
                return;
            }

            mCurrentData = data;

            //头像框
            ETCImageLoader.LoadSprite(ref mHeadPortrait, mCurrentData.iconPath);

            //头像名称
            mName.text = mCurrentData.Name;

            //获得条件
            mGetConditions.text = mCurrentData.conditions;
            
            if (mCurrentData.isObtain == true)
            {
                if (mCurrentData.expireTime > 0)
                {
                    mUsePeriod.text = Function.SetShowTimeDay(mCurrentData.expireTime);
                }
                else
                {
                    mUsePeriod.text = mPermanent;
                }
            }
            else
            {
                mUsePeriod.text = mDesc;
            }

            mWearBtn.CustomActive(mCurrentData.isObtain && (mCurrentData.itemID != HeadPortraitFrameDataManager.WearHeadPortraitFrameID));

            mDressedInGo.CustomActive(mCurrentData.itemID == HeadPortraitFrameDataManager.WearHeadPortraitFrameID);
        }

        private void OnWearBtnClick()
        {
            //操作数据为空，直接返回
            if (mCurrentData == null)
            {
                return;
            }

            //如果穿戴默认头像框，向服务器请求ID传0
            if (mCurrentData.itemID == HeadPortraitFrameDataManager.iDefaultHeadPortraitID)
            {
                HeadPortraitFrameDataManager.GetInstance().OnSendSceneHeadFrameUseReq(0);
                return;
            }
            HeadPortraitFrameDataManager.GetInstance().OnSendSceneHeadFrameUseReq((uint)mCurrentData.itemID);
        }

        private void OnDestroy()
        {
            mCurrentData = null;

            if (mWearBtn != null)
            {
                mWearBtn.onClick.RemoveAllListeners();
            }
        }
    }
}
