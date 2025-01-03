using UnityEngine;
using System.Collections;
using ProtoTable;
using System.Collections.Generic;
///////删除linq

#if UNITY_EDITOR
    using UnityEditor;
#endif 

public class ItemSearchEngine : Singleton<ItemSearchEngine> {

    const string ItemSearchPath = "Data/CommonData/ItemSearchEngine";

#if UNITY_EDITOR
   
    [MenuItem("[TM工具集]/[道具]/GenItemSerachCache")]
    public static void GenItemSearchTable()
    {
        ItemSearchCache asset = ScriptableObject.CreateInstance<ItemSearchCache>();
       
        Dictionary<char, List<ItemTable>>       tempIndexDic = new Dictionary<char, List<ItemTable>>();
        Dictionary<int, object> itemDatas = TableManager.instance.GetTable<ItemTable>();
        var enumerator = itemDatas.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = enumerator.Current.Value as ItemTable;
            if(item == null)
            {
                continue;
            }

            char[] nameArray = item.Name.ToCharArray();
            for(int i = 0; i < nameArray.Length; i++)
            {
                char nameChar = nameArray[i];
                List<ItemTable> itemList;
                if (!tempIndexDic.TryGetValue(nameArray[i], out itemList))
                {
                    itemList = new List<ItemTable>();
                    tempIndexDic.Add(nameChar, itemList);
                }

                itemList.Add(item);
                itemList.Sort((a,b) => {
                    if(a.ID == b.ID)
                    {
                        return 0;
                    }
                    else if(a.ID > b.ID)
                    {
                        return 1;
                    }
                    return -1;
                });
            }
        }

        asset.itemCaches = new List<ItemSearchItem>();
        
        foreach(var item in tempIndexDic)
        {
            ItemSearchItem current = new ItemSearchItem
            {
                key = item.Key,
                tableList = new List<int>()
            };

            foreach(var tab in item.Value)
            {
                current.tableList.Add(tab.ID);
            }

            asset.itemCaches.Add(current);
        }

        asset.itemCaches.Sort((a, b) =>
        {
            if (a.key == b.key)
            {
                return 0;
            }
            else if (a.key > b.key)
            {
                return 1;
            }
            return -1;
        });
        AssetDatabase.CreateAsset(asset, "Assets/Resources/" + ItemSearchPath + ".asset");
    }

#endif

    public sealed override void Init()
    {
        itemSearchCache = AssetLoader.instance.LoadRes(ItemSearchPath, typeof(ItemSearchCache),false).obj as ItemSearchCache;
    }

    public class ItemSearchComparser : IComparer<ItemSearchItem>
    {
        public int Compare(ItemSearchItem a, ItemSearchItem b)
        {
            if (a.key == b.key)
            {
                return 0;
            }
            else if (a.key > b.key)
            {
                return 1;
            }
            else
                return -1;
        }
    }


    public List<ItemTable> FindItemListByName(string name)
    {
        // 找出包含每个字的物品列表，再对他们求交集，就是想要查找的所有物品
        List<int> itemList = new List<int>();
        char[] nameArray = name.ToCharArray();
        ItemSearchComparser op = new ItemSearchComparser();
        var ItemCaches = itemSearchCache.itemCaches;

        for (int i = 0; i < nameArray.Length; i++)
        {
            char nameChar = nameArray[i];
            List<int> tmpItemList;
            ItemSearchItem item = new ItemSearchItem
            {
                key = nameChar
            };
            int Index = ItemCaches.BinarySearch(item,op);

            if(Index >= 0 && Index < ItemCaches.Count)
            {
                tmpItemList = ItemCaches[Index].tableList;
                if(itemList.Count == 0)
                {
                    itemList = tmpItemList.ToList<int>();
                    continue;
                }

                _CountIntersection(ref itemList, tmpItemList);
            }
        }

        //构造对外ItemList
        List<ItemTable> itemTableList = new List<ItemTable>();
        Dictionary<int, object> itemDatas = TableManager.instance.GetTable<ItemTable>();

        for(int i = 0; i < itemList.Count; ++i)
        {
            var id = itemList[i];
            object tab = null;
            if( itemDatas.TryGetValue(id,out tab) )
            {
                ItemTable current = tab as ItemTable;

                if(current != null)
                {
                    itemTableList.Add(current);    
                }
            }
        }

        return itemTableList;
    }

    private void _CountIntersection(ref List<int> a, List<int> b)
    {
        if(a == null || b == null)
        {
            return;
        }

        a = a.Intersect(b).ToList<int>();
    }

    // 根据物品的每个字建的索引
    ItemSearchCache                         itemSearchCache;
}
