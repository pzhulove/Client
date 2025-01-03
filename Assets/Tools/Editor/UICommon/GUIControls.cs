using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

 
namespace GUIControls
{
    public class UIStyles
    {
        protected static Font font;
        public static void UpdateEditorStyles()
        {
            if (!EditorGUIUtility.isProSkin)
                return;
            
            DG.DemiEditor.DeGUI.BeginGUI (DG.DOTweenEditor.Core.ABSAnimationInspector.colors, DG.DOTweenEditor.Core.ABSAnimationInspector.styles);
            //DG.DOTweenEditor.Core.EditorGUIUtils.SetGUIStyles();

            if (font != null)
            {
                return;
            }

            try
            {
                Color normal = Color.white * 0.9f;
                Color selected = Color.white;

                font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");

                //foldout
                EditorStyles.foldout.fontSize = 14;
                EditorStyles.foldout.font = font;
                EditorStyles.foldout.normal.textColor = normal;
                EditorStyles.foldout.hover.textColor = selected;

                //textField
                EditorStyles.textField.fontSize = 12;
                EditorStyles.textField.fixedHeight = 18;
                EditorStyles.textField.font = font;
                EditorStyles.textField.normal.textColor = normal;
                EditorStyles.textField.hover.textColor = selected;

                //label
                EditorStyles.label.fontSize = 12;
                EditorStyles.label.fixedHeight = 20;
                EditorStyles.label.font = font;
                EditorStyles.label.normal.textColor = normal;
                EditorStyles.label.hover.textColor = selected;

                //minibutton
                EditorStyles.miniButton.fontSize = 14;
                EditorStyles.miniButton.font = font;
                EditorStyles.miniButton.fixedHeight = 20;
                EditorStyles.miniButton.stretchWidth = true;
                EditorStyles.miniButton.normal.textColor = normal;
                EditorStyles.miniButton.hover.textColor = selected;


                EditorStyles.miniButtonMid.fontSize = 14;
                EditorStyles.miniButtonMid.font = font;
                EditorStyles.miniButtonMid.fixedHeight = 20;
                EditorStyles.miniButtonMid.stretchWidth = true;
                EditorStyles.miniButtonMid.normal.textColor = normal;
                EditorStyles.miniButtonMid.hover.textColor = selected;

                //popup
                EditorStyles.popup.fontSize = 12;
                EditorStyles.popup.font = font;
                EditorStyles.popup.stretchWidth = true;
                EditorStyles.popup.fixedHeight = 16;
                EditorStyles.popup.normal.textColor = normal;
                EditorStyles.popup.hover.textColor = selected;

            }
            catch
            {
                UnityEngine.Debug.LogWarning("Update EditorStyles!");
            }
        }
    }
    public class UICommon
    {
        public static string titleStyle = "MeTransOffRight";
        public static string removeButtonStyle = "TL SelectionBarCloseButton";
        public static string addButtonStyle = "CN CountBadge";
        public static string rootGroupStyle = "GroupBox";
        public static string subGroupStyle = "ObjectFieldThumb";
        public static string arrayElementStyle = "flow overlay box";
        public static string subArrayElementStyle = "HelpBox";
        public static string foldStyle = "Foldout";
        public static string enumStyle = "MiniPopup";
        public static string toggleStyle = "BoldToggle";
        public static string borderBarStyle = "ProgressBarBack";
        public static string fillBarStyle1 = "ProgressBarBar";
        public static string fillBarStyle2 = "flow node 2 on";
        public static string fillBarStyle3 = "flow node 4 on";
        public static string fillBarStyle4 = "flow node 6 on";

        

        public static float SizeSlider(Vector3 p, Vector3 d, float r)
        {
            Vector3 vector = p + d * r;
            float handleSize = HandleUtility.GetHandleSize(vector);
            bool changed = GUI.changed;
            GUI.changed = false;
            vector = Handles.Slider(vector, d, handleSize * 0.03f, new Handles.DrawCapFunction(Handles.DotCap), 0f);
            if (GUI.changed)
            {
                r = Vector3.Dot(vector - p, d);
            }
            GUI.changed |= changed;
            return r;
        }

