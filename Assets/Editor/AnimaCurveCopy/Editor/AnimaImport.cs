using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
public class AnimaImport : AssetPostprocessor
{
    private static string[] exceptStr = new string[] { "Bone", "Dummy", "Bip" };
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        string targetPath = Application.dataPath + "/AnimaCurve";
        List<string> list = new List<string>(importedAssets);
        list.AddRange(movedAssets);
        foreach (string assetPath in list)
        {
            if (Path.GetExtension(assetPath).Equals(".anim"))
            {
                string sourcePath = assetPath.Replace("\\", "/");
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
                targetPath = targetPath + "/" + targetName;
                if (File.Exists(targetPath))
                {
                    string rootPath = Path.GetFullPath("./").Replace("\\", "/");
                    targetPath = targetPath.Replace(rootPath, "");
                    //Debug.LogError(targetPath);
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(targetPath);
                    List<EditorCurveBinding> target_curve_binding = new List<EditorCurveBinding>();
                    foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                    {
                        //Debug.LogError(binding.propertyName + "          " + binding.path + "    " + binding.type.ToString());
                        string name = binding.path.Substring(binding.path.LastIndexOf('/') + 1);
                        bool isContain = false;
                        for (int j = 0; j < exceptStr.Length; j++)
                        {
                            if (name.IndexOf(exceptStr[j]) == 0)
                            {
                                isContain = true;
                                continue;
                            }
                        }
                        if (!isContain)
                        {
                            //Debug.LogError(binding.path);
                            target_curve_binding.Add(binding);
                        }
                    }
                    AnimationClip clip1 = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                    for (int k = 0; k < target_curve_binding.Count; k++)
                    {
                        if (target_curve_binding[k].path != null)
                        {
                            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, target_curve_binding[k]);
                            EditorCurveBinding curveBinding = new EditorCurveBinding();
                            curveBinding.path = target_curve_binding[k].path;
                            curveBinding.type = target_curve_binding[k].type;
                            curveBinding.propertyName = target_curve_binding[k].propertyName;
                            AnimationUtility.SetEditorCurve(clip1, curveBinding, curve);
                        }
                    }
                    EditorUtility.SetDirty(clip1);
                    Debug.LogErrorFormat("拷贝Anima Curve 到{0}成功", assetPath);
                    if (Directory.Exists(Application.dataPath + "/AnimaCurve"))
                    {
                        Directory.Delete(Application.dataPath + "/AnimaCurve", true);
                    }
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}

