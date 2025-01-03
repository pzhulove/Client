/// <summary>
/// 自动生成序列帧动画
/// </summary>
using UnityEngine;
using System.IO;
using UnityEditor;
using Spine.Unity;
using Spine.Unity.Editor;

public class BuildAnimation : EditorWindow 
{

    public static void CreateSpineUIObject()
    {
        Object[] selection = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);
        if (selection.Length <= 0)
        {
            Logger.LogErrorFormat("请选择一个spine动画目录");
            return;
        }

        var path = AssetDatabase.GetAssetPath(selection[0]);
        Logger.LogErrorFormat("path:{0} name:{1}", path, selection[0].name);
        {
            var parentPath = path;

            DirectoryInfo dirInfo = new DirectoryInfo(parentPath);
            if (!dirInfo.Exists)
                return;

            string atlasFile = "";
            string jsonFile = "";
            string atlasFileName = "";
            string jsonFileName = "";


            var files = dirInfo.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension == ".txt")
                {
                    atlasFileName = file.Name.Replace(file.Extension, "");
                    atlasFile = parentPath + "/" + file.Name;
                }

                else if (file.Extension == ".bytes")
                {
                    jsonFileName = file.Name.Replace(file.Extension, "");
                    jsonFileName = jsonFileName.Replace(".skel", "");
                    jsonFile = parentPath + "/" + file.Name;
                }
                else if (file.Extension == ".json")
                {
                    jsonFileName = file.Name.Replace(file.Extension, "");
                    jsonFile = parentPath + "/" + file.Name;
                }
            }

            if (atlasFile.Length <= 0 || jsonFile.Length <= 0)
            {
                Logger.LogErrorFormat("[SPINE]altas或者json文件找不到：{0}", parentPath);
            }


            {


                var skeletonDataPath = parentPath + "/" + jsonFileName + "_SkeletonData.asset";

                //Logger.LogErrorFormat("skeletonDataPath:{0}", skeletonDataPath);

                var data = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skeletonDataPath);
                string defaultAniName = null;

                if (data != null)
                {

                }
                else
                    Logger.LogErrorFormat("加载{0}失败", skeletonDataPath);

                var graphic = SkeletonGraphicInspector.InstantiateSkeletonGraphic(data);
                if (graphic != null)
                {
                    var uigo = graphic.gameObject;

                    var animations = graphic.skeletonDataAsset.GetSkeletonData(false).Animations;

                    for (int i = 0; i < animations.Count; ++i)
                    {
                        var animation = animations.Items[i];
                        if (animation != null)
                        {
                            defaultAniName = animation.Name;
                        }
                    }

                    if (defaultAniName != null)
                    {
                        graphic.startingLoop = true;
                        graphic.startingAnimation = defaultAniName;
                    }

                    PrefabUtility.SaveAsPrefabAsset(uigo, parentPath + "/" + selection[0].name + "_spine_ui.prefab");
                    GameObject.DestroyImmediate(uigo);
                }
            }
        }

    }

    public static void CreateSpineObject()
    {
        Object[] selection = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);
        if (selection.Length <= 0)
        {
            Logger.LogErrorFormat("请选择一个spine动画目录");
            return;
        }

        var path = AssetDatabase.GetAssetPath(selection[0]);
        Logger.LogErrorFormat("path:{0} name:{1}", path, selection[0].name);



        //var ret = AssetDatabase.FindAssets("*.atlas", new string[1] { path });
        //if (ret.Length == 1)
        {
            var parentPath = path;

            DirectoryInfo dirInfo = new DirectoryInfo(parentPath);
            if (!dirInfo.Exists)
                return;

            string atlasFile = "";
            string jsonFile = "";
            string atlasFileName = "";
            string jsonFileName = "";


            var files = dirInfo.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension == ".txt")
                {
                    atlasFileName = file.Name.Replace(file.Extension, "");
                    atlasFile = parentPath + "/" + file.Name;
                }

                else if (file.Extension == ".bytes")
                {
                    jsonFileName = file.Name.Replace(file.Extension, "");
                    jsonFileName = jsonFileName.Replace(".skel", "");
                    jsonFile = parentPath + "/" + file.Name;
                }
                else if (file.Extension == ".json")
                {
                    jsonFileName = file.Name.Replace(file.Extension, "");
                    jsonFile = parentPath + "/" + file.Name;
                }
            }

            if (atlasFile.Length <= 0 || jsonFile.Length <= 0)
            {
                Logger.LogErrorFormat("[SPINE]altas或者json文件找不到：{0}", parentPath);
            }


            //var filePath = path + "/" + selection[0].name + ".atlas";

            // Logger.LogErrorFormat("atlasFile:{0}", atlasFile);

            // File.Move(atlasFile, atlasFile + ".txt");

            // atlasFile += ".txt";

            {
                AssetDatabase.ImportAsset(atlasFile);
                AssetDatabase.ImportAsset(jsonFile);

                GameObject go = new GameObject();
                go.name = selection[0].name;
                var skeletonAnimation = go.AddComponent<SkeletonAnimation>();

                var skeletonDataPath = parentPath + "/" + jsonFileName + "_SkeletonData.asset";

                //Logger.LogErrorFormat("skeletonDataPath:{0}", skeletonDataPath);

                var data = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skeletonDataPath);

                if (data != null)
                {
                    string defaultAniName = null;
                    skeletonAnimation.skeletonDataAsset = data;

                    var animations = skeletonAnimation.skeletonDataAsset.GetSkeletonData(false).Animations;

                    for (int i = 0; i < animations.Count; ++i)
                    {
                        var animation = animations.Items[i];
                        if (animation != null)
                        {
                            defaultAniName = animation.Name;
                        }
                    }

                    if (defaultAniName != null)
                    {
                        skeletonAnimation.loop = true;

                        skeletonAnimation.Initialize(false);
                        skeletonAnimation.AnimationName = defaultAniName;
                    }

                }
                else
                    Logger.LogErrorFormat("加载{0}失败", skeletonDataPath);

                PrefabUtility.SaveAsPrefabAsset(go, parentPath + "/" + go.name + "_spine.prefab");
            }
        }
    }


}
