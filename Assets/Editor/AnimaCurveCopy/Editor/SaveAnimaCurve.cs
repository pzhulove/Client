using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;
public class SaveAnimaCurve : Editor
{
    [MenuItem("Assets/特效动作/提取特效动作")]
    static void SaveAnimaCurves()
    {
        string targetPath = Application.dataPath + "/AnimaCurve";
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        else
        {
            Directory.Delete(targetPath, true);
            Directory.CreateDirectory(targetPath);
        }
        Object obj = Selection.activeObject;
        if (obj != null && obj is AnimationClip)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj).Replace("\\", "/");
            string[] nameSplits = sourcePath.Split('/');
            string targetName = "";
            int i = nameSplits.Length > 5 ? nameSplits.Length - 5 : 0;
            for (; i < nameSplits.Length; i++)
            {
                if (i != nameSplits.Length - 1)
                    targetName += nameSplits[i] + "_";
                else
                    targetName += nameSplits[i];
            }
            string targetCurvePath = targetPath + "/" + targetName;
            File.Copy(sourcePath, targetCurvePath);
            Debug.LogErrorFormat("保存AnimaClip Curve 成功 {0}", targetCurvePath.Replace("\\", "/"));
        }
        AssetDatabase.Refresh();
    }
}
