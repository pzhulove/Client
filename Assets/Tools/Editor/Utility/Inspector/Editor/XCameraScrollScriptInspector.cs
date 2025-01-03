using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(XCameraScrollScript))]
[CanEditMultipleObjects]
public class XCameraScrollScriptInspector : Editor {

    public override void OnInspectorGUI()
    {
        XCameraScrollScript scrollScript = this.target as XCameraScrollScript;
        string ErrorMessage = "";

        GameObject obj = (GameObject)EditorGUILayout.ObjectField(scrollScript.CameraObject, typeof(GameObject),true);

        if(obj && obj != scrollScript.CameraObject)
        {
            scrollScript.CameraObject = obj;

            foreach (var v in this.targets)
            {
                XCameraScrollScript  s = v as XCameraScrollScript;
                s.CameraObject = obj;
                s.Init();
            }
        }

        float speed = EditorGUILayout.FloatField("速度：", scrollScript.fSpeed);
        
        if (speed != scrollScript.fSpeed)
        {
            scrollScript.fSpeed = speed;

            foreach (var v in this.targets)
            {
                XCameraScrollScript s = v as XCameraScrollScript;
                s.fSpeed = scrollScript.fSpeed;
            }
        }
       

        float offset = EditorGUILayout.FloatField("偏移：", scrollScript.fOffset);

        if( offset != scrollScript.fOffset)
        {
            scrollScript.fOffset = offset;

            foreach (var v in this.targets)
            {
                XCameraScrollScript s = v as XCameraScrollScript;
                s.fOffset = scrollScript.fOffset;
            }
        }


        EditorGUILayout.BeginHorizontal();
        if(scrollScript.bPauseScroll)
        {
            if (GUILayout.Button("开启滚动", "miniButton"))
            {
                foreach( var v in this.targets)
                {
                    XCameraScrollScript s = v as XCameraScrollScript;
                    s.bPauseScroll = false;
                    s.UpdateForce();
                }
            }
        }
        else
        {
            if (GUILayout.Button("暂停滚动", "miniButton"))
            {
                foreach (var v in this.targets)
                {
                    XCameraScrollScript s = v as XCameraScrollScript;
                    s.bPauseScroll = true;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
