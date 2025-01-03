using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine.Assertions;
using Network;
using Protocol;

namespace GameClient
{
    class EquipForgeDataManager : DataManager<EquipForgeDataManager>
    {
        public class CheckForgeResult
        {
            public enum EType
            {
                CanForge,
                LessPrice,
                LessMaterial,
            }

            public EType eType;
            public object userData;
        }

        public class ForgeInfo
        {
            public int nRecommendLevel;
            public IList<int> arrRecommendJobs;
            public ItemData itemData;
            public List<ItemData> arrMaterials = new List<ItemData>();
            public List<ItemData> arrPrices = new List<ItemData>();
        }

        public class TreeForgeInfo
        {
            public string strKey;
            public TreeForgeInfo parent;
            public List<TreeForgeInfo> arrInfos = new List<TreeForgeInfo>();

            /// <summary>
            /// 是否适合当前穿戴
            /// </summary>
            public bool bSuitable;

            /// <summary>
            /// 是否有可以打造的装备
            /// </summary>
            public bool bCanForge;

            public object param;
        }

        TreeForgeInfo m_treeForgeInfo = new TreeForgeInfo();

        public override void Initialize()
        {
            var iter = TableManager.GetInstance().GetTable<ProtoTable.EquipForgeTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                int nID = iter.Current.Key;
                ProtoTable.EquipForgeTable table = iter.Current.Value as ProtoTable.EquipForgeTable;
                ForgeInfo forgeInfo = new ForgeInfo();
                forgeInfo.itemData = ItemDataManager.CreateItemDataFromTable(nID);
                forgeInfo.nRecommendLevel = table.RecommendLevel;
                forgeInfo.arrRecommendJobs = table.RecommendJobs;

                for (int i = 0; i < table.Material.Count; ++i)
                {
                    string[] values = table.Material[i].Split('_');
                    Assert.IsTrue(values.Length == 2);
                    ItemData item = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                    item.Count = int.Parse(values[1]);
                    forgeInfo.arrMaterials.Add(item);
                }

                for (int i = 0; i < table.Price.Count; ++i)
                {
                    string[] values = table.Price[i].Split('_');
                    Assert.IsTrue(values.Length == 2);
                    ItemData item = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                    item.Count = int.Parse(values[1]);
                    forgeInfo.arrPrices.Add(item);
                }

                
                for (int i= 0; i < forgeInfo.arrRecommendJobs.Count; ++i)
                {
                    ProtoTable.JobTable jobTable = TableManager.instance.GetTableItem<ProtoTable.JobTable>(forgeInfo.arrRecommendJobs[i]);
                    if (jobTable != null && jobTable.Open > 0)
                    {
                        TreeForgeInfo tempInfo1 = _FindTreeForgeInfo(m_treeForgeInfo, jobTable.Name);
                        if (tempInfo1 == null)
                        {
                            tempInfo1 = new TreeForgeInfo();
                            tempInfo1.strKey = jobTable.Name;
                            tempInfo1.parent = m_treeForgeInfo;
                            m_treeForgeInfo.arrInfos.Add(tempInfo1);

                            tempInfo1.bSuitable = false;
                            tempInfo1.bCanForge = false;
                            tempInfo1.param = forgeInfo.arrRecommendJobs[i];
                        }

                        TreeForgeInfo tempInfo2 = _FindTreeForgeInfo(tempInfo1, table.MainTypeName);
                        if (tempInfo2 == null)
                        {
                            tempInfo2 = new TreeForgeInfo();
                            tempInfo2.strKey = table.MainTypeName;
                            tempInfo2.parent = tempInfo1;
                            tempInfo1.arrInfos.Add(tempInfo2);

                            tempInfo2.bSuitable = false;
                            tempInfo2.bCanForge = false;
                        }

                        TreeForgeInfo tempInfo3 = _FindTreeForgeInfo(tempInfo2, table.SubTypeName);
                        if (tempInfo3 == null)
                        {
                            tempInfo3 = new TreeForgeInfo();
                            tempInfo3.strKey = table.SubTypeName;
                            tempInfo3.parent = tempInfo2;
                            tempInfo2.arrInfos.Add(tempInfo3);

                            tempInfo3.bSuitable = false;
                            tempInfo3.bCanForge = false;
                        }

                        string strName = forgeInfo.itemData.TableID.ToString();
                        TreeForgeInfo tempInfo4 = _FindTreeForgeInfo(tempInfo3, strName);
                        if (tempInfo4 == null)
                        {
                            tempInfo4 = new TreeForgeInfo();
                            tempInfo4.strKey = strName;
                            tempInfo4.parent = tempInfo3;
                            tempInfo3.arrInfos.Add(tempInfo4);

                            tempInfo4.bSuitable = false;
                            tempInfo4.bCanForge = false;
                        }

                        tempInfo4.param = forgeInfo;

                        //bool bSuitable = IsEquipSuitable(forgeInfo);
                        //if (bSuitable)
                        //{
                        //    m_treeForgeInfo.bSuitable = true;
                        //    tempInfo1.bSuitable = true;
                        //    tempInfo2.bSuitable = true;
                        //    tempInfo3.bSuitable = true;
                        //    tempInfo4.bSuitable = true;
                        //}

                        //bool bCanForge = CheckEquipCanForge(forgeInfo).eType == CheckForgeResult.EType.CanForge;
                        //if (bCanForge)
                        //{
                        //    m_treeForgeInfo.bCanForge = true;
                        //    tempInfo1.bCanForge = true;
                        //    tempInfo2.bCanForge = true;
                        //    tempInfo3.bCanForge = true;
                        //    tempInfo4.bCanForge = true;
                        //}
                    }
                }
            }
        }

