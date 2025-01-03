using UnityEngine;
using UnityEditor;

namespace AssetBundleTool
{
    public class AB_MainWindow : EditorWindow
    {
        public delegate void OnDo();
        public static event OnDo sm_OnInit, sm_OnDestory;
        public enum Model
        {
            PackStrategy = 0,
            Build = 1,
            Inspect = 2
        }
        private static Model sm_model;
        [MenuItem("[打包工具]/AssetBundle可视化配置窗口")]
        public static void GetWindow()
        {
            var window = GetWindow<AB_MainWindow>();
            window.Focus();
            window.minSize = new Vector2(1400, 600);
            window.Repaint();

        }
        private void OnEnable()
        {
            sm_model = Model.PackStrategy;
            StrategyView.OnInit();
            BuildView.OnInit();
            InspectView.OnInit();

            //AssetProcessor.textureEnable = true;
        }
        private void OnGUI()
        {
            ModeToggle();
            GUILayout.BeginScrollView(Vector2.zero, GUILayout.Height(position.height));
            switch (sm_model)
            {
                case Model.PackStrategy:
                    StrategyView.OnDraw(position);
                    break;
                case Model.Build:
                    BuildView.OnDraw(position);
                    break;
                case Model.Inspect:
                    InspectView.OnDraw(position);
                    break;
                default:
                    break;
            }
            //如果编辑器在更新或者编译，则弹出提示
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                GUIStyle style = EditorStyles.label;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Bold;
                GUI.contentColor = Color.red;
                style.fontSize = 80;
                GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height / 2f - 50, 800, 100), "Compiling", style);
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = 0;
                GUI.contentColor = Color.white;
                style.fontStyle = FontStyle.Normal;
            }
            GUILayout.EndScrollView();
        }
        void ModeToggle()
        {
            GUILayout.BeginHorizontal();
            float toolbarWidth = position.width;
            string[] labels = new string[] { "PackStrategyModel", "BuildModel", "InspectPanel" };
            Model modelTemp = (Model)GUILayout.Toolbar((int)sm_model, labels, "LargeButton", GUILayout.Width(toolbarWidth));
            sm_model = modelTemp;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private Rect GetSubWindowArea()
        {
            float padding = 30;
            Rect subPos = new Rect(0, 0, position.width, position.height - padding);
            return subPos;
        }
        private void OnDestroy()
        {
            if (sm_model == Model.PackStrategy && EditorUtility.DisplayDialog("Save", "是否保存策略？", "保存", "取消"))
            {
               StrategyView.SaveStrategy();
               StrategyView.sm_selectStrategyData = null;
            }
            StrategyView.sm_strategyDataList = null;


        }
        private void OnDisable()
        {
            StrategyView.OnLeave();
            BuildView.OnLeave();
            InspectView.OnLeave();
        }
    }
}
