using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
///////删除linq

enum TittleComeType
{
    TCT_INVALID = -1,
    [System.ComponentModel.Description("商城")]
    TCT_SHOP,
    [System.ComponentModel.Description("任务")]
    TCT_MISSION,
    [System.ComponentModel.Description("活动")]
    TCT_ACTIVE,
    [System.ComponentModel.Description("时限")]
    TCT_TIMELIMITED,
    [System.ComponentModel.Description("可交易")]
    TCT_TRADE,
    [System.ComponentModel.Description("称号合成")]
    TCT_MERGE,
    TCT_COUNT,
}

class TitleMergeMaterialData
{
    public int id;
    public int count;
    static string ms_enough_desc = "<color=#00ff00>{0}</color>/{1}";
    static string ms_not_enough_desc = "<color=#ff0000>{0}</color>/{1}";

    public string getColorName()
    {
        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
        if(null != item)
        {
            var quality_info = ItemData.GetQualityInfo(item.Color, item.Color2 == 1);
            if(null != quality_info)
            {
                return string.Format("<color={0}>{1}</color>", quality_info.ColStr, item.Name);
            }
        }
        return string.Empty;
    }

    public string getDescString()
    {
        int iCount = ItemDataManager.GetInstance().GetOwnedItemCount(id);
        bool bEnough = iCount >= count;
        if(bEnough)
        {
            return string.Format(ms_enough_desc, iCount, count);
        }
        return string.Format(ms_not_enough_desc, iCount, count);
    }

    public bool hasOwned
    {
        get
        {
            return true;// TittleBookManager.GetInstance().HasTittle();
        }
    }
}

class TitleMergeData
{
    public ProtoTable.EquipForgeTable forgeItem;
    public ProtoTable.ItemTable item;
    public List<TitleMergeMaterialData> materials = new List<TitleMergeMaterialData>();
    public List<TitleMergeMaterialData> moneys = new List<TitleMergeMaterialData>();

    ProtoTable.ItemTable moneyItem;
    int moneyCount;

    public int getMoneyId()
    {
        if(null == moneyItem)
        {
            getMoneyIcon();
        }
        if(null != moneyItem)
        {
            return moneyItem.ID;
        }
        return 0;
    }

    public string getMoneyIcon()
    {
        if(null != forgeItem)
        {
            if (null == moneyItem)
            {
                if (forgeItem.Price.Count > 0)
                {
                    var tokens = forgeItem.Price[0].Split('_');
                    if (null != tokens && tokens.Length == 2)
                    {
                        int iId = 0;
                        int cnt = 0;
                        if (int.TryParse(tokens[0], out iId) && int.TryParse(tokens[1], out cnt))
                        {
                            moneyItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iId);
                            moneyCount = cnt;
                        }
                    }
                }
            }

            if (null != moneyItem)
            {
                return moneyItem.Icon;
            }
        }
        return string.Empty;
    }

    public int getMoneyCount()
    {
        if(null == moneyItem)
        {
            getMoneyIcon();
        }

        return moneyCount;
    }

    public int getOwnedMoneyCount()
    {
        return ItemDataManager.GetInstance().GetOwnedItemCount(getMoneyId());
    }
}

class TittleBookManager : DataManager<TittleBookManager>
{
    public delegate void OnAddTittle(ulong uid);
    public delegate void OnRemoveTittle(ulong uid);
    public delegate void OnUpdateTittle(ulong uid);

    public OnAddTittle onAddTittle;
    public OnRemoveTittle onRemoveTittle;
    public OnUpdateTittle onUpdateTittle;

    public delegate void OnRemoveTableTittle(ulong tableid);
    public delegate void OnAddTableTittle(ulong tableid);

    public OnRemoveTableTittle onRemoveTableTittle;
    public OnAddTableTittle onAddTableTittle;

