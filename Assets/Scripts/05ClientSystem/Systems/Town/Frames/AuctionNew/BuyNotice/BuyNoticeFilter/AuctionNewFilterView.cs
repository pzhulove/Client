using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    
    public delegate void OnAuctionNewFilterElementItemButtonClick(AuctionNewFilterData auctionNewFilterData);

    public class AuctionNewFilterView : MonoBehaviour
    {

        [SerializeField] private AuctionNewFilterControl firstFilterControl = null;
        [SerializeField] private AuctionNewFilterControl secondFilterControl = null;
        [SerializeField] private AuctionNewFilterControl thirdFilterControl = null;

        public void InitAuctionNewFilterView(AuctionNewFilterData firstFilterData,
            OnAuctionNewFilterElementItemButtonClick firstFilterElementItemButtonClick,
            AuctionNewFilterData secondFilterData,
            OnAuctionNewFilterElementItemButtonClick secondFilterElementItemButtonClick,
            AuctionNewFilterData thirdFilterData = null,
            OnAuctionNewFilterElementItemButtonClick thirdFilterElementItemButtonClick = null)
        {

            if (firstFilterControl != null)
            {
                if (firstFilterData != null && firstFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_NONE)
                {
                    firstFilterControl.gameObject.CustomActive(true);
                    firstFilterControl.InitFilterControl(firstFilterData,
                        firstFilterElementItemButtonClick,
                        OnResetFilterListInFilterView);
                }
                else
                {
                    firstFilterControl.gameObject.CustomActive(false);
                }
            }

            if (secondFilterControl != null)
            {
                if (secondFilterData != null 
                    && secondFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_NONE)
                {
                    secondFilterControl.gameObject.CustomActive(true);
                    secondFilterControl.InitFilterControl(secondFilterData,
                        secondFilterElementItemButtonClick,
                        OnResetFilterListInFilterView);
                }
                else
                {
                    secondFilterControl.gameObject.CustomActive(false);
                }
            }

            if (thirdFilterControl != null)
            {
                if (thirdFilterData != null
                    && thirdFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_NONE)
                {
                    thirdFilterControl.gameObject.CustomActive(true);
                    thirdFilterControl.InitFilterControl(thirdFilterData,
                        thirdFilterElementItemButtonClick,
                        OnResetFilterListInFilterView);
                }
                else
                {
                    thirdFilterControl.gameObject.CustomActive(false);
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

            if (thirdFilterControl != null)
            {
                thirdFilterControl.ResetFilterList();
            }

        }

    }
}