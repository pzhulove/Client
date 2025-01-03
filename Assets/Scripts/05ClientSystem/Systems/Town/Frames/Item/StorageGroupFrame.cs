using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class ItemGroupData
    {
        public bool isPackage;
        public EPackageType ePackageType;
		public bool openDecompose;
    }

    // 仓库主界面
	public class StorageGroupFrame : ClientFrame
	{
        private StorageGroupView mStorageGroupView;

        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                string[] arrParams = strParam.Split('|');
                if (arrParams.Length < 2)
                {
                    return;
                }
                ItemGroupData data = new ItemGroupData();
                data.isPackage = int.Parse(arrParams[0]) == 1;
                data.ePackageType = (EPackageType)(int.Parse(arrParams[1]));
				if (arrParams.Length == 3)
				{
					data.openDecompose = (int.Parse(arrParams[2]) == 1);
				}
                ClientSystemManager.GetInstance().OpenFrame<StorageGroupFrame>(FrameLayer.Middle, data);
            }
            catch (System.Exception e)
            {
                Logger.LogError("PackageFrame.OpenLinkFrame : ==>" + e.ToString());
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/StorageGroupFrame";
        }

        protected override void _OnOpenFrame()
		{
            ItemGroupData groupData = userData as ItemGroupData;
            if (groupData == null)
            {
                groupData = new ItemGroupData();
                groupData.isPackage = true;
                groupData.ePackageType = EPackageType.Equip;
            }

            if (mStorageGroupView != null)
            {
                mStorageGroupView.Init(groupData, this);
            }

            _RegisterUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);
        }

        protected override void _bindExUI()
        {
            mStorageGroupView = mBind.GetCom<StorageGroupView>("StorageGroupFrame");
        }

        protected override void _unbindExUI()
        {
            mStorageGroupView = null;
        }

        protected override void _OnCloseFrame()
		{
            _UnRegisterUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);
        }

        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemSellSuccess, _OnItemSellSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemQualityChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DecomposeFinished, _OnItemDecomposeFinished);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnUpdateItemList);

            //存取道具
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemListByUiEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemListByUiEvent);

            //名字修改
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveStorageChangeNameMessage, _OnReceiveStorageChangeNameMessages);
            //某个仓库解锁
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveStorageUnlockMessage, _OnReceiveStorageUnlockMessage);
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemSellSuccess, _OnItemSellSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemQualityChanged, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DecomposeFinished, _OnItemDecomposeFinished);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnUpdateItemList);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemListByUiEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemListByUiEvent);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveStorageChangeNameMessage, _OnReceiveStorageChangeNameMessages);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveStorageUnlockMessage, _OnReceiveStorageUnlockMessage);
        }

        #region UIEvent
        void _OnUpdateItemList(UIEvent a_event)
        {
            if (mStorageGroupView != null)
            {
                mStorageGroupView.UpdateItemList(a_event);
            }
        }

        void _OnItemSellSuccess(UIEvent a_event)
        {
            if (mStorageGroupView != null)
            {
                mStorageGroupView.ItemSellSuccess(a_event);
            }
        }

        void _OnItemDecomposeFinished(UIEvent a_event)
        {
            if (mStorageGroupView != null)
            {
                mStorageGroupView.ItemDecomposeFinished(a_event);
            }
        }

        void _OnUpdateItemListByUiEvent(UIEvent uiEvent)
        {
            if (mStorageGroupView != null)
            {
                mStorageGroupView.UpdateItemListByUiEvent(uiEvent);
            }
        }

        //改名成功
        private void _OnReceiveStorageChangeNameMessages(UIEvent uiEvent)
        {
            if (mStorageGroupView != null)
            {
                mStorageGroupView.ReceiveStorageChangeNameMessages(uiEvent);
            }
        }

        //最新的一个解锁
        private void _OnReceiveStorageUnlockMessage(UIEvent uiEvent)
        {
            if (mStorageGroupView != null)
            {
                mStorageGroupView.ReceiveStorageUnlockMessage(uiEvent);
            }
        }
        #endregion

        public void ChangeStorageName(int index, string name, StorageType storageType = StorageType.RoleStorage)
        {
            SceneSettingDataManager.GetInstance().OnSendSceneShortcutKeySetReq(index,
                name, storageType);
        }
    }
}
