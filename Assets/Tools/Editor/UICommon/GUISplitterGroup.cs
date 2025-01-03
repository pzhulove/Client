using UnityEditor;
using UnityEngine;

namespace EditorUI
{
    public class SplitterGUILayout
    {
        static public void SetHorizontal(ref SplitterState state, float x, float width)
        {
            state.xOffset = x;
            int i;
            if ( (int)width != state.lastTotalSize )
            {
                state.RelativeToRealSizes((int)width);
                state.lastTotalSize = (int)width;
                for (i = 0; i < state.realSizes.Length - 1; i++)
                {
                    state.DoSplitter(i, i + 1, 0);
                }
            }
        }

        private static int splitterHash = "EditorUISplitter".GetHashCode();
        public static void BeginHorizontalSplit(ref SplitterState state, Rect rect,float titleHeight)
        {
            Rect crect = rect;
            rect.height = titleHeight;
            GUI.BeginGroup(crect);
            state.ID = GUIUtility.GetControlID(SplitterGUILayout.splitterHash, FocusType.Passive);
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && Event.current.clickCount == 1)
                    {
                        int num = 0;//((int)rect.x)  ;
                        int num2 =  ((int)Event.current.mousePosition.x);
                        for (int i = 0; i < state.relativeSizes.Length - 1; i++)
                        {
                            if ( new Rect(state.xOffset + (float)num + (float)state.realSizes[i] - (float)(state.splitSize / 2), 
                                 0, (float)state.splitSize, rect.height).Contains(Event.current.mousePosition) )
                            {
                                state.splitterInitialOffset = num2;
                                state.currentActiveSplitter = i;
                                GUIUtility.hotControl = state.ID;
                                Event.current.Use();
                                break;
                            }
                            num += state.realSizes[i];
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == state.ID)
                    {
                        GUIUtility.hotControl = 0;
                        state.currentActiveSplitter = -1;
                        state.RealToRelativeSizes();
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == state.ID && state.currentActiveSplitter >= 0)
                    {
                        int num2 =  ((int)Event.current.mousePosition.x);
                        int num3 = num2 - state.splitterInitialOffset;
                        if (num3 != 0)
                        {
                            state.splitterInitialOffset = num2;
                            state.DoSplitter(state.currentActiveSplitter, state.currentActiveSplitter + 1, num3);
                        }
                        Event.current.Use();
                    }
                    break;
                case EventType.Repaint:
                    {
                        SetHorizontal(ref state, 0, rect.width);
                        int num4 = 0;//((int)rect.x);
                        for (int j = 0; j < state.relativeSizes.Length - 1; j++)
                        {
                            Rect position = new Rect(state.xOffset + (float)num4 + (float)state.realSizes[j] - (float)(state.splitSize / 2),  0, (float)state.splitSize, rect.height);

                            EditorGUIUtility.AddCursorRect(position,  MouseCursor.SplitResizeLeftRight , state.ID);
                            num4 += state.realSizes[j];
                        }
                        break;
                    }
                case EventType.Layout:
                    {
                        //SetHorizontal(ref state, 0, rect.width);
                        break;
                    }
            }
        }
        
       
        public static void EndHorizontalSplit()
        {
            GUI.EndGroup();
        }
    }
}