        public static void DrawRect(Quaternion rotation, Vector3 position, Vector2 size)
        {
            Vector3 b = rotation * Vector3.up;
            Vector3 vector = rotation * Vector3.right;
            Vector3 vector2 = rotation * Vector3.forward;
            float num = 0.5f * size.x;
            float num2 = 0.5f * size.y;
            Vector3 vector3 = position + vector * num2 + vector2 * num;
            Vector3 vector4 = position - vector * num2 + vector2 * num;
            Vector3 vector5 = position - vector * num2 - vector2 * num;
            Vector3 vector6 = position + vector * num2 - vector2 * num;
            Handles.DrawLine(vector3, vector4);
            Handles.DrawLine(vector4, vector5);
            Handles.DrawLine(vector5, vector6);
            Handles.DrawLine(vector6, vector3);
        }

        public static Vector4 RectHandles(Vector3 position, Vector4 rect)
        {
            Vector3 b = Vector3.up;
            Vector3 vector =  Vector3.right;
            Vector3 vector2 =  Vector3.forward;
 
            Vector3 vector3 = position + vector * rect.x + vector2 * rect.z;
            Vector3 vector4 = position + vector * rect.x + vector2 * rect.w;
            Vector3 vector5 = position + vector * rect.y + vector2 * rect.w;
            Vector3 vector6 = position + vector * rect.y + vector2 * rect.z;
            Handles.DrawLine(vector3, vector4);
            Handles.DrawLine(vector4, vector5);
            Handles.DrawLine(vector5, vector6);
            Handles.DrawLine(vector6, vector3);
            Color color = Handles.color;
            color.a = Mathf.Clamp01(color.a * 2f);
            Handles.color = color;
            rect.x = SizeSlider(position, vector, rect.x);
            rect.y = SizeSlider(position, vector, rect.y);
            rect.z = SizeSlider(position, vector2, rect.z);
            rect.w = SizeSlider(position, vector2, rect.w);
            if ((Tools.current != Tool.Move && Tools.current != Tool.Scale) || Tools.pivotRotation != PivotRotation.Local)
            {
                Handles.DrawLine(position, position + b);
            }

            return rect;
        }

        public static Vector2 RectHandles(Quaternion rotation, Vector3 position, Vector2 size)
        {
            Vector3 b = rotation * Vector3.up;
            Vector3 vector = rotation * Vector3.right;
            Vector3 vector2 = rotation * Vector3.forward;
            float num = 0.5f * size.x;
            float num2 = 0.5f * size.y;
            Vector3 vector3 = position + vector * num + vector2 * num2;
            Vector3 vector4 = position - vector * num + vector2 * num2;
            Vector3 vector5 = position - vector * num - vector2 * num2;
            Vector3 vector6 = position + vector * num - vector2 * num2;
            Handles.DrawLine(vector3, vector4);
            Handles.DrawLine(vector4, vector5);
            Handles.DrawLine(vector5, vector6);
            Handles.DrawLine(vector6, vector3);
            Color color = Handles.color;
            color.a = Mathf.Clamp01(color.a * 2f);
            Handles.color = color;
            num = SizeSlider(position, vector, num);
            num = SizeSlider(position, -vector, num);
            num2 =  SizeSlider(position, vector2, num2);
            num2 =  SizeSlider(position, -vector2, num2);
            if ((Tools.current != Tool.Move && Tools.current != Tool.Scale) || Tools.pivotRotation != PivotRotation.Local)
            {
                Handles.DrawLine(position, position + b);
            }
            size.x = 2f * num;
            size.y = 2f * num2;
            return size;
        }


        internal static void DrawRadius2DHandle(Vector3 position, float radius)
        {
            Vector3 b = Vector3.up;
            Handles.DrawWireDisc(position, b, radius);
        }

