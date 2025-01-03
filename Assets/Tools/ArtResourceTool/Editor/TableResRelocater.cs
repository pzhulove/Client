using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
public class TableResRelocater : Editor
{
    static string NEW_ICON_PATH = "Assets/Resources/UI/Image/Icon/";

    static List<SpriteDesc> m_SpriteDescList = new List<SpriteDesc>();
    static Dictionary<string, int> m_SpriteDescNameTbl = new Dictionary<string, int>();


    class SpriteDesc
    {
        public string m_SpriteName = null;
        public string m_ImagePath = null;
    }

    [MenuItem("[TM工具集]/ArtTools/Rel")]
    static public void ReloactorIconRes()
    {
        string[] spriteList = AssetDatabase.FindAssets("t:Texture", new string[] {
            "Assets/Resources/UI/Image/Icon/Icon_Buff",
            "Assets/Resources/UI/Image/Icon/Icon_Duanwei",
            "Assets/Resources/UI/Image/Icon/Icon_Equip",
            "Assets/Resources/UI/Image/Icon/Icon_Head",
            "Assets/Resources/UI/Image/Icon/Icon_Fashion",
            "Assets/Resources/UI/Image/Icon/Icon_Item",
            "Assets/Resources/UI/Image/Icon/Icon_Jar",
            "Assets/Resources/UI/Image/Icon/Icon_Recharge",
            "Assets/Resources/UI/Image/Icon/Icon_Skill" 
        });
        
        m_SpriteDescList.Clear();
        for (int i = 0,icnt = spriteList.Length;i<icnt;++i)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteList[i]);
            if (string.IsNullOrEmpty(assetPath)) continue;
        
