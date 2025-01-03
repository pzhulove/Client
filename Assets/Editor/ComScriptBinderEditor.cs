using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameClient;
using System.Reflection;
using System;
using System.Linq;
using System.Text;

[CustomEditor(typeof(ComScriptBinder))]
public class ComScriptBinderEditor : Editor
{
    protected SerializedProperty components = null;
    protected SerializedProperty labelSpace = null;
    protected SerializedProperty scriptStatus = null;
    protected string mInitializeCode = string.Empty;
    protected List<string> mInitializeCodeGUI = new List<string>();

    protected string getInitializeCode()
    {
        string ret = string.Empty;
        StringBuilder stringBuilder = StringBuilderCache.Acquire();
        for(int i = 0; i < mInitializeCodeGUI.Count; ++i)
        {
            stringBuilder.Append(mInitializeCodeGUI[i]);
            if(i != mInitializeCodeGUI.Count - 1)
            {
                stringBuilder.Append("\r\n");
            }
        }
        ret = stringBuilder.ToString();
        StringBuilderCache.Release(stringBuilder);
        return ret;
    }

    public void OnEnable()
    {
        components = serializedObject.FindProperty("scriptItems");
        labelSpace = serializedObject.FindProperty("labelSpace");
        scriptStatus = serializedObject.FindProperty("scriptStatus");
        createInitializeCodes();
    }

