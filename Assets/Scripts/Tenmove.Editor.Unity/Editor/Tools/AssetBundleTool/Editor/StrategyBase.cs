using AssetBundleTool;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetBundleTool
{
    public abstract class StrategyBase
    {
        public abstract void OnInit();
        public abstract void OnDraw(Rect pos,StrategyDataBase data);
       
        public abstract void OnExcute(StrategyDataBase data,Dictionary<long, List<AssetPackageDesc>> sm_strategyDataDic);
    }
}
