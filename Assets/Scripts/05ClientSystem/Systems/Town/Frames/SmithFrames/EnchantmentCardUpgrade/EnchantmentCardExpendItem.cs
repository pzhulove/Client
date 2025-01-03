using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EnchantmentCardExpendItem : MonoBehaviour
    {
        [SerializeField] private Image itemBackground;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Button itemIconBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private Text itemLevel;
        [SerializeField]private string sLevelDesc = "+{0}";

        private ItemData itemData;

        private void Awake()
        {
            if(closeBtn != null)
            {
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(OnCloseBtnClick);
            }

            if(itemIconBtn != null)
            {
                itemIconBtn.onClick.RemoveAllListeners();
                itemIconBtn.onClick.AddListener(OnShowItemTip);
            }
        }

        private void OnDestroy()
        {
            if (closeBtn != null)
            {
                closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            }

            if (itemIconBtn != null)
            {
                itemIconBtn.onClick.RemoveListener(OnShowItemTip);
            }
        }

        public void SetItem(ItemData itemData)
        {
            this.itemData = itemData;
            if (itemData == null)
            {
                SetBackGroudState(false);
                SetCloseBtnState(false);
                SetIconState(false);
                SetItemLevelState(false);
            }
            else
            {
                SetBackGroudState(true);
                SetCloseBtnState(true);
                SetIconState(true);
                SetItemLevelState(true);

                if (itemBackground != null)
                    ETCImageLoader.LoadSprite(ref itemBackground, itemData.GetQualityInfo().Background);

                if (itemIcon != null)
                    ETCImageLoader.LoadSprite(ref itemIcon, itemData.Icon);

                if (itemLevel != null)
                {
                    if (itemData.mPrecEnchantmentCard.iEnchantmentCardLevel > 0)
                    {
                        itemLevel.text = string.Format(sLevelDesc, itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                    }
                    else
                    {
                        itemLevel.text = string.Empty;
                    }
                }
            }
        }

        private void SetCloseBtnState(bool isFlag)
        {
            if (closeBtn != null)
                closeBtn.CustomActive(isFlag);
        }

        private void SetIconState(bool isFlag)
        {
            if (itemIcon != null)
                itemIcon.CustomActive(isFlag);
        }
        
        private void SetBackGroudState(bool isFlag)
        {
            if (itemBackground != null)
                itemBackground.CustomActive(isFlag);
        }

        private void SetItemLevelState(bool isFlag)
        {
            if (itemLevel != null)
                itemLevel.CustomActive(isFlag);
        }

        private void OnCloseBtnClick()
        {
            if(itemData != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRemoveExpendEnchantmentCard, itemData.TableID);
            }
        }

        private void OnShowItemTip()
        {
            if(itemData != null)
            {
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }
    }
}