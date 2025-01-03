using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GenerateMapEditorWindow : EditorWindow
{
    public List<CitySceneTableData> m_CitySceneDataList;
    public static string XML_PATH = "../Share/table/xml";
    private Vector2 m_pSelectedVec = new Vector2();
    public static GenerateMapEditorWindow editorWindow;

    private string mFilter = "";

    [MenuItem("[TM工具集]/转表工具/阻挡asset转xml v4")]
    public static void OpenWindow()
    {
        if (editorWindow != null)
            editorWindow.Close();
        editorWindow = GetWindow<GenerateMapEditorWindow>();
        editorWindow.Show();
    }

    void OnGUI()
    {
        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.HelpBox(string.Format("正在编译中\n"), MessageType.Warning);
            return;
        }

        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox(string.Format("游戏正在运行\n"), MessageType.Warning);
            return;
        }

        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.BeginVertical();
            {
                if (mFilter == null)
                    mFilter = "";
                var str = EditorGUILayout.TextField("筛选", mFilter);
                if (str != mFilter)
                {
                    mFilter = str;
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();

        m_pSelectedVec = EditorGUILayout.BeginScrollView(m_pSelectedVec, GUILayout.Height(500));
        {
            EditorGUILayout.BeginVertical("ObjectFieldThumb");
            {
                if (m_CitySceneDataList == null)
                    m_CitySceneDataList = BuildCitySceneList();
                for (int i = 0; i < m_CitySceneDataList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUI.indentLevel++;

                        CitySceneTableData info = m_CitySceneDataList[i];

                        if (i % 2 == 0)
                        {
                            GUI.color = Color.yellow;
                        }

                        if ((mFilter.Length <= 0 || info.tableData.ID.ToString().StartsWith(mFilter.ToLower()) ||
                                info.tableData.Name.StartsWith(mFilter.ToString()) || info.tableData.Desc.StartsWith(mFilter.ToString()) || info.toggle))
                        {
                            EditorGUILayout.LabelField(info.tableData.ID + "   " + info.tableData.Name + "   " + info.tableData.Desc, GUILayout.Width(400));

                            bool value = EditorGUILayout.Toggle("", info.toggle, GUILayout.Width(50));
                            if (value != info.toggle)
                            {
                                info.toggle = value;
                            }
                        }

                        //#endif
                        GUI.color = Color.white;

                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal("ObjectFieldThumb");
        {
            if (StyledButton("刷新"))
            {
                m_CitySceneDataList = BuildCitySceneList();
            }

            if (StyledButton("全选"))
            {
                for (int i = 0; i < m_CitySceneDataList.Count; i++)
                {
                    var info = m_CitySceneDataList[i];
                    info.toggle = true;
                }
            }

            if (StyledButton("反选"))
            {
                for (int i = 0; i < m_CitySceneDataList.Count; i++)
                {
                    var info = m_CitySceneDataList[i];
                    info.toggle = !info.toggle;
                }
            }

            if (StyledButton("清空"))
            {
                for (int i = 0; i < m_CitySceneDataList.Count; i++)
                {
                    var info = m_CitySceneDataList[i];
                    info.toggle = false;
                }
            }

            if (StyledButton("转数据"))
            {
                var before = System.Environment.TickCount;
                GenerateMap();

                UnityEngine.Debug.Log(string.Format("转表用时：:{0}", System.Environment.TickCount - before));
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public static bool StyledButton(string label)
    {
        EditorGUILayout.Space();
        GUILayoutUtility.GetRect(1, 20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool clickResult = GUILayout.Button(label, "miniButton");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        return clickResult;
    }

    public class CitySceneTableData
    {
        public CitySceneTable tableData;
        public bool toggle = true;
    }

    private static List<CitySceneTableData> BuildCitySceneList()
    {
        List<CitySceneTableData> res = new List<CitySceneTableData>();
        var tableItem = TableManager.instance.GetTable<ProtoTable.CitySceneTable>();
        foreach (var item in tableItem)
        {
            var table = item.Value as CitySceneTable;
            res.Add(new CitySceneTableData() { tableData = table });
        }
        return res;
    }

    private void GenerateMap()
    {
        List<DTownDoor> doors = new List<DTownDoor>();
        Dictionary<string, DSceneData> sceneDataDic = new Dictionary<string, DSceneData>();
        for (int h = 0; h < m_CitySceneDataList.Count; ++h)
        {
            if (m_CitySceneDataList[h] == null || !m_CitySceneDataList[h].toggle)
            {
                continue;
            }
            var table = m_CitySceneDataList[h].tableData;
            if (table != null)
            {
                DSceneData data;
                if (sceneDataDic.ContainsKey(table.ResPath))
                {
                    data = sceneDataDic[table.ResPath];
                }
                else
                {
                    data = AssetLoader.instance.LoadRes(table.ResPath, typeof(DSceneData)).obj as DSceneData;
                    
                    sceneDataDic.Add(table.ResPath, data);
                }

                if (data != null)
                {
                    //string byteStr = data._blocklayer[0].ToString();
                    //for (int i = 1; i < data._blocklayer.Length; i++)
                    //{
                    //    byteStr += "," + data._blocklayer[i].ToString();
                    //}
                    //Logger.LogErrorFormat("{0} block += Use Time:{1}", data._blocklayer.Length, Utility.GetTimeStamp() - time2);
                    StringBuilder sb = new StringBuilder(data._blocklayer.Length * 2);
                    sb.Append(data._blocklayer[0].ToString());
                    for (int i = 1; i < data._blocklayer.Length; i++)
                    {
                        sb.Append(",");
                        sb.Append(data._blocklayer[i].ToString());
                    }
                    var pos = data._birthposition.position;
                    var posX = data._LogicXSize;
                    var posZ = data._LogicZSize;

                    string finalStr = string.Format("<Floor Row=\"{0}\" Col=\"{1}\">{2}</Floor><Birth X={3} Y={4}></Birth><GirdSize W={5} H={6}></GirdSize>"
                        , data.LogicZ, data.LogicX, sb.ToString(), (int)((pos.x - posX.x) * 10000), (int)((pos.z - posZ.x) * 10000), data._GridSize.x, data._GridSize.y);

                    sb.Clear();

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

                        var targetScene = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(door.TargetSceneID);
                        if (targetScene == null)
                        {
                            Logger.LogErrorFormat("[SceneData->{0}][door->{1}] ==> TargetSceneID:{2} is not exist!!", table.ResPath, door.DoorID, door.TargetSceneID);
                            continue;
                        }

                        bool bFound = false;
                        DSceneData sceneData;
                        if (sceneDataDic.ContainsKey(targetScene.ResPath))
                        {
                            sceneData = sceneDataDic[targetScene.ResPath];
                        }
                        else
                        {
                            sceneData = AssetLoader.instance.LoadRes(targetScene.ResPath, typeof(DSceneData)).obj as DSceneData;
                            sceneDataDic.Add(targetScene.ResPath, sceneData);
                        }
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
                                Logger.LogErrorFormat("[SceneData->{0}][door->{1}] ==> BirthPos can not in trigger range!!,传送点x = {2}, 矩形x = {3}, 传送点z = {4}, 矩形y ={5}", table.ResPath, door.DoorID, door.BirthPos.x, door.rect.x, door.BirthPos.z, door.rect.y);
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

                    finalStr = string.Format("<Map>{0}{1}{2}{3}{4}</Map>", finalStr, doorStr, npcStr, resourceStr, transferStr);

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
}
