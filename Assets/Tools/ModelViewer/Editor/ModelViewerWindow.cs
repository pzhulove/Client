using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.IMGUI.Controls;
using LitJson;
using UnityEditor.SceneManagement;

public class ModelViewerWindow : EditorWindow
{
    private List<AssetFolder> assetFolderList = new List<AssetFolder>();
    Texture2D mRefreshTexture;
    ModelTreeView modelTreeView;
    TreeViewState modelTreeViewState;
    private AssetPrefab selectModelPrefab = null;
    private GameObject selectModelGameobject = null;
    private GameObject lastPreviewObject;//上一个预览的模型  
    private GameObject lastModelObj;
    private List<string> listAnimaNames = new List<string>();
    private List<AnimationClip> listAnimas = new List<AnimationClip>();
    private string configPath = "";
    private string modelViewPath = "";
    private int selectAnima;//选中的Anima
    private float animaPlaySpeed=1;//播放速度
    private bool isPlaying;//是否正在播放动画
    private float animaProgress;//动画播放进度
    private float animaPlayTimer = 0;
    private float animatPlayFrame;
    private bool hasGeAnimDescProxy = false;
    private double previousTime;
    private bool autoRotation = false;
    private bool isLeftMouseClick = false;
    private float rotationSpeed = 0;
    private string defaultScene = "Assets/Resources/Scene/Heduncheng_Xin/Heduncheng_Dungeons/scenes/P_Room_xhdc.prefab";
    private ModelViewerConfig config;
    private GameObject scene;
    private GameObject instanceScene;//实例化场景
    private Animation animation;
    private GameObject tempObj;
    SearchField m_SearchField;
    Rect toolbarRect
    {
        get { return new Rect(5f, 25f, position.width / 3 * 2, 20f); }
    }
    [MenuItem("[TM工具集]/模型预览")]
    static void EffectVarientWindow()
    {
        ModelViewerWindow window = EditorWindow.GetWindow<ModelViewerWindow>("模型预览");
        window.Show();
        window.minSize = new Vector2(1280, 720);
    }
    private void OnEnable()
    {
        hasGeAnimDescProxy = true;
        mRefreshTexture = EditorGUIUtility.FindTexture("Refresh");
        if (modelTreeViewState == null)
            modelTreeViewState = new TreeViewState();
        modelTreeView = new ModelTreeView(modelTreeViewState, this);
        m_SearchField = new SearchField();
        m_SearchField.downOrUpArrowKeyPressed += modelTreeView.SetFocusAndEnsureSelectedItem;
        configPath = Application.dataPath + "/Tools/ModelViewer/configPath.json";
        modelViewPath = "Assets/Tools/ModelViewer/ModelViewer.prefab";
        if (File.Exists(configPath))
        {
            using (FileStream fs = new FileStream(configPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                int fsLen = (int)fs.Length;
                byte[] heByte = new byte[fsLen];
                int r = fs.Read(heByte, 0, heByte.Length);
                string myStr = System.Text.Encoding.UTF8.GetString(heByte);
                config = LitJson.JsonMapper.ToObject<ModelViewerConfig>(myStr);
                fs.Close();
            };
        }
        else
        {
            config = new ModelViewerConfig();
            config.paths = new List<string>();
            config.scenes = defaultScene;
        }
        scene = scene == null ? AssetDatabase.LoadAssetAtPath<GameObject>(config.scenes) : scene;
        tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(modelViewPath);
        RefreshTreeViewList();
    }
    private void LoadScene()
    {
        scene = scene == null ? AssetDatabase.LoadAssetAtPath<GameObject>(config.scenes) : scene;
        if (scene != null)
        {
            if (instanceScene != null)
            {
                DestroyImmediate(instanceScene);
            }
            instanceScene = Instantiate<GameObject>(scene);
        }
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Model", GUILayout.Width(75), GUILayout.Height(20)))
        {
            string folderName = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath + "/Resources", "");
            folderName = GetStandardPath(folderName);
            if (!string.IsNullOrEmpty(folderName) && !config.paths.Contains(folderName) && folderName.Contains("Assets/"))
            {
                config.paths.Add(folderName);
                RefreshTreeViewList();
            }
        }
        if (GUILayout.Button(mRefreshTexture, GUILayout.Width(25), GUILayout.Height(20)))
        {
            RefreshTreeViewList();
        }
        GUILayout.EndHorizontal();
        SearchBar(toolbarRect);
        if (config.paths.Count > 0)
            modelTreeView.OnGUI(new Rect(0, 50, position.width / 3 * 2 + 10, position.height - 50));
        GUILayout.BeginArea(new Rect(position.width / 3 * 2 + 30, 50, position.width / 3 - 30, position.height / 3), EditorStyles.helpBox);
        scene = EditorGUILayout.ObjectField("场景", scene, typeof(GameObject), false) as GameObject;
        config.scenes = AssetDatabase.GetAssetPath(scene).Replace("\\", "/");
        if (GUILayout.Button("加载场景", EditorStyles.toolbarButton))
        {
            LoadScene();
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(position.width / 3 * 2 + 30, position.height / 3 + 50, position.width / 3 - 30, position.height / 3 * 2 - 50), EditorStyles.helpBox);
        if (listAnimas.Count <= 0)
            goto endArea;
        GUILayout.BeginVertical();
        int newClip = EditorGUILayout.Popup("动作:", selectAnima, listAnimaNames.ToArray(), EditorStyles.toolbarDropDown);
        if (newClip != selectAnima)
        {
            isPlaying = false;
            animaPlayTimer = 0;
            animaProgress = 0;
            animatPlayFrame = 0;
            selectAnima = newClip;
            AddClip(selectAnima);
        }
        animaPlaySpeed = EditorGUILayout.Slider("播放速度:", animaPlaySpeed, 0, 3);
        float currentRotationSpeed = EditorGUILayout.Slider("旋转速度:", rotationSpeed, -3, 3);
        if (currentRotationSpeed != rotationSpeed)
        {
            autoRotation = currentRotationSpeed == 0 ? false : true;
            rotationSpeed = currentRotationSpeed;
        }
        Rect tempRect = GUILayoutUtility.GetRect(1, 20);
        Rect rect = new Rect(5, tempRect.y, position.width / 3 - 95, 20);
        EditorGUI.ProgressBar(rect, animaProgress, "动画");
        Rect tempRect1 = GUILayoutUtility.GetRect(1, 15);
        Rect rect1 = new Rect(5, tempRect1.y - 6, position.width / 3 - 40, 15);
        if (isPlaying)
        {
            animatPlayFrame = EditorGUI.Slider(rect1, animaPlayTimer, 0, listAnimas[selectAnima].length);
        }
        else if (listAnimas.Count > selectAnima && lastModelObj != null)
        {
            animatPlayFrame = EditorGUI.Slider(rect1, animatPlayFrame, 0, listAnimas[selectAnima].length);
            animaProgress = animatPlayFrame / listAnimas[selectAnima].length;
            listAnimas[selectAnima].SampleAnimation(lastModelObj, animatPlayFrame);
        }
        EditorGUILayout.EndFadeGroup();
        bool currentPlayState = EditorGUILayout.Toggle("播放动画:", isPlaying, EditorStyles.toggle);
        if (isPlaying == false && currentPlayState == true)
        {
            animaPlayTimer = 0;
        }
        if (isPlaying == true && currentPlayState == false)
        {
            animatPlayFrame = animaPlayTimer;
        }
        isPlaying = currentPlayState;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("朝左", EditorStyles.toolbarButton))
        {
            autoRotation = false;
            lastModelObj.transform.localScale = new Vector3(-1, 1, 1);
            lastModelObj.transform.localEulerAngles = Vector3.zero;
        }
        GUILayout.Space(100);
        if (GUILayout.Button("朝右", EditorStyles.toolbarButton))
        {
            autoRotation = false;
            lastModelObj.transform.localScale = new Vector3(1, 1, 1);
            lastModelObj.transform.localEulerAngles = Vector3.zero;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        endArea:
        GUILayout.EndArea();
    }
    private void AddClip(int clip)
    {
        if (animation != null && listAnimaNames.Count > 0)
        {
            animation.AddClip(listAnimas[clip], listAnimaNames[clip]);
        }
    }
    private void Update()
    {
        if (autoRotation && lastModelObj != null)
        {
            lastModelObj.transform.Rotate(new Vector3(0, 30 * (float)(EditorApplication.timeSinceStartup - previousTime) * rotationSpeed, 0), Space.Self);
        }
        if (listAnimas.Count > selectAnima && lastModelObj != null)
        {
            if (isPlaying)
            {
                animaPlayTimer += (float)(EditorApplication.timeSinceStartup - previousTime) * animaPlaySpeed;
                if (animaPlayTimer <= listAnimas[selectAnima].length)
                {
                    listAnimas[selectAnima].SampleAnimation(lastModelObj, animaPlayTimer);
                }
                else
                {
                    if (listAnimas[selectAnima].wrapMode == WrapMode.Loop)
                    {
                        animaPlayTimer = 0;
                    }
                    else
                    {
                        animaPlayTimer = 0;
                        //listAnimas[selectAnima].SampleAnimation(lastModelObj, animaPlayTimer);
                        isPlaying = false;
                    }
                }
                animaProgress = animaPlayTimer / listAnimas[selectAnima].length;
                Repaint();
            }
        }
        previousTime = EditorApplication.timeSinceStartup;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    void SearchBar(Rect rect)
    {
        modelTreeView.searchString = m_SearchField.OnGUI(rect, modelTreeView.searchString);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public void RemoveFolder(string path)
    {
        for (int i = 0; i < config.paths.Count; i++)
        {
            if (config.paths[i].Equals(path))
            {
                config.paths.RemoveAt(i);
                break;
            }
        }
        RefreshTreeViewList();
    }
    private void RefreshTreeViewList()
    {
        assetFolderList.Clear();
        for (int i = 0; i < config.paths.Count; i++)
        {
            AssetFolder folder = new AssetFolder();
            CollectAssetRecursive(config.paths[i], folder, 0, true);
            assetFolderList.Add(folder);
        }
        if (assetFolderList.Count > 0)
            modelTreeView.SetTreeViewData(assetFolderList, SetModelSelect);
    }
    
    private void SetModelSelect(bool isfolder, string path, string guid)
    {
        //if (!SaveScenes())
        //{
        //    return;
        //}
        if (isfolder)
        {
            selectModelPrefab = null;
        }
        else
        {
            selectModelPrefab = new AssetPrefab();
            selectModelPrefab.path = path;
            selectModelPrefab.guid = guid;
            Selection.activeGameObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            selectModelGameobject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject)) as GameObject;
        }
        if (lastPreviewObject != null)
        {
            DestroyImmediate(lastPreviewObject);
        }
        lastPreviewObject = Instantiate<GameObject>(tempObj);
        lastPreviewObject.transform.position = Vector3.zero;
        lastModelObj = Instantiate<GameObject>(selectModelGameobject);
        lastModelObj.transform.SetParent(lastPreviewObject.transform);
        lastModelObj.transform.localPosition = Vector3.zero;
        lastModelObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        selectAnima = 0;
        autoRotation = false;
        animaPlayTimer = 0;
        animaProgress = 0;
        animatPlayFrame = 0;
        isPlaying = false;
        animation = lastModelObj.GetComponent<Animation>();
        CollectListAnimas();
        if (instanceScene == null)
        {
            LoadScene();
        }
    }
    private void CollectListAnimas()
    {
        listAnimas.Clear();
        listAnimaNames.Clear();
        if (lastModelObj == null)
            return;
        GeAnimDescProxy proxy = lastModelObj.GetComponent<GeAnimDescProxy>();
        if (null != proxy)
        {
            GeAnimDesc[] animClips = proxy.animDescArray;
            if (null != animClips)
            {
                for (int i = 0, icnt = animClips.Length; i < icnt; ++i)
                {
                    if (string.IsNullOrEmpty(animClips[i].animClipPath))
                        continue;
                    animClips[i].animClipData = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + animClips[i].animClipPath, typeof(AnimationClip)) as AnimationClip;
                    if (animClips[i].animClipData == null || (!animClips[i].animClipPath.ToLower().Contains("idle") && !animClips[i].animClipPath.ToLower().Contains("walk") && animClips[i].animClipData.length <= 1f))
                        continue;
                    listAnimas.Add(animClips[i].animClipData);
                    listAnimaNames.Add(animClips[i].animClipName);
                }
            }
        }
        AddClip(0);
    }
    private bool SaveScenes()
    {
        if (EditorSceneManager.GetActiveScene().name == "")
        {
            return true;
        }
        int option = EditorUtility.DisplayDialogComplex("Save Scene",
                "是否保存场景？",
                "保存",
                "不保存",
                "取消");
        switch (option)
        {
            case 0:
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                break;
            case 1:
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                break;
            case 2:
                return false;
            default:
                return false;
        }
        return true;
    }
    private string GetStandardPath(string path)
    {
        return path.Replace(Application.dataPath, "Assets").Replace("\\", "/");
    }
    private void CollectAssetRecursive(string path, AssetFolder folder, int depth, bool isRoot = false)
    {
        if (isRoot)
        {
            folder.name = path;
            folder.path = path;
        }
        else
        {
            folder.name = path.Substring(path.LastIndexOf('/') + 1);
            folder.path = path;
        }
        folder.depth = depth;
        string[] topDirectories = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < topDirectories.Length; i++)
        {
            string tempPath = topDirectories[i].Replace("\\", "/");
            AssetFolder assetFolder = new AssetFolder();
            assetFolder.parentFolder = folder;
            CollectAssetRecursive(tempPath, assetFolder, depth + 1);
            folder.childFolder.Add(assetFolder);
        }
        string[] topFiles = Directory.GetFiles(path, "*.prefab", SearchOption.TopDirectoryOnly);
        List<string> topFilesList = new List<string>();
        for (int i = 0; i < topFiles.Length; i++)
        {
            string tempPath = topFiles[i].Replace("\\", "/");
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(tempPath);
            if (hasGeAnimDescProxy && (obj == null || obj.GetComponent<GeAnimDescProxy>() == null || obj.GetComponent<Animation>() == null))
            {
                continue;
            }
            AssetPrefab assetPrefab = new AssetPrefab();
            assetPrefab.parentFolder = folder;
            assetPrefab.depth = depth + 1;
            assetPrefab.path = tempPath;
            assetPrefab.guid = AssetDatabase.AssetPathToGUID(tempPath);
            assetPrefab.name = tempPath.Substring(tempPath.LastIndexOf('/') + 1);
            folder.childPrefab.Add(assetPrefab);
            topFilesList.Add(tempPath);
        }
        if (topFilesList.Count > 0)
        {
            SetFolderHasPrefab(folder);
        }
    }

    private void SetFolderHasPrefab(AssetFolder assetFolder)
    {
        assetFolder.hasPrefabRecursive = true;
        AssetFolder parent = assetFolder.parentFolder;
        while (parent != null)
        {
            parent.hasPrefabRecursive = true;
            parent = parent.parentFolder;
        }
    }

    private void OnDestroy()
    {
        if (lastPreviewObject != null)
        {
            DestroyImmediate(lastPreviewObject);
        }
        if (instanceScene != null)
        {
            DestroyImmediate(instanceScene);
        }
        using (FileStream fs = new FileStream(configPath, FileMode.Create, FileAccess.Write))
        {
            string msg = LitJson.JsonMapper.ToJson(config);
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(msg);
            fs.Write(myByte, 0, myByte.Length);
            fs.Close();
        };
        GC.Collect();
        AssetDatabase.Refresh();
    }
}
public class AssetFolder
{
    public string name;
    public string path;
    public int depth;
    public List<AssetFolder> childFolder = new List<AssetFolder>();
    public List<AssetPrefab> childPrefab = new List<AssetPrefab>();
    public AssetFolder parentFolder;
    public bool hasPrefabRecursive = false; // 该目录或其子目录有prefab
}
public class AssetPrefab
{
    public int depth;
    public string guid = "";
    public string path = "";
    public string name = "";
    public AssetFolder parentFolder;
}
public struct ModelViewerConfig
{
    public List<string> paths;
    public string scenes;
}