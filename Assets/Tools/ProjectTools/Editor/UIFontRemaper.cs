using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UIFontRemaper : EditorWindow
{
    static readonly string UI_RESOURCE_PATH = "Assets/Resources/UI";
    static readonly string UI_RESOURCE_PATH_NEW = "Assets/Resources/UIFlatten";

    static List<UnityEngine.Object> m_GameObject = new List<UnityEngine.Object>();

    [MenuItem("[TM工具集]/ArtTools/批量替换字体文件")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        UIFontRemaper window = (UIFontRemaper)EditorWindow.GetWindow(typeof(UIFontRemaper));
        window.titleContent = new GUIContent("批量替换字体文件");
        window.Show();
    }

    static Font m_SrcFont = null;
    static Font m_DstFont = null;

    void OnGUI()
    {
        GUILayout.BeginVertical();
        m_SrcFont = EditorGUILayout.ObjectField("目标字体", m_SrcFont, typeof(Font)) as Font;
        m_DstFont = EditorGUILayout.ObjectField("==替换成==>", m_DstFont, typeof(Font)) as Font;
        GUILayout.EndVertical();

        if(null == m_SrcFont || null == m_DstFont)
        {
            EditorGUILayout.HelpBox(string.Format("请先选择好被替换的字体和替换的目标字体！"), MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("执行替换"))
            {
                _ReplaceFont();
            }
        }
    }

    private void _ReplaceFont()
    {
        List<GameObject> resLst = new List<GameObject>();
        string[] prefabAll = AssetDatabase.FindAssets("t:prefab", new string[] { UI_RESOURCE_PATH, UI_RESOURCE_PATH_NEW });
        for (int i = 0, icnt = prefabAll.Length; i < icnt; ++i)
        {
            UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabAll[i]));
            if (null != root)
                resLst.Add(root);
        }

        bool dirty = false;
        GameObject[] allObjects = resLst.ToArray();
        for (int j = 0,jcnt = allObjects.Length; j < jcnt; j++)
        {
            GameObject go = allObjects[j];
            if(null == go) continue;


            Text[] components = go.GetComponentsInChildren<Text>();
            for (int i = 0,icnt = components.Length; i < icnt; i++)
            {
                Text cur = components[i];
                if (null == cur) continue;
                
                if (cur.font.GetInstanceID() == m_SrcFont.GetInstanceID())
                {
                    cur.font = m_DstFont;
                    EditorUtility.SetDirty(go);
                    dirty = true;
                }
            }
        }

        if (dirty) AssetDatabase.SaveAssets();
    }

    private static void _FindReferencesOfComponent(UnityEngine.Object to,ref List<UnityEngine.Object> resObject)
    {
        var allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        for (int j = 0; j < allObjects.Length; j++)
        {
            var go = allObjects[j];

            if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
            {
                if (PrefabUtility.GetPrefabParent(go) == to)
                {
                    UnityEngine.Debug.Log(string.Format("referenced by {0}, {1}", go.name, go.GetType()), go);
                    resObject.Add(go);
                }
            }

            var components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                var c = components[i];
                if (!c) continue;

                var so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.Next(true))
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == to)
                        {
                            UnityEngine.Debug.Log(string.Format("referenced by {0}, {1}", c.name, c.GetType()), c);
                            resObject.Add(c.gameObject);
                        }
                    }
            }
        }
    }
}
