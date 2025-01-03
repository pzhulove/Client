using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class FashionMallSuitElementItem : MonoBehaviour
    {

        private FashionMallElementData _fashionMallElementData = null;
        private OnFashionMallElementItemBuy _elementItemBuyDelegate = null;
        private OnFashionMallElementItemTryOn _elementItemTryOnDelegate = null;

        [SerializeField] private Image suitImage;
        [SerializeField] private Image intergralMutiple;

        [SerializeField] private GameObject discountRoot;

        [HeaderAttribute("Button")]
        [SerializeField] private GameObject buttonRoot;
        [SerializeField] private Button tryOnButton;
        [SerializeField] private Button buyButton;

        [HeaderAttribute("Gameobject")]
        [SerializeField]private GameObject intergralRoot;
        [SerializeField]private GameObject intergralFlagRoot;
        [SerializeField] private GameObject singleRoot;
        [SerializeField] private GameObject multiplePlictityRoot;

        [HeaderAttribute("Text")]
        [SerializeField]private Text intergralLimtText;

        private bool _isUpdate = false; //积分时间戳更新
        private void Awake()
        {
            _fashionMallElementData = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuyButtonClickCallBack);
            }

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
            _elementItemBuyDelegate = null;
            _isUpdate = false;
        }

        private void UnBindUiEventSystem()
        {
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
            }

            if (tryOnButton != null)
            {
                tryOnButton.onClick.RemoveAllListeners();
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSendQueryMallItemInfo, ReqQueryMallItemInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnQueryMallItenInfoSuccess, OnQueryMallItenInfoSuccess);
        }

        public void InitData(FashionMallElementData fashionMallElementData,
            OnFashionMallElementItemBuy elementItemBuy = null,
            OnFashionMallElementItemTryOn elementItemTryOn = null)
        {
            _fashionMallElementData = fashionMallElementData;
            _elementItemBuyDelegate = elementItemBuy;
            _elementItemTryOnDelegate = elementItemTryOn;

            UpdateSuitElementVisible();

            InitSuitElement();

            UpdateIntergralInfo();
        }

        //显示套装图片和按钮
        private void UpdateSuitElementVisible()
        {
            if (suitImage != null)
            {
                if (suitImage.gameObject.activeSelf == false)
                {
                    suitImage.gameObject.CustomActive(true);
                }
            }

            if (buttonRoot != null)
            {
                if (buttonRoot.activeSelf == false)
                {
                    buttonRoot.CustomActive(true);
                }
            }
        }


        private void InitSuitElement()
        {
            if (_fashionMallElementData == null)
                return;

            //todo 更新图片
            //根据类型获得图片的地址
            var imagePath = GetSuitItemImagePath();
            if (suitImage != null)
            {
                suitImage.sprite = AssetLoader.instance.LoadRes(imagePath,
                    typeof(Sprite)).obj as Sprite;
            }

        }

        /// <summary>
        /// 初始化积分信息
        /// </summary>
        private void UpdateIntergralInfo()
        {
            if (_fashionMallElementData == null)
                return;

            intergralFlagRoot.CustomActive(_fashionMallElementData.MallItemInfo.multiple > 0);
            intergralRoot.CustomActive(_fashionMallElementData.MallItemInfo.multipleEndTime > 0);
            singleRoot.CustomActive(_fashionMallElementData.MallItemInfo.multiple == 1);
            multiplePlictityRoot.CustomActive(_fashionMallElementData.MallItemInfo.multiple > 1);

            if (intergralMutiple != null)
            {
                ETCImageLoader.LoadSprite(ref intergralMutiple, TR.Value("mall_new_limit_item_left_intergral_multiple_sprit_path", _fashionMallElementData.MallItemInfo.multiple));
            }

            intergralLimtText.text = TR.Value("mall_new_limit_item_left_intergral_multiple_limit", _fashionMallElementData.MallItemInfo.multiple, Function.SetShowTimeDay((int)_fashionMallElementData.MallItemInfo.multipleEndTime));
        }

        private void OnTryOnButtonClickCallBack()
        {
            if (_fashionMallElementData == null)
                return;

            if(_elementItemTryOnDelegate == null)
                return;

            _elementItemTryOnDelegate(_fashionMallElementData.MallItemInfo);
        }

        private void OnBuyButtonClickCallBack()
        {
            if (_fashionMallElementData == null)
                return;

            if(_elementItemBuyDelegate == null)
                return;

            _elementItemBuyDelegate(_fashionMallElementData.MallItemInfo);
        }

        private string GetSuitItemImagePath()
        {
            
            string path = "UI/Image/Background/UI_Beijing_Shangdian_Hunsha_05.jpg:UI_Beijing_Shangdian_Hunsha_05";

            if (_fashionMallElementData == null)
                return path;

            return _fashionMallElementData.MallItemInfo.fashionImagePath;
            
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
