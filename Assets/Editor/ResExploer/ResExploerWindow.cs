using UnityEditor;
using UnityEngine;
using System;
using GameClient;
using System.Reflection;
using UnityEditor.AnimatedValues;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Network;


public class ResExploerWindow : EditorWindow
{
    [MenuItem("[TM工具集]/资源查看器/打开资源查看器", false, 1)]
    static void Init()
    {
        var window = GetWindow<ResExploerWindow>();
        window.position = new Rect(50, 50, 250, 60);
        window.Show();
        window.Populate();
    }

    private GUIStyle fontStyle;
    protected EditorUI.UIListBox entityList;

    Editor gameObjPreview;
    GameObject objSelected;
    UnityEngine.Object objIcon;
    Material defaultMaterial;

    int listCount = 0;
    string respath;
    ItemTable currentTable;

    EditorUI.GUIVerticalFlexBlock ResListBoxBlock;
    EditorUI.GUIVerticalFlexBlock ResPreviewBlock;
    protected int ResListBoxHeight;
    protected int ResPreviewHeight;

    protected Editor iconPreview;

    void UpdateIconPreview(UnityEngine.Object t)
    {
        Editor.CreateCachedEditor(t, null, ref iconPreview);
    }


    void Clear()
    {
        if (gameObjPreview)
        {
            Editor.DestroyImmediate(gameObjPreview);
            gameObjPreview = null;
        }

        if (iconPreview)
        {
            Editor.DestroyImmediate(iconPreview);
            iconPreview = null;
        }

        objSelected = null;
        objIcon = null;
        currentTable = null;
        respath = null;
    }

    protected void SaveConfig()
    {
        EditorPrefs.SetInt("ResExploerWindow.ResListBoxHeight", ResListBoxHeight);
        EditorPrefs.SetInt("ResExploerWindow.ResPreviewHeight", ResPreviewHeight);
    }

    void OnDisable()
    {
        Clear();
        SaveConfig();
    }

    void OnDestroy()
    {
        Clear();
        SaveConfig();
    }

    protected void LoadConfig()
    {
        if (ResListBoxBlock != null || ResPreviewBlock != null)
        {
            return;
        }

        ResListBoxHeight = EditorPrefs.GetInt("ResExploerWindow.ResListBoxHeight");
        ResPreviewHeight = EditorPrefs.GetInt("ResExploerWindow.ResPreviewHeight");

        if (ResListBoxHeight == 0)
        {
            ResListBoxHeight = 300;
        }

        if (ResPreviewHeight == 0)
        {
            ResPreviewHeight = 300;
        }

        ResListBoxBlock = new EditorUI.GUIVerticalFlexBlock(ResListBoxHeight);
        ResPreviewBlock = new EditorUI.GUIVerticalFlexBlock(ResPreviewHeight);
    }


    void Populate()
    {
        LoadConfig();
        UpdateSelected();
    }

    void CreateFontStyle()
    {
        if (fontStyle == null)
        {
            fontStyle = new GUIStyle();
            fontStyle.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle.fontSize = 22;
            fontStyle.alignment = TextAnchor.MiddleCenter;
            fontStyle.normal.textColor = Color.green;
            fontStyle.hover.textColor = Color.green;

        }
    }

    void Callback(object obj)
    {
        Debug.Log("Selected: " + obj);
    }


    Vector2 InfoScrollPos;
    int preindex = -1;
    int sortorder = 1;

