using UnityEditor;
using UnityEngine;

namespace TM.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.Experimental.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine.UI;


    public static class HirerarchyEditorInitializeOnLoadHelper
    {
        public delegate bool FilterCondition(Component com);
        public static GameObject[] ConvertFromComs(this IList<Component> coms, FilterCondition condition = null)
        {
            if (null == coms)
            {
                return new GameObject[0];
            }

            List<GameObject> gos = new List<GameObject>(coms.Count);

            for (int i = 0; i < coms.Count; ++i)
            {
                if (null != condition)
                {
                    if (condition(coms[i]))
                    {
                        gos.Add(coms[i].gameObject);
                    }
                }
                else
                {
                    gos.Add(coms[i].gameObject);
                }
            }

            return gos.ToArray();
        }
    }

    [InitializeOnLoad]
    public class UGUIOptimizeTool
    {
        static UGUIOptimizeTool()
        {
            EditorApplication.hierarchyWindowItemOnGUI += _HierarchyWindowItemOnGUI;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.update += _OnUpdate;
        }

        public static void _OnUpdate()
        {
            mHierarchyNodeManager.OnUpdate();
        }

        #region hierarchy窗口的回调
        static string SceneName = string.Empty;
        public static void OnHierarchyChanged()
        {
            //UnityEngine.Debug.LogFormat("OnHierarchyChanged");
            //从prefab预览模式切回普通场景
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                mNode = null;
                mRootNodeInstanceId = 0;
                SceneName = string.Empty;
            }
            else
            {
                //prefab预览模式里的prefab变化了
                if (string.CompareOrdinal(SceneName, PrefabStageUtility.GetCurrentPrefabStage().scene.name) != 0)
                {
                    SceneName = PrefabStageUtility.GetCurrentPrefabStage().scene.name;
                    mNode = null;
                    mRootNodeInstanceId = 0;
                }
            }
            mHierarchyNodeManager.ClearInvalids();
        }

        public interface IHierarchyNodeManager
        {
            HierarchyNode FindHierarchyNode(int id);
            HierarchyNode AddHierarchyNodes(int id);
        }

        public class HierarchyNodeManager : IHierarchyNodeManager
        {
            private Dictionary<int, HierarchyNode> mHierarchyNodes = new Dictionary<int, HierarchyNode>();

            public HierarchyNode FindHierarchyNode(int id)
            {
                if (!mHierarchyNodes.ContainsKey(id))
                {
                    return null;
                }

                return mHierarchyNodes[id];
            }

            public GameObject[] GetHierarchyRootNode()
            {
                List<GameObject> gos = new List<GameObject>(mHierarchyNodes.Count);

                foreach (var kv in mHierarchyNodes)
                {
                    gos.Add(kv.Value.GO);
                }

                return gos.ToArray();
            }

            public HierarchyNode AddHierarchyNodes(int id)
            {
                HierarchyNode node = FindHierarchyNode(id);
                if (null != node)
                {
                    return node;
                }

                var obj = EditorUtility.InstanceIDToObject(id);
                node = new HierarchyNode(id, obj, this);
                mHierarchyNodes.Add(id, node);
                return node;

            }

            public void ClearInvalids()
            {
                List<int> removeKeys = new List<int>();
                var iter = mHierarchyNodes.GetEnumerator();
                foreach (var curKV in mHierarchyNodes)
                {
                    bool remove = false;

                    if (null == curKV.Value)
                    {
                        remove = true;
                    }
                    else if (curKV.Value.Object == null)
                    {
                        remove = true;
                        curKV.Value.Clear();
                    }

                    if (remove)
                    {
                        removeKeys.Add(curKV.Key);
                    }
                }

                foreach (var id in removeKeys)
                {
                    mHierarchyNodes.Remove(id);
                }
            }

            public void OnUpdate()
            {
                foreach (var kv in mHierarchyNodes)
                {
                    kv.Value.UpdateExpand();
                }
            }
        }

        private static HierarchyNodeManager mHierarchyNodeManager = new HierarchyNodeManager();

        public class BaseHierarchyNode
        {
            float mContentUsedWidth = 0;

            protected void _DrawDesc(GUIStyle style, Rect board, string str)
            {
                Rect curRect = _GetContentRect(style, board, string.Format(" {0} ", str), 0);

                GUI.Label(curRect, str, style);

                _AddContentSpace(0.5f);
            }

            protected bool _DrawDescButton(GUIStyle style, Rect board, string str)
            {
                Rect curRect = _GetContentRect(style, board, string.Format(" {0} ", str), 0);

                bool hasClick = (GUI.Button(curRect, ""));
                
                GUI.Label(curRect, str, style);

                _AddContentSpace(0.5f);

                return hasClick;
            }


            protected Rect _GetContentRect(GUIStyle style, Rect board, string str, float height)
            {
                GUIContent content = new GUIContent(str);

                var size = style.CalcSize(content);

                mContentUsedWidth += size.x;

                return new Rect(board.xMax - mContentUsedWidth, board.y, size.x, board.height);
            }

            protected void _AddContentSpace(float space)
            {
                mContentUsedWidth += space;
            }

            protected void _ResetContent()
            {
                mContentUsedWidth = 0;
            }
        }

        public class HierarchyNode : BaseHierarchyNode
        {
            public IHierarchyNodeManager Manager { get; private set; }
            private Canvas FrameRoot = null;
            private GameObject[] mGraphics = new GameObject[0];
            private List<GameObject> mNullSprites = new List<GameObject>();
            private List<GameObject> mRichTexts = new List<GameObject>();
            //private TreeNode mDrawCallNodes = null;
            //private int defaultDepth = UIAssistantTools.DefaultDepth;


            //private UIFrameAttribute mUIFrameAttr = null;
            private string FrameName;


            public int ID { get; private set; }
            public Object Object { get; private set; }


            public HierarchyNode(int id, UnityEngine.Object obj, IHierarchyNodeManager mng)
            {
                Manager = mng;
                ID = id;
                Object = obj;
                _InitWithType();
            }

            public void Clear()
            {
                Manager = null;
                FrameRoot = null;
                mGraphics = null;
                ID = -1;
                Object = null;
            }

            public GameObject GO
            {
                get
                {
                    return Object as GameObject;
                }
            }

            private void _InitWithType()
            {
                if (null == GO)
                {
                    return;
                }

                FrameRoot = GO.GetComponent<Canvas>();
                _RefreshRaycastTarget();
                _RefreshNullImages();
                _RefreshRichText();
            }

            private bool _FilterGraphicCondition(Component coms)
            {
                Graphic g = coms as Graphic;

                if (null != g)
                {
                    return g.raycastTarget;
                }

                return false;
            }

            public int RichTextCount
            {
                get
                {
                    if (null == mRichTexts)
                    {
                        return 0;
                    }

                    return mRichTexts.Count;
                }
            }

            public int RayCastCountInCludeChild
            {
                get
                {
                    if (null == mGraphics)
                    {
                        return 0;
                    }

                    return mGraphics.Length;
                }
            }

            public int NullSpriteImageCount
            {
                get
                {
                    return mNullSprites.Count;
                }
            }

            public void OnGUI(Rect rect)
            {
                _ResetContent();
                _OnFrameRootGUI(rect);
            }

            public enum EFunction
            {
                None,
                RaycastTarget,
                NullImage,
                RichText,
                DrawCall
            }
            private EFunction mSelectFunction;
            private bool mIsShowDrop = false;
            private bool mIsSelect = false;
            private bool[] mIsFunctionEnable = new bool[(int)EFunction.DrawCall + 1];
            List<GameObject> mSelectGos = new List<GameObject>();

            private void _OnFrameRootGUI(Rect rect)
            {
                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;

                style.hover.textColor = Color.cyan;
                style.normal.textColor = Color.yellow;
                var str = "<=点击展开";
                if (mIsShowDrop)
                {
                    str = "点击隐藏";
                }
                _AddContentSpace(0.5f);

                if (_DrawDescButton(style, rect, string.Format(str)))
                {
                    mIsShowDrop = !mIsShowDrop;
                }
                List<Object> selectObjects = new List<Object>();

                if (mIsShowDrop)
                {
                    if (_DrawDropButton(style, ref rect, string.Format("RayCastTarget:{0}", RayCastCountInCludeChild), EFunction.RaycastTarget))
                    {
                        if (mIsFunctionEnable[(int)EFunction.RaycastTarget])
                        {
                            _RefreshRaycastTarget();
                        }
                        _SelectionObjs();
                    }

                    if (_DrawDropButton(style, ref rect, string.Format("NullImage:{0}", NullSpriteImageCount), EFunction.NullImage))
                    {
                        _RefreshNullImages();
                        _SelectionObjs();
                    }

                    if (_DrawDropButton(style, ref rect, string.Format("RichText:{0}", RichTextCount), EFunction.RichText))
                    {
                        if (mIsFunctionEnable[(int)EFunction.RichText])
                        {
                            _RefreshRichText();
                        }
                        _SelectionObjs();
                    }

                    if (_DrawDropButton(style, ref rect, string.Format("DrawCall"), EFunction.DrawCall))
                    {

                    }
                }
            }

            private void _SelectionObjs()
            {
                if (mIsFunctionEnable == null)
                {
                    return;
                }

                if (mSelectGos == null)
                {
                    mSelectGos = new List<GameObject>();
                }

                mSelectGos.Clear();

                if (mIsFunctionEnable[(int)EFunction.RaycastTarget])
                {
                    mSelectGos.AddRange(mGraphics.ToList());
                }

                if (mIsFunctionEnable[(int)EFunction.NullImage])
                {
                    mSelectGos.AddRange(mNullSprites);
                }

                if (mIsFunctionEnable[(int)EFunction.RichText])
                {
                    mSelectGos.AddRange(mRichTexts);
                }

                if (mIsFunctionEnable[(int)EFunction.DrawCall])
                {

                }

                Selection.objects = mSelectGos.ToArray();
            }

            private void _RefreshRaycastTarget()
            {
                mGraphics = GO.GetComponentsInChildren<Graphic>(true).ConvertFromComs(_FilterGraphicCondition);
            }

            private void _RefreshNullImages()
            {
                var images = GO.GetComponentsInChildren<Image>(true);
                mNullSprites.Clear();
                for (int i = 0; i < images.Length; ++i)
                {
                    var image = images[i].GetComponent<Image>();
                    if (image.sprite == null && image.overrideSprite == null)
                    {
                        mNullSprites.Add(images[i].gameObject);
                    }
                }
            }

            private void _RefreshRichText()
            {
                mRichTexts.Clear();
                var texts = GO.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < texts.Length; ++i)
                {
                    var text = texts[i].GetComponent<Text>();
                    if (text.supportRichText)
                    {
                        mRichTexts.Add(texts[i].gameObject);
                    }
                }
            }

            //没有用 ExpandTreeViewItem应该只是展开一级 不是展开所有
            private void _Expand(bool value, GameObject root)
            {
                var wnd = EditorWindow.focusedWindow;
                if (wnd.GetType().Name != "SceneHierarchyWindow")
                    return;
                var filed = wnd.GetType().GetField("m_SceneHierarchy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (filed == null)
                    return;

                var treeView = filed.FieldType.GetField("treeView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (treeView == null)
                {
                    //Dbg.Error(wnd, "Could not find method 'UnityEditor.SceneHierarchyWindow.ExpandTreeViewItem(int, bool)'.");
                    return;
                }

                var tv = treeView.FieldType.GetField("data", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var method = tv.FieldType.GetMethod("ExpandTreeViewItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(int), typeof(bool) }, null);

            }
            private int ExpandState = -1;
            public void UpdateExpand()
            {
                //var wnd = EditorWindow.focusedWindow;
                //if (wnd.GetType().Name != "SceneHierarchyWindow")
                //    return;
                //if (ExpandState == 0)
                //{
                //    var key = new Event { keyCode = KeyCode.LeftArrow, type = EventType.KeyDown, modifiers = EventModifiers.Alt };
                //    wnd.SendEvent(key);

                //    //key = new Event { keyCode = KeyCode.LeftArrow, type = EventType.KeyUp};
                //    //wnd.SendEvent(key);

                //    //key = new Event { keyCode = KeyCode.RightArrow, type = EventType.KeyDown, modifiers = EventModifiers.Alt };
                //    //wnd.SendEvent(key);
                //    ExpandState = 1;
                //}
                ////需要等到缩回的动画播放完，但好像没有接口 50帧不一定够
                //else if (ExpandState >= 1 && ExpandState < 50)
                //{
                //    //var key = new Event { keyCode = KeyCode.LeftArrow, type = EventType.KeyUp, modifiers = EventModifiers.Alt };
                //    //wnd.SendEvent(key);
                //    ExpandState++;
                //}
                //else if (ExpandState == 500)
                //{
                //    var key = new Event { keyCode = KeyCode.RightArrow, type = EventType.KeyDown, modifiers = EventModifiers.Alt };
                //    wnd.SendEvent(key);
                //    ExpandState = -1;
                //}
                //else if (ExpandState == 3)
                //{
                //    var key = new Event { keyCode = KeyCode.RightArrow, type = EventType.KeyDown };
                //    wnd.SendEvent(key);
                //    ExpandState = -1;
                //}


            }

            public void OnSubNodesGUI(int id, Rect rect)
            {
                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;

                style.hover.textColor = Color.cyan;
                style.normal.textColor = Color.yellow;
                float usedWidth = 0;
                if (mIsFunctionEnable[1])
                {
                    _DrawSubNode(id, mGraphics, "<color=white>事件</color>", style, ref usedWidth, rect);
                }

                if (mIsFunctionEnable[2])
                {
                    _DrawSubNode(id, mNullSprites, "空图", style, ref usedWidth, rect);
                }
                if (mIsFunctionEnable[3])
                {
                    _DrawSubNode(id, mRichTexts, "富文本", style, ref usedWidth, rect);
                }
            }

            private void _DrawSubNode(int id, IList<GameObject> list, string str, GUIStyle style, ref float usedWidth, Rect rect)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (id == list[i].GetInstanceID())
                    {
                        GUIContent content = new GUIContent(str);
                        var size = style.CalcSize(content);
                        usedWidth += size.x + 3f;
                        var board = new Rect(rect.xMax - usedWidth, rect.y, size.x, rect.height);
                        GUI.Label(board, str, style);
                        break;
                    }
                }
            }

            private bool _DrawDropButton(GUIStyle style, ref Rect board, string str, EFunction func)
            {
                Rect curRect = _GetContentRect(style, board, string.Format(" {0} ", str), 0);

                bool hasClick = (GUI.Button(curRect, ""));


                _AddContentSpace(0.5f);
                if (hasClick)
                {
                    mSelectFunction = func;
                    //mCurrentSelectFunction = "当前:" + str;
                    mIsFunctionEnable[(int)func] = !mIsFunctionEnable[(int)func];
                }

                if (mIsFunctionEnable[(int)func])
                {
                    str = "<color=#00EC01>" + str + "</color>";
                }
                else
                {
                    str = "<color=white>" + str + "</color>";
                }

                GUI.Label(curRect, str, style);

                return hasClick;
            }

        }

        private static HierarchyNode mNode;
        private static int mRootNodeInstanceId;
        private static void _HierarchyWindowItemOnGUI(int id, Rect selectionrect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(id) as GameObject;

            if ((mNode == null || mRootNodeInstanceId == id) && go != null && go.GetComponent<Canvas>() != null)
            {
                mRootNodeInstanceId = id;
                mNode = mHierarchyNodeManager.AddHierarchyNodes(id);
                if (null != mNode)
                {
                    mNode.OnGUI(selectionrect);
                }
            }

            if (null != mNode)
            {
                mNode.OnSubNodesGUI(id, selectionrect);
            }

        }
        #endregion

    }
}
