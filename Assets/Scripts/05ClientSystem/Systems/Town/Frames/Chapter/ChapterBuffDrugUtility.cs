using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    public sealed class ChapterBuffDrugManager : DataManager<ChapterBuffDrugManager>
    {
        private List<int> mMark2UseBuffDrugs = new List<int>();

        public enum eBuffDrugUseType
        {
            /// <summary>
            /// 无效类型
            /// </summary>
            None,

            /// <summary>
            /// 免费试用
            /// </summary>
            FreeUse,

            /// <summary>
            /// 使用背包中
            /// </summary>
            PackageUse,

            /// <summary>
            /// 付费使用
            /// </summary>
            PayUse,

			/// <summary>
			/// 付费钱不够
			/// </summary>
			NotEnoughPay2Use,
        }

        public override void Initialize()
        {
        }

        public override void Clear()
        {
            ResetAllMarkedBuffDrugs();
        }

#region BuffDrugUseType
        private static bool _buffDrugFree2Use(int id)
        {
            BuffDrugConfigTable table = TableManager.instance.GetTableItem<BuffDrugConfigTable>(id);
            if (null != table)
            {
                return table.FreeUseLevel >= PlayerBaseData.GetInstance().Level;
            }

            return false;
        }

        public static eBuffDrugUseType GetBuffDrugType(int id)
        {
            if (_buffDrugFree2Use(id))
            {
                return eBuffDrugUseType.FreeUse;
            }
            else if (GetBuffDrugCount(id) > 0)
            {
                return eBuffDrugUseType.PackageUse;
            }
            else
            {
                CostItemManager.CostInfo costInfo = GetBuffDrugCost(id);
                if (CostItemManager.GetInstance().IsEnough2Cost(costInfo))
                {
                    return eBuffDrugUseType.PayUse;
                }
				else
				{
                    return eBuffDrugUseType.NotEnoughPay2Use;
                }
            }

            return eBuffDrugUseType.None;
        }


        public static int GetBuffDrugCount(int id)
        {
            return (int)ItemDataManager.GetInstance().GetOwnedItemCount(id);
        }

        public static CostItemManager.CostInfo GetBuffDrugCost(int id)
        {
            QuickBuyTable quicktable = TableManager.instance.GetTableItem<QuickBuyTable>(id);

            if (null != quicktable)
            {
                CostItemManager.CostInfo cost = new CostItemManager.CostInfo()
                {
                    nMoneyID = quicktable.CostItemID,
                    nCount = quicktable.CostNum
                };
                return cost;
            }

            return null;
        }
#endregion

#region MarkBuffDrug
        public void ResetAllMarkedBuffDrugs()
        {
            mMark2UseBuffDrugs.Clear();
        }

        public bool IsBuffDrugMarked(int id)
        {
            for (int i = 0; i < mMark2UseBuffDrugs.Count; ++i)
            {
                if (id == mMark2UseBuffDrugs[i])
                {
                    return true;
                }
            }

            return false;
        }

        public int[] GetAllMarkedBuffDrugs()
        {
            return mMark2UseBuffDrugs.ToArray();
        }

        public void SetMarkedBuffDrugsAtLocal()
        {
            for (int i = 0; i < mMark2UseBuffDrugs.Count; ++i)
            {
                if (PlayerPrefs.GetInt(_getBuffDrugSettingString(mMark2UseBuffDrugs[i]), 0) == 0) 
                {
                    PlayerPrefs.SetInt(_getBuffDrugSettingString(mMark2UseBuffDrugs[i]), 1);
                }
            }
        }

        public void ResetAllLocalMarkedBuffDrugs(IList<int> buffDrugConfig)
        {
            for (int i = 0; i < buffDrugConfig.Count; ++i)
            {
                PlayerPrefs.SetInt(_getBuffDrugSettingString(buffDrugConfig[i]), 0);
            }
        }

        public void ResetBuffDrugsFromLocal(IList<int> drugs)
        {
            if(null == drugs)
            {
                return;
            }
            for (int i = 0; i < drugs.Count; ++i)
            {
                if (IsBuffDrugSetted(drugs[i]))
                {
                    MarkBuffDrug2Use(drugs[i]);
                }
            }
        }

        public bool IsBuffDrugSetted(int id)
        {
            if (PlayerPrefs.GetInt(_getBuffDrugSettingString(id), 0) == 0) 
            {
                return false;
            }
            return true;
        }

        private void _deleteSettedBuffDrug(int id)
        {
            string buffDrugSettingString = _getBuffDrugSettingString(id);
            if (PlayerPrefs.HasKey(buffDrugSettingString))
            {
                PlayerPrefs.DeleteKey(buffDrugSettingString);
            }
        }

        private string _getBuffDrugSettingString(int id)
        {
            string serverID = "";
            string roleID = "";
            if (null != ClientApplication.playerinfo)
            {
                serverID = ClientApplication.playerinfo.serverID.ToString();
            }
            if (null != PlayerBaseData.GetInstance())
            {
                roleID = PlayerBaseData.GetInstance().RoleID.ToString();
            }
            return TR.Value("chapter_buffdrug_setting", serverID, roleID, id.ToString());
        }

        public void SetBuffDrugToggleState(bool isOn)
        {
            if (isOn)
            {
                PlayerPrefs.SetInt(_getBuffDrugToggleString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt(_getBuffDrugToggleString(), 0);
            }
        }

        public bool IsBuffDrugToggleOn()
        {
            if (PlayerPrefs.GetInt(_getBuffDrugToggleString(),0) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string _getBuffDrugToggleString()
        {
            string serverID = "";
            string roleID = "";
            if (null != ClientApplication.playerinfo)
            {
                serverID = ClientApplication.playerinfo.serverID.ToString();
            }
            if (null != PlayerBaseData.GetInstance())
            {
                roleID = PlayerBaseData.GetInstance().RoleID.ToString();
            }
            return TR.Value("chapter_buffdrug_toggleIsOn", serverID, roleID);
        }

        public bool MarkBuffDrug2Use(int id)
        {
            if (!IsBuffDrugMarked(id))
            {
                mMark2UseBuffDrugs.Add(id);
                return true;
            }
           
            return false;
        }

        public bool UnMarkBuffDrug2Use(int id)
        {
            if (IsBuffDrugMarked(id))
            {
                mMark2UseBuffDrugs.RemoveAll(x=>{return id == x;});
                return true;
            }
           
            return false;
        }
#endregion

#region MarkedBuffCost
        private void _addCostInfo(List<CostItemManager.CostInfo> costs, CostItemManager.CostInfo costInfo)
        {
            if (null != costs && null != costInfo)
            {
                bool find = false;
                for (int j = 0; j < costs.Count; j++)
                {
                    if (costInfo.nMoneyID == costs[j].nMoneyID)
                    {
						costs[j].nCount += costInfo.nCount;
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    costs.Add(costInfo);
                }
            }
        }


        /// <summary>
        /// 根据关卡来筛选标记的buff药的使用
        /// </summary>
        public List<uint> GetAllMarkedBuffDrugsByDungeonID(int dungeonID)
        {
            DungeonTable table = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);

            if (null == table)
            {
                return null;
            }

            IList<int> allValidBuff  = table.BuffDrugConfig;
            List<uint> validBuff2Use = new List<uint>();

            for (int i = 0; i < allValidBuff.Count; ++i)
            {
                if (IsBuffDrugMarked(allValidBuff[i]))
                {
                    validBuff2Use.Add((uint)allValidBuff[i]);
                }
            }

            return validBuff2Use;
        }

        public List<CostItemManager.CostInfo> GetAllMarkedBuffDrugsCost(int dungeonID)
        {
            List<uint> buffs = GetAllMarkedBuffDrugsByDungeonID(dungeonID);

            if (null == buffs)
            {
                return null;
            }

            List<CostItemManager.CostInfo> costs        = new List<CostItemManager.CostInfo>();

            for (int i = 0; i < buffs.Count; ++i)
            {
                eBuffDrugUseType type = GetBuffDrugType((int)buffs[i]);
                if (eBuffDrugUseType.PayUse == type ||
                    eBuffDrugUseType.NotEnoughPay2Use == type)
                {
                    CostItemManager.CostInfo costInfo = GetBuffDrugCost((int)buffs[i]);
                    _addCostInfo(costs, costInfo);
                }
            }

            return costs;
        }
#endregion

#region UseItem 

        private byte _getBuffDrugIdx(int dungeonID, int itemid)
        {
            DungeonTable table = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);
            if (null != table)
            {
                IList<int> drugs = table.BuffDrugConfig;
                for (int i = 0; i < drugs.Count; ++i)
                {
                    if (drugs[i] == itemid)
                    {
                        return (byte)i;
                    }
                }
            }

            return byte.MaxValue;
        }


        private IEnumerator _useItem(int itemID)
        {
            MessageEvents        msg = new MessageEvents();
            SceneQuickUseItemRet res = new SceneQuickUseItemRet();
            SceneQuickUseItemReq req = new SceneQuickUseItemReq();

            int id                   = ChapterBaseFrame.sDungeonID;

            req.dungenid             = (uint)id;
            req.idx                  = _getBuffDrugIdx(id, itemID);

            yield return MessageUtility.Wait<SceneQuickUseItemReq, SceneQuickUseItemRet>(ServerType.GATE_SERVER, msg, req, res);

            if (msg.IsAllMessageReceived())
            {
                if (res.code != 0)
                {
                    // TODO send fail message
                    //SystemNotifyManager.SystemNotify((int)res.code);
                }
                else 
                {
                    // TODO send success message
                }
            }

            // TODO send fail message
        }

        private void _usePackageItem(int id)
        {
            ItemData data = ItemDataManager.GetInstance().GetItemByTableID(id);
            ItemDataManager.GetInstance().UseItem(data);
        }


        public IEnumerator UseBuffDrug(int id)
        {
            eBuffDrugUseType type = GetBuffDrugType(id);

            if (type == eBuffDrugUseType.FreeUse)
            {
                Logger.LogProcessFormat("[buffdrug] 免费使用 {0}", id);

                yield return _useItem(id);

                GameStatisticManager.instance.DoStatDrugUse(id, "free");
            }
            else if (type == eBuffDrugUseType.PackageUse)
            {
                Logger.LogProcessFormat("[buffdrug] 消耗道具 {0} 剩余 {1}", id, GetBuffDrugCount(id) );
                _usePackageItem(id);

                GameStatisticManager.instance.DoStatDrugUse(id, "package");
            }
            else if (type == eBuffDrugUseType.PayUse)
            {
                CostItemManager.CostInfo info = GetBuffDrugCost(id);

                Logger.LogProcessFormat("[buffdrug] 消耗{0} {1}", info.nMoneyID, info.nCount);

                yield return _useItem(id);

                GameStatisticManager.instance.DoStatDrugUse(id, "pay&use");
            }

            // TODO send fail message
        }

        public IEnumerator UseAllMarkBuffDrug(int dungeonID)
        {
            List<uint> node = GetAllMarkedBuffDrugsByDungeonID(dungeonID);

            if (null != node)
            {
                for (int i = 0; i < node.Count; ++i)
                {
                    yield return UseBuffDrug((int)node[i]);
                }
            }

            ResetAllMarkedBuffDrugs();
        }
#endregion
    }
}
