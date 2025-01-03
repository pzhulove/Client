using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.Text;

// 超过64张图片称号：Chengzhu_Tiankong, Shengshoubaihu, Wanshiwangzhe.
public class CreateChenghaoWindow : EditorWindow
{
    static string exeFilePath = "";
    static string chenghaoRootPath = "";

    //设置参数-多参数使用空格键进行分隔
    static string param0 = " --format unity" + " --data ./out.plist --sheet ";
    static string Algorithm = " --algorithm Basic --basic-order Ascending --basic-sort-by Name --trim-mode None --disable-auto-alias ";
    static string padding = "--border-padding 0 --shape-padding 0 ";

    private string m_SrcDir = "";
    private string m_DestDir = "";

    //[MenuItem("[TM工具集]/ArtTools/Chenghao/拷贝称号贴图到美术目录")]
    static void CopyTexToArtDir()
    {
        string artChenghaoRoot = Application.dataPath + "/../../UIPacker/称号";
        if (!Directory.Exists(artChenghaoRoot))
        {
            Directory.CreateDirectory(artChenghaoRoot);
        }

        chenghaoRootPath = Application.dataPath + "/Resources/UI/Animation/Anim_chengHao_Formal/";
        string[] chenghaoDirs = Directory.GetDirectories(chenghaoRootPath);
        for (int i = 0; i < chenghaoDirs.Length; ++i)
        {
            string chenghaoDir = chenghaoDirs[i];
            string dirName = chenghaoDir.Substring(chenghaoDir.LastIndexOf("/") + 1);

            string artChanghaoDir = artChenghaoRoot + "/" + dirName;
            if (!Directory.Exists(artChanghaoDir))
            {
                Directory.CreateDirectory(artChanghaoDir);
            }

            string[] fileNames = Directory.GetFiles(chenghaoDir, "*.png");
            if(fileNames.Length == 0)
            {
                fileNames = Directory.GetFiles(chenghaoDir, "*.tga");
            }

            if (fileNames.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "称号贴图目录下没有图片资源（png, tga）", "确认");
                return;
            }

            foreach(string fileName in fileNames)
            {
                // 与目录同名的是合图，不拷贝
                if (fileName.Contains(dirName + ".png"))
                    continue;

                int parentFolderIndex = fileName.LastIndexOf('\\');
                if(parentFolderIndex < 0)
                    parentFolderIndex = fileName.LastIndexOf('/');

                string name = fileName.Substring(parentFolderIndex + 1);

                File.Copy(fileName, artChanghaoDir + "/" + name, true);

                File.Delete(fileName);
                File.Delete(fileName + ".meta");
            }
        }

