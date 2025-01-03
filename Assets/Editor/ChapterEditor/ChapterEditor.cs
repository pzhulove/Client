using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;
using ProtoTable;
using System.Linq;


[CustomEditor(typeof(DChapterData))]
public class DChapterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("open"))
        {
            ChapterEditor.Init();
        }
    }
}


[CustomEditor(typeof(DChapterData))]

public class ChapterEditor : EditorWindow
{

    [MenuItem("[关卡编辑器]/关卡/下一个兄弟节点 &e")]
    public static void Next()
    {
        var go = Selection.activeGameObject;

        if (go != null)
        {
            var pa = go.transform.parent;
            var cnt = pa.transform.childCount;

            var idx = go.transform.GetSiblingIndex();

            idx++;
            idx = Mathf.Clamp(idx, 0, cnt - 1);

            var se = pa.transform.GetChild(idx);
            Selection.activeGameObject = se.gameObject;
        }
    }

    [MenuItem("[关卡编辑器]/关卡/上一个兄弟节点 &w")]
    public static void Last()
    {
        var go = Selection.activeGameObject;

        if (go != null)
        {
            var pa = go.transform.parent;
            var cnt = pa.transform.childCount;

            var idx = go.transform.GetSiblingIndex();

            idx--;
            idx = Mathf.Clamp(idx, 0, cnt - 1);

            var se = pa.transform.GetChild(idx);
            Selection.activeGameObject = se.gameObject;
        }
    }

    [MenuItem("[关卡编辑器]/关卡/创建关卡数据")]
    public static void Create()
    {
        var pinObject = FileTools.CreateAsset<DChapterData>("ChapterData");

        EditorGUIUtility.PingObject(pinObject);
        Selection.activeObject = pinObject;
    }


    public static ChapterEditor mEditorWindow;

    [MenuItem("[关卡编辑器]/关卡/关卡数据编辑*")]
    public static void Init()
    {
        mEditorWindow = EditorWindow.GetWindow<ChapterEditor>(false, "chapter", true);
        mEditorWindow.autoRepaintOnSceneChange = true;
        mEditorWindow.ShowUtility();
        //mEditorWindow.Show();
    }

    private DChapterData mData = null;

    private bool mIsView = false;
    private int mChapterID = 1;


    private GameObject mRoot = null;
    private GameObject mRootTop = null;


    private void _loadEnv()
    {
        var root = Utility.FindGameObject("UIRoot", false);
        if (root != null)
        {
            GameObject.DestroyImmediate(root);
            root = null;
        }

        root = AssetLoader.instance.LoadResAsGameObject("Base/UI/Prefabs/Root/UIRoot");
        root.SetActive(true);
        root.name = "UIRoot";
        mRoot = root;

        mRootTop = Utility.FindGameObject(root, "UI2DRoot/Top", false);
    }

    private GameObject mMiddleGo = null;
    private GameObject mChapterNodeRoot = null;
    private Image mChapterBackground = null;

    private void _loadChapter()
    {
        GameObject chapter = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Chapter/Select/ChapterSelect");
        Utility.AttachTo(chapter, mRootTop);

        ComCommonBind bind = chapter.GetComponent<ComCommonBind>();

        mChapterNodeRoot = bind.GetGameObject("NodeRoot");

        mChapterBackground = bind.GetCom<Image>("LeftBackground");
        // mChapterBackground.sprite = _loadSprite(mData.backgroundPath);
        _loadSprite(ref mChapterBackground, mData.backgroundPath);

        Text ChapterName = bind.GetCom<Text>("ChapterName");
        ChapterName.text = mData.name;

        Image ChapterNameImage = bind.GetCom<Image>("ChapterNameImage");
        // ChapterNameImage.sprite = AssetLoader.instance.LoadRes(mData.namePath, typeof(Sprite)).obj as Sprite;
        ETCImageLoader.LoadSprite(ref ChapterNameImage, mData.namePath);
        ChapterNameImage.SetNativeSize();

    }

    private void _unloadEnv()
    {
        mMiddleGo = null;
        mRootTop = null;
        GameObject.DestroyImmediate(mRoot);
        mRoot = null;
        mNodes = new Node[0];
    }

