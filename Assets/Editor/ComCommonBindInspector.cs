using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System;
using UnityEngine.Assertions;


public struct ViewBindUnit
{
    public string tag;
    public Component com;
    public ComCommonBind.eBindUnitType type;
    public bool isGenerateEventFunction;
}

[CustomEditor(typeof(ComCommonBind))]
public class ComCommonBindInspector : Editor
{

    protected List<string> mCacheString = new List<string>();


    protected Vector2 mScrollSprites = Vector2.zero;
    
    GUIStyle fontstyleWarnning;
    GUIStyle fontstyleUnitInfo;
    GUIStyle fontstyleUnitInfoSelect;
    GUIStyle headerStyle;
    GUIStyle radioButtonStyle;

    const string ViewNameSpace = "GameClient";

    void CreateFontStyle()
    {
        if (fontstyleWarnning == null)
        {
            fontstyleWarnning = new GUIStyle(EditorStyles.label);
            fontstyleWarnning.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleWarnning.fontSize = 12;
            fontstyleWarnning.alignment = TextAnchor.LowerLeft;
            fontstyleWarnning.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.active.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.focused.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }

        if (fontstyleUnitInfo == null)
        {
            fontstyleUnitInfo = new GUIStyle(EditorStyles.label);
            fontstyleUnitInfo.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfo.fontSize = 12;
            //fontstyleUnitInfo.fontStyle = FontStyle.Bold;
            fontstyleUnitInfo.alignment = TextAnchor.MiddleLeft;
            fontstyleUnitInfo.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleUnitInfo.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }

        if(fontstyleUnitInfoSelect == null)
        {
            fontstyleUnitInfoSelect = new GUIStyle(EditorStyles.label);
            fontstyleUnitInfoSelect.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfoSelect.fontSize = 12;
            //fontstyleUnitInfo.fontStyle = FontStyle.Bold;
            fontstyleUnitInfoSelect.alignment = TextAnchor.MiddleLeft;
            fontstyleUnitInfoSelect.normal.textColor = Color.green;
            fontstyleUnitInfoSelect.hover.textColor = Color.green;
        }

        // if(deStylePalette == null)
        // {
        //     deStylePalette = new DG.DemiEditor.DeStylePalette();

        //     var type = deStylePalette.GetType();
        //     MethodInfo info = type.GetMethod("Init",
        //         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

        //     info.Invoke(deStylePalette,new object[0]);
        // }

        if(deToggleButtonStyle == null)
        {
            deToggleButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
        }

        if(headerStyle == null)
        {
            headerStyle = "RL Header";
        }
 
        if(radioButtonStyle == null)
        {
			radioButtonStyle = new GUIStyle(EditorStyles.miniButton);//new GUIStyle(DG.DemiEditor.DeGUI.styles.button.def);
        }
    }

    ComArrayListDrawer binddrawer = new ComArrayListDrawer();
    ComArrayListDrawer spritedrawer = new ComArrayListDrawer();
    ComArrayListDrawer prefabdrawer = new ComArrayListDrawer();
    ComArrayListDrawer viewdrawer = new ComArrayListDrawer();

    //DG.DemiEditor.DeStylePalette    deStylePalette;
    GUIStyle                        deToggleButtonStyle;

