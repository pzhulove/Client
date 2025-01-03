using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace AssetBundleTool
{
    public static class DetectStrategyFile
    {
        private static List<string> GetWithinTopDir(string localPath) {
            List<string> allPath = new List<string>();
            DirectoryInfo dirs = new DirectoryInfo(Application.dataPath +localPath ); 
            DirectoryInfo[] dir = dirs.GetDirectories();
            foreach (var i in dir) {
                string p = i.FullName.Substring(Application.dataPath.Length-6).Replace("\\", "/");
                allPath.Add(p);
            }
            return allPath;
        }

        private static void removeBaseFolder(List<StrategyDataBase> storeStrategy)
        {
            for (int i=0;i< storeStrategy.Count;i++) {
                if (storeStrategy[i].strategyPath.Contains("/Base")) {
                    storeStrategy.Remove(storeStrategy[i]);
                }
            }
        }

        public static List<StrategyDataBase> CompareVsStoreStrategy(List<StrategyDataBase> storeStrategy)
        {
            List<string> allTopPaths = GetWithinTopDir("/Resources");
            List<string> storeList = GetStoreDir(storeStrategy);
            AddNotStrategy(storeStrategy, allTopPaths, storeList);
            CheckUnexclude(storeStrategy, storeList);
            removeBaseFolder(storeStrategy);
            return storeStrategy;
        }

        private static void CheckUnexclude(List<StrategyDataBase> storeStrategy, List<string> storeList)
        {
            foreach (var i in storeList) {
                for (int j = 0; j < storeStrategy.Count; j++) {
                    if (i.Contains(storeStrategy[j].strategyPath) && i.Length > storeStrategy[j].strategyPath.Length && !storeStrategy[j].excludePath.Contains(i)) {
                        storeStrategy.Remove(storeStrategy.Where(x => x.strategyPath == i).ToArray()[0]);
                    }
                }
            }
        }

        private static void AddNotStrategy(List<StrategyDataBase> storeStrategy, List<string> allTopPaths, List<string> storeList)
        {
            for (int i = 0; i < allTopPaths.Count; i++)
            {
                if (!storeList.Contains(allTopPaths[i]))
                {
                    Add(storeStrategy, allTopPaths[i]);
                }
            }
        }

        private static List<string> GetStoreDir(List<StrategyDataBase> storeStrategy)
        {
            List<string> dic = new List<string>();
            for (int i = 0; i < storeStrategy.Count; i++)
            {
                dic.Add(storeStrategy[i].strategyPath);
            }
            for (int i = 0; i < storeStrategy.Count; i++)
            {
                if (storeStrategy[i].excludePath.Count != 0)
                {
                    foreach (var ii in storeStrategy[i].excludePath)
                    {
                        if (!dic.Contains(ii))
                        {
                            Add(storeStrategy, ii);
                        }
                    }
                }
            }
            return dic;
        }
        
        private static void Add(List<StrategyDataBase> storeStrategy, string i)
        {
            StrategyDataBase d = new StrategyByFileData();
            d.strategyPath = i;
            d.defaultBool = true;
            d.strategyId = SmallFunc.GetTimeStamp();
            d.strateggy.Init(typeof(StrategyByDirectoryLevel).FullName);
            d.strateggy.Refresh();
            storeStrategy.Add(d);
        }
    }
}