    //private Sprite _loadSprite(string path)
    //{
    //    return AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
    //}
    private void _loadSprite(ref Image img, string path)
    {
        // return AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
        ETCImageLoader.LoadSprite(ref img, path);
    }

    private bool InitFiledAfterEnter(string label, ref int len, int left, int right)
    {
        var tlen = Mathf.Clamp(EditorGUILayout.IntField(label, len), left, right);
        if (tlen != len)
        {
            //if (Event.current.isKey && Event.current.keyCode.ToString() == "Return")
            {
                len = tlen;
                return true;
            }
        }

        return false;
    }

    public void OnFocus()
    {
    }
    public void OnLostFocus()
    {
    }

    public void OnSelectedChange()
    {
        //EditorUtility.SetDirty(mData);
        //Repaint();
    }

    public void OnHierarchyChange()
    {
        //EditorUtility.SetDirty(mData);
        //Repaint();
    }

    private Vector2 mScroll;
    private Dictionary<int, List<DungeonTable>> mCacheDungeonTable = new Dictionary<int, List<DungeonTable>>();

    private int[] mCacheDungeonKey = new int[0];
    private string[] mCacheDungeonKeyString = new string[0];

    private string[] mCacheDungeonString = new string[0];
    private int mChSelectId = 1;
    private int mChSelect = -1;

    private void _clearNodes()
    {
        foreach (var item in mNodes)
        {
            GameObject.DestroyImmediate(item.go);
            item.go = null;
        }
        mNodes = new Node[0];
    }

    private void _insertNode(int idx)
    {
        List<ChaptertDungeonUnit> list = new List<ChaptertDungeonUnit>(mData.chapterList);
        ChaptertDungeonUnit unit = new ChaptertDungeonUnit();
        unit.dungeonID = list[idx].dungeonID;
        unit.position = list[idx].position;
        list.Insert(idx, unit);

        mData.chapterList = list.ToArray();

        _clearNodes();
    }

    private void _deleteNode(int idx)
    {
        List<ChaptertDungeonUnit> list = new List<ChaptertDungeonUnit>(mData.chapterList);
        list.RemoveAt(idx);

        mData.chapterList = list.ToArray();

        _clearNodes();
    }


    private void _loadTable()
    {
        mCacheDungeonTable.Clear();

        var slist = new List<string>();
        var node = TableManagerEditor.instance.GetTable<DungeonTable>();
        DungeonID id = new DungeonID(0);
        foreach (var kv in node)
        {
            var tb = kv.Value as DungeonTable;
            id.dungeonID = tb.ID;
            // marked by ckm
            // if (id.diffID == 0)
            // {
                if (!mCacheDungeonTable.ContainsKey(id.chapterID))
                {
                    mCacheDungeonTable.Add(id.chapterID, new List<DungeonTable>());
                }

                mCacheDungeonTable[id.chapterID].Add(tb);
            // }
        }

        mCacheDungeonKey = mCacheDungeonTable.Keys.ToArray();
        var list = new List<string>();
        foreach (var item in mCacheDungeonKey)
        {
            list.Add(item.ToString());
        }
        mCacheDungeonKeyString = list.ToArray();

        _updateDungeonString();
    }

    private void _updateDungeonString()
    {
        var keys = mCacheDungeonKey;
        var chID = keys[mChSelectId];

        if (!mCacheDungeonTable.ContainsKey(chID))
        {
            return;
        }

        mChSelect = chID;

        var slist = new List<string>();
        foreach(var kv in mCacheDungeonTable[chID])
        {
            slist.Add(string.Format("{0}:{1}", kv.ID, kv.Name));
        }

        mCacheDungeonString = slist.ToArray();
    }

    private void _popDungeonSelect(ref int dungeonId, ref int selectId)
    {
        int fid = dungeonId;

        if(mCacheDungeonTable.ContainsKey(mChSelect))
        {
            selectId = mCacheDungeonTable[mChSelect].FindIndex(x => { return x.ID == fid; });
            int cache = EditorGUILayout.Popup(selectId, mCacheDungeonString);
            if (cache < 0)
            {
                return;
            }
            if (cache != selectId)
            {
                selectId = cache;
                dungeonId = mCacheDungeonTable[mChSelect][selectId].ID;
            }
        }
    }

