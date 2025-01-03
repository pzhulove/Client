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
    public class MapItemFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/MapItemFrame";
        }

        protected override void _OnOpenFrame()
        {
            BindUIEvent();
            //ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            //ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            //ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;

            _InitNearItemScrollListBind();
            //_InitPackageItemList();

            RefreshNearItemListCount();
            //_RefreshPackageItemListCount();

            //_UpdateActorShow();
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();

            //ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            //ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            //ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;

            _ClearData();
        }

        private void _ClearData()
        {
            //mActorShowGeAvatarRender.ClearAvatar();
        }

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateAvatar, OnUpdateAvatar);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NearItemsChanged, _OnNearItemsChanged);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateAvatar, OnUpdateAvatar);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NearItemsChanged, _OnNearItemsChanged);
        }

        void _OnNearItemsChanged(UIEvent iEvent)
        {
            RefreshNearItemListCount();
        }

        void OnUpdateAvatar(UIEvent iEvent)
        {
            // 刷新角色模型
            _UpdateActorShow();
        }

        void _OnAddNewItem(List<Item> items)
        {
            // 捡道具刷新背包
            //_RefreshPackageItemListCount();
        }

        void _OnRemoveItem(ItemData data)
        {       
        }
       
        void _OnUpdateItem(List<Item> items)
        {
            // 穿装备刷新背包
            //_RefreshPackageItemListCount();
        }

        void _InitNearItemScrollListBind()
        {
            mMapItemsScrollView.Initialize();

            mMapItemsScrollView.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateItemScrollListBind(item);
                }
            };

            mMapItemsScrollView.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Button iconBtn = combind.GetCom<Button>("item");
                iconBtn.onClick.RemoveAllListeners();
            };
        }

        void _InitPackageItemList()
        {
            mPackageItemsScrollView.Initialize();

            mPackageItemsScrollView.onBindItem = (obj) =>
            {
                return CreateComItem(obj);
            };

            mPackageItemsScrollView.onItemVisiable = (item) =>
            {
                List<ulong> itemGuids = ItemDataManager.GetInstance().GetPackageItems();

                if (item.m_index >= 0)
                {
                    var comItem = item.gameObjectBindScript as ComItem;
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[item.m_index]);
                    if (itemData != null)
                    {
                        comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                        comItem.Setup(itemData, _OnPackageItemClicked);

                        comItem.SetEnable(true);
                    }
                }
            };
        }

        void _UpdateItemScrollListBind(ComUIListElementScript item)
        {
            //  var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            if (current.MainPlayer == null)
            {
                return;
            }

            // List<BeTownItem> nearitems = current.MainPlayer.FindNearestTownItems();
            List<BeItem> nearitems = current.MainPlayer.FindNearestTownItems();
            if (nearitems == null || item.m_index < 0 || item.m_index >= nearitems.Count)
            {
                return;
            }

            RefreshItemInfo(item, nearitems[item.m_index].ItemID, nearitems[item.m_index].ActorData.GUID);
        }
        
        private void RefreshItemInfo(ComUIListElementScript item,int itemId,UInt64 GUID)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            Text name = combind.GetCom<Text>("name");
            Image icon = combind.GetCom<Image>("icon");
            Button btItem = combind.GetCom<Button>("item");
            Image itemBg = combind.GetCom<Image>("itemBg");

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemId);
            if (itemData == null)
            {
                return;
            }

            if (name != null)
            {
                name.text = itemData.GetColorName();
            }

            if (icon != null)
            {
                ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
            }

            if (itemBg != null)
            {
                ETCImageLoader.LoadSprite(ref itemBg, itemData.GetQualityInfo().Background);
            }

            if (btItem != null)
            {
                btItem.onClick.RemoveAllListeners();
                int itemid = itemId;
                UInt64 guid = GUID;
                btItem.onClick.AddListener(() => { _OnClickNearItem(itemid, guid); });
            }
        }

        public void RefreshNearItemListCount()
        {
            if(mMapItemsScrollView == null)
            {
                return;
            }

            //  var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            if (current.MainPlayer == null)
            {
                return;
            }

            List<BeItem> nearitems = current.MainPlayer.FindNearestTownItems();

            if (nearitems == null)
            {
                return;
            }

            mMapItemsScrollView.SetElementAmount(nearitems.Count);
        }

        void _RefreshPackageItemListCount()
        {
            if(mPackageItemsScrollView == null)
            {
                return;
            }

            List<ulong> itemGuids = ItemDataManager.GetInstance().GetPackageItems();
            if (itemGuids == null)
            {
                return;
            }

            mPackageItemsScrollView.SetElementAmount(itemGuids.Count);
        }

        private void _UpdateActorShow()
        {
            JobTable jobTableData = TableManager.instance.GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobTableData == null)
            {
                Logger.LogErrorFormat("can not find JobTable with id:{0}", PlayerBaseData.GetInstance().JobTableID);
                return;
            }

            ResTable res = TableManager.instance.GetTableItem<ResTable>(jobTableData.Mode);

            if (res == null)
            {
                Logger.LogErrorFormat("can not find ResTable with id:{0}", jobTableData.Mode);
                return;
            }

            mActorShowGeAvatarRender.ClearAvatar();
            mActorShowGeAvatarRender.LoadAvatar(res.ModelPath);

            PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(mActorShowGeAvatarRender);

            mActorShowGeAvatarRender.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
            mActorShowGeAvatarRender.SuitAvatar();
            mActorShowGeAvatarRender.ChangeAction("Anim_Show_Idle", 1f, true);
        }

        void _OnClickNearItem(int itemId, UInt64 guid)
        {
            if(itemId <= 0)
            {
                return;
            }

            //   ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not GameBattle!!!");
                return;
            }

            ChijiDataManager.GetInstance().SendPickUpMapBoxs(guid);
        }

        void _OnPackageItemClicked(GameObject obj, ItemData item)
        {
            if (item == null)
            {
                return;
            }

            List<TipFuncButon> funcs = new List<TipFuncButon>();
            TipFuncButon tempFunc = null;

            // 使用，穿戴
            if (item.UseType == ItemTable.eCanUse.UseOne || item.UseType == ItemTable.eCanUse.UseTotal)
            {
                if (item.IsCooling() == false && !item.isInSidePack)
                {
                    tempFunc = new TipFuncButonSpecial();

                    if (item.PackageType == EPackageType.Equip || item.PackageType == EPackageType.Fashion || item.PackageType == EPackageType.Title || item.PackageType == EPackageType.Bxy)
                    {
                        tempFunc.text = TR.Value("tip_wear");
                        tempFunc.callback = _TryOnUseItem;
                    }
                    else if (item.PackageType == EPackageType.Consumable && item.SubType == (int)ItemTable.eSubType.ChangeName)
                    {
                        tempFunc.text = TR.Value("tip_use");
                        //tempFunc.callback = _OnUseChangeName;
                    }
                    else
                    {
                        tempFunc.text = TR.Value("tip_use");
                        //tempFunc.callback = _TryOnUseItem;
                    }

                    funcs.Add(tempFunc);
                }
            }

            ItemData compareItem = _GetCompareItem(item);
            if (compareItem != null)
            {
                ItemTipManager.GetInstance().ShowTipWithCompareItem(item, compareItem, funcs);
            }
            else
            {
                ItemTipManager.GetInstance().ShowTip(item, funcs, TextAnchor.MiddleLeft);
            }
        }

        void _TryOnUseItem(ItemData item, object data)
        {
            if (item.Type == ItemTable.eType.EQUIP)
            {
                int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                if (iEquipedMasterPriority == 2)
                {
                    SystemNotifyManager.SystemNotifyOkCancel(7019,
                        () =>
                        {
                            _OnUseItem(item, data);
                        },
                        null);
                    return;
                }
            }

            _OnUseItem(item, data);
        }

        void _OnUseItem(ItemData item, object data)
        {
            if (item == null)
            {
                return;
            }

            ItemDataManager.GetInstance().UseItem(item, false);

            if (item.PackageType == EPackageType.Equip || item.PackageType == EPackageType.Fashion)
            {
                AudioManager.instance.PlaySound(102);
            }

            if (item.Count <= 1 || item.CD > 0)
            {
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        ItemData _GetCompareItem(ItemData item)
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

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (null != mActorShowGeAvatarRender)
            {
                while (global::Global.Settings.avatarLightDir.x > 360)
                    global::Global.Settings.avatarLightDir.x -= 360;
                while (global::Global.Settings.avatarLightDir.x < 0)
                    global::Global.Settings.avatarLightDir.x += 360;

                while (global::Global.Settings.avatarLightDir.y > 360)
                    global::Global.Settings.avatarLightDir.y -= 360;
                while (global::Global.Settings.avatarLightDir.y < 0)
                    global::Global.Settings.avatarLightDir.y += 360;

                while (global::Global.Settings.avatarLightDir.z > 360)
                    global::Global.Settings.avatarLightDir.z -= 360;
                while (global::Global.Settings.avatarLightDir.z < 0)
                    global::Global.Settings.avatarLightDir.z += 360;

                mActorShowGeAvatarRender.m_LightRot = global::Global.Settings.avatarLightDir;
            }
        }

		#region ExtraUIBind
		private ComUIListScript mMapItemsScrollView = null;
		private ComUIListScript mPackageItemsScrollView = null;
		private GeAvatarRendererEx mActorShowGeAvatarRender = null;
		private Button mAllPickUp = null;
        private RectTransform mNearitemsRect = null;


        protected override void _bindExUI()
		{
			mMapItemsScrollView = mBind.GetCom<ComUIListScript>("MapItemsScrollView");
			mPackageItemsScrollView = mBind.GetCom<ComUIListScript>("PackageItemsScrollView");
			mActorShowGeAvatarRender = mBind.GetCom<GeAvatarRendererEx>("ActorShowGeAvatarRender");
			mAllPickUp = mBind.GetCom<Button>("PickUp");
			if (null != mAllPickUp)
			{
				mAllPickUp.onClick.AddListener(_onAllPickUpButtonClick);
			}
            mNearitemsRect = mBind.GetCom<RectTransform>("nearitemsRect");
		}
		
		protected override void _unbindExUI()
		{
			mMapItemsScrollView = null;
			mPackageItemsScrollView = null;
			mActorShowGeAvatarRender = null;
			if (null != mAllPickUp)
			{
				mAllPickUp.onClick.RemoveListener(_onAllPickUpButtonClick);
			}
			mAllPickUp = null;
            mNearitemsRect = null;
		}
        #endregion

        #region Callback
        private void _onAllPickUpButtonClick()
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            if (current.MainPlayer == null)
            {
                return;
            }

            List<BeItem> nearitems = current.MainPlayer.FindNearestTownItems();
            if (nearitems != null)
            {
                for (int i = 0; i < nearitems.Count; i++)
                {
                    ChijiDataManager.GetInstance().SendPickUpMapBoxs(nearitems[i].ActorData.GUID);
                }
            }
        }
        #endregion
    }
}
