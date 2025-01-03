using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
   
    public class ShopNewFilterElementItem : MonoBehaviour
    {

        private OnShopNewFilterElementItemTabValueChanged _onToggleValueChanged = null;

        private ShopNewFilterData _shopNewFilterData = null;

        [SerializeField]
        private Text nameText;
        [SerializeField] private Image selectedFlag;
        [SerializeField] private Button button;

        private void Awake()
        {
            _shopNewFilterData = null;
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

        public void InitData(ShopNewFilterData shopNewFilterData,
            OnShopNewFilterElementItemTabValueChanged toggleValueChanged = null)
        {
            _shopNewFilterData = shopNewFilterData;
            if (_shopNewFilterData == null)
                return;

            if (nameText != null)
            {
                nameText.text = _shopNewFilterData.Name;
            }

            _onToggleValueChanged = toggleValueChanged;

            UpdateFilterElementItem();
        }

        private void OnButtonClick()
        {

            _shopNewFilterData.IsSelected = true;

            if (_onToggleValueChanged != null)
                _onToggleValueChanged(_shopNewFilterData);

            UpdateFilterElementItem();
        }
        
        public void UpdateFilterElementItem()
        {

            if(_shopNewFilterData == null)
                return;

            if(selectedFlag == null)
                return;

            selectedFlag.gameObject.CustomActive(_shopNewFilterData.IsSelected);
            var colorChange = nameText.GetComponent<ComSetTextColor>();
            if (null != colorChange)
                colorChange.SetColor(_shopNewFilterData.IsSelected ? 1 : 0);
        }
    }
}
