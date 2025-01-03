using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComDefaultBindItemIds : MonoBehaviour
    {
        [Header("材料合成默认展示, 除特殊物品")]
        public List<int> mergeBindItemIds = null;
        [Header("材料合成默认展示，特殊物品")]
        public List<int> specialMergeItemIds = null;
        [Header("融合默认展示")]
        public List<int> fuseBindItemIds = null;
        [Header("合成或融合时，只需要展示需要数量的道具ids")]
        public List<int> onlyShowNeedCountItemIds = null;
        [Header("前往获取默认")]
        public int getBindItemId = 0;
        [Header("融合强化券数目")]
        public int fuseTicketCount = 2;
        [Header("等待加载特效界面时间")]
        public float waitToLoadEffectPlane = 0.2f;
        [Header("等待材料合成界面选中时间")]
        public float waitToSelectMaterialFirstItem = 0.2f;
        [Header("合成或融合时，需要快捷购买的道具ids")]
        public List<int> needFastButItemIds = null;
    }
}