using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class AssetProcessProfile
{
    public enum AssetType
    {
        Texture,
        Mesh,
        Audio,
        Animation,
        AssetTypeNum
    }

    public const string configSavePath ="Editor/AssetProcessor/Config/AssetProcessProfile.xml";
    public static Dictionary<AssetType, List<BaseAssetProcessorData>> m_AssetProfile = new Dictionary<AssetType, List<BaseAssetProcessorData>>();

    protected static bool m_NeedSave = false;
    protected static bool m_Loaded = false;

    public static bool IsLoaded()
    {
        return m_Loaded;
    }

    public static BaseAssetProcessorData CreateAssetStrategy(AssetType assetType)
    {
        switch(assetType)
        {
            case AssetType.Texture:
                return new TextureProcessorData();
            case AssetType.Audio:
                return new AudioProcessorData();
            default:
                return null;
        }
    }

    public static List<BaseAssetProcessorData> GetAssetProfile(AssetType assetType)
    {
        List<BaseAssetProcessorData> outProcessor = null ;
        if(!m_AssetProfile.TryGetValue(assetType, out outProcessor))
        {
            outProcessor = new List<BaseAssetProcessorData>();
            m_AssetProfile.Add(assetType, outProcessor);
        }

        return outProcessor;
    }

    public static void ClearProfile()
    {
        m_AssetProfile.Clear();
        m_Loaded = false;
    }

    public static void LoadAssetProfile()
    {
        m_Loaded = true;

        TextAsset xmlAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + configSavePath);
        if (xmlAsset == null)
            return;

        ClearProfile();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlAsset.text);

        var xmlRoot = xmlDoc.SelectSingleNode("Root");
        if (xmlRoot != null)
        {
            // 每种类型
            foreach (XmlNode xmlAssetType in xmlRoot.ChildNodes)
            {
                if (xmlAssetType.LocalName == "AssetType")
                {
                    string assetTypeName = (xmlAssetType as XmlElement).GetAttribute("name");

                    AssetType assetType = (AssetType)Enum.Parse(typeof(AssetType), assetTypeName);

                    List<BaseAssetProcessorData> assetStratagies;
                    if(!m_AssetProfile.TryGetValue(assetType, out assetStratagies))
                    {
                        assetStratagies = new List<BaseAssetProcessorData>();
                        m_AssetProfile.Add(assetType, assetStratagies);
                    }

                    // 每种strategy
                    foreach (XmlNode xmlStrategy in xmlAssetType.ChildNodes)
                    {
                        if (xmlStrategy.LocalName == "Strategy")
                        {
                            BaseAssetProcessorData processor = CreateAssetStrategy(assetType);
                            if (processor != null)
                            {
                                processor.LoadFromXML(xmlStrategy);

                                assetStratagies.Add(processor);
                            }
                        }
                    }

                }
            }
        }

        m_Loaded = true;
    }

    public static void SaveAssetProfile()
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement xmlRoot = xmlDoc.CreateElement("Root");

        if (xmlRoot != null)
        {
            // 每种类型
            foreach (var pair in m_AssetProfile)
            {
                if (pair.Value.Count == 0)
                    continue;

                XmlElement xmlAssetType = xmlDoc.CreateElement("AssetType");
                xmlAssetType.SetAttribute("name", pair.Key.ToString());

                foreach(var processor in pair.Value)
                {
                    XmlElement xmlStrategy = xmlDoc.CreateElement("Strategy");
                    processor.SaveToXML(xmlDoc, xmlStrategy);

                    xmlAssetType.AppendChild(xmlStrategy);
                }

                xmlRoot.AppendChild(xmlAssetType);
            } 
        }

        xmlDoc.AppendChild(xmlRoot);

        xmlDoc.Save(Application.dataPath + "/" + configSavePath);
    }

    public static void CommitConfigToSVN(BaseAssetProcessorData currentDisplayStrategy)
    {
        SaveAssetProfile();

        string commitFileNames = Application.dataPath + "/" + configSavePath;
        string logName = "Bug(提交资源导入规范)：提交资源导入规范Config文件。";
        if (currentDisplayStrategy != null)
        {
            HashSet<string> commitFiles = currentDisplayStrategy.ReimportFiles;
            var itr = commitFiles.GetEnumerator();
            while (itr.MoveNext())
            {
                commitFileNames += "*" + Application.dataPath + "/" + itr.Current + ".meta";
            }

            logName = string.Format("Bug(提交资源导入规范)：提交资源导入规范Config及\"{0}\"策略资源meta文件。", currentDisplayStrategy.StrategyName);
        }

        SvnTool.SvnCommit(commitFileNames, logName);
    }
}