    protected void BindDelegate()
    {
        ItemDataManager.GetInstance().onAddNewItem += _OnAddTittle;
        ItemDataManager.GetInstance().onRemoveItem += _OnRemoveTittle;
        ItemDataManager.GetInstance().onUpdateItem += _OnUpdateTittle;
    }

    protected void UnBindDelegate()
    {
        ItemDataManager.GetInstance().onAddNewItem -= _OnAddTittle;
        ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveTittle;
        ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateTittle;
    }

    public override void Initialize()
    {
        LoadMergeTitleFromTable();
        LoadTittleFromTable();
        UnBindDelegate();
        BindDelegate();
    }

    public override EEnterGameOrder GetOrder()
    {
        return EEnterGameOrder.TittleBookManager;
    } 

    public bool CanAsMergeMaterial(ItemData itemData)
    {
        if(null != itemData)
        {
            var curData = ItemDataManager.GetInstance().GetItem(itemData.GUID);
            if(null != curData)
            {
                var find = MergeTitles.Find(x =>
                {
                    return null != x && null != x.materials.Find(t => { return t.id == itemData.TableID; });
                });
                return null != find;
            }
        }
        return false;
    }

    public bool IsTitleMergeLevelFit()
    {
        return Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Title);
    }

    public void OnGotoMerge(ItemData itemData, object data)
    {
        ClientSystemManager.GetInstance().OpenFrame<TitleBookFrame>(FrameLayer.Middle,new TitleBookFrameData { eTittleComeType = TittleComeType.TCT_MERGE});
        ItemTipManager.GetInstance().CloseAll();
    }


    public bool CanTrade(ItemData itemData)
    {
        var targetType = GetTittleType(itemData.GUID);
        if(targetType == TittleComeType.TCT_TRADE)
        {
            return true;
        }
        return false;
    }

    public bool HasBindedTitle(int iTableID)
    {
        for (int i = 0; i < m_akTittleList.Count; ++i)
        {
            var itemData = ItemDataManager.GetInstance().GetItem(m_akTittleList[i]);
            if(itemData != null && itemData.TableID == iTableID)
            {
                var targetType = GetTittleType(itemData);
                if(targetType != TittleComeType.TCT_TRADE)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool HasExtraTitle(ItemData compare)
    {
        if(!HasBindedTitle(compare.TableID))
        {
            return false;
        }

        var type = GetTittleType(compare);
        if(type != TittleComeType.TCT_TRADE)
        {
            return false;
        }

        return true;
    }

    bool _IsBelongToTitleBook(TittleComeType eCurrent)
    {
        if (eCurrent > TittleComeType.TCT_INVALID && eCurrent < TittleComeType.TCT_COUNT)
        {
            if(eCurrent == TittleComeType.TCT_TRADE||eCurrent==TittleComeType.TCT_TIMELIMITED)
            {
                return false;
            }
           
            return true;
        }
        return false;
    }

    void _TryRemoveTableTitle(Protocol.Item current)
    {
        if(current == null)
        {
            return;
        }

        var itemData = ItemDataManager.GetInstance().GetItem(current.uid);
        TittleComeType eCurrent = GetTittleType(current.uid);
        if (itemData != null && _IsBelongToTitleBook(eCurrent))
        {
            var enumerator = m_akTableTittleDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.Remove((ulong)itemData.TableID))
                {
                    if (onRemoveTableTittle != null)
                    {
                        onRemoveTableTittle((ulong)itemData.TableID);
                    }
                    break;
                }
            }
        }
    }

    void _OnAddTittle(List<Protocol.Item> tittle)
    {
        for (int i = 0; i < tittle.Count; ++i)
        {
            if(tittle[i] != null)
            {
                TittleComeType eCurrent = GetTittleType(tittle[i].uid);
                TittleComeType eTableType = GetTittleTableType(tittle[i].uid);
                if (eCurrent > TittleComeType.TCT_INVALID && eCurrent < TittleComeType.TCT_COUNT)
                {
                    m_akTittleList.Add(tittle[i].uid);
                    List<ulong> outValue = null;
                    if(!m_akType2Tittle.TryGetValue(eCurrent,out outValue))
                    {
                        outValue = new List<ulong>();
                        m_akType2Tittle.Add(eCurrent, outValue);
                    }
                    outValue.Add(tittle[i].uid);

                    //AddNewTittleMark(tittle[i].uid);

                    _TryRemoveTableTitle(tittle[i]);

                    if (onAddTittle != null)
                    {
                        onAddTittle(tittle[i].uid);
                    }
                }
            }
        }
    }

    void _OnRemoveTittle(ItemData data)
    {
        //RemoveNewTittleMark(data);

        m_akTittleList.Remove(data.GUID);

        bool bErase = false;
        var enumerator = m_akType2Tittle.GetEnumerator();
        while(!bErase && enumerator.MoveNext())
        {
            var values = enumerator.Current.Value;
            for(int i = 0; i < values.Count; ++i)
            {
                if(values[i] == data.GUID)
                {
                    values.RemoveAt(i);
                    bErase = true;
                    break;
                }
            }
        }

        if (onRemoveTittle != null)
        {
            onRemoveTittle(data.GUID);
        }

        bool bNeedAdd = true;
        var titleList0 = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.WearEquip, ProtoTable.ItemTable.eSubType.TITLE);
        var titleLists = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Title, ProtoTable.ItemTable.eSubType.TITLE);
        titleLists.AddRange(titleList0);
        for(int i = 0; i < titleLists.Count; ++i)
        {
            var itemData = ItemDataManager.GetInstance().GetItem(titleLists[i]);
            if(itemData != null && itemData.TableID == data.TableID)
            {
                bNeedAdd = false;
                break;
            }
        }
        if(!bNeedAdd)
        {
            return;
        }

        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)data.TableID);
        if(item != null)
        {
            var eType = GetTittleType(data);
            if(_IsBelongToTitleBook(eType))
            {
                List<ulong> outValue = null;
                if (!m_akTableTittleDic.TryGetValue(eType, out outValue))
                {
                    outValue = new List<ulong>();
                    m_akTableTittleDic.Add(eType, outValue);
                }

                if (!outValue.Contains((ulong)data.TableID))
                {
                    if (!outValue.Contains((ulong)data.TableID))
                    {
                        outValue.Add((ulong)data.TableID);
                    }

                    if (onAddTableTittle != null)
                    {
                        onAddTableTittle((ulong)data.TableID);
                    }
                }
            }
        }
    }

    public TittleComeType GetTittleTableType(ulong uid)
    {
        TittleComeType eTittleComeType = TittleComeType.TCT_INVALID;

        var itemData = ItemDataManager.GetInstance().GetItem(uid);
        if(itemData != null)
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
            if(itemTable != null)
            return GetTittleType(itemTable);
        }

        return eTittleComeType;
    }

    public TittleComeType GetTittleType(ItemData itemData)
    {
        TittleComeType eTittleComeType = TittleComeType.TCT_INVALID;
        if (itemData != null && itemData.Type == ProtoTable.ItemTable.eType.FUCKTITTLE)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
            if (item != null)
            {
                if (string.IsNullOrEmpty(itemData.GetTimeLeftDesc()) == false)
                {
                    eTittleComeType = TittleComeType.TCT_TIMELIMITED;
                }
                else if (itemData.CanTrade())
                {
                    eTittleComeType = TittleComeType.TCT_TRADE;
                }
                else
                {
                    if (item.ComeType == ProtoTable.ItemTable.eComeType.CT_ACTIVITY)
                    {
                        eTittleComeType = TittleComeType.TCT_ACTIVE;
                    }
                    else if (item.ComeType == ProtoTable.ItemTable.eComeType.CT_MISSION)
                    {
                        eTittleComeType = TittleComeType.TCT_MISSION;
                    }
                    else if (item.ComeType == ProtoTable.ItemTable.eComeType.CT_SHOP)
                    {
                        eTittleComeType = TittleComeType.TCT_SHOP;
                    }
                }
            }
        }
        return eTittleComeType;
    }

    public TittleComeType GetTittleType(ulong uid)
    {
        var itemData = ItemDataManager.GetInstance().GetItem(uid);
        return GetTittleType(itemData);
    }

    public TittleComeType GetTittleType(ProtoTable.ItemTable item)
    {
        TittleComeType eTittleComeType = TittleComeType.TCT_INVALID;

        if(item.TimeLeft != 0)
        {
            //eTittleComeType = TittleComeType.TCT_TIMELIMITED;
        }
        else
        {
            if (item.ComeType == ProtoTable.ItemTable.eComeType.CT_MISSION)
            {
                eTittleComeType = TittleComeType.TCT_MISSION;
            }
            else if (item.ComeType == ProtoTable.ItemTable.eComeType.CT_SHOP)
            {
                eTittleComeType = TittleComeType.TCT_SHOP;
            }
        }

        return eTittleComeType;
    }

    void _OnUpdateTittle(List<Protocol.Item> tittle)
    {
        for(int i = 0; i < tittle.Count; ++i)
        {
            var current = tittle[i];
            var itemData = ItemDataManager.GetInstance().GetItem(current.uid);
            if(itemData == null)
            {
                continue;
            }

            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
            if(item == null || item.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
            {
                continue;
            }

            var enumerator = m_akType2Tittle.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var values = enumerator.Current.Value;
                if (values.Contains(current.uid))
                {
                    values.Remove(current.uid);
                    //if(onRemoveTittle != null)
                    //{
                    //    onRemoveTittle.Invoke(current.uid);
                    //}
                }
            }

            TittleComeType eCurrent = GetTittleType(current.uid);
            if (eCurrent > TittleComeType.TCT_INVALID && eCurrent < TittleComeType.TCT_COUNT)
            {
                List<ulong> outValue = null;
                if (!m_akType2Tittle.TryGetValue(eCurrent,out outValue))
                {
                    outValue = new List<ulong>();
                    m_akType2Tittle.Add(eCurrent, outValue);
                }
                outValue.Add(current.uid);
                //if(onAddTittle != null)
                //{
                //    onAddTittle.Invoke(current.uid);
                //}
            }

            _TryRemoveTableTitle(current);

            if (onUpdateTittle != null)
            {
                onUpdateTittle(current.uid);
            }
        }
    }

    List<ulong> m_akTittleList = new List<ulong>();
    public List<ulong> TittleList
    {
        get { return m_akTittleList; }
        set { TittleList = value; }
    }

    Dictionary<TittleComeType, List<ulong>> m_akType2Tittle = new Dictionary<TittleComeType, List<ulong>>();
    public bool GetTittle(TittleComeType eTittleComeType,out List<ulong> tittles)
    {
        return m_akType2Tittle.TryGetValue(eTittleComeType,out tittles);
    }

    public bool IsTittleTabMark(TittleComeType eCurrent)
    {
        if(m_akType2Tittle.ContainsKey(eCurrent))
        {
            var titles = m_akType2Tittle[eCurrent];
            for(int i = 0; i < titles.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(titles[i]);
                if(itemData != null && itemData.IsNew)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsTittleMark(ulong guid)
    {
        var itemData = ItemDataManager.GetInstance().GetItem(guid);
        if(itemData != null)
        {
            return itemData.IsNew == true;
        }
        return false;
    }

    public bool HasTittleTabMark()
    {
        for(int i = 0; i < (int)TittleComeType.TCT_COUNT;++i)
        {
            if(IsTittleTabMark((TittleComeType)i))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasTitle(int tableId)
    {
        var enumerator = m_akType2Tittle.GetEnumerator();
        while(enumerator.MoveNext())
        {
            var lists = enumerator.Current.Value;
            if(null != lists)
            {
                for(int i = 0; i < lists.Count; ++i)
                {
                    var item = ItemDataManager.GetInstance().GetItem(lists[i]);
                    if(null != item && item.TableID == tableId)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool HasTittle(TittleComeType eTittleComeType)
    {
        return m_akType2Tittle.ContainsKey(eTittleComeType) && m_akType2Tittle[eTittleComeType].Count > 0;
    }

    public override void Clear()
    {
        m_akTittleList.Clear();
        m_akTableTittleDic.Clear();
        m_akType2Tittle.Clear();
    }

    Dictionary<TittleComeType, List<ulong>> m_akTableTittleDic = new Dictionary<TittleComeType, List<ulong>>();
    public bool GetTableTittle(TittleComeType eTittleComeType, out List<ulong> tittles)
    {
        //Logger.LogErrorFormat("<color=#00ff00>title[{0}]</color>", eTittleComeType);
        return m_akTableTittleDic.TryGetValue(eTittleComeType, out tittles);
    }

    List<TitleMergeData> m_mergeTitles = new List<TitleMergeData>();
    void LoadMergeTitleFromTable()
    {
        if(m_mergeTitles.Count == 0)
        {
            var mergeTitles = TableManager.GetInstance().GetTable<ProtoTable.EquipForgeTable>();
            if (null != mergeTitles)
            {
                var enumerator = mergeTitles.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ProtoTable.EquipForgeTable forgeItem = enumerator.Current.Value as ProtoTable.EquipForgeTable;
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(forgeItem.ID);
                    if (null != item && item.SubType == ProtoTable.ItemTable.eSubType.TITLE)
                    {
                        var mergeData = new TitleMergeData();
                        mergeData.forgeItem = forgeItem;
                        mergeData.item = item;
                        mergeData.materials.Clear();

                        for(int i = 0; i < forgeItem.Material.Count; ++i)
                        {
                            if(!string.IsNullOrEmpty(forgeItem.Material[i]))
                            {
                                var tokens = forgeItem.Material[i].Split('_');
                                int id = 0;
                                int count = 0;
                                if(tokens.Length == 2 && int.TryParse(tokens[0],out id) && int.TryParse(tokens[1],out count))
                                {
                                    if(count > 0)
                                    {
                                        var mItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
                                        if(null != mItem)
                                        {
                                            mergeData.materials.Add(new TitleMergeMaterialData
                                            {
                                                 id = id,
                                                 count = count
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        if(mergeData.materials.Count > 0)
                        {
                            m_mergeTitles.Add(mergeData);
                        }
                    }
                }
            }
        }
    }

    public List<TitleMergeData> MergeTitles
    {
        get
        {
            if(m_mergeTitles.Count <= 0)
            {
                LoadMergeTitleFromTable();
            }
            return m_mergeTitles;
        }
    }

    void LoadTittleFromTable()
    {
        if(m_akTableTittleDic.Count == 0)
        {
            var itemTable = TableManager.GetInstance().GetTable<ProtoTable.ItemTable>();
            var enumerator = itemTable.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current.Value as ProtoTable.ItemTable;
                if(item.Type == ProtoTable.ItemTable.eType.FUCKTITTLE)
                {
                    var eCurrent = GetTittleType(item);
                    if(eCurrent > TittleComeType.TCT_INVALID && eCurrent < TittleComeType.TCT_COUNT)
                    {
                        List<ulong> outValue = null;
                        if (!m_akTableTittleDic.TryGetValue(eCurrent, out outValue))
                        {
                            outValue = new List<ulong>();
                            m_akTableTittleDic.Add(eCurrent, outValue);
                        }
                        outValue.Add((ulong)enumerator.Current.Key);
                    }
                }
            }

            var diecEnumerator = m_akTableTittleDic.GetEnumerator();
            while(diecEnumerator.MoveNext())
            {
                diecEnumerator.Current.Value.Sort();
            }
        }
    }
}