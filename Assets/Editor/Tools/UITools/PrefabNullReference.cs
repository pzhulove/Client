using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class PrefabNullReference
{
    
    //[MenuItem("[UITest]/检查所有预制体，是否有资源丢失", false, 50)]
    public static void FindAllMissionPrefab()
    {
        string[] allPrefabsGUIDs = AssetDatabase.FindAssets("t:prefab");

        EditorUtility.ClearProgressBar();

        for (int i = 0; i < allPrefabsGUIDs.Length; ++i)
        {
            string resPath = AssetDatabase.GUIDToAssetPath(allPrefabsGUIDs[i]);

            GameObject prefabGo = AssetDatabase.LoadAssetAtPath<GameObject>(resPath);

            EditorUtility.DisplayProgressBar("FindNull", System.IO.Path.GetFileName(resPath), i * 1.0f / allPrefabsGUIDs.Length);

            FindMissingReferences(resPath, new GameObject[] { prefabGo });
        }

        EditorUtility.ClearProgressBar();
    }

    private static void FindMissingReferences(string context, GameObject[] objects)
    {
        foreach (var go in objects)
        {
            var components = go.GetComponents<Component>();
             
            foreach (var c in components)
            {
                if (!c)
                {
                    Debug.LogError(context + "组件 在预制体内丢失: " + FullPath(go), go);
                    continue;
                }
                 
                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();
                 
                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == null
                            && sp.objectReferenceInstanceIDValue != 0)
                        {
                            ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
                        }
                    }
                }
            }
        }
    }

    private const string err = "资源引用丢失: [{3}]{0}. 组件: {1}, 字段: {2}";
     
    private static void ShowError (string context, GameObject go, string c, string property)
    {
        Debug.Log(string.Format(err, FullPath(go), c, property, context), go);
    }
     
    private static string FullPath(GameObject go)
    {
        return go.transform.parent == null
            ? go.name
                : FullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}
