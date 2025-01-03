using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIAssistantWindow : EditorWindow
{
    private int defaultDepth = UIAssistantTools.DefaultDepth;
    private int depthIdx = 1;
    private GameObject m_selectObj = null;
    private TreeNode m_treeRootNode = null;
    private Vector2 m_ScrollPos = Vector2.zero;
    private Color[] m_colors = new Color[6] { Color.red, Color.yellow, Color.blue, Color.green, Color.black, Color.white };

    void OnGUI()
    {
        GUILabelType();
        GUILayout.Label("UIAssistant");

        GUILabelType(TextAnchor.UpperLeft);
        GUILayout.Space(2);
        CreateSplit();
        
        GUILayout.BeginHorizontal();
        for(int i = 0; i < m_colors.Length; ++i) 
        {
            GUI.color = m_colors[i];
            GUILayout.Label(string.Format("{0} : ", i));
            GUILayout.Box("", GUILayout.Width(15), GUILayout.Height(15));
            GUILayout.Space(140);
        }
        GUILayout.EndHorizontal();

        GUILabelType(TextAnchor.UpperLeft);
        GUILayout.Space(2);
        CreateSplit();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Catch"))
        {
            Catch();
        }
        if (GUILayout.Button("Refresh"))
        {
            Refresh();
        }
        if (GUILayout.Button("Clear"))
        {
            Reset();
        }
        GUILayout.EndHorizontal();

        GUILabelType(TextAnchor.UpperLeft);
        GUILayout.Space(2);
        CreateSplit();

        ShowCatchUI();
    }

    private void ShowCatchUI() 
    {
        if (m_selectObj != null) 
        {
            GUILabelType(TextAnchor.UpperLeft);
            GUILayout.Space(2);
            GUILayout.Label(m_treeRootNode == null ? "Result: " : string.Format("Result: {0}({1})", m_treeRootNode.batchCount, m_treeRootNode.maskBatchCount));
            GUILayout.Space(2);

            m_ScrollPos = GUI.BeginScrollView(new Rect(10, 140, 800, position.height - 160), m_ScrollPos, new Rect(0, 0, m_treeRootNode.RecursiveSize.x, m_treeRootNode.RecursiveSize.y), true, true);
            m_treeRootNode.OnGUI();
            GUI.EndScrollView();
        }
    }

    private void Catch()
    {
        if(Selection.activeGameObject == null) 
        {
            EditorUtility.DisplayDialog("Tips", "Select Object is null!", "close");
            return;
        }

        if (Selection.activeGameObject.layer == LayerMask.NameToLayer("UI")) 
        {
            m_selectObj = Selection.activeGameObject;
            Refresh();
        }
    }

    private void Refresh()
    {
        if (m_treeRootNode != null)
        {
            m_treeRootNode.Destroy();
        }

        if (m_selectObj != null)
        {
            depthIdx = 1;
            m_treeRootNode = new TreeNode(m_selectObj.name, m_selectObj, depthIdx * defaultDepth);
            m_treeRootNode.IsRoot = true;
            GenChildNodes(m_treeRootNode, m_selectObj.transform);
            UIAssistantTools.GenTreeInfo(m_treeRootNode);
        }
    }

    private void GenChildNodes(TreeNode node, Transform transform)
    {
        if(transform.childCount > 0) 
        {
            int depth = 0;
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                /*var graphic = child.GetComponent<Graphic>();
                var image = child.GetComponent<Image>();
                var imageEx = child.GetComponent<ImageEx>();
                var imageNull = child.GetComponent<NullImage>();
                var text = child.GetComponent<Text>();
                bool flag = true;
                /*if (graphic != null)
                {
                    var type = typeof(Graphic);
                    var fieldInfo = type.GetField("s_VertexHelper", BindingFlags.NonPublic | BindingFlags.Static);
                    if (fieldInfo != null)
                    {
                        var retObj = fieldInfo.GetValue(graphic) as VertexHelper;
                        if (retObj != null)
                        {
                            var type2 = typeof(VertexHelper);
                            var fieldInfo2 = type2.GetField("m_Positions", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
                            if (fieldInfo2 != null)
                            {
                                var retObj2 = fieldInfo2.GetValue(retObj) as List<Vector3>;
                                if (retObj2 != null)
                                {
                                    if(retObj2.Count == 0)
                                        flag = false;
                                }
                            }
                        }
                    }
                }
                if (image != null && !image.enabled)
                {
                    flag = false;
                }
                if (imageEx != null)
                {
                    if ((imageEx.overrideSprite != null && imageEx.overrideSprite.texture == null)
                        || (imageEx.overrideSprite == null && !imageEx.IsEnableWhiteImage && imageEx.color == Color.white) || imageEx.color.a == 0)
                    {
                        flag = false;
                    }
                }
                if (imageNull != null)
                {
                    flag = false;
                }
                if (text != null && !text.enabled)
                {
                    flag = false;
                }*/
                if(child.gameObject.activeSelf) 
                {
                    depth = node.Depth + 1;
                    if (child.GetComponent<Canvas>() != null) 
                    {
                        ++depthIdx;
                        depth = depthIdx * defaultDepth;
                    }
                    TreeNode childNode = new TreeNode(child.name, child.gameObject, depth);
                    GenChildNodes(childNode, child);
                    node.AddChild(childNode);
                }
            }
        }
    }

    private void Reset()
    {
        m_selectObj = null;
        m_treeRootNode = null;
        m_ScrollPos = Vector2.zero;
    }

    public GUIStyle GUILabelType(TextAnchor anchor = TextAnchor.UpperCenter)
    {
        GUIStyle labelstyle = GUI.skin.GetStyle("Label");
        labelstyle.alignment = anchor;
        return labelstyle;
    }

    public void CreateSplit()
    {
        GUILayout.Label("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
    }
}