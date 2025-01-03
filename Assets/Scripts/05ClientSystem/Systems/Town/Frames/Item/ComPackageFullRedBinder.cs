using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using UnityEngine.Events;

namespace GameClient
{
    class ComPackageFullRedBinder : MonoBehaviour
    {
        public enum PackageTab
        {
            PT_EQUIPMENT = 0,
            PT_FASHION,
            PT_TITLE,
            PT_PET,
            PT_BXY,
            PT_COUNT,
        }

        public UnityEvent onSucceed;
        public UnityEvent onFailed;
        public PackageTab[] mFlags = new PackageTab[0];

        public bool HasFlag(PackageTab iFlag)
        {
            for(int i = 0; i < mFlags.Length; ++i)
            {
                if(mFlags[i] == iFlag)
                {
                    return true;
                }
            }
            return false;
        }

        void _CheckRedPoints()
        {
            bool bHasRedPoint = false;
            for(int i = 0; i < (int)PackageTab.PT_COUNT && !bHasRedPoint; ++i)
            {
                if(!HasFlag((PackageTab)i))
                {
                    continue;
                }

                switch((PackageTab)i)
                {
                    case PackageTab.PT_EQUIPMENT:
                        {
                            bHasRedPoint = _CheckEquipmentIsFull();
                        }
                        break;
                    case PackageTab.PT_FASHION:
                        {
                            bHasRedPoint = _CheckFashionIsFull();
                        }
                        break;
                    case PackageTab.PT_TITLE:
                        {
                            bHasRedPoint = _CheckTitleIsFull();
                        }
                        break;
                    case PackageTab.PT_BXY:
                        {
                            bHasRedPoint = _CheckBxyIsFull();
                        }
                        break;    
                    case PackageTab.PT_PET:
                        {
                            bHasRedPoint = false;
                            //bHasRedPoint = PetDataManager.GetInstance().IsNeedShowPetEggRedPoint() || PetDataManager.GetInstance().IsNeedShowOnUsePetsRedPoint();
                        }
                        break;
                }
            }

            var action = bHasRedPoint ? onSucceed : onFailed;
            if (null != action)
            {
                action.Invoke();
            }
        }

        bool _CheckEquipmentIsFull()
        {
            return _checkIsFullByType(EPackageType.Equip);
        }

        bool _CheckFashionIsFull()
        {
            return _checkIsFullByType(EPackageType.Fashion);
        }

        bool _CheckTitleIsFull()
        {
            return _checkIsFullByType(EPackageType.Title);
        }

        bool _CheckBxyIsFull()
        {
            return _checkIsFullByType(EPackageType.Bxy);
        }

        private bool _checkIsFullByType(EPackageType type)
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(type);
            int iCount = 0;
            if (null != itemGuids)
            {
                iCount = itemGuids.Count;
            }

            if (PlayerBaseData.GetInstance().PackTotalSize.Count > (int)type)
            {
                return PlayerBaseData.GetInstance().PackTotalSize[(int)type] <= iCount;
            }

            return false;
        }

        void Awake()
        {
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdatePackageTabRedPoint, _UpdatePackageTabRedPoint);
        }

        void Start()
        {
            _CheckRedPoints();
        }

        void _OnAddNewItem(List<Item> items)
        {
            _CheckRedPoints();
        }

        void _OnUpdateItem(List<Item> items)
        {
            _CheckRedPoints();
        }

        void _OnRemoveItem(ItemData data)
        {
            _CheckRedPoints();
        }

        void _UpdatePackageTabRedPoint(UIEvent uiEvent)
        {
            _CheckRedPoints();
        }
        void OnDestroy()
        {
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdatePackageTabRedPoint, _UpdatePackageTabRedPoint);
        }
    }
}