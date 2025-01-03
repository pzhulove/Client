using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using System.Linq;
using System.IO;
using System.Text;
using GameClient;

public class FEquipHandBookSourceScripts
{
    public interface IItemSourceInfo
    {
        SourceInfo sourceInfo { get; }
    }

    #region 测试
    [MenuItem("[TM工具集]/[EquipHandbook]/*生成空来源装备列表")]
    public static void Generate()
    {
        var equipTb = TableManager.instance.GetTable<EquipHandbookCollectionTable>();

        StringBuilder builder = StringBuilderCache.Acquire();

        GameClient.ItemSourceInfoTableManager.GetInstance().Clear();
        GameClient.ItemSourceInfoTableManager.GetInstance().Initialize();

        foreach (var equipHb in equipTb)
        {
            var tb = equipHb.Value as EquipHandbookCollectionTable;

            List<int> allItems = new List<int>();
            if (tb.Type == EquipHandbookCollectionTable.eType.eEquipSuit)
            {
                var suit = TableManager.instance.GetTableItem<EquipSuitTable>(tb.EquipSuitID);

                allItems.AddRange(suit.EquipIDs);
            }
            else
            {
                allItems.AddRange(tb.CustomEquipIDs);
            }

            foreach (var id in allItems)
            {
                if(!GameClient.ItemSourceInfoTableManager.GetInstance().IsContainSourceInfoExceptAuction(id))
                {
                    var data = TableManager.instance.GetTableItem<ItemTable>(id);
                    if (null != data)
                    {
                        builder.AppendFormat("{0} {1} {2} {3} {4}\n", tb.ID, tb.EquipHandbookContentID, tb.Name, id, data.Name);
                    }
                    else
                    {
                        builder.AppendFormat("{0} {1} {2} {3} *-\n", tb.ID, tb.EquipHandbookContentID, tb.Name, id);
                    }
                }
            }
        }

        File.WriteAllText("./allMissingEquips.txt", builder.ToString());
    }
    #endregion

