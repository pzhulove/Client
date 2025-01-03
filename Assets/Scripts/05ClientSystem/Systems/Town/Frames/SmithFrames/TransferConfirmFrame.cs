using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    class TransferConfirmFrameData
    {
        public ItemData data;
        public ItemData item;
        public UnityAction<ItemData, ItemData> onOk;
    }
    
    class TransferConfirmFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/EquipTransferConfirm";
        }

        TransferConfirmFrameData m_kData = null;
        Text title;
        ComItemNew comItem;
        Text name;
        Button btnOk;
        GameObject goParent;
        GameObject goPrefab;
        protected override void _OnOpenFrame()
        {
            m_kData = userData as TransferConfirmFrameData;
            title = Utility.FindComponent<Text>(frame, "Text");
            btnOk = Utility.FindComponent<Button>(frame, "ok");
            goParent = Utility.FindChild(frame, "middle/body/Viewport/content");
            goPrefab = Utility.FindChild(goParent, "prefabs");
            goPrefab.CustomActive(true);
            comItem = CreateComItemNew(Utility.FindChild(goPrefab, "itemParent"));
            comItem.Setup(null, null);
            name = Utility.FindComponent<Text>(goPrefab, "name");

            _SetData();
        }

        protected override void _OnCloseFrame()
        {
            m_kData = null;
            title = null;
            comItem.Setup(null,null);
            name = null;
            if(btnOk != null)
            {
                btnOk.onClick.RemoveAllListeners();
                btnOk = null;
            }
        }

        void OnItemClicked(GameObject obj, IItemDataModel item)
        {

        }

        void _SetData()
        {
            if(m_kData != null && m_kData.data != null)
            {
                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)m_kData.data.TableID);
                comItem.Setup(itemData, OnItemClicked);

                if (itemData != null)
                {
                    name.text = itemData.GetColorName();
                }
            }
        }

        [UIEventHandle("ok")]
        void OnClickOk()
        {
            btnOk.enabled = false;
            frameMgr.CloseFrame(this);

            if (null != m_kData)
            {
                if (null != m_kData.onOk)
                {
                    m_kData.onOk(m_kData.item, m_kData.data);
                }

                m_kData.item = null;
                m_kData.data = null;
                m_kData.onOk = null;
                m_kData = null;
            }
        }

        [UIEventHandle("close/image")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }
    }
}