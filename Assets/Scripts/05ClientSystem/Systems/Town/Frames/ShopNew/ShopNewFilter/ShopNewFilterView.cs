using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{

    public delegate void OnShopNewFilterElementItemTabValueChanged(ShopNewFilterData shopNewFilterData);

    public class ShopNewFilterView : MonoBehaviour
    {

        [SerializeField] private ShopNewFilterControl firstFilterControl = null;
        [SerializeField] private ShopNewFilterControl secondFilterControl = null;

        public void InitShopNewFilterView(ShopNewFilterData firstFilterData,
            OnShopNewFilterElementItemTabValueChanged firstFilterElementItemTabValueChanged,
            ShopNewFilterData secondFilterData,
            OnShopNewFilterElementItemTabValueChanged secondFilterElementItemTabValueChanged,
            bool isShowFilterTitle = false)
        {

            if (firstFilterControl != null)
            {
                if (firstFilterData != null 
                    && firstFilterData.FilterType != ShopTable.eFilter.SF_NONE
                    && firstFilterData.FilterType != ShopTable.eFilter.SF_PLAY_OCCU)
                {
                    firstFilterControl.gameObject.CustomActive(true);
                    firstFilterControl.InitFilterControl(firstFilterData, 
                        firstFilterElementItemTabValueChanged,
                        OnResetFilterListInFilterView,
                        isShowFilterTitle);
                }
                else
                {
                    firstFilterControl.gameObject.CustomActive(false);
                }
            }

            if (secondFilterControl != null)
            {
                if (secondFilterData != null 
                    && secondFilterData.FilterType != ShopTable.eFilter.SF_NONE
                    && secondFilterData.FilterType != ShopTable.eFilter.SF_PLAY_OCCU)
                {
                    secondFilterControl.gameObject.CustomActive(true);
                    secondFilterControl.InitFilterControl(secondFilterData,
                        secondFilterElementItemTabValueChanged,
                        OnResetFilterListInFilterView,
                        isShowFilterTitle);
                }
                else
                {
                    secondFilterControl.gameObject.CustomActive(false);
                }
            }
        }

        private void OnResetFilterListInFilterView()
        {
            if (firstFilterControl != null)
            {
                firstFilterControl.ResetFilterList();
            }

            if (secondFilterControl != null)
            {
                secondFilterControl.ResetFilterList();
            }
        }

    }
}