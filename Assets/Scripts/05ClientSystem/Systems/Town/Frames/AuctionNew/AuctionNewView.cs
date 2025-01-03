using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public enum AuctionNewMainTabType
    {
        None = 0,
        AuctionBuyType = 1,       //我要购买
        AuctionNoticeType = 2,    //公示商品
        AuctionSellType = 3,      //我要寄售
    }

    [Serializable]
    public class AuctionNewMainTabDataModel
    {
        public int index;
        public AuctionNewMainTabType mainTabType;
        public string mainTabName;
    }


    public class AuctionNewView : MonoBehaviour
    {

        private int _defaultSelectedIndex = 1;

        private AuctionNewContentBaseView _buyContentView;
        private AuctionNewContentBaseView _noticeContentView;
        private AuctionNewContentBaseView _sellContentView;

        //初始化页签默认选中的数据，由外部传进来，初始化的时候使用一次
        private AuctionNewUserData _auctionNewUserData;
        //主页签实际显示的数据
        private List<AuctionNewMainTabDataModel> _showMainTabDataModelList = new List<AuctionNewMainTabDataModel>();
        //是否显示关注页签
        private bool _isShowNoticeTab = false;

        [Space(5)]
        [HeaderAttribute("Title")]
        [SerializeField] private Text titleLabel = null;
        [SerializeField] private ComUIListScript mainTabItemList;

        [Space(5)]
        [HeaderAttribute("Button")]
        [SerializeField] private Button closeButton;

        [Space(15)] [HeaderAttribute("AuctionNewMainTabData")]
        [SerializeField] private List<AuctionNewMainTabDataModel> mainTabDataModelList = new List<AuctionNewMainTabDataModel>();

        [Space(15)] [HeaderAttribute("AuctionNewContent")]
        [SerializeField] private GameObject buyContentRoot;
        [SerializeField] private GameObject noticeContentRoot;
        [SerializeField] private GameObject sellContentRoot;

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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (mainTabItemList != null)
            {
                mainTabItemList.Initialize();
                mainTabItemList.onItemVisiable += OnMainTabItemVisible;
            }
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (mainTabItemList != null)
                mainTabItemList.onItemVisiable -= OnMainTabItemVisible;
        }

        private void ClearData()
        {
            _defaultSelectedIndex = 0;
            _buyContentView = null;
            _noticeContentView = null;
            _sellContentView = null;
            _auctionNewUserData = null;
            _showMainTabDataModelList = null;
            _isShowNoticeTab = false;
        }

        //需要传递默认的参数
        public void InitView(AuctionNewUserData auctionNewUserData = null)
        {
            _auctionNewUserData = auctionNewUserData;
            InitBaseView();

            InitAuctionContent();
        }

        private void InitBaseView()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("auction_new_title");

        }

        private void InitAuctionContent()
        {
            if(mainTabDataModelList == null)
                return;

            var mainTabNumber = mainTabDataModelList.Count;
            if(mainTabNumber <= 0)
                return;

            InitShowMainTabData();

            SetDefaultSelectedIndex();

            if (mainTabItemList != null)
            {
                mainTabItemList.SetElementAmount(_showMainTabDataModelList.Count);
            }
        }

        private void InitShowMainTabData()
        {
            //是否显示公示页签的标志，默认显示，数值由服务器同步
            _isShowNoticeTab = true;

            //珍品机制没有打开, 不显示公示页签
            if (AuctionNewUtility.IsAuctionTreasureItemOpen() == false)
                _isShowNoticeTab = false;
            
            for (var i = 0; i < mainTabDataModelList.Count; i++)
            {
                var mainTabDataModel = mainTabDataModelList[i];
                if (mainTabDataModel == null)
                    continue;

                //不显示公示页签,且当前数据为公示商品类型
                if (_isShowNoticeTab == false && mainTabDataModel.mainTabType == AuctionNewMainTabType.AuctionNoticeType)
                    continue;

                //实际显示的页签内容
                var showMainTabDataModel = new AuctionNewMainTabDataModel
                {
                    index = mainTabDataModel.index,
                    mainTabType = mainTabDataModel.mainTabType,
                    mainTabName = mainTabDataModel.mainTabName,
                };
                _showMainTabDataModelList.Add(showMainTabDataModel);
            }
        }

        //设置默认的选择按钮
        private void SetDefaultSelectedIndex()
        {
            if (_auctionNewUserData == null)
            {
                _defaultSelectedIndex = 0;
                return;
            }
            _defaultSelectedIndex = 0;

            for (var i = 0; i < _showMainTabDataModelList.Count; i++)
            {
                var tabDataModel = _showMainTabDataModelList[i];
                if(tabDataModel == null)
                    continue;

                if (tabDataModel.mainTabType == _auctionNewUserData.MainTabType)
                {
                    _defaultSelectedIndex = i;
                    return;
                }
            }
        }

        private void OnMainTabItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var mainTabItem = item.GetComponent<AuctionNewMainTabItem>();
            if (mainTabItem == null)
                return;

            if (_showMainTabDataModelList != null
                && item.m_index >= 0 && item.m_index < _showMainTabDataModelList.Count)
            {
                var mainTabDataModel = _showMainTabDataModelList[item.m_index];
                if (mainTabDataModel != null)
                {
                    if (_defaultSelectedIndex == item.m_index)
                    {
                        mainTabItem.Init(mainTabDataModel,
                            OnMainTabClicked,
                            true);
                    }
                    else
                    {
                        mainTabItem.Init(mainTabDataModel, 
                            OnMainTabClicked,
                            false);
                    }
                }
            }
        }

        #region MainTabClicked
        private void OnMainTabClicked(AuctionNewMainTabType mainTabType)
        {
            ResetContentRoot();

            switch (mainTabType)
            {
                case AuctionNewMainTabType.AuctionSellType:
                    OnSellTabClicked(mainTabType);
                    break;
                case AuctionNewMainTabType.AuctionBuyType:
                    OnBuyTabClicked(mainTabType);
                    break;
                case AuctionNewMainTabType.AuctionNoticeType:
                    OnNoticeTabClicked(mainTabType);
                    break;
            }

            AuctionNewDataManager.GetInstance().SetLastTimeUserDataMainTabType(mainTabType);
        }

        //我要购买界面
        private void OnBuyTabClicked(AuctionNewMainTabType mainTabType)
        {
            //我要购买页面
            if (buyContentRoot != null && buyContentRoot.activeSelf == false)
                buyContentRoot.CustomActive(true);

            if (_buyContentView == null)
            {
                //第一次加载
                _buyContentView = LoadContentBaseView(buyContentRoot);
                _buyContentView.InitView(mainTabType, _auctionNewUserData);
                _auctionNewUserData = null;
            }
            else
            {
                _buyContentView.OnEnableView(mainTabType);
            }
        }

        //寄售界面
        private void OnSellTabClicked(AuctionNewMainTabType mainTabType)
        {
            //寄售
            if (sellContentRoot != null && sellContentRoot.activeSelf == false)
                sellContentRoot.CustomActive(true);

            if (_sellContentView == null)
            {
                //第一次加载
                _sellContentView = LoadContentBaseView(sellContentRoot);
                _sellContentView.InitView(mainTabType, _auctionNewUserData);
                _auctionNewUserData = null;
            }
            else
            {
                if (_sellContentView != null)
                    _sellContentView.OnEnableView(mainTabType);
            }

        }

        //关注页面
        private void OnNoticeTabClicked(AuctionNewMainTabType mainTabType)
        {
            //关注页面
            if (noticeContentRoot != null && noticeContentRoot.activeSelf == false)
                noticeContentRoot.CustomActive(true);

            if (_noticeContentView == null)
            {
                _noticeContentView = LoadContentBaseView(noticeContentRoot);
                _noticeContentView.InitView(mainTabType, _auctionNewUserData);
                _auctionNewUserData = null;
            }
            else
            {
                _noticeContentView.OnEnableView(mainTabType);
            }

        }

        private void ResetContentRoot()
        {
            if (buyContentRoot != null)
                buyContentRoot.CustomActive(false);

            if (noticeContentRoot != null)
                noticeContentRoot.CustomActive(false);

            if (sellContentRoot != null)
                sellContentRoot.CustomActive(false);
        }
        #endregion

        private AuctionNewContentBaseView LoadContentBaseView(GameObject contentRoot)
        {
            if (contentRoot == null)
                return null;

            AuctionNewContentBaseView contentBaseView = null;

            var uiPrefabWrapper = contentRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(contentRoot.transform, false);

                    contentBaseView = uiPrefab.GetComponent<AuctionNewContentBaseView>();
                }
            }

            return contentBaseView;
        }

        private void OnCloseFrame()
        {
            AuctionNewDataManager.GetInstance().ResetAuctionNewItemIdDictionary();
            ClientSystemManager.GetInstance().CloseFrame<AuctionNewFrame>();
        }
        
    }
}