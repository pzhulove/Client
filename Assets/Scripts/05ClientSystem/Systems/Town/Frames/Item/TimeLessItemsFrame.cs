using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Scripts.UI;
using ProtoTable;
using EItemType = ProtoTable.ItemTable.eType;
using UnityEngine.Events;


namespace GameClient
{
    class TimeLessItemsFrameData
    {
        public UnityAction onCloseFrame;
    }

    class TimeLessItemsFrame : ClientFrame
    {
        [UIControl("Items")]
        ComUIListScript m_comItems;

        TimeLessItemsFrameData data;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/TimeLessItems";
        }

        protected override void _OnOpenFrame()
        {
            data = userData as TimeLessItemsFrameData;

            m_comItems.Initialize();

            m_comItems.onBindItem = var =>
            {
                GameObject objItemRoot = Utility.FindGameObject(var.gameObject, "Item");
                return CreateComItem(objItemRoot);
            };
            m_comItems.onItemVisiable = var =>
            {
                List<ItemData> arrItemData = ItemDataManager.GetInstance().GetTimeLessItems();
                if (var.m_index >= 0 && var.m_index < arrItemData.Count)
                {
                    ItemData itemData = arrItemData[var.m_index];

                    ComItem comItem = var.gameObjectBindScript as ComItem;
                    comItem.Setup(itemData, (var1, var2) =>
                    {
                        ItemTipManager.GetInstance().ShowTip(var2);
                    });

                    Text labName = Utility.GetComponetInChild<Text>(var.gameObject, "Name");
                    labName.text = itemData.GetColorName();

                    Text labRemainTime = Utility.GetComponetInChild<Text>(var.gameObject, "TimeRemain");
                    labRemainTime.text = _GetTimeLeftDesc(itemData);

                    Button btnFunc = Utility.GetComponetInChild<Button>(var.gameObject, "Func");
                    Text labFunc = Utility.GetComponetInChild<Text>(var.gameObject, "Func/Text");
                    string strFuncName;
                    UnityEngine.Events.UnityAction func;
                    if (_GetSuitableFunc(itemData, out strFuncName, out func))
                    {
                        btnFunc.onClick.RemoveAllListeners();
                        btnFunc.onClick.AddListener(func);
                        labFunc.text = strFuncName;
                        btnFunc.gameObject.SetActive(true);
                    }
                    else
                    {
                        btnFunc.gameObject.SetActive(false);
                    }
                }
            };

            m_comItems.SetElementAmount(ItemDataManager.GetInstance().GetTimeLessItems().Count);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TimeLessItemsChanged, _OnTimeLessItemsChanged);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TimeLessItemsChanged, _OnTimeLessItemsChanged);
            if(null != data)
            {
                if(null != data.onCloseFrame)
                {
                    data.onCloseFrame.Invoke();
                    data.onCloseFrame = null;
                }
                data = null;
            }
        }

        string _GetTimeLeftDesc(ItemData a_itemData)
        {
            int timeLeft;
            bool bStartCountdown;
            a_itemData.GetTimeLeft(out timeLeft, out bStartCountdown);
            if (bStartCountdown)
            {
                if (timeLeft > 0)
                {
                    int second = 0;
                    int minute = 0;
                    int hour = 0;
                    second = timeLeft % 60;
                    int temp = timeLeft / 60;
                    if (temp > 0)
                    {
                        minute = temp % 60;
                        hour = temp / 60;
                    }

                    string value = "";
                    if (hour > 0)
                    {
                        value += string.Format("{0}小时", hour);
                    }
                    if (minute > 0)
                    {
                        value += string.Format("{0}分", minute);
                    }
                    if (second > 0)
                    {
                        value += string.Format("{0}秒", second);
                    }

                    return TR.Value("tip_color_bad", TR.Value("item_time_left", value));
                }
                else
                {
                    return TR.Value("tip_color_bad", TR.Value("item_time_left_none"));
                }
            }
            else
            {
                return string.Empty;
            }
        }

        bool _GetSuitableFunc(ItemData a_itemData, out string a_strName, out UnityEngine.Events.UnityAction a_func)
        {
            if (a_itemData.CanRenewal())
            {
                a_strName = TR.Value("tip_renewal");
                a_func = () => { _OnRenewalItem(a_itemData); };
                return true;
            }
            else
            {
                int nTimeLeft;
                bool bStartCountdown;
                a_itemData.GetTimeLeft(out nTimeLeft, out bStartCountdown);
                if (bStartCountdown == true && nTimeLeft <= 0)
                {
                    a_strName = string.Empty;
                    a_func = null;
                    return false;
                }

                if (a_itemData.UseType == ProtoTable.ItemTable.eCanUse.UseOne ||
                    a_itemData.UseType == ProtoTable.ItemTable.eCanUse.UseTotal)
                {
                    if (
                        a_itemData.Type != EItemType.FUCKTITTLE && 
                        a_itemData.Type != EItemType.EQUIP && 
                        a_itemData.Type != EItemType.FASHION
                        )
                    {
                        // 使用
                        a_strName = TR.Value("tip_use");
                        a_func = () => { _TryOnUseItem(a_itemData); };
                        return true;
                    }
                }
                else
                {
                    var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)a_itemData.TableID);
                    if (dataItem != null && !string.IsNullOrEmpty(dataItem.LinkInfo))
                    {
                        // 用途
                        a_strName = TR.Value("tip_itemLink");
                        a_func = () => { _OnItemLink(a_itemData); };
                        return true;
                    }
                    else
                    {
                        if (
                            a_itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                            a_itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard &&
                            Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Forge)
                            )
                        {
                            // 锻冶
                            a_strName = TR.Value("tip_forge");
                            a_func = () => { _OnForgeItem(a_itemData); };
                            return true;
                        }
                    }
                }

                a_strName = string.Empty;
                a_func = null;
                return false;
            }
        }

        void _OnRenewalItem(ItemData item)
        {
            ClientSystemManager.GetInstance().OpenFrame<RenewalItemFrame>(FrameLayer.Middle, item);
        }

        void _OnDeSealClicked(ItemData item)
        {
            if (item != null && item.Packing == true)
            {
                if (item.CanEquip())
                {
                    SystemNotifyManager.SystemNotify(2006,
                        () =>
                        {
                            ItemDataManager.GetInstance().UseItem(item);
                            AudioManager.instance.PlaySound(102);
                        },
                        null,
                        item.GetColorName()
                        );
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("equip_deseal_notify_cannot", item.GetColorName()));
                }
            }
        }

        void _TryOnUseItem(ItemData item)
        {
            if (item.Type == EItemType.EQUIP)
            {
                int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                if (iEquipedMasterPriority == 2)
                {
                    SystemNotifyManager.SystemNotifyOkCancel(7019,
                        () =>
                        {
                            _OnUseItem(item);
                        },
                        null);
                    return;
                }
            }

            _OnUseItem(item);
        }

        void _OnUseItem(ItemData item)
        {
            if (item != null)
            {
                if (item.PackID > 0)
                {
                    GiftPackTable giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(item.PackID);
                    if (giftPackTable != null)
                    {
                        if (giftPackTable.FilterType == GiftPackTable.eFilterType.Custom || giftPackTable.FilterType == GiftPackTable.eFilterType.CustomWithJob)
                        {
                            if (giftPackTable.FilterCount > 0)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<SelectItemFrame>(FrameLayer.Middle, item);
                            }
                            else
                            {
                                Logger.LogErrorFormat("礼包{0}的FilterCount小于等于0", item.PackID);
                            }
                        }
                        else
                        {
                            ItemDataManager.GetInstance().UseItem(item);
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("道具{0}的礼包ID{1}不存在", item.TableID, item.PackID);
                    }
                }
                else
                {
                    ItemDataManager.GetInstance().UseItem(item);
                    if (item.PackageType == EPackageType.Equip || item.PackageType == EPackageType.Fashion)
                    {
                        AudioManager.instance.PlaySound(102);
                    }
                }
            }
        }

        void _OnItemLink(ItemData item)
        {
            var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.LinkInfo))
            {
                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(dataItem.FunctionID);
                if (FuncUnlockdata != null && FuncUnlockdata.FinishLevel > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SystemNotify(FuncUnlockdata.CommDescID);
                    return;
                }

                ActiveManager.GetInstance().OnClickLinkInfo(dataItem.LinkInfo);
            }
        }

        void _OnForgeItem(ItemData a_item)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData();
                data.itemData = a_item;

                if (a_item.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
                {
                    data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_ENCHANTMENTCARD;
                    data.iDefaultSecondTabId = (int)EnchantmentCardSubTabType.ECSTT_EQUIPMENTENCHANT;
                }
                else
                {
                    data.iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_STRENGTHEN;
                }

                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true);
                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, data);
            }
        }

        void _OnTimeLessItemsChanged(UIEvent a_event)
        {
            m_comItems.SetElementAmount(ItemDataManager.GetInstance().GetTimeLessItems().Count);
        }

        [UIEventHandle("Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
