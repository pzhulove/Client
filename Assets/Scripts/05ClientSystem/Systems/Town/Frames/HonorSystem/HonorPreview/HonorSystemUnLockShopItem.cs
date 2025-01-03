using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemUnLockShopItem : MonoBehaviour
    {

        private int _shopItemId;

        [Space(10)]
        [HeaderAttribute("Root")]
        [Space(10)]
        [SerializeField] private GameObject shopItemRoot;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _shopItemId = 0;
        }

        public void InitUnLockShopItem(int shopItemId)
        {
            _shopItemId = shopItemId;
            if (_shopItemId <= 0)
                return;

            InitItemView();
        }

        private void InitItemView()
        {
            if (shopItemRoot == null)
                return;

            var commonNewItem = shopItemRoot.GetComponentInChildren<CommonNewItem>();
            if (commonNewItem == null)
            {
                commonNewItem = CommonUtility.CreateCommonNewItem(shopItemRoot);
            }
            else
            {
                commonNewItem.Reset();
            }

            if (commonNewItem != null)
            {
                commonNewItem.InitItem(_shopItemId);
                commonNewItem.SetItemLevelFontSize(28);
            }
        }

        public void OnRecycleItem()
        {
            _shopItemId = 0;
        }


    }
}