    #region 生成装备基础分数
    [MenuItem("[TM工具集]/[EquipHandbook]/*生成装备基础分数")]
    public static void GenerateTheItemScore()
    {
        GameClient.EquipMasterDataManager.GetInstance().Initialize();
        GameClient.ItemDataManager.GetInstance().InitData();

        var equipTb = TableManager.instance.GetTable<ItemTable>();
        int cnt = 0;
        List<ItemScoreInfo> itemScoreInfo = new List<ItemScoreInfo>();
        foreach (var equipHb in equipTb)
        {
            var tb = equipHb.Value as ItemTable;

            int id = tb.ID;
            cnt++;
            EditorUtility.DisplayProgressBar("计算装备基础评分", string.Format("{0} {1}", tb.ID, tb.Name), 1.0f * cnt / equipTb.Count);

            if (tb.Type != ItemTable.eType.EQUIP)
            {
                continue;
            }

            {
                bool isFind = false;
                foreach (var item in itemScoreInfo)
                {
                    if (item.itemID==id)
                    {
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    ItemScoreInfo info = new ItemScoreInfo();
                    GameClient.ItemData data = GameClient.ItemDataManager.CreateItemDataFromTable(id);
                    data.RefreshRateScore();
                    info.itemID = id;
                    info.score = data.finalRateScore;
                    itemScoreInfo.Add(info);
                }
            }
        }

        ItemSourceInfoTable assettn = AssetDatabase.LoadAssetAtPath<ItemSourceInfoTable>(kAssetPathInEditor);
        if (assettn != null)
        {
            itemScoreInfo.Sort();

            assettn.scores = itemScoreInfo.ToArray();
            EditorUtility.SetDirty(assettn);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();

            EditorGUIUtility.PingObject(assettn);
            Selection.activeObject = assettn;
        }
        else
        {
            Logger.LogErrorFormat("[装备图鉴] 无法加载 {0}", kAssetPathInEditor);
        }

        EditorUtility.ClearProgressBar();
    }

    #endregion

    #region 主逻辑

    //public Dictionary<int, ItemSourceInfo> mDictItemID2ItemSourceInfo = new Dictionary<int, ItemSourceInfo>();
    //public List<string> mSouceInfoNameCacheTables = new List<string>();

    private List<MainNode> mAllSourceInfos = new List<MainNode>();
    private class MainNode : System.IComparable<MainNode>
    {
        public MainNode(int itemId)
        {
            this.itemId = itemId;
        }

        public int itemId { private set; get; }
        public List<SourceInfo> sourceInfos = new List<SourceInfo>();

        public ItemSourceInfo Convert2ItemSourceInfo()
        {
            ItemSourceInfo info = new ItemSourceInfo();
            info.itemID = itemId;
            sourceInfos.Sort();
            List<SourceInfo> infos = new List<SourceInfo>();

            foreach (var x in sourceInfos)
            {
                bool isSame = false;
                foreach (var item in infos)
                {
                    if (item.CompareTo(x) == 0)
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame)
                {
                    infos.Add(x);
                }
            }
            
            info.sources = infos.ToArray();

            return info;
        }

        public int CompareTo(MainNode other)
        {
            return itemId - other.itemId;
        }
    }

    [MenuItem("[TM工具集]/[EquipHandbook]/*GenerateSource")]
    public static void Main()
    {
        FEquipHandBookSourceScripts scripts = new FEquipHandBookSourceScripts();

        EditorUtility.DisplayProgressBar("加载", "", 0);

        scripts._loadFromItemIdJarTypeMap();
        scripts._loadDungeonNodeMap();
        scripts._loadItemId2LegendNodesMap();
        scripts._loadItemId2MallNodesMaps();
        scripts._loadItemId2ShopNodesMaps();
        scripts._loadItemIdTeamDuplicateNodesMaps();

        var items = TableManager.instance.GetTable<ItemTable>();

        EditorUtility.DisplayProgressBar("匹配", "", 0);

        int cnt = 0;
        foreach (var iter in items)
        {
            cnt++;

            var itemdata = iter.Value as ItemTable;

            if (itemdata.Type == ItemTable.eType.EQUIP)
            {
                EditorUtility.DisplayProgressBar("匹配", string.Format("道具 {0}", itemdata.Name), cnt * 1.0f / items.Count);
                
                scripts.MatchJarNode(itemdata);
                scripts.MatchAuction(itemdata);
                scripts.MatchDungeonNode(itemdata);
                scripts.MatchLegendNode(itemdata);
                scripts.MatchMallNode(itemdata);
                scripts.MatchShopNode(itemdata);
                scripts.MatchTeamDuplicateNode(itemdata);
            }
        }

        scripts.GenerateFinalData();

        GenerateTheItemScore();

        EditorUtility.ClearProgressBar();
    }

    private static string kAssetPathInEditor = "Assets/Resources/" + GameClient.ItemSourceInfoTableManager.kItemSourceInfoTablePath;

    public void GenerateFinalData()
    {
        mAllSourceInfos.Sort();

        string assetPath = kAssetPathInEditor;

        if (File.Exists(assetPath))
        {
            File.Delete(assetPath);
        }

        ItemSourceInfoTable tb = ScriptableObject.CreateInstance<ItemSourceInfoTable>();

        Dictionary<string, int> allStringDicts = new Dictionary<string, int>();

        List<ItemSourceInfo> itemSourceInfo = new List<ItemSourceInfo>();
        mAllSourceInfos.ForEach(x => {
            itemSourceInfo.Add(x.Convert2ItemSourceInfo());
        }); 

        tb.sources = itemSourceInfo.ToArray();

        foreach (var item in tb.sources)
        {
            foreach (var single in item.sources)
            {
                if (!allStringDicts.ContainsKey(single.name))
                {
                    allStringDicts.Add(single.name, 0);
                }

                if (!allStringDicts.ContainsKey(single.linkParm))
                {
                    allStringDicts.Add(single.linkParm, 0);
                }

                allStringDicts[single.name]++;
                allStringDicts[single.linkParm]++;
            }
        }

        List<string> allStrs = allStringDicts.Keys.ToList();
        allStrs.Sort();

        foreach (var item in tb.sources)
        {
            foreach (var single in item.sources)
            {
                single.nameIndex = allStrs.FindIndex(x=> { return x==single.name; });
                single.linkParmIndex = allStrs.FindIndex(x=> { return x==single.linkParm; });
            }
        }

        tb.strTables = allStrs.ToArray();

        AssetDatabase.CreateAsset(tb, assetPath);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();

        EditorGUIUtility.PingObject(tb);
        Selection.activeObject = tb;
    }

    public void AddSourceInfo(int itemId, IItemSourceInfo info)
    {
        if (null == info)
        {
            return;
        }

        MainNode node = _findMainNode(itemId);
        if (null == node)
        {
            node = new MainNode(itemId);
            mAllSourceInfos.Add(node);
        }

        node.sourceInfos.Add(info.sourceInfo);
    }

    private MainNode _findMainNode(int itemId)
    {
        foreach (var item in mAllSourceInfos)
        {
            if (item.itemId == itemId)
            {
                return item;
            }
        }

        return null;
    }
    #endregion

    #region 罐子
    private int[] kJarNodeId = new int[] {
        801, 802,
        601, 602, 603, 604,605,606,
        901, 902, 903, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920 };

    private class JarNode : IItemSourceInfo
    {
        public JarNode(ProtoTable.JarBonus tb)
        {
            this.type = tb.Type;
            this.name = tb.Name;

            var pocketjartype = _convert2EPoketJarType(type);
            int levellimit = 0;

            if (pocketjartype != GameClient.EPocketJarType.Invalid)
            {
                if (pocketjartype != EPocketJarType.Artifact_Tank60)
                {
                    if (pocketjartype == GameClient.EPocketJarType.Gold)
                    {
                        this.name = "金罐";
                        if (tb.Filter.Count > 1)
                        {
                            levellimit = tb.Filter[1];
                        }
                    }
                    int openID = 36;
                    if (pocketjartype == GameClient.EPocketJarType.Magic_Lv55)
                    {
                        openID = 68;
                    }
                    mSourceInfo = new SourceInfo()
                    {
                        type = eItemSourceType.eJar,
                        name = name,
                        linkParm = string.Format("<type=framename param={0} value=GameClient.PocketJarFrame>", (int)_convert2EPoketJarType(type)),
                        openFunctionID = openID,
                        data = levellimit,
                    };
                }
                else
                {
                    mSourceInfo = new SourceInfo()
                    {
                        type = eItemSourceType.eArtifact_Tank60,
                        name = name,
                        linkParm = string.Format("<type=framename value=GameClient.ActivityJarFrame>"),
                    };
                }
            }
            else
            {
                //this.name = "袖珍罐";
                if (tb.Filter.Count > 0)
                {
                    levellimit = tb.Filter[0];
                }

                var activityTb = TableManager.instance.GetTable<ActivityJarTable>();
                int activityId = -1;
                foreach (var item in activityTb)
                {
                    ActivityJarTable curTb = item.Value as ActivityJarTable;
                    
                    if (tb.ID == curTb.JarID)
                    {
                        activityId = curTb.ID;
                        break;
                    }
                }

                mSourceInfo = new SourceInfo()
                {
                    type = eItemSourceType.eJar,
                    name = name,
                    linkParm = string.Format("<type=framename param={0} value=GameClient.ActivityJarFrame>", activityId),
                    openFunctionID = 58,
                    data = levellimit,
                };
            }
        }

        private GameClient.EPocketJarType _convert2EPoketJarType(ProtoTable.JarBonus.eType type)
        {
            switch (type)
            {
                case ProtoTable.JarBonus.eType.MagicJar:
                    return GameClient.EPocketJarType.Magic;
                case ProtoTable.JarBonus.eType.MagicJar_Lv55:
                    return GameClient.EPocketJarType.Magic_Lv55;
                case ProtoTable.JarBonus.eType.GoldJar:
                    return GameClient.EPocketJarType.Gold;
                case ProtoTable.JarBonus.eType.EquipJar:
                    return GameClient.EPocketJarType.Artifact_Tank60;
            }

            return GameClient.EPocketJarType.Invalid;
        }

        public ProtoTable.JarBonus.eType type { private set; get; }
        public string name { private set; get; }

        private SourceInfo mSourceInfo = null;

        public SourceInfo sourceInfo
        {
            get { return mSourceInfo; }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", type, name);
        }
    }

    public void MatchJarNode(ItemTable data)
    {
        if (mDictItemID2ListJarTypeIDs.ContainsKey(data.ID))
        {
            //string ans = string.Empty;

            foreach (var itemdataiter in mDictItemID2ListJarTypeIDs[data.ID])
            {
                AddSourceInfo(data.ID, itemdataiter);

                //ans += itemdataiter.name;
                //ans += ",";
            }

            //Logger.LogErrorFormat("[jar] {0} -> {1}", data.Name, ans);
        }
    }

    /// <summary>
    /// ItemID->List<int> JarTypeID
    /// </summary>
    private Dictionary<int, List<JarNode>> mDictItemID2ListJarTypeIDs = null;
    private List<JarNode> mAllJarTypeCache = new List<JarNode>();

    private void _loadFromItemIdJarTypeMap()
    {
        if (null != mDictItemID2ListJarTypeIDs)
        {
            return;
        }

        mDictItemID2ListJarTypeIDs = new Dictionary<int, List<JarNode>>();

        mAllJarTypeCache.Clear();

        var jarTable = TableManager.instance.GetTable<JarItemPool>();

        int cnt = 0;

        foreach (var curTable in jarTable)
        {
            JarItemPool poolItem = curTable.Value as JarItemPool;

            cnt++;

            EditorUtility.DisplayProgressBar("加载", string.Format("罐子 {0}", poolItem.ID), cnt * 1.0f / jarTable.Count);

            if (!_isValidNode(poolItem))
            {
                continue;
            }
            //JarBonus.eType jartype = (JarBonus.eType)poolItem.JarType;
            //if (jartype != JarBonus.eType.GoldJar &&
            //    jartype != JarBonus.eType.MagicJar &&
            //    jartype != JarBonus.eType.MagicJar_Lv55 &&
            //    jartype != JarBonus.eType.EquipJar
            //    )
            //{
            //    continue;
            //}

            if (!mDictItemID2ListJarTypeIDs.ContainsKey(poolItem.ItemID))
            {
                mDictItemID2ListJarTypeIDs.Add(poolItem.ItemID, new List<JarNode>());
            }

            var jarBonusItem = TableManager.instance.GetTableItem<ProtoTable.JarBonus>(poolItem.JarType);

            mDictItemID2ListJarTypeIDs[poolItem.ItemID].Add(_getJarNode(jarBonusItem));
        }

        EditorUtility.ClearProgressBar();
    }

    private bool _isValidNode(JarItemPool poolItem)
    {
        foreach (var item in kJarNodeId)
        {
            if (item == poolItem.JarType)
            {
                return true;
            }
        }

        return false;
    }

    private JarNode _getJarNode(ProtoTable.JarBonus tb)
    {
        string name = tb.Name;

        if (tb.Type == ProtoTable.JarBonus.eType.GoldJar)
        {
            name = "金罐";
        }

        for (int i = 0; i < mAllJarTypeCache.Count; i++)
        {
            if (mAllJarTypeCache[i].type == tb.Type &&
                mAllJarTypeCache[i].name == name)
            {
                return mAllJarTypeCache[i];
            }
        }

        JarNode node = new JarNode(tb);
        mAllJarTypeCache.Add(node);
        return node;
    }
    #endregion

    #region 副本掉落
    private class DungeonNode : IItemSourceInfo, System.IComparable<DungeonNode>
    {
        public DungeonNode(int dungeonID, string name)
        {
            this.dungeonID = dungeonID;
            this.name = name;

            DungeonTable tb = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);
            string linkParm = string.Empty;
            int openFunctionId = -1;
            eItemSourceType type = eItemSourceType.eNone;

            if (_InternalUtility.IsNormalType(tb))
            {
                linkParm = string.Empty;
                type = eItemSourceType.eDungeon;
            }
            else
            {
                type = eItemSourceType.eDungeonActivity;

                switch (tb.SubType)
                {
                    case DungeonTable.eSubType.S_HELL:
                    case DungeonTable.eSubType.S_HELL_ENTRY:
                        linkParm = string.Format("<type=framename param=1|{0}|{0} value=GameClient.ChallengeMapFrame>", _InternalUtility.GetHellEntryID(tb));
                        name = _InternalUtility.GetHellEntryName(tb);
                        openFunctionId = 23;
                        break;
                    case DungeonTable.eSubType.S_JINBI:
                    case DungeonTable.eSubType.S_NANBUXIGU:
                    case DungeonTable.eSubType.S_NIUTOUGUAI:
                    case DungeonTable.eSubType.S_SIWANGZHITA:
                        openFunctionId = 23;
                        linkParm = string.Format("<type=framename param={0}|1|{0} value=GameClient.ActivityDungeonFrame>", dungeonID);
                        break;
                    case DungeonTable.eSubType.S_YUANGU:
                        linkParm = string.Format("<type=framename param=2|{0}|{0} value=GameClient.ChallengeMapFrame>", dungeonID);
                        openFunctionId = 24;
                        break;
                    case DungeonTable.eSubType.S_GUILDPK:
                    //case DungeonTable.eSubType.S_MONEYREWARDS_PVP:
                        linkParm = string.Format("<type=framename param=1|2 value=GameClient.ActivityDungeonFrame>", dungeonID);
                        break;
                    case DungeonTable.eSubType.S_WUDAOHUI:
                        linkParm = string.Format("<type=framename param=0|2 value=GameClient.ActivityDungeonFrame>", dungeonID);
                        break;
                    case DungeonTable.eSubType.S_WEEK_HELL:
                    case DungeonTable.eSubType.S_WEEK_HELL_ENTRY:
                    case DungeonTable.eSubType.S_WEEK_HELL_PER:
                        linkParm = string.Format("<type=framename param=3|800000|800000 value=GameClient.ChallengeMapFrame>");
                        break;
                }
            }

            mInfo = new SourceInfo()
            {
                type = type,
                name = name,
                linkParm = linkParm,
                data = dungeonID,
                openFunctionID = openFunctionId,
            };
        }

        private SourceInfo mInfo = null;

        public int dungeonID { private set; get; }
        public string name { private set; get; }

        public SourceInfo sourceInfo
        {
            get { return mInfo; }
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", dungeonID, name);
        }

        public int CompareTo(DungeonNode other)
        {
            return dungeonID - other.dungeonID;
        }
    }

