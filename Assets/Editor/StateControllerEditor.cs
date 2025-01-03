using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(StateController))]
    class StateControllerEditor : Editor
    {
        protected SerializedObject _serializedObject;
        protected SerializedProperty elements,expand, count, defKey;
        protected string[] stateAttrs = new string[] { "instanceVisible", "instanceImageSprite", "instanceCanvasGroup", "instanceGraphicColor", "instanceUIPrefabWrapper" };

        public void OnEnable()
        {
            _serializedObject = new SerializedObject(target);

            elements = _serializedObject.FindProperty("elements");
            expand = _serializedObject.FindProperty("expand");
            count = _serializedObject.FindProperty("count");
            defKey = _serializedObject.FindProperty("defKey");
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            {
                if(expand.boolValue = EditorGUILayout.Toggle(new GUIContent("状态组:"), expand.boolValue))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(this.count);
                    for (int i = 0; i < this.count.intValue && i < elements.arraySize; ++i)
                    {
                        var element = elements.GetArrayElementAtIndex(i);
                        var key = element.FindPropertyRelative("key");
                        var desc = element.FindPropertyRelative("desc");
                        var showInEditor = element.FindPropertyRelative("showInEditor");
                        var onEvent = element.FindPropertyRelative("onEvent");

                        EditorGUILayout.Space();
                        if(showInEditor.boolValue = EditorGUILayout.Toggle(new GUIContent(key.stringValue),showInEditor.boolValue))
                        {
                            EditorGUILayout.PropertyField(key);
                            EditorGUILayout.PropertyField(desc, new GUIContent("状态描述:"));
                            EditorGUILayout.PropertyField(onEvent);
                            _onVisible(element);
                            EditorGUILayout.Space();
                            _onImageSprite(element);
                            EditorGUILayout.Space();
                            _onCanvasGroup(element);
                            EditorGUILayout.Space();
                            _onGraphicColor(element);
                            EditorGUILayout.Space();
                            _onUIPrefabWrapper(element);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            _onGui();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(defKey, new GUIContent("当前状态:"));
            EditorGUI.indentLevel++;
            var find = (target as StateController).elements.Find(x => { return x.key == defKey.stringValue; });
            if (find == null)
            {
                find = (target as StateController).elements.Find(x => { return !string.IsNullOrEmpty(x.key); });
            }
            if (find != null)
            {
                if (defKey.stringValue != find.key)
                {
                    defKey.stringValue = find.key;
                }
                EditorGUILayout.LabelField(find.key);
                EditorGUILayout.LabelField(find.desc);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                _onEelementCountChanged(this.count, elements);

                for (int i = 0; i < this.count.intValue && i < elements.arraySize; ++i)
                {
                    var element = elements.GetArrayElementAtIndex(i);


                    for (int j = 0; j < stateAttrs.Length; j++)
                    {
                        var attrname = stateAttrs[j];
                        var instanceVisible = element.FindPropertyRelative(attrname);
                        if (null != instanceVisible)
                        {
                            var countchild = instanceVisible.FindPropertyRelative("count");
                            var elementschild = instanceVisible.FindPropertyRelative("elements");
                            if (elementschild.arraySize != countchild.intValue)
                            {
                                _onEelementCountChanged(countchild, elementschild);
                            }
                        }
                    }
                }

                _serializedObject.ApplyModifiedProperties();

                if((target as StateController).Key == defKey.stringValue)
                {
                    (target as StateController)._ChangeStatus();
                }
            }
        }

        void _onEelementCountChanged(SerializedProperty count, SerializedProperty elements)
        {
            int orgSize = elements.arraySize;

            if (orgSize < count.intValue)
            {
                for (int i = orgSize; i < count.intValue; ++i)
                {
                    elements.InsertArrayElementAtIndex(i);
                }
            }
            else if (orgSize > count.intValue)
            {
                for (int i = count.intValue; i < orgSize; ++i)
                {
                    elements.DeleteArrayElementAtIndex(elements.arraySize - 1);
                }
            }
        }

    void _onVisible(SerializedProperty element)
    {
        var instanceVisible = element.FindPropertyRelative("instanceVisible");
        {
            var expand = instanceVisible.FindPropertyRelative("expand");
            if (expand.boolValue = EditorGUILayout.Toggle(new GUIContent("可视控制:"), expand.boolValue))
            {
                var count = instanceVisible.FindPropertyRelative("count");
                var elements = instanceVisible.FindPropertyRelative("elements");                    
                _ShowArrayElements(elements, count);
                EditorGUILayout.PropertyField(count, new GUIContent("对象数量:"));
            }
        }
    }

        void _onImageSprite(SerializedProperty element)
        {
            var instanceImageSprite = element.FindPropertyRelative("instanceImageSprite");
            {
                var expand = instanceImageSprite.FindPropertyRelative("expand");
                if (expand.boolValue = EditorGUILayout.Toggle(new GUIContent("图片控制:"), expand.boolValue))
                {
                    var count = instanceImageSprite.FindPropertyRelative("count");
                    var elements = instanceImageSprite.FindPropertyRelative("elements");
                    EditorGUILayout.PropertyField(count, new GUIContent("对象数量:"));
                    for (int j = 0; j < elements.arraySize; ++j)
                    {
                        var curElement = elements.GetArrayElementAtIndex(j);
                        if (null != curElement)
                        {
                            EditorGUILayout.Space();
                            var imagePath = curElement.FindPropertyRelative("imageData");
                            var gameObject = curElement.FindPropertyRelative("image");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(imagePath, new GUIContent("图片资源:"));
                            EditorGUILayout.PropertyField(gameObject, new GUIContent(""));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }

        void _onCanvasGroup(SerializedProperty element)
        {
            var instanceCanvasGroup = element.FindPropertyRelative("instanceCanvasGroup");
            {
                var expand = instanceCanvasGroup.FindPropertyRelative("expand");
                if (expand.boolValue = EditorGUILayout.Toggle(new GUIContent("CanvasGroup控制:"), expand.boolValue))
                {
                    var count = instanceCanvasGroup.FindPropertyRelative("count");
                    var elements = instanceCanvasGroup.FindPropertyRelative("elements");
                    EditorGUILayout.PropertyField(count, new GUIContent("对象数量:"));
                    for (int j = 0; j < elements.arraySize; ++j)
                    {
                        var curElement = elements.GetArrayElementAtIndex(j);
                        if (null != curElement)
                        {
                            EditorGUILayout.Space();
                            var imagePath = curElement.FindPropertyRelative("alpha");
                            var isRaycast = curElement.FindPropertyRelative("isRaycast");
                            var gameObject = curElement.FindPropertyRelative("canvasGroup");
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(imagePath, new GUIContent("alpha:"));
                            EditorGUILayout.PropertyField(gameObject, new GUIContent(""));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.PropertyField(isRaycast, new GUIContent("isRaycast:"));
                            EditorGUILayout.EndVertical();

                        }
                    }
                }
            }
        }

        void _onUIPrefabWrapper(SerializedProperty element)
        {
            var instanceUIPrefabWrapper = element.FindPropertyRelative("instanceUIPrefabWrapper");
            {
                var expand = instanceUIPrefabWrapper.FindPropertyRelative("expand");
                if (expand.boolValue = EditorGUILayout.Toggle(new GUIContent("UIPrefabWrapper控制:"), expand.boolValue))
                {
                    var count = instanceUIPrefabWrapper.FindPropertyRelative("count");
                    var elements = instanceUIPrefabWrapper.FindPropertyRelative("elements");
                    EditorGUILayout.PropertyField(count, new GUIContent("对象数量:"));
                    for (int j = 0; j < elements.arraySize; ++j)
                    {
                        var curElement = elements.GetArrayElementAtIndex(j);
                        if (null != curElement)
                        {
                            EditorGUILayout.Space();
                            var isLoad = curElement.FindPropertyRelative("isLoad");
                            var uiPrefabWrapper = curElement.FindPropertyRelative("uiPrefabWrapper");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(isLoad, new GUIContent("isLoad:"));
                            EditorGUILayout.PropertyField(uiPrefabWrapper, new GUIContent(""));
                            EditorGUILayout.EndHorizontal();

                        }
                    }
                }
            }
        }

        void _onGraphicColor(SerializedProperty element)
        {
            var instanceGraphicColor = element.FindPropertyRelative("instanceGraphicColor");
            {
                var expand = instanceGraphicColor.FindPropertyRelative("expand");
                if (expand.boolValue = EditorGUILayout.Toggle(new GUIContent("颜色控制:"), expand.boolValue))
                {
                    var count = instanceGraphicColor.FindPropertyRelative("count");
                    var elements = instanceGraphicColor.FindPropertyRelative("elements");
                    EditorGUILayout.PropertyField(count, new GUIContent("对象数量:"));
                    for (int j = 0; j < elements.arraySize; ++j)
                    {
                        var curElement = elements.GetArrayElementAtIndex(j);
                        if (null != curElement)
                        {
                            EditorGUILayout.Space();
                            var imagePath = curElement.FindPropertyRelative("color");
                            var gameObject = curElement.FindPropertyRelative("graphic");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(imagePath, new GUIContent("color:"));
                            EditorGUILayout.PropertyField(gameObject, new GUIContent(""));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }

        private static GUIContent
        duplicateBtnContent = new GUIContent("+", "duplicate"),
        deleteBtnContent = new GUIContent("-", "delete"),
        addBtnContent = new GUIContent("+", "add");

    private static GUILayoutOption miniBtnWidth = GUILayout.Width(50f);

    void _ShowArrayElements(SerializedProperty list, SerializedProperty count)
    {
        if (list == null || count == null)
        {
            return;
        }
        if (_serializedObject != null)
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField(string.Format("对象数量:---------------------------------->{0}", list.arraySize));
            if (list.arraySize > 0)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var curElement = list.GetArrayElementAtIndex(i);
                    if (null != curElement)
                    {
                        EditorGUILayout.Space();
                        var visible = curElement.FindPropertyRelative("bVisible");
                        var gameObject = curElement.FindPropertyRelative("gameObject");
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(visible, new GUIContent("是否可见:"));
                        EditorGUILayout.PropertyField(gameObject, new GUIContent(""));
                        EditorGUILayout.EndHorizontal();
                    }

                    //EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                    ShowButtonsInArrayElement(list, i);
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.Space();

            if (list.arraySize == 0 &&
                GUILayout.Button(addBtnContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }

            count.intValue = list.arraySize;
        }
    }

    private void ShowButtonsInArrayElement(SerializedProperty list, int index)
    {
        if (list == null)
        {
            return;
        }
        if (GUILayout.Button(duplicateBtnContent, EditorStyles.miniButtonLeft, miniBtnWidth))
        {
            list.InsertArrayElementAtIndex(index);            
        }
        if (GUILayout.Button(deleteBtnContent, EditorStyles.miniButtonRight, miniBtnWidth))
        {
            int oldSize = list.arraySize;
            list.DeleteArrayElementAtIndex(index);
            if (list.arraySize == oldSize)
            {
                list.DeleteArrayElementAtIndex(index);
            }
        }
    }

        void _onGui()
        {
            if(GUILayout.Button(new GUIContent("next state")))
            {
                var elements = (target as StateController).elements;
                for(int i = 0; i < elements.Count; ++i)
                {
                    if(elements[i].key == defKey.stringValue)
                    {
                        int iTarget = (i + 1) % elements.Count;
                        if(iTarget != i)
                        {
                            defKey.stringValue = elements[iTarget].key;
                            (target as StateController).Key = elements[iTarget].key;
                        }
                        break;
                    }
                }
            }
        }
    }
}