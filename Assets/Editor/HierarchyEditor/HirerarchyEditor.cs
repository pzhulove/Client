using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

namespace GameClient
{
    [InitializeOnLoad]
    public class HirerarchyEditorInitializeOnLoad
    {
        static HirerarchyEditorInitializeOnLoad()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            //EditorApplication.projectWindowItemOnGUI += _projectWindowItemOnGUI;
        }


        private static void HierarchyItemCB(int instanceid, Rect selectionrect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceid) as GameObject;
            _updateGameObject(obj, selectionrect);
        }


        private static void _projectWindowItemOnGUI(string guid, Rect selectionrect)
        {
            string assetpath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
            _updateGameObject(obj, selectionrect);
        }


        private static void _updateGameObject(GameObject obj, Rect selectionrect)
        {
            if (obj != null)
            {
                Rect r = new Rect(selectionrect);

                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleRight;

                if (obj.GetComponent<GameFrameWork>() != null)
                {
                    style.normal.textColor = Color.yellow;
                    style.hover.textColor = Color.cyan;
                    GUI.Label(r, "[入口]1.5版本开发分支", style);
                }

                ComGraphicControl graphCtrl = obj.GetComponent<ComGraphicControl>();
                if(graphCtrl != null)
                {
                    style.normal.textColor = Color.yellow;
                    style.hover.textColor = Color.cyan;
                    GUI.Label(r,graphCtrl.getViewString(),style);
                }
                //if (obj.GetComponent<ComGraphicControl>() != null)
                {
                    ComCommonBind bind = obj.GetComponent<ComCommonBind>();

                    if (bind != null)
                    {
                        var nstyle = new GUIStyle(GUI.skin.label);

                        nstyle.normal.textColor = Color.yellow;
                        nstyle.hover.textColor = Color.cyan;
                        nstyle.alignment = TextAnchor.MiddleLeft;

                        Rect nr = new Rect(0, r.y, r.width, r.height);

                        GUI.Label(nr, string.Format("[bind {0}]({1})", bind.units.Length, obj.transform.childCount), nstyle);
                    }
                }

                if (obj.name.CompareTo("UIRoot") == 0)
                {
                    style.normal.textColor = Color.yellow;
                    style.hover.textColor = Color.cyan;
                    GUI.Label(r, "[UI]1.5版本开发分支◉‿◉", style);
                }

                {
                    ComClientFrame client = obj.GetComponent<ComClientFrame>();

                    if (client != null)
                    {
                        style.normal.textColor = Color.green;
                        style.hover.textColor = Color.cyan;
                        GUI.Label(r, string.Format("[{0}]({1})", client.mGroupTag, obj.transform.childCount), style);
                    }
                }

                {
                    CPooledGameObjectScript poolStcipt = obj.GetComponent<CPooledGameObjectScript>();
                    if(null != poolStcipt)
                    {
                        string tag = "";
                        if (poolStcipt.m_IsRecycled)
                        {
                            style.normal.textColor = Color.green;
                            style.hover.textColor = Color.cyan;
                            tag = "POOL [Recycled!]";
                        }
                        else
                        {
                            style.normal.textColor = Color.yellow;
                            style.hover.textColor = Color.red;
                            tag = "POOL [Occupied!]";
                        }
                        GUI.Label(r, tag, style);
                    }
                }

            }
        }

        private static void _updateGameObjectChildCount(GameObject obj, GUIStyle style, Rect r)
        {
            if (null == obj)
            {
                return ;
            }

            style.normal.textColor = Color.yellow;
            style.hover.textColor = Color.cyan;
            GUI.Label(r, string.Format("[{0}]", obj.transform.childCount), style);
        }
    }
}