    void InitResList(List<ItemTable> resList)
    {
        if (entityList == null)
        {
            entityList = new EditorUI.UIListBox(this);
            entityList.SetTitles(new string[] { "id", "name", "level", "color" }, new float[] { 0.3f, 0.3f, 0.2f, 0.2f });

            entityList.onSelectionsChange = dragitems =>
            {
                if(dragitems != null && dragitems.Length > 0)
                    UpdateResPreview(((EditorUI.UIListItem)dragitems[0]).userdata);
            };

            entityList.onClickTitle = (EditorUI.UIListBox box, int index) =>
            {
                if (preindex == index)
                {
                    sortorder = -sortorder;
                }

                preindex = index;

                box.SortChild(
                (left, right) =>
                {
                    var leftTable = left.userdata as ItemTable;
                    var rightTable = right.userdata as ItemTable;

                    if (index == 0)
                    {
                        return leftTable.ID.CompareTo(rightTable.ID) * sortorder;
                    }
                    else if (index == 1)
                    {
                        return leftTable.Name.CompareTo(rightTable.Name) * sortorder;
                    }
                    else if (index == 2)
                    {
                        return leftTable.NeedLevel.CompareTo(rightTable.NeedLevel) * sortorder;
                    }
                    else if (index == 3)
                    {
                        return leftTable.Color.CompareTo(rightTable.Color) * sortorder;
                    }

                    return leftTable.ID.CompareTo(rightTable.ID) * sortorder;
                }
                );
            };

        }

        entityList.Clear();

        for (int i = 0; i < resList.Count; ++i)
        {
            var current = resList[i];
            var item = entityList.AddItems(
                new string[] {
                    current.ID.ToString(),
                    current.Name,
                    current.NeedLevel.ToString(),
                    current.Color.GetDescription()
                }
            );

            item.userdata = current;
        }
    }

