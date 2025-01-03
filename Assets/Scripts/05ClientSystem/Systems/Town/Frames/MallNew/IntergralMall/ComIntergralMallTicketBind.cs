using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComIntergralMallTicketBind : MonoBehaviour
    {
        [SerializeField]private Image mIcon;
        [SerializeField]private Text mCount;
        [SerializeField]private Button mAddBtn;
        [SerializeField]private int mIntergralItemId;

        private void Awake()
        {
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        private void Start()
        {
            SetAddBtn();
            InitIntergralIcon();
            UpdateTicketCount();
        }

        private void InitIntergralIcon()
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(mIntergralItemId);
            if (itemTable == null)
            {
                return;
            }

            if (mIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mIcon, itemTable.Icon);
            }
        }
        
        private void UpdateTicketCount()
        {
            if (mCount != null)
            {
                mCount.text = Utility.ToThousandsSeparator(PlayerBaseData.GetInstance().IntergralMallTicket);
            }
        }

        private void SetAddBtn()
        {
            if (mAddBtn != null)
            {
                mAddBtn.onClick.AddListener(OnAddBtnClick);
            }
        }

        private void OnAddBtnClick()
        {
            ItemComeLink.OnLink(mIntergralItemId, 0, false);
        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            if (eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_INTERGRALMALL_TICKET)
            {
                UpdateTicketCount();
            }
        }
        
        private void OnDestroy()
        {
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }
    }
}