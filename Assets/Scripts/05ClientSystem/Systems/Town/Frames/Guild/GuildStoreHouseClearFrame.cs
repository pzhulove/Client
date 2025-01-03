using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Protocol;
///////删除linq

namespace GameClient
{
    class GuildStoreHouseClearFrameData
    {
        public string name;
        public string hint;
        public string btnName;
        public UnityAction onOK;
        public UnityAction onCancel;
        public bool bClear;
    }

    class GuildStoreHouseClearFrame : ClientFrame 
	{
		public override string GetPrefabPath ()
		{
			return "UIFlatten/Prefabs/Guild/GuildStoreHouseClearFrame";
		}

		[UIEventHandleAttribute("Close")]
		void _OnClickCloseFrame()
		{
			frameMgr.CloseFrame (this);
		}

		[UIObjectAttribute("middleback/Goods/ScrollView/Viewport/Content")]
		GameObject goParent;
		[UIObjectAttribute("middleback/Goods/ScrollView/Viewport/Content/Prefab")]
		GameObject goPrefab;
        [UIControl("Name", typeof(Text))]
        Text Name;
        [UIControl("Hint", typeof(Text))]
        Text Hint;
        [UIControl("Space", typeof(Text))]
        Text Space;
        [UIControl("middleback/BtnOK/Text", typeof(Text))]
        Text okText;
        [UIObject("Mask")]
        GameObject goMask;
        [UIControl("middleback", typeof(StateController))]
        StateController comState;

        CachedObjectListManager<GuildStoreHouseClearItem> m_akItemDatas = new CachedObjectListManager<GuildStoreHouseClearItem>();

        GuildStoreHouseClearFrameData data = null;

        public static GuildStoreHouseClearFrameData ReadyStoreRemoveData()
        {
            GuildStoreHouseClearFrameData data = new GuildStoreHouseClearFrameData();
            data.name = TR.Value("guild_store_house_clear_name_0");
            data.hint = TR.Value("guild_store_house_hint_name_0");
            data.btnName = TR.Value("guild_store_house_ok_name_0");
            data.bClear = true;

            data.onOK = () =>
            {
                if (ComGuildItem.PoolItems.Count <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_clear_need_selected_item"));
                    return;
                }

                //if has no power then
                EGuildDuty eEGuildDuty = PlayerBaseData.GetInstance().eGuildDuty;
                GuildPost ePost = (GuildPost)GuildDataManager.GetInstance().GetServerDuty(eEGuildDuty);
                if (ePost < GuildDataManager.GetInstance().clearPower)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_clear_need_power"));
                    return;
                }

                SystemNotifyManager.SystemNotify(7018,
                    () =>
                    {
                    },
                    () =>
                    {
                        List<GuildStorageDelItemInfo> items = new List<GuildStorageDelItemInfo>();
                        for (int i = 0; i < ComGuildItem.PoolItems.Count; ++i)
                        {
                            if (null != ComGuildItem.PoolItems[i])
                            {
                                var poolItem = ComGuildItem.PoolItems[i].Value;
                                if (null != poolItem && null != poolItem.itemData && poolItem.iSelectedCount > 0)
                                {
                                    items.Add(new GuildStorageDelItemInfo
                                    {
                                        uid = poolItem.itemData.GUID,
                                        num = (ushort)poolItem.iSelectedCount,
                                    });
                                }
                            }
                        }

                        if(items.Count > 0)
                        {
                            GuildDataManager.GetInstance().SendDeleteStorageItems(items.ToArray());
                        }
                    });
            };

