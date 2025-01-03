using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tenmove.Runtime.Unity;
using UnityEngine.Rendering;

[CustomEditor(typeof(PostprocessLayer))]
public class PostprocessLayerInspector : Editor
{
    static PostprocessProfile profile;

    //public override void OnInspectorGUI()
    //{
    //    //PostprocessLayer layer = (PostprocessLayer)target;

    //    //PostprocessProfile oldProfile = profile;
    //    //profile = EditorGUILayout.ObjectField("PostprocessProfile:", profile, typeof(PostprocessProfile), false) as PostprocessProfile;
    //    //if(oldProfile != profile)
    //    //{
    //    //    layer.OverrideSettings(profile);
    //    //}

    //    //if(GUILayout.Button("Reload"))
    //    //{
    //    //    layer.OnDisable();
    //    //    layer.OnEnable();
    //    //}
    //}

}
