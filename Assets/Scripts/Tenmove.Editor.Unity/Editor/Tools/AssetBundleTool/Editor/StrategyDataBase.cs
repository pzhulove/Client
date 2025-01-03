using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace AssetBundleTool
{
    [Serializable]
    [XmlInclude(typeof(StrategyByFileData))]
    public  class StrategyDataBase
    {
        public long strategyId;//策略ID 每一个策略ID不同
        public bool enable=true;//策略ID 每一个策略ID不同
        public string strategyPath;//策略路径
        public bool defaultBool;
        public TypeDropList_EditorWindow<StrategyBase> strateggy = new TypeDropList_EditorWindow<StrategyBase>();
        public List<string> excludePath = new List<string>();//排除路径
        public  EAssetType m_EAssetType;

        public virtual void Init() { }
        public virtual void LoadSetting() { }
    }
}
