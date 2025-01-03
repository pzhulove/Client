
using UnityEditor;
using UnityEngine;
using Tenmove.Runtime.Unity;

[UnityEditor.CustomEditor(typeof(DSceneData))]
public class DSceneDataInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Editor"))
            DSceneDataEditorWindow.Init();
    }
}


[UnityEditor.CustomEditor(typeof(DSceneEntitySelection))]
public class SceneEntityInfoInspector  : UnityEditor.Editor
{
    private DSceneEntitySelection _Info;

    void OnEnable()
    {
        _Info = target as DSceneEntitySelection;
        //XlsxDataManager.Instance().AddMainEnterPoint("SceneTable");
    }
    public void OnPreSceneGUI()
    {
        /*
        if( Event.current.type == EventType.mouseDown
            && Event.current.button == 0 )
        {
            Event.current.Use();
        }
        */
    }
    GUIStyle fontStyle;

    static string[] regiontypestring;
    static string[] monsterswapstring;
    static string[] monsterfacestring;
    static string[] flowRegionTypeString;
    static string[] doortype = {"Normal", "PVEPracticeCourt" };

    public void SetDyeColor(Color dyeColor, GameObject[] modelRoot)
    {
        for (int i = 0; i < modelRoot.Length; ++i)
        {
            MeshRenderer[] amr = modelRoot[i].transform.GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < amr.Length; ++j)
            {
                Material[] am = amr[j].materials;
                for (int k = 0; k < am.Length; ++k)
                {
                    if (am[k].HasProperty("_DyeColor"))
                        am[k].SetColor("_DyeColor", dyeColor);
                }
            }
        }
    }

    


    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUIControls.UICommon.subGroupStyle);

        if (fontStyle == null)
        {
            fontStyle = new GUIStyle();
            fontStyle.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle.fontSize = 24;
            fontStyle.alignment = TextAnchor.UpperLeft;
            fontStyle.normal.textColor = Color.gray;
            fontStyle.hover.textColor = Color.gray;
        }

        DEntityInfo[] baseInfos = _Info.SelectedObjects;
        if (baseInfos != null && baseInfos.Length > 0)
            MultiPropertyGUI.DrawMultiProperty(baseInfos);
/*        if (baseInfos != null && baseInfos.Length > 0)
        {
            if (baseInfos.Length == 1)
            {
                _DiscardUI(baseInfos[0]);
            }
            else
            {
                _MultiUI(baseInfos);
            }
        }*/
        
        EditorGUILayout.EndVertical();
       
        EditorGUI.EndChangeCheck();
        if (GUI.changed)
        {
/*            Debug.LogError("changer");
            Undo.RecordObject(target, "entityInfo");
            EditorUtility.SetDirty(target);*/
            DSceneEntitySelection.MarkDirty();
            Repaint();
            SceneView.RepaintAll();
            
        }
    }

    protected override void OnHeaderGUI()
    {
       
    }
