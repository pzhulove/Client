using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace GameClient
{
    [CustomEditor(typeof(ComEffectTransparentSolve))]
    class ComEffectTransparentSolveEditor : Editor
    {
        protected SerializedObject _serializedObject;

        public void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button(new GUIContent("EditorValue")))
            {
                ClientFrameClassWindow window = EditorWindow.GetWindow<ClientFrameClassWindow>(false,"All Classes Of ClientFrame", true);
                if(null != window)
                {
                    window.Initialize(this);
                    window.Show();
                }
            }

            GUILayout.BeginVertical("GroupBox");
            var solve = target as ComEffectTransparentSolve;
            if (solve.types.Length > 0)
            {
                GUILayout.BeginVertical("GroupBox");
                GUI.color = Color.cyan;
                GUILayout.Label("Already Selected Frames:");
                GUILayout.EndVertical();
            }
            else
            {
                GUI.color = Color.yellow;
                GUILayout.Label("Has No Selected Frames !!!");
            }

            GUI.color = Color.cyan;
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < solve.types.Length; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(solve.types[i]);
                GUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel -= 1;
            GUILayout.EndVertical();
            GUI.color = Color.white;

            if(EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }

    struct ClientFrameValue
    {
        public Type type;
        public bool bToggle;
    }

    class ClientFrameClassWindow : EditorWindow
    {
        ComEffectTransparentSolveEditor mTarget;
        ClientFrameValue[] mValues = new ClientFrameValue[0];
        public void Initialize(ComEffectTransparentSolveEditor target)
        {
            mTarget = target;
            ComEffectTransparentSolve solve = mTarget.target as ComEffectTransparentSolve;
            if(0 == mValues.Length)
            {
               var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IClientFrame)))) .ToArray();
                mValues = new ClientFrameValue[types.Length];
                for (int i = 0; i < types.Length; ++i)
                {
                    mValues[i].type = types[i];
                    mValues[i].bToggle = false;
                    for (int j = 0; j < solve.types.Length; ++j)
                    {
                        if(solve.types[j] == types[i].Name)
                        {
                            mValues[i].bToggle = true;
                            break;
                        }
                    }
                }
            }
        }

        Vector2 scrollPos;
        Vector2 scrollPos2;
        string mSearchText = string.Empty;

        void OnGUI()
        {
            GUI.color = Color.gray;
            GUILayout.BeginVertical("GroupBox");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
            mSearchText = GUILayout.TextField(mSearchText);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel += 1;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.Height(180));
            int iIndex = 0;
            for (int i = 0; i < mValues.Length; ++i)
            {
                if(mValues[i].bToggle)
                {
                    continue;
                }

                if ((!string.IsNullOrEmpty(mSearchText)) && (!mValues[i].type.Name.StartsWith(mSearchText,StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }

                if (1 == (iIndex & 1))
                {
                    GUI.color = Color.yellow;
                }
                else
                {
                    GUI.color = Color.green;
                }
                EditorGUILayout.BeginHorizontal();
                mValues[i].bToggle = EditorGUILayout.Toggle(mValues[i].bToggle);
                EditorGUILayout.LabelField(mValues[i].type.Name);
                EditorGUILayout.EndHorizontal();
                ++iIndex;
            }
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel -= 1;
            GUILayout.EndVertical();

            GUI.color = Color.gray;
            GUILayout.BeginVertical("GroupBox");
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);

            bool bHasSelectedFrame = false;
            for (int i = 0; i < mValues.Length; ++i)
            {
                if(mValues[i].bToggle)
                {
                    bHasSelectedFrame = true;
                    break;
                }
            }
            if(bHasSelectedFrame)
            {
                GUI.color = Color.cyan;
                GUILayout.Label("Already Selected Frames:");
            }
            else
            {
                GUI.color = Color.yellow;
                GUILayout.Label("Has No Selected Frames !!!");
            }

            EditorGUI.indentLevel += 1;
            iIndex = 0;
            for (int i = 0; i < mValues.Length; ++i)
            {
                if(!mValues[i].bToggle)
                {
                    continue;
                }

                if (1 == (iIndex & 1))
                {
                    GUI.color = Color.yellow;
                }
                else
                {
                    GUI.color = Color.green;
                }
                EditorGUILayout.BeginHorizontal();
                mValues[i].bToggle = EditorGUILayout.Toggle(mValues[i].bToggle);
                EditorGUILayout.LabelField(mValues[i].type.Name);
                EditorGUILayout.EndHorizontal();
                ++iIndex;
            }
            EditorGUI.indentLevel -= 1;
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUI.color = Color.white;
        }

        void OnDestroy()
        {
            List<string> types = new List<string>();
            for(int i = 0; i < mValues.Length; ++i)
            {
                if(mValues[i].bToggle)
                {
                    types.Add(mValues[i].type.Name);
                }
            }
            ComEffectTransparentSolve solve = mTarget.target as ComEffectTransparentSolve;
            if(null != solve)
            {
                solve.types = types.ToArray();
            }
        }
    }
}