using System;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AccountStorageView : MonoBehaviour
    {

        private StorageType _storageType;
        private List<StorageItemDataModel> _storageItemDataModelList;
        private StorageSelectView _accountStorageSelectView;

        private int _accountStorageCurrentSelectedIndex = 1;
        private int _accountStorageOwnerStorageNumber = 1;

        private StorageGroupFrame _storageGroupFrame;
        private bool _inited = false;

        [Space(10)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private ComButtonWithCd arrangeButtonWithCd;
        [SerializeField] private Button addGridButton;

        [Space(10)] [HeaderAttribute("GridValue")] [Space(10)]
        [SerializeField] private Text gridValueLabel;

        [Space(10)]
        [HeaderAttribute("ComUIList")]
        [Space(10)]
        [SerializeField] private ComUIListScriptEx storageItemListEx;

        [SerializeField] private Text accountStorageSelectNameLabel;
        [SerializeField] private Button accountStorageSelectButton;
        [SerializeField] private GameObject accountStorageSelectViewRoot;
        [SerializeField] private Button changeNameButton;

        [Space(10)]
        [HeaderAttribute("Page")]
        [Space(10)]
        [SerializeField] private Button nextPageButton;
        [SerializeField] private UIGray nextPageGray;
        [SerializeField] private Button prePageButton;
        [SerializeField] private UIGray prePageGray;
        [SerializeField] private Text pageValueLabel;

        private void OnDestroy()
        {
            UnBindUiEvents();
        }

        private void BindUiEvents()
        {
            if (storageItemListEx != null)
            {
                storageItemListEx.Initialize();
                storageItemListEx.onItemVisiable += OnItemVisible;
                storageItemListEx.OnItemRecycle += OnItemRecycle;
            }

            if (arrangeButtonWithCd != null)
            {
                arrangeButtonWithCd.ResetButtonListener();
                arrangeButtonWithCd.SetButtonListener(OnArrangeButtonClick);
            }

            if (addGridButton != null)
            {
                addGridButton.onClick.RemoveAllListeners();
                addGridButton.onClick.AddListener(OnAddGridButtonClick);
            }

            if (accountStorageSelectButton != null)
            {
                accountStorageSelectButton.onClick.RemoveAllListeners();
                accountStorageSelectButton.onClick.AddListener(OnAccountStorageSelectButtonClick);
            }

            if (changeNameButton != null)
            {
                changeNameButton.onClick.RemoveAllListeners();
                changeNameButton.onClick.AddListener(_OnChangeNameButtonClick);
            }

            if (nextPageButton != null)
            {
                nextPageButton.onClick.RemoveAllListeners();
                nextPageButton.onClick.AddListener(_OnNextPageButtonClick);
            }

            if (prePageButton != null)
            {
                prePageButton.onClick.RemoveAllListeners();
                prePageButton.onClick.AddListener(_OnPrePageButtonClick);
            }
        }

        private void UnBindUiEvents()
        {

            if (storageItemListEx != null)
            {
                storageItemListEx.onItemVisiable -= OnItemVisible;
                storageItemListEx.OnItemRecycle -= OnItemRecycle;
                storageItemListEx.UnInitialize();
            }

            if (arrangeButtonWithCd != null)
                arrangeButtonWithCd.ResetButtonListener();

            if (addGridButton != null)
                addGridButton.onClick.RemoveAllListeners();

            if (changeNameButton != null)
                changeNameButton.onClick.RemoveAllListeners();

            if (nextPageButton != null)
                nextPageButton.onClick.RemoveAllListeners();

            if (prePageButton != null)
                prePageButton.onClick.RemoveAllListeners();
        }

        public void InitView(StorageType storageType, StorageGroupFrame storageGroupFrame)
        {
            if (!_inited)
            {
                BindUiEvents();
                _storageGroupFrame = storageGroupFrame;

                _inited = true;
            }

            _storageType = storageType;
            ItemDataManager.GetInstance().ArrangeItems(EPackageType.Storage);

            _accountStorageCurrentSelectedIndex = StorageDataManager.GetInstance().GetAccountStorageCurrentSelectedIndex();
            _accountStorageOwnerStorageNumber = StorageDataManager.GetInstance().GetAccountStorageOwnerStorageNumber();

            _UpdateContent();

            ResetArrangeButton();
        }

        public void OnEnableView()
        {
            ResetStorageItemList();
            _UpdateContent();
            ResetArrangeButton();
        }

        #region Update

        private void _UpdateSelectView(int preIndex, int nextIndex)
        {
            if (_accountStorageSelectView != null && _accountStorageSelectView.gameObject.activeInHierarchy)
            {
                _accountStorageSelectView.OnUpdateStorageSelect(preIndex, nextIndex);
            }
        }

        private void _UpdateContent()
        {
            _UpdateRoleStoragePageContent();
            _UpdateAccountStorageSelectStorageName();
            UpdateStorageItemList();
        }

        //重置ArrangeButton
        private void ResetArrangeButton()
        {
            if (arrangeButtonWithCd != null)
                arrangeButtonWithCd.Reset();
        }

        //更新选择仓库的名字
        private void _UpdateAccountStorageSelectStorageName()
        {
            if (accountStorageSelectNameLabel == null)
                return;

            var selectNameStr = StorageUtility.GetStorageNameByStorageIndex(_accountStorageCurrentSelectedIndex, EPackageType.Storage);
            accountStorageSelectNameLabel.text = selectNameStr;
        }

        //更新StorageItemList
        void UpdateStorageItemList()
        {
            //更新格子的数值
            UpdateGridInfo();

            if (storageItemListEx == null)
                return;

            _storageItemDataModelList = StorageUtility.GetStorageItemDataModelList(_storageType,
                _accountStorageCurrentSelectedIndex);
            int count = 0;
            if (_storageItemDataModelList != null)
                count = _storageItemDataModelList.Count;

            //_storageItemDataModelList = StorageUtility.GetStorageItemDataModelList(_storageType);
            //int count = 0;
            //if (_storageItemDataModelList != null)
            //    count = _storageItemDataModelList.Count;

            storageItemListEx.SetElementAmount(count);
        }

        private void UpdateGridInfo()
        {
            if (gridValueLabel == null)
                return;

            var itemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Storage);
            var accountStorageItemNumber = 0;
            if (itemGuidList != null && itemGuidList.Count > 0)
                accountStorageItemNumber = itemGuidList.Count;

            var totalAccountStorageSize = PlayerBaseData.GetInstance().AccountStorageSize;

            gridValueLabel.text = TR.Value("Common_Two_Number_Format_One",
                accountStorageItemNumber,
                totalAccountStorageSize);
        }

        //重置位置
        private void ResetStorageItemList()
        {
            if (storageItemListEx == null)
                return;

            storageItemListEx.ResetComUiListScriptEx();
        }

        #endregion

        #region BindEvents
        //整理背包中的数据
        protected void OnArrangeButtonClick()
        {
            SceneTrimItem msg = new SceneTrimItem();

            msg.pack = (byte) _storageType;
            
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneTrimItemRet>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    ItemDataManager.GetInstance().ArrangeItems(EPackageType.Storage);
                    UpdateStorageItemList();
                }
            });
        }

        //增加格子数量
        protected void OnAddGridButtonClick()
        {
            int addAccountStorageSize = PlayerBaseData.GetInstance().AccountStorageSize + 10;

            if (addAccountStorageSize > 100)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("storage_unlock_max"));
                return;
            }

            //花费数量
            var costCountTable = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                addAccountStorageSize);

            var costMoneyTable = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                addAccountStorageSize + 1);

            if (costCountTable != null && costMoneyTable != null)
            {
                CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo()
                {
                    nMoneyID = costMoneyTable.Value,
                    nCount = costCountTable.Value,
                };

                var costDescStr = StorageUtility.GetEnlargeStorageSizeCostDesc(costInfo);

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(costDescStr,
                    () =>
                    {
                        CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                        {
                            SceneEnlargeStorage msg = new SceneEnlargeStorage();
                            msg.itemEnlargeType = (byte)StorageType.AccountStorage;
                            NetManager netMgr = NetManager.Instance();
                            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

                            WaitNetMessageManager.GetInstance().Wait<SceneEnlargeStorageRet>(msgRet =>
                            {
                                if (msgRet == null)
                                {
                                    return;
                                }

                                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                                {
                                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                                }
                                else
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("storage_unlock_success"));
                                    _UpdateContent();
                                }
                            });
                        });
                    });
            }
        }

        private void _UpdateRoleStoragePageContent()
        {
            var roleStorageTotalNumber = StorageDataManager.RoleStorageTotalNumber;

            if (pageValueLabel != null)
            {
                pageValueLabel.text = TR.Value("Common_Two_Number_Format_One",
                    _accountStorageCurrentSelectedIndex,
                    roleStorageTotalNumber);
            }

            //翻页按钮
            if (_accountStorageCurrentSelectedIndex <= 1)
            {
                CommonUtility.UpdateButtonState(prePageButton, prePageGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(prePageButton, prePageGray, true);
            }

            if (_accountStorageCurrentSelectedIndex >= _accountStorageOwnerStorageNumber)
            {
                CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, true);
            }
        }

        private void _OnNextPageButtonClick()
        {
            int preIndex = _accountStorageCurrentSelectedIndex;
            if (_accountStorageCurrentSelectedIndex <= 0)
            {
                _accountStorageCurrentSelectedIndex = 1;
            }
            else if (_accountStorageCurrentSelectedIndex < _accountStorageOwnerStorageNumber)
            {
                _accountStorageCurrentSelectedIndex = _accountStorageCurrentSelectedIndex + 1;
            }
            else
            {
                _accountStorageCurrentSelectedIndex = _accountStorageOwnerStorageNumber;
            }

            StorageDataManager.GetInstance().SetAccountStorageCurrentSelectedIndex(_accountStorageCurrentSelectedIndex);

            //UpdateRoleStorageContent();
            _UpdateContent();
            _UpdateSelectView(preIndex, _accountStorageCurrentSelectedIndex);

            //换页重置
            ResetArrangeButton();
        }

        private void _OnPrePageButtonClick()
        {
            int preIndex = _accountStorageCurrentSelectedIndex;
            if (_accountStorageCurrentSelectedIndex > _accountStorageOwnerStorageNumber)
            {
                _accountStorageCurrentSelectedIndex = _accountStorageOwnerStorageNumber;
            }
            else
            {
                if (_accountStorageCurrentSelectedIndex <= 1)
                {
                    _accountStorageCurrentSelectedIndex = 1;
                }
                else
                {
                    _accountStorageCurrentSelectedIndex -= 1;
                }
            }

            StorageDataManager.GetInstance().SetAccountStorageCurrentSelectedIndex(_accountStorageCurrentSelectedIndex);

            //UpdateRoleStorageContent();
            _UpdateContent();
            _UpdateSelectView(preIndex, _accountStorageCurrentSelectedIndex);

            //换页重置
            ResetArrangeButton();
        }

        private void _OnChangeNameButtonClick()
        {
            var defaultContentNameStr =
                StorageUtility.GetStorageNameByStorageIndex(_accountStorageCurrentSelectedIndex, EPackageType.Storage);

            var titleStr = TR.Value("storage_change_name_Title");

            CommonSetContentDataModel setContentDataModel = new CommonSetContentDataModel()
            {
                TitleStr = titleStr,
                DefaultEmptyStr = TR.Value("storage_change_name_default_content"),
                DefaultContentStr = defaultContentNameStr,
                MaxWordNumber = StorageDataManager.RoleStorageNameMaxNumber,
                OnOkClicked = _OnChangeStorageNameAction,
            };

            CommonUtility.OnOpenCommonSetContentFrame(setContentDataModel);
        }

        private void _OnChangeStorageNameAction(string setContentStr)
        {
            var currentItemNameStr = StorageUtility.GetStorageNameByStorageIndex(_accountStorageCurrentSelectedIndex, EPackageType.Storage);
            if (string.Equals(setContentStr, currentItemNameStr) == true)
            {
                CommonUtility.OnCloseCommonSetContentFrame();
                return;
            }

            if (_storageGroupFrame != null)
            {
                _storageGroupFrame.ChangeStorageName(_accountStorageCurrentSelectedIndex, setContentStr, StorageType.AccountStorage);
            }
        }

        private void OnAccountStorageSelectButtonClick()
        {
            if (accountStorageSelectViewRoot == null)
                return;

            if (accountStorageSelectViewRoot.gameObject.activeInHierarchy == true)
            {
                //已经显示，直接隐藏
                CommonUtility.UpdateGameObjectVisible(accountStorageSelectViewRoot, false);
            }
            else
            {
                //显示
                CommonUtility.UpdateGameObjectVisible(accountStorageSelectViewRoot, true);

                if (_accountStorageSelectView == null)
                {
                    //初始化， OnEnable
                    var accountStorageSelectViewPrefab = CommonUtility.LoadGameObject(accountStorageSelectViewRoot);
                    if (accountStorageSelectViewPrefab != null)
                    {
                        _accountStorageSelectView = accountStorageSelectViewPrefab.GetComponent<StorageSelectView>();
                    }
                    if (_accountStorageSelectView != null)
                        _accountStorageSelectView.InitStorageDropDownView(OnAccountStorageSelectItemClickAction);
                }
                else
                {
                    //更新下拉单
                    _accountStorageSelectView.OnUpdateRoleStorageDropDownView();
                }
            }
        }

        //如果下拉单正在展示，则更新下拉单的View
        private void _UpdateAccountStorageDropDownView()
        {
            if (accountStorageSelectViewRoot == null)
                return;

            if (accountStorageSelectViewRoot.gameObject.activeInHierarchy == false)
                return;

            if (_accountStorageSelectView == null)
                return;

            _accountStorageSelectView.OnUpdateRoleStorageDropDownView();
        }

        //换页
        private void OnAccountStorageSelectItemClickAction(int index)
        {
            if (_accountStorageCurrentSelectedIndex == index)
                return;

            _accountStorageCurrentSelectedIndex = index;
            StorageDataManager.GetInstance().SetAccountStorageCurrentSelectedIndex(_accountStorageCurrentSelectedIndex);

            _UpdateContent();
            //UpdateRoleStorageContent();

            //隐藏
            CommonUtility.UpdateGameObjectVisible(accountStorageSelectViewRoot, false);

            //换页重置
            ResetArrangeButton();
        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var storageItem = item.GetComponent<StorageItem>();
            if (storageItem != null)
                storageItem.OnItemRecycle();
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (storageItemListEx == null)
                return;

            if (_storageItemDataModelList == null
               || _storageItemDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _storageItemDataModelList.Count)
                return;

            var storageItemDataModel = _storageItemDataModelList[item.m_index];
            var storageItem = item.GetComponent<StorageItem>();

            if (storageItem != null && storageItemDataModel != null)
                storageItem.InitStorageItem(storageItemDataModel);
        }

        #endregion

        #region BindMessages

        public void UpdateItemListByUiEvent(UIEvent uiEvent)
        {
            _UpdateContent();
            //UpdateStorageItemList();
        }

        //最新的一个解锁
        public void ReceiveAccountStorageUnlockMessage(UIEvent uiEvent)
        {
            if (uiEvent == null
                || uiEvent.Param1 == null
                || uiEvent.Param2 == null)
                return;

            //更新总的拥有数量和当前选择的Index
            _accountStorageOwnerStorageNumber = (int)uiEvent.Param1;
            _accountStorageCurrentSelectedIndex = (int)uiEvent.Param2;

            StorageDataManager.GetInstance().SetAccountStorageCurrentSelectedIndex(_accountStorageCurrentSelectedIndex);

            _UpdateContent();
            _UpdateAccountStorageDropDownView();
        }

        //改名成功
        public void ReceiveAccountStorageChangeNameMessages(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var changeNameKey = (int)uiEvent.Param1;
            if (_accountStorageCurrentSelectedIndex != changeNameKey)
                return;

            //更新名字
            _UpdateAccountStorageSelectStorageName();

            //更新下拉单的名字
            _UpdateAccountStorageDropDownView();
        }

        #endregion


    }
}
