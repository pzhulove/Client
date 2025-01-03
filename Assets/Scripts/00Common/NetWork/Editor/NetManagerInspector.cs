using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Network
{
    [CustomEditor(typeof(NetManager))]
    public class NetManagerInspector : Editor
    {
        bool showSendedMsgList = false;
        Vector2 infoScrollPos;

        bool showReceivedMsgList = false;
        Vector2 infoScrollPos2;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDefaultInspector();

            if (GUILayout.Button("清空消息记录", "minibutton", GUILayout.Width(100)))
            {
                NetManager.instance.recordSendedMsg.Clear();
                NetManager.instance.recordReceivedMsg.Clear();
            }

            showSendedMsgList = EditorGUILayout.Toggle("显示发送的消息", showSendedMsgList);
            if (showSendedMsgList)
            {
                EditorGUILayout.BeginVertical();
                infoScrollPos = EditorGUILayout.BeginScrollView(infoScrollPos, GUILayout.Height(500));

                foreach(var item in NetManager.instance.recordSendedMsg)
                {
                    GUILayout.Label(item);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            showReceivedMsgList = EditorGUILayout.Toggle("显示收到的消息", showReceivedMsgList);
            if (showReceivedMsgList)
            {
                EditorGUILayout.BeginVertical();
                infoScrollPos2 = EditorGUILayout.BeginScrollView(infoScrollPos2, GUILayout.Height(500));

                foreach (var item in NetManager.instance.recordReceivedMsg)
                {
                    GUILayout.Label(item);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

        }
    }
}

