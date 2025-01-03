using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemPreviewItem : MonoBehaviour
    {

        private PreviewLevelItemDataModel _previewLevelItemDataModel;
        private List<int> _unlockShopItemList;

        [Space(10)] [HeaderAttribute("Title")] [Space(10)]
        [SerializeField] private GameObject specialFlag;
        [SerializeField] private Text levelNameLabel;

        [Space(10)]
        [HeaderAttribute("LevelIcon")]
        [Space(10)]
        [SerializeField] private Image leftLevelFlagImage;
        [SerializeField] private Image rightLevelFlagImage;
        [SerializeField] private Image honorImg;

        [Space(10)] [HeaderAttribute("Content")] [Space(10)]
        [SerializeField] private Text levelValueLabel;
        [SerializeField] private Text needExpValueLabel;
        [SerializeField] private Text titleRewardLabel;
        [SerializeField] private Text shopDiscountValueLabel;

        [Space(10)] [HeaderAttribute("UnlockShopItemList")] [Space(10)]
        [SerializeField] private ComUIListScriptEx unLockShopItemList;


        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void ClearData()
        {
            _previewLevelItemDataModel = null;
            _unlockShopItemList = null;
        }

        #region UnLockShopItemList
        private void BindEvents()
        {
            if (unLockShopItemList != null)
            {
                unLockShopItemList.Initialize();
                unLockShopItemList.onItemVisiable += OnUnLockShopItemVisible;
                unLockShopItemList.OnItemRecycle += OnUnLockShopItemRecycle;
            }
        }

        private void UnBindEvents()
        {
            if (unLockShopItemList != null)
            {
                unLockShopItemList.onItemVisiable -= OnUnLockShopItemVisible;
                unLockShopItemList.OnItemRecycle -= OnUnLockShopItemRecycle;
            }
        }

        private void OnUnLockShopItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_unlockShopItemList == null || _unlockShopItemList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _unlockShopItemList.Count)
                return;

            var unLockShopItem = item.GetComponent<HonorSystemUnLockShopItem>();
            var unLockShopItemId = _unlockShopItemList[item.m_index];
            if (unLockShopItem != null)
                unLockShopItem.InitUnLockShopItem(unLockShopItemId);

        }

        private void OnUnLockShopItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var unLockShopItem = item.GetComponent<HonorSystemUnLockShopItem>();
            if(unLockShopItem != null)
                unLockShopItem.OnRecycleItem();
        }
        #endregion


        public void InitPreviewItem(PreviewLevelItemDataModel previewLevelItemDataModel)
        {
            _previewLevelItemDataModel = previewLevelItemDataModel;
            if (_previewLevelItemDataModel == null)
            {
                Logger.LogErrorFormat("PreviewLevelItemDataModel is null");
                return;
            }

            _unlockShopItemList = _previewLevelItemDataModel.UnLockShopItemList;

            InitBaseView();
            //InitUnLockShopItemList();
        }

        private void InitBaseView()
        {
            //"当前" Flag
            if (specialFlag != null)
            {
                if (_previewLevelItemDataModel.HonorSystemLevel ==
                    (int) HonorSystemDataManager.GetInstance().PlayerHonorLevel)
                {
                    //当前等级
                    CommonUtility.UpdateGameObjectVisible(specialFlag, true);
                }
                else
                {
                    CommonUtility.UpdateGameObjectVisible(specialFlag, false);
                }
            }

            //避免重复计算
            var honorSystemTitleRewardStr =
                HonorSystemUtility.GetTitleNameByTitleId(_previewLevelItemDataModel.TitleId);

            //荣誉头衔的奖励
            if (levelNameLabel != null)
            {
                levelNameLabel.text = honorSystemTitleRewardStr;
            }

            //等级
            if (levelValueLabel != null)
            {
                var levelStr = TR.Value("Honor_System_Current_Level_Format",
                    _previewLevelItemDataModel.HonorSystemLevel);
                levelValueLabel.text = levelStr;
            }

            //等级的Icon
            if (leftLevelFlagImage != null)
                ETCImageLoader.LoadSprite(ref leftLevelFlagImage, _previewLevelItemDataModel.HonorLevelFlagPath);
            if (rightLevelFlagImage != null)
                ETCImageLoader.LoadSprite(ref rightLevelFlagImage, _previewLevelItemDataModel.HonorLevelFlagPath);

            ////升级经验
            //if (needExpValueLabel != null)
            //{
            //    needExpValueLabel.text = _previewLevelItemDataModel.HonorSystemNeedExpValue.ToString();
            //}

            //头衔奖励
            if (titleRewardLabel != null)
            {
                titleRewardLabel.text = honorSystemTitleRewardStr;
            }

            if (honorImg != null)
            {
                ETCImageLoader.LoadSprite(ref honorImg, HonorSystemUtility.GetTitleIconPathByTitleId(_previewLevelItemDataModel.TitleId));
            }

            //if (shopDiscountValueLabel != null)
            //{
            //    if (_previewLevelItemDataModel.ShopDiscountList == null
            //        || _previewLevelItemDataModel.ShopDiscountList.Count <= 0)
            //    {
            //        shopDiscountValueLabel.text = TR.Value("Honor_System_Title_Reward_No");
            //    }
            //    else
            //    {
            //        var shopDiscountStr = HonorSystemUtility.GetShopDiscountValue(_previewLevelItemDataModel
            //            .ShopDiscountList);
            //        shopDiscountValueLabel.text = shopDiscountStr;
            //    }
            //}
        }

        private void InitUnLockShopItemList()
        {
            if (unLockShopItemList == null)
                return;

            //重置
            unLockShopItemList.ResetComUiListScriptEx();

            var count = 0;
            if (_unlockShopItemList != null)
                count = _unlockShopItemList.Count;

            unLockShopItemList.SetElementAmount(count);
        }

        public void OnRecycleItem()
        {
            ClearData();
        }


    }
}