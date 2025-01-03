using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using XUPorterJSON;

namespace AssetBundleTool
{
    public static class SmallFunc
    {
        
        [MenuItem("[打包工具]/GetFileMD5")]
        public static void Test()
        {
            UnityEngine.Object[] os = Selection.objects;
            for (int i = 0; i < os.Length; i++)
            {
                string s = FileUtil.GetFileMD5(AssetDatabase.GetAssetPath(os[i]));
                string s1 = FileUtil.GetFileMD5(AssetDatabase.GetAssetPath(os[i]) + ".meta");
                Debug.LogErrorFormat("文件{0}的MD5为   {1}    .meta文件的Md5为{2}", os[i].name, s, s1);
            }
        }
        /// <summary>
        /// 创建shader映射表
        /// </summary>
        public static void BuildShaderMap()
        {
            string[] shaderList = Directory.GetFiles("Assets/Resources/Shader", "*.shader", SearchOption.AllDirectories);
            Hashtable shaderResList = new Hashtable();

            for (int i = 0; i < shaderList.Length; ++i)
            {
                Shader shader = AssetDatabase.LoadAssetAtPath(shaderList[i], typeof(Shader)) as Shader;
                if (null != shader)
                {
                    if (!shaderResList.ContainsKey(shader.name))
                    {
                        shaderResList.Add(shader.name, shaderList[i].Replace("Assets/Resources/", "").Replace('\\', '/'));
                    }
                }
            }
            string shaderListJsonPath = Path.Combine("Assets/Resources/Shader", "ShaderList.json");
            FileStream streamW = new FileStream(shaderListJsonPath, FileMode.Create, FileAccess.Write, FileShare.Write);
            //中文编码
            StreamWriter sw = new StreamWriter(streamW/*, Encoding.GetEncoding(936)*/);

            sw.Write(MiniJSON.jsonEncode(shaderResList));
            streamW.Flush();

            sw.Close();
            streamW.Close();
            Debug.LogError("生成ShaderList.json------------>>>>>>>>>>>>>>>>>>");
            AssetDatabase.ImportAsset(shaderListJsonPath, ImportAssetOptions.Default);
            AssetDatabase.Refresh();
        }

        #region 序列化工具相关
        /// <summary>
        /// 序列化类到xml文档
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="obj">类的对象</param>
        /// <param name="filePath">xml文档路径（包含文件名）</param>
        /// <returns>成功：true，失败：false</returns>
        public static bool CreateXML<T>(T obj, string filePath)
        {
            XmlWriter writer = null;    //声明一个xml编写器
            XmlWriterSettings writerSetting = new XmlWriterSettings //声明编写器设置
            {
                Indent = true,//定义xml格式，自动创建新的行
                Encoding = UTF8Encoding.UTF8,//编码格式
            };

            try
            {
                //创建一个保存数据到xml文档的流
                writer = XmlWriter.Create(filePath, writerSetting);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("创建xml文档失败：{0}", ex.Message));
                return false;
            }

            XmlSerializer xser = new XmlSerializer(typeof(T));  //实例化序列化对象

            try
            {
                xser.Serialize(writer, obj);  //序列化对象到xml文档
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("创建xml文档失败：{0}", ex.Message));
                return false;
            }
            finally
            {
                writer.Close();
            }
            return true;
        }
        /// <summary>
        /// 从 XML 文档中反序列化为对象
        /// </summary>
        /// <param name="filePath">文档路径（包含文档名）</param>
        /// <param name="type">对象的类型</param>
        /// <returns>返回object类型</returns>
        public static object FromXmlString(string filePath, Type type)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            string xmlString = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                try
                {
                    return serializer.Deserialize(stream);
                }
                catch
                {
                    return null;
                }
            }

        }
        #endregion
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        [MenuItem("[打包工具]/GeneratePackageMD5")]
        static public void GenerateCheckList()
        {
            _GenerateCheckListAndCalculateMD5();
        }

        static private void _GenerateCheckListAndCalculateMD5(bool exportCheckList = true)
        {
            string[] allPackages =Directory.GetFiles("Assets/StreamingAssets/AssetBundles", "*.*");
            List<string> allPackagesList = new List<string>(1024);
            for (int i = 0, icnt = allPackages.Length; i < icnt; ++i)
            {
                string curPath = allPackages[i];
                if (curPath.EndsWith(".pck", System.StringComparison.OrdinalIgnoreCase) ||
                    curPath.EndsWith(".pak", System.StringComparison.OrdinalIgnoreCase))
                {
                    allPackagesList.Add(Tenmove.Runtime.Utility.Path.Normalize(curPath));
                }
            }

            allPackagesList.Sort((string str1, string str2) =>
            {
                int str1Len = str1.Length;
                int str2Len = str2.Length;

                int compareLen = Tenmove.Runtime.Utility.Math.Min(str1Len, str2Len);
                for (int i = 0; i < compareLen; ++i)
                {
                    if (str1[i] != str2[i])
                        return str1[i] - str2[i];
                }

                if (str1Len < str2Len)
                    return 0 - str2[compareLen];
                else if (str1Len > str2Len)
                    return str1[compareLen];
                else
                    return 0;
            });

            List<string> allPackageListMD5 = new List<string>(allPackagesList.Count);
            if (exportCheckList)
            {
                using (Stream file = Tenmove.Runtime.Utility.File.OpenWrite("Assets/StreamingAssets/AssetBundles/checkfile.lst", true))
                {
                    StreamWriter sw = new StreamWriter(file);
                    for (int i = 0, icnt = allPackagesList.Count; i < icnt; ++i)
                    {
                        string curPackage = allPackagesList[i];
                        string md5 = FileUtil.GetFileMD5(curPackage);
                        long size = FileUtil.GetFileBytes(curPackage);
                        string fileName = Tenmove.Runtime.Utility.Path.GetFileName(allPackagesList[i]);

                        allPackageListMD5.Add(md5);
                        sw.WriteLine(string.Format("{0},{1},{2}", fileName, md5, size));
                    }

                    sw.Flush();
                    file.Flush();
                }
            }
            else
            {
                for (int i = 0, icnt = allPackagesList.Count; i < icnt; ++i)
                    allPackageListMD5.Add(FileUtil.GetFileMD5(allPackagesList[i]));
            }

            using (Stream stream = Tenmove.Runtime.Utility.Memory.OpenStream(allPackageListMD5.Count * 32))
            {
                byte[] byteBuf = null;
                stream.Seek(0, SeekOrigin.Begin);
                for (int i = 0, icnt = allPackageListMD5.Count; i < icnt; ++i)
                {
                    byteBuf = System.Text.Encoding.ASCII.GetBytes(allPackageListMD5[i]);
                    stream.Write(byteBuf, 0, byteBuf.Length);
                }
                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                Tenmove.Runtime.MD5Verifier verifier = new Tenmove.Runtime.MD5Verifier();
                if (verifier.BeginVerify(stream, 128 * 1024, 0.1f))
                {
                    bool isEnd = false;
                    do
                    {/// 危险需要加入熔断措施 异常捕获退出 或者时间机制
                        isEnd = verifier.EndVerify();
                    }
                    while (!isEnd);
                }

                using (Stream verifacation = Tenmove.Runtime.Utility.File.OpenWrite("Assets/StreamingAssets/AssetBundles/verification.md5", true))
                {
                    StreamWriter sw = new StreamWriter(verifacation);
                    sw.WriteLine(verifier.GetVerifySum());
                    sw.Flush();
                }
            }
        }
    }
}

