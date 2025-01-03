using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class StrengthGrowthBaseView : MonoBehaviour
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="linkData">跳转数据</param>
        /// <param name="type">强化增幅类型</param>
        public virtual void IniteData(SmithShopNewLinkData linkData,StrengthenGrowthType type,StrengthenGrowthView strengthenGrowthView)
        {

        }

        public virtual void OnEnableView() { }
        public virtual void OnDisableView() { }
    }
}

