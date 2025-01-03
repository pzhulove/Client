#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using ProtoTable;
using GameClient;

/// <summary>
/// 批量拷贝文件
/// </summary>
public class TMCopyFile
{
    private static string _resourceRootPath;    //资源目录根路径
    private static string _imageRootPath;   //技能图标跟路径

    [UnityEditor.MenuItem("[TM工具集]/[战斗优化]/拷贝职业技能Icon图标", false, 1011)]
    public static void CopyProfessionSkillIcon()
    {
        InitRootPath();
        var skillTableDic = TableManagerEditor.instance.GetTable<SkillTable>();
        if (skillTableDic == null)
            return;
        int index = 0;
        var enumerator = skillTableDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            var table = current.Value as SkillTable;
            if (table == null)
                continue;
            if (table.SkillType != SkillTable.eSkillType.ACTIVE)
                continue;
            if (table.JobID.Count == 1 && table.JobID[0] == 0)
                continue;
            for (int i = 0; i < table.JobID.Count; i++)
            {
                CopyFile(table.Icon, table.JobID[i]);
            }
            EditorUtility.DisplayProgressBar("拷贝职业图标,当前技能:", string.Format("{0} {1}/{2}", table.Name, index, skillTableDic.Count), (index / (float)skillTableDic.Count));
            index++;
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 初始化资源根目录
    /// </summary>
    private static void InitRootPath()
    {
        _resourceRootPath = Application.dataPath + "/Resources/";
        _imageRootPath = _resourceRootPath + "UI/Image/Icon/Icon_Skill/";
    }

    /// <summary>
    /// 获取职业对应的枚举名
    /// </summary>
    private static string GetOccuCopyFileName(ActorOccupation Occupation)
    {
        var name = Enum.GetName(typeof(ActorOccupation), Occupation);
        if (string.IsNullOrEmpty(name))
            return null;
        return name + "/";
    }

    /// <summary>
    /// 获取技能Icon对应的文件路径
    /// </summary>
    private static string GetIconTextureName(string path)
    {
        string[] pathArr = path.Split(':');
        return pathArr[1] + ".png";
    }

    /// <summary>
    /// 拷贝文件函数
    /// </summary>
    private static void CopyFile(string iconPath, int occId)
    {
        if (string.IsNullOrEmpty(iconPath) || iconPath.Equals("-"))
            return;
        string iconTextureName = GetIconTextureName(iconPath);
        if (string.IsNullOrEmpty(iconTextureName))
        {
            Logger.LogErrorFormat("path is null; iconPath:{0}", iconPath);
            return;
        }
        string originPath = Path.Combine(_imageRootPath, iconTextureName);
        if (!File.Exists(originPath))
        {
            Logger.LogErrorFormat("can not find;  path:{0}", originPath);
            return;
        }
        var name = GetOccuCopyFileName((ActorOccupation)occId);
        if (string.IsNullOrEmpty(name))
        {
            Logger.LogErrorFormat("not find occu Id:{0} iconPath:{1}", occId, iconPath);
            return;
        }   
        string copyRootPath = _resourceRootPath + "../../CopyRoot/Battle_OccuSkillIcon_" + name;
        CheckPath(copyRootPath);
        string copyFilePath = Path.Combine(copyRootPath, iconTextureName);
        RealCopyFile(originPath, copyFilePath);
    }

    /// <summary>
    /// 真正的拷贝文件
    /// </summary>
    /// <param name="originPath">原始路径</param>
    /// <param name="targetPath">目标路径</param>
    private static void RealCopyFile(string originPath, string targetPath)
    {
        try
        {
            File.Copy(originPath, targetPath, true);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("error:{0}", e.ToString());
        }
    }

    /// <summary>
    /// 检测文件夹 如果不存在则新建文件夹
    /// </summary>
    private static void CheckPath(string path)
    {
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("CheckPath {0} failed {1}", path, e.Message);
            }
        }
    }
}
#endif