            Object[] assetObjList = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            for (int j = 0,jcnt = assetObjList.Length;j<jcnt;++j)
            {
                Sprite sp = assetObjList[j] as Sprite;
                if(null == sp) continue;
        
                SpriteDesc newSpDesc = new SpriteDesc();
                newSpDesc.m_SpriteName = sp.name;
                newSpDesc.m_ImagePath = assetPath;
        
                m_SpriteDescList.Add(newSpDesc);
            }
        }
        
        m_SpriteDescNameTbl.Clear();
        for (int i = 0, icnt = m_SpriteDescList.Count; i < icnt; ++i)
        {
            if (!m_SpriteDescNameTbl.ContainsKey(m_SpriteDescList[i].m_SpriteName))
                m_SpriteDescNameTbl.Add(m_SpriteDescList[i].m_SpriteName, i);
            else
                Logger.LogErrorFormat("Sprite {0} 资源名冲突！！！", m_SpriteDescList[i].m_SpriteName);
        }
        
        List<string> oldResList = new List<string>();
        
        string[] feildList_SkillTable = new string[] { "Icon" };
        string[] expathList_SkillTable = new string[] { "" };
        _RenameIconInXlsxTable("SkillTable", feildList_SkillTable, expathList_SkillTable, ref oldResList);
        _ClearXlsxTableRes("SkillTable", oldResList);
        
        string[] feildList_ItemTable = new string[] { "Icon", "ModelPath" };
        string[] expathList_ItemTable = new string[] { "" ,""};
        _RenameIconInXlsxTable("ItemTable", feildList_ItemTable, expathList_ItemTable, ref oldResList);
        _ClearXlsxTableRes("ItemTable", oldResList);
        
        string[] feildList_ResTable = new string[] { "IconPath" };
        string[] expathList_ResTable = new string[] { "" };
        _RenameIconInXlsxTable("ResTable", feildList_ResTable, expathList_ResTable, ref oldResList);
        _ClearXlsxTableRes("ResTable", oldResList);
        
        string[] feildList_JobTable = new string[] { "SkillIcon" };
        string[] expathList_JobTable = new string[] { "" };
        _RenameIconInXlsxTable("JobTable", feildList_JobTable, expathList_JobTable, ref oldResList);
        _ClearXlsxTableRes("JobTable", oldResList);
        
        string[] feildList_ItemCollectionTable = new string[] { "Icon" };
        string[] expathList_ItemCollectionTable = new string[] { "" };
        _RenameIconInXlsxTable("ItemCollectionTable", feildList_ItemCollectionTable, expathList_ItemCollectionTable, ref oldResList);
        _ClearXlsxTableRes("ItemCollectionTable", oldResList);
        
        string[] feildList_NpcTable = new string[] { "NpcIcon" };
        string[] expathList_NpcTable = new string[] { "" };
        _RenameIconInXlsxTable("NpcTable", feildList_NpcTable, expathList_NpcTable, ref oldResList);
        _ClearXlsxTableRes("NpcTable", oldResList);
        
        string[] feildList_SeasonLevelTable = new string[] { "Icon", "SmallIcon", "SubLevelIcon" };
        string[] expathList_SeasonLevelTable = new string[] { "", "", "" };
        _RenameIconInXlsxTable("SeasonLevelTable", feildList_SeasonLevelTable, expathList_SeasonLevelTable,ref oldResList);
        _ClearXlsxTableRes("SeasonLevelTable", oldResList);
        
        string[] feildList_PkLevelTable = new string[] { "Path" };
        string[] expathList_PkLevelTable = new string[] { "" };
        _RenameIconInXlsxTable("PkLevelTable", feildList_PkLevelTable, expathList_PkLevelTable, ref oldResList);
        _ClearXlsxTableRes("PkLevelTable", oldResList);
        
        string[] feildList_JarBonus = new string[] { "JarImage" };
        string[] expathList_JarBonus = new string[] { "" };
        _RenameIconInXlsxTable("JarBonus", feildList_JarBonus, expathList_JarBonus, ref oldResList);
        _ClearXlsxTableRes("JarBonus", oldResList);
        
        string[] feildList_PetTable = new string[] { "IconPath" };
        string[] expathList_PetTable = new string[] { "" };
        _RenameIconInXlsxTable("PetTable", feildList_PetTable, expathList_PetTable, ref oldResList);
        _ClearXlsxTableRes("PetTable", oldResList);
        
        string[] feildList_MissionScoreTable = new string[] { "UnOpenedChestBoxIcon", "OpenedChestBoxIcon" };
        string[] expathList_MissionScoreTable = new string[] { "","" };
        _RenameIconInXlsxTable("MissionScoreTable", feildList_MissionScoreTable, expathList_MissionScoreTable, ref oldResList);
        _ClearXlsxTableRes("MissionScoreTable", oldResList);
        
        string[] feildList_ChargeMallTable = new string[] { "Icon" };
        string[] expathList_ChargeMallTable = new string[] { ""};
        _RenameIconInXlsxTable("ChargeMallTable", feildList_ChargeMallTable, expathList_ChargeMallTable, ref oldResList);
        _ClearXlsxTableRes("ChargeMallTable", oldResList);

        //_RepathIconXlsxTable(new string[] { "Assets/Resources/UI/Image/Icon/Icon_Duanwei"                                                                                           }, "SeasonLevelTable"   , new string[] { "Icon"         }, new string[] { "" });
        //_RepathIconXlsxTable(new string[] { "Assets/Resources/UI/Image/Icon/Icon_Equip", "Assets/Resources/UI/Image/Icon/Icon_Item" , "Assets/Resources/UI/Image/Icon/Icon_Fashion" }, "ItemTable"          , new string[] { "Icon"         }, new string[] { "" });
        //_RepathIconXlsxTable(new string[] { "Assets/Resources/UI/Image/Icon/Icon_Equip", "Assets/Resources/UI/Image/Icon/Icon_Item", "Assets/Resources/UI/Image/Icon/Icon_Fashion"  }, "ItemCollectionTable", new string[] { "Icon"         }, new string[] { "" });
        //_RepathIconXlsxTable(new string[] { "Assets/Resources/UI/Image/Icon/Icon_Head"                                                                                              }, "NpcTable"           , new string[] { "NpcIcon"      }, new string[] { "" });
        //_RepathIconXlsxTable(new string[] { "Assets/Resources/UI/Image/Icon/Icon_Head"                                                                                              }, "ResTable"           , new string[] { "Icon"         }, new string[] { "" });
        //_RepathIconXlsxTable(new string[] { "Assets/Resources/UI/Image/Icon/Icon_Skill"                                                                                             }, "JobTable"           , new string[] { "SkillIcon"    }, new string[] { "" });
        //_RepathIconXlsxTable(new string[] { "Assets/Resources/UI/Image/Icon/Icon_Skill"                                                                                             }, "SkillTable"         , new string[] { "Icon"         }, new string[] { "" });

    }

    static protected void _ClearXlsxTableRes(string tableName,List<string> resList)
    {
        return;
        if (resList.Count > 0)
            AssetDatabase.ExportPackage(resList.ToArray(), tableName + "_UI图标备份.unitypackage");

        for(int i =0,icnt = resList.Count;i<icnt;++i)
        {
            string path = resList[i];
            File.Delete(path);
        }

    }

    static protected void _RepathIconXlsxTable(string[] spritePackImage,string tableName, string[] feildList, string[] expathList)
    {

        string[] spriteList = AssetDatabase.FindAssets("t:Texture", spritePackImage);

        m_SpriteDescList.Clear();
        for (int i = 0, icnt = spriteList.Length; i < icnt; ++i)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteList[i]);
            if (string.IsNullOrEmpty(assetPath)) continue;

            Object[] assetObjList = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            for (int j = 0, jcnt = assetObjList.Length; j < jcnt; ++j)
            {
                Sprite sp = assetObjList[j] as Sprite;
                if (null == sp) continue;

                SpriteDesc newSpDesc = new SpriteDesc();
                newSpDesc.m_SpriteName = sp.name;
                newSpDesc.m_ImagePath = assetPath;

                m_SpriteDescList.Add(newSpDesc);
            }
        }

        m_SpriteDescNameTbl.Clear();
        for (int i = 0, icnt = m_SpriteDescList.Count; i < icnt; ++i)
        {
            if (!m_SpriteDescNameTbl.ContainsKey(m_SpriteDescList[i].m_SpriteName))
                m_SpriteDescNameTbl.Add(m_SpriteDescList[i].m_SpriteName, i);
            else
                Logger.LogErrorFormat("Sprite {0} 资源名冲突！！！", m_SpriteDescList[i].m_SpriteName);
        }

        List<string> oldResList = new List<string>();
        _RenameIconInXlsxTable(tableName, feildList, expathList, ref oldResList);

        //if (oldResList.Count > 0)
        //    AssetDatabase.ExportPackage(oldResList.ToArray(), tableName + "_UI图标备份.unitypackage");
    }


    static protected void _RenameIconInXlsxTable(string tableName, string[] feildList, string[] expathList,ref List<string> oldResList)
    {
        oldResList.Clear();
        XlsxDataUnit bufftbl = XlsxDataManager.Instance().GetXlsxByName(tableName);
        if (null != bufftbl)
        {
            for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < bufftbl.RowCount; ++i)
            {
                Dictionary<string, ICell> tbl = bufftbl.GetRowDataByLine(i);

                for (int feild = 0; feild < feildList.Length; ++feild)
                {
                    ICell curCell = tbl[feildList[feild]];
                    if (null == curCell)
                        continue;

                    string file = curCell.CustomToString();
                    if (string.IsNullOrEmpty(file))
                        continue;

                    if("－" == file)
                    {
                        curCell.SetCellValue("-");
                        continue;
                    }

                    if (!string.IsNullOrEmpty(file) && "-" != file)
                    {
                        string[] fileList = file.Split('|');
                        string resNewPath = "";
                        for (int split = 0; split < fileList.Length; ++split)
                        {
                            string resKey = expathList[feild] + fileList[split];

                            string[] oldResArray = resKey.Split(':');
                            if (null != oldResArray)
                            {
                                //Logger.LogError("### " + oldResArray[0]);
                                oldResList.Add(Path.Combine("Assets/Resources/", Path.ChangeExtension(oldResArray[0], "png")));
                            }

                            string spriteName = _GetSpriteName(resKey);

                            int idx = 0;
                            if (m_SpriteDescNameTbl.TryGetValue(spriteName, out idx))
                            {
                                SpriteDesc spDesc = m_SpriteDescList[idx];
                                if (null != spDesc)
                                {
                                    string newPath = spDesc.m_ImagePath.Replace("Assets/Resources/", null) + ":" + spDesc.m_SpriteName;
                                    Sprite sp = AssetLoader.instance.LoadRes(newPath, typeof(Sprite)).obj as Sprite;
                                    if (null != sp)
                                    {
                                        resNewPath += newPath;
                                        resNewPath += "|";
                                        Resources.UnloadAsset(sp);
                                    }
                                    else
                                        Logger.LogError("原始资源不存在表格修改失败！！！");
                                }
                                else
                                    Logger.LogError("无效的表格资源索引，表格修改失败！！！");
                            }
                            else
                                Logger.LogErrorFormat("Sprite名字[{0}]不存在，表格修改失败！！", spriteName);
                        }

                        if(!string.IsNullOrEmpty(resNewPath))
                        {
                            resNewPath = resNewPath.Substring(0, resNewPath.Length - 1);
                            curCell.SetCellValue(resNewPath);
                        }
                    }
                }
            }

            /// 保存
            bufftbl.Write();

#if USE_FB_TABLE
            Xls2FBWindow.DoConvertAFile(XlsxDataManager.m_pDictKeyPath[tableName]);
#else
            /// 转表
            bufftbl.GetText();
#endif
        }
        else
        {
            Logger.LogWarningFormat("Can not find table {0}!", tableName);
        }
    }

    static string _GetSpriteName(string res)
    {
        string[] resSplit = res.Split(':');
        if(resSplit.Length > 1)
            return resSplit[1];
        else
            return Path.GetFileNameWithoutExtension(resSplit[0]);
    }
}
