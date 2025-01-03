using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public class GiftBagItemEx : MonoBehaviour
    {
        [SerializeField]
        Image imgFrame = null;

        [SerializeField]
        Image imgCheckMark = null;

        [SerializeField]
        Button btnSelect = null;

        [SerializeField]
        GameObject item = null;

        [SerializeField]
        Image imgOwn = null;

        [SerializeField]
        Text txtItemName = null;

        [SerializeField]
        Text txtItemDesc = null;

        [SerializeField]
        Image imgMask = null;

        ItemData m_item = null;
        ComItem.OnItemClicked m_callback = null;

        // Use this for initialization
        void Start()
        {
            IsSelect = false;
            Update();
        }

        public void RegistClickCallBack()
        {
            if (btnSelect != null)
            {
                btnSelect.onClick.RemoveAllListeners();
                btnSelect.onClick.AddListener(OnClickSelect);
            }
        }

        public void CancleClickCallBack()
        {
            if (btnSelect != null)
            {
                btnSelect.onClick.RemoveAllListeners();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (imgFrame != null && imgCheckMark != null && imgMask != null)
            {
                imgFrame.CustomActive(IsSelect);
                imgCheckMark.CustomActive(IsSelect);
                imgMask.CustomActive(IsSelect);
            }
        }

        void OnClickSelect()
        {
            if (m_callback != null)
            {
                m_callback(gameObject, m_item);
            }
        }

        public bool IsSelect
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        void OnItemClick(GameObject gameObject, ItemData itemData)
        {
            ItemTipManager.GetInstance().CloseAll();
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        public void Setup(int iIndex, ItemData itemData, ComItem.OnItemClicked callBack)
        {
            if (btnSelect != null)
            {
                btnSelect.onClick.RemoveAllListeners();
                btnSelect.onClick.AddListener(OnClickSelect);
            }

            Index = iIndex;
            m_item = itemData;
            m_callback = callBack;

            ComItem comItem = item.GetComponent<ComItem>();
            if (comItem != null)
            {
                comItem.Setup(itemData, OnItemClick);
            }

            if (txtItemName != null)
            {
                txtItemName.text = itemData.GetColorName();
            }

            if(txtItemDesc != null)
            {
                txtItemDesc.text = itemData.GetDescription();             
            }

            ItemData itemDataTemp = itemData;
            if (imgOwn != null)
            {
                if (itemDataTemp.Type == ItemTable.eType.EQUIP && ItemDataManager.GetInstance().GetItemByTableID(itemDataTemp.TableID) != null)
                {
                    imgOwn.CustomActive(true);
                }
                else
                {
                    imgOwn.CustomActive(false);
                }
            }
        }
    }
}
