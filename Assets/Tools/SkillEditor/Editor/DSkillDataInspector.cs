using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
 

[CustomEditor(typeof(DSkillData))]
public class DSkillDataInspector : Editor
{
    public static string GetNewAssetPath(string assetName)
    {
        //string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //if (path == "")
        //{
        //    path = "Assets";
        //}
        //else if (System.IO.Path.GetExtension(path) != "")
        //{
        //    Debug.Log(path);
        //    path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        //}

        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
                break;
            }
        }

        string assetPathAndName = System.IO.Path.Combine(path, string.Format("New{0}.asset", assetName));
        string uniqueAssetPathAndName = AssetDatabase.GenerateUniqueAssetPath(assetPathAndName);
        return uniqueAssetPathAndName;
    }
  

    

    [MenuItem("[技能编辑器]/CreateSkill",false,1)]
    public static void CreateAsset()
    {
       DSkillDataEditorWindow.CreateAsset<DSkillData>();
    }

    [MenuItem("[技能编辑器]/EditorSkill",false,1)]
    public static void AssetEditor()
    {
        DSkillDataEditorWindow.Init();
    }
      
    [MenuItem("[技能编辑器]/GizmoShow",false,10)]
    static void ToggleGizmosShow()
    {
        DSkillDataEditorWindow.ShowGizmo(true);
    }

    [MenuItem("[技能编辑器]/GizmoShow",true)]
    static bool ValidateGizmosShow()
    {
        return Tools.hidden;
    }
      
    [MenuItem("[技能编辑器]/GizmoHide1",false,10)]
    static void ToggleGizmosHide()
    {
        DSkillDataEditorWindow.ShowGizmo(false);
    }

    [MenuItem("[技能编辑器]/GizmoHide1", true)]
    static bool ValidateGizmosHide()
    {
        return !Tools.hidden;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Editor"))
            DSkillDataEditorWindow.Init();
    }
}