            return data;
        }
        public static GuildStoreHouseClearFrameData ReadyStoreAddData()
        {
            GuildStoreHouseClearFrameData data = new GuildStoreHouseClearFrameData();
            data.name = TR.Value("guild_store_house_clear_name_1");
            data.hint = TR.Value("guild_store_house_hint_name_1");
            data.btnName = TR.Value("guild_store_house_ok_name_1");
            data.bClear = false;

            data.onOK = () =>
            {
                if (ComGuildItem.PoolItems.Count <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_store_need_selected_item"));
                    return;
                }

                //if has no power then
                EGuildDuty eEGuildDuty = PlayerBaseData.GetInstance().eGuildDuty;
                GuildPost ePost = (GuildPost)GuildDataManager.GetInstance().GetServerDuty(eEGuildDuty);
                if (ePost < GuildDataManager.GetInstance().contributePower)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_store_need_power"));
                    return;
                }

                //if has no enough space then return
                int iUsedSpace = ComGuildItem.PoolItems.Count + GuildDataManager.GetInstance().storeDatas.Count;
                int iCapacity = GuildDataManager.GetInstance().storeageCapacity;
                if (iUsedSpace > iCapacity)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_store_has_no_space"));
                    return;
                }

                List<GuildStorageItemInfo> items = new List<GuildStorageItemInfo>();
                for(int i = 0; i < ComGuildItem.PoolItems.Count;++i)
                {
                    if(null != ComGuildItem.PoolItems[i])
                    {
                        var poolItem = ComGuildItem.PoolItems[i].Value;
                        if(null != poolItem && null != poolItem.itemData && poolItem.iSelectedCount > 0)
                        {
                            items.Add(new GuildStorageItemInfo
                            {
                                uid = poolItem.itemData.GUID,
                                dataId = (uint)poolItem.itemData.TableID,
                                num = (ushort)poolItem.iSelectedCount,
                            });
                        }
                    }
                }

