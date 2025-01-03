using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class WeekSignInPreviewAwardItem : MonoBehaviour
    {
        private WeekSignInPreviewAwardDataModel _previewAwardItemDataModel = null;

        private CommonNewItem _commonNewItem;

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]

        [SerializeField] private GameObject itemRoot;
        [SerializeField] private GameObject specialFlag;

        private void OnDestroy()
        {
            _previewAwardItemDataModel = null;
            _commonNewItem = null;
        }

        public void InitItem(WeekSignInPreviewAwardDataModel previewAwardItemDataModel)
        {
            _previewAwardItemDataModel = previewAwardItemDataModel;
            if (_previewAwardItemDataModel == null)
            {
                Logger.LogErrorFormat("WeekSingInPreviewAwardItem itemDataModel is null");
                return;
            }

            var commonNewItemDataModel = new CommonNewItemDataModel()
            {
                ItemId = _previewAwardItemDataModel.ItemId,
                ItemCount = _previewAwardItemDataModel.ItemNumber
            };

            if (itemRoot != null)
            {
                _commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                if (_commonNewItem == null)
                    _commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);
            }

            if(_commonNewItem != null)
                _commonNewItem.InitItem(commonNewItemDataModel);

            if (_commonNewItem != null)
            {
                _commonNewItem.SetItemCountFontSize(32);
                _commonNewItem.SetItemLevelFontSize(32);
            }
            
            if (specialFlag != null)
            {
                specialFlag.CustomActive(_previewAwardItemDataModel.IsSpecialAward == true);
            }
        }

    }
}