    public static GameObject LoadAssetAtPath(string path)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + path + ".prefab");

        if (prefab == null)
        {
            prefab = EditorGUIUtility.Load("PointPreb.prefab") as GameObject;
        }
        return prefab;
    }

    void ReleaseIcon()
    {
        if (objIcon != null)
        {
            //Editor.DestroyImmediate(objIcon);
            objIcon = null;
        }
    }

    void UpdateResPreview(object userdata)
    {
        objSelected = LoadAssetAtPath("");
        respath = "";

        ItemTable t = userdata as ItemTable;
        currentTable = t;
        if (t != null)
        {
            ResTable res = TableManager.instance.GetTableItem<ResTable>(t.ResID);
            if (res != null)
            {
                objSelected = LoadAssetAtPath(res.ModelPath);
                respath = res.ModelPath;
            }
            //objIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/" + t.Icon + ".png");
            string iconPath = "Assets/Resources/" + t.Icon;
            string[] assetKeys = iconPath.Split(':');

            assetKeys[0] = System.IO.Path.ChangeExtension(assetKeys[0], ".png");

            if (assetKeys.Length == 2)
            {
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(assetKeys[0]);
                foreach (var item in objs)
                {
                    if (item.name == assetKeys[1])
                    {
                        objIcon = item;
                    }
                }
            }
            else
            {
                objIcon = AssetDatabase.LoadAssetAtPath<Texture>(assetKeys[0]);
            }
            //AssetLoader.instance.LoadRes("Assets/Resources/" + t.Icon);
        }

        var pre = gameObjPreview;
        //GetEditor(gameObjPreview);
        Editor.CreateCachedEditor(objSelected, null, ref gameObjPreview);
        /*if (pre != gameObjPreview)
        {
            InitEditor(gameObjPreview, objSelected);
        }*/
    }

    Vector2 currentViewDir = new Vector2(171.1f, -0.8f);

    void InitEditor(Editor gameObjPreview, GameObject obj)
    {
        if (gameObjPreview != null)
        {
            Type t = gameObjPreview.GetType();

            var info = t.GetField("previewDir", BindingFlags.NonPublic | BindingFlags.Instance);


            //Vector2 dir = new Vector2(120f, -20f);
            /*if (obj && Is2DMode(cinfo))
            {
                dir = new Vector2(obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.y > 0 ? -obj.transform.localEulerAngles.x : obj.transform.localEulerAngles.x);
            }
            */

            info.SetValue(gameObjPreview, currentViewDir);
        }
    }

    void GetEditor(Editor gameObjPreview)
    {
        if (gameObjPreview != null)
        {
            Type t = gameObjPreview.GetType();
            var info = t.GetField("previewDir", BindingFlags.NonPublic | BindingFlags.Instance);
            currentViewDir = (Vector2)info.GetValue(gameObjPreview);
        }
    }

    void UpdateSelected()
    {
        Clear();

        List<ItemTable> list = new List<ItemTable>();
        var ItemTables = TableManager.instance.GetTable<ItemTable>();
        var enumerator = ItemTables.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current.Value as ItemTable;

            if (current.Type == selectedType && current.SubType == selectedSubType
            && current.ThirdType == selectedThirdType)
            {
                list.Add(current);
            }
        }
        listCount = list.Count;
        InitResList(list);
    }

    ItemTable.eType selectedType = ItemTable.eType.EQUIP;
    ItemTable.eSubType selectedSubType = ItemTable.eSubType.WEAPON;
    ItemTable.eThirdType selectedThirdType = ItemTable.eThirdType.HUGESWORD;

    int addNum = 1;
    int str = 1;
    int ql = 100;

    void OnGUI()
    {
        bool bSelectedChange = false;
        GUIControls.UIStyles.UpdateEditorStyles();
        CreateFontStyle();
        EditorGUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginChangeCheck();

        selectedType = (ItemTable.eType)EditorGUILayout.EnumPopup("主类别", selectedType);
        selectedSubType = (ItemTable.eSubType)EditorGUILayout.EnumPopup("子类别", selectedSubType);
        selectedThirdType = (ItemTable.eThirdType)EditorGUILayout.EnumPopup("三类别", selectedThirdType);

        bSelectedChange = EditorGUI.EndChangeCheck();

        if (entityList == null || bSelectedChange)
        {
            UpdateSelected();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Height(100.0f));
        EditorGUILayout.LabelField("当前类型个数: " + listCount);
        EditorGUILayout.LabelField("当前信息：" + (currentTable == null ? "" : currentTable.Name + " [" + currentTable.ID.ToString() + "]"));
        EditorGUILayout.LabelField("当前模型路径: " + respath);
        EditorGUILayout.LabelField("当前图标路径: " + (currentTable == null ? "" : currentTable.Icon));
      
        EditorGUILayout.EndVertical();


        Rect rtt = GUILayoutUtility.GetRect(1, 100);
        if (objIcon)
        {
            UpdateIconPreview(objIcon);
            iconPreview.OnPreviewGUI(rtt, "Box");
        }
        //defaultMaterial.mainTexture = objIcon == null ? Texture2D.blackTexture : objIcon;
        //EditorGUI.DrawPreviewTexture(rtt, objIcon == null ? Texture2D.blackTexture : objIcon, null, ScaleMode.ScaleToFit);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (selectedType == ItemTable.eType.EQUIP)
        {
            //!!additem id=155415002 num=1 str=15 ql=100
            float back = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            addNum = EditorGUILayout.IntField("数量", addNum,GUILayout.ExpandWidth(false));
            str = EditorGUILayout.IntField("强化", str,GUILayout.ExpandWidth(false));
            ql = EditorGUILayout.IntField("品级", ql,GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = back;
            if (GUILayout.Button("添加"))
            {
                if (NetManager.Instance() != null)
                    ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1} str={2} ql={3}", currentTable.ID, addNum,str,ql));
            }
        }
        else
        {
            addNum = EditorGUILayout.IntField("数量", addNum);
            if (GUILayout.Button("添加"))
            {
                if (NetManager.Instance() != null)
                    ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", currentTable.ID, addNum));
            }
        }

        EditorGUILayout.EndHorizontal();

        if (ResListBoxBlock == null)
        {
            LoadConfig();
        }

        ResListBoxHeight = (int)ResListBoxBlock.OnLayoutGUI();
        entityList.OnLayoutGUI(ResListBoxHeight);

        EditorGUILayout.Space();
        if (gameObjPreview)
        {
            float height = position.height - ResListBoxHeight - 200;
            if (height <= 5) height = 5;
            Rect rt = GUILayoutUtility.GetRect(1, height);
            gameObjPreview.OnPreviewGUI(rt, "Box");
        }



        EditorGUILayout.EndVertical();
    }

    int EditorGUILayout_EnumPopup(string text, Enum value)
    {
        return Convert.ToInt32(EditorGUILayout.EnumPopup(text, value));
    }

}