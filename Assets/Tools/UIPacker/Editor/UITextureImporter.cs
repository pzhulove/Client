using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
//using YamlDotNet.RepresentationModel;
using System.Diagnostics;
using ProtoTable;

namespace GameClient
{
    public static class ImageUITool
    {
        private static List<string> mUIList = new List<string>();

        private static void _addUIList(Sprite sprite)
        {
            var txt = sprite.texture;
            mUIList.Add(AssetDatabase.GetAssetPath(txt.GetInstanceID()));
        }

        private static void _getAllUI(GameObject root)
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var child = root.transform.GetChild(i);

                var coms = child.gameObject.GetComponents<Image>();
                for (int j = 0; j < coms.Length; ++j)
                {
                    _addUIList(coms[j].sprite);
                }
            }
        }

        private static Dictionary<string, int> mTextureDict = new Dictionary<string, int>();

        private static void _loadObject(string path)
        {
            path = CFileManager.EraseExtension(path);

            var go = Resources.Load(path);
            if (go != null)
            {
                var iid = go.GetInstanceID();
                var assetpath = AssetDatabase.GetAssetPath(iid);

                if (mTextureDict.ContainsKey(assetpath))
                {
                    mTextureDict[assetpath]++;
                }
                else
                {
                    Logger.LogErrorFormat("new icon ! with path {0}", assetpath);
                    //mTextureDict.Add(assetpath, 1);
                }
            }

            go = null;
            Resources.UnloadUnusedAssets();
        }

        private static void _allUIIMmage()
        {
            mUIList.Clear();

            var prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UI", "Assets/Resources/UIFrame" });
            var textures = AssetDatabase.FindAssets("t:texture", new string[] { "Assets/Resources/UI/Image"});

            mTextureDict.Clear();
            foreach (var txt in textures)
            {
                var path = AssetDatabase.GUIDToAssetPath(txt);
                mTextureDict.Add(path, 0);
            }

            TableManager.instance.UnInit();
            TableManager.instance.Init();

            var itemtable = TableManager.instance.GetTable<ProtoTable.ItemTable>();
            foreach (var item in itemtable)
            {
                var nItem = item.Value as ProtoTable.ItemTable;
                var path = "";

                if (nItem.Type == ItemTable.eType.EXPENDABLE && nItem.SubType == ItemTable.eSubType.EnchantmentsCard)
                { path = "UI/Image/Icon/Icon_enchant/" + nItem.Icon; }
                else { path = "UI/Image/ICON/Icon_equipment/" + nItem.Icon; }

                _loadObject(path);
                _loadObject("UI/Image/ICON/Icon_equipment/" + nItem.ModelPath);
            }

            var skillTable = TableManager.instance.GetTable<SkillTable>();
            foreach (var item in skillTable)
            {
                var nItem = item.Value as SkillTable;
                _loadObject(nItem.Icon);
            }

            var buffTable = TableManager.instance.GetTable<BuffTable>();
            foreach (var item in buffTable)
            {
                var nItem = item.Value as BuffTable;
                _loadObject(nItem.Icon);
            }

            {
                var resTable = TableManager.instance.GetTable<ResTable>();
                foreach (var item in resTable)
                {
                    var nItem = item.Value as ResTable;
                    _loadObject(nItem.IconPath);
                }
            }

            {
                var table = TableManager.instance.GetTable<FunctionUnLock>();
                foreach (var item in table)
                {
                    var nItem = item.Value as FunctionUnLock;
                    _loadObject(nItem.IconPath);
                }
            }
            {
                var table = TableManager.instance.GetTable<AreaTable>();
                foreach (var item in table)
                {
                    var nItem = item.Value as AreaTable;
                    _loadObject(nItem.LoadBG);
                }
            }
            {
                var table = TableManager.instance.GetTable<JobTable>();
                foreach (var item in table)
                {
                    var nItem = item.Value as JobTable;
                    _loadObject(nItem.JobPortrayal);
                    _loadObject(nItem.JobHead);
                    _loadObject(nItem.JobBody);
                    _loadObject(nItem.JobIcon);
                    foreach (var iiii in nItem.SkillIcon)
                        _loadObject(iiii);
                    _loadObject(nItem.AwakeJobIcon);
                    _loadObject(nItem.AwakeJobName);
                    _loadObject(nItem.AwakeSkillName);
                }
            }

            {
                var table = TableManager.instance.GetTable<MissionTable>();
                foreach (var item in table)
                {
                    var nItem = item.Value as MissionTable;
                    _loadObject(nItem.MissionNpcIcon);
                }
            }

            {
                var table = TableManager.instance.GetTable<FaceTable>();
                foreach (var item in table)
                {
                    var nItem = item.Value as FaceTable;
                    _loadObject(nItem.Path);
                }
            }
            {
                var table = TableManager.instance.GetTable<DungeonUIConfigTable>();
                foreach (var item in table)
                {
                    var nItem = item.Value as DungeonUIConfigTable;
                    _loadObject(nItem.BackgroundPath);
                }
            }{
                var table = TableManager.instance.GetTable<NpcTable>();
                foreach (var item in table)
                {
                    var nItem = item.Value as NpcTable;
                    _loadObject(nItem.FunctionIcon);
                    _loadObject(nItem.NpcIcon);
                    _loadObject(nItem.NpcBody);
                    _loadObject(nItem.NpcTitleIcon);
                }
            }

            int ci = 0;
            foreach (var guid in prefabs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var all = AssetDatabase.GetDependencies(path);

                EditorUtility.DisplayProgressBar("find delete", string.Format("prefab {0}", path), ci++ * 1.0f / prefabs.Length);

                foreach (var dep in all)
                {
                    if (dep.EndsWith(".jpg")
                     || dep.EndsWith(".png")
                     || dep.EndsWith(".jpge")
                     || dep.EndsWith(".tga"))
                    {
                        if (mTextureDict.ContainsKey(dep))
                        {
                            mTextureDict[dep]++;
                        }
                        else
                        {
                            Logger.LogErrorFormat("dep is out of UI/Image, {0}", dep);
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            ci = 0;
            string final = "";
            foreach (var item in mTextureDict)
            {
                EditorUtility.DisplayProgressBar("find delete", string.Format("png {0}", item.Key), ci++ * 1.0f / mTextureDict.Count);

                if (item.Value <= 0)
                {
                    UnityEngine.Debug.LogWarningFormat("item {0}", item.Key);
                    final += item.Key + "\n";
                }
            }

            EditorUtility.ClearProgressBar();
            File.WriteAllText("all.txt", final);
        }

        private static void _allUIIMmage1s()
        {
            mUIList.Clear();

            var prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });

            foreach (var paths in prefabs)
            {
                var nepath = AssetDatabase.GUIDToAssetPath(paths);

                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(nepath);
                var go = PrefabUtility.InstantiatePrefab(obj) as GameObject;

                if (go != null)
                {
                    var com = go.GetComponent<ComClientFrame>();

                    if (null != com)
                    {
                        com.mLayer = FrameLayer.Middle;
                        com.mZOrder = -1;

                        Logger.LogErrorFormat("process {0}", nepath);

                        PrefabTools.ApplyPrefab(go);
                    }

                    //_getAllUI(go);

                    GameObject.DestroyImmediate(go);
                }
            }

            //foreach (var img in mUIList)
            //    Logger.LogError(img);
        }

        //[MenuItem("[UITest]/[=>guessUnuseImage]")]
        public static void AllUIImage()
        {
            _allUIIMmage();
        }

        //[MenuItem("[UITest]/[=>moveBigPic]")]
        public static void MoveBigPic()
        {
            // var textures = AssetDatabase.FindAssets("t:texture", new string[] { "Assets/UI/Image"});

            // foreach (var guid in textures)
            // {
            //     var path = AssetDatabase.GUIDToAssetPath(guid);

            //     var text = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            //     var tip = AssetImporter.GetAtPath(path) as TextureImporter;

            //     int width = 0;
            //     int height = 0;

            //     TPImporter.TexturePackerImporter.GetOrigImageSize(text, tip, out width, out height);

            //     if (width >= 1024 || height>=1024)
            //     {
            //         var filename = Path.GetFileName(path);
            //         if (!Directory.Exists("Assets/UI/Image/Back/"))
            //         {
            //             Directory.CreateDirectory("Assets/UI/Image/Back/");
            //         }
            //         AssetDatabase.MoveAsset(path, "Assets/UI/Image/Back/" + filename);
            //     }
            // }

        }

        private static string _getDirName(string packedname, char split = '_')
        {
            var sp = packedname.Split(split);
            packedname = packedname.Replace(sp[0] + split, "");
            packedname = packedname.Replace(split + sp[sp.Length - 1], "");
            return packedname;
        }

        private static string _getFinalPath(string packedname, string name, bool isMuti = false)
        {

            if (isMuti)
            {
                var dirname = _getDirName(packedname, '-');
                var namearr = name.Split('-');
                var path = string.Join("/", namearr);
                return "UI/Image/" + dirname + "/" + path;
            }
            else
            {
                var sp = packedname.Split('-');
                var path = string.Join("/", sp, 1, sp.Length - 1);
                return "UI/Image/" + path;
            }
        }

        //[MenuItem("[UITest]/[=>generateJsonFile]")]
        public static void generateJsonFile()
        {
            Hashtable tabel = new Hashtable();

            Dictionary<string, Dictionary<string, string>> mDic = new Dictionary<string, Dictionary<string, string>>();

            string[] srcText = AssetDatabase.FindAssets("t:texture", new string[] { "Assets/UI/Image" });
            foreach (string guid in srcText)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                path = CFileManager.EraseExtension(path);

                string[] sp = path.Split('/');

                if (sp.Length > 4)
                {
                    string dirname = sp[3];
                    string finalName = path.Replace("Assets/", "");

                    if (dirname == "Back" || dirname == "Comic" || dirname == "Emotion")
                    {
                        Hashtable newtable = new Hashtable();

                        string addPath = "UIPacked/up-" + dirname + "-" + Path.GetFileName(path);
                        string addPathNoEt = Path.GetFileNameWithoutExtension(addPath);
                        newtable.Add("path", addPath);
                        newtable.Add("name", addPathNoEt);

                        tabel.Add(finalName.ToLower(), newtable);
                    }
                    else
                    {
                        string spriteName = sp[sp.Length - 1]; //string.Join("-", sp, 4, sp.Length - 4);
                        //string extensionName = Path.GetExtension(spriteName);
                        //string spriteNameWithOutEx = spriteName.Replace(extensionName, "");

                        string[] packedtxt = AssetDatabase.FindAssets("t:sprite " + spriteName, new string[] { "Assets/Resources/UIPacked" });

                        bool isFind = false;
                        for (int i = 0; i < packedtxt.Length; ++i)
                        {
                            string packedPath = AssetDatabase.GUIDToAssetPath(packedtxt[i]);
                            TextureImporter ti = AssetImporter.GetAtPath(packedPath) as TextureImporter;

                            packedPath = packedPath.Replace("Assets/Resources/", "");

                            for (int j = 0; j < ti.spritesheet.Length; ++j )
                            {
                                if (ti.spritesheet[j].name == spriteName)
                                {
                                    Hashtable newtable = new Hashtable();
                                    newtable.Add("path", packedPath);
                                    newtable.Add("name", spriteName);
                                    tabel.Add(finalName.ToLower(), newtable);
                                    isFind = true;
                                    break;
                                }
                            }

                            if (!isFind && ti.spriteImportMode == SpriteImportMode.Single)
                            {
                                if (spriteName == Path.GetFileNameWithoutExtension(packedPath))
                                {
                                    Hashtable newtable = new Hashtable();
                                    newtable.Add("path", packedPath);
                                    newtable.Add("name", spriteName);
                                    tabel.Add(finalName.ToLower(), newtable);
                                }
                            }

                            if (isFind) break;

                        }

                        if (!isFind) UnityEngine.Debug.LogErrorFormat("can't find sprite {0} with count {1}", spriteName, packedtxt.Length);
                    }
                }
            }

            var jsonstr = XUPorterJSON.MiniJSON.jsonEncode(tabel);
            File.WriteAllText("./Assets/StreamingAssets/ui.json", jsonstr);
        }

        public static void UIPdate()
        {
            var mTextureDict = new Dictionary<string, int>();

            {
                var newstr = File.ReadAllText("fileList.txt").Split('\n');
                foreach(var sss in newstr)
                {
                    if (!mTextureDict.ContainsKey(sss))
                    {
                        mTextureDict.Add(sss, 0);
                    }
                    else
                    {
                        mTextureDict[sss]++;
                    }
                }
            }

            var str = File.ReadAllText("filterImage.txt").Split('\n');

            foreach (var item in str)
            {
                var path = CFileManager.EraseExtension(item);

                var go = Resources.Load(path);
                if (go != null)
                {
                    var iid = go.GetInstanceID();
                    var assetpath = AssetDatabase.GetAssetPath(iid);

                    if (mTextureDict.ContainsKey(assetpath))
                    {
                        mTextureDict[assetpath]++;
                        Logger.LogErrorFormat("delete asset path {0}", assetpath);
                    }
                    else
                    {
                        //Logger.LogErrorFormat("new icon ! with path {0}", assetpath);
                        //mTextureDict.Add(assetpath, 1);
                    }
                }

                go = null;
                Resources.UnloadUnusedAssets();
            }

            var finalstr = "";
            foreach(var ite in mTextureDict)
            {
                if (ite.Value == 0)
                {
                    finalstr += ite.Key + "\n";
                }
            }

            File.WriteAllText("finalIamgeList.txt", finalstr);
        }
    }
}


