using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class ComTwoLevelToggleControl : MonoBehaviour
    {

        [Space(5)] [HeaderAttribute("TwoLevelControl")] [Space(5)]
        [SerializeField] private GameObject twoLevelToggleRoot;
        [SerializeField] private GameObject twoLevelToggleItemPrefab;

        //初始化：设置数据和回调
        public void InitTwoLevelToggleControl(List<ComTwoLevelToggleData> twoLevelToggleDataList,
            OnComToggleClick _onFirstLevelToggleClick = null,
            OnComToggleClick _onSecondLevelToggleClick = null)
        {
            if (twoLevelToggleRoot == null || twoLevelToggleItemPrefab == null)
            {
                Logger.LogErrorFormat("ComTwoLevelToggleControl root or prefab is null");
                return;
            }

            if (twoLevelToggleDataList == null || twoLevelToggleDataList.Count <= 0)
            {
                Logger.LogErrorFormat("ComTwoLevelToggleControl twoLevelToggleDataList is null");
                return;
            }

            for (var i = 0; i < twoLevelToggleDataList.Count; i++)
            {
                var twoLevelToggleData = twoLevelToggleDataList[i];
                if(twoLevelToggleData == null
                   || twoLevelToggleData.FirstLevelToggleData == null)
                    continue;

                var twoLevelToggleItemGo = GameObject.Instantiate(twoLevelToggleItemPrefab) as GameObject;
                if (twoLevelToggleItemGo != null)
                {
                    twoLevelToggleItemGo.CustomActive(true);
                    Utility.AttachTo(twoLevelToggleItemGo, twoLevelToggleRoot);
                    twoLevelToggleItemGo.transform.name = twoLevelToggleItemGo.transform.name + "_" + (i + 1);

                    var twoLevelToggleItem = twoLevelToggleItemGo.GetComponent<ComTwoLevelToggleItem>();
                    if (twoLevelToggleItem != null)
                    {
                        twoLevelToggleItem.InitItem(twoLevelToggleData,
                            _onFirstLevelToggleClick,
                            _onSecondLevelToggleClick);
                    }
                }
            }
        }
    }
}