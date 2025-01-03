using UnityEditor;
using UnityEngine;

public class DGridLayoutGroup 
{
    public enum GridState
    {
        normal = 0,
        selected,
        horver
    };
    
    static int ccx,ccy;
    static Texture arrow;
    public delegate void OnGridGUI(int x,int y,Rect rect,GridState state);
    public static void OnGridField(int cell_xnum, int cell_ynum,
                                float cellwidth, float cellheight, float space, float thinkness,OnGridGUI guiCallBack,EditorWindow win)
    {
        float height = cell_ynum * space + space + cell_ynum * cellheight + cell_ynum * thinkness * 2;
        Rect rect = GUILayoutUtility.GetRect(1, height);
        float width = cell_xnum * space + space + cell_xnum * cellwidth  + cell_xnum * thinkness * 2;
        

        float offsetx = rect.xMin + (rect.width / 2) - width / 2;
        float offsety = rect.yMin;

        Rect cellRect;
        Rect fillRect;
 
        
        for (int cy = 0; cy < cell_ynum; ++cy)
        {
            for (int cx = 0; cx < cell_xnum; ++cx)
            {
                float ox = offsetx + cx * (cellwidth + space + thinkness * 2);
                float oy = offsety + cy * (cellheight + space  + thinkness * 2);
                
                Event current = Event.current;
                
                bool bSelected = false;
                
                fillRect = new Rect(ox,oy,cellwidth  + thinkness * 2 + 2 * space,cellheight  + thinkness * 2 + 2 * space);
                cellRect = new Rect(ox + space,oy + space,cellwidth  + thinkness * 2 ,cellheight  + thinkness * 2);
                Rect contentRect = new Rect(ox + space + thinkness,oy + space + thinkness,cellwidth,cellheight);
                if(current.type == EventType.MouseDown)
                {
                    bSelected = cellRect.Contains(current.mousePosition);
                    if(bSelected)
                    {
                        UnityEngine.Debug.Log("Cell" + cx + cy);
                        ccx = cx;
                        ccy = cy;
                        win.Repaint();
                    }
                }
                
                
                //GUI.Box(fillRect,"cell" + cx + cy);
                
                if(current.type == EventType.Repaint)
                {
                    EditorGUI.DrawRect(fillRect,Color.black);
                    EditorGUI.DrawRect(cellRect,((ccx == cx && ccy == cy)) ? Color.yellow : Color.black);
                    EditorGUI.DrawRect(contentRect,Color.gray);    
                }
                
                
                if(guiCallBack != null)
                {
                    guiCallBack(cx,cy,cellRect,GridState.normal);
                }
                
                if(current.type == EventType.MouseDrag)
                {
                    //Handles.DrawArrow(0,current.mousePosition,Quaternion.identity,1.0f);
                    
                    
                    //DragRect = new Rect(current.mousePosition,new Vector2(64,64));
                    //EditorGUI.DrawRect(contentRect,Color.yellow);
                    //win.Repaint();
                    //if(DragAndDrop
                }
                
                if(current.type == EventType.Repaint)
                {
                    if(arrow == null)
                    {
						arrow = Resources.Load("UI/Image/Mission/Mission-28") as Texture;
                    }
                    //EditorGUI.DrawTextureTransparent(DragRect,arrow);
                }
            }
        }
    }
}