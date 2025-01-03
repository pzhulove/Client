using UnityEngine;
using System.Collections;
using ProtoTable;
using System.Collections.Generic;
///////删除linq

#if UNITY_EDITOR
using UnityEditor;
#endif 

[System.Serializable]
public struct ItemSearchItem
{
    public char key;
    public List<int> tableList;
}

public class ItemSearchCache : ScriptableObject
{
    public List<ItemSearchItem> itemCaches;
}
