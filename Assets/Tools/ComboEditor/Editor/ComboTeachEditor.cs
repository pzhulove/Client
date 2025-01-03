using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

public class ComboTeachEditor : EditorWindow
{
    private ComboTeachData comboData;
    public void OnGUI()
    {
        if (comboData != null)
        {
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle.fontSize = 24;
            fontStyle.alignment = TextAnchor.UpperCenter;
            fontStyle.normal.textColor = Color.white;
            fontStyle.hover.textColor = Color.white;
            EditorGUILayout.LabelField("", comboData.dataName, fontStyle, GUILayout.Height(32));


        }
    }

    [MenuItem("[技能编辑器]/CreateCombo", false)]
    public static ComboTeachData CreateAsset()
    {
        ComboTeachData asset = FileTools.CreateAsset<ComboTeachData>("New ComboData");
        return asset;
    }

    
    [MenuItem("[技能编辑器]/clear", false)]
    public static void ClearData()
    {
        Array array = typeof(GameClient.ActorOccupation).GetValues();
        for (int i = 0; i < array.Length; i++)
        {
            int jobID = (int)array.GetValue(i);
            string jobPath = GetJobComboAssetPath(jobID);
            string path = "Assets/Resources/Data/ComboData/" + jobPath;
            AssetDatabase.CreateFolder(path,"Advance");
            AssetDatabase.CreateFolder(path, "Easy");
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("[技能编辑器]/Check", false)]
    public static void Check()
    {
        List<string> prefabs_names = new List<string>();
        //获取指定路径下面的所有资源文件
        if (Directory.Exists("Assets/Resources/Data/ComboData"))
        {
            DirectoryInfo info = new DirectoryInfo("Assets/Resources/Data/ComboData");
            FileInfo[] files =  info.GetFiles("*",SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".asset"))
                {
                    FileInfo file = files[i];
                    var prefab = AssetDatabase.LoadAssetAtPath(files[i].Name, typeof(UnityEngine.Object));
                    var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    if (go != null)
                    {
                        GameObject orign = FindOrign(go.transform);
                        if (orign != null)
                        {
                            if (orign.transform.localPosition.z != 0)
                                Debug.LogWarning(PrefabUtility.FindPrefabRoot(go).name);
                        }
                    }
                }
            }
        }    
    }

    public static int IsAdvance(int id)
    {
        DirectoryInfo info = new DirectoryInfo("Assets/Resources/Data/ComboData");
        FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".asset"))
            {
                FileInfo file = files[i];
                int name = int.Parse(file.Name.Split('.')[0]);
                if (name == id)
                {
                    if (file.Directory.Name == "Advance")
                        return 1;
                    else
                        return 2;
                }
            }
        }
        return 3;
    }

    public static int GetName(int id)
    {
        DirectoryInfo info = new DirectoryInfo("Assets/Resources/Data/ComboData");
        FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".asset"))
            {
                FileInfo file = files[i];
                int name = int.Parse(file.Name.Split('.')[0]);
                if (name == id)
                {
                    string jobName = file.Directory.Parent.Name;
                    Array array = typeof(GameClient.ActorOccupation).GetValues();
                    for (int j = 0; j < array.Length; j++)
                    {
                        int jobID = (int)array.GetValue(j);
                        string jobPath = GetJobComboAssetPath(jobID);
                        if (jobPath == jobName)
                            return j;
                    }
                }
            }
        }
        return 0;
    }



    private static GameObject FindOrign(Transform child)
    {
        if (child == null) return null;
        for (int i = 0; i < child.childCount; i++)
        {
            Transform t = child.GetChild(i);
            if (t.name == "Orign")
                return t.gameObject;
            FindOrign(t);
        }
        return null;
    }

    private static string GetJobComboAssetPath(int jobID)
    {
        var pidValues = typeof(GameClient.ActorOccupation).GetValues();
        string[] name = Enum.GetNames(typeof(GameClient.ActorOccupation));
        int iIndex = System.Array.BinarySearch(pidValues, (GameClient.ActorOccupation)jobID);
        return name[iIndex];
    }
}




