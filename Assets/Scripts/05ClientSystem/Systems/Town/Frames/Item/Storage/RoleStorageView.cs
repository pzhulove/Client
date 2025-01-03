using System;
using System.Collections.Generic;
using Network;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class RoleStorageView : MonoBehaviour
    {

        private StorageType _storageType;
        private List<StorageItemDataModel> _storageItemDataModelList;

        private int _roleStorageCurrentSelectedIndex = 1;
        private int _roleStorageOwnerStorageNumber = 1;

        private StorageGroupFrame _storageGroupFrame;
        private bool _inited = false;

        //selectView的空间
        private StorageSelectView _roleStorageSelectView;

        [Space(10)] [HeaderAttribute("Button")] [Space(10)]
        [SerializeField] private ComButtonWithCd arrangeButtonWithCd;
        [SerializeField] private Button changeNameButton;

        [Space(10)] [HeaderAttribute("Page")] [Space(10)]
        [SerializeField] private Button nextPageButton;
        [SerializeField] private UIGray nextPageGray;
        [SerializeField] private Button prePageButton;
        [SerializeField] private UIGray prePageGray;
        [SerializeField] private Text pageValueLabel;

        [Space(10)] [HeaderAttribute("RoleStorageSelect")] [Space(10)]
        [SerializeField] private Text roleStorageSelectNameLabel;
        [SerializeField] private Button roleStorageSelectButton;
        [SerializeField] private GameObject roleStorageSelectViewRoot;

        [Space(10)]
        [HeaderAttribute("storageItemList")]
        [Space(10)]
        [SerializeField] private ComUIListScriptEx storageItemListEx;

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
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
            
            if (nextPageButton != null)
            {
                nextPageButton.onClick.RemoveAllListeners();
                nextPageButton.onClick.AddListener(OnNextPageButtonClick);
            }

            if (prePageButton != null)
            {
                prePageButton.onClick.RemoveAllListeners();
                prePageButton.onClick.AddListener(OnPrePageButtonClick);
            }

            if (changeNameButton != null)
            {
                changeNameButton.onClick.RemoveAllListeners();
                changeNameButton.onClick.AddListener(OnChangeNameButtonClick);
            }

            if (roleStorageSelectButton != null)
            {
                roleStorageSelectButton.onClick.RemoveAllListeners();
                roleStorageSelectButton.onClick.AddListener(OnRoleStorageSelectButtonClick);
            }
        }

        private void UnBindUiEvents()
        {

            if (arrangeButtonWithCd != null)
            {
                arrangeButtonWithCd.ResetButtonListener();
            }

            if (storageItemListEx != null)
            {
                storageItemListEx.onItemVisiable -= OnItemVisible;
                storageItemListEx.OnItemRecycle -= OnItemRecycle;
                storageItemListEx.UnInitialize();
            }

            if (nextPageButton != null)
                nextPageButton.onClick.RemoveAllListeners();

            if (prePageButton != null)
                prePageButton.onClick.RemoveAllListeners();

            if (changeNameButton != null)
                changeNameButton.onClick.RemoveAllListeners();

            if (roleStorageSelectButton != null)
                roleStorageSelectButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _roleStorageOwnerStorageNumber = 0;
            _roleStorageCurrentSelectedIndex = 0;
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

            var itemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.RoleStorage);
            ItemDataUtility.ArrangeItemGuidList(itemGuidList);

            _roleStorageCurrentSelectedIndex = StorageDataManager.GetInstance().GetRoleStorageCurrentSelectedIndex();
            _roleStorageOwnerStorageNumber = StorageDataManager.GetInstance().GetRoleStorageOwnerStorageNumber();

            UpdateRoleStorageContent();

            ResetArrangeButton();
        }

        public void OnEnableView()
        {
            _roleStorageCurrentSelectedIndex = StorageDataManager.GetInstance().GetRoleStorageCurrentSelectedIndex();
            _roleStorageOwnerStorageNumber = StorageDataManager.GetInstance().GetRoleStorageOwnerStorageNumber();

            UpdateRoleStorageContent();

            UpdateRoleStorageDropDownView();

            ResetArrangeButton();
        }

        #region Update
        //重置整理按钮的CD
        private void ResetArrangeButton()
        {
            if(arrangeButtonWithCd != null)
                arrangeButtonWithCd.Reset();
        }

        //更新StorageItemList
        void UpdateRoleStorageItemList()
        {
            if (storageItemListEx == null)
                return;

            _storageItemDataModelList = StorageUtility.GetStorageItemDataModelList(_storageType,
                _roleStorageCurrentSelectedIndex);
            int count = 0;
            if (_storageItemDataModelList != null)
                count = _storageItemDataModelList.Count;

            storageItemListEx.SetElementAmount(count);
        }

        //重置位置
        private void ResetRoleStorageItemList()
        {
            if (storageItemListEx == null)
                return;

            storageItemListEx.ResetComUiListScriptEx();
        }

        private void UpdateRoleStoragePageContent()
        {
            var roleStorageTotalNumber = StorageDataManager.RoleStorageTotalNumber;

            if (pageValueLabel != null)
            {
                pageValueLabel.text = TR.Value("Common_Two_Number_Format_One",
                    _roleStorageCurrentSelectedIndex,
                    roleStorageTotalNumber);
            }

            //翻页按钮
            if (_roleStorageCurrentSelectedIndex <= 1)
            {
                CommonUtility.UpdateButtonState(prePageButton, prePageGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(prePageButton, prePageGray, true);
            }

            if (_roleStorageCurrentSelectedIndex >= _roleStorageOwnerStorageNumber)
            {
                CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, true);
            }
        }

        //更新选择仓库的名字
        private void UpdateRoleStorageSelectStorageName()
        {
            if (roleStorageSelectNameLabel == null)
                return;

            var selectNameStr = StorageUtility.GetStorageNameByStorageIndex(_roleStorageCurrentSelectedIndex);
            roleStorageSelectNameLabel.text = selectNameStr;
        }

        //如果下拉单正在展示，则更新对应Item的名字
        private void UpdateRoleStorageDropDownName()
        {
            if (roleStorageSelectViewRoot == null)
                return;

            if (roleStorageSelectViewRoot.gameObject.activeInHierarchy == false)
                return;

            if (_roleStorageSelectView == null)
                return;

            _roleStorageSelectView.OnUpdateRoleStorageDropDownName(_roleStorageCurrentSelectedIndex);
        }


        //如果下拉单正在展示，则更新下拉单的View
        private void UpdateRoleStorageDropDownView()
        {
            if (roleStorageSelectViewRoot == null)
                return;

            if (roleStorageSelectViewRoot.gameObject.activeInHierarchy == false)
                return;

            if (_roleStorageSelectView == null)
                return;

            _roleStorageSelectView.OnUpdateRoleStorageDropDownView();
        }

        private void _UpdateSelectView(int preIndex, int nextIndex)
        {
            if (_roleStorageSelectView != null && _roleStorageSelectView.gameObject.activeInHierarchy)
            {
                _roleStorageSelectView.OnUpdateStorageSelect(preIndex, nextIndex);
            }
        }

        private void UpdateRoleStorageContent()
        {
            //翻页
            UpdateRoleStoragePageContent();
            //选中仓库的名字
            UpdateRoleStorageSelectStorageName();

            //重置道具列表
            ResetRoleStorageItemList();
            //选中道具的列表
            UpdateRoleStorageItemList();
        }

        #endregion

        #region BindEvents

        private void OnNextPageButtonClick()
        {
            int preIndex = _roleStorageCurrentSelectedIndex;
            if (_roleStorageCurrentSelectedIndex <= 0)
            {
                _roleStorageCurrentSelectedIndex = 1;
            }
            else if (_roleStorageCurrentSelectedIndex < _roleStorageOwnerStorageNumber)
            {
                _roleStorageCurrentSelectedIndex = _roleStorageCurrentSelectedIndex + 1;
            }
            else
            {
                _roleStorageCurrentSelectedIndex = _roleStorageOwnerStorageNumber;
            }

            StorageDataManager.GetInstance().SetRoleStorageCurrentSelectedIndex(_roleStorageCurrentSelectedIndex);

            UpdateRoleStorageContent();
            _UpdateSelectView(preIndex, _roleStorageCurrentSelectedIndex);

            //换页重置
            ResetArrangeButton();
        }

        private void OnPrePageButtonClick()
        {
            int preIndex = _roleStorageCurrentSelectedIndex;
            if (_roleStorageCurrentSelectedIndex > _roleStorageOwnerStorageNumber)
            {
                _roleStorageCurrentSelectedIndex = _roleStorageOwnerStorageNumber;
            }
            else
            {
                if (_roleStorageCurrentSelectedIndex <= 1)
                {
                    _roleStorageCurrentSelectedIndex = 1;
                }
                else
                {
                    _roleStorageCurrentSelectedIndex -= 1;
                }
            }
        
            StorageDataManager.GetInstance().SetRoleStorageCurrentSelectedIndex(_roleStorageCurrentSelectedIndex);

            UpdateRoleStorageContent();
            _UpdateSelectView(preIndex, _roleStorageCurrentSelectedIndex);

            //换页重置
            ResetArrangeButton();
        }

        private void OnChangeNameButtonClick()
        {
            var defaultContentNameStr =
                StorageUtility.GetStorageNameByStorageIndex(_roleStorageCurrentSelectedIndex);

            var titleStr = TR.Value("storage_change_name_Title");

            CommonSetContentDataModel setContentDataModel = new CommonSetContentDataModel()
            {
                TitleStr = titleStr,
                DefaultEmptyStr = TR.Value("storage_change_name_default_content"),
                DefaultContentStr = defaultContentNameStr,
                MaxWordNumber = StorageDataManager.RoleStorageNameMaxNumber,
                OnOkClicked = OnChangeStorageNameAction,
            };

            CommonUtility.OnOpenCommonSetContentFrame(setContentDataModel);
        }

        private void OnChangeStorageNameAction(string setContentStr)
        {
            var currentItemNameStr = StorageUtility.GetStorageNameByStorageIndex(_roleStorageCurrentSelectedIndex);

            if (string.Equals(setContentStr, currentItemNameStr) == true)
            {
                CommonUtility.OnCloseCommonSetContentFrame();
                return;
            }

            if (_storageGroupFrame != null)
            {
                _storageGroupFrame.ChangeStorageName(_roleStorageCurrentSelectedIndex, setContentStr);
            }
        }


        //整理背包中的数据
        protected void OnArrangeButtonClick()
        {
            var minGridIndex = StorageUtility.GetRoleStorageItemGridMinGridIndex(_roleStorageCurrentSelectedIndex);
            var maxGridIndex = StorageUtility.GetRoleStorageItemGridMaxGridIndex(_roleStorageCurrentSelectedIndex);
            StorageUtility.ResortRoleStorageItemGuidByGridIndex(minGridIndex, maxGridIndex);
            
            UpdateRoleStorageItemList();
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

        //roleStorageSelect
        private void OnRoleStorageSelectButtonClick()
        {
            if (roleStorageSelectViewRoot == null)
                return;

            if (roleStorageSelectViewRoot.gameObject.activeInHierarchy == true)
            {
                //已经显示，直接隐藏
                CommonUtility.UpdateGameObjectVisible(roleStorageSelectViewRoot, false);
            }
            else
            {
                //显示
                CommonUtility.UpdateGameObjectVisible(roleStorageSelectViewRoot, true);

                if (_roleStorageSelectView == null)
                {
                    //初始化， OnEnable
                    var roleStorageSelectViewPrefab = CommonUtility.LoadGameObject(roleStorageSelectViewRoot);
                    if (roleStorageSelectViewPrefab != null)
                    {
                        _roleStorageSelectView = roleStorageSelectViewPrefab.GetComponent<StorageSelectView>();
                    }
                    if (_roleStorageSelectView != null)
                        _roleStorageSelectView.InitStorageDropDownView(OnRoleStorageSelectItemClickAction, EPackageType.RoleStorage);
                }
                else
                {
                    //更新下拉单
                    _roleStorageSelectView.OnUpdateRoleStorageDropDownView();
                }
            }
        }

        //换页
        private void OnRoleStorageSelectItemClickAction(int index)
        {
            if (_roleStorageCurrentSelectedIndex == index)
                return;

            _roleStorageCurrentSelectedIndex = index;
            StorageDataManager.GetInstance().SetRoleStorageCurrentSelectedIndex(_roleStorageCurrentSelectedIndex);

            UpdateRoleStorageContent();

            //隐藏
            CommonUtility.UpdateGameObjectVisible(roleStorageSelectViewRoot, false);

            //换页重置
            ResetArrangeButton();
        }


        #endregion

        #region BindMessages

        public void UpdateItemListByUiEvent(UIEvent uiEvent)
        {
            UpdateRoleStorageItemList();
        }

        //改名成功
        public void ReceiveRoleStorageChangeNameMessages(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var changeNameKey = (int)uiEvent.Param1;
            if (_roleStorageCurrentSelectedIndex != changeNameKey)
                return;

            //更新名字
            UpdateRoleStorageSelectStorageName();

            //更新下拉单的名字
            UpdateRoleStorageDropDownName();
        }

        //最新的一个解锁
        public void ReceiveRoleStorageUnlockMessage(UIEvent uiEvent)
        {
            if (uiEvent == null 
                || uiEvent.Param1 == null
                || uiEvent.Param2 == null)
                return;

            //更新总的拥有数量和当前选择的Index
            _roleStorageOwnerStorageNumber = (int) uiEvent.Param1;
            _roleStorageCurrentSelectedIndex = (int) uiEvent.Param2;

            StorageDataManager.GetInstance().SetRoleStorageCurrentSelectedIndex(_roleStorageCurrentSelectedIndex);

            UpdateRoleStorageContent();
            UpdateRoleStorageDropDownView();
        }
        #endregion
    }
}
