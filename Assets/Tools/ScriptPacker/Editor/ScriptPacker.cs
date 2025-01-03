using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using MemoryWriteReaderAnimation;
using NPOI.SS.Formula.Functions;

public static class ScriptPacker
{
    private enum EScriptType
    {
        Script_DAnimationData = 0,
        Script_DNLAvataraPartInfo,
        Script_FrameEffect
    }

    private class PackScriptInfo
    {
        public PackHeaderItemInfo itemInfo;
        public byte[] compressedByte;
        public string scriptName;
    }

    private static string m_SaveAssetDir    = "Assets/Resources/Data/ScriptData";
    private static string m_SaveResourceDir = "Data/ScriptData/";
    private static HashSet<string> m_PackedScriptNames = new HashSet<string>();

    private class PackScriptInput
    {
        public string[] resPath;
        public string resFilter;
        public string packFileName;
        public bool lz4Compress;
        public string progressPrefix;
        public float progressStart;
        public float progressLength;

        public PackScriptInput(string[] _resPath, string _resFilter, string _packFileName, bool _lz4Compress, string _progressPrefix, float _progressStart, float _progressLength)
        {
            resPath = _resPath;
            resFilter = _resFilter;
            packFileName = _packFileName;
            lz4Compress = _lz4Compress;
            progressPrefix = _progressPrefix;
            progressStart = _progressStart;
            progressLength = _progressLength;
        }
    }

    //private static PackScriptInput[] m_ScriptInput = new PackScriptInput[]
    //{
    //    new PackScriptInput(new string[] { "Assets/Resources" }, "t:DAnimationData", "DNLAnimation.bytes", true, "处理动画：", 0, 0.4f),
    //    new PackScriptInput(new string[] { "Assets/Resources" }, "t:DNLAvatarPartInfoData", "DNLAavatarPart.bytes", true, "处理Avatar数据：", 0.4f, 0.5f),
    //    new PackScriptInput(new string[] { "Assets/Resources" }, "t:tm.FashionSuit", "DNLFashion.bytes", true, "处理Fashion数据：", 0.5f, 0.6f),
    //    new PackScriptInput(new string[] { "Assets/Resources" }, "t:DNLGraphicEffectObject", "DNLGraphicEffect.bytes", true, "处理帧动画：", 0.6f, 0.8f),
    //};

    [MenuItem("[TM工具集]/PackScript/转换ScriptData")]
    public static void TransScriptData()
    {
        try
        {
            BeginPackScript();

            // TODO, 按照职业进行分组合并，不是按照类型。

            bool bSuccess = true;
            //bSuccess = bSuccess && PackScript<DAnimationData>(m_ScriptInput[0]);
            //bSuccess = bSuccess && PackScript<DNLAvatarPartInfoData>(m_ScriptInput[1]);
            //bSuccess = bSuccess && PackScript<tm.FashionSuit>(m_ScriptInput[2]);
            //bSuccess = bSuccess && PackScript<DNLGraphicEffectObject>(m_ScriptInput[3]);

            bSuccess = bSuccess && PackSkillListFile("SkillList.bytes", false, 0f, 1.0f);

            EndPackScript(bSuccess);

            //EditorUtility.DisplayDialog("完成", "转换脚本数据成功", "确定");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", "转换脚本数据失败" + e.ToString(), "确定");
            throw;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.ImportAsset(m_SaveAssetDir, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceSynchronousImport);
        }
    }

