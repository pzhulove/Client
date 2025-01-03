using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;


public class AssetPackageSpliter : EditorWindow
{
    public class AssetGatherDesc
    {
        public AssetGatherDesc(string path, bool collent)
        {
            assetPath = path;
            collectDepend = collent;
        }

        public string assetPath = null;
        public bool collectDepend = false;
    }

    static int m_PlayerLevel = 25;/// 玩家体验等级

    [MenuItem("[打包工具]/Build Split Package")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        AssetPackageSpliter window = (AssetPackageSpliter)EditorWindow.GetWindow(typeof(AssetPackageSpliter));
        window.titleContent = new GUIContent("资源分包");
        window.Show();
    }

    void OnGUI()
    {

        EditorGUILayout.Space();

        EditorGUILayout.Space();

        if (GUILayout.Button("提取逆向资源加载记录"))
        {
            _ExtractResLoadList();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("生成资源分包"))
        {
            _GenerateBasePacakge();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("清理缓存"))
        {
            _ClearCaches();
        }

        EditorGUILayout.Space();
    }

    static protected void _ClearCaches()
    {
    }

    static protected void _ExtractResLoadList()
    {
        string resRecordDirectory = Path.Combine(Application.streamingAssetsPath, "ResLoadRecord");
        resRecordDirectory = resRecordDirectory.Replace('\\', '/');
        string[] assetPathList = Directory.GetFiles(resRecordDirectory, "*.rec", SearchOption.AllDirectories);

        List<string> allResList = new List<string>();

        if(File.Exists("Assets/StreamingAssets/AllAssetList.txt"))
        {
            FileStream streamR = new FileStream("Assets/StreamingAssets/AllAssetList.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(streamR);
            string buf = sr.ReadToEnd();
            sr.Close();
            streamR.Close();

            string[] lines = buf.Split(new char[] { '\n', '\t' });
            if (null != lines)
            {
                for (int i = 0, icnt = lines.Length; i < icnt; ++i)
                {
                    if (string.IsNullOrEmpty(lines[i])) continue;
                    if ("-" == lines[i]) continue;

                    string pathR = lines[i].Replace("\r", null);
                    if (!allResList.Contains(pathR))
                        allResList.Add(pathR);
                }
            }
        }

        for (int i = 0, icnt = assetPathList.Length; i < icnt; ++i)
        {
            FileStream streamR = new FileStream(assetPathList[i], FileMode.Open, FileAccess.Read);

            if (null != streamR)
            {
                StreamReader sr = new StreamReader(streamR);
                string buf = sr.ReadToEnd();
                sr.Close();
                streamR.Close();

                _LoadAssetList(buf, ref allResList);
            }
        }

        FileStream asset_fs = new FileStream("Assets/StreamingAssets/AllAssetList.txt", FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter asset_sw = new StreamWriter(asset_fs);
        asset_sw.BaseStream.Seek(0, SeekOrigin.Begin);
        asset_sw.Flush();

        for (int i = 0, icnt = allResList.Count; i < icnt; ++i)
            asset_sw.WriteLine(allResList[i]);
        asset_sw.Flush();
        asset_sw.Close();
        asset_fs.Close();

        for (int i = 0, icnt = assetPathList.Length; i < icnt; ++i)
        {
            if (File.Exists(assetPathList[i]))
                File.Delete(assetPathList[i]);
        }
    }

    static protected void _LoadAssetList(string buf,ref List<string> assetList)
    {
        string[] lines = buf.Split(new char[] { '\n', '\t' });

        List<string> pathList = new List<string>();
        if (null != lines)
        {
            for (int i = 0, icnt = lines.Length; i < icnt; ++i)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                if ("-" == lines[i]) continue;

                string pathR = lines[i].Replace("\r", null);
                if (!pathList.Contains(pathR))
                    pathList.Add(pathR);
            }
        }

        List<string> allAssetsWithDepend = new List<string>();
        for (int i = 0, icnt = pathList.Count; i < icnt; ++i)
        {
            UnityEngine.Object obj = AssetLoader.instance.LoadRes(pathList[i]).obj;
            if (null == obj) continue;

            string Path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(Path))
            {
                if (obj is GameObject)
                {
                    pathList[i] = PathUtil.EraseExtension(pathList[i]);
                    Path = "Assets/Resources/" + pathList[i] + ".prefab";
                }
            }

            allAssetsWithDepend.Add(Path);
            allAssetsWithDepend.AddRange(AssetDatabase.GetDependencies(Path));
        }
        
        for (int i = 0, icnt = allAssetsWithDepend.Count; i < icnt; ++i)
        {
            string Path = allAssetsWithDepend[i];
            if (!assetList.Contains(Path))
                assetList.Add(Path);
        }


    }


