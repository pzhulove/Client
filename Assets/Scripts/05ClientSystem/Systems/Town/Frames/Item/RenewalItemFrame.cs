using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{
    class RenewalItemFrame : ClientFrame
    {
        ItemData m_itemData = null;
        int m_nSelectIndex = -1;

        [UIControl("Group/Items")]
        ComUIListScript m_comItems;

        [UIObject("Item")]
        GameObject m_objItemRoot;

        [UIControl("ItemName")]
        Text m_labItemName;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/RenewalItem";
        }

        protected override void _OnOpenFrame()
        {
            m_itemData = userData as ItemData;
            if (m_itemData == null || m_itemData.CanRenewal() == false)
            {
                return;
            }

            ComItem comItem = CreateComItem(m_objItemRoot);
            comItem.Setup(m_itemData, (var1, var2) =>
            {
                ItemTipManager.GetInstance().ShowTip(var2);
            });

            m_labItemName.text = m_itemData.GetColorName();

            m_comItems.Initialize();

            m_comItems.onItemVisiable = var =>
            {
                if (m_itemData == null)
                {
                    return;
                }

                if (m_itemData.CanRenewal() == false)
                {
                    return;
                }

                if (var.m_index >= 0 && var.m_index < m_itemData.arrRenewals.Count)
                {
                    int nIndex = var.m_index;
                    RenewalInfo data = m_itemData.arrRenewals[nIndex];

                    Text labTime = Utility.GetComponetInChild<Text>(var.gameObject, "Time");
                    if (data.nDay <= 0)
                    {
                        labTime.text = TR.Value("item_renewal_forevel");
                    }
                    else
                    {
                        labTime.text = TR.Value("item_renewal_by_day", data.nDay);
                    }

                    Image imgCostIcon = Utility.GetComponetInChild<Image>(var.gameObject, "Icon");
                    ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.nCostID);
                    Assert.IsNotNull(itemData);
                    // imgCostIcon.sprite = AssetLoader.GetInstance().LoadRes(itemData.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref imgCostIcon, itemData.Icon);

                    Text labCostCount = Utility.GetComponetInChild<Text>(var.gameObject, "Count");
                    labCostCount.text = data.nCostCount.ToString();

                    Toggle togSelect = Utility.GetComponetInChild<Toggle>(var.gameObject, "Toggle");
                    togSelect.onValueChanged.RemoveAllListeners();
                    togSelect.onValueChanged.AddListener(var2 =>
                    {
                        if (var2)
                        {
                            m_nSelectIndex = nIndex;
                        }
                    });

                    if (m_nSelectIndex == var.m_index)
                    {
                        togSelect.isOn = true;
                    }
                    else
                    {
                        togSelect.isOn = false;
                    }
                }
            };

            m_nSelectIndex = 0;
            m_comItems.SetElementAmount(m_itemData.arrRenewals.Count);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemRenewalSuccess, _OnRenewalSuccess);
        }

        protected override void _OnCloseFrame()
        {
            m_itemData = null;
            m_nSelectIndex = -1;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemRenewalSuccess, _OnRenewalSuccess);
        }

        void _OnRenewalSuccess(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Func")]
        void _OnOkClicked()
        {
            if (m_itemData != null && m_itemData.CanRenewal())
            {
                if (m_nSelectIndex >= 0 && m_nSelectIndex < m_itemData.arrRenewals.Count)
                {
                    int nMoneyID = m_itemData.arrRenewals[m_nSelectIndex].nCostID;
                    if (nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT) || nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT))
                    {
                        if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                        {
                            return;
                        }
                    }

                    ItemDataManager.GetInstance().RenewalItem(m_itemData, (uint)m_itemData.arrRenewals[m_nSelectIndex].nDay);
                }
            }
        }

        [UIEventHandle("Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