    private void _popDungeonChSelect()
    {
        int cache = EditorGUILayout.Popup("章节选择", mChSelectId, mCacheDungeonKeyString);
        if (cache != mChSelectId)
        {
            mChSelectId = cache;
            _updateDungeonString();
        }
    }

    public class Node
    {
        public bool isOpen = false;
        public int selectedId = -1;
        public GameObject go = null;
        public ComCommonBind bind = null;
        public ChaptertDungeonUnit unit = null;
    }

    protected Node[] mNodes = new Node[0];

    private GameObject _loadChapterGameObject(int idx)
    {
        var item = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Chapter/Select/ChapterSelectUnit");

        item.name = string.Format("Level{0}", idx);

        ComCommonBind bind = item.GetComponent<ComCommonBind>();

        Utility.AttachTo(item, mChapterNodeRoot);

        var rect = item.GetComponent<RectTransform>();
        rect.offsetMin = new Vector2(-50f, -50f);
        rect.offsetMax = new Vector2(50.0f, 50.0f);

        rect.localPosition = mData.chapterList[idx].position;

        Text text = bind.GetCom<Text>("OrderNumber");
        text.text = string.Format("{0}", idx+1);

        RectTransform sourcePosition = bind.GetCom<RectTransform>("sourcePosition");
        RectTransform targetPosition = bind.GetCom<RectTransform>("targetPosition");
        TriangleGraph angleGraph = bind.GetCom<TriangleGraph>("angleGraph");
        RectTransform thumbRoot = bind.GetCom<RectTransform>("thumbRoot");

        //mData.chapterList[idx].angleSourcePosition = Vector3.zero;
        //mData.chapterList[idx].angleTargetPosition = mData.chapterList[idx].thumbOffset;

        sourcePosition.localPosition = mData.chapterList[idx].angleSourcePosition;
        targetPosition.localPosition = mData.chapterList[idx].angleTargetPosition;
        RectTransform targetRightPosition = bind.GetCom<RectTransform>("targetRightPosition");
        targetRightPosition.localPosition = mData.chapterList[idx].angleTargetRightPosition;
        //angleGraph.sourceOffset      = mData.chapterList[idx].angleWeidth;
        thumbRoot.localPosition      = mData.chapterList[idx].thumbOffset;


        return item;
    }

