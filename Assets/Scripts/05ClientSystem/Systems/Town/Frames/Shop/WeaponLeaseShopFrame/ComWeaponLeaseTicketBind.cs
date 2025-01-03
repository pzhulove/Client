using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComWeaponLeaseTicketBind : MonoBehaviour
    {
        [SerializeField]
        private Button mAddBtn;
        [SerializeField]
        private Image mIcon;
        [SerializeField]
        private Text mCount;
        [SerializeField]
        private int mWeaponLeaseTicketItemId;
        [SerializeField]
        private int mWeaponLeaseTicketTatleCount;

        void Awake()
        {
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }
        // Use this for initialization
        void Start()
        {
            SetTicketIcon();
            SetOnAddClick();
            UpdateTicketCount();
        }
        
        void SetTicketIcon()
        {
            if (mIcon)
            {
                string itemIconPath = ItemDataManager.GetInstance().GetOwnedItemIconPath(mWeaponLeaseTicketItemId);
                ETCImageLoader.LoadSprite(ref mIcon, itemIconPath);
            }
        }

        void UpdateTicketCount()
        {
            if (mCount)
            {
                mCount.text = PlayerBaseData.GetInstance().WeaponLeaseTicket.ToString() + "/" + mWeaponLeaseTicketTatleCount.ToString();
            }
        }

        void SetOnAddClick()
        {
            if (mAddBtn)
            {
                mAddBtn.onClick.RemoveAllListeners();
                mAddBtn.onClick.AddListener(OnAddBtnClick);
            }
        }

        void OnAddBtnClick()
        {
            ItemComeLink.OnLink(mWeaponLeaseTicketItemId, 0, false);
        }
        void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            if (eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_WEAPON_LEASE_TICKET)
            {
                UpdateTicketCount();
            }
        }
        void OnDestroy()
        {
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }
    }
}

