using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Protocol;
using Network;

namespace GameClient
{
    class GuildStoreHouseClearItemData
    {
        public ItemData itemData;
        public bool bClear;
    }

    class GuildStoreHouseClearItem : CachedNormalObject<GuildStoreHouseClearItemData>
    {
        ComGuildItem comGuildItem;
        public override void Initialize()
        {
            comGuildItem = goLocal.GetComponent<ComGuildItem>();
            if (null != comGuildItem)
            {
                comGuildItem.InitComGuildItem((GameObject obj, ItemData item) =>
                {
                    if (null != item)
                    {
                        ItemTipManager.GetInstance().ShowTip(item);
                    }
                }, 
                true,
                Value.bClear);
            }
        }

        public override void UnInitialize()
        {
            comGuildItem = null;
        }

        public override void OnUpdate()
        {
            if (null != comGuildItem)
            {
                comGuildItem.SetItemData(Value.itemData);
            }
        }
    }

    class GuildHouseItemData
    {
        public ItemData itemData;
    }

    class GuildHouseItem : CachedNormalObject<GuildHouseItemData>
    {
        GameObject goBack;
        GameObject goItemParent;
        ComItem comItem;

        public override void Initialize()
        {
            goBack = Utility.FindChild(goLocal,"BG");
            goItemParent = Utility.FindChild(goLocal, "ItemParent");
        }

        public override void OnUpdate()
        {
            if(null != Value)
            {
                if(null != Value.itemData)
                {
                    goBack.CustomActive(false);
                    goItemParent.CustomActive(true);

                    if (null == comItem)
                    {
                        comItem = ComItemManager.Create(goItemParent);
                    }

                    if(null != comItem)
                    {
                        comItem.Setup(Value.itemData,(GameObject obj, ItemData item)=>
                        {
                            if(null != item)
                            {
                                if (item.Type == ProtoTable.ItemTable.eType.EQUIP)
                                {
                                    WorldWatchGuildStorageItemReq kSend = new WorldWatchGuildStorageItemReq();
                                    kSend.uid = item.GUID;

                                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
                                }
                                else
                                {
                                    ItemTipManager.GetInstance().ShowTip(item);
                                }
                            }
                        });
                    }
                }
                else
                {
                    goBack.CustomActive(true);
                    goItemParent.CustomActive(false);
                }
            }
        }

        public override void UnInitialize()
        {
            goBack = null;
            goItemParent = null;
            if(null != comItem)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }
        }
    }
}