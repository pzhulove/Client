using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;

namespace GameClient
{
    public class ShopBaseView : MonoBehaviour
    {
        protected virtual void _InitShopView()
        {
            _InitShopTitle();
            _InitShopMoneyView();
            _InitShopTabList();
            _PlayShopNpcSound(NpcVoiceComponent.SoundEffectType.SET_Start);
        }
        protected virtual void _InitShopTitle() { }
        protected virtual void _InitShopMoneyView() { }
        protected virtual void _InitShopTabList() { }
        protected virtual void _PlayShopNpcSound(NpcVoiceComponent.SoundEffectType soundEffType) { }

        protected virtual void _InitData() { }
        protected virtual void _ClearData() { }
    }

    public class AccountShopView : ShopBaseView
    {
        #region MODEL PARAMS

        private string tr_bless_shop_no_goods_tip = "";
        private string tr_acc_shop_refresh_time_format = "";

        //Frame数据
        private ShopNewFrameViewData _mFrameCommonData = null;

        #endregion
        
        #region VIEW PARAMS

        [Space(5)]
        [HeaderAttribute("Title")]
        [SerializeField] private Text shopNameText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private ComConsumeItemGroup consumeItemGroup;
        
        [Space(10)] [HeaderAttribute("ShopElement")] 
        [SerializeField] private CommonTabToggleGroup mToggleGroup;
        [SerializeField] private ComUIListScript shopItemList;
        [SerializeField] private ShopNewFilterView shopFilterView;

        [Space(10)] [HeaderAttribute("OtherInfo")] 
        [SerializeField] private Text shopNoGoodsLabel;
        [SerializeField] private Text shopRefreshLabel;
        [SerializeField] private TimeRefresh shopTimeRefreshControl;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake()
        {
            _BindUIEvent();
            _InitData();
        }
       
        //Unity life cycle
        void OnDestroy () 
        {
            _UnBindUIEvent();
            _ClearData();

            _PlayShopNpcSound(NpcVoiceComponent.SoundEffectType.SET_End);
        }

        #region EVENT
        