    private Dictionary<int, List<DungeonNode>> mDictItemID2DungeonNodes = null;
    private List<DungeonNode> mCacheDungeonNodes = new List<DungeonNode>();

    public void MatchDungeonNode(ItemTable data)
    {
        if (null == data)
        {
            return;
        }

        if (!mDictItemID2DungeonNodes.ContainsKey(data.ID))
        {
            return;
        }

        var dungeons = mDictItemID2DungeonNodes[data.ID];
        string ans = string.Empty;

        foreach (var item in dungeons)
        {

            AddSourceInfo(data.ID, item);

            //ans += item;
            //ans += ",";
        }

        //Logger.LogErrorFormat("[dungeon] {0} {1}", data.Name ,ans);
    }

    private void _loadDungeonNodeMap()
    {
        EditorUtility.DisplayProgressBar("加载", "加载数据。。。", 0);
        _loadDungeonDropItemTable();

        /*
        File.WriteAllText("groupID2items.txt", JsonUtility.ToJson(mDictDropGroupID2ItemsList));

        StringBuilder builder = StringBuilderCache.Acquire(5000);
        foreach (var item in mDictDropGroupID2ItemsList)
        {
            builder.Append("groupId ");
            builder.Append(item.Key);
            builder.AppendLine();
            foreach(var id in item.Value)
            {
                var data = TableManager.instance.GetTableItem<ItemTable>(id);
                if (null == data)
                {
                    builder.AppendFormat("{0} - -\n", id);
                }
                else
                {
                    builder.AppendFormat("{0} {1} {2}\n", data.ID, data.Name, data.Color);
                }
            }
            builder.AppendLine();
        }

        File.WriteAllText("groupIDitems-detail.txt", builder.ToString());
        */

        if (null == mDictDropGroupID2ItemsList)
        {
            return;
        }

        if (null != mDictItemID2DungeonNodes)
        {
            return;
        }

        mDictItemID2DungeonNodes = new Dictionary<int, List<DungeonNode>>();

        var dungeonTable = TableManager.instance.GetTable<DungeonTable>();

        TableManager.instance.AddTableInEditorMode(typeof(DungeonChestTable));

        int cnt = 0;

        foreach (var iter in dungeonTable)
        {
            cnt++;

            DungeonTable dt = iter.Value as DungeonTable;

            if (dt.SubType == DungeonTable.eSubType.S_TEAM_BOSS||
                dt.SubType == DungeonTable.eSubType.S_MONEYREWARDS_PVP ||
                dt.SubType == DungeonTable.eSubType.S_WUDAOHUI)
            {
                continue;
            }

            if (dt.SubType == DungeonTable.eSubType.S_YUANGU)
            {
                int k = 0;
            }

            DungeonID did = new DungeonID(dt.ID);
            if (did.prestoryID > 0)
            {
                continue;
            }


            EditorUtility.DisplayProgressBar("加载", string.Format("地下城数据 {0} 场景掉落", dt.Name), cnt * 1.0f / dungeonTable.Count);

            dt.NormalMonsterDrop.ForEach(dropGroupId => { _generateItemId2DungeonIDMap(dt, dropGroupId); });
            dt.EliteMonsterDrop.ForEach(dropGroupId =>  { _generateItemId2DungeonIDMap(dt, dropGroupId); });
            dt.BossMonsterDrop.ForEach(dropGroupId =>   { _generateItemId2DungeonIDMap(dt, dropGroupId); });
            dt.DungeonDrop.ForEach(dropGroupId =>       { _generateItemId2DungeonIDMap(dt, dropGroupId); });
            dt.DungeonFirstDrop.ForEach(dropGroupId =>  { _generateItemId2DungeonIDMap(dt, dropGroupId); });
            dt.DestructionDrop.ForEach(dropGroupId =>   { _generateItemId2DungeonIDMap(dt, dropGroupId); });
            dt.EasterEggDrop.ForEach(dropGroupId =>     { _generateItemId2DungeonIDMap(dt, dropGroupId); });
            dt.TaskDrop.ForEach(dropGroupId =>          { _generateItemId2DungeonIDMap(dt, dropGroupId); });

            if (dt.SubType == DungeonTable.eSubType.S_HELL ||
                dt.SubType == DungeonTable.eSubType.S_HELL_ENTRY)
            {
                for (int i = 0; i < dt.HellDrop1.Length; i++)
                {
                    _generateItemId2DungeonIDMap(dt, dt.HellDrop1[i]);
                }

                for (int i = 0; i < dt.HellDrop2.Length; i++)
                {
                    _generateItemId2DungeonIDMap(dt, dt.HellDrop2[i]);
                }
            }


            EditorUtility.DisplayProgressBar("加载", string.Format("地下城数据 {0} 怪物掉落", dt.Name), cnt * 1.0f / dungeonTable.Count);
            // TODO 怪物的特定掉落
            {
                DDungeonData data = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", Path.ChangeExtension(dt.DungeonConfig, "asset")), typeof(DDungeonData)) as DDungeonData;
                if (null != data)
                {
                    foreach (var con in data.areaconnectlist)
                    {
                        DSceneData scene = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", Path.ChangeExtension(con.sceneareapath, "asset")), typeof(DSceneData)) as DSceneData;
                        if (null != scene)
                        {
                            foreach (var mon in scene._monsterinfo)
                            {
                                var montb = TableManager.instance.GetTableItem<UnitTable>(mon.resid);
                                if (null != montb)
                                {
                                    _generateItemId2DungeonIDMap(dt, montb.DropID);
                                }
                            }
                        }
                    }
                }
            }

            // TODO 结算宝箱表

            EditorUtility.DisplayProgressBar("加载", string.Format("地下城数据 {0} 结算宝箱表", dt.Name), cnt * 1.0f / dungeonTable.Count);

            var chestTable = TableManager.instance.GetTableItem<DungeonChestTable>(dt.ID);
            if (null != chestTable)
            {
                _generateItemId2DungeonIDMap(dt, chestTable.GoldID);
                _generateItemId2DungeonIDMap(dt, chestTable.NormalChestID);
                _generateItemId2DungeonIDMap(dt, chestTable.PayChestID);
                _generateItemId2DungeonIDMap(dt, chestTable.VipChestID);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private void _generateItemId2DungeonIDMap(DungeonTable dungeonTable, int dropGroupId, eItemSourceType type = eItemSourceType.eNone)
    {
        List<int> allItems = _findDropItemsByGroupId(dropGroupId);

        if (null == allItems)
        {
            return;
        }

        foreach (var item in allItems)
        {
            if (!mDictItemID2DungeonNodes.ContainsKey(item))
            {
                mDictItemID2DungeonNodes.Add(item, new List<DungeonNode>());
            }

            if (_InternalUtility.IsNormalType(dungeonTable))
            {
                mDictItemID2DungeonNodes[item].Add(_findDungeonNodes(dungeonTable.ID / 10 * 10, dungeonTable.Name));
            }
            else
            {
                if (dungeonTable.SubType == DungeonTable.eSubType.S_HELL ||
                    dungeonTable.SubType == DungeonTable.eSubType.S_HELL_ENTRY)
                {
                    dungeonTable = _InternalUtility.GetHellEntryTb(dungeonTable);
                    if (null != dungeonTable)
                    {
                        mDictItemID2DungeonNodes[item].Add(_findDungeonNodes(dungeonTable.ID / 10 * 10, dungeonTable.Name));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    mDictItemID2DungeonNodes[item].Add(_findDungeonNodes(dungeonTable.ID / 10 * 10, dungeonTable.Name));
                }
            }
        }
    }

    private class _InternalUtility
    {
        public static string GetHellEntryName(DungeonTable tb)
        {
            var ftb = GetHellEntryTb(tb);

            if (null != ftb)
            {
                return ftb.Name;
            }

            return string.Empty;
        }

        public static int GetHellEntryID(DungeonTable tb)
        {
            var ftb = GetHellEntryTb(tb);
            if (null != ftb)
            {
                return ftb.ID;
            }
            return -1;
        }

        public static DungeonTable GetHellEntryTb(DungeonTable tb)
        {
            List<DungeonTable> hellEntrys = new List<DungeonTable>();

            var dungeonTb = TableManager.instance.GetTable<DungeonTable>();
            foreach (var item in dungeonTb)
            {
                DungeonTable curtb = item.Value as DungeonTable;

                if (curtb.SubType == DungeonTable.eSubType.S_HELL_ENTRY)
                {
                    hellEntrys.Add(curtb);
                }
            }

            foreach (var item in hellEntrys)
            {
                if (item.HellSplitLevel == tb.HellSplitLevel)
                {
                    return item;
                }
            }

            return null;
        }

        public static bool IsNormalType(DungeonTable dungeonTb)
        {
            if (null == dungeonTb)
            {
                return false;
            }

            switch (dungeonTb.SubType)
            {
                case DungeonTable.eSubType.S_NORMAL:
                    return true;
                case DungeonTable.eSubType.S_YUANGU:
                case DungeonTable.eSubType.S_NIUTOUGUAI:
                case DungeonTable.eSubType.S_NANBUXIGU:
                case DungeonTable.eSubType.S_SIWANGZHITA:
                case DungeonTable.eSubType.S_NEWBIEGUIDE:
                case DungeonTable.eSubType.S_PK:
                case DungeonTable.eSubType.S_JINBI:
                case DungeonTable.eSubType.S_HELL:
                case DungeonTable.eSubType.S_GUILDPK:
                case DungeonTable.eSubType.S_MONEYREWARDS_PVP:
                case DungeonTable.eSubType.S_WUDAOHUI:
                case DungeonTable.eSubType.S_HELL_ENTRY:
                case DungeonTable.eSubType.S_TEAM_BOSS:
                    return false;
            }

            return false;
        }
    }


    private DungeonNode _findDungeonNodes(int dungeonId, string name)
    {
        foreach (var dungeonNode in mCacheDungeonNodes)
        {
            if (dungeonNode.dungeonID == dungeonId &&
                dungeonNode.name == name)
            {
                return dungeonNode;
            }
        }

        DungeonNode node = new DungeonNode(dungeonId, name);
        mCacheDungeonNodes.Add(node);
        return node;
    }

    /// <summary>
    /// groupId -> List<itemId>
    /// </summary>
    private Dictionary<int, List<int>> mDictDropGroupID2ItemsList = null;
    private List<DungeonDropNode> mCacheDungeonDrops = new List<DungeonDropNode>();

    private class DungeonDropNode 
    {
        public DungeonDropNode(int singleDropID, bool isGroupID)
        {
            this.singleDropID = singleDropID;
            this.isGroupID = isGroupID;
        }

        public int singleDropID { private set; get; }
        public bool isGroupID { private set; get; }
        public List<int> items = new List<int>();// { get; }
    }

    private List<int> _findDropItemsByGroupId(int dropGroupId)
    {
        if (!mDictDropGroupID2ItemsList.ContainsKey(dropGroupId))
        {
            return null;
        }

        return mDictDropGroupID2ItemsList[dropGroupId];
    }

    private void _loadDungeonDropItemTable()
    {
        if (null != mDictDropGroupID2ItemsList)
        {
            return;
        }

        var dropTable = TableManager.instance.AddTableInEditorMode(typeof(DropItemTable));
        mCacheDungeonDrops.Clear();

        foreach (var iter in dropTable)
        {
            var dropData = iter.Value as DropItemTable;
            if (dropData.ItemProb <= 0)
            {
                continue;
            }

            DungeonDropNode node = _findDungeonDropNode(dropData.GroupID);
            if (null == node)
            {
                node = new DungeonDropNode(dropData.GroupID, dropData.DataType == 1);
                mCacheDungeonDrops.Add(node);
            }

            node.items.Add(dropData.ItemID);
        }

        var dropTable2 = TableManager.instance.AddTableInEditorMode(typeof(DropItemTable2));
        foreach (var iter in dropTable2)
        {
            var dropData = iter.Value as DropItemTable2;

            if (dropData.ItemProb <= 0)
            {
                continue;
            }

            DungeonDropNode node = _findDungeonDropNode(dropData.GroupID);
            if (null == node)
            {
                node = new DungeonDropNode(dropData.GroupID, dropData.DataType == 1);
                mCacheDungeonDrops.Add(node);
            }

            node.items.Add(dropData.ItemID);
        }


        mDictDropGroupID2ItemsList = new Dictionary<int, List<int>>();
        mCacheDungeonDrops.ForEach(item => {
            if (item.isGroupID)
            {
                List<int> cacheList = new List<int>();
                _dfsDropData(item, cacheList);
            }
        });
    }

    private void _dfsDropData(DungeonDropNode root, List<int> cacheList)
    {
        if (null == root)
        {
            return;
        }

        foreach (var item in root.items)
        {
            DungeonDropNode findNode = _findDungeonDropNode(item);
            if (null == findNode)
            {
                cacheList.Add(item);
            }
            else
            {
                if (findNode.isGroupID)
                {
                    if (mDictDropGroupID2ItemsList.ContainsKey(item))
                    {
                        cacheList.AddRange(mDictDropGroupID2ItemsList[item]);
                        //Logger.LogErrorFormat("dp {0}", item);
                    }
                    else
                    {
                        _dfsDropData(findNode, cacheList);
                    }
                }
                else
                {
                    cacheList.AddRange(findNode.items);
                }
            }
        }

        if (!mDictDropGroupID2ItemsList.ContainsKey(root.singleDropID))
        {
            mDictDropGroupID2ItemsList.Add(root.singleDropID, new List<int>());
        }

        mDictDropGroupID2ItemsList[root.singleDropID].AddRange(cacheList);
    }

    private DungeonDropNode _findDungeonDropNode(int id)
    {
        for (int i = 0; i < mCacheDungeonDrops.Count; i++)
        {
            if (mCacheDungeonDrops[i].singleDropID == id)
            {
                return mCacheDungeonDrops[i];
            }
        }

        return null;
    }


    #endregion

    #region 拍卖行
    private class AuctionNode : IItemSourceInfo
    {
        public AuctionNode()
        {
            mSourceInfo = new SourceInfo()
            {
                type = eItemSourceType.eAuction,
                name = "拍卖行",
                linkParm = "<type=framename value=GameClient.AuctionNewFrame>",
                openFunctionID = 18,
            };

        }

        private SourceInfo mSourceInfo;

        public SourceInfo sourceInfo
        {
            get
            {
                return mSourceInfo;
            }
        }
    }

    private AuctionNode mAuctionNode = new AuctionNode();
    public void MatchAuction(ItemTable data)
    {
        if (data.Owner == ItemTable.eOwner.NOTBIND || data.IsSeal)
        {
            AddSourceInfo(data.ID, mAuctionNode);
        }
    }
    #endregion

    #region 传奇之路

    private class LegendNode : IItemSourceInfo
    {
        public LegendNode(int itemId, MissionTable tb)
        {
            this.itemId = itemId;
            this.type = tb.SubType;

            int missionId = tb.ID;

            int mainId = -1;
            var legendtb = TableManager.instance.GetTable<LegendMainTable>();
            foreach (var item in legendtb)
            {
                var maintb = item.Value as LegendMainTable;
                foreach (var x in maintb.missionIds)
                {
                    if (x == missionId)
                    {
                        mainId = maintb.ID;
                        break;
                    }
                }

                if (mainId > 0)
                {
                    break;
                }
            }

            mSourceInfo = new SourceInfo()
            {
                type = eItemSourceType.eLegend,
                name = "传奇之路",
                linkParm = string.Format("<type=framename param={0} value=GameClient.LegendFrame>", mainId),
                openFunctionID = 60,
            };
        }

        public int itemId { private set; get; }
        public MissionTable.eSubType type { private set; get; }
        public string linkInfo { private set; get; }


        private SourceInfo mSourceInfo = null;

        public SourceInfo sourceInfo
        {
            get { return mSourceInfo; }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", itemId, type, linkInfo);
        }
    }

    private Dictionary<int, List<LegendNode>> mDictItemId2LegendNodes = null;

    public void MatchLegendNode(ItemTable itemData)
    {
        if (null == itemData)
        {
            return;
        }

        if (!mDictItemId2LegendNodes.ContainsKey(itemData.ID))
        {
            return;
        }

        foreach (var item in mDictItemId2LegendNodes[itemData.ID])
        {
            AddSourceInfo(itemData.ID, item);
            //Logger.LogError(item.ToString());
        }
    }

    private void _loadItemId2LegendNodesMap()
    {
        if (null != mDictItemId2LegendNodes)
        {
            return;
        }

        mDictItemId2LegendNodes = new Dictionary<int, List<LegendNode>>();

        var missionTable = TableManager.instance.GetTable<MissionTable>();

        int cnt = 0;
        foreach (var iter in missionTable)
        {
            var misstb = iter.Value as MissionTable;

            cnt++;

            EditorUtility.DisplayProgressBar("加载", string.Format("传奇之路 {0}", misstb.ID), cnt * 1.0f / missionTable.Count);

            if (null != misstb)
            {
                if (misstb.TaskType == MissionTable.eTaskType.TT_LEGEND)
                {
                    List<int> wardItems = _convertMissionAward2ItemList(
                        new string[] { misstb.OccuAward, misstb.Award, },
                        new int[]    { 1,                0 }
                    );

                    wardItems.ForEach(itemId =>
                    {
                        if (!mDictItemId2LegendNodes.ContainsKey(itemId))
                        {
                            mDictItemId2LegendNodes.Add(itemId, new List<LegendNode>());
                        }

                        mDictItemId2LegendNodes[itemId].Add(new LegendNode(itemId, misstb));
                    });
                }
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private List<int> _convertMissionAward2ItemList(string[] awardStrs, int[] index)
    {
        List<int> items = new List<int>();

        int idx = 0;
        foreach (var awardStr in awardStrs)
        {
            var origin = awardStr.Split(',');

            foreach (var item in origin)
            {
                var itemarsg = item.Split('_');
                int itemId = 0;

                if (itemarsg.Length > index[idx] && index[idx] >= 0)
                {
                    if (int.TryParse(itemarsg[index[idx]], out itemId))
                    {
                        items.Add(itemId);
                    }
                }
            }

            idx++;
        }

        return items;
    }


    #endregion

    #region 商店
    private class ShopNode : IItemSourceInfo
    {
        public ShopNode(ShopTable tb, int itemId)
        {
            this.type = tb.ShopKind;
            this.name = tb.ShopName;
            this.linkInfo = tb.Link;
            this.id = tb.ID;

            int openID = -1;

            ItemTable itemData = TableManager.instance.GetTableItem<ItemTable>(itemId);
            ShopTable.eSubType subtype = ShopTable.eSubType.ST_EQUIP;

            if (tb.ShopKind == ShopTable.eShopKind.SK_Fight)
            {
                openID = 7;
            }

            if (itemData.Type == ItemTable.eType.EQUIP)
            {
                if (GameClient.PlayerBaseData.IsWeapon(itemData.SubType))
                {
                    subtype = ShopTable.eSubType.ST_WEAPON;
                }
                else if (GameClient.PlayerBaseData.IsArmy(itemData.SubType))
                {
                    subtype = ShopTable.eSubType.ST_ARMOR;
                }
                else if (GameClient.PlayerBaseData.IsJewelry(itemData.SubType))
                {
                    subtype = ShopTable.eSubType.ST_JEWELRY;
                }
            }

            bool isEquipFound = false;
            foreach (var item in tb.SubType)
            {
                if (item == ShopTable.eSubType.ST_EQUIP)
                {
                    isEquipFound = true;
                    break;
                }
            }
            if (isEquipFound)
            {
                subtype = ShopTable.eSubType.ST_EQUIP;
            }

            string linkParm = string.Format("<type=framename param={0}|0|{1} value=GameClient.ShopFrame>", id, (int)subtype);

            if (id == 7)
            {
                linkParm = string.Format("<type=framename param={0}|-1|{1}|2 value=GameClient.ShopMainFrame>", id, (int)subtype);
            }

            mSourceInfo = new SourceInfo()
            {
                type = eItemSourceType.eShop,
                name = this.name,
                linkParm = linkParm,
                openFunctionID = openID,
            };
        }

        public int id { private set; get; }
        public ShopTable.eShopKind type { private set; get; }
        public string name { private set; get; } 
        public string linkInfo { private set; get; }

        private SourceInfo mSourceInfo;
        public SourceInfo sourceInfo
        {
            get { return mSourceInfo; }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", id, type, name);
        }
    }

    private Dictionary<int, List<ShopNode>> mDictItemId2ShopNodes = null;

    public void MatchShopNode(ItemTable data)
    {
        if (null == data)
        {
            return;
        }

        if (!mDictItemId2ShopNodes.ContainsKey(data.ID))
        {
            return;
        }

        foreach (var item in mDictItemId2ShopNodes[data.ID])
        {
            AddSourceInfo(data.ID, item);
            //Logger.LogError(data.Name + item);
        }
    }

    private int[] mShopNodesIds = new int[]
    {
        2, 5, 6, 7, 9, 13, 14, 15, 16,
    };

    private bool _isValidShopID(int shopId)
    {
        for (int i = 0; i < mShopNodesIds.Length; i++)
        {
            if (shopId == mShopNodesIds[i])
            {
                return true;
            }
        }

        return false;
    }
    
    private void _loadItemId2ShopNodesMaps()
    {
        if (null != mDictItemId2ShopNodes)
        {
            return;
        }
        mDictItemId2ShopNodes = new Dictionary<int, List<ShopNode>>();

        var mallTb = TableManager.instance.GetTable<ShopItemTable>();

        int cnt = 0;
        foreach (var item in mallTb)
        {
            var mitem = item.Value as ShopItemTable;

            cnt++;

            EditorUtility.DisplayProgressBar("加载", string.Format("商店物品表 {0}", mitem.ID), cnt * 1.0f / mallTb.Count);

            if (!_isValidShopID(mitem.ShopID))
            {
                continue;
            }

            var allItems = _convertShopItem2ItemIds(mitem);
            allItems.ForEach(itemId =>
            {
                if (!mDictItemId2ShopNodes.ContainsKey(itemId))
                {
                    mDictItemId2ShopNodes.Add(itemId, new List<ShopNode>());
                }

                var shopTb = TableManager.instance.GetTableItem<ShopTable>(mitem.ShopID);
                if (null != shopTb)
                {
                    mDictItemId2ShopNodes[itemId].Add(new ShopNode(shopTb, itemId));
                }
            });
        }

        EditorUtility.ClearProgressBar();
    }

    private List<int> _convertShopItem2ItemIds(ShopItemTable tb)
    {
        List<int> items = new List<int>();

        items.Add(tb.ItemID);

        return items;
    }
    #endregion

    #region 商城
    private class MallNode : IItemSourceInfo
    {
        public MallNode(MallTypeTable.eMallType type, string name)
        {
            this.type = type;
            this.name = name;

            mSourceInfo = new SourceInfo()
            {
                type = eItemSourceType.eMall,
                name = this.name,
                linkParm = string.Format("<type=framename param={0}|-1 value=GameClient.MallFrame>", (int)type),
                openFunctionID = 37,
            };
        }

        public MallTypeTable.eMallType type { private set; get; }
        public string name { private set; get; } 

        private SourceInfo mSourceInfo;
        public SourceInfo sourceInfo
        {
            get { return mSourceInfo; }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", type, name);
        }
    }

    private Dictionary<int, List<MallNode>> mDictItemId2MallNodes = null;

    public void MatchMallNode(ItemTable data)
    {
        if (null == data)
        {
            return;
        }

        if (!mDictItemId2MallNodes.ContainsKey(data.ID))
        {
            return;
        }

        foreach (var item in mDictItemId2MallNodes[data.ID])
        {
            AddSourceInfo(data.ID, item);
            //Logger.LogError(data.Name + item);
        }
    }
    
    private void _loadItemId2MallNodesMaps()
    {
        if (null != mDictItemId2MallNodes)
        {
            return;
        }
        mDictItemId2MallNodes = new Dictionary<int, List<MallNode>>();

        var mallTb = TableManager.instance.GetTable<MallItemTable>();

        int cnt = 0;

        foreach (var item in mallTb)
        {
            var mitem = item.Value as MallItemTable;

            cnt++;

            EditorUtility.DisplayProgressBar("加载", string.Format("商城物品表 {0}", mitem.ID), cnt * 1.0f / mallTb.Count);

            var allItems = _convertMallItem2ItemIds(mitem);
            allItems.ForEach(itemId =>
            {
                if (!mDictItemId2MallNodes.ContainsKey(mitem.itemid))
                {
                    mDictItemId2MallNodes.Add(mitem.itemid, new List<MallNode>());
                }

                var mallTypetb = TableManager.instance.GetTableItem<MallTypeTable>(mitem.type);
                if (null != mallTypetb)
                {
                    mDictItemId2MallNodes[mitem.itemid].Add(new MallNode((MallTypeTable.eMallType)mitem.type, mallTypetb.MainTypeName));
                }
            });
        }
    }

    private List<int> _convertMallItem2ItemIds(MallItemTable tb)
    {
        List<int> items = new List<int>();

        if (tb.itemid > 0)
        {
            items.Add(tb.itemid);
        }

        if (!string.IsNullOrEmpty(tb.giftpackitems))
        {
            var allitems = tb.giftpackitems.Split('|');

            foreach (var item in allitems)
            {
                var to = item.Split(':');
                int itemid = 0;
                if (int.TryParse(to[0], out itemid))
                {
                    items.Add(itemid);
                }
            }
        }

        return items;
    }
    #endregion

    #region 团本翻牌奖励

    private Dictionary<int, List<TeamDuplicateNode>> mDictItemIdTeamDuplicateNode = null;
    private class TeamDuplicateNode : IItemSourceInfo
    {
        public TeamDuplicateNode()
        {
            mSourceInfo = new SourceInfo()
            {
                type = eItemSourceType.eTeamDuplicate,
                name = "团队副本",
                linkParm = string.Format("<type=framename param=5 value=GameClient.ChallengeMapFrame>"),
                openFunctionID = 91,
            };
        }

        private SourceInfo mSourceInfo;
        public SourceInfo sourceInfo
        {
            get { return mSourceInfo; }
        }
    }

    private void _loadItemIdTeamDuplicateNodesMaps()
    {
        if (mDictItemIdTeamDuplicateNode != null)
        {
            return;
        }

        mDictItemIdTeamDuplicateNode = new Dictionary<int, List<TeamDuplicateNode>>();

        var teamCopyFlopTb = TableManager.instance.GetTable<TeamCopyFlopTable>();

        int cnt = 0;

        foreach (var table in teamCopyFlopTb)
        {
            var mitem = table.Value as TeamCopyFlopTable;

            cnt++;

            EditorUtility.DisplayProgressBar("加载", string.Format("团本翻牌表 {0}", mitem.ID), cnt * 1.0f / teamCopyFlopTb.Count);

            List<int> allItems = _findDropItemsByGroupId(mitem.DropId);

            if (null == allItems)
            {
                continue; ;
            }

            foreach (var item in allItems)
            {
                if (!mDictItemIdTeamDuplicateNode.ContainsKey(item))
                {
                    mDictItemIdTeamDuplicateNode.Add(item, new List<TeamDuplicateNode>());
                }

                mDictItemIdTeamDuplicateNode[item].Add(new TeamDuplicateNode());
            }
        }
    }

    public void MatchTeamDuplicateNode(ItemTable data)
    {
        if (null == data)
        {
            return;
        }

        if (!mDictItemIdTeamDuplicateNode.ContainsKey(data.ID))
        {
            return;
        }

        foreach (var item in mDictItemIdTeamDuplicateNode[data.ID])
        {
            AddSourceInfo(data.ID, item);
        }
    }

    #endregion
}
