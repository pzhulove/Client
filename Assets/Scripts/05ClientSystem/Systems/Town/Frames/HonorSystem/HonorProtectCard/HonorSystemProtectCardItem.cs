using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemProtectCardItem : MonoBehaviour
    {

        private int _protectCardItemId;
        private ItemData _itemData;

        private ItemTable _itemTable;

        [Space(10)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text itemNameLabel;
        [SerializeField] private Text itemLevelLimitLabel;

        [Space(10)]
        [HeaderAttribute("Item")]
        [Space(10)]
        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text itemCountLabel;

        [Space(10)] [HeaderAttribute("ButtonRoot")] [Space(10)]
        [SerializeField] private Button getButton;
        [SerializeField] private UIGray getButtonGray;
        [SerializeField] private Button useButton;
        [SerializeField] private UIGray useButtonGray;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (getButton != null)
            {
                getButton.onClick.RemoveAllListeners();
                getButton.onClick.AddListener(OnGetButtonClicked);
            }

            if (useButton != null)
            {
                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(OnUseButtonClicked);
            }

            //荣耀币获得和数量改变
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnItemInPackageAddedMessage,
                OnItemInPackageAddedMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemPropertyChanged,
                OnItemPropertyChanged);
        }

        private void UnBindEvents()
        {
            if(getButton != null)
                getButton.onClick.RemoveAllListeners();

            if(useButton != null)
                useButton.onClick.RemoveAllListeners();

            //荣耀币获得和数量改变
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnItemInPackageAddedMessage,
                OnItemInPackageAddedMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemPropertyChanged,
                OnItemPropertyChanged);
        }

        private void ClearData()
        {
            _protectCardItemId = 0;
            _itemData = null;
            _itemTable = null;
        }

        public void InitItem(int protectCardItemId,
            ItemData itemData)
        {
            _protectCardItemId = protectCardItemId;
            if (_protectCardItemId <= 0)
                return;

            _itemData = itemData;

            _itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_protectCardItemId);
            if (_itemTable == null)
                return;

            InitBaseView();

            UpdateItemCountLabel();

            UpdateButtonState();
        }
        
        private void InitBaseView()
        {
            if (itemNameLabel != null)
                itemNameLabel.text = CommonUtility.GetItemColorName(_itemTable);

            if (itemLevelLimitLabel != null)
            {
                var levelLimitStr = TR.Value("Honor_System_Protect_Card_Level_Format_Label",
                    _itemTable.UseLimiteValue);
                itemLevelLimitLabel.text = levelLimitStr;
            }

            if (itemRoot != null)
            {
                var commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                if (commonNewItem == null)
                {
                    commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);
                }
                commonNewItem.InitItem(_protectCardItemId);
            }
        }

        private void UpdateItemCountLabel()
        {
            if (itemCountLabel == null)
                return;

            //至少存在一个
            if (_itemData != null
                && _itemData.Count > 0)
            {
                itemCountLabel.text = _itemData.Count.ToString();
                itemCountLabel.color = Color.green;
            }
            else
            {
                itemCountLabel.text = "0";
                itemCountLabel.color = Color.red;
            }
        }

        private void UpdateButtonState()
        {
            var isHonorLevelLimited = HonorSystemDataManager.GetInstance().PlayerHonorLevel > _itemTable.UseLimiteValue;

            //至少存在一个
            if (_itemData != null
                && _itemData.Count > 0)
            {
                CommonUtility.UpdateButtonVisible(getButton, false);
                CommonUtility.UpdateButtonVisible(useButton, true);

                //判断等级
                if (isHonorLevelLimited == true)
                {
                    //置灰色
                    CommonUtility.UpdateButtonState(useButton, useButtonGray, false);
                }
                else
                {
                    CommonUtility.UpdateButtonState(useButton, useButtonGray, true);
                }
                
            }
            else
            {
                CommonUtility.UpdateButtonVisible(useButton, false);

                CommonUtility.UpdateButtonVisible(getButton, true);

                //判断等级
                if (isHonorLevelLimited == true)
                {
                    //置灰色
                    CommonUtility.UpdateButtonState(getButton, getButtonGray, false);
                }
                else
                {
                    CommonUtility.UpdateButtonState(getButton, getButtonGray, true);
                }
            }
        }

        private void OnCloseFrame()
        {
            HonorSystemUtility.OnCloseHonorSystemProtectCardFrame();
        }

        private void OnGetButtonClicked()
        {
            //链接跳转
            if (_itemTable == null)
            {
                return;
            }

            HonorSystemUtility.OnCloseHonorSystemProtectCardFrame();

            //超链接跳转
            ItemComeLink.OnLink(_protectCardItemId, 0, false, null, false, _itemTable.bNeedJump > 0);
        }

        private void OnUseButtonClicked()
        {
            if (_itemData == null || _itemTable == null)
                return;

            var itemNameStr = CommonUtility.GetItemColorName(_itemTable);

            var tipContentStr = TR.Value("Honor_System_Protect_Card_Use_Content_Format",
                itemNameStr);

            CommonUtility.OnShowCommonMsgBox(tipContentStr,
                OnUseProtectCardItem,
                TR.Value("common_data_sure_2"));

        }

        private void OnUseProtectCardItem()
        {
            HonorSystemUtility.OnCloseHonorSystemProtectCardFrame();

            if (_itemData == null)
                return;

            ItemDataManager.GetInstance().UseItem(_itemData);
        }

        #region UIEvent

        private void OnItemPropertyChanged(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var propertyType = (EItemProperty)uiEvent.Param2;
            if (propertyType != EItemProperty.EP_NUM)
                return;

            var itemData = uiEvent.Param1 as ItemData;
            if (itemData == null)
                return;

            if (itemData.TableID != _protectCardItemId)
                return;

            UpdateProtectCardItemInfo(itemData);
        }

        private void OnItemInPackageAddedMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var itemGuid = (ulong)uiEvent.Param1;
            var itemTableId = (int)uiEvent.Param2;

            if (itemTableId != _protectCardItemId)
                return;

            var itemData = ItemDataManager.GetInstance().GetItem(itemGuid);
            if (itemData == null)
                return;

            UpdateProtectCardItemInfo(itemData);
        }

        private void UpdateProtectCardItemInfo(ItemData itemData)
        {
            if (itemData == null)
                return;

            _itemData = itemData;

            //更新道具的数量和按钮的状态
            UpdateItemCountLabel();
            UpdateButtonState();
        }

        #endregion 

    }
}