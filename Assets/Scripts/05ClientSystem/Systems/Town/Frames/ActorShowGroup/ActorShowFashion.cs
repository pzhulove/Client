using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameClient
{
    class ActorShowFashion : ClientFrame
    {
        ActorShowEquipData m_kData;

        public override string GetPrefabPath()
        {
            m_kData = userData as ActorShowEquipData;
            if (null != m_kData)
            {
                if (!string.IsNullOrEmpty(m_kData.prefabPath))
                {
                    return m_kData.prefabPath;
                }
            }
            return "UIFlatten/Prefabs/ActorShow/ActorShowFashion";
        }

        protected override void _OnOpenFrame()
        {
            m_akCachedEquiptItemObjects.Clear();
            m_kData = userData as ActorShowEquipData;

            _InitSlots();
            _InitFashions();

            FashionItemObject.onItemClicked += _OnFashionClicked;
        }

        ItemData _GetCompareItem(ItemData item)
        {
            ItemData compareItem = null;
            if (item != null && item.WillCanEquip())
            {
                List<ulong> guids = null;
                guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);

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

        void _OnFashionClicked(GameObject obj, ItemData item)
        {
            if (item != null)
            {
                if(!m_kData.bCompare)
                {
                    ItemTipManager.GetInstance().ShowTip(item);
                }
                else
                {
                    ItemData compareItem = _GetCompareItem(item);
                    if (compareItem != null)
                    {
                        ItemTipManager.GetInstance().ShowTipWithCompareItem(item, compareItem);
                    }
                    else
                    {
                        ItemTipManager.GetInstance().ShowTip(item);
                    }
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            m_kData = null;
            m_akCachedEquiptItemObjects.Clear();
            FashionItemObject.onItemClicked -= _OnFashionClicked;
        }

        GameObject m_goLeft;
        GameObject m_goRight;

        void _InitSlots()
        {
            m_goLeft = Utility.FindChild(frame, "Fashions/Left");
            m_goRight = Utility.FindChild(frame, "Fashions/Right");
            List<ComItem> akGoItem = new List<ComItem>();
            int iCount = (int)EFashionWearSlotType.Max - ((int)EFashionWearSlotType.Invalid + 1);
            for (int i = 0; i < iCount; ++i)
            {
                ComItem comItem = CreateComItem(i < iCount / 2 ? m_goRight : m_goLeft);
                akGoItem.Add(comItem);
            }

            for (int i = (int)EFashionWearSlotType.Invalid + 1; i < (int)EFashionWearSlotType.Max; ++i)
            {
                MapIndex mapIndex = Utility.GetEnumAttribute<EFashionWearSlotType, MapIndex>((EFashionWearSlotType)i);
                if (mapIndex.Index >= 0 && mapIndex.Index < akGoItem.Count)
                {
                    var comItem = akGoItem[mapIndex.Index];
                    GameObject goParent = comItem.transform.parent.gameObject;
                    GameObject goLocal = comItem.transform.gameObject;
                    EFashionWearSlotType eEFashionWearSlotType = (EFashionWearSlotType)i;
                    m_akCachedEquiptItemObjects.Create(eEFashionWearSlotType, new object[] { goParent, goLocal, eEFashionWearSlotType, this, null });
                }
            }
        }

        void _InitFashions()
        {
            if (m_kData.m_akFashions != null)
            {
                for (int i = 0; i < m_kData.m_akFashions.Count; ++i)
                {
                    var itemData = m_kData.m_akFashions[i];
                    if (itemData != null)
                    {
                        if (m_akCachedEquiptItemObjects.HasObject(itemData.FashionWearSlotType))
                        {
                            m_akCachedEquiptItemObjects.RefreshObject(itemData.FashionWearSlotType, new object[] { itemData });
                        }
                    }
                }
            }
        }
        #region fashions
        public class FashionItemObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected EFashionWearSlotType eEFashionWearSlotType;
            protected ActorShowEquip THIS;
            public static ComItem.OnItemClicked onItemClicked = null;

            ComItem comItem;
            ItemData itemData;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goLocal = param[1] as GameObject;
                eEFashionWearSlotType = (EFashionWearSlotType)param[2];
                THIS = param[3] as ActorShowEquip;
                itemData = param[4] as ItemData;
                comItem = goLocal.GetComponent<ComItem>();
                comItem.Setup(itemData, OnItemClicked);

                Enable();
                _UpdateItem();
            }

            public override void OnRecycle()
            {
                Disable();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param) { OnCreate(param); }
            public override void OnRefresh(object[] param)
            {
                OnCreate(new object[] { goParent, goLocal, eEFashionWearSlotType, THIS, param[0] });
            }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                string part = Utility.GetEnumDescription<EFashionWearSlotType>(eEFashionWearSlotType);
                part = TR.Value(part);
                comItem.SetupSlot(ComItem.ESlotType.Opened, part);
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                if(onItemClicked != null)
                {
                    onItemClicked.Invoke(obj,item);
                }
            }
        }

        CachedObjectDicManager<EFashionWearSlotType, FashionItemObject> m_akCachedEquiptItemObjects = new CachedObjectDicManager<EFashionWearSlotType, FashionItemObject>();
        #endregion
    }
}