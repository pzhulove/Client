using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEditor;

public class DDungeonDataInfoChangeEditor : EditorWindow
{
    private float mLookAtHight = 1;
    private float mLookAtDis = 10;
    private float mLookAtAngle = 20;
    private float mNearClip = -1;
    private float mFarClip = 100;

    private float mSize = 3.3f;

    private Vector2 mGridSize = new Vector2(0.25f, 0.25f);

    private List<DSceneData> mSceneDatas = new List<DSceneData>();

    private string filePath = "";

    private string[] filePaths;

    [MenuItem("[关卡编辑器]/区域/场景数据修改工具")]
    public static void Init()
    {
        var win = GetWindow<DDungeonDataInfoChangeEditor>();
        win.Show();
        win.titleContent.text = "场景数据修改工具";
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("GroupBox");

        EditorGUILayout.LabelField("查找场景文件夹路径，用|分隔开,路径格式如(Data/SceneData/1_1luolan)");
        var path = EditorGUILayout.TextField("文件夹路径", filePath);
        if(!path.Equals(filePath))
        {
            filePath = path;
            filePaths = filePath.Split('|');
            for(int i = 0; i < filePaths.Length; i++)
            {
                filePaths[i] = "Assets/Resources/" + filePaths[i];
            }
        }
        
        //"Assets/Resources/UIPacked", "Assets/Resources/Base"
        if (GUILayout.Button("获取所有场景数据"))
        {
            _GetMapDatas(mSceneDatas, filePaths);
        }

        mGridSize = EditorGUILayout.Vector2Field("FridSize", mGridSize);

        var nearClip = Global.Settings.battleCameraNearClip;
        var farClip = Global.Settings.battleCameraFarClip;

        mLookAtHight = EditorGUILayout.FloatField("LooadAt高度", mLookAtHight);
        mLookAtDis = EditorGUILayout.FloatField("LooadAt距离", mLookAtDis);
        mLookAtAngle = EditorGUILayout.FloatField("LooadAt角度", mLookAtAngle);
        mNearClip = EditorGUILayout.FloatField("LooadAt近裁剪", nearClip);
        mFarClip = EditorGUILayout.FloatField("LooadAt远裁剪", farClip);
        mSize = EditorGUILayout.FloatField("视域大小", mSize);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(string.Format("共有{0}个SceneData数据", mSceneDatas.Count));

        if (GUILayout.Button("一键替换"))
        {
            for(int i = 0; i < mSceneDatas.Count; i++)
            {
                var sceneData = mSceneDatas[i];
                if(sceneData == null)
                {
                    continue;
                }

                sceneData._GridSize = mGridSize;

                sceneData._CameraLookHeight = mLookAtHight;
                sceneData._CameraDistance = mLookAtDis;
                sceneData._CameraAngle = mLookAtAngle;
                sceneData._CameraNearClip = mNearClip;
                sceneData._CameraFarClip = mFarClip;
                sceneData._CameraSize = mSize;
            }
            AssetDatabase.SaveAssets();
        }



        EditorGUILayout.EndVertical();
    }


    private void _GetMapDatas(List<DSceneData> list, string[] filePath) 
    {
        list.Clear();
        if(filePath == null || filePath.Length <= 0)
        {
            return;
        }
        IList<string> mapDatas = AssetDatabase.FindAssets("t:DSceneData", filePath);
        for (int i = 0; i < mapDatas.Count; i++)
        {
            var mapDataPath = AssetDatabase.GUIDToAssetPath(mapDatas[i]);
            if (mapDataPath == null)
            {
                continue;
            }
            DSceneData mapData = (DSceneData)AssetDatabase.LoadAssetAtPath(mapDataPath, typeof(DSceneData));

            if (mapData != null)
            {
                list.Add(mapData);
            }
        }
    }
}
