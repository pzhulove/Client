using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComRanomTreasureItemBind : MonoBehaviour
    {
        
        #region Model Params

        [SerializeField]
        private Protocol.DigType mItemType = Protocol.DigType.DIG_INVALID;
        public RandomTreasureInfo mInfo;

        private int mItemId = 0;

        #endregion
        
        #region View Params
        

        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake()
        {
            if (mInfo != null)
            {
                mInfo.onTitleBtnClick = () => { OnTryGetBtnClick(); };
            }

            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, OnChange);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, OnChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTresureItemCountChanged, OnChange);
        }
        
        //Unity life cycle
        void Start () 
        {
            if (mItemType == Protocol.DigType.DIG_GLOD)
            {
                mItemId = RandomTreasureDataManager.GetInstance().Gold_Treasure_Item_Id;
            }
            else if (mItemType == Protocol.DigType.DIG_SILVER)
            {
                mItemId = RandomTreasureDataManager.GetInstance().Silver_Treasure_Item_Id;
            }

            if (mInfo != null)
            {
                string itemIcon = ItemDataManager.GetInstance().GetOwnedItemIconPath(mItemId);
                mInfo.SetInfoTitleImg(itemIcon);
            }
            SetInfoContent();
        }
        
        //Unity life cycle
        void Update () 
        {
            
        }
        
        //Unity life cycle
        void OnDestroy () 
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, OnChange);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, OnChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTresureItemCountChanged, OnChange);
        }

        void OnTryGetBtnClick()
        {
            if (mItemId == 0)
            {
                return;
            }
            ItemComeLink.OnLink(mItemId, 0, false);
        }

        void OnChange(UIEvent uiEvent)
        {
            SetInfoContent();
        }

        void SetInfoContent()
        {
            if (mInfo != null)
            {
                mInfo.SetInfoContent(Utility.ToThousandsSeparator((ulong)ItemDataManager.GetInstance().GetOwnedItemCount(mItemId, false)));
            }
        }
        
        #endregion
        
        #region  PUBLIC METHODS

        #endregion
    }
}