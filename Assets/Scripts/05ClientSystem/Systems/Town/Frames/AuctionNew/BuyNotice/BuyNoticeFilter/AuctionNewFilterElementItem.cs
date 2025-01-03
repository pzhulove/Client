using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class AuctionNewFilterElementItem : MonoBehaviour
    {

        private OnAuctionNewFilterElementItemButtonClick _onButtonClick = null;

        private AuctionNewFilterData _auctionNewFilterData = null;

        [SerializeField] private Text nameText;
        [SerializeField] private Image selectedFlag;
        [SerializeField] private Text selectedNameText;
        [SerializeField] private Button button;

        private void Awake()
        {
            _auctionNewFilterData = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
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

        public void InitData(AuctionNewFilterData auctionNewFilterData,
            OnAuctionNewFilterElementItemButtonClick onButtonClick = null)
        {
            _auctionNewFilterData = auctionNewFilterData;
            if (_auctionNewFilterData == null)
                return;

            if (nameText != null)
            {
                nameText.text = _auctionNewFilterData.Name;
            }

            if (selectedNameText != null)
                selectedNameText.text = _auctionNewFilterData.Name;

            _onButtonClick = onButtonClick;

            UpdateFilterElementItem();
        }

        private void OnButtonClick()
        {

            _auctionNewFilterData.IsSelected = true;

            if (_onButtonClick != null)
                _onButtonClick(_auctionNewFilterData);

            UpdateFilterElementItem();
        }

        public void UpdateFilterElementItem()
        {
            if (_auctionNewFilterData == null)
                return;

            CommonUtility.UpdateImageVisible(selectedFlag, _auctionNewFilterData.IsSelected);

            //默认字体
            CommonUtility.UpdateTextVisible(nameText, !_auctionNewFilterData.IsSelected);
        }
    }
}
