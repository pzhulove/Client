using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class MagicCardMergeItem : MonoBehaviour
    {
        private ItemReward _itemReward;
        private CommonNewItem _commonNewItem;

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]

        [SerializeField] private GameObject itemRoot;

        private void OnDestroy()
        {
            _commonNewItem = null;
        }

        public void InitItem(ItemReward itemReward)
        {
            _itemReward = itemReward;

            if (_itemReward == null)
                return;

            var commonNewItemDataModel = new CommonNewItemDataModel()
            {
                ItemId = (int)_itemReward.id,
                ItemCount = (int)_itemReward.num,
                ItemStrengthenLevel = (int)_itemReward.strength,
            };

            if (itemRoot != null)
            {
                _commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                if (_commonNewItem == null)
                    _commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);
            }

            if (_commonNewItem != null)
                _commonNewItem.InitItem(commonNewItemDataModel);
        }

    }
}
