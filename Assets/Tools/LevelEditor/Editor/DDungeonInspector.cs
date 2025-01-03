using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(DDungeonData))]
public class DDungeonInspector : Editor
{

    [MenuItem("[关卡编辑器]/地下城/创建地下城数据")]
    public static void Create()
    {
        var pinObject = FileTools.CreateAsset<DDungeonData>("DungeonData");

        EditorGUIUtility.PingObject(pinObject);
        Selection.activeObject = pinObject;
    }

    private bool _updateSliderConnectByType(DSceneDataConnect data, TransportDoorType curtype)
    {
        DSceneDataConnect target = mDungeonData.GetSideByType(data, curtype) as DSceneDataConnect;
        if (target != null)
        {
            target.isconnect[(int)curtype.OppositeType()] = data.isconnect[(int)curtype];
            return true;
        }
        
        return false;
    }

    private void _updateSliderConnect(DSceneDataConnect data)
    {
        for (int i = 0; i < 4; i++)
        {
            _updateSliderConnectByType(data, (TransportDoorType)i);
        }
    }

    private void _updateConnect(ref DSceneDataConnect data)
    {
        EditorGUILayout.BeginVertical();
        {
            for (int i = 0; i < 4; i++)
            {
                var curtype = (TransportDoorType)i;

                var toggle = EditorGUILayout.Toggle(curtype.ToString(), data.isconnect[i]);
                if (toggle != data.isconnect[i])
                {
                    data.isconnect[i] = toggle;
                    if (!_updateSliderConnectByType(data, curtype))
                    {
                        data.isconnect[i] = false;
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    #region Update
    public DDungeonData mDungeonData = null;
    private int[] mSelectedArray = new int[0];
    //private string[] mResourcePath = new string[0];

    public int mCurSelectX = -1;
    public int mCurSelectY = -1;
    public int mLastSelectX = -1;
    public int mLastSelectY = -1;
    public bool mAutoLoadSceneData = false;

    public DSceneDataConnect _getCurrentSelectedCeil(int x, int y)
    {
        if ((x < 0 || x >= mDungeonData.weidth)
         || (y < 0 || y >= mDungeonData.height))
        {
            return null;
        }

        int len = mDungeonData.areaconnectlist.Length;
        for (int i = 0; i < len; ++i)
        {
            var item = mDungeonData.areaconnectlist[i];
            if (item.positionx == x && item.positiony == y)
            {
                return mDungeonData.areaconnectlist[i];
            }
        }

        ++len;

        HeroGo.ArrayUtility.ArrayFiled<DSceneDataConnect>(len, ref mDungeonData.areaconnectlist,
        delegate (int idx)
        {
            return new DSceneDataConnect() { positionx = x, positiony = y, areaindex = idx };
        });

        EditorUtility.SetDirty(mDungeonData);
        return mDungeonData.areaconnectlist[len - 1];
    }

    public void _checkArray()
    {
        HeroGo.ArrayUtility.ArrayRemoveBy<DSceneDataConnect>(ref mDungeonData.areaconnectlist, item =>
        {
            if ((item.positionx < 0 || item.positionx >= mDungeonData.weidth)
             || (item.positiony < 0 || item.positiony >= mDungeonData.height) )
            {
                return true;
            }

            return false;
        });
        _resetIdx();
    }

    private void _resetIdx()
    {
        for (int i = 0; i < mDungeonData.areaconnectlist.Length; i++)
        {
            var item = mDungeonData.areaconnectlist[i];
            item.areaindex = i;
        }
        EditorUtility.SetDirty(mDungeonData);
    }

    private void _clearCurrentSelectedData(int x, int y)
    {
        HeroGo.ArrayUtility.ArrayRemoveBy<DSceneDataConnect>(ref mDungeonData.areaconnectlist, delegate(DSceneDataConnect item)
        {
            if (item.positionx == x && item.positiony == y)
            {
                return true;
            }
            return false;
        });

        mCurSelectX = -1;
        mCurSelectY = -1;
        mLastSelectX = -1;
        mLastSelectY = -1;

        var list = mDungeonData.areaconnectlist;
        if (list.Length > 0)
        {
            mCurSelectX = list[list.Length - 1].positionx;
            mCurSelectY = list[list.Length - 1].positiony;
        }

        _resetIdx();
    }

    enum AppendType
    {
        eNone = 0,
        eWidthLeft,
        eWidthRight,
        eHeightTop,
        eHeightBottom,
    }

    class Node
    {
        public Node(int i)
        {
            index = i;
        }

        public int index { get; private set; }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.fieldWidth = 50;
        var dungeondata = target as DDungeonData;
        if (dungeondata == null)
        {
            return;
        }

        if (dungeondata != mDungeonData)
        {
            mDungeonData = dungeondata;

            mCurSelectX = -1;
            mCurSelectY = -1;
            mLastSelectX = -1;
            mLastSelectY = -1;


            var lista = mDungeonData.areaconnectlist;
            if (lista.Length > 0)
            {
                mCurSelectX = lista[lista.Length - 1].positionx;
                mCurSelectY = lista[lista.Length - 1].positiony;
            }

            for (int i = 0; i < lista.Length; i++)
            {
                if (lista[i].sceneareapath != null)
                {
                    lista[i].scenedata = AssetDatabase.LoadAssetAtPath<DSceneData>(string.Format("Assets/Resources/{0}.asset", lista[i].sceneareapath));
                }
            }

            EditorUtility.SetDirty(mDungeonData);
        }

        mLastSelectX = mCurSelectX;
        mLastSelectY = mCurSelectY;
        EditorGUILayout.BeginVertical("GroupBox");
        mAutoLoadSceneData = EditorGUILayout.Toggle("选中后是否自动跳转场景编辑", mAutoLoadSceneData);
        DSceneDataEditorWindow.ResetAutoLoadSceneData(mAutoLoadSceneData);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("GroupBox");
        {
            _updateGrid();
            _updateLine();
        }
        EditorGUILayout.EndVertical();


        AppendType appendType = AppendType.eNone;
        ///        htop
        ///  wleft         wright
        ///        hbottom

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(250);
        if (GUILayout.Button("高度顶部+1"))
        {
            appendType = AppendType.eHeightTop;
        }
        GUILayout.Space(250);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("宽度左边+1"))
        {
            appendType = AppendType.eWidthLeft;
        }

        GUILayout.Space(300);

        if (GUILayout.Button("宽度右边+1"))
        {
            appendType = AppendType.eWidthRight;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(250);
        if (GUILayout.Button("高度底部+1"))
        {
            appendType = AppendType.eHeightBottom;
        }
        GUILayout.Space(250);
        EditorGUILayout.EndHorizontal();

        {
            int height = mDungeonData.height;
            int width = mDungeonData.weidth;
            bool isdirty = false;
            switch (appendType)
            {
                case AppendType.eWidthLeft:
                    width++;
                    foreach (var iter in mDungeonData.areaconnectlist)
                    {
                        iter.positionx++;
                    }
                    isdirty = true;
                    break;
                case AppendType.eWidthRight:
                    width++;
                    isdirty = true;
                    break;
                case AppendType.eHeightTop:
                    height++;
                    foreach (var iter in mDungeonData.areaconnectlist)
                    {
                        iter.positiony++;
                    }
                    isdirty = true;
                    break;
                case AppendType.eHeightBottom:
                    height++;
                    isdirty = true;
                    break;
                default:
                    break;
            }

            if (isdirty)
            {
                mDungeonData.height = height;
                mDungeonData.weidth = width;
                EditorUtility.SetDirty(mDungeonData);
                Repaint();
                mCurSelectX = -1;
                mCurSelectY = -1;
                mLastSelectX = -1;
                mLastSelectY = -1;

                return;
            }
        }

        EditorGUILayout.BeginHorizontal();
        {
            int height = mDungeonData.height;
            int width = mDungeonData.weidth;
            HeroGo.CustomGUIUtility.InitFiledAfterEnter("height", ref height, 1, 20);
            HeroGo.CustomGUIUtility.InitFiledAfterEnter("weidth", ref width, 1, 20);

            if (height != mDungeonData.height
                || width != mDungeonData.weidth
             )
            //|| mDungeonData.datas.Length != height * width)
            {
                _checkArray();
                EditorUtility.SetDirty(mDungeonData);
                Repaint();
                mDungeonData.height = height;
                mDungeonData.weidth = width;
                return;
            }

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("start idx : " + mDungeonData.startindex);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();


        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("当前: [" + mCurSelectX + "," + mCurSelectY + "]");
        EditorGUILayout.Space();

        var current = _getCurrentSelectedCeil(mCurSelectX, mCurSelectY);
        bool needReloadSceneData = false;
        if (mLastSelectX != mCurSelectX || mLastSelectY != mCurSelectY)
        {
            needReloadSceneData = true;
        }

        if (current != null)
        {
            EditorGUILayout.LabelField("AreaID :" + current.id);

            current.scenedata = EditorGUILayout.ObjectField(current.scenedata, typeof(DSceneData), false) as DSceneData;

            if (current.scenedata != null)
            {
                //current.sceneareaid = current.scenedata._id;
                current.sceneareapath = CFileManager.EraseExtension(AssetDatabase.GetAssetPath(current.scenedata).Replace("Assets/Resources/", ""));
                Repaint();
            }
            
            //EditorGUILayout.

            EditorGUILayout.Space();

            _updateConnect(ref current);



            
            current.isboss = EditorGUILayout.Toggle("Boss关卡", current.isboss);
            current.isstart = EditorGUILayout.Toggle("起始关卡", current.isstart);
            current.isegg = EditorGUILayout.Toggle("是否是彩蛋房间", current.isegg);
            current.isnothell = EditorGUILayout.Toggle("排除是深渊关卡", current.isnothell);

            if (current.isstart)
            {
                for (int i = 0; i < mDungeonData.areaconnectlist.Length; i++)
                {
                    var item = mDungeonData.areaconnectlist[i];
                    item.areaindex = i;
                    if (item != current)
                    {
                        item.isstart = false;
                    }
                    else
                    {
                        mDungeonData.startindex = item.areaindex;
                    }
                }
            }

            if (current.isboss)
            {
                {
                    for (int i = 0; i < mDungeonData.areaconnectlist.Length; i++)
                    {
                        var item = mDungeonData.areaconnectlist[i];
                        if (item != current)
                        {
                            item.isboss = false;
                        }
                    }
                }
            }

            EditorGUILayout.BeginVertical("GroupBox");
            EditorGUILayout.Space();
            {
                int newsize = EditorGUILayout.DelayedIntField("随机关卡数目", current.linkAreaIndex.Length);
                if (newsize != current.linkAreaIndex.Length)
                {
                    HeroGo.ArrayUtility.ArrayFiled<int>(newsize, ref current.linkAreaIndex, delegate (int v)
                    {
                        return -1;
                    });
                }

                List<string> allSelectedList = new List<string>();
                for (int i = 0; i < mDungeonData.areaconnectlist.Length; ++i)
                {
                    var node = mDungeonData.areaconnectlist[i];
                    string res = string.Format("{0}索引:{1}, AreaID:{2}, 路径: {3}", (current.areaindex == i || current.linkAreaIndex.Contains(i)) ? "x" : "", i, node.id, System.IO.Path.GetFileName(node.sceneareapath));
                    allSelectedList.Add(res);
                }

                for (int i = 0; i < current.linkAreaIndex.Length; ++i)
                {
                    int linkAreaIdx = current.linkAreaIndex[i];

                    if (linkAreaIdx == current.areaindex)
                    {
                        current.linkAreaIndex[i] = -1;

                        if (EditorUtility.DisplayDialog("数据无效", "当前房间配置为此，请另选一个", "确定"))
                        {

                        }
                    }

                    EditorGUILayout.BeginHorizontal();
                    DSceneData linkData = null;
                    if (linkAreaIdx >= 0 && linkAreaIdx < mDungeonData.areaconnectlist.Length)
                    {
                        linkData = mDungeonData.areaconnectlist[linkAreaIdx].scenedata;
                    }
                    else
                    {
                        GUI.color = Color.red;
                    }

                    int selectedIndex = linkAreaIdx;

                    selectedIndex = EditorGUILayout.Popup(selectedIndex, allSelectedList.ToArray());
                    EditorGUILayout.EndHorizontal();

                    GUI.color = Color.white;

                    EditorGUILayout.ObjectField(linkData, typeof(DSceneData), false);

                    if (selectedIndex != linkAreaIdx)
                    {
                        int newLinkAreaIndex = selectedIndex;

                        bool hasContain = false;
                        for (int j = 0; j < current.linkAreaIndex.Length; j++)
                        {
                            if (current.linkAreaIndex[j] == newLinkAreaIndex)
                            {
                                hasContain = true;
                                break;
                            }
                        }

                        if (!hasContain)
                        {
                            current.linkAreaIndex[i] = newLinkAreaIndex;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("数据无效", "配置重复，请另选一个", "确定");
                        }
                    }
                }
                if (needReloadSceneData && mAutoLoadSceneData)
                {
                    DSceneDataEditorWindow.SetSendData(current.scenedata, this.mDungeonData, mCurSelectX, mCurSelectY, mAutoLoadSceneData, mDungeonData);
                }
            }
            EditorGUILayout.EndVertical();



            EditorGUILayout.Space();
            if (GUILayout.Button("Editor") && current.scenedata)
            {
                DSceneDataEditorWindow.SetSendData(current.scenedata, this.mDungeonData, mCurSelectX, mCurSelectY);
            }
            if (GUILayout.Button("Clear"))
            {
                _clearCurrentSelectedData(mCurSelectX, mCurSelectY);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        #region List
        EditorGUILayout.BeginVertical("GroupBox");
        {

            if (GUILayout.Button("加一个"))
            {
                List<DSceneDataConnect> list = new List<DSceneDataConnect>(mDungeonData.areaconnectlist);

                list.Add(new DSceneDataConnect());

                mDungeonData.areaconnectlist = list.ToArray();
            }

            DSceneDataConnect curCon = null;
            for (int i = 0; i < mDungeonData.areaconnectlist.Length; i++)
            {
                var data = mDungeonData.areaconnectlist[i];
                bool iscurrent = data.positionx == mCurSelectX && data.positiony == mCurSelectY;
                if (iscurrent && data.positionx >= 0 && data.positiony >= 0)
                {
                    curCon = data;
                    break;
                }
            }

            for (int i = 0; i < mDungeonData.areaconnectlist.Length; i++)
            {
                var data = mDungeonData.areaconnectlist[i];

                if (null == curCon)
                {
                    GUI.color = Color.white;
                }
                else
                {
                    if (curCon.areaindex == i)
                    {
                        GUI.color = Color.yellow;
                    }
                    else
                    {
                        bool hasFoundInLink = false;
                        for (int j = 0; j < curCon.linkAreaIndex.Length; j++)
                        {
                            if (curCon.linkAreaIndex[j] == i)
                            {
                                hasFoundInLink = true;
                                break;
                            }
                        }

                        if (hasFoundInLink)
                        {
                            GUI.color = Color.green;
                        }
                        else
                        {
                            GUI.color = Color.white;
                        }
                    }
                }

                EditorGUILayout.BeginHorizontal();
                Rect filed1 = GUILayoutUtility.GetRect(100, 1);
                EditorGUI.LabelField(filed1, string.Format("索引 [{0}] areaId[{1}]", i, data.id));
                DSceneData localscenedata = EditorGUILayout.ObjectField(data.scenedata, typeof(DSceneData), false) as DSceneData;
                if (localscenedata != data.scenedata)
                {
                    data.scenedata = localscenedata;

                    if (null != data.scenedata)
                    {
                        data.sceneareapath = CFileManager.EraseExtension(AssetDatabase.GetAssetPath(data.scenedata).Replace("Assets/Resources/", ""));
                    }
                }

                EditorGUILayout.EndHorizontal();

                GUI.color = Color.white;
            }
        }
        EditorGUILayout.EndVertical();
        #endregion

        if (GUI.changed)
        {
            EditorUtility.SetDirty(mDungeonData);
        }
    }
    #endregion

    #region GUI - Small Map 
    private const int kGirdWidth = 50;
    private const int kGirdHeight = 50;

    private void _drawLine(int x, int y, int linestrong, TransportDoorType type, Color color)
    {
        var rect = GUILayoutUtility.GetLastRect();
        var lh = linestrong;
        var lw = linestrong;
        var offsetx = rect.width / 2;
        var offsety = rect.height / 2;

        var tcolor = color;

        switch (type)
        {
            case TransportDoorType.Buttom:
                lh = kGirdHeight;
                offsetx += linestrong;
                //tcolor = Color.blue;
                break;
            case TransportDoorType.Top:
                lh = -kGirdHeight;
                //tcolor = Color.white;
                //offsetx -= linestrong;
                break;
            case TransportDoorType.Left:
                lw = -kGirdWidth;
                offsety += linestrong;
                //tcolor = Color.magenta;
                break;
            case TransportDoorType.Right:
                lw = kGirdWidth;
                //tcolor = Color.gray;
                //offsety -= linestrong;
                break;
        }


        var nrect = new Rect(rect.x + offsetx, rect.y + offsety, lw, lh);
        var cW = kGirdWidth;
        var cH = kGirdHeight;

        var gHeight = mDungeonData.height;
        var gWeidth = mDungeonData.weidth;

        nrect.x += (x - gWeidth / 2) * cW;
        if (gWeidth % 2 == 0)
        {
            nrect.x += cW / 2;
        }

        nrect.y += (y - gHeight / 2) * cH;
        if (gHeight % 2 == 0)
        {
            nrect.y += cH / 2;
        }

        EditorGUI.DrawRect(nrect, tcolor);
    }

    private bool _drawRectEx(int x, int y, int width, int height, Color color, int padding = 1)
    {
        var rect = GUILayoutUtility.GetLastRect();
        var nrect = new Rect(rect.x + rect.width / 2 - width / 2, rect.y + rect.height / 2 - height / 2, width, height);

        var cW = kGirdWidth + padding;
        var cH = kGirdHeight + padding;

        var gHeight = mDungeonData.height;
        var gWeidth = mDungeonData.weidth;

        nrect.x += (x - gWeidth / 2) * cW;
        if (gWeidth % 2 == 0)
        {
            nrect.x += cW / 2;
        }

        nrect.y += (y - gHeight / 2) * cH;
        if (gHeight % 2 == 0)
        {
            nrect.y += cH / 2;
        }

        if (Event.current.type == EventType.Repaint)
        {
            EditorGUI.DrawRect(nrect, color);
        }
        else if (Event.current.type == EventType.MouseDown)
        {
            if (nrect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                return true;
            }
        }

        return false;
    }
    private void _drawRect(int x, int y, int width, int height, Color color, int padding = 1)
    {
        var rect = GUILayoutUtility.GetLastRect();
        var nrect = new Rect(rect.x + rect.width / 2 - width / 2, rect.y + rect.height / 2 - height / 2, width, height);

        var cW = kGirdWidth + padding;
        var cH = kGirdHeight + padding;

        var gHeight = mDungeonData.height;
        var gWeidth = mDungeonData.weidth;

        nrect.x += (x - gWeidth / 2) * cW;
        if (gWeidth % 2 == 0)
        {
            nrect.x += cW / 2;
        }

        nrect.y += (y - gHeight / 2) * cH;
        if (gHeight % 2 == 0)
        {
            nrect.y += cH / 2;
        }

        if (Event.current.type == EventType.Repaint)
        {
            EditorGUI.DrawRect(nrect, color);
        }
    }


    private void _updateGrid()
    {
        var newrect = GUILayoutUtility.GetRect(1, mDungeonData.height * 50);
        EditorGUI.DrawRect(newrect, Color.black);

        //EditorGUI.DrawRect(new Rect(padding, lastrect.yMin + 1, width, lastrect.yMax - lastrect.yMin), Color.gray);

        var conlist = mDungeonData.areaconnectlist;

        for (int i = 0; i < mDungeonData.weidth; i++)
        {
            for (int j = 0; j < mDungeonData.height; j++)
            {
                if (mCurSelectX == i && mCurSelectY == j)
                {
                    _drawRect(i, j, 44, 44, Color.yellow);
                }

                if (_drawRectEx(i, j, 40, 40, Color.gray))
                {
                    mCurSelectX = i;
                    mCurSelectY = j;
                    Repaint();
                }
            }
        }

        for (int i = 0; i < conlist.Length; i++)
        {
            var conitem = conlist[i];

            if (conitem.positionx < 0 && conitem.positiony < 0)
            {
                continue;
            }

            _drawRect(conitem.positionx, conitem.positiony, 40, 40, Color.red);


            if (conitem.isstart)
            {
                _drawRect(conitem.positionx, conitem.positiony, 30, 30, Color.green);
            }

            if (conitem.isnothell)
            {
                _drawRect(conitem.positionx, conitem.positiony, 25, 25, Color.blue);
            }

            if (conitem.isboss)
            {
                _drawRect(conitem.positionx, conitem.positiony, 20, 20, Color.yellow);
            }

        }

    }

    private void _updateLine()
    {
        var conlist = mDungeonData.areaconnectlist;
        for (int i = 0; i < conlist.Length; i++)
        {
            var conitem = conlist[i];

            for (int j = 0; j < 4; j++)
            {
                if (conitem.isconnect[j])
                {
                    _drawLine(conitem.positionx, conitem.positiony, 2, (TransportDoorType)j, Color.blue);
                }
            }
        }
    }
    #endregion

    void _load()
    {
    }

    void _unload()
    {
    }

    void OnEnable()
    {
        _load();
    }

    void OnDisable()
    {
    }

    void OnDestroy()
    {

    }
}
