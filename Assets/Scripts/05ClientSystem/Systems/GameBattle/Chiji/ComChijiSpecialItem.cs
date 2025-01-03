using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ComChijiSpecialItem : MonoBehaviour
    {
        public int ItemID = 0;
        public Button btItem = null;
        public Text NumText = null;
        [SerializeField]private GameObject mMaskRoot;
        [SerializeField]private Text mTime;

        private int num = 0;
        private bool isUseItem = false;
        private float timer = 0;
        private int CD = 0;
        private void Start()
        {
            _BindUIEvent();

            if(ItemID == 0)
            {
                Logger.LogError("[ComChijiSpecialItem] 脚本未绑定 ItemID，请检查预制体 : ChijiMainFrame");
            }

            num = ItemDataManager.GetInstance().GetItemCount(ItemID);

            if(NumText != null)
            {
                NumText.text = num.ToString();
            }

            if(btItem != null)
            {
                btItem.onClick.RemoveAllListeners();
                btItem.onClick.AddListener(_OnClickItem);
            }
        }

        private void OnDestroy()
        {
            _UnBindUIEvent();

            ItemID = 0;
            num = 0;

            if (btItem != null)
            {
                btItem.onClick.RemoveAllListeners();
                btItem = null;
            }

            if(NumText != null)
            {
                NumText = null;
            }

            isUseItem = false;
            timer = 0;
            CD = 0;
        }

        private void _BindUIEvent()
        {
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, OnItemUseSuccess);
        }

        private void _UnBindUIEvent()
        {
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, OnItemUseSuccess);
        }

        void _OnAddNewItem(List<Item> items)
        {
            bool bHasSelfItem = false;

            for(int i = 0; i < items.Count; i++)
            {
                if(items[i].tableID != ItemID)
                {
                    continue;
                }

                num += items[i].num;
                bHasSelfItem = true;
            }

            if(bHasSelfItem)
            {
                if (NumText != null)
                {
                    NumText.text = num.ToString();
                }
            }
        }

        void _OnRemoveItem(ItemData data)
        {
            if (data.TableID != ItemID)
            {
                return;
            }

            num -= data.Count;

            if (NumText != null)
            {
                NumText.text = num.ToString();
            }
        }

        void _OnUpdateItem(List<Item> items)
        {
            bool bHasSelfItem = false;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].tableID != ItemID)
                {
                    continue;
                }
               
                num = items[i].num;
                bHasSelfItem = true;
            }

            if(bHasSelfItem)
            {
                if (NumText != null)
                {
                    NumText.text = num.ToString();
                }
            }
        }

        private void OnItemUseSuccess(UIEvent uiEvent)
        {
            var item = uiEvent.Param1;
            if (item is ItemData)
            {
                var chijiItemTable = TableManager.GetInstance().GetTableItem<ChijiItemTable>((item as ItemData).TableID);
                if (chijiItemTable != null)
                {
                    if (chijiItemTable.Type == ChijiItemTable.eType.EXPENDABLE &&
                        chijiItemTable.SubType ==  ChijiItemTable.eSubType.ChijiHp)
                    {
                        var currentItemTable = TableManager.GetInstance().GetTableItem<ChijiItemTable>(ItemID);
                        if (currentItemTable != null)
                        {
                            if (currentItemTable.Type == ChijiItemTable.eType.EXPENDABLE &&
                                currentItemTable.SubType == ChijiItemTable.eSubType.ChijiHp)
                            {
                                _GetCD(ItemID);
                            }
                        }
                    }
                }
            }
            else if (item is int)
            {
                if ((int)item == ItemID)
                {
                    _GetCD(ItemID);
                }
            }
        }

        private void _GetCD(int itemID)
        {
            ChijiItemTable chijiItemTable = TableManager.GetInstance().GetTableItem<ChijiItemTable>(ItemID);
            if (chijiItemTable != null)
            {
                CD = chijiItemTable.CoolTime;
            }

            timer = 0.0f;
            _SetTime(true, CD);
            isUseItem = true;
        } 

        private void _SetTime(bool isFlag,int time)
        {
            if (mMaskRoot != null)
            {
                mMaskRoot.CustomActive(isFlag);
            }

            if (mTime != null)
            {
                mTime.text = string.Format("{0}s", time);
            }
        }

        private void _OnClickItem()
        {
            if (isUseItem == true)
            {
                SystemNotifyManager.SysNotifyTextAnimation("该道具在CD中");
                return;
            }

            if(ItemID == 0)
            {
                return;
            }

            if(num <= 0)
            {
                return;
            }

            ItemData itemData = ItemDataManager.GetInstance().GetItemByTableID(ItemID);
            if (itemData == null)
            {
                return;
            }

            ChijiItemTable tableData = TableManager.GetInstance().GetTableItem<ChijiItemTable>(ItemID);
            if(tableData == null)
            {
                return;
            }

            if (tableData.ThirdType == ChijiItemTable.eThirdType.UseToSelf)      // 对自己使用
            {
                if(PlayerBaseData.GetInstance().Chiji_HP_Percent >= 1.0f)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你的血量已满，无需补充！");
                    return;
                }

                ChijiDataManager.GetInstance().CurrentUseDrugId = ItemID;
                ItemDataManager.GetInstance().UseItem(itemData);
            }
            else if(tableData.ThirdType == ChijiItemTable.eThirdType.UseToOther)    // 对别人使用
            {
                var current = ClientSystemManager.instance.CurrentSystem as ClientSystemGameBattle;
                if (current == null || current.MainPlayer == null)
                {
                    return;
                }

                // 陷阱 
                if (tableData.SubType ==  ChijiItemTable.eSubType.ST_CHIJI_TRAP)
                {
                    current.DoTrap(itemData.GUID, 1);
                }
                else
                {
                    // 对接投掷物的处理
                    var playerId = current.MainPlayer.FindNearestPlayer();
                    if (playerId != 0ul)
                    {
                        ChijiDataManager.GetInstance().CreateBullet(itemData.GUID, ItemID, playerId);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemUseSuccess, ItemID);
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("附近没有可投掷的敌人!");
                    }
                }   
            }
        }

        private void Update()
        {
            if (isUseItem == true)
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    timer = 0.0f;
                    CD -= 1;
                    _SetTime(true, CD);

                    if (CD <= 0)
                    {
                        isUseItem = false;
                        _SetTime(false, CD);
                    }
                }
            }
        }
    }
}
