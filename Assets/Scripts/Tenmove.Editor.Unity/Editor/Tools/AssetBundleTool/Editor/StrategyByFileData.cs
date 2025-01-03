using System;
using System.Collections.Generic;
using System.IO;
namespace AssetBundleTool
{
    [Serializable]
    public class StrategyByFileData:StrategyDataBase
    {
        public int m_IncludeMask = 0;
        public List<extStructs> IncludeMask = new List<extStructs>();
        public int assetType = 0;//资源类型
        public int packLevel = 0;//打包层级
        public string bundleExtensionName = "";//boundle后缀名
        public class extStructs {
            public string extString;
            public bool extBool;
            public extStructs() {
                extString = "";
                extBool = true;
            }
            public extStructs(string extString, bool extBool)
            {
                this.extString = extString;
                this.extBool = extBool;
            }
        }
        public override void Init()
        {
            CheckNew();
        }
        public override void LoadSetting()
        {
            if (strateggy.m_TypeName == "")
            {
                strateggy.Init(StrategyView.sm_selectStrategy);
            }
            else
            {
                strateggy.Init(strateggy.m_TypeName);
            }
            strateggy.Refresh();
        }
        public void CheckNew() {
            if (!Directory.Exists(strategyPath))
            {
                return;
            }
            string[] filesList = Directory.GetFiles(strategyPath, "*.*", SearchOption.AllDirectories);
            Dictionary<string,bool> extsDic = new Dictionary<string, bool>();
            for (int i = 0; i < filesList.Length; i++)
            {
                if (Path.GetExtension(filesList[i]) != ".meta")
                {
                    string ext = Path.GetExtension(filesList[i]);
                    if (!extsDic.ContainsKey(ext))
                    {
                        extsDic.Add(ext,true);
                    }
                }
            }
            foreach (var i in IncludeMask) {
                if (extsDic.ContainsKey(i.extString)) {
                    extsDic[i.extString] = i.extBool;
                }
            }
            IncludeMask = new List<extStructs>();
            foreach (var i in extsDic) {
                IncludeMask.Add(new extStructs( i.Key, i.Value));
            }
        }

        public bool Valid(string ext)
        {
            foreach (var i in IncludeMask) {
                if (i.extString == ext && i.extBool)
                    return true;
            }
            return false;
        }
        public bool ExcludePath(string path)
        {
            for (int i = 0; i < excludePath.Count; ++i)
            {
                if (path.Contains(excludePath[i]))
                    return true;
            }
            return false;
        }
    }
}