using System.Collections.Generic;
using System;
using UnityEngine;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{

    public class TheyLuckyData
    {
        public int nID;
        public  List<AwardItemData> itemData;
    }
    public class SyncDrawPrizeCountData
    {
        public uint drawPrizeTableID;
        public string countKey;
        public string totalCountKey;
    }
    public class OPPOPrivilegeDataManager : DataManager<OPPOPrivilegeDataManager>
    {
        Dictionary<int, TheyLuckyData> m_dictDrawData = new Dictionary<int, TheyLuckyData>();
        public const int oppoPrivilegeID = 12000;
        public const int vivoPrivilegeID = 23000;
        public const int dailID = 15000;
        public const int luckyGuyID = 17000;
        public const int tableID = 10001;
        int IActivitytEmplateID = 20000;
        int iAmberPrivilegeActivityId = 27000;//琥珀特权
        int iOPPOGrowthHaoLiActivityId = 26000; //OPPO成长豪礼
        public int surplusNum = 0;
        public int totalNum = 0;

        int[] oppoActivityID = new int[] { 12000, 15000, 17000, 20000, 260000, 27000 };

       /// <summary>
       /// Oppo琥珀等级
       /// </summary>
        public int OppOAmberLevel
        {
            get;set;
        }

        public sealed override void Clear()
        {
            if (SDKInterface.Instance.IsOppoPlatform() || SDKInterface.Instance.IsVivoPlatForm())
            {
                _ClearDrawTableData();
                OppOAmberLevel = 0;
            }
        }

        public sealed override void Initialize()
        {
            if (SDKInterface.Instance.IsOppoPlatform() || SDKInterface.Instance.IsVivoPlatForm())
            {
                _InitDrawPrizeTableData();
            }
        }
        public TheyLuckyData GetTheLuckyData(int a_nID)
        {
            TheyLuckyData luckyData = null;
            m_dictDrawData.TryGetValue(a_nID, out luckyData);
            return luckyData;
        }
        void _InitDrawPrizeTableData()
        {
            m_dictDrawData.Clear();
            Dictionary<int, List<AwardItemData>> RewardPoolDataDic = new Dictionary<int, List<AwardItemData>>();
            var rewardPoolTable = TableManager.GetInstance().GetTable<RewardPoolTable>().GetEnumerator();
            while (rewardPoolTable.MoveNext())
            {
                RewardPoolTable table = rewardPoolTable.Current.Value as RewardPoolTable;

                if ( table == null)
                {
                    continue;
                }
                AwardItemData data = new AwardItemData();
                data.ID = table.ItemID;
                data.Num = table.ItemNum;

                if (RewardPoolDataDic.ContainsKey(table.DrawPrizeTableID) == false)
                {
                    RewardPoolDataDic.Add(table.DrawPrizeTableID, new List<AwardItemData>());
                }
                RewardPoolDataDic[table.DrawPrizeTableID].Add(data);
            }


            var iter = TableManager.GetInstance().GetTable<DrawPrizeTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                DrawPrizeTable table = iter.Current.Value as DrawPrizeTable;
                if (table == null)
                {
                    continue;
                }

                TheyLuckyData data = new TheyLuckyData();
                data.nID = table.ID;
                if (RewardPoolDataDic.ContainsKey(data.nID))
                {
                    data.itemData = RewardPoolDataDic[data.nID];
                }
                else
                {
                    data.itemData = new List<AwardItemData>();
                }

                m_dictDrawData.Add(data.nID, data);
            }

           
        }
        //登陆特权
        public bool _CheckPrivilrge(int templateID)
        {
            int index = 0;
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(templateID);

            if (activeData == null)
            {
                return false;
            }
            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                if (activeData.akChildItems[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    index++;
                }
            }
            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //幸运转盘
        public bool _CheckLuckyGuy()
        {
            List<TaskPair> myLuckyGuyTaskPairList = new List<TaskPair>();
            DrawPrizeTable drawPrizeTabel = TableManager.GetInstance().GetTableItem<DrawPrizeTable>(tableID);
            if (drawPrizeTabel == null)
            {
                Logger.LogErrorFormat("DrawPrizeTabl is null");
            }
            myLuckyGuyTaskPairList.Clear();
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(luckyGuyID);

            if (activeData == null)
            {
                return false;
            }

            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                for (int j = 0; j < activeData.akChildItems[i].akActivityValues.Count; j++)
                {
                    myLuckyGuyTaskPairList.Add(activeData.akChildItems[i].akActivityValues[j]);
                }
            }

            int count = 0;
            for (int i = 0; i < myLuckyGuyTaskPairList.Count; i++)
            {
                if (myLuckyGuyTaskPairList[i].key == drawPrizeTabel.RestCountKey)
                {
                    int.TryParse(myLuckyGuyTaskPairList[i].value, out count);
                }
            }
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        //七日签到
        public bool _CheckDail()
        {
            int index = 0;
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(dailID);
            if (activeData == null)
            {
                return false;
            }
            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                if (activeData.akChildItems[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    index++;
                }
            }

            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        //琥珀会员
        public bool _CheckAmberGiftBag()
        {
            int index = 0;
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(IActivitytEmplateID);
            if (activeData == null)
            {
                return false;
            }
            for (int j = 0; j < activeData.akChildItems.Count; j++)
            {
                if (activeData.akChildItems[j].status == (int)TaskStatus.TASK_FINISHED)
                {
                    index++;
                }
            }
            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 琥珀特权
        /// </summary>
        /// <returns></returns>
        public bool _CheckAmberPrivilege()
        {
            int index = 0;
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(iAmberPrivilegeActivityId);
            if (activeData == null)
            {
                return false;
            }
            for (int j = 0; j < activeData.akChildItems.Count; j++)
            {
                if (activeData.akChildItems[j].status == (int)TaskStatus.TASK_FINISHED)
                {
                    index++;
                }
            }
            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool _CheckOPPOGrowthHaoLi()
        {
            int index = 0;
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(iOPPOGrowthHaoLiActivityId);
            if (activeData == null)
            {
                return false;
            }
            for (int j = 0; j < activeData.akChildItems.Count; j++)
            {
                if (activeData.akChildItems[j].status == (int)TaskStatus.TASK_FINISHED)
                {
                    index++;
                }
            }
            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool _ActiveIsOpen()
        {
            if (SDKInterface.Instance.IsOppoPlatform())
            {
                for (int i = 0; i < oppoActivityID.Length; i++)
                {
                    ActiveManager.ActiveData Data = ActiveManager.GetInstance().GetActiveData(oppoActivityID[i]);
                    if (Data == null)
                    {
                        continue;
                    }

                    if (Data.mainInfo.state != (byte)StateType.Running)
                    {
                        continue;
                    }

                    return true;
                }

                return false;
            }
            else if (SDKInterface.Instance.IsVivoPlatForm())
            {
                ActiveManager.ActiveData privilegeActive = ActiveManager.GetInstance().GetActiveData(vivoPrivilegeID);

                if (privilegeActive == null)
                {
                    return false;
                }

                if (privilegeActive.mainInfo.state != (byte)StateType.Running)
                {
                    return false;
                }
            }
            
            return true;
        }
        protected void _ClearDrawTableData()
        {
            m_dictDrawData.Clear();
        }

        /// <summary>
        /// 得到OPPO琥珀等级
        /// </summary>
        /// <returns></returns>
        public string GetAmberLevel()
        {
            string amberLevel = "";
            switch ((OppoAmberLevel)OppOAmberLevel)
            {
                case OppoAmberLevel.OAL_NONE:
                    amberLevel = "无";
                    break;
                case OppoAmberLevel.OAL_GREEN_ONE:
                    amberLevel = "绿珀一星";
                    break;
                case OppoAmberLevel.OAL_GREEN_TWO:
                    amberLevel = "绿珀二星";
                    break;
                case OppoAmberLevel.OAL_GREEN_THREE:
                    amberLevel = "绿珀三星";
                    break;
                case OppoAmberLevel.OAL_BLUE_ONE:
                    amberLevel = "蓝珀一星";
                    break;
                case OppoAmberLevel.OAL_BLUE_TWO:
                    amberLevel = "蓝珀二星";
                    break;
                case OppoAmberLevel.OAL_BLUE_THREE:
                    amberLevel = "蓝珀三星";
                    break;
                case OppoAmberLevel.OAL_GOLD_ONE:
                    amberLevel = "金珀一星";
                    break;
                case OppoAmberLevel.OAL_GOLD_TWO:
                    amberLevel = "金珀二星";
                    break;
                case OppoAmberLevel.OAL_GOLD_THREE:
                    amberLevel = "金珀三星";
                    break;
                case OppoAmberLevel.OAL_RED:
                    amberLevel = "华贵红珀";
                    break;
                case OppoAmberLevel.OAL_PURPLE:
                    amberLevel = "至尊紫珀";
                    break;
                default:
                    break;
            }

            return amberLevel;
        }
    }
}