    private static void DelAllSciptData()
    {
        string[] assetGUIDNames = AssetDatabase.FindAssets("", new string[] { m_SaveAssetDir });
        foreach (string assetGUIDName in assetGUIDNames)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(assetGUIDName));
        }
    }

    private static void BeginPackScript()
    {
        PackScriptData.Instance.Clear();

        if (!Directory.Exists(m_SaveAssetDir))
            Directory.CreateDirectory(m_SaveAssetDir);

        DelAllSciptData();
        m_PackedScriptNames.Clear();
    }

    private static void EndPackScript(bool bSuccess)
    {
        if (bSuccess)
        {
            PackScriptData.Instance.SaveHeaderInfo();
        }
        else
        {
            DelAllSciptData();
        }

        m_PackedScriptNames.Clear();
        PackScriptData.Instance.Clear();
    }

    private static bool PackSkillListFile(string fileName, bool lz4Compress, float progressBegin, float progressLength)
    {
        List<string> header_file_names = PackScriptData.Instance.header_file_names;
        Dictionary<string, PackHeaderItemInfo> header_dic_string = PackScriptData.Instance.header_dic_string;

        for(int i = 0; i < header_file_names.Count; ++i)
        {
            if(header_file_names[i] == fileName)
            {
                EditorUtility.DisplayDialog("错误", "二进制文件名重复：" + fileName, "确定");
                return false;
            }
        }

        // 以Resource相对路径存储，避免runtime时拼接字符串
        header_file_names.Add(m_SaveResourceDir + fileName);
        int fileIndex = header_file_names.Count - 1;

        List<PackScriptInfo> packedScripts = new List<PackScriptInfo>();
        int offset = 0;

        HashSet<string> dealedSkillName = new HashSet<string>();

        var resData = TableManagerEditor.GetInstance().GetTable<ProtoTable.ResTable>();

        float fProgress = progressBegin;
        float progressStep = progressLength / resData.Count;

        foreach(var item in resData)
        {
            ProtoTable.ResTable data = item.Value as ProtoTable.ResTable;

            EditorUtility.DisplayProgressBar("", "处理SkillList", fProgress);
            fProgress += progressStep;

            for(int i = 0; i < 1; ++i)
            {
                //var actionConfigPath = i == 0 ? data.ActionConfigPath :  new string[]{ data.ActionConfigPath } ;
                //var te = data.ActionConfigPath2;
                var actionConfigPath = data.ActionConfigPath.ToList();
                actionConfigPath.Add(data.ActionConfigPath2);

                foreach (var path in actionConfigPath)
                {
                    if (string.IsNullOrEmpty(path) || path == "-")
                    {
                        continue;
                    }

                    //string parentDir = "Assets/Resources/" + Path.GetDirectoryName(path);
                    //UnityEngine.Debug.Log("parentDir " + parentDir);

                    //var allDirs = Directory.GetDirectories(parentDir);
                    //foreach (var dir in allDirs)
                    //{
                    //    string name = Path.GetFileName(dir);
                    //    if (name.Contains("_"))
                    //    {
                    //        UnityEngine.Debug.LogErrorFormat("parentDir->subdir dir:{0} name:{1}", dir, name);
                    //    }

                    //    UnityEngine.Debug.LogWarning("parentDir->subdir " + dir);
                    //}

                    for (int j = 0; j < 100; j++)
                    {
                        var newPath = j > 0 ? path + "_" + j : path;

                        if (!Directory.Exists("Assets/Resources/" + newPath))
                        {
                            continue;
                        }

                        if(dealedSkillName.Add(newPath))
                        {
                            string genPath = "Assets/Resources/" + newPath;

                            byte[] rawBuffer = GenSkillFileList(genPath, lz4Compress);

                            //UnityEngine.Debug.Log("genPath " + genPath);

                            if (rawBuffer != null)
                            {
                                PackScriptInfo packedInfo = new PackScriptInfo();
                                packedInfo.itemInfo = new PackHeaderItemInfo(fileIndex, offset, rawBuffer.Length);
                                packedInfo.compressedByte = rawBuffer;
                                packedInfo.scriptName = newPath;  // 相对resources目录路径

                                packedScripts.Add(packedInfo);

                                offset += rawBuffer.Length;
                            }
                        }
                    }
                }
            }
        }

        if (packedScripts.Count > 0)
        {
            for (int i = 0; i < packedScripts.Count; ++i)
            {
                header_dic_string.Add(packedScripts[i].scriptName, packedScripts[i].itemInfo);
            }

            // 生成新的compressedFile
            string compressedFileName = m_SaveAssetDir + "/" + fileName;
            UnityEngine.Debug.Log("compressedFileName " + compressedFileName);
            using (FileStream fs = new FileStream(compressedFileName, FileMode.Create))
            {
                for (int i = 0; i < packedScripts.Count; ++i)
                {
                    fs.Write(packedScripts[i].compressedByte, 0, packedScripts[i].compressedByte.Length);
                }
            }
        }
        else
        {
            header_file_names.RemoveAt(fileIndex);
        }

        return true;
    }

    private static byte[] GenSkillFileList(string fullPath, bool lz4Compress)
    {
        DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);
        if (dirOutPath.Exists)
        {
            UnityEngine.Debug.LogFormat("{0} exit found", fullPath);
            List<SkillFileName> finalList = BeUtility.GetSkillFileList(fullPath, BeUtility.ESkillFileNameType.Dir);//SkillConfigLoader.GetSkillFiles(dirOutPath);

            if(finalList != null && finalList.Count > 0)
            {
                SkillFileNameList nameList = new SkillFileNameList();
                nameList.m_SkillFileName = finalList;

                TMemoryBufferReaderWriter<SkillFileNameList> memWriter = new TMemoryBufferReaderWriter<SkillFileNameList>(nameList);

                return memWriter.SerializeTo(lz4Compress);
            }
        }

        UnityEngine.Debug.LogErrorFormat("{0} not found", fullPath);

        return null;
    }

    /// <summary>
    /// 将dirPath下scriptType类型的小文件合并成一个二进制文件fileName
    /// </summary>
    private static bool PackScript<T>(PackScriptInput scriptInput) where T : ScriptableObject, IBinaryable
    {
        string fileName = scriptInput.packFileName;

        string[] atlasGUIDNames = AssetDatabase.FindAssets(scriptInput.resFilter, scriptInput.resPath);
        if (atlasGUIDNames.Length == 0)
            return true;

        bool bNeedExt = true;
        bool bJustName = false; // Fashion只需要名字，不需要后缀和路径

        // Ugly, 动画路径信息是刷在Prefab上的，无后缀。部件信息和帧效果是填表的，需要后缀名。
        //if(typeof(T) == typeof(DAnimationData))
            //bNeedExt = false;

//        if (typeof(T) == typeof(tm.FashionSuit))
//            bJustName = true;
//
        List<string> header_file_names = PackScriptData.Instance.header_file_names;
        Dictionary<string, PackHeaderItemInfo> header_dic_string = PackScriptData.Instance.header_dic_string;

        for(int i = 0; i < header_file_names.Count; ++i)
        {
            if(header_file_names[i] == fileName)
            {
                EditorUtility.DisplayDialog("错误", "二进制文件名重复：" + fileName, "确定");
                return false;
            }
        }

        // 以Resource相对路径存储，避免runtime时拼接字符串
        header_file_names.Add(m_SaveResourceDir + fileName);
        int fileIndex = header_file_names.Count - 1;

        List<PackScriptInfo> packedScripts = new List<PackScriptInfo>(atlasGUIDNames.Length);

        float fProgress = scriptInput.progressStart;
        float progressStep = scriptInput.progressLength / atlasGUIDNames.Length;

        int offset = 0;
        for (int i = 0; i < atlasGUIDNames.Length; ++i)
        {
            string assetName = AssetDatabase.GUIDToAssetPath(atlasGUIDNames[i]);

            // 脚本名称规范：Actor/Player/swordman/animations/attack
            string scriptName = assetName.Replace("Assets/Resources/", null);

            if (!bNeedExt)
                scriptName = Path.ChangeExtension(scriptName, null);

            if(bJustName)
                scriptName = Path.GetFileNameWithoutExtension(scriptName);

            if (!m_PackedScriptNames.Add(scriptName))
            {
                //EditorUtility.DisplayDialog("错误", "文件重复打包：" + scriptName, "确定");
                continue;
            }

            EditorUtility.DisplayProgressBar("", scriptInput.progressPrefix + Path.GetFileName(scriptName), fProgress);
            fProgress += progressStep;

            byte[] rawBuffer = GetScriptableObjectAsBinary<T>(assetName, scriptInput.lz4Compress);

            if (rawBuffer == null)
                continue;

            PackScriptInfo packedInfo = new PackScriptInfo();
            packedInfo.itemInfo = new PackHeaderItemInfo(fileIndex, offset, rawBuffer.Length);
            packedInfo.compressedByte = rawBuffer;
            packedInfo.scriptName = scriptName;

            packedScripts.Add(packedInfo);

            offset += rawBuffer.Length;
        }

        if (packedScripts.Count > 0)
        {
            for (int i = 0; i < packedScripts.Count; ++i)
            {
                header_dic_string.Add(packedScripts[i].scriptName, packedScripts[i].itemInfo);
            }

            // 生成新的compressedFile
            string compressedFileName = m_SaveAssetDir + "/" + fileName;
            using (FileStream fs = new FileStream(compressedFileName, FileMode.Create))
            {
                for (int i = 0; i < packedScripts.Count; ++i)
                {
                    fs.Write(packedScripts[i].compressedByte, 0, packedScripts[i].compressedByte.Length);
                }
            }
        }
        else
        {
            header_file_names.RemoveAt(fileIndex);
        }

        return true;
    }

    private static byte[] GetScriptableObjectAsBinary<T>(string scriptName, bool lz4Compress) 
        where T : ScriptableObject, IBinaryable
    {
        // 加载ScriptableObject，转成IBinaryable
        IBinaryable binaryable = AssetDatabase.LoadAssetAtPath<T>(scriptName) as IBinaryable;
        if (binaryable == null)
            return null;

        // 从IBinaryable中获取可二级制化的IBufferObject对象，然后序列化到byte数组中
        IBufferObject bufferObject = binaryable.ToBufferObject();
        if (bufferObject == null)
            return null;

        TMemoryBufferReaderWriter<IBufferObject> memWriter = new TMemoryBufferReaderWriter<IBufferObject>(bufferObject);

        return memWriter.SerializeTo(lz4Compress);
    }

    /*
        private static int GetFrameIndex(string imageName)
        {
            // sm_body0000_000.png

            int indexPos = imageName.LastIndexOf("_");

            int originIndex;
            int.TryParse(imageName.Substring(indexPos + 1), out originIndex);

            return originIndex;
        }*/


    /*   [MenuItem("[TM工具集]/PackScript/DAnimation LZ4加载测试")]
       public static void ReadDAnimation()
       {
           string[] atlasGUIDNames = AssetDatabase.FindAssets(".asset", new string[] { "Assets/Resources/Actor/Player/swordman/animations" });
           if (atlasGUIDNames.Length == 0)
               return;

           try
           {
               float fProgress = 0;
               for (int i = 0; i < atlasGUIDNames.Length; ++i)
               {
                   string fileName = AssetDatabase.GUIDToAssetPath(atlasGUIDNames[i]);
                   EditorUtility.DisplayProgressBar("", "处理" + Path.GetFileName(fileName), fProgress++ / atlasGUIDNames.Length);

                   using (StreamReader st = new StreamReader(fileName))
                   {
                       byte[] lz4bt = new byte[st.BaseStream.Length];
                       st.BaseStream.Read(lz4bt, 0, lz4bt.Length);

                       //  byte[] uncompressed = PackScriptData.DecodeLZ4(lz4bt, 0, lz4bt.Length, 556);

                       byte[] uncompressed = new byte[1024];
                       int uncompressSize = 0;// LZ4.LZ4Codec.Decode(lz4bt, 0, lz4bt.Length, uncompressed, 0, 1024);

                       MemoryBufferReader y = new MemoryBufferReader(uncompressed);
                       DAnimationData animation = new DAnimationData();

                       y.Read(ref animation.name);
                       y.Read(ref animation.loop);
                       y.Read(ref animation.path);
                   }

               }
           }
           finally
           {
               EditorUtility.ClearProgressBar();
           }
       }*/
}