        internal static float Radius2DHandle(Vector3 position, float radius)
        {
            Vector3 b = Vector3.up;
            Vector3 vector = Vector3.right;
            Vector3 vector2 = Vector3.forward;
            float num = radius;
            Handles.DrawWireDisc(position, b, radius);
            Color color = Handles.color;
            color.a = Mathf.Clamp01(color.a * 2f);
            Handles.color = color;
            num = SizeSlider(position, vector, num);
            num = SizeSlider(position, -vector, num);
            num = SizeSlider(position, vector2, num);
            num = SizeSlider(position, -vector2, num);
            if ((Tools.current != Tool.Move && Tools.current != Tool.Scale) || Tools.pivotRotation != PivotRotation.Local)
            {
                Handles.DrawLine(position, position + b);
            }
            radius = num;
            return radius;
        }

        public static int AnimFrameSlider(string label, int targetVar, int indentLevel, int minValue, int maxValue)
        {
            int indentSpacing = 25 * indentLevel;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Rect tempRect = GUILayoutUtility.GetRect(1, 10);

            float width = Screen.width / EditorGUIUtility.pixelsPerPoint;

            Rect rect = new Rect(indentSpacing, tempRect.y, width - indentSpacing - 100, 20);
            float fProgress = 1.0f;
            if (minValue != maxValue)
            {
                fProgress = Mathf.Abs((float)targetVar / maxValue);
            }
            EditorGUI.ProgressBar(rect, fProgress, label);

            tempRect = GUILayoutUtility.GetRect(1, 20);
            rect.y += 10;
            rect.x = indentLevel * 10 - 3;
            rect.width = (width - (indentLevel * 10) - 100) + 62; // Changed for 4.3;

            return EditorGUI.IntSlider(rect, "", targetVar, minValue, maxValue);
        }

        public static int AnimFrameSegment(int sum, IList<int> list, int currentLight, int indentLevel)
        {
            int indentSpacing = 25 * indentLevel;
            Rect tempRect = GUILayoutUtility.GetRect(1, 10);
            float width = Screen.width / EditorGUIUtility.pixelsPerPoint;
            Rect rect = new Rect(indentSpacing, tempRect.y, width - indentSpacing - 100, 15);

            int clickFlag = -1;

            //int sum = 0;
            //for (int i = 0; i < list.Count; ++i)
            //{
            //    sum += list[i];
            //}

            int tick = 0;
            float perUnit =  1.0f / sum * rect.width;
            for (int i = 0; i < list.Count; ++i)
            {
                //EditorGUI.Button()
                float posx = rect.x + (tick * perUnit);

                Rect newRect = new Rect(posx, rect.y, perUnit * list[i], rect.height);
                tick += list[i];
                var bk = GUI.color;
                if (currentLight == i)
                {
                    GUI.color = Color.yellow;
                }
                //EditorGUI.jk
                if (GUI.Button(newRect, i.ToString()))
                {
                    clickFlag = i;
                }
                GUI.color = bk;
            }

            return clickFlag;
        }

        public static float AnimFrameSlider(string label, float targetVar, int indentLevel, float minValue, float maxValue)
        {
            int indentSpacing = 25 * indentLevel;
            EditorGUILayout.Space();
            
            EditorGUILayout.Space();

            Rect tempRect = GUILayoutUtility.GetRect(1, 10);
            float width = Screen.width / EditorGUIUtility.pixelsPerPoint;
            Rect rect = new Rect(indentSpacing, tempRect.y, width - indentSpacing - 100, 20);

            float fProgress = 1.0f;
            if (minValue != maxValue)
            {
                fProgress = Mathf.Abs((float)targetVar / maxValue);
            }
            EditorGUI.ProgressBar(rect, fProgress, label);

            tempRect = GUILayoutUtility.GetRect(1, 20);
            rect.y += 10;
            rect.x = indentLevel * 10 - 3;
            rect.width = (width - (indentLevel * 10) - 100) + 62; // Changed for 4.3;

            return EditorGUI.Slider(rect, "", targetVar, minValue, maxValue);
        }

        public static bool StyledToggleButton(bool bToggle,string text)
        {
            return EditorGUILayout.Toggle(text,bToggle, "minibutton");
        }

