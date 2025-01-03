using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using XUPorterJSON;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Xml;
using BehaviorTreeMechanism;

namespace HeroGo
{
    public class UtilityTools
    {
        public static void GetSkillFiles(ArrayList fileDic, string fullPath, bool recursive = false, int level = 1)
        {
            DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);

            DirectoryInfo[] dirList = dirOutPath.GetDirectories();

            if (recursive)
            {
                foreach (DirectoryInfo info in dirList)
                {
                    GetSkillFiles(fileDic, fullPath + "/" + info.Name, recursive, level + 1);
                }
            }

            FileInfo[] objFileList = dirOutPath.GetFiles();
            foreach (FileInfo objFile in objFileList)
            {
                string fileName = objFile.Name;
                if (recursive && level > 1)
                    fileName = dirOutPath.Name + "/" + fileName;
                if (!fileName.StartsWith(".") && !fileName.Contains(".meta") && !fileName.Contains(".json") && !fileName.Contains(".bytes"))
                {
                    fileDic.Add(fileName.Replace(".asset", ""));
                }
            }
        }

        public static void GenFlatBufferSKillCollection(string jsonFileName, string fullpath, ArrayList filelist)
        {
            FBSkillDataTools tools = new FBSkillDataTools();
            List<FBSkillDataTools.DSkillDataItem> skillDataList = new List<FBSkillDataTools.DSkillDataItem>();

            foreach (var file in filelist)
            {
                string path = fullpath + "/" + file + ".asset";
                var data = AssetDatabase.LoadAssetAtPath(path, typeof(DSkillData)) as DSkillData;
                skillDataList.Add(new FBSkillDataTools.DSkillDataItem(file as string, data));
            }

            FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);
            tools.CreateFBSkillDataCollection(builder, skillDataList);

