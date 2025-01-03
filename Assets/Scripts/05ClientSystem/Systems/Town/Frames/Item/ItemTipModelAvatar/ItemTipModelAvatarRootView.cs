using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ItemTipModelAvatarRootView : MonoBehaviour
    {

        private ItemTipShowAvatarType _itemTipShowAvatarType;
        private ItemTipModelAvatarController _itemTipModelAvatarController;

        private ItemTable _itemTable;
        private int _itemTipModelAvatarLayerIndex = 0;

        private int _otherProfessionId = 0;

        private const float ShowModelAvatarDelayTime = 0.3f;            //单独一个模型加载延迟的时间
        private float _showModelAvatarDelayTimeInterval = -1.0f;

        private GiftPackModelAvatarDataModel _giftPackModelAvatarDataModel = null;
        
        [Space(10)] [HeaderAttribute("ItemTipModelAvatarRoot")] [Space(10)] [SerializeField]
        private GameObject tipModelAvatarRoot;


        private void OnDestroy()
        {
            _itemTable = null;
            _itemTipShowAvatarType = ItemTipShowAvatarType.None;
            _showModelAvatarDelayTimeInterval = -1.0f;
            _itemTipModelAvatarLayerIndex = 0;

            _giftPackModelAvatarDataModel = null;
            _otherProfessionId = 0;
        }

        #region CompareItemTip
        public void UpdateModelAvatarRootViewByCompareItemType(ItemTable itemTable, 
            ItemTipShowAvatarType itemTipShowAvatarType,
            int itemTipModelAvatarLayerIndex = 0,
            int otherProfessionId = 0)
        {
            _itemTipShowAvatarType = itemTipShowAvatarType;

            _itemTable = itemTable;
            _otherProfessionId = otherProfessionId;
            if (_itemTable == null)
                return;
            
            _itemTipModelAvatarLayerIndex = itemTipModelAvatarLayerIndex;

            OnShowModelAvatarView();
        }
        #endregion

        #region SingleItemTip

        //更新单独一个道具
        public void UpdateModelAvatarRootViewBySingleItemType(ItemTable itemTable,
            ItemTipShowAvatarType itemTipShowAvatarType,
            int itemTipModelAvatarLayerIndex,
            GiftPackModelAvatarDataModel giftPackModelAvatarDataModel = null,
            int otherProfessionId = 0)
        {
            ResetShowModelAvatarDelayTimeInterval();
            _itemTipShowAvatarType = itemTipShowAvatarType;

            _itemTable = itemTable;
            _giftPackModelAvatarDataModel = giftPackModelAvatarDataModel;

            _otherProfessionId = otherProfessionId;

            if (_itemTable == null)
                return;

            _itemTipModelAvatarLayerIndex = itemTipModelAvatarLayerIndex;
            _showModelAvatarDelayTimeInterval = 0.0f;
            
        }

        private void Update()
        {
            if (_itemTipShowAvatarType != ItemTipShowAvatarType.SingleTipType)
                return;

            if (_showModelAvatarDelayTimeInterval < 0.0f)
                return;

            _showModelAvatarDelayTimeInterval += Time.deltaTime;
            if (_showModelAvatarDelayTimeInterval >= ShowModelAvatarDelayTime)
            {
                OnShowModelAvatarView();
            }
        }
        #endregion

        //隐藏ModelAvatar
        public void OnDisappearShowModelAvatarView()
        {
            ResetShowModelAvatarDelayTimeInterval();

            //对Layer进行重置
            if(_itemTipModelAvatarController != null)
                _itemTipModelAvatarController.ResetModelAvatarEx();

            CommonUtility.UpdateGameObjectVisible(tipModelAvatarRoot, false);
        }

        //展示ModelAvatar
        public void OnShowModelAvatarView()
        {
            ResetShowModelAvatarDelayTimeInterval();
            ShowModelAvatarControllerView();
        }

        private void ShowModelAvatarControllerView()
        {
            if (tipModelAvatarRoot == null)
                return;

            CommonUtility.UpdateGameObjectVisible(tipModelAvatarRoot, true);
            //加载
            if (_itemTipModelAvatarController == null)
            {
                var tipModelAvatarPrefab = CommonUtility.LoadGameObject(tipModelAvatarRoot);
                if (tipModelAvatarPrefab != null)
                {
                    _itemTipModelAvatarController =
                        tipModelAvatarPrefab.GetComponent<ItemTipModelAvatarController>();
                }
            }

            //更新
            if (_itemTipModelAvatarController != null)
                _itemTipModelAvatarController.UpdateModelAvatarController(_itemTable,
                    _itemTipModelAvatarLayerIndex,
                    _giftPackModelAvatarDataModel,
                    _otherProfessionId);
        }

        private void ResetShowModelAvatarDelayTimeInterval()
        {
            _showModelAvatarDelayTimeInterval = -1.0f;
        }

    }
}

