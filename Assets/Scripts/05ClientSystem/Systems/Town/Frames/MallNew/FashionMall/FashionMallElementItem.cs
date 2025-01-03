using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class FashionMallElementData
    {
        public string Name;
        public FashionMallClothTabType ClothTabType;
        public MallItemInfo MallItemInfo;
    }

    public delegate void OnFashionMallElementItemTryOn(MallItemInfo mallItemInfo);
    public delegate void OnFashionMallElementItemBuy(MallItemInfo mallItemInfo);

    public class FashionMallElementItem : MonoBehaviour
    {

        private FashionMallElementData _fashionMallElementData = null;
        private OnFashionMallElementItemTryOn _elementItemTryOnDelegate = null;

        [HeaderAttribute("Text")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text curPriceText;

        [HeaderAttribute("Image")]
        [SerializeField] private Image costIcon;

        [HeaderAttribute("Button")]
        [SerializeField] private Button tryOnButton;
        [SerializeField] private GameObject itemRoot;

        // [SerializeField] private GameObject intergralFlagRoot;
        // [SerializeField] private GameObject intergralRoot;
        // [SerializeField] private GameObject singleRoot;
        // [SerializeField] private GameObject multiplePlictityRoot;
        private bool _isUpdate = false; //积分时间戳更新
        private void Awake()
        {
            _fashionMallElementData = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (tryOnButton != null)
            {
                tryOnButton.onClick.RemoveAllListeners();
                tryOnButton.onClick.AddListener(OnTryOnButtonClickCallBack);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSendQueryMallItemInfo, ReqQueryMallItemInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnQueryMallItenInfoSuccess, OnQueryMallItenInfoSuccess);
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void Update()
        {
            if (_fashionMallElementData != null &&
                _fashionMallElementData.MallItemInfo != null &&
                _fashionMallElementData.MallItemInfo.multipleEndTime > 0)
            {
                _isUpdate = true;
            }

            if (_isUpdate == true)
            {
                int time = (int)(_fashionMallElementData.MallItemInfo.multipleEndTime - TimeManager.GetInstance().GetServerTime());
                if (time <= 0)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSendQueryMallItemInfo, _fashionMallElementData.MallItemInfo.itemid);
                    _isUpdate = false;
                }
            }
        }

        private void ClearData()
        {
            _fashionMallElementData = null;
            _elementItemTryOnDelegate = null;
            _isUpdate = false;
        }

        private void UnBindUiEventSystem()
        {
            if (tryOnButton != null)
            {
                tryOnButton.onClick.RemoveAllListeners();
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSendQueryMallItemInfo, ReqQueryMallItemInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnQueryMallItenInfoSuccess, OnQueryMallItenInfoSuccess);
        }

        [SerializeField] private GameObject mObjUnselect;
        [SerializeField] private GameObject mObjSelect;
        public void InitData(FashionMallElementData fashionMallElementData,
            OnFashionMallElementItemTryOn onFashionMallElementItemTryOn = null, bool isTryOn = false)
        {
            _fashionMallElementData = fashionMallElementData;
            _elementItemTryOnDelegate = onFashionMallElementItemTryOn;
            mObjSelect.CustomActive(isTryOn);
            mObjUnselect.CustomActive(!isTryOn);
            InitElementView();

            UpdateIntergralInfo();
        }

        private void InitElementView()
        {
            if (_fashionMallElementData == null)
                return;

            var mallItemInfo = _fashionMallElementData.MallItemInfo;
            if(mallItemInfo == null)
                return;

            int itemId = MallNewDataManager.GetInstance().GetFashionItemId(mallItemInfo, _fashionMallElementData.ClothTabType);

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemId);
            if(itemData == null)
                return;

            if (itemData.SubType != (int) ItemTable.eSubType.GOLD
                && itemData.SubType != (int) ItemTable.eSubType.BindGOLD)
            {
                itemData.Count = (int) mallItemInfo.itemnum;
            }

            if (itemRoot != null)
            {
                var comItem = itemRoot.GetComponentInChildren<ComItem>();
                if (comItem == null)
                    comItem = ComItemManager.Create(itemRoot);

                //单品点击显示ItemTip
                if (_fashionMallElementData.ClothTabType == FashionMallClothTabType.Single
                    || _fashionMallElementData.ClothTabType == FashionMallClothTabType.Weapon)
                {
                    comItem.Setup(itemData,ShowItemTipFrame);
                    comItem.SetFashionMaskShow(false);
                }
                else
                {
                    comItem.Setup(itemData, null);
                    comItem.SetFashionMaskShow(false);
                }
            }

            //时装的套装类型
            if (_fashionMallElementData.ClothTabType == FashionMallClothTabType.Suit)
            {
                nameText.text = mallItemInfo.giftName;
                FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().ResetItemNameColor(itemTable, nameText);
            }
            else
            {
                //单品
                if (nameText != null)
                {
                    nameText.text = PetDataManager.GetInstance()
                        .GetColorName(itemTable.Name, (PetTable.eQuality) itemTable.Color);
                }
            }

            if(costIcon != null)
            {
                var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(mallItemInfo.moneytype);
                if (costItemTable != null)
                {
                    ETCImageLoader.LoadSprite(ref costIcon, costItemTable.Icon);
                }
                else
                {
                    Logger.LogErrorFormat("CostItemTable is null and moneyType is {0}", mallItemInfo.moneytype);
                }
            }

            if (curPriceText != null)
            {
                curPriceText.text = Utility.GetMallRealPrice(mallItemInfo).ToString();
            }
        }

        /// <summary>
        /// 初始化积分信息
        /// </summary>
        private void UpdateIntergralInfo()
        {
            if (_fashionMallElementData == null)
                return;

            // intergralFlagRoot.CustomActive(_fashionMallElementData.MallItemInfo.multiple > 0);
            // intergralRoot.CustomActive(_fashionMallElementData.MallItemInfo.multipleEndTime > 0);
            // singleRoot.CustomActive(_fashionMallElementData.MallItemInfo.multiple == 1);
            // multiplePlictityRoot.CustomActive(_fashionMallElementData.MallItemInfo.multiple > 1);
        }

        private void OnTryOnButtonClickCallBack()
        {
            if(_fashionMallElementData == null)
                return;

            if(_elementItemTryOnDelegate == null)
                return;

            //试穿回调
            _elementItemTryOnDelegate(_fashionMallElementData.MallItemInfo);
        }


        //如果是单品的话，显示ItemTipFrame
        private void ShowItemTipFrame(GameObject go, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTipWithoutModelAvatar(itemData);
        }


        private void ReqQueryMallItemInfo(UIEvent uiEvent)
        {
            //是否为同一个商品
            if (_fashionMallElementData == null)
                return;

            var itemId = (UInt32)uiEvent.Param1;

            if (_fashionMallElementData.MallItemInfo.itemid != itemId)
                return;

            MallNewDataManager.GetInstance().ReqQueryMallItemInfo((int)_fashionMallElementData.MallItemInfo.itemid);
        }

        private void OnQueryMallItenInfoSuccess(UIEvent uiEvent)
        {
            _fashionMallElementData.MallItemInfo = MallNewDataManager.GetInstance().QueryMallItemInfo;
            UpdateIntergralInfo();
        }
    }
}
