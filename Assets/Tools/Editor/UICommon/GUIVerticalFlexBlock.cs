using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace EditorUI
{
    public class GUIVerticalFlexBlock
    {
        private static int VerticalFlexBlockHash = "VerticalFlexBlock".GetHashCode();
        private int ID;
        private static int Margin = 6;
        private int initpos;
        private int initheight;
        private float height;
        private float minHeight;

        public GUIVerticalFlexBlock(float height,float minHeight = 50)
        {
            this.height = height;
            this.minHeight = minHeight;
        }
          
        public float OnLayoutGUI()
        {
            if (height < minHeight)
            {
                height = minHeight;
            }
            Rect rect = GUILayoutUtility.GetRect(1, 1);
            rect.height = height;
            ID = GUIUtility.GetControlID(VerticalFlexBlockHash, FocusType.Passive);
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && Event.current.clickCount == 1)
                    {
                        int num = ((int)rect.y + (int)rect.height) - Margin / 2;
                        int num2 = num + Margin / 2;

                        if(new Rect(rect.x,num,rect.width,Margin).Contains(Event.current.mousePosition))
                        {
                            GUIUtility.hotControl =  ID;
                            initpos = (int)Event.current.mousePosition.y;
                            initheight = (int)height;
                            Event.current.Use();
                            break;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl ==  ID)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl ==  ID)
                    {
                        int num2 = ((int)Event.current.mousePosition.y);
                        int num3 = num2 - initpos;
                        
                        if(num3 != 0)
                        {
                            height = initheight + num3;
                            if(height < minHeight)
                            {
                                height = minHeight;
                            }
                        }
                        Event.current.Use();
                    }
                    break;
                case EventType.Repaint:
                    {
                        int num = ((int)rect.y + (int)rect.height) - Margin / 2;
                        int num2 = num + Margin / 2;
                        EditorGUIUtility.AddCursorRect(new Rect(rect.x, num, rect.width, Margin), MouseCursor.SplitResizeUpDown,ID);
                        break;
                    }
                case EventType.Layout:
                    {
                        //SetHorizontal(ref state, 0, rect.width);
                        break;
                    }
            }

            return height;
        }
    }
}
