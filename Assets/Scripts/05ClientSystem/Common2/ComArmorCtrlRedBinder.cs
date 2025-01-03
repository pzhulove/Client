using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Protocol;

namespace GameClient
{
    class ComArmorCtrlRedBinder : MonoBehaviour
    {
        public UnityEvent onOK;
        public UnityEvent onFailed;
        bool bDirty = false;
        bool Dirty
        {
            get
            {
                return bDirty;
            }
            set
            {
                bDirty = value;
                InvokeMethod.RemoveInvokeCall(this);
                InvokeMethod.Invoke(this, 0.0f, () =>
                  {
                      _Check();
                  });
            }
        }

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _OnItemUseSeccess);
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            Dirty = true;
        }

        void _OnAddNewItem(List<Item> items)
        {
            Dirty = true;
        }

        void _OnRemoveItem(ItemData data)
        {
            Dirty = true;
        }

        void _OnUpdateItem(List<Item> items)
        {
            Dirty = true;
        }

        // Use this for initialization
        void Start()
        {
            Dirty = true;
        }

        public void OnClickOpenLowArmyHintFrame()
        {
            //Logger.LogErrorFormat("open low army hint frame!");
            ClientSystemManager.GetInstance().OpenFrame<LowArmyHintFrame>(FrameLayer.Middle);
        }

        void _Check()
        {
            bool bNeedRedPoint = false;
            {
                var equips = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                for (int i = 0; i < equips.Count; ++i)
                {
                    var item = ItemDataManager.GetInstance().GetItem(equips[i]);
                    if (null == item)
                    {
                        continue;
                    }

                    int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                    if (iEquipedMasterPriority == 2)
                    {
                        bNeedRedPoint = true;
                        break;
                    }
                }
            }

            var action = bNeedRedPoint ? onOK : onFailed;
            if (null != action)
            {
                action.Invoke();
            }
        }

        void _OnItemUseSeccess(UIEvent uiEvent)
        {
            Dirty = true;
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _OnItemUseSeccess);
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            InvokeMethod.RemoveInvokeCall(this);
        }
    }
}