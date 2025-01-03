using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComItemSettingData
    {
        public int iId;
        public int count;
        public int cost;
    }
    class ComItemSetting : MonoBehaviour
    {
        public GameObject goItemParent;
        public Text Name;
        public Text Process;
        public StateController comProcessMode;
        string processFormatString;
        public void SetProcessFormat(string format)
        {
            this.processFormatString = format;
        }
        public bool bUseTipsForItemClicked;
        public bool bUseForItemComLink;
        ComItem comItem;
        ItemData value;
        int cost;

        public void SetValueByTableData(int iId,int count, int cost)
        {
            value = ItemDataManager.CreateItemDataFromTable(iId);
            if(null != value)
            {
                value.Count = count;
            }
            this.cost = cost;

            _setComItem(value);
            _setName(value);
            _setProcess(value);
        }

        void _setComItem(ItemData value)
        {
            this.value = value;
            if(null == comItem)
            {
                comItem = ComItemManager.Create(goItemParent);
            }

            if(null != comItem)
            {
                if(bUseTipsForItemClicked)
                {
                    comItem.Setup(value, (GameObject obj, ItemData item) =>
                    {
                        if(null != item)
                        {
                            ItemTipManager.GetInstance().ShowTip(item);
                        }
                    });
                }
                else if(bUseForItemComLink)
                {
                    comItem.Setup(value, (GameObject obj, ItemData item) =>
                    {
                        if (null != item)
                        {
                            ItemComeLink.OnLink(item.TableID, this.cost, true);
                        }
                    });
                }
                else
                {
                    comItem.Setup(value,null);
                }
            }
        }
        void _setName(ItemData value)
        {
            if(null != Name)
            {
                if(null != value)
                {
                    Name.text = value.GetColorName();
                }
                else
                {
                    Name.text = string.Empty;
                }
            }
        }
        void _setProcess(ItemData value)
        {
            if(null != Process)
            {
                if(null != value)
                {
                    int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(value.TableID);
                    if (null != comProcessMode)
                    {
                        if (iOwnedCount >= cost)
                        {
                            comProcessMode.Key = "enough";
                            processFormatString = @"<color=#00ff00>{0}</color>/{1}";
                        }
                        else
                        {
                            comProcessMode.Key = "not_enough";
                            processFormatString = @"<color=#ff0000>{0}</color>/{1}";
                        }

                        Process.text = string.Format(processFormatString, iOwnedCount, cost);
                    }
                    else
                    {
                        Process.text = string.Empty;
                    }
                }
                else
                {
                    Process.text = string.Empty;
                }
            }
        }

        public void OnDestroy()
        {
            if(null != comItem)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }
        }
    }
}