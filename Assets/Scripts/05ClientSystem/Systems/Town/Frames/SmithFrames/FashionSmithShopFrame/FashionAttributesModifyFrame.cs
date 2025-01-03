using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;
using Network;

namespace GameClient
{
    class FashionAttributeItemData
    {
        public ProtoTable.EquipAttrTable item;
        public ProtoTable.EquipAttrTable selected;
        public bool bLast;
    }

    class FashionAttributesModifyFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionAttribute";
        }

        ComFashionAttributeModifyEquipmentList m_kComFashionAttributeModifyEquipmentList = new ComFashionAttributeModifyEquipmentList();

        ulong linkGUID = 0;

        protected override void _OnOpenFrame()
        {
            linkGUID = (ulong)userData;
            m_kComFashionAttributeModifyEquipmentList.Initialize(this, Utility.FindChild(frame, "Equiptments"), _OnItemSelected,linkGUID);
            Locked = false;

            _RegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
        }

        void _UnRegisterUIEvent()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
        }

        void OnAddNewItem(List<Item> items)
        {
            m_kComFashionAttributeModifyEquipmentList.RefreshAllEquipments();
        }

        void OnRemoveItem(ItemData data)
        {
            m_kComFashionAttributeModifyEquipmentList.RefreshAllEquipments();
        }

        void OnUpdateItem(List<Item> items)
        {
            m_kComFashionAttributeModifyEquipmentList.RefreshAllEquipments();
        }

        [UIControl("EquipAdjust/Root/Light_BG/Text", typeof(Text))]
        Text curAttribute;
        [UIObject("EquipAdjust/Root/middleback/Scroll View/Viewport/Content/AttrItems")]
        GameObject goAttributeParent;
        [UIObject("EquipAdjust/Root/middleback/Scroll View/Viewport/Content/AttrItems/AttrItem")]
        GameObject goAttributePrefab;
        CachedObjectListManager<FashionAttributeItem> m_akFashionAttributeItems = new CachedObjectListManager<FashionAttributeItem>();
        [UIObject("EquipAdjust/Root")]
        GameObject goRoot;
        [UIControl("EquipAdjust", typeof(StatusBinder))]
        StatusBinder comBinder;
        [UIControl("EquipAdjust/Root/CostItem/Name", typeof(Text))]
        Text costItemName;
        [UIControl("EquipAdjust/Root/CostItem/Count", typeof(Text))]
        Text costItemCount;
        [UIControl("EquipAdjust/Root/CostItem/Acquired", typeof(Text))]
        Text costItemAcquired;
        [UIObject("EquipAdjust/Root/CostItem/ItemParent")]
        GameObject goCostItemParent;
        ComItem comItem;
        [UIObject("EquipAdjust/Mask")]
        GameObject goMask;
        [UIObject("Equiptments/ViewPort/AddMagicFriendlyHint")]
        GameObject goFriendlyHint;

        public static void CommandOpen(object data)
        {
            ClientSystemManager.GetInstance().OpenFrame<FashionAttributesModifyFrame>(FrameLayer.Middle,(ulong)0);
        }

        void _RefreshFashionAttributeItems(ItemData itemData)
        {
            ProtoTable.EquipAttrTable lastSelectedItem = null;
            if(null != FashionAttributeItem.Selected &&
                null != FashionAttributeItem.Selected.Value)
            {
                lastSelectedItem = FashionAttributeItem.Selected.Value.item;
            }

            m_akFashionAttributeItems.RecycleAllObject();
            if (itemData.fashionAttributes != null)
            {
                var CurFashionAttribute = itemData.CurFashionAttribute;
                for (int i = 0; i < itemData.fashionAttributes.Count; ++i)
                {
                    m_akFashionAttributeItems.Create(new object[]
                    {
                        goAttributeParent,
                        goAttributePrefab,
                        new FashionAttributeItemData
                        {
                            item = itemData.fashionAttributes[i],
                            selected = CurFashionAttribute,
                            bLast = (i ==  itemData.fashionAttributes.Count - 1)
                        },
                        System.Delegate.CreateDelegate(typeof(FashionAttributeItem.OnSelectedDelegate), this, "_OnFashionAttributeItemSelected"),
                    });
                }
            }
            var find = m_akFashionAttributeItems.ActiveObjects.Find(x => 
            {
                return lastSelectedItem == x.Value.item && lastSelectedItem != itemData.CurFashionAttribute;
            });
            if(find == null)
            {
                find = m_akFashionAttributeItems.ActiveObjects.Find(x => { return x.Value.item != itemData.CurFashionAttribute; });
            }
            if (find != null)
            {
                find.OnSelected();
            }
        }

        void _OnItemSelected(ItemData itemData)
        {
            goFriendlyHint.CustomActive(!m_kComFashionAttributeModifyEquipmentList.HasEquipments());

            if (comItem == null)
            {
                comItem = CreateComItem(goCostItemParent);
            }

            goRoot.CustomActive(itemData != null);
            if (itemData == null)
            {
                return;
            }

            comBinder.ChangeStatus(itemData.FashionFreeTimes > 0 ? 2 : 1);

            if (itemData.HasFashionAttribute)
            {
                curAttribute.text = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(itemData.FashionAttributeID, "fashion_attribute_color_white");
            }

            goAttributePrefab.CustomActive(false);

            _RefreshFashionAttributeItems(itemData);

            var costItem = FashionAttributeSelectManager.GetInstance().CostItem;
            if (null != costItem)
            {
                var costItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(costItem.ID);
                if(null != costItemData)
                {
                    costItemName.text = costItemData.GetColorName();
                }

                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)costItem.ID);
                int iCostCount = 1;
                if(iHasCount < iCostCount)
                {
                    costItemCount.text = string.Format("<color=#FF0000FF>{0}/{1}</color>", iHasCount, iCostCount);
                }
                else
                {
                    costItemCount.text = string.Format("<color=#00FD7DFF>{0}/{1}</color>", iHasCount, iCostCount);
                }

                comItem.SetShowNotEnoughState(iHasCount < iCostCount);
                costItemAcquired.CustomActive(iHasCount < iCostCount);

                if (null != comItem && costItemData != null)
                {
                    comItem.Setup(costItemData, ((GameObject obj, ItemData item) => 
                    {
                        if(iHasCount < iCostCount)
                        {
                            ItemComeLink.OnLink(costItemData.TableID, 0);
                        }
                        else
                        {
                            ItemTipManager.GetInstance().ShowTip(item);
                        }
                    }));
                }
            }
        }

        void _OnFashionAttributeItemSelected(FashionAttributeItemData data)
        {

        }

        [UIEventHandle("EquipAdjust/Root/BtnApplay")]
        void _OnClickBtnApplay()
        {
            if (Locked)
            {
                return;
            }

            if(!_CheckIsLegal())
            {
                return;
            }

            _OnConfirmToSelectAttribute();
        }

        [UIEventHandle("EquipAdjust/Root/BtnApplyConfirm")]
        void _OnClickBtnApplyConfirm()
        {
            if(Locked)
            {
                return;
            }

            if (!_CheckIsLegal())
            {
                return;
            }

            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(FashionAttributeSelectManager.GetInstance().CostItemID);
            if(iHasCount > 0)
            {
                _OnConfirmToSelectAttribute();
                return;
            }
            WorldGetMallItemByItemIdReq msg = new WorldGetMallItemByItemIdReq();
            msg.itemId = (uint)FashionAttributeSelectManager.GetInstance().CostItemID;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGetMallItemByItemIdRes>(msgRet =>
            {
                if (msgRet.retCode != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.retCode);
                    return;
                }
                var mallItemInfo = msgRet.mallItem;
                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
            }, false);
            //ItemComeLink.OnLink(FashionAttributeSelectManager.GetInstance().CostItemID, 1, true, _OnConfirmToSelectAttribute);
        }

        void _OnConfirmToSelectAttribute()
        {
            var orgAttribute = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(FashionAttributeItem.Selected.Value.selected.ID);
            var curSelectedAttribute = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(FashionAttributeItem.Selected.Value.item.ID);
            SystemNotifyManager.SystemNotify(7007,
                () =>
                {
                    Locked = true;
                    _NetSyncSelectFashionAttribute(ComFashionAttributeModifyEquipment.ms_selected, FashionAttributeItem.Selected.Value.item.ID);
                },
                null,
                new object[] { orgAttribute, curSelectedAttribute });
        }

        bool bLocked = false;
        public bool Locked
        {
            get
            {
                return bLocked;
            }

            set
            {
                bLocked = value;
                goMask.CustomActive(value);
            }
        }

        bool _CheckIsLegal()
        {
            if (ComFashionAttributeModifyEquipment.ms_selected == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashion_not_exist"));
                return false;
            }

            if (FashionAttributeItem.Selected == null ||
                FashionAttributeItem.Selected.Value == null ||
                FashionAttributeItem.Selected.Value.item == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashion_attribute_not_exist"));
                return false;
            }

            return true;
        }

        protected void _NetSyncSelectFashionAttribute(ItemData a_item,int iFashionAttributeId)
        {
            FashionAttributeSelectManager.GetInstance().SendFashionAttributeSelect(a_item.GUID, iFashionAttributeId);

            WaitNetMessageManager.GetInstance().Wait<Protocol.SceneFashionAttributeSelectRes>(msgRet =>
            {
                if(msgRet.result == 0)
                {
                    m_kComFashionAttributeModifyEquipmentList.RefreshAllEquipments();
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashion_select_attribute_ok"));
                }
                else
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                Locked = false;
            });
        }

        protected override void _OnCloseFrame()
        {
            comItem = null;
            FashionAttributeItem.Clear();
            m_akFashionAttributeItems.DestroyAllObjects();
            m_kComFashionAttributeModifyEquipmentList.UnInitialize();
            _UnRegisterUIEvent();
        }
    }
}