            string outfilename = fullpath + "/" + jsonFileName + "_bin.bytes";
            outfilename = UtilityTools.GetRawResPathOutOfUnity(outfilename);
            using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
            {
                File.WriteAllBytes(outfilename, ms.ToArray());
            }
            Debug.LogWarning("FlatBuffer :" + outfilename);
            //AssetDatabase.ImportAsset(outfilename);
        }

        public static void ClearFlatBufferSKillCollection(string jsonFileName, string fullpath)
        {
            string outfilename = fullpath + "/" + jsonFileName + "_bin.bytes";
            AssetDatabase.DeleteAsset(outfilename);

        }

        public static void GenSkillFileList2(string fullPath)
        {
            string folderName = Utility.GetPathLastName(fullPath);
            string jsonFileName = fullPath + "/" + folderName + "_FileList.json";

            //先删掉这个文件
            if (File.Exists(jsonFileName))
                File.Delete(jsonFileName);

            ArrayList fileDic = new ArrayList();

            GetSkillFiles(fileDic, fullPath, true, 1);

            string json = MiniJSON.jsonEncode(fileDic);

            File.WriteAllText(jsonFileName, json);

            AssetDatabase.ImportAsset(jsonFileName);

            GenFlatBufferSKillCollection(folderName + "_FileList", fullPath, fileDic);

            Debug.Log("Write " + jsonFileName + " succeed!!!!");
        }

        public static void GenSkillFileListFB(string fullPath)
        {
            string folderName = Utility.GetPathLastName(fullPath);
            ArrayList fileDic = new ArrayList();
            GetSkillFiles(fileDic, fullPath, true, 1);
            GenFlatBufferSKillCollection(folderName + "_FileList", fullPath, fileDic);
        }

        public static void GenSkillJsonFile(string fullPath)
        {
            string folderName = Utility.GetPathLastName(fullPath);
            string jsonFileName = fullPath + "/" + folderName + "_FileList.json";

            //先删掉这个文件
            if (File.Exists(jsonFileName))
                File.Delete(jsonFileName);

            ArrayList fileDic = new ArrayList();

            GetSkillFiles(fileDic, fullPath, true, 1);

            string json = MiniJSON.jsonEncode(fileDic);

            File.WriteAllText(jsonFileName, json);

            Debug.Log("Write " + jsonFileName + " succeed!!!!");
        }

        public static void GenSkillFileListFull(string fullPath)
        {
            string folderName = Utility.GetPathLastName(fullPath);
            string jsonFileName = fullPath + "/" + folderName + "_FileList.json";

            //先删掉这个文件
            if (File.Exists(jsonFileName))
                File.Delete(jsonFileName);

            ArrayList fileDic = new ArrayList();

            GetSkillFiles(fileDic, fullPath, true, 1);

            string json = MiniJSON.jsonEncode(fileDic);

            File.WriteAllText(jsonFileName, json);

            //AssetDatabase.ImportAsset(jsonFileName);

            GenFlatBufferSKillCollection(folderName + "_FileList", fullPath, fileDic);

            Debug.Log("Write " + jsonFileName + " succeed!!!!");
        }
        private static HashSet<uint> sm_MarkSet = new HashSet<uint>();
        private static int sm_RepeatCount = 0;
        static private StringBuilder m_MarkMapBuilder;
        static private HashSet<uint> m_MarkIdSet = new HashSet<uint>();
        // 导出水印描述信息
        public static void ExportReadableMark()
        {
            m_MarkIdSet.Clear();
            string exportPath = Application.dataPath + "/Scripts";

            m_MarkMapBuilder = new StringBuilder();
            SearchAllCSFiles(exportPath);

            SaveExportFile();
            Logger.LogErrorFormat("导出水印描述信息完成");
        }

        private static void SearchAllCSFiles(string path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            foreach (FileInfo nextFile in folder.GetFiles("*.cs"))
            {
                ExportReadableFile(nextFile.FullName);
            }
            foreach (DirectoryInfo nextFolder in folder.GetDirectories())
            {
                SearchAllCSFiles(nextFolder.FullName);
            }
        }

        private static void SaveExportFile(string path = "")
        {
            StreamWriter sw = null;
            try
            {
                string savePath = Application.dataPath + "/Editor/RecordMarkDesc//ReadableMarkMap.txt";
                if (!string.IsNullOrEmpty(path))
                {
                    savePath = path;
                }
                sw = new StreamWriter(savePath);
                sw.Write(m_MarkMapBuilder);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        private static void ExportReadableFile(string filePath)
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader(filePath);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(line, @"//\s*Mark:"))
                    {
                        m_MarkMapBuilder.Append(GetMarkInfo(line)).Append("\n");
                    }
                }
                file.Close();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                if (file != null)
                {
                    file.Close();
                }
            }
        }
        private static string GetMarkInfo(string str)
        {
            StringBuilder buildParam = new StringBuilder();
            foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(str, @"0[xX][0-9a-fA-F]*"))
            {
                uint id = Convert.ToUInt32(match.ToString(), 16);
                if (m_MarkIdSet.Contains(id))
                {
                    Logger.LogErrorFormat("[ExportReadableMark Error]:Mark id {0} 重复", match.ToString());
                }
                else
                {
                    m_MarkIdSet.Add(id);
                }

                buildParam.Append(match).Append("|@@|").Append(str.Substring(match.Index + match.Length)).Append("\n");
                return buildParam.ToString();
            }

            Logger.LogErrorFormat("[ExportReadableMark Error]:can not get mark id:{0}", str);
            return "";
        }
        private static void SearchAllFile(List<FileInfo> fileList, string path, string filter)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            fileList.AddRange(folder.GetFiles(filter));
            foreach (DirectoryInfo nextFolder in folder.GetDirectories())
            {
                SearchAllFile(fileList, nextFolder.FullName, filter);
            }
        }
        public static void CheckMarkRepeat()
        {
            sm_MarkSet.Clear();
            sm_RepeatCount = 0;
            string path = Application.dataPath + "/Scripts";
            List<FileInfo> fileList = new List<FileInfo>();
            SearchAllFile(fileList, path, "*.cs");
            foreach (var file in fileList)
            {
                CheckMarkWithFile(file.FullName);
            }
            Logger.LogErrorFormat("校验水印重复完成, 发现重复个数[{0}]", sm_RepeatCount);
        }

        private static void CheckMarkWithFile(string path)
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader(path);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(line, @"\.Mark\w*?\(\s*0[xX][0-9a-fA-F]*"))
                    {
                        foreach (System.Text.RegularExpressions.Match match2 in System.Text.RegularExpressions.Regex.Matches(line, @"0[xX][0-9a-fA-F]*"))
                        {
                            uint id = Convert.ToUInt32(match2.ToString(), 16);
                            if (sm_MarkSet.Contains(id))
                            {
                                sm_RepeatCount++;
                                Logger.LogErrorFormat("Mark id {0} 重复", match2.ToString());
                            }
                            else
                            {
                                sm_MarkSet.Add(id);
                            }
                        }
                    }
                }
                file.Close();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                if (file != null)
                {
                    file.Close();
                }
            }
        }


        public static void ClearSkillFileList2(string fullPath)
        {
            string folderName = Utility.GetPathLastName(fullPath);
            string jsonFileName = fullPath + "/" + folderName + "_FileList.json";

            AssetDatabase.DeleteAsset(jsonFileName);
            ClearFlatBufferSKillCollection(folderName + "_FileList", fullPath);
        }
        static string[] SpecialEffectNames = {
            "p_Xueren_guo",
            "P_Dungeon_Ruolan_L_mutong",
            "Common_xuli_red",
        };

        [MenuItem("Assets/特效数据预Cook", false)]
        public static void CookEffectData()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            string fullPath = FileTools.GetAssetFullPath(selection[0]);

            DirectoryInfo mydir = new DirectoryInfo(fullPath);
            if (mydir.Exists)
            {
                string[] pathList = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);
                for (int i = 0; i < pathList.Length; ++i)
                {
                    var path = pathList[i];
                    {
                        EditorUtility.DisplayProgressBar("特效数据预Cook　path:" + fullPath, "Converting .. " + i + "/" + pathList.Length, (i + 1) / (float)pathList.Length);
                        DoCookData(path);
                    }
                }
            }
            else
            {
                DoCookData(fullPath);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();

            Logger.LogErrorFormat("特效数据预Cook成功!!!");
        }

        static bool IsInSpecial(string path)
        {
            for (int i = 0; i < SpecialEffectNames.Length; ++i)
            {
                if (path.Contains(SpecialEffectNames[i]))
                    return true;
            }

            return false;
        }

        static void DoCookData(string path)
        {
            if (!path.Contains(".meta") && (path.Contains("Skill_") || path.Contains("Eff_") || path.Contains("EffUI") || path.Contains("eff_") || path.Contains("Topicon_")))
            {
                GameObject assetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (assetPrefab == null) return;
                GameObject instance = GameObject.Instantiate(assetPrefab) as GameObject;
                GeEffectProxy assetProxy = instance.GetComponent<GeEffectProxy>();
                if (assetProxy == null)
                    assetProxy = instance.AddComponent<GeEffectProxy>();
                if (assetProxy != null)
                    assetProxy.DoCookData();

                PrefabUtility.SaveAsPrefabAsset(instance, path);
                AssetDatabase.SaveAssets();
                GameObject.DestroyImmediate(instance);
            }
        }
        [MenuItem("Assets/GenALLSkillFileList", false, 1011)]
        public static void GenAllSkillFileList2()
        {
            //		fullPath	"Assets/Resources/Data/SkillData/Swordman/Swordman_2"	System.String
            string[] folders =
            {
                "APC",
                "Bullet_Object",
                "Gungirl",
                "Gunman",
                "Item_Object",
                "Mage",
                "Monster_Object",
                "Objects",
                "Pet",
                "Skill_Object",
                "Suicong",
                "Swordman",
                "Monster/Bantunvzhanshi",
                "Monster/Bantuzhanshi",
                "Monster/Bingxuenvwang",
                "Monster/Chali",
                "Monster/Chapter2",
                "Monster/Chapter3",
                "Monster/Chapter4",
                "Monster/Chapter7",
                "Monster/Daozei",
                "Monster/Emo",
                "Monster/Goblin",
                "Monster/Hanbinghu",
                "Monster/Item",
                "Monster/jiangshi",
                "Monster/Juren",
                "Monster/Kelahe",
                "Monster/Laoshu",
                "Monster/Longtoupao",
                "Monster/Longren",
                "Monster/Mage",
                "Monster/Maoyao",
                "Monster/Niutou",
                "Monster/Renoushi",
                "Monster/Skill",
                "Monster/Shujing",
                "Monster/Wanjushib",
                "Monster/Weishi",
                "Monster/Xueguai",
                "Monster/Yecha",
                "Monster/Yilong"
            };

            string[] names =
            {
                "Common",
                "Monster/Buwanjia",
                "Monster/Cube",
                "Monster/Laoshudui",
                "Monster/Laoshuta",
                "Monster/Pite",
                "Monster/Saigehate",
                "Monster/Zuiezhiyan"
            };


            List<string> convertList = new List<string>();

            for (int i = 0; i < names.Length; ++i)
            {
                string fullPath = "Assets/Resources/Data/SkillData/" + names[i];
                convertList.Add(fullPath);
            }


            for (int i = 0; i < folders.Length; ++i)
            {

                string fullPath = "Assets/Resources/Data/SkillData/" + folders[i];

                DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);

                DirectoryInfo[] dirList = dirOutPath.GetDirectories();

                for (int j = 0; j < dirList.Length; ++j)
                {
                    string full = dirList[j].FullName;
                    int index = full.IndexOf("Assets");

                    string newPath = full.Substring(index, full.Length - index);
                    newPath = newPath.Replace("\\", "/");
                    convertList.Add(newPath);
                }

            }


            for (int i = 0; i < convertList.Count; ++i)
            {
                string fullPath = convertList[i];
                EditorUtility.DisplayProgressBar("转换技能", "Converting .. " + fullPath, (i + 1) / (float)convertList.Count);
                GenSkillFileList2(fullPath);
            }

            EditorUtility.ClearProgressBar();
            UnityEngine.Debug.LogWarning("GenSkillFile Success!! ");
        }

        static string[] postFix = new string[]
        {
            "",
            "_0",
            "_1",
            "_2",
            "_3",
            "_4"
        };

        [MenuItem("[TM工具集]/[FB转ScriptsObject]/AllInOne", false, -1)]
        public static void ConvertAllUnityAssets2FBData()
        {
            //GenerateTableData();
            GenerateFBTableData();
            GenerateAIXmlData();
            GenerateTransportDoorData();
            GenerateFBDSceneData();
            GenerateFBDDungeonData();

            GenAllSkillFileListFromTableFull();

#if !LOGIC_SERVER
            ModelDataFlatBufferConverter.ConvertAllModelData();
            GlobalSettingFlatBufferConverter.ConvertAllGlobalSetting();
#endif
        }

        [MenuItem("[TM工具集]/[FB转ScriptsObject]/ConvertSkillFileListFull", false, 1011)]
        public static void GenAllSkillFileListFromTableFull()
        {
            var resTable = TableManager.GetInstance().GetTable<ProtoTable.ResTable>();
            List<string> convertedCached = new List<string>();
            List<string> ignoreCached = new List<string>();

            convertedCached.Add("Assets/Resources/Data/SkillData/Common");
            foreach (var item in resTable)
            {
                ProtoTable.ResTable t = item.Value as ProtoTable.ResTable;

                for (int i = 0; i < t.ActionConfigPath.Count; ++i)
                {
                    var path = t.ActionConfigPath[i];
                    path = "Assets/Resources/" + path;

                    for (int j = 0; j < postFix.Length; ++j)
                    {
                        var tempPath = path + postFix[j];
                        if (convertedCached.IndexOf(tempPath) >= 0)
                        {
                            continue;
                        }

                        if (ignoreCached.IndexOf(tempPath) >= 0)
                        {
                            continue;
                        }

                        if (Directory.Exists(tempPath))
                        {
                            convertedCached.Add(tempPath);
                        }
                        else
                        {
                            ignoreCached.Add(tempPath);
                        }
                    }
                }
            }

            for (int k = 0; k < convertedCached.Count; ++k)
            {
                var fullpath = convertedCached[k];
                string msg = string.Format("Converting {0} / {1} Skill：{2}.. ", k + 1, convertedCached.Count, fullpath);
                EditorUtility.DisplayProgressBar("转换技能", msg, (k + 1) / (float)convertedCached.Count);
                GenSkillFileListFull(fullpath);
            }

            EditorUtility.ClearProgressBar();
            UnityEngine.Debug.LogWarning("GenSkillFile Success!! ");
        }


        [MenuItem("Assets/GenSkillFileList", false, 1011)]
        public static void GenSkillFileList()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);

            for (int i = 0; i < selection.Length; ++i)
            {
                var s = selection[i];
                UnityEngine.Debug.LogWarning("GenSkillFile: " + s);
                string fullPath = FileTools.GetAssetFullPath(s);
                EditorUtility.DisplayProgressBar("转换技能", "Converting .. " + s, (i + 1) / (float)selection.Length);
                GenSkillFileList2(fullPath);
            }


            EditorUtility.ClearProgressBar();
            UnityEngine.Debug.LogWarning("GenSkillFile Success!! ");
        }

        [MenuItem("Assets/ClearSkillFileList", false, 1011)]
        public static void ClearSkillFileList()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);

            for (int i = 0; i < selection.Length; ++i)
            {
                var s = selection[i];
                UnityEngine.Debug.LogWarning("ClearSkillFile: " + s);
                string fullPath = FileTools.GetAssetFullPath(s);
                EditorUtility.DisplayProgressBar("清理技能", "Converting .. " + s, (i + 1) / (float)selection.Length);
                ClearSkillFileList2(fullPath);
            }

            EditorUtility.ClearProgressBar();
            UnityEngine.Debug.LogWarning("Clear Success!! ");
        }


        //[MenuItem("Assets/GenALLSkillFileList", false)]
        public static void GenAllSkillFileList()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            string fullPath = FileTools.GetAssetFullPath(selection[0]);

            /*            DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);
                        DirectoryInfo[] dirList = dirOutPath.GetDirectories("*", SearchOption.TopDirectoryOnly);

                        foreach (DirectoryInfo info in dirList)
                        {
                            GenSkillFileList2(fullPath + "/" + info.Name);
                        }*/

            GenSkillFileListRecursion(fullPath);
        }

        public static void GenSkillFileListRecursion(string fullPath)
        {
            DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);
            DirectoryInfo[] dirList = dirOutPath.GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (DirectoryInfo info in dirList)
            {
                string newPath = fullPath + "/" + info.Name;
                GenSkillFileList2(newPath);
                GenSkillFileListRecursion(newPath);
            }
        }

        [MenuItem("Assets/CopyAssetsPath", false)]
        public static void CopyAssetsPath()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            if (selection.Length > 0)
            {
                if (selection[0] is Texture2D)
                {
                    string path = FileTools.GetAssetPath(Selection.activeObject);
                    GUIUtility.systemCopyBuffer = path + ":" + Selection.activeObject.name;
                }
                else
                {
                    string path = FileTools.GetAssetPath(selection[0]);
                    if (path.Contains(".prefab"))
                        path = path.Replace(".prefab", "");
                    GUIUtility.systemCopyBuffer = path;
                }
            }
        }

        [MenuItem("Assets/创建UI代码文件", false)]
        public static void CreateUIFrameCodeFile()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            if (selection.Length > 0)
            {
                string path = FileTools.GetAssetPath(selection[0]);
                if (path.Contains(".prefab") && path.Contains("UIFlatten"))
                {
                    path = path.Replace(".prefab", "");

                    CreateUIFrameCodeWnd.ShowWnd(path);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请选择一个UI预制体", "确定");
                }
            }
        }

        [MenuItem("GameObject/DummyRectTransform", false, 0)]
        public static void DummyGameObjectRectTransform()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.Unfiltered);
            if (selection.Length > 0)
            {
                GameObject obj = (selection[0] as GameObject);

                if (obj != null)
                {
                    var rectTransform = obj.GetComponent<RectTransform>();
                    UnityEngine.Debug.Log(ObjectDumper.Dump(rectTransform));
                }
            }
        }

        [MenuItem("[TM工具集]/[FB转ScriptsObject]/生成传送门位置数据")]
        static void GenerateTransportExtraData()
        {
            string[] allDatas = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Scene" });
            for (int i = 0; i < allDatas.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(allDatas[i]);

                _GenerateSingleTransportExtraData(path);
                EditorUtility.DisplayProgressBar("共:" + allDatas.Length + "个", "Checking ..当前第 " + i + "个", (i + 1) / (float)allDatas.Length);
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Assets/战斗相关/生成传送门数据", false)]
        public static void GenerateTransportExtraDataSelectFolder()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            if (selection[0] == null)
                return;

            string fullPath = FileTools.GetAssetFullPath(selection[0]);

            if (fullPath.Contains("prefab"))
            {
                _GenerateSingleTransportExtraData(fullPath);
                return;
            }

            ArrayList fileDic = new ArrayList();
            GetAllAssetByType(fileDic, fullPath + "/", AssetFileType.prefab);

            for (int i = 0; i < fileDic.Count; i++)
            {
                string path = (string)fileDic[i];
                _GenerateSingleTransportExtraData(path);
                EditorUtility.DisplayProgressBar("共:" + fileDic.Count + "个", "Checking ..当前第 " + i + "个", (i + 1) / (float)fileDic.Count);
            }
            EditorUtility.ClearProgressBar();
        }

        private static void _GenerateSingleTransportExtraData(string path)
        {
            string resdatapath = path.Replace(".prefab", "_extradata.asset");

            resdatapath = GameClient.DungeonUtility.GetSceneTransportExtraDataPath(resdatapath);

            string dirname = resdatapath.Replace(Path.GetFileName(resdatapath), "");

            if (File.Exists(resdatapath))
            {
                File.Delete(resdatapath);
            }

            if (!File.Exists(resdatapath))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (null != obj)
                {


                    ControlDoorState controldoorstate = obj.GetComponent<ControlDoorState>();

                    if (null != controldoorstate)
                    {
                        if (!Directory.Exists(dirname))
                        {
                            Directory.CreateDirectory(dirname);
                        }

                        if (obj.transform.position != Vector3.zero)
                        {
                            UnityEngine.Debug.LogErrorFormat("{0} is not at origin point ", path);
                            obj.transform.position = Vector3.zero;
                        }

                        DTransportDoorExtraData doordata = ScriptableObject.CreateInstance(typeof(DTransportDoorExtraData)) as DTransportDoorExtraData;

                        if (null != controldoorstate.OpenDoorObj_TOP)
                        {
                            doordata.top = controldoorstate.OpenDoorObj_TOP.transform.position;
                        }

                        if (null != controldoorstate.OpenDoorObj_BOTTOM)
                        {
                            doordata.buttom = controldoorstate.OpenDoorObj_BOTTOM.transform.position;
                        }

                        if (null != controldoorstate.OpenDoorObj_LEFT)
                        {
                            doordata.left = controldoorstate.OpenDoorObj_LEFT.transform.position;
                        }

                        if (null != controldoorstate.OpenDoorObj_RIGHT)
                        {
                            doordata.right = controldoorstate.OpenDoorObj_RIGHT.transform.position;
                        }

                        AssetDatabase.CreateAsset(doordata, resdatapath);
                    }

                    //GameObject.DestroyImmediate(obj);
                }
            }
        }

        public enum AssetFileType
        {
            asset,
            prefab,
        }

        //获取文件夹下面的所有Asset文件
        static void GetAllAssetByType(ArrayList fileDic, string fullPath, AssetFileType fileType)
        {
            if (!Directory.Exists(fullPath)) return;
            DirectoryInfo direction = new DirectoryInfo(fullPath);

            string type = null;
            switch (fileType)
            {
                case AssetFileType.asset:
                    type = "*.asset";
                    break;
                case AssetFileType.prefab:
                    type = "*.prefab";
                    break;
            }
            FileInfo[] files = direction.GetFiles(type, SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                string fileName = files[i].FullName;
                string[] sArray = fileName.Split(new string[] { "Client\\" }, StringSplitOptions.RemoveEmptyEntries);
                fileName = sArray[1].Replace('\\', '/');
                fileDic.Add(fileName);
            }
        }


        [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert TableData", false, 0)]
        public static void GenerateTableData()
        {
            string[] allIds = AssetDatabase.FindAssets("t:AssetBinary", new string[] { "Assets/Resources" });

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                UnityEngine.Debug.LogErrorFormat("{0}", path);

                AssetBinary data = AssetDatabase.LoadAssetAtPath<AssetBinary>(path);

                string outfilename = GetRawResPathOutOfUnity(path.Replace(".asset", ".bytes"));

                using (var ms = new MemoryStream(data.m_DataBytes, 0, data.m_DataBytes.Length))
                {
                    File.WriteAllBytes(outfilename, ms.ToArray());
                }
            }

        }


        [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert FBTableData", false, 0)]
        public static void GenerateFBTableData()
        {
            string[] allIds = AssetDatabase.FindAssets("t:textasset", new string[] { "Assets/Resources/Data/table_fb" });

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                UnityEngine.Debug.LogErrorFormat("{0}", path);

                if (path.EndsWith("bytes"))
                {
                    string outfilename = GetRawResPathOutOfUnity(path);

                    if (File.Exists(outfilename))
                    {
                        File.Delete(outfilename);
                    }

                    File.Copy(path, outfilename);
                }
            }
        }

        [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert AIXmlData", false, 0)]
        public static void GenerateAIXmlData()
        {
            string[] allIds = AssetDatabase.FindAssets("t:textasset", new string[] { "Assets/Resources/Data/AI/behaviac/exported" });

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                UnityEngine.Debug.LogErrorFormat("{0}", path);

                string outfilename = GetRawResPathOutOfUnity(path);

                if (File.Exists(outfilename))
                {
                    File.Delete(outfilename);
                }

                File.Copy(path, outfilename);
            }
        }

        [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert FBTransportData", false, 0)]
        public static void GenerateTransportDoorData()
        {
            //GenerateTransportExtraData();

            string[] allIds = AssetDatabase.FindAssets("t:DTransportDoorExtraData");

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                UnityEngine.Debug.LogErrorFormat("{0}", path);

                DTransportDoorExtraData data = AssetDatabase.LoadAssetAtPath<DTransportDoorExtraData>(path);

                FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);

                FlatBuffers.Offset<FBTransportDoorExtraData.DTransportDoorExtraData> sdata = FBTransportDoorExtraDataTool.CreateTransportDoorExtraData(builder, data);
                FBTransportDoorExtraData.DTransportDoorExtraData.FinishDTransportDoorExtraDataBuffer(builder, sdata);

                string outfilename = GetRawResPathOutOfUnity(path.Replace(".asset", "_bytes.bytes"));

                using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
                {
                    File.WriteAllBytes(outfilename, ms.ToArray());

                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

        public static string GetRawResPathOutOfUnity(string path)
        {
            string origin = Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar;
            string replaced = Path.DirectorySeparatorChar + "RawData" + Path.DirectorySeparatorChar;
            string fullPath = Path.GetFullPath(path);

            UnityEngine.Debug.LogFormat("[_getResPath] fullPath {0}", fullPath);
            UnityEngine.Debug.LogFormat("[_getResPath] originrep {0}", origin);
            UnityEngine.Debug.LogFormat("[_getResPath] replaced {0}", replaced);


            string finalPath = fullPath.Replace(origin, replaced);

            string fileName = Path.GetFileName(finalPath);

            string dirName = finalPath.Replace(fileName, "");

            UnityEngine.Debug.LogFormat("[_getResPath] filename {0}", fileName);

            UnityEngine.Debug.LogFormat("[_getResPath] create dirname {0}", dirName);

            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }


            return finalPath;
        }


        [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert FBDungeonData", false, 0)]
        public static void GenerateFBDDungeonData()
        {
            string[] allIds = AssetDatabase.FindAssets("t:DDungeonData");

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                UnityEngine.Debug.LogErrorFormat("{0}", path);

                DDungeonData data = AssetDatabase.LoadAssetAtPath<DDungeonData>(path);

                FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);

                FlatBuffers.Offset<FBDungeonData.DDungeonData> sdata = FBDungeonDataTools.CreateFBDungeonData(builder, data);
                FBDungeonData.DDungeonData.FinishDDungeonDataBuffer(builder, sdata);

                string outfilename = GetRawResPathOutOfUnity(path.Replace(".asset", "_bytes.bytes"));

                using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
                {
                    File.WriteAllBytes(outfilename, ms.ToArray());

                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }


        [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert FBSceneData", false, 0)]
        public static void GenerateFBDSceneData()
        {
            string[] allIds = AssetDatabase.FindAssets("t:DSceneData");

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                DSceneData data = AssetDatabase.LoadAssetAtPath<DSceneData>(path);

                FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);
                var sdata = FBSceneDataTools.CreateFBSceneData(builder, data);

                FBSceneData.DSceneData.FinishDSceneDataBuffer(builder, sdata);

                string outfilename = GetRawResPathOutOfUnity(path.Replace(".asset", "_bytes.bytes"));

                using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
                {
                    File.WriteAllBytes(outfilename, ms.ToArray());
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

        /*
        [MenuItem("Assets/ModifyPrefabRoot", false)]
        public static void ModifyPrefabRoot()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.Assets);
            for (int i = 0; i < selection.Length; ++i)
            {
                GameObject obj = (selection[i] as GameObject);
                
                if(obj != null)
                {                    
                    var transforms = obj.transform;
                    bool bInit = (transforms.localPosition == Vector3.zero) && (transforms.localRotation == Quaternion.identity) && (transforms.localScale == Vector3.one);

                    if(bInit == false)
                    {
                        GameObject select = PrefabUtility.InstantiatePrefab(selection[i]) as GameObject;
                        GameObject gameObj = new GameObject();
                        gameObj.name = select.name;
                        select.name  = select.name + "RootModify";
                        select.transform.SetParent(gameObj.transform,false);
                        PrefabUtility.ReplacePrefab(gameObj,selection[i],ReplacePrefabOptions.Default);
                        Editor.DestroyImmediate(gameObj);
                    }
                }
            }
        }
		*/

        [MenuItem("[TM工具集]/新手引导编辑器/TestAchievementEffectPlay", false, 1)]
        public static void TestAchievementEffectPlay()
        {
            GameClient.MissionManager.GetInstance().PushTestAchievementItems(3251);
        }

        [MenuItem("[TM工具集]/新手引导编辑器/TestAchievementAwardPlayFrame", false, 1)]
        public static void AchievementAwardPlayFrame()
        {
            GameClient.AchievementAwardPlayFrame.CommandOpen(null);
        }

        [MenuItem("[TM工具集]/新手引导编辑器/TestAchievementLevelUpPlayFrame", false, 1)]
        public static void AchievementLevelUpPlayFrame()
        {
            GameClient.AchievementLevelUpPlayFrame.CommandOpen(null);
        }

        [MenuItem("GameObject/UI相关/统计GameObject和Componet的数目", false, 30)]
        public static void CalculateGameObjectCount()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
            for (int i = 0; i < selection.Length; ++i)
            {
                GameObject root = selection[i] as GameObject;

                int goCnt = 0;
                int comCnt = 0;

                Dictionary<string, int> dic = new Dictionary<string, int>();

                _dfsGameObjectRoot(root, dic, ref goCnt, ref comCnt);

                string showAns = string.Format("GameObject数目: {0} Component数目: {1}\n", goCnt, comCnt);

                List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(dic);

                list.Sort((l, r) => { return r.Value - l.Value; });

                foreach (var item in list)
                {
                    showAns += string.Format("Com {0}:{1}\n", item.Key, item.Value);
                }

                UnityEngine.Debug.LogFormat(showAns);
            }
        }

        private static void _dfsGameObjectRoot(GameObject root, Dictionary<string, int> dict, ref int goCount, ref int comCount)
        {
            if (null == root)
            {
                return;
            }

            goCount++;

            Component[] coms = root.GetComponents<Component>();

            if (null != coms)
            {
                comCount += coms.Length;
                for (int i = 0; i < coms.Length; ++i)
                {
                    string name = coms[i].GetType().Name;

                    if (!dict.ContainsKey(name))
                    {
                        dict.Add(name, 0);
                    }

                    dict[name]++;
                }
            }

            int childcnt = root.transform.childCount;

            for (int i = 0; i < childcnt; ++i)
            {
                _dfsGameObjectRoot(root.transform.GetChild(i).gameObject, dict, ref goCount, ref comCount);
            }
        }


        [MenuItem("Assets/预览预制体", false)]
        public static GameObject PreviewPrefab()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            GameObject select = PrefabUtility.InstantiatePrefab(selection[0]) as GameObject;
            if (select != null)
            {
                GameObject root = Utility.FindGameObject("UIRoot", false);

                if (root != null)
                {
                    GameObject.DestroyImmediate(root);
                    root = null;
                }

                root = AssetLoader.instance.LoadResAsGameObject("Base/UI/Prefabs/Root/UIRoot");
                root.SetActive(true);
                root.name = "UIRoot";

                GameObject layer = Utility.FindGameObject(root, "UI2DRoot/Top");

                Utility.AttachTo(select, layer);
                EditorGUIUtility.PingObject(select);
                return select;
            }
            else
            {
                Logger.LogError("请选择一个Prefab对象");
                return null;
            }

        }

        [MenuItem("Assets/检查预制体脚本Missing", false)]
        public static void CheckEffectMiss()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            string fullPath = FileTools.GetAssetFullPath(selection[0]);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("丢失脚本预制体:{0}", "\n");
            DirectoryInfo mydir = new DirectoryInfo(fullPath);
            if (mydir.Exists)
            {
                string[] pathList = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);
                for (int i = 0; i < pathList.Length; i++)
                {
                    GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathList[i]);
                    if (asset == null)
                        continue;
                    EditorUtility.DisplayProgressBar("共:" + pathList.Length + "个", "Checking ..当前第 " + i + "个", (i + 1) / (float)pathList.Length);
                    CheckScriptsMissing(asset, asset.name, stringBuilder);
                }
            }
            else
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                CheckScriptsMissing(asset, asset.name, stringBuilder);
            }
            EditorUtility.ClearProgressBar();
            Logger.LogError(stringBuilder.ToString());
            stringBuilder.Clear();
        }

        /// <summary>
        /// 检测脚本丢失
        /// </summary>
        protected static void CheckScriptsMissing(GameObject obj, string parentName, StringBuilder stringBuilder)
        {
            if (obj == null)
                return;
            var components = obj.GetComponents<Component>();
            foreach (var c in components)
            {
                if (c == null)
                {
                    stringBuilder.AppendFormat("Path:{0}/{1} \n", parentName, obj.name);
                    break;
                }
            }

            for (int k = 0; k < obj.transform.childCount; k++)
            {
                var child = obj.transform.GetChild(k).gameObject;
                parentName += "/" + child.name;
                CheckScriptsMissing(child, parentName, stringBuilder);
            }
        }
        [MenuItem("Assets/UI相关/Texture去除MipMap", false)]
        public static void RemoveMipMap()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            string fullPath = FileTools.GetAssetFullPath(selection[0]);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("修改的文件列表:{0}", "\n");
            if (!Directory.Exists(fullPath))
                return;
            string[] searchFolder = { fullPath.Substring(fullPath.IndexOf("Assets", StringComparison.Ordinal)) };
            string[] allAssetsGuid = AssetDatabase.FindAssets("t:texture", searchFolder);
            for (int i = 0; i < allAssetsGuid.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
                var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter == null)
                    continue;
                if (!textureImporter.mipmapEnabled && !textureImporter.isReadable)
                    continue;
                textureImporter.mipmapEnabled = false;
                textureImporter.isReadable = false;
                textureImporter.SaveAndReimport();
                Logger.LogErrorFormat(textureImporter.maxTextureSize.ToString());
                stringBuilder.AppendFormat("{0} \n", path);
                EditorUtility.DisplayProgressBar("共:" + allAssetsGuid.Length + "个", "Checking ..当前第 " + i + "个", (i + 1) / (float)allAssetsGuid.Length);
            }
            EditorUtility.ClearProgressBar();
            Logger.LogError(stringBuilder.ToString());
            stringBuilder.Clear();
        }

        //获取文件夹下面的所有技能配置文件
        static void GetAllSkillData(ArrayList fileDic, string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*.asset", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    string fileName = files[i].FullName;
                    string[] sArray = fileName.Split(new string[] { "Client\\" }, StringSplitOptions.RemoveEmptyEntries);
                    fileName = sArray[1].Replace('\\', '/');
                    fileDic.Add(fileName);
                }
            }
        }

        [MenuItem("Assets/战斗相关/过滤实体触底弹一下技能配置文件", false, 3004)]
        static void FindAllTwoAttachmentAsset()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            if (selection[0] == null)
                return;
            string fullPath = FileTools.GetAssetFullPath(selection[0]) + "/";

            try
            {
                ArrayList fileDic = new ArrayList();
                GetAllSkillData(fileDic, fullPath);
                string log = null;
                for (int i = 0; i < fileDic.Count; i++)
                {
                    string path = (string)fileDic[i];
                    var skillObj = AssetDatabase.LoadAssetAtPath(path, typeof(DSkillData));
                    var data = skillObj as DSkillData;
                    if (data != null && data.entityFrames != null && data.entityFrames.Length > 0)
                    {
                        for (int j = 0; j < data.entityFrames.Length; j++)
                        {
                            if (data.entityFrames[j].hitGroundClick)
                                log += "\r\n" + "path:" + path;
                        }
                    }
                    EditorUtility.DisplayProgressBar("共:" + fileDic.Count + "个", "Checking ..当前第 " + i + "个", (i + 1) / (float)fileDic.Count);
                }
                EditorUtility.ClearProgressBar();
                Logger.LogErrorFormat("{0}", log);
            }
            catch(Exception e)
            {
                Logger.LogErrorFormat(e.ToString());
                EditorUtility.ClearProgressBar();
            }
            
        }

        [MenuItem("[TM工具集]/[机制编辑器]/导出事件接口信息", false)]
        public static void ExportBTMEventFuncInfo()
        {
            List<Assembly> Assemblys = AppDomain.CurrentDomain.GetAssemblies().ToList<Assembly>();
            List<Type> parserList = new List<Type>();
            foreach (var AssemblyItem in Assemblys)
            {
                var list = AssemblyItem.GetTypes();
                foreach (var item in list)
                {
                    if (item.BaseType == typeof(BTEventDataParser))
                    {
                        parserList.Add(item);
                    }
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = new UTF8Encoding(false);
                settings.NewLineChars = Environment.NewLine;
                using (XmlWriter xmlWriter = XmlWriter.Create(ms, settings))
                {
                    xmlWriter.WriteStartDocument(false);
                    xmlWriter.WriteStartElement("root");
                        xmlWriter.WriteStartElement("actor");
                        foreach (var parser in parserList)
                        {
                            var attri = parser.GetCustomAttribute<BTEventTypeAttribute>();
                            if (attri != null)
                            {
                                xmlWriter.WriteStartElement("eventData");
                                xmlWriter.WriteAttributeString("eventType", attri.eventType.ToString());
                                xmlWriter.WriteAttributeString("eventValue", ((int)attri.eventType).ToString());
                                
                                var methods = parser.GetMethods();
                                foreach (var method in methods)
                                {
                                    var selfAttribute = method.GetCustomAttribute<BTEventDataAttribute>(false);
                                    Type baseType = method.DeclaringType.BaseType;
                                    if(baseType == null || baseType != typeof(BTEventDataParser))
                                        continue;
                                
                                    var m = baseType.GetMethod(method.Name);
                                    BTEventDataAttribute parentAttribute = null;
                                    if (m != null)
                                    {
                                        parentAttribute = m.GetCustomAttribute<BTEventDataAttribute>(false);
                                    }
                                    if (selfAttribute == null  &&  parentAttribute != null)
                                    {
                                        xmlWriter.WriteStartElement("list");
                                        xmlWriter.WriteAttributeString("function", parentAttribute.agentFuncName);
                                        xmlWriter.WriteEndElement();
                                    }
                                }
                                xmlWriter.WriteEndElement();
                            }
                        }
                        xmlWriter.WriteEndElement();
                    
                        xmlWriter.WriteStartElement("scene");
                        foreach (var parser in parserList)
                        {
                            var attri = parser.GetCustomAttribute<BTSceneEventTypeAttribute>();
                            if (attri != null)
                            {
                                xmlWriter.WriteStartElement("eventData");
                                xmlWriter.WriteAttributeString("eventType", attri.eventType.ToString());
                                xmlWriter.WriteAttributeString("eventValue", ((int)attri.eventType).ToString());
                                
                                var methods = parser.GetMethods();
                                foreach (var method in methods)
                                {
                                    var selfAttribute = method.GetCustomAttribute<BTSceneEventTypeAttribute>(false);
                                    Type baseType = method.DeclaringType.BaseType;
                                    if(baseType == null || baseType != typeof(BTEventDataParser))
                                        continue;
                                
                                    var m = baseType.GetMethod(method.Name);
                                    BTEventDataAttribute parentAttribute = null;
                                    if (m != null)
                                    {
                                        parentAttribute = m.GetCustomAttribute<BTEventDataAttribute>(false);
                                    }
                                    if (selfAttribute == null  &&  parentAttribute != null)
                                    {
                                        xmlWriter.WriteStartElement("list");
                                        xmlWriter.WriteAttributeString("function", parentAttribute.agentFuncName);
                                        xmlWriter.WriteEndElement();
                                    }
                                }
                                xmlWriter.WriteEndElement();
                            }
                        }
                        xmlWriter.WriteEndElement();//scene
                    
                    xmlWriter.WriteEndElement();// root

                    xmlWriter.WriteEndDocument();
                }
                
                string xml = Encoding.UTF8.GetString(ms.ToArray());
                string path = Application.dataPath+"/../../ExternalTool/BehaviacDesigner/workspace/my_event_filter.xml";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(xml);
                sw.Close();
                Debug.Log("[机制编辑器]事件接口过滤信息导出成功");
            }
        }
    }
}
