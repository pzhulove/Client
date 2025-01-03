using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{


    public class AuctionNewMagicCardStrengthenLevelView : MonoBehaviour
    {

        private AuctionNewMagicCardDataModel _magicCardDataModel;

        private List<AuctionNewMagicCardStrengthenLevelDataModel> _magicCardStrengthenLevelDataList
            = new List<AuctionNewMagicCardStrengthenLevelDataModel>();

        private int _selectedLevel = -1;
        private int _itemId = -1;

        [Space(15)]
        [HeaderAttribute("AuctionNewMagicCardItemList")]
        [Space(5)]
        [SerializeField] private Button closeButton;
        [SerializeField] private ComUIListScriptEx magicCardStrengthLevelItemList;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ResetData();
        }

        private void ResetData()
        {
            _magicCardDataModel = null;
            _magicCardStrengthenLevelDataList.Clear();
        }

        private void BindEvents()
        {

            if (magicCardStrengthLevelItemList != null)
            {
                magicCardStrengthLevelItemList.Initialize();
                magicCardStrengthLevelItemList.onItemVisiable += OnItemVisible;
                magicCardStrengthLevelItemList.OnItemUpdate += OnItemUpdate;
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

        }

        private void UnBindEvents()
        {

            if (magicCardStrengthLevelItemList != null)
            {
                magicCardStrengthLevelItemList.onItemVisiable -= OnItemVisible;
                magicCardStrengthLevelItemList.OnItemUpdate -= OnItemUpdate;
            }

            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();

        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryMagicCardOnSaleResSucceed,
                OnReceiveMagicCardOnSaleRes);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryMagicCardOnSaleResSucceed,
                OnReceiveMagicCardOnSaleRes);
        }


        //需要传递默认的参数
        public void Init(AuctionNewMagicCardDataModel dataModel)
        {
            _selectedLevel = -1;
            _itemId = -1;

            _magicCardDataModel = dataModel;
            if (_magicCardDataModel == null)
                return;

            _itemId = (int)_magicCardDataModel.ItemId;
            _selectedLevel = _magicCardDataModel.DefaultLevel;

            SendMagicCardOnSaleReq();
        }
        
        private void SendMagicCardOnSaleReq()
        {
            //发送拉去交易记录的信息
            AuctionNewDataManager.GetInstance().SendAuctionNewMagicCardOnSaleReq(_magicCardDataModel.ItemId);
        }

        private void OnReceiveMagicCardOnSaleRes(UIEvent uiEvent)
        {
            var onSaleRes = AuctionNewDataManager.GetInstance().GetAuctionNewMagicCardOnSaleRes();

            _magicCardStrengthenLevelDataList.Clear();

            if (onSaleRes != null 
                && onSaleRes.magicOnsales != null 
                && onSaleRes.magicOnsales.Length > 0)
            {
                for (var i = 0; i < onSaleRes.magicOnsales.Length; i++)
                {
                    var curSale = onSaleRes.magicOnsales[i];
                    if (curSale != null)
                    {
                        var magicCardStrengthenLevelDataModel = new AuctionNewMagicCardStrengthenLevelDataModel()
                        {
                            StrengthenLevel = curSale.strength,
                            Number = (int) curSale.num,
                        };

                        if (curSale.strength == _magicCardDataModel.DefaultLevel)
                            magicCardStrengthenLevelDataModel.IsSelected = true;

                        _magicCardStrengthenLevelDataList.Add(magicCardStrengthenLevelDataModel);
                    }
                }
            }

            SortMagicCardStrengthenLevelDataList();

            if (magicCardStrengthLevelItemList != null)
                magicCardStrengthLevelItemList.SetElementAmount(_magicCardStrengthenLevelDataList.Count);
        }

        private void SortMagicCardStrengthenLevelDataList()
        {
            //有在售排在前面，等级低的排在前面
            if (_magicCardStrengthenLevelDataList == null || _magicCardStrengthenLevelDataList.Count <= 1)
                return;

            _magicCardStrengthenLevelDataList.Sort((x, y) =>
            {
                if (x.Number > 0 && y.Number > 0)
                {
                    return x.StrengthenLevel.CompareTo(y.StrengthenLevel);
                }
                else if (x.Number > 0)
                {
                    return -1;
                }
                else
                {
                    if (y.Number > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return x.StrengthenLevel.CompareTo(y.StrengthenLevel);
                    }
                }
            });
        }

        
        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_magicCardStrengthenLevelDataList == null)
                return;

            if (item.m_index < 0 || item.m_index > _magicCardStrengthenLevelDataList.Count)
                return;

            var magicCardStrengthenLevelDataModel = _magicCardStrengthenLevelDataList[item.m_index];
            var magicCardStrengthenLevelItem = item.GetComponent<AuctionNewMagicCardStrengthenLevelItem>();

            if (magicCardStrengthenLevelDataModel != null
                && magicCardStrengthenLevelItem != null)
            {
                magicCardStrengthenLevelItem.InitItem(magicCardStrengthenLevelDataModel,
                    OnMagicCardItemClick);
            }
        }

        private void OnMagicCardItemClick(int strengthenLevel)
        {
            _selectedLevel = strengthenLevel;

            for (var i = 0; i < _magicCardStrengthenLevelDataList.Count; i++)
            {
                var curMagicCardData = _magicCardStrengthenLevelDataList[i];
                if (curMagicCardData != null)
                {
                    if (curMagicCardData.StrengthenLevel == _selectedLevel)
                    {
                        curMagicCardData.IsSelected = true;
                    }
                    else
                    {
                        curMagicCardData.IsSelected = false;
                    }
                }
            }

            if(magicCardStrengthLevelItemList != null)
                magicCardStrengthLevelItemList.UpdateElement();

            OnCloseFrame();
        }

        private void OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var magicCardStrengthenLevelItem = item.GetComponent<AuctionNewMagicCardStrengthenLevelItem>();
            if (magicCardStrengthenLevelItem != null)
                magicCardStrengthenLevelItem.UpdateMagicCardItem();
        }


        private void OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewSelectMagicCardStrengthenLevel,
                _itemId,
                _selectedLevel);

            AuctionNewUtility.OnCloseAuctionNewMagicCardStrengthLevelFrame();
        }

    }
}