                if(items.Count > 0)
                {
                    GuildDataManager.GetInstance().SendStoreItems(items.ToArray());
                }
            };

            return data;
        }

        public static void CommandOpen(object argv = null)
		{
			if(argv == null)
			{
                argv = ReadyStoreAddData();
			}

            ClientSystemManager.GetInstance().CloseFrame<GuildStoreHouseClearFrame>();
            ClientSystemManager.GetInstance().OpenFrame<GuildStoreHouseClearFrame>(FrameLayer.Middle, argv);
		}

		protected override void _OnOpenFrame()
		{
            data = userData as GuildStoreHouseClearFrameData;
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemAdd, _OnStorageItemAdd);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemRemoved, _OnStorageRemoved);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemUpdate, _OnStorageItemUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemStoreRet, _OnGuildHouseItemStoreRet);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemDeleteRet, _OnGuildHouseItemDeleteRet);

            goMask.CustomActive(false);
            _RefreshStoreHouseClearItems();
            _UpdateStatus();
        }

        void _OnAddNewItem(List<Item> items)
        {
            if(data.bClear)
            {
                return;
            }

            for(int i = 0; i < items.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if(itemData == null)
                    continue;
                //在未启用的装备方案中
                if(itemData.IsItemInUnUsedEquipPlan == true)
                    continue;

                var find = m_akItemDatas.Find(x =>
                {
                    return null != x &&
                           null != x.Value &&
                           null != x.Value.itemData &&
                           itemData.GUID == x.Value.itemData.GUID;
                });

                if (null == find)
                {
                    m_akItemDatas.Create(new object[] {
                        goParent,
                        goPrefab,
                        new GuildStoreHouseClearItemData
                        {
                            itemData = itemData,
                            bClear = data.bClear,
                        },
                        false,
                    });
                }
                
            }

            _UpdateStatus();
        }

        void _OnUpdateItem(List<Item> items)
        {
            if (data.bClear)
            {
                return;
            }

            for (int i = 0; i < items.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (null != itemData)
                {
                    var find = m_akItemDatas.Find(x =>
                    {
                        return null != x &&
                        null != x.Value &&
                        null != x.Value.itemData &&
                        itemData.GUID == x.Value.itemData.GUID;
                    });

                    if (null != find)
                    {
                        find.OnRefresh(new object[]
                        {
                            new GuildStoreHouseClearItemData
                            {
                                itemData = itemData,
                                bClear = data.bClear,
                            }
                        });
                    }
                }
            }

            _UpdateStatus();
        }

        void _OnRemoveItem(ItemData itemData)
        {
            if (data.bClear)
            {
                return;
            }

            if(null != itemData)
            {
                m_akItemDatas.Recycle(x =>
                {
                    return null != x &&
                    null != x.Value &&
                    null != x.Value.itemData &&
                    itemData.GUID == x.Value.itemData.GUID;
                });
            }

            _UpdateStatus();
        }

        void _OnStorageItemAdd(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            if (data.bClear && null != itemData)
            {
                //在未启用的装备方案中
                if (itemData.IsItemInUnUsedEquipPlan == true)
                {
                    //不添加
                }
                else
                {
                    //不在未启用的装备方案中,添加
                    m_akItemDatas.Create(new object[] {
                        goParent,
                        goPrefab,
                        new GuildStoreHouseClearItemData
                        {
                            itemData = itemData,
                            bClear = data.bClear,
                        },
                        false,
                    });
                }
            }
            _UpdateStatus();
            _UpdateSpace();
        }

        void _OnStorageRemoved(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            if (data.bClear && null != itemData)
            {
                m_akItemDatas.Recycle(x =>
                {
                    return null != x &&
                    null != x.Value &&
                    null != x.Value.itemData &&
                    itemData.GUID == x.Value.itemData.GUID;
                });
            }
            _UpdateStatus();
            _UpdateSpace();
        }

        void _OnStorageItemUpdate(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            if (data.bClear && null != itemData)
            {
                var find = m_akItemDatas.Find(x =>
                {
                    return null != x &&
                    null != x.Value &&
                    null != x.Value.itemData &&
                    itemData.GUID == x.Value.itemData.GUID;
                });

                if (null != find)
                {
                    find.OnRefresh(new object[]
                    {
                            new GuildStoreHouseClearItemData
                            {
                                itemData = itemData,
                                bClear = data.bClear
                            }
                    });
                }
            }
            _UpdateStatus();
            _UpdateSpace();
        }

        void _OnGuildHouseItemStoreRet(UIEvent uiEvent)
        {
            if (!data.bClear)
            {
                ComGuildItem.CancelSelectedItems();
                ComGuildItem.ClearPools();
                goMask.CustomActive(false);
            }
        }

        void _OnGuildHouseItemDeleteRet(UIEvent uiEvent)
        {
            if(data.bClear)
            {
                ComGuildItem.CancelSelectedItems();
                ComGuildItem.ClearPools();
                goMask.CustomActive(false);
            }
        }

        protected override void _OnCloseFrame()
		{
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemAdd, _OnStorageItemAdd);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemRemoved, _OnStorageRemoved);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemUpdate, _OnStorageItemUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemStoreRet, _OnGuildHouseItemStoreRet);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemDeleteRet, _OnGuildHouseItemDeleteRet);
            data.onCancel = null;
            data.onOK = null;
            data = null;
			ComGuildItem.ClearPools();
			m_akItemDatas.DestroyAllObjects();
            InvokeMethod.RemoveInvokeCall(this);
        }

        void _UpdateStatus()
        {
            if(null != comState)
            {
                if(m_akItemDatas.ActiveObjects.Count > 0)
                {
                    comState.Key = "normal";
                }
                else
                {
                    if (data.bClear)
                    {
                        comState.Key = "no_clear";
                    }
                    else
                    {
                        comState.Key = "no_items";
                    }
                }
            }
        }

		void _RefreshStoreHouseClearItems()
		{
			m_akItemDatas.RecycleAllObject();

            if(data.bClear)
            {
                var datas = GuildDataManager.GetInstance().storeDatas;
                for (int i = 0; i < datas.Count; ++i)
                {
                    var itemData = datas[i];
                    if(itemData == null)
                        continue;

                    //在未启用的装备方案中
                    if(itemData.IsItemInUnUsedEquipPlan == true)
                        continue;

                    m_akItemDatas.Create(new object[] {
                        goParent,
                        goPrefab,
                        new GuildStoreHouseClearItemData
                        {
                            itemData = itemData,
                            bClear = data.bClear,
                        },
                        false,
                    });
                }
            }
            else
            {
                List<object> datas = GamePool.ListPool<object>.Get();
                var enumerator = ItemDataManager.GetInstance().GetAllPackageItems().GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var itemData = enumerator.Current.Value;
                    if (null == itemData)
                    {
                        continue;
                    }

                    if (itemData.EquipType == EEquipType.ET_REDMARK)
                    {
                        continue;
                    }

                    if (itemData.StrengthenLevel >= 11)
                    {
                        continue;
                    }

                    if(itemData.Quality > ProtoTable.ItemTable.eColor.PURPLE)
                    {
                        continue;
                    }

                    if(itemData.PackageType != EPackageType.Equip &&
                        itemData.PackageType != EPackageType.Material &&
                        itemData.PackageType != EPackageType.Consumable)
                    {
                        continue;
                    }

                    if (itemData.Type == ProtoTable.ItemTable.eType.EQUIP && itemData.BEquipIsInsetBead)
                    {
                        continue;
                    }

                    if (itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                        itemData.SubType == (int)ProtoTable.ItemTable.eSubType.Bead &&
                        itemData.Quality >= ProtoTable.ItemTable.eColor.PURPLE)
                    {
                        continue;
                    }

                    if (itemData.Type == ProtoTable.ItemTable.eType.EQUIP)
                    {
                        if (itemData.BindAttr == ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            datas.Add(itemData);
                        }
                        else
                        {
                            if (itemData.Packing)
                            {
                                datas.Add(itemData);
                            }
                        }
                        continue;
                    }

                    if (itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE)
                    {
                        if (itemData.BindAttr == ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            datas.Add(itemData);
                        }
                        else
                        {
                            if (itemData.Packing)
                            {
                                datas.Add(itemData);
                            }
                        }
                        continue;
                    }

                    if (itemData.Type == ProtoTable.ItemTable.eType.MATERIAL)
                    {
                        if (itemData.BindAttr == ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            datas.Add(itemData);
                        }
                        else
                        {
                            if (itemData.Packing)
                            {
                                datas.Add(itemData);
                            }
                        }
                        continue;
                    }
                }

                for(int i = 0; i < datas.Count; ++i)
                {
                    var itemData = datas[i] as ItemData;
                    if(itemData == null)
                        continue;

                    //在未启用的装备方案中
                    if (itemData.IsItemInUnUsedEquipPlan == true)
                        continue;

                    m_akItemDatas.Create(new object[] {
                        goParent,
                        goPrefab,
                        new GuildStoreHouseClearItemData
                        {
                            itemData = itemData,
                            bClear = data.bClear,
                        },
                        false,
                    });
                }

                GamePool.ListPool<object>.Release(datas);
            }

            if (null != data)
            {
                if (null != Name.text)
                    Name.text = data.name;
                if(null != Hint)
                    Hint.text = data.hint;
                if(null != okText)
                {
                    okText.text = data.btnName;
                }
            }

            _UpdateSpace();
        }

        void _UpdateSpace()
        {
            if (null != data)
            {
                if (null != Space)
                {
                    Space.CustomActive(!data.bClear);
                    int iUsedSpace = GuildDataManager.GetInstance().storeDatas.Count;
                    int iTotalSpace = GuildDataManager.GetInstance().storeageCapacity;
                    Space.text = TR.Value("guild_store_house_space_desc", iUsedSpace, iTotalSpace);
                }
            }
        }

		[UIEventHandleAttribute("middleback/BtnOK")]
		void _OnClickOK()
		{
            if(null != data && null != data.onOK)
            {
                InvokeMethod.RemoveInvokeCall(this);
                goMask.CustomActive(true);
                data.onOK.Invoke();
                InvokeMethod.Invoke(this, 2.0f, () =>
                {
                    goMask.CustomActive(false);
                });
            }
		}
	}
}