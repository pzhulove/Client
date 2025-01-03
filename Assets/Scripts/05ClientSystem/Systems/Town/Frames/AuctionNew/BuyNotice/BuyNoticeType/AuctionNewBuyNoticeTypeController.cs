using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public delegate void OnBuyNoticeTypeItemClick(AuctionNewMenuTabDataModel auctionNewMenuTabDataModel);
    public class AuctionNewBuyNoticeTypeController : AuctionNewBuyNoticeBaseController
    {
        private AuctionNewMenuTabDataModel _auctionNewMenuTabDataModel;
        private AuctionNewMainTabType _mainTabType;
        private List<AuctionNewMenuTabDataModel> childrenMenuTabDataModelList;

        private OnBuyNoticeTypeItemClick _onBuyNoticeTypeItemClick;
        private OnUpdateFilterBackground _onUpdateFilterBackground;


        [Space(10)]
        [HeaderAttribute("ItemList")]
        [SerializeField] private ComUIListScript typeItemList;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ResetData();
        }

        private void BindEvents()
        {
            if (typeItemList != null)
            {
                typeItemList.Initialize();
                typeItemList.onItemVisiable += OnTypeItemVisible;
                typeItemList.OnItemRecycle += OnTypeItemRecycle;
            }
        }

        private void UnBindEvents()
        {
            if (typeItemList != null)
            {
                typeItemList.onItemVisiable -= OnTypeItemVisible;
                typeItemList.OnItemRecycle -= OnTypeItemRecycle;
            }
        }

        private void ResetData()
        {
            _auctionNewMenuTabDataModel = null;
            _onBuyNoticeTypeItemClick = null;
            _mainTabType = AuctionNewMainTabType.None;
            childrenMenuTabDataModelList = null;
        }

        public void InitTypeControllerData(OnBuyNoticeTypeItemClick onBuyNoticeTypeItemClick = null,
            OnUpdateFilterBackground onUpdateFilterBackground = null)
        {
            _onBuyNoticeTypeItemClick = onBuyNoticeTypeItemClick;
            _onUpdateFilterBackground = onUpdateFilterBackground;
        }

        public void OnEnableTypeController(AuctionNewMenuTabDataModel auctionNewMenuTabDataModel,
            AuctionNewMainTabType auctionNewMainTabType = AuctionNewMainTabType.None)
        {
            //二级节点的数据，展示三级节点
            _auctionNewMenuTabDataModel = auctionNewMenuTabDataModel;
            _mainTabType = auctionNewMainTabType;

            childrenMenuTabDataModelList = AuctionNewUtility.GetAuctionNewChildrenLayerTabDataModelList(
                _auctionNewMenuTabDataModel,
                _mainTabType);
            
            if (childrenMenuTabDataModelList == null
                || childrenMenuTabDataModelList.Count <= 0)
            {
                SetTypeItemList(0);
                Logger.LogErrorFormat("OnEnableTypeController AuctionNewMenuTabDataModel List is null");
                return;
            }

            //更新背景
            if (_onUpdateFilterBackground != null)
                _onUpdateFilterBackground(false);

            var childrenTabDataCount = childrenMenuTabDataModelList.Count;
            SetTypeItemList(childrenTabDataCount);
            
        }

        private void SetTypeItemList(int count)
        {
            if (typeItemList != null)
            {
                typeItemList.ResetContentPosition();
                typeItemList.SetElementAmount(count);
            }
        }
        
        private void OnTypeItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var typeItem = item.GetComponent<AuctionNewBuyNoticeTypeItem>();
            if(typeItem == null)
                return;

            if (item.m_index >= 0
                && item.m_index < _auctionNewMenuTabDataModel.AuctionNewFrameTable.LayerRelation.Count)
            {
                var curMenuTabDateModel = childrenMenuTabDataModelList[item.m_index];
                typeItem.InitItem(_auctionNewMenuTabDataModel,
                    curMenuTabDateModel,
                    OnItemClick,
                    _mainTabType);
            }

        }

        private void OnTypeItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;
            var typeItem = item.GetComponent<AuctionNewBuyNoticeTypeItem>();
            if(typeItem != null)
                typeItem.OnItemRecycle();
        }

        private void OnItemClick(AuctionNewMenuTabDataModel auctionNewMenuTabDataModel)
        {
            if (_onBuyNoticeTypeItemClick != null)
                _onBuyNoticeTypeItemClick(auctionNewMenuTabDataModel);
        }

    }
}