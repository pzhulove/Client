using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public delegate void OnMagicCardStrengthenLevelItemClick(int strengthenLevel);

    public class AuctionNewMagicCardStrengthenLevelItem : MonoBehaviour
    {

        //数据和回调
        private OnMagicCardStrengthenLevelItemClick _onItemButtonClick = null;
        protected AuctionNewMagicCardStrengthenLevelDataModel _dataModel = null;

        [SerializeField] private Text levelText;
        [SerializeField] private Text numberText;
        [SerializeField] private Image normalBackgroundImage;
        [SerializeField] private Image selectedBackgroundImage;
        [SerializeField] private Image disableImage;
        [SerializeField] private Button button;

        private void Awake()
        {
            _dataModel = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnItemButtonClick);
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        public virtual void InitItem(AuctionNewMagicCardStrengthenLevelDataModel itemData,
            OnMagicCardStrengthenLevelItemClick onItemClick = null)
        {
            _dataModel = itemData;
            if (_dataModel == null)
                return;

            _onItemButtonClick = onItemClick;

            InitView();
        }

        private void InitView()
        {
            UpdateMagicCardItem();
        }

        private void ResetItemImage()
        {
            UpdateItemImageVisible(disableImage, false);
            UpdateItemImageVisible(normalBackgroundImage, false);
            UpdateItemImageVisible(selectedBackgroundImage, false);
        }

        private void UpdateItemImageVisible(Image image, bool flag)
        {
            if (image != null)
                image.gameObject.CustomActive(flag);
        }

        public void UpdateMagicCardItem()
        {

            if (_dataModel == null)
                return;

            ResetItemImage();
            if (_dataModel.Number <= 0)
            {
                UpdateItemImageVisible(disableImage, true);
                UpdateItemImageVisible(normalBackgroundImage, false);
                UpdateItemImageVisible(selectedBackgroundImage, false);
                UpdateItemText(false);
            }
            else
            {
                UpdateItemImageVisible(disableImage, false);
                if (_dataModel.IsSelected)
                {
                    UpdateItemImageVisible(selectedBackgroundImage, true);
                    UpdateItemText(true, true);
                }
                else
                {
                    UpdateItemImageVisible(normalBackgroundImage, true);
                    UpdateItemText(true, false);
                }
            }
        }

        private void UpdateItemText(bool isHaveNumber, bool isSelected = false)
        {
            if (levelText != null)
            {
                //没有在售
                if (isHaveNumber == false)
                {
                    levelText.text = TR.Value("auction_new_magic_card_strengthen_level_less",
                        _dataModel.StrengthenLevel);
                }
                else
                {
                    //存在在售，选中和没有选中
                    if (isSelected == true)
                    {
                        levelText.text = TR.Value("auction_new_magic_card_strengthen_level_selected",
                            _dataModel.StrengthenLevel);
                    }
                    else
                    {
                        levelText.text = TR.Value("auction_new_magic_card_strengthen_level",
                            _dataModel.StrengthenLevel);
                    }
                }
            }

            if (numberText != null)
            {
                //没有在售
                if (isHaveNumber == false)
                {
                    numberText.text = TR.Value("auction_new_magic_card_number_less",
                        _dataModel.Number);
                }
                else
                {
                    //存在在售，选中和没有选中
                    if (isSelected == true)
                    {
                        numberText.text = TR.Value("auction_new_magic_card_number_selected",
                            _dataModel.Number);
                    }
                    else
                    {
                        numberText.text = TR.Value("auction_new_magic_card_number",
                            _dataModel.Number);
                    }
                }
            }
        }


        private void OnItemButtonClick()
        {

            if (_dataModel.Number <= 0)
                return;

            _dataModel.IsSelected = true;

            //执行回调，更新所有的Item选中状态
            if (_onItemButtonClick != null)
                _onItemButtonClick(_dataModel.StrengthenLevel);

            UpdateMagicCardItem();
        }

    }
}
