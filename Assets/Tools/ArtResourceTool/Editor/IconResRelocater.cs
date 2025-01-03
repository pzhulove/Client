using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
public class IconResRelocator : Editor
{
    static string NEW_ICON_PATH = "Assets/Resources/UI/Image/Icon/";

    static List<SpriteDesc> m_SpriteDescList = new List<SpriteDesc>();
    static Dictionary<string, int> m_SpriteDescNameTbl = new Dictionary<string, int>();

    protected class SpriteDesc
    {
        public string m_SpriteName = null;
        public string m_ImagePath = null;
    }

    [MenuItem("[TM工具集]/ArtTools/RelIcon")]
    static public void ReloactorIconRes()
    {
        _InitIconResDesc();
        _ReloactorIconResTable();
        _ReplaceIconResInPrefab();
    }

    static protected void _InitIconResDesc()
    {
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Buff");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Chest");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Duanwei");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Equip");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Fashion");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Head");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Item");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Jar");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Recharge");
        _GenerateIconPathDesc("Assets/Resources/UI/Image/Icon/Icon_Skill");
    }

    static protected void _ReloactorIconResTable()
    {
        string[] tables = XlsxDataManager.Instance().GetAllTableName();
        for (int i = 0, icnt = tables.Length; i < icnt; ++i)
        {
            string curTable = tables[i];
            EditorUtility.DisplayProgressBar("正在处理表格", curTable, i*1.0f / icnt);

            if (null == curTable)
                continue;

            XlsxDataUnit bufftbl = XlsxDataManager.Instance().GetXlsxByName(curTable);
            if (null != bufftbl)
            {
                bool needModify = false;
                int chkIdx = XlsxDataUnit.XLS_DATA_INDEX;
                int curCol = 0;
                Dictionary<string, ICell> tblChk = bufftbl.GetRowDataByLine(chkIdx);
                Dictionary<string, ICell>.Enumerator itTbl = tblChk.GetEnumerator();
                while(itTbl.MoveNext())
                {
                    ++curCol;
                    var cur = itTbl.Current;
                    Debug.LogFormat("### 当前键值：[{0}:{1}]" , curTable, cur.Key);
                    if( cur.Key.Contains("Icon",System.StringComparison.OrdinalIgnoreCase) ||
                        cur.Key.Contains("ModelPath", System.StringComparison.OrdinalIgnoreCase) ||
                        cur.Key.Contains("JarImage", System.StringComparison.OrdinalIgnoreCase))
                    {
                        ICell chkCell = cur.Value;
                        if (null == chkCell)
                            continue;

                        string chkFile = chkCell.CustomToString();
                        if (string.IsNullOrEmpty(chkFile))
                            continue;

                        if (chkFile.Contains("UI/Image/Icon/"))
                        {
                            needModify = true;
                            for (int k = XlsxDataUnit.XLS_DATA_INDEX; k < bufftbl.RowCount; ++k)
                            {
                                Dictionary<string, ICell> tbl = bufftbl.GetRowDataByLine(k);
                                ICell curCell = tbl[cur.Key];
                                if (null == curCell)
                                    continue;

                                string file = curCell.CustomToString();
                                if (string.IsNullOrEmpty(file))
                                    continue;

                                if ("－" == file)
                                {
                                    curCell.SetCellValue("-");
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(file) && "-" != file)
                                {
                                    string[] fileList = file.Split('|');
                                    string resNewPath = "";
                                    for (int split = 0; split < fileList.Length; ++split)
                                    {
                                        string resKey = fileList[split];

                                        string pathKey = _GetSpriteResPath(resKey);
                                        string spriteName = _GetSpriteName(resKey);

                                        SpriteDescPair curPairDesc = null;
                                        if (m_SpriteDescListTable.TryGetValue(pathKey, out curPairDesc))
                                        {
                                            int newIdx = 0;
                                            if (curPairDesc.SetMap.TryGetValue(resKey, out newIdx))
                                            {
                                                if (newIdx < curPairDesc.NewSet.Count)
                                                {
                                                    SpriteDesc spDesc = curPairDesc.NewSet[newIdx];
                                                    if (null != spDesc)
                                                    {
                                                        string newPath = spDesc.m_ImagePath.Replace("Assets/Resources/", null) + ":" + spDesc.m_SpriteName;
                                                        Sprite sp = AssetLoader.instance.LoadRes(newPath, typeof(Sprite)).obj as Sprite;
                                                        if (null != sp)
                                                        {
                                                            resNewPath += newPath;
                                                            resNewPath += "|";
                                                            Resources.UnloadAsset(sp);
                                                        }
                                                        else
                                                            Logger.LogErrorFormat("原始资源[{0}]不存在表格修改失败！！！", newPath);
                                                    }
                                                    else
                                                        Logger.LogError("无效的表格资源索引，表格修改失败！！！");
                                                }
                                            }
                                        }
                                        else
                                            Logger.LogErrorFormat("Sprite名字[{0}]不存在，表格修改失败！！", spriteName);

                                    }

                                    if (!string.IsNullOrEmpty(resNewPath))
                                    {
                                        resNewPath = resNewPath.Substring(0, resNewPath.Length - 1);
                                        curCell.SetCellValue(resNewPath);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(chkFile) || chkFile.Equals("-"))
                            {
                                ++chkIdx;

                                tblChk = bufftbl.GetRowDataByLine(chkIdx);
                                if (null != tblChk)
                                {
                                    itTbl = tblChk.GetEnumerator();
                                    int rollback = --curCol;
                                    while (itTbl.MoveNext())
                                    {
                                        --rollback;
                                        if (0 >= rollback)
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                if(needModify)
                {
                    Debug.Log("保存表：" + curTable);
                    /// 保存
                    bufftbl.Write();
#if USE_FB_TABLE
                    Xls2FBWindow.DoConvertAFile(XlsxDataManager.m_pDictKeyPath[curTable]);
#else
                     /// 转表
                    bufftbl.GetText();
#endif
                }
            }
        }

        EditorUtility.ClearProgressBar();
    }

    static protected void _ReplaceIconResInPrefab()
    {
        Dictionary<Sprite, List<string>> spriteDict = new Dictionary<Sprite, List<string>>();
        _GetComponentByType<Sprite>(new string[] { "Assets/Resources/UIFlatten/Prefabs" },ref spriteDict);

        List<GameObject> cacheGameObject = new List<GameObject>();
        Dictionary<Sprite, List<string>>.Enumerator it = spriteDict.GetEnumerator();
        float progress = 0;
        while(it.MoveNext())
        {
            var cur = it.Current;
            progress += 1.0f;
            EditorUtility.DisplayProgressBar("正在处理",cur.Key.name, progress / spriteDict.Count);

            for(int i = 0,icnt = cur.Value.Count;i<icnt;++i)
            {
                cacheGameObject.Add(AssetDatabase.LoadAssetAtPath<GameObject>(cur.Value[i]));

                string oldSpritePath = AssetDatabase.GetAssetPath(cur.Key);
                string oldSpriteKey = Path.GetDirectoryName(oldSpritePath);

                if (!oldSpriteKey.Contains("UI/Image/Icon/")) continue;

                Debug.Log("####::oldSpritePath::" + oldSpritePath);
                Debug.Log("####::oldSpriteKey::" + oldSpriteKey);

                SpriteDescPair spriteDescPair = null;
                if (m_SpriteDescListTable.TryGetValue(oldSpriteKey, out spriteDescPair))
                {
                    int newIdx = -1;
                    if(spriteDescPair.SetMap.TryGetValue(oldSpritePath.Replace("Assets/Resources/",null) + ":" + cur.Key.name,out newIdx))
                    {
                        if (newIdx < spriteDescPair.NewSet.Count)
                        {
                            SpriteDesc curDesc = spriteDescPair.NewSet[newIdx];
                            Sprite newSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine("Assets/Resources/",curDesc.m_ImagePath));

                            if(null != newSprite)
                            {
                                for(int j = 0,jcnt = cacheGameObject.Count;j<jcnt;++j)
                                {
                                    if(CollectDependencies.ComUtility.CustomReplaceSprite(cacheGameObject[j], cur.Key, newSprite))
                                    {
                                        EditorUtility.SetDirty(cacheGameObject[j]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    static protected Dictionary<string, SpriteDescPair> m_SpriteDescListTable = new Dictionary<string, SpriteDescPair>();

    protected class SpriteDescPair
    {
        public List<SpriteDesc> NewSet = new List<SpriteDesc>();
        public List<SpriteDesc> OldSet = new List<SpriteDesc>();

        public Dictionary<string, int> SetMap = new Dictionary<string,int>();
    }

    static protected void _GenerateIconPathDesc(string pathKey)
    {
        string[] spriteList = AssetDatabase.FindAssets("t:Texture", new string[] {pathKey});

        SpriteDescPair spriteDescPair = null;
        if(!m_SpriteDescListTable.TryGetValue(pathKey,out spriteDescPair))
        {
            spriteDescPair = new SpriteDescPair();
            m_SpriteDescListTable.Add(pathKey, spriteDescPair);
        }

        spriteDescPair.NewSet.Clear();
        spriteDescPair.OldSet.Clear();
        for (int i = 0, icnt = spriteList.Length; i < icnt; ++i)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteList[i]);
            if (string.IsNullOrEmpty(assetPath)) continue;
            
            Object[] assetObjList = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if(null != assetObjList)
            {
                for (int j = 0, jcnt = assetObjList.Length; j < jcnt; ++j)
                {
                    Sprite sp = assetObjList[j] as Sprite;
                    if (null == sp) continue;

                    string spName = sp.name;
                    SpriteDesc curSpDesc = new SpriteDesc();
                    curSpDesc.m_SpriteName = sp.name;
                    curSpDesc.m_ImagePath = assetPath.Replace("Assets/Resources/", null);
                    if (spName == Path.GetFileNameWithoutExtension(assetPath) && jcnt<=2)
                    {
                        spriteDescPair.NewSet.Add(curSpDesc);
                        break;
                    }
                    else
                    {
                        spriteDescPair.OldSet.Add(curSpDesc);
                    }
                }
            }
        }

        for(int i = 0,icnt = spriteDescPair.OldSet.Count;i<icnt;++i)
        {
            SpriteDesc curSpDesc = spriteDescPair.OldSet[i];
            int newSprite = 0;
            string spDescKey = curSpDesc.m_ImagePath + ":" + curSpDesc.m_SpriteName;
            if (!spriteDescPair.SetMap.TryGetValue(spDescKey, out newSprite))
            {
                bool hasNew = false;
                for(int j = 0,jcnt = spriteDescPair.NewSet.Count;j<jcnt;++j)
                {
                    SpriteDesc curNewSpDesc = spriteDescPair.NewSet[j];
                    if (curSpDesc.m_SpriteName.Equals(curNewSpDesc.m_SpriteName))
                    {
                        spriteDescPair.SetMap.Add(spDescKey,  j);
                        hasNew = true;
                        break;
                    }
                }

                if(!hasNew)
                    Logger.LogErrorFormat("Spirte [{0}] can not found in new set!!!", curSpDesc.m_SpriteName);
            }
        }
    }




    static protected void _ReloactorIconResPrefab()
    {

    }


    static protected void _ClearXlsxTableRes(string tableName,List<string> resList)
    {
        return;
        if (resList.Count > 0)
            AssetDatabase.ExportPackage(resList.ToArray(), tableName + "_UI图标备份.unitypackage");

        for(int i =0,icnt = resList.Count;i<icnt;++i)
        {
            string path = resList[i];
            File.Delete(path);
        }

    }

    static protected void _RepathIconXlsxTable(string[] spritePackImage,string tableName, string[] feildList, string[] expathList)
    {

        string[] spriteList = AssetDatabase.FindAssets("t:Texture", spritePackImage);

        m_SpriteDescList.Clear();
        for (int i = 0, icnt = spriteList.Length; i < icnt; ++i)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteList[i]);
            if (string.IsNullOrEmpty(assetPath)) continue;

            Object[] assetObjList = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            for (int j = 0, jcnt = assetObjList.Length; j < jcnt; ++j)
            {
                Sprite sp = assetObjList[j] as Sprite;
                if (null == sp) continue;

                SpriteDesc newSpDesc = new SpriteDesc();
                newSpDesc.m_SpriteName = sp.name;
                newSpDesc.m_ImagePath = assetPath;

                m_SpriteDescList.Add(newSpDesc);
            }
        }

        m_SpriteDescNameTbl.Clear();
        for (int i = 0, icnt = m_SpriteDescList.Count; i < icnt; ++i)
        {
            if (!m_SpriteDescNameTbl.ContainsKey(m_SpriteDescList[i].m_SpriteName))
                m_SpriteDescNameTbl.Add(m_SpriteDescList[i].m_SpriteName, i);
            else
                Logger.LogErrorFormat("Sprite {0} 资源名冲突！！！", m_SpriteDescList[i].m_SpriteName);
        }

        List<string> oldResList = new List<string>();
        _RenameIconInXlsxTable(tableName, feildList, expathList, ref oldResList);

        //if (oldResList.Count > 0)
        //    AssetDatabase.ExportPackage(oldResList.ToArray(), tableName + "_UI图标备份.unitypackage");
    }


    static protected void _RenameIconInXlsxTable(string tableName, string[] feildList, string[] expathList,ref List<string> oldResList)
    {
        oldResList.Clear();
        XlsxDataUnit bufftbl = XlsxDataManager.Instance().GetXlsxByName(tableName);
        if (null != bufftbl)
        {
            for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < bufftbl.RowCount; ++i)
            {
                Dictionary<string, ICell> tbl = bufftbl.GetRowDataByLine(i);

                for (int feild = 0; feild < feildList.Length; ++feild)
                {
                    ICell curCell = tbl[feildList[feild]];
                    if (null == curCell)
                        continue;

                    string file = curCell.CustomToString();
                    if (string.IsNullOrEmpty(file))
                        continue;

                    if("－" == file)
                    {
                        curCell.SetCellValue("-");
                        continue;
                    }

                    if (!string.IsNullOrEmpty(file) && "-" != file)
                    {
                        string[] fileList = file.Split('|');
                        string resNewPath = "";
                        for (int split = 0; split < fileList.Length; ++split)
                        {
                            string resKey = expathList[feild] + fileList[split];

                            string pathKey = _GetSpriteResPath(resKey);
                            string spriteName = _GetSpriteName(resKey);

                            SpriteDescPair curPairDesc = null;
                            if(m_SpriteDescListTable.TryGetValue(pathKey,out curPairDesc))
                            {
                                int newIdx = 0;
                                if(curPairDesc.SetMap.TryGetValue(resKey, out newIdx))
                                {
                                    if(newIdx < curPairDesc.NewSet.Count)
                                    {
                                        SpriteDesc spDesc = curPairDesc.NewSet[newIdx];
                                        if (null != spDesc)
                                        {
                                            string newPath = spDesc.m_ImagePath.Replace("Assets/Resources/", null) + ":" + spDesc.m_SpriteName;
                                            Sprite sp = AssetLoader.instance.LoadRes(newPath, typeof(Sprite)).obj as Sprite;
                                            if (null != sp)
                                            {
                                                resNewPath += newPath;
                                                resNewPath += "|";
                                                Resources.UnloadAsset(sp);
                                            }
                                            else
                                                Logger.LogErrorFormat("原始资源[{0}]不存在表格修改失败！！！", newPath);
                                        }
                                        else
                                            Logger.LogError("无效的表格资源索引，表格修改失败！！！");
                                    }
                                }
                            }
                            else
                                Logger.LogErrorFormat("Sprite名字[{0}]不存在，表格修改失败！！", spriteName);

                        }

                        if(!string.IsNullOrEmpty(resNewPath))
                        {
                            resNewPath = resNewPath.Substring(0, resNewPath.Length - 1);
                            curCell.SetCellValue(resNewPath);
                        }
                    }
                }
            }

            /// 保存
            bufftbl.Write();

#if USE_FB_TABLE
            Xls2FBWindow.DoConvertAFile(XlsxDataManager.m_pDictKeyPath[tableName]);
#else
            /// 转表
            bufftbl.GetText();
#endif
        }
        else
        {
            Logger.LogWarningFormat("Can not find table {0}!", tableName);
        }
    }

    static string _GetSpriteName(string res)
    {
        string[] resSplit = res.Split(':');
        if(resSplit.Length > 1)
            return resSplit[1];
        else
            return Path.GetFileNameWithoutExtension(resSplit[0]);
    }

    static string _GetSpriteResPath(string resKey)
    {
        string[] resSplit = resKey.Split(':');
        string path =  Path.GetDirectoryName( resSplit[0]);
        path = Path.Combine("Assets/Resources/", path);
        return path;
    }


    static void _GetComponentByType<T>(string[] prefabpaths,ref Dictionary<T, List<string>> outDict) where T : UnityEngine.Object
    {
        if (null == outDict)
            return;

        outDict.Clear();
        var prefabs = AssetDatabase.FindAssets("t:prefab", prefabpaths);

        int cnt = prefabs.Length;
        int count = 0;

        UnityEngine.Object[] curObjs = new UnityEngine.Object[1] { null };
        foreach (var preguid in prefabs)
        {
            count++;

            var path = AssetDatabase.GUIDToAssetPath(preguid);

            UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            curObjs[0] = root;
            var deps = EditorUtility.CollectDependencies(curObjs);

            foreach (var com in deps)
            {
                T sp = com as T;
                if (null != sp)
                {
                    string objpath = AssetDatabase.GetAssetPath(sp);

                    EditorUtility.DisplayProgressBar(Path.GetFileName(path), Path.GetFileName(objpath), count * 1.0f / cnt);
                    if (!outDict.ContainsKey(sp))
                    {
                        outDict.Add(sp, new List<string>());
                    }

                    outDict[sp].Add(path);
                }
            }
        }

        EditorUtility.ClearProgressBar();
    }
}
