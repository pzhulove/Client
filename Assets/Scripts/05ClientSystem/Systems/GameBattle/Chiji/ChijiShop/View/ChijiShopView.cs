using System;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class ChijiShopView : MonoBehaviour
    {
        private ChijiShopType chijiShopType = ChijiShopType.Buy;

        private float timeInterval = 0.0f;

        [Space(10)]
        [HeaderAttribute("Title")]
        [Space(10)]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button backgroundButton;

        [Space(10)] [HeaderAttribute("GloryCoin")] [Space(10)]
        [SerializeField] private Image gloryCoinIcon;
        [SerializeField] private Text gloryCoinNumberValue;

        [Space(10)] [HeaderAttribute("ToggleControl")] [Space(10)]
        [SerializeField] private Toggle itemBuyToggle;
        [SerializeField] private Toggle itemSellToggle;

        [Space(10)]
        [HeaderAttribute("ShopRefreshContent")]
        [Space(10)]
        [SerializeField] private GameObject shopRefreshContentRoot;

        [Space(10)] [HeaderAttribute("ShopAutoRefresh")] [Space(10)]
        [SerializeField] private GameObject autoRefreshRoot;
        [SerializeField] private Text shopRefreshLeftTimeLabel;

        [Space(10)]
        [HeaderAttribute("ShopRefreshContent")]
        [Space(10)]
        [SerializeField] private Button shopRefreshButton;
        [SerializeField] private UIGray shopRefreshButtonGray;
        [SerializeField] private Image shopRefreshCostItemIcon;
        [SerializeField] private Text shopRefreshCostItemValueText;

        [Space(10)] [HeaderAttribute("ShopItemController")] [Space(10)]
        [SerializeField] private ChijiShopItemController chijiShopItemController;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            chijiShopType = ChijiShopType.None;

            UnBindUiEvents();

            ChijiShopDataManager.GetInstance().ResetChijiShopData();
        }

        private void BindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (backgroundButton != null)
            {
                backgroundButton.onClick.RemoveAllListeners();
                backgroundButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (itemBuyToggle != null)
            {
                itemBuyToggle.onValueChanged.RemoveAllListeners();
                itemBuyToggle.onValueChanged.AddListener(OnItemBuyToggleClick);
            }

            if (itemSellToggle != null)
            {
                itemSellToggle.onValueChanged.RemoveAllListeners();
                itemSellToggle.onValueChanged.AddListener(OnItemSellToggleClick);
            }

            if (shopRefreshButton != null)
            {
                shopRefreshButton.onClick.RemoveAllListeners();
                shopRefreshButton.onClick.AddListener(OnShopRefreshButtonClicked);
            }
        }

        private void UnBindUiEvents()
        {
            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if(backgroundButton != null)
                backgroundButton.onClick.RemoveAllListeners();

            if(itemBuyToggle != null)
                itemBuyToggle.onValueChanged.RemoveAllListeners();

            if(itemSellToggle != null)
                itemSellToggle.onValueChanged.RemoveAllListeners();

            if (shopRefreshButton != null)
            {
                shopRefreshButton.onClick.RemoveAllListeners();
            }
        }

        
        private void OnEnable()
        {
            BindUiMessages();
        }

        private void OnDisable()
        {
            UnBindUiMessages();
        }

        private void BindUiMessages()
        {
            //荣耀币代表的Counter字段发生了改变
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCounterValueChanged);

            //出售商品成功
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemSellSuccess, OnReceiveItemSellSucceed);

            //商店查询成功
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveSceneShopQuerySucceed,
                OnReceiveSceneShopQuerySucceed);
            //商店刷新成功
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveSceneShopRefreshSucceed,
                OnReceiveSceneShopRefreshSucceed);
        }


        private void UnBindUiMessages()
        {

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCounterValueChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemSellSuccess, OnReceiveItemSellSucceed);

            //商店查询成功
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveSceneShopQuerySucceed,
                OnReceiveSceneShopQuerySucceed);

            //商店刷新成功
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveSceneShopRefreshSucceed,
                OnReceiveSceneShopRefreshSucceed);

        }

        //第一次进行初始化操作
        public void InitShopView()
        {
            

            ChijiShopDataManager.GetInstance().ResetChijiShopData();

            ChijiShopDataManager.GetInstance().OnSendSceneShopQueryReq();

            chijiShopType = ChijiShopType.Buy;

            InitGloryCoinInfo();
            UpdateOwnerGloryCoinValue();

            //初始化Toggle和Toggle相关的界面
            InitShopToggle();
            UpdateShopContent();
            
        }

       
        public void OnEnableShopView()
        {
            UpdateOwnerGloryCoinValue();
            UpdateShopContent();
        }

        private void UpdateShopContent()
        {
            if (chijiShopType == ChijiShopType.Sell)
            {
                //出售界面
                CommonUtility.UpdateGameObjectVisible(shopRefreshContentRoot,
                    false);
                UpdateShopItemController();
            }
            else
            {
                //购买界面

                CommonUtility.UpdateGameObjectVisible(shopRefreshContentRoot,
                    true);

                UpdateChijiShopBuyContent();
            }
        }
        

        #region GloryCoinShow

        private void InitGloryCoinInfo()
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(
                ChijiShopDataManager.GloryCoinId);
            if (itemTable != null)
            {
                if (gloryCoinIcon != null)
                    ETCImageLoader.LoadSprite(ref gloryCoinIcon, itemTable.Icon);

                if (shopRefreshCostItemIcon != null)
                    ETCImageLoader.LoadSprite(ref shopRefreshCostItemIcon, itemTable.Icon);
            }
        }

        private void UpdateOwnerGloryCoinValue()
        {

            var itemNumber = ChijiShopUtility.GetCurrentOwnerGloryCoinNumber();

            if (gloryCoinNumberValue != null)
                gloryCoinNumberValue.text = itemNumber.ToString();

        }

        #endregion

        #region ShopToggle

        private void InitShopToggle()
        {
            if (itemBuyToggle != null)
            {
                itemBuyToggle.isOn = false;
                itemBuyToggle.isOn = true;
            }
        }

        private void OnItemBuyToggleClick(bool value)
        {
            if (value == false)
                return;

            if (chijiShopType == ChijiShopType.Buy)
            {
                return;
            }

            chijiShopType = ChijiShopType.Buy;
            

            UpdateShopContent();
        }

        private void OnItemSellToggleClick(bool value)
        {
            if (value == false)
                return;

            if (chijiShopType == ChijiShopType.Sell)
                return;

            chijiShopType = ChijiShopType.Sell;

            UpdateShopContent();
        }
        
        #endregion
        
        #region ShopItemController
        //只更新ShopItemController
        private void UpdateShopItemController()
        {
            if (chijiShopItemController == null)
                return;

            chijiShopItemController.InitShopItemController(chijiShopType);
        }

        private void UpdateShopItemControllerByGloryCoinChanged()
        {
            if (chijiShopItemController == null)
                return;

            if (chijiShopType == ChijiShopType.Sell)
                return;

            chijiShopItemController.UpdateShopItemContentByGloryCoinChanged();

        }

        #endregion

        #region ShopBuyContent
        
        private void UpdateChijiShopBuyContent()
        {

            UpdateShopAutoRefreshRoot();
            UpdateShopAutoRefreshContent();

            if (shopRefreshCostItemValueText != null)
                shopRefreshCostItemValueText.text =
                    ChijiShopDataManager.GetInstance().ChijiShopRefreshCostValue.ToString();
            
            //更新按钮状态
            UpdateRefreshButtonState();

            //更新商店的Item内容
            UpdateShopItemController();
        }

        private void UpdateRefreshButtonState()
        {
            if (chijiShopType != ChijiShopType.Buy)
                return;

            if (ChijiShopUtility.GetCurrentOwnerGloryCoinNumber() >=
                ChijiShopDataManager.GetInstance().ChijiShopRefreshCostValue)
            {
                CommonUtility.UpdateButtonState(shopRefreshButton,
                    shopRefreshButtonGray,
                    true);
            }
            else
            {
                CommonUtility.UpdateButtonState(shopRefreshButton,
                    shopRefreshButtonGray,
                    false);
            }
        }
        #endregion

        #region UIEvent

        private void OnReceiveSceneShopQuerySucceed(UIEvent uiEvent)
        {
            if (chijiShopType == ChijiShopType.Sell)
                return;

            UpdateChijiShopBuyContent();
        }

        private void OnReceiveSceneShopRefreshSucceed(UIEvent uiEvent)
        {
            if (chijiShopType == ChijiShopType.Sell)
                return;

            UpdateChijiShopBuyContent();
        }

        //荣耀币代表的Counter发生了改变
        private void OnCounterValueChanged(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var counterKey = (string) uiEvent.Param1;
            if (string.Equals(counterKey,
                    ChijiShopDataManager.GloryCoinCounterStr) == false)
                return;

            UpdateShopViewByOwnerGloryCoinChanged();
        }

        private void OnReceiveItemSellSucceed(UIEvent uiEvent)
        {
            if (chijiShopType != ChijiShopType.Sell)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            EPackageType packageType = (EPackageType)uiEvent.Param1;
            if (packageType != EPackageType.Equip)
                return;

            UpdateShopItemController();
        }

        //荣耀币改变进行更新
        private void UpdateShopViewByOwnerGloryCoinChanged()
        {
            UpdateOwnerGloryCoinValue();
            UpdateRefreshButtonState();
            UpdateShopItemControllerByGloryCoinChanged();
        }

        #endregion

        #region ButtonClicked

        //手动刷新，需要荣耀币
        private void OnShopRefreshButtonClicked()
        {
            if (chijiShopType == ChijiShopType.Sell)
                return;

            if (ChijiShopUtility.IsCanRefreshChijiShopByGloryCoin() == false)
                return;

            ChijiShopDataManager.GetInstance().OnSendSceneShopRefreshReq(
                ChijiShopDataManager.GetInstance().ChijiShopId);
        }


        private void OnCloseButtonClicked()
        {
            CommonUtility.UpdateGameObjectVisible(gameObject, false);
        }

        #endregion

        #region Update

        private void Update()
        {
            
            if (chijiShopType == ChijiShopType.Sell)
                return;

            if (ChijiShopDataManager.GetInstance().ChijiShopRefreshTimeStamp <= 0)
                return;

            timeInterval += Time.deltaTime;
            if (timeInterval >= 1.0f)
            {
                //刷新页签
                UpdateShopAutoRefreshContent();
            }
        }

        private void UpdateShopAutoRefreshContent()
        {
            //更新页签
            UpdateShopRefreshTimeLabel();
            if (ChijiShopUtility.IsChijiShopRefreshTimeUp() == true)
            {
                ChijiShopDataManager.GetInstance().ChijiShopRefreshTimeStamp = 0;
                ChijiShopDataManager.GetInstance().OnSendSceneShopQueryReq();
            }
        }

        private void UpdateShopAutoRefreshRoot()
        {
            if (ChijiShopUtility.IsChijiShopCanAutoRefresh() == true)
            {
                CommonUtility.UpdateGameObjectVisible(autoRefreshRoot, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(autoRefreshRoot, false);
            }
            
        }

        private void UpdateShopRefreshTimeLabel()
        {
            timeInterval = 0.0f;

            if (shopRefreshLeftTimeLabel == null)
                return;

            var leftTimeStr = CountDownTimeUtility.GetCountDownTimeByMinuteSecondFormat(
                (uint) ChijiShopDataManager.GetInstance().ChijiShopRefreshTimeStamp,
                TimeManager.GetInstance().GetServerTime());

            shopRefreshLeftTimeLabel.text = leftTimeStr;
        }

        #endregion 

    }

}