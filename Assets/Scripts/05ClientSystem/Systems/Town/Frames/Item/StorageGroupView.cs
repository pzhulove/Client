using GameClient;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StorageGroupView : MonoBehaviour
    {
        [SerializeField] private Text mLabTitle;
        [SerializeField] private GameObject m_objContentLeft;
        [SerializeField] private GameObject mObjContentRight;
        [SerializeField] private HelpAssistant mComHelp;
        [SerializeField] private string mStoragePackagePath;

        [SerializeField] private Toggle roleStorageToggle = null;
        [SerializeField] private Toggle accountStorageToggle = null;
        [SerializeField] private GameObject roleStorageViewRoot;
        [SerializeField] private GameObject accountStorageViewRoot;



        //当前的页签类型
        private StorageType _currentStorageType;
        public StorageType CurrentStorageType
        {
            get { return _currentStorageType; }
            set
            {
                _currentStorageType = value;
                StorageDataManager.GetInstance().CurrentStorageType = _currentStorageType;
            }
        }

        private bool _inited = false;

        private RoleStorageView _roleStorageView;
        private AccountStorageView _accountStorageView;
        private StorageGroupFrame mStorageGroupFrame;
        private StoragePackageView mStoragePackageView;

        private ItemGroupData mItemGroupData;

        public void Init(ItemGroupData itemGroupData, StorageGroupFrame storageGroupFrame)
        {
            if (!_inited)
            {
                mStorageGroupFrame = storageGroupFrame;

                if (roleStorageToggle != null)
                {
                    roleStorageToggle.onValueChanged.RemoveAllListeners();
                    roleStorageToggle.onValueChanged.AddListener(OnRoleStorageToggleClick);
                }

                if (accountStorageToggle != null)
                {
                    accountStorageToggle.onValueChanged.RemoveAllListeners();
                    accountStorageToggle.onValueChanged.AddListener(OnAccountStorageToggleClick);
                }
            }

            mItemGroupData = itemGroupData;
            
            mLabTitle.text = TR.Value("storage_title");
            mComHelp.eType = HelpFrameContentTable.eHelpType.HT_STORAGE;

            if (itemGroupData != null && itemGroupData.openDecompose && mStoragePackageView != null)
            {
                mStoragePackageView.OpenQuickDecompose();
            }

            CurrentStorageType = StorageType.RoleStorage;
            InitStorageView();
            UpdateStoragePackageView();
        }

        public void UpdateStoragePackageView()
        {
            if (mStoragePackageView != null)
            {
                if (mItemGroupData != null)
                {
                    mStoragePackageView.EnableView(mItemGroupData.ePackageType);
                }
            }
            else
            {
                UIManager.GetInstance().LoadObject(mStorageGroupFrame, mStoragePackagePath, null, _LoadPackageViewCallback, typeof(GameObject));
            }
        }

        void OnDestroy()
        {
            _roleStorageView = null;
            _accountStorageView = null;

            if (roleStorageToggle != null)
            {
                roleStorageToggle.onValueChanged.RemoveAllListeners();
                roleStorageToggle = null;
            }

            if (accountStorageToggle != null)
            {
                accountStorageToggle.onValueChanged.RemoveAllListeners();
                accountStorageToggle = null;
            }
        }

        private void _LoadPackageViewCallback(string path, object asset, object userData)
        {
            GameObject go = asset as GameObject;
            if (go == null)
            {
                return;
            }

            go.transform.SetParent(mObjContentRight.transform, false);
            mStoragePackageView = go.GetComponent<StoragePackageView>();

            if (mStoragePackageView != null)
            {
                if (mItemGroupData != null)
                {
                    mStoragePackageView.Init(mStorageGroupFrame ,mItemGroupData.ePackageType);
                }
            }
        }

        private void InitStorageView()
        {
            InitStorageToggle();
            OnUpdateStorageView();
        }

        private void InitStorageToggle()
        {
            if (CurrentStorageType == StorageType.RoleStorage)
            {
                if (roleStorageToggle != null)
                {
                    roleStorageToggle.isOn = false;
                    roleStorageToggle.isOn = true;
                }
            }
            else
            {
                if (accountStorageToggle != null)
                {
                    accountStorageToggle.isOn = false;
                    accountStorageToggle.isOn = true;
                }
            }
        }


        private void OnUpdateStorageView()
        {
            if (CurrentStorageType == StorageType.AccountStorage)
            {
                CommonUtility.UpdateGameObjectVisible(roleStorageViewRoot, false);
                CommonUtility.UpdateGameObjectVisible(accountStorageViewRoot, true);

                if (_accountStorageView == null)
                {
                    if (accountStorageViewRoot != null)
                    {
                        var accountStorageViewPrefab = CommonUtility.LoadGameObject(accountStorageViewRoot);
                        if (accountStorageViewPrefab != null)
                            _accountStorageView =
                                accountStorageViewPrefab.GetComponent<AccountStorageView>();

                        if (_accountStorageView != null)
                            _accountStorageView.InitView(CurrentStorageType, mStorageGroupFrame);
                    }
                }
                else
                {
                    _accountStorageView.OnEnableView();
                }
            }
            else
            {

                CommonUtility.UpdateGameObjectVisible(roleStorageViewRoot, true);
                CommonUtility.UpdateGameObjectVisible(accountStorageViewRoot, false);

                if (_roleStorageView == null)
                {
                    if (roleStorageViewRoot != null)
                    {
                        var roleStorageViewPrefab = CommonUtility.LoadGameObject(roleStorageViewRoot);
                        if (roleStorageViewPrefab != null)
                            _roleStorageView = roleStorageViewPrefab.GetComponent<RoleStorageView>();
                        if (_roleStorageView != null)
                            _roleStorageView.InitView(CurrentStorageType, mStorageGroupFrame);
                    }
                }
                else
                {
                    _roleStorageView.OnEnableView();
                }
            }

            if (mStoragePackageView != null)
            {
                mStoragePackageView.UpdateItemList();
            }
        }

        #region BindEvent
        private void OnRoleStorageToggleClick(bool value)
        {
            if (value == false)
                return;

            if (CurrentStorageType == StorageType.RoleStorage)
                return;
            CurrentStorageType = StorageType.RoleStorage;

            OnUpdateStorageView();
        }

        private void OnAccountStorageToggleClick(bool value)
        {
            if (value == false)
                return;

            if (CurrentStorageType == StorageType.AccountStorage)
                return;
            CurrentStorageType = StorageType.AccountStorage;

            OnUpdateStorageView();
        }
        #endregion

        public void UpdateItemList(UIEvent a_event)
        {
            if (mStoragePackageView != null)
            {
                mStoragePackageView.UpdateItemList(a_event);
            }
        }

        public void ItemSellSuccess(UIEvent a_event)
        {
            if (mStoragePackageView != null)
            {
                mStoragePackageView.ItemSellSuccess(a_event);
            }
        }

        public void ItemDecomposeFinished(UIEvent a_event)
        {
            if (mStoragePackageView != null)
            {
                mStoragePackageView.ItemDecomposeFinished(a_event);
            }
        }

        public void UpdateItemListByUiEvent(UIEvent uiEvent)
        {
            if (CurrentStorageType == StorageType.RoleStorage)
            {
                if (_roleStorageView != null)
                {
                    _roleStorageView.UpdateItemListByUiEvent(uiEvent);
                }
            }
            else if (CurrentStorageType == StorageType.AccountStorage)
            {
                if (_accountStorageView != null)
                {
                    _accountStorageView.UpdateItemListByUiEvent(uiEvent);
                }
            }
        }

        public void ReceiveStorageChangeNameMessages(UIEvent uiEvent)
        {
            if (CurrentStorageType == StorageType.RoleStorage)
            {
                if (_roleStorageView != null)
                {
                    _roleStorageView.ReceiveRoleStorageChangeNameMessages(uiEvent);
                }
            }
            else if (CurrentStorageType == StorageType.AccountStorage)
            {
                if (_accountStorageView != null)
                {
                    _accountStorageView.ReceiveAccountStorageChangeNameMessages(uiEvent);
                }
            }
        }

        public void ReceiveStorageUnlockMessage(UIEvent uiEvent)
        {
            if (CurrentStorageType == StorageType.RoleStorage)
            {
                if (_roleStorageView != null)
                {
                    _roleStorageView.ReceiveRoleStorageUnlockMessage(uiEvent);
                }
            }
            else if (CurrentStorageType == StorageType.AccountStorage)
            {
                if (_accountStorageView != null)
                {
                    _accountStorageView.ReceiveAccountStorageUnlockMessage(uiEvent);
                }
            }
        }
    }
}