    public void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox(string.Format("无法在 Play 模式下编辑 "), MessageType.Warning);
            return;
        }

        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(DChapterData), UnityEditor.SelectionMode.Assets);
        if (selection.Length > 0)
        {
            var data = selection[0] as DChapterData;
            if (mData != data)
            {
                mData = data;

                if (mIsView)
                {
                    _unloadEnv();
                    _loadEnv();
                    _loadChapter();
                }
            }
        }


        if( mData == null )
        {
            return;
        }
        
        if (!mIsView && GUILayout.Button("预览", "miniButton"))
        {
            mIsView = true;
            _loadEnv();
            _loadTable();
            _loadChapter();
        }

        if (mIsView && GUILayout.Button("关闭预览", "miniButton"))
        {
            mIsView = false;
            _unloadEnv();
        }

        if (!mIsView) return;

        if(GUILayout.Button("保存文件","miniButton"))
        {
            EditorUtility.SetDirty(mData);
            AssetDatabase.SaveAssets();
        }

        string path = EditorGUILayout.TextField("背景路径", mData.backgroundPath);
        if (!path.Equals(mData.backgroundPath))
        {
            mData.backgroundPath = path;

            //var sprite = _loadSprite(path);
            //if (sprite != null)
            //{
            //    mChapterBackground.sprite = _loadSprite(mData.backgroundPath);
            //}
            _loadSprite(ref mChapterBackground, mData.backgroundPath);

        }

        mData.middlegroudnPath = EditorGUILayout.TextField("路径图片", mData.middlegroudnPath);

        //HeroGo.CustomGUIUtility.InitFiledAfterEnter("关卡数目", ref len, 0, 55);
        if (null != mMiddleGo)
        {
            mData.middlePos = mMiddleGo.GetComponent<RectTransform>().localPosition;
        }

        EditorGUILayout.Vector2Field("路径背景图片位置", mData.middlePos);


        mData.name = EditorGUILayout.TextField("名字", mData.name);
        mData.nameNumberIdx = EditorGUILayout.IntField("章节序号哦", mData.nameNumberIdx);
        mData.namePath = EditorGUILayout.TextField("名字图片路径", mData.namePath);

        int len = mData.chapterList.Length;

        EditorGUILayout.Vector2Field("min", mData.offsetMin);
        EditorGUILayout.Vector2Field("max", mData.offsetMax);


        _popDungeonChSelect();

        HeroGo.ArrayUtility.ArrayFiled<ChaptertDungeonUnit>(len, ref mData.chapterList);
        HeroGo.ArrayUtility.ArrayFiled<Node>(len, ref mNodes,
        delegate (int idx)
        {
            Node node = new Node();
            node.unit = mData.chapterList[idx];
            node.isOpen = true;
            node.selectedId = -1;
            node.go = _loadChapterGameObject(idx);

            var bind = node.go.GetComponent<ComCommonBind>();

            node.bind = bind;


            RectTransform sourcePosition = bind.GetCom<RectTransform>("sourcePosition");
            RectTransform targetPosition = bind.GetCom<RectTransform>("targetPosition");
            TriangleGraph angleGraph = bind.GetCom<TriangleGraph>("angleGraph");
            RectTransform thumbRoot      = bind.GetCom<RectTransform>("thumbRoot");

            sourcePosition.localPosition = mData.chapterList[idx].angleSourcePosition;
            targetPosition.localPosition = mData.chapterList[idx].angleTargetPosition;
            //angleGraph.sourceOffset      = mData.chapterList[idx].angleWeidth;
            thumbRoot.localPosition      = mData.chapterList[idx].thumbOffset;

            RectTransform targetRightPosition = bind.GetCom<RectTransform>("targetRightPosition");
            targetRightPosition.localPosition = mData.chapterList[idx].angleTargetRightPosition;

            ComChapterDungeonUnit cmp = bind.GetCom<ComChapterDungeonUnit>("DungeonUnit");
            var dunid = new DungeonID( node.unit.dungeonID );


            cmp.SetDungeonType(dunid.prestoryID > 0 ? ComChapterDungeonUnit.eDungeonType.Prestory: ComChapterDungeonUnit.eDungeonType.Normal);
            cmp.SetState(ComChapterDungeonUnit.eState.Passed);

            var tableNode = TableManagerEditor.instance.GetTableItem<ProtoTable.DungeonTable>(dunid.dungeonID);
            if (null != tableNode)
            {
                cmp.SetName(tableNode.Name, tableNode.Level.ToString());
            }


            return node;
        },
        delegate (Node node)
        {
            GameObject.DestroyImmediate(node.go);
            node.bind = null;
            node.go = null;
            node.unit = null;
        });


        mScroll = EditorGUILayout.BeginScrollView(mScroll);
        {
            for (int i = 0; i < len; ++i)
            {
                var item = mNodes[i].unit;
                {
                    EditorGUILayout.BeginVertical("GroupBox");
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("从后插入", "miniButton"))
                            {
                                _insertNode(i);
                                return;
                            }

                            if (GUILayout.Button("删除", "miniButton"))
                            {
                                _deleteNode(i);
                                return;
                            }

                            if (GUILayout.Button("显示", "miniButton"))
                            {
                                Selection.activeObject = mNodes[i].go;
                            }

                            if (GUILayout.Button("缩略图位置", "miniButton"))
                            {
                                Selection.activeObject = mNodes[i].bind.GetCom<RectTransform>("thumbRoot");
                            }

                            if (GUILayout.Button("三角起始点", "miniButton"))
                            {
                                Selection.activeObject = mNodes[i].bind.GetCom<RectTransform>("sourcePosition");
                            }

                            if (GUILayout.Button("三角目标点", "miniButton"))
                            {
                                Selection.activeObject = mNodes[i].bind.GetCom<RectTransform>("targetPosition");
                            }

                            if (GUILayout.Button("三角目标右边点", "miniButton"))
                            {
                                Selection.activeObject = mNodes[i].bind.GetCom<RectTransform>("targetRightPosition");
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    bool isSe = EditorGUILayout.BeginToggleGroup(item.dungeonID.ToString(), mNodes[i].isOpen);
                    {
                        if (isSe != mNodes[i].isOpen)
                        {
                            mNodes[i].isOpen = isSe;
                            if (isSe) Selection.activeObject = mNodes[i].go;
                        }

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUI.indentLevel++;
                                {
                                    if (mNodes[i].isOpen)
                                    {
                                        EditorGUILayout.BeginVertical("GroupBox");
                                        {
                                            _popDungeonSelect(ref item.dungeonID, ref mNodes[i].selectedId);
                                            EditorGUILayout.Vector3Field("圆点位置", item.position);

                                            var obj = mNodes[i].go;
                                            if (null != obj)
                                            {
                                                var objRect = obj.GetComponent<RectTransform>();
                                                if (item.position != objRect.localPosition)
                                                {
                                                    var p = objRect.localPosition;
                                                    item.position = new Vector3(p.x, p.y, 0.0f);
                                                    EditorUtility.SetDirty(mData);
                                                }
                                            }

                                            RectTransform thumbRoot = mNodes[i].bind.GetCom<RectTransform>("thumbRoot");
                                            RectTransform targetPosition = mNodes[i].bind.GetCom<RectTransform>("targetPosition");
                                            RectTransform targetRightPosition = mNodes[i].bind.GetCom<RectTransform>("targetRightPosition");
                                            EditorGUILayout.Vector3Field("缩略图位置", item.thumbOffset);
                                            if (null != thumbRoot)
                                            {
                                                if (thumbRoot.localPosition != item.thumbOffset)
                                                {
                                                    item.thumbOffset = thumbRoot.localPosition;
                                                    item.angleTargetPosition = thumbRoot.localPosition;
                                                    targetPosition.localPosition = thumbRoot.localPosition + 30 * Vector3.left;
                                                    targetRightPosition.localPosition = thumbRoot.localPosition + 30 * Vector3.right;
                                                    EditorUtility.SetDirty(mData);
                                                }
                                            }

                                            RectTransform sourcePosition = mNodes[i].bind.GetCom<RectTransform>("sourcePosition");
                                            EditorGUILayout.Vector3Field("角度目标点位置", item.angleSourcePosition);

                                            if (null != sourcePosition)
                                            {
                                                if (sourcePosition.localPosition != item.angleSourcePosition)
                                                {
                                                    item.angleSourcePosition = sourcePosition.localPosition;
                                                    EditorUtility.SetDirty(mData);
                                                }
                                            }

                                            EditorGUILayout.Vector3Field("角度右边目标点位置", item.angleTargetRightPosition);
                                            if (null != targetRightPosition)
                                            {
                                                if (targetRightPosition.localPosition != item.angleTargetRightPosition)
                                                {
                                                    item.angleTargetRightPosition = targetRightPosition.localPosition;
                                                    EditorUtility.SetDirty(mData);
                                                }
                                            }


                                            EditorGUILayout.Vector3Field("角度起始点点位置", item.angleTargetPosition);

                                            if (null != targetPosition)
                                            {
                                                if (targetPosition.localPosition != item.angleTargetPosition)
                                                {
                                                    item.angleTargetPosition = targetPosition.localPosition;
                                                    EditorUtility.SetDirty(mData);
                                                }
                                            }
                                            
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                }
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndToggleGroup();

                    EditorGUILayout.EndVertical();
                }
            }

            //if (mLeftPanelRoot != null)
            //{
            //    mLeftPanelRoot.GetComponent<ComChapterSelectUnlock>().SetUnlockCount(len);
            //}

            if (GUI.changed)
            {
                EditorUtility.SetDirty(mData);
            }
        }
        EditorGUILayout.EndScrollView();
    }


}