    static protected void _GenerateBasePacakge()
    {
        List<AssetGatherDesc> assetList = new List<AssetGatherDesc>();

        /// Data
        _GatherAssetInDirectory("Assets/Resources/Data", new string[] { ".asset", ".xml", ".json", ".prefab", ".bytes" }, false, ref assetList);
        /// Animat
        _GatherAssetInDirectory("Assets/Resources/Animat", new string[] { ".asset" }, true, ref assetList);
        /// Shader
        _GatherAssetInDirectory("Assets/Resources/Shader", new string[] { ".shader", ".asset", ".json" }, true, ref assetList);
        /// Environment
        _GatherAssetInDirectory("Assets/Resources/Environment", new string[] { ".prefab", ".asset", ".json", ".xml" }, true, ref assetList);

        /// Model
        _GatherAssetInDirectory("Assets/Resources/Actor/Other", new string[] { ".prefab" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Other_ShadowPlane", new string[] { ".prefab" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Pot", new string[] { ".prefab" }, true, ref assetList);

        /// Actor
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Gungirl", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_Gungirl/Animations", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Gunman", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_Gunman/Animations", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_MageGirl/Hero_MageGirl_Magegirl", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_MageGirl/Animations", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Swordsman", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Hero_Swordsman/Animations", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);
        _GatherAssetInDirectory("Assets/Resources/Actor/Weapon", new string[] { ".prefab", ".anim", ".fbx", ".asset" }, true, ref assetList);

        /// UI icon
        /// 

        /// 刷怪表
        /// 获取随机刷怪的资源
        XlsxDataUnit monstertbl = XlsxDataManager.Instance().GetXlsxByName("MonsterGroupTable");
        if (null != monstertbl)
        {
            for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < monstertbl.RowCount; ++i)
            {
                Dictionary<string, ICell> tbl = monstertbl.GetRowDataByLine(i);
                ICell monsterLstCell = tbl["MonsterList"];

                if (null == monsterLstCell)
                    continue;

                string lst = monsterLstCell.CustomToString();
                string[] fileList = lst.Split('|');
                for (int split = 0; split < fileList.Length; ++split)
                {
                    int resID = int.Parse(fileList[split]);

                    _GetherMonsterRes(resID, ref assetList);
                }
            }
        }

