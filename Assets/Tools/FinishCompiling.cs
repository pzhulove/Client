#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[InitializeOnLoad]
public class FinishCompiling
{
    const string compilingKey = "Compiling";
    static bool compiling;
    static FinishCompiling()
    {
        compiling = EditorPrefs.GetBool(compilingKey, false);
        EditorApplication.update += Update;
    }
 
    static void Update()
    {
        if(compiling && !EditorApplication.isCompiling)
        {
            Debug.Log(string.Format("Compiling DONE {0}", DateTime.Now));
            compiling = false;
            EditorPrefs.SetBool(compilingKey, false);
        }
        else if (!compiling && EditorApplication.isCompiling)
        {
            Debug.Log(string.Format("Compiling START {0}", DateTime.Now));
            compiling = true;
            EditorPrefs.SetBool(compilingKey, true);
        }
    }
}
#endif