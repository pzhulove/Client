using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Object = UnityEngine.Object;

/// <summary>
/// UI检测类 遍历输出选中GameObject的图集信息
/// </summary>
namespace GameClient
{
    public partial class TMUITools
    {
        private enum CheckType
        {
            Sprite,
            Canvas,
            ScaleNegative,
            ZGreaterZero,
        }

        private static StringBuilder m_stringBuilder = new StringBuilder();

        [MenuItem("Assets/UI相关/遍历输出Sprite引用图集信息", false)]
        public static void PrintSpriteInfo()
        {
            _AssetPrintInfo(CheckType.Sprite);
        }

        [MenuItem("GameObject/UI相关/遍历输出Sprite引用图集信息", false, 30)]
        public static void PrintSpriteInfo1()
        {
            _GameObjectPrintInfo(CheckType.Sprite);
        }

        [MenuItem("Assets/UI相关/遍历输出包含Canvas的节点", false)]
        public static void PrintCanvasInfo()
        {
            _AssetPrintInfo(CheckType.Canvas);
        }

        [MenuItem("GameObject/UI相关/遍历输出包含Canvas的节点", false, 30)]
        public static void PrintCanvasInfo1()
        {
            _GameObjectPrintInfo(CheckType.Canvas);
        }

        [MenuItem("Assets/UI相关/遍历输出Scale为负数节点", false)]
        public static void PrintScaleNegativeInfo()
        {
            _AssetPrintInfo(CheckType.ScaleNegative);
        }

        [MenuItem("GameObject/UI相关/遍历输出Scale为负数节点", false, 30)]
        public static void PrintScaleNegativeInfo1()
        {
            _GameObjectPrintInfo(CheckType.ScaleNegative);
        }

        [MenuItem("Assets/UI相关/遍历输出Z轴坐标大于0", false)]
        public static void GetUITransformGreaterXZero()
        {
            _AssetPrintInfo(CheckType.ZGreaterZero);
        }

        [MenuItem("GameObject/UI相关/遍历输出Z轴坐标大于0", false, 30)]
        public static void GetUITransformGreaterXZero1()
        {
            _GameObjectPrintInfo(CheckType.ZGreaterZero);
        }

