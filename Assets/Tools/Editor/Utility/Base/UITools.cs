//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEditor;

//namespace UITools
//{
//    public static class Controls
//    {
//        private const float kWidth = 160f;
//        private const float kThickHeight = 30f;
//        private const float kThinHeight = 20f;
//        private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
//        private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
//        private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
//        private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
//        private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
//        private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

//        private static GameObject CreateUIElementRoot(string name, Vector2 size)
//        {
//            GameObject child = new GameObject(name);
//            RectTransform rectTransform = child.AddComponent<RectTransform>();
//            rectTransform.sizeDelta = size;
//            return child;
//        }

//        static GameObject CreateUIObject(string name, GameObject parent)
//        {
//            GameObject go = new GameObject(name);
//            go.AddComponent<RectTransform>();
//            SetParentAndAlign(go, parent);
//            return go;
//        }

//        private static void SetDefaultTextValues(Text lbl)
//        {
//            // Set text values we want across UI elements in default controls.
//            // Don't set values which are the same as the default values for the Text component,
//            // since there's no point in that, and it's good to keep them as consistent as possible.
//            lbl.color = s_TextColor;
//        }

//        private static void SetDefaultColorTransitionValues(Selectable slider)
//        {
//            ColorBlock colors = slider.colors;
//            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
//            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
//            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
//        }

//        private static void SetParentAndAlign(GameObject child, GameObject parent)
//        {
//            if (parent == null)
//                return;

//            child.transform.SetParent(parent.transform, false);
//            SetLayerRecursively(child, parent.layer);
//        }

//        private static void SetLayerRecursively(GameObject go, int layer)
//        {
//            go.layer = layer;
//            Transform t = go.transform;
//            for (int i = 0; i < t.childCount; i++)
//                SetLayerRecursively(t.GetChild(i).gameObject, layer);
//        }

//        // Actual controls
 
//        public static GameObject CreateInputAxisControl(DefaultControls.Resources resources)
//        {
//            GameObject buttonRoot = CreateUIElementRoot("InputAxisControl", s_ThickElementSize);

      
//            Image image = buttonRoot.AddComponent<Image>();
//            image.sprite = resources.standard;
//            image.type = Image.Type.Sliced;
//            image.color = s_DefaultSelectableColor;

//            GameClient.UI.InputAxisControl bt = buttonRoot.AddComponent<GameClient.UI.InputAxisControl>();
//            SetDefaultColorTransitionValues(bt);

//            return buttonRoot;
//        }
//    }
//}