/*

    private void _MultiUI(DEntityInfo[] baseInfos)
    {
        MultiPropertyGUI.LabelField(baseInfos, "typename", "", fontStyle);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        MultiPropertyGUI.LabelField(baseInfos, "globalid", "单场景唯一ID");
        MultiPropertyGUI.LabelField(baseInfos, "resid", "资源ID");
        MultiPropertyGUI.TextField(baseInfos, "name", "名字", (item, str) => { 
            if (DSceneEntitySelection.onRenameEntityInfo != null)
            {
                DSceneEntitySelection.onRenameEntityInfo((DEntityInfo)item);
            }});
        
        MultiPropertyGUI.TextField(baseInfos, "description", "描述", (item, str) => { 
            if (DSceneEntitySelection.onRenameEntityInfo != null)
            {
                DSceneEntitySelection.onRenameEntityInfo((DEntityInfo)item);
            }});
        
        EditorGUILayout.Space();
        
        MultiPropertyGUI.Vector3Field(baseInfos, "position", "位置", (item, pos) => { 
            ((DEntityInfo)item).obj.transform.localPosition = pos;});

        EditorGUILayout.Space();

        DEntityType entityType = default(DEntityType);
        if (MultiPropertyGUI.IsSame(baseInfos, "type", ref entityType))
        {
            if (entityType != DEntityType.TRANSPORTDOOR)
            {
                EditorGUILayout.Space();

                MultiPropertyGUI.FloatField(baseInfos, "scale", "缩放",
                    (item, v) => { ((DEntityInfo) item).obj.transform.localScale = v * Vector3.one; });
                EditorGUILayout.Space();
            }
        }

        MultiPropertyGUI.ColorField(baseInfos, "color", "颜色", (item, color) => { 
            SetDyeColor(color, new GameObject[] { ((DEntityInfo)item).obj });});
        
        MultiPropertyGUI.LabelField(baseInfos, "path", "Path");

        if (MultiPropertyGUI.IsSame(baseInfos, "type", ref entityType))
        {
            if(doorinfo.materialAsset.m_AssetObj == null)
            {
                doorinfo.materialAsset.m_AssetObj = null;
            }
            if(doorinfo.materialAsset.m_AssetObj == null && !string.IsNullOrEmpty(doorinfo.materialAsset.m_AssetPath))
            {
                doorinfo.materialAsset.m_AssetObj = AssetDatabase.LoadAssetAtPath<Material>(doorinfo.materialAsset.m_AssetPath);
            }
            var material = (Material)EditorGUILayout.ObjectField("材质", doorinfo.materialAsset.m_AssetObj, typeof(Material), false);
            if(material != doorinfo.materialAsset.m_AssetObj)
            {
                doorinfo.materialAsset.m_AssetObj = material;
                doorinfo.materialAsset.m_AssetPath = FileTools.GetAssetFullPath(material);
                
                if(doorinfo.obj != null)
                { 
                    var materialReplacer = doorinfo.obj.GetComponent<MaterialReplacerComponent>();
                    if(materialReplacer != null)
                    {
                        materialReplacer.SetMaterial(material);
                    }
                }
            }
            EditorGUILayout.LabelField("材质路径：", doorinfo.materialAsset.m_AssetPath);


            doorinfo.isEggDoor = EditorGUILayout.Toggle("是否是彩蛋房间的门", doorinfo.isEggDoor);
            doorinfo.doortype = (TransportDoorType)EditorGUILayout.EnumPopup("始发地类型：", doorinfo.doortype);
            doorinfo.nextdoortype = doorinfo.doortype.OppositeType();

                    MultiPropertyGUI.QuaternionField(baseInfos, "rotation", "旋转", (o, q) =>
                            ((DDecoratorInfo) o).obj.transform.localRotation = q);
                }
                    break;
                case DEntityType.MONSTER:
                case DEntityType.BOSS:
                case DEntityType.ELITE:
                case DEntityType.ACTIVITY_BOSS_POS:
                case DEntityType.ACTIVITY_ELITE_POS:
                case DEntityType.ACTIVITY_MONSTER_POS:
                    {
                        MultiPropertyGUI.LabelField(baseInfos,"path", "Path");
                        foreach (var item in baseInfos)
                        {
                            DMonsterInfo monsteInfo = item as DMonsterInfo;
                            monsteInfo.SetID(item.resid);
                        }

                        if (monsterswapstring == null)
                        {
                            monsterswapstring = typeof(DMonsterInfo.MonsterSwapType).GetDescriptions();
                        }

                        if (monsterfacestring == null)
                        {
                            monsterfacestring = typeof(DMonsterInfo.FaceType).GetDescriptions();
                        }

                        MultiPropertyGUI.Popup(baseInfos, "swapType", "刷怪类型:", monsterswapstring);
                        MultiPropertyGUI.Popup(baseInfos, "faceType", "朝向:", monsterfacestring);
                        MultiPropertyGUI.IntField(baseInfos,"swapNum", "数量");
                        MultiPropertyGUI.IntField(baseInfos,"swapDelay", "延迟刷新:");
                        MultiPropertyGUI.IntField(baseInfos,"monsterLevel", "怪物等级:",(item, v) =>
                            {
                                ((DMonsterInfo) item)._encodeID();
                            });
                        
                        MultiPropertyGUI.IntField(baseInfos,"monsterDiffcute", "怪物剧情难度:",(item, v) =>
                        {
                            ((DMonsterInfo) item)._encodeID();
                        });
                        MultiPropertyGUI.IntField(baseInfos,"flushGroupID", "刷怪组ID:");
                        
                        if (flowRegionTypeString == null)
                        {
                            flowRegionTypeString = typeof(DMonsterInfo.FlowRegionType).GetDescriptions();
                        }
                        MultiPropertyGUI.Popup(baseInfos, "flowRegionType", "跟随区域类型:", flowRegionTypeString);
                        
                        var flowRegionType = default(DMonsterInfo.FlowRegionType);
                        if (MultiPropertyGUI.IsSame(baseInfos, "flowRegionType", ref flowRegionType))
                        {
                            if (flowRegionType == DMonsterInfo.FlowRegionType.Destruct)
                            {
                                var destructInfos = MultiPropertyGUI.FindProperty<DDestructibleInfo>(baseInfos, "destructInfo");
                                MultiPropertyGUI.IntField(destructInfos,"resid", "破坏物ID:");
                                foreach (var i in baseInfos)
                                {
                                    var monsterInfo = (DMonsterInfo)i;
                                    monsterInfo.destructInfo.scale = monsterInfo.scale;
                                    monsterInfo.destructInfo.position = monsterInfo.position;
                                }
                            }
                            
                            if (flowRegionType != DMonsterInfo.FlowRegionType.None)
                            {
                                _editorRegion( MultiPropertyGUI.FindProperty<DRegionInfo>(baseInfos, "regionInfo"), true);
                            }
                        }
                    }
                    break;
                case DEntityType.REGION:
                case DEntityType.TRANSPORTDOOR:
                case DEntityType.TOWNDOOR:
                    {
                        _editorRegion(baseInfos);
                    }
                    break;
                case DEntityType.RESOURCE_POS:
                    {
                        MultiPropertyGUI.IntField(baseInfos,"resouceId", "资源表ID:");
                       // var d = baseInfo as DResourceInfo;
                       // d.resouceId = EditorGUILayout.IntField("资源表ID", d.resouceId);
                    }
                    break;
                case DEntityType.FIGHTER_BORN_POS:
                    {
                        MultiPropertyGUI.IntField(baseInfos,"regionId", "区域ID:");
                       // var d = baseInfo as DTransferInfo;
                       // d.regionId = EditorGUILayout.IntField("区域ID", d.regionId);
                    }
                    break;
            }
        }
    }
    private void _editorRegion(object[] baseInfos, bool withID = false)
    {
        if (withID)
        {
            MultiPropertyGUI.IntField(baseInfos, "resid", "资源ID");
        }
        
        if (regiontypestring == null)
        {
            regiontypestring = typeof(DRegionInfo.RegionType).GetDescriptions();
        }

        MultiPropertyGUI.Popup(baseInfos, "regiontype", "区域类型", regiontypestring);
        var regiontype = default(DRegionInfo.RegionType);
        if (MultiPropertyGUI.IsSame(baseInfos, "regiontype", ref regiontype))
        {
            if (regiontype == DRegionInfo.RegionType.Circle)
            {
                MultiPropertyGUI.FloatField(baseInfos, "radius", "半径");
            }
            else
            {
                MultiPropertyGUI.Vector2Field(baseInfos, "rect", "矩形");
            }
        }

        // 传送门
        var infos = MultiPropertyGUI.ConvertTo<DTransportDoor>(baseInfos);
        if (infos != null && infos.Length > 0)
        {
            MultiPropertyGUI.Toggle(infos, "isEggDoor", "是否是彩蛋房间的门");
            
            var enumDesc = typeof(TransportDoorType).GetDescriptions();
            MultiPropertyGUI.Popup(infos, "doortype", "始发地类型", enumDesc);
            foreach (var info in infos)
            {
                var item = info as DTransportDoor;
                item.nextdoortype = item.doortype.OppositeType();
            }

            MultiPropertyGUI.LabelField(infos, "nextdoortype", "目的地类型");
            MultiPropertyGUI.TextField(infos, "townscenepath", "镇目的地路径（测试使用，请在版本过后删除）");
            MultiPropertyGUI.IntField(infos, "nextsceneid", "城镇目的地路径ID（测试使用，请在版本过后删除）");
            MultiPropertyGUI.Vector3Field(infos, "birthposition", "出生点位置");
        }

        // townDoor
        var townDoorInfos = MultiPropertyGUI.ConvertTo<DTownDoor>(baseInfos);
        if (townDoorInfos != null && townDoorInfos.Length > 0)
        {
            MultiPropertyGUI.IntField(townDoorInfos, "SceneID", "场景ID");
            MultiPropertyGUI.IntField(townDoorInfos, "DoorID", "门ID");
            MultiPropertyGUI.Vector3Field(townDoorInfos, "BirthPos", "传送点:");
            MultiPropertyGUI.IntField(townDoorInfos, "TargetSceneID", "目标场景ID:");
            MultiPropertyGUI.IntField(townDoorInfos, "TargetDoorID", "目标门ID:");
            MultiPropertyGUI.Popup(townDoorInfos, "DoorType", "门类型:", doortype);
        }
    }

    // 老接口用于单选显示
    private void _DiscardUI(DEntityInfo baseInfo)
    {
        if (baseInfo != null)
        {
            //EditorGUILayout.LabelField("", baseInfo.name, fontStyle);
            EditorGUILayout.LabelField("", baseInfo.typename, fontStyle);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("单场景唯一ID:", baseInfo.globalid.ToString());
            EditorGUILayout.LabelField("资源ID:", baseInfo.resid.ToString());

            string name = EditorGUILayout.TextField("名字:", baseInfo.name);
            if (name != baseInfo.name)
            {
                baseInfo.name = name;
                if (DSceneEntitySelection.onRenameEntityInfo != null)
                {
                    DSceneEntitySelection.onRenameEntityInfo(baseInfo);
                }
            }

            string description = EditorGUILayout.TextField("描述:", baseInfo.description);
            if (description != baseInfo.description)
            {
                baseInfo.description = description;
                if (DSceneEntitySelection.onRenameEntityInfo != null)
                {
                    DSceneEntitySelection.onRenameEntityInfo(baseInfo);
                }
            }
            
            EditorGUILayout.Space();
            baseInfo.position = EditorGUILayout.Vector3Field("位置:", baseInfo.position);
            if(baseInfo.obj != null)
            {
                baseInfo.obj.transform.localPosition = baseInfo.position;
            }
            EditorGUILayout.Space();

            if (baseInfo.type != DEntityType.TRANSPORTDOOR)
            {
                EditorGUILayout.Space();
                baseInfo.scale = EditorGUILayout.FloatField("缩放:", baseInfo.scale);
                if(baseInfo.obj != null)
                {
                    baseInfo.obj.transform.localScale = baseInfo.scale * Vector3.one;
                }
                EditorGUILayout.Space();
            }

            var color = EditorGUILayout.ColorField("颜色", baseInfo.color);
            if (color != baseInfo.color)
            {
                baseInfo.color = color;

                if (null != baseInfo.obj)
                {
                    SetDyeColor(baseInfo.color, new GameObject[] { baseInfo.obj });
                }
            }

            EditorGUILayout.LabelField("Path", baseInfo.path);

            switch (baseInfo.type)
            {
                case DEntityType.NPC:
                    {
                        var d = baseInfo as DNPCInfo;
                        d.minFindRange = EditorGUILayout.Vector2Field("最小寻路范围", d.minFindRange);
                        d.maxFindRange = EditorGUILayout.Vector2Field("最大寻路范围", d.maxFindRange);
                    }
                    break;
                case DEntityType.DESTRUCTIBLE:
                    {
                        var d = baseInfo as DDestructibleInfo;
                        d.flushGroupID = EditorGUILayout.IntField("刷怪组ID", d.flushGroupID);
                        //if (d.bflowRegion)
                        //{
                        //    _editorRegion(d.regionInfo, true);
                        //}
                    }
                    break;
                case DEntityType.DECORATOR:
                    {
                        var d = baseInfo as DDecoratorInfo;
                        d.localScale = EditorGUILayout.Vector3Field("修饰物缩放", d.localScale);
                        if (d.obj != null)
                        {
                            d.obj.transform.localScale = d.localScale;
                        }

                        {
                            var eulerAngle = d.rotation.eulerAngles;
                            var nAngle = EditorGUILayout.Vector3Field("旋转", eulerAngle);

                            if (nAngle != eulerAngle)
                            {
                                d.rotation = Quaternion.Euler(nAngle);

                                if (null != d.obj)
                                {
                                    d.obj.transform.localRotation = d.rotation;
                                }
                            }
                        }
                    }
                    break;
                case DEntityType.MONSTER:
                case DEntityType.BOSS:
                case DEntityType.ELITE:
				case DEntityType.ACTIVITY_BOSS_POS:
				case DEntityType.ACTIVITY_ELITE_POS:
				case DEntityType.ACTIVITY_MONSTER_POS:
                    {
                        EditorGUILayout.LabelField("Path", baseInfo.path);
                        DMonsterInfo info = baseInfo as DMonsterInfo;

                        info.SetID(info.resid);

                        if (monsterswapstring == null)
                        {
                            monsterswapstring = typeof(DMonsterInfo.MonsterSwapType).GetDescriptions();
                        }

                        if (monsterfacestring == null)
                        {
                            monsterfacestring = typeof(DMonsterInfo.FaceType).GetDescriptions();
                        }

                        info.swapType = (DMonsterInfo.MonsterSwapType)EditorGUILayout.Popup("刷怪类型：", (int)info.swapType, monsterswapstring);
                        info.faceType = (DMonsterInfo.FaceType)EditorGUILayout.Popup("朝向：",(int)info.faceType, monsterfacestring);
                        info.swapNum  = EditorGUILayout.IntField("数量:", info.swapNum);
                        info.swapDelay = EditorGUILayout.IntField(new GUIContent("延迟刷新:","MS为单位，负数为不会自动刷新."), info.swapDelay);

                        //info.monID = EditorGUILayout.LabelField("怪物", info.monID);
                        //info.monTypeID = EditorGUILayout.IntField("怪物类型", info.monTypeID);
                        info.monLevel = EditorGUILayout.IntField("怪物等级", info.monLevel);
                        info.monDiffcute = EditorGUILayout.IntField("怪物剧情难度", info.monDiffcute);

                        info.flushGroupID = EditorGUILayout.IntField("刷怪组ID", info.flushGroupID);
                        info.flowRegionType = (DMonsterInfo.FlowRegionType)EditorGUILayout.EnumPopup("跟随区域类型", info.flowRegionType);

                        if (info.flowRegionType == DMonsterInfo.FlowRegionType.Destruct)
                        {
                            info.destructInfo.resid = EditorGUILayout.IntField("破坏物ID", info.destructInfo.resid);
                            info.destructInfo.scale = info.scale;
                            info.destructInfo.position = info.position;
                        }

                        if (info.flowRegionType != DMonsterInfo.FlowRegionType.None)
                        {
                            _DiscardEditorRegion(info.regionInfo, true);
                        }

                    }
                    break;
                case DEntityType.REGION:
                case DEntityType.TRANSPORTDOOR:
                case DEntityType.TOWNDOOR:
                    {
                        _DiscardEditorRegion(baseInfo);
                    }
                    break;
                case DEntityType.RESOURCE_POS:
                    {
                        var d = baseInfo as DResourceInfo;
                        d.resouceId = EditorGUILayout.IntField("资源表ID", d.resouceId);
                    }
                    break;
                case DEntityType.FIGHTER_BORN_POS:
                    {
                        var d = baseInfo as DTransferInfo;
                        d.regionId = EditorGUILayout.IntField("区域ID", d.regionId);
                    }
                    break;
                case DEntityType.ECOSYSTEM_RESOURCE_POS:
                    {
                        var d = baseInfo as DEcosystemResourceInfo;
                        d.ecosystemResourceId = EditorGUILayout.IntField("生态资源表ID", d.ecosystemResourceId);
                        d.ecosystemResourceType = EditorGUILayout.IntField("生态资源表类型", d.ecosystemResourceType);
                    }
                    break;
            }
        }
    }
    
    private void _DiscardEditorRegion(DEntityInfo baseInfo, bool withID = false)
    {
        if (withID)
        {
            baseInfo.resid = EditorGUILayout.IntField("资源ID:", baseInfo.resid);
        }

        DRegionInfo info = baseInfo as DRegionInfo;

        if (regiontypestring == null)
        {
            regiontypestring = typeof(DRegionInfo.RegionType).GetDescriptions();
        }

        info.regiontype = (DRegionInfo.RegionType)EditorGUILayout.Popup("区域类型：", (int)info.regiontype, regiontypestring);
        if (info.regiontype == DRegionInfo.RegionType.Circle)
        {
            info.radius = EditorGUILayout.FloatField("半径:", info.radius);
        }
        else
        {
            info.rect = EditorGUILayout.Vector2Field("矩形:", info.rect);
        }

        // 传送门
        DTransportDoor doorinfo = baseInfo as DTransportDoor;
        if (doorinfo != null)
        {
            doorinfo.isEggDoor = EditorGUILayout.Toggle("是否是彩蛋房间的门", doorinfo.isEggDoor);
            doorinfo.doortype = (TransportDoorType)EditorGUILayout.EnumPopup("始发地类型：", doorinfo.doortype);
            doorinfo.nextdoortype = doorinfo.doortype.OppositeType();

            //doorinfo.nextdoortype = (TransportDoorType)EditorGUILayout.EnumPopup("目的地类型：", doorinfo.nextdoortype);
            EditorGUILayout.LabelField("目的地类型：", doorinfo.nextdoortype.ToString());

            doorinfo.townscenepath = EditorGUILayout.TextField("城镇目的地路径（测试使用，请在版本过后删除）：", doorinfo.townscenepath);
            doorinfo.nextsceneid = EditorGUILayout.IntField("城镇目的地路径ID（测试使用，请在版本过后删除）：", doorinfo.nextsceneid);
            doorinfo.birthposition = EditorGUILayout.Vector3Field("出生点位置:", doorinfo.birthposition);
            //doorinfo.nextsceneid = EditorGUILayout.IntField("传送目的地:", doorinfo.nextsceneid);
            //EditorGUI.indentLevel++;
            //XlsxDataManager.Instance().OnGUIUpdate("SceneTable", "数据表", doorinfo.nextsceneid);
            //EditorGUI.indentLevel--;
        }

        DTownDoor townDoor = baseInfo as DTownDoor;
        if (townDoor != null)
        {
            townDoor.SceneID = EditorGUILayout.IntField("场景ID：", townDoor.SceneID);
            townDoor.DoorID = EditorGUILayout.IntField("门ID：", townDoor.DoorID);

            townDoor.BirthPos = EditorGUILayout.Vector3Field("传送点：", townDoor.BirthPos);

            townDoor.TargetSceneID = EditorGUILayout.IntField("目标场景ID：", townDoor.TargetSceneID);
            townDoor.TargetDoorID = EditorGUILayout.IntField("目标门ID：", townDoor.TargetDoorID);
            townDoor.DoorType = (DoorTargetType)EditorGUILayout.Popup("门类型：", (int)townDoor.DoorType, doortype);
        }
    }*/
}
