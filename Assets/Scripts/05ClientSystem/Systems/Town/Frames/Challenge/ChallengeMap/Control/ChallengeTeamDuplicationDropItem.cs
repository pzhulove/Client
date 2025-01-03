using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class ChallengeTeamDuplicationDropItem : MonoBehaviour
    {

        private CommonNewItem _commonNewItem;

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]
        [SerializeField] private GameObject itemRoot;

        private void OnDestroy()
        {
            _commonNewItem = null;
        }

        public void InitItem(int itemId, int itemNumber = 1)
        {

            if (itemId <= 0 || itemNumber <= 0)
            {
                Logger.LogErrorFormat(
                    "ChallengeTeamDuplicationDropItem Data is invalid and itemId is {0}, itemNumber is {1}",
                    itemId, itemNumber);
                return;
            }


            var commonNewItemDataModel = new CommonNewItemDataModel()
            {
                ItemId = itemId,
                ItemCount = itemNumber,
            };

            if (itemRoot != null)
            {
                _commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                if (_commonNewItem == null)
                    _commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);
            }

            if (_commonNewItem != null)
                _commonNewItem.InitItem(commonNewItemDataModel);

            if (_commonNewItem != null)
            {
                _commonNewItem.SetItemCountFontSize(32);
                _commonNewItem.SetItemLevelFontSize(32);
            }
        }
    }
}
