using UnityEngine;
using UnityEditor;
using Tenmove.Runtime.Unity;
using Tenmove.Runtime.Client;

namespace Tenmove.Editor.Unity
{
    [CustomEditor(typeof(RadialBlurSetting))]
    public class RadialBlurSettingInspector : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/后处理/创建径向模糊配置")]
        public static void CreateRadialBlurSetting()
        {
            if (Selection.objects == null || Selection.objects.Length < 1 || AssetDatabase.GetAssetPath(Selection.objects[0]) == string.Empty)
            {
                UnityEngine.Debug.LogError("请选择要处理的文件夹");
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
            string absolutePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + path;
            // 不是个path
            if (!System.IO.Directory.Exists(absolutePath))
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            var a = ScriptableObject.CreateInstance<RadialBlurSetting>();
            AssetDatabase.CreateAsset(a, path + "/RadialBlurSetting.asset");
            AssetDatabase.SaveAssets();
        }

        PostprocessLayer layer;

        RadialBlurSetting radialBlurSetting;

        private void OnEnable()
        {
            radialBlurSetting = target as RadialBlurSetting;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Apply"))
            {
                Camera mainCamera = Camera.main;
                if(mainCamera != null)
                {
                    layer = mainCamera.gameObject.GetComponent<PostprocessLayer>();
                    if(layer != null)
                    {
                        layer.ActiveEffect(PostProcessType.RadialBlur,radialBlurSetting);
                        layer.UpdateCommandBuffer();
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(radialBlurSetting);
                if (layer != null)
                {
                    layer.ActiveEffect(PostProcessType.RadialBlur, radialBlurSetting);
                    layer.UpdateCommandBuffer();
                }
            }
        }
    }
}

