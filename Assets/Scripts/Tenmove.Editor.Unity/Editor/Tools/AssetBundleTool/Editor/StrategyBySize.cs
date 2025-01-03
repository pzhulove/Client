using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AssetBundleTool
{
    public class StrategyBySize : StrategyBase
    {
        

        public override void OnDraw(Rect position, StrategyDataBase data)
        {
            Debug.Log("StrategyBySize Excute");
        }

        public override void OnExcute(StrategyDataBase data, Dictionary<long, List<AssetPackageDesc>> sm_strategyDataDic)
        {
            Debug.Log("StrategyBySize OnExcute");
        }

        public override void OnInit()
        {
            Debug.Log("StrategyBySize OnInit");
        }
    }
}
