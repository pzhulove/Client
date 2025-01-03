using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputData))]
public class InputDataInspector : Editor
{
    //[MenuItem("按钮配置/创建全局按键数据")]
    public static void Create()
    {
        var pinObject = FileTools.CreateAsset<InputData>("inputdata");

        EditorGUIUtility.PingObject(pinObject);
        Selection.activeObject = pinObject;
    }

    protected string[] buttonList = new string[0];

    public override void OnInspectorGUI()
    {
        InputData data = target as InputData;

        //GameObject curObject = (GameObject)EditorGUILayout.ObjectField("按钮预制体", data.prefabPath, typeof(GameObject));
        data.buttonMode = (InputManager.ButtonMode)EditorGUILayout.EnumPopup("类型", data.buttonMode);
        GameObject curObject = null;
        //if ()
        //if (curObject != data.prefabPath)
        //{
        //    data.prefabPath = curObject;
        //}
        
        if (curObject != null)
        {
            int cnt = curObject.transform.childCount;
            if (cnt != buttonList.Length)
            {
                buttonList = new string[cnt];
            }

            for (int i = 0; i < cnt; i++)
            {
                buttonList[i] = curObject.transform.GetChild(i).name;
            }
        }
        
        //if (data.buttonMode == InputManager.ButtonMode.DRAG)
        {
            if (data.inputData.Length != buttonList.Length)
            {
                data.inputData = new InputSlotKeyNameMap[buttonList.Length];
                for (int i = 0; i < buttonList.Length; i++)
                {
                    data.inputData[i] = new InputSlotKeyNameMap() {slot = i + 1, code = KeyCode.Space};
                }
            }

            for (int i = 0; i < buttonList.Length; i++)
            {
                var item = data.inputData[i];
                EditorGUILayout.BeginHorizontal("GroupBox");
                {
                    EditorGUILayout.BeginVertical();
                    item.name = buttonList[i];
                    item.slot = EditorGUILayout.IntField(item.name, item.slot);
                    //GUILayoutUtility.GetRect(1, 1);
                    item.code = (KeyCode)EditorGUILayout.EnumPopup("按键", item.code);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        
        if (GUI.changed) {
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }
}
