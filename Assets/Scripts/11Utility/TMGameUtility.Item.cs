
using ProtoTable;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public static partial class GameUtility
    {
        public static class Item
        {
            public static string GetItemQualityBg(ItemTable.eColor qualityEnum)
            {
                var bgData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.ITEM_QUALITY_BG_PATH);
                int quality = qualityEnum - ItemTable.eColor.WHITE;
                if (bgData == null || quality < 0 || quality >= bgData.StrParamsLength)
                {
                    return string.Empty;
                }

                var rootPath = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.ITEM_QUALITY_BG_ROOT_PATH);
                string root = "UI/Image/Packed/p_UI_Common01.png:";
                if (rootPath != null && rootPath.StrParamsLength > 0)
                {
                    root = rootPath.StrParams[0];
                }

                return root + bgData.StrParamsArray(quality);
            }

            private static Color sWhiteColor = Color.black;
            private static Color sBlueColor = Color.black;
            private static Color sPurpleColor = Color.black;
            private static Color sGreenColor = Color.black;
            private static Color sPinkColor = Color.black;
            private static Color sYellowColor = Color.black;
            public static Color GetItemColor(ItemTable.eColor qualityEnum)
            {
                switch (qualityEnum)
                {
                    case ItemTable.eColor.CL_NONE:
                        return Color.white;
                    case ItemTable.eColor.WHITE:
                        if (sWhiteColor != Color.black)
                        {
                            return sWhiteColor;
                        }
                        break;
                    case ItemTable.eColor.BLUE:
                        if (sBlueColor != Color.black)
                        {
                            return sBlueColor;
                        }
                        break;
                    case ItemTable.eColor.PURPLE:
                        if (sPurpleColor != Color.black)
                        {
                            return sPurpleColor;
                        }
                        break;
                    case ItemTable.eColor.GREEN:
                        if (sGreenColor != Color.black)
                        {
                            return sGreenColor;
                        }
                        break;
                    case ItemTable.eColor.PINK:
                        if (sPinkColor != Color.black)
                        {
                            return sPinkColor;
                        }
                        break;
                    case ItemTable.eColor.YELLOW:
                        if (sYellowColor != Color.black)
                        {
                            return sYellowColor;
                        }
                        break;
                }

                var data = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.ITEM_QUALITY_FONT_COLOR);
                int quality = qualityEnum - ItemTable.eColor.WHITE;
                if (data == null || quality < 0 || quality >= data.StrParamsLength)
                {
                    return Color.white;
                }

                var str = data.StrParamsArray(quality);
                var chars = str.ToCharArray();
                int r = 255;
                int g = 255;
                int b = 255;
                int a = 255;
                if (chars.Length >= 9 && chars[0] == '#')
                {
                    r = Convert.ToInt32(chars[1].ToString() + chars[2].ToString(), 16);
                    g = Convert.ToInt32(chars[3].ToString() + chars[4].ToString(), 16);
                    b = Convert.ToInt32(chars[5].ToString() + chars[6].ToString(), 16);
                    a = Convert.ToInt32(chars[7].ToString() + chars[8].ToString(), 16);
                }
                var color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                switch (qualityEnum)
                {
                    case ItemTable.eColor.WHITE:
                        sWhiteColor = color;
                        break;
                    case ItemTable.eColor.BLUE:
                        sBlueColor = color;
                        break;
                    case ItemTable.eColor.PURPLE:
                        sPurpleColor = color;
                        break;
                    case ItemTable.eColor.GREEN:
                        sGreenColor = color;
                        break;
                    case ItemTable.eColor.PINK:
                        sPinkColor = color;
                        break;
                    case ItemTable.eColor.YELLOW:
                        sYellowColor = color;
                        break;
                }

                return color;
            }

            private static Dictionary<int/*AttributeType*/, string> mAttributeStringMap = new Dictionary<int, string>();
            //获取枚举值对应的string类型
            public static string GetAttributeEnumString(AttributeType att)
            {
                int at = (int)att;
                if (mAttributeStringMap.ContainsKey(at))
                    return mAttributeStringMap[at];

                string str = Enum.GetName(typeof(AttributeType), att);
                mAttributeStringMap[at] = str;

                return str;
            }

            public static EFashionWearNewSlotType GetFashionNewSlotTypeByOld(EFashionWearSlotType type)
            {
                switch (type)
                {
                    case EFashionWearSlotType.Halo:
                        return EFashionWearNewSlotType.Halo;
                    case EFashionWearSlotType.Head:
                        return EFashionWearNewSlotType.Head;
                    case EFashionWearSlotType.Waist:
                        return EFashionWearNewSlotType.Waist;
                    case EFashionWearSlotType.UpperBody:
                        return EFashionWearNewSlotType.UpperBody;
                    case EFashionWearSlotType.LowerBody:
                        return EFashionWearNewSlotType.LowerBody;
                    case EFashionWearSlotType.Chest:
                        return EFashionWearNewSlotType.Chest;
                    case EFashionWearSlotType.Weapon:
                        return EFashionWearNewSlotType.Weapon;
                    case EFashionWearSlotType.Auras:
                        return EFashionWearNewSlotType.Auras;

                }
                return EFashionWearNewSlotType.Invalid;
            }

            public static int GetReduceRate(int level, int disPlayDefend)
            {
                float attackReduceRate = 0f;
                int value = 200 * level + disPlayDefend;
                if (value != 0)
                {
                    attackReduceRate = disPlayDefend * 1.0f / value;
                }
                return (int)(System.Math.Round(attackReduceRate, 2) * 100);
            }

            public static Dictionary<int, DecomposeTable> mDecomposeTableDic;

            public static DecomposeTable GetDecomposeData(ItemData item)
            {
                if (mDecomposeTableDic == null)
                {
                    mDecomposeTableDic = new Dictionary<int, DecomposeTable>();
                    var table = TableManager.GetInstance().GetTable<DecomposeTable>();
                    foreach(var data in table.Values)
                    {
                        var tableData = data as DecomposeTable;
                        mDecomposeTableDic.Add(tableData.LvArray(0) + tableData.Color2 * 1000 + (int)tableData.Color * 100000, tableData);
                    }
                }

                if (mDecomposeTableDic.ContainsKey(item.LevelLimit + item.Quality2 * 1000 + (int)item.Quality * 100000))
                {
                    return mDecomposeTableDic[item.LevelLimit + item.Quality2 * 1000 + (int)item.Quality * 100000];
                }

                return null;
            }

            public static void UpdateNumDecomposeMaterial(int length, Func<int, int> getNum, int id, Dictionary<int, KeyValuePair<int, int>> dic)
            {
                if (length == 0)
                {
                    return;
                }
                var min = int.MaxValue;
                var max = int.MinValue;
                for (int j = 0; j < length; ++j)
                {
                    min = Mathf.Min(min, getNum(j));
                    max = Mathf.Max(max, getNum(j));
                }

                if (min == max && min == 0 || (min == int.MaxValue && max == int.MinValue))
                {
                    return;
                }

                if (dic.ContainsKey(id))
                {
                    var keyValueData = dic[id];
                    keyValueData = new KeyValuePair<int, int>(keyValueData.Key + min, keyValueData.Value + max);
                    dic[id] = keyValueData;
                }
                else
                {
                    dic.Add(id, new KeyValuePair<int, int>(min, max));
                }
            }

            public static void UpdateStringDecomposeMaterial(int length, Func<int, string> getNum, string numStr, int id, Dictionary<int, KeyValuePair<int, int>> dic)
            {
                if (length == 0)
                {
                    return;
                }
                var min = int.MaxValue;
                var max = int.MinValue;
                int value = 0;
                if (length > 1)
                {
                    for (int j = 0; j < length; ++j)
                    {
                        numStr = getNum(j);
                        var numStrs = numStr.Split('|');
                        for (int i = 0; i < numStrs.Length; ++i)
                        {
                            var values = numStrs[i].Split('_');
                            if (values.Length > 1)
                            {
                                int.TryParse(values[1], out value);
                                min = Mathf.Min(min, value);
                                max = Mathf.Max(max, value);
                            }
                        }
                    }
                }
                else if (numStr != null)
                {
                    var numStrs = numStr.Split('|');
                    for (int i = 0; i < numStrs.Length; ++i)
                    {
                        var values = numStrs[i].Split('_');
                        if (values.Length > 1)
                        {
                            int.TryParse(values[1], out value);
                            min = Mathf.Min(min, value);
                            max = Mathf.Max(max, value);
                        }
                    }
                }

                if (min == max && min == 0 || (min == int.MaxValue && max == int.MinValue))
                {
                    return;
                }

                if (dic.ContainsKey(id))
                {
                    var keyValueData = dic[id];
                    keyValueData = new KeyValuePair<int, int>(keyValueData.Key + min, keyValueData.Value + max);
                    dic[id] = keyValueData;
                }
                else
                {
                    dic.Add(id, new KeyValuePair<int, int>(min, max));
                }
            }

            public static Dictionary<int, FashionDecomposeTable> mFashionDecomposeTableDic;

            public static FashionDecomposeTable GetFashionDecomposeData(ItemData item)
            {
                if (mFashionDecomposeTableDic == null)
                {
                    mFashionDecomposeTableDic = new Dictionary<int, FashionDecomposeTable>();
                    var table = TableManager.GetInstance().GetTable<FashionDecomposeTable>();
                    foreach (var data in table.Values)
                    {
                        var tableData = data as FashionDecomposeTable;
                        mFashionDecomposeTableDic.Add((int)tableData.Color + (int)tableData.ThirdType * 10 + (int)tableData.SubType * 100000, tableData);
                    }
                }

                if (mFashionDecomposeTableDic.ContainsKey((int)item.Quality + (int)item.ThirdType * 10 + (int)item.SubType * 100000))
                {
                    return mFashionDecomposeTableDic[(int)item.Quality + (int)item.ThirdType * 10 + (int)item.SubType * 100000];
                }

                return null;
            }

        }
    }
}