    void _menuFunction(object value)
    {
        var argv = value as object[];
        if(null != argv && argv.Length == 2)
        {
            try
            {
                ScriptBinderItem component = argv[1] as ScriptBinderItem;
                if(null != component)
                {
                    component.component = argv[0] as UnityEngine.Object;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogErrorFormat(ex.ToString());
            }
        }
    }

    void _enumSelect(object value)
    {
        var argv = value as object[];
        if(null != argv)
        {
            ComScriptBinder script = argv[0] as ComScriptBinder;
            if(null != script)
            {
                script.labelSpace = argv[1] as string;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.color = new Color32(0x6a,0xff,0x5e,0xFF);
        GUILayout.BeginVertical("GroupBox");

        GUI.color = Color.gray;
        GUILayout.BeginVertical("GroupBox");
        GUI.color = Color.magenta;
        EditorGUILayout.LabelField("ClientScriptBinder(神器)", GUILayout.MinWidth(100));
        GUI.color = Color.red;
        EditorGUILayout.LabelField("May I Pay Your Attention Please ?");
        EditorGUILayout.LabelField("1:Add your class name to GameClient.ScriptLabelDeclare.declares !");
        EditorGUILayout.LabelField("2:Add your label to GameClient.ComScriptLabel !");
        EditorGUILayout.LabelField("3:Label must have a fixedValue !");
        EditorGUILayout.LabelField("4:Label must have a unique value in the same prefixed class name!");
        GUILayout.EndVertical();

        GUI.color = Color.gray;
        GUILayout.BeginVertical("GroupBox");
        GUI.color = Color.white;
        base.OnInspectorGUI();
        GUILayout.EndVertical();

        EditorGUI.BeginChangeCheck();

        OnScriptItemGUI();
        OnInitializedCodeGUI();
        OnScriptStatusGUI();

        GUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            createInitializeCodes();
            serializedObject.ApplyModifiedProperties();
        }
    }

    protected void OnScriptItemGUI()
    {
        GUI.color = Color.gray;
        GUILayout.BeginVertical("GroupBox");
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        EditorGUILayout.LabelField(labelSpace.stringValue, GUILayout.MinWidth(60));
        GUI.color = Color.white;

        if (GUILayout.Button("SelectFrame", "GV Gizmo DropDown", GUILayout.MinWidth(100)))
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < GameClient.ScriptLabelDeclare.declares.Length; ++i)
            {
                var name = GameClient.ScriptLabelDeclare.declares[i];
                menu.AddItem(new GUIContent(name), (target as ComScriptBinder).labelSpace.Equals(name), _enumSelect, new object[] { (target as ComScriptBinder), name });
            }
            menu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();

        for (int i = 0; i < components.arraySize; ++i)
        {
            var scriptBindItem = components.GetArrayElementAtIndex(i);
            if (null != scriptBindItem)
            {
                GUI.color = Color.gray;
                GUILayout.BeginVertical("GroupBox");

                //ScriptBinderItem
                SerializedProperty bindIndex = scriptBindItem.FindPropertyRelative("bindIndex");
                SerializedProperty component = scriptBindItem.FindPropertyRelative("component");
                SerializedProperty varName = scriptBindItem.FindPropertyRelative("varName");
                SerializedProperty locked = scriptBindItem.FindPropertyRelative("locked");
                ScriptBinderItem scriptItem = null;
                if (i < (target as ComScriptBinder).scriptItems.Length)
                {
                    scriptItem = (target as ComScriptBinder).scriptItems[i];
                }

                if (true)
                {
                    string labelFixed = "Label_" + labelSpace.stringValue + "_";
                    var labels = System.Enum.GetValues(typeof(ComScriptLabel));
                    var labelNames = System.Enum.GetNames(typeof(ComScriptLabel));
                    List<string> displayedOptions = new List<string>();
                    List<int> optionValues = new List<int>();
                    for (int j = 0; j < labelNames.Length; ++j)
                    {
                        var label = (ComScriptLabel)labels.GetValue(j);
                        var name = labelNames.GetValue(j) as string;
                        if (name.StartsWith(labelFixed) && labelFixed.Length < name.Length)
                        {
                            displayedOptions.Add(name.Substring(labelFixed.Length,name.Length - labelFixed.Length));
                            optionValues.Add((int)label);
                        }
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUI.color = Color.white;

                    locked.boolValue = EditorGUILayout.Toggle(locked.boolValue);
                    GUI.enabled = locked.boolValue;
                    varName.stringValue = EditorGUILayout.TextField(varName.stringValue);
                    GUI.enabled = true;

                    var labelValue = string.Empty;
                    if(displayedOptions.Count > bindIndex.intValue && bindIndex.intValue >= 0)
                    {
                        GUI.color = Color.green;
                        labelValue = displayedOptions[bindIndex.intValue];
                    }
                    else
                    {
                        GUI.color = Color.yellow;
                        labelValue = "Please Select One Label !";
                    }
                    EditorGUILayout.LabelField(labelValue, GUILayout.MinWidth(60));

                    GUI.color = Color.white;
                    bindIndex.intValue = EditorGUILayout.IntPopup(bindIndex.intValue, displayedOptions.ToArray(), optionValues.ToArray(),GUILayout.MinWidth(100));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                component.objectReferenceValue = EditorGUILayout.ObjectField(component.objectReferenceValue, typeof(UnityEngine.Object), true) as UnityEngine.Object;
                if (null != component.objectReferenceValue)
                {
                    GameObject gameObject = component.objectReferenceValue as GameObject;
                    if (null == gameObject)
                    {
                        if (component.objectReferenceValue as Component)
                        {
                            gameObject = (component.objectReferenceValue as Component).gameObject;
                        }
                    }

                    if (GUILayout.Button("Select Component", "GV Gizmo DropDown",GUILayout.MaxWidth(120)))
                    {
                        Component[] coms = gameObject.GetComponents<Component>();

                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("GameObject"), component.objectReferenceValue is GameObject, _menuFunction, new object[] { gameObject, scriptItem });
                        if (null != coms)
                        {
                            for (int j = 0; j < coms.Length; ++j)
                            {
                                menu.AddItem(new GUIContent(coms[j].GetType().Name), component.objectReferenceValue == coms[j], _menuFunction, new object[] { coms[j], scriptItem });
                            }
                        }
                        menu.ShowAsContext();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("insert"))
                {
                    if (i > 0)
                    {
                        components.InsertArrayElementAtIndex(i - 1);
                    }
                    else
                    {
                        components.InsertArrayElementAtIndex(i);
                    }
                }
                if (GUILayout.Button("append"))
                {
                    components.InsertArrayElementAtIndex(i);
                }
                GUI.enabled = !string.IsNullOrEmpty(varName.stringValue);
                if (GUILayout.Button("getcode"))
                {
                    string codeInfo = getCopyString(component,varName,bindIndex);
                    if(!string.IsNullOrEmpty(codeInfo))
                    {
                        GUIUtility.systemCopyBuffer = codeInfo;
                        Logger.LogErrorFormat("<color=#00ff00>copy succeed : {0}</color>", codeInfo);
                    }
                }
                GUI.enabled = true;
                if (GUILayout.Button("  -  "))
                {
                    components.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }

        GUI.color = Color.gray;
        EditorGUILayout.BeginVertical("GroupBox");
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        EditorGUILayout.LabelField("Object TotalCount:", GUILayout.MinWidth(60));
        GUI.color = Color.white;
        GUI.enabled = false;
        EditorGUILayout.IntField(components.arraySize, GUILayout.MinWidth(60));
        GUI.enabled = true;
        if (GUILayout.Button("  +   ", GUILayout.MinWidth(60)))
        {
            components.InsertArrayElementAtIndex(components.arraySize);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    protected string getCopyString(SerializedProperty component, SerializedProperty varName,SerializedProperty bindIndex)
    {
        if(null != component && null != bindIndex)
        {
            string labelFixed = "Label_" + labelSpace.stringValue + "_";
            var labels = System.Enum.GetValues(typeof(ComScriptLabel));
            var labelNames = System.Enum.GetNames(typeof(ComScriptLabel));
            List<string> displayedOptions = new List<string>();
            List<int> optionValues = new List<int>();
            for (int j = 0; j < labelNames.Length; ++j)
            {
                var label = (ComScriptLabel)labels.GetValue(j);
                var name = labelNames.GetValue(j) as string;
                if (name.StartsWith(labelFixed) && labelFixed.Length < name.Length)
                {
                    displayedOptions.Add(name.Substring(labelFixed.Length, name.Length - labelFixed.Length));
                    optionValues.Add((int)label);
                }
            }

            if(bindIndex.intValue >= 0 && bindIndex.intValue < displayedOptions.Count)
            {
                string enumVarName = labelFixed + displayedOptions[bindIndex.intValue];
                string componentName = component.objectReferenceValue.GetType().FullName;
                string fmtContent = @"{1} {2} = mScriptBinder.GetObject((int)ComScriptLabel.{0}) as {1};";
                fmtContent = string.Format(fmtContent, enumVarName, componentName, varName.stringValue);
                return fmtContent;
            }
        }
        return string.Empty;
    }

    protected void OnScriptStatusGUI()
    {
        if (scriptStatus.arraySize > 0)
        {
            for (int i = 0; i < scriptStatus.arraySize; ++i)
            {
                var component = scriptStatus.GetArrayElementAtIndex(i);
                if (null != component)
                {
                    GUI.color = Color.gray;
                    EditorGUILayout.BeginVertical("GroupBox");
                    GUI.color = Color.white;

                    var bindIndex = component.FindPropertyRelative("bindIndex");
                    var action = component.FindPropertyRelative("action");
                    if (null != bindIndex)
                    {
                        string labelFixed = "Label_" + labelSpace.stringValue + "_";
                        var labels = System.Enum.GetValues(typeof(ComScriptLabel));
                        var labelNames = System.Enum.GetNames(typeof(ComScriptLabel));
                        List<string> displayedOptions = new List<string>();
                        List<int> optionValues = new List<int>();
                        for (int j = 0; j < labelNames.Length; ++j)
                        {
                            var label = (ComScriptLabel)labels.GetValue(j);
                            var name = labelNames.GetValue(j) as string;
                            if (name.StartsWith(labelFixed) && labelFixed.Length < name.Length)
                            {
                                displayedOptions.Add(name.Substring(labelFixed.Length,name.Length - labelFixed.Length));
                                optionValues.Add((int)label);
                            }
                        }
                        GUI.color = Color.white;
                        EditorGUILayout.BeginHorizontal();
                        GUI.color = Color.white;

                        var labelValue = string.Empty;
                        if (displayedOptions.Count > bindIndex.intValue && bindIndex.intValue >= 0)
                        {
                            GUI.color = Color.green;
                            labelValue = displayedOptions[bindIndex.intValue];
                        }
                        else
                        {
                            GUI.color = Color.yellow;
                            labelValue = "Please Select One Label !";
                        }
                        EditorGUILayout.LabelField(labelValue, GUILayout.MinWidth(60));

                        GUI.color = Color.white;
                        bindIndex.intValue = EditorGUILayout.IntPopup(bindIndex.intValue, displayedOptions.ToArray(), optionValues.ToArray(), GUILayout.MinWidth(100));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.PropertyField(action);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("insert"))
                    {
                        if (i > 0)
                            scriptStatus.InsertArrayElementAtIndex(i - 1);
                        else
                            scriptStatus.InsertArrayElementAtIndex(i);
                    }
                    if (GUILayout.Button("append"))
                    {
                        scriptStatus.InsertArrayElementAtIndex(i);
                    }
                    if (GUILayout.Button("execute action"))
                    {
                        var script = (target as ComScriptBinder);
                        if(null != script)
                        {
                            script.SetAction(bindIndex.intValue);
                        }
                    }
                    if (GUILayout.Button("-"))
                    {
                        scriptStatus.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }
        }

        GUI.color = Color.gray;
        EditorGUILayout.BeginVertical("GroupBox");
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        EditorGUILayout.LabelField("Status TotalCount:", GUILayout.MinWidth(60));
        GUI.color = Color.white;
        GUI.enabled = false;
        EditorGUILayout.IntField(scriptStatus.arraySize, GUILayout.MinWidth(60));
        GUI.enabled = true;
        if (GUILayout.Button("  +   ", GUILayout.MinWidth(60)))
        {
            scriptStatus.InsertArrayElementAtIndex(scriptStatus.arraySize);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    Vector2 scrollPos = Vector2.zero;
    protected void OnInitializedCodeGUI()
    {
        if(mInitializeCodeGUI.Count > 0)
        {
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("GroupBox");
            GUI.color = Color.green;
            for (int i = 0; i < mInitializeCodeGUI.Count; ++i)
            {
                EditorGUILayout.LabelField(mInitializeCodeGUI[i]);
            }
            GUI.color = Color.white;
            if (GUILayout.Button("copy this code"))
            {
                GUIUtility.systemCopyBuffer = string.Empty;
                var repeatedValue = string.Empty;
                if (!checkVarNameRepeated(ref repeatedValue))
                {
                    GUIUtility.systemCopyBuffer = getInitializeCode();
                    Logger.LogErrorFormat("<color=#00ff00>copy succeed !</color>");
                }
                else
                {
                    GUIUtility.systemCopyBuffer = string.Format("copy failed repeated name = [{0}]!", repeatedValue);
                    Logger.LogErrorFormat("<color=#ff0000>copy failed repeated name = [<color=#00ff00>{0}</color>]!</color>", repeatedValue);
                }
            }
            EditorGUILayout.EndVertical();
            GUI.color = Color.white;
        }
    }

    protected bool checkVarNameRepeated(ref string repeatValue)
    {
        List<string> varNames = new List<string>();
        for (int i = 0; i < components.arraySize; ++i)
        {
            var scriptBindItem = components.GetArrayElementAtIndex(i);
            if (null != scriptBindItem)
            {
                SerializedProperty bindIndex = scriptBindItem.FindPropertyRelative("bindIndex");
                SerializedProperty component = scriptBindItem.FindPropertyRelative("component");
                SerializedProperty varName = scriptBindItem.FindPropertyRelative("varName");
                if (!string.IsNullOrEmpty(varName.stringValue))
                {
                    if(varNames.Contains(varName.stringValue))
                    {
                        repeatValue = varName.stringValue;
                        return true;
                    }

                    varNames.Add(varName.stringValue);
                }
            }
        }
        return false;
    }

    protected void createInitializeCodes()
    {
        mInitializeCodeGUI.Clear();
        for (int i = 0; i < components.arraySize; ++i)
        {
            var scriptBindItem = components.GetArrayElementAtIndex(i);
            if (null != scriptBindItem)
            {
                SerializedProperty bindIndex = scriptBindItem.FindPropertyRelative("bindIndex");
                SerializedProperty component = scriptBindItem.FindPropertyRelative("component");
                SerializedProperty varName = scriptBindItem.FindPropertyRelative("varName");
                if (!string.IsNullOrEmpty(varName.stringValue))
                {
                    mInitializeCodeGUI.Add(getCopyString(component, varName, bindIndex));
                }
            }
        }
    }
}