using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;


namespace GameClient
{

    public delegate void OnAuctionNewTypeItemClick(AuctionNewMenuTabDataModel auctionNewMenuTabDataModel);

    public class AuctionNewBuyNoticeTypeItem : MonoBehaviour
    {

        private OnAuctionNewTypeItemClick _onAuctionNewTypeItemClick;
        private AuctionNewMenuTabDataModel _parentMenuTabDataModel;
        private AuctionNewMenuTabDataModel _curMenuTabDataModel;
        private AuctionNewMainTabType _mainTabType;
        

        [SerializeField] private Image typeIcon;
        [SerializeField] private Image typeImageFrame;
        [SerializeField] private Text typeName;
        [SerializeField] private Button typeItemButton;

        [Space(10)] [HeaderAttribute("ProfessionRoot")] [Space(10)]
        [SerializeField] private GameObject typeProfessionRoot;
        [SerializeField] private Text typeProfessionLabel;      //推荐职业页签

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
            if (typeItemButton != null)
            {
                typeItemButton.onClick.RemoveAllListeners();
                typeItemButton.onClick.AddListener(OnTypeItemButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if(typeItemButton != null)
                typeItemButton.onClick.RemoveAllListeners();
        }

        private void ResetData()
        {
            _onAuctionNewTypeItemClick = null;
            _parentMenuTabDataModel = null;
            _curMenuTabDataModel = null;
            _mainTabType = AuctionNewMainTabType.None;
        }

        public void InitItem(AuctionNewMenuTabDataModel parentMenuTabDataModel,
            AuctionNewMenuTabDataModel curMenuTabDataModel, 
            OnAuctionNewTypeItemClick onAuctionNewTypeItemClick = null,
            AuctionNewMainTabType auctionNewMainTabType = AuctionNewMainTabType.None)
        {
            _parentMenuTabDataModel = parentMenuTabDataModel;
            _curMenuTabDataModel = curMenuTabDataModel;
            _onAuctionNewTypeItemClick = onAuctionNewTypeItemClick;
            _mainTabType = auctionNewMainTabType;

            InitItemView();
        }

        private void InitItemView()
        {
           
            if (typeName != null)
                typeName.text = _curMenuTabDataModel.AuctionNewFrameTable.Name;

            //推荐职业不存在
            if (string.IsNullOrEmpty(_curMenuTabDataModel.AuctionNewFrameTable.RecommendedJob) == true)
            {
                CommonUtility.UpdateGameObjectVisible(typeProfessionRoot, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(typeProfessionRoot, true);
                CommonUtility.UpdateTextVisible(typeProfessionLabel, true);
                if (typeProfessionLabel != null)
                    typeProfessionLabel.text = _curMenuTabDataModel.AuctionNewFrameTable.RecommendedJob.ToString();
            }

            //类型Icon
            if (typeIcon != null)
            {
                //非空
                if(string.IsNullOrEmpty(_curMenuTabDataModel.AuctionNewFrameTable.IconPath) == false)
                {
                    ETCImageLoader.LoadSprite(ref typeIcon, _curMenuTabDataModel.AuctionNewFrameTable.IconPath);
                }
            }

            if (typeImageFrame != null)
            {
                //非空
                if(string.IsNullOrEmpty(_curMenuTabDataModel.AuctionNewFrameTable.IconPath) == false)
                {
                    ETCImageLoader.LoadSprite(ref typeImageFrame, _curMenuTabDataModel.AuctionNewFrameTable.BaseMap);
                }
            }

        }

        public void OnItemRecycle()
        {
            ResetData();
        }

        private void OnTypeItemButtonClick()
        {
            if(_curMenuTabDataModel == null)
                return;

            if(_onAuctionNewTypeItemClick == null)
                return;

            _onAuctionNewTypeItemClick(_curMenuTabDataModel);
        }

    }
}