        #region 将OutLine修改为NicerOutLine
        [MenuItem("Assets/UI相关/将OutLine修改为NicerOutLine")]
        private static void ChangeOutLineToNicerOutLine()
        {
            string[] selectPath = Selection.assetGUIDs;
            if (selectPath.Length <= 0)
                return;
            string findRootPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
            if (!Directory.Exists(findRootPath))
                return;
            string[] searchFolder = { findRootPath };
            string[] allPrefabGuid = AssetDatabase.FindAssets("t:prefab", searchFolder);
            int corrCount = allPrefabGuid.Length;
            for (int i = 0; i < corrCount; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGuid[i]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                GameObject insPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (insPrefab == null)
                    continue;
                if (ChangeTransOutLine(insPrefab.transform))
                    PrefabUtility.SaveAsPrefabAsset(insPrefab, prefabPath);
                Object.DestroyImmediate(insPrefab);
            }
        }
        private static bool ChangeTransOutLine(Transform transRoot)
        {
            bool haveChange = false;
            int childCount = transRoot.childCount;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    Transform transChild = transRoot.GetChild(i);
                    if (ChangeTransOutLine(transChild))
                        haveChange = true;
                }
            }
            Outline[] outLines = transRoot.GetComponents<Outline>();
            int outLineCount = outLines.Length;
            if (outLineCount == 0)
                return haveChange;
            for (int j = 0; j < outLineCount; j++)
            {
                Outline outLine = outLines[j];
                GameObject objOutLine = transRoot.gameObject;
                NicerOutline nicerOutLine = objOutLine.AddComponent<NicerOutline>();
                nicerOutLine.effectColor = outLine.effectColor;
                nicerOutLine.effectDistance = outLine.effectDistance;
                nicerOutLine.useGraphicAlpha = outLine.useGraphicAlpha;
                Object.DestroyImmediate(outLine);
            }
            return true;
        }
        #endregion

        private static void _AssetPrintInfo(CheckType checkType)
        {
            m_stringBuilder.Clear();
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            EditorUtility.ClearProgressBar();
            GameObject root = selection[0] as GameObject;
            if (root == null)
            {
                string fullPath = FileTools.GetAssetFullPath(selection[0]);
                if (!Directory.Exists(fullPath))
                    return;
                string[] searchFolder = { fullPath.Substring(fullPath.IndexOf("Assets", StringComparison.Ordinal)) };
                string[] allAssetsGuid = AssetDatabase.FindAssets("t:prefab", searchFolder);

                for (int i = 0; i < allAssetsGuid.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("检测预制体:", "Converting .. " + i + "/" + allAssetsGuid.Length, (i + 1) / (float)allAssetsGuid.Length);
                    var path = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
                    GameObject assetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (assetPrefab != null)
                        _PrintSingleInfo(checkType, assetPrefab);
                }
                EditorUtility.ClearProgressBar();
            }
            else
                _PrintSingleInfo(checkType, root);
            if (m_stringBuilder.Length > 0)
                Logger.LogErrorFormat("{0}: {1}", checkType, m_stringBuilder);
        }

        private static void _GameObjectPrintInfo(CheckType checkType)
        {
            m_stringBuilder.Clear();
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);

            GameObject root = selection[0] as GameObject;
            if (root == null)
                return;
            _PrintSingleInfo(checkType, root);
            if (m_stringBuilder.Length > 0)
                Logger.LogErrorFormat("{0}: {1}", checkType, m_stringBuilder);
        }

        private static void _PrintSingleInfo(CheckType checkType, GameObject obj)
        {
            Transform trans = obj.transform;
            switch (checkType)
            {
                case CheckType.Canvas:
                    _GetCanvasInfo(trans);
                    break;
                case CheckType.Sprite:
                    _GetSpriteInfo(trans);
                    break;
                case CheckType.ScaleNegative:
                    _GetScaleNegativeInfo(trans);
                    break;
                case CheckType.ZGreaterZero:
                    _GetUITransformGreaterXZero(trans);
                    break;
            }
        }

        private static void _GetSpriteInfo(Transform go)
        {
            var images = go.GetComponents<Image>();
            foreach (var image in images)
            {
                var sprite = image.sprite;
                if (sprite != null && sprite.texture != null)
                {
                    string str = _GetFullPath(go);
                    m_stringBuilder.AppendFormat("Path:{0} ---- sprite:{1} ---- texture:{2} {3}", str, sprite.name, sprite.texture.name, Environment.NewLine);
                }
            }

            for (int i = 0; i < go.transform.childCount; i++)
            {
                _GetSpriteInfo(go.transform.GetChild(i));
            }
        }

        private static void _GetCanvasInfo(Transform go)
        {
            if (go.GetComponent<Canvas>() != null || go.GetComponent<CanvasGroup>() || go.GetComponent<GraphicRaycaster>())
            {
                string str = _GetFullPath(go);
                m_stringBuilder.AppendFormat("\n Path:{0}", str);
            }

            for (int i = 0; i < go.childCount; i++)
            {
                var child = go.GetChild(i);
                if (child.GetComponent<Canvas>() != null || child.GetComponent<CanvasGroup>() || child.GetComponent<GraphicRaycaster>())
                {
                    string str = _GetFullPath(child);
                    m_stringBuilder.AppendFormat("\n Path:{0}", str);
                }
                else
                    break;
                _GetScaleNegativeInfo(child);
            }
        }

        private static void _GetScaleNegativeInfo(Transform go)
        {
            if (go.transform.localScale.x < 0
                    || go.transform.localScale.y < 0
                    || go.transform.localScale.z < 0)
            {
                string str = _GetFullPath(go);
                m_stringBuilder.AppendFormat("\n Path:{0}", str);
            }

            for (int i = 0; i < go.childCount; i++)
            {
                var child = go.GetChild(i);
                if (child.transform.localScale.x < 0
                    || child.transform.localScale.y < 0
                    || child.transform.localScale.z < 0)
                {
                    string str = _GetFullPath(child);
                    m_stringBuilder.AppendFormat("\n Path:{0}", str);
                }
                else
                    break;
                _GetScaleNegativeInfo(child);
            }
        }

        private static void _GetUITransformGreaterXZero(Transform go)
        {
            if (go.transform.localPosition.z > 0)
            {
                string str = _GetFullPath(go);
                m_stringBuilder.AppendFormat("\n Path:{0}", str);
            }

            for (int i = 0; i < go.childCount; i++)
            {
                var child = go.GetChild(i);
                if (child.transform.localPosition.z > 0)
                {
                    string str = _GetFullPath(child);
                    m_stringBuilder.AppendFormat("\n Path:{0}", str);
                }
                else
                    break;
                _GetUITransformGreaterXZero(child);
            }
        }

        /// <summary>
        /// 向上遍历十层父节点
        /// </summary>
        private static string _GetFullPath(Transform obj)
        {
            Transform curTrans = obj;
            string info = obj.name;
            for (int i = 0; i < 10; i++)
            {
                if (curTrans != null)
                {
                    info = curTrans.name + "/" + info;
                    curTrans = curTrans.parent;
                }
                else
                {
                    break;
                }
            }
            return info;
        }
    }
}