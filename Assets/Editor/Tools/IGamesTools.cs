using UnityEngine;
using System.Collections;
using UnityEditor;
public class IGamesTools : Editor
{
    [MenuItem("Assets/spine导入转换")]
    private static void CreateSpineObject()
    {
        BuildAnimation.CreateSpineObject();
    }

    [MenuItem("Assets/spine生成UI预制体")]
    private static void CreateSpineUIObject()
    {
        BuildAnimation.CreateSpineUIObject();
    }

}
