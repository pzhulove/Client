using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

//[InitializeOnLoad]
public class ChangeFontWindow : EditorWindow
{
    static ChangeFontWindow()
    {
        //toChangeFont = new Font("Arial");
        //toChangeFontStyle = FontStyle.Normal;
    }

    [MenuItem("Window/Change Font")]
    private static void ShowWindow()
    {
        ChangeFontWindow cw = EditorWindow.GetWindow<ChangeFontWindow>(true, "Window/Change Font");

    }

    static Font dstFont = new Font("MicrosoftYaHeiGB");
    static Font dstBlodFont = new Font("MicrosoftYaHeiBold");
    
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("目标字体:");
        dstFont = (Font)EditorGUILayout.ObjectField(dstFont, typeof(Font), true, GUILayout.MinWidth(100f));
        GUILayout.Label("目标粗体字体:");
        dstBlodFont = (Font)EditorGUILayout.ObjectField(dstBlodFont, typeof(Font), true, GUILayout.MinWidth(100f));

        GUILayout.Space(10);
        
        if (GUILayout.Button("修改字体！"))
        {
            Change();
        }
    }
    public static void Change()
    {
        //         //获取所有UILabel组件
        //         if (Selection.objects == null || Selection.objects.Length == 0) return;
        //         //如果是UGUI讲UILabel换成Text就可以
        //         Object[] labels = Selection.GetFiltered(typeof(Text), SelectionMode.Deep);
        //         foreach (Object item in labels)
        //         {
        //             //如果是UGUI讲UILabel换成Text就可以
        //             Text label = (Text)item;
        //             label.font = toChangeFont;
        //             //label.font = toChangeFont;（UGUI）
        //             Debug.Log(item.name + ":" + label.text);
        //             //
        //             EditorUtility.SetDirty(item);//重要
        //         }

        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UI" });

        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            UnityEngine.Debug.LogWarning("Process Prefab :" + temp.name + "....\n");
            bool bChange = false;
            List<Text> coms = new List<Text>();
            FileTools.GetComponetsInChildrenWithHide<Text>(temp, ref coms);

            foreach (Text PText in coms)
            {
                FontStyle originStyle = PText.fontStyle;

                //                 if (PText.font.name == "GameFontYaHei #3")
                //                 {
                //                     if (null != dstFont)

                //                     {

                //                         PText.font = dstFont;

                //                         PText.fontStyle = originStyle;

                //                     }
                //                 }
                //                 else if (PText.font.name == "yaheibold")

                //                 {
                //                     if (null != dstBlodFont)

                //                     {

                //                         PText.font = dstBlodFont;

                //                         PText.fontStyle = FontStyle.Normal;

                //                     }

                //                 }

                if (PText.font.name == "yaheibold")
                {
                    switch(originStyle)
                    {
                        case FontStyle.BoldAndItalic:
                            PText.fontStyle = FontStyle.Italic;
                            break;
                        case FontStyle.Bold:
                            PText.fontStyle = FontStyle.Normal;
                            break;
                        default:
                            PText.fontStyle = FontStyle.Normal;
                            break;
                    }
                }
                EditorUtility.SetDirty(temp);
            }
            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
        }
    }
}