        public static bool StyledButton(string label)
        {
            EditorGUILayout.Space();
            GUILayoutUtility.GetRect(1, 20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool clickResult = GUILayout.Button(label, "miniButton");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            return clickResult;
        }

        public delegate void OnButtonClick();
        public static void StyledButtonEx(string label, OnButtonClick onClick, ref Rect rect)
        {
            EditorGUILayout.Space();
            GUILayoutUtility.GetRect(1, 20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(label, "miniButton"))
            {
                if (onClick != null)
                {
                    onClick();
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                rect = GUILayoutUtility.GetLastRect();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }


        public static bool StyledToggle(bool value, string label)
        {
            EditorGUILayout.Space();
            GUILayoutUtility.GetRect(1, 20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool clickResult = GUILayout.Toggle(value, label, (GUIStyle)"miniButton");
            //bool clickResult = GUILayout.SelectionGrid(value ? 0 : -1, new Texture[] { Skill.Editor.Resources.UITextures.SelectedEventBorder }, 1) == 0;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            return clickResult;
        }

        public static bool StyledToggle(bool value, GUIContent content)
        {
            EditorGUILayout.Space();
            GUILayoutUtility.GetRect(1, 20);
            EditorGUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            bool clickResult = GUILayout.Toggle(value, content, (GUIStyle)"ButtonMid");
            //bool clickResult = GUILayout.SelectionGrid(value ? 0 : -1, new Texture[] { Skill.Editor.Resources.UITextures.SelectedEventBorder }, 1) == 0;
            //GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            return clickResult;
        }

        public static void StyledTimePeriodShow(string label, ref float current, float min, float max, float minLimit, float maxLimit, int indentLevel, string style = "flow node 2 on")
        {
            int indentSpacing = 25 * indentLevel;
            //indentSpacing += 30;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            float minValueFloat = (float)min;
            float maxValueFloat = (float)max;
            float minLimitFloat = (float)minLimit;
            float maxLimitFloat = (float)maxLimit;

            Rect tempRect = GUILayoutUtility.GetRect(1, 10);
            Rect rect = new Rect(indentSpacing, tempRect.y, Screen.width - indentSpacing - 100 - 20, 20);

            float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
            float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
            float fillWidth = fillRightPos - fillLeftPos;

            // Border
            GUI.Box(rect, "", borderBarStyle);

            // Overlay
            GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), style);

            // Text
            //GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            //centeredStyle.alignment = TextAnchor.UpperCenter;
            //labelStyle.alignment = TextAnchor.UpperCenter;
            //GUI.Label(rect, label + " : " + current + "[" + (minValueFloat) + " - " + (maxValueFloat) + "]", labelStyle);
            // labelStyle.alignment = TextAnchor.MiddleCenter;

            // Slider
            rect.y += 20;
            rect.x = indentLevel * 10;
            rect.width = (Screen.width - (indentLevel * 20) - 100) + 55;

            current = EditorGUI.Slider(rect, (float)current, minLimitFloat, maxLimitFloat);
            tempRect = GUILayoutUtility.GetRect(1, 30);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        public void StyledTimePeriodShow(string label, ref int current, int min, int max, int minLimit, int maxLimit, int indentLevel)
        {
            int indentSpacing = 25 * indentLevel;
            //indentSpacing += 30;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            float minValueFloat = (float)min;
            float maxValueFloat = (float)max;
            float minLimitFloat = (float)minLimit;
            float maxLimitFloat = (float)maxLimit;

            Rect tempRect = GUILayoutUtility.GetRect(1, 10);
            Rect rect = new Rect(indentSpacing, tempRect.y, Screen.width - indentSpacing - 100, 20);

            float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
            float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
            float fillWidth = fillRightPos - fillLeftPos;

            // Border
            GUI.Box(rect, "", borderBarStyle);

            // Overlay
            GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);

            // Text
            //GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            //centeredStyle.alignment = TextAnchor.UpperCenter;
            //labelStyle.alignment = TextAnchor.UpperCenter;
            //GUI.Label(rect, label + " : " + current + "[" + Mathf.Floor(minValueFloat) + " - " + Mathf.Floor(maxValueFloat) + "]", labelStyle);
            //labelStyle.alignment = TextAnchor.MiddleCenter;

            // Slider
            rect.y += 20;
            rect.x = indentLevel * 10;
            rect.width = (Screen.width - (indentLevel * 10) - 100) + 55;

            current = (int)EditorGUI.Slider(rect, (float)current, minLimitFloat, maxLimitFloat);
            tempRect = GUILayoutUtility.GetRect(1, 30);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        public void StyledMinMaxSlider(string label, ref int minValue, ref int maxValue, int minLimit, int maxLimit, int indentLevel)
        {
            int indentSpacing = 25 * indentLevel;
            //indentSpacing += 30;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (minValue < 1) minValue = 1;
            if (maxValue < 2) maxValue = 2;
            if (maxValue > maxLimit) maxValue = maxLimit;
            if (minValue == maxValue) minValue--;
            float minValueFloat = (float)minValue;
            float maxValueFloat = (float)maxValue;
            float minLimitFloat = (float)minLimit;
            float maxLimitFloat = (float)maxLimit;

            Rect tempRect = GUILayoutUtility.GetRect(1, 10);

            Rect rect = new Rect(indentSpacing, tempRect.y, Screen.width - indentSpacing - 100, 20);
            //Rect rect = new Rect(indentSpacing + 15,tempRect.y, Screen.width - indentSpacing - 70, 20);
            float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
            float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
            float fillWidth = fillRightPos - fillLeftPos;

            fillWidth += (rect.width / maxLimitFloat);
            fillLeftPos -= (rect.width / maxLimitFloat);

            // Border
            GUI.Box(rect, "", borderBarStyle);

            // Overlay
            GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);

            // Text
            //GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            //centeredStyle.alignment = TextAnchor.UpperCenter;
            //labelStyle.alignment = TextAnchor.UpperCenter;
            //GUI.Label(rect, label + " between " + Mathf.Floor(minValueFloat) + " and " + Mathf.Floor(maxValueFloat), labelStyle);
            //labelStyle.alignment = TextAnchor.MiddleCenter;

            // Slider
            rect.y += 10;
            rect.x = indentLevel * 10;
            rect.width = (Screen.width - (indentLevel * 10) - 100);

            EditorGUI.MinMaxSlider(rect, ref minValueFloat, ref maxValueFloat, minLimitFloat, maxLimitFloat);
            minValue = (int)minValueFloat;
            maxValue = (int)maxValueFloat;

            tempRect = GUILayoutUtility.GetRect(1, 20);
        }

        public void StyledMarker(string label, int[] locations, int maxValue, int indentLevel)
        {
            if (indentLevel == 1) indentLevel++;
            int indentSpacing = 25 * indentLevel;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Rect tempRect = GUILayoutUtility.GetRect(1, 20);
            Rect rect = new Rect(indentSpacing, tempRect.y, Screen.width - indentSpacing - 60, 20);

            // Border
            GUI.Box(rect, "", borderBarStyle);

            // Overlay
            foreach (int i in locations)
            {
                float xPos = ((rect.width / (float)maxValue) * (float)i) + rect.x;
                if (xPos + 5 > rect.width + rect.x) xPos -= 5;
                GUI.Box(new Rect(xPos, rect.y, 5, rect.height), new GUIContent(), fillBarStyle2);
            }

            // Text
            //GUI.Label(rect, new GUIContent(label), labelStyle);

            tempRect = GUILayoutUtility.GetRect(1, 20);
        }

        public void StyledMarker(string label, Vector2[] locations, int maxValue, int indentLevel, bool fillBounds)
        {
            if (indentLevel == 1) indentLevel++;
            int indentSpacing = 25 * indentLevel;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Rect tempRect = GUILayoutUtility.GetRect(1, 20);
            Rect rect = new Rect(indentSpacing, tempRect.y, Screen.width - indentSpacing - 60, 20);

            // Border
            GUI.Box(rect, "", borderBarStyle);

            if (fillBounds && locations.Length > 0)
            {
                float firstLeftPos = ((rect.width / maxValue) * locations[0].x);
                firstLeftPos -= (rect.width / maxValue);
                GUI.Box(new Rect(rect.x, rect.y, firstLeftPos, rect.height), new GUIContent(), fillBarStyle3);
            }

            // Overlay
            float fillLeftPos = 0;
            float fillRightPos = 0;
            foreach (Vector2 i in locations)
            {
                fillLeftPos = ((rect.width / maxValue) * i.x) + rect.x;
                fillRightPos = ((rect.width / maxValue) * i.y) + rect.x;

                float fillWidth = fillRightPos - fillLeftPos;
                fillWidth += (rect.width / maxValue);
                fillLeftPos -= (rect.width / maxValue);

                GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);
            }

            if (fillBounds && locations.Length > 0)
            {
                float fillWidth = rect.width - fillRightPos + rect.x;
                GUI.Box(new Rect(fillRightPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle4);
            }

            // Text
            //GUI.Label(rect, new GUIContent(label), labelStyle);

            if (fillBounds && locations.Length > 0)
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal(subArrayElementStyle);
                /*{
                    labelStyle.normal.textColor = Color.yellow;
                    skillData.startUpFrames = (skillData.HurtBlocks[0].activeFramesBegin - 1);
                    GUILayout.Label("Start Up: " + skillData.startUpFrames, labelStyle);
                    labelStyle.normal.textColor = Color.cyan;
                    skillData.activeFrames = (skillData.HurtBlocks[skillData.HurtBlocks.Length - 1].activeFramesEnds - skillData.HurtBlocks[0].activeFramesBegin);
                    GUILayout.Label("Active: " + skillData.activeFrames, labelStyle);
                    labelStyle.normal.textColor = Color.red;
                    skillData.recoveryFrames = (skillData.totalFrames - skillData.HurtBlocks[skillData.HurtBlocks.Length - 1].activeFramesEnds + 1);
                    GUILayout.Label("Recovery: " + skillData.recoveryFrames, labelStyle);
                }*/
                GUILayout.EndHorizontal();
            }
            //labelStyle.normal.textColor = Color.white;

            //GUI.skin.label.normal.textColor = new Color(.706f, .706f, .706f, 1);
            tempRect = GUILayoutUtility.GetRect(1, 20);
        }
    }
    public class IGUIElement
    {
        public delegate string dGetName();
        public virtual bool OnGui() { return true; }
        protected dGetName getName;
    }

