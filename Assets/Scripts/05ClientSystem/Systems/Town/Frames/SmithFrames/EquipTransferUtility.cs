using UnityEngine;
using System.Collections.Generic;
using System;
///////删除linq
using ProtoTable;

namespace GameClient
{
    public class EquipTransferUtility
    {

        class MapNode : IComparable<MapNode>
        {
            public MapNode(int id, int count, int[] levels, int[] subTypes)
            {
                itemData = ItemDataManager.CreateItemDataFromTable(id);

                if (null == itemData)
                {
                    Logger.LogErrorFormat("itemId {0} is nil", id);
                }

                itemData.Count = count;

                if (levels.Length > 0)
                {
                    minLevel = levels[0];
                    maxLevel = levels[0];
                }

                for (int i = 0; i < levels.Length; ++i)
                {
                    minLevel = Mathf.Min(levels[i], minLevel);
                    maxLevel = Mathf.Max(levels[i], maxLevel);
                }

                this.subTypes = subTypes;
                if (null == this.subTypes)
                {
                    this.subTypes = new int[0];
                }
            }

            private int minLevel { get; set; }
            private int maxLevel { get; set; }
            private int[] subTypes { get; set; }

            public ItemData itemData { get; private set; }

            public int CompareTo(MapNode other)
            {
                if (minLevel != other.minLevel)
                {
                    return minLevel - other.minLevel;
                }

                if (maxLevel != other.maxLevel)
                {
                    return maxLevel - other.maxLevel;
                }

                return subTypes.Length - other.subTypes.Length;
            }

            public bool IsFitLevel(int level)
            {
                if (minLevel > level)
                {
                    return false;
                }

                if (maxLevel < level)
                {
                    return false;
                }

                return true;
            }

            public bool IsFitSubType(int subType)
            {
                if (-1 == subType)
                {
                    return true;
                }

                for (int i = 0; i < subTypes.Length; ++i)
                {
                    if (subTypes[i] == subType)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static List<ItemData> GetTransferStones(ItemData data)
        {
            List<ItemData> stones = new List<ItemData>();

            if (null != data)
            {
                for (int i = 0; i < mapNodes.Count; ++i)
                {
                    if (mapNodes[i].IsFitLevel(data.LevelLimit)
                     && mapNodes[i].IsFitSubType(data.SubType))
                    {
                        int cnt = ItemDataManager.GetInstance().GetOwnedItemCount(mapNodes[i].itemData.TableID);
                        if (cnt > 0)
                        {
                            stones.Add(mapNodes[i].itemData);
                        }
                    }
                }
            }

            return stones;
        }

        private static ItemData GetTransferStoneId(ItemData data)
        {
            if (null == data)
            {
                return GetTransferStoneId();
            }

            return GetTransferStoneId(data.LevelLimit, data.SubType);
        }

        public static ItemData GetTransferStoneId(int levelLimit = 1, int subType = -1)
        {
            for (int i = 0; i < mapNodes.Count; ++i)
            {
                if (mapNodes[i].IsFitLevel(levelLimit)
                 && mapNodes[i].IsFitSubType(subType))
                {
                    return mapNodes[i].itemData;
                }
            }

            return null;
        }

        public static bool IsTransferStone(int itemId)
        {
            for (int i = 0; i < mapNodes.Count; ++i)
            {
                if (mapNodes[i].itemData.TableID == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMatch(ItemData item, ItemData stoneItem)
        {
            if (null == stoneItem)
            {
                return false;
            }

            List<ItemData> stones = GetTransferStones(item);

            for (int i = 0; i < stones.Count; ++i)
            {
                if (stones[i].TableID == stoneItem.TableID)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<MapNode> mMapNodes = null;
        private static List<MapNode> mapNodes
        {
            get
            {
                if (null == mMapNodes)
                {
                    mMapNodes = new List<MapNode>();

                    var mapData = TableManager.instance.GetTable<EquipTransMapTable>();
                    var mapDataIter = mapData.GetEnumerator();

                    while (mapDataIter.MoveNext())
                    {
                        EquipTransMapTable tb = mapDataIter.Current.Value as EquipTransMapTable;
                        mMapNodes.Add(new MapNode(tb.ItemId, 1, tb.Level.ToArray(), tb.SubTypes.ToArray()));
                    }

                    mMapNodes.Sort();
                }

                return mMapNodes;
            }

        }
    }
}