/*
public class UITextureImporter : AssetPostprocessor
{
    public static string sPrePath = "Assets/UI/Image/";
    public static string sPacckedPath = "Assets/Resources/UIPacked";
    public static string sSplitStr = "-";

    private static void print(string pre, List<string> array)
    {
        UnityEngine.Debug.LogErrorFormat("[{0}]", pre);

        foreach (var item in array)
        {
            UnityEngine.Debug.LogErrorFormat("{0}", item);
        }
    }

    private static List<string> sAddList = new List<string>();
    private static List<string> sRemovedList = new List<string>();
    private static Dictionary<string, bool> sModifyDir = new Dictionary<string, bool>();

    private static string _getDir(string path)
    {
        if (path.StartsWith(sPrePath))
        {
            path = path.Replace(sPrePath, "");
            return path.Split('/')[0];
        }

        return "";
    }

    private static void _getModifyDir(List<string> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            var dir = _getDir(list[i]);
            if (dir.Length > 0)
            {
                if (!sModifyDir.ContainsKey(dir))
                    sModifyDir.Add(dir, true);
            }
        }
    }

    private static void _repack()
    {
        int i = 0;

        foreach (var item in sModifyDir)
        {
            if (item.Key == "Comic" || item.Key == "Back" || item.Key == "Emotion" )
            {
                continue;
            }

            var path = sPrePath + item.Key;

            if (Directory.Exists(path))
            {
                var args = string.Format("\"{0}\" --format unity-texture2d --trim-mode None --sheet \"{1}/p-{2}-{{n}}.png\" --data \"{1}/p-{2}-{{n}}.tpsheet\" --force-squared --max-width 1024 --max-height 1024 --multipack", path, sPacckedPath, item.Key);
                EditorUtility.DisplayProgressBar("Wait!", "==>==>==>", i++ / sModifyDir.Count);
                try 
                {
                    var resulte = _runCmd("../../texturetool/TexturePacker/bin/TexturePacker.exe", args, ".");
                    UnityEngine.Debug.LogFormat("resulte {0}", resulte);
                } 
                catch (System.Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }
            else
            {
                var deleteasset = sPacckedPath +"/"+ item.Key;
                var find = AssetDatabase.FindAssets(item.Key, new string[] { sPacckedPath });
                for (int j = 0; j < find.Length; ++j)
                {
                    var fpath = AssetDatabase.GUIDToAssetPath(find[j]);
                    UnityEngine.Debug.LogErrorFormat("delete asset {0}", fpath);
                    AssetDatabase.MoveAssetToTrash(fpath);
                }
            }
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        return;
        sModifyDir.Clear();

        sRemovedList.Clear();
        sRemovedList.AddRange(deletedAssets);
        sRemovedList.AddRange(movedFromAssetPaths);
        sRemovedList.RemoveAll(x => { return !x.StartsWith(sPrePath); });

        _getModifyDir(sRemovedList);

        sAddList.Clear();
        sAddList.AddRange(importedAssets);
        sAddList.AddRange(movedAssets);
        sAddList.RemoveAll(x => { return !x.StartsWith(sPrePath); });

        _getModifyDir(sAddList);

        GameClient.ImageUITool.MoveBigPic();

        //_unpackedUI();
        //_repack();
    }


    public static string UnpackFileName(string path)
    {
        // 根据 Asset/UI/Images/
        return "";

        //return string.Join(sSplitStr, new string[] { "up", dirname, filename });
    }

    public static string PackFileName(string filepath)
    {
        string[] sp = filepath.Split('/');

        string spriteName = string.Join("-", sp, 4, sp.Length - 4);
        string extensionName = Path.GetExtension(spriteName);

        return spriteName.Replace(extensionName, "");
    }

    private static void _unpackedUI()
    {
        for (int i = 0; i < sRemovedList.Count; ++i)
        {
            var dirname = _getDir(sRemovedList[i]);
            if (dirname == "Back" || dirname == "Comic" || dirname == "Emotion")
            {
                var filename = Path.GetFileName(sRemovedList[i]);
                var toPath = string.Format("{0}/up-{1}-{2}", sPacckedPath, dirname, filename);
                if (File.Exists(toPath))
                {
                    UnityEngine.Debug.LogFormat("delete fiel {0}", toPath);
                    AssetDatabase.MoveAssetToTrash(toPath);
                }
            }
        }

        for (int i = 0; i < sAddList.Count; ++i)
        {
            var dirname = _getDir(sAddList[i]);
            if (dirname == "Back" || dirname == "Comic" || dirname == "Emotion")
            {
                var filename = Path.GetFileName(sAddList[i]);
                var toPath = string.Format("{0}/up-{1}-{2}", sPacckedPath, dirname, filename);
                UnityEngine.Debug.LogFormat("Copy {0} -> {1}", sAddList[i], toPath);
                AssetDatabase.CopyAsset(sAddList[i], toPath);
            }
        }
    }

    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    {
        var path = assetPath.Trim();

        //if (path.StartsWith(sPrePath))
        //{
        //}

        if (path.StartsWith(sPrePath))
        {
            //GameClient.ImageUITool.generateJsonFile();

            UnityEngine.Debug.LogFormat("update the path : " + path);

            string dirname = Path.GetDirectoryName(path);
            dirname = dirname.Replace(sPrePath, "");
            dirname = dirname.Split(Path.DirectorySeparatorChar)[0];

            if (dirname == "Back" || dirname == "Comic" || dirname == "Emotion")
            {
                return;
            }

            string filename = Path.GetFileNameWithoutExtension(path);
            string packedName = PackFileName(path);
            string findstr = string.Format("t:sprite {0}", packedName);

            UnityEngine.Debug.LogFormat("findstr {0} at {1}", findstr, sPacckedPath);

            string[] allTexture = AssetDatabase.FindAssets(findstr, new string[] { sPacckedPath });

            UnityEngine.Debug.LogFormat("find texture count {0}", allTexture.Length);

            for (int i = 0; i < allTexture.Length; ++i)
            {
                string txtPath = AssetDatabase.GUIDToAssetPath(allTexture[i]);

                TextureImporter ti = AssetImporter.GetAtPath(txtPath) as TextureImporter;
                for (int j = 0; j < ti.spritesheet.Length; ++j)
                {
                    if (ti.spritesheet[j].name == packedName)
                    {
                        _modifyMetaInfo(AssetDatabase.GetTextMetaFilePathFromAssetPath(txtPath), packedName, sprites[0]);
                        AssetDatabase.ImportAsset(txtPath, ImportAssetOptions.ForceUpdate);
                        break;
                    }
                }
            }
        }
    }

    private static void _modifyMetaInfo(string path, string keyname, Sprite srcSprite)
    {
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamReader sr = new StreamReader(stream, Encoding.UTF8);

        YamlStream yamlStream = new YamlStream();
        yamlStream.Load(sr);

        YamlMappingNode rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        var spritesNode = UITexturePacker._FindYAMLHierarchyNode("sprites", rootNode);

        YamlSequenceNode sprites = spritesNode as YamlSequenceNode;

        if (null != sprites)
        {
            foreach (var sprite in sprites.Children)
            {
                YamlMappingNode spriteAttris = (YamlMappingNode)sprite;
                foreach (var spriteAttri in spriteAttris.Children)
                {
                    if (spriteAttri.Key.ToString() == "name")
                    {
                        YamlScalarNode name = (YamlScalarNode)spriteAttri.Value;
                        if (name.ToString() == keyname)
                        {
                            YamlMappingNode boarderNode = UITexturePacker._FindYAMLHierarchyNode("border", sprite) as YamlMappingNode;
                            if (null != boarderNode)
                            {
                                if (null != srcSprite)
                                {
                                    foreach (var boarder in boarderNode.Children)
                                    {
                                        YamlScalarNode key = boarder.Key as YamlScalarNode;
                                        YamlScalarNode value = boarder.Value as YamlScalarNode;
                                        if (key.Value == "x")
                                            value.Value = srcSprite.border.x.ToString();
                                        else if (key.Value == "y")
                                            value.Value = srcSprite.border.y.ToString();
                                        else if (key.Value == "z")
                                            value.Value = srcSprite.border.z.ToString();
                                        else if (key.Value == "w")
                                            value.Value = srcSprite.border.w.ToString();
                                        else
                                            ;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        stream.Close();

        FileStream streamW = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(streamW, Encoding.UTF8);
        yamlStream.Save(sw, false);

        streamW.Flush();

        sw.Close();
        streamW.Close();
    }

    private static string _runCmd(string exe, string args, string workpath)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo();
        processInfo.FileName = exe;
        processInfo.Arguments = args;
        processInfo.CreateNoWindow = true;
        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
        processInfo.WorkingDirectory = workpath;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardOutput = true;

        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        return process.StandardOutput.ReadToEnd();
    }

}
*/
