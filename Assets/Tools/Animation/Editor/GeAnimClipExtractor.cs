using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class GeAnimClipExtractor : Editor
{
    /// 替换掉所有的AlphaTest材质
    static protected readonly string SCENE_RES_PATH = "Assets/Resources/Actor";

    [MenuItem("Assets/角色工具/40、Extract Animation Clip", false, 40)]
    static public void ExtractAnimtionClip()
    {
        //string[] prefabList = Directory.GetFiles(SCENE_RES_PATH, "*.prefab", SearchOption.TopDirectoryOnly);
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        
        for (int i = 0; i < selection.Length; ++i)
        {
            GameObject curPrefab = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == curPrefab)
                continue;

            string curPrefabName = curPrefab.name;

            string animClipPath = AssetDatabase.GetAssetPath(curPrefab);
            animClipPath = Path.GetDirectoryName(animClipPath);

            Animation curAnimation = curPrefab.GetComponent<Animation>();
            int icnt = 0;
            int total = curAnimation.GetClipCount();
            foreach (AnimationState state in curAnimation)
            {
                EditorUtility.DisplayProgressBar("抽取动画资源", "正在抽取第" + icnt + "个动画资源...（共"+ total + "个）", ((float)icnt / total));

                AnimationClip newClip = new AnimationClip();
                EditorUtility.CopySerialized(state.clip, newClip);
                AssetDatabase.CreateAsset(newClip, Path.Combine(animClipPath, state.name + ".anim"));
                EditorUtility.SetDirty(newClip);
                ++icnt;
            }

            Editor.DestroyImmediate(curPrefab, false);
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
    }
}