    public class GUIBlockSpace : IGUIElement
    {
        GUIBlockSpace(int iNum)
        {
            num = iNum;
        }
        public override bool OnGui()
        {
            for(int i = 0; i < num; ++i)
            {
                EditorGUILayout.Space();
            }
            return true;
        }

        protected int num;
    }

    public class GUIElementBlock : IGUIElement
    {
        public GUIElementBlock(GUIStyle style = null,int indent = 0,int beginspace = 0,int endspace = 0)
        {
            this.indent = indent;
            this.style = style;
            this.beginspace = beginspace;
            this.endspace = endspace;
        }

        public override bool OnGui()
        {
            for(int i = 0; i < beginspace; ++i)
            {
                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel += indent;

            for (int i = 0; i < child.Count; ++i)
            {
                IGUIElement children = child[i];
                if (children != null)
                {
                    if( children.OnGui() == false )
                    {
                        return false;
                    }
                }
            }

            EditorGUI.indentLevel -= indent;

            for (int i = 0; i < endspace; ++i)
            {
                EditorGUILayout.Space();
            }

            return true;
        }

        public void SetElementArray(IGUIElement[] childs)
        {
            child = new List<IGUIElement>(childs);
        }

        public void AddElement(IGUIElement  childs)
        {
            child.Add(childs);
        }

        protected List<IGUIElement> child;
        protected int               indent;
        protected GUIStyle          style;
        protected int               beginspace;
        protected int               endspace;
    }

