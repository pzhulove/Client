using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipUpgradeResultFrame : ClientFrame
    {
        #region ExtraUIBind
        private GameObject mItemParent = null;
        private Text mItemname = null;
        private Button mClose = null;
        private Text mArrt = null;
        private GameObject mHint = null;
        private GameObject mInscriptionFail = null;
        private GameObject mImageEx9 = null;
        private GameObject mImageEx10 = null;
        private Image mTitleLeft = null;
        private Image mTitleRight = null;

        protected sealed override void _bindExUI()
        {
            mItemParent = mBind.GetGameObject("ItemParent");
            mItemname = mBind.GetCom<Text>("itemname");
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
            mArrt = mBind.GetCom<Text>("Arrt");
            mHint = mBind.GetGameObject("Hint");
            mInscriptionFail = mBind.GetGameObject("InscriptionFail");
            mImageEx9 = mBind.GetGameObject("ImageEx9");
            mImageEx10 = mBind.GetGameObject("ImageEx10");
            mTitleLeft = mBind.GetCom<Image>("TitleLeft");
            mTitleRight = mBind.GetCom<Image>("TitleRight");
        }

        protected sealed override void _unbindExUI()
        {
            mItemParent = null;
            mItemname = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
            mArrt = null;
            mHint = null;
            mInscriptionFail = null;
            mImageEx9 = null;
            mImageEx10 = null;
            mTitleLeft = null;
            mTitleRight = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/EquipUpgrade/EquipUpgradeResultFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData is ItemData)
            {
                ItemData itemData = userData as ItemData;
                if (itemData != null)
                {
                    ComItemNew item = ComItemManager.CreateNew(mItemParent);

                    item.Setup(itemData, Utility.OnItemClicked);

                    mItemname.text = itemData.GetColorName();

                    _HideGameObject();
                }
            }
           else if (userData is EnchantmentCardUpgradeSuccessData)
           {
                EnchantmentCardUpgradeSuccessData mData = userData as EnchantmentCardUpgradeSuccessData;
                if (mData != null)
                {
                    ItemData mItemData = ItemDataManager.CreateItemDataFromTable(mData.mEnchantmentCardID);
                    ComItemNew item = ComItemManager.CreateNew(mItemParent);

                    item.Setup(mItemData, Utility.OnItemClicked);

                    if (mData.mEnchantmentCardLevel > 0)
                    {
                        mItemname.text = string.Format("{0}+{1}", mItemData.GetColorName(), mData.mEnchantmentCardLevel);
                    }
                    else
                    {
                        mItemname.text = string.Format("{0}", mItemData.GetColorName());
                    }

                    if (mData.isSuccess)
                    {
                        mArrt.text = string.Format("<color={0}>附魔卡属性:</color>", "#0FCF6Aff");
                        mArrt.text += EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(mData.mEnchantmentCardID, mData.mEnchantmentCardLevel);
                    }
                    else
                    {
                        ETCImageLoader.LoadSprite(ref mTitleLeft, TR.Value("Item_upgrade_faild_shengji"));
                        ETCImageLoader.LoadSprite(ref mTitleRight, TR.Value("Item_upgrade_faild_shibai"));

                        _HideGameObject();
                    }
                }
           }
            else if (userData is InscriptionMosaicSuccessData)
            {
                InscriptionMosaicSuccessData data = userData as InscriptionMosaicSuccessData;
                if (data != null)
                {
                    AudioManager.instance.PlaySound(12);

                    ETCImageLoader.LoadSprite(ref mTitleLeft, TR.Value("Item_Mosaic_success_xiangqian"));

                    ItemData mItemData = ItemDataManager.GetInstance().GetItem(data.guid);
                    if (mItemData != null)
                    {
                        ComItemNew item = ComItemManager.CreateNew(mItemParent);

                        item.Setup(mItemData, Utility.OnItemClicked);

                        mItemname.text = mItemData.GetColorName();
                    }

                    mArrt.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(data.inscriptionId);
                }
            }
            else if ( userData is InscriptionExtractResultData)
            {
                InscriptionExtractResultData data = userData as InscriptionExtractResultData;
                if (data != null)
                {
                    if (data.IsSuccessed == true)
                    {
                        ETCImageLoader.LoadSprite(ref mTitleLeft, TR.Value("Item_pick_successed_zhaiqu"));

                        ItemData mItemData = data.InscriptionItemData;
                        if (mItemData != null)
                        {
                            ComItemNew item = ComItemManager.CreateNew(mItemParent);

                            item.Setup(mItemData, Utility.OnItemClicked);

                            mItemname.text = mItemData.GetColorName();

                            mArrt.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(mItemData.TableID);
                        }
                    }
                    else
                    {
                        ETCImageLoader.LoadSprite(ref mTitleLeft, TR.Value("Item_pick_faild_zhaiqu"));
                        ETCImageLoader.LoadSprite(ref mTitleRight, TR.Value("Item_pick_faild_shibai"));

                        mInscriptionFail.CustomActive(true);

                        _HideGameObject();
                    }
                }
            }
            else if (userData is InscriptionFracturnResultData)
            {
                InscriptionFracturnResultData mData = userData as InscriptionFracturnResultData;
                if (mData != null)
                {
                    if (mData.IsSuccessed == true)
                    {
                        ETCImageLoader.LoadSprite(ref mTitleLeft, TR.Value("Item_fracture_successed_suilie"));

                        mInscriptionFail.CustomActive(true);

                        _HideGameObject();
                    }
                }
            }
            else if (userData is EpicEquipmentTransformationSuccessData)
            {
                EpicEquipmentTransformationSuccessData mData = userData as EpicEquipmentTransformationSuccessData;
                if (mData != null && mData.itemData != null)
                {
                    ETCImageLoader.LoadSprite(ref mTitleLeft, TR.Value("Item_changed_successed_zhuanhua"));
                   
                    ComItemNew item = ComItemManager.CreateNew(mItemParent);

                    item.Setup(mData.itemData, Utility.OnItemClicked);

                    mItemname.text = mData.itemData.GetColorName();

                    _HideGameObject();
                }
            }
        }

        private void _HideGameObject()
        {
            mImageEx9.CustomActive(false);
            mImageEx10.CustomActive(false);
            mHint.CustomActive(false);
        }

        protected sealed override void _OnCloseFrame()
        {
            
        }
    }
}

