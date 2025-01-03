using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Diagnostics;
using System.Reflection;
using ProtoTable;

public class ServerTextTool
{
    public static string SCENEN_DATA_PATH = "Assets/Resources/Data/SceneData";
    public static string XML_PATH = "../Share/table/xml";
    public static string SERVER_TEXT_PATH = "../Share/table/server";
    public static string ST_TEXT_PATH = "../Share";

    public static string INPUT_PREFAB = "Assets/Resources/UIFlatten/Prefabs/ETCInput";

    //[MenuItem("[TM工具集]/转表工具/死亡之塔")]
    static void GetDeadTower()
    {
        var path = "Data/SceneData/HD-siwangzhita/Room_swzt_{0}";
        var dungeonPath = "Data/DungeonData/HD-siwangzhita";

        var dungeon = Resources.Load<DDungeonData>(dungeonPath);

        for (int i = 0; i < dungeon.areaconnectlist.Length; ++i)
        {
            dungeon.areaconnectlist[i].sceneareapath = string.Format(path, i + 1);
            dungeon.areaconnectlist[i].scenedata = Resources.Load<DSceneData>(string.Format(path, i + 1));
        }

        EditorUtility.SetDirty(dungeon);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("[TM工具集]/转表工具/检查所有传送门的缩放")]
    static void CheckTransportDoorScale()
    {
        string[] allDatas = AssetDatabase.FindAssets("t:DSceneData", new string[] { "Assets/Resources/Data" });

        for (int i = 0; i < allDatas.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(allDatas[i]);

            DSceneData data = AssetDatabase.LoadAssetAtPath<DSceneData>(path);

            if (null != data && null != data._transportdoor)
            {
                for (int j = 0; j < data._transportdoor.Length; ++j)
                {
                    if (!Mathf.Approximately(data._transportdoor[j].scale, 1.0f))
                    {
                        UnityEngine.Debug.LogErrorFormat("{0} door scale is {1}", path, data._transportdoor[j].scale);
                    }

                    UnityEngine.Debug.LogFormat("{0} door scale is {1}", path, data._transportdoor[j].scale);
                }
            }
        }
    }