        UnityEditor.EditorUtility.DisplayDialog("成功", "拷贝称号贴图到美术目录完成", "确认");
    }

    //[MenuItem("[TM工具集]/ArtTools/Chenghao/合并称号贴图")]
    static void CombineTex()
    {
        exeFilePath = Application.dataPath + "/../../ExternalTool/TexturePacker/bin/TexturePacker.exe";
        chenghaoRootPath = Application.dataPath + "/Resources/UI/Animation/Anim_chengHao_Formal/";

        StringBuilder strBuilder = new StringBuilder();
        string currentChenghao = "";

        string[] searchFolder = new string[1];
        Sprite chenghaoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/Image/ChenghaoDefault/ChenghaoSprite.png");
        Material chenghaoMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/UI/Material/ChenghaoMat.mat");
        if (chenghaoSprite == null || chenghaoMat == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("找不到称号默认Sprite或材质", "Assets/Resources/UI/Image/ChenghaoDefault/ChenghaoSprite.png, Assets/Resources/UI/Material/ChenghaoMat.mat", "确认");
            return;
        }

        try
        {
            string[] chenghaoDirs = Directory.GetDirectories(chenghaoRootPath);
            float fProgress = 0;
            bool bError = false;
            for (int i = 0; i < chenghaoDirs.Length; ++i)
            {
                string chenghaoDir = chenghaoDirs[i];
                currentChenghao = chenghaoDir;

                string outFileName = chenghaoDir.Substring(chenghaoDir.LastIndexOf("/") + 1);
                strBuilder.Clear();
                strBuilder.AppendFormat("{0}/{1}.png", chenghaoDir, outFileName);

                // 显示进度条
                string title = "正在处理( " + i + " of " + chenghaoDirs.Length + " )";
                fProgress += 1.0F;
                EditorUtility.DisplayProgressBar(title, outFileName, fProgress / chenghaoDirs.Length);

                int atlasTexture = 0;
                string outpngpath = strBuilder.ToString();
                if (File.Exists(outpngpath))
                {
                    atlasTexture = 1;
                    File.Delete(outpngpath);
                }

                // 统计称号图片数量
                searchFolder[0] = chenghaoDir.Substring(chenghaoDir.LastIndexOf("Assets/Resources"));
                var textures = AssetDatabase.FindAssets("t:texture", searchFolder);
                int textureNum = textures.Length - atlasTexture;
                if (textureNum > 64)
                {
                    UnityEditor.EditorUtility.DisplayDialog("称号贴图数量请限制在64个以内，然后重新合并", searchFolder[0], "确认");
                    //bError = true;
                    //break;
                }

                Process process = new Process();
                process.StartInfo.FileName = exeFilePath;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments = param0 + outpngpath + Algorithm + padding + chenghaoDir;
                process.EnableRaisingEvents = true;
                process.Start();
                process.WaitForExit();

                // UnityEngine.Debug.Log("exe已经运行关闭了");

                // int ExitCode = process.ExitCode;

                //print(ExitCode);

                // 修改Prefab Sprite和GeAnimaFrameBillboard脚本
                var prefabs = AssetDatabase.FindAssets("t:prefab", searchFolder);
                if (prefabs.Length == 0)
                {
                    UnityEditor.EditorUtility.DisplayDialog("目录下没有称号Prefab", searchFolder[0], "确认");
                }
                else if (prefabs.Length > 1)
                {
                    UnityEditor.EditorUtility.DisplayDialog("目录下有多个Prefab", searchFolder[0], "确认");
                }
                else
                {
                    var path = AssetDatabase.GUIDToAssetPath(prefabs[0]);
                    GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    SpriteRenderer spriteRenderer = root.GetComponent<SpriteRenderer>();
                    GeAnimFrameBillboard animScript = root.GetComponent<GeAnimFrameBillboard>();

                    if (spriteRenderer == null || animScript == null)
                    {
                        UnityEditor.EditorUtility.DisplayDialog("需要有SpriteRenderer和GeAnimFrameBillboard组件", chenghaoDir, "确认");
                        continue;
                    }

                    spriteRenderer.sprite = chenghaoSprite;
                    spriteRenderer.material = chenghaoMat;

                    animScript.m_FrameCount = textureNum;
                    animScript.m_AtlasName = outpngpath.Substring(outpngpath.LastIndexOf("UI/Animation"));

                    EditorUtility.SetDirty(root);
                    AssetDatabase.SaveAssets();
                }
            }

            if (bError)
            {
                EditorUtility.ClearProgressBar();
            }
            else
            {
                EditorUtility.DisplayProgressBar("正在刷新资源", "请耐心等待...", fProgress / chenghaoDirs.Length);
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();

                UnityEditor.EditorUtility.DisplayDialog("恭喜你", "合并称号贴图完成", "确认");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogErrorFormat("合并称号贴图失败：{0}", currentChenghao);
            UnityEditor.EditorUtility.DisplayDialog("合并失败，检查原因", currentChenghao, "确认");
        }

    }

    [MenuItem("[TM工具集]/ArtTools/Chenghao/自动创建称号")]
    static void CreateChenghao()
    {
        // Get existing open window or if none, make a new one:
        CreateChenghaoWindow window = (CreateChenghaoWindow)EditorWindow.GetWindow(typeof(CreateChenghaoWindow));
        window.titleContent = new GUIContent("自动导入称号");
        window.Show();

        exeFilePath = Application.dataPath + "/../../../texturetool/TexturePacker/bin/TexturePacker.exe";
        chenghaoRootPath = Application.dataPath + "/Resources/UI/Animation/Anim_chengHao_Formal/";
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("称号贴图目录", GUILayout.Width(150));
        EditorGUILayout.TextField(m_SrcDir);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
        {
            m_SrcDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "../../UIPacker/称号", "");
            if(m_SrcDir != "")
            {
                m_DestDir = Application.dataPath + "/Resources/UI/Animation/Anim_chengHao_Formal" + m_SrcDir.Substring(m_SrcDir.LastIndexOf("/"));
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出目录", GUILayout.Width(150));
        EditorGUILayout.TextField(m_DestDir);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
        {
            m_DestDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath, "");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("自动创建称号资源"))
        {
            AutoCreateChenghao();
        }

        EditorGUILayout.EndVertical();
    }

    void AutoCreateChenghao()
    {
        if(m_SrcDir == "" || m_DestDir == "")
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "请先指定称号贴图目录和输出目录", "确认");
            return;
        }

        if(!Directory.Exists(m_DestDir))
        {
            Directory.CreateDirectory(m_DestDir);
        }

        string[] spriteTextures = Directory.GetFiles(m_SrcDir, "*.png");

        int textureNum = spriteTextures.Length;
        if (textureNum == 0)
        {
            spriteTextures = Directory.GetFiles(m_SrcDir, "*.tga");
            textureNum = spriteTextures.Length;
        }

        if (textureNum == 0)
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "称号贴图目录下没有图片资源（png, tga）", "确认");
            return;
        }

        if (textureNum > 64)
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "称号贴图数量请限制在64个以内", "确认");
            return;
        }

        Sprite chenghaoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/Image/ChenghaoDefault/ChenghaoSprite.png");
        Sprite chenghaoSprite128 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/Image/ChenghaoDefault/ChenghaoSprite128.png");
        Material chenghaoMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/UI/Material/ChenghaoMat.mat");
        if (chenghaoSprite == null || chenghaoMat == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("找不到称号默认Sprite或材质", "Assets/Resources/UI/Image/ChenghaoDefault/ChenghaoSprite.png, Assets/Resources/UI/Material/ChenghaoMat.mat", "确认");
            return;
        }

        StringBuilder strBuilder = new StringBuilder();
        string outFileName = m_DestDir.Substring(m_DestDir.LastIndexOf("/") + 1);
        strBuilder.Clear();
        strBuilder.AppendFormat("{0}/{1}.png", m_DestDir, outFileName);

        string outpngpath = strBuilder.ToString();


        exeFilePath = Application.dataPath + "/../../ExternalTool/TexturePacker/bin/TexturePacker.exe";
     //   Logger.LogErrorFormat("exefilePath:{0}", exeFilePath);
        // 启动TexturePacker进行合图
        Process process = new Process();
        process.StartInfo.FileName = exeFilePath;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.Arguments = param0 + outpngpath + Algorithm + padding + m_SrcDir;

       // Logger.LogErrorFormat("arg:{0}", process.StartInfo.Arguments);

        process.EnableRaisingEvents = true;
        process.Start();
        process.WaitForExit();

        // 修改Prefab Sprite和GeAnimaFrameBillboard脚本
        string assetDestDir = m_DestDir.Substring(m_DestDir.LastIndexOf("Assets/"));
        string[] searchFolder = new string[] { assetDestDir };
        var prefabs = AssetDatabase.FindAssets("t:prefab", searchFolder);
        if (prefabs.Length > 1)
        {
            UnityEditor.EditorUtility.DisplayDialog("目录下有多个Prefab", searchFolder[0], "确认");
            return;
        }


        bool bSprite64 = true;
        File.Copy(spriteTextures[0], Application.dataPath + "/ChenghaoTemp.png");
        AssetDatabase.ImportAsset("Assets/ChenghaoTemp.png");
        Texture2D spritetex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChenghaoTemp.png");
        if (spritetex)
        {
            bSprite64 = (spritetex.height == 64);
            AssetDatabase.DeleteAsset("Assets/ChenghaoTemp.png");
        }
        else
        {
            File.Delete(Application.dataPath + "/ChenghaoTemp.png");
        }

        bool bNewCreated = false;
        GameObject root = null;
        if (prefabs.Length == 0)
        {
            root = new GameObject("Title_" + outFileName);
            root.transform.localScale = new Vector3(55.0F, 55.0F, 55.0F); 
            bNewCreated = true;
        }
        else
        {
            var path = AssetDatabase.GUIDToAssetPath(prefabs[0]);
            root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        SpriteRenderer spriteRenderer = root.GetComponent<SpriteRenderer>();
        GeAnimFrameBillboard animScript = root.GetComponent<GeAnimFrameBillboard>();

        if (spriteRenderer == null)
        {
            spriteRenderer =  root.AddComponent<SpriteRenderer>();
        }

        if (animScript == null)
        {
            animScript = root.AddComponent<GeAnimFrameBillboard>();
        }

        spriteRenderer.sprite = bSprite64 ? chenghaoSprite : chenghaoSprite128;
        spriteRenderer.material = chenghaoMat;

        animScript.m_FrameCount = textureNum;
        animScript.m_AtlasName = outpngpath.Substring(outpngpath.LastIndexOf("UI/Animation"));

        if (bNewCreated)
        {
            PrefabUtility.CreatePrefab(assetDestDir + "/" + root.name + ".prefab", root);
            AssetDatabase.SaveAssets();

            GameObject.DestroyImmediate(root);
        }
        else
        {
            EditorUtility.SetDirty(root);
            AssetDatabase.SaveAssets();
        }

        AssetDatabase.Refresh();
        UnityEditor.EditorUtility.DisplayDialog("", "自动创建称号成功", "确认");
    }
}