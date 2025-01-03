using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class SevenDaysFrame : ClientFrame
    {
        private SevenDaysView mSevenDaysView = null;

        private float mTickTime = 0.0f;
        private bool canUpdate = true;
        private bool dirty = false;

        protected override void _bindExUI()
        {
            mSevenDaysView = mBind.GetCom<SevenDaysView>("SevenDaysView");
        }

        protected override void _unbindExUI()
        {
            mSevenDaysView = null;
        }

        protected override void _OnOpenFrame()
        {
            _BindEvent();

            if (mSevenDaysView != null)
            {
                mSevenDaysView.Init(this);
            }
        }

        protected override void _OnCloseFrame()
        {
            _UnBindEvent();
            _ClearData();
        }

        public override bool IsNeedUpdate()
        {
            return canUpdate;
        }

        
        protected override void _OnUpdate(float delta)
        {
            mTickTime += delta;
            if (mTickTime > 1.0f)
            {

                mTickTime = 0f;
            }
        }

        private void _ClearData()
        {
        }

        private void _BindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SevenDaysActivityUpdate, _OnSevenDaysLoginDatasUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCounterUpdate);
        }

        private void _UnBindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SevenDaysActivityUpdate, _OnSevenDaysLoginDatasUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCounterUpdate);
        }

        private void _OnSevenDaysLoginDatasUpdate(UIEvent uIEvent)
        {
            if (uIEvent == null || uIEvent.Param1 == null)
            {
                return;
            }

            ProtoTable.SevenDaysActiveTable.eSevenDaysActiveType activeType = (ProtoTable.SevenDaysActiveTable.eSevenDaysActiveType)uIEvent.Param1;

            if (mSevenDaysView != null)
            {
                mSevenDaysView.UpdateView(activeType);
            }
        }

        private void _OnCounterUpdate(UIEvent uIEvent)
        {
            if (uIEvent == null || uIEvent.Param1 == null)
            {
                return;
            }

            string key = (string)uIEvent.Param1;
            if (!key.Equals(CounterKeys.COUNTER_SEVENDAYS_ACTIVITY_POINT))
            {
                return;
            }

            if (mSevenDaysView != null)
            {
                mSevenDaysView.UpdateScore();
            }
        }

        //获取礼包信息
        private void _OnGetGiftData(UIEvent param)
        {
            if (param == null || param.Param1 == null)
            {
                Logger.LogError("礼包数据为空");
                return;
            }
            Protocol.GiftPackSyncInfo data = param.Param1 as Protocol.GiftPackSyncInfo;

            if (data == null || data.gifts == null)
            {
                return;
            }

            List<ItemData> itemDatas = new List<ItemData>();
            foreach (var item in data.gifts)
            {
                var itemData = ItemDataManager.CreateItemDataFromTable((int)item.itemId);
                itemData.Count = (int)item.itemNum;
                if (itemData.IsOccupationFit())
                {
                    itemDatas.Add(itemData);
                }
            }

            if (mSevenDaysView != null)
            {
                mSevenDaysView.UpdateGiftAward((int)data.id, itemDatas);
            }
        }

        private ItemData _GetCompareItem(ItemData item)
        {
            ItemData compareItem = null;
            if (item != null && item.WillCanEquip())
            {
                List<ulong> guids = null;
                if (item.PackageType == EPackageType.Equip)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                }
                else if (item.PackageType == EPackageType.Fashion)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                }
                if (guids != null)
                {
                    for (int i = 0; i < guids.Count; ++i)
                    {
                        ItemData tempItem = ItemDataManager.GetInstance().GetItem(guids[i]);
                        if (
                            tempItem != null &&
                            tempItem.GUID != item.GUID &&
                            tempItem.IsWearSoltEqual(item)
                            )
                        {
                            compareItem = tempItem;
                            break;
                        }
                    }
                }
            }
            return compareItem;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/SevenDays/SevenDaysFrame";
        }

        public void Close()
        {
            frameMgr.CloseFrame<SevenDaysFrame>();
        }

        public void GoFunction(ProtoTable.ActiveTable activeItem)
        {
            if (activeItem == null)
            {
                return;
            }

            if (ActiveManager.GetInstance()._CheckCanJumpGo(activeItem.LinkLimit, true))
            {
                ActiveManager.GetInstance().OnClickLinkInfo(activeItem.LinkInfo);
            }
        }

        public void SubmitActive(int activeId, int param = 0)
        {
            SevendaysDataManager.GetInstance().SubmitTask(activeId, param);
        }

        public void ShowTips(ItemData model)
        {
            ItemData compareItem = _GetCompareItem(model);
            if (compareItem != null)
            {
                ItemTipManager.GetInstance().ShowTipWithCompareItem(model, compareItem, null);
            }
            else
            {
                ItemTipManager.GetInstance().ShowTip(model, null, TextAnchor.MiddleLeft);
            }
        }
    }
}