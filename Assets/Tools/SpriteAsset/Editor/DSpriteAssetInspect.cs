using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DSpriteAsset))]
public class DSpriteAssetInspector : EditorWindow
{
    [MenuItem("[TM工具集]/[UI分析]/打开窗口")]
    public static void Open()
    {
        DSpriteAssetInspector window = (DSpriteAssetInspector)EditorWindow.GetWindow( typeof(DSpriteAssetInspector) );
        window.titleContent = new GUIContent("残留");
        window.Show();
    }

    private DSpriteAsset mData;
    private Vector2      mScrollPos;

    public void OnGUI()
    {
        DSpriteAsset data = Selection.activeObject as DSpriteAsset;
        if (data != mData && null != data)
        {
            mData = data;
        }

        if (null == mData)
        {
            return ;
        }

        EditorGUILayout.BeginVertical();
        mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
        {
            var tbrect = GUILayoutUtility.GetRect(20, 20);
            if (GUI.Button(tbrect, "定位", "minibutton"))
            {
                Selection.objects = new UnityEngine.Object[] {(UnityEngine.Object)mData};
            }
        }

        for (int i = 0; i < mData.refs.Count; ++i)
        {
            DSpriteNode node = mData.refs[i];

            EditorGUILayout.BeginHorizontal();
            {
                var rect = GUILayoutUtility.GetRect(50, 50);
                EditorGUI.ObjectField(rect, "", node.sprite, typeof(Sprite));
            }

            if (DSpriteAssetTools.Contains(node.sprite))
            {
                EditorGUILayout.HelpBox("已经保存", MessageType.Info);
            }
            else
            {
                var brect = GUILayoutUtility.GetRect(20, 20);
                if (GUI.Button(brect, "存到", "minibutton"))
                {
                    DSpriteAssetTools.Add(node.sprite);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            for (int j = 0; j < node.refsPrefabs.Count; ++j)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(node.refsPrefabs[j], typeof(GameObject));

                {
                    var brect = GUILayoutUtility.GetRect(20, 20);

                    if (GUI.Button(brect, "开", "minibutton"))
                    {
                        Selection.activeGameObject = node.refsPrefabs[j];
                        GameObject obj = HeroGo.UtilityTools.PreviewPrefab();
						CollectDependencies.CollectDependenciesExample.FindReferencesTo(node.sprite, obj);
                    }
                }

                

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    void OnInspectorUpdate( )
    {
        Repaint( );
    }
}

