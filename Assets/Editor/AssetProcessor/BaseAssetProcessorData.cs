using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

public abstract class BaseAssetProcessorData
{
    public List<string> IncludePaths = new List<string>();
    public List<string> ExcludePaths = new List<string>();
    public List<string> IncludeFiles = new List<string>();
    public List<string> ExcludeFiles = new List<string>();

    public HashSet<string> ReimportFiles = new HashSet<string>();

    public string StrategyName;

    protected System.Type assetType;

    public bool IsBaseField(string fieldName)
    {
        return (fieldName == "IncludePaths") ||
               (fieldName == "ExcludePaths") ||
               (fieldName == "IncludeFiles") ||
               (fieldName == "ExcludeFiles") ||
               (fieldName == "StrategyName") || 
               (fieldName == "ReimportFiles");
    }

    public void AddPathOrFile(string relativePath, bool bInclude)
    {
        string absPath = RelativePath2AbsolutePath(relativePath);
        FileAttributes attr = File.GetAttributes(absPath);

        List<string> refList = null;
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        {
            //路径
            refList = bInclude ? IncludePaths : ExcludePaths;
        }
        else
        {
            //文件
            refList = bInclude ? IncludeFiles : ExcludeFiles;
        }

        if(refList != null && !refList.Contains(relativePath))
        {
            refList.Add(relativePath);
        }
    }

    public void ClearIncludeOrExclude(bool bInclude)
    {
        if(bInclude)
        {
            IncludeFiles.Clear();
            IncludePaths.Clear();
        }
        else
        {
            ExcludeFiles.Clear();
            ExcludePaths.Clear();
        }
    }

    public void PasteIncludeOrExclude(List<string> paths, List<string> files, bool bInclude)
    {
        if(bInclude)
        {
            IncludePaths = new List<string>(paths);
            IncludeFiles = new List<string>(files);
        }
        else
        {
            ExcludePaths = new List<string>(paths);
            ExcludeFiles = new List<string>(files);
        }
    }

    public virtual bool LoadFromXML(XmlNode node)
    {
        StrategyName = (node as XmlElement).GetAttribute("name");
        foreach (XmlNode assetNode in node.ChildNodes)
        {
            List<string> curList = new List<string>();
            if (assetNode.LocalName == "IncludePaths")
            {
                IncludePaths = curList;
            }
            else if (assetNode.LocalName == "ExcludePaths")
            {
                ExcludePaths = curList;
            }
            else if (assetNode.LocalName == "IncludeFiles")
            {
                IncludeFiles = curList;
            }
            else if (assetNode.LocalName == "ExcludeFiles")
            {
                ExcludeFiles = curList;
            }

            foreach (XmlElement assetNameNode in assetNode.ChildNodes)
            {
                string assetName = assetNameNode.GetAttribute("name");

                curList.Add(assetName);
            }
        }

        return true;
    }

    public virtual void SaveToXML(XmlDocument xml, XmlNode node)
    {
        (node as XmlElement).SetAttribute("name", StrategyName);

        if(IncludePaths.Count > 0)
        {
            XmlElement assetNode = xml.CreateElement("IncludePaths");

            foreach(var assetName in IncludePaths)
            {
                XmlElement assetNameNode = xml.CreateElement("asset");
                assetNameNode.SetAttribute("name", assetName);

                assetNode.AppendChild(assetNameNode);
            }

            node.AppendChild(assetNode);
        }

        if (ExcludePaths.Count > 0)
        {
            XmlElement assetNode = xml.CreateElement("ExcludePaths");

            foreach (var assetName in ExcludePaths)
            {
                XmlElement assetNameNode = xml.CreateElement("asset");
                assetNameNode.SetAttribute("name", assetName);

                assetNode.AppendChild(assetNameNode);
            }

            node.AppendChild(assetNode);
        }

        if (IncludeFiles.Count > 0)
        {
            XmlElement assetNode = xml.CreateElement("IncludeFiles");

            foreach (var assetName in IncludeFiles)
            {
                XmlElement assetNameNode = xml.CreateElement("asset");
                assetNameNode.SetAttribute("name", assetName);

                assetNode.AppendChild(assetNameNode);
            }

            node.AppendChild(assetNode);
        }

        if (ExcludeFiles.Count > 0)
        {
            XmlElement assetNode = xml.CreateElement("ExcludeFiles");

            foreach (var assetName in ExcludeFiles)
            {
                XmlElement assetNameNode = xml.CreateElement("asset");
                assetNameNode.SetAttribute("name", assetName);

                assetNode.AppendChild(assetNameNode);
            }

            node.AppendChild(assetNode);
        }
    }

    /// <summary>
    /// 根据相对路径得到绝对路径
    /// assetsFolderPath为工程Assets文件夹的路径
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    private string RelativePath2AbsolutePath(string relativePath)
    {
        var assetsFolderPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
        return assetsFolderPath + relativePath;
    }

    private string AbsolutePath2RelativePath(string absolutePath)
    {
        var assetsFolderPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
        int index = absolutePath.IndexOf(assetsFolderPath);
        int length = assetsFolderPath.Length;
        return absolutePath.Remove(index, length);
    }