    void OnUnitBindGUI(ComArrayListDrawer drawer,ComCommonBind bind)
    {
        drawer.DrawHeader("",bind.units.Length,this);
        StringBuilder builder = StringBuilderCache.Acquire();
        for(int i = 0; i < bind.units.Length; ++i)
        {
            drawer.BeginDrawElement(i);
            var cur = bind.units[i];
            EditorGUILayout.BeginVertical();
            builder.Clear();
            builder.AppendFormat("  [{0}] {1} : {2}",i,cur.tag,cur.com == null ? "Null":cur.com.GetType().Name);
            if(!bSimpleShow)GUILayout.Label(builder.ToString(),drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            //if(string.IsNullOrEmpty(bind.units[i].tag))
            //GUILayout.Label("    Pls Enter a Name:",EditorStyles.helpBox);
            //EditorGUILayout.
            if(!bSimpleShow)GUILayout.Label("    Tag:");
            if(bSimpleShow)EditorGUILayout.BeginHorizontal();
            if(bSimpleShow)GUILayout.Label(builder.ToString(),drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            if (searchTypeComIndex == i)
            {
                GUI.color = Color.green;
            }
            
            var tag = EditorGUILayout.TextField(cur.tag);
            if (tag != cur.tag)
            {
                Undo.RecordObject (target, "tag " + target.name);
                bind.units[i].tag = tag;
                EditorUtility.SetDirty(target);
            }
            if (searchTypeComIndex == i)
            {
                GUI.color = Color.white;
            }

            if(bSimpleShow)EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if(bind.units[i].com == null)
            GUILayout.Label("    Pls Select a Component or GameObject:",EditorStyles.helpBox);
            if(!bSimpleShow)GUILayout.Label("    Unit:");
            EditorGUILayout.BeginHorizontal();
            var com = EditorGUILayout.ObjectField(cur.com,typeof(Component),true) as Component;
            if (com != cur.com)
            {
                Undo.RecordObject (target, "com " + target.name);
                bind.units[i].com = com;
                EditorUtility.SetDirty(target);
            }
            if (cur.com != null)
            {
                if (cur.tag == null || cur.tag.Equals(""))
                {
                    bind.units[i].tag = cur.com.name;
                }
                string name = cur.type == ComCommonBind.eBindUnitType.GameObject ? "GameObject" : cur.com.GetType().Name;
                if (GUILayout.Button(name, "GV Gizmo DropDown"))
                {
                    var coms = cur.com.GetComponents(typeof(Component));

                    int currentindex = i;
                    GenericMenu.MenuFunction2 funccall = (object value)=>
                    {
                        if(value == null)
                        {
                            bind.units[currentindex].type = ComCommonBind.eBindUnitType.GameObject;
                            bind.units[currentindex].com  = bind.units[currentindex].com.GetComponent<RectTransform>();
                        }
                        else
                        {
                            bind.units[currentindex].type = ComCommonBind.eBindUnitType.Component;
                            if ((Component) value != bind.units[currentindex].com)
                            {
                                Undo.RecordObject (target, "com " + target.name);
                                bind.units[currentindex].com  = value as Component;
                                EditorUtility.SetDirty(target);
                            }
                        }
                    };
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("GameObject"), cur.type == ComCommonBind.eBindUnitType.GameObject, funccall, null);

                    for(int k = 0; k < coms.Length; ++k)
                    {
                        var curcom = coms[k];
                        menu.AddItem(new GUIContent(curcom.GetType().Name), cur.com == curcom, funccall, curcom);

                    }
                    menu.ShowAsContext();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            drawer.EndDrawElement();
        }

        if (drawer.DrawFooter(ref bind.units))
        {
            EditorUtility.SetDirty(target);
        }
        StringBuilderCache.Release(builder);
    }


    string ScriptPath = "Assets/";
    void OnViewBindGUI(ComArrayListDrawer drawer, ComCommonBind bind)
    {
        drawer.DrawHeader("", viewUnits.Length, this);
        StringBuilder builder = StringBuilderCache.Acquire();
        for (int i = 0; i < viewUnits.Length; ++i)
        {
            drawer.BeginDrawElement(i);
            var cur = viewUnits[i];
            EditorGUILayout.BeginVertical();
            builder.Clear();
            builder.AppendFormat("  [{0}] {1} : {2}", i, cur.tag, cur.com == null ? "Null" : cur.com.GetType().Name);
            if (!bSimpleShow) GUILayout.Label(builder.ToString(), drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            //if(string.IsNullOrEmpty(viewUnits[i].tag))
            //GUILayout.Label("    Pls Enter a Name:",EditorStyles.helpBox);
            //EditorGUILayout.
            if (!bSimpleShow) GUILayout.Label("    Tag:");
            var com = this.viewUnits[i].com;
            if (com is Button || com is Toggle || com is Dropdown)
            {
                viewUnits[i].isGenerateEventFunction = EditorGUILayout.Toggle("是否在Frame中处理事件", viewUnits[i].isGenerateEventFunction);
            }

            if (bSimpleShow) EditorGUILayout.BeginHorizontal();
            if (bSimpleShow) GUILayout.Label(builder.ToString(), drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            viewUnits[i].tag = EditorGUILayout.TextField(cur.tag);
            if (bSimpleShow) EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (viewUnits[i].com == null)
                GUILayout.Label("    Pls Select a Component or GameObject:", EditorStyles.helpBox);
            if (!bSimpleShow) GUILayout.Label("    Unit:");
            EditorGUILayout.BeginHorizontal();
            viewUnits[i].com = EditorGUILayout.ObjectField(cur.com, typeof(Component), true) as Component;
            if (cur.com != null)
            {
                if (cur.tag == null || cur.tag.Equals(""))
                {
                    viewUnits[i].tag = cur.com.name;
                }
                string name = cur.type == ComCommonBind.eBindUnitType.GameObject ? "GameObject" : cur.com.GetType().Name;
                if (GUILayout.Button(name, "GV Gizmo DropDown"))
                {
                    var coms = cur.com.GetComponents(typeof(Component));

                    int currentindex = i;
                    GenericMenu.MenuFunction2 funccall = (object value) =>
                    {
                        if (value == null)
                        {
                            viewUnits[currentindex].type = ComCommonBind.eBindUnitType.GameObject;
                            viewUnits[currentindex].com = viewUnits[currentindex].com.GetComponent<RectTransform>();
                        }
                        else
                        {
                            viewUnits[currentindex].type = ComCommonBind.eBindUnitType.Component;
                            viewUnits[currentindex].com = value as Component;
                        }
                    };
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("GameObject"), cur.type == ComCommonBind.eBindUnitType.GameObject, funccall, null);

                    for (int k = 0; k < coms.Length; ++k)
                    {
                        var curcom = coms[k];
                        menu.AddItem(new GUIContent(curcom.GetType().Name), cur.com == curcom, funccall, curcom);

                    }
                    menu.ShowAsContext();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            drawer.EndDrawElement();
        }
        drawer.DrawFooter(ref viewUnits);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成View", new GUIStyle(EditorStyles.miniButton)))
        {
            UnityEngine.Debug.Log("生成View了");
            viewScriptName = bind.gameObject.name + "View";
            ComCommonBindHelper.CreateView(bind.gameObject, ScriptPath, viewScriptName, viewUnits, ViewNameSpace);
            isCreatingViewScript = true;
        }

        if (GUILayout.Button("从View恢复", new GUIStyle(EditorStyles.miniButton)))
        {
            var component = bind.GetComponent(bind.gameObject.name + "View");
            Assert.IsNotNull(component);
            ComCommonBindHelper.RecoverUnitsFromView(component, ref viewUnits);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("生成ClientFrame", new GUIStyle(EditorStyles.miniButton)))
        {
            var viewClass = bind.GetComponent(bind.gameObject.name + "View");
            Assert.IsNotNull(viewClass);
            ComCommonBindHelper.CreateClientFrame(bind.gameObject.name + "Frame", ScriptPath, bind.gameObject.name + "View", ViewNameSpace, viewClass);
        }
        if (GUILayout.Button("绑定View到Frame", new GUIStyle(EditorStyles.miniButton)))
        {
            var viewClass = bind.GetComponent(bind.gameObject.name + "View");
            Assert.IsNotNull(viewClass);
            ComCommonBindHelper.BindViewToFrame(ref bind.units, viewClass, bind.gameObject.name + "View");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("脚本路径:" + ScriptPath);
        if (GUILayout.Button("选择脚本生成路径", new GUIStyle(EditorStyles.miniButton)))
        {
            ScriptPath = EditorUtility.OpenFolderPanel("选择脚本保存的目录", Application.dataPath, "") + "/";
        }
        EditorGUILayout.EndHorizontal();
        if (isCreatingViewScript)
        {
            AddComponentAfterCompiling(bind.gameObject);
        }

        StringBuilderCache.Release(builder);
    }

    bool isCreatingViewScript = false;
    string viewScriptName;

    void AddComponentAfterCompiling(GameObject go)
    {
        Type type = typeof(ComCommonBind).Assembly.GetType(ViewNameSpace + "." + viewScriptName);
        while (EditorApplication.isCompiling || type == null)
            return;
        ComCommonBindHelper.AddComponentAfterCompilingAction(go, viewScriptName, type, viewUnits);
        isCreatingViewScript = false;
    }


    void OnAssetGUI(ComArrayListDrawer drawer,ComCommonBind bind)
    {
        drawer.DrawHeader("",bind.reses.Length,this);
        StringBuilder builder = StringBuilderCache.Acquire();
        for(int i = 0; i < bind.reses.Length; ++i)
        {
            drawer.BeginDrawElement(i);
            var cur = bind.reses[i];
            EditorGUILayout.BeginVertical();
            builder.Clear();
            builder.AppendFormat("  [{0}] {1} : {2}",i,cur.tag,cur.sprite == null ? "":cur.sprite.name);
            if(!bSimpleShow)GUILayout.Label(builder.ToString(),drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            if(string.IsNullOrEmpty(bind.reses[i].tag))
            GUILayout.Label("    Pls Enter a Name:",EditorStyles.helpBox);
            //EditorGUILayout.
            if(!bSimpleShow)GUILayout.Label("    Tag:");
            if(bSimpleShow)EditorGUILayout.BeginHorizontal();
            if(bSimpleShow)GUILayout.Label(builder.ToString(),drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            bind.reses[i].tag = EditorGUILayout.TextField(cur.tag);
            if(bSimpleShow)EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if(bind.reses[i].sprite == null)
            GUILayout.Label("    Pls Select a Sprite Resource:",EditorStyles.helpBox);
            if(!bSimpleShow)GUILayout.Label("    Sprite:");
            bind.reses[i].sprite =  EditorGUILayout.ObjectField(cur.sprite,typeof(Sprite),false) as Sprite;
            if(null != bind.reses[i].sprite || null != bind.reses[i].sprite.texture)
            {
                string texPath = AssetDatabase.GetAssetPath(bind.reses[i].sprite.texture);
                string texExt = Path.GetExtension(texPath);
                string materialPath = texPath + "_Material.mat";
                if(null != texExt && texExt.Length > 0)
                {
                    materialPath = texPath.Replace(texExt, "_Material.mat");
                }

                bind.reses[i].material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            }
            EditorGUILayout.ObjectField(bind.reses[i].material, typeof(Material), false);

            EditorGUILayout.EndVertical();
            drawer.EndDrawElement();
        }
        drawer.DrawFooter(ref bind.reses);
        StringBuilderCache.Release(builder);
    }

    void OnPrefabGUI(ComArrayListDrawer drawer,ComCommonBind bind)
    {
        drawer.DrawHeader("",bind.prefabs.Length,this);
        StringBuilder builder = StringBuilderCache.Acquire();
        for(int i = 0; i < bind.prefabs.Length; ++i)
        {
            drawer.BeginDrawElement(i);
            var cur = bind.prefabs[i];
            EditorGUILayout.BeginVertical();
            builder.Clear();
            builder.AppendFormat("  [{0}] {1} : {2}",i,cur.tag,cur.path);
            if(!bSimpleShow)GUILayout.Label(builder.ToString(),drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            if(string.IsNullOrEmpty(bind.prefabs[i].tag))
            GUILayout.Label("    Pls Enter a Tag Name:",EditorStyles.helpBox);
            //EditorGUILayout.
            if(!bSimpleShow)GUILayout.Label("    Tag:");
            if(bSimpleShow)EditorGUILayout.BeginHorizontal();
            if(bSimpleShow)GUILayout.Label("["+i+"]",drawer.selected == i ? fontstyleUnitInfoSelect : fontstyleUnitInfo);
            bind.prefabs[i].tag = EditorGUILayout.TextField(cur.tag);
            if(bSimpleShow)EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if(string.IsNullOrEmpty(bind.prefabs[i].path))
            GUILayout.Label("    Pls Enter a prefab path:",EditorStyles.helpBox);
            if(!bSimpleShow)GUILayout.Label("    PrefabPath:");
            bind.prefabs[i].path =  EditorGUILayout.TextField(cur.path);
            EditorGUILayout.EndVertical();
            drawer.EndDrawElement();
        }
        drawer.DrawFooter(ref bind.prefabs);
        StringBuilderCache.Release(builder);
    }

    private bool bShowUnitBind = true;
    private bool bShowAssets = true;
    private bool bShowPrefab = true;

    private static bool bSimpleShow = true;
    private static bool bShowView = false;
    bool ToggleButton(bool bCheck,GUIContent content,GUIStyle style)
    {
        var back = GUI.backgroundColor;
        var colorPalete = DG.DemiEditor.DeGUI.colors;
        if(colorPalete == null)
        {
            Repaint();
            //return bCheck;
        }
        else
        {
            GUI.backgroundColor = bCheck ? Color.green : (Color)colorPalete.bg.toggleOff;
            style.normal.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
            style.active.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
        } 
       
        bool flag = GUILayout.Button(content,style);
        if(flag)
        {
            bCheck = !bCheck;
            GUI.changed = true;
        }
        GUI.backgroundColor = back;

        return bCheck;
    }
    bool ToggleButton(bool bCheck,GUIContent content)
    {
        var back = GUI.backgroundColor;
        var colorPalete = DG.DemiEditor.DeGUI.colors;
        if(colorPalete == null)
        {
            Repaint();
            //return bCheck; 
        }
        else
        {
            GUI.backgroundColor = bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
            deToggleButtonStyle.normal.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
            deToggleButtonStyle.active.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
        }
       
        deToggleButtonStyle.fontSize = 13;
        bool flag = GUILayout.Button(content,deToggleButtonStyle);
        if(flag)
        {
            bCheck = !bCheck;
            GUI.changed = true;
        }
        GUI.backgroundColor = back;

        return bCheck;
    }

    bool bCheck1;
    bool bCheck2;
    bool bCheck3;
    bool bCheck4;

    [SerializeField]
    public ViewBindUnit[] viewUnits = new ViewBindUnit[0];

    void DrawBox(Color c,int height)
    {
        var back = GUI.backgroundColor;
        GUI.backgroundColor = c;
        Rect rt = GUILayoutUtility.GetRect(1,height);
        rt.height = height;
        GUI.Box(rt,"",headerStyle);
        GUI.backgroundColor = back;
    }

    int index = 0;
    Rect headerRect;

    int searchTypeIndex = 0;
    string searchTypeText = "";
    Component searchTypeCom = null;
    int searchTypeComIndex = -1;

    public override void OnInspectorGUI()
    {
        if(Application.isPlaying)
        {
            GUILayout.Label("Playing Mode Can not Save",EditorStyles.helpBox);
            //return;
        }

        //serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        GUIControls.UIStyles.UpdateEditorStyles();
        CreateFontStyle();
        ComCommonBind bind = target as ComCommonBind;
        
        if(index < 0 || index > 3) index = 0;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        var fontsize = fontstyleUnitInfo.fontSize;
        fontstyleUnitInfo.fontSize = 18;
        EditorGUILayout.LabelField("[Bind System For UI]",fontstyleUnitInfo);
        bShowView = ToggleButton(bShowView, new GUIContent("Show View", ""), radioButtonStyle);
        fontstyleUnitInfo.fontSize = fontsize;
        bSimpleShow = ToggleButton(bSimpleShow,new GUIContent("Simple Show",""),radioButtonStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawBox(Color.white,4);
        GUILayout.BeginHorizontal();
        bCheck1 = ToggleButton(index == 0,new GUIContent("UnitsBind[" + bind.units.Length + "]","GameObject Binding help to use in Code."));
        if(bCheck1) index = 0;
        bCheck2 = ToggleButton(index == 1,new GUIContent("Sprites[" + bind.reses.Length + "]","Sprite Resource Binding help to use in Code."));
        if(bCheck2) index = 1;
        bCheck3 = ToggleButton(index == 2,new GUIContent("Prefabs[" + bind.prefabs.Length + "]","Prefab path Table help to use in Code."));
        if(bCheck3) index = 2;
        if (bShowView)
        {
            bCheck4 = ToggleButton(index == 3, new GUIContent("View[" + viewUnits.Length + "]", "View generator help to use in Code."));
            if (bCheck4)
            {
                index = 3;
            }
        }
        GUILayout.EndHorizontal();
        
        if(index == 0)
        {
            EditorGUILayout.BeginVertical();
            DrawBox(headerStyle.normal.textColor,6);
            OnUnitBindGUI(binddrawer,bind);
            EditorGUILayout.EndVertical();
        }else if(index == 1)
        {   
            EditorGUILayout.BeginVertical();
            DrawBox(headerStyle.normal.textColor,6);
            OnAssetGUI(spritedrawer,bind);
            EditorGUILayout.EndVertical();
        }else if(index == 2)
        {   EditorGUILayout.BeginVertical();
            DrawBox(headerStyle.normal.textColor,6);
            OnPrefabGUI(prefabdrawer,bind);
            EditorGUILayout.EndVertical();
        }else if (index == 3)
        {
            EditorGUILayout.BeginVertical();
            DrawBox(headerStyle.normal.textColor, 6);
            OnViewBindGUI(viewdrawer, bind);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(6);

        //serializedObject.ApplyModifiedProperties();
        //serializedObject.SetIsDifferentCacheDirty();
        if (EditorGUI.EndChangeCheck()) //other: if (GUI.changed)
        {
            UnityEngine.Debug.Log("ComCommonBind Inspector Changed!!!");
            //other: EditorUtility.SetDirty(target);
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                UnityEngine.Debug.Log("ComCommonBind Inspector in prefab stage");
                var prefabStageScene = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().scene;
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStageScene);
            }
        }

#if NEWADD_COMBIND
        GUI.color = Color.green;
        EditorGUILayout.BeginVertical();
        searchTypeIndex = EditorGUILayout.Popup("请选择搜索类型", searchTypeIndex, new string[4] { "Tag(节点自定义名称)", "Name(节点GameObject名称)", "Type(节点GameObject类型)", "Null(空节点)" });
        EditorGUILayout.BeginHorizontal();
        searchTypeText = EditorGUILayout.TextField("请输入搜索文本(忽略大小写,可只填部分)", searchTypeText);

        if (ComCommonBindHelper.HasNextSelectGameObject())
        {
            if (GUILayout.Button("下一个"))
            {
                searchTypeComIndex = ComCommonBindHelper.FocusOneSelectGameObject(bind.gameObject, binddrawer, this);
            }
            if (GUILayout.Button("停止搜索"))
            {
               searchTypeComIndex = ComCommonBindHelper.StopSearchFocusGameObejct(bind.gameObject);
            }
        }
        else
        {
            if (GUILayout.Button("搜索"))
            {
                searchTypeComIndex = ComCommonBindHelper.StopSearchFocusGameObejct(bind.gameObject);
                switch (searchTypeIndex)
                {
                    case 0:
                        searchTypeComIndex = ComCommonBindHelper.FocueSelectedPrefabByTag(searchTypeText, bind, binddrawer, this);
                        break;
                    case 1:
                        searchTypeComIndex = ComCommonBindHelper.FocusSelectedPrefabByName(searchTypeText, bind, binddrawer, this);
                        break;
                    case 2:
                        searchTypeComIndex = ComCommonBindHelper.FocusSelectedPrefabeByType(searchTypeText, bind, binddrawer, this);
                        break;
                    case 3:
                        searchTypeComIndex = ComCommonBindHelper.FocusSelectedPrefabByNullReference(bind, binddrawer, this);
                        break;
                }
            }
            if (GUILayout.Button("停止搜索"))
            {
               searchTypeComIndex = ComCommonBindHelper.StopSearchFocusGameObejct(bind.gameObject);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;
#endif
    }
}

[CustomPreview(typeof(ComCommonBind))]
public class ComCommonBindPreview : ObjectPreview
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override GUIContent GetPreviewTitle()
    {
        return new GUIContent("ComCommonBind");
    }

    private string _getFiled(string filed)
    {
        if (string.IsNullOrEmpty(filed))
        {
            return "";
        }

        return string.Format("{0}{1}", char.ToUpper(filed[0]), filed.Substring(1));
    }


    private string _getGenerateText(ComCommonBind bind)
    {
        string ans = "#region ExtraUIBind\n";
        // filed
        for (int i = 0; i < bind.units.Length; ++i)
        {
            string lable = "";
            if (bind.units[i].com != null)
            {
                if (bind.units[i].type == ComCommonBind.eBindUnitType.GameObject)
                {
                    lable = string.Format("private GameObject m{0} = null;\n", _getFiled( bind.units[i].tag));
                }
                else 
                {
                    lable = string.Format("private {0} m{1} = null;\n", bind.units[i].com.GetType().Name, _getFiled( bind.units[i].tag));
                }
                ans += lable;
            }
        }
        ans += "\n";

        ans += "protected override void _bindExUI()\n{\n";
        // init
        for (int i = 0; i < bind.units.Length; ++i)
        {
            if (bind.units[i].com != null)
            {
                string lable = "";

                if (bind.units[i].type == ComCommonBind.eBindUnitType.GameObject)
                {
                    lable = string.Format("\tm{0} = mBind.GetGameObject(\"{1}\");\n", _getFiled(bind.units[i].tag), bind.units[i].tag);
                }
                else
                {
                    lable = string.Format("\tm{1} = mBind.GetCom<{0}>(\"{2}\");\n", bind.units[i].com.GetType().Name, _getFiled(bind.units[i].tag), bind.units[i].tag);
                }
                ans += lable;

                //switch (bind.units[i].com )
                {
                        if (bind.units[i].com is Button)
                        {
                            string addcb = string.Format("\tm{0}.onClick.AddListener({1});\n", _getFiled(bind.units[i].tag), _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                        else if (bind.units[i].com is Toggle)
                        {
                            string addcb = string.Format("\tm{0}.onValueChanged.AddListener({1});\n", _getFiled(bind.units[i].tag), _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                        else if (bind.units[i].com is Dropdown)
                        {
                            string addcb = string.Format("\tm{0}.onValueChanged.AddListener({1});\n", _getFiled(bind.units[i].tag), _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                        //case ComCommonBind.eBindUnitType.ComSlider:
                        //    break;
                }
            }
        }
        ans += "}\n";

        ans += "\n";

        // uninit
        ans += "protected override void _unbindExUI()\n{\n";
        for (int i = 0; i < bind.units.Length; ++i)
        {
            if (bind.units[i].com != null)
            {

                //switch (bind.units[i].type)
                {
                        if (bind.units[i].com is Button)
                        {
                            string addcb = string.Format("\tm{0}.onClick.RemoveListener({1});\n", _getFiled(bind.units[i].tag), _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                        else if (bind.units[i].com is Toggle)
                        {
                            string addcb = string.Format("\tm{0}.onValueChanged.RemoveListener({1});\n", _getFiled(bind.units[i].tag), _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                        else if (bind.units[i].com is Dropdown)
                        {
                            string addcb = string.Format("\tm{0}.onValueChanged.RemoveListener({1});\n", _getFiled(bind.units[i].tag), _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                    //case ComCommonBind.eBindUnitType.ComSlider:
                    //    break;
                    //
                }

                string lable = string.Format("\tm{0} = null;\n", _getFiled(bind.units[i].tag));
                ans += lable;
            }
        }
        ans += "}\n#endregion   \n"; 
        ans += "\n";

        ans += "#region Callback\n";


        for (int i = 0; i < bind.units.Length; ++i)
        {
            if (bind.units[i].com != null)
            {
                //switch (bind.units[i].type)
                {
                        if (bind.units[i].com is Button)
                        {
                            string addcb = string.Format("private void {0}()\n{{\n\t/* put your code in here */\n\n}}\n", _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                        else if (bind.units[i].com is Toggle)
                        {
                            string addcb = string.Format("private void {0}(bool changed)\n{{\n\t/* put your code in here */\n\n}}\n", _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }
                        else if (bind.units[i].com is Dropdown)
                        {
                            string addcb = string.Format("private void {0}(int index)\n{{\n\t/* put your code in here */\n\n}}\n", _getBindButtonCB(bind.units[i]));
                            ans += addcb;
                        }

                }
            }
        }

        ans += "#endregion\n";


        // local
        for (int i = 0; i < bind.units.Length; ++i)
        {
            if (bind.units[i].com != null)
            {
                string lable = string.Empty;//string.Format("{0} {1} = bind.GetCom<{0}>(\"{1}\");\n", bind.units[i].com.GetType().Name, bind.units[i].tag);
                if (bind.units[i].type == ComCommonBind.eBindUnitType.GameObject)
                {
                    lable = string.Format("GameObject {1} = bind.GetGameObject(\"{1}\");\n", bind.units[i].com.GetType().Name, bind.units[i].tag);
                }
                else
                {
                    lable = string.Format("{0} {1} = bind.GetCom<{0}>(\"{1}\");\n", bind.units[i].com.GetType().Name, bind.units[i].tag);
                    //lable = string.Format("{0} {1} = bind.GetCom<{0}>(\"{2}\");\n", bind.units[i].com.GetType().Name, _getFiled(bind.units[i].tag), bind.units[i].tag);
                }
                ans += lable;
            }
        }
        return ans;
    }

    private string _getBindButtonCB(ComCommonBind.BindUnit unit)
    {
        //switch (unit.type)
        {
            if (unit.com is Button)
                return string.Format("_on{0}ButtonClick", _getFiled(unit.tag));
            else if (unit.com is Toggle)
                return string.Format("_on{0}ToggleValueChange", _getFiled(unit.tag));
            else if (unit.com is Dropdown)
                return string.Format("_on{0}DropdownValueChange", _getFiled(unit.tag));
        }

        return "";
    }

    public override void OnPreviewGUI(Rect r, GUIStyle bg)
    {
        ComCommonBind bind = target as ComCommonBind;
        if (null != bind)
        {
#if NEWADD_COMBIND
            if (GUILayout.Button("替换一波试试看"))
            {
                Replace(_getGenerateText(bind).Split('\n'), bind.gameObject);
            }

            if (GUILayout.Button("导出一下试试看"))
            {
                string bindGameObjectName = bind.gameObject.name;
                WriteComCommonBindTxt(bindGameObjectName, _getGenerateText(bind));
            }
#endif
            EditorGUILayout.BeginVertical();
            {
                GUIStyle st = new GUIStyle(GUI.skin.textArea);

                st.stretchHeight = true;

                EditorGUI.TextArea(r, _getGenerateText(bind), st);
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void WriteComCommonBindTxt(string fileName, string content)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }
        string filePath = Path.Combine(Application.dataPath,"./123/"+fileName+".txt");
        using (FileStream fs = File.Create(filePath))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(content);
            }
        }
        Logger.LogError("Output End Path is : " + filePath);
    }

    public void Replace(string[] bindStr, GameObject root)
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (null == root)
        {
            return;
        }

        string prefabName = root.name;

        if (!prefabName.Contains("Frame"))
        {
            prefabName += "Frame";
        }

        string csFilePath = _findCSFile(prefabName);
        if (string.IsNullOrEmpty(csFilePath))
        {
            return;
        }

        string[] lines = File.ReadAllLines(csFilePath);
        lines=_mergeLines(prefabName, lines, bindStr);

        File.WriteAllLines(csFilePath, lines);
    }

    private string[] _mergeLines(string csFileName, string[] originLines, string[] replaceLines)
    {
        List<string> allLines = new List<string>();

        bool isInBindArea = false;
        int indentLevel = 0;
        string[] indentLevelStr = new string[]
        {
            "",
            "\t",
            "\t\t",
            "\t\t\t",
            "\t\t\t\t",
            "\t\t\t\t\t",
            "\t\t\t\t\t\t",
        };

        bool hasExtraUIBindRegion = false;

        foreach (var line in originLines)
        {
            if (line.Contains("#region ExtraUIBind"))
            {
                hasExtraUIBindRegion = true;
                break;
            }
        }

        bool vgFlag = false;

        foreach (var line in originLines)
        {
            if (line.Contains("{"))
            {
                indentLevel++;
            }

            if (line.Contains("}"))
            {
                indentLevel--;
            }

            if (hasExtraUIBindRegion)
            {
                if (line.Contains("#region ExtraUIBind"))
                {
                    isInBindArea = true;
                    foreach (var replaceLine in replaceLines)
                    {
                        allLines.Add(indentLevelStr[indentLevel] + replaceLine);
                    }
                }

                if (!isInBindArea)
                {
                    allLines.Add(line);
                }

                if (isInBindArea && line.Contains("#endregion"))
                {
                    isInBindArea = false;
                }
            }
            else
            {
                allLines.Add(line);

                if (vgFlag)
                { 
                    foreach (var replaceLine in replaceLines)
                    {
                        allLines.Add(indentLevelStr[indentLevel] + replaceLine);
                    }

                    vgFlag = false;
                }

                if (line.Contains(csFileName) && line.Contains("class") && line.Contains(":") && line.Contains("ClientFrame"))
                {
                    vgFlag = true;
                }
            }
        }

        return allLines.ToArray();
    }

    private string _findCSFile(string fileName)
    {
        string[] allFindCSFiles = AssetDatabase.FindAssets(string.Format("t:script {0}", fileName));
        
        if (null != allFindCSFiles && allFindCSFiles.Length > 0)
        {
            return AssetDatabase.GUIDToAssetPath(allFindCSFiles[0]);
        }

        return string.Empty;
    }
}