        public override void Clear()
        {
            m_treeForgeInfo = new TreeForgeInfo();
        }

        public TreeForgeInfo GetTreeForgeInfo()
        {
            return m_treeForgeInfo;
        }

        public bool CheckRedPoint()
        {
            return _CheckRedPoint(m_treeForgeInfo);
        }

        public void UpdateSuitable()
        {
            _UpdateSuitable(m_treeForgeInfo);
        }

        public void UpdateCanForge()
        {
            _UpdateCanForge(m_treeForgeInfo);
        }

        public bool IsEquipSuitable(ForgeInfo a_equip)
        {
            int nMinLevel = PlayerBaseData.GetInstance().Level - 30;
            int nMaxLevel = PlayerBaseData.GetInstance().Level;
            if (a_equip.nRecommendLevel >= nMinLevel && a_equip.nRecommendLevel <= nMaxLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CheckForgeResult CheckEquipCanForge(ForgeInfo a_info)
        {
            CheckForgeResult result = new CheckForgeResult();
            for (int i = a_info.arrMaterials.Count - 1; i >= 0; --i)
            {
                ItemData material = a_info.arrMaterials[i];
                if (material.Count > ItemDataManager.GetInstance().GetOwnedItemCount(material.TableID))
                {
                    result.eType = CheckForgeResult.EType.LessMaterial;
                    result.userData = material;
                    return result;
                }
            }

            for (int i = 0; i < a_info.arrPrices.Count; ++i)
            {
                ItemData price = a_info.arrPrices[i];
                if (price.Count > ItemDataManager.GetInstance().GetOwnedItemCount(price.TableID))
                {
                    result.eType = CheckForgeResult.EType.LessPrice;
                    result.userData = price;
                    return result;
                }
            }

            result.eType = CheckForgeResult.EType.CanForge;
            return result;
        }

        public void ForgeEquip(int a_nEquipID)
        {
            SceneEquipMakeReq msgReq = new SceneEquipMakeReq();
            msgReq.equipId = (uint)a_nEquipID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msgReq);

            WaitNetMessageManager.GetInstance().Wait<SceneEquipMakeRet>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipForgeFail, msgRet.code);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipForgeSuccess, a_nEquipID);
                }
            });
        }

        bool _CheckRedPoint(TreeForgeInfo a_treeForgeInfo)
        {
            bool bResult = false;
            for (int i = 0; i < a_treeForgeInfo.arrInfos.Count; ++i)
            {
                TreeForgeInfo treeForgeInfo = a_treeForgeInfo.arrInfos[i];
                ForgeInfo forgeInfo = treeForgeInfo.param as ForgeInfo;
                if (forgeInfo != null)
                {
                    if (CheckEquipCanForge(forgeInfo).eType == CheckForgeResult.EType.CanForge)
                    {
                        bResult = true;
                        break;
                    }
                }
                else
                {
                    bResult = _CheckRedPoint(treeForgeInfo);
                }
            }

            return bResult;
        }

        TreeForgeInfo _FindTreeForgeInfo(TreeForgeInfo a_info, params string[] a_keys)
        {
            TreeForgeInfo info = a_info;
            for (int i = 0; i < a_keys.Length; ++i)
            {
                string key = a_keys[i];

                bool bFound = false;
                for (int j = 0; j < info.arrInfos.Count; ++j)
                {
                    if (info.arrInfos[j].strKey == key)
                    {
                        info = info.arrInfos[j];
                        bFound = true;
                        break;
                    }
                }

                if (bFound == false)
                {
                    return null;
                }
            }

            return info;
        }

        void _UpdateSuitable(TreeForgeInfo a_treeForgeInfo)
        {
            for (int i = 0; i < a_treeForgeInfo.arrInfos.Count; ++i)
            {
                TreeForgeInfo treeForgeInfo = a_treeForgeInfo.arrInfos[i];
                treeForgeInfo.bSuitable = false;
                ForgeInfo forgeInfo = treeForgeInfo.param as ForgeInfo;
                if (forgeInfo != null)
                {
                    bool isSuitable = IsEquipSuitable(forgeInfo);
                    if (isSuitable)
                    {
                        while (treeForgeInfo != null)
                        {
                            treeForgeInfo.bSuitable = true;
                            treeForgeInfo = treeForgeInfo.parent;
                        }
                    }
                }
                else
                {
                    _UpdateSuitable(treeForgeInfo);
                }
            }
        }

        void _UpdateCanForge(TreeForgeInfo a_treeForgeInfo)
        {
            for (int i = 0; i < a_treeForgeInfo.arrInfos.Count; ++i)
            {
                TreeForgeInfo treeForgeInfo = a_treeForgeInfo.arrInfos[i];
                treeForgeInfo.bCanForge = false;
                ForgeInfo forgeInfo = treeForgeInfo.param as ForgeInfo;
                if (forgeInfo != null)
                {
                    if (CheckEquipCanForge(forgeInfo).eType == CheckForgeResult.EType.CanForge)
                    {
                        while (treeForgeInfo != null)
                        {
                            treeForgeInfo.bCanForge = true;
                            treeForgeInfo = treeForgeInfo.parent;
                        }
                    }
                }
                else
                {
                    _UpdateCanForge(treeForgeInfo);
                }
            }
        }
    }
}