        /// 地图城镇表
        /// 根据玩家等级读表获取城镇和地下城的地图信息
        XlsxDataUnit citySceneTable = XlsxDataManager.Instance().GetXlsxByName("CitySceneTable");
        if (null != citySceneTable)
        {
            for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < citySceneTable.RowCount; ++i)
            {
                Dictionary<string, ICell> tbl = citySceneTable.GetRowDataByLine(i);
                ICell levelLimitCell = tbl["LevelLimit"];

                if (null == levelLimitCell)
                    continue;

                int levelLimit = levelLimitCell.CustomToInt();
                if(levelLimit < m_PlayerLevel)
                {
                    ICell resPathCell = tbl["ResPath"];

                    if (null == resPathCell)
                        continue;

                    string resPath = resPathCell.CustomToString();
                    if(string.IsNullOrEmpty(resPath))
                        continue;

                    DSceneData curScene = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", Path.ChangeExtension(resPath, "asset")), typeof(DSceneData)) as DSceneData;
                    if (null != curScene)
                    {
                        assetList.Add(new AssetGatherDesc(curScene._prefabpath, true));
                        
                        for (int j = 0; j < curScene._npcinfo.Length; ++j)
                        {
                            string modelPath = curScene._npcinfo[j].GetModelPathByResID();
                            assetList.Add(new AssetGatherDesc(modelPath, true));
                        }
                    }

                    ICell bgmPathCell = tbl["BGMPath"];

                    if (null == bgmPathCell)
                        continue;

                    string bgmPath = resPathCell.CustomToString();
                    if (string.IsNullOrEmpty(bgmPath))
                        continue;
                    assetList.Add(new AssetGatherDesc(bgmPath, true));
                }
            }
        }
        /// 

        /// 加载可破坏物表
        XlsxDataUnit destrucTable = XlsxDataManager.Instance().GetXlsxByName("DestrucTable");
        if (null != destrucTable)
        {
            for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < destrucTable.RowCount; ++i)
            {
                Dictionary<string, ICell> tbl = destrucTable.GetRowDataByLine(i);
                ICell destrucTableCell = tbl["Mode"];

                if (null == destrucTableCell)
                    continue;

                int resID = destrucTableCell.CustomToInt();
                _GatherModelDataRes(resID, ref assetList);
            }
        }

        /// 加载所有职业
        XlsxDataUnit jobtbl = XlsxDataManager.Instance().GetXlsxByName("JobTable");
        if (null != jobtbl)
        {
            for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < jobtbl.RowCount; ++i)
            {
                Dictionary<string, ICell> tbl = jobtbl.GetRowDataByLine(i);
                ICell modelCell = tbl["Mode"];

                if (null == modelCell)
                    continue;

                int resID = modelCell.CustomToInt();
                _GatherModelDataRes(resID, ref assetList);
            }
        }

        /// Gather dependency
        List<string> finalResList = new List<string>();
        _RefineAssets(assetList, ref finalResList);

        List<string> specificAssetList = new List<string>();
        if (File.Exists("Assets/StreamingAssets/AllAssetList.txt"))
        {
            FileStream streamR = new FileStream("Assets/StreamingAssets/AllAssetList.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(streamR);
            string buf = sr.ReadToEnd();
            sr.Close();
            streamR.Close();

            string[] lines = buf.Split(new char[] { '\n', '\t' });

            if (null != lines)
            {
                for (int i = 0, icnt = lines.Length; i < icnt; ++i)
                {
                    if (string.IsNullOrEmpty(lines[i])) continue;
                    if ("-" == lines[i]) continue;

                    string pathR = lines[i].Replace("\r", null);
                    if (!specificAssetList.Contains(pathR))
                        specificAssetList.Add(pathR);
                }
            }
        }

        specificAssetList.AddRange(finalResList);
        finalResList.Clear();
        _UnifyAssetResList(specificAssetList, ref finalResList);

        FileStream asset_fs = new FileStream("Assets/StreamingAssets/BaseAssetList.lst", FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter asset_sw = new StreamWriter(asset_fs);

        asset_sw.BaseStream.Seek(0, SeekOrigin.Begin);
        for (int i = 0, icnt = finalResList.Count; i < icnt; ++i)
            asset_sw.WriteLine(finalResList[i]);
        asset_sw.Flush();
        asset_sw.Close();
        asset_fs.Close();
    }

    static protected void _UnifyAssetResList(List<string> assetList, ref List<string> outResList)
    {
        for (int i = 0, icnt = assetList.Count; i < icnt; ++i)
        {
            string curPath = assetList[i].Replace("Assets/Resources/", null);
            if (string.IsNullOrEmpty(curPath) || curPath == "-")
                continue;

            if (!outResList.Contains(curPath))
                outResList.Add(curPath);
        }
    }

    static protected void _GatherDungeonAssets(int dungeonID, string dungeonData, ref List<AssetGatherDesc> assetList)
    {
        DDungeonData data = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", Path.ChangeExtension(dungeonData, "asset")), typeof(DDungeonData)) as DDungeonData;
        if (null != data && null != data.areaconnectlist)
        {
            for (int i = 0, icnt = data.areaconnectlist.Length; i < icnt; ++i)
            {
                DSceneDataConnect cur = data.areaconnectlist[i];
                if (null == cur) continue;

                DSceneData curScene = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", Path.ChangeExtension(cur.sceneareapath, "asset")), typeof(DSceneData)) as DSceneData;

                if (null != curScene)
                {
                    assetList.Add(new AssetGatherDesc(curScene._prefabpath, true));

                    for (int j = 0; j < curScene._decoratorinfo.Length; ++j)
                    {
                        string modelPath = curScene._decoratorinfo[j].GetModelPathByResID();
                        assetList.Add(new AssetGatherDesc(modelPath, true));
                    }

                    for (int j = 0; j < curScene._monsterinfo.Length; ++j)
                    {
                        int resID = (curScene._monsterinfo[j].resid / 10000) * 10000 + (curScene._monsterinfo[j].resid % 100);
                        ProtoTable.UnitTable unit = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(resID);
                        if (null == unit) continue;

                        _GatherModelDataRes(unit.Mode, ref assetList);
                    }

                    for (int j = 0; j < curScene._desructibleinfo.Length; ++j)
                    {
                        string modelPath = curScene._desructibleinfo[j].GetModelPathByResID();
                        assetList.Add(new AssetGatherDesc(modelPath, true));
                    }

                    for (int j = 0; j < curScene._transportdoor.Length; ++j)
                    {
                        string modelPath = curScene._transportdoor[j].GetModelPathByResID();
                        assetList.Add(new AssetGatherDesc(modelPath, true));
                    }

                    for (int j = 0; j < curScene._regioninfo.Length; ++j)
                    {
                        string modelPath = curScene._regioninfo[j].GetModelPathByResID();
                        assetList.Add(new AssetGatherDesc(modelPath, true));
                    }
                }
            }
        }
    }

    static protected void _GatherModelDataRes(int resID, ref List<AssetGatherDesc> assetResList)
    {
        ProtoTable.ResTable res = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
        if (null == res) return;

        assetResList.Add(new AssetGatherDesc(res.ModelPath, true));
        GameObject actor = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", Path.ChangeExtension(res.ModelPath, "prefab")), typeof(GameObject)) as GameObject;
        if (null != actor)
        {
            GeAnimDescProxy animProxy = actor.GetComponent<GeAnimDescProxy>();
            if (null != animProxy && null != animProxy.animDescArray)
            {
                for (int i = 0, icnt = animProxy.animDescArray.Length; i < icnt; ++i)
                {
                    GeAnimDesc cur = animProxy.animDescArray[i];
                    if (null == cur) continue;

                    assetResList.Add(new AssetGatherDesc(cur.animClipPath, false));
                }
            }
        }

        for (int k = 0, kcnt = res.ActionConfigPath.Count; k < kcnt; ++k)
        {
            string path = res.ActionConfigPath[k];
            _GatherSkillDataRes(path, ref assetResList);
        }
    }

    static protected void _GetherBuffDataRes(int buffID, ref List<AssetGatherDesc> assetResList)
    {
        if (buffID <= 0)
            return;

        var buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffID);
        if (null == buffData)
            return;

        if (buffData.Type == (int)BuffType.SUMMON)
        {
            _GetherMonsterRes(buffData.summon_monsterID, ref assetResList);
            for (int k = 0; k < buffData.summon_entity.Count; ++k)
                _GatherModelDataRes(buffData.summon_entity[k], ref assetResList);
        }
    }

    static protected void _GetherMonsterRes(int monsterID, ref List<AssetGatherDesc> assetResList)
    {
        if (monsterID <= 0) return;

        int level = (monsterID - monsterID / 10000 * 10000) / 100;
        monsterID -= level * 100;

        var monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monsterID);
        if (null == monsterData) return;

        assetResList.Add(new AssetGatherDesc(monsterData.AIActionPath, false));
        assetResList.Add(new AssetGatherDesc(monsterData.AIDestinationSelectPath, false));
        assetResList.Add(new AssetGatherDesc(monsterData.AIEventPath, false));

        _GatherModelDataRes(monsterData.Mode, ref assetResList);
    }

    public static ArrayList _GetSkillFileList(string path)
    {
        string folderName = Utility.GetPathLastName(path);
        string fileListName = path + "/" + folderName + "_FileList";
        UnityEngine.Object obj = AssetLoader.instance.LoadRes(fileListName).obj;
        if (obj == null)
            return null;

        string content = System.Text.ASCIIEncoding.Default.GetString((obj as TextAsset).bytes);
        ArrayList list = XUPorterJSON.MiniJSON.jsonDecode(content) as ArrayList;

        return list;
    }

    static protected void _GatherSkillDataRes(string path, ref List<AssetGatherDesc> assetResList)
    {
        ArrayList list = _GetSkillFileList(path);
        if (list == null)
        {
            Logger.LogErrorFormat("can't find the filelist with path {0}", path);
            return;
        }

        for (int i = 0; i < list.Count; ++i)
        {
            string filename = (string)list[i];
            Logger.Log("try load skill config:" + filename);
            string key = path + "/" + filename;

            DSkillData data = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", Path.ChangeExtension(key, "asset")), typeof(DSkillData)) as DSkillData;
            if (null != data)
            {
                for (int j = 0; j < data.attachFrames.Length; ++j)
                {
                    string assetPath = data.attachFrames[j].entityAsset.m_AssetPath;
                    assetResList.Add(new AssetGatherDesc(assetPath, true));
                }

                for (int j = 0; j < data.effectFrames.Length; ++j)
                {
                    string assetPath = data.effectFrames[j].effectAsset.m_AssetPath;
                    assetResList.Add(new AssetGatherDesc(assetPath, true));
                }

                for (int j = 0; j < data.entityFrames.Length; ++j)
                {
                    string assetPath = data.entityFrames[j].entityAsset.m_AssetPath;
                    assetResList.Add(new AssetGatherDesc(assetPath, true));
                }

                for (int j = 0; j < data.sfx.Length; ++j)
                {
                    string assetPath = data.sfx[j].soundClipAsset.m_AssetPath;
                    assetResList.Add(new AssetGatherDesc(assetPath, true));
                }

                for (int j = 0; j < data.frameEffects.Length; ++j)
                {
                    DSkillFrameEffect curEff = data.frameEffects[j];

                    if (curEff.effectID <= 0)
                        continue;

                    var effectData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(curEff.effectID);
                    if (null == effectData) continue;

                    //召唤怪物
                    if (effectData.SummonID > 0)
                    {
                        List<int> summonList = new List<int>();
                        if (effectData.SummonRandList.Count >= 2)
                        {
#if USE_FB_TABLE
                            summonList.AddRange(effectData.SummonRandList);
                            
#else
                            summonList.AddRange(Utility.toList(effectData.SummonRandList) );           
#endif
                        }
                        else
                        {
                            summonList.Add(effectData.SummonID);
                        }

                        for (int k = 0, kcnt = summonList.Count; k < kcnt; ++k)
                        {
                            int monsterID = summonList[k];
                            _GetherMonsterRes(monsterID, ref assetResList);
                        }
                    }

                    //召唤buff
                    _GetherBuffDataRes(effectData.BuffID, ref assetResList);

                    for (int k = 0; k < effectData.BuffInfoID.Count; ++k)
                    {
                        int buffInfoID = effectData.BuffInfoID[k];
                        if (buffInfoID <= 0)
                            continue;

                        var buffInfoData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfoID);
                        if (buffInfoData == null)
                            continue;

                        _GetherBuffDataRes(buffInfoData.BuffID, ref assetResList);
                    }
                }
            }
        }

    }

    static protected void _RefineAssets(List<AssetGatherDesc> originRes, ref List<string> finalRes)
    {
        /// Gather dependency
        List<string> dependList = new List<string>();
        for (int i = 0, icnt = originRes.Count; i < icnt; ++i)
        {
            if (originRes[i].collectDepend)
            {
                string[] dependency = _GatherDependency(originRes[i].assetPath, new string[] { ".cs", ".dll" });
                if (null != dependency)
                    dependList.AddRange(dependency);
            }

            dependList.Add(originRes[i].assetPath);
        }

        _UnifyAssetResList(dependList, ref finalRes);
    }

    static protected string[] _GatherDependency(string objectPath, string[] excludeExt)
    {
        if (!objectPath.Contains("Assets/Resources/"))
        {
            objectPath = Path.Combine("Assets/Resources/", Path.ChangeExtension(objectPath, "prefab"));
            objectPath.Replace('\\', '/');
        }

        string[] dependency = AssetDatabase.GetDependencies(objectPath);
        if (null != excludeExt && excludeExt.Length > 0)
        {
            List<string> dependFilter = new List<string>();
            bool bSkip = false;
            for (int i = 0; i < dependency.Length; ++i)
            {
                bSkip = false;
                for (int j = 0; j < excludeExt.Length; ++j)
                {
                    if (excludeExt[j] == PathUtil.GetExtension(dependency[i]))
                    {
                        bSkip = true;
                        break;
                    }
                }

                if (bSkip) continue;
                dependFilter.Add(dependency[i]);
            }

            return dependFilter.ToArray();
        }
        else
            return dependency;
    }

    static protected void _GatherAssetInDirectory(string directory, string[] extFilter, bool gatherDepend, ref List<AssetGatherDesc> res)
    {
        directory = directory.Replace('\\', '/');
        string[] assetPathList = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < assetPathList.Length; ++i)
        {
            string curPath = assetPathList[i];
            curPath = curPath.Replace('\\', '/');
            string curExt = Path.GetExtension(curPath);

            if (curExt.Equals(".meta", StringComparison.OrdinalIgnoreCase))
                continue;

            for (int j = 0; j < extFilter.Length; ++j)
            {
                if (extFilter[j].Equals(curExt, StringComparison.OrdinalIgnoreCase))
                {
                    res.Add(new AssetGatherDesc(curPath, gatherDepend));
                    break;
                }
            }
        }
    }
}