    public class GUIScrollViewBlock : GUIElementBlock
    {
        public GUIScrollViewBlock(GUIStyle style = null, int indent = 0, int beginspace = 0, int endspace = 0) : base(style,indent,beginspace,endspace)
        {
        }

        public override bool OnGui(){
            

            if (style != null)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos,style);
                if( base.OnGui() == false )
                {
                    return false;
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                if (base.OnGui() == false)
                {
                    return false;
                }
                EditorGUILayout.EndScrollView();
            }

            return true;
        }

        protected Vector2 scrollPos;
    }


    public class GUIVerticalBlock : GUIElementBlock
    {
        public GUIVerticalBlock(GUIStyle style = null, int indent = 0, int beginspace = 0, int endspace = 0) : base(style, indent, beginspace, endspace)
        {
        }

        public override bool OnGui()
        {
            if (style != null)
            {
                EditorGUILayout.BeginVertical(style);
                if (base.OnGui() == false)
                {
                    return false;
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical();
                if (base.OnGui() == false)
                {
                    return false;
                }
                EditorGUILayout.EndVertical();
            }

            return true;
        }
    }

    public class GUIHorizontalBlock : GUIElementBlock
    {
        public GUIHorizontalBlock(GUIStyle style = null, int indent = 0, int beginspace = 0, int endspace = 0) : base(style, indent, beginspace, endspace)
        {
        }

