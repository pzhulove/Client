using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameClient
{
  
    /// <summary>
    /// 限时活动兑换的参数
    /// </summary>
    public class MallActivityExchangeParams : MonoBehaviour
    {
        [Serializable]
        public class ExchangeGoodParams
        {
            public int ShopId;
            public int GoodId;
            public int Count;//要买的商品数量
        }

        public List<ExchangeGoodParams> ExchangeGoodParamList = new List<ExchangeGoodParams>();

    }
}
