using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/*
public class AssetProcessorUtility
{
    public static string configSavePath = Application.dataPath + @"\Editor\AssetProcessor\Config\";

    public enum AssetType
    {
        Texture,
        Mesh,
        Audio,
        Animation
    }

    public static void Serialize(System.Object item,string path)
    {
        XmlSerializer serializer = new XmlSerializer(item.GetType());
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer.BaseStream, item);
        writer.Close();
    }

    public static T Deserialize<T>(string path) 
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader reader = new StreamReader(path);
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }

 
    public static string GetStrategySavePath(string StrategyName,AssetType assetType)
    {
        switch(assetType)
        {
            case AssetType.Texture:
                return configSavePath + "Tex_" + StrategyName + ".xml"; 
            case AssetType.Mesh:
                return configSavePath + "Tex_" + StrategyName + ".xml";
            case AssetType.Audio:
                return configSavePath + "Tex_" + StrategyName + ".xml";
            case AssetType.Animation:
                return configSavePath + "Tex_" + StrategyName + ".xml";
            default:
                return String.Empty;
        }
    }

    public static List<string> GetAllStrategyFileName(AssetType assetType)
    {
        List<string> result = new List<string>();
        DirectoryInfo folder = new DirectoryInfo(configSavePath);
        switch (assetType)
        {
            case AssetType.Texture:
                foreach (var file in folder.GetFiles("*.xml"))
                {
                    if (file.Name.Length < 3)
                        break;

                    if(file.Name.Substring(0,3).Equals("Tex"))
                    {
                        result.Add(file.FullName);
                    }
                }
                break;
            case AssetType.Mesh:
                foreach (var file in folder.GetFiles("*.xml"))
                {
                    if (file.Name.Length < 4)
                        break;

                    if (file.Name.Substring(0, 4).Equals("Mesh"))
                    {
                        result.Add(file.FullName);
                    }
                }
                break;
            case AssetType.Audio:
                foreach (var file in folder.GetFiles("*.xml"))
                {
                    if (file.Name.Length < 5)
                        break;

                    if (file.Name.Substring(0, 5).Equals("Audio"))
                    {
                        result.Add(file.FullName);
                    }
                }
                break;
            case AssetType.Animation:
                foreach (var file in folder.GetFiles("*.xml"))
                {
                    if (file.Name.Length < 4)
                        break;

                    if (file.Name.Substring(0, 4).Equals("Anim"))
                    {
                        result.Add(file.FullName);
                    }
                }
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// 删除对应策略文件
    /// </summary>
    /// <param 策略名="strategyName"></param>
    /// <param 类型="assetType"></param>
    public static void DeleteStrategyFileByName(string strategyName,AssetType assetType)
    {
        var filePath = GetStrategySavePath(strategyName, assetType);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
        var fileMetaPath = filePath + ".meta";
        if (File.Exists(fileMetaPath))
        {
            try
            {
                File.Delete(fileMetaPath);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
*/
