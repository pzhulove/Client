using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComTapDonate : MonoBehaviour
    {
        public GameObject goItemParent;
        public Text Name;
        public Toggle toggle;
        public GameObject goCheckMark;
        ComItem comItem = null;
        public ItemData Value
        {
            get
            {
                return data;
            }
        }

        static List<ItemData> ms_selectItems = new List<ItemData>();
        public static void Clear()
        {
            ms_selectItems.Clear();
        }

        public static List<ItemData> SelectedItems
        {
            get
            {
                return ms_selectItems;
            }
        }

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDonateSelecteItemChanged, _OnDonateSelecteItemChanged);
        }

        void _OnDonateSelecteItemChanged(UIEvent uiEvent)
        {
            _UpdateCheckMark();
        }

        void _UpdateCheckMark()
        {
            //Maybe Fix a null reference error..?
            if (ms_selectItems == null)
                return;

            var find = ms_selectItems.Find(x =>
            {
                return null != x && null != Value && x.GUID == Value.GUID;
            });
            goCheckMark.CustomActive(null != find);
        }

        public static void DelecteAllItems()
        {
            while(ms_selectItems.Count > 0)
            {
                var current = ms_selectItems[0];
                ms_selectItems.RemoveAt(0);
                if(null != current)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnDonateSelecteItemChanged);
                }
            }
        }

        public static ulong[] GetSelectedItems()
        {
            List<ulong> pools = GamePool.ListPool<ulong>.Get();
            for(int i = 0; i < ms_selectItems.Count; ++i)
            {
                if(null != ms_selectItems[i])
                {
                    pools.Add(ms_selectItems[i].GUID);
                }
            }
            ulong[] items = pools.ToArray();
            GamePool.ListPool<ulong>.Release(pools);
            return items;
        }

        ItemData data = null;
        public void OnItemVisible(ItemData value)
        {
            data = value;
            if (null != data)
            {
                if (null != Name)
                {
                    Name.text = data.GetColorName();
                }

                if (null == comItem)
                {
                    comItem = ComItemManager.Create(goItemParent);
                }

                if (null != comItem)
                {
                    comItem.Setup(data, (GameObject obj, ItemData item) =>
                    {
                        if (null != item)
                        {
                            ItemTipManager.GetInstance().ShowTip(item);
                        }
                    });
                }

                if (null != toggle)
                {
                    toggle.onValueChanged.RemoveListener(_OnToggleChanged);
                }

                _UpdateCheckMark();

                var find = ms_selectItems.Find(x =>
                {
                    return null != x && x.GUID == Value.GUID;
                });

                if (null != toggle)
                {
                    toggle.isOn = null != find;
                    toggle.onValueChanged.AddListener(_OnToggleChanged);
                }
            }
        }

        void _OnToggleChanged(bool bValue)
        {
            if(bValue)
            {
                if(!ms_selectItems.Contains(Value))
                {
                    ms_selectItems.Add(Value);
                }
            }
            else
            {
                ms_selectItems.Remove(Value);
            }
            var find = ms_selectItems.Find(x =>
            {
                return null != x && x.GUID == Value.GUID;
            });
            goCheckMark.CustomActive(null != find);
        }

        void OnDestroy()
        {
            if (null != comItem)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }

            if (null != toggle)
            {
                toggle.onValueChanged.RemoveListener(_OnToggleChanged);
                toggle = null;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDonateSelecteItemChanged, _OnDonateSelecteItemChanged);
        }
    }
}