        public override bool OnGui()
        {
            if (style != null)
            {
                EditorGUILayout.BeginHorizontal(style);
                if (base.OnGui() == false)
                {
                    return false;
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (base.OnGui() == false)
                {
                    return false;
                }
                EditorGUILayout.EndHorizontal();
            }

            return true;
        }
    }

    public class GUIFlodOutBlock : GUIElementBlock
    {
        public GUIFlodOutBlock(dGetName gname,bool defaultFlod = false,bool bHorizon = false,GUIStyle style = null, int indent = 0, int beginspace = 0, int endspace = 0) : base(style, indent, beginspace, endspace)
        {
            flodout = defaultFlod;
            Horizon = bHorizon;
            getName = gname;
        }

        public override bool OnGui()
        {
            if (Horizon) EditorGUILayout.BeginHorizontal();
            if (style != null)
            {
                flodout = EditorGUILayout.Foldout(flodout, getName(), style);
            }
            else
            {
                flodout = EditorGUILayout.Foldout(flodout, getName());
            }
            if (Horizon) EditorGUILayout.EndHorizontal();

            if (flodout)
            {
                if (base.OnGui() == false)
                {
                    return false;
                }
            }
 

            return true;
        }

        protected bool flodout;
        protected bool Horizon;

    }

    public class GUIElement<T>  : IGUIElement   
    {
        protected delegate bool dSetV(T value);
        protected delegate T dGetV();
        protected T value;
        protected System.Type type;
        protected dSetV _set;
        protected dGetV _get;

        public virtual T GuiElement() { return default(T); }
        public override bool OnGui() {

            if (_set == null || _get == null)
            {
                return true;
            }

            value = GuiElement();

            if (_set(value) == false)
            {
                return false;
            }

            return true;
        }
    }

    public class GUIFloatFiled : GUIElement<float>
    {
        GUIFloatFiled(dGetName getN, dSetV setV, dGetV getV)
        {
            getName = getN;
            _set = setV;
            _get = getV;
        }

        public override float GuiElement(){
            return EditorGUILayout.FloatField(getName(), _get());
        }
    }

    public class GUIIntFiled : GUIElement<int>
    {
        GUIIntFiled(dGetName getN, dSetV setV, dGetV getV)
        {
            getName = getN;
            _set = setV;
            _get = getV;
        }

        public override int GuiElement()
        {
            return EditorGUILayout.IntField(getName(), _get());
        }
    }

    public class GUIStringFiled : GUIElement<string>
    {
        GUIStringFiled(dGetName getN, dSetV setV, dGetV getV)
        {
            getName = getN;
            _set = setV;
            _get = getV;
        }

        public override string GuiElement()
        {
            return EditorGUILayout.TextField(getName(), _get());
        }
    }

    public class GUIIntSliderFiled : GUIElement<int>
    {
        GUIIntSliderFiled(dGetName getN, dSetV setV, dGetV getV,int iMin,int iMax)
        {
            getName = getN;
            _set = setV;
            _get = getV;
            min = iMin;
            max = iMax;
        }

        public override int GuiElement()
        {
            return EditorGUILayout.IntSlider(getName(), _get(),min,max);
        }

        protected int min;
        protected int max;
    }

}