    [MenuItem("[TM工具集]/转表工具/生成特效没有引用的贴图")]
    static void GetUnusedEffectTexture()
    {
        Dictionary<string, int> allEffectTextureDict = new Dictionary<string, int>();
        Dictionary<string, List<string>> allEffectTextureDictStr = new Dictionary<string, List<string>>();



        string[] alleffecttexture = AssetDatabase.FindAssets("t:texture", new string[] { "Assets/Resources/Effects" });

        for (int i = 0; i < alleffecttexture.Length; ++i)
        {
            string effectTexturePath = AssetDatabase.GUIDToAssetPath(alleffecttexture[i]);

            if (!allEffectTextureDict.ContainsKey(effectTexturePath))
            {
                allEffectTextureDict.Add(effectTexturePath, 0);
                allEffectTextureDictStr.Add(effectTexturePath, new List<string>());
            }
        }


        string[] alleffectprefab = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });

        for (int i = 0; i < alleffectprefab.Length; ++i)
        {
            string effectPath = AssetDatabase.GUIDToAssetPath(alleffectprefab[i]);
            string[] allDependence = AssetDatabase.GetDependencies(effectPath);

            EditorUtility.DisplayProgressBar(Path.GetFileName(effectPath), effectPath, 1.0f * i / alleffectprefab.Length);
            for (int j = 0; j < allDependence.Length; ++j)
            {

                if (allEffectTextureDict.ContainsKey(allDependence[j]))
                {
                    allEffectTextureDict[allDependence[j]]++;
                    allEffectTextureDictStr[allDependence[j]].Add((effectPath));

                    EditorUtility.DisplayProgressBar(Path.GetFileName(effectPath), string.Format("{0}\n->{1}", effectPath, allDependence[j]), 1.0f * i / alleffectprefab.Length);
                }
            }
        }

        EditorUtility.ClearProgressBar();

        string ans = "Path, Count, Size, Refs\n";
        foreach (var kv in allEffectTextureDict)
        {
            var list = allEffectTextureDictStr[kv.Key];
            ans += string.Format("{0}, {1}, {2}, {3}\n", kv.Key, kv.Value, new System.IO.FileInfo(kv.Key).Length, string.Join("|", list.ToArray()));
        }

        File.WriteAllText("特效没有使用的贴图.csv", ans);
    }
    //[MenuItem("[TM工具集]/转表工具/test #&%L")]
    static void GetPrefabs()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UI/Prefabs" });

        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            var com = data.GetComponent<ComClientFrame>();
            if (null != com)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                PrefabUtility.InstantiatePrefab(go);
                Logger.LogErrorFormat("path {0}", path);
            }

            data = null;
        }
        Logger.LogErrorFormat("finish ");
    }

    static string[] GetFileList(bool withAll = false)
    {
        List<string> finalList = new List<string>();

        var strList = Directory.GetFiles(SCENEN_DATA_PATH, "*.asset", withAll ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        for (int i = 0; i < strList.Length; i++)
        {
            strList[i] = strList[i].Replace('\\', '/');
            if (strList[i].EndsWith(".asset"))
            {
                finalList.Add(strList[i]);
            }
        }
        strList = finalList.ToArray();
        UnityEngine.Debug.Log(string.Join("\n", strList));

        return strList;
    }

    static void _convertelayFile2Text(string path)
    {
        _printLog("[录像] 处理 {0} ", path);

        if (!File.Exists(path))
        {
            _printLog("[录像] {0} 不存在", path);
            return;
        }

        byte[] buff = System.IO.File.ReadAllBytes(path);
        Protocol.ReplayFile replayFile = new Protocol.ReplayFile();


        int pos = 0;
        replayFile.decode(buff, ref pos);

        string content = ObjectDumper.Dump(replayFile);

        Protocol.SceneDungeonStartRes rep = new Protocol.SceneDungeonStartRes();
        rep.decode(buff, ref pos);

        rep.key1 = 0;
        rep.key2 = 0;
        rep.key3 = 0;
        rep.key4 = 0;

        rep.bossDropItems = null;
        rep.md5 = null;

        if (null != rep.players && rep.players.Length > 0)
        {
            path += "_";
            path += rep.players[0].serverName;
        }

        System.IO.File.WriteAllText(path + "_RelayFile_log.txt", content);

        for (int i = 0; i < rep.areas.Length; ++i)
        {
            foreach (var monster in rep.areas[i].monsters)
            {
                monster.dropItems = null;
            }

            foreach (var des in rep.areas[i].destructs)
            {
                des.dropItems = null;
            }

            if (null != rep.hellInfo)
            {
                rep.hellInfo.dropItems = null;
            }
        }

        content = ObjectDumper.Dump(rep);
        System.IO.File.WriteAllText(path + "_SceneDungeonStartRes_log.txt", content);
    }

    [MenuItem("验证服务器日志/Dump")]
    public static void ConvertelayFile2Text()
    {
        ConvertelayFile2TextWithRoot(Global.Settings.scenePath);
    }

    public static void _printLog(string format, params object[] args)
    {
#if LOGIC_SERVER
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format(format, args));
#else
        UnityEngine.Debug.LogErrorFormat(format, args);
#endif
    }

    public static void ConvertelayFile2TextWithRoot(string path)
    {
        if (!Directory.Exists(path))
        {
            _printLog("[验证服务器日志] {0}", path);
            return;
        }

        _printLog("[验证服务器日志] {0}", path);

        string[] allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        
        for (int i = 0; i < allFiles.Length; ++i)
        {
            if (!allFiles[i].Contains("."))
            {
                _printLog("[验证服务器日志] {0}", allFiles[i]);

                try
                {
                    _convertelayFile2Text(allFiles[i]);
                }
                catch (System.Exception e)
                {
                    _printLog("[验证服务器日志] error {0}", e.ToString());
                }
            }
        }

        _printLog("[验证服务器日志] 总数目 {0}", allFiles.Length);
    }

    static string[] GetFileListPrefab(string path, string type)
    {
        List<string> finalList = new List<string>();
        string[] strList = Directory.GetFiles(path, string.Format("*.{0}", type), SearchOption.AllDirectories);
        for (int i = 0; i < strList.Length; i++)
        {
            strList[i] = strList[i].Replace('\\', '/');
            finalList.Add(strList[i]);
        }
        strList = finalList.ToArray();
        UnityEngine.Debug.Log(string.Join("\n", strList));

        return strList;
    }

    private static Dictionary<int, List<ProtoTable.DropItemTable>> mChestCache = new Dictionary<int, List<ProtoTable.DropItemTable>>();

    private static bool _checkValidDungeonChestTable(int id)
    {
        bool re = false;

        string errorMsg = "";

        if (mChestCache.ContainsKey(id))
        {
            List<ProtoTable.DropItemTable> list = mChestCache[id];

            if (null != list)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    ProtoTable.DropItemTable item = list[i];
                    if (null != item)
                    {
                        if (item.DataType == 1)
                        {
                            re = _checkValidDungeonChestTable(item.ItemID);

                            if (!re)
                            {
                                errorMsg += string.Format(", 在[掉落表]中找不到GroupID {0}", item.ItemID);
                            }
                        }
                        else
                        {
                            re = _checkItem(item.ItemID);

                            if (!re)
                            {
                                errorMsg += string.Format(", 在[道具表]中找不到ItemID {0}", item.ItemID);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            errorMsg += string.Format(", 在[掉落表]中找不到GroupID {0}", id);
        }

        if (!re)
        {
            Logger.LogErrorFormat("[结算掉落表] {0}", errorMsg);
        }

        return re;
    }

    private static bool _checkItem(int itemID)
    {
        var itemData = TableManagerEditor.instance.GetTableItem<ItemTable>(itemID);
        bool re = null != itemData;
        if (!re)
        {
            Logger.LogErrorFormat("[结算掉落表] 道具表中找不到ID {0}", itemID);
        }
        return re;
    }

    [MenuItem("[TM工具集]/转表工具/检查/结算宝箱表")]
    static void CheckDungeonChestTable()
    {
        var droptb = TableManagerEditor.instance.GetTable<ProtoTable.DropItemTable>();
        mChestCache.Clear();
        foreach (var kv in droptb)
        {
            ProtoTable.DropItemTable fk = kv.Value as ProtoTable.DropItemTable;
            if (null != fk)
            {
                List<ProtoTable.DropItemTable> list = new List<ProtoTable.DropItemTable>();

                if (!mChestCache.ContainsKey(fk.GroupID))
                {
                    mChestCache.Add(fk.GroupID, list);
                }
                else
                {
                    list = mChestCache[fk.GroupID];
                }

                if (null != list)
                {
                    list.Add(fk);
                }
            }
        }
        

        var tb = TableManagerEditor.instance.GetTable<ProtoTable.DungeonChestTable>();
        foreach (var kv in tb)
        {
            var fk = kv.Value as ProtoTable.DungeonChestTable;
            if (null != fk)
            {
                UnityEngine.Debug.LogWarningFormat("[结算掉落表] 开始检查 {0}", fk.ID);

                if (!_checkValidDungeonChestTable(fk.NormalChestID))
                {
                    Logger.LogErrorFormat("[结算掉落表] 出错, 普通翻牌 {0}", fk.NormalChestID);
                }

                if (! _checkValidDungeonChestTable(fk.PayChestID))
                {
                    Logger.LogErrorFormat("[结算掉落表] 出错, VIP翻牌 {0}", fk.PayChestID);
                }

                UnityEngine.Debug.LogWarningFormat("[结算掉落表] 结束检查 {0}", fk.ID);
            }

        }
    }

    [MenuItem("[TM工具集]/转表工具/阻挡asset转xml v3")]
    static void GenerateMapv3()
    {
        List<DTownDoor> doors = new List<DTownDoor>();

        var tableItem = TableManagerEditor.instance.GetTable<ProtoTable.CitySceneTable>();
        foreach (var item in tableItem)
        {
            var table = item.Value as ProtoTable.CitySceneTable;
            if (table != null)
            {
                var data = AssetDatabase.LoadAssetAtPath<DSceneData>(_Convert2EditorPath(table.ResPath));
                if (data != null)
                {
                    string byteStr = data._blocklayer[0].ToString();
                    for (int i = 1; i < data._blocklayer.Length; i++)
                    {
                        byteStr += "," + data._blocklayer[i].ToString();
                    }

                    var pos = data._birthposition.position;
                    var posX = data._LogicXSize;
                    var posZ = data._LogicZSize;

                    string finalStr = string.Format("<Floor Row=\"{0}\" Col=\"{1}\">{2}</Floor><Birth X={3} Y={4}></Birth><GirdSize W={5} H={6}></GirdSize>"
                        , data.LogicZ, data.LogicX, byteStr, (int)((pos.x - posX.x) * 10000), (int)((pos.z - posZ.x) * 10000), data._GridSize.x, data._GridSize.y);

                    string ushortStr = string.Empty;
                    string ecosystemStr = string.Empty;
                    if (data._ecosystemLayer.Length > 0)
                    {
                        ushortStr = data._ecosystemLayer[0].ToString();
                        for (int i = 1; i < data._ecosystemLayer.Length; i++)
                        {
                            ushortStr += "," + data._ecosystemLayer[i].ToString();
                        }
                        ecosystemStr = string.Format("<EcosystemRegion> {0}</EcosystemRegion>", ushortStr);
                    }

                    string resourceStr = string.Empty;
                    for (int i = 0; i < data._resourcePosition.Length; i++)
                    {
                        var resPos = data._resourcePosition[i].position;
                        resourceStr += string.Format(
                           "<Resource X={0} Y={1} resourceID={2}></Resource>",
                           (int)((resPos.x - posX.x) * 10000),
                           (int)((resPos.z - posZ.x) * 10000),
                           data._resourcePosition[i].resouceId);
                    }

                    string transferStr = string.Empty;
                    for (int i = 0; i < data._fighterBornPosition.Length; i++)
                    {
                        var resPos = data._fighterBornPosition[i].position;
                        transferStr += string.Format(
                           "<Transfer X={0} Y={1} regionId={2}></Transfer>",
                           (int)((resPos.x - posX.x) * 10000),
                           (int)((resPos.z - posZ.x) * 10000),
                           data._fighterBornPosition[i].regionId);
                    }

                    string ecosystemResStr = string.Empty;
                    for(int i = 0;i < data._ecosystemResoucePosition.Length;i++)
                    {
                        var resPos = data._ecosystemResoucePosition[i].position;
                        ecosystemResStr += string.Format(
                           "<EcosystemResource X={0} Y={1} ecosystemId={2} ecosystemType={3}></EcosystemResource>",
                           (int)((resPos.x - posX.x) * 10000),
                           (int)((resPos.z - posZ.x) * 10000),
                           data._ecosystemResoucePosition[i].ecosystemResourceId, data._ecosystemResoucePosition[i].ecosystemResourceType);
                    }

                    List<int> doorIDs = new List<int>();

                    string doorStr = "";
                    for (int i = 0; i < data._townDoor.Length; i++)
                    {
                        var door = data._townDoor[i];
                        if (doorIDs.Contains(door.DoorID))
                        {
                            Logger.LogErrorFormat("[SceneData->{0}] ==> DoorId:{1} is already exist!!", table.ResPath, door.DoorID);
                            continue;
                        }

                        doorIDs.Add(door.DoorID);

                        if (door.SceneID != table.ID)
                        {
                            Logger.LogErrorFormat("[SceneData->{0}][door->{1}] ==> SceneID must be {2}!!", table.ResPath, door.DoorID, table.ID);
                            continue;
                        }

                        var targetScene = TableManagerEditor.instance.GetTableItem<ProtoTable.CitySceneTable>(door.TargetSceneID);
                        if (targetScene == null)
                        {
                            Logger.LogErrorFormat("[SceneData->{0}][door->{1}] ==> TargetSceneID:{2} is not exist!!", table.ResPath, door.DoorID, door.TargetSceneID);
                            continue;
                        }

                        bool bFound = false;
                        var sceneData = AssetDatabase.LoadAssetAtPath<DSceneData>(_Convert2EditorPath(targetScene.ResPath));
                        for (int j = 0; j < sceneData._townDoor.Length; j++)
                        {
                            var nextdooritem = sceneData._townDoor[j];
                            if (sceneData._townDoor[j].DoorID == door.TargetDoorID)
                            {
                                bFound = true;
                                break;
                            }
                        }

                        if (bFound == false)
                        {
                            Logger.LogErrorFormat("[SceneData->{0}][door->{1}][TargetScene->{2}] ==> TargetDoorID is not exist!!", table.ResPath, door.DoorID, door.TargetSceneID);
                            continue;
                        }

                        int startposX = (int)((door.BirthPos.x + door.position.x - posX.x) * 10000);
                        if (startposX < 0)
                        {
                            Logger.LogErrorFormat("[SceneData->{0}][door->{1}] ==> startposX < 0!!", table.ResPath, door.DoorID);
                            continue;
                        }

                        int startPosZ = (int)((door.BirthPos.z + door.position.z - posZ.x) * 10000);
                        if (startPosZ < 0)
                        {
                            Logger.LogErrorFormat("[SceneData->{0}][door->{1}] startPosZ < 0!!", table.ResPath, door.DoorID);
                            continue;
                        }

                        bool bMatch = false;
                        for (int idx = 0; idx < doors.Count; ++idx)
                        {
                            var temp = doors[idx];
                            if (
                                door.TargetSceneID == temp.SceneID && door.TargetDoorID == temp.DoorID &&
                                temp.TargetSceneID == door.SceneID && temp.TargetDoorID == door.DoorID
                                )
                            {
                                doors.RemoveAt(idx);
                                bMatch = true;
                                break;
                            }
                        }
                        if (bMatch == false)
                        {
                            doors.Add(door);
                        }

                        // TODO 检查门的位置是否在阻挡内


                        if (door.regiontype == DRegionInfo.RegionType.Rectangle)
                        {
                            if (
                                Mathf.Abs(door.BirthPos.x) <= Mathf.Abs(door.rect.x) * 0.5f &&
                                Mathf.Abs(door.BirthPos.z) <= Mathf.Abs(door.rect.y) * 0.5f
                                )
                            {
                                Logger.LogErrorFormat("[SceneData->{0}][door->{1}] ==> BirthPos can not in trigger range!!", table.ResPath, door.DoorID);
                                continue;
                            }
                        }
                        else if (door.regiontype == DRegionInfo.RegionType.Circle)
                        {
                            if (door.BirthPos.sqrMagnitude <= door.radius * door.radius)
                            {
                                Logger.LogErrorFormat("[SceneData->{0}][door->{1}] ==> BirthPos can not in trigger range!!", table.ResPath, door.DoorID);
                                continue;
                            }
                        }

                        doorStr += string.Format(
                            "<Door DoorID={0} TragetSceneID={1} TargetDoorID={2} DoorStartPosX={3} DoorStartPosY={4}></Door>",
                            door.DoorID,
                            door.TargetSceneID,
                            door.TargetDoorID,
                            startposX,
                            startPosZ
                            );
                    }

                    string npcStr = _generateNpc(data);

                    finalStr = string.Format("<Map>{0}{1}{2}{3}{4}{5}{6}</Map>", finalStr, doorStr, npcStr, resourceStr, transferStr,ecosystemStr, ecosystemResStr);

                    UnityEngine.Debug.Log(finalStr);


                    //File.WriteAllText(Path.Combine(XML_PATH, table.ID.ToString() + ".xml"), finalStr);
                    string directory = Path.Combine(XML_PATH, table.ID.ToString());
                    if (CFileManager.CreateDirectory(directory))
                    {
                        File.WriteAllText(Path.Combine(XML_PATH, table.ID.ToString() + "/Config.xml"), finalStr);
                    }
                }
                else
                {
                    // DChapterData
                }
            }
            else
            {

            }
        }

        for (int i = 0; i < doors.Count; ++i)
        {
            Logger.LogErrorFormat(
                "SceneID:{0} door:{1} has no back door!!!!!!",
                doors[i].SceneID,
                doors[i].DoorID
                );
        }
    }

    static string _generateNpc(DSceneData data)
    { 
        string npcStr = "";
        for (int i = 0; i < data._npcinfo.Length; i++)
        {
            var npc = data._npcinfo[i];
           
            npcStr += string.Format(
                "<Npc NpcID={0} posX={1} posY={2}></Npc>",
                npc.resid,
                (int)(npc.position.x * 10000),
                (int)(npc.position.y * 10000)
                );
        }

        return npcStr;
    }

    static bool _checkMonsterType(int id, DMonsterInfo info)
    {
        var tableData = TableManagerEditor.instance.GetTableItem<ProtoTable.UnitTable>(id);

        if (null != tableData)
        {
            switch (tableData.Type)
            {
                case UnitTable.eType.MONSTER:
                    if (info.type != DEntityType.MONSTER) { info.type = DEntityType.MONSTER; return true; }
                    break;
                case UnitTable.eType.ELITE:
                    if (info.type != DEntityType.ELITE) { info.type = DEntityType.ELITE; return true; }
                    break;
                case UnitTable.eType.BOSS:
                    if (info.type != DEntityType.BOSS) { info.type = DEntityType.BOSS; return true; }
                    break;
                case UnitTable.eType.SKILL_MONSTER:
                    if (info.type != DEntityType.MONSTERDESTRUCT) { info.type = DEntityType.MONSTERDESTRUCT; return true; }
                    break;
            }
        }

        return false;
    }

    static bool _checkMonsterID(int monsterID, out int sugguestID)
    {
        sugguestID = -1;

        var diff = monsterID % 10;
        var type = monsterID / 10 % 10;
        var level = monsterID / 100 % 100;
        var id = monsterID / 10000;

        var idInTab = monsterID / 10000 * 10000 + monsterID % 100;

        var tableData = TableManagerEditor.instance.GetTableItem<ProtoTable.UnitTable>(idInTab);
        if (tableData != null)
        {
            return true;
        }

        for (int i = 1; i <= 3; ++i)
        {
            var nid = id * 10000 + i * 10 + diff;
            var data = TableManagerEditor.instance.GetTableItem<ProtoTable.UnitTable>(nid);
            if (data != null)
            {
                sugguestID = id * 10000 + level * 100 + i * 10 + diff;
                break;
            }
        }

        return false;
    }

    static string _Convert2EditorPath(string path)
    {
        string newPath = "Assets/Resources/" + path;

        if (!newPath.EndsWith(".asset"))
        {
            newPath += ".asset";
        }

        return newPath;
    }

    [MenuItem("[TM工具集]/转表工具/SceneAsset转MonsterList2 -> DungeonAreaTable")]
    static void GenerateMonsterListWithGroupID2()
    {
        string ansStr = "";

        TableManagerEditor.instance.ReloadTable(typeof(ProtoTable.DungeonTable));
        var dungeonTable = TableManagerEditor.instance.GetTable<ProtoTable.DungeonTable>();
        var globalId = 5;
        var idx = 0;
        var count = dungeonTable.Count;
        var commitList = "AreaTable.txt & DungeonTable.txt";

        int skipDungeonIDRange = 100;


        //List<string> renumberList = new List<string>();
        //List<int> hasNumberList = new List<int>();
        //foreach (var kv in dungeonTable)
        //{
        //    ProtoTable.DungeonTable dungeonData = kv.Value as ProtoTable.DungeonTable;
        //    if (dungeonData.ID < skipDungeonIDRange)
        //    {
        //        continue;
        //    }
        //    var newpath = "Assets/Resources/" + dungeonData.DungeonConfig + ".asset";
        //    if (System.IO.File.Exists(newpath))
        //    {
        //        var dungeonAssetData = AssetDatabase.LoadAssetAtPath<DDungeonData>(newpath);
        //        if (null != dungeonAssetData)
        //        {
        //            for (int i = 0; i < dungeonAssetData.are
        //            hasNumberList.Find(x=>{ return x == dungeonAssetData.are });
        //        }
        //    }
        //}
        //

        Dictionary<int, int> idmaps = new Dictionary<int, int>();
        Dictionary<int, bool> idmapsflags = new Dictionary<int, bool>();
        Dictionary<string, bool> maxGlobalIdFlag = new Dictionary<string, bool>();

        { 
            foreach (var kv in dungeonTable)
            {
                var dungeonData = kv.Value as ProtoTable.DungeonTable;

                if (dungeonData.ID < skipDungeonIDRange)
                {
                    continue;
                }

                var newpath = _Convert2EditorPath(dungeonData.DungeonConfig);

                UnityEngine.Debug.Log("path : " + newpath);

                EditorUtility.DisplayProgressBar
                    ("(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง",
                    "ヾ(。◕ฺ∀◕ฺ)ノ " + Path.GetFileName(newpath),
                    1.0f * idx++ / count);

                if (System.IO.File.Exists(newpath))
                {
                    if (maxGlobalIdFlag.ContainsKey(newpath))
                    {
                        continue;
                    }

                    maxGlobalIdFlag.Add(newpath, true);

                    var dungeonAssetData = AssetDatabase.LoadAssetAtPath<DDungeonData>(newpath);

                    if (null != dungeonAssetData)
                    {
                        foreach (var con in dungeonAssetData.areaconnectlist)
                        {
                            if (!idmaps.ContainsKey(con.id))
                            {
                                idmaps.Add(con.id, 1);
                            }
                            else
                            {
                                idmaps[con.id]++;
                            }

                            globalId = Mathf.Max(globalId, con.id);
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }

        Dictionary<KeyValuePair<string, int>, bool> hasVisited = new Dictionary<KeyValuePair<string, int>, bool>();
        DungeonID did = new DungeonID(0);
        idx = 0;

        foreach (var kv in dungeonTable)
        {
            var dungeonData = kv.Value as ProtoTable.DungeonTable;

            if (dungeonData.ID < skipDungeonIDRange)
            {
                continue;
            }

            var newpath = _Convert2EditorPath(dungeonData.DungeonConfig);
            
            UnityEngine.Debug.Log("path : " + newpath);

            EditorUtility.DisplayProgressBar
                ("(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง"+globalId.ToString(),
                "ヾ(。◕ฺ∀◕ฺ)ノ " + Path.GetFileName(newpath),
                1.0f * idx++ / count);

            did.dungeonID = dungeonData.ID;

            if (System.IO.File.Exists(newpath))
            {
                var kyv = new KeyValuePair<string, int>(newpath, did.diffID);

                if (!hasVisited.ContainsKey(kyv))
                {
                    hasVisited.Add(kyv, true);
                }
                else 
                {
                    UnityEngine.Debug.LogErrorFormat("the path {0}, is has transformed", newpath);
                    continue;
                }

                var dungeonAssetData = AssetDatabase.LoadAssetAtPath<DDungeonData>(newpath);

                if (null != dungeonAssetData)
                {
                    for (int ii = 0; ii < dungeonAssetData.areaconnectlist.Length; ++ii)
                    {
                        var sceneData = dungeonAssetData.areaconnectlist[ii];

                        if (!idmaps.ContainsKey(sceneData.id))
                        {
                            sceneData.id = ++globalId;

                            if (dungeonData.ID > 0)
                            {
                                EditorUtility.SetDirty(dungeonAssetData);
                            }
                        }
                        else
                        {
                            if (sceneData.id == 0)
                            {
                                sceneData.id = ++globalId;

                                if (dungeonData.ID > 0)
                                {
                                    EditorUtility.SetDirty(dungeonAssetData);
                                }
                            }
                            else
                            {
                                int cnt = idmaps[sceneData.id];
                                if (cnt > 1)
                                {
                                    if (idmapsflags.ContainsKey(sceneData.id))
                                    {
                                        sceneData.id = ++globalId;

                                        if (dungeonData.ID > 0)
                                        {
                                            EditorUtility.SetDirty(dungeonAssetData);
                                        }
                                    }
                                    else
                                    {
                                        idmapsflags.Add(sceneData.id, true);
                                    }
                                }
                            }
                        }

                        {
                            var item = "Assets/Resources/" + sceneData.sceneareapath + ".asset";

                            var data = AssetDatabase.LoadAssetAtPath<DSceneData>(item);

                            if (data == null)
                            {
                                EditorUtility.ClearProgressBar();
                                EditorUtility.DisplayDialog("错误", newpath + " 丢失 " + item, "ok");
                                return;
                            }

                            // check
                            // {
                            //     Logger.LogError("====================>data._prefab" + data._prefab);
                            //     var path = FileTools.GetAssetPath(data._prefab);
                            //     if (path != data._prefabpath)
                            //     {
                            //         if (EditorUtility.DisplayDialog("DSceneData错误", string.Format("{0}, 是否prefab路径 {1} 用 {2} 替换", item, data._prefabpath, path), "ok"))
                            //         {
                            //             data._prefabpath = path;
                            //             if (dungeonData.ID > 0)
                            //             {
                            //                 EditorUtility.SetDirty(data);
                            //             }
                            //         }
                            //         else
                            //         {
                            //             EditorUtility.ClearProgressBar();
                            //             EditorGUIUtility.PingObject(data);
                            //             return;
                            //         }
                            //     }
                            // }

                            int id = sceneData.id;

                            var monstlist = data._monsterinfo;
                            var desctrucList = data._desructibleinfo;
                            var doorList = data._transportdoor;


                            if (dungeonData.Type != ProtoTable.DungeonTable.eType.L_DEADTOWER)
                            {
                                // 检查传送门
                                bool[] isDup = new bool[] { false, false, false, false };
                                for (int i = 0; i < 4; ++i)
                                {
                                    if (!sceneData.isconnect[i])
                                    {
                                        continue;
                                    }

                                    var type = (TransportDoorType)i;

                                    bool flag = false;
                                    for (int j = 0; j < doorList.Length; ++j)
                                    {
                                        if (doorList[j].DoorType == type)
                                        {
                                            if (!isDup[j])
                                            {
                                                isDup[j] = true;
                                                flag = true;
                                            }
                                            else
                                            {
                                                EditorUtility.ClearProgressBar();
                                                if (EditorUtility.DisplayDialog("传送门重复", string.Format("传送门类型重复 {0}, {1}, {2}", doorList[i].DoorType, doorList[i].Name, item), "ok"))
                                                {
                                                    EditorGUIUtility.PingObject(data);
                                                }
                                                return;
                                            }
                                            break;
                                        }
                                    }

                                    if (!flag)
                                    {
                                        EditorUtility.ClearProgressBar();
                                        if (EditorUtility.DisplayDialog("没有传送门", string.Format("传送门类型 {0}, {1}", type, item), "ok"))
                                        {
                                            EditorGUIUtility.PingObject(data);
                                        }
                                        return;
                                    }
                                }
                            }

                            //for (int j = 1; j <= 4; j++)
                            {
                                int j = did.diffID + 1;
                                // pointid
                                // id, group, areaid, type, unitid, flushgroupid, level
                                for (int i = 0; i < monstlist.Length; i++)
                                {
                                    var monsterResIDWithDiff = monstlist[i].resid;
                                    var areaIDWithDiff = id * 10 + j;
                                    var monsterIDWithDiff = (id * 10 + j) * 100 + i;

                                    monstlist[i].SetID(monsterResIDWithDiff);

                                    var level = monstlist[i].monLevel;

                                    monstlist[i].monLevel = 0;
                                    monsterResIDWithDiff = monstlist[i].resid;

                                    if (monsterResIDWithDiff > 10000)
                                    {
                                        monsterResIDWithDiff = monstlist[i].resid / 10 * 10 + j;
                                    }

                                    int suggeustId = -1;
                                    bool dirtyFlag = false;

                                    //if (!_checkMonsterType(monsterIDWithDiff, monstlist[i]))
                                    //{
                                    //    EditorUtility.SetDirty(data);
                                    //}

                                    if (!_checkMonsterID(monsterResIDWithDiff, out suggeustId))
                                    {
                                        if (suggeustId != -1)
                                        {
                                            if (EditorUtility.DisplayDialog("没有怪物", string.Format("无法找到 {0}, 是否用 {1} 替换, 替换之后 请提交 {2}", monsterResIDWithDiff, suggeustId, item), "ok", "cancel"))
                                            {
                                                monstlist[i].SetID(suggeustId);
                                                monsterResIDWithDiff = monstlist[i].resid / 10 * 10 + j;
                                                dirtyFlag = true;
                                            }
                                        }
                                        else
                                        {
                                            EditorUtility.ClearProgressBar();
                                            if (EditorUtility.DisplayDialog("错误", string.Format("怪物表中没有ID {0}, item {1}", monsterResIDWithDiff, item), "ok"))
                                            {
                                                EditorGUIUtility.PingObject(data);
                                            }
                                            return;
                                        }
                                    }

                                    bool fkflag = true;
                                    var fkunit = TableManagerEditor.instance.GetTableItem<UnitTable>(monsterResIDWithDiff);
                                    if (null != fkunit)
                                    {
                                        if (fkunit.Type == UnitTable.eType.SKILL_MONSTER)
                                        {
                                            //fkflag = false;
                                        }
                                    }

                                    if (fkflag)
                                    {
										// pointid
										// id, group, areaid, type, unitid, flushgroupid, level
										//
										// type:0 -> 普通怪 
										// type:1 -> 组刷怪
										// type:2 -> 破坏物
										// type:3 -> 活动普通
										// type:4 -> 活动精英
										// type:5 -> 活动boss

										int type = 0;
										int flushGroupID = 0;

										switch (monstlist[i].type)
										{
										case DEntityType.ACTIVITY_BOSS_POS:
											type = 5;
											break;
										case DEntityType.ACTIVITY_ELITE_POS:
											type = 4;
											break;
										case DEntityType.ACTIVITY_MONSTER_POS:
											type = 3;
											break;
										case DEntityType.BOSS:
										case DEntityType.ELITE:
										case DEntityType.MONSTER:
                                        case DEntityType.MONSTERDESTRUCT:
                                            type = monstlist[i].flushGroupID > 0 ? 1 : 0;
											flushGroupID = monstlist[i].flushGroupID > 0 ? monstlist[i].flushGroupID : monsterResIDWithDiff;
											break;
										}

                                        ansStr += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n",
                                            monsterIDWithDiff,
                                            0,
                                            areaIDWithDiff,
											type,
											flushGroupID,
                                            0,
                                            level);
                                    }

                                    monstlist[i].monLevel = level;

                                    if (dirtyFlag)
                                    {
                                        monstlist[i].globalid = monstlist[i].globalid % 100 + monstlist[i].resid * 100;
                                        if (dungeonData.ID > 0)
                                        {
                                            EditorUtility.SetDirty(data);
                                        }
                                    }

                                    //var unitItem = TableManagerEditor.instance.GetTableItem<ProtoTable.UnitTable>(monsterResIDWithDiff);
                                    //if (unitItem == null)
                                    //{
                                    //    UnityEngine.Debug.LogErrorFormat("单位表中没有 ID 为 {0} 的字段, 怪物 {1}, with {2}", monsterResIDWithDiff, monstlist[i].name, item);
                                    //}
                                }
                            }

                            // pointid
                            // id, group, areaid, type, unitid, flushgroupid, level
                            // type : 0 -> 单刷, unitid 怪物id
                            // type : 1 -> 组刷， 刷怪组id
                            // type : 2 -> 破坏物，
                            //for (int j = 1; j <= 4; ++j)
                            {
                                int j = did.diffID + 1;
                                for (int i = 0; i < desctrucList.Length; ++i)
                                {
                                    var destructResIDWithDiff = desctrucList[i].resid;
                                    var areaIDWithDiff = id * 10 + j;
                                    var destructIDWithDiff = (id * 10 + j) * 100 + i + monstlist.Length;

                                    ansStr += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n",
                                        destructIDWithDiff,
                                        0,
                                        areaIDWithDiff,
                                        2,
                                        destructResIDWithDiff,
                                        desctrucList[i].flushGroupID,
                                        1);

                                    //var unitItem = TableManagerEditor.instance.GetTableItem<ProtoTable.DestrucTable>(destructResIDWithDiff);
                                    //if (unitItem == null)
                                    //{
                                    //    UnityEngine.Debug.LogErrorFormat("可破坏物表中没有 ID 为 {0} 的字段,可破坏物品 {1}, with {2}", destructResIDWithDiff, desctrucList[i].name, item);
                                    //}
                                }
                            }

                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("the file not DDungeonData : " + newpath);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("the file not exist : " + newpath);
            }


        }

        File.WriteAllText(Path.Combine(SERVER_TEXT_PATH, "DungeonAreaTable.txt"), ansStr);

        EditorUtility.DisplayProgressBar(" =≡Σ((( つ•̀ω•́)つ =≡Σ((( つ•̀ω•́)つ =≡Σ((( つ•̀ω•́)つ", " 最后10秒！", 0.98f);
#if USE_FB_TABLE
        Xls2FBWindow.DoConvertAFile("D-地下城表.xls");
#else
        var unit = XlsxDataManager.Instance().GetXlsxByName("DungeonTable");
        unit.GetText();
#endif

        EditorUtility.DisplayProgressBar(" =≡Σ((( つ•̀ω•́)つ =≡Σ((( つ•̀ω•́)つ =≡Σ((( つ•̀ω•́)つ", " 最后3秒！", 0.99f);

        AssetDatabase.SaveAssets();

        EditorUtility.ClearProgressBar();

        EditorUtility.DisplayDialog("提交", string.Format("请提交 {0}", commitList), "ok");

    }

    [MenuItem("[TM工具集]/转表工具/统计怪物")]
    static void GenerateMonsterData()
    {
        string ansStr = "";

        var dungeonTable = TableManagerEditor.instance.GetTable<ProtoTable.DungeonTable>();
        var idx = 0;
        var count = dungeonTable.Count;

        ansStr = "地下城ID,区域名字,区域ID,普通数量,精英数量,Boss数量,总数量\n";

        foreach (var kv in dungeonTable)
        {
            var dungeonData = kv.Value as ProtoTable.DungeonTable;
            var newpath = "Assets/Resources/" + dungeonData.DungeonConfig + ".asset";
            UnityEngine.Debug.Log("path : " + newpath);

            EditorUtility.DisplayProgressBar
                ("(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง(ง •̀_•́)ง",
                "ヾ(。◕ฺ∀◕ฺ)ノ " + newpath,
                1.0f * idx++ / count);

            if (dungeonData.ID == 0 || dungeonData.ID % 10 > 0)
            {
                continue;
            }

            if (System.IO.File.Exists(newpath))
            {
                var dungeonAssetData = AssetDatabase.LoadAssetAtPath<DDungeonData>(newpath);


                if (null != dungeonAssetData)
                {
                    var aky = new Dictionary<DEntityType, string>();
                    aky.Add(DEntityType.BOSS, "");
                    aky.Add(DEntityType.ELITE, "");
                    aky.Add(DEntityType.MONSTER, "");
                    aky.Add(DEntityType.MONSTERDESTRUCT, "");

                    var acky = new Dictionary<int, int>();

                    for (int ii = 0; ii < dungeonAssetData.areaconnectlist.Length; ++ii)
                    {
                        var sceneData = dungeonAssetData.areaconnectlist[ii];

                        {
                            var item = "Assets/Resources/" + sceneData.sceneareapath + ".asset";

                            var data = AssetDatabase.LoadAssetAtPath<DSceneData>(item);

                            int id = sceneData.id;

                            var monstlist = data._monsterinfo;
                            var desctrucList = data._desructibleinfo;

                            var ky = new Dictionary<DEntityType, string>();
                            ky.Add(DEntityType.BOSS, "");
                            ky.Add(DEntityType.ELITE, "");
                            ky.Add(DEntityType.MONSTER, "");
                            ky.Add(DEntityType.MONSTERDESTRUCT, "");

                            var cky = new Dictionary<int, int>();

                            foreach (var monster in monstlist)
                            {
                                //ky[monster.type] += monster.resid.ToString() + "|";
                                //aky[monster.type] += monster.resid.ToString() + "|";

                                if (!acky.ContainsKey(monster.resid))
                                {
                                    acky.Add(monster.resid, 0);
                                }
                                acky[monster.resid]++;


                                if (!cky.ContainsKey(monster.resid))
                                {
                                    cky.Add(monster.resid, 0);
                                }
                                cky[monster.resid]++;
                                //ansStr += string.Format("{0},{1},{2},{3}\n", 
                                //    dungeonData.ID,
                                //    sceneData.id,
                                //    string.Format("{0}*{1}", dungeonAssetData.weidth, dungeonAssetData.height),
                                //    string.Format("<{0}-{1}>", sceneData.positionx, sceneData.positiony),
                                //    monster.type.GetDescription(), monster.name, monster.resid, monster.monID, monster.monTypeID, monster.monLevel);
                            }
                            string fkq = "";
                            foreach(var fkv in cky)
                            {
                                fkq += string.Format(",{0}({1})", fkv.Key, fkv.Value);
                            }

                            //ansStr = "地下城ID,区域名字,区域ID,普通数量,精英数量,Boss数量,总数量\n";
                            ansStr += string.Format("{0},{1},{2},{3},{4},{5},{6}\n",
                                dungeonData.ID,
                                dungeonData.Name,
                                sceneData.id,
                                fkq,
                                "",
                                "",
                                //ky[DEntityType.MONSTER],
                                //ky[DEntityType.ELITE],
                                //ky[DEntityType.BOSS],
                                "");
                                //ky[DEntityType.MONSTER] + ky[DEntityType.ELITE] + ky[DEntityType.BOSS]);

                        }
                    }

                    string afkq = "";
                    foreach (var fkv in acky)
                    {
                        afkq += string.Format(",{0}({1})", fkv.Key, fkv.Value);
                    }

                    ansStr += string.Format("{0},{1},{2},{3},{4},{5},{6}\n",
                                                                 dungeonData.ID,
                                                                 "*",
                                                                 "*",
                                                                 afkq,
                                                                 "",
                                                                 "",
                                                                 //aky[DEntityType.MONSTER],
                                                                 //aky[DEntityType.ELITE],
                                                                 //aky[DEntityType.BOSS],
                                                                 "");
                                                                 //aky[DEntityType.MONSTER] + aky[DEntityType.ELITE] + aky[DEntityType.BOSS]);
                                                                 //aky[DEntityType.MONSTER] + aky[DEntityType.ELITE] + aky[DEntityType.BOSS]);
                }
                else
                {
                    UnityEngine.Debug.LogError("the file not DDungeonData : " + newpath);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("the file not exist : " + newpath);
            }


        }

        EditorUtility.DisplayProgressBar(" =≡Σ((( つ•̀ω•́)つ =≡Σ((( つ•̀ω•́)つ =≡Σ((( つ•̀ω•́)つ", " 最后1秒！", 0.99f);

        File.WriteAllBytes(Path.Combine(ST_TEXT_PATH, "dungeon.csv"), System.Text.Encoding.Default.GetBytes(ansStr.ToCharArray()));

        EditorUtility.ClearProgressBar();

        if (EditorUtility.DisplayDialog("目录", Path.GetFullPath(Path.Combine(ST_TEXT_PATH, "dungeon.csv")), "ok"))
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = Path.GetFullPath(Path.Combine(ST_TEXT_PATH, "dungeon.csv"));
            processInfo.Arguments = "";

            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();

        }
    }

    private static bool _checkResData(int resId, DEntityInfo info)
    {
        var resData = TableManagerEditor.instance.GetTableItem<ProtoTable.ResTable>(resId);
        if (resData == null)
        {
            return false;
        }

        string path = resData.ModelPath + ".prefab";
        if (path != info.path)
        {
            UnityEngine.Debug.LogWarningFormat("new path {0}, \nold path {1}", path, info.path);
            info.path = path;
            return true;
        }

        return false;
    }

    //[MenuItem("[TM工具集]/转表工具/SceneAsset转修复,")]
    static void _FixSceneGroundWaterLevel()
    {
        foreach (var item in GetFileList(true))
        {
            var dirname = System.IO.Path.GetDirectoryName(item);
            if (dirname.EndsWith("SceneData"))
            {
                continue;
            }

            UnityEngine.Debug.Log("item : " + item);

            var data = AssetDatabase.LoadAssetAtPath<DSceneData>(item);
            data._ScenePostion.y = 0;
            EditorUtility.SetDirty(data);

            _NormalizeSceneWaterLevel(data._prefab);
            EditorUtility.SetDirty(data._prefab);
        }

        AssetDatabase.SaveAssets();
    }


    [MenuItem("[TM工具集]/转表工具/重新刷新技音频数据")]
    static void DealDSkillRefreshPath()
    {
        var str = AssetDatabase.FindAssets("t:DSkillData", new string[] { "Assets/Resources/Data/SkillData" });
        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            var data = AssetDatabase.LoadAssetAtPath<DSkillData>(path);

            data.goSFXAsset.Set(data.goSFX);

            for (int i = 0; i < data.sfx.Length; ++i)
                data.sfx[i].soundClipAsset.Set(data.sfx[i].soundClip);

            EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
    }

    static public GameObject _GetSceneGroundObj(GameObject sceneRoot)
    {
        if ("Ground" == sceneRoot.tag/* || (sceneRoot.name.Contains("ground") && null != sceneRoot.GetComponents<MeshRenderer>())*/)
            return sceneRoot;

        GameObject groundObj = null;
        int childNum = sceneRoot.transform.childCount;
        for (int i = 0; i < childNum; ++i)
        {
            Transform curChild = sceneRoot.transform.GetChild(i);

            groundObj = _GetSceneGroundObj(curChild.gameObject);
            if (null != groundObj)
                return groundObj;
        }


        return groundObj;
    }

    static public void _NormalizeSceneWaterLevel(GameObject sceneRoot)
    {
        /// Auto adjust scene ground to zero-plane.
        GameObject ground = _GetSceneGroundObj(sceneRoot);
        if (null != ground)
        {
            Vector3 sceneRootPos = sceneRoot.transform.position;
            sceneRootPos.y -= ground.transform.position.y + 0.05f;
            sceneRoot.transform.position = sceneRootPos;

            Logger.LogErrorFormat("Ground:{0}", ground.transform.position.y);
        }
    }
}
