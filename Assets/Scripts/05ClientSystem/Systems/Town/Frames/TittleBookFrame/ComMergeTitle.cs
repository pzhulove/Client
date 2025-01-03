using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComMergeTitle : MonoBehaviour
    {
        public StateController comState;
        public GameObject ownedMark;
        public GameObject goItemParent;
        public GameObject goCheckMark;
        public Text Name;
        public UIGray gray;

        ComItem comItem;
        TitleMergeData data;
        public TitleMergeData Value
        {
            get
            {
                return data;
            }
        }

        void _OnSelectedMergeTitleChanged(UIEvent uiEvent)
        {
            if(null != Value)
            {
                OnItemVisible(data);
            }
        }

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSelectedMergeTitleChanged, _OnSelectedMergeTitleChanged);
        }

        public static void Clear()
        {
            Selected = null;
        }

        static TitleMergeData ms_selected = null;
        public static TitleMergeData Selected
        {
            get
            {
                return ms_selected;
            }
            set
            {
                ms_selected = value;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSelectedMergeTitleChanged);
            }
        }

        public void OnItemVisible(TitleMergeData value)
        {
            data = value;

            bool isEnough = checkMaterialEnough(value);

            if (null != comState)
            {
                if(isEnough)
                {
                    comState.Key = "can_make";
                }
                else
                {
                    comState.Key = "need_material";
                }
            }

            if(null == comItem)
            {
                comItem = ComItemManager.Create(goItemParent);
            }

            if(null != comItem)
            {
                if(null != data)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.item.ID);
                    if(null != itemData)
                    {
                        itemData.Count = 1;
                    }
                    comItem.Setup(itemData, (GameObject obj, ItemData item) =>
                    {
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    });
                }
            }

            bool bHasOwned = false;
            if(null != data && null != data.item)
            {
                bHasOwned = TittleBookManager.GetInstance().HasTitle(data.item.ID);
            }
            ownedMark.CustomActive(bHasOwned);

            goCheckMark.CustomActive(null != ms_selected && Value.forgeItem == ms_selected.forgeItem);

            if(null != comItem && null != Name && null != comItem.ItemData)
            {
                Name.text = comItem.ItemData.Name;
            }

            if(null != gray)
            {
                gray.enabled = true;
            }
        }

        public static bool checkMoneyEnough(TitleMergeData data)
        {
            int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(data.getMoneyId());
            if(iOwnedCount >= data.getMoneyCount())
            {
                return true;
            }
            return false;
        }

        public static bool checkMaterialEnough(TitleMergeData data,bool bNeedLink = false)
        {
            if(null != data)
            {
                for(int i = 0; i < data.materials.Count; ++i)
                {
                    int iCount = ItemDataManager.GetInstance().GetOwnedItemCount(data.materials[i].id);
                    if(iCount < data.materials[i].count)
                    {
                        if(bNeedLink)
                        {
                            ItemComeLink.OnLink(data.materials[i].id, data.materials[i].count, true);
                        }
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public void OnDestroy()
        {
            if(null != comItem)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSelectedMergeTitleChanged, _OnSelectedMergeTitleChanged);
        }
    }
}