    /// <summary>
    /// 获取文件夹下某个类型的所有资源的路径
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    private HashSet<string> GetAssetPathInFolder(string folderPath)
    {
        HashSet<string> assetPaths = new HashSet<string>();
        string[] searchFolders = new string[]{ folderPath };
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", assetType.ToString().Replace("UnityEngine.","")), searchFolders);
        for(int i = 0;i < guids.Length; i++)
        {
            assetPaths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
        return assetPaths;
    }

    public void ReImportAsset()
    {
        //Include Exclude 绝对路径
        HashSet<string> includeAbsolutePath = new HashSet<string>();
        HashSet<string> excludeAbsolutePath = new HashSet<string>();
        //Include Exclude 该类型资源的路径
        HashSet<string> includeAssetPath = new HashSet<string>();

        ReimportFiles.Clear();

        //include
        foreach (var path in IncludePaths)
        {
            includeAbsolutePath.Add(RelativePath2AbsolutePath(path));
        }

        foreach (var path in includeAbsolutePath)
        {
            FileAttributes attr = File.GetAttributes(path);
            var absPath = AbsolutePath2RelativePath(path);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                //路径
                var assetPaths = GetAssetPathInFolder(absPath);
                foreach(var assetPath in assetPaths)
                {
                    includeAssetPath.Add(assetPath);
                }
            }
            else
            {
                //文件
                includeAssetPath.Add(absPath);
            }
        }

        //include
        foreach (var path in IncludeFiles)
        {
            includeAssetPath.Add(path);
        }

        //exclude
        foreach (var path in ExcludePaths)
        {
            excludeAbsolutePath.Add(RelativePath2AbsolutePath(path));
        }

        foreach(var path in excludeAbsolutePath)
        {
            FileAttributes attr = File.GetAttributes(path);
            var relativePath = AbsolutePath2RelativePath(path);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                //路径
                var assetPaths = GetAssetPathInFolder(relativePath);
                foreach(var assetPath in assetPaths)
                {
                    if(includeAssetPath.Contains(assetPath))
                    {
                        includeAssetPath.Remove(assetPath);
                    }
                }
            }
            else
            {
                //文件
                if(includeAssetPath.Contains(relativePath))
                {
                    includeAssetPath.Remove(relativePath);
                }
            }
        }

        //exclude
        foreach (var path in ExcludeFiles)
        {
            if (includeAssetPath.Contains(path))
            {
                includeAssetPath.Remove(path);
            }
        }

        foreach (var path in includeAssetPath)
        {
            AssetDatabase.ImportAsset(path);

            ReimportFiles.Add(path.Replace("Assets/", null));
        }

    }

    public abstract void DisplayAndChangeData();
    public abstract void OnImportAsset(AssetImporter assetImporter, string assetPath);

    public bool ImportAsset(string assetPath, AssetImporter assetImporter)
    {
        if (ExcludeFiles.Contains(assetPath))
        {
            return false;
        }
        else
        {
            foreach(var exDir in ExcludePaths)
            {
                if(assetPath.Contains(exDir))
                {
                    return false;
                }
            }
        }

        bool bNeedDeal = false;
        if(IncludeFiles.Contains(assetPath))
        {
            bNeedDeal = true;
        }
        else
        {
            foreach (var inDir in IncludePaths)
            {
                if (assetPath.Contains(inDir))
                {
                    bNeedDeal = true;
                }
            }
        }

        if(bNeedDeal)
        {
            OnImportAsset(assetImporter, assetPath);
        }

        return bNeedDeal;
    }

    protected void DisplayIntInfo(string name, ref int value, int space,ref bool canChange)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(space);
            canChange = EditorGUILayout.Toggle(canChange, GUILayout.Width(10));

            EditorGUI.BeginDisabledGroup(canChange == false);

            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            value = EditorGUILayout.IntField(value, GUILayout.Width(100));


            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void DisplayFloatInfo(string name, ref float value, int space,ref bool canChange)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(space);
            canChange = EditorGUILayout.Toggle(canChange, GUILayout.Width(10));

            EditorGUI.BeginDisabledGroup(canChange == false);

            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            value = EditorGUILayout.FloatField(value, GUILayout.Width(100));

            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void DisplayHeaderBoolInfo(string name, ref bool value, ref bool canChange)
    {
        EditorGUILayout.BeginHorizontal();
        {
            canChange = EditorGUILayout.Toggle(canChange, GUILayout.Width(10));

            EditorGUI.BeginDisabledGroup(canChange == false);

            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            value = GUILayout.Toggle(value, string.Empty);

            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void DisplayBoolInfo(string name, ref bool value, int space,ref bool canChange)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(space);
            canChange = EditorGUILayout.Toggle(canChange, GUILayout.Width(10));

            EditorGUI.BeginDisabledGroup(canChange == false);

            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            value = GUILayout.Toggle(value, string.Empty);

            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void DisplayBoolInfo(string name, ref bool value, int space)
    {
        //EditorGUILayout.BeginHorizontal();
        {
            value = EditorGUILayout.Toggle(value, GUILayout.Width(10));
            GUILayout.FlexibleSpace();
        }
        //EditorGUILayout.EndHorizontal();
    }
}