        private void _BindUIEvent()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (shopItemList != null)
            {
                if (shopItemList.IsInitialised() == false)
                {
                    shopItemList.Initialize();
                    shopItemList.onItemVisiable += OnElementItemVisible;
                    shopItemList.OnItemRecycle += OnElementItemRecycle;
                    shopItemList.onItemSelected += OnElementItemSelect;
                }
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountShopUpdate, _OnShopGoodsRefresh);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountShopItemUpdata, _OnShopGoodsRefresh);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountShopReqFailed, _OnShopGoodsRefresh);
        }

        private void _UnBindUIEvent()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(OnCloseFrame);
            }

            if (shopItemList != null)
            {
                shopItemList.UnInitialize();
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountShopUpdate, _OnShopGoodsRefresh);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountShopItemUpdata, _OnShopGoodsRefresh);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountShopReqFailed, _OnShopGoodsRefresh);
        }

        #endregion

        #region Base Data

        protected sealed override void _InitData()
        {
            tr_bless_shop_no_goods_tip = TR.Value("adventure_team_shop_no_goods_tips");
            tr_acc_shop_refresh_time_format = TR.Value("adventure_team_shop_refresh_next_time_format");
        }

        protected sealed override void _ClearData() 
        {
            tr_bless_shop_no_goods_tip = "";
            tr_acc_shop_refresh_time_format = "";

            if (_mFrameCommonData != null)
            {
                _mFrameCommonData.Clear();
                _mFrameCommonData = null;
            }

            //部分使用到这儿的刷新界面缓存用的数据  要清一下 ！！！
            ShopNewDataManager.GetInstance().ClearData();
        }
        protected sealed override void _InitShopView()
        {
            base._InitShopView();
        }

        protected sealed override void _InitShopTitle()
        {
            if (_mFrameCommonData == null)
            {
                return;
            }
            if (shopNameText)
            {
                shopNameText.text = AccountShopDataManager.GetInstance().GetShopName(_mFrameCommonData.shopId);
            }

            int helpId = AccountShopDataManager.GetInstance().GetShopHelpId(_mFrameCommonData.shopId);

            if (helpId <= 0)
            {
                helpButton.CustomActive(false);
            }
            else
            {
                helpButton.CustomActive(true);
                var helpAssistant = helpButton.GetComponent<HelpAssistant>();
                if (helpAssistant != null)
                {
                    helpAssistant.eType = (HelpFrameContentTable.eHelpType)helpId;
                }
            }
        }

        private void _InitTabList()
        {
        }

        protected sealed override void _InitShopTabList()
        {
            if (_mFrameCommonData == null)
            {
                return;
            }

            _mFrameCommonData.totalTabDataList = ShopNewDataManager.GetInstance().GetShopNewMainTabDataList(_mFrameCommonData.shopId);
            var totalTabDataList = _mFrameCommonData.totalTabDataList;
            if (totalTabDataList == null)
            {
                Logger.LogErrorFormat("[AccountShopView] - _InitShopTabList failed, shopid is {0}", _mFrameCommonData.shopId);
                return;
            }
            for (int i = 0; i < totalTabDataList.Count; i++)
            {
                var totalTabData = totalTabDataList[i];
                if (totalTabData == null)
                    continue;
                if (totalTabData.MainTabType == _mFrameCommonData.defaultSelectedTabData.MainTabType &&
                    totalTabData.Index == _mFrameCommonData.defaultSelectedTabData.Index)
                {
                    _mFrameCommonData.defaultSelectedTabIndex = i;
                    break;
                }
            }

            if (null != mToggleGroup)
            {
                var tabList = new List<CommonTabData>();
                int index = 0;
                foreach (var data in _mFrameCommonData.totalTabDataList)
                {
                    var tabData = new CommonTabData();
                    tabData.id = index;
                    tabData.tabName = data.Name;
                    ++index;
                    tabList.Add(tabData);
                }
                mToggleGroup.InitComTab(_ToggleClick, _mFrameCommonData.defaultSelectedTabIndex, tabList);
            }
        }

        protected sealed override void _InitShopMoneyView()
        {
            if (_mFrameCommonData == null)
            {
                return;
            }

            if(consumeItemGroup != null)
            {
                consumeItemGroup.SetAllItemActive(false);
            }
            //_RefreshConsumeItemGroup(_mFrameCommonData.shopId);
        }

        protected sealed override void _PlayShopNpcSound(NpcVoiceComponent.SoundEffectType soundEffType)
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (null == current)
                return;

            if (_mFrameCommonData == null)
                return;

            if (_mFrameCommonData.npcId <= 0)
                return;

            var npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(_mFrameCommonData.npcId);
            if (npcItem == null)
                return;

            current.PlayNpcSound(_mFrameCommonData.npcId, soundEffType);
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// 刷新商店刷新时间描述
        /// </summary>
        /// <param name="shopId"></param>
        private void _RefreshShopItemRefreshTimeDesc(int shopId)
        {
            if (_mFrameCommonData != null)
            {
                if (shopRefreshLabel != null)
                {
                    string desc = AccountShopDataManager.GetInstance().GetShopRefreshTimeDesc(shopId);
                    if (!string.IsNullOrEmpty(desc))
                    {
                        shopRefreshLabel.text = desc;
                        shopRefreshLabel.enabled = true;
                    }
                    else
                    {
                        shopRefreshLabel.enabled = false;
                        shopRefreshLabel.text = "";
                    }
                }
            }
        }

        /// <summary>
        /// 刷新商店刷新时间
        /// </summary>
        private void _RefreshShopItemRefreshTime(Protocol.AccountShopItemInfo[] accShopItemInfos)
        {
            //刷新时间 描述不为空  则不进行其他显示
            if(shopRefreshLabel != null && shopRefreshLabel.enabled)
            {
                return;
            }

            if(accShopItemInfos == null)
            {
                return;
            }
            int refreshTime = AccountShopDataManager.GetInstance().GetAccountShopRefreshTime(accShopItemInfos);
            if(shopTimeRefreshControl) 
            {
                if(refreshTime > 0)
                {
                    shopTimeRefreshControl.Initialize();
                    shopTimeRefreshControl.SetFormatString(tr_acc_shop_refresh_time_format);
                    shopTimeRefreshControl.Time = (uint)refreshTime;
                    shopTimeRefreshControl.enabled = true;
                    if(shopTimeRefreshControl.text)
                    {
                        shopTimeRefreshControl.text.enabled = true;
                    }
                }
                else
                {
                    shopTimeRefreshControl.enabled = false;
                    if(shopTimeRefreshControl.text)
                    {
                        shopTimeRefreshControl.text.enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// 刷新 基本的 消耗道具组 读ShopTable 
        /// </summary>
        /// <param name="showCostMoney"></param>
        private void _RefreshConsumeItemGroup(int shopId)
        {
            if (consumeItemGroup != null)
            {
                //刷新基础消耗道具时  重置状态
                //consumeItemGroup.SetAllItemActive(false);

                List<int> baseMoneyIds = AccountShopDataManager.GetInstance().GetShopBaseMoneyIds(shopId);
                if (baseMoneyIds == null || baseMoneyIds.Count <= 0)
                {
                    return;
                }

                consumeItemGroup.ResetSelectedItemIds(baseMoneyIds.ToArray(), false, true);

                // for (int i = 0; i < baseMoneyIds.Count; i++)
                // {
                //     if (baseMoneyIds[i] <= 0)
                //     {
                //         return;
                //     }
                //     consumeItemGroup.SetItemActiveByItemId(baseMoneyIds[i], true);
                // }
            }
        }

        private void _RefreshConsumeItemGroup(Protocol.AccountShopItemInfo[] accShopItemInfos)
        {
            if (consumeItemGroup != null)
            {
                if(accShopItemInfos == null)
                {
                    return;
                }
                List<int> extraMoneyIds = AccountShopDataManager.GetInstance().GetShopExtraMoneyIds(accShopItemInfos);
                if (extraMoneyIds == null || extraMoneyIds.Count <= 0)
                {
                    return;
                }

                consumeItemGroup.ResetSelectedItemIds(extraMoneyIds.ToArray(), true, true);

                // for (int i = 0; i < extraMoneyIds.Count; i++)
                // {
                //     if (extraMoneyIds[i] <= 0)
                //     {
                //         return;
                //     }
                //     consumeItemGroup.SetItemActiveByItemId(extraMoneyIds[i], true);
                // }
            }
        }

        private void _RefreshFilterView(int index)
        {
            if(_mFrameCommonData == null)
            {
                return;
            }

            //重新缓存 过滤器数据
            AccountShopDataManager.GetInstance().RestoreFilterDataByIndex(ref _mFrameCommonData.currFirstFilterData,
                _mFrameCommonData.currentSelectedTabData.FirstFilterData,
                AccountShopFilterType.First, index);
            AccountShopDataManager.GetInstance().RestoreFilterDataByIndex(ref _mFrameCommonData.currSecondFilterData,
                _mFrameCommonData.currentSelectedTabData.SecondFilterData,
                AccountShopFilterType.Second, index);

            bool isShopFilter = _mFrameCommonData.currentSelectedTabData != null &&
                _mFrameCommonData.currentSelectedTabData.MainTabType == ShopNewMainTabType.ShopType;

            if (shopFilterView != null)
            {
                shopFilterView.InitShopNewFilterView(_mFrameCommonData.currFirstFilterData,
                    OnFirstFilterElementItemTabClick,
                    _mFrameCommonData.currSecondFilterData,
                    OnSecondFilterElementItemTabClick,
                    isShopFilter);
            }
        }

        #endregion

        #region Main Tabs
        //点击tab事件
        private void _ToggleClick(CommonTabData data)
        {
            if (null != _mFrameCommonData.totalTabDataList && _mFrameCommonData.totalTabDataList.Count > data.id)
                OnMainTabClickCallBack(data.id, _mFrameCommonData.totalTabDataList[data.id]);
        }
        private void OnMainTabClickCallBack(int mainTabIndex, ShopNewMainTabData shopNewMainTabData)
        {
            if (_mFrameCommonData == null)
                return;

            _mFrameCommonData.currentSelectedTabIndex = mainTabIndex;
            //检测主按钮的数据是否相同， 如果相同，则直接返回
            if (_mFrameCommonData.currentSelectedTabData == shopNewMainTabData)
            {
                Logger.LogErrorFormat("The same mainTabType and index. the mainTabType is {0}, the index is {1}",
                    shopNewMainTabData.MainTabType.ToString(), shopNewMainTabData.Index);
                return;
            }

            _mFrameCommonData.currentSelectedTabData = shopNewMainTabData;

            int _currShopId = 0;
            //如果打开商店 主页签（右上角）类型是 商店 ， 即还有子商店
            if (_mFrameCommonData.currentSelectedTabData.MainTabType == ShopNewMainTabType.ShopType)
            {
                _currShopId = _mFrameCommonData.currentSelectedTabData.Index;
            }
            //如果打开商店 主页签（右上角）类型是 商品  
            else if(_mFrameCommonData.currentSelectedTabData.MainTabType == ShopNewMainTabType.ItemType)
            {
                _currShopId = _mFrameCommonData.shopId;
            }

            //刷新商店的基础消耗品 如果id不为0
            _RefreshConsumeItemGroup(_currShopId);
            //刷新商店的刷新时间
            _RefreshShopItemRefreshTimeDesc(_currShopId);

            //刷新过滤器
            _RefreshFilterView(mainTabIndex);

            //第一次打开界面 需要请求一次
            if (_mFrameCommonData.currentSelectedTabData.IsClicked == false)
            {
                AccountShopDataManager.GetInstance().SendWorldAccountShopItemQueryReq(_mFrameCommonData);
                _mFrameCommonData.currentSelectedTabData.IsClicked = true;
            }
            else
            {
                _RefreshShopElementData();
            }
        }

        #endregion

        #region Filter Tabs

        private void OnFirstFilterElementItemTabClick(ShopNewFilterData shopNewFilterData)
        {
            if (_mFrameCommonData == null)
            {
                return;
            }

            _mFrameCommonData.currFirstFilterData = shopNewFilterData;

            _ResetCurrSelectFilterDatas();

            //请求当前选中的商店商品
            AccountShopDataManager.GetInstance().SendWorldAccountShopItemQueryReq(_mFrameCommonData);
        }

        private void OnSecondFilterElementItemTabClick(ShopNewFilterData shopNewFilterData)
        {
            if (_mFrameCommonData == null)
            {
                return;
            }

            _mFrameCommonData.currSecondFilterData = shopNewFilterData;

            _ResetCurrSelectFilterDatas();

            //请求当前选中的商店商品
            AccountShopDataManager.GetInstance().SendWorldAccountShopItemQueryReq(_mFrameCommonData);
        }

        private void _ResetCurrSelectFilterDatas()
        {
            if (_mFrameCommonData == null || _mFrameCommonData.currentSelectedTabData == null)
            {
                return;
            }
            _mFrameCommonData.currentSelectedTabData.FirstFilterData = _mFrameCommonData.currFirstFilterData;
            _mFrameCommonData.currentSelectedTabData.SecondFilterData = _mFrameCommonData.currSecondFilterData;
        }

        #endregion

        #region Element Item

        private int mCurSelectIndex = 0;
        private void OnElementItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_mFrameCommonData == null)
                return;

            var localItemInfos = _TryFilterShopElementData();
            if (localItemInfos == null)
            {
                Logger.LogErrorFormat("[AccountShopView] - get account shop data is null, shopId is {0}", _mFrameCommonData.shopId);
                return;
            }
            int iIndex = item.m_index;
            if (iIndex >= 0 && iIndex < localItemInfos.Length)
            {
                var elementItem = item.GetComponent<AccountShopElementItem>();
                if (elementItem == null)
                {
                    return;
                }

                var queryIndex = AccountShopDataManager.GetInstance().GetAccountShopQueryIndex(_mFrameCommonData);
                if (queryIndex != null)
                {
                    elementItem.InitElementItem(queryIndex.shopId, localItemInfos[iIndex]);
                    elementItem.SetSelect(item.m_index == mCurSelectIndex);
                }
            }
        }

        private void OnElementItemSelect(ComUIListElementScript item)
        {
            var elementItem = item.GetComponent<AccountShopElementItem>();
            var localItemInfos = _TryFilterShopElementData();
            if (localItemInfos == null)
            {
                Logger.LogErrorFormat("[AccountShopView] - get account shop data is null, shopId is {0}", _mFrameCommonData.shopId);
                return;
            }
            if (elementItem != null && item.m_index >= 0 && item.m_index < localItemInfos.Length)
            {
                var selectItem = shopItemList.GetElemenet(mCurSelectIndex);
                if (null != selectItem)
                {
                    var selectElementItem = selectItem.GetComponent<AccountShopElementItem>();
                    if (null != selectElementItem)
                        selectElementItem.SetSelect(false);
                }
                elementItem.SetSelect(true);
                InitMallContent(localItemInfos[item.m_index]);
                mCurSelectIndex = item.m_index;
            }
        }
        [SerializeField] private MallNewItemContent mContent;
        private void InitMallContent(Protocol.AccountShopItemInfo itemData)
        {
            mContent.OnInitAccountShopItem(itemData);
        }
        private void OnElementItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var elementItem = item.GetComponent<AccountShopElementItem>();
            if (elementItem != null)
            {
                elementItem.OnRecycleElementItem();
            }
        }

        private void _OnShopGoodsRefresh(UIEvent _uiEvent)
        {
            _RefreshShopElementData();
        }

        private void _RefreshShopElementData()
        {
            if (shopItemList != null)
            {
                var localItemInfos = _TryFilterShopElementData();
                if (localItemInfos == null || localItemInfos.Length == 0)
                {
                    if (shopNoGoodsLabel)
                    {
                        shopNoGoodsLabel.text = tr_bless_shop_no_goods_tip;
                        shopNoGoodsLabel.CustomActive(true);
                    }
                    shopItemList.SetElementAmount(0);
                }
                else
                {
                    shopNoGoodsLabel.CustomActive(false);
                    shopItemList.SetElementAmount(localItemInfos.Length);
                    shopItemList.ResetContentPosition();
                    mCurSelectIndex = 0;
                    //默认选中
                    if (0 != mDefaultItemId)
                    {
                        for (int index = 0; index < localItemInfos.Length; ++index)
                        {
                            if (localItemInfos[index].shopItemId == mDefaultItemId)
                            {
                                mCurSelectIndex = index;
                                break;
                            }
                        }
                        mDefaultItemId = 0;
                    }
                    shopItemList.SelectElement(mCurSelectIndex);
                    if (localItemInfos.Length > 0)
                        InitMallContent(localItemInfos[mCurSelectIndex]);
                    //刷新下次刷新时间倒计时
                    _RefreshShopItemRefreshTime(localItemInfos);
                    //刷新消耗品列表
                    _RefreshConsumeItemGroup(localItemInfos);
                }
            }
        }

        private Protocol.AccountShopItemInfo[] _TryFilterShopElementData()
        {
            var localItemInfos = AccountShopDataManager.GetInstance().GetAccountShopData(_mFrameCommonData);
            if(_mFrameCommonData != null)
            {
                //尝试本地过滤
                localItemInfos =
                ShopNewDataManager.GetInstance().TryFilterAccountShopItemInfos(localItemInfos, 
                                                                                    _mFrameCommonData.currFirstFilterData, 
                                                                                    _mFrameCommonData.currSecondFilterData);
            }
            return localItemInfos;
        }

        #region Common

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<AccountShopFrame>();
        }

        #endregion

        #endregion

        #endregion

        #region  PUBLIC METHODS

        private int mDefaultItemId = 0;
        public void InitShopView(ShopNewFrameViewData frameData)
        {
            if (frameData == null)
            {
                Logger.LogError("[AccountShopView] - InitShop View Failed, ShopNewFrameCommonData is null !");
                return;
            }
            this._mFrameCommonData = frameData;
            mDefaultItemId = frameData.defalutSelectShopItemId;
            _InitShopView();
        }

        #region PUBLIC STATIC METHODS
        #endregion

